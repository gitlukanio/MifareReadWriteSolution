using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AG.MiFARE.Exceptions
{
    public class CardDecValueException: Exception 
    {
        public CardDecValueException(String msg)
            : base(msg)
        {
        }
    }
}
