using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACR122U_Helper_Library
{
    public class CardLoginException : Exception
    {
        public CardLoginException(String msg)
            : base(msg)
        {
        }
    }
}
