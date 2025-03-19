using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ELY_TRAVEL_DOC
{
    public partial class FormMRZ : Form
    {
        #region Variables
        private int nbLines = 1;
        private IniFile SecIni = new IniFile(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            + "\\ELYCTIS\\ELY TRAVEL DOC.ini");

        string szDocNum = "";
        string szDBirth = "";
        string szDExp = "";
        #endregion


        #region Constructor
        public FormMRZ()
        {
            try
            {
                InitializeComponent();

                string path = Path.Combine(System.Environment.GetFolderPath(
                    Environment.SpecialFolder.MyDocuments), "ELYCTIS");
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                // Load the saved MRZ from the file
                if (SecIni.StringToBool(SecIni.IniReadValue("Manual MRZ", "UseSavedMRZ")))
                    cbUseSavedMRZ.Checked = true;
            }
            catch (Exception) { Console.WriteLine("Icon Error"); }
        }
        #endregion


        #region Event helpers
        private void ReadSavedDetailsIfAny()
        {
            szDocNum = SecIni.IniReadValue("Manual MRZ", "DocNum");
            szDBirth = SecIni.IniReadValue("Manual MRZ", "DOB");
            szDExp = SecIni.IniReadValue("Manual MRZ", "DOE");
            if (!string.IsNullOrEmpty(szDocNum)) textBoxDocNum.Text = szDocNum;
            if (!string.IsNullOrEmpty(szDBirth)) textBoxDBirth.Text = szDBirth;
            if (!string.IsNullOrEmpty(szDExp)) textBoxDExp.Text = szDExp;
        }

        public string MRZInfo, docNumDigit;

        public string GetLine1()
        {
            return this.textBoxLine1.Text;
        }

        public string GetLine2()
        {
            return this.textBoxLine2.Text;
        }

        public string GetLine3()
        {
            return this.textBoxLine3.Text;
        }

        public string GetMrzInfo()
        {
            if (GetLine2().Length < 2)
            {
                if(GetLine1().Length > 29)
                    return GetLine1().Substring(1, 28);
            }
            return this.textBoxDocNum.Text + this.textBoxDBirth.Text + this.textBoxDExp.Text;
        }

        private void DisplayInfos()
        {
            if (this.textBoxLine1.Text.Length == 30 && this.textBoxLine2.Text.Length == 30)
            {
                this.textBoxDocNum.Text = this.textBoxLine1.Text.Substring(5, 10);
                if (this.textBoxLine1.Text.Substring(14, 1) == "<")
                {
                    String extended = this.textBoxLine1.Text.Substring(15, 15);
                    extended = extended.Substring(0, extended.IndexOf('<'));
                    this.textBoxDocNum.Text = this.textBoxLine1.Text.Substring(5, 9) + extended;
                }
                this.textBoxDBirth.Text = this.textBoxLine2.Text.Substring(0, 7);
                this.textBoxDExp.Text = this.textBoxLine2.Text.Substring(8, 7);
            }
            else
            {
                if (this.textBoxLine2.Text.Length > 27)
                {
                    this.textBoxDocNum.Text = this.textBoxLine2.Text.Substring(0, 10);
                    this.textBoxDBirth.Text = this.textBoxLine2.Text.Substring(13, 7);
                    this.textBoxDExp.Text = this.textBoxLine2.Text.Substring(21, 7);
                }
            }
        }

        private void SaveMrzToIniIfRequired()
        {
            SecIni.IniWriteValue("Manual MRZ", "UseSavedMRZ", (cbUseSavedMRZ.Checked == true) ? "true" : "false");
            if (cbSaveMRZ.Checked)
            {
                SecIni.IniWriteValue("Manual MRZ", "DocNum", this.textBoxDocNum.Text);
                SecIni.IniWriteValue("Manual MRZ", "DOB", this.textBoxDBirth.Text);
                SecIni.IniWriteValue("Manual MRZ", "DOE", this.textBoxDExp.Text);
            }
        }
        #endregion


        #region Event triggers
        private void textBoxLine1_KeyPress(object sender, KeyPressEventArgs e)
        {
            MRZInfo = "";
            if (e.KeyChar == (char)Keys.Escape)
                this.Close();
            if (e.KeyChar == (char)Keys.Enter)
            {
                nbLines++;
                this.textBoxLine3.Enabled = false;
                this.textBoxLine2.Focus();
                e.Handled = true;
            }
        }

        private void textBoxLine1_TextChanged(object sender, EventArgs e)
        {   
            this.textBoxLine2.Focus();
            DisplayInfos();
        }

        private void textBoxLine2_TextChanged(object sender, EventArgs e)
        {
            if (this.textBoxLine2.Text.Length < 34)
            {
                this.textBoxLine3.Enabled = true;
                this.textBoxLine3.Focus();
            }
            else
            {
                DisplayInfos();
            }
        }

        private void textBoxLine2_KeyPress(object sender, KeyPressEventArgs e)
        {
            nbLines = 2;

            if (e.KeyChar == (char)Keys.Escape)
                this.Close();
            if (e.KeyChar == (char)Keys.Enter)
            {
                nbLines=3;
                if (this.textBoxLine2.Text.Length < 34)
                {
                    this.textBoxLine3.Enabled = true;
                    this.textBoxLine3.Focus();
                    e.Handled = true;
                }
                else
                {
                    DisplayInfos();
                }
            }
        }

        private void textBoxLine3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                this.Close();
            if (e.KeyChar == (char)Keys.Enter)
            {
                nbLines =3;
                DisplayInfos();
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            MRZInfo = GetMrzInfo();
            this.DialogResult = DialogResult.Yes;
            SaveMrzToIniIfRequired();
            MessageBox.Show("Place the Passport on reader and continue");
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void textBoxDBirth_Leave(object sender, EventArgs e)
        {
            if (this.textBoxDBirth.Text.Length == 6)
            {
                this.textBoxDBirth.Text += ElyMRTDDotNet.ElyMRTDDotNet.calculateChecksum(this.textBoxDBirth.Text);
            }
        }

        private void textBoxDExp_Leave(object sender, EventArgs e)
        {
            if (this.textBoxDExp.Text.Length == 6)
            {
                this.textBoxDExp.Text += ElyMRTDDotNet.ElyMRTDDotNet.calculateChecksum(this.textBoxDExp.Text);
            }
        }

        private void textBoxDocNum_Leave(object sender, EventArgs e)
        {
            if (this.textBoxDocNum.Text.Length == 9)
            {
                this.textBoxDocNum.Text += ElyMRTDDotNet.ElyMRTDDotNet.calculateChecksum(this.textBoxDocNum.Text);
            }
        }

        private void TextBoxLine1_Leave(object sender, EventArgs e)
        {
            DisplayInfos();
        }

        private void TextBoxLine2_Leave(object sender, EventArgs e)
        {
            DisplayInfos();
        }

        private void buttonReadMRZfile_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult res = this.openFileDialogMRZfile.ShowDialog();
                if (res == System.Windows.Forms.DialogResult.OK)
                {
                    this.textBoxLine1.Text = "";
                    this.textBoxLine2.Text = "";
                    this.textBoxLine3.Text = "";
                    this.textBoxLine3.Enabled = false;
                    string[] lines = new string[3];
                    System.IO.StreamReader file = new System.IO.StreamReader(this.openFileDialogMRZfile.FileName);
                    lines[0] = file.ReadLine();
                    lines[1] = file.ReadLine();
                    lines[2] = file.ReadLine();
                    this.textBoxLine1.Text = lines[0];
                    this.textBoxLine2.Text = lines[1];
                    if (lines[2] != null && lines[2].Length > 0)
                    {
                        this.textBoxLine3.Text = lines[2];
                        this.textBoxLine3.Enabled = true;
                        DisplayInfos();
                    }
                    else
                        DisplayInfos();
                    file.Close();
                }
                else
                    return;
            }
            catch (Exception)
            {
                MessageBox.Show("The file does not exists !", "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void cbUseLastSavedMRZ_CheckedChanged(object sender, EventArgs e)
        {
            if (cbUseSavedMRZ.Checked)
                ReadSavedDetailsIfAny();
        }
        #endregion
    }
}
