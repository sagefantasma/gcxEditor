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
            positionLabel = new Label();
            SuspendLayout();
            // 
            // nameLabel
            // 
            nameLabel.AutoSize = true;
            nameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold | FontStyle.Underline);
            nameLabel.Location = new Point(149, 0);
            nameLabel.Margin = new Padding(4, 0, 4, 0);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new Size(170, 25);
            nameLabel.TabIndex = 0;
            nameLabel.Text = "Generic Statement";
            // 
            // argLabel
            // 
            argLabel.AutoSize = true;
            argLabel.Location = new Point(9, 50);
            argLabel.Margin = new Padding(4, 0, 4, 0);
            argLabel.Name = "argLabel";
            argLabel.Size = new Size(202, 25);
            argLabel.TabIndex = 1;
            argLabel.Text = "Restart Statement Value:";
            argLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // argContentsTextBox
            // 
            argContentsTextBox.Location = new Point(200, 30);
            argContentsTextBox.Margin = new Padding(4, 5, 4, 5);
            argContentsTextBox.Multiline = true;
            argContentsTextBox.Name = "argContentsTextBox";
            argContentsTextBox.Size = new Size(240, 67);
            argContentsTextBox.TabIndex = 2;
            // 
            // positionLabel
            // 
            positionLabel.AutoSize = true;
            positionLabel.Location = new Point(3, 0);
            positionLabel.Name = "positionLabel";
            positionLabel.Size = new Size(22, 25);
            positionLabel.TabIndex = 8;
            positionLabel.Text = "0";
            // 
            // GenericStatementUC
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(positionLabel);
            Controls.Add(argContentsTextBox);
            Controls.Add(argLabel);
            Controls.Add(nameLabel);
            Margin = new Padding(4, 5, 4, 5);
            Name = "GenericStatementUC";
            Size = new Size(451, 108);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public Label nameLabel;
        public Label argLabel;
        public TextBox argContentsTextBox;
        public Label positionLabel;
    }
}
