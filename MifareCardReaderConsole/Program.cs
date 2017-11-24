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
            cardReader.DebugMode = true;
            Console.WriteLine("Connecting to card:");
            cardReader.connectCard();
            Console.WriteLine("Get data serial number:");
            cardReader.GetData_SerialNumber(out ReceiveBuffor);
            Console.WriteLine("Main receive buffor :" + WriteDataFromBuff(ref ReceiveBuffor));
            Console.WriteLine("Get data ATS:");
            cardReader.GetData_ATS(out ReceiveBuffor);
            Console.WriteLine("Main receive buffor :" + WriteDataFromBuff(ref ReceiveBuffor));

            Console.WriteLine("Login for block 0");
            cardReader.Login(0, KeyTypeEnum.KeyA);
            Console.WriteLine("Main receive buffor :" + WriteDataFromBuff(ref ReceiveBuffor));







            Console.ReadKey();
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




    }
}
