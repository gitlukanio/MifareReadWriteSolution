using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleACR122U_2
{
    class Program
    {
        //static cardAccessor mya = new cardAccessor();
        static ZTMManager ztm = new ZTMManager();
        static void Main(string[] args)
        {
            try
            {
                ztm.DisableBuzzer();
                ztm.mya.OnCardAppeared += Mya_OnCardAppeared;
                ztm.mya.OnCardDisappeared += Mya_OnCardDisappeared;
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
                Console.ReadKey();
            }
        }

        private static void Mya_OnCardDisappeared(object sender, EventArgs e)

        {
            //ztm.DisableBuzzer();
            Console.WriteLine(" * * * * * * * * * * * * * * * * * *\n * * * Brak karty na czytniku  * * *\n * * * * * * * * * * * * * * * * * *\n\n\n\n\n\n\n");
        }

        private static void Mya_OnCardAppeared(object sender, OnCardApearedEventArgs e)
        {
            ztm.DisableBuzzer();
            ztm.ShowCardDetails();
        }

    }
}
