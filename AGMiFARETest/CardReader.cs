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
            throw new NotImplementedException();
        }

        public bool Read(int sector, int datablock, out byte[] data)
        {
            throw new NotImplementedException();
        }

        public bool Write(int sector, int datablock, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
