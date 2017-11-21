using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AG.MiFARE.Exceptions
{
    public class CardIncValueException: Exception 
    {
        public CardIncValueException(String msg)
            : base(msg)
        {
        }
    }
}
