using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACR122U_Helper_Library
{
    public class CardWriteException : Exception
    {
        public CardWriteException(String msg)
            : base(msg)
        {
        }
    }
}
