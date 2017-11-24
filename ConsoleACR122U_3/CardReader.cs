using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACR122U_Helper_Library;
using ConsoleACR122U_2;

namespace ConsoleACR122U_3
{
    public class CardReader : ICardReader
    {
        static ZTMManager ztm = new ZTMManager();

        string[] KeyA = new[]
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
        string[] KeyB = new[]
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


        public CardReader()
        {
            Console.WriteLine("Disable Buzzer:");
            ztm.DisableBuzzer();
            //Console.WriteLine("Led status:");
            //ztm.mya.ShowLEDstatus();

        }


        public bool GetCardType(out CardTypeEnum cardType)
        {
            switch (ztm.mya.CardExist())
            {
                case true:
                    cardType = CardTypeEnum.MiFARE_1K;
                    return true;

                default:
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
            // bool resp = false;
            switch (key)
            {
                case KeyTypeEnum.KeyA:
                    ztm.mya.Login(sector * 4, 0, KeyA[sector]);
                    break;
                case KeyTypeEnum.KeyB:
                    ztm.mya.Login(sector * 4, 1, KeyB[sector]);
                    break;
                case KeyTypeEnum.KeyDefaultF:
                    ztm.mya.Login(sector * 4, 0, "FFFFFFFFFFFF");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
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
            string tmp = ztm.mya.GetStringFromCard(sector * 4 + datablock);
            data = new byte[20];
            for (int i = 0; i < tmp.Length / 2; i++)
            {
                data[i] = (byte)Convert.ToInt32(tmp.Substring(i * 2, 2), 16);
            }

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
            throw new NotImplementedException();
        }
    }
}
