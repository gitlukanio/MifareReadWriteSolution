using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACR122U_Helper_Library;

namespace MifareCardReaderLibrary
{
    public class CardValueReader : CardReader, ICardValueReader
    {


        // Events and all related with it








        // Value Block Related Commands

        // Value Block Operation - Write, Increment, Decrement
        public bool ValueBlockOperation(int blockNumber, VBOperationType OperationType, int VB_Value)
        {

            byte[] tmpArray = new byte[4];
            tmpArray = BitConverter.GetBytes(VB_Value);
            Array.Reverse(tmpArray);

            ClearBuffers();
            SendBuff[0] = 0xFF;
            SendBuff[1] = 0xD7;
            SendBuff[2] = 0x00;
            SendBuff[3] = (byte)blockNumber;
            SendBuff[4] = 0x05;
            SendBuff[5] = (byte)OperationType;

            for (int i = 6; i < 10; i++)
            {
                SendBuff[i] = tmpArray[i - 6];
            }

            SendLen = 10;
            RecvLen = 2;

            if (SendDataToAPDU())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Read Value Block
        public bool ReadValueBlock(int blockNumber, out int Value)
        {
            byte[] DataReceiveBuffor = new byte[4];
            Value = 0;
            ClearBuffers();
            SendBuff[0] = 0xFF;
            SendBuff[1] = 0xB1;
            SendBuff[2] = 0x00;
            SendBuff[3] = (byte)blockNumber;
            SendBuff[4] = 0x04;

            SendLen = 5;
            RecvLen = 4 + 2;

            string tmpStr = "";
            for (int indx = 0; indx <= RecvLen - 1; indx++)
            {
                tmpStr = tmpStr + String.Format("{0:X2}", RecvBuff[indx]);
            }

            if (SendDataToAPDU())
            {
                if (RecvCodeStr == "9000")
                {
                    Array.Reverse(DataReceiveBuffor);
                    Array.Copy(RecvBuff, DataReceiveBuffor, DataReceiveBuffor.Length);
                    Value = BitConverter.ToInt32(DataReceiveBuffor, 0);
                }
                return true;
            }
            else
            {
                return false;
            }

        }

        // Restore Value Block - Copy a value from a value block to another value block
        public bool RestoreValueBlock(int sourceBlockNumber, int targetBlockNumber)
        {
            // Source Block and Target Block must be in the same sector!!!!!!
            ClearBuffers();
            SendBuff[0] = 0xFF;
            SendBuff[1] = 0xD7;
            SendBuff[2] = 0x00;
            SendBuff[3] = (byte)sourceBlockNumber;
            SendBuff[4] = 0x02;
            SendBuff[5] = 0x03;
            SendBuff[6] = (byte)targetBlockNumber;

            SendLen = 7;
            RecvLen = 2;

            string tmpStr = "";
            for (int indx = 0; indx <= RecvLen - 1; indx++)
            {
                tmpStr = tmpStr + String.Format("{0:X2}", RecvBuff[indx]);
            }

            if (SendDataToAPDU())
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        //=========================================================
        // Members required by ICardValueReader

        public bool ReadValue(int sector, int datablock, out int value)
        {


            return ReadValueBlock(sector * 4 + datablock, out value);
        }

        public bool WriteValue(int sector, int datablock, int value)
        {
            return ValueBlockOperation(sector * 4 + datablock, VBOperationType.WriteVB_Value, value);
        }

        public bool IncValue(int sector, int datablock, int delta, out int value)
        {
            // Login(sector, KeyTypeEnum.KeyA) &&
            if (ValueBlockOperation(sector * 4 + datablock, VBOperationType.IncrementByVB_Value, delta))
            {
                return ReadValue(sector, datablock, out value);
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public bool DecValue(int sector, int datablock, int delta, out int value)
        {
            if (Login(sector, KeyTypeEnum.KeyA) && ValueBlockOperation(sector * 4 + datablock, VBOperationType.DecrementByVB_Value, delta))
            {
                return ReadValue(sector, datablock, out value);
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public bool CopyValue(int sector, int srcBlock, int dstBlock, out int value)
        {
            if (RestoreValueBlock(sector * 4 + srcBlock, sector * 4 + dstBlock))
            {
                return ReadValue(sector, dstBlock, out value);
            }
            else
            {
                value = 0;
                return false;
            }
        }
    }
}
