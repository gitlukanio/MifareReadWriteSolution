using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MifareCardReaderLibrary
{
    public class CardWriteValueException : Exception
    {
        public CardWriteValueException(String msg)
            : base(msg)
        {
        }
    }
}
