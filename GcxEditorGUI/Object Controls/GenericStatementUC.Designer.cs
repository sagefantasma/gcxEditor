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
            paramsLabel = new Label();
            paramsContentsTextBox = new TextBox();
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
            argContentsTextBox.ScrollBars = ScrollBars.Vertical;
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
            // paramsLabel
            // 
            paramsLabel.AutoSize = true;
            paramsLabel.Location = new Point(9, 126);
            paramsLabel.Margin = new Padding(4, 0, 4, 0);
            paramsLabel.Name = "paramsLabel";
            paramsLabel.Size = new Size(202, 25);
            paramsLabel.TabIndex = 9;
            paramsLabel.Text = "Restart Statement Value:";
            paramsLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // paramsContentsTextBox
            // 
            paramsContentsTextBox.Location = new Point(200, 107);
            paramsContentsTextBox.Margin = new Padding(4, 5, 4, 5);
            paramsContentsTextBox.Multiline = true;
            paramsContentsTextBox.Name = "paramsContentsTextBox";
            paramsContentsTextBox.ScrollBars = ScrollBars.Vertical;
            paramsContentsTextBox.Size = new Size(240, 67);
            paramsContentsTextBox.TabIndex = 10;
            // 
            // GenericStatementUC
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(paramsContentsTextBox);
            Controls.Add(paramsLabel);
            Controls.Add(positionLabel);
            Controls.Add(argContentsTextBox);
            Controls.Add(argLabel);
            Controls.Add(nameLabel);
            Margin = new Padding(4, 5, 4, 5);
            Name = "GenericStatementUC";
            Size = new Size(451, 191);
            Load += GenericStatementUC_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public Label nameLabel;
        public Label argLabel;
        public TextBox argContentsTextBox;
        public Label positionLabel;
        public Label paramsLabel;
        public TextBox paramsContentsTextBox;
    }
}
