using GcxEditor;
using Newtonsoft.Json;

namespace GcxEditorGUI
{
    public partial class GcxEditorGUI : Form
    {
        private GcxClasses.Gcx? _loadedGcx;
        private string? _loadedJson;
        private Procedure? _displayedProcedure;
        private static RichTextBoxFinds richTextBoxFinds = RichTextBoxFinds.None;
        private List<DictionaryEntry>? dictionaryEntries = new List<DictionaryEntry>();

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
                string filename = openFileDialog.FileName;
                FileInfo fileInfo = new FileInfo(filename);
                _loadedGcx = Importer.ImportGcxFile(filename);
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

        private void exportModifiedgcxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel.Text = "Exporting new .gcx with json edits...";
            SaveActiveJsonFile();
            Dictionary<Procedure, byte[]> reEncodedProcs = Importer.ImportJsonFile(_loadedJson!);
            Importer.AssembleReencodedFile(_loadedGcx!, reEncodedProcs, $"{_loadedJson!.Split(".")[0]}.gcx");
            toolStripStatusLabel.Text = ".gcx successfully exported!";
        }

        private void savejsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveActiveJsonFile();
        }

        private bool VerifyJson()
        {
            bool isValid = false;

            try
            {
                GcxClasses.Gcx currentState = JsonConvert.DeserializeObject<GcxClasses.Gcx>(richTextBox.Text);
                Dictionary<Procedure, byte[]> procs = Importer.EncodeProcsFromJson(currentState.ProcBlock.Procedures);
                isValid = true;
            }
            catch(Exception e)
            {
                MessageBox.Show($"Invalid gcx json: {e}");
            }

            return isValid;
        }

        private void SaveActiveJsonFile()
        {
            if(VerifyJson())
                File.WriteAllText(_loadedJson!, richTextBox.Text);
        }

        CancellationTokenSource searchCancelTokenSource = new CancellationTokenSource();

        private void richTextBox_SelectionChanged(object sender, EventArgs e)
        {
            //The goal with this function will be to display a tooltip/textbox near the selectedtext
            //to inform the user as to what, if anything, the selected strcode corresponds to
            if(richTextBox.SelectedText.Length > 2)
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
    }
}
