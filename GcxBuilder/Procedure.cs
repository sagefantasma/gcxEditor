using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GcxEditor
{
    public interface IProcedureElement
    {
        public uint Size { get; set; }
        public string Type { get; set; }
        public byte[] EncodedContents { get; set; }

        public byte[] Encode();
    }

    public class Procedure : Term
    {
        public string Name
        {
            get
            {
                byte[] orderString = BitConverter.GetBytes(Order).Reverse().ToArray();
                if (orderString[0] == 0)
                {
                    orderString = orderString.TakeLast(3).ToArray();
                }
                return BitConverter.ToString(orderString).Replace("-", "");
            }
            set
            {
                Order = uint.Parse(value);
            }
        }
        [JsonIgnore]
        public uint Order { get; private set; }
        [JsonIgnore]
        public uint Size { get; set; }
        public string Type { get; set; }
        [JsonIgnore]
        public byte[] EncodedContents { get; set; }
        [JsonIgnore]
        public byte[] RawContents { get; set; } //TODO: to be implemented for editing
        public List<dynamic> DecodedContents { get; set; }
        public Procedure()
        {
            Type = GetType().Name;
        }

        public byte[] Encode()
        {
            //TODO: confirm this works
            //Encode decoded contents, get the size, slap that on?

            List<byte[]> encodedContents = new List<byte[]>();

            int sizeOfEncodedContents = 0;
            foreach(dynamic decodedContent in DecodedContents)
            {
                byte[] encodedContent = decodedContent.Encode();
                encodedContents.Add(encodedContent);
                sizeOfEncodedContents += encodedContent.Length;
            }
            sizeOfEncodedContents++; //increase by one to get 00 padding at the end :S

            byte[] encodedBytes;
            int position = 0;
            if (sizeOfEncodedContents < 0xD)
            {
                encodedBytes = new byte[sizeOfEncodedContents + 1];
                encodedBytes[position++] = (byte)(0x80 + (byte)sizeOfEncodedContents);
            }
            else if(sizeOfEncodedContents < 0xFF)
            {
                encodedBytes = new byte[sizeOfEncodedContents + 2];
                encodedBytes[position++] = 0x8D;
                encodedBytes[position++] = (byte)(sizeOfEncodedContents);
            }
            else if(sizeOfEncodedContents < 0xFFFF)
            {
                encodedBytes = new byte[sizeOfEncodedContents + 3];
                encodedBytes[position++] = 0x8E;
                Array.Copy(BitConverter.GetBytes((ushort)sizeOfEncodedContents), 0, encodedBytes, position, sizeof(ushort));
                position += 2;
            }
            else
            {
                encodedBytes = new byte[sizeOfEncodedContents + 4];
                encodedBytes[position++] = 0x8F;
                Array.Copy(BitConverter.GetBytes(sizeOfEncodedContents), 1, encodedBytes, position, 3);
                position += 3;
            }

            foreach (byte[] encodedContent in encodedContents)
            {
                Array.Copy(encodedContent, 0, encodedBytes, position, encodedContent.Length);
                position += encodedContent.Length; //i dont know why, but for some reason putting this in the array.copy causes an erroneous overflow error
            }

            EncodedContents = encodedBytes;
            Size = (uint)EncodedContents.Length;

            return encodedBytes;
        }

        public override string ToString()
        {
            string printedString = "";
            if (Name != "000000")
                printedString += $"Procedure {Name}:";
            else
                printedString += "Subprocedure: ";

            if (DecodedContents != null)
            {
                foreach (dynamic item in DecodedContents)
                {
                    printedString += @$"{Environment.NewLine}     {item.ToString()}";
                }
            }

            return printedString;
        }
    }

    public class Main : Procedure
    {
        [JsonIgnore]
        public new uint Order { get; set; }
        public new string Name { get; set; }
        public Main()
        {
            Name = "main";
            Order = 0;
            Type = GetType().Name;
        }

        public byte[] Encode()
        {
            //TODO: determine if necessary, and implement if so
            throw new NotImplementedException();
        }
    }

    public abstract class Command : IProcedureElement
    {
        [JsonIgnore]
        public uint Size { get; set; }
        public string Type { get; set; }
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
        public List<Term> Args = new List<Term>();
        [JsonIgnore]
        public byte[] EncodedContents { get; set; }
        public Command()
        {
            Type = GetType().Name;
        }

        public override string ToString()
        {
            string printedString = $"Args: ";

            foreach (Term arg in Args)
            {
                if (Args.Last() != arg)
                    printedString += $"{arg.ToString()}, ";
                else
                    printedString += arg.ToString();
            }

            printedString += $"{Environment.NewLine}{Environment.NewLine}" +
                $"Parameters: ";

            foreach (Parameter param in Parameters)
            {
                if (Parameters.Last() != param)
                    printedString += $"{param.ToString()}, ";
                else
                    printedString += param.ToString();
            }

            return printedString;
        }

        public abstract byte[] Encode();
    }

    public class Expression : Term
    {
        public Term? Term1 { get; set; }
        public Term? Term2 { get; set; }
        public GcxEditor.Gcx.Operation Operator { get; set; }
        [JsonIgnore]
        public uint Size { get; set; }
        public string Type { get; set; }
        [JsonIgnore]
        public byte[] EncodedContents { get; set; }

        public Expression()
        {
            Type = GetType().Name;
        }

        public byte[] Encode()
        {
            //TODO: confirm this works
            List<byte[]> encodedContents = new List<byte[]>();

            int sizeOfEncodedContents = 0;
            if (Term1 != null)
            {
                byte[] term1Encoded = Term1.Encode();
                sizeOfEncodedContents += term1Encoded.Length;
                encodedContents.Add(term1Encoded);
            }
            byte[] term2Encoded = Term2.Encode();
            sizeOfEncodedContents += term2Encoded.Length;
            encodedContents.Add(term2Encoded);
            sizeOfEncodedContents += 2; //for final operator and end of expression

            byte[] encodedBytes;
            int position = 0;
            if (sizeOfEncodedContents < 0xD)
            {
                encodedBytes = new byte[sizeOfEncodedContents + 1];
                encodedBytes[position++] = (byte)(0x30 + (byte)sizeOfEncodedContents); //off by 2
            }
            else if (sizeOfEncodedContents < 0xFF)
            {
                encodedBytes = new byte[sizeOfEncodedContents + 2];
                encodedBytes[position++] = 0x3D;
                encodedBytes[position++] = (byte)sizeOfEncodedContents;
            }
            else if (sizeOfEncodedContents < 0xFFFF)
            {
                encodedBytes = new byte[sizeOfEncodedContents + 3];
                encodedBytes[position++] = 0x3E;
                Array.Copy(BitConverter.GetBytes((ushort)sizeOfEncodedContents), 0, encodedBytes, position += 2, sizeof(ushort));
            }
            else
            {
                encodedBytes = new byte[sizeOfEncodedContents + 4];
                encodedBytes[position++] = 0x3F;
                Array.Copy(BitConverter.GetBytes(sizeOfEncodedContents), 1, encodedBytes, position += 3, 3);
            }

            foreach (byte[] encodedContent in encodedContents)
            {
                Array.Copy(encodedContent, 0, encodedBytes, position, encodedContent.Length);
                position += encodedContent.Length;
            }
            encodedBytes[encodedBytes.Length - 1] = 0xA0;
            encodedBytes[encodedBytes.Length - 2] = (byte)Operator;

            return encodedBytes;
        }

        public override string ToString()
        {
            return $"{Term1}{Gcx.OperationToString(Operator)}{Term2}";
        }
    }

    public abstract class Statement : Command
    {
        public Statement()
        {
            Type = GetType().Name;
        }
    }

    public class IfBlock : Statement
    {
        public IfBlock()
        {
            Type = GetType().Name;
        }

        public override byte[] Encode()
        {
            //TODO: confirm this works
            byte[] contents = Builder.EncodeCommandWithArgsAndParams(Args, Parameters, new byte[] { 0x86, 0x0D, 0x00 });
            return Builder.BuildContainerElement(0x60, contents);
        }
    }

    public class SwitchBlock : Statement
    {
        public SwitchBlock()
        {
            Type = GetType().Name;
        }

        public override byte[] Encode()
        {
            //TODO: confirm this works
            byte[] contents = Builder.EncodeCommandWithArgsAndParams(Args, Parameters, new byte[] { 0xB5, 0x5D, 0xA6 });
            return Builder.BuildContainerElement(0x60, contents);
        }
    }

    public class Evaluate : Statement
    {
        public Evaluate()
        {
            Type = GetType().Name;
        }

        public override byte[] Encode()
        {
            //TODO: implement
            return null;
            //throw new NotImplementedException();
        }
    }

    public class Invoke : Statement
    {
        public Procedure ProcedureInvoked { get; set; } = new Procedure();
        public List<Term> Args { get; set; } = new List<Term>();
        public Invoke()
        {
            Type = GetType().Name;
        }

        public override byte[] Encode()
        {
            //TODO: confirm this works
            //TODO: does an invoke always end with a 00 buffer? -- i think so
            int procedureInvokedBytes = 4;
            uint argsBytesLength = 0;

            foreach (Term argument in Args)
            {
                argsBytesLength += argument.Size;
            }

            argsBytesLength++; //getting the 00 padding at the end of an invoke
            byte[] encodedBytes;
            int position = 0;
            if ((argsBytesLength + 3) < 0xD)
            {
                //no change to procedureInvokedBytes
                encodedBytes = new byte[procedureInvokedBytes + argsBytesLength];
                encodedBytes[position++] = (byte)(0x70 + procedureInvokedBytes + argsBytesLength);
            }
            else if((argsBytesLength + 3) < 0xFF)
            {
                procedureInvokedBytes++;
                encodedBytes = new byte[procedureInvokedBytes + argsBytesLength];
                encodedBytes[position++] = 0x7D;
                encodedBytes[position++] = (byte)(argsBytesLength + 3);
            }
            else if((argsBytesLength + 3) > 0xFF && (argsBytesLength + 3) < 0xFFFF)
            {
                procedureInvokedBytes += 2;
                encodedBytes = new byte[procedureInvokedBytes + argsBytesLength];
                encodedBytes[position++] = 0x7E;
                Array.Copy(BitConverter.GetBytes((ushort)(argsBytesLength + 3)), 0, encodedBytes, position, sizeof(ushort));
                position += sizeof(ushort);
            }
            else
            {
                procedureInvokedBytes += 3;
                encodedBytes = new byte[procedureInvokedBytes + argsBytesLength];
                encodedBytes[position++] = 0x7F;
                Array.Copy(BitConverter.GetBytes(argsBytesLength + 3), 0, encodedBytes, position, 3);
                position += 3;
            }


            Array.Copy(BitConverter.GetBytes(ProcedureInvoked.Order), 0, encodedBytes, position, 3);
            position += 3;

            foreach (Term arg in Args)
            {
                byte[] encodedArg = arg.Encode();
                Array.Copy(encodedArg, 0, encodedBytes, position, encodedArg.Length);
                position += encodedArg.Length;
            }

            return encodedBytes;
        }

        public override string ToString()
        {
            string printedString = $"Invoke:{Environment.NewLine}" +
                @$" - Procedure Invoked: {ProcedureInvoked}" +
                @$"      - Args on invoke: ";

            foreach (Term arg in Args)
            {
                if (Args.Last() != arg)
                    printedString += $"{arg.ToString()}, ";
                else
                    printedString += arg.ToString();
            }

            return printedString;
        }
    }

    public interface Term : IProcedureElement
    {
    }

    public interface Argument : Term
    {
        public Term Value { get; set; }

    }

    public class Return : Statement
    {
        public Return()
        {
            Type = GetType().Name;
        }

        public override byte[] Encode()
        {
            //TODO: confirm this works
            byte[] contents = Builder.EncodeCommandWithOnlyArgs(Args, new byte[] { 0x8B, 0xE3, 0x98 });
            return Builder.BuildContainerElement(0x60, contents);
        }

        public override string ToString()
        {
            return $"Return:{Environment.NewLine}{base.ToString()}";
        }
    }

    public class Print : Statement
    {
        public Print()
        {
            Type = GetType().Name;
        }

        public override byte[] Encode()
        {
            //TODO: confirm this works
            byte[] contents = Builder.EncodeCommandWithOnlyArgs(Args, new byte[] { 0x3B, 0xB2, 0x3A });
            return Builder.BuildContainerElement(0x60, contents);
        }

        public override string ToString()
        {
            return $"Print:{Environment.NewLine}{base.ToString()}";
        }
    }

    public class Msg : Command
    {
        public Msg()
        {
            Type = GetType().Name;
        }

        public override byte[] Encode()
        {
            //TODO: confirm this works
            byte[] contents = Builder.EncodeCommandWithOnlyArgs(Args, new byte[] { 0xC7, 0x22, 0x38 });
            return Builder.BuildContainerElement(0x60, contents);
        }

        public override string ToString()
        {
            return $"MessageCommand:{base.ToString()}";
        }
    }

    public class GameCommand : Command
    {
        public GameCommand()
        {
            Type = GetType().Name;
        }

        public override byte[] Encode()
        {
            //TODO: confirm this works
            byte[] contents = Builder.EncodeCommandWithArgsAndParams(Args, Parameters, new byte[] { 0xC9, 0x2B, 0x08 });
            return Builder.BuildContainerElement(0x60, contents);
        }

        public override string ToString()
        {
            return $"GameCommand:{base.ToString()}";
        }
    }

    public class Chara : Command
    {
        //0x6592A7
        public Chara()
        {
            Type = GetType().Name;
        }

        public override byte[] Encode()
        {
            //TODO: confirm this works
            byte[] contents = Builder.EncodeCommandWithArgsAndParams(Args, Parameters, new byte[] { 0xA7, 0x92, 0x65 });
            return Builder.BuildContainerElement(0x60, contents);
        }

        public override string ToString()
        {
            return $"CreateCharaCommand:{base.ToString()}";
        }
    }

    public class Trap : Command
    {
        public Trap()
        {
            Type = GetType().Name;
        }

        public override byte[] Encode()
        {
            //TODO: confirm this works
            byte[] contents = Builder.EncodeCommandWithArgsAndParams(Args, Parameters, new byte[] { 0x90, 0xD4, 0x3B });
            return Builder.BuildContainerElement(0x60, contents);
        }

        public override string ToString()
        {
            return $"CreateTrapCommand:{base.ToString()}";
        }
    }

    public class Load : Command
    {
        public Load()
        {
            Type = GetType().Name;
        }

        public override byte[] Encode()
        {
            //TODO: confirm this works
            byte[] contents = Builder.EncodeCommandWithOnlyArgs(Args, new byte[] { 0x84, 0xC8, 0x37 });
            return Builder.BuildContainerElement(0x60, contents);
        }

        public override string ToString()
        {
            return $"LoadCommand:{base.ToString()}";
        }
    }

    public class UnknownCommand : Command
    {
        //0x8B3DF5 -- from gcx analysis, this looks like a warping function? maybe related to the notification that pops up when you transition screens?
        public UnknownCommand()
        {
            Type = this.GetType().Name;
        }

        public override byte[] Encode()
        {
            //TODO: confirm this works
            byte[] contents = Builder.EncodeCommandWithOnlyArgs(Args, new byte[] { 0xF5, 0x3D, 0x8B });
            return Builder.BuildContainerElement(0x60, contents);
        }

        public override string ToString()
        {
            return $"KnownUnknownCommand:{base.ToString()}";
        }
    }

    public class Map : Command
    {
        public Map()
        {
            Type = GetType().Name;
        }

        public override byte[] Encode()
        {
            //TODO: implement
            return null;
            //throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"MapCommand:{base.ToString()}";
        }
    }

    public class Restart : Command
    {
        public Restart()
        {
            Type = GetType().Name;
        }

        public override byte[] Encode()
        {
            //TODO: confirm this works
            byte[] contents = Builder.EncodeCommandWithOnlyArgs(Args, new byte[] { 0x05, 0xB0, 0x6B });
            return Builder.BuildContainerElement(0x60, contents);
        }

        public override string ToString()
        {
            return $"RestartCommand:{base.ToString()}";
        }
    }

    public enum ParameterType
    {
        a=0x61,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z
    }

    

    public class Parameter : IProcedureElement
    {
        [JsonIgnore]
        public uint Size { get; set; }
        public char ParamType { get; set; }
        [JsonIgnore]
        public byte[] EncodedContents { get; set; }
        public List<Term> Args { get; set; }
        public string Type { get; set; } = "Parameter";

        public byte[] Encode()
        {
            //TODO: confirm this works
            byte[] encodedBytes = Builder.InitializeSize(Size, 0x50, out int position);
            encodedBytes[position++] = (byte)ParamType;
            
            foreach (Term arg in Args)
            {
                byte[] encodedArg = arg.Encode();
                Array.Copy(encodedArg, 0, encodedBytes, position, encodedArg.Length);
                position += encodedArg.Length;
            }

            return encodedBytes;
        }

        public override string ToString()
        {
            string printedString = $"parameter({ParamType}):";
            foreach(Term arg in Args)
            {
                if (Args.Last() != arg)
                    printedString += $"{arg.ToString()}, ";
                else
                    printedString += arg.ToString();
            }

            return printedString;
        }
    }

    public class Constant : Term
    {
        [JsonIgnore]
        public uint Size { get; set; }
        public string Type { get; set; }
        [JsonIgnore]
        public byte[] EncodedContents { get; set; }
        public byte Value { get; set; }
        public Constant()
        {
            Type = GetType().Name;
        }

        public byte[] Encode()
        {
            return new[] { (byte)(0xC1 + Value) }; 
        }

        public override string ToString()
        {
            return BitConverter.ToString(new[] { Value });
        }
    }

    public class Literal : Term
    {
        [JsonIgnore]
        public uint Size { get; set; }
        public string Type { get; set; }
        [JsonIgnore]
        public byte[] EncodedContents { get; set; }
        public dynamic Value { get; set; }
        public byte DataTypeByte { get; set; }
        public Gcx.DataType DataType { get; set; }
        public Literal()
        {
            Type = GetType().Name;
        }

        public byte[] Encode()
        {
            //TODO: confirm this works
            byte[] encodedBytes;
            if(DataType == Gcx.DataType.String)
            {
                encodedBytes = new byte[(Value as string).Length + 1];
                encodedBytes[0] = 0x07;
                encodedBytes[1] = Value.Length();
                Array.Copy(Encoding.Default.GetBytes(Value), 0, encodedBytes, 2, Value.Length);
            }
            else
            {
                encodedBytes = new byte[DataType.Length + 1];
                encodedBytes[0] = DataTypeByte; //TODO: can we reverse engineer what determines this so we can make a "fresh" file?
                byte[] dataBytes = BitConverter.GetBytes(Value);
                Array.Copy(dataBytes, 0, encodedBytes, 1, DataType.Length);
            }

            return encodedBytes;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class PassedArg : Term
    {
        [JsonIgnore]
        public uint Size { get; set; }
        public string Type { get; set; }
        [JsonIgnore]
        public byte[] EncodedContents { get; set; }
        public byte ArgNum { get; set; }
        public PassedArg()
        {
            Type = GetType().Name;
        }

        public byte[] Encode()
        {
            //TODO: confirm this works
            if(ArgNum < 0xF)
            {
                return new[] { (byte)(0x40 + ArgNum) };
            }
            else
            {
                return new[] { (byte)0x4F, (byte)(ArgNum - 0xF) };
            }
        }

        public override string ToString()
        {
            return $"arg{ArgNum}";
        }
    }

    public interface Variable : Term
    {
        public ushort Id { get; set; }
        public byte LowNibble { get; set; }
    }

    public class VariableArray : Term
    {
        //public ushort Size { get; set; } //byte instead?
        //public ushort Index { get; set; } //byte instead?
        public List<Term> SizeAndIndex { get; set; }
        public ushort Id { get; set; }
        public byte LowNibble { get; set; }
        public byte ArrayType { get; set; }
        [JsonIgnore]
        public uint Size { get; set; }
        public string Type { get; set; }
        [JsonIgnore]
        public byte[] EncodedContents { get; set; }
        public VariableArray()
        {
            Type = GetType().Name;
        }

        public VariableArray(ushort id, byte lowNibble, byte arrayType, List<Term> sizeAndIndex)
        {
            Type = GetType().Name;
            Id = id;
            LowNibble = lowNibble;
            ArrayType = arrayType;
            SizeAndIndex = sizeAndIndex;
        }

        public byte[] Encode()
        {
            //TODO: confirm this works
            int idAndTypeDeclarationSize = 4;
            int sizeOfArgs = 0;
            foreach (Term argument in SizeAndIndex) 
            {
                sizeOfArgs += (int)argument.Size;
            }
            byte[] encodedBytes = new byte[idAndTypeDeclarationSize + sizeOfArgs];
            encodedBytes[0] = (byte)(0x20 + LowNibble);
            encodedBytes[1] = ArrayType;
            Array.Copy(BitConverter.GetBytes(Id).Reverse().ToArray(), 0, encodedBytes, 2, sizeof(ushort));
            int position = 4;
            foreach(Term argument in SizeAndIndex)
            {
                byte[] encodedArg = argument.Encode();
                Array.Copy(encodedArg, 0, encodedBytes, position, encodedArg.Length);
                position += encodedArg.Length;
            }

            return encodedBytes;
        }

        public override string ToString()
        {
            return $"varArray_0x{BitConverter.ToString(BitConverter.GetBytes(Id).Reverse().ToArray()).Replace("-", "")}";
        }
    }

    public class Linkvarbuf : Variable
    {
        [JsonIgnore]
        public uint Size { get; set; }
        public string Type { get; set; }
        [JsonIgnore]
        public byte[] EncodedContents { get; set; }
        public ushort Id { get; set; }
        public byte LowNibble { get; set; }
        public Linkvarbuf()
        {
            Type = GetType().Name;
        }

        public Linkvarbuf(ushort id)
        {
            Type = GetType().Name;
            Id = id;
        }

        public byte[] Encode()
        {
            //TODO: confirm this works
            byte[] encodedVarbuf = new byte[4];
            encodedVarbuf[0] = (byte)(0x10 + LowNibble);
            encodedVarbuf[1] = 0x80; //is this always correct?
            Array.Copy(BitConverter.GetBytes(Id).Reverse().ToArray(), 0, encodedVarbuf, 2, 2);

            return encodedVarbuf;
        }

        public override string ToString()
        {
            return $"linkVarbuf_0x{BitConverter.ToString(BitConverter.GetBytes(Id).Reverse().ToArray()).Replace("-", "")}";
        }
    }

    public class Varbuf : Variable
    {
        [JsonIgnore]
        public uint Size { get; set; }
        public string Type { get; set; }
        [JsonIgnore]
        public byte[] EncodedContents { get; set; }
        public ushort Id { get; set; }
        public byte ByteType { get; set; }
        public byte LowNibble { get; set; }

        public Varbuf()
        {
            Type = GetType().Name;
        }

        public Varbuf(ushort id)
        {
            Type = GetType().Name;
            Id = id;
        }

        public byte[] Encode()
        {
            //TODO: confirm this works
            byte[] encodedVarbuf = new byte[4];
            encodedVarbuf[0] = (byte)(0x10 + LowNibble);
            encodedVarbuf[1] = ByteType; 
            Array.Copy(BitConverter.GetBytes(Id).Reverse().ToArray(), 0, encodedVarbuf, 2, 2);

            return encodedVarbuf;
        }

        public override string ToString()
        {
            return $"varbuf_0x{BitConverter.ToString(BitConverter.GetBytes(Id).Reverse().ToArray()).Replace("-", "")}";
        }
    }

    public class Localvarbuf : Variable
    {
        [JsonIgnore]
        public uint Size { get; set; }
        public string Type { get; set; }
        [JsonIgnore]
        public byte[] EncodedContents { get; set; }
        public ushort Id { get; set; }
        public byte LowNibble { get; set; }

        public Localvarbuf()
        {
            Type = GetType().Name;
        }

        public Localvarbuf(ushort id)
        {
            Type = GetType().Name;
            Id = id;
        }

        public byte[] Encode()
        {
            //TODO: confirm this works
            byte[] encodedVarbuf = new byte[4];
            encodedVarbuf[0] = (byte)(0x10 + LowNibble);
            encodedVarbuf[1] = 0x10; //is this always correct?
            Array.Copy(BitConverter.GetBytes(Id).Reverse().ToArray(), 0, encodedVarbuf, 2, 2);

            return encodedVarbuf;
        }

        public override string ToString()
        {
            return $"localvarbuf_0x{BitConverter.ToString(BitConverter.GetBytes(Id).Reverse().ToArray()).Replace("-", "")}";
        }
    }

    public class LocalVar : Variable
    {
        [JsonIgnore]
        public uint Size { get; set; }
        public string Type { get; set; }
        [JsonIgnore]
        public byte[] EncodedContents { get; set; }
        public ushort Id { get; set; }
        public byte LowNibble { get; set; } //not used

        public LocalVar()
        {
            Type = GetType().Name;
        }

        public LocalVar(byte input)
        {
            Type = GetType().Name;
            Id = input;
        }

        public byte[] Encode()
        {
            //TODO: confirm this works
            byte[] idBytes = BitConverter.GetBytes(Id);
            byte highNibble = 0x90;
            return new[] { (byte)(highNibble + idBytes.FirstOrDefault()) };
        }


        public override string ToString()
        {
            return $"localVar_0x{BitConverter.ToString(BitConverter.GetBytes(Id).Reverse().ToArray()).Replace("-","")}";
        }
    }
}
