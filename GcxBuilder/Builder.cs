using System.Drawing;

namespace GcxEditor
{
    public static class Builder
    {
        private static int sizeOfU24 = 3;

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
                Array.Copy(BitConverter.GetBytes(sizeOfContentsPadded), 1, containerBytes, position, sizeOfU24);
                position += sizeOfU24;
            }

            Array.Copy(contents, 0, containerBytes, position, contents.Length);

            return containerBytes;
        }

        public static byte[] EncodeCommandWithArgsAndParams(List<Term> args, List<Parameter> parameters, byte[] commandDeclarationBytes)
        {
            byte[] argsBytes = EncodeCommandWithOnlyArgs(args, commandDeclarationBytes);
            uint parametersBytesLength = 0;

            List<byte[]> encodedParams = new List<byte[]>();
            foreach (Parameter parameter in parameters)
            {
                //parametersBytesLength += parameter.Size;
                byte[] encodedParam = parameter.Encode();
                encodedParams.Add(encodedParam);
                parametersBytesLength += (uint)encodedParam.Length;

                /*if(parameter.Size < 0xD)
                {
                    parametersBytesLength += 1;
                }
                else if(parameter.Size < 0xFF)
                {
                    parametersBytesLength += 2;
                }
                else if(parameter.Size < 0xFFFF)
                {
                    parametersBytesLength += 3;
                }
                else
                {
                    parametersBytesLength += 4;
                }*/
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

        public static byte[] EncodeCommandWithOnlyArgs(List<Term> args, byte[] commandDeclarationBytes) 
        {
            uint argsBytesLength = 0;
            uint declarationSize = 4;

            foreach (Term argument in args)
            {
                byte[] encodedArg = argument.Encode();
                //argsBytesLength += argument.Size;
                argsBytesLength += (uint)encodedArg.Length;
            }

            byte[] encodedBytes;
            int position = 3;
            //if ((argsBytesLength + 3) < 0x80)
            if ((argsBytesLength) < 0x80)
            {
                //declarationSize++;
                encodedBytes = new byte[declarationSize + argsBytesLength];
                encodedBytes[position++] = (byte)(argsBytesLength);
            }
            else
            {
                //size = C4;
                //BitConverted => C4;
                //highNibble = (C0) + (04-80==-7B?) == 44???
                /*
                 * In the compiled gcx, a size might look something like this: C4 01
                 * This size would be equal to 0x441(?)
                 */
                declarationSize++;
                encodedBytes = new byte[declarationSize + argsBytesLength];
                byte[] sizeInBytes = BitConverter.GetBytes(argsBytesLength);
                if (argsBytesLength < 0xFF)
                {
                    //lowNibble is lowNibble of second size byte
                    encodedBytes[position++] = 0x80;
                    //encodedBytes[position++] = (byte)(sizeInBytes[0] & 0x0F);
                    encodedBytes[position++] = (byte)argsBytesLength;
                }
                else if(argsBytesLength < 0xFFF)
                {
                    //lowByte is second size byte
                    encodedBytes[position++] = (byte)(0x80 + sizeInBytes[1]);
                    encodedBytes[position++] = sizeInBytes[0];
                }
                else if(argsBytesLength < 0xFFFF)
                {
                    //TODO: confirm
                    //lowByte is second sizeByte and lowNibble is lowNibble of first size byte
                    byte highNibble = (byte)((sizeInBytes[0] & 0x0F) + (sizeInBytes[0] & 0xF0 - 0x80));
                    byte lowNibble = (byte)(sizeInBytes[0] & 0x0F);
                    encodedBytes[position++] = (byte)(highNibble + lowNibble);
                    encodedBytes[position++] = lowNibble;
                }
                else
                {

                }


                    //if < FF
                    //elseif < FFF
                    //else?
                    //byte lowByte = sizeInBytes[1];//need to figure out how to get the last byte of the size 100% of the time when it can be up to a u24...
                //encodedBytes[position++] = (byte)(declarationSize + argsBytesLength - 0x80);
            }
                /*else if ((argsBytesLength + 3) > 0xFF && (argsBytesLength + 3) < 0xFFFF) //this is hecked somehow
                {
                    declarationSize += 2;
                    encodedBytes = new byte[declarationSize + argsBytesLength];
                    encodedBytes[position++] = 0x7E;
                    Array.Copy(BitConverter.GetBytes((ushort)(argsBytesLength)), 0, encodedBytes, position, sizeof(ushort));
                    position += sizeof(ushort);
                }
                else
                {
                    declarationSize += 3;
                    encodedBytes = new byte[declarationSize + argsBytesLength];
                    encodedBytes[position++] = 0x7F;
                    Array.Copy(BitConverter.GetBytes(argsBytesLength), 0, encodedBytes, position, 3);
                    position += 3;
                }*/

                Array.Copy(commandDeclarationBytes, encodedBytes, commandDeclarationBytes.Length);

            foreach (Term arg in args)
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
                Array.Copy(BitConverter.GetBytes(size + 1), 1, encodedBytes, 1, sizeOfU24);
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
                Array.Copy(BitConverter.GetBytes(size + 1), 1, encodedBytes, 1, sizeOfU24);
                position = 4;
            }

            return encodedBytes;
        }
    }
}
