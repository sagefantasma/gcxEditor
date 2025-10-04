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
        private static Dictionary<string, uint> FileTable = new();

        public static Dictionary<Procedure, byte[]> ImportJsonFile(string path)
        {
            try
            {
                string fileContents = File.ReadAllText(path);
                JsonSerializerSettings settings = new()
                {
                    MaxDepth = 128,
                    TypeNameHandling = TypeNameHandling.All
                };
                Gcx deserializedGcx = JsonConvert.DeserializeObject<Gcx>(fileContents, settings)!;
                List<Procedure> proceduresToEncode = deserializedGcx.ProcBlock.Procedures;
                proceduresToEncode.Add(deserializedGcx.Main!);
                return EncodeProcsFromJson(proceduresToEncode);
            }
            catch (Exception ex)
            {
                throw new JsonImporterException("Failed to import JSON file", ex);
            }
        }

        public static Dictionary<Procedure, byte[]> EncodeProcsFromJson(List<Procedure> jsonProcedures)
        {
            Dictionary<Procedure, byte[]> encodedProcs = new();
            foreach (Procedure procedure in jsonProcedures)
            {
                try
                {
                    byte[] encodedBytes = procedure.Encode();
                    encodedProcs.Add(procedure, encodedBytes);
                }
                catch(Exception e)
                {
                    throw new EncoderException("Failed to encode procs from json", e);
                }
            }

            return encodedProcs;
        }

        public static void DecodeProcsFromRawGcx(List<Procedure> parsedProcedures)
        {
            foreach (Procedure procedure in parsedProcedures)
            {
                Procedure parsedProc = ProcDecoder.DecodeProc(procedure.RawContents);
                procedure.DecodedContents = parsedProc.DecodedContents;
            }
        }

        public static Dictionary<Procedure, byte[]> EncodeProcsFromRawGcx(List<Procedure> parsedProcedures)
        {
            Dictionary<Procedure, byte[]> reEncodedProcs = new();
            List<EncodingComparer> misEncodedProcs = new();
            List<EncodingComparer> correctEncoding = new();
            foreach (Procedure procedure in parsedProcedures)
            {
                try
                {
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
                    throw new EncoderException("Failed to encode procs from raw gcx", ex);
                }
            }

            return reEncodedProcs;
        }

        public static dynamic ImportGcxFile(string path)
        {
            try
            {
                cursor = 0;
                FileInfo gcxFile = new(path);
                if (gcxFile.Exists)
                {
                    byte[] fileContents = File.ReadAllBytes(gcxFile.FullName);
                    byte[] signature = TakeAndAdvance4Bytes(fileContents);
                    byte[] timestamp = TakeAndAdvance4Bytes(fileContents);
                    Dictionary<byte[], byte[]> procedureTable = GetProcedureTable(fileContents);
                    FileTable = GetFileTable(fileContents);
                    byte[] resourceData = GetResourceData(fileContents);
                    List<int> resourceDataTable = ParseResourceDataTable(resourceData);
                    byte[] stringData = GetStringData(fileContents);
                    ParseStringData(stringData, resourceDataTable);
                    byte[] fontData = GetFontData(fileContents);
                    //Key is actually an offset(always 14?, which starts with an int. that int says how many ints come after? sometimes containing data?
                    //File.WriteAllText($"{gcxFile.Name}_fontData.json", JsonConvert.SerializeObject(fontData));
                    byte[] procedureData = GetProcedureData(fileContents);
                    byte[] mainProcedureData = GetMainData(procedureData);

                    List<Procedure> parsedProcedures = new();
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

                    FileTable fileTable = new()
                    {
                        ScriptTableOffset = FileTable["scriptOffset"],
                        ResourceTableOffset = FileTable["resourceOffset"],
                        StringTableOffset = FileTable["stringsOffset"],
                        FontDataOffset = FileTable["fontOffset"],
                        Key = FileTable["key"]
                    };
                    ProcedureBlock procedureBlock = new()
                    {
                        Procedures = parsedProcedures
                    };
                    Gcx gcx = new(fileTable, procedureBlock)
                    {
                        FileContents = fileContents,
                        FileTable = fileTable
                    };

                    DecodeProcsFromRawGcx(procedureBlock.Procedures);

                    Procedure decodedMain = ProcDecoder.DecodeProc(mainProcedure.RawContents);
                    Main main = new()
                    {
                        EncodedContents = mainProcedure.RawContents,
                        DecodedContents = decodedMain.DecodedContents
                    };
                    gcx.Main = main;
                    
                    return gcx;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("Failed to import gcx file, no error handling for this case", ex);
            }
        }

        public static void AssembleReencodedFile(Gcx gcx, Dictionary<Procedure, byte[]> reEncodedProcs, string outputFile = "lastModifiedGcx.gcx")
        {
            KeyValuePair<Procedure, byte[]> mainProc = reEncodedProcs.Last();
            byte[] customMain = mainProc.Value;
            byte[] mainSize = BitConverter.GetBytes((uint)customMain.Length);
            reEncodedProcs.Remove(mainProc.Key);

            byte[] fileContents = gcx.FileContents;
            FileTable fileTable = gcx.FileTable;
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

            byte[] wholeFileReencoded = new byte[preamble.Length + procTableSize + constantData.Length + procBodyCollection.Length + 8 + customMain.Length];
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
            Array.Copy(mainSize, 0, wholeFileReencoded, position, 4);
            position += 4;
            Array.Copy(customMain, 0, wholeFileReencoded, position, customMain.Length);

            File.WriteAllBytes(outputFile, wholeFileReencoded);
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
            Dictionary<byte[], byte[]> procedureTable = new();
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
            Dictionary<string, uint> fileTable = new()
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

        private static dynamic ParseResourceDataTable(byte[] resourceData)
        {
            int position = 0;
            List<int> stringResourceStartingPositions = new();
            while (position < resourceData.Length)
            {
                stringResourceStartingPositions.Add(BitConverter.ToInt32(TakeRangeFromArray(resourceData, position, position+=4)) & 0xFFFFFF);
            }
            return stringResourceStartingPositions;
        }

        private static byte[] GetStringData(byte[] gcxContents)
        {
            return gcxContents.Take(new Range(new Index((int)FileTable["stringsOffset"] + cursor), new Index((int)FileTable["fontOffset"] + cursor))).ToArray();
        }

        class GcxString
        {
            public int StartingPosition { get; set; }
            public string Name { get; set; }
            public string Region { get; set; }
            public Literal ID { get; set; }
        }

        class DogTagDesignation
        {
            public int StartingPosition { get; set; }
            public byte Id { get; set; }
            public List<GcxString> Strings { get; set; } = new();
        }

        private static GcxString ParseGcxString(byte[] stringData, ref int position)
        {
            GcxString gcxString = new GcxString();
            gcxString.StartingPosition = position;
            position++;
            int gcxStringLength = stringData[position];
            position++;
            gcxString.Name = Encoding.ASCII.GetString(TakeRangeFromArray(stringData, position, position + gcxStringLength - 1));
            position += gcxStringLength;
            if (stringData[position] == 0x07)
            {
                position++;
                gcxStringLength = stringData[position];
                position++;
                gcxString.Region = Encoding.ASCII.GetString(TakeRangeFromArray(stringData, position, position + gcxStringLength - 1));
                position += gcxStringLength;
            }
            else
            {
                //unexpected, doesnt happen at all in w12c - yippee
            }
            ExpressionElements.DataTypeEnum dataType = (ExpressionElements.DataTypeEnum)stringData[position];
            position++;
            int idLength = ExpressionElements.DataTypeLength(dataType);
            byte[] idArray = TakeRangeFromArray(stringData, position, position + idLength).Reverse().ToArray();
            Literal literal = new Literal { DataType = dataType, Value = Convert.ToHexString(idArray) };
            gcxString.ID = literal;
            position += idLength;

            return gcxString;
        }

        private static dynamic ParseStringData(byte[] stringData, List<int> stringDataTable)
        {
            int dataPosition = 0;
            List<byte[]> stringResources = new();
            
            for (int i = 0; i < stringDataTable.Count; i++)
            {
                if (i + 1 < stringDataTable.Count)
                    stringResources.Add(TakeRangeFromArray(stringData, stringDataTable[i], stringDataTable[i + 1]));
                else
                    stringResources.Add(TakeRangeFromArray(stringData, stringDataTable[i], stringData.Length - 1));
            }

            //File.WriteAllText("stringData.json", JsonConvert.SerializeObject(stringResources, Formatting.Indented));

            List<DogTagDesignation> dogTagDesignations = new();
            foreach (byte[] stringResource in stringResources)
            {
                int resourcePosition = 0;
                if (stringResource[resourcePosition] == 0x02)
                {
                    DogTagDesignation dog = new DogTagDesignation();
                    dog.StartingPosition = resourcePosition;
                    resourcePosition++;
                    dog.Id = stringResource[resourcePosition++];
                    if(stringResource[resourcePosition] == 0x07)
                    {
                        GcxString tag1 = ParseGcxString(stringResource, ref resourcePosition);
                        dog.Strings.Add(tag1);
                        GcxString tag2 = ParseGcxString(stringResource, ref resourcePosition);
                        dog.Strings.Add(tag2);
                        dogTagDesignations.Add(dog);
                    }
                    else
                    {
                        //this shouldnt ever occur, theoretically?
                        byte currentByte = stringResource[resourcePosition];
                    }
                }
                else
                {
                    byte currentByte = stringResource[resourcePosition];
                    //58 on the first string resource in w12c
                    //C0 on the dummy array in w12c
                    //3rd to last in w12c seems like some kind of dictionary? (starts on 61 27 00 20 86 03)
                    //C9 9E 00 20 03 9F 00 20 FF FF FF FF is the second to last one
                    //44 9F 00 20 86 9F 00 20 FE FF FF FF C9 9F 00 20 FF FF FF is the last one
                }
            }


            return dogTagDesignations;
            //Leaving the below in for reference for now
            //currently using w12c for testing and figuring this shit out
            //one full block of a string?:
            /*
             * 02 80 
             * 07 13 4D 65 67 75 6D 69 20 4E 61 6B 61 6E 69 69 68 61 72 61 00 
             * 07 04 4A 50 4E 00 
             * 09 16 07 77 19 
             * 07 14 4A 61 63 71 75 65 6C 69 6E 65 20 44 20 42 65 6E 7A 6F 6E 00 
             * 07 04 4A 50 4E 00 
             * 09 03 10 81 19 00
             */
            byte stringMarker = 0x07;
            int position = 0;
            List<DogTagDesignation> dogtags = new();
            List<DogTagDesignation> badtags = new();
            List<string> strings = new();
            List<int> floats = new();

            while (position < stringData.Length)
            {
                byte currentByte = stringData[position];
                if (stringData[position] == 0x02)
                {
                    //Start of Dogtag designation?
                    DogTagDesignation dog = new DogTagDesignation();
                    dog.StartingPosition = position;
                    position++;
                    dog.Id = stringData[position++];
                    if (stringData[position] == 0x07)
                    {
                        GcxString string1 = ParseGcxString(stringData, ref position);
                        dog.Strings.Add(string1);
                        GcxString string2 = ParseGcxString(stringData, ref position);
                        dog.Strings.Add(string2);
                        dogtags.Add(dog);
                        //As far as I can tell, these are parsing out correctly, but the rest of the block needs work
                    }
                    else
                    {
                        badtags.Add(dog);
                    }
                    
                }
                else if(stringData[position] == 0x07)
                {
                    //start of string
                    position++;
                    int stringLength = stringData[position];
                    position++;
                    string embeddedString = Encoding.ASCII.GetString(TakeRangeFromArray(stringData, position, position + stringLength - 1));
                    strings.Add(embeddedString);
                    position += stringLength;
                }
                else if (stringData[position] == 0x09)
                {
                    //start of float designation
                    position++;
                    floats.Add(BitConverter.ToInt32(stringData, position));
                    position += 4;
                }
                else
                {
                    position++;
                }
            }
            return stringData;
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
                    Name = BitConverter.ToString(name.Reverse().ToArray().TakeLast(3).ToArray()).Replace("-","")
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
