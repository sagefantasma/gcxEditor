using Gcx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GcxEditor
{
    public static class ProcParser
    {
        static byte[] SubprocDeclaration = [0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E]; 
        static byte[] CommandDeclaration = [0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E]; 
        public static Gcx.Procedure ParseProc(byte[] bytes)
        {
            int index = 0;
            int nestedLevel = 0; //how important is this?
            do
            {
                byte highByte = (byte)(bytes[index] & 0xF0);
                //if (SubprocDeclaration.Contains(bytes[index]))
                if (highByte == 0x80)
                {
                    //Going into nested subproc
                    nestedLevel++; //i think this is unimportant
                    //int size = bytes[index] == 0x8D ? bytes[index + 1] : BitConverter.ToInt16(bytes, index + 1); 
                    int size = ParseSize(bytes.Take(new Range(new Index(index), new Index(index + 3))).ToArray(), ref index); //TODO: verify
                    byte[] procContents = new byte[size];
                    Array.Copy(bytes, index, procContents, 0, size);
                    Gcx.Procedure subProcedure = ParseProc(procContents);
                    index += size;
                }
                //if (CommandDeclaration.Contains(bytes[index]))
                else if (highByte == 0x60)
                {
                    //going into nested command
                    nestedLevel++;
                    //int size = bytes[index] == 0x6D ? bytes[index + 1] : BitConverter.ToInt16(bytes, index + 1);
                    //int size = ParseSize(bytes.Take(new Range(new Index(index), new Index(index + 3))).ToArray(), ref index); //TODO: verify
                    //Gcx.Command parsedCommand = ParseCommand(bytes.Take(size).ToArray());
                    int size = ParseSize(bytes.Take(new Range(new Index(index), new Index(index + 3))).ToArray(), ref index); //TODO: verify
                    byte[] cmdContents = new byte[size];
                    Array.Copy(bytes, index, cmdContents, 0, size);
                    Gcx.Command command = ParseCommand(cmdContents);
                    index += size;
                }
                else
                {
                    return null;
                    //if (bytes[index] == 0x0)
                        index++;
                }
            } while (index < bytes.Length);

            return null;
        }

        private static int ParseSize(byte[] bytes, ref int index)
        {
            byte lowByte = (byte)(bytes[0] & 0x0F);

            if(lowByte < 0xD)
            {
                index++;
                return lowByte;
            }
            else
            {
                if(lowByte == 0xD)
                {
                    index += 2;
                    return bytes[1];
                }
                else
                {
                    index += 3;
                    return BitConverter.ToInt16(bytes, 1);
                }
            }
        }

        private static Gcx.Command ParseCommand(byte[] bytes)
        {
            int startType = 0;
            int endType = 3;
            /*if(bytes.Length > 0xFF)
            {
                startType++;
                endType++;
            }*/
            byte[] knownCommandType = bytes.Take(new Range(new Index(startType), new Index(endType))).ToArray();
            string commandTypeInHex = BitConverter.ToString(knownCommandType.Reverse().ToArray()).Replace("-", "");

            switch (commandTypeInHex)
            {
                case "6592A7":
                    //chara
                    //initial testing with w01a and w22a passed(in that "parsing" those files did not crash xdd)
                    Chara chara = new Chara();
                    chara.Size = (ushort) (bytes.Length - 2);
                    int charaArgsLength = bytes[3]; //TODO: is this ALWAYS true? i think so, but idk for sure
                    byte[] charaArgs = bytes.Take(new Range(new Index(4), new Index(4 + charaArgsLength))).ToArray();
                    int position = 0;
                    do
                    {
                        byte typeLength = charaArgs[position];
                        if((typeLength & 0xF0) != 0x40)
                        {
                            Gcx.Gcx.DataType dataType = Gcx.Gcx.DataType.FromCode(typeLength);
                            chara.Args.Add(charaArgs.Take(new Range(new Index(position + 1), new Index(position + 1 + dataType.Length))).ToArray());
                            position += dataType.Length + 1;
                        }
                        else //arg, single byte
                        {
                            chara.Args.Add(new byte[] { charaArgs[position] });
                            position++;
                        }
                    } while(position < charaArgsLength);

                    return chara;
                case "3822C7":
                    //mesg
                    Msg msg = new Msg();
                    break;
                case "3BD490":
                    //trap
                    Trap trap = new Trap();
                    break;
                case "082BC9":
                    //generic command
                    GameCommand gameCommand = new GameCommand();
                    break;
                case "37C884":
                    //load
                    Load load = new Load();
                    break;
                case "01C090":
                    //map
                    Map map = new Map();
                    break;
                case "6BB005":
                    //restart
                    Restart restart = new Restart();
                    break;
                case "8B3DF5":
                    //unknown command
                    UnknownCommand unknownCommand = new UnknownCommand();
                    break;
                case "000D86":
                    IfBlock ifblock = new IfBlock();
                    break;
                case "A65DB5":
                    SwitchBlock switchBlock = new SwitchBlock();
                    break;
                case "34648C":
                    Evaluate evaluateStatement = new Evaluate();
                    break;
                case "3311EC":
                    Invoke invokeStatement = new Invoke();
                    break;
                case "8BE398":
                    Return returnStatement = new Return();
                    break;
                case "3AB23B":
                    Print printStatement = new Print();
                    break;
                default:
                    throw new NotImplementedException("Unrecognized command type");
            }

            return null;
        }

        private static Gcx.Statement ParseStatement(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
