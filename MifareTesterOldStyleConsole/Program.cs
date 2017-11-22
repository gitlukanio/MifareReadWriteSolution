using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AG.MiFARE;

namespace MifareTesterOldStyleConsole
{
    class Program
    {
        static void Main(string[] args)
        {


            Console.ReadKey();

        }

        private static void Mya_OnCardAppeared(object sender, OnCardApearedEventArgs e)
        {
            Console.WriteLine("Pojawiła sie");
        }
    }
}
