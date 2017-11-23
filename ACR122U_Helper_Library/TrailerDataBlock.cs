using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACR122U_Helper_Library
{
    internal class TrailerDataBlock : DataBlock
    {
        #region Constructor
        public TrailerDataBlock(Byte[] data)
            : base(3, data, true)
        {
        }
        #endregion

        #region Properties

        #region AccessBits
        public AccessBits AccessBits { get; private set; }
        #endregion

        #endregion

        #region Public functions
        #endregion
    }
}
