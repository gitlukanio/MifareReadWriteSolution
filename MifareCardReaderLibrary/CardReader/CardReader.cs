using System;
using System.ComponentModel;
using System.Threading;

namespace MifareCardReaderLibrary
{
    public class CardReader : ICardReader, ICardValueReader
    {
        private int retCode;
        private int hCard;
        private int hContext;
        private int Protocol;
        public bool connActive = false;
        //private bool autoDet;
        public bool DebugMode { get; set; }
        //string sCard = "ACS ACR122 0";      // change depending on reader
        //private string sCard = "ACS ACR122U PICC Interface 0"; //"ACS ACR122 0" - do sprawdzenia w 
        private string sCard = CardReaderFinder.FindReaderName(false);
        private byte[] SendBuff = new byte[263];
        private byte[] RecvBuff = new byte[263];
        private byte[] RecvCode = new byte[2];
        private string RecvCodeStr = "";

        private int SendLen, RecvLen, Aprotocol = 0; //, nBytesRet, reqType, dwProtocol, cbPciLength;
        //private ModWinsCard.SCARD_READERSTATE RdrState;
        private ModWinsCard.SCARD_IO_REQUEST pioSendRequest;


        // Constructor

        public CardReader()
        {
            this.connectCard();
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += Worker_DoWork;
            //worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            _worker.WorkerReportsProgress = true;
            _worker.ProgressChanged += Worker_ProgressChanged;
            _worker.RunWorkerAsync();
            Thread.Sleep(100);
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

        private readonly BackgroundWorker _worker = new BackgroundWorker();

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Thread.Sleep(1000);
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
                        //if (cardExistNow)
                        //    worker.ReportProgress(10); // Pojawiła się karta na czytniku
                        //else
                        //    worker.ReportProgress(20); // Zabrano kartę z czytnika
                        _worker.ReportProgress(cardExistNow ? 10 : 20); // To samo co wyżej tylko w jednej linijce :-)
                        cardExistOld = cardExistNow;
                    }
                    WorkerStatusBusy(false);
                }
                Thread.Sleep(200); // Sprawdzamy czy pojawiła się nowa karta co 200ms.
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 10) RaiseEventOnCardAppeared();
            if (e.ProgressPercentage == 20) RaiseEventOnCardDisappeared();
        }

        private bool connectCard()
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

        private void Close()
        {
            if (connActive)
            {
                retCode = ModWinsCard.SCardDisconnect(hCard, ModWinsCard.SCARD_UNPOWER_CARD);
            }

            retCode = ModWinsCard.SCardReleaseContext(hCard);
        }

        private void ClearBuffers()
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

        private bool SendDataToAPDU()
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

            while (WorkerBusy)
            {
                Thread.Sleep(10);
            }

            HaltWorkerNow(true);

            retCode = ModWinsCard.SCardTransmit(hCard, ref pioSendRequest, ref SendBuff[0],
                SendLen, ref pioSendRequest, ref RecvBuff[0], ref RecvLen);


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

        private bool ReadBinaryBlock(int blockNumber, out byte[] DataReceiveBuffor)
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

        private bool WriteBinaryBlock(int blockNumber, byte[] DataToWrite)
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

        //// Value Block Related Commands

        // Value Block Operation - Write, Increment, Decrement
        private bool ValueBlockOperation(int blockNumber, VBOperationType OperationType, int VB_Value)
        {

            byte[] tmpArray = new byte[4];
            tmpArray = BitConverter.GetBytes(VB_Value);
            Array.Reverse(tmpArray);

            ClearBuffers();
            SendBuff[0] = 0xFF;
            SendBuff[1] = 0xD7;
            SendBuff[2] = 0x00;
            SendBuff[3] = (byte)blockNumber;
            SendBuff[4] = 0x05;
            SendBuff[5] = (byte)OperationType;

            for (int i = 6; i < 10; i++)
            {
                SendBuff[i] = tmpArray[i - 6];
            }

            SendLen = 10;
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

        // Read Value Block
        private bool ReadValueBlock(int blockNumber, out int Value)
        {
            byte[] DataReceiveBuffor = new byte[4];
            Value = 0;
            ClearBuffers();
            SendBuff[0] = 0xFF;
            SendBuff[1] = 0xB1;
            SendBuff[2] = 0x00;
            SendBuff[3] = (byte)blockNumber;
            SendBuff[4] = 0x04;

            SendLen = 5;
            RecvLen = 4 + 2;

            if (SendDataToAPDU())
            {
                if (RecvCodeStr == "9000")
                {
                    Array.Copy(RecvBuff, DataReceiveBuffor, DataReceiveBuffor.Length);
                    Array.Reverse(DataReceiveBuffor);
                    Value = BitConverter.ToInt32(DataReceiveBuffor, 0);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        // Restore Value Block - Copy a value from a value block to another value block
        private bool RestoreValueBlock(int sourceBlockNumber, int targetBlockNumber)
        {
            // Source Block and Target Block must be in the same sector!!!!!!
            ClearBuffers();
            SendBuff[0] = 0xFF;
            SendBuff[1] = 0xD7;
            SendBuff[2] = 0x00;
            SendBuff[3] = (byte)sourceBlockNumber;
            SendBuff[4] = 0x02;
            SendBuff[5] = 0x03;
            SendBuff[6] = (byte)targetBlockNumber;

            SendLen = 7;
            RecvLen = 2;

            string tmpStr = "";
            for (int indx = 0; indx <= RecvLen - 1; indx++)
            {
                tmpStr = tmpStr + String.Format("{0:X2}", RecvBuff[indx]);
            }

            if (SendDataToAPDU())
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        //// Pseudo APDU commands

        // Set buzzer ON or OFF
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

        //// Not implemented
        public void ShowFirmwareVersion()
        {
            ClearBuffers();
            SendBuff[0] = 0xFF;                         // CLA
            SendBuff[1] = 0x00;                         // INS: for stored key input
            SendBuff[2] = 0x48;                         // P1: same for all source types 
            SendBuff[3] = 0x00;                         // P2 : Led state control
            SendBuff[4] = 0x00;                         // LC: 

            SendLen = 0x05;
            RecvLen = 0x02;

            //    retCode = SendAPDUandDisplay(2);
            Console.WriteLine($"FirmwareVersion retCode: { retCode }");
            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                //Console.Out.WriteLine("FAIL Authentication!");
                return;
            }

        }
        public void ShowLEDstatus()
        {
            ClearBuffers();
            SendBuff[0] = 0xFF;                         // CLA
            SendBuff[1] = 0x00;                         // INS: for stored key input
            SendBuff[2] = 0x40;                         // P1: same for all source types 
            SendBuff[3] = 0x00;                         // P2 : Led state control
            SendBuff[4] = 0x04;                         // LC: 
            SendBuff[5] = 0x00;                                            //========================================================================================
            SendBuff[6] = 0x00;
            SendBuff[7] = 0x00;
            SendBuff[8] = 0x00;

            SendLen = 0x09;
            RecvLen = 0x02;

            // retCode = SendAPDUandDisplay(2);
            //Console.WriteLine($"LED STATUS retCode: { retCode }" );

        }

        //// Requrided members from ICardReader interface

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

        //// Members required by ICardValueReader

        public bool ReadValue(int sector, int datablock, out int value)
        {
            return ReadValueBlock(sector * 4 + datablock, out value);
        }

        public bool WriteValue(int sector, int datablock, int value)
        {
            return ValueBlockOperation(sector * 4 + datablock, VBOperationType.WriteVB_Value, value);
        }

        public bool IncValue(int sector, int datablock, int delta, out int value)
        {
            if (ValueBlockOperation(sector * 4 + datablock, VBOperationType.IncrementByVB_Value, delta))
            {
                return ReadValue(sector, datablock, out value);
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public bool DecValue(int sector, int datablock, int delta, out int value)
        {
            if (Login(sector, KeyTypeEnum.KeyA) && ValueBlockOperation(sector * 4 + datablock, VBOperationType.DecrementByVB_Value, delta))
            {
                return ReadValue(sector, datablock, out value);
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public bool CopyValue(int sector, int srcBlock, int dstBlock, out int value)
        {
            if (RestoreValueBlock(sector * 4 + srcBlock, sector * 4 + dstBlock))
            {
                return ReadValue(sector, dstBlock, out value);
            }
            else
            {
                value = 0;
                return false;
            }
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
