namespace GcxEditorGUI.Object_Controls
{
    partial class NestableUC
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            nameLabel = new Label();
            positionLabel = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            contentFlowPanel = new FlowLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // nameLabel
            // 
            nameLabel.AutoSize = true;
            nameLabel.Location = new Point(26, 0);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new Size(38, 15);
            nameLabel.TabIndex = 0;
            nameLabel.Text = "label1";
            nameLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // positionLabel
            // 
            positionLabel.AutoSize = true;
            positionLabel.Location = new Point(2, 0);
            positionLabel.Margin = new Padding(2, 0, 2, 0);
            positionLabel.Name = "positionLabel";
            positionLabel.Size = new Size(13, 15);
            positionLabel.TabIndex = 9;
            positionLabel.Text = "0";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 9.428572F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 90.57143F));
            tableLayoutPanel1.Controls.Add(nameLabel, 1, 0);
            tableLayoutPanel1.Controls.Add(positionLabel, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Top;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 17F));
            tableLayoutPanel1.Size = new Size(245, 17);
            tableLayoutPanel1.TabIndex = 10;
            tableLayoutPanel1.MouseDown += NestableUC_MouseDown;
            // 
            // contentFlowPanel
            // 
            contentFlowPanel.AutoSize = true;
            contentFlowPanel.Dock = DockStyle.Fill;
            contentFlowPanel.Location = new Point(0, 17);
            contentFlowPanel.MaximumSize = new Size(995, 0);
            contentFlowPanel.Name = "contentFlowPanel";
            contentFlowPanel.Size = new Size(245, 140);
            contentFlowPanel.TabIndex = 11;
            // 
            // NestableUC
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(contentFlowPanel);
            Controls.Add(tableLayoutPanel1);
            Name = "NestableUC";
            Size = new Size(245, 157);
            MouseDown += NestableUC_MouseDown;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public Label nameLabel;
        private TableLayoutPanel tableLayoutPanel1;
        public FlowLayoutPanel contentFlowPanel;
        public Label positionLabel;
    }
}
