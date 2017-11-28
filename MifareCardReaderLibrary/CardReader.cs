﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ACR122U_Helper_Library;

namespace MifareCardReaderLibrary
{
    public class CardReader : ICardReader
    {
        int retCode;
        int hCard;
        int hContext;
        int Protocol;
        public bool connActive = false;
        public bool autoDet;
        public bool DebugMode { get; set; }
        //string sCard = "ACS ACR122 0";      // change depending on reader
        //private string sCard = "ACS ACR122U PICC Interface 0"; //"ACS ACR122 0" - do sprawdzenia w 
        private string sCard = CardReaderFinder.FindReaderName(false);
        public byte[] SendBuff = new byte[263];
        public byte[] RecvBuff = new byte[263];
        public byte[] RecvCode = new byte[2];
        public string RecvCodeStr = "";

        public int SendLen, RecvLen, nBytesRet, reqType, Aprotocol, dwProtocol, cbPciLength;
        public ModWinsCard.SCARD_READERSTATE RdrState;
        public ModWinsCard.SCARD_IO_REQUEST pioSendRequest;


        // Constructor

        public CardReader()
        {
            this.connectCard();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Worker_DoWork;
            //worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerAsync();
        }

        // Events and all related with it

        private bool HaltWorker = false;
        private bool WorkerBusy = false;

        private void WorkerStatusBusy(bool Busy)
        {
            if (!WorkerBusy && Busy)
            {
                WorkerBusy = Busy;
                Thread.Sleep(50);
            }
            else
            {
                WorkerBusy = Busy;
            }
        }

        private void HaltWorkerNow(bool Halt)
        {
            if (!HaltWorker && Halt)
            {
                HaltWorker = Halt;
                Thread.Sleep(50);
            }
            else
            {
                HaltWorker = Halt;
            }
        }

        public event EventHandler OnCardAppeared;

        protected virtual void RaiseEventOnCardAppeared()
        {
            if (OnCardAppeared != null)
                OnCardAppeared(this, EventArgs.Empty);
        }

        public event EventHandler OnCardDisappeared;

        protected virtual void RaiseEventOnCardDisappeared()
        {
            if (OnCardDisappeared != null)
                OnCardDisappeared(this, EventArgs.Empty);
        }

        BackgroundWorker worker = new BackgroundWorker();

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);

            bool cardExistOld = false;
            bool cardExistNow = false;
            while (true)
            {
                if (!HaltWorker)
                {
                    WorkerStatusBusy(true);
                    cardExistNow = connectCard();
                    if (cardExistOld != cardExistNow)
                    {
                        if (cardExistNow)
                        {
                            worker.ReportProgress(10); // Pojawiła się karta na czytniku
                        }
                        else
                        {
                            worker.ReportProgress(20); // Zabrano kartę z czytnika
                        }
                        cardExistOld = cardExistNow;
                    }
                    WorkerStatusBusy(false);
                }

                Thread.Sleep(200);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 10) RaiseEventOnCardAppeared();
            if (e.ProgressPercentage == 20) RaiseEventOnCardDisappeared();
        }

        public bool connectCard()
        {
            connActive = true;
            retCode = ModWinsCard.SCardEstablishContext(ModWinsCard.SCARD_SCOPE_USER, 0, 0, ref hContext);

            retCode = ModWinsCard.SCardConnect(hContext, sCard, ModWinsCard.SCARD_SHARE_SHARED,
                ModWinsCard.SCARD_PROTOCOL_T0 | ModWinsCard.SCARD_PROTOCOL_T1, ref hCard, ref Protocol);

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                if (DebugMode && !WorkerBusy)
                {
                    Console.Out.WriteLine("Please Insert Card");
                }
                connActive = false;
                return false;
            }
            else
            {
                if (DebugMode && !WorkerBusy)
                {
                    Console.Out.WriteLine("Card has been read");
                }
                return true;

            }

        }

        public void Close()
        {
            if (connActive)
            {
                retCode = ModWinsCard.SCardDisconnect(hCard, ModWinsCard.SCARD_UNPOWER_CARD);
            }

            retCode = ModWinsCard.SCardReleaseContext(hCard);
        }

        protected void ClearBuffers()
        {
            long indx;

            for (indx = 0; indx <= 262; indx++)
            {
                RecvBuff[indx] = 0;
                SendBuff[indx] = 0;
            }
            RecvCode[0] = 0;
            RecvCode[1] = 0;
        }

        protected bool SendDataToAPDU()
        {
            int indx;
            string tmpStr = "";

            pioSendRequest.dwProtocol = Aprotocol;
            pioSendRequest.cbPciLength = 8;

            if (DebugMode)
            {
                // Display Apdu In
                for (indx = 0; indx <= SendLen - 1; indx++)
                {
                    tmpStr = tmpStr + " " + string.Format("{0:X2}", SendBuff[indx]);

                }
                Console.WriteLine($"APDU IN: { tmpStr }");
            }

            //this.connectCard(); //Do przeniesienia do methody wykrywającej kartę na czytniku

            while (WorkerBusy)
            {
                Thread.Sleep(10);
            }

            HaltWorkerNow(true);

            //if (connActive)
            //{
            retCode = ModWinsCard.SCardTransmit(hCard, ref pioSendRequest, ref SendBuff[0],
                SendLen, ref pioSendRequest, ref RecvBuff[0], ref RecvLen);
            //}
            //else
            //{
            //    this.connectCard();
            //}

            HaltWorkerNow(false);

            RecvCode[0] = RecvBuff[RecvLen - 2];
            RecvCode[1] = RecvBuff[RecvLen - 1];

            if (DebugMode)
            {
                Console.WriteLine($"APDU OUT: {retCode}");
                tmpStr = "";
                for (indx = 0; indx <= RecvLen - 1; indx++)
                {
                    tmpStr = tmpStr + String.Format("{0:X2}", RecvBuff[indx]) + " ";
                }
                Console.WriteLine($"RecvBuff: { tmpStr }");
            }

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                return false;
            }
            else
            {
                tmpStr = "";
                for (indx = 0; indx <= 1; indx++)
                {
                    tmpStr = tmpStr + String.Format("{0:X2}", RecvCode[indx]);
                }
                RecvCodeStr = tmpStr;
                if (DebugMode)
                {
                    Console.WriteLine("RetCode:  {0}", tmpStr);
                }
                return true;
            }

        }

        public bool GetData_SerialNumber(out byte[] DataReceiveBuffor)
        {
            DataReceiveBuffor = new byte[4];
            ClearBuffers();
            SendBuff[0] = 0xFF;
            SendBuff[1] = 0xCA;
            SendBuff[2] = 0x00;
            SendBuff[3] = 0x00;
            SendBuff[4] = 0x00;

            SendLen = 5;
            RecvLen = 4 + 2;

            // retCode = SendAPDUandDisplay(2);

            if (SendDataToAPDU())
            {
                Array.Copy(RecvBuff, DataReceiveBuffor, DataReceiveBuffor.Length);
                return true;
            }
            else
            {

                return false;
            }
        }

        public bool GetData_ATS(out byte[] DataReceiveBuffor)
        {
            DataReceiveBuffor = new byte[4];
            ClearBuffers();
            SendBuff[0] = 0xFF;
            SendBuff[1] = 0xCA;
            SendBuff[2] = 0x01;
            SendBuff[3] = 0x00;
            SendBuff[4] = 0x00;

            SendLen = 5;
            RecvLen = 4 + 2;

            if (SendDataToAPDU())
            {
                Array.Copy(RecvBuff, DataReceiveBuffor, DataReceiveBuffor.Length);
                return true;
            }
            else
            {

                return false;
            }
        }

        private bool LoadAuthKeyToAPDU(string Key)
        {
            ClearBuffers();
            SendBuff[0] = 0xFF;                         // CLA
            SendBuff[1] = 0x82;                         // INS: for stored key input
            SendBuff[2] = 0x00;                         // P1: Key Structure
            SendBuff[3] = 0x00;                         // P2 : Key Number -  00h ~01h  
            SendBuff[4] = 0x06;                         // LC: 0x06h

            for (int i = 5; i < 11; i++)
            {
                SendBuff[i] = (byte)Convert.ToInt32(Key.Substring((i - 5) * 2, 2), 16);
            }

            SendLen = 0x0B;
            RecvLen = 0x02;

            if (SendDataToAPDU())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool authBlock(int BlockNumber, KeyTypeEnum KeyType) // function na pang authenticate dahil lumilipat tyo ng sector
        {
            ClearBuffers();
            SendBuff[0] = 0xFF;                         // CLA
            SendBuff[1] = 0x88;                         // INS: for stored key input
            SendBuff[2] = 0x00;                         // P1: same for all source types 
            SendBuff[3] = (byte)BlockNumber;           // P2 : Memory location;  P2: for stored key input
            switch (KeyType)                            // P3/LC: for stored key input
            {
                case KeyTypeEnum.KeyA:
                    SendBuff[4] = 0x60;
                    break;
                case KeyTypeEnum.KeyB:
                    SendBuff[4] = 0x61;
                    break;
                case KeyTypeEnum.KeyDefaultF:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(KeyType), KeyType, null);
            }
            SendBuff[5] = 0x00;                         // Byte 1: version number

            SendLen = 0x06;
            RecvLen = 0x02;

            if (SendDataToAPDU())
            {

                return true;
            }
            else
            {

                return false;
            }

        }

        public bool ReadBinaryBlock(int blockNumber, out byte[] DataReceiveBuffor)
        {
            DataReceiveBuffor = new byte[16];
            ClearBuffers();
            SendBuff[0] = 0xFF;
            SendBuff[1] = 0xB0;
            SendBuff[2] = 0x00;
            SendBuff[3] = (byte)blockNumber;
            SendBuff[4] = (byte)16;

            SendLen = 5;
            RecvLen = DataReceiveBuffor.Length + 2;

            string tmpStr = "";
            for (int indx = 0; indx <= RecvLen - 1; indx++)
            {
                tmpStr = tmpStr + String.Format("{0:X2}", RecvBuff[indx]);
            }


            if (SendDataToAPDU())
            {
                if (RecvCodeStr == "9000")
                {
                    Array.Copy(RecvBuff, DataReceiveBuffor, DataReceiveBuffor.Length);
                }
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool WriteBinaryBlock(int blockNumber, byte[] DataToWrite)
        {
            ClearBuffers();
            SendBuff[0] = 0xFF;
            SendBuff[1] = 0xD6;
            SendBuff[2] = 0x00;
            SendBuff[3] = (byte)blockNumber;
            SendBuff[4] = (byte)16;

            for (int i = 5; i < 16 + 5; i++)
            {
                SendBuff[i] = DataToWrite[i - 5];
            }

            SendLen = 16 + 5;
            RecvLen = 2;

            if (SendDataToAPDU())
            {

                return true;
            }
            else
            {
                return false;
            }

        }

        // Pseudo APDU commands

        public bool SetBuzzerOutputEnable(bool BuzzerON)
        {
            ClearBuffers();
            SendBuff[0] = 0xFF;                         // CLA
            SendBuff[1] = 0x00;                         // INS: for stored key input
            SendBuff[2] = 0x52;                         // P1: Key Structure
            if (BuzzerON)
            {
                SendBuff[3] = 0xFF;                         // P2 : Key Number -  00h ~01h  
            }
            else
            {
                SendBuff[3] = 0x00;                         // P2 : Key Number -  00h ~01h  
            }
            SendBuff[4] = 0x00;                         // P2 : Key Number -  00h ~01h  

            SendLen = 0x05;
            RecvLen = 0x02;

            if (SendDataToAPDU())
            {
                return true;
            }
            else
            {
                return false;
            }
        }





        // Requrided members from ICardReader interface

        public bool GetCardType(out CardTypeEnum cardType)
        {
            byte[] tmp;
            if (GetData_SerialNumber(out tmp))
            {
                cardType = CardTypeEnum.MiFARE_1K;
                return true;
            }
            else
            {
                cardType = CardTypeEnum.Unknown;
                return false;
            }
        }

        public bool Login(int sector, KeyTypeEnum key)
        {
            switch (key)
            {
                case KeyTypeEnum.KeyA:
                    if (!LoadAuthKeyToAPDU(Keys.A[sector]))
                    {
                        return false;
                    }
                    if (!authBlock(sector * 4, key))
                    {
                        return false;
                    }


                    break;
                case KeyTypeEnum.KeyB:
                    if (!LoadAuthKeyToAPDU(Keys.B[sector]))
                    {
                        return false;
                    }
                    if (!authBlock(sector * 4, key))
                    {
                        return false;
                    }

                    break;
                case KeyTypeEnum.KeyDefaultF:
                    if (!LoadAuthKeyToAPDU("FFFFFFFFFFFF"))
                    {
                        return false;
                    }
                    if (!authBlock(sector * 4, key))
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }

        public bool Read(int sector, int datablock, out byte[] data)
        {
            return ReadBinaryBlock(sector * 4 + datablock, out data);
        }

        public bool Write(int sector, int datablock, byte[] data)
        {
            return WriteBinaryBlock(sector * 4 + datablock, data);
        }
    }

    public enum VBOperationType
    {
        WriteVB_Value = 0x00,
        IncrementByVB_Value = 0x01,
        DecrementByVB_Value = 0x02
    }

    public class Keys
    {
        public static string[] A = new[]
        {
            "A0A1A2A3A4A5",
            "B6F0FC87F57F",
            "5888180ADBE6",
            "64EA317B7ABD",
            "898989890823",
            "898989891789",
            "898989893089",
            "B6E56BAD206A"
        };

        public static string[] B = new[]
        {
            "2481118E5355",
            "E4FDAC292BED",
            "D572C9491137",
            "A39A286285DB",
            "898989890823",
            "898989891789",
            "898989893089",
            "8FE6FA230C69"
        };

        //  "FFFFFFFFFFFF"
    }


}
