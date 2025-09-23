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
        }
    }
}
