using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCSC;

namespace MifareCardReaderLibrary
{
    public static class CardReaderFinder
    {
        public static string FindReaderName(bool DebugMode)
        {
            using (var context = new SCardContext())
            {
                context.Establish(SCardScope.System);
                string[] readerNames = null;
                try
                {
                    // retrieve all reader names
                    readerNames = context.GetReaders();
                    // get the card status of each reader that is currently connected
                    foreach (var readerName in readerNames)
                    {
                        using (var reader = new SCardReader(context))
                        {
                            if (DebugMode)
                            {
                                Console.WriteLine("Trying to connect to reader: {0}.", readerName);
                            }
                            var sc = reader.Connect(readerName, SCardShareMode.Shared, SCardProtocol.Any);
                            if (sc == SCardError.Success)
                            {
                                if (DebugMode)
                                {
                                    Console.WriteLine("Connected to reader succesfully :-)");
                                }
                                return reader.ReaderName;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    return null;
                }
                catch (Exception)
                {
                    if (readerNames == null)
                    {
                        return null;
                    }
                    return null;
                }
            }





        }
    }
}
