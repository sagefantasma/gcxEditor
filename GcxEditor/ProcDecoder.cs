using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Expression = GcxEditor.Expression;
using Parameter = GcxEditor.Parameter;

namespace GcxEditor
{
    public static class ProcDecoder
    {
        private static byte[] TakeRange(byte[] bytes, uint startingIndex, uint endingIndex)
        {
            byte[] subArray = new byte[endingIndex - startingIndex];
            for(uint i = startingIndex; i < endingIndex; i++)
            {
                if (bytes.Length <= i)
                    break;
                subArray[i-startingIndex] = bytes[i];
            }

            return subArray;
        }

        /// <summary>
        /// Takes in an array of bytes and decodes them into a series of MGS2 GCL-like objects.
        /// </summary>
        /// <param name="bytes">An array of bytes taken from a GCX file compatible with MGS2 that represents a procedure.</param>
        /// <returns></returns>
        /// <exception cref="ParserException"></exception>
        public static Procedure DecodeProc(byte[] bytes)
        {
            try
            {
                uint index = 0;
                Procedure procedure = new()
                {
                    DecodedContents = new List<IProcedureElement>()
                };
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
                            size = DecodeSize(TakeRange(bytes, index, index + 3), ref index);
                            if (size < 0xD)
                                size--;
                            byte[] procContents = new byte[size];
                            procContents = TakeRange(bytes, index, index + size);
                            Procedure subProcedure = DecodeProc(procContents);
                            if (subProcedure != null)
                                procedure.DecodedContents.Add(subProcedure);
                            index += size;
                            break;
                        case 0x70:
                            //going into invoke
                            size = DecodeSize(TakeRange(bytes, index, index + 3), ref index);
                            byte[] invokeContents = new byte[size];
                            invokeContents = TakeRange(bytes, index, index + size);
                            Invoke invoke = DecodeInvoke(invokeContents);
                            if (invoke != null)
                            {
                                invoke.Size = size;
                                if(size < 0xD)
                                {
                                    invoke.Size++;
                                }
                                else if(size < 0xFF)
                                {
                                    invoke.Size += 2;
                                }
                                else if(size < 0xFFFF)
                                {
                                    invoke.Size += 3;
                                }
                                else
                                {
                                    invoke.Size += 4;
                                }
                                procedure.DecodedContents.Add(invoke);   
                            }
                            index += size;
                            break;
                        case 0x60:
                            //going into command
                            size = DecodeSize(TakeRange(bytes, index, index + 3), ref index);
                            byte[] cmdContents = new byte[size];
                            cmdContents = TakeRange(bytes, index, index + size);
                            IProcedureElement command = DecodeCommand(cmdContents);
                            if (command != null)
                                procedure.DecodedContents.Add(command);
                            index += size;
                            break;
                        case 0x30:
                            //expression
                            size = DecodeSize(TakeRange(bytes, index, index + 3), ref index);
                            byte[] expressionContents = new byte[size];
                            expressionContents = TakeRange(bytes, index, index + size);
                            Expression expression = DecodeExpression(expressionContents);
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

                procedure.Size = (uint) bytes.Length;
                if (procedure.Size < 0xD)
                {
                    procedure.Size++;
                }
                else if (procedure.Size < 0xFF)
                {
                    procedure.Size += 2;
                }
                else if (procedure.Size < 0xFFFF)
                {
                    procedure.Size += 3;
                }
                else
                {
                    procedure.Size += 4;
                }
                return procedure;
            }
            catch(Exception e)
            {
                throw new ParserException($"Failed to parse procedure: {e}");
            }
        }

        private static uint DecodeSize(byte[] bytes, ref uint index)
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

        private static Expression DecodeNestedExpression(ITerm term1, ITerm term2, ExpressionElements.Operation operation)
        {
            try
            {
                Expression expression = new()
                {
                    Term1 = term1,
                    Term2 = term2,
                    Operator = operation,
                    Size = (ushort)(term1.Size + term2.Size + 1) //+1 to capture operator
                };

                return expression;
            }
            catch(Exception e)
            {
                throw new ParserException ($"Failed to parse nested expression: {e}");
            }
        }

        private static Expression DecodeExpression(byte[] bytes)
        {
            #region Expression Notes
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
            #endregion
            try
            {
                Expression expression = new()
                {
                    EncodedContents = bytes
                };
                byte[] argBytes = bytes.Take(bytes.Length).ToArray();
                List<ITerm> args = DecodeArgs(argBytes); //seems to work well enough?
                uint argsLength = 0;
                if (args.Count > 1)
                {
                    expression.Term1 = args[0];
                    argsLength += args[0].Size;
                    expression.Term2 = args[1];
                    argsLength += args[1].Size;
                }
                else
                {
                    expression.Term1 = null;
                    expression.Term2 = args[0];
                    argsLength += args[0].Size;
                }

                expression.Operator = DecodeOperator(bytes[argsLength]);
                expression.Size = (ushort)bytes.Length;
                return expression;
            }
            catch(Exception e)
            {
                throw new ParserException($"Failed to parse gcx expression from bytearray [{BitConverter.ToString(bytes).Replace("-", "")}]: {e}");
            }
        }

        private static ExpressionElements.Operation DecodeOperator(byte operatorByte)
        {
            return operatorByte switch
            {
                0xA0 => ExpressionElements.Operation.NoOp,
                0xA1 => ExpressionElements.Operation.NegateValue2,
                0xA2 => ExpressionElements.Operation.Value2Equals0,
                0xA3 => ExpressionElements.Operation.BitwiseComplementOfValue2,
                0xA4 => ExpressionElements.Operation.Value1PlusValue2,
                0xA5 => ExpressionElements.Operation.Value1MinusValue2,
                0xA6 => ExpressionElements.Operation.Value1MulitpliedByValue2,
                0xA7 => ExpressionElements.Operation.Value1DividedByValue2,
                0xA8 => ExpressionElements.Operation.Value1ModuloValue2,
                0xA9 => ExpressionElements.Operation.Value1LeftShiftValue2,
                0xAA => ExpressionElements.Operation.Value1RightShiftValue2,
                0xAB => ExpressionElements.Operation.Value1IsEqualToValue2,
                0xAC => ExpressionElements.Operation.Value1NotEqualToValue2,
                0xAD => ExpressionElements.Operation.Value1LessThanValue2,
                0xAE => ExpressionElements.Operation.Value1LessThanOrEqualToValue2,
                0xAF => ExpressionElements.Operation.Value1GreaterThanValue2,
                0xB0 => ExpressionElements.Operation.Value1GreaterThanOrEqualToValue2,
                0xB1 => ExpressionElements.Operation.Value1BitwiseOrValue2,
                0xB2 => ExpressionElements.Operation.Value1BitwiseAndValue2,
                0xB3 => ExpressionElements.Operation.Value1BitwiseXorValue2,
                0xB4 => ExpressionElements.Operation.Value1OrValue2,
                0xB5 => ExpressionElements.Operation.Value1AndValue2,
                0xB6 => ExpressionElements.Operation.Value1SetToValue2,
                0xB7 => ExpressionElements.Operation.Value2,
                _ => throw new ParserException("Invalid expression operator provided"),
            };
        }

        private static Invoke DecodeInvoke(byte[] bytes)
        {
            try
            {
                Invoke invoke = new()
                {
                    EncodedContents = bytes
                };
                byte[] procedureName = new byte[4]; //TODO: confirm if this is 100% always the case. i havent SEEN a 4byte proc name, but i won't say its impossible.
                Array.Copy(bytes.Take(3).ToArray(), procedureName, 3);
                invoke.ProcedureInvoked = new Procedure { Name = BitConverter.ToString(procedureName.Reverse().ToArray().TakeLast(3).ToArray()).Replace("-", "") };
                invoke.Args = DecodeArgs(bytes.Take(new Range(new Index(3), new Index(bytes.Length))).ToArray());
                return invoke;
            }
            catch(Exception e)
            {
                throw new ParserException($"Failed to parse invoke from byte array [{BitConverter.ToString(bytes).Replace("-", "")}]: {e}");
            }
        }

        private static List<ITerm> DecodeVarArrayArgs(byte[] bytes, out uint varArraySize)
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
                List<ITerm> args = new();
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
                        args.Add(new Constant { Size = 1, Value = (byte)(bytes[position++] - 0xC1) });
                    }
                    else if (highNibble == 0x40)
                    {
                        args.Add(new PassedArg { ArgNum = bytes[position++] });
                    }
                    else
                    {
                        byte[] expressionSizeBytes = TakeRange(bytes, position, position + 4);
                        uint size = DecodeSize(expressionSizeBytes, ref position);

                        byte[] expressionBytes = TakeRange(bytes, position, position + size);
                        Expression expression = DecodeExpression(expressionBytes);
                        args.Add(expression);
                        position += expression.Size;
                    }
                }

                varArraySize = position;
                return args;
            }
            catch(Exception e)
            {
                throw new ParserException($"Failed to parse var array args from byte array [{BitConverter.ToString(bytes).Replace("-", "")}]: {e}");
            }
        }

        private static List<ITerm> DecodeArgs(byte[] bytes)
        {
            uint position = 0;
            List<ITerm> args = new();
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
                                args.Add(new Constant { Size = 1, Value = (byte)(bytes[position] - 0xC1), EncodedContents = new[] { bytes[position] } });
                                position++;
                            }
                            catch (Exception e)
                            {
                                throw new ParserException($"Failed to parse basic number in args from byte array [{BitConverter.ToString(bytes).Replace("-", "")}] @{position}: {e}");
                            }
                            break;

                        case 0xB0:
                        case 0xA0:
                            if((position != bytes.Length - 1) && (bytes[position+1] != 0xA0))
                            {
                                try
                                {
                                    //nested expression x_x;;

                                    Expression expression = DecodeNestedExpression(args[^2], args[^1], DecodeOperator(currentByte));
                                    args.RemoveAt(args.Count - 1);
                                    args.RemoveAt(args.Count - 1);
                                    args.Add(expression);

                                    position++;
                                }
                                catch (Exception e)
                                {
                                    throw new ParserException($"Failed to parse nested expression in args from byte array [{BitConverter.ToString(bytes).Replace("-", "")}] @{position}: {e}");
                                }
                            }
                            else
                            {
                                //this *should* be just the end of the whole expression that got started, so return is safe
                                return args;
                            }
                            break;

                        case 0x90:
                            try
                            {
                                args.Add(new LocalVar { Id = (byte)(currentByte & 0x0F), EncodedContents = new[] { currentByte }, Size = 1 });
                                position++;
                            }
                            catch (Exception e)
                            {
                                throw new ParserException($"Failed to parse local variable in args from byte array [{BitConverter.ToString(bytes).Replace("-", "")}] @{position}: {e}");
                            }
                            break;

                        case 0x80:
                            try
                            {
                                //nested proc
                                byte[] nestedProcBytes = TakeRange(bytes, position, (uint)(bytes.Length));
                                uint size = DecodeSize(nestedProcBytes, ref position);
                                nestedProcBytes = TakeRange(bytes, position, size + position);
                                //TODO: i'm *pretty sure* this will cause issues if the nested proc is not the final parameter. - is this still the case?
                                Procedure procedure = DecodeProc(nestedProcBytes);
                                procedure.EncodedContents = nestedProcBytes;
                                args.Add(procedure);
                                position += size;
                            }
                            catch (Exception e)
                            {
                                throw new ParserException($"Failed to parse procedure in args from byte array [{BitConverter.ToString(bytes).Replace("-", "")}] @{position}: {e}");
                            }
                            break;

                        case 0x40:
                            try
                            {
                                if (currentByte < 0x4F)
                                {
                                    args.Add(new PassedArg { ArgNum = (byte)(bytes[position] - 0x40), EncodedContents = new[] { bytes[position] }, Size = 1 });
                                    position++;
                                }
                                else
                                {
                                    //looks like 0x4F will be followed by a 0 if it is 15
                                    int argNum = 0xF;
                                    argNum += bytes[position + 1];
                                    args.Add(new PassedArg { ArgNum = (byte)argNum, EncodedContents = TakeRange(bytes, position, position + 1), Size = 2 });
                                    position += 2;
                                }
                            }
                            catch (Exception e)
                            {
                                throw new ParserException($"Failed to parse passed arg in args from byte array [{BitConverter.ToString(bytes).Replace("-", "")}] @{position}: {e}");
                            }
                            break;

                        case 0x30:
                            try
                            {
                                byte[] expressionSizeBytes = TakeRange(bytes, position, position + 4);
                                uint size = DecodeSize(expressionSizeBytes, ref position);
                                byte[] expressionBytes = TakeRange(bytes, position, size + position);
                                Expression expression = DecodeExpression(expressionBytes);
                                args.Add(expression);
                                position += size;
                            }
                            catch (Exception e)
                            {
                                throw new ParserException($"Failed to parse expression in args from byte array [{BitConverter.ToString(bytes).Replace("-", "")}] @{position}: {e}");
                            }
                            break;

                        case 0x20:
                            try
                            {
                                //var array
                                VariableArray variableArray = new()
                                {
                                    DataTypeNibble = (ExpressionElements.DataTypeEnum)(byte)(bytes[position++] & 0x0F),
                                    ArrayType = bytes[position++]
                                };
                                byte[] id = TakeRange(bytes, position, position += 2);
                                variableArray.Id = BitConverter.ToUInt16(id.Reverse().ToArray());
                                //I *think* lownibble may be indicating var size? maybe?
                                variableArray.SizeAndIndex = DecodeVarArrayArgs(TakeRange(bytes, position, (uint)bytes.Length), out uint varArraySize);
                                variableArray.Size = varArraySize + 4;
                                variableArray.EncodedContents = TakeRange(bytes, position - 4, position + varArraySize);

                                //22 00 04 B4 C9 32 41 A0 == $var:varbuf_0x4B4[$arg1,8]
                                //my thinking: 22 is array, 00 is varbuf, 04 B4 is ID, C9 is 8, 32 is ??, 41 is arg1, A0 is ??
                                //after a little more poking around, setting 22 to anything greater(23->2F) results in no changed behavior.
                                //however, setting 22 to 21 or 20 results in the locker states getting reset entirely on load. (w01a behavior)
                                //i'm thinking then that the lower nibble might determine how many bits to track or something for each index of the array?
                                //maybe the answer lies in single variable declarations, i should poke around in those to see if there's any info to glean

                                //22 00 04 8B CA C2 00 == $var:varbuf_0x48B[2,9]
                                //22 is array, 00 is varbuf, 04 8B is ID, CA is 9, C2 is 1.
                                //does 32 indicate the previous number was a real and A0 indicated the previous number was an arg? 
                                //and if neither are present, then both are reals? not sure. need to study oct's decomp more to have a better understanding i think.
                                //it was an expression. thats all there is to it. :facepalm:
                                
                                args.Add(variableArray);
                                position += varArraySize; 
                            }
                            catch (Exception e)
                            {
                                throw new ParserException($"Failed to parse vararray in args from byte array [{BitConverter.ToString(bytes).Replace("-", "")}] @{position}: {e}");
                            }
                            break;

                        case 0x10:
                            try
                            {
                                //single variable
                                IVariable variable;
                                switch(bytes[position + 1])
                                {
                                    case 0x80:
                                        variable = new Linkvarbuf();
                                        break;
                                    case 0x10:
                                        variable = new Localvarbuf();
                                        break;
                                    default:
                                        variable = new Varbuf();
                                        (variable as Varbuf)!.SpecifiedBit = bytes[position + 1];
                                        break;
                                }
                                variable.DataTypeNibble = (ExpressionElements.DataTypeEnum)(byte)(bytes[position] & 0x0F);
                                byte[] id = TakeRange(bytes, position + 2, position + 4);
                                variable.Id = BitConverter.ToUInt16(id.Reverse().ToArray());
                                variable.EncodedContents = TakeRange(bytes, position, position + 4);

                                variable.Size = 4;
                                args.Add(variable);
                                position += 4;
                            }
                            catch (Exception e)
                            {
                                throw new ParserException($"Failed to parse variable in args from byte array [{BitConverter.ToString(bytes).Replace("-", "")}] @{position}: {e}");
                            } 
                            break;

                        default:
                            if (currentByte == 0)
                            {
                                //empty value, ignore
                                position++;
                            }
                            else
                            {
                                try
                                {
                                    ExpressionElements.DataTypeEnum dataType = (ExpressionElements.DataTypeEnum)currentByte;
                                    byte[] dataValue = new byte[4];
                                    int length = 0;
                                    if (dataType == ExpressionElements.DataTypeEnum.String)
                                    {
                                        length = bytes[position + 1];
                                        dataValue = new byte[length];
                                        Array.Copy(bytes, position + 2, dataValue, 0, length);
                                        args.Add(new Literal { Value = Convert.ToBase64String(dataValue.ToArray()), EncodedContents = TakeRange(bytes, position, (uint)(position + 2 + length)), DataType = dataType, Size = (ushort)(length +1)}); //+1 for dataType declaration
                                    }
                                    else
                                    {
                                        length = ExpressionElements.DataTypeLength(dataType);
                                        Array.Copy(bytes, position + 1, dataValue, 0, length);
                                        args.Add(new Literal { Value = $"0x{Convert.ToHexString(dataValue.Take(length).ToArray().Reverse().ToArray())}", EncodedContents = TakeRange(bytes, position, (uint)(position + 1 + length)), DataType = dataType, Size = (ushort)(length + 1) }); //+1 for dataType declaration
                                    }
                                    position += (uint)(length + 1);
                                }
                                catch (Exception e)
                                {
                                    throw new ParserException($"Failed to parse dataType in args from byte array [{BitConverter.ToString(bytes).Replace("-", "")}] @{position}: {e}");
                                }
                            }
                            break;
                    }
                } while (position < bytes.Length);

                return args;
            }
            catch(Exception e)
            {
                throw new ParserException($"Failed to parse args: {e}");
            }
        }

        private static List<Parameter> DecodeParams(byte[] bytes)
        {
            try
            {
                List<Parameter> parameters = new();

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
                    uint size = DecodeSize(TakeRange(bytes, position, (uint)bytes.Length), ref position);

                    Parameter parameter = new()
                    {
                        ParamType = (char)bytes[position]
                    };
                    position++;
                    parameter.EncodedContents = TakeRange(bytes, position, position + size - 1);
                    parameter.Args = DecodeArgs(parameter.EncodedContents);
                    uint parameterSize = 0;
                    foreach (ITerm arg in parameter.Args)
                    {
                        parameterSize += arg.Size;
                    }
                    parameter.Size = parameterSize + 1; //+1 for paramtype declaration
                    position += (uint)parameter.EncodedContents.Length;
                    parameters.Add(parameter);
                }

                return parameters;
            }
            catch(Exception e)
            {
                throw new ParserException($"Failed to parse params from byte array [{BitConverter.ToString(bytes).Replace("-", "")}]: {e}");
            }
        }

        private static IProcedureElement DecodeCommand(byte[] bytes)
        {
            try
            {
                int startType = 0;
                int endType = 3;
                uint position = 0;

                byte[] knownCommandType = bytes.Take(new Range(new Index(startType), new Index(endType))).ToArray();
                string commandTypeInHex = BitConverter.ToString(knownCommandType.Reverse().ToArray()).Replace("-", "");

                switch (commandTypeInHex)
                {
                    case "6592A7":
                        Chara chara = new()
                        {
                            Size = (ushort)(bytes.Length - 2)
                        };
                        position = 3;
                        chara.EncodedContents = bytes;
                        uint charaArgsLength = DecodeArgsLength(bytes, ref position);
                        byte[] charaArgs = TakeRange(bytes, position, position + charaArgsLength);
                        chara.Args = DecodeArgs(charaArgs);

                        byte[] charaParams = TakeRange(bytes, position + charaArgsLength, (uint)bytes.Length);
                        chara.Parameters = DecodeParams(charaParams);

                        return chara;
                    case "3822C7":
                        Msg msg = new()
                        {
                            Size = (ushort)(bytes.Length - 2) //TODO: where did i get this from? this doesn't make sense
                        };
                        position = 3;
                        msg.EncodedContents = bytes;
                        uint messageArgsLength = DecodeArgsLength(bytes, ref position);
                        byte[] mesgArgs = TakeRange(bytes, position, position + messageArgsLength);
                        msg.Args = DecodeArgs(mesgArgs);

                        position += (uint)mesgArgs.Length;
                        if (bytes[position] != 0x00)
                        {
                            byte[] mesgParams = TakeRange(bytes, position, (uint)bytes.Length);
                            msg.Parameters = DecodeParams(mesgParams);
                        }

                        return msg;
                    case "3BD490":
                        Trap trap = new()
                        {
                            Size = (ushort)(bytes.Length - 2)
                        };
                        position = 3;
                        trap.EncodedContents = bytes;
                        uint trapArgsLength = DecodeArgsLength(bytes, ref position);
                        byte[] trapArgs = TakeRange(bytes, position, position + trapArgsLength);
                        trap.Args = DecodeArgs(trapArgs);

                        byte[] trapParams = TakeRange(bytes, position + trapArgsLength, (uint)bytes.Length);
                        trap.Parameters = DecodeParams(trapParams);

                        return trap;
                    case "082BC9":
                        GameCommand gameCommand = new()
                        {
                            Size = (ushort)(bytes.Length - 2)
                        };
                        position = 3;
                        gameCommand.EncodedContents = bytes;
                        uint gameCommandArgsLength = DecodeArgsLength(bytes, ref position);
                        byte[] gameCommandArgs = TakeRange(bytes, position, position + gameCommandArgsLength);
                        gameCommand.Args = DecodeArgs(gameCommandArgs);

                        byte[] gameCommandParams = TakeRange(bytes, position + gameCommandArgsLength, (uint)bytes.Length);
                        gameCommand.Parameters = DecodeParams(gameCommandParams);
                        return gameCommand;
                    case "37C884": 
                        Load load = new();
                        position = 3;
                        load.EncodedContents = bytes;
                        load.Size = DecodeArgsLength(bytes, ref position); //TODO: fix these size declarations: this is depicting the size of the args, not the whole command
                        byte[] loadArgs = TakeRange(bytes, position, position + load.Size);
                        load.Args = DecodeArgs(loadArgs);
                        //can, in fact, have params
                        position += (uint)loadArgs.Length;
                        if (bytes[position] != 0x00)
                        {
                            byte[] loadParams = TakeRange(bytes, position, (uint)bytes.Length);
                            load.Parameters = DecodeParams(loadParams);
                        }
                        return load;
                    case "01C090":
                        Map map = new();
                        //used anywhere?
                        return map;
                    case "6BB005":
                        Restart restart = new();
                        position = 3;
                        restart.EncodedContents = bytes;
                        restart.Size = DecodeArgsLength(bytes, ref position);
                        byte[] restartArgs = TakeRange(bytes, position, position + restart.Size);
                        restart.Args = DecodeArgs(restartArgs);
                        //can, in fact, have params
                        position += (uint)restartArgs.Length;
                        if (bytes[position] != 0x00)
                        {
                            byte[] restartParams = TakeRange(bytes, position, (uint)bytes.Length);
                            restart.Parameters = DecodeParams(restartParams);
                        }
                        return restart;
                    case "8B3DF5": 
                        UnknownCommand unknownCommand = new();
                        position = 3;
                        unknownCommand.EncodedContents = bytes;
                        unknownCommand.Size = DecodeArgsLength(bytes, ref position);
                        byte[] unknownCommandArgs = TakeRange(bytes, position, position + unknownCommand.Size);
                        unknownCommand.Args = DecodeArgs(unknownCommandArgs);

                        position += (uint)unknownCommandArgs.Length;
                        if (bytes[position] != 0x00)
                        {
                            byte[] unknownParams = TakeRange(bytes, position, (uint)bytes.Length);
                            unknownCommand.Parameters = DecodeParams(unknownParams);
                        }
                        return unknownCommand;
                    case "000D86":
                        IfBlock ifblock = new();
                        position = 3;
                        ifblock.EncodedContents = bytes;
                        ifblock.Size = DecodeArgsLength(bytes, ref position);
                        byte[] ifBlockArgs = TakeRange(bytes, position, position + ifblock.Size);
                        ifblock.Args = DecodeArgs(ifBlockArgs);
                        //args are the main if

                        byte[] ifParams = TakeRange(bytes, position + ifblock.Size, (uint)bytes.Length);
                        //i param is elif, e param is else?
                        ifblock.Parameters = DecodeParams(ifParams);
                        return ifblock;
                    case "A65DB5":
                        SwitchBlock switchBlock = new();
                        position = 3;
                        switchBlock.EncodedContents = bytes;
                        switchBlock.Size = DecodeArgsLength(bytes, ref position);
                        byte[] switchArgs = TakeRange(bytes, position, position + switchBlock.Size);
                        switchBlock.Args = DecodeArgs(switchArgs);

                        byte[] switchParams = TakeRange(bytes, position + switchBlock.Size, (uint)bytes.Length);
                        switchBlock.Parameters = DecodeParams(switchParams);
                        return switchBlock;
                    case "34648C":
                        Evaluate evaluateStatement = new();
                        //used anywhere?
                        return evaluateStatement;
                    case "3311EC":
                        Invoke invokeStatement = new();
                        //used anywhere?
                        return invokeStatement;
                    case "8BE398":
                        Return returnStatement = new();
                        position = 3;
                        returnStatement.EncodedContents = bytes;
                        returnStatement.Size = DecodeArgsLength(bytes, ref position);
                        byte[] returnArgs = TakeRange(bytes, position, position + returnStatement.Size);
                        returnStatement.Args = DecodeArgs(returnArgs);

                        position += (uint)returnArgs.Length;
                        if (bytes[position] != 0x00)
                        {
                            byte[] returnParams = TakeRange(bytes, position, (uint)bytes.Length);
                            returnStatement.Parameters = DecodeParams(returnParams);
                        }

                        return returnStatement;
                    case "3AB23B": 
                        Print printStatement = new();
                        position = 3;
                        printStatement.EncodedContents = bytes;
                        printStatement.Size = DecodeArgsLength(bytes, ref position);
                        byte[] printArgs = TakeRange(bytes, position, position + printStatement.Size);
                        printStatement.Args = DecodeArgs(printArgs);

                        position += (uint)printArgs.Length;
                        if (bytes[position] != 0x00)
                        {
                            byte[] printParams = TakeRange(bytes, position, (uint)bytes.Length);
                            printStatement.Parameters = DecodeParams(printParams);
                        }

                        return printStatement;
                    default:
                        throw new NotImplementedException("Unrecognized command type");
                }
            }
            catch(Exception e)
            {
                throw new ParserException($"Failed to parse command: {e}");
            }
        }

        private static uint DecodeArgsLength(byte[] bytes, ref uint position)
        {
            uint size = bytes[position++];
            if (size >= 0x80)
            {
                byte highNibble = (byte)((size & 0x0F) + ((size & 0xF0)-0x80));
                byte lowByte = (byte)(bytes[position++]);
                byte[] sizeBytes = new byte[4] { lowByte, highNibble, 0, 0 };
                size = BitConverter.ToUInt32(sizeBytes);
            }

            return size;
        }
    }
}
