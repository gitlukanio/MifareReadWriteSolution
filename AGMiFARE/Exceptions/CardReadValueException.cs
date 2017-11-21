using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AG.MiFARE.Exceptions
{
    public class CardReadValueException: Exception 
    {
        public CardReadValueException(String msg)
            : base(msg)
        {
        }
    }
}
