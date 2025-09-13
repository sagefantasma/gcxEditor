using System.Drawing;

namespace GcxEditor
{
    public static class Builder
    {
        private static int sizeOfU24 = 3;

        public static byte[] BuildContainerElement(byte declarationNibble, byte[] contents)
        {
            byte[] containerBytes;

            int position = 0;
            if (contents.Length < 0xD)
            {
                containerBytes = new byte[contents.Length + 1];
                containerBytes[position++] = (byte)(declarationNibble + contents.Length);
            }
            else if (contents.Length < 0xFF)
            {
                containerBytes = new byte[contents.Length + 2];
                containerBytes[position++] = (byte)(declarationNibble + 0xD);
                containerBytes[position++] = (byte)contents.Length;
            }
            else if (contents.Length < 0xFFFF)
            {
                containerBytes = new byte[contents.Length + 2];
                containerBytes[position++] = (byte)(declarationNibble + 0xE);
                Array.Copy(BitConverter.GetBytes((ushort)contents.Length), 0, containerBytes, position+=2, sizeof(short));
            }
            else
            {
                containerBytes = new byte[contents.Length + 3];
                containerBytes[0] = (byte)(declarationNibble + 0xF);
                Array.Copy(BitConverter.GetBytes(contents.Length), 1, containerBytes, position += sizeOfU24, sizeOfU24);
            }

            Array.Copy(contents, 0, containerBytes, position, contents.Length);

            return containerBytes;
        }

        public static byte[] EncodeCommandWithArgsAndParams(List<Term> args, List<Parameter> parameters, byte[] commandDeclarationBytes)
        {
            byte[] argsBytes = EncodeCommandWithOnlyArgs(args, commandDeclarationBytes);
            uint parametersBytesLength = 0;

            foreach (Parameter parameter in parameters)
            {
                parametersBytesLength += parameter.Size;
            }

            byte[] encodedBytes = new byte[argsBytes.Length +  parametersBytesLength];

            Array.Copy(argsBytes, encodedBytes, argsBytes.Length);
            int position = argsBytes.Length;

            foreach(Parameter parameter in parameters)
            {
                byte[] encodedParam = parameter.Encode();
                Array.Copy(encodedParam, 0, encodedBytes, position += encodedParam.Length, encodedParam.Length);
            }

            return encodedBytes;
        }

        public static byte[] EncodeCommandWithOnlyArgs(List<Term> args, byte[] commandDeclarationBytes) 
        {
            uint argsBytesLength = 0;
            uint declarationSize = 4;

            foreach (Term argument in args)
            {
                argsBytesLength += argument.Size;
            }

            byte[] encodedBytes;
            int position = 3;
            if ((argsBytesLength + 3) < 0x80)
            {
                declarationSize++;
                encodedBytes = new byte[declarationSize + argsBytesLength];
                encodedBytes[position++] = (byte)(argsBytesLength);
            }
            else if ((argsBytesLength + 3) > 0xFF && (argsBytesLength + 3) < 0xFFFF)
            {
                declarationSize += 2;
                encodedBytes = new byte[declarationSize + argsBytesLength];
                encodedBytes[position++] = 0x7E;
                Array.Copy(BitConverter.GetBytes((ushort)(argsBytesLength + 3)), 0, encodedBytes, position += sizeof(ushort), sizeof(ushort));
            }
            else
            {
                declarationSize += 3;
                encodedBytes = new byte[declarationSize + argsBytesLength];
                encodedBytes[position++] = 0x7F;
                Array.Copy(BitConverter.GetBytes(argsBytesLength + 3), 0, encodedBytes, position += 3, 3);
            }

            Array.Copy(commandDeclarationBytes, encodedBytes, commandDeclarationBytes.Length);

            foreach (Term arg in args)
            {
                byte[] encodedArg = arg.Encode();
                Array.Copy(encodedArg, 0, encodedBytes, position, encodedArg.Length);
                position += encodedArg.Length;
            }

            return encodedBytes;
        }

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
