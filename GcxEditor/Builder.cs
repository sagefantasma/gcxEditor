using System.Drawing;

namespace GcxEditor
{
    public static class Builder
    {
        private static readonly int SizeOfU24 = 3;

        public static byte[] BuildContainerElement(byte declarationNibble, byte[] contents)
        {
            byte[] containerBytes;

            uint sizeOfContentsPadded = (uint)(contents.Length + 1);
            int position = 0;
            if (sizeOfContentsPadded < 0xD)
            {
                containerBytes = new byte[sizeOfContentsPadded + 1];
                containerBytes[position++] = (byte)(declarationNibble + sizeOfContentsPadded);
            }
            else if (sizeOfContentsPadded < 0xFF)
            {
                containerBytes = new byte[sizeOfContentsPadded + 2];
                containerBytes[position++] = (byte)(declarationNibble + 0xD);
                containerBytes[position++] = (byte)(sizeOfContentsPadded);
            }
            else if (sizeOfContentsPadded < 0xFFFF)
            {
                containerBytes = new byte[sizeOfContentsPadded + 3];
                containerBytes[position++] = (byte)(declarationNibble + 0xE);
                Array.Copy(BitConverter.GetBytes((ushort)(sizeOfContentsPadded)), 0, containerBytes, position, sizeof(short));
                position += 2;
            }
            else
            {
                containerBytes = new byte[sizeOfContentsPadded + 4];
                containerBytes[0] = (byte)(declarationNibble + 0xF);
                Array.Copy(BitConverter.GetBytes(sizeOfContentsPadded), 1, containerBytes, position, SizeOfU24);
                position += SizeOfU24;
            }

            Array.Copy(contents, 0, containerBytes, position, contents.Length);

            return containerBytes;
        }

        public static byte[] EncodeCommandWithArgsAndParams(List<ITerm> args, List<Parameter> parameters, byte[] commandDeclarationBytes)
        {
            byte[] argsBytes = EncodeCommandWithOnlyArgs(args, commandDeclarationBytes);
            uint parametersBytesLength = 0;

            List<byte[]> encodedParams = new();
            foreach (Parameter parameter in parameters)
            {
                byte[] encodedParam = parameter.Encode();
                encodedParams.Add(encodedParam);
                parametersBytesLength += (uint)encodedParam.Length;
            }

            byte[] encodedBytes = new byte[argsBytes.Length +  parametersBytesLength];

            Array.Copy(argsBytes, encodedBytes, argsBytes.Length);
            int position = argsBytes.Length;

            foreach(byte[] encodedParam in encodedParams)
            {
                Array.Copy(encodedParam, 0, encodedBytes, position, encodedParam.Length);
                position += encodedParam.Length;
            }

            return encodedBytes;
        }

        public static byte[] EncodeCommandWithOnlyArgs(List<ITerm> args, byte[] commandDeclarationBytes) 
        {
            uint argsBytesLength = 0;
            uint declarationSize = 4;

            foreach (ITerm argument in args)
            {
                byte[] encodedArg = argument.Encode();
                argsBytesLength += (uint)encodedArg.Length;
            }

            byte[] encodedBytes;
            int position = 3;
            if ((argsBytesLength) < 0x80)
            {
                encodedBytes = new byte[declarationSize + argsBytesLength];
                encodedBytes[position++] = (byte)(argsBytesLength);
            }
            else
            {
                declarationSize++;
                encodedBytes = new byte[declarationSize + argsBytesLength];
                byte[] sizeInBytes = BitConverter.GetBytes(argsBytesLength);
                if (argsBytesLength < 0xFF)
                {
                    //byte after 0x80 is size
                    encodedBytes[position++] = 0x80;
                    encodedBytes[position++] = (byte)argsBytesLength;
                }
                else if(argsBytesLength < 0xFFF)
                {
                    //lowNibble of 0x8? is the highest nibble of the u24 size
                    encodedBytes[position++] = (byte)(0x80 + sizeInBytes[1]);
                    encodedBytes[position++] = sizeInBytes[0];
                }
                else if(argsBytesLength < 0xFFFF)
                {
                    //TODO: confirm
                    //0x?? is the highByte of the u32 size once subtracted by 0x80, second byte is lowByte of u32
                    byte highNibble = (byte)((sizeInBytes[0] & 0x0F) + (sizeInBytes[0] & 0xF0 - 0x80));
                    byte lowNibble = (byte)(sizeInBytes[0] & 0x0F);
                    encodedBytes[position++] = (byte)(highNibble + lowNibble);
                    encodedBytes[position++] = lowNibble;
                }
                else
                {
                    //TODO: is this even a case?
                }

            }

            Array.Copy(commandDeclarationBytes, encodedBytes, commandDeclarationBytes.Length);

            foreach (ITerm arg in args)
            {
                byte[] encodedArg = arg.Encode();
                Array.Copy(encodedArg, 0, encodedBytes, position, encodedArg.Length);
                position += encodedArg.Length;
            }

            return encodedBytes;
        }

        public static byte[] InitializeParamSize(uint size, byte declaringHighNibble, out int position)
        {
            byte[] encodedBytes;
            //TODO: confirm these sizes are accurate
            if (size <= 0xB)
            {
                encodedBytes = new byte[size + 2];
                encodedBytes[0] = (byte)(declaringHighNibble + size + 1);
                position = 1;
            }
            else if (size < 0xFF)
            {
                encodedBytes = new byte[size + 3];
                encodedBytes[0] = (byte)(declaringHighNibble + 0xD);
                encodedBytes[1] = (byte)(size + 1);
                position = 2;
            }
            else if (size < 0xFFFF)
            {
                encodedBytes = new byte[size + 4];
                encodedBytes[0] = (byte)(declaringHighNibble + 0xE);
                Array.Copy(BitConverter.GetBytes((ushort)(size + 1)), 0, encodedBytes, 1, sizeof(ushort));
                position = 3;
            }
            else
            {
                encodedBytes = new byte[size + 5];
                encodedBytes[0] = (byte)(declaringHighNibble + 0xF);
                Array.Copy(BitConverter.GetBytes(size + 1), 1, encodedBytes, 1, SizeOfU24);
                position = 4;
            }

            return encodedBytes;
        }

        public static byte[] InitializeSize(uint size, byte declaringHighNibble, out int position)
        {
            byte[] encodedBytes;
            //TODO: confirm these sizes are accurate
            if (size < 0xD)
            {
                encodedBytes = new byte[size + 2];
                encodedBytes[0] = (byte)(declaringHighNibble + size + 1);
                position = 1;
            }
            else if(size < 0xFF)
            {
                encodedBytes = new byte[size + 3];
                encodedBytes[0] = (byte)(declaringHighNibble + 0xD);
                encodedBytes[1] = (byte)(size + 1);
                position = 2;
            }
            else if(size < 0xFFFF)
            {
                encodedBytes = new byte[size + 4];
                encodedBytes[0] = (byte)(declaringHighNibble + 0xE);
                Array.Copy(BitConverter.GetBytes((ushort)(size + 1)),0, encodedBytes, 1, sizeof(ushort));
                position = 3;
            }
            else 
            {
                encodedBytes = new byte[size + 5];
                encodedBytes[0] = (byte)(declaringHighNibble + 0xF);
                Array.Copy(BitConverter.GetBytes(size + 1), 1, encodedBytes, 1, SizeOfU24);
                position = 4;
            }

            return encodedBytes;
        }
    }
}
