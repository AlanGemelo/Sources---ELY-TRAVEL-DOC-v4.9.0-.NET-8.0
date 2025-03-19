
namespace ELY_TRAVEL_DOC
{
    partial class FormPasswordPACE
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPasswordPACE));
            groupBox1 = new System.Windows.Forms.GroupBox();
            textBoxCAN = new System.Windows.Forms.TextBox();
            rbPACEPasswordMRZ = new System.Windows.Forms.RadioButton();
            rbPACEPasswordCAN = new System.Windows.Forms.RadioButton();
            buttonOK = new System.Windows.Forms.Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(textBoxCAN);
            groupBox1.Controls.Add(rbPACEPasswordMRZ);
            groupBox1.Controls.Add(rbPACEPasswordCAN);
            groupBox1.Location = new System.Drawing.Point(11, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(148, 82);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Password options";
            // 
            // textBoxCAN
            // 
            textBoxCAN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxCAN.Font = new System.Drawing.Font("Courier New", 9F);
            textBoxCAN.Location = new System.Drawing.Point(73, 43);
            textBoxCAN.MaxLength = 6;
            textBoxCAN.Name = "textBoxCAN";
            textBoxCAN.Size = new System.Drawing.Size(55, 21);
            textBoxCAN.TabIndex = 2;
            textBoxCAN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            textBoxCAN.KeyPress += textBoxCAN_KeyPress;
            // 
            // rbPACEPasswordMRZ
            // 
            rbPACEPasswordMRZ.AutoSize = true;
            rbPACEPasswordMRZ.Checked = true;
            rbPACEPasswordMRZ.Location = new System.Drawing.Point(20, 22);
            rbPACEPasswordMRZ.Name = "rbPACEPasswordMRZ";
            rbPACEPasswordMRZ.Size = new System.Drawing.Size(49, 17);
            rbPACEPasswordMRZ.TabIndex = 0;
            rbPACEPasswordMRZ.TabStop = true;
            rbPACEPasswordMRZ.Text = "MRZ";
            rbPACEPasswordMRZ.UseVisualStyleBackColor = true;
            // 
            // rbPACEPasswordCAN
            // 
            rbPACEPasswordCAN.AutoSize = true;
            rbPACEPasswordCAN.Location = new System.Drawing.Point(20, 45);
            rbPACEPasswordCAN.Name = "rbPACEPasswordCAN";
            rbPACEPasswordCAN.Size = new System.Drawing.Size(47, 17);
            rbPACEPasswordCAN.TabIndex = 1;
            rbPACEPasswordCAN.Text = "CAN";
            rbPACEPasswordCAN.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            buttonOK.Location = new System.Drawing.Point(88, 103);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new System.Drawing.Size(71, 23);
            buttonOK.TabIndex = 2;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // FormPasswordPACE
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(171, 136);
            Controls.Add(buttonOK);
            Controls.Add(groupBox1);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormPasswordPACE";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "PACE";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxCAN;
        private System.Windows.Forms.RadioButton rbPACEPasswordMRZ;
        private System.Windows.Forms.RadioButton rbPACEPasswordCAN;
        private System.Windows.Forms.Button buttonOK;
    }
}