using System;
using System.Collections.Generic;
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
            MiFARECard card = new MiFARECard(cardReader);



            while (true)
            {
                Console.WriteLine("=====================================");
                for (int i = 0; i < 5; i++)
                    ReadData(card, i);
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
            string tmp = sec.Access.Trailer.AccessBitsRead.ToString();
            string tmp2 = sec.Access.Trailer.AccessBitsWrite.ToString();
            Console.WriteLine("DataBlock0:      " + hexString0);
            Console.WriteLine("DataBlock1:      " + hexString1);
            Console.WriteLine("DataBlock2:      " + hexString2);
            Console.WriteLine("TrailerBlock:    " + hexString3);
            Console.WriteLine("AccesConditionRead:  " + tmp);
            Console.WriteLine("AccesConditionWrite:  " + tmp2);


            Console.WriteLine();
        }




    }
}
