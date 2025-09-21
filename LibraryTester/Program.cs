using GcxEditor;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace LibraryTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
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
