using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GcxDecompiler
{
    internal class Parser
    {
        private List<int> highByteOptions = new List<int>{ 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90 };
        private List<int> lowByteOptions = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0xA, 0xD, 0xE };
        private bool Stop = false;
        private static Data data;

        private byte ReadByteAtOffset(int offset)
        {
            return data.DataArray[offset];
        }

        private byte ReadByte()
        {
            byte value = data.DataArray[data.position];
            data.position++;
            return value;
        }

        private ushort ReadShortAtOffset(int offset)
        {
            return BitConverter.ToUInt16(data.DataArray, offset);
        }

        private ushort ReadShort()
        {
            ushort value = BitConverter.ToUInt16(data.DataArray, data.position);
            data.position += 2;
            return value;
        }

        private int Read3ByteInt()
        {
            byte[] buffer = new byte[4];
            Array.Copy(data.DataArray, data.position, buffer, 0, 3);
            data.position += 3;

            return BitConverter.ToInt32(buffer, 0);
        }

        private uint ReadIntAtOffset(int offset)
        {
            return BitConverter.ToUInt32(data.DataArray, offset);
        }

        private uint ReadInt()
        {
            uint value = BitConverter.ToUInt32(data.DataArray, data.position);
            data.position += 4;
            return value;
        }

        private uint ReadBigEndianInt()
        {
            byte[] reversedArray = new byte[4];
            Array.Copy(data.DataArray, data.position+2, reversedArray, 0, 2);
            Array.Copy(data.DataArray, data.position, reversedArray, 2, 2);
            data.position += 4;
            uint value = BitConverter.ToUInt32(reversedArray);
            return value;
        }

        private int DecodeType(byte byteCode)
        {
            int codeHi = byteCode & 0xF0;
            int codeLo = byteCode & 0xF;

            if ((byteCode & 0xC0) == 0xC0)
                return 0xC0;

            if(codeHi != 0)
            {
                if (highByteOptions.Contains(codeHi))
                    return codeHi;
            }
            else
            {
                if (lowByteOptions.Contains(byteCode))
                    return byteCode;
            }

            Stop = true;
            return -1;
        }

        private int GetBlockSize()
        {
            int value = ReadByte() & 0xF;

            if (value == 13)
                // 2 bytes
                return ReadByte();
            if (value == 14)
                //3 bytes
                return ReadShort();
            if (value == 15)
                //4 bytes
                return Read3ByteInt();

            return value;
        }
    }
}
