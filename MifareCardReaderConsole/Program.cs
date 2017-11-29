using System;
using System.Collections.Generic;
using System.Linq;
using MifareCardReaderLibrary;

namespace MifareCardReaderConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //byte[] ReceiveBuffor;//////////
            CardReader cardReader = new CardReader();
            MiFARECard card = new MiFARECard(cardReader, cardReader);
            //cardReader.DebugMode = true;
            cardReader.OnCardAppeared += CardReader_OnCardAppeared;
            cardReader.OnCardDisappeared += CardReader_OnCardDisappeared;
            try
            {
                int indx = 0;
                cardReader.SetBuzzerOutputEnable(false);
                while (true)
                {
                    //ReadData(card, i);
                    //{
                    //int LiczbaZcarty = card.ReadValue(4, 0);
                    //Console.WriteLine("Uzyskana liczba z bloku 0: {0}", LiczbaZcarty);
                    //int LiczbaZcarty2 = card.ReadValue(4, 1);
                    //Console.WriteLine("Uzyskana liczba z bloku 0: {0}", LiczbaZcarty2);
                    ShowAccessConditions(card, indx);
                    indx++;
                    // }
                    Console.ReadKey();
                }
                //Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("Wystąpił błąd: {0}", e.Message);
            }

            //for (int i = 0; i < 5; i++)
            //{
            //    int LiczbaZcarty = card.ReadValue(4, 0);
            //    Console.WriteLine("Uzyskana liczba z karty: {0}", LiczbaZcarty);
            //    Console.ReadKey();
            //}

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

        public static void ShowAccessConditions(MiFARECard card, int sectorNumber)
        {
            Sector sec = card.GetSector(sectorNumber);
            string AcceessBitsRead = sec.Access.Trailer.AccessBitsRead.ToString();
            string AccessBitsWrite = sec.Access.Trailer.AccessBitsWrite.ToString();
            string KayARead = sec.Access.Trailer.KeyARead.ToString();
            string KayAWrite = sec.Access.Trailer.KeyAWrite.ToString();
            string KayBRead = sec.Access.Trailer.KeyBRead.ToString();
            string KayBWrite = sec.Access.Trailer.KeyBWrite.ToString();

            List<string> DataReadCondition = new List<string>();
            List<string> DataWriteCondition = new List<string>();
            List<string> DataIncrementCondition = new List<string>();
            List<string> DataDecrementCondition = new List<string>();


            for (int i = 0; i < 3; i++)
            {
                DataReadCondition.Add(sec.Access.DataAreas[i].Read.ToString());
                DataWriteCondition.Add(sec.Access.DataAreas[i].Write.ToString());
                DataIncrementCondition.Add(sec.Access.DataAreas[i].Increment.ToString());
                DataDecrementCondition.Add(sec.Access.DataAreas[i].Decrement.ToString());
            }
            string border = "|-------------------------------------------------|";
            Console.WriteLine(" Data sector {0} conditions ", sectorNumber);
            Console.WriteLine(border);
            Console.WriteLine("| BlockNR |   READ  |  WRITE  |  INCRE  |  DECRE  |");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("| {4,7} | {0,7} | {1,7} | {2,7} | {3,7} |", DataReadCondition.ElementAt(0), DataWriteCondition.ElementAt(i), DataIncrementCondition.ElementAt(i), DataDecrementCondition.ElementAt(i), i);
            }

            Console.WriteLine(border);
            Console.WriteLine(" Trailer {0} conditions ", sectorNumber);
            border = "|-----------------------------------------------------------|";
            Console.WriteLine(border);
            Console.WriteLine("|      Key A        |   Access Bits     |      Key B        |");
            Console.WriteLine("|   READ  |  WRITE  |   READ  |  WRITE  |   READ  |  WRITE  |");
            Console.WriteLine("| {0,7} | {1,7} | {2,7} | {3,7} | {4,7} | {5,7} |",
                 KayARead, KayAWrite, AcceessBitsRead, AccessBitsWrite, KayBRead, KayBWrite);
            Console.WriteLine(border);
            Console.WriteLine("\n=============================================================\n");
        }




    }
}
