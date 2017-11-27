using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ACR122U_Helper_Library
{
    public class Sector
    {
        #region Private functions
        private MiFARECard _Card;
        private int _Sector;

        private DataBlock[] _DataBlocks;
        private AccessConditions _Access;
        #endregion

        #region Constructor
        internal Sector(MiFARECard card, int sector)
        {
            _Card = card;
            _Sector = sector;

            _DataBlocks = new DataBlock[NumDataBlocks];
            _Access = null;
        }
        #endregion

        #region Properties

        #region Access
        /// <summary>
        /// Sector access conditions 
        /// </summary>
        public AccessConditions Access
        {
            get
            {
                if (_Access == null)
                {
                    Byte[] data = GetData(GetTrailerBlockIndex());
                    _Access = AccessBits.GetAccessConditions(data);
                }

                return _Access;
            }
        }
        #endregion

        #region KeyA
        /// <summary>
        /// Key A for the sector. This needs to be set when setting the access conditions. Key could not be read from card
        /// </summary>
        public String KeyA
        {
            get
            {
                Byte[] data = GetData(GetTrailerBlockIndex());
                Byte[] keyA = new Byte[6];
                Array.Copy(data, 0, keyA, 0, 6);

                return HexEncoding.ToString(keyA);
                //return ByteArrayToString(keyA);
            }
            set
            {
                int discarded;
                Byte[] keyA = HexEncoding.GetBytes(value, out discarded);

                DataBlock db = GetDataBlockInt(GetTrailerBlockIndex());
                Array.Copy(keyA, 0, db.Data, 0, 6);
            }
        }

        //public string ByteArrayToString(byte[] ba)
        //{
        //    StringBuilder hex = new StringBuilder(ba.Length * 2);
        //    foreach (byte b in ba)
        //        hex.AppendFormat("{0:x2}", b);
        //    return hex.ToString();
        //}


        //public byte[] StringToByteArray(String hex)
        //{
        //    int NumberChars = hex.Length;
        //    byte[] bytes = new byte[NumberChars / 2];
        //    for (int i = 0; i < NumberChars; i += 2)
        //        bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        //    return bytes;
        //}




        #endregion

        #region KeyB
        /// <summary>
        /// Key B for the sector. This needs to be set when setting the access conditions. Key could not be read from card
        /// </summary>
        public String KeyB
        {
            get
            {
                Byte[] data = GetData(GetTrailerBlockIndex());
                Byte[] keyB = new Byte[6];
                Array.Copy(data, 10, keyB, 0, 6);

                return HexEncoding.ToString(keyB);
            }
            set
            {
                int discarded;
                Byte[] keyB = HexEncoding.GetBytes(value, out discarded);

                DataBlock db = GetDataBlockInt(GetTrailerBlockIndex());
                Array.Copy(keyB, 0, db.Data, 10, 6);
            }
        }
        #endregion

        #region DataLength
        /// <summary>
        /// number of data bytes in the sector (trailer datablock is excluded)
        /// </summary>
        public int DataLength
        {
            get
            {
                return (NumDataBlocks - 1) * DataBlock.Length;
            }
        }
        #endregion

        #region TotalLength
        /// <summary>
        /// number of bytes in the sector (including trailer datablock)
        /// </summary>
        public int TotalLength
        {
            get
            {
                return NumDataBlocks * DataBlock.Length;
            }
        }
        #endregion

        #region NumDataBlocks
        /// <summary>
        /// number of datablocks in the sector
        /// </summary>
        public int NumDataBlocks
        {
            get
            {
                if (_Sector < 32)
                    return 4;

                return 16;
            }
        }
        #endregion

        #endregion

        #region Public functions

        #region GetData
        /// <summary>
        /// read data from a datablock
        /// </summary>
        /// <param name="block">index of the datablock</param>
        /// <returns>data read (always 16 bytes)</returns>
        /// <remarks>may throw CardLoginException and CardReadException</remarks>
        public Byte[] GetData(int block)
        {
            DataBlock db = GetDataBlockInt(block);
            if (db == null)
                return null;

            return db.Data;
        }
        #endregion

        #region SetData
        /// <summary>
        /// write data in the sector
        /// </summary>
        /// <param name="data">data to write</param>
        /// <param name="firstBlock">the index of the block to start write</param>
        /// <remarks>may throw CardLoginException and CardWriteException.
        /// if the length of the data to write overcomes the number of datablocks, the remaining data is not written
        /// </remarks>
        public void SetData(Byte[] data, int firstBlock)
        {
            int blockIdx = firstBlock;
            int bytesWritten = 0;

            while ((blockIdx < (NumDataBlocks - 1)) && (bytesWritten < data.Length))
            {
                int numBytes = Math.Min(DataBlock.Length, data.Length - bytesWritten);

                Byte[] blockData = GetData(blockIdx);
                Array.Copy(data, bytesWritten, blockData, 0, numBytes);

                bytesWritten += numBytes;
                blockIdx++;
            }
        }
        #endregion

        #region Flush
        /// <summary>
        /// commit changes to card
        /// </summary>
        /// <remarks>may throw CardLoginException and CardWriteException</remarks>
        public void Flush()
        {
            foreach (DataBlock dataBlock in _DataBlocks)
            {
                if (dataBlock == null)
                    continue;

                if (dataBlock.IsTrailer)
                    continue;

                if (dataBlock.IsChanged)
                    FlushDataBlock(dataBlock);
            }
        }
        #endregion

        #region FlushTrailer
        /// <summary>
        /// commit changes made to trailer datablock
        /// </summary>
        /// <remarks>may throw CardLoginException and CardWriteException</remarks>
        public void FlushTrailer(String keyA, String keyB)
        {
            DataBlock dataBlock = _DataBlocks[GetTrailerBlockIndex()];
            if (dataBlock == null)
                return;

            KeyA = keyA;
            KeyB = keyB;

            Byte[] data = AccessBits.CalculateAccessBits(Access);
            Array.Copy(data, 0, dataBlock.Data, 6, 4);

            if (dataBlock.IsChanged)
                FlushDataBlock(dataBlock);

            _Card.ActiveSector = -1;
        }
        #endregion

        #region Value access functions

        #region ReadValue
        public int ReadValue(int datablock)
        {
            int value;

            //if (!_Card.Reader.Login(_Sector, KeyTypeEnum.KeyA)) //LLU
            //    throw new CardLoginException(String.Format("Unable to login in sector {0} with key A", _Sector));


            if (!_Card.ValueReader.ReadValue(_Sector, datablock, out value))
                throw new CardReadValueException("Error in ReadValue");

            return value;
        }
        #endregion

        #region WriteValue
        public void WriteValue(int datablock, int value)
        {
            if (!_Card.ValueReader.WriteValue(_Sector, datablock, value))
                throw new CardWriteValueException("Error in WriteValue");
        }
        #endregion

        #region IncValue
        public int IncValue(int datablock, int delta)
        {
            int value;
            if (!_Card.ValueReader.IncValue(_Sector, datablock, delta, out value))
                throw new CardWriteValueException("Error in IncValue");

            return value;
        }
        #endregion

        #region DecValue
        public int DecValue(int datablock, int delta)
        {
            int value;
            if (!_Card.ValueReader.DecValue(_Sector, datablock, delta, out value))
                throw new CardWriteValueException("Error in DecValue");

            return value;
        }
        #endregion

        #region CopyValue
        public int CopyValue(int srcBlock, int dstBlock)
        {
            int value;
            if (!_Card.ValueReader.CopyValue(_Sector, srcBlock, dstBlock, out value))
                throw new CardWriteValueException("Error in CopyValue");

            return value;
        }
        #endregion

        #endregion

        #endregion

        #region Private functions

        #region GetDataBlockInt
        private DataBlock GetDataBlockInt(int block)
        {
            DataBlock db = _DataBlocks[block];

            if (db != null)
                return db;

            if (_Card.ActiveSector != _Sector)
            {
                if (!_Card.Reader.Login(_Sector, KeyTypeEnum.KeyA))
                    throw new CardLoginException(String.Format("Unable to login in sector {0} with key A", _Sector));

                Byte[] data;
                if (!_Card.Reader.Read(_Sector, block, out data))
                    throw new CardReadException(String.Format("Unable to read from sector {0}, block {1}", _Sector, block));

                db = new DataBlock(block, data, (block == GetTrailerBlockIndex()));
                _DataBlocks[block] = db;
            }

            return db;
        }

        #endregion

        #region FlushDataBlock
        private void FlushDataBlock(DataBlock dataBlock)
        {
            if (_Card.ActiveSector != _Sector)
            {
                if (!_Card.Reader.Login(_Sector, GetWriteKey(dataBlock.Number)))
                    throw new CardLoginException(String.Format("Unable to login in sector {0} with key A", _Sector));

                _Card.ActiveSector = _Sector;
            }

            if (!_Card.Reader.Write(_Sector, dataBlock.Number, dataBlock.Data))
                throw new CardWriteException(String.Format("Unable to write in sector {0}, block {1}", _Sector, dataBlock.Number));
        }
        #endregion

        #region GetWriteKey
        private KeyTypeEnum GetWriteKey(int datablock) //private
        {
            if (Access == null)
                return KeyTypeEnum.KeyDefaultF;

            if (datablock == 3)
                return GetTrailerWriteKey();

            return (Access.DataAreas[Math.Min(datablock, Access.DataAreas.Length - 1)].Write == DataAreaAccessCondition.ConditionEnum.KeyA) ? KeyTypeEnum.KeyA : KeyTypeEnum.KeyB;
        }
        #endregion

        //LLU
        public KeyTypeEnum GetKeyType(int dataBlock, OperationType operationType)
        {
            if (Access == null)
                return KeyTypeEnum.KeyDefaultF;
            switch (operationType)
            {
                case OperationType.Write:
                    return (Access.DataAreas[dataBlock].Write == DataAreaAccessCondition.ConditionEnum.KeyA) ? KeyTypeEnum.KeyA : KeyTypeEnum.KeyB;

                case OperationType.Increment:
                    return (Access.DataAreas[dataBlock].Increment == DataAreaAccessCondition.ConditionEnum.KeyA) ? KeyTypeEnum.KeyA : KeyTypeEnum.KeyB;

                case OperationType.Decrement:
                    return (Access.DataAreas[dataBlock].Decrement == DataAreaAccessCondition.ConditionEnum.KeyA) ? KeyTypeEnum.KeyA : KeyTypeEnum.KeyB;

                default:
                    throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null);
            }
        }





        #region GetTrailerWriteKey
        private KeyTypeEnum GetTrailerWriteKey()
        {
            if (Access == null)
                return KeyTypeEnum.KeyDefaultF;

            return (Access.Trailer.AccessBitsWrite == TrailerAccessCondition.ConditionEnum.KeyA) ? KeyTypeEnum.KeyA : KeyTypeEnum.KeyB;
        }
        #endregion

        #region GetTrailerBlockIndex
        private int GetTrailerBlockIndex()
        {
            return NumDataBlocks - 1;
        }
        #endregion

        #endregion

    }

    public enum OperationType
    {
        Write,
        Increment,
        Decrement
    }

    public static class HexEncoding
    {
        public static string ToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }


        public static byte[] GetBytes(String hex, out int all)
        {

            int NumberChars = hex.Length;

            all = NumberChars;

            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }

}
