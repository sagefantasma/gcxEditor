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
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            searchToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            jSONEditorToolStripMenuItem = new ToolStripMenuItem();
            interactiveEditorToolStripMenuItem = new ToolStripMenuItem();
            mainPanel = new Panel();
            tableLayoutPanel = new TableLayoutPanel();
            procedureListBox = new ListBox();
            richTextBox = new RichTextBox();
            statusStrip = new StatusStrip();
            toolStripProgressBar = new ToolStripProgressBar();
            toolStripStatusLabel = new ToolStripStatusLabel();
            menuStrip.SuspendLayout();
            mainPanel.SuspendLayout();
            tableLayoutPanel.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.ImageScalingSize = new Size(24, 24);
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, searchToolStripMenuItem, viewToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1264, 24);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadgcxToolStripMenuItem, savejsonToolStripMenuItem, exportModifiedgcxToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // loadgcxToolStripMenuItem
            // 
            loadgcxToolStripMenuItem.Name = "loadgcxToolStripMenuItem";
            loadgcxToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            loadgcxToolStripMenuItem.Size = new Size(184, 22);
            loadgcxToolStripMenuItem.Text = "Load .gcx...";
            loadgcxToolStripMenuItem.Click += loadgcxToolStripMenuItem_Click;
            // 
            // savejsonToolStripMenuItem
            // 
            savejsonToolStripMenuItem.Enabled = false;
            savejsonToolStripMenuItem.Name = "savejsonToolStripMenuItem";
            savejsonToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            savejsonToolStripMenuItem.Size = new Size(184, 22);
            savejsonToolStripMenuItem.Text = "Save .json";
            savejsonToolStripMenuItem.Click += savejsonToolStripMenuItem_Click;
            // 
            // exportModifiedgcxToolStripMenuItem
            // 
            exportModifiedgcxToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { replaceOpenedFileOnExportToolStripMenuItem, chooseLocationToolStripMenuItem, exportToolStripMenuItem });
            exportModifiedgcxToolStripMenuItem.Enabled = false;
            exportModifiedgcxToolStripMenuItem.Name = "exportModifiedgcxToolStripMenuItem";
            exportModifiedgcxToolStripMenuItem.Size = new Size(184, 22);
            exportModifiedgcxToolStripMenuItem.Text = "Export modified .gcx";
            // 
            // replaceOpenedFileOnExportToolStripMenuItem
            // 
            replaceOpenedFileOnExportToolStripMenuItem.CheckOnClick = true;
            replaceOpenedFileOnExportToolStripMenuItem.Name = "replaceOpenedFileOnExportToolStripMenuItem";
            replaceOpenedFileOnExportToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.R;
            replaceOpenedFileOnExportToolStripMenuItem.Size = new Size(315, 22);
            replaceOpenedFileOnExportToolStripMenuItem.Text = "Replace Opened File On Export?";
            replaceOpenedFileOnExportToolStripMenuItem.Click += replaceOpenedFileOnExportToolStripMenuItem_Click;
            // 
            // chooseLocationToolStripMenuItem
            // 
            chooseLocationToolStripMenuItem.Name = "chooseLocationToolStripMenuItem";
            chooseLocationToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Alt | Keys.Shift | Keys.E;
            chooseLocationToolStripMenuItem.Size = new Size(301, 22);
            chooseLocationToolStripMenuItem.Text = "Choose export location...";
            chooseLocationToolStripMenuItem.Click += chooseLocationToolStripMenuItem_Click;
            // 
            // exportToolStripMenuItem
            // 
            exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            exportToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.E;
            exportToolStripMenuItem.Size = new Size(301, 22);
            exportToolStripMenuItem.Text = "Export";
            exportToolStripMenuItem.Click += exportToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            exitToolStripMenuItem.Size = new Size(184, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.Enabled = false;
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(39, 20);
            editToolStripMenuItem.Text = "Edit";
            // 
            // searchToolStripMenuItem
            // 
            searchToolStripMenuItem.Enabled = false;
            searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            searchToolStripMenuItem.Size = new Size(54, 20);
            searchToolStripMenuItem.Text = "Search";
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { jSONEditorToolStripMenuItem, interactiveEditorToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "View";
            // 
            // jSONEditorToolStripMenuItem
            // 
            jSONEditorToolStripMenuItem.Checked = true;
            jSONEditorToolStripMenuItem.CheckOnClick = true;
            jSONEditorToolStripMenuItem.CheckState = CheckState.Checked;
            jSONEditorToolStripMenuItem.Name = "jSONEditorToolStripMenuItem";
            jSONEditorToolStripMenuItem.Size = new Size(163, 22);
            jSONEditorToolStripMenuItem.Text = "JSON Editor";
            // 
            // interactiveEditorToolStripMenuItem
            // 
            interactiveEditorToolStripMenuItem.Enabled = false;
            interactiveEditorToolStripMenuItem.Name = "interactiveEditorToolStripMenuItem";
            interactiveEditorToolStripMenuItem.Size = new Size(163, 22);
            interactiveEditorToolStripMenuItem.Text = "Interactive Editor";
            // 
            // mainPanel
            // 
            mainPanel.Controls.Add(tableLayoutPanel);
            mainPanel.Controls.Add(statusStrip);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 24);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new Size(1264, 657);
            mainPanel.TabIndex = 1;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
            tableLayoutPanel.Controls.Add(procedureListBox, 0, 0);
            tableLayoutPanel.Controls.Add(richTextBox, 1, 0);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Size = new Size(1264, 635);
            tableLayoutPanel.TabIndex = 0;
            // 
            // procedureListBox
            // 
            procedureListBox.Dock = DockStyle.Fill;
            procedureListBox.FormattingEnabled = true;
            procedureListBox.ItemHeight = 15;
            procedureListBox.Location = new Point(3, 3);
            procedureListBox.Name = "procedureListBox";
            procedureListBox.Size = new Size(246, 629);
            procedureListBox.TabIndex = 0;
            procedureListBox.SelectedIndexChanged += procedureListBox_SelectedIndexChanged;
            // 
            // richTextBox
            // 
            richTextBox.DetectUrls = false;
            richTextBox.Dock = DockStyle.Fill;
            richTextBox.Enabled = false;
            richTextBox.Location = new Point(255, 3);
            richTextBox.Name = "richTextBox";
            richTextBox.Size = new Size(1006, 629);
            richTextBox.TabIndex = 1;
            richTextBox.Text = "";
            richTextBox.SelectionChanged += richTextBox_SelectionChanged;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripProgressBar, toolStripStatusLabel });
            statusStrip.Location = new Point(0, 635);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1264, 22);
            statusStrip.TabIndex = 2;
            statusStrip.Text = "statusStrip";
            // 
            // toolStripProgressBar
            // 
            toolStripProgressBar.Name = "toolStripProgressBar";
            toolStripProgressBar.Size = new Size(100, 16);
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(0, 17);
            // 
            // GcxEditorGUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1264, 681);
            Controls.Add(mainPanel);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            Name = "GcxEditorGUI";
            Text = "GCX Editor";
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            tableLayoutPanel.ResumeLayout(false);
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
        private RichTextBox richTextBox;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel toolStripStatusLabel;
        private ToolStripMenuItem replaceOpenedFileOnExportToolStripMenuItem;
        private ToolStripMenuItem chooseLocationToolStripMenuItem;
        private ToolStripProgressBar toolStripProgressBar;
        private ToolStripMenuItem exportToolStripMenuItem;
    }
}
