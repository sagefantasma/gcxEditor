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

        public enum DataTypeEnum
        {
            Null = 0,
            Short = 1,
            Byte = 2,
            Nibble = 3,
            ByteAsBits = 4,
            StrCode = 6,
            String = 7,
            ProcedureId = 8,
            Float = 9,
            Integer = 10,
            Long = 13,
            StringResource = 14
        }

        public static int DataTypeLength(DataTypeEnum dataType) 
        {
            switch (dataType)
            {
                case DataTypeEnum.Null:
                    return 0;
                case DataTypeEnum.Short:
                case DataTypeEnum.StringResource:
                    return 2;
                case DataTypeEnum.Byte:
                case DataTypeEnum.Nibble:
                case DataTypeEnum.ByteAsBits:
                    return 1;
                case DataTypeEnum.StrCode:
                case DataTypeEnum.ProcedureId:
                    return 3;
                case DataTypeEnum.Float:
                case DataTypeEnum.Integer:
                case DataTypeEnum.Long:
                    return 4;
                case DataTypeEnum.String:
                    throw new InvalidOperationException("Cannot determine string length by itself, look at next byte for length");
                default:
                    throw new NotImplementedException("Unknown data type, cannot determine length");
            }
        }
    }
}
