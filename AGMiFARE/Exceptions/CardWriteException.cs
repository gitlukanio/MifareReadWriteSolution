using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AG.MiFARE.Exceptions
{
    public class CardWriteException: Exception
    {
        public CardWriteException(String msg)
            : base(msg)
        {
        }
    }
}
