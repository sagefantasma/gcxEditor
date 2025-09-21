using GcxEditor;
using Newtonsoft.Json;

namespace GcxEditorGUI
{
    public partial class GcxEditorGUI : Form
    {
        private GcxClasses.Gcx _loadedGcx;
        private string _loadedJson;
        private Procedure _displayedProcedure;
        private static RichTextBoxFinds richTextBoxFinds = RichTextBoxFinds.None;

        public GcxEditorGUI()
        {
            InitializeComponent();
        }

        private void procedureListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loadedGcx != null)
            {
                _displayedProcedure = _loadedGcx.ProcBlock.Procedures.FirstOrDefault(proc => (procedureListBox.SelectedItem as string)!.Contains(proc.Name))!;
                int location = richTextBox.Find($"\"Name\": \"{_displayedProcedure.Name}\"", richTextBox.SelectionStart + 1, -1, richTextBoxFinds);
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
            }
        }

        private void LoadProcedureList(GcxClasses.Gcx gcxFile)
        {
            foreach (Procedure procedure in gcxFile.ProcBlock.Procedures)
            {
                procedureListBox.Items.Add(procedure.Name);
            }
            
            //procedureListBox.Items.Add(gcxFile.Main.Name); //TODO: add support for main as well :)
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void exportModifiedgcxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveActiveJsonFile();
            Dictionary<Procedure, byte[]> reEncodedProcs = Importer.ImportJsonFile(_loadedJson);
            Dictionary<Procedure, byte[]> rawReEncodes = Importer.EncodeProcsFromRawGcx(_loadedGcx.ProcBlock.Procedures); //is this necessary?
            Importer.AssembleReencodedFile(_loadedGcx, reEncodedProcs, $"{_loadedJson.Split(".")[0]}.gcx");
        }

        private void savejsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveActiveJsonFile();
        }

        private void SaveActiveJsonFile()
        {
            File.WriteAllText(_loadedJson, richTextBox.Text);
        }
    }
}
