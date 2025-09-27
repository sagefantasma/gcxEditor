using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace LibraryTester
{
    
    internal class Program
    {
        internal class DictionaryEntry
        {
            public string StrCode;
            public string Name;

            public DictionaryEntry(string strCode, string name)
            {
                StrCode = strCode;
                Name = name;
            }
        }

        private static List<DictionaryEntry> ParseDictionaryEntries(FileInfo file)
        {
            List<string> fileContents = File.ReadAllLines(file.FullName).ToList();
            List<DictionaryEntry> dictionaryEntries = new();
            foreach (string line in fileContents)
            {
                string[] parts = line.Split(' ');
                DictionaryEntry entry = new(parts[0].Split("0x")[1], parts[1]);
                dictionaryEntries.Add(entry);
            }

            return dictionaryEntries;
        }

        static void Main(string[] args)
        {
            List<DictionaryEntry> parsedDict = JsonConvert.DeserializeObject<List<DictionaryEntry>>(File.ReadAllText("masterDictionary.json"));
            int count = 0;
            Dictionary<DictionaryEntry, DictionaryEntry> turboDict = new();
            foreach(DictionaryEntry entry in parsedDict)
            {
                List<DictionaryEntry> sampleDict = parsedDict.ToList();
                sampleDict.Remove(entry);
                if (sampleDict.Any(x => x.StrCode.Contains(entry.StrCode)))
                {
                    if (!sampleDict.Any(x => x.StrCode == entry.StrCode))
                    {
                        Console.WriteLine($"FUCK#{count++}");
                    }
                    turboDict.Add(entry, sampleDict.First(x => x.StrCode.Contains(entry.StrCode)));
                }
            }
            //DirectoryInfo directoryInfo = new("C:\\Users\\Andy\\repos\\gcx_decompiler\\dictionaries");
            DirectoryInfo directoryInfo = new("C:\\Users\\yonan\\Source\\Repos\\MGS2-Cheat-Trainer\\gcx\\dictionaries");
            List<DictionaryEntry> allDictionaryEntries = new();
            foreach(var file in directoryInfo.GetFiles())
            {
                allDictionaryEntries.AddRange(ParseDictionaryEntries(file));
            }
            File.WriteAllText("masterDictionary.json",JsonConvert.SerializeObject(allDictionaryEntries));
        }
    }
}
