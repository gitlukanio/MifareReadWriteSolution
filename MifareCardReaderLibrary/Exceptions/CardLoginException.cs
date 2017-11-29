using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MifareCardReaderLibrary
{
    public class CardLoginException : Exception
    {
        public CardLoginException(String msg)
            : base(msg)
        {
        }
    }
}
