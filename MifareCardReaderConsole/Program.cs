using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACR122U_Helper_Library;
using MifareCardReaderLibrary;

namespace MifareCardReaderConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] ReceiveBuffor;
            CardReader cardReader = new CardReader();
            CardValueReader cardValueReader = new CardValueReader();
            cardReader.DebugMode = true;
            cardValueReader.DebugMode = true;

            cardReader.OnCardAppeared += CardReader_OnCardAppeared;
            cardReader.OnCardDisappeared += CardReader_OnCardDisappeared;

            //Console.WriteLine("Connecting to card:");
            //cardReader.connectCard();
            cardReader.SetBuzzerOutputEnable(false);
            //Console.WriteLine("Get data serial number:");
            //cardReader.GetData_SerialNumber(out ReceiveBuffor);
            //Console.WriteLine("Main receive buffor :" + WriteDataFromBuff(ref ReceiveBuffor));
            //Console.WriteLine("Get data ATS:");
            //cardReader.GetData_ATS(out ReceiveBuffor);
            //Console.WriteLine("Main receive buffor :" + WriteDataFromBuff(ref ReceiveBuffor));

            //Console.WriteLine("Login for block 0");
            //cardReader.Login(0, KeyTypeEnum.KeyA);
            //Console.WriteLine("Main receive buffor :" + WriteDataFromBuff(ref ReceiveBuffor));
            //Console.WriteLine(" * * * Zaczynamy zabawe: * * * ");

            MiFARECard card = new MiFARECard(cardReader, cardValueReader);
            Sector sec;
            for (int i = 0; i < 7; i++)
            {
                ReadData(card, i);

                Console.ReadKey();


            }
            //Console.WriteLine(" * * * READ SECTOR * * *");
            //ReadData(card, 4);
            //Console.WriteLine(" * * * INCREMENT SECTOR * * * ");
            //card.IncValue(4, 0, 1);
            //Console.WriteLine(" * * * FLUSH SECTOR * * * ");
            //card.Flush();
            //Console.WriteLine(" * * * READ SECTOR AFTER FLUSH* * *");
            //card = new MiFARECard(cardReader, cardValueReader);
            //ReadData(card, 4);
            ////sec = card.GetSector(4);
            //Console.WriteLine(" * * * DECREMENT SECTOR * * * ");
            //card.DecValue(4, 0, 1);
            ////Console.WriteLine(" * * * FLUSH SECTOR * * * ");
            ////sec.Flush();
            //Console.WriteLine(" * * * READ SECTOR AFTER FLUSH* * *");
            //card = new MiFARECard(cardReader, cardValueReader);
            //ReadData(card, 4);
            ////sec = card.GetSector(4);
            //// card.IncValue(4, 0, 1);
            ////Console.ReadKey();
            ////card.Flush();
            //int value;
            //cardValueReader.IncValue(4, 0, 1, out value);
            //Console.WriteLine(" * * * * => Value: {0}", value);
            //Console.WriteLine("Direct Incement");
            //cardValueReader.ValueBlockOperation(12, VBOperationType.IncrementByVB_Value, value);
            //Console.WriteLine("Read sector");
            //card = new MiFARECard(cardReader, cardValueReader);
            //ReadData(card, 4);


            //card.GetData(4 * 4, 16);
            Sector sector = card.GetSector(4);

            // Value Block Operation tests
            int value = 0;
            int sectorNr = 4;
            //Sector sector = card.GetSector(4);
            //sector.Access.DataAreas[0].Decrement


            //ReadData(card, 4);
            //value = card.ReadValue(4, 0);
            //Console.WriteLine("Wartość: {0}", value);

            //Console.WriteLine(" * * * INCREMENT SECTOR * * * ");
            //card.IncValue(4, 0, 1);
            //card = new MiFARECard(cardReader, cardValueReader);
            //ReadData(card, 4);

            //Console.WriteLine(" * * * DECREMENT SECTOR * * * ");
            //card.DecValue(4, 0, 1);

            //card = new MiFARECard(cardReader, cardValueReader);
            //ReadData(card, 4);
            //value = card.ReadValue(4, 0);
            //Console.WriteLine("Wartość: {0}", value);
            //value = Int32.MaxValue - value;
            //Console.WriteLine("Wartość: {0}", value);


            Console.ReadKey();
        }

        private static void CardReader_OnCardDisappeared(object sender, EventArgs e)
        {
            Console.WriteLine("Karta znikła!!!");
        }

        private static void CardReader_OnCardAppeared(object sender, EventArgs e)
        {
            Console.WriteLine("Pojawiła się nowa karta");
        }

        public static string WriteDataFromBuff(ref byte[] buf)
        {
            string tmp = "";

            for (int i = 0; i < buf.Length; i++)
            {
                tmp = tmp + String.Format("{0:X2}" + " ", buf[i]);
            }
            return tmp;
        }

        public static void ReadData(MiFARECard card, int sector)
        {


            Console.WriteLine("Sector {0}: ", sector);
            Byte[] data0 = card.GetData(sector, 0, 16);
            Byte[] data1 = card.GetData(sector, 1, 16);
            Byte[] data2 = card.GetData(sector, 2, 16);
            Byte[] data3 = card.GetData(sector, 3, 16);

            Console.WriteLine("Successfully read {0} bytes", data0.Length);

            string hexString0 = "";
            string hexString1 = "";
            string hexString2 = "";
            string hexString3 = "";

            for (int i = 0; i < data0.Length; i++)
            {
                hexString0 += data0[i].ToString("X2") + " ";
                hexString1 += data1[i].ToString("X2") + " ";
                hexString2 += data2[i].ToString("X2") + " ";
                hexString3 += data3[i].ToString("X2") + " ";
            }

            //Sector sec = card.GetSector(sector);
            //string AcceessBitsRead = sec.Access.Trailer.AccessBitsRead.ToString();
            //string AccessBitsWrite = sec.Access.Trailer.AccessBitsWrite.ToString();
            //string KayARead = sec.Access.Trailer.KeyARead.ToString();
            //string KayAWrite = sec.Access.Trailer.KeyAWrite.ToString();
            //string KayBRead = sec.Access.Trailer.KeyBRead.ToString();
            //string KayBWrite = sec.Access.Trailer.KeyBWrite.ToString();

            Console.WriteLine("Sector: {0}", sector);
            Console.WriteLine("DataBlock0:     " + hexString0);
            Console.WriteLine("DataBlock1:     " + hexString1);
            Console.WriteLine("DataBlock2:     " + hexString2);
            Console.WriteLine("TrailerBlock:   " + hexString3);

            //Console.WriteLine("AccesBitsRead:  " + AcceessBitsRead);
            //Console.WriteLine("AccesBitsWrite: " + AccessBitsWrite);
            //Console.WriteLine("Key A Read      " + KayARead);
            //Console.WriteLine("Key A Write     " + KayAWrite);
            //Console.WriteLine("Key B Read      " + KayBRead);
            //Console.WriteLine("Key B Write     " + KayBWrite);
            //Console.WriteLine("=========================================");
            //Console.WriteLine("Key A: " + sec.KeyA);
            //Console.WriteLine("Key B: " + sec.KeyB);



            Console.WriteLine();
        }


    }
}
