using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MifareCardReaderLibrary
{
    public class CardWriteException : Exception
    {
        public CardWriteException(String msg)
            : base(msg)
        {
        }
    }
}
