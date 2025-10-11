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
            contentFlowPanel = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // nameLabel
            // 
            nameLabel.AutoSize = true;
            nameLabel.Dock = DockStyle.Top;
            nameLabel.Location = new Point(0, 0);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new Size(38, 15);
            nameLabel.TabIndex = 0;
            nameLabel.Text = "label1";
            nameLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // contentFlowPanel
            // 
            contentFlowPanel.AutoSize = true;
            contentFlowPanel.Dock = DockStyle.Fill;
            contentFlowPanel.Location = new Point(0, 15);
            contentFlowPanel.Name = "contentFlowPanel";
            contentFlowPanel.Size = new Size(245, 142);
            contentFlowPanel.TabIndex = 1;
            // 
            // NestableUC
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(contentFlowPanel);
            Controls.Add(nameLabel);
            Name = "NestableUC";
            Size = new Size(245, 157);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public Label nameLabel;
        public FlowLayoutPanel contentFlowPanel;
    }
}
