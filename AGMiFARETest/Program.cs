using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace AG.MiFARE
{
    public class Program
    {
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

        public static void ReadData(MiFARECard card, int sector)
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


        public static void Main()
        {
            string strPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);

            if (strPath.IndexOf("file:\\") != -1)
                strPath = strPath.Remove(0, "file:\\".Length);

            FileReader cr = new FileReader(Path.Combine(strPath, "card_in.txt"));
            MiFARECard card = new MiFARECard(cr);

            //MiFARECard card2 = new MiFARECard()
            cr.GetCardType()


            // controlla data block 3, sector 0
            FormatSector0(card);

            // add application id into MAD
            int sector = ReserveSectorForApplication(card);

            ReadData(card, sector);

            WriteData(card, sector);

            card.Flush();
            cr.Flush(Path.Combine(strPath, "card_out.txt"));

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
