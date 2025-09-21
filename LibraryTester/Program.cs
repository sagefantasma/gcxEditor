using GcxEditor;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace LibraryTester
{
    
    internal class Program
    {
        internal class DictionaryEntry
        {
            public int StrCode;
            public string Name;

            public DictionaryEntry(int strCode, string name)
            {
                StrCode = strCode;
                Name = name;
            }
        }

        private static List<DictionaryEntry> ParseDictionaryEntries(FileInfo file)
        {
            List<string> fileContents = File.ReadAllLines(file.FullName).ToList();
            List<DictionaryEntry> dictionaryEntries = new List<DictionaryEntry>();
            foreach (string line in fileContents)
            {
                string[] parts = line.Split(' ');
                DictionaryEntry entry = new DictionaryEntry(int.Parse(parts[0].Split('x')[1], System.Globalization.NumberStyles.HexNumber), parts[1]);
                dictionaryEntries.Add(entry);
            }

            return dictionaryEntries;
        }

        static void Main(string[] args)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo("C:\\Users\\Andy\\repos\\gcx_decompiler\\dictionaries");
            List<DictionaryEntry> allDictionaryEntries = new List<DictionaryEntry>();
            foreach(var file in directoryInfo.GetFiles())
            {
                allDictionaryEntries.AddRange(ParseDictionaryEntries(file));
            }
            File.WriteAllText("masterDictionary.json",JsonConvert.SerializeObject(allDictionaryEntries));
            /*foreach (string file in Directory.GetFiles("C:\\Users\\yonan\\Documents\\Pinned Folders\\C Drive Steam Games\\MGS2\\assets\\gcx\\eu\\_bp"))
            {
                //issue in scenerio.gcx
                //issue in scenerio_stage_a00b.gcx
                if (file.Contains("scenerio.gcx") || file.Contains("scenerio_stage_a00b.gcx") || file.Contains("scenerio_stage_a00c.gcx"))
                {
                    continue;
                }
                if (!file.Contains("_w"))
                {
                    continue;
                }
            if (file.Contains("bak"))
                {
                    continue;
                }
                GcxEditor.Importer.ImportGcxFile(file);
            }*/

            Dictionary<GcxEditor.Procedure, byte[]> reEncodedProcs = GcxEditor.Importer.ImportJsonFile("C:\\Users\\yonan\\Source\\Repos\\gcxEditor\\LibraryTester\\bin\\Debug\\net8.0\\gcxOutput.json");
            GcxEditor.GcxClasses.Gcx gcxFile = GcxEditor.Importer.ImportGcxFile("C:\\Users\\yonan\\Documents\\Pinned Folders\\C Drive Steam Games\\MGS2\\assets\\gcx\\eu\\_bp\\scenerio_stage_w01a.gcx");
            Dictionary<Procedure, byte[]> rawReEncodes = Importer.EncodeProcsFromRawGcx(gcxFile.ProcBlock.Procedures);
            GcxEditor.Importer.AssembleReencodedFile(gcxFile, reEncodedProcs);
            //Importer.AssembleReencodedFile(gcxFile, rawReEncodes);
            Task serializeTask = SerializeIt(gcxFile);
            while (!serializeTask.IsCompleted)
            {

            }
        }

        static async Task SerializeIt(GcxEditor.GcxClasses.Gcx gcxFile)
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                //TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented
            };
            File.WriteAllText("gcxOutput.json", JsonConvert.SerializeObject(gcxFile, jsonSerializerSettings));
        }
    }
}
