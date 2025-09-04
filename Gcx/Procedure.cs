using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcx
{
    public partial interface IProcedureElement
    {
        public uint Size { get; set; }
        public string Type { get; set; }
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
        public uint Order { get; private set; }
        public ushort Size { get; set; }
        public string Type { get; set; }
        public byte[] RawContents { get; set; } //TODO: to be implemented for editing
        public List<dynamic> DecodedContents { get; set; }
        public Procedure()
        {
            Type = GetType().Name;
        }
    }

    public class Main : Procedure
    {
        public new uint Order { get; set; }
        public new string Name { get; set; }
        public Main()
        {
            Name = "main";
            Order = 0;
            Type = GetType().Name;
        }
    }

    public class Command : IProcedureElement
    {
        //0x6D/6E ....? what?
        public uint Size { get; set; }
        public string Type { get; set; }
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
        public List<Argument> Args = new List<Argument>();
        public Command()
        {
            Type = GetType().Name;
        }
    }

    public class Expression : Term
    {
        public Term? Term1 { get; set; }
        public Term? Term2 { get; set; }
        public Gcx.Operation Operator { get; set; }
        public Expression()
        {
            Type = GetType().Name;
        }
    }

    public class Statement : Command
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
    }

    public class ElseIfBlock : IfBlock
    {
        public ElseIfBlock()
        {
            Type = GetType().Name;
        }
    }

    public class ElseBlock : IfBlock
    {
        public ElseBlock()
        {
            Type = GetType().Name;
        }
    }

    public class SwitchBlock : Statement
    {
        public SwitchBlock()
        {
            Type = GetType().Name;
        }
    }

    public class Evaluate : Statement
    {
        public Evaluate()
        {
            Type = GetType().Name;
        }
    }

    public class Invoke : Statement
    {
        public Procedure ProcedureInvoked { get; set; } = new Procedure();
        public List<Argument> Args { get; set; } = new List<Argument>();
        public Invoke()
        {
            Type = GetType().Name;
        }
    }

    public class Term : IProcedureElement
    {
        public uint Size { get; set; }
        public string Type { get; set; }
        public Term()
        {
            Type = GetType().Name;
        }
    }

    public class Argument : Term
    {
        public Term Value { get; set; } = new Term();
        public Argument()
        {
            Type = GetType().Name;
        }
    }

    public class Return : Statement
    {
        public Return()
        {
            Type = GetType().Name;
        }
    }

    public class Print : Statement
    {
        public Print()
        {
            Type = GetType().Name;
        }
    }

    public class Msg : Command
    {
        public Msg()
        {
            Type = GetType().Name;
        }
    }

    public class GameCommand : Command
    {
        public GameCommand()
        {
            Type = GetType().Name;
        }
    }

    public class Chara : Command
    {
        //0x6592A7
        public Chara()
        {
            Type = GetType().Name;
        }
    }

    public class Trap : Command
    {
        public Trap()
        {
            Type = GetType().Name;
        }
    }

    public class Load : Command
    {
        public Load()
        {
            Type = GetType().Name;
        }
    }

    public class UnknownCommand : Command
    {
        //0x8B3DF5 -- from gcx analysis, this looks like a warping function? maybe related to the notification that pops up when you transition screens?
        public UnknownCommand()
        {
            Type = this.GetType().Name;
        }
    }

    public class Map : Command
    {
        public Map()
        {
            Type = GetType().Name;
        }
    }

    public class Restart : Command
    {
        public Restart()
        {
            Type = GetType().Name;
        }
    }

    public enum ParameterType
    {
        a=0x61,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z
    }

    public class Parameter : IProcedureElement
    {
        public uint Size { get; set; }
        public char ParamType { get; set; }
        public byte[] Contents { get; set; }
        public List<Argument> Args { get; set; }
        public string Type { get; set; } = "Parameter";
    }

    /*public class Value : IProcedureElement 
    {
        public ushort Size { get; set; }
    }*/

    public class Constant : Term
    {
        public byte Value { get; set; }
        public Constant()
        {
            Type = GetType().Name;
        }
    }

    public class Literal : Term
    {
        public dynamic Value { get; set; }
        public Literal()
        {
            Type = GetType().Name;
        }
    }

    public class PassedArg : Term
    {
        public byte ArgNum { get; set; }
        public PassedArg()
        {
            Type = GetType().Name;
        }
    }

    public class Variable : Term
    {
        public ushort Id { get; set; }
        public byte LowNibble { get; set; }
        public Variable()
        {
            Type = GetType().Name;
        }
    }

    public class VariableArray : Term
    {
        //public ushort Size { get; set; } //byte instead?
        //public ushort Index { get; set; } //byte instead?
        public List<Argument> SizeAndIndex { get; set; }
        public ushort Id { get; set; }
        public byte LowNibble { get; set; }
        public byte ArrayType { get; set; }
        public VariableArray()
        {
            Type = GetType().Name;
        }
    }

    public class Linkvarbuf : Variable
    {
        public Linkvarbuf()
        {
            Type = GetType().Name;
        }
    }

    public class Varbuf : Variable
    {
        public Varbuf()
        {
            Type = GetType().Name;
        }
    }

    public class LocalVar : Variable
    {
        public LocalVar()
        {
            Type = GetType().Name;
        }
    }
}
