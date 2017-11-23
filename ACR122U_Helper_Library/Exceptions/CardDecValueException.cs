using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ACR122U_Helper_Library
{
    public class CardDecValueException : Exception
    {
        public CardDecValueException(String msg)
            : base(msg)
        {
        }
    }
}
