using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACR122U_Helper_Library;
using ConsoleACR122U_2;


namespace ConsoleACR122U_3
{
    class Program
    {

        //static cardAccessor mya = new cardAccessor();
        // static ZTMManager ztm = new ZTMManager();
        static void Main(string[] args)
        {
            CardReader cardReader = new CardReader();




            while (true)
            {
                MiFARECard card = new MiFARECard(cardReader);
                Console.WriteLine("=====================================");
                ReadData(card, 0);
                //Console.WriteLine("=====================================");
                //ReadData(card, 7);
                Console.ReadKey();
                //card.Flush();
            }




            // Console.ReadKey();

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

            Sector sec = card.GetSector(sector);
            string AcceessBitsRead = sec.Access.Trailer.AccessBitsRead.ToString();
            string AccessBitsWrite = sec.Access.Trailer.AccessBitsWrite.ToString();
            string KayARead = sec.Access.Trailer.KeyARead.ToString();
            string KayAWrite = sec.Access.Trailer.KeyAWrite.ToString();
            string KayBRead = sec.Access.Trailer.KeyBRead.ToString();
            string KayBWrite = sec.Access.Trailer.KeyBWrite.ToString();

            Console.WriteLine("DataBlock0:     " + hexString0);
            Console.WriteLine("DataBlock1:     " + hexString1);
            Console.WriteLine("DataBlock2:     " + hexString2);
            Console.WriteLine("TrailerBlock:   " + hexString3);
            Console.WriteLine("AccesBitsRead:  " + AcceessBitsRead);
            Console.WriteLine("AccesBitsWrite: " + AccessBitsWrite);
            Console.WriteLine("Kay A Read      " + KayARead);
            Console.WriteLine("Kay A Write     " + KayAWrite);
            Console.WriteLine("Kay B Read      " + KayBRead);
            Console.WriteLine("Kay B Write     " + KayBWrite);
            Console.WriteLine("=========================================");
            Console.WriteLine("Key A: " + sec.KeyA);
            Console.WriteLine("Key B: " + sec.KeyB);



            GetAccessDataAreasConditions(sec, 0);
            GetAccessDataAreasConditions(sec, 1);
            GetAccessDataAreasConditions(sec, 2);


            Console.WriteLine();
        }

        private static void GetAccessDataAreasConditions(Sector sec, int BlockNumber)
        {
            //int sector;
            Console.WriteLine(" Dostęp do Data Block {0}", BlockNumber);
            string DataRead = sec.Access.DataAreas[BlockNumber].Read.ToString();
            string DataWrite = sec.Access.DataAreas[BlockNumber].Write.ToString();
            string DataIncrement = sec.Access.DataAreas[BlockNumber].Increment.ToString();
            string DataDecrement = sec.Access.DataAreas[BlockNumber].Decrement.ToString();

            Console.WriteLine("Data Read:      " + DataRead);
            Console.WriteLine("Data Write:     " + DataWrite);
            Console.WriteLine("Data Increment: " + DataIncrement);
            Console.WriteLine("Data Decrement: " + DataDecrement);
            Console.WriteLine("------------------------------------------");
        }

        public static void FormatSector0(MiFARECard card)
        {
            Console.WriteLine("Loading sector 0...");
            Sector sector0 = card.GetSector(0);
            Console.WriteLine("Sector 0 successfully loaded...");

            if (!((sector0.Access.Trailer.KeyAWrite == TrailerAccessCondition.ConditionEnum.KeyB) &&
                (sector0.Access.Trailer.KeyBWrite == TrailerAccessCondition.ConditionEnum.KeyB) &&
                (sector0.Access.Trailer.AccessBitsRead == TrailerAccessCondition.ConditionEnum.KeyAOrB) &&
                (sector0.Access.Trailer.AccessBitsWrite == TrailerAccessCondition.ConditionEnum.KeyB)))
            {
                // format 
                Console.WriteLine("Format required...");
                sector0.Access.DataAreas[0].Read = DataAreaAccessCondition.ConditionEnum.KeyAOrB;
                sector0.Access.DataAreas[0].Write = DataAreaAccessCondition.ConditionEnum.KeyB;
                sector0.Access.DataAreas[0].Increment = DataAreaAccessCondition.ConditionEnum.Never;
                sector0.Access.DataAreas[0].Decrement = DataAreaAccessCondition.ConditionEnum.Never;

                sector0.Access.DataAreas[1].Read = DataAreaAccessCondition.ConditionEnum.KeyAOrB;
                sector0.Access.DataAreas[1].Write = DataAreaAccessCondition.ConditionEnum.KeyB;
                sector0.Access.DataAreas[1].Increment = DataAreaAccessCondition.ConditionEnum.Never;
                sector0.Access.DataAreas[1].Decrement = DataAreaAccessCondition.ConditionEnum.Never;

                sector0.Access.DataAreas[2].Read = DataAreaAccessCondition.ConditionEnum.KeyAOrB;
                sector0.Access.DataAreas[2].Write = DataAreaAccessCondition.ConditionEnum.KeyB;
                sector0.Access.DataAreas[2].Increment = DataAreaAccessCondition.ConditionEnum.Never;
                sector0.Access.DataAreas[2].Decrement = DataAreaAccessCondition.ConditionEnum.Never;

                sector0.Access.Trailer.KeyARead = TrailerAccessCondition.ConditionEnum.Never;
                sector0.Access.Trailer.KeyAWrite = TrailerAccessCondition.ConditionEnum.KeyB;
                sector0.Access.Trailer.AccessBitsRead = TrailerAccessCondition.ConditionEnum.KeyAOrB;
                sector0.Access.Trailer.AccessBitsWrite = TrailerAccessCondition.ConditionEnum.KeyB;
                sector0.Access.Trailer.KeyBRead = TrailerAccessCondition.ConditionEnum.Never;
                sector0.Access.Trailer.KeyBWrite = TrailerAccessCondition.ConditionEnum.KeyB;
                sector0.KeyA = "A0A1A2A3A4A5";
                sector0.KeyB = "111111111111";
                sector0.Access.MADVersion = AccessConditions.MADVersionEnum.Version1;

                Console.WriteLine("Format completed...");

                Console.WriteLine("Flushing trailer sector...");
                sector0.FlushTrailer("A0A1A2A3A4A5", "111111111111");
                Console.WriteLine("Flush completed...");
            }
        }

        public static int ReserveSectorForApplication(MiFARECard card)
        {
            Console.WriteLine("Looking for application id 0x5210 in MAD..");
            int[] sectors = card.GetAppSectors(0x5210);
            if ((sectors != null) && (sectors.Length > 0))
            {
                Console.WriteLine("Application id found in sector {0}", sectors[0]);
                return sectors[0];
            }

            Console.WriteLine("Reserving a new sector...");
            int newSector = card.AddAppId(0x5210);
            Console.WriteLine("Sector {0} successfully reserved", newSector);

            return newSector;
        }

        public static void ReadData2(MiFARECard card, int sector)
        {
            Console.WriteLine("Reading 20 bytes of data from sector {0}...", sector);
            Byte[] data = card.GetData(sector, 1, 20);
            Console.WriteLine("Successfully read {0} bytes", data.Length);

            string hexString = "";
            for (int i = 0; i < data.Length; i++)
            {
                hexString += data[i].ToString("X2") + " ";
            }

            Console.WriteLine(hexString);
            Console.WriteLine();
        }

        public static void WriteData(MiFARECard card, int sector)
        {
            Byte[] data = new Byte[20];
            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)i;

            Console.WriteLine("Writing 20 bytes of data in sector {0}...", sector);
            card.SetData(sector, 1, data);
            Console.WriteLine("Write completed. Reading back data");

            ReadData(card, sector);
        }





    }
}
