using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AG.MiFARE.Exceptions
{
    public class CardWriteValueException: Exception
    {
        public CardWriteValueException(String msg)
            : base(msg)
        {
        }
    }
}
