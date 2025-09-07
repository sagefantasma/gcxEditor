using Gcx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Expression = Gcx.Expression;
using Parameter = Gcx.Parameter;

namespace GcxEditor
{
    public static class ProcParser
    {
        private static byte[] TakeRange(byte[] bytes, uint startingIndex, uint endingIndex)
        {
            var test = bytes.Take(new Range(new Index((int)startingIndex), new Index((int)endingIndex)));
            if(startingIndex != 0)
            {

            }
            byte[] subArray = new byte[endingIndex - startingIndex];
            for(uint i = startingIndex; i < endingIndex; i++)
            {
                try
                {
                    subArray[i-startingIndex] = bytes[i];
                }
                catch(IndexOutOfRangeException e)
                {
                    //squelch this error because i'm lazy and it works :)
                }
            }

            return subArray;
        }

        public static Gcx.Procedure ParseProc(byte[] bytes)
        {
            try
            {
                uint index = 0;
                int nestedLevel = 0; //how important is this?
                Gcx.Procedure procedure = new Procedure();
                procedure.DecodedContents = new List<dynamic>();
                if (bytes.Length == 0)
                {
                    return procedure;
                }

                do
                {
                    byte highByte = (byte)(bytes[index] & 0xF0);
                    uint size = 0;

                    switch (highByte)
                    {
                        case 0x80:
                            //Going into nested subproc
                            nestedLevel++; //i think this is unimportant
                            size = ParseSize(TakeRange(bytes, index, index + 3), ref index);
                            if (size < 0xD)
                                size--;
                            byte[] procContents = new byte[size];
                            procContents = TakeRange(bytes, index, index + size);
                            Gcx.Procedure subProcedure = ParseProc(procContents);
                            if (subProcedure != null)
                                procedure.DecodedContents.Add(subProcedure);
                            index += size;
                            break;
                        case 0x70:
                            //going into invoke
                            size = ParseSize(TakeRange(bytes, index, index + 3), ref index);
                            byte[] invokeContents = new byte[size];
                            invokeContents = TakeRange(bytes, index, index + size);
                            Invoke invoke = ParseInvoke(invokeContents);
                            if (invoke != null)
                                procedure.DecodedContents.Add(invoke);
                            index += size;
                            break;
                        case 0x60:
                            //going into command
                            nestedLevel++;
                            size = ParseSize(TakeRange(bytes, index, index + 3), ref index);
                            byte[] cmdContents = new byte[size];
                            cmdContents = TakeRange(bytes, index, index + size);
                            IProcedureElement command = ParseCommand(cmdContents);
                            if (command != null)
                                procedure.DecodedContents.Add(command);
                            index += size;
                            break;
                        case 0x30:
                            //expression
                            size = ParseSize(TakeRange(bytes, index, index + 3), ref index);
                            byte[] expressionContents = new byte[size];
                            expressionContents = TakeRange(bytes, index, index + size);
                            Gcx.Expression expression = ParseExpression(expressionContents);
                            if (expression != null)
                                procedure.DecodedContents.Add(expression);
                            index += size;
                            break;
                        case 0x0:
                            index++;
                            break;
                        default:
                            throw new InvalidDataException($"{highByte} at {index} is an invalid declaration.");
                    }
                } while (index < bytes.Length);

                return procedure;
            }
            catch(Exception e)
            {
                throw new ParserException($"Failed to parse procedure: {e}");
            }
        }

        private static uint ParseSize(byte[] bytes, ref uint index)
        {
            try
            {
                byte lowNibble = (byte)(bytes[0] & 0x0F);

                if (lowNibble < 0xD)
                {
                    index++;
                    return lowNibble;
                }
                else if (lowNibble == 0xD)
                {
                    index += 2;
                    return bytes[1];
                }
                else if(lowNibble == 0xE)
                {
                    index += 3;
                    return BitConverter.ToUInt16(bytes, 1);
                }
                else
                {
                    index += 4;
                    byte[] u24 = new byte[4];
                    Array.Copy(bytes, 1, u24, 0, 3);
                    //Theoretically, I could follow the same pattern used for the u16 sizing, but would require
                    //sending a larger byte-array for all requests.
                    //That said, this in itself is already untested, as no standard gcx files have a u24 size anywhere.
                    return BitConverter.ToUInt32(u24); 
                }
            }
            catch(Exception e)
            {
                throw new ParserException($"Something unexpected went wrong when parsing a size from byte array of [{BitConverter.ToString(bytes).Replace("-", "")}]: {e}");
            }
        }

        private static Gcx.Expression ParseNestedExpression(Argument term1, Argument term2, Gcx.Gcx.Operation operation)
        {
            try
            {
                Gcx.Expression expression = new Gcx.Expression();
                expression.Term1 = term1;
                expression.Term2 = term2;
                expression.Operator = operation;
                expression.Size = (ushort)(term1.Size + term2.Size + 1);

                return expression;
            }
            catch(Exception e)
            {
                throw new ParserException ($"Failed to parse nested expression: {e}");
            }
        }

        private static Gcx.Expression ParseExpression(byte[] bytes)
        {
            try
            {
                Gcx.Expression expression = new Gcx.Expression();
                /*3A 19 00 0B E8 01 26 F7 B6 A0 == $var:varbuf_0xBE8 = 0xF726
                //3A is denoting expression, A bytes long
                //19 is single variable, not sure what lowbyte signifies. maybe long?
                //00 is varbuf
                //0B E8 is variable being modified
                //01 is denoting short (describing value set?)
                //26 F7 is value being set
                //B6 is set equal
                //A0 is end
                */

                /*3A 19 00 0C 88 19 00 0C 7C B6 A0 == $var:varbuf_0xC88 = $var:varbuf_0xC7C
                //3A is denoting expression, A bytes long
                //19 is single variable, not sure what lowbyte signifies. maybe long?
                //00 is varbuf
                //0C 88 is variable being modified
                //19 is single variable, not sure what lowbyte signifies. maybe long?
                //00 is varbuf
                //0C 7C is variable being compared
                //B6 is set equal
                //A0 is end
                */

                /*39 19 00 0C 88 01 5E 1A AF A0 == $var:varbuf_0xC88 > 0x1A5E
                //39 is expression, 9 bytes long
                //19 is single variable, not sure what lowbyte signifies. maybe long?
                //00 is varbuf
                //0C88 is variable being modified
                //01 is ?? (maybe denoting short?)
                //5E 1A is value being set
                //AF is greater than
                //A0 is end
                */

                /* 35 41 02 80 AD A0 == $arg1 < 0x80 
                //35 is expression, 5 bytes long
                //41 is arg1
                //02 is ?? (maybe denoting byte?)
                //80 is value being set
                //AD is less than [13 - A0 as base]
                //A0 is end
                */

                /* 37 14 06 0A 7D C2 B6 A0 == $varbuf:varbuf_0xA7D = 1
                //37 is expression, 7 bytes long
                //14 is single variable, not sure what lowbyte signifies. maybe byte? (is 4 saying C1 literal?)
                //06 is designating a strcode?
                //0A 7D is variable being modified
                //C2 is 1 literal
                //B6 is set equal 
                //A0 is end
                */

                /* 37 19 00 0B 9C 42 B6 A0 == $var:varbuf_0xB9C = $arg2
                //37 is expression, 7 bytes long
                //19 is single variable, not sure what lowbyte signifies. maybe long?
                //00 is varbuf
                //0B 9C is variable being modified
                //42 is arg2
                //B6 is set equal
                //A0 is end
                */

                /* 39 11 80 15 8A 01 00 04 B2 A0 == $var:linkvarbuf_0x158A & 0x400
                //39 is expression, 9 bytes long
                //11 is single variable, not sure what lowbyte signifies. maybe short?
                //80 is linkvarbuf
                //15 8A is variable being modified
                //01 is denoting short (describing value being set?)
                //00 04 is value being set
                //B2 is & operator
                //A0 is end
                */

                byte[] argBytes = bytes.Take(bytes.Length).ToArray();
                List<Argument> args = ParseArgs(argBytes); //seems to work well enough?
                if (args.Count > 1)
                {
                    expression.Term1 = args[0];
                    expression.Term2 = args[1];
                }
                else
                {
                    expression.Term1 = null;
                    expression.Term2 = args[0];
                }
                
                expression.Operator = ParseOperator(bytes.Last(x => x != 0x00));
                expression.Size = (ushort)bytes.Length;
                return expression;
            }
            catch(Exception e)
            {
                throw new ParserException($"Failed to parse gcx expression: {e}");
            }
        }

        private static Gcx.Gcx.Operation ParseOperator(byte operatorByte)
        {
            switch (operatorByte)
            {
                default:
                    throw new ParserException("Invalid expression operator provided");
                case 0xA0:
                    return Gcx.Gcx.Operation.NoOp;
                case 0xA1:
                    return Gcx.Gcx.Operation.NegateValue2;
                case 0xA2:
                    return Gcx.Gcx.Operation.Value2Equals0;
                case 0xA3:
                    return Gcx.Gcx.Operation.BitwiseComplementOfValue2;
                case 0xA4:
                    return Gcx.Gcx.Operation.Value1PlusValue2;
                case 0xA5:
                    return Gcx.Gcx.Operation.Value1MinusValue2;
                case 0xA6:
                    return Gcx.Gcx.Operation.Value1MulitpliedByValue2;
                case 0xA7:
                    return Gcx.Gcx.Operation.Value1DividedByValue2;
                case 0xA8:
                    return Gcx.Gcx.Operation.Value1ModuloValue2;
                case 0xA9:
                    return Gcx.Gcx.Operation.Value1LeftShiftValue2;
                case 0xAA:
                    return Gcx.Gcx.Operation.Value1RightShiftValue2;
                case 0xAB:
                    return Gcx.Gcx.Operation.Value1IsEqualToValue2;
                case 0xAC:
                    return Gcx.Gcx.Operation.Value1NotEqualToValue2;
                case 0xAD:
                    return Gcx.Gcx.Operation.Value1LessThanValue2;
                case 0xAE:
                    return Gcx.Gcx.Operation.Value1LessThanOrEqualToValue2;
                case 0xAF:
                    return Gcx.Gcx.Operation.Value1GreaterThanValue2;
                case 0xB0:
                    return Gcx.Gcx.Operation.Value1GreaterThanOrEqualToValue2;
                case 0xB1:
                    return Gcx.Gcx.Operation.Value1BitwiseOrValue2;
                case 0xB2:
                    return Gcx.Gcx.Operation.Value1BitwiseAndValue2;
                case 0xB3:
                    return Gcx.Gcx.Operation.Value1BitwiseXorValue2;
                case 0xB4:
                    return Gcx.Gcx.Operation.Value1OrValue2;
                case 0xB5:
                    return Gcx.Gcx.Operation.Value1AndValue2;
                case 0xB6:
                    return Gcx.Gcx.Operation.Value1SetToValue2;
                case 0xB7:
                    return Gcx.Gcx.Operation.Value2;
            }
        }

        private static Invoke ParseInvoke(byte[] bytes)
        {
            try
            {
                Invoke invoke = new Invoke();
                byte[] procedureName = new byte[4]; //TODO: confirm if this is 100% always the case. i havent SEEN a 4byte proc name, but i won't say its impossible.
                Array.Copy(bytes.Take(3).ToArray(), procedureName, 3);
                invoke.ProcedureInvoked = new Procedure { Name = BitConverter.ToUInt32(procedureName).ToString() };
                invoke.Args = ParseArgs(bytes.Take(new Range(new Index(3), new Index(bytes.Length))).ToArray());
                return invoke;
            }
            catch(Exception e)
            {
                throw new ParserException($"Failed to parse invoke: {e}");
            }
        }

        private static List<Argument> ParseVarArrayArgs(byte[] bytes, out uint varArraySize)
        {
            //NOTE: for some reason, a varbuf used inside a vararray is always done as an expression... i dont understand why.

            //Okay, so i think the max size of the array is always a literal... but theoretically there's nothing stopping it from being a varbuf reference.
            //I think the best way to do it will be this:
            //If byte >= 0xC0: literal
            //If byte & 0xF0 == 0x40: arg
            //else, expression?
            try
            {
                uint position = 0;
                List<Argument> args = new List<Argument>();
                while (position < bytes.Length)
                {
                    if (args.Count == 2)
                    {
                        varArraySize = position;
                        break;
                    }
                    int highNibble = bytes[position] & 0xF0;

                    if (highNibble >= 0xC0)
                    {
                        args.Add(new Argument { Value = new Constant { Size = 1, Value = (byte)(bytes[position++] - 0xC1) } });
                    }
                    else if (highNibble == 0x40)
                    {
                        args.Add(new Argument { Value = new PassedArg { ArgNum = bytes[position++] } });
                    }
                    else
                    {
                        byte[] expressionSizeBytes = TakeRange(bytes, position, position + 4);
                        uint size = ParseSize(expressionSizeBytes, ref position);

                        byte[] expressionBytes = TakeRange(bytes, position, position + size);
                        Gcx.Expression expression = ParseExpression(expressionBytes);
                        args.Add(new Argument { Value = expression });
                        position += expression.Size;
                    }
                }

                varArraySize = position;
                return args;
            }
            catch(Exception e)
            {
                throw new ParserException($"Failed to parse var array args: {e}");
            }
        }

        private static List<Argument> ParseArgs(byte[] bytes)
        {
            uint position = 0;
            List<Argument> args = new List<Argument>();
            if(bytes.Length == 0)
            {
                return args;
            }
            try
            {
                do
                {
                    byte currentByte = bytes[position];
                    int highNibble = currentByte & 0xF0;
                    //if second byte == 00, then just varbuf. if == 10, then localvarbuf. if == 80, then linkvarbuf
                    //20 means array, 10 means single?
                    switch (highNibble)
                    {
                        case 0xF0:
                        case 0xE0:
                        case 0xD0:
                        case 0xC0:
                            //basic number
                            try
                            {
                                args.Add(new Argument { Value = new Constant { Size = 1, Value = (byte)(bytes[position] - 0xC1) } });
                                position++;
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                            break;

                        case 0xB0:
                        case 0xA0:
                            if (position != bytes.Length - 1)
                            {
                                try
                                {
                                    //nested expression x_x;;

                                    Gcx.Expression expression = ParseNestedExpression(args[args.Count - 2], args[args.Count - 1], ParseOperator(currentByte));
                                    args.RemoveAt(args.Count - 1);
                                    args.RemoveAt(args.Count - 1);
                                    args.Add(new Argument { Value = expression, Size = expression.Size });

                                    position++;
                                }
                                catch (Exception e)
                                {
                                    throw e;
                                }
                            }
                            else
                            {
                                position++; //this *should* be just the end of the whole expression that got started, so a break might do, but im nervous so sticking with pos++
                            }
                            break;

                        case 0x90:
                            try
                            {
                                args.Add(new Argument { Value = new LocalVar { Id = (ushort)(currentByte & 0x0F) } });
                                position++;
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                            break;

                        case 0x80:
                            try
                            {
                                //nested proc
                                byte[] nestedProcBytes = TakeRange(bytes, position, (uint)(bytes.Length - 1));
                                uint size = ParseSize(nestedProcBytes, ref position);
                                if (size < 0xD)
                                {
                                    size--;
                                }
                                nestedProcBytes = TakeRange(bytes, position, size + position);
                                //TODO: i'm *pretty sure* this will cause issues if the nested proc is not the final parameter.
                                Gcx.Procedure procedure = ParseProc(nestedProcBytes);
                                args.Add(new Argument { Value = procedure, Size = procedure.Size });
                                if (size < 0xC)
                                {
                                    if (size == 0)
                                    {
                                        position--;
                                    }
                                    size++;
                                }
                                position += size;
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                            break;

                        case 0x40:
                            try
                            {
                                if (currentByte < 0x4F)
                                {
                                    args.Add(new Argument { Value = new PassedArg { ArgNum = bytes[position] }, Size = 1 });
                                    position++;
                                }
                                else// if(position != bytes.Length - 1)
                                {
                                    //looks like 0x4F will be followed by a 0 if it is 15
                                    int argNum = 0xF;
                                    argNum += bytes[position + 1];
                                    args.Add(new Argument { Value = new PassedArg { ArgNum = (byte)argNum }, Size = 2 });
                                    position += 2;
                                }
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                            break;

                        case 0x30:
                            try
                            {
                                byte[] expressionSizeBytes = TakeRange(bytes, position, position + 4);
                                uint size = ParseSize(expressionSizeBytes, ref position);
                                byte[] expressionBytes = TakeRange(bytes, position, size + position);
                                Gcx.Expression expression = ParseExpression(expressionBytes);
                                args.Add(new Argument { Value = expression, Size = expression.Size });
                                position += size;
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                            break;

                        case 0x20:
                            try
                            {
                                //var array, still needs work
                                VariableArray variableArray = new VariableArray();
                                variableArray.LowNibble = (byte)(bytes[position++] & 0x0F);
                                variableArray.ArrayType = bytes[position++];
                                byte[] id = TakeRange(bytes, position, position += 2);
                                variableArray.Id = BitConverter.ToUInt16(id.Reverse().ToArray());
                                //I *think* lownibble may be indicating var size? maybe?
                                variableArray.SizeAndIndex = ParseVarArrayArgs(TakeRange(bytes, position, (uint)bytes.Length), out uint varArraySize);
                                /*if (bytes[position] > 0xC0)
                                {
                                    //literal
                                    variableArray.Size = (ushort)bytes[position++];
                                }
                                else
                                {
                                    Gcx.Gcx.DataType dataType = Gcx.Gcx.DataType.FromCode(bytes[position++]);
                                    byte[] dataValue = new byte[4];
                                    Array.Copy(bytes.Take(new Range(new Index(position), new Index(position += dataType.Length))).ToArray(), dataValue, dataType.Length);
                                    variableArray.Size = BitConverter.ToUInt32(dataValue.Reverse().ToArray()); //TODO: confirm this should be reversed or not
                                }
                                    variableArray.Size = (ushort)bytes[4];
                                variableArray.Index = (ushort)bytes[5];*/
                                //21 80 03 3C F1 DE C1 AB




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

                                args.Add(new Argument { Value = variableArray });
                                position += varArraySize; //TODO: confirm it is always this
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                            break;

                        case 0x10:
                            try
                            {
                                //single variable
                                Variable variable = new Variable();
                                variable.LowNibble = (byte)(bytes[position] & 0x0F);
                                byte[] id = bytes.Take(new Range(new Index(2), new Index(4))).ToArray(); //this is fucked
                                variable.Id = BitConverter.ToUInt16(id.Reverse().ToArray());

                                args.Add(new Argument { Value = variable, Size = 3 }); //TODO: confirm always 3
                                position += 4;
                            }
                            catch (Exception e)
                            {
                                throw e;
                            } //i believe this should be fixed now
                            break;

                        case 0x0:
                            //empty value, ignore
                            position++;
                            break;

                        default:
                            try
                            {
                                Gcx.Gcx.DataType dataType = Gcx.Gcx.DataType.FromCode(currentByte);
                                
                                byte[] dataValue = new byte[4];
                                if (dataType == Gcx.Gcx.DataType.String)
                                {
                                    dataType.Length = bytes[position + 1];
                                    dataValue = new byte[dataType.Length];
                                    Array.Copy(bytes, position + 2, dataValue, 0, dataType.Length);
                                    args.Add(new Argument { Value = new Literal { Value = dataValue }, Size = (ushort)dataType.Length });
                                }
                                else
                                {
                                    Array.Copy(bytes, position + 1, dataValue, 0, dataType.Length);
                                    args.Add(new Argument { Value = new Literal { Value = BitConverter.ToUInt32(dataValue) }, Size = (ushort)dataType.Length, });
                                }
                                position += (uint)(dataType.Length + 1);
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                            break;
                    }
                } while (position < bytes.Length);

                return args;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private static List<Parameter> ParseParams(byte[] bytes)
        {
            try
            {
                List<Parameter> parameters = new List<Parameter>(); //TODO: verify

                uint position = 0;
                while (position < bytes.Length)
                {
                    int startOfParameterDeclaration = bytes[position];
                    if (startOfParameterDeclaration == 0)
                    {
                        position++;
                        continue;
                    }
                    byte lowNibble = (byte)(startOfParameterDeclaration & 0x0F);
                    //int size = ParseSize(bytes.Take(new Range(new Index(position), new Index(bytes.Length))).ToArray(), ref position);
                    uint size = ParseSize(TakeRange(bytes, position, (uint)bytes.Length), ref position);

                    Parameter parameter = new Parameter();
                    parameter.ParamType = (char)bytes[position];
                    position++;
                    //parameter.Contents = bytes.Take(new Range(new Index(position), new Index(position + size - 1))).ToArray();
                    parameter.Contents = TakeRange(bytes, position, position + size - 1);
                    if ((int)parameter.ParamType == 0x41)
                    {

                    }
                    parameter.Args = ParseArgs(parameter.Contents);
                    position += (uint)parameter.Contents.Length;
                    parameters.Add(parameter);
                }

                return parameters;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private static IProcedureElement ParseCommand(byte[] bytes)
        {
            try
            {
                int startType = 0;
                int endType = 3;
                int position = 0;
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
                        chara.Size = (ushort)(bytes.Length - 2);
                        position = 3;
                        uint charaArgsLength = ParseArgsLength(bytes, ref position);
                        byte[] charaArgs = bytes.Take(new Range(new Index(position), new Index((int) (position + charaArgsLength)))).ToArray();
                        chara.Args = ParseArgs(charaArgs);

                        byte[] charaParams = bytes.Take(new Range(new Index((int) (position + charaArgsLength)), new Index(bytes.Length))).ToArray();
                        chara.Parameters = ParseParams(charaParams);

                        return chara;
                    case "3822C7": //passed w01a
                                   //mesg
                        Msg msg = new Msg();
                        msg.Size = (ushort)(bytes.Length - 2); //TODO: where did i get this from? this doesn't make sense
                        position = 3;
                        uint messageArgsLength = ParseArgsLength(bytes, ref position);
                        byte[] mesgArgs = bytes.Take(new Range(new Index(position), new Index((int)(position + messageArgsLength)))).ToArray();
                        msg.Args = ParseArgs(mesgArgs);

                        return msg;
                    case "3BD490": //passed w01a
                                   //trap
                        Trap trap = new Trap();
                        trap.Size = (ushort)(bytes.Length - 2);
                        position = 3;
                        uint trapArgsLength = ParseArgsLength(bytes, ref position);
                        byte[] trapArgs = bytes.Take(new Range(new Index(position), new Index((int)(position + trapArgsLength)))).ToArray();
                        trap.Args = ParseArgs(trapArgs);

                        byte[] trapParams = bytes.Take(new Range(new Index((int)(position + trapArgsLength)), new Index(bytes.Length))).ToArray();
                        trap.Parameters = ParseParams(trapParams);

                        return trap;
                    case "082BC9": //passed w01a
                                   //generic command
                        GameCommand gameCommand = new GameCommand();
                        gameCommand.Size = (ushort)(bytes.Length - 2);
                        position = 3;
                        uint gameCommandArgsLength = ParseArgsLength(bytes, ref position);
                        byte[] gameCommandArgs = bytes.Take(new Range(new Index(position), new Index((int)(position + gameCommandArgsLength)))).ToArray();
                        gameCommand.Args = ParseArgs(gameCommandArgs);

                        byte[] gameCommandParams = bytes.Take(new Range(new Index((int)(position + gameCommandArgsLength)), new Index(bytes.Length))).ToArray();
                        gameCommand.Parameters = ParseParams(gameCommandParams);
                        return gameCommand;
                    case "37C884": //passed w01a
                                   //load
                        Load load = new Load();
                        position = 3;
                        load.Size = ParseArgsLength(bytes, ref position); //TODO: fix these size declarations, these are wrong. this is depicting the size of the args, not the whole command
                        byte[] loadArgs = bytes.Take(new Range(new Index(position), new Index((int)(position + load.Size)))).ToArray();
                        load.Args = ParseArgs(loadArgs);

                        return load;
                    case "01C090":
                        //map
                        Map map = new Map();
                        //used anywhere?
                        return map;
                    case "6BB005":
                        //restart
                        Restart restart = new Restart();
                        position = 3;
                        restart.Size = ParseArgsLength(bytes, ref position);
                        byte[] restartArgs = bytes.Take(new Range(new Index(position), new Index((int)(position + restart.Size)))).ToArray();
                        restart.Args = ParseArgs(restartArgs);
                        //def used
                        return restart;
                    case "8B3DF5": //passed w01a
                        //unknown command
                        UnknownCommand unknownCommand = new UnknownCommand();
                        position = 3;
                        unknownCommand.Size = ParseArgsLength(bytes, ref position);
                        byte[] unknownCommandArgs = bytes.Take(new Range(new Index(position), new Index((int)(position + unknownCommand.Size)))).ToArray();
                        unknownCommand.Args = ParseArgs(unknownCommandArgs);
                        return unknownCommand;
                    case "000D86":
                        IfBlock ifblock = new IfBlock();
                        //byte after is length of if block?
                        position = 3;
                        ifblock.Size = ParseArgsLength(bytes, ref position);
                        //is the size for ifs different than other elements? why are there these dead bytes in some of the if blocks?
                        //holy shit, i think it is lmaooo
                        byte[] ifBlockArgs = bytes.Take(new Range(new Index(position), new Index((int)(position + ifblock.Size)))).ToArray();
                        ifblock.Args = ParseArgs(ifBlockArgs);
                        //args are the main if

                        byte[] ifParams = bytes.Take(new Range(new Index((int)(position + ifblock.Size)), new Index(bytes.Length))).ToArray();
                        //i param is elif, e param is else?
                        ifblock.Parameters = ParseParams(ifParams);
                        //def used
                        return ifblock;
                    case "A65DB5":
                        //TODO: does this share the same weird size pattern as ifs?
                        SwitchBlock switchBlock = new SwitchBlock();
                        position = 3;
                        switchBlock.Size = ParseArgsLength(bytes, ref position);
                        byte[] switchArgs = bytes.Take(new Range(new Index(position), new Index((int)(position + switchBlock.Size)))).ToArray();
                        switchBlock.Args = ParseArgs(switchArgs);

                        byte[] switchParams = bytes.Take(new Range(new Index((int)(position + switchBlock.Size)), new Index(bytes.Length))).ToArray();
                        switchBlock.Parameters = ParseParams(switchParams);

                        //def used
                        return switchBlock;
                    case "34648C":
                        Evaluate evaluateStatement = new Evaluate();
                        //used anywhere?
                        return evaluateStatement;
                    case "3311EC":
                        Invoke invokeStatement = new Invoke();
                        //used anywhere?
                        return invokeStatement;
                    case "8BE398": //passed w01a
                        Return returnStatement = new Return();
                        position = 3;
                        returnStatement.Size = ParseArgsLength(bytes, ref position);
                        byte[] returnArgs = bytes.Take(new Range(new Index(position), new Index((int)(position + returnStatement.Size)))).ToArray();
                        returnStatement.Args = ParseArgs(returnArgs);

                        return returnStatement;
                    case "3AB23B": //passed w01a
                        Print printStatement = new Print();
                        position = 3;
                        printStatement.Size = ParseArgsLength(bytes, ref position);
                        byte[] printArgs = bytes.Take(new Range(new Index(position), new Index((int)(position + printStatement.Size)))).ToArray();
                        printStatement.Args = ParseArgs(printArgs);

                        return printStatement;
                    default:
                        throw new NotImplementedException("Unrecognized command type");
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private static uint ParseArgsLength(byte[] bytes, ref int position)
        {
            uint size = bytes[position++];
            if (size >= 0x80)
            {
                byte highNibble = (byte)(size & 0x0F);
                byte lowByte = (byte)(bytes[position++]);
                byte[] sizeBytes = new byte[4] { lowByte, highNibble, 0, 0 };
                size = BitConverter.ToUInt32(sizeBytes);
            }

            return size;
        }

        private static Gcx.Statement ParseStatement(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
