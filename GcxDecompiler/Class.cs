using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GcxDecompiler
{
    public class Data
    {
        public byte[] DataArray;
        public int position;

        public Data(byte[] data)
        {
            DataArray = data;
        }
    }

    public class CodeInfo
    {
        public string Name { get; set; }
        public IntPtr Function { get; set; }

        public CodeInfo(string name, IntPtr function)
        {
            Name = name;
            Function = function;
        }
    }

    public class CommandInfo
    {
        public string Name { get; set; }
        public IntPtr Function { get; set; }

        public CommandInfo(string name, IntPtr function)
        {
            Name = name;
            Function = function;
        }
    }
}
