using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MifareCardReaderLibrary
{
    public class CardDecValueException : Exception
    {
        public CardDecValueException(String msg)
            : base(msg)
        {
        }
    }
}
