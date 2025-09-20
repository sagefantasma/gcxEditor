using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GcxEditor.GcxClasses;

namespace GcxEditor
{
    public class EncodingComparer
    {
        public byte[] OriginalBytes;
        public byte[] ReEncodedBytes;
        public string Name;
    }

    public static class Importer
    {
        private static int cursor = 0;
        private static int positionOfZeroPadding;
        private static Dictionary<string, uint> FileTable = new Dictionary<string, uint>();

        public static Dictionary<Procedure, byte[]> ImportJsonFile(string path)
        {
            string fileContents = File.ReadAllText(path);
            GcxClasses.Gcx deserializedGcx = JsonConvert.DeserializeObject<GcxClasses.Gcx>(fileContents);
            return EncodeProcsFromJson(deserializedGcx.ProcBlock.Procedures);
            
        }

        public static Dictionary<Procedure, byte[]> EncodeProcsFromJson(List<Procedure> jsonProcedures)
        {
            Dictionary<Procedure, byte[]> encodedProcs = new Dictionary<Procedure, byte[]>();
            //TODO: running into an issue where, when pulling from json, nested objects aren't getting created as the correct objects,
            //but instead as jobjects... how can i fix this?
            foreach (Procedure procedure in jsonProcedures)
            {
                try
                {
                    byte[] encodedBytes = procedure.Encode();
                    encodedProcs.Add(procedure, encodedBytes);
                }
                catch(Exception e)
                {

                }
            }

            return encodedProcs;
        }

        public static Dictionary<Procedure, byte[]> EncodeProcsFromRawGcx(List<Procedure> parsedProcedures)
        {
            Dictionary<Procedure, byte[]> reEncodedProcs = new Dictionary<Procedure, byte[]>();
            List<EncodingComparer> misEncodedProcs = new List<EncodingComparer>();
            List<EncodingComparer> correctEncoding = new List<EncodingComparer>();
            foreach (Procedure procedure in parsedProcedures)
            {
                try
                {
                    if (procedure.Name == "547619")
                    {
                        //no known broken procedures ~o~
                        //able to go through ALL native gcx files and decode and reencode without throwing any exceptions!
                    }
                    Procedure parsedProc = ProcDecoder.DecodeProc(procedure.RawContents);
                    byte[] reEncodedBytes = parsedProc.Encode();
                    if (!reEncodedBytes.TakeLast(procedure.RawContents.Length).SequenceEqual(procedure.RawContents))
                    {
                        misEncodedProcs.Add(new EncodingComparer { Name = procedure.Name, OriginalBytes = procedure.RawContents, ReEncodedBytes = reEncodedBytes });
                    }
                    else
                    {
                        correctEncoding.Add(new EncodingComparer { Name = procedure.Name, OriginalBytes = procedure.RawContents, ReEncodedBytes = reEncodedBytes });
                    }
                    reEncodedProcs.Add(procedure, reEncodedBytes);
                    procedure.DecodedContents = parsedProc.DecodedContents;
                }
                catch (Exception ex)
                {
                }
            }

            return reEncodedProcs;
        }

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
                        int startOffset = procedureSize > 0xFFFF ? 4 : procedureSize > 0xFF ? 3 : procedureSize > 0xC ? 2 : 1; //if the function is less than 255 bytes, the data starts 2 bytes after the procedure offset in the table, otherwise it is 3.

                        byte[] procedureBody = TakeRangeFromArray(procedureData, startingIndex + startOffset, startingIndex + procedureSize + startOffset);
                        Procedure parsedProcedure = ParseProcedure(procedureBody, procedureOffset.Key, procedureSize);
                        parsedProcedures.Add(parsedProcedure);
                    }

                    int mainStartOffset = 4;
                    ushort mainSize = ParseProcedureSize(mainProcedureData, mainStartOffset);
                    int sizeOffset = mainSize > 0xFFFF ? 4 : mainSize > 0xFF ? 3 : mainSize > 0xC ? 2 : 1;
                    byte[] mainBody = TakeRangeFromArray(mainProcedureData, mainStartOffset + sizeOffset, mainSize + mainStartOffset + sizeOffset);
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
                    gcx.FileContents = fileContents;
                    gcx.FileTable = fileTable;

                    EncodeProcsFromRawGcx(procedureBlock.Procedures);

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

        public static void AssembleReencodedFile(GcxClasses.Gcx gcx, Dictionary<Procedure, byte[]> reEncodedProcs)
        {
            byte[] fileContents = gcx.FileContents;
            FileTable fileTable = gcx.FileTable;
            byte[] procedureData = GetProcedureData(fileContents);
            byte[] mainProcedureData = GetMainData(procedureData);
            byte[] preamble = fileContents.Take(8).ToArray();
            //proc table
            int procTableSize = reEncodedProcs.Count * 4 * 2;
            byte[] constantData = fileContents.Take(new Range(new Index(positionOfZeroPadding - 8), new Index(positionOfZeroPadding + (int)fileTable.ScriptTableOffset))).ToArray();
            int procCollectionSize = 0;
            foreach (KeyValuePair<Procedure, byte[]> reEncodedProc in reEncodedProcs)
            {
                procCollectionSize += reEncodedProc.Value.Length;
            }
            byte[] procTableBytes = new byte[procTableSize];
            int procTablePosition = 0;
            int procBodyPosition = 0;
            byte[] procBodyCollection = new byte[procCollectionSize];
            foreach (KeyValuePair<Procedure, byte[]> reEncodedProc in reEncodedProcs)
            {
                Array.Copy(BitConverter.GetBytes(reEncodedProc.Key.Order), 0, procTableBytes, procTablePosition, 4);
                procTablePosition += 4;
                Array.Copy(BitConverter.GetBytes(procBodyPosition), 0, procTableBytes, procTablePosition, 4);
                procTablePosition += 4;
                Array.Copy(reEncodedProc.Value, 0, procBodyCollection, procBodyPosition, reEncodedProc.Value.Length);
                procBodyPosition += reEncodedProc.Value.Length;
            }

            byte[] wholeFileReencoded = new byte[preamble.Length + procTableSize + constantData.Length + procBodyCollection.Length + 4 + mainProcedureData.Length];
            int position = 0;
            Array.Copy(preamble, 0, wholeFileReencoded, position, preamble.Length);
            position += preamble.Length;
            Array.Copy(procTableBytes, 0, wholeFileReencoded, position, procTableBytes.Length);
            position += procTableBytes.Length;
            Array.Copy(constantData, 0, wholeFileReencoded, position, constantData.Length);
            position += constantData.Length;
            Array.Copy(BitConverter.GetBytes(procCollectionSize), 0, wholeFileReencoded, position, 4);
            position += 4;
            Array.Copy(procBodyCollection, 0, wholeFileReencoded, position, procCollectionSize);
            position += procCollectionSize;
            Array.Copy(mainProcedureData, 0, wholeFileReencoded, position, mainProcedureData.Length);

            File.WriteAllBytes("reencodedGcxAttempt2.gcx", wholeFileReencoded);
        }

        private static void ReencodeFileTake1()
        {/*
            int preambleSize = 8;
            int procTableSize = reEncodedProcs.Count * 4 * 2;
            int endOfTablePadding = 8;
            int fileTableSize = FileTable.Count * 4;
            int sizeOfProcCollection = 4;
            int procCollection = 0;
            foreach (KeyValuePair<Procedure, byte[]> reEncodedProc in reEncodedProcs)
            {
                procCollection += reEncodedProc.Value.Length;
            }
            byte[] wholeFileReEncodedBytes = new byte[preambleSize + procTableSize + endOfTablePadding + fileTableSize +
                sizeOfProcCollection + procCollection + mainProcedureData.Length + resourceData.Length + stringData.Length + fontData.Length];

            int position = 0;
            Array.Copy(signature, 0, wholeFileReEncodedBytes, position, signature.Length);
            position += signature.Length;
            Array.Copy(timestamp, 0, wholeFileReEncodedBytes, position, timestamp.Length);
            position += timestamp.Length;
            byte[] procTableBytes = new byte[procTableSize];
            int procTablePosition = 0;
            int procBodyPosition = 0;
            byte[] procBodyCollection = new byte[procCollection];
            foreach (KeyValuePair<Procedure, byte[]> reEncodedProc in reEncodedProcs)
            {
                Array.Copy(BitConverter.GetBytes(reEncodedProc.Key.Order), 0, procTableBytes, procTablePosition, 4);
                procTablePosition += 4;
                Array.Copy(BitConverter.GetBytes(procBodyPosition), 0, procTableBytes, procTablePosition, 4);
                procTablePosition += 4;
                Array.Copy(reEncodedProc.Value, 0, procBodyCollection, procBodyPosition, reEncodedProc.Value.Length);
                procBodyPosition += reEncodedProc.Value.Length;
            }
            Array.Copy(procTableBytes, 0, wholeFileReEncodedBytes, position, procTableBytes.Length);
            position += procTableBytes.Length;
            Array.Copy(new byte[] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, }, 0, wholeFileReEncodedBytes, position, endOfTablePadding);
            position += endOfTablePadding;
            foreach (KeyValuePair<string, uint> entry in FileTable)
            {
                Array.Copy(BitConverter.GetBytes(entry.Value), 0, wholeFileReEncodedBytes, position, 4);
                position += 4;
            }
            Array.Copy(resourceData, 0, wholeFileReEncodedBytes, position, resourceData.Length);
            position += resourceData.Length;
            Array.Copy(stringData, 0, wholeFileReEncodedBytes, position, stringData.Length);
            position += stringData.Length;
            Array.Copy(fontData, 0, wholeFileReEncodedBytes, position, fontData.Length);
            position += fontData.Length;
            Array.Copy(BitConverter.GetBytes(procCollection), 0, wholeFileReEncodedBytes, position, 4);
            position += 4;
            Array.Copy(procBodyCollection, 0, wholeFileReEncodedBytes, position, procBodyCollection.Length);
            position += procBodyCollection.Length;
            Array.Copy(mainProcedureData, 0, wholeFileReEncodedBytes, position, mainProcedureData.Length);

            File.WriteAllBytes("reEncodedFile.gcx", wholeFileReEncodedBytes);
            */
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
            positionOfZeroPadding = cursor;
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
