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
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            loadgcxToolStripMenuItem = new ToolStripMenuItem();
            savejsonToolStripMenuItem = new ToolStripMenuItem();
            exportModifiedgcxToolStripMenuItem = new ToolStripMenuItem();
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
            menuStrip1.SuspendLayout();
            mainPanel.SuspendLayout();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, searchToolStripMenuItem, viewToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(9, 3, 0, 3);
            menuStrip1.Size = new Size(1806, 35);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadgcxToolStripMenuItem, savejsonToolStripMenuItem, exportModifiedgcxToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(54, 29);
            fileToolStripMenuItem.Text = "File";
            // 
            // loadgcxToolStripMenuItem
            // 
            loadgcxToolStripMenuItem.Name = "loadgcxToolStripMenuItem";
            loadgcxToolStripMenuItem.Size = new Size(278, 34);
            loadgcxToolStripMenuItem.Text = "Load .gcx";
            loadgcxToolStripMenuItem.Click += loadgcxToolStripMenuItem_Click;
            // 
            // savejsonToolStripMenuItem
            // 
            savejsonToolStripMenuItem.Enabled = false;
            savejsonToolStripMenuItem.Name = "savejsonToolStripMenuItem";
            savejsonToolStripMenuItem.Size = new Size(278, 34);
            savejsonToolStripMenuItem.Text = "Save .json";
            savejsonToolStripMenuItem.Click += savejsonToolStripMenuItem_Click;
            // 
            // exportModifiedgcxToolStripMenuItem
            // 
            exportModifiedgcxToolStripMenuItem.Enabled = false;
            exportModifiedgcxToolStripMenuItem.Name = "exportModifiedgcxToolStripMenuItem";
            exportModifiedgcxToolStripMenuItem.Size = new Size(278, 34);
            exportModifiedgcxToolStripMenuItem.Text = "Export modified .gcx";
            exportModifiedgcxToolStripMenuItem.Click += exportModifiedgcxToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(278, 34);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(58, 29);
            editToolStripMenuItem.Text = "Edit";
            // 
            // searchToolStripMenuItem
            // 
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
            tableLayoutPanel.Controls.Add(richTextBox, 1, 0);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Margin = new Padding(4, 5, 4, 5);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Size = new Size(1806, 1100);
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
            procedureListBox.Size = new Size(353, 1090);
            procedureListBox.TabIndex = 0;
            procedureListBox.SelectedIndexChanged += procedureListBox_SelectedIndexChanged;
            // 
            // richTextBox
            // 
            richTextBox.DetectUrls = false;
            richTextBox.Dock = DockStyle.Fill;
            richTextBox.Enabled = false;
            richTextBox.Location = new Point(365, 5);
            richTextBox.Margin = new Padding(4, 5, 4, 5);
            richTextBox.Name = "richTextBox";
            richTextBox.Size = new Size(1437, 1090);
            richTextBox.TabIndex = 1;
            richTextBox.Text = "";
            richTextBox.SelectionChanged += richTextBox_SelectionChanged;
            // 
            // GcxEditorGUI
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1806, 1135);
            Controls.Add(mainPanel);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 5, 4, 5);
            Name = "GcxEditorGUI";
            Text = "GCX Editor";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            mainPanel.ResumeLayout(false);
            tableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
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
    }
}
