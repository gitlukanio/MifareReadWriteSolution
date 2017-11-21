using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace AG.MiFARE
{
    public class FileReader : ICardReader
    {
        Byte[] _Data;


        public FileReader(string filename)
        {
            TextReader tr = File.OpenText(filename);
            String s = tr.ReadToEnd();
            tr.Close();

            s = s.Replace("\r", "");
            s = s.Replace("\n", "");

            int discarded;
            _Data = GetBytes(s, out discarded);
        }

        public void Flush(String filename)
        {
            String s = BytesToString(_Data);
            String[] dbs = Explode(s, 32);

            s = String.Join("\r\n", dbs);

            TextWriter tw = File.CreateText(filename);
            tw.Write(s);
            tw.Close();
        }

        #region ICardReader Members
        public bool GetCardType(out CardTypeEnum cardType)
        {
            cardType = CardTypeEnum.MiFARE_1K;
            return true;
        }

        public bool Login(int sector, KeyTypeEnum key)
        {
            Console.WriteLine("FileReader: Login({0}, {1}) invoked", sector, key);
            return true;
        }

        public bool Read(int sector, int datablock, out byte[] data)
        {
            Console.WriteLine("FileReader: Read({0}, {1}) invoked", sector, datablock);

            data = new Byte[16];
            Array.Copy(_Data, ((sector * 4) + datablock) * 16, data, 0, 16);
            return true;
        }

        public bool Write(int sector, int datablock, byte[] data)
        {
            Console.WriteLine("FileReader: Write({0}, {1}, {2}) invoked", sector, datablock, BytesToString(data));

            Array.Copy(data, 0, _Data, ((sector * 4) + datablock) * 16, 16);
            return true;
        }
        #endregion

        private byte[] GetBytes(string hexString, out int discarded)
        {
            discarded = 0;
            string newString = "";
            char c;
            // remove all none A-F, 0-9, characters
            for (int i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (IsHexDigit(c))
                    newString += c;
                else
                    discarded++;
            }
            // if odd number of characters, discard last character
            if (newString.Length % 2 != 0)
            {
                discarded++;
                newString = newString.Substring(0, newString.Length - 1);
            }

            int byteLength = newString.Length / 2;
            byte[] bytes = new byte[byteLength];
            string hex;
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                hex = new String(new Char[] { newString[j], newString[j + 1] });
                bytes[i] = HexToByte(hex);
                j = j + 2;
            }
            return bytes;
        }

        private bool IsHexDigit(Char c)
        {
            int numChar;
            int numA = Convert.ToInt32('A');
            int num1 = Convert.ToInt32('0');
            c = Char.ToUpper(c);
            numChar = Convert.ToInt32(c);
            if (numChar >= numA && numChar < (numA + 6))
                return true;
            if (numChar >= num1 && numChar < (num1 + 10))
                return true;
            return false;
        }

        private byte HexToByte(string hex)
        {
            if (hex.Length > 2 || hex.Length <= 0)
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return newByte;
        }

        private string BytesToString(byte[] bytes)
        {
            string hexString = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                hexString += bytes[i].ToString("X2");
            }
            return hexString;
        }

        private string[] Explode(String value, int size)
        {
            // Number of segments exploded to except last.
            int count = value.Length / size;

            // Determine if we need to store a final segment.
            // ... Sometimes we have a partial segment.
            bool final = false;
            if ((size * count) < value.Length)
            {
                final = true;
            }

            // Allocate the array to return.
            // ... The size varies depending on if there is a final fragment.
            string[] result;
            if (final)
            {
                result = new string[count + 1];
            }
            else
            {
                result = new string[count];
            }

            // Loop through each index and take a substring.
            // ... The starting index is computed with multiplication.
            for (int i = 0; i < count; i++)
            {
                result[i] = value.Substring((i * size), size);
            }

            // Sometimes we need to set the final string fragment.
            if (final)
            {
                result[result.Length - 1] = value.Substring(count * size);
            }
            return result;
        }
    }
}
