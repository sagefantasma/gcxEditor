using Gcx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Gcx.Gcx;
using static Gcx.GcxClasses;

namespace GcxEditor
{
    public static class Importer
    {
        private static int cursor = 0;
        private static Dictionary<string, uint> FileTable = new Dictionary<string, uint>();

        public static dynamic ImportGcxFile(string path)
        {
            try
            {
                cursor = 0;
                FileInfo gcxFile = new FileInfo(path);
                if (gcxFile.Exists)
                {
                    byte[] fileContents = File.ReadAllBytes(gcxFile.FullName);
                    byte[] signature = TakeAndAdvance4Bytes(fileContents);
                    byte[] timestamp = TakeAndAdvance4Bytes(fileContents);
                    Dictionary<byte[], byte[]> procedureTable = GetProcedureTable(fileContents);
                    FileTable = GetFileTable(fileContents);
                    byte[] resourceData = GetResourceData(fileContents);
                    byte[] stringData = GetStringData(fileContents);
                    byte[] fontData = GetFontData(fileContents);
                    byte[] procedureData = GetProcedureData(fileContents);
                    byte[] mainProcedureData = GetMainData(procedureData);

                    List<Procedure> parsedProcedures = new List<Procedure>();
                    foreach (KeyValuePair<byte[],byte[]> procedureOffset in procedureTable)
                    {
                        int startingIndex = (BitConverter.ToInt32(procedureOffset.Value) & 0xFFFFFF ) + sizeof(uint);
                        //NOTE: I don't understand _why_ the compiler makes the above necessary... but this is what seems to make things work.
                        ushort procedureSize = ParseProcedureSize(procedureData, startingIndex);
                        int startOffset = procedureSize > 0xFF ? 3 : procedureSize > 0xC ? 2 : 1; //if the function is less than 255 bytes, the data starts 2 bytes after the procedure offset in the table, otherwise it is 3.

                        byte[] procedureBody = TakeRangeFromArray(procedureData, startingIndex + startOffset, startingIndex + procedureSize + startOffset);
                        Procedure parsedProcedure = ParseProcedure(procedureBody, procedureOffset.Key, procedureSize);
                        parsedProcedures.Add(parsedProcedure);
                    }

                    ushort mainSize = ParseProcedureSize(mainProcedureData, 4);
                    int mainStartOffset = 4;
                    byte[] mainBody = TakeRangeFromArray(mainProcedureData, mainStartOffset, mainSize);
                    Procedure mainProcedure = ParseProcedure(mainBody, null, mainSize);

                    FileTable fileTable = new FileTable();
                    fileTable.ScriptTableOffset = FileTable["scriptOffset"];
                    fileTable.ResourceTableOffset = FileTable["resourceOffset"];
                    fileTable.StringTableOffset = FileTable["stringsOffset"];
                    fileTable.FontDataOffset = FileTable["fontOffset"];
                    fileTable.Key = FileTable["key"];
                    ProcedureBlock procedureBlock = new ProcedureBlock();
                    procedureBlock.Procedures = parsedProcedures;
                    procedureBlock.Main = mainProcedure;
                    GcxClasses.Gcx gcx = new GcxClasses.Gcx(fileTable, procedureBlock);

                    foreach(Procedure procedure in parsedProcedures)
                    {
                        if(procedure.Name == "085B23")
                        {

                        }
                        if(procedure.Name == "025E89")
                        {

                        }
                        if(procedure.Name == "0DD51D")
                        {

                        }
                        if(procedure.Name == "0FC3D5")
                        {
                            //these ifblocks are gonna be the death of me. right now we're breaking on the second if's base args
                            //Okay, so the issue I'm running into NOW is that it is possible, for some reason, for a sub-element to reach
                            //BEYOND it's parent's capacity. This might be able to be mitigated by using bytes.Take instead of array.copy
                            //but may cause problems later on when trying to recompile - not sure how the game will handle it. So bizarre.

                            //okay, making that change got me a little further, but it still busted on this function xdd.
                            //starts at 0x1EB6
                        }
                        //next issue im diagnosing: 9a3d0f8b69c92b0804067f -- solved, i think
                        if (procedure.Name == "3D8589")
                        {
                            //now onto having an issue with 0x3D8589 trying to parse a variable array
                            //key off of 7c2d64d4220004b4c93241a0006d1a
                        }
                        if(procedure.Name == "6A8F69")
                        {
                            //the latest problematic function
                        }
                        if(procedure.Name == "8DCEDB")
                        {
                            //the latest-est problematic function, starts at 4A41
                            //3512000aa0a0ada0 is what broke it, presumably because of all the a0s?
                        }
                        if(procedure.Name == "F19AA7")
                        {
                            //even more latest problematic function :*(
                            //breaks in the if statement's args, specifically on param l in the chara... weird
                        }
                        if(procedure.Name == "AA023F")
                        {
                            //breaking on second command
                        }
                        if(procedure.Name == "2F6F8A")
                        {
                            //Arithmetic operation resulted in an overflow error thrown on this one in scenerio_stage_select
                        }
                        try
                        {
                            Procedure parsedProc = ProcParser.ParseProc(procedure.RawContents); //raw contents arent getting filled properly?
                            procedure.DecodedContents = parsedProc.DecodedContents;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    return gcx;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("Failed to import gcx file, no error handling for this case");
            }
        }

        private static byte[] TakeRangeFromArray(byte[] array, int start, int end)
        {
            return array.Take(new Range(new Index(start), new Index(end))).ToArray();
        }

        private static byte[] TakeAndAdvance4Bytes(byte[] bytes)
        {
            return bytes.Take(new Range(new Index(cursor), new Index(cursor += 4))).ToArray();
        }

        private static byte[] TakeAndAdvance8Bytes(byte[] bytes)
        {
            return bytes.Take(new Range(new Index(cursor), new Index(cursor += 8))).ToArray();
        }

        private static Dictionary<byte[], byte[]> GetProcedureTable(byte[] gcxContents)
        {
            Dictionary<byte[], byte[]> procedureTable = new Dictionary<byte[], byte[]>();
            while (!gcxContents.Take(new Range(new Index(cursor), new Index(cursor + 8))).ToArray().SequenceEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }))
            {
                procedureTable.Add(TakeAndAdvance4Bytes(gcxContents),
                    TakeAndAdvance4Bytes(gcxContents));
            } 

            TakeAndAdvance8Bytes(gcxContents);

            return procedureTable;
        }

        private static Dictionary<string, uint> GetFileTable(byte[] gcxContents)
        {
            Dictionary<string, uint> fileTable = new Dictionary<string, uint>
            {
                { "scriptOffset", BitConverter.ToUInt32(TakeAndAdvance4Bytes(gcxContents)) },
                { "resourceOffset", BitConverter.ToUInt32(TakeAndAdvance4Bytes(gcxContents)) },
                { "stringsOffset", BitConverter.ToUInt32(TakeAndAdvance4Bytes(gcxContents)) },
                { "fontOffset", BitConverter.ToUInt32(TakeAndAdvance4Bytes(gcxContents)) },
                { "key", BitConverter.ToUInt32(TakeAndAdvance4Bytes(gcxContents)) }
            };
            cursor -= 20; //need to back up to the start of the file table to get contents
            return fileTable;
        }

        private static byte[] GetResourceData(byte[] gcxContents)
        {
            return gcxContents.Take(new Range(new Index((int)FileTable["resourceOffset"] + cursor), new Index((int)FileTable["stringsOffset"] + cursor))).ToArray();
        }

        private static byte[] GetStringData(byte[] gcxContents)
        {
            return gcxContents.Take(new Range(new Index((int)FileTable["stringsOffset"] + cursor), new Index((int)FileTable["fontOffset"] + cursor))).ToArray();
        }

        private static byte[] GetFontData(byte[] gcxContents)
        {
            return gcxContents.Take(new Range(new Index((int)FileTable["fontOffset"] + cursor), new Index((int)FileTable["scriptOffset"] + cursor))).ToArray();
        }

        private static byte[] GetProcedureData(byte[] gcxContents)
        {
            return gcxContents.Take(new Range(new Index((int)FileTable["scriptOffset"] + cursor), new Index(gcxContents.Length))).ToArray();
        }

        private static byte[] GetMainData(byte[] gcxContents)
        {
            uint sizeOfSubprocedures = BitConverter.ToUInt32(TakeRangeFromArray(gcxContents, 0, 4));
            return TakeRangeFromArray(gcxContents, (int)sizeOfSubprocedures + sizeof(uint), gcxContents.Length);
        }

        private static ushort ParseProcedureSize(byte[] procContents, int offset)
        {
            if (procContents[offset] == 0x8D)
            {
                return (ushort)procContents[offset+1];
            }
            else if (procContents[offset] < 0x8D)
            {
                return (ushort)(procContents[offset] & 0x0F);
            }
            else
            {
                return BitConverter.ToUInt16(procContents, offset+1);
            }
        }

        private static Procedure ParseProcedure(byte[] procContents, byte[]? name, ushort size)
        {
            Procedure procedure;
            if (name != null)
            {
                procedure = new Procedure
                {
                    Size = size,
                    RawContents = procContents,
                    Name = BitConverter.ToUInt32(name).ToString()
                };
            }
            else
            {
                procedure = new Main
                {
                    Size = size,
                    RawContents = procContents
                };
            }

            return procedure;
        }
    }
}
