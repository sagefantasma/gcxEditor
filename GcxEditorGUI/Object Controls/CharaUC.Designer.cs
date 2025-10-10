namespace GcxEditorGUI.Object_Controls
{
    partial class CharaUC
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
            idLabel = new Label();
            typeLabel = new Label();
            paramsLabel = new Label();
            typeTextBox = new TextBox();
            idTextBox = new TextBox();
            paramTextBox = new TextBox();
            SuspendLayout();
            // 
            // nameLabel
            // 
            nameLabel.AutoSize = true;
            nameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold | FontStyle.Underline);
            nameLabel.Location = new Point(124, 0);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new Size(38, 15);
            nameLabel.TabIndex = 0;
            nameLabel.Text = "Chara";
            nameLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // idLabel
            // 
            idLabel.AutoSize = true;
            idLabel.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            idLabel.Location = new Point(162, 27);
            idLabel.Name = "idLabel";
            idLabel.Size = new Size(21, 15);
            idLabel.TabIndex = 1;
            idLabel.Text = "ID:";
            // 
            // typeLabel
            // 
            typeLabel.AutoSize = true;
            typeLabel.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            typeLabel.Location = new Point(3, 27);
            typeLabel.Name = "typeLabel";
            typeLabel.Size = new Size(34, 15);
            typeLabel.TabIndex = 2;
            typeLabel.Text = "Type:";
            // 
            // paramsLabel
            // 
            paramsLabel.AutoSize = true;
            paramsLabel.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            paramsLabel.Location = new Point(3, 95);
            paramsLabel.Name = "paramsLabel";
            paramsLabel.Size = new Size(69, 15);
            paramsLabel.TabIndex = 3;
            paramsLabel.Text = "Parameters:";
            // 
            // typeTextBox
            // 
            typeTextBox.Location = new Point(43, 24);
            typeTextBox.Name = "typeTextBox";
            typeTextBox.Size = new Size(100, 23);
            typeTextBox.TabIndex = 4;
            // 
            // idTextBox
            // 
            idTextBox.Location = new Point(189, 24);
            idTextBox.Name = "idTextBox";
            idTextBox.Size = new Size(100, 23);
            idTextBox.TabIndex = 5;
            // 
            // paramTextBox
            // 
            paramTextBox.Location = new Point(78, 64);
            paramTextBox.Multiline = true;
            paramTextBox.Name = "paramTextBox";
            paramTextBox.Size = new Size(211, 83);
            paramTextBox.TabIndex = 6;
            // 
            // CharaUC
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(paramTextBox);
            Controls.Add(idTextBox);
            Controls.Add(typeTextBox);
            Controls.Add(paramsLabel);
            Controls.Add(typeLabel);
            Controls.Add(idLabel);
            Controls.Add(nameLabel);
            Name = "CharaUC";
            Size = new Size(299, 156);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label nameLabel;
        private Label idLabel;
        private Label typeLabel;
        private Label paramsLabel;
        public TextBox typeTextBox;
        public TextBox idTextBox;
        public TextBox paramTextBox;
    }
}
