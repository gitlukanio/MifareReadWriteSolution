﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACR122U_Helper_Library
{
    public class CardReadValueException : Exception
    {
        public CardReadValueException(String msg)
            : base(msg)
        {
        }
    }
}
