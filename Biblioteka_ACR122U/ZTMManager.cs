using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleACR122U_2 // namespace AG.MiFARE
{
    public class ZTMManager
    {
        public ZTMManager()
        {
            AssigneDefoulteKeys();
        }

        Klucze KluczeZTM = new Klucze();

        public cardAccessor mya = new cardAccessor();
        
        private void AssigneDefoulteKeys()
        {
            string[] KeysBuff = new string[320];
                        
            #region Klucze
                // Klucze B
                KeysBuff[0] = "a0a1a2a3a4a5";
                KeysBuff[1] = "b6f0fc87f57f";
                KeysBuff[2] = "5888180adbe6";
                KeysBuff[3] = "64ea317b7abd";
                KeysBuff[4] = "898989890823";
                KeysBuff[5] = "898989891789";
                KeysBuff[6] = "898989893089";
                KeysBuff[7] = "b6e56bad206a";
                KeysBuff[8] = "4d1095f1af34";
                KeysBuff[9] = "891089898989";
                KeysBuff[10] = "896389898989";
                KeysBuff[11] = "890163898989";
                // Klucze B
                KeysBuff[100] = "2481118e5355";
                KeysBuff[101] = "e4fdac292bed";
                KeysBuff[102] = "d572c9491137";
                KeysBuff[103] = "a39a286285db";
                KeysBuff[104] = "898989890823";
                KeysBuff[105] = "898989891789";
                KeysBuff[106] = "898989893089";
                KeysBuff[107] = "8fe6fa230c69";
                KeysBuff[108] = "1ad2f99bb9e9";
                KeysBuff[109] = "891089898989";
                KeysBuff[110] = "896389898989";
                KeysBuff[111] = "890163898989";
            #endregion

            // Dodajemy Klucze A
            for (int i = 0; i < 12; i++)
            {
                KluczeZTM.KluczeA.Add(KeysBuff[i]);
            }
            // Dodajemy Klucze B
            for (int i = 100; i < 112; i++)
            {
                KluczeZTM.KluczeB.Add(KeysBuff[i]);
            }
        }

        public void PrintAllKeys()
        {
            string x = "0";
            Console.WriteLine("Pary kluczy dla sektorów:\n");
            for (int i = 0; i < KluczeZTM.KluczeA.Count; i++)
            {
                if (i > 9) x = ""; 
                Console.WriteLine($"Sektor {x}{i} Klucz A: {KluczeZTM.KluczeA[i]} Klucz B: {KluczeZTM.KluczeB[i]}");
            }
            
        }

        public void ShowCardDetails()
        {
            Console.WriteLine($" * * * * * * * * * * * * * * * * * * *\n * * * Nowa karta na czytniku!!  * * *\n * * * * * * * * * * * * * * * * * * *\n { mya.getID() } ");
            Console.WriteLine("=======================================");

            ZTMSectorSkasowania TestSektor = new ZTMSectorSkasowania(mya.getStringFromCard(8));
            Console.WriteLine(" Liczba skasowań: " + TestSektor.LiczbaSkasowan.ToString());
            Console.WriteLine($" Bilet ważny do: { TestSektor.RokWaznosci }/{ TestSektor.MiesiacWaznosci }/{ TestSektor.DzienWaznosci } { TestSektor.GodzinaWaznosci }:{ TestSektor.MinutaWaznosci }");
            Console.WriteLine($" Skasowano: { TestSektor.RokSkasowania }/{ TestSektor.MiesiacSkasowania }/{ TestSektor.DzienSkasowania } { TestSektor.GodzinaSkasowania }:{ TestSektor.MinutaSkasowania }");
            Console.WriteLine($" Linia: { TestSektor.Linia }, Brygada: { TestSektor.Brygada }");

            Console.WriteLine("=======================================");

            ZTMSectorSkasowania ss = new ZTMSectorSkasowania(mya.getStringFromCard(12));
            Console.WriteLine(" Liczba skasowań: " + ss.LiczbaSkasowan.ToString());
            Console.WriteLine($" Bilet ważny do: { ss.RokWaznosci }/{ ss.MiesiacWaznosci }/{ ss.DzienWaznosci } { ss.GodzinaWaznosci }:{ ss.MinutaWaznosci }");
            Console.WriteLine($" Skasowano: { ss.RokSkasowania }/{ ss.MiesiacSkasowania }/{ ss.DzienSkasowania } { ss.GodzinaSkasowania }:{ ss.MinutaSkasowania }");
            Console.WriteLine($" Linia: { ss.Linia }, Brygada: { ss.Brygada }");

        }

        public void DisableBuzzer()
        {
            mya.DisableBuzzer();
        }









    }

    public class Klucze
    {
        public List<string> KluczeA = new List<string>();
        public List<string> KluczeB = new List<string>();
    }

    public class ZTMSectorID
    {
        public ZTMSectorID(string sektor0)
        {
            this.StringID = sektor0.Substring(0, 8);
        }

        private string id;
        public string StringID
        {
            get { return id; }
            set
            {
                id = value.Substring(0, 8);
                intID = Convert.ToInt32(id, 16);
            }
        }

        private Int32 intID;
        public Int32 IntID
        {
            get { return intID; }
        }

        //    id1 = Convert.ToInt32(tmp.Substring(6, 2), 16);
        //    id2 = Convert.ToInt32(tmp.Substring(4, 2) + tmp.Substring(2, 2) + tmp.Substring(0, 2), 16);
        //    if (id2< 10000000) id3 = "0";
        //    if (id1< 100) id4 = "0";
        //    id = $"Karta nr: { id4 }{ id1 } { id3 }{ id2 }";
    }

    public class ZTMSectorTypu
    {
        public ZTMSectorTypu(string sektor1_4)
        {
            typ = Convert.ToInt32(sektor1_4.Substring(2, 2), 16);
        }
        
        private Int32 typ;
        public string TypBiletu
        {
            get { return string.Format("{0:X2}", typ); }
            //set { typ = Convert.ToInt32(value, 16); }
        }


        #region TypyBiletow

            //00	 czysty bilet					
            //02	 bilet jednorazowy strefa 1, normalny					
            //16	 bilet dobowy strefa 1, normalny					
            //18	 bilet trzydniowy strefa 1, normalny					
            //21	 bilet dobowy strefa 1, normalny(nowa)
            //22	 bilet trzydniowy strefa 1, normalny					
            //23	 bilet dobowy strefa 1, normalny					
            //24	 bilet trzydniowy strefa 1, normalny					
            //25	 bilet siedmiodniowy strefa 1, normalny					
            //27	 bilet 14-dniowy strefa 1, normalny					
            //29	 bilet 30-dniowy imienny na 1 linie strefa 1, normalny					
            //2a     bilet 30-dniowy imienny na wszystkie linie strefa 1, normalny					
            //2b     bilet 30-dniowy imienny na wszystkie linie strefa 1, normalny					
            //2e	 bilet 90-dniowy imienny na wszystkie linie strefa 1, normalny					
            //2f	 bilet 90-dniowy imienny na wszystkie linie strefa 1, normalny					
            //34	 bilet jednorazowy strefa 1, ulgowy 50					
            //48	 bilet dobowy strefa 1, ulgowy 50					
            //4a     bilet trzydniowy strefa 1, ulgowy 50					
            //53	 bilet dobowy strefa 1, ulgowy 50					
            //54	 bilet trzydniowy strefa 1, ulgowy 50					
            //55	 bilet dobowy strefa 1, ulgowy 50					
            //56	 bilet trzydniowy strefa 1, ulgowy 50					
            //57	 bilet siedmiodniowy strefa 1, ulgowy 50					
            //59	 bilet 14-dniowy strefa 1, ulgowy 50					
            //5b     bilet 30-dniowy imienny na 1 linie strefa 1, ulgowy 50					
            //5c     bilet 30-dniowy imienny na wszystkie linie strefa 1, ulgowy 50 					
            //5d	 bilet 30-dniowy imienny na wszystkie linie strefa 1, ulgowy 50					
            //60	 bilet 90-dniowy imienny na wszystkie linie strefa 1, ulgowy 50					
            //61	 bilet 90-dniowy imienny na wszystkie linie strefa 1, ulgowy 50					
            //66	 bilet jednorazowy strefa 1+2, normalny					
            //6f	 bilet 20 minutowy strefa 1+2, normalny					
            //70	 bilet 40 minutowy strefa 1+2, normalny					
            //71	 bilet 60 minutowy strefa 1+2, normalny					
            //7a     bilet dobowy strefa 1+2, normalny					
            //7c     bilet trzydniowy strefa 1+2, normalny					
            //85	 bilet dobowy strefa 1+2, normalny					
            //86	 bilet trzydniowy strefa 1+2, normalny					
            //87	 bilet dobowy strefa 1+2, normalny					
            //88	 bilet trzydniowy strefa 1+2, normalny					
            //89	 bilet siedmiodniowy strefa 1+2, normalny					
            //8b     bilet 14-dniowy strefa 1+2, normalny					
            //8d	 bilet 30-dniowy imienny na 1 linie strefa 1+2, normalny					
            //8e	 bilet 30-dniowy imienny na wszystkie linie strefa 1+2, normalny					
            //8f	 bilet 30-dniowy imienny na wszystkie linie strefa 1+2, normalny					
            //90	 bilet 30-dniowy na okaziciela na wszystkie linie strefa 1+2, normalny					
            //91	 bilet 30-dniowy na okaziciela na wszystkie linie strefa 1+2, normalny					
            //92	 bilet 90-dniowy imienny na wszystkie linie strefa 1+2, normalny					
            //93	 bilet 90-dniowy imienny na wszystkie linie strefa 1+2, normalny					
            //94	 bilet 90-dniowy na okaziciela na wszystkie linie strefa 1+2, normalny					
            //95	 bilet 90-dniowy na okaziciela na wszystkie linie strefa 1+2, normalny					
            //96	 bilet seniora, waĹźny 365 dni na wszystkie linie strefa 1+2					
            //98	 bilet jednorazowy strefa 1+2, ulgowy
            //a1     bilet 20 minutowy strefa 1+2, ulgowy
            //a2     bilet 40 minutowy strefa 1+2, ulgowy
            //a3     bilet 60 minutowy strefa 1+2, ulgowy
            //ac     bilet dobowy strefa 1+2, ulgowy
            //ae     bilet trzydniowy strefa 1+2, ulgowy
            //b7     bilet dobowy strefa 1+2, ulgowy
            //b8     bilet trzydniowy strefa 1+2, ulgowy
            //b9     bilet dobowy strefa 1+2, ulgowy
            //ba     bilet trzydniowy strefa 1+2, ulgowy
            //bb     bilet siedmiodniowy strefa 1+2, ulgowy
            //bd     bilet 14-dniowy strefa 1+2, ulgowy
            //bf     bilet 30-dniowy imienny na 1 linie strefa 1+2, ulgowy
            //c0     bilet 30-dniowy imienny na wszystkie linie strefa 1+2, ulgowy
            //c1     bilet 30-dniowy imienny na wszystkie linie strefa 1+2, ulgowy
            //c2     bilet 30-dniowy na okaziciela na wszystkie linie strefa 1+2, ulgowy
            //c3     bilet 30-dniowy na okaziciela na wszystkie linie strefa 1+2, ulgowy
            //c4     bilet 90-dniowy imienny na wszystkie linie strefa 1+2, ulgowy
            //c5     bilet 90-dniowy imienny na wszystkie linie strefa 1+2, ulgowy
            //c6     bilet 90-dniowy na okaziciela na wszystkie linie strefa 1+2, ulgowy
            //c7     bilet 90-dniowy na okaziciela na wszystkie linie strefa 1+2, ulgowy
            //d2	 ??? 210 bilet nieznany - pokazuje wersje kasownika bez blokowania(pracowniczy?) - waĹźny 3 lata od skasowania
            //d3	 "przepustka na stacje metra" (pasazerska/HDK/70+)					
            //d4	 "przepustka na stacje metra" (pasazerska/HDK/70+)					
            //d6	 "przepustka na stacje metra" (SOM/policja/obsluga techniczna), waĹźny rok od skasowania
            //dd	 "ZTM pass - karta testowa", waĹźny rok od skasowania
            //e7     bilet pracowniczy ZTM, waĹźny rok od skasowania
            //e8     bilet pracowniczy MZA, waĹźny rok od skasowania
            //e9     bilet pracowniczy TW, waĹźny rok od skasowania
            //ea     bilet pracowniczy MW, waĹźny rok od skasowania
            //eb     bilet pracowniczy SKM, waĹźny rok od skasowania
            //ef     bilet dzieci z rodzin wielodzietnych, waĹźny rok od skasowania
            //f0     bilet osoby niepeĹ? nosprawnej, waĹźny rok od skasowania
            //f5	 ??? 245 bilet nieznany - "poza dozwolonym terminem uzycia"					
            //fa	 "precode" (nie rozpoznawany przez kasowniki)					
            //fe	 "navette" (karta kontrolerska)					
            //ff     admin



        #endregion

        
    }

    public class ZTMSectorSkasowania
    {
        public ZTMSectorSkasowania(string blok)
        {
            BlockALL = blok;
        }


        private string StringBlock;
        public string BlockALL
        {
            get { return StringBlock; }
            set
            {
                StringBlock = value;

                for (int indx = 0; indx < 13; indx++)
                {
                    StringBlockBinary += Convert.ToString(Convert.ToInt32(value.Substring(indx*2, 2), 16), 2).PadLeft(8, '0');
                }

                liczbaSkasowan = Convert.ToInt32(StringBlockBinary.Substring(4, 12), 2);
                rokWaznosci = Convert.ToInt32(StringBlockBinary.Substring(16, 7), 2);
                miesiacWaznosci = Convert.ToInt32(StringBlockBinary.Substring(23, 4), 2);
                dzienWaznosci = Convert.ToInt32(StringBlockBinary.Substring(27, 5), 2);
                godzinaWaznosci = Convert.ToInt32(StringBlockBinary.Substring(32, 5), 2);
                minutaWaznosci = Convert.ToInt32(StringBlockBinary.Substring(37, 6), 2);
                rokSkasowania = Convert.ToInt32(StringBlockBinary.Substring(43, 7), 2);
                miesiacSkasowania = Convert.ToInt32(StringBlockBinary.Substring(50, 4), 2);
                dzienSkasowania = Convert.ToInt32(StringBlockBinary.Substring(54, 5), 2);
                godzinaSkasowania = Convert.ToInt32(StringBlockBinary.Substring(59, 5), 2);
                minutaSkasowania = Convert.ToInt32(StringBlockBinary.Substring(64, 6), 2);
                linia = Convert.ToInt32(StringBlockBinary.Substring(70, 14), 2);
                brygada = Convert.ToInt32(StringBlockBinary.Substring(84, 10), 2);




            }
        }

        private string StringBlockBinary;
        public string StringBlockBinarnie
        {
            get { return StringBlockBinary; }
            //set { StringBlockBinary = value; }
        }


        private Int32 liczbaSkasowan;
        public Int32 LiczbaSkasowan
        {
            get { return liczbaSkasowan; }
            //set { liczbaSkasowan = value; }
        }

        private Int32 rokWaznosci;
        public Int32 RokWaznosci
        {
            get { return rokWaznosci; }
            //set { rokWaznosci = value; }
        }

        private Int32 miesiacWaznosci;
        public Int32 MiesiacWaznosci
        {
            get { return miesiacWaznosci; }
            //set { miesiacWaznosci = value; }
        }
        
        private Int32 dzienWaznosci;
        public Int32 DzienWaznosci
        {
            get { return dzienWaznosci; }
            //set { dzienWaznosci = value; }
        }

        private Int32 godzinaWaznosci;
        public Int32 GodzinaWaznosci
        {
            get { return godzinaWaznosci; }
            //set { godzinaWaznosci = value; }
        }

        private Int32 minutaWaznosci;
        public Int32 MinutaWaznosci
        {
            get { return minutaWaznosci; }
            //set { minutaWaznosci = value; }
        }

        private Int32 rokSkasowania;
        public Int32 RokSkasowania
        {
            get { return rokSkasowania; }
            //set { rokSkasowania = value; }
        }

        private Int32 miesiacSkasowania;
        public Int32 MiesiacSkasowania
        {
            get { return miesiacSkasowania; }
            //set { miesiacSkasowania = value; }
        }

        private Int32 dzienSkasowania;
        public Int32 DzienSkasowania
        {
            get { return dzienSkasowania; }
            //set { dzienSkasowania = value; }
        }

        private Int32 godzinaSkasowania;
        public Int32 GodzinaSkasowania
        {
            get { return godzinaSkasowania; }
            //set { godzinaSkasowania = value; }
        }

        private Int32 minutaSkasowania;
        public Int32 MinutaSkasowania
        {
            get { return minutaSkasowania; }
            //set { minutaSkasowania = value; }
        }

        private Int32 linia;
        public Int32 Linia
        {
            get { return linia; }
            //set { linia = value; }
        }

        private Int32 brygada;
        public Int32 Brygada
        {
            get { return brygada; }
            //set { brygada = value; }
        }




    }
    
    public class ZTMSectorParkowania
    {

    }

    public class ZTMKartaMiejska
    {
        public ZTMKartaMiejska(string blok0, string blok4, string blok8, string blok12)
        {
            ZTMSectorID SektorID = new ZTMSectorID(blok0);
            ZTMSectorTypu SektorTypu = new ZTMSectorTypu(blok4);
            ZTMSectorSkasowania SektorSkasowania2 = new ZTMSectorSkasowania(blok8);
            ZTMSectorSkasowania SektorSkasowania3 = new ZTMSectorSkasowania(blok12);
            ZTMSectorParkowania SektorParkowania = new ZTMSectorParkowania();
        }
        
    }

}
