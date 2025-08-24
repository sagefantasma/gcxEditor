using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gcx
{
    public sealed class GcxClasses
    {
        public class Gcx
        {
            public uint Signature { get; set; }
            public uint Timestamp { get; set; }
            private FileTable FileTable { get; set; }
            public ProcedureBlock ProcBlock { get; set; }

            private const uint IntPadding = 0x00000000;
            private const ulong LongPadding = 0x0000000000000000;

            public Gcx(FileTable fileTable, ProcedureBlock procedureBlock) 
            {
                Signature = BitConverter.ToUInt32(Encoding.UTF8.GetBytes("LCGB"));
                Timestamp = BitConverter.ToUInt32([0x3D, 0x88, 0x92, 0x3D]); ;
                FileTable = fileTable;
                ProcBlock = procedureBlock;
            }
        }

        

        private class ResourceData { byte[] Data { get; set; } } //TODO: to be implemented for editing

        private class StringData { byte[] Data { get; set; } } //TODO: to be implemented for editing

        private class FontData { byte[] Data { get; set; } } //TODO: to be implemented for editing

        public class FileTable
        {
            public uint ScriptTableOffset;
            public uint ResourceTableOffset;
            public uint StringTableOffset;
            public uint FontDataOffset;
            public uint Key;
        }

        internal class ProcedureTable
        {
            public Dictionary<Procedure, uint> Procedures { get; set; } = new Dictionary<Procedure, uint>();

            public void Recalculate()
            {
                List<Procedure> procedureOrder = Procedures.OrderBy(proc => proc.Key.Order).Select(x=>x.Key).ToList(); //this is a requirement for the gcx format

                Procedures.Clear();

                uint position = 0;
                foreach(Procedure proc in procedureOrder)
                {
                    Procedures.Add(proc, position);
                    position += (uint)proc.RawContents.Length;
                }
            }
        }

        public class ProcedureBlock
        {
            private List<Procedure> procedures = new List<Procedure>();
            public List<Procedure> Procedures { 
                get { return procedures; } 
                set
                {
                    procedures = value;
                }
            }
            public Procedure Main { get; set; } = new Procedure();
        }
    }
}
