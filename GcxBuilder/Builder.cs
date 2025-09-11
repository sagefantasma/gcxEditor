using System.Drawing;

namespace GcxEditor
{
    public static class Builder
    {
        private static int sizeOfU24 = 3;
        public static byte[] InitializeSize(uint size, byte declaringHighNibble, out int position)
        {
            byte[] encodedBytes;
            //TODO: confirm these sizes are accurate
            if (size < 0xD)
            {
                encodedBytes = new byte[size];
                encodedBytes[0] = (byte)(declaringHighNibble + size);
                position = 1;
            }
            else if(size < 0xFF)
            {
                encodedBytes = new byte[size + 1];
                encodedBytes[0] = (byte)(declaringHighNibble + 0xD);
                encodedBytes[1] = (byte)size;
                position = 2;
            }
            else if(0xFF > size && size > 0xFFFF)
            {
                encodedBytes = new byte[size + 2];
                encodedBytes[0] = (byte)(declaringHighNibble + 0xE);
                Array.Copy(BitConverter.GetBytes((ushort)size),0, encodedBytes, 1, sizeof(ushort));
                position = 3;
            }
            else 
            {
                encodedBytes = new byte[size + 3];
                encodedBytes[0] = (byte)(declaringHighNibble + 0xF);
                Array.Copy(BitConverter.GetBytes(size), 1, encodedBytes, 1, sizeOfU24);
                position = 4;
            }

            return encodedBytes;
        }
    }
}
