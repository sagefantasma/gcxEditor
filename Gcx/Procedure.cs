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
        public string Name = "main";
        public uint Order = 0;
        public ushort Size { get; set; }
        public byte[] RawContents { get; set; }
    }

    public class Command : IProcedureElement
    {
        //0x6D/6E ....? what?
        public ushort Size { get; set; }
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
    }

    public class Expression : IProcedureElement
    {
        public ushort Size { get; set; }
        public ushort Term1 { get; set; }
        public ushort Term2 { get; set; }
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
        public List<byte[]> Args = new List<byte[]>();
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

    public class Constant : IProcedureElement
    {
        public ushort Size { get; set; }
    }

    public class Variable : IProcedureElement
    {
        public ushort Size { get; set; } = 3; //TODO: confirm
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
