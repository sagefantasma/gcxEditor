using Newtonsoft.Json;

namespace GcxEditor
{
    public class ExpressionElements
    {
        public static string OperationToString(Operation operation)
        {
            switch (operation)
            {
                case Operation.NoOp:
                case Operation.Value2:
                    return "";
                case Operation.NegateValue2:
                case Operation.Value1MinusValue2:
                    return " - ";
                case Operation.Value1PlusValue2:
                    return " + ";
                case Operation.Value2Equals0:
                    return "0 == ";
                case Operation.Value1MulitpliedByValue2:
                    return " * ";
                case Operation.Value1DividedByValue2:
                    return " / ";
                case Operation.Value1ModuloValue2:
                    return " % ";
                case Operation.Value1LeftShiftValue2:
                    return " << ";
                case Operation.Value1RightShiftValue2:
                    return " >> ";
                case Operation.Value1IsEqualToValue2:
                    return " == ";
                case Operation.Value1NotEqualToValue2:
                    return " != ";
                case Operation.Value1LessThanValue2:
                    return " < ";
                case Operation.Value1LessThanOrEqualToValue2:
                    return " <= ";
                case Operation.Value1GreaterThanValue2:
                    return " > ";
                case Operation.Value1GreaterThanOrEqualToValue2:
                    return " >= ";
                case Operation.Value1BitwiseOrValue2:
                    return " | ";
                case Operation.Value1BitwiseAndValue2:
                    return " & ";
                case Operation.Value1BitwiseXorValue2:
                    return " XOR ";
                case Operation.Value1OrValue2:
                    return " || ";
                case Operation.Value1AndValue2:
                    return " && ";
                case Operation.Value1SetToValue2:
                    return " = ";
                default:
                    return " UNKNOWN OPERATOR ";
            }
        }

        public enum Operation
        {
            NoOp = 0xA0,
            NegateValue2,
            Value2Equals0,
            BitwiseComplementOfValue2,
            Value1PlusValue2,
            Value1MinusValue2,
            Value1MulitpliedByValue2,
            Value1DividedByValue2,
            Value1ModuloValue2,
            Value1LeftShiftValue2,
            Value1RightShiftValue2,
            Value1IsEqualToValue2,
            Value1NotEqualToValue2,
            Value1LessThanValue2,
            Value1LessThanOrEqualToValue2,
            Value1GreaterThanValue2,
            Value1GreaterThanOrEqualToValue2,
            Value1BitwiseOrValue2,
            Value1BitwiseAndValue2,
            Value1BitwiseXorValue2,
            Value1OrValue2,
            Value1AndValue2,
            Value1SetToValue2,
            Value2
        }

        public sealed class DataType
        {
            public int Length;
            private readonly byte[] _values;
            public string DataTypeName { get; set; }

            [JsonConstructor]
            private DataType(string name, byte[] values, int length)
            {
                DataTypeName = name;
                _values = values;
                Length = length;
            }

            public static DataType End = new DataType("End", [0x00], 0); //TODO: confirm
            public static DataType Short = new DataType("Short", [0x01], 2); 
            public static DataType Byte = new DataType("Byte", [0x02, 0x03, 0x04], 1); 
            public static DataType StrCode = new DataType("StrCode", [0x06, 0x08], 3); //TODO: confirm 8 also results in 3 bytes
            public static DataType String = new DataType("String", [0x07], 0); //determined by byte following string designation
            public static DataType Long = new DataType("Long", [0x09, 0x0A, 0x0D], 4);
            public static DataType StringResource = new DataType("StringResource", [0x0E], 2); //TODO: confirm

            public static DataType FromCode(byte code)
            {
                switch (code)
                {
                    case 0x0:
                        return End;
                    case 0x1:
                        return Short;
                    case 0x2:
                    case 0x3:
                    case 0x4:
                        return Byte;
                    case 0x6:
                    case 0x8:
                        return StrCode;
                    case 0x7:
                        return String;
                    case 0x9:
                    case 0xA:
                    case 0xD:
                        return Long;
                    case 0xE:
                        return StringResource;
                    default:
                        throw new InvalidDataException($"Byte: \"{code}\" is an invalid Datatype!");
                }
            }

            public override string ToString()
            {
                return DataTypeName;
            }
        }
    }
}
