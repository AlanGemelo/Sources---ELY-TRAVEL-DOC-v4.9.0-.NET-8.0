using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq;
using System.Diagnostics;

namespace ELY_TRAVEL_DOC
{
    public partial class FormOptions : Form
    {
        private IniFile SecIni;

        #region Set initial view from ini file
        private void setItemDisabled(ListViewItem item)
        {
            item.ForeColor = SystemColors.GrayText;
            item.Tag = "Disabled";
        }
        private void setListViewToDefaultForBac()
        {
            foreach (ListViewItem item in this.listView1.Items)
            {
                item.Checked = SecIni.StringToBool(SecIni.IniReadValue("ListView", item.Name));

                if (item.Name == "listViewItemCA" || item.Name == "listViewItemDG1" || item.Name == "listViewItemDG2" ||
                    item.Name == "listViewItemDG14" || item.Name == "listViewItemDG15")
                {
                    setItemDisabled(item);
                    item.Checked = true;
                }
                if (item.Checked)
                    item.StateImageIndex = 1;
            }
        }
        private void setListViewToDefaultForBap()
        {
            foreach (ListViewItem item in this.listView2.Items)
            {
                item.Checked = SecIni.StringToBool(SecIni.IniReadValue("ListViewBAP", item.Name));

                if (item.Name == "listViewItemDG1" || item.Name == "listViewItemDG4" || item.Name == "listViewItemDG6" ||
                    item.Name == "listViewItemDG13" || item.Name == "listViewItemDG14")
                {
                    setItemDisabled(item);
                    item.Checked = true;
                }
                if (item.Checked)
                    item.StateImageIndex = 1;
            }
        }
        private void setViewAccessControlOptions()
        {
            this.rbAccessControlAuto.Checked = SecIni.StringToBool(SecIni.IniReadValue("AccessControl", "Auto"));
            this.rbAccessControlBAC.Checked = SecIni.StringToBool(SecIni.IniReadValue("AccessControl", "BAC"));
            this.rbAccessControlPACE.Checked = SecIni.StringToBool(SecIni.IniReadValue("AccessControl", "PACE"));
            if (!this.rbAccessControlAuto.Checked && !this.rbAccessControlBAC.Checked && !this.rbAccessControlPACE.Checked)
                this.rbAccessControlAuto.Checked = true;
        }
        private void setViewPasswordTypeOptions()
        {
            this.rbPasswordTypeAskPwd.Checked = SecIni.StringToBool(SecIni.IniReadValue("PasswordType", "AskPwd"));
            this.rbPasswordTypeMRZ.Checked = SecIni.StringToBool(SecIni.IniReadValue("PasswordType", "MRZ"));
            this.rbPasswordTypeCAN.Checked = SecIni.StringToBool(SecIni.IniReadValue("PasswordType", "CAN"));
            this.textBoxDefaultCAN.Text = SecIni.IniReadValue("PasswordType", "DefaultCAN");
            if (!this.rbPasswordTypeAskPwd.Checked && !this.rbPasswordTypeMRZ.Checked && !this.rbPasswordTypeCAN.Checked)
                this.rbPasswordTypeMRZ.Checked = true;
        }
        private void setViewReadingOptions()
        {
            this.checkBoxPA.Checked = SecIni.StringToBool(SecIni.IniReadValue("Reading Options", "PA"));
            this.checkBoxAA.Checked = SecIni.StringToBool(SecIni.IniReadValue("Reading Options", "AA"));
            this.checkBoxCA.Checked = SecIni.StringToBool(SecIni.IniReadValue("Reading Options", "CA"));
            this.checkBoxTA.Checked = SecIni.StringToBool(SecIni.IniReadValue("Reading Options", "TA"));
            this.checkBoxEnableLog.Checked = SecIni.StringToBool(SecIni.IniReadValue("Reading Options", "Enable Log"));
            this.checkBoxManualMRZ.Checked = SecIni.StringToBool(SecIni.IniReadValue("Reading Options", "Manual MRZ"));
            this.checkBoxFindAntenna.Checked = SecIni.StringToBool(SecIni.IniReadValue("Reading Options", "Find Antenna"));
            this.checkBoxCSV.Checked = SecIni.StringToBool(SecIni.IniReadValue("Reading Options", "CSV"));
            this.checkBoxAutoDetect.Checked = SecIni.StringToBool(SecIni.IniReadValue("Reading Options", "AutoDetect"));
            this.checkboxEmrtdTest.Checked = SecIni.StringToBool(SecIni.IniReadValue("Reading Options", "EmrtdTest"));
            this.checkboxEmrtdTest.Visible = this.checkboxEmrtdTest.Checked;

            this.labelLogPath.Text = GetLogPath();

            string index = SecIni.IniReadValue("Card Configuration", "ApduSelection");
            if (index.Equals("0") || index.Equals("1") || index.Equals("2"))
            {
                this.comboBoxApduType.SelectedIndex = Int16.Parse(index);
            }
            else
            {
                // By default "Automatic" is selected
                this.comboBoxApduType.SelectedIndex = 0;
            }

            index = SecIni.IniReadValue("Card Configuration", "MaxLe");
            if (index.Equals("1") || index.Equals("2") || index.Equals("3") || index.Equals("4") ||
                index.Equals("5") || index.Equals("6") || index.Equals("7") || index.Equals("8"))
            {
                this.comboBoxMaxLe.SelectedIndex = Int16.Parse(index);
            }
            else
            {
                // By default "Default" is selected
                this.comboBoxMaxLe.SelectedIndex = 0;
            }
        }
        private void setViewCertificateOptions()
        {
            this.textBoxCSCA.Text = SecIni.IniReadValue("Passive Authentication", "CSCA");
            this.textBoxExternalDS.Text = SecIni.IniReadValue("Passive Authentication", "External DS");

            this.textBoxCVCALink.Text = SecIni.IniReadValue("Terminal Authentication", "CVCALink");
            this.textBoxDV.Text = SecIni.IniReadValue("Terminal Authentication", "DV");
            this.textBoxIS.Text = SecIni.IniReadValue("Terminal Authentication", "IS");
            this.textBoxISPK.Text = SecIni.IniReadValue("Terminal Authentication", "ISPK");
        }

        public FormOptions()
        {
            InitializeComponent();
            InitListViewBAC();
            InitListViewBAP();

            string path = Path.Combine(System.Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments), "ELYCTIS");
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            string iniPath = Path.Combine(path, "ELY TRAVEL DOC.ini");
            if (!File.Exists(iniPath))
                File.WriteAllText(iniPath, Properties.Resources.ELY_TRAVEL_DOC);

            SecIni = new IniFile(iniPath);
            if (SecIni == null)
            {
                MessageBox.Show(iniPath, "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            setListViewToDefaultForBac();
            setListViewToDefaultForBap();
            setViewAccessControlOptions();
            setViewPasswordTypeOptions();
            setViewReadingOptions();
            setViewCertificateOptions();
        }

        private void InitListViewBAC()
        {
            System.Windows.Forms.ListViewItem listViewItemCA = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "0",
                "Card Access",
                "Yes"}, -1);
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "1",
                "MRZ Data",
                "Yes"}, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "2",
                "Encoded Face",
                "Yes"}, -1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "3",
                "Encoded Finger(s)",
                "Yes"}, -1);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "4",
                "Encoded Eye(s)",
                "Yes"}, -1);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "5",
                "Displayed Portrait",
                "No"}, -1);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "6",
                "Reserved",
                "No"}, -1);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "7",
                "Displayed Signature or Usual Mark",
                "Yes"}, -1);
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "8",
                "Data Feature(s)",
                "No"}, -1);
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "9",
                "Structure Features",
                "No"}, -1);
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "10",
                "Substance Feature(s)",
                "No"}, -1);
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "11",
                "Additional Personal Detail(s)",
                "Yes"}, -1);
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "12",
                "Additional Document Detail(s)",
                "Yes"}, -1);
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "13",
                "Optional Detail(s)",
                "Yes"}, -1);
            System.Windows.Forms.ListViewItem listViewItem14 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "14",
                "EAC Public Key Info",
                "Yes"}, -1);
            System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "15",
                "AA Public Key Info",
                "Yes"}, -1);
            System.Windows.Forms.ListViewItem listViewItem16 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "16",
                "Person(s) to notify",
                "No"}, -1);
            listViewItemCA.Checked = true;
            listViewItemCA.StateImageIndex = 1;
            listViewItem1.Checked = true;
            listViewItem1.StateImageIndex = 1;
            listViewItem2.Checked = true;
            listViewItem2.StateImageIndex = 1;
            listViewItem3.StateImageIndex = 0;
            listViewItem4.StateImageIndex = 0;
            listViewItem5.StateImageIndex = 0;
            listViewItem6.StateImageIndex = 0;
            listViewItem7.StateImageIndex = 0;
            listViewItem8.StateImageIndex = 0;
            listViewItem9.StateImageIndex = 0;
            listViewItem10.StateImageIndex = 0;
            listViewItem11.StateImageIndex = 0;
            listViewItem12.StateImageIndex = 0;
            listViewItem13.StateImageIndex = 0;
            listViewItem14.Checked = true;
            listViewItem14.StateImageIndex = 1;
            listViewItem15.Checked = true;
            listViewItem15.StateImageIndex = 1;
            listViewItem16.StateImageIndex = 0;
            listViewItemCA.Name = "listViewItemCA";
            listViewItem1.Name = "listViewItemDG1";
            listViewItem2.Name = "listViewItemDG2";
            listViewItem3.Name = "listViewItemDG3";
            listViewItem4.Name = "listViewItemDG4";
            listViewItem5.Name = "listViewItemDG5";
            listViewItem6.Name = "listViewItemDG6";
            listViewItem7.Name = "listViewItemDG7";
            listViewItem8.Name = "listViewItemDG8";
            listViewItem9.Name = "listViewItemDG9";
            listViewItem10.Name = "listViewItemDG10";
            listViewItem11.Name = "listViewItemDG11";
            listViewItem12.Name = "listViewItemDG12";
            listViewItem13.Name = "listViewItemDG13";
            listViewItem14.Name = "listViewItemDG14";
            listViewItem15.Name = "listViewItemDG15";
            listViewItem16.Name = "listViewItemDG16";
            this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
                listViewItemCA,
                listViewItem1,
                listViewItem2,
                listViewItem3,
                listViewItem4,
                listViewItem5,
                listViewItem6,
                listViewItem7,
                listViewItem8,
                listViewItem9,
                listViewItem10,
                listViewItem11,
                listViewItem12,
                listViewItem13,
                listViewItem14,
                listViewItem15,
                listViewItem16});
        }

        private void InitListViewBAP()
        {
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "1",
                "Mandatory text data elements",
                "Yes"}, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "2",
                "Optional licence holder details",
                "No"}, -1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "3",
                "Optional issuing authority details",
                "No"}, -1);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "4",
                "Optional portrait image",
                "Yes"}, -1);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "5",
                "Optional signature / usual mark image",
                "Yes"}, -1);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "6",
                "Optional facial, fingerprint, iris and other biometric templates",
                "Yes"}, -1);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "7",
                "Optional facial, fingerprint, iris and other biometric templates",
                "No"}, -1);
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "8",
                "Optional facial, fingerprint, iris and other biometric templates",
                "No"}, -1);
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "9",
                "Optional facial, fingerprint, iris and other biometric templates",
                "No"}, -1);
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "10",
                "Reserved",
                "No"}, -1);
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "11",
                "Optional domestic data",
                "No"}, -1);
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "12",
                "Non-match alert",
                "No"}, -1);
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "13",
                "Active Authentication",
                "Yes"}, -1);
            System.Windows.Forms.ListViewItem listViewItem14 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "14",
                "Extended Access Protection",
                "Yes"}, -1);
            /*System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "15",
                "AA Public Key Info",
                "No"}, -1);*/
            /*System.Windows.Forms.ListViewItem listViewItem16 = new System.Windows.Forms.ListViewItem(new string[] {
                "",
                "16",
                "Person(s) to notify",
                "No"}, -1);*/
            listViewItem1.Checked = true;
            listViewItem1.StateImageIndex = 1;
            listViewItem2.StateImageIndex = 0;
            listViewItem3.StateImageIndex = 0;
            listViewItem4.Checked = true;
            listViewItem4.StateImageIndex = 1;
            listViewItem5.StateImageIndex = 0;
            listViewItem6.Checked = true;
            listViewItem6.StateImageIndex = 1;
            listViewItem7.StateImageIndex = 0;
            listViewItem8.StateImageIndex = 0;
            listViewItem9.StateImageIndex = 0;
            listViewItem10.StateImageIndex = 0;
            listViewItem11.StateImageIndex = 0;
            listViewItem12.StateImageIndex = 0;
            listViewItem13.Checked = true;
            listViewItem13.StateImageIndex = 1;
            listViewItem14.Checked = true;
            listViewItem14.StateImageIndex = 1;
            /*listViewItem15.StateImageIndex = 0;*/
            /*listViewItem16.StateImageIndex = 0;*/
            listViewItem1.Name = "listViewItemDG1";
            listViewItem2.Name = "listViewItemDG2";
            listViewItem3.Name = "listViewItemDG3";
            listViewItem4.Name = "listViewItemDG4";
            listViewItem5.Name = "listViewItemDG5";
            listViewItem6.Name = "listViewItemDG6";
            listViewItem7.Name = "listViewItemDG7";
            listViewItem8.Name = "listViewItemDG8";
            listViewItem9.Name = "listViewItemDG9";
            listViewItem10.Name = "listViewItemDG10";
            listViewItem11.Name = "listViewItemDG11";
            listViewItem12.Name = "listViewItemDG12";
            listViewItem13.Name = "listViewItemDG13";
            listViewItem14.Name = "listViewItemDG14";
            /*listViewItem15.Name = "listViewItemDG15";*/
            /*listViewItem16.Name = "listViewItemDG16";*/
            this.listView2.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
                listViewItem1,
                listViewItem2,
                listViewItem3,
                listViewItem4,
                listViewItem5,
                listViewItem6,
                listViewItem7,
                listViewItem8,
                listViewItem9,
                listViewItem10,
                listViewItem11,
                listViewItem12,
                listViewItem13,
                listViewItem14,
                /*listViewItem15,*/
                /*listViewItem16*/});
        }

        private void FormOptions_Load(object sender, EventArgs e)
        {
            setListViewToDefaultForBac();
            setListViewToDefaultForBap();
            setViewAccessControlOptions();
            setViewPasswordTypeOptions();
            setViewReadingOptions();
            setViewCertificateOptions();
            ModifyAccessControlOptions();
        }
        #endregion

        #region Trigger events
        private void listView1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            ListViewItem currentItem = this.listView1.Items[e.Index];
            if ((string)currentItem.Tag == "Disabled")
                e.NewValue = e.CurrentValue;
            if (currentItem.Name == "listViewItemCA" || currentItem.Name == "listViewItemDG1" || currentItem.Name == "listViewItemDG2" ||
                currentItem.Name == "listViewItemDG14" || currentItem.Name == "listViewItemDG15")
                e.NewValue = CheckState.Checked;
        }

        private void listView2_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            ListViewItem currentItem = this.listView2.Items[e.Index];
            if ((string)currentItem.Tag == "Disabled")
                e.NewValue = e.CurrentValue;
            if (currentItem.Name == "listViewItemDG1" || currentItem.Name == "listViewItemDG4" || currentItem.Name == "listViewItemDG6" ||
                currentItem.Name == "listViewItemDG13" || currentItem.Name == "listViewItemDG14")
                e.NewValue = CheckState.Checked;
        }

        private void checkBoxAA_CheckedChanged(object sender, EventArgs e)
        {
            /*ListViewItem item = (this.listView1.Items.Find("listViewItemDG15", true))?[0];
            item.Checked = this.checkBoxAA.Checked;

            item = (this.listView2.Items.Find("listViewItemDG13", true))?[0];
            item.Checked = this.checkBoxAA.Checked;*/
        }

        private void checkBoxCA_CheckedChanged(object sender, EventArgs e)
        {
            /*ListViewItem item = (this.listView1.Items.Find("listViewItemDG14", true))?[0];
            item.Checked = this.checkBoxCA.Checked;

            item = (this.listView2.Items.Find("listViewItemDG14", true))?[0];
            item.Checked = this.checkBoxCA.Checked;*/

            if (!this.checkBoxCA.Checked)
                this.checkBoxTA.Checked = false;
        }

        private void checkBoxTA_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBoxCertTA.Enabled = this.checkBoxTA.Checked;
            ListViewItem item = (this.listView1.Items.Find("listViewItemDG3", true))?[0];
            item.Checked = this.checkBoxTA.Checked;
            if (this.checkBoxTA.Checked)
                this.checkBoxCA.Checked = true;
        }

        private void checkBoxPA_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBoxCertPA.Enabled = this.checkBoxPA.Checked;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            bool error = false;

            if (this.checkBoxTA.Checked)
            {
                this.textBoxCVCALink.BackColor = System.Drawing.Color.White;
                this.textBoxDV.BackColor = System.Drawing.Color.White;
                this.textBoxIS.BackColor = System.Drawing.Color.White;
                this.textBoxISPK.BackColor = System.Drawing.Color.White;

                if (this.textBoxCVCALink.Text != string.Empty && !System.IO.File.Exists(this.textBoxCVCALink.Text))
                {
                    this.textBoxCVCALink.BackColor = System.Drawing.Color.Red;
                    error = true;
                }
                if (this.textBoxDV.Text == string.Empty || !System.IO.File.Exists(this.textBoxDV.Text))
                {
                    this.textBoxDV.BackColor = System.Drawing.Color.Red;
                    error = true;
                }
                if (this.textBoxIS.Text == string.Empty || !System.IO.File.Exists(this.textBoxIS.Text))
                {
                    this.textBoxIS.BackColor = System.Drawing.Color.Red;
                    error = true;
                }
                if (this.textBoxISPK.Text == string.Empty || !System.IO.File.Exists(this.textBoxISPK.Text))
                {
                    this.textBoxISPK.BackColor = System.Drawing.Color.Red;
                    error = true;
                }
                if (!error)
                {
                    SecIni.IniWriteValue("Terminal Authentication", "CVCALink", this.textBoxCVCALink.Text);
                    SecIni.IniWriteValue("Terminal Authentication", "DV", this.textBoxDV.Text);
                    SecIni.IniWriteValue("Terminal Authentication", "IS", this.textBoxIS.Text);
                    SecIni.IniWriteValue("Terminal Authentication", "ISPK", this.textBoxISPK.Text);
                }
            }
            if (this.checkBoxPA.Checked)
            {
                this.textBoxCSCA.BackColor = System.Drawing.Color.White;
                this.textBoxExternalDS.BackColor = System.Drawing.Color.White;

                // If certificates are provided, CSCA is mandatory and External DS is optional
                if (this.textBoxCSCA.Text != string.Empty && !System.IO.File.Exists(this.textBoxCSCA.Text))
                {
                    this.textBoxCSCA.BackColor = System.Drawing.Color.Red;
                    error = true;
                }
                if (this.textBoxExternalDS.Text != string.Empty && !System.IO.File.Exists(this.textBoxExternalDS.Text))
                {
                    this.textBoxExternalDS.BackColor = System.Drawing.Color.Red;
                    error = true;
                }
                if ((this.textBoxCSCA.Text == string.Empty) && (this.textBoxExternalDS.Text != string.Empty))
                {
                    this.textBoxCSCA.BackColor = System.Drawing.Color.Red;
                    error = true;
                }
                if (!error)
                {
                    SecIni.IniWriteValue("Passive Authentication", "CSCA", this.textBoxCSCA.Text);
                    SecIni.IniWriteValue("Passive Authentication", "External DS", this.textBoxExternalDS.Text);
                }
            }

            if (error)
                return;

            foreach (ListViewItem item in this.listView1.Items)
            {
                if (item.Name == "listViewItemDG1" || item.Name == "listViewItemDG2")
                    SecIni.IniWriteValue("ListView", item.Name, SecIni.boolToString(true));
                else
                    SecIni.IniWriteValue("ListView", item.Name, SecIni.boolToString(item.Checked));
            }

            foreach (ListViewItem item in this.listView2.Items)
            {
                if (item.Name == "listViewItemDG1" || item.Name == "listViewItemDG4" || item.Name == "listViewItemDG6")
                    SecIni.IniWriteValue("ListViewBAP", item.Name, SecIni.boolToString(true));
                else
                    SecIni.IniWriteValue("ListViewBAP", item.Name, SecIni.boolToString(item.Checked));
            }

            SecIni.IniWriteValue("AccessControl", "Auto", SecIni.boolToString(this.rbAccessControlAuto.Checked));
            SecIni.IniWriteValue("AccessControl", "BAC", SecIni.boolToString(this.rbAccessControlBAC.Checked));
            SecIni.IniWriteValue("AccessControl", "PACE", SecIni.boolToString(this.rbAccessControlPACE.Checked));
            SecIni.IniWriteValue("PasswordType", "AskPwd", SecIni.boolToString(this.rbPasswordTypeAskPwd.Checked));
            SecIni.IniWriteValue("PasswordType", "MRZ", SecIni.boolToString(this.rbPasswordTypeMRZ.Checked));
            SecIni.IniWriteValue("PasswordType", "CAN", SecIni.boolToString(this.rbPasswordTypeCAN.Checked));
            SecIni.IniWriteValue("PasswordType", "DefaultCAN", this.textBoxDefaultCAN.Text);

            SecIni.IniWriteValue("Reading Options", "PA", SecIni.boolToString(this.checkBoxPA.Checked));
            SecIni.IniWriteValue("Reading Options", "AA", SecIni.boolToString(this.checkBoxAA.Checked));
            SecIni.IniWriteValue("Reading Options", "CA", SecIni.boolToString(this.checkBoxCA.Checked));
            SecIni.IniWriteValue("Reading Options", "TA", SecIni.boolToString(this.checkBoxTA.Checked));
            SecIni.IniWriteValue("Reading Options", "Enable Log", SecIni.boolToString(this.checkBoxEnableLog.Checked));
            SecIni.IniWriteValue("Reading Options", "Manual MRZ", SecIni.boolToString(this.checkBoxManualMRZ.Checked));
            SecIni.IniWriteValue("Reading Options", "Find Antenna", SecIni.boolToString(this.checkBoxFindAntenna.Checked));
            SecIni.IniWriteValue("Reading Options", "CSV", SecIni.boolToString(this.checkBoxCSV.Checked));
            SecIni.IniWriteValue("Reading Options", "AutoDetect", SecIni.boolToString(this.checkBoxAutoDetect.Checked));
            SecIni.IniWriteValue("Reading Options", "EmrtdTest", SecIni.boolToString(this.checkboxEmrtdTest.Checked));
            SecIni.IniWriteValue("Card Configuration", "ApduSelection", this.comboBoxApduType.SelectedIndex.ToString());
            SecIni.IniWriteValue("Card Configuration", "MaxLe", this.comboBoxMaxLe.SelectedIndex.ToString());

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void buttonBrowseISPK_Click(object sender, EventArgs e)
        {
            if (this.openFileDialogISPK.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            this.textBoxISPK.Text = this.openFileDialogISPK.FileName;
            this.textBoxISPK.BackColor = System.Drawing.SystemColors.Window;
        }

        private void buttonBrowseDV_Click(object sender, EventArgs e)
        {
            if (this.openFileDialogCert.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            this.textBoxDV.Text = this.openFileDialogCert.FileName;
            this.textBoxDV.BackColor = System.Drawing.SystemColors.Window;
        }

        private void buttonBrowseIS_Click(object sender, EventArgs e)
        {
            if (this.openFileDialogCert.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            this.textBoxIS.Text = this.openFileDialogCert.FileName;
            this.textBoxIS.BackColor = System.Drawing.SystemColors.Window;
        }

        private void buttonBrowseCVCALink_Click(object sender, EventArgs e)
        {
            if (this.openFileDialogCert.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            this.textBoxCVCALink.Text = this.openFileDialogCert.FileName;
            this.textBoxCVCALink.BackColor = System.Drawing.SystemColors.Window;
        }

        private void buttonOpenCSV_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(GetLogPath(), "Extracted data.csv");
            if (File.Exists(fileName))
                MRZForm.OpenFile(fileName);
            else
                MessageBox.Show("The file does not exist yet\nPlease read a travel document first !", "No CSV file generated", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonBrowseCSCA_Click(object sender, EventArgs e)
        {
            if (this.openFileDialogPA.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            this.textBoxCSCA.Text = this.openFileDialogPA.FileName;
            this.textBoxCSCA.BackColor = System.Drawing.SystemColors.Window;
        }

        private void buttonBrowseExternalDS_Click(object sender, EventArgs e)
        {
            if (this.openFileDialogPA.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            this.textBoxExternalDS.Text = this.openFileDialogPA.FileName;
            this.textBoxExternalDS.BackColor = System.Drawing.SystemColors.Window;
        }

        private void ModifyAccessControlOptions()
        {
            if (rbAccessControlBAC.Checked)
            {
                rbPasswordTypeAskPwd.Enabled = false;
                rbPasswordTypeCAN.Enabled = false;
                rbPasswordTypeMRZ.Enabled = false;
                textBoxDefaultCAN.Enabled = false;
            }
            else
            {
                rbPasswordTypeAskPwd.Enabled = true;
                rbPasswordTypeCAN.Enabled = true;
                rbPasswordTypeMRZ.Enabled = true;
                textBoxDefaultCAN.Enabled = rbPasswordTypeCAN.Checked;
            }
        }
        private void rbAccessControlAuto_CheckedChanged(object sender, EventArgs e) { ModifyAccessControlOptions(); }
        private void rbAccessControlBAC_CheckedChanged(object sender, EventArgs e) { ModifyAccessControlOptions(); }
        private void rbAccessControlPACE_CheckedChanged(object sender, EventArgs e) { ModifyAccessControlOptions(); }
        private void rbPasswordTypeAskPwd_CheckedChanged(object sender, EventArgs e) { ModifyAccessControlOptions(); }
        private void rbPasswordTypeMRZ_CheckedChanged(object sender, EventArgs e) { ModifyAccessControlOptions(); }
        private void rbPasswordTypeCAN_CheckedChanged(object sender, EventArgs e) { ModifyAccessControlOptions(); }
        private void textBoxDefaultCAN_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for an invalid character in the KeyDown event
            // Only Numbers & backspace should be handled
            // NOTE: Delete key is handled by default
            e.Handled = !(char.IsDigit(e.KeyChar) || e.KeyChar.Equals('\b'));
        }

        private void checkboxEmrtdTest_CheckedChanged(object sender, EventArgs e)
        {
            // Get instance of MRZForm
            MRZForm MRZForm = Application.OpenForms.OfType<MRZForm>().FirstOrDefault();
            if (MRZForm == null)
                return;

            // Start or stop the eMRTD test loop
            MRZForm.SetEmrtdTest(checkboxEmrtdTest.Checked);
        }
        #endregion

        #region Get functions
        public String GetLogPath()
        {
            String path = SecIni.IniReadValue("Reading Options", "LogPath").Replace("\"", "").Replace("'", "");
            if (String.IsNullOrEmpty(path))
            {
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ELYCTIS");
            }
            return path;
        }
        public String GetPythonPath()
        {
            return SecIni.IniReadValue("Reading Options", "PythonPath").Replace("\"", "").Replace("'", "");
        }
        public bool IsUseTestMrzEnabled()
        {
            return SecIni.StringToBool(SecIni.IniReadValue("Manual MRZ", "UseTestMRZ"));
        }
        public string GetTestMrz()
        {
            if (IsUseTestMrzEnabled())
            {
                return SecIni.IniReadValue("Manual MRZ", "TestMrz")
                    .Replace("\\r", "\r").Replace("\\n", "\n")
                    .Replace("\"", "").Replace("'", "");
            }
            return "";
        }
        public int GetMaxLe()
        {
            int[] maxLe = { 0, 223, 256, 512, 1024, 2048, 3072, 4096, -1 };
            int index = (this.comboBoxMaxLe.SelectedIndex <= 8) ? this.comboBoxMaxLe.SelectedIndex : 0;
            return maxLe[index];
        }
        #endregion
    }
}
