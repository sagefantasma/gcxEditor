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
            positionLabel = new Label();
            SuspendLayout();
            // 
            // nameLabel
            // 
            nameLabel.AutoSize = true;
            nameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold | FontStyle.Underline);
            nameLabel.Location = new Point(177, 0);
            nameLabel.Margin = new Padding(4, 0, 4, 0);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new Size(61, 25);
            nameLabel.TabIndex = 0;
            nameLabel.Text = "Chara";
            nameLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // idLabel
            // 
            idLabel.AutoSize = true;
            idLabel.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            idLabel.Location = new Point(231, 45);
            idLabel.Margin = new Padding(4, 0, 4, 0);
            idLabel.Name = "idLabel";
            idLabel.Size = new Size(34, 25);
            idLabel.TabIndex = 1;
            idLabel.Text = "ID:";
            // 
            // typeLabel
            // 
            typeLabel.AutoSize = true;
            typeLabel.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            typeLabel.Location = new Point(4, 45);
            typeLabel.Margin = new Padding(4, 0, 4, 0);
            typeLabel.Name = "typeLabel";
            typeLabel.Size = new Size(53, 25);
            typeLabel.TabIndex = 2;
            typeLabel.Text = "Type:";
            // 
            // paramsLabel
            // 
            paramsLabel.AutoSize = true;
            paramsLabel.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            paramsLabel.Location = new Point(4, 158);
            paramsLabel.Margin = new Padding(4, 0, 4, 0);
            paramsLabel.Name = "paramsLabel";
            paramsLabel.Size = new Size(103, 25);
            paramsLabel.TabIndex = 3;
            paramsLabel.Text = "Parameters:";
            // 
            // typeTextBox
            // 
            typeTextBox.Location = new Point(61, 40);
            typeTextBox.Margin = new Padding(4, 5, 4, 5);
            typeTextBox.Name = "typeTextBox";
            typeTextBox.Size = new Size(141, 31);
            typeTextBox.TabIndex = 4;
            // 
            // idTextBox
            // 
            idTextBox.Location = new Point(270, 40);
            idTextBox.Margin = new Padding(4, 5, 4, 5);
            idTextBox.Name = "idTextBox";
            idTextBox.Size = new Size(141, 31);
            idTextBox.TabIndex = 5;
            // 
            // paramTextBox
            // 
            paramTextBox.Location = new Point(111, 107);
            paramTextBox.Margin = new Padding(4, 5, 4, 5);
            paramTextBox.Multiline = true;
            paramTextBox.Name = "paramTextBox";
            paramTextBox.Size = new Size(300, 136);
            paramTextBox.TabIndex = 6;
            // 
            // positionLabel
            // 
            positionLabel.AutoSize = true;
            positionLabel.Location = new Point(4, 0);
            positionLabel.Name = "positionLabel";
            positionLabel.Size = new Size(22, 25);
            positionLabel.TabIndex = 7;
            positionLabel.Text = "0";
            // 
            // CharaUC
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(positionLabel);
            Controls.Add(paramTextBox);
            Controls.Add(idTextBox);
            Controls.Add(typeTextBox);
            Controls.Add(paramsLabel);
            Controls.Add(typeLabel);
            Controls.Add(idLabel);
            Controls.Add(nameLabel);
            Margin = new Padding(4, 5, 4, 5);
            Name = "CharaUC";
            Size = new Size(427, 260);
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
        public Label positionLabel;
    }
}
