namespace ELY_TRAVEL_DOC
{
    partial class FormMRZ
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMRZ));
            textBoxLine3 = new System.Windows.Forms.TextBox();
            textBoxLine2 = new System.Windows.Forms.TextBox();
            textBoxLine1 = new System.Windows.Forms.TextBox();
            label9 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            buttonOk = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            textBoxDocNum = new System.Windows.Forms.TextBox();
            textBoxDExp = new System.Windows.Forms.TextBox();
            textBoxDBirth = new System.Windows.Forms.TextBox();
            buttonReadMRZfile = new System.Windows.Forms.Button();
            openFileDialogMRZfile = new System.Windows.Forms.OpenFileDialog();
            label7 = new System.Windows.Forms.Label();
            cbSaveMRZ = new System.Windows.Forms.CheckBox();
            cbUseSavedMRZ = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // textBoxLine3
            // 
            textBoxLine3.Enabled = false;
            textBoxLine3.Location = new System.Drawing.Point(61, 95);
            textBoxLine3.Name = "textBoxLine3";
            textBoxLine3.Size = new System.Drawing.Size(300, 23);
            textBoxLine3.TabIndex = 6;
            textBoxLine3.KeyPress += textBoxLine3_KeyPress;
            // 
            // textBoxLine2
            // 
            textBoxLine2.Location = new System.Drawing.Point(61, 70);
            textBoxLine2.Name = "textBoxLine2";
            textBoxLine2.Size = new System.Drawing.Size(300, 23);
            textBoxLine2.TabIndex = 4;
            textBoxLine2.TextChanged += textBoxLine2_TextChanged;
            textBoxLine2.KeyPress += textBoxLine2_KeyPress;
            textBoxLine2.Leave += TextBoxLine2_Leave;
            // 
            // textBoxLine1
            // 
            textBoxLine1.Location = new System.Drawing.Point(61, 45);
            textBoxLine1.Name = "textBoxLine1";
            textBoxLine1.Size = new System.Drawing.Size(300, 23);
            textBoxLine1.TabIndex = 2;
            textBoxLine1.TextChanged += textBoxLine1_TextChanged;
            textBoxLine1.KeyPress += textBoxLine1_KeyPress;
            textBoxLine1.Leave += TextBoxLine1_Leave;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(14, 99);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(55, 17);
            label9.TabIndex = 5;
            label9.Text = "Line 3 :";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(14, 74);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(55, 17);
            label6.TabIndex = 3;
            label6.Text = "Line 2 :";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(14, 49);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(55, 17);
            label5.TabIndex = 1;
            label5.Text = "Line 1 :";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            label1.Location = new System.Drawing.Point(14, 19);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(168, 17);
            label1.TabIndex = 0;
            label1.Text = "Please enter the full MRZ";
            label1.Click += label1_Click;
            // 
            // buttonOk
            // 
            buttonOk.Location = new System.Drawing.Point(400, 136);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new System.Drawing.Size(88, 40);
            buttonOk.TabIndex = 17;
            buttonOk.Text = "OK";
            buttonOk.UseVisualStyleBackColor = true;
            buttonOk.Click += buttonOk_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new System.Drawing.Point(494, 136);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(88, 40);
            buttonCancel.TabIndex = 18;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(411, 49);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(95, 17);
            label2.TabIndex = 8;
            label2.Text = "Date of Birth :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(425, 74);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(77, 17);
            label3.TabIndex = 10;
            label3.Text = "Valid until :";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(383, 99);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(132, 17);
            label4.TabIndex = 12;
            label4.Text = "Document number :";
            // 
            // textBoxDocNum
            // 
            textBoxDocNum.Location = new System.Drawing.Point(489, 95);
            textBoxDocNum.Name = "textBoxDocNum";
            textBoxDocNum.Size = new System.Drawing.Size(93, 23);
            textBoxDocNum.TabIndex = 13;
            textBoxDocNum.Leave += textBoxDocNum_Leave;
            // 
            // textBoxDExp
            // 
            textBoxDExp.Location = new System.Drawing.Point(489, 70);
            textBoxDExp.Name = "textBoxDExp";
            textBoxDExp.Size = new System.Drawing.Size(93, 23);
            textBoxDExp.TabIndex = 11;
            textBoxDExp.Leave += textBoxDExp_Leave;
            // 
            // textBoxDBirth
            // 
            textBoxDBirth.Location = new System.Drawing.Point(489, 45);
            textBoxDBirth.Name = "textBoxDBirth";
            textBoxDBirth.Size = new System.Drawing.Size(93, 23);
            textBoxDBirth.TabIndex = 9;
            textBoxDBirth.Leave += textBoxDBirth_Leave;
            // 
            // buttonReadMRZfile
            // 
            buttonReadMRZfile.Location = new System.Drawing.Point(61, 136);
            buttonReadMRZfile.Name = "buttonReadMRZfile";
            buttonReadMRZfile.Size = new System.Drawing.Size(88, 40);
            buttonReadMRZfile.TabIndex = 14;
            buttonReadMRZfile.Text = "Read MRZ file";
            buttonReadMRZfile.UseVisualStyleBackColor = true;
            buttonReadMRZfile.Click += buttonReadMRZfile_Click;
            // 
            // openFileDialogMRZfile
            // 
            openFileDialogMRZfile.Filter = "Fichiers texte|*.txt";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            label7.Location = new System.Drawing.Point(386, 19);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(132, 17);
            label7.TabIndex = 7;
            label7.Text = "(or) the partial MRZ";
            label7.Click += label7_Click;
            // 
            // cbSaveMRZ
            // 
            cbSaveMRZ.AutoSize = true;
            cbSaveMRZ.Location = new System.Drawing.Point(275, 136);
            cbSaveMRZ.Name = "cbSaveMRZ";
            cbSaveMRZ.Size = new System.Drawing.Size(122, 21);
            cbSaveMRZ.TabIndex = 15;
            cbSaveMRZ.Text = "Save this MRZ";
            cbSaveMRZ.UseVisualStyleBackColor = true;
            // 
            // cbUseSavedMRZ
            // 
            cbUseSavedMRZ.AutoSize = true;
            cbUseSavedMRZ.Location = new System.Drawing.Point(275, 159);
            cbUseSavedMRZ.Name = "cbUseSavedMRZ";
            cbUseSavedMRZ.Size = new System.Drawing.Size(142, 21);
            cbUseSavedMRZ.TabIndex = 16;
            cbUseSavedMRZ.Text = "Use saved details";
            cbUseSavedMRZ.UseVisualStyleBackColor = true;
            cbUseSavedMRZ.CheckedChanged += cbUseLastSavedMRZ_CheckedChanged;
            // 
            // FormMRZ
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(615, 220);
            Controls.Add(cbUseSavedMRZ);
            Controls.Add(cbSaveMRZ);
            Controls.Add(label7);
            Controls.Add(buttonReadMRZfile);
            Controls.Add(textBoxDBirth);
            Controls.Add(textBoxDExp);
            Controls.Add(textBoxDocNum);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOk);
            Controls.Add(label1);
            Controls.Add(textBoxLine3);
            Controls.Add(textBoxLine2);
            Controls.Add(textBoxLine1);
            Controls.Add(label9);
            Controls.Add(label6);
            Controls.Add(label5);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "FormMRZ";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "MRZ";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox textBoxLine3;
        private System.Windows.Forms.TextBox textBoxLine2;
        private System.Windows.Forms.TextBox textBoxLine1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxDocNum;
        private System.Windows.Forms.TextBox textBoxDExp;
        private System.Windows.Forms.TextBox textBoxDBirth;
        private System.Windows.Forms.Button buttonReadMRZfile;
        private System.Windows.Forms.OpenFileDialog openFileDialogMRZfile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox cbSaveMRZ;
        private System.Windows.Forms.CheckBox cbUseSavedMRZ;
    }
}