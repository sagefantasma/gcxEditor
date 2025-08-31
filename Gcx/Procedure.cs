using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcx
{
    public partial interface IProcedureElement
    {
        public ushort Size { get; set; }
    }

    public class Procedure : IProcedureElement
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
        public byte[] RawContents { get; set; } //TODO: to be implemented for editing
        public List<dynamic> DecodedContents { get; set; }
    }

    public class Main : Procedure
    {
        public new uint Order { get; set; }
        public new string Name { get; set; }
        public Main()
        {
            Name = "main";
            Order = 0;
        }
    }

    public class Command : IProcedureElement
    {
        //0x6D/6E ....? what?
        public ushort Size { get; set; }
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
    }

    public class Expression : Term
    {
        public Term Term1 { get; set; }
        public Term Term2 { get; set; }
        public Gcx.Operation Operator { get; set; }
    }

    public class Statement : Command
    {
    }

    public class IfBlock : Statement
    {

    }

    public class ElseIfBlock : IfBlock
    {

    }

    public class ElseBlock : IfBlock
    {

    }

    public class SwitchBlock : Statement
    {

    }

    public class Evaluate : Statement
    {

    }

    public class Invoke : Statement
    {
        public Procedure ProcedureInvoked { get; set; } = new Procedure();
        public List<Argument> Args { get; set; } = new List<Argument>();
    }

    public class Term : IProcedureElement
    {
        public ushort Size { get; set; }
    }

    public class Argument : Term
    {
        public Term Value { get; set; } = new Term();
    }

    public class Return : Statement
    {

    }

    public class Print : Statement
    {

    }

    public class Msg : Command
    {

    }

    public class GameCommand : Command
    {

    }

    public class Chara : Command
    {
        //0x6592A7
        public List<Argument> Args = new List<Argument>();
    }

    public class Trap : Command
    {

    }

    public class Load : Command
    {

    }

    public class UnknownCommand : Command
    {
        //0x8B3DF5 -- from gcx analysis, this looks like a warping function? maybe related to the notification that pops up when you transition screens?
    }

    public class Map : Command
    {

    }

    public class Restart : Command
    {

    }

    public enum ParameterType
    {
        a=0x61,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z
    }

    public class Parameter : IProcedureElement
    {
        public ushort Size { get; set; }
        public ParameterType Type { get; set; }
        public byte[] Contents { get; set; }
    }

    /*public class Value : IProcedureElement 
    {
        public ushort Size { get; set; }
    }*/

    public class Constant : Term
    {
        public byte Value { get; set; }
    }

    public class Literal : Term
    {
        public uint Value { get; set; }
    }

    public class PassedArg : Term
    {
        public byte ArgNum { get; set; }
    }

    public class Variable : Term
    {
        public ushort Id { get; set; }
        public byte LowNibble { get; set; }
    }

    public class VariableArray : Term
    {
        //public ushort Size { get; set; } //byte instead?
        public ushort Index { get; set; } //byte instead?
        public ushort Id { get; set; }
        public byte LowNibble { get; set; }
    }

    public class Linkvarbuf : Variable
    {

    }

    public class Varbuf : Variable
    {

    }

    public class LocalVar : Variable
    {

    }
}
