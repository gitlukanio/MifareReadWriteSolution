﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MifareCardReaderLibrary
{
    public class CardReadException : Exception
    {
        public CardReadException(String msg)
            : base(msg)
        {
        }
    }
}
