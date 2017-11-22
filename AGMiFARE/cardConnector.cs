using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AG.MiFARE
{
    public class cardConnector
    {

        //bool pinSubmission = true;
        //bool amountSubmission = true;
        //bool loyaltyClaiming = true;

        int retCode;
        int hCard;
        int hContext;
        int Protocol;
        public bool connActive = false;
        public bool autoDet;
        //string sCard = "ACS ACR122 0";      // change depending on reader
        string sCard = "ACS ACR122U PICC Interface 0";
        public byte[] SendBuff = new byte[263];
        public byte[] RecvBuff = new byte[263];
        public int SendLen, RecvLen, nBytesRet, reqType, Aprotocol, dwProtocol, cbPciLength;
        public ModWinsCard.SCARD_READERSTATE RdrState;
        public ModWinsCard.SCARD_IO_REQUEST pioSendRequest;

        public string[] KeysBuffA = new string[320];
        public string[] KeysBuffB = new string[320];

        void LoadKeyBuff()
        {
            KeysBuffA[0] = "A0A1A2A3A4A5";
            KeysBuffA[1] = "B6F0FC87F57F";
            KeysBuffA[2] = "5888180ADBE6";
            KeysBuffA[3] = "64EA317B7ABD";
            KeysBuffA[4] = "898989890823";
            KeysBuffA[5] = "898989891789";
            KeysBuffA[6] = "898989893089";
            KeysBuffA[7] = "B6E56BAD206A";

            #region Klucze
            //a0 a1 a2 a3 a4 a5
            //b6 f0 fc 87 f5 7f
            //58 88 18 0a db e6
            //64 ea 31 7b 7a bd
            //89 89 89 89 08 23
            //89 89 89 89 17 89
            //89 89 89 89 30 89
            //b6 e5 6b ad 20 6a
            //4d 10 95 f1 af 34
            //89 10 89 89 89 89
            //89 63 89 89 89 89
            //89 01 63 89 89 89


            KeysBuffB[0] = "2481118E5355";
            KeysBuffB[1] = "E4FDAC292BED";
            KeysBuffB[2] = "D572C9491137";
            KeysBuffB[3] = "A39A286285DB";
            KeysBuffB[4] = "898989890823";
            KeysBuffB[5] = "898989891789";
            KeysBuffB[6] = "898989893089";
            KeysBuffB[7] = "8FE6FA230C69";
            //B
            //24 81 11 8e 53 55
            //e4 fd ac 29 2b ed
            //d5 72 c9 49 11 37
            //a3 9a 28 62 85 db
            //89 89 89 89 08 23
            //89 89 89 89 17 89
            //89 89 89 89 30 89
            //8f e6 fa 23 0c 69
            //1a d2 f9 9b b9 e9
            //89 10 89 89 89 89
            //89 63 89 89 89 89
            //89 01 63 89 89 89 
            #endregion






        }

        private void LoadAuthkeysToAPDU(string s)
        {
            //byte[] Key = KeysBuffA[0]
            LoadKeyBuff();
            ClearBuffers();
            SendBuff[0] = 0xFF;                         // CLA
            SendBuff[1] = 0x82;                         // INS: for stored key input
            SendBuff[2] = 0x00;                         // P1: Key Structure
            SendBuff[3] = 0x00;                         // P2 : Key Number -  00h ~01h  
            SendBuff[4] = 0x06;                         // LC: 0x06h

            //SendBuff[5] = (byte)Convert.ToInt32( KeysBuffA[Convert.ToInt32(s) / 4].Substring(0, 2), 16 );

            for (int i = 5; i < 11; i++)
            {
                SendBuff[i] = (byte)Convert.ToInt32(KeysBuffA[Convert.ToInt32(s) / 4].Substring((i - 5) * 2, 2), 16);
            }

            SendLen = 0x0B;
            RecvLen = 0x02;

            retCode = SendAPDUandDisplay(0);
        }

        public void DisableBuzz()
        {
            ShowLEDstatus();
            ClearBuffers();
            SendBuff[0] = 0xFF;                         // CLA
            SendBuff[1] = 0x00;                         // INS: for stored key input
            SendBuff[2] = 0x52;                         // P1: same for all source types 
            SendBuff[3] = 0x00;                         // P2 : Led state control
            SendBuff[4] = 0x00;                         // LC: 

            SendLen = 0x05;
            RecvLen = 0x02;

            retCode = SendAPDUandDisplay(2);
            //Console.WriteLine($"DisableBuzz retCode: { retCode }");
            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                //Console.Out.WriteLine("FAIL Authentication!");
                return;
            }

        }

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

            retCode = SendAPDUandDisplay(2);
            Console.WriteLine($"FirmwareVersion retCode: { retCode }");
            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                //Console.Out.WriteLine("FAIL Authentication!");
                return;
            }

        }

        private void ShowUID()
        {
            ClearBuffers();
            SendBuff[0] = 0xFF;                         // CLA
            SendBuff[1] = 0xCA;                         // INS: for stored key input
            SendBuff[2] = 0x01;                         // P1: same for all source types 
            SendBuff[3] = 0x00;                         // P2 : Led state control
            SendBuff[4] = 0x00;                         // LC: 

            SendLen = 0x05;
            RecvLen = 0x0A;

            retCode = SendAPDUandDisplay(2);
            //Console.WriteLine($"ShowUID retCode: { retCode }");
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

            retCode = SendAPDUandDisplay(2);
            //Console.WriteLine($"LED STATUS retCode: { retCode }" );

        }

        private void authBlock(String s) // function na pang authenticate dahil lumilipat tyo ng sector
        {

            LoadAuthkeysToAPDU(s);


            ClearBuffers();
            SendBuff[0] = 0xFF;                         // CLA
            SendBuff[1] = 0x88;                         // INS: for stored key input
            SendBuff[2] = 0x00;                         // P1: same for all source types 
            SendBuff[3] = (byte)int.Parse(s);           // P2 : Memory location;  P2: for stored key input
            SendBuff[4] = 0x60;                         // P3/LC: for stored key input
            SendBuff[5] = 0x00;                         // Byte 1: version number


            SendLen = 0x06;
            RecvLen = 0x02;

            retCode = SendAPDUandDisplay(0);

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                //Console.Out.WriteLine("FAIL Authentication!");
                return;
            }
        }

        public bool CardExist()
        {
            ClearBuffers();
            SendBuff[0] = 0xFF;                         // CLA
            SendBuff[1] = 0x88;                         // INS: for stored key input
            SendBuff[2] = 0x00;                         // P1: same for all source types 
            SendBuff[3] = (byte)int.Parse("0");           // P2 : Memory location;  P2: for stored key input
            SendBuff[4] = 0x60;                         // P3/LC: for stored key input
            SendBuff[5] = 0x00;                         // Byte 1: version number


            SendLen = 0x06;
            RecvLen = 0x02;

            retCode = SendAPDUandDisplay(0);

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                //Console.Out.WriteLine("FAIL Authentication!");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ClearBuffers()
        {
            long indx;

            for (indx = 0; indx <= 262; indx++)
            {
                RecvBuff[indx] = 0;
                SendBuff[indx] = 0;
            }
        }

        private long getIntValues(String s)
        {
            long lVal = 0;

            ClearBuffers();
            SendBuff[0] = 0xFF;                           // CLA     
            SendBuff[1] = 0xB1;                           // INS
            SendBuff[2] = 0x00;                           // P1
            SendBuff[3] = (byte)int.Parse(s);             // P2 : Block No.
            SendBuff[4] = 0x00;                           // Le

            SendLen = 0x05;
            RecvLen = 0x06;

            retCode = SendAPDUandDisplay(2);

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                Console.Out.WriteLine("FAIL!");
            }

            lVal = RecvBuff[3];
            lVal = lVal + (RecvBuff[2] * 256);
            lVal = lVal + (RecvBuff[1] * 256 * 256);
            lVal = lVal + (RecvBuff[0] * 256 * 256 * 256);

            return (lVal);
        }

        private int SendAPDUandDisplay(int reqType)
        {
            int indx;
            string tmpStr = "";

            pioSendRequest.dwProtocol = Aprotocol;
            pioSendRequest.cbPciLength = 8;

            // Display Apdu In
            for (indx = 0; indx <= SendLen - 1; indx++)
            {
                tmpStr = tmpStr + " " + string.Format("{0:X2}", SendBuff[indx]);

            }
            //Console.WriteLine($"APDU IN: { tmpStr }");

            retCode = ModWinsCard.SCardTransmit(hCard, ref pioSendRequest, ref SendBuff[0],
                                 SendLen, ref pioSendRequest, ref RecvBuff[0], ref RecvLen);

            //Console.WriteLine($"APDU OUT: {retCode}");

            //tmpStr = "";
            //for (indx = 0; indx <= RecvLen - 1; indx++)
            //{
            //    //tmpStr = tmpStr + Convert.ToChar(RecvBuff[indx]);
            //    //ToString
            //    tmpStr = tmpStr + String.Format("{0:X2}", RecvBuff[indx]);

            //}

            //Console.WriteLine($"RecvBuff: { tmpStr }");





            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {

                return retCode;
            }

            else
            {
                tmpStr = "";
                switch (reqType)
                {
                    case 0:
                        for (indx = (RecvLen - 2); indx <= (RecvLen - 1); indx++)
                        {
                            tmpStr = tmpStr + " " + string.Format("{0:X2}", RecvBuff[indx]);
                        }

                        if ((tmpStr).Trim() != "90 00")
                        {
                            Console.WriteLine((tmpStr).Trim());
                            Console.Out.WriteLine("Return bytes are not acceptable.");
                        }

                        break;

                    case 1:

                        for (indx = (RecvLen - 2); indx <= (RecvLen - 1); indx++)
                        {
                            tmpStr = tmpStr + string.Format("{0:X2}", RecvBuff[indx]);
                        }

                        if (tmpStr.Trim() != "90 00")
                        {
                            tmpStr = tmpStr + " " + string.Format("{0:X2}", RecvBuff[indx]);
                            Console.WriteLine("ABC:" + (tmpStr).Trim());
                        }

                        else
                        {
                            tmpStr = "ATR : ";
                            for (indx = 0; indx <= (RecvLen - 3); indx++)
                            {
                                tmpStr = tmpStr + " " + string.Format("{0:X2}", RecvBuff[indx]);
                                Console.WriteLine("BCD" + (tmpStr).Trim());
                            }
                        }

                        break;

                    case 2:

                        for (indx = 0; indx <= (RecvLen - 1); indx++)
                        {
                            tmpStr = tmpStr + " " + string.Format("{0:X2}", RecvBuff[indx]);
                        }

                        break;
                }
            }
            return retCode;
        }

        public void connectCard()
        {
            connActive = true;
            retCode = ModWinsCard.SCardEstablishContext(ModWinsCard.SCARD_SCOPE_USER, 0, 0, ref hContext);

            retCode = ModWinsCard.SCardConnect(hContext, sCard, ModWinsCard.SCARD_SHARE_SHARED,
                      ModWinsCard.SCARD_PROTOCOL_T0 | ModWinsCard.SCARD_PROTOCOL_T1, ref hCard, ref Protocol);

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                //Console.Out.WriteLine("Please Insert Card");
                //Program.MessageWinsCard = "Please Insert Card";
                connActive = false;
            }

            if (retCode == ModWinsCard.SCARD_S_SUCCESS)
            {
                //Console.Out.WriteLine("Card has been read");
                //Program.MessageWinsCard = "Card has been read";
            }
        }

        public void submitText(String Text, String Block)
        {

            String tmpStr = Text;
            int indx;
            authBlock(Block);
            ClearBuffers();
            SendBuff[0] = 0xFF;                             // CLA
            SendBuff[1] = 0xD6;                             // INS
            SendBuff[2] = 0x00;                             // P1
            SendBuff[3] = (byte)int.Parse(Block);           // P2 : Starting Block No.
            SendBuff[4] = (byte)int.Parse("16");            // P3 : Data length

            for (indx = 0; indx <= (tmpStr).Length - 1; indx++)
            {
                SendBuff[indx + 5] = (byte)tmpStr[indx];
            }
            SendLen = SendBuff[4] + 5;
            RecvLen = 0x02;

            retCode = SendAPDUandDisplay(2);

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                Console.Out.WriteLine("fail write");
            }
            else
                Console.Out.WriteLine("write success");
        }

        public string readBlock(String Block)
        {
            string tmpStr = "";
            int indx;
            authBlock(Block);
            ClearBuffers();
            SendBuff[0] = 0xFF;
            SendBuff[1] = 0xB0;
            SendBuff[2] = 0x00;
            SendBuff[3] = (byte)int.Parse(Block);
            SendBuff[4] = (byte)int.Parse("16");

            SendLen = 5;
            RecvLen = SendBuff[4] + 2;
            //RecvLen = SendBuff[4];


            retCode = SendAPDUandDisplay(2);

            if (retCode != ModWinsCard.SCARD_S_SUCCESS)
            {
                //Console.Out.WriteLine("fail read");
            }

            // Display data in text format
            for (indx = 0; indx <= RecvLen - 1; indx++)
            {
                //tmpStr = tmpStr + Convert.ToChar(RecvBuff[indx]);
                //ToString
                tmpStr = tmpStr + String.Format("{0:X2}", RecvBuff[indx]);

            }

            return (tmpStr);
        }

        public void Close()
        {
            if (connActive)
            {
                retCode = ModWinsCard.SCardDisconnect(hCard, ModWinsCard.SCARD_UNPOWER_CARD);
            }

            retCode = ModWinsCard.SCardReleaseContext(hCard);
        }

        public void clearBlock(string startBlock, string endBlock)
        {
            int start = int.Parse(startBlock);
            int end = int.Parse(endBlock);
            string spaces = "                ";

            for (int i = start; i <= end; i++)
            {
                if ((i + 1) % 4 != 0)
                {
                    this.submitText(spaces, i.ToString());
                }
            }
        }






    }
}
