using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AG.MiFARE;
//using ConsoleACR122U_2;

namespace AGMiFARETest
{
    public class CardReader : cardConnector, ICardReader
    {
        //Byte[] _Data;



        public bool GetCardType(out CardTypeEnum cardType)
        {
            if (CardExist())
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

            //getStringFromCard()





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
            throw new NotImplementedException();
        }
    }
}
