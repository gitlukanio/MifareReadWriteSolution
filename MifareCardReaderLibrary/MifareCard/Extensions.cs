﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MifareCardReaderLibrary
{
    public static class Extensions
    {
        public static bool IsEqual(this BitArray value, BitArray ba)
        {
            if (value.Length != ba.Length)
                return false;

            for (int i = 0; i < ba.Length; i++)
            {
                if (value.Get(i) != ba.Get(i))
                    return false;
            }

            return true;
        }
    }
}
