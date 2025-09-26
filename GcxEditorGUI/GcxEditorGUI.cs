using GcxEditor;
using Newtonsoft.Json;

namespace GcxEditorGUI
{
    public partial class GcxEditorGUI : Form
    {
        private GcxClasses.Gcx? LoadedGcx { get; set; }
        private string? LoadedGcxLocation { get; set; }
        private string? LoadedJson { get; set; }
        private string? ExportLocation { get; set; }
        private Procedure? DisplayedProcedure { get; set; }
        private readonly static RichTextBoxFinds richTextBoxFinds = RichTextBoxFinds.None;
        private readonly List<DictionaryEntry>? dictionaryEntries = new();
        private bool _overwriteLoadedGcx = false;

        public GcxEditorGUI()
        {
            InitializeComponent();
            dictionaryEntries = JsonConvert.DeserializeObject<List<DictionaryEntry>>(File.ReadAllText("dictionary.json"));
        }

        private void ProcedureListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LoadedGcx != null)
            {
                int location;
                string name = (procedureListBox.SelectedItem as string)!;
                if (!string.Equals(name!.ToLower(), "main"))
                {
                    if (name.Contains("("))
                    {
                        name = name.Split("(")[1].Split(")")[0];
                    }
                    DisplayedProcedure = LoadedGcx.ProcBlock.Procedures.FirstOrDefault(proc => name.Contains(proc.Name))!;
                    location = richTextBox.Find($"\"Name\": \"{DisplayedProcedure.Name}\"", richTextBox.SelectionStart + 1, -1, richTextBoxFinds);
                }
                else
                {
                    DisplayedProcedure = LoadedGcx.Main;
                    location = richTextBox.Find($"\"Type\": \"Main\"", richTextBox.SelectionStart + 1, -1, richTextBoxFinds);
                }

                if (location == -1)
                {
                    richTextBox.Find($"\"Name\": \"{DisplayedProcedure!.Name}\"", richTextBoxFinds);
                }
                richTextBox.ScrollToCaret();
            }
        }

        private void LoadGcxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = false,
                DefaultExt = "gcx",
                Title = "Select a GCX file to edit"
            };
            toolStripStatusLabel.Text = "Waiting for a .gcx to be selected...";
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                LoadedGcxLocation = openFileDialog.FileName;
                FileInfo fileInfo = new(LoadedGcxLocation);
                LoadedGcx = Importer.ImportGcxFile(LoadedGcxLocation);
                richTextBox.Enabled = true;

                JsonSerializerSettings jsonSerializerSettings = new()
                {
                    Formatting = Formatting.Indented
                };
                LoadedJson = $"{fileInfo.Name}.json";
                if (File.Exists(LoadedJson))
                {
                    string jsonText = File.ReadAllText(LoadedJson);
                    richTextBox.Text = jsonText;
                }
                else
                {
                    string jsonText = JsonConvert.SerializeObject(LoadedGcx, jsonSerializerSettings);
                    richTextBox.Text = jsonText;
                    File.WriteAllText(LoadedJson, jsonText);
                }
                LoadProcedureList(LoadedGcx);

                savejsonToolStripMenuItem.Enabled = true;
                exportModifiedgcxToolStripMenuItem.Enabled = true;
                toolStripStatusLabel.Text = "Loading dictionary...";
                Application.DoEvents();
                richTextBox.Text = ReplaceDictionaryValues(true, richTextBox.Text);
                toolStripStatusLabel.Text = ".gcx loaded!";
            }
        }

        private void LoadProcedureList(GcxClasses.Gcx gcxFile)
        {
            foreach (Procedure procedure in gcxFile.ProcBlock.Procedures)
            {
                DictionaryEntry? dictEntry = dictionaryEntries?.FirstOrDefault(x => x.StrCode == procedure.Order);
                if (dictEntry != null)
                {
                    procedureListBox.Items.Add($"{dictEntry.Name} ({procedure.Name})");
                }
                else
                {
                    procedureListBox.Items.Add(procedure.Name);
                }
            }

            procedureListBox.Items.Add("main");
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ExportNewGcx(string fileLocation)
        {
            ExportLocation = fileLocation;
            SaveActiveJsonFile();
            Dictionary<Procedure, byte[]> reEncodedProcs = Importer.ImportJsonFile(LoadedJson!);
            Importer.AssembleReencodedFile(LoadedGcx!, reEncodedProcs, fileLocation);
        }

        private void SaveJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveActiveJsonFile();
        }

        private static bool VerifyJson(string textToVerify)
        {
            bool isValid = false;

            try
            {
                GcxClasses.Gcx currentState = JsonConvert.DeserializeObject<GcxClasses.Gcx>(textToVerify)!;
                Dictionary<Procedure, byte[]> procs = Importer.EncodeProcsFromJson(currentState.ProcBlock.Procedures);
                isValid = true;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Invalid gcx json: {e}");
            }

            return isValid;
        }

        private string ReplaceDictionaryValues(bool replaceWithValue, string textToModify)
        {
            if (replaceWithValue)
            {
                toolStripProgressBar.Value = 0;
                toolStripProgressBar.Maximum = dictionaryEntries!.Count;
                foreach (DictionaryEntry dictionaryEntry in dictionaryEntries)
                {
                    if (textToModify.Contains(dictionaryEntry.StrCode.ToString()))
                        textToModify = textToModify.Replace(dictionaryEntry.StrCode.ToString(), $"\"{dictionaryEntry.Name}\"");
                    toolStripProgressBar.Value++;
                }
            }
            else
            {
                toolStripProgressBar.Value = 0;
                toolStripProgressBar.Maximum = dictionaryEntries!.Count;
                foreach (DictionaryEntry dictionaryEntry in dictionaryEntries!)
                {
                    if (textToModify.Contains(dictionaryEntry.Name))
                        textToModify = textToModify.Replace($"\"{dictionaryEntry.Name}\"", dictionaryEntry.StrCode.ToString());
                    toolStripProgressBar.Value++;
                }
            }

            return textToModify;
        }

        private void SaveActiveJsonFile()
        {
            string unDictionariedString = ReplaceDictionaryValues(false, richTextBox.Text);
            if (VerifyJson(unDictionariedString))
                File.WriteAllText(LoadedJson!, unDictionariedString);
        }

        private void RichTextBox_SelectionChanged(object sender, EventArgs e)
        {
            
        }

        private void ReplaceOpenedFileOnExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (replaceOpenedFileOnExportToolStripMenuItem.Checked)
            {
                _overwriteLoadedGcx = true;
            }
            else
            {
                _overwriteLoadedGcx = false;
            }
        }

        private void ChooseLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                DefaultExt = ".gcx",
                OverwritePrompt = true
            };
            DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                ExportNewGcx(saveFileDialog.FileName);
            }
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel.Text = "Exporting new .gcx with json edits...";
            Application.DoEvents();
            if (!_overwriteLoadedGcx)
            {
                if (string.IsNullOrEmpty(ExportLocation))
                    ExportNewGcx($"{LoadedJson!.Split(".")[0]}.gcx");
                else
                    ExportNewGcx(ExportLocation);
            }
            else
                ExportNewGcx(LoadedGcxLocation!);
            toolStripStatusLabel.Text = ".gcx successfully exported!";
        }
    }
}
