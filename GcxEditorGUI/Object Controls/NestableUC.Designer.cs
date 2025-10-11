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
            nameLabel.Location = new Point(37, 0);
            nameLabel.Margin = new Padding(4, 0, 4, 0);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new Size(59, 25);
            nameLabel.TabIndex = 0;
            nameLabel.Text = "label1";
            nameLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // positionLabel
            // 
            positionLabel.AutoSize = true;
            positionLabel.Location = new Point(3, 0);
            positionLabel.Name = "positionLabel";
            positionLabel.Size = new Size(22, 25);
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
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(350, 28);
            tableLayoutPanel1.TabIndex = 10;
            // 
            // contentFlowPanel
            // 
            contentFlowPanel.AutoSize = true;
            contentFlowPanel.Dock = DockStyle.Fill;
            contentFlowPanel.Location = new Point(0, 28);
            contentFlowPanel.Margin = new Padding(4, 5, 4, 5);
            contentFlowPanel.Name = "contentFlowPanel";
            contentFlowPanel.Size = new Size(350, 234);
            contentFlowPanel.TabIndex = 11;
            // 
            // NestableUC
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(contentFlowPanel);
            Controls.Add(tableLayoutPanel1);
            Margin = new Padding(4, 5, 4, 5);
            Name = "NestableUC";
            Size = new Size(350, 262);
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
