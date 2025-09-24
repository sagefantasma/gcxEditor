using GcxEditor;
using Newtonsoft.Json;

namespace GcxEditorGUI
{
    public partial class GcxEditorGUI : Form
    {
        private GcxClasses.Gcx? _loadedGcx;
        private string _loadedGcxLocation;
        private string? _loadedJson;
        private string _exportLocation;
        private Procedure? _displayedProcedure;
        private static RichTextBoxFinds richTextBoxFinds = RichTextBoxFinds.None;
        private List<DictionaryEntry>? dictionaryEntries = new List<DictionaryEntry>();
        private bool _overwriteLoadedGcx = false;

        public GcxEditorGUI()
        {
            InitializeComponent();
            dictionaryEntries = JsonConvert.DeserializeObject<List<DictionaryEntry>>(File.ReadAllText("dictionary.json"));
        }

        private void procedureListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loadedGcx != null)
            {
                int location;
                if ((procedureListBox.SelectedItem as string)!.ToLower() != "main")
                {
                    _displayedProcedure = _loadedGcx.ProcBlock.Procedures.FirstOrDefault(proc => (procedureListBox.SelectedItem as string)!.Contains(proc.Name))!;
                    location = richTextBox.Find($"\"Name\": \"{_displayedProcedure.Name}\"", richTextBox.SelectionStart + 1, -1, richTextBoxFinds);
                }
                else
                {
                    _displayedProcedure = _loadedGcx.Main;
                    location = richTextBox.Find($"\"Type\": \"Main\"", richTextBox.SelectionStart + 1, -1, richTextBoxFinds);
                }

                if (location == -1)
                {
                    richTextBox.Find($"\"Name\": \"{_displayedProcedure.Name}\"", richTextBoxFinds);
                }
                richTextBox.ScrollToCaret();
            }
        }

        private void loadgcxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                DefaultExt = "gcx",
                Title = "Select a GCX file to edit"
            };
            toolStripStatusLabel.Text = "Waiting for a .gcx to be selected...";
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                _loadedGcxLocation = openFileDialog.FileName;
                FileInfo fileInfo = new FileInfo(_loadedGcxLocation);
                _loadedGcx = Importer.ImportGcxFile(_loadedGcxLocation);
                richTextBox.Enabled = true;

                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                };
                _loadedJson = $"{fileInfo.Name}.json";
                if (File.Exists(_loadedJson))
                {
                    string jsonText = File.ReadAllText(_loadedJson);
                    richTextBox.Text = jsonText;
                }
                else
                {
                    string jsonText = JsonConvert.SerializeObject(_loadedGcx, jsonSerializerSettings);
                    richTextBox.Text = jsonText;
                    File.WriteAllText(_loadedJson, jsonText);
                }
                LoadProcedureList(_loadedGcx);

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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ExportNewGcx(string fileLocation)
        {
            _exportLocation = fileLocation;
            SaveActiveJsonFile();
            Dictionary<Procedure, byte[]> reEncodedProcs = Importer.ImportJsonFile(_loadedJson!);
            Importer.AssembleReencodedFile(_loadedGcx!, reEncodedProcs, fileLocation);
        }

        private void savejsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveActiveJsonFile();
        }

        private bool VerifyJson(string textToVerify)
        {
            bool isValid = false;

            try
            {
                GcxClasses.Gcx currentState = JsonConvert.DeserializeObject<GcxClasses.Gcx>(textToVerify);
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
                        textToModify = textToModify.Replace(dictionaryEntry.StrCode.ToString(), $"({dictionaryEntry.Name})");
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
                        textToModify = textToModify.Replace($"({dictionaryEntry.Name})", dictionaryEntry.StrCode.ToString());
                    toolStripProgressBar.Value++;
                }
            }

            return textToModify;
        }

        private void SaveActiveJsonFile()
        {
            string unDictionariedString = ReplaceDictionaryValues(false, richTextBox.Text);
            if (VerifyJson(unDictionariedString))
                File.WriteAllText(_loadedJson!, unDictionariedString);
        }

        CancellationTokenSource searchCancelTokenSource = new CancellationTokenSource();

        private void richTextBox_SelectionChanged(object sender, EventArgs e)
        {
            //The goal with this function will be to display a tooltip/textbox near the selectedtext
            //to inform the user as to what, if anything, the selected strcode corresponds to
            if (richTextBox.SelectedText.Length > 2)
            {
                searchCancelTokenSource.Cancel();
                searchCancelTokenSource = new CancellationTokenSource();
                string selectedText = new string(richTextBox.SelectedText);
                //TODO: hand SearchDictionaryForStrCode a delegate to a tooltip or textbox?
                ToolTip strCodeToolTip = new ToolTip();
                strCodeToolTip.OwnerDraw = false;
                Task searchTask = Task.Run(() => SearchDictionaryForStrCode(selectedText), searchCancelTokenSource.Token);
            }
        }

        private void SearchDictionaryForStrCode(string strCode)
        {
            DictionaryEntry? dictEntry = dictionaryEntries?.FirstOrDefault(entry => entry.StrCode.ToString("X") == strCode);
            if (dictEntry != null)
            {

            }
        }

        private void replaceOpenedFileOnExportToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void chooseLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".gcx";
            saveFileDialog.OverwritePrompt = true;
            DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                ExportNewGcx(saveFileDialog.FileName);
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel.Text = "Exporting new .gcx with json edits...";
            Application.DoEvents();
            if (!_overwriteLoadedGcx)
            {
                if (string.IsNullOrEmpty(_exportLocation))
                    ExportNewGcx($"{_loadedJson!.Split(".")[0]}.gcx");
                else
                    ExportNewGcx(_exportLocation);
            }
            else
                ExportNewGcx(_loadedGcxLocation);
            toolStripStatusLabel.Text = ".gcx successfully exported!";
        }
    }
}
