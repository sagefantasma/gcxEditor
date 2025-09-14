using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                    int mainStartOffset = 4;
                    ushort mainSize = ParseProcedureSize(mainProcedureData, mainStartOffset);                    
                    byte[] mainBody = TakeRangeFromArray(mainProcedureData, mainStartOffset, mainSize + mainStartOffset);
                    Procedure mainProcedure = ParseProcedure(mainBody, null, mainSize);

                    GcxClasses.FileTable fileTable = new GcxClasses.FileTable();
                    fileTable.ScriptTableOffset = FileTable["scriptOffset"];
                    fileTable.ResourceTableOffset = FileTable["resourceOffset"];
                    fileTable.StringTableOffset = FileTable["stringsOffset"];
                    fileTable.FontDataOffset = FileTable["fontOffset"];
                    fileTable.Key = FileTable["key"];
                    GcxClasses.ProcedureBlock procedureBlock = new GcxClasses.ProcedureBlock();
                    procedureBlock.Procedures = parsedProcedures;
                    procedureBlock.Main = mainProcedure;
                    GcxClasses.Gcx gcx = new GcxClasses.Gcx(fileTable, procedureBlock);

                    string formattedContents = "";

                    Dictionary<Procedure, byte[]> reEncodedProcs = new Dictionary<Procedure, byte[]>();
                    foreach (Procedure procedure in parsedProcedures)
                    {
                        try
                        {
                            if (procedure.Name == "096A6A")
                            {
                                //okay, 5B6127 looks good! ^___^
                                //7A6AFF looks good now
                                //6572E6 is correct
                                //092DCE - last one that failed encoding when i was testing that
                                //096A6A next to check/
                                //2BAE1D broken on scenerio.gcx
                            }
                            Procedure parsedProc = ProcDecoder.DecodeProc(procedure.RawContents); //raw contents arent getting filled properly?
                            //TODO: i think expressions arent actually getting decoded correctly, it looks like they might always be getting returned as nested?
                            //byte[] reEncodedBytes = parsedProc.Encode();
                            //reEncodedProcs.Add(procedure, reEncodedBytes);
                            procedure.DecodedContents = parsedProc.DecodedContents;
                            formattedContents += procedure.ToString();
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    

                    File.WriteAllText("formattedOutput.txt", formattedContents);

                    Procedure decodedMain = ProcDecoder.DecodeProc(mainProcedure.RawContents);
                    Main main = new Main();
                    main.EncodedContents = mainProcedure.RawContents;
                    main.DecodedContents = decodedMain.DecodedContents;
                    gcx.Main = main;

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
