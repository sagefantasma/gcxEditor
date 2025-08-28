using Gcx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GcxEditor
{
    public static class ProcParser
    {
        public static Gcx.Procedure ParseProc(byte[] bytes)
        {
            int index = 0;
            int nestedLevel = 0; //how important is this?
            Gcx.Procedure procedure = new Procedure();
            procedure.DecodedContents = new List<dynamic>();
            do
            {
                byte highByte = (byte)(bytes[index] & 0xF0);
                
                if(highByte == 0xC0)
                {
                    //this seems to not be a real case
                    //going into num
                }
                else if (highByte == 0x90)
                {
                    //this seems to not be a real case
                    //local
                }
                else if (highByte == 0x80)
                {
                    //Going into nested subproc
                    nestedLevel++; //i think this is unimportant
                    int size = ParseSize(bytes.Take(new Range(new Index(index), new Index(index + 3))).ToArray(), ref index); //TODO: verify
                    byte[] procContents = new byte[size];
                    Array.Copy(bytes, index, procContents, 0, size);
                    Gcx.Procedure subProcedure = ParseProc(procContents);
                    if(subProcedure != null)
                        procedure.DecodedContents.Add(subProcedure);
                    index += size;
                }
                else if (highByte == 0x70)
                {
                    //going into invoke
                    int size = ParseSize(bytes.Take(new Range(new Index(index), new Index(index + 3))).ToArray(), ref index);
                    byte[] invokeContents = new byte[size];
                    Array.Copy(bytes, index, invokeContents, 0, size);
                    Invoke invoke = ParseInvoke(invokeContents);
                    if (invoke != null)
                        procedure.DecodedContents.Add(invoke);
                    index += size;
                }
                //if (CommandDeclaration.Contains(bytes[index]))
                else if (highByte == 0x60)
                {
                    //going into command
                    nestedLevel++;
                    int size = ParseSize(bytes.Take(new Range(new Index(index), new Index(index + 3))).ToArray(), ref index); //TODO: verify
                    byte[] cmdContents = new byte[size];
                    Array.Copy(bytes, index, cmdContents, 0, size);
                    Gcx.Command command = ParseCommand(cmdContents);
                    if(command != null)
                        procedure.DecodedContents.Add(command);
                    index += size;
                }
                else if (highByte == 0x50)
                {
                    //this seems to not be a real case
                    //param
                }
                else if (highByte == 0x40)
                {
                    //this seems to not be a real case
                    //args
                }
                else if (highByte == 0x30)
                {
                    //expression
                    int size = ParseSize(bytes.Take(new Range(new Index(index), new Index(index + 3))).ToArray(), ref index);
                    byte[] expressionContents = new byte[size];
                    Array.Copy(bytes, index, expressionContents, 0, size);
                    Expression expression = ParseExpression(expressionContents);
                    if (expression != null)
                        procedure.DecodedContents.Add(expression);
                    index += size;
                }
                else if (highByte == 0x20)
                {
                    //this seems to not be a real case
                    //var array
                }
                else if (highByte == 0x10)
                {
                    //this seems to not be a real case
                    //var
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

        private static Gcx.Expression ParseExpression(byte[] bytes)
        {
            //TODO: finish implementation
            Expression expression = new Expression();

            return expression;
        }

        private static Invoke ParseInvoke(byte[] bytes)
        {
            Invoke invoke = new Invoke();
            byte[] procedureName = new byte[4]; //TODO: confirm if this is 100% always the case. i havent SEEN a 4byte proc name, but i won't say its impossible.
            Array.Copy(bytes.Take(3).ToArray(), procedureName, 3);
            invoke.ProcedureInvoked = new Procedure { Name = BitConverter.ToUInt32(procedureName).ToString() };
            invoke.Args = ParseArgs(bytes.Take(new Range(new Index(3), new Index(bytes.Length))).ToArray());
            return invoke;
        }

        private static List<Argument> ParseArgs(byte[] bytes)
        {
            int position = 0;
            List<Argument> args = new List<Argument>();
            try
            {
                do
                {
                    byte typeLength = bytes[position];
                    //if second byte == 00, then just varbuf. if == 10, then localvarbuf. if == 80, then linkvarbuf
                    //20 means array, 10 means single?
                    if ((bytes[position] & 0xF0) == 0x20)
                    {
                        //var array, still needs work - is not accurate
                        VariableArray variableArray = new VariableArray();
                        variableArray.Size = (ushort)bytes[4];
                        variableArray.LowNibble = (byte)(bytes[position] & 0x0F);
                        variableArray.Index = (ushort)bytes[6];
                        byte[] id = bytes.Take(new Range(new Index(2), new Index(4))).ToArray();
                        variableArray.Id = BitConverter.ToUInt16(id.Reverse().ToArray());
                        //22 00 04 B4 C9 32 41 A0 == $var:varbuf_0x4B4[$arg1,8]
                        //my thinking: 22 is array, 00 is varbuf, 04 B4 is ID, C9 is 8, 32 is ??, 41 is arg1, A0 is ??
                        //i have no idea what the significance is of the lower nibble in 22. i tried messing with different values
                        //and got nothing changed on oct's decompiler, nor did the game crash or have any kind of hindered performance from what
                        //i could see. *surely* it isnt a totally random value, right? why is 0x4B4 always 22, but 0x494 is 29?
                        //why does the game not crash when i change them?
                        //
                        //after a little more poking around, setting 22 to anything greater(23->2F) results in no changed behavior.
                        //however, setting 22 to 21 or 20 results in the locker states getting reset entirely on load. (w01a behavior)
                        //i'm thinking then that the lower nibble might determine how many bits to track or something for each index of the array?
                        //maybe the answer lies in single variable declarations, i should poke around in those to see if there's any info to glean

                        //22 00 04 8B CA C2 00 == $var:varbuf_0x48B[2,9]
                        //22 is array, 00 is varbuf, 04 8B is ID, CA is 9, C2 is 1.
                        //does 32 indicate the previous number was a real and A0 indicated the previous number was an arg?
                        //and if neither are present, then both are reals? not sure. need to study oct's decomp more to have a better understanding i think.

                        position += 8; //TODO: confirm it is always this
                    }
                    else if ((bytes[position] & 0xF0) == 0x10)
                    {
                        //
                    }
                    else if ((typeLength & 0xF0) >= 0xC0)
                    {
                        //basic number
                        args.Add(new Argument { Bytes = new List<byte> { (byte)(bytes[position] - 0xC1) } });
                        position++;
                    }
                    else if ((typeLength & 0xF0) != 0x40 && bytes[position] != 0)
                    {
                        Gcx.Gcx.DataType dataType = Gcx.Gcx.DataType.FromCode(typeLength);
                        //args.Add(bytes.Take(new Range(new Index(position + 1), new Index(position + 1 + dataType.Length))).ToArray());
                        args.Add(new Argument { Bytes = bytes.Take(new Range(new Index(position + 1), new Index(position + 1 + dataType.Length))).ToList() });
                        position += dataType.Length + 1;
                    }
                    else if (bytes[position] != 0) //given arg, single byte
                    {
                        args.Add(new Argument { Bytes = new List<byte> { bytes[position] } });
                        position++;
                    }
                    else
                    {
                        //empty value, ignore
                        position++;
                    }
                } while (position < bytes.Length);

                return args;
            }
            catch(Exception e)
            {
                //throw e;
                return null;
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
                    chara.Args = ParseArgs(charaArgs);

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
                    //def used
                    break;
                case "01C090":
                    //map
                    Map map = new Map();
                    //used anywhere?
                    break;
                case "6BB005":
                    //restart
                    Restart restart = new Restart();
                    //def used
                    break;
                case "8B3DF5":
                    //unknown command
                    UnknownCommand unknownCommand = new UnknownCommand();
                    break;
                case "000D86":
                    IfBlock ifblock = new IfBlock();
                    //def used
                    break;
                case "A65DB5":
                    SwitchBlock switchBlock = new SwitchBlock();
                    //def used
                    break;
                case "34648C":
                    Evaluate evaluateStatement = new Evaluate();
                    //used anywhere?
                    break;
                case "3311EC":
                    Invoke invokeStatement = new Invoke();
                    //used anywhere?
                    break;
                case "8BE398":
                    Return returnStatement = new Return();
                    //def used
                    break;
                case "3AB23B":
                    Print printStatement = new Print();
                    //def used
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
