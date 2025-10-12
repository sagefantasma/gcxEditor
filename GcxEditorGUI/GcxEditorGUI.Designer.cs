namespace GcxEditorGUI
{
    partial class GcxEditorGUI
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            loadgcxToolStripMenuItem = new ToolStripMenuItem();
            savejsonToolStripMenuItem = new ToolStripMenuItem();
            exportModifiedgcxToolStripMenuItem = new ToolStripMenuItem();
            replaceOpenedFileOnExportToolStripMenuItem = new ToolStripMenuItem();
            chooseLocationToolStripMenuItem = new ToolStripMenuItem();
            exportToolStripMenuItem = new ToolStripMenuItem();
            closeFileToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            searchToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            jSONEditorToolStripMenuItem = new ToolStripMenuItem();
            interactiveEditorToolStripMenuItem = new ToolStripMenuItem();
            mainPanel = new Panel();
            tableLayoutPanel = new TableLayoutPanel();
            procedureListBox = new ListBox();
            tabControl1 = new TabControl();
            JsonViewTabPage = new TabPage();
            richTextBox = new RichTextBox();
            ObjectViewTabPage = new TabPage();
            flowLayoutPanel = new FlowLayoutPanel();
            statusStrip = new StatusStrip();
            toolStripProgressBar = new ToolStripProgressBar();
            toolStripStatusLabel = new ToolStripStatusLabel();
            menuStrip.SuspendLayout();
            mainPanel.SuspendLayout();
            tableLayoutPanel.SuspendLayout();
            tabControl1.SuspendLayout();
            JsonViewTabPage.SuspendLayout();
            ObjectViewTabPage.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.ImageScalingSize = new Size(24, 24);
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, searchToolStripMenuItem, viewToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Padding = new Padding(9, 3, 0, 3);
            menuStrip.Size = new Size(1806, 35);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadgcxToolStripMenuItem, savejsonToolStripMenuItem, exportModifiedgcxToolStripMenuItem, closeFileToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(54, 29);
            fileToolStripMenuItem.Text = "File";
            // 
            // loadgcxToolStripMenuItem
            // 
            loadgcxToolStripMenuItem.Name = "loadgcxToolStripMenuItem";
            loadgcxToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            loadgcxToolStripMenuItem.Size = new Size(278, 34);
            loadgcxToolStripMenuItem.Text = "Load .gcx...";
            loadgcxToolStripMenuItem.Click += LoadGcxToolStripMenuItem_Click;
            // 
            // savejsonToolStripMenuItem
            // 
            savejsonToolStripMenuItem.Enabled = false;
            savejsonToolStripMenuItem.Name = "savejsonToolStripMenuItem";
            savejsonToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            savejsonToolStripMenuItem.Size = new Size(278, 34);
            savejsonToolStripMenuItem.Text = "Save .json";
            savejsonToolStripMenuItem.Click += SaveJsonToolStripMenuItem_Click;
            // 
            // exportModifiedgcxToolStripMenuItem
            // 
            exportModifiedgcxToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { replaceOpenedFileOnExportToolStripMenuItem, chooseLocationToolStripMenuItem, exportToolStripMenuItem });
            exportModifiedgcxToolStripMenuItem.Enabled = false;
            exportModifiedgcxToolStripMenuItem.Name = "exportModifiedgcxToolStripMenuItem";
            exportModifiedgcxToolStripMenuItem.Size = new Size(278, 34);
            exportModifiedgcxToolStripMenuItem.Text = "Export modified .gcx";
            // 
            // replaceOpenedFileOnExportToolStripMenuItem
            // 
            replaceOpenedFileOnExportToolStripMenuItem.CheckOnClick = true;
            replaceOpenedFileOnExportToolStripMenuItem.Name = "replaceOpenedFileOnExportToolStripMenuItem";
            replaceOpenedFileOnExportToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.R;
            replaceOpenedFileOnExportToolStripMenuItem.Size = new Size(477, 34);
            replaceOpenedFileOnExportToolStripMenuItem.Text = "Replace Opened File On Export?";
            replaceOpenedFileOnExportToolStripMenuItem.Click += ReplaceOpenedFileOnExportToolStripMenuItem_Click;
            // 
            // chooseLocationToolStripMenuItem
            // 
            chooseLocationToolStripMenuItem.Name = "chooseLocationToolStripMenuItem";
            chooseLocationToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Alt | Keys.Shift | Keys.E;
            chooseLocationToolStripMenuItem.Size = new Size(477, 34);
            chooseLocationToolStripMenuItem.Text = "Choose export location...";
            chooseLocationToolStripMenuItem.Click += ChooseLocationToolStripMenuItem_Click;
            // 
            // exportToolStripMenuItem
            // 
            exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            exportToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.E;
            exportToolStripMenuItem.Size = new Size(477, 34);
            exportToolStripMenuItem.Text = "Export";
            exportToolStripMenuItem.Click += ExportToolStripMenuItem_Click;
            // 
            // closeFileToolStripMenuItem
            // 
            closeFileToolStripMenuItem.Enabled = false;
            closeFileToolStripMenuItem.Name = "closeFileToolStripMenuItem";
            closeFileToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.W;
            closeFileToolStripMenuItem.Size = new Size(278, 34);
            closeFileToolStripMenuItem.Text = "Close File";
            closeFileToolStripMenuItem.Click += CloseFileToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            exitToolStripMenuItem.Size = new Size(278, 34);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.Enabled = false;
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(58, 29);
            editToolStripMenuItem.Text = "Edit";
            // 
            // searchToolStripMenuItem
            // 
            searchToolStripMenuItem.Enabled = false;
            searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            searchToolStripMenuItem.Size = new Size(80, 29);
            searchToolStripMenuItem.Text = "Search";
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { jSONEditorToolStripMenuItem, interactiveEditorToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(65, 29);
            viewToolStripMenuItem.Text = "View";
            // 
            // jSONEditorToolStripMenuItem
            // 
            jSONEditorToolStripMenuItem.Checked = true;
            jSONEditorToolStripMenuItem.CheckOnClick = true;
            jSONEditorToolStripMenuItem.CheckState = CheckState.Checked;
            jSONEditorToolStripMenuItem.Name = "jSONEditorToolStripMenuItem";
            jSONEditorToolStripMenuItem.Size = new Size(247, 34);
            jSONEditorToolStripMenuItem.Text = "JSON Editor";
            // 
            // interactiveEditorToolStripMenuItem
            // 
            interactiveEditorToolStripMenuItem.Enabled = false;
            interactiveEditorToolStripMenuItem.Name = "interactiveEditorToolStripMenuItem";
            interactiveEditorToolStripMenuItem.Size = new Size(247, 34);
            interactiveEditorToolStripMenuItem.Text = "Interactive Editor";
            // 
            // mainPanel
            // 
            mainPanel.Controls.Add(tableLayoutPanel);
            mainPanel.Controls.Add(statusStrip);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 35);
            mainPanel.Margin = new Padding(4, 5, 4, 5);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new Size(1806, 1100);
            mainPanel.TabIndex = 1;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
            tableLayoutPanel.Controls.Add(procedureListBox, 0, 0);
            tableLayoutPanel.Controls.Add(tabControl1, 1, 0);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Margin = new Padding(4, 5, 4, 5);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Size = new Size(1806, 1065);
            tableLayoutPanel.TabIndex = 0;
            // 
            // procedureListBox
            // 
            procedureListBox.Dock = DockStyle.Fill;
            procedureListBox.FormattingEnabled = true;
            procedureListBox.ItemHeight = 25;
            procedureListBox.Location = new Point(4, 5);
            procedureListBox.Margin = new Padding(4, 5, 4, 5);
            procedureListBox.Name = "procedureListBox";
            procedureListBox.Size = new Size(353, 1055);
            procedureListBox.TabIndex = 0;
            procedureListBox.SelectedIndexChanged += ProcedureListBox_SelectedIndexChanged;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(JsonViewTabPage);
            tabControl1.Controls.Add(ObjectViewTabPage);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(365, 5);
            tabControl1.Margin = new Padding(4, 5, 4, 5);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1437, 1055);
            tabControl1.TabIndex = 1;
            tabControl1.SelectedIndexChanged += tabControl1_TabIndexChanged;
            // 
            // JsonViewTabPage
            // 
            JsonViewTabPage.Controls.Add(richTextBox);
            JsonViewTabPage.Location = new Point(4, 34);
            JsonViewTabPage.Margin = new Padding(4, 5, 4, 5);
            JsonViewTabPage.Name = "JsonViewTabPage";
            JsonViewTabPage.Padding = new Padding(4, 5, 4, 5);
            JsonViewTabPage.Size = new Size(1429, 1017);
            JsonViewTabPage.TabIndex = 0;
            JsonViewTabPage.Text = "JSON View";
            JsonViewTabPage.UseVisualStyleBackColor = true;
            // 
            // richTextBox
            // 
            richTextBox.DetectUrls = false;
            richTextBox.Dock = DockStyle.Fill;
            richTextBox.Enabled = false;
            richTextBox.Location = new Point(4, 5);
            richTextBox.Margin = new Padding(4, 5, 4, 5);
            richTextBox.Name = "richTextBox";
            richTextBox.Size = new Size(1421, 1007);
            richTextBox.TabIndex = 2;
            richTextBox.Text = "";
            // 
            // ObjectViewTabPage
            // 
            ObjectViewTabPage.Controls.Add(flowLayoutPanel);
            ObjectViewTabPage.Location = new Point(4, 34);
            ObjectViewTabPage.Margin = new Padding(4, 5, 4, 5);
            ObjectViewTabPage.Name = "ObjectViewTabPage";
            ObjectViewTabPage.Padding = new Padding(4, 5, 4, 5);
            ObjectViewTabPage.Size = new Size(1429, 1017);
            ObjectViewTabPage.TabIndex = 1;
            ObjectViewTabPage.Text = "Object View";
            ObjectViewTabPage.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel
            // 
            flowLayoutPanel.AutoScroll = true;
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.Location = new Point(4, 5);
            flowLayoutPanel.Margin = new Padding(4, 5, 4, 5);
            flowLayoutPanel.MaximumSize = new Size(1421, 0);
            flowLayoutPanel.Name = "flowLayoutPanel";
            flowLayoutPanel.Size = new Size(1421, 1007);
            flowLayoutPanel.TabIndex = 0;
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new Size(24, 24);
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripProgressBar, toolStripStatusLabel });
            statusStrip.Location = new Point(0, 1065);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new Padding(1, 0, 20, 0);
            statusStrip.Size = new Size(1806, 35);
            statusStrip.TabIndex = 2;
            statusStrip.Text = "statusStrip";
            // 
            // toolStripProgressBar
            // 
            toolStripProgressBar.Name = "toolStripProgressBar";
            toolStripProgressBar.Size = new Size(143, 27);
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(0, 28);
            // 
            // GcxEditorGUI
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1806, 1135);
            Controls.Add(mainPanel);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            Margin = new Padding(4, 5, 4, 5);
            Name = "GcxEditorGUI";
            Text = "GCX Editor";
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            tableLayoutPanel.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            JsonViewTabPage.ResumeLayout(false);
            ObjectViewTabPage.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem searchToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem loadgcxToolStripMenuItem;
        private ToolStripMenuItem savejsonToolStripMenuItem;
        private ToolStripMenuItem exportModifiedgcxToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem jSONEditorToolStripMenuItem;
        private ToolStripMenuItem interactiveEditorToolStripMenuItem;
        private Panel mainPanel;
        private TableLayoutPanel tableLayoutPanel;
        private ListBox procedureListBox;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel toolStripStatusLabel;
        private ToolStripMenuItem replaceOpenedFileOnExportToolStripMenuItem;
        private ToolStripMenuItem chooseLocationToolStripMenuItem;
        private ToolStripProgressBar toolStripProgressBar;
        private ToolStripMenuItem exportToolStripMenuItem;
        private ToolStripMenuItem closeFileToolStripMenuItem;
        private TabControl tabControl1;
        private TabPage JsonViewTabPage;
        private RichTextBox richTextBox;
        private TabPage ObjectViewTabPage;
        private FlowLayoutPanel flowLayoutPanel;
    }
}
