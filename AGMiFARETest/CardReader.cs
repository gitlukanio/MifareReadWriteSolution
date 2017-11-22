using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AG.MiFARE;
//using ConsoleACR122U_2;

namespace AGMiFARETest
{
    public class CardReader : ICardReader
    {

        cardAccessor cardAcc = new cardAccessor();

        //Byte[] _Data;
        //A0A1A2A3A4A5 - 2481118E5355
        //B6F0FC87F57F - E4FDAC292BED
        //5888180ADBE6 - D572C9491137
        //64EA317B7ABD - A39A286285DB
        //898989890823 - 898989890823

        string[] KeysA = new string[5]
        {
            "A0A1A2A3A4A5",
            "B6F0FC87F57F",
            "5888180ADBE6",
            "64EA317B7ABD",
            "898989890823"
        };

        string[] KeysB = new string[5]
        {
            "2481118E5355",
            "E4FDAC292BED",
            "D572C9491137",
            "A39A286285DB",
            "898989890823"
        };

        //cardConnector myc = new cardConnector();

        public bool GetCardType(out CardTypeEnum cardType)
        {
            if (cardAcc.CardExist())
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

        /// <summary>
        /// Login into the given sector using the given key
        /// </summary>
        /// <param name="sector">sector to login into</param>
        /// <param name="key">key to use</param>
        /// <returns>tru on success, false otherwise</returns>
        public bool Login(int sector, KeyTypeEnum key)
        {
            Console.WriteLine("CardReader: Login({0}, {1}) invoked", sector, key);

            cardAcc.GetNewCard();

            Console.WriteLine("Czy karta istnieje:{0}", cardAcc.CardExist());




            switch (key)
            {


                case KeyTypeEnum.KeyA:
                    cardAcc.LoadKeys(KeysA[sector], 0);
                    //LoadAuthkeysToAPDU("0");
                    cardAcc.AuthBlock(sector * 4, 0);
                    //authBlock("0");


                    //authBlock(sector * 4, 0);
                    //return false;
                    break;
                case KeyTypeEnum.KeyB:
                    cardAcc.LoadKeys(KeysB[sector], 1);
                    cardAcc.AuthBlock(sector * 4, 1);

                    break;
                case KeyTypeEnum.KeyDefaultF:
                    cardAcc.LoadKeys("FFFFFFFFFFFF", 0);
                    cardAcc.AuthBlock(sector * 4, 0);

                    break;
            }


            return true;
        }

        /// <summary>
        /// read a datablock from a sector 
        /// </summary>
        /// <param name="sector">sector to read</param>
        /// <param name="datablock">datablock to read</param>
        /// <param name="data">data read out from the datablock</param>
        /// <returns>true on success, false otherwise</returns>
        public bool Read(int sector, int datablock, out byte[] data)
        {

            // data = readBlock(sector * 4 + datablock);
            data = null;
            //Console.WriteLine("Read returned code:{1}", BlockNumber, retCode);


            return true;
        }

        /// <summary>
        /// write data in a datablock
        /// </summary>
        /// <param name="sector">sector to write</param>
        /// <param name="datablock">datablock to write</param>
        /// <param name="data">data to write. this is a 16-bytes array</param>
        /// <returns>true on success, false otherwise</returns>
        public bool Write(int sector, int datablock, byte[] data)
        {
            //WriteData(data, sector * 4 + datablock);

            return true;

        }
    }
}
