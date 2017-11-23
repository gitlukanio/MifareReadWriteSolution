using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleACR122U_2
{

    public class OnCardApearedEventArgs : EventArgs
    {
        //public cardConnector Karta { get; set; }
        public cardAccessor MojCardAccessor { get; set; }
    }

    public class cardAccessor
    {
        private cardConnector card;
        private bool HaltWorker = false;
        private bool WorkerBusy = false;
        /*
         *  Some sites you may want to check:
         *  http://www.csharp-examples.net/string-format-double/
         *  http://www.codeproject.com/KB/cs/String2DateTime.aspx
         *  http://www.blackwasp.co.uk/StringComparison.aspx
         */

        public cardAccessor()
        {
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Worker_DoWork;
            //worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerAsync();
        }

        public event EventHandler<OnCardApearedEventArgs> OnCardAppeared;

        protected virtual void RaiseEventOnCardAppeared(cardAccessor mCardAccessor)
        {
            if (OnCardAppeared != null)
                OnCardAppeared(this, new OnCardApearedEventArgs() { MojCardAccessor = this });
        }

        public event EventHandler OnCardDisappeared;

        protected virtual void RaiseEventOnCardDisappeared()
        {
            if (OnCardDisappeared != null)
                OnCardDisappeared(this, EventArgs.Empty);
        }

        BackgroundWorker worker = new BackgroundWorker();

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);

            bool cardExistOld = false;
            bool cardExistNow = false;
            while (true)
            {
                if (!HaltWorker)
                {
                    WorkerBusy = true;
                    Thread.Sleep(50);
                    cardExistNow = CardExist();
                    if (cardExistOld != cardExistNow)
                    {
                        if (cardExistNow)
                        {
                            worker.ReportProgress(10); // Pojawiła się karta na czytniku
                        }
                        else
                        {
                            worker.ReportProgress(20); // Zabrano kartę z czytnika
                        }
                        cardExistOld = cardExistNow;
                    }
                    WorkerBusy = false;
                }

                Thread.Sleep(200);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 10) RaiseEventOnCardAppeared(this);
            if (e.ProgressPercentage == 20) RaiseEventOnCardDisappeared();
        }

        private DateTime stringToDate(string sDate)
        {
            DateTime MyDateTime;
            MyDateTime = new DateTime();
            MyDateTime = DateTime.ParseExact(sDate, "yyyy-MM-dd", null);
            return MyDateTime;
        }

        private string dateToString(DateTime Date)
        {
            String MyString;
            MyString = Date.ToString("yyyy-MM-dd");
            return MyString;
        }

        private float stringToFloat(string sNum)
        {
            float myFloat;
            myFloat = int.Parse(sNum) / 100;
            return myFloat;
        }

        private string floatToString(float fNum)
        {
            string myString = null;
            //fNum = 123.45f;
            int myInt = (int)fNum * 100;
            myString = myInt.ToString();
            return myString;
        }

        public string getStringFromCard(int nBlock)
        {
            string temp = "";
            try
            {
                HaltWorker = true;

                card = new cardConnector();
                card.connectCard();
                temp = card.readBlock(nBlock.ToString());
                temp = temp.Substring(0, temp.Length - 4);
                card.Close();

                HaltWorker = false;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return temp;
        }


        public string GetStringFromCard(int nBlock)
        {
            string temp = "";
            try
            {
                HaltWorker = true;
                //  card = new cardConnector();
                //  card.connectCard();
                temp = card.readBlock(nBlock.ToString());
                temp = temp.Substring(0, temp.Length - 4);
                //card.Close();

                // HaltWorker = false;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return temp;
        }





        public bool Login(int BlockNumber, int KeyType, string Key)
        {
            HaltWorker = true;

            card = new cardConnector();
            card.connectCard();
            string tmp = card.authBlock(BlockNumber, KeyType, Key);
            //            card.Close();
            //Console.WriteLine("Login authBlock response: {0}", tmp);
            //HaltWorker = false;

            return true;

        }



        public void DisableBuzzer()
        {
            try
            {
                while (WorkerBusy)
                {
                    //Console.WriteLine("Przerwałem workerowi");
                    Thread.Sleep(1);
                }
                HaltWorker = true;
                card = new cardConnector();
                card.connectCard();
                card.DisableBuzz();
                card.Close();
                HaltWorker = false;

            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        public void ShowFirmwareVersion()
        {
            try
            {
                while (WorkerBusy)
                {
                    //Console.WriteLine("Przerwałem workerowi");
                    Thread.Sleep(1);
                }
                HaltWorker = true;
                card = new cardConnector();
                //card.connectCard();
                card.ShowFirmwareVersion();
                //card.Close();
                HaltWorker = false;

            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        public void ShowLEDstatus()
        {
            try
            {
                while (WorkerBusy)
                {
                    //Console.WriteLine("Przerwałem workerowi");
                    Thread.Sleep(1);
                }
                HaltWorker = true;
                card = new cardConnector();
                card.connectCard();
                card.ShowLEDstatus();
                card.Close();
                HaltWorker = false;
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        public bool CardExist()
        {
            try
            {
                card = new cardConnector();
                card.connectCard();
                if (card.connActive)
                {
                    card.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        public void setStringToCard(string sTemp, int nBlock)
        {
            try
            {
                card = new cardConnector();
                card.connectCard();
                card.submitText(sTemp, nBlock.ToString());
                card.Close();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public string getID()
        {
            // location of ID
            string id;
            Int32 id1 = 0;
            Int32 id2 = 0;
            string id3 = "";
            string id4 = "";
            string tmp = getStringFromCard(0);
            //Int32 xxx = Convert.ToInt32(tmp.Substring(8, 2), 16);
            id1 = Convert.ToInt32(tmp.Substring(6, 2), 16);
            id2 = Convert.ToInt32(tmp.Substring(4, 2) + tmp.Substring(2, 2) + tmp.Substring(0, 2), 16);
            if (id2 < 10000000) id3 = "0";
            if (id1 < 100) id4 = "0";
            id = $"Karta nr: { id4 }{ id1 } { id3 }{ id2 }";
            return id;
        }

        // do others
    }


}
