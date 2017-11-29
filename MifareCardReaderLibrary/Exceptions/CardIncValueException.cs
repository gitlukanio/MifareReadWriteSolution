using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MifareCardReaderLibrary
{
    public class CardIncValueException : Exception
    {
        public CardIncValueException(String msg)
            : base(msg)
        {
        }
    }
}
