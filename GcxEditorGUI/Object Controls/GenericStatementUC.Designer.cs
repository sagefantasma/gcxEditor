namespace GcxEditorGUI.Object_Controls
{
    partial class GenericStatementUC
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
            argLabel = new Label();
            argContentsTextBox = new TextBox();
            SuspendLayout();
            // 
            // nameLabel
            // 
            nameLabel.AutoSize = true;
            nameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold | FontStyle.Underline);
            nameLabel.Location = new Point(104, 0);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new Size(114, 15);
            nameLabel.TabIndex = 0;
            nameLabel.Text = "Generic Statement";
            // 
            // argLabel
            // 
            argLabel.AutoSize = true;
            argLabel.Location = new Point(6, 30);
            argLabel.Name = "argLabel";
            argLabel.Size = new Size(134, 15);
            argLabel.TabIndex = 1;
            argLabel.Text = "Restart Statement Value:";
            argLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // argContentsTextBox
            // 
            argContentsTextBox.Location = new Point(140, 18);
            argContentsTextBox.Multiline = true;
            argContentsTextBox.Name = "argContentsTextBox";
            argContentsTextBox.Size = new Size(169, 42);
            argContentsTextBox.TabIndex = 2;
            // 
            // GenericStatementUC
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(argContentsTextBox);
            Controls.Add(argLabel);
            Controls.Add(nameLabel);
            Name = "GenericStatementUC";
            Size = new Size(316, 65);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public Label nameLabel;
        public Label argLabel;
        public TextBox argContentsTextBox;
    }
}
