using GcxEditor;
using GcxEditorGUI.Object_Controls;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

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
            CheckForUpdates();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private void CheckForUpdates()
        {
            FileVersionInfo appInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
            string appVersion = appInfo.FileVersion!;

            bool newUpdateExists = VersionSupport.CheckIfNewUpdateExists(appVersion);
            if (newUpdateExists)
            {
                DialogResult dialogResult = MessageBox.Show("Your version of the GCX Editor is out-of-date. Would you like to go to the releases page to get the latest version?", "Out-of-date warning", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Process.Start("https://github.com/sagefantasma/gcxEditor/releases");
                }
            }
        }

        private void UpdateStatusStrip(string inputString, bool resetAfter = false)
        {
            Task.Factory.StartNew(() =>
            {
                Invoke((MethodInvoker)delegate
                {
                    toolStripStatusLabel.Text = inputString;
                    Application.DoEvents();
                });
                if (resetAfter)
                {
                    Thread.Sleep(5000);
                }
                Invoke((MethodInvoker)delegate
                {
                    toolStripStatusLabel.Text = "";
                    if (resetAfter)
                    {
                        toolStripProgressBar.Value = 0;
                    }
                });
            });
            Application.DoEvents();
        }

        private void ProcedureListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LoadedGcx != null)
            {
                int location;
                string name = (procedureListBox.SelectedItem as string)!;
                DictionaryEntry? dictionaryEntry = null;
                if (!string.Equals(name!.ToLower(), "main"))
                {
                    if (name.Contains("("))
                    {
                        name = name.Split("(")[1].Split(")")[0];
                    }
                    DisplayedProcedure = LoadedGcx.ProcBlock.Procedures.FirstOrDefault(proc => name.Contains(proc.Name, StringComparison.OrdinalIgnoreCase))!;
                    dictionaryEntry = dictionaryEntries!.FirstOrDefault(x => DisplayedProcedure.Name.ToLower().Contains(x.StrCode.ToLower()));
                    if (dictionaryEntry != default)
                    {
                        location = richTextBox.Find($"\"Name\": \"{dictionaryEntry.Name}\"", richTextBox.SelectionStart + 1, -1, richTextBoxFinds);
                    }
                    else
                    {
                        location = richTextBox.Find($"\"Name\": \"{DisplayedProcedure.Name}\"", richTextBox.SelectionStart + 1, -1, richTextBoxFinds);
                    }
                }
                else
                {
                    DisplayedProcedure = LoadedGcx.Main;
                    location = richTextBox.Find($"\"Type\": \"Main\"", richTextBox.SelectionStart + 1, -1, richTextBoxFinds);
                }

                if (location == -1)
                {
                    if (dictionaryEntry != default)
                    {
                        location = richTextBox.Find($"\"Name\": \"{dictionaryEntry.Name}\"", richTextBoxFinds);
                    }
                    else
                    {
                        richTextBox.Find($"\"Name\": \"{DisplayedProcedure!.Name}\"", richTextBoxFinds);
                    }
                }
                richTextBox.ScrollToCaret();
            }

            if(tabControl1.SelectedIndex == 1)
            {
                InteractiveLoadActiveProc();
            }
        }

        private void CreateNewJsonFile(string fileName)
        {
            JsonSerializerSettings jsonSerializerSettings = new()
            {
                Formatting = Formatting.Indented
            };

            string jsonText = JsonConvert.SerializeObject(LoadedGcx, jsonSerializerSettings);
            richTextBox.Text = jsonText;
            File.WriteAllText(fileName, jsonText);
        }

        private void LoadGcxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseFileToolStripMenuItem_Click(sender, e);
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = false,
                DefaultExt = "gcx",
                Title = "Select a GCX file to edit"
            };

            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                LoadedGcxLocation = openFileDialog.FileName;
                FileInfo fileInfo = new(LoadedGcxLocation);
                LoadedGcx = Importer.ImportGcxFile(LoadedGcxLocation);
                richTextBox.Enabled = true;

                
                LoadedJson = $"{fileInfo.Name}.json";
                if (File.Exists(LoadedJson))
                {
                    dialogResult = MessageBox.Show("A json file was found for the selected file; would you like to load it instead of starting fresh? (No will overwrite existing file)", "Existing JSON detected!", MessageBoxButtons.YesNoCancel);
                    if (dialogResult == DialogResult.Yes)
                    {
                        string jsonText = File.ReadAllText(LoadedJson);
                        richTextBox.Text = jsonText;
                    }
                    else if(dialogResult == DialogResult.No)
                    {
                        CreateNewJsonFile(LoadedJson);
                    }
                    else
                    {
                        CloseFileToolStripMenuItem_Click(null, null);
                        return;
                    }
                }
                else
                {
                    CreateNewJsonFile(LoadedJson);
                }
                LoadProcedureList(LoadedGcx);

                savejsonToolStripMenuItem.Enabled = true;
                exportModifiedgcxToolStripMenuItem.Enabled = true;
                closeFileToolStripMenuItem.Enabled = true;
                UpdateStatusStrip("Loading dictionary...");
                richTextBox.Text = ReplaceDictionaryValues(true, richTextBox.Text);
                UpdateStatusStrip(".gcx loaded!", true);
            }
        }

        private void LoadProcedureList(GcxClasses.Gcx gcxFile)
        {
            UpdateStatusStrip("Loading procedure list...");
            foreach (Procedure procedure in gcxFile.ProcBlock.Procedures)
            {
                DictionaryEntry? dictEntry = dictionaryEntries?.FirstOrDefault(x => $"0x{x.StrCode}" == procedure.Name);
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
            UpdateStatusStrip("Exporting new .gcx with json edits...");
            ExportLocation = fileLocation;
            SaveActiveJsonFile();
            Dictionary<Procedure, byte[]> reEncodedProcs = Importer.ImportJsonFile(LoadedJson!);
            Importer.AssembleReencodedFile(LoadedGcx!, reEncodedProcs, fileLocation);
            UpdateStatusStrip(".gcx successfully exported!", true);
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
                    if (textToModify.Contains($"0x{dictionaryEntry.StrCode.ToUpper()}"))
                        textToModify = textToModify.Replace($"\"0x{dictionaryEntry.StrCode}\"", $"\"{dictionaryEntry.Name}\"");
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
                        textToModify = textToModify.Replace($"\"{dictionaryEntry.Name}\"", $"\"0x{dictionaryEntry.StrCode.ToUpper()}\"");
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
                AddExtension = true,
                DefaultExt = ".gcx",
                Filter = "GCX File (*.gcx)|*.gcx",
                OverwritePrompt = true,
            };
            DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                ExportNewGcx(saveFileDialog.FileName);
            }
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_overwriteLoadedGcx)
            {
                if (string.IsNullOrEmpty(ExportLocation))
                    ExportNewGcx($"{LoadedJson!.Split(".")[0]}.gcx");
                else
                    ExportNewGcx(ExportLocation);
            }
            else
                ExportNewGcx(LoadedGcxLocation!);
        }

        private void CloseFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            procedureListBox.Items.Clear();
            richTextBox.Text = string.Empty;
            LoadedGcx = null;
            LoadedGcxLocation = null;
            LoadedJson = null;
            ExportLocation = null;
            DisplayedProcedure = null;
            savejsonToolStripMenuItem.Enabled = false;
            exportModifiedgcxToolStripMenuItem.Enabled = false;
            closeFileToolStripMenuItem.Enabled = false;
        }

        private void UpdateJsonTab()
        {
            //TODO: whenever we do a change to the INTERACTIVE tab, we should update the json tab
        }

        private void UpdateInteractiveTab()
        {
            //whenever we do a change to the JSON tab, we should update the interactive tab?
        }

        private void InteractiveLoadProc(Procedure procedure, FlowLayoutPanel masterPanel)
        {
            //TODO: create new custom flowpanel that has a text label for better signaling of each nested object
            foreach (IProcedureElement item in procedure.DecodedContents)
            {
                if (item is Print printItem)
                {
                    GenericStatementUC printUC = new GenericStatementUC();
                    printUC.nameLabel.Text = "Print Statement";
                    printUC.argLabel.Text = "Text to print:";
                    byte[] byteString = Convert.FromBase64String(printItem.Args.FirstOrDefault().ToString());
                    printUC.argContentsTextBox.Text = Encoding.GetEncoding("euc-jp").GetString(byteString.ToArray());
                    masterPanel.Controls.Add(printUC);
                }
                else if (item is Return returnItem)
                {
                    GenericStatementUC returnUC = new GenericStatementUC();
                    returnUC.nameLabel.Text = "Return Statement";
                    returnUC.argLabel.Text = "Value to return:";
                    returnUC.argContentsTextBox.Text = returnItem.Args.FirstOrDefault().ToString();
                    masterPanel.Controls.Add(returnUC);
                }
                else if (item is Msg msgItem)
                {
                    GenericStatementUC msgUC = new GenericStatementUC(); //TODO: break this down further? Is it really just a generic statement?
                    msgUC.nameLabel.Text = "Message Statement";
                    msgUC.argLabel.Text = "Message to send:";
                    string messageString = "";
                    foreach (ITerm arg in msgItem.Args)
                    {
                        messageString += arg.ToString();
                        if (arg != msgItem.Args[^1])
                            messageString += ", ";
                    }
                    msgUC.argContentsTextBox.Text = messageString;
                    masterPanel.Controls.Add(msgUC);
                }
                else if (item is Load loadItem)
                {
                    GenericStatementUC loadUC = new GenericStatementUC(); //TODO: fix this, is not generic statement - can have params
                    loadUC.nameLabel.Text = "Load Statement";
                    loadUC.argLabel.Text = "Stage to load:";
                    loadUC.argContentsTextBox.Text = loadItem.Args.FirstOrDefault().ToString(); //TODO: stored as base64 string, need to convert
                    masterPanel.Controls.Add(loadUC);
                }
                else if (item is Restart restart)
                {
                    GenericStatementUC restartUC = new GenericStatementUC(); //TODO: change this to a different type of UC?
                    restartUC.nameLabel.Text = "Restart Statement";
                    if (restart.Parameters.Count == 0)
                    {
                        restartUC.argLabel.Text = "";
                        restartUC.argContentsTextBox = null;
                    }
                    else
                    {
                        restartUC.argLabel.Text = "Parameters";
                        restartUC.argContentsTextBox.Text = restart.Parameters.ToString();
                    }
                    masterPanel.Controls.Add(restartUC);
                }
                else if (item is UnknownCommand load2)
                {
                    GenericStatementUC unknownUC = new GenericStatementUC();
                    unknownUC.nameLabel.Text = "Load2 Statement";
                    unknownUC.argLabel.Text = "Stage to load:";
                    unknownUC.argContentsTextBox.Text = load2.Args.FirstOrDefault().ToString();
                    masterPanel.Controls.Add(unknownUC);
                }
                else if (item is Procedure subProc)
                {
                    //TODO: should be fine
                    NestableUC subProcPanel = new();
                    subProcPanel.nameLabel.Text = "Subproc";
                    InteractiveLoadProc(subProc, subProcPanel.contentFlowPanel);
                    masterPanel.Controls.Add(subProcPanel);
                }
                else if(item is Trap trap)
                {
                    //TODO: flesh out
                    NestableUC trapPanel = new();
                    trapPanel.nameLabel.Text = "Trap";
                    InteractiveLoadProc(trap.Parameters.First(x => x.ParamType == 'e').Args[0] as Procedure, trapPanel.contentFlowPanel);
                    masterPanel.Controls.Add(trapPanel);
                }
                else if(item is IfBlock ifBlock)
                {
                    //TODO: need to finish
                    NestableUC ifBlockPanel = new();
                    ifBlockPanel.nameLabel.Text = "If Block";
                    InteractiveLoadProc(ifBlock.Args[1] as Procedure, ifBlockPanel.contentFlowPanel);
                    foreach(Parameter param in ifBlock.Parameters)
                    {
                        if (param.ParamType != 'e')
                            InteractiveLoadProc(param.Args[1] as Procedure, ifBlockPanel.contentFlowPanel);
                        else
                            InteractiveLoadProc(param.Args[0] as Procedure, ifBlockPanel.contentFlowPanel);
                    }
                    masterPanel.Controls.Add(ifBlockPanel);
                }
                else if(item is Chara chara)
                {
                    //TODO: need to clean this up
                    NestableUC charaPanel = new();
                    charaPanel.nameLabel.Text = "";
                    CharaUC charaUC = new CharaUC();
                    charaUC.typeTextBox.Text = chara.Args[0].ToString();
                    charaUC.idTextBox.Text = chara.Args[1].ToString();
                    string paramsString = "";
                    foreach(Parameter parameter in chara.Parameters)
                    {
                        paramsString += parameter.ToString() + Environment.NewLine;
                    }
                    charaUC.paramTextBox.Text = paramsString;
                    charaPanel.contentFlowPanel.Controls.Add(charaUC);
                    if(chara.Parameters.Any(x=>x.ParamType == 'e'))
                        InteractiveLoadProc(chara.Parameters.First(x => x.ParamType == 'e').Args[0] as Procedure, charaPanel.contentFlowPanel);
                    if (chara.Parameters.Any(x => x.ParamType == 'x'))
                        InteractiveLoadProc(chara.Parameters.First(x => x.ParamType == 'x').Args[0] as Procedure, charaPanel.contentFlowPanel);
                    masterPanel.Controls.Add(charaPanel);
                }
                else if(item is GameCommand gameCommand)
                {
                    //TODO: flesh out
                    NestableUC gameCommandPanel = new();
                    gameCommandPanel.nameLabel.Text = "GameCommand";
                    if (gameCommand.Parameters.Any(x => x.ParamType == 's'))
                    {
                        try
                        {
                            if(gameCommand.Parameters.First(x => x.ParamType == 's').Args[0] is Procedure scriptProc)
                                InteractiveLoadProc(scriptProc, gameCommandPanel.contentFlowPanel);
                        }
                        catch (Exception ex)
                        {
                            //just not a script
                        }
                    }
                    masterPanel.Controls.Add(gameCommandPanel);
                }
                else if(item is GcxEditor.Invoke invokeCommand)
                {
                    //TODO: finish
                    GenericStatementUC invokeUC = new();
                    invokeUC.nameLabel.Text = "Invoke Statement";
                    invokeUC.argLabel.Text = "Procedure Invoked:";
                    invokeUC.argContentsTextBox.Text = invokeCommand.ProcedureInvoked.Name;
                    //TODO: add args passed to invoke
                    masterPanel.Controls.Add(invokeUC);
                }
                else if(item is Expression expression)
                {
                    //TODO: finish
                    GenericStatementUC expressionUC = new();
                    expressionUC.nameLabel.Text = "Expression Statement";
                    expressionUC.argLabel.Text = "Expression:";
                    expressionUC.argContentsTextBox.Text = expression.ToString();
                    masterPanel.Controls.Add(expressionUC);
                }
                else if(item is SwitchBlock switchBlock)
                {
                    //TODO: finish
                    NestableUC switchBlockPanel = new();
                    switchBlockPanel.nameLabel.Text = "Switch Block";
                    masterPanel.Controls.Add(switchBlockPanel);
                }
            }
        }

        private void InteractiveLoadActiveProc()
        {
            flowLayoutPanel.Controls.Clear();
            if (DisplayedProcedure == null)
            {
                return;
            }
            InteractiveLoadProc(DisplayedProcedure!, flowLayoutPanel);
        }

        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
            //Save json file?
            if(tabControl1.SelectedIndex == 0)
            {
                //going to json tab
            }
            else
            {
                //going to interactive tab
                InteractiveLoadActiveProc();
            }
        }
    }
}
