using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AG.MiFARE.Exceptions
{
    public class CardLoginException: Exception 
    {
        public CardLoginException(String msg)
            : base(msg)
        {
        }
    }
}
