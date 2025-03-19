namespace ELY_TRAVEL_DOC
{
    partial class FormOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOptions));
            listView1 = new System.Windows.Forms.ListView();
            ColumnSelect = new System.Windows.Forms.ColumnHeader();
            ColumnDatagroup = new System.Windows.Forms.ColumnHeader();
            ColumnContent = new System.Windows.Forms.ColumnHeader();
            ColumnSupported = new System.Windows.Forms.ColumnHeader();
            checkBoxPA = new System.Windows.Forms.CheckBox();
            checkBoxAA = new System.Windows.Forms.CheckBox();
            checkBoxCA = new System.Windows.Forms.CheckBox();
            checkBoxTA = new System.Windows.Forms.CheckBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            tabControlDG = new System.Windows.Forms.TabControl();
            tabPageBAC = new System.Windows.Forms.TabPage();
            tabPageBAP = new System.Windows.Forms.TabPage();
            listView2 = new System.Windows.Forms.ListView();
            columnHeader1 = new System.Windows.Forms.ColumnHeader();
            columnHeader2 = new System.Windows.Forms.ColumnHeader();
            columnHeader3 = new System.Windows.Forms.ColumnHeader();
            columnHeader4 = new System.Windows.Forms.ColumnHeader();
            buttonOK = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            buttonBrowseDV = new System.Windows.Forms.Button();
            buttonBrowseIS = new System.Windows.Forms.Button();
            buttonBrowseISPK = new System.Windows.Forms.Button();
            groupBoxCertTA = new System.Windows.Forms.GroupBox();
            labelCvcaLinkCert = new System.Windows.Forms.Label();
            textBoxCVCALink = new System.Windows.Forms.TextBox();
            buttonBrowseCVCALink = new System.Windows.Forms.Button();
            labelIsPrivateKey = new System.Windows.Forms.Label();
            labelIsCert = new System.Windows.Forms.Label();
            textBoxISPK = new System.Windows.Forms.TextBox();
            textBoxIS = new System.Windows.Forms.TextBox();
            labelDvCert = new System.Windows.Forms.Label();
            textBoxDV = new System.Windows.Forms.TextBox();
            openFileDialogCert = new System.Windows.Forms.OpenFileDialog();
            openFileDialogISPK = new System.Windows.Forms.OpenFileDialog();
            checkBoxEnableLog = new System.Windows.Forms.CheckBox();
            groupBox3 = new System.Windows.Forms.GroupBox();
            checkboxEmrtdTest = new System.Windows.Forms.CheckBox();
            checkBoxFindAntenna = new System.Windows.Forms.CheckBox();
            checkBoxAutoDetect = new System.Windows.Forms.CheckBox();
            labelLogPath = new System.Windows.Forms.Label();
            checkBoxManualMRZ = new System.Windows.Forms.CheckBox();
            buttonOpenCSV = new System.Windows.Forms.Button();
            checkBoxCSV = new System.Windows.Forms.CheckBox();
            groupBoxCertPA = new System.Windows.Forms.GroupBox();
            labelExtDSCert = new System.Windows.Forms.Label();
            textBoxExternalDS = new System.Windows.Forms.TextBox();
            labelCSCACert = new System.Windows.Forms.Label();
            textBoxCSCA = new System.Windows.Forms.TextBox();
            buttonBrowseExternalDS = new System.Windows.Forms.Button();
            buttonBrowseCSCA = new System.Windows.Forms.Button();
            openFileDialogPA = new System.Windows.Forms.OpenFileDialog();
            groupBox4 = new System.Windows.Forms.GroupBox();
            rbAccessControlPACE = new System.Windows.Forms.RadioButton();
            rbAccessControlBAC = new System.Windows.Forms.RadioButton();
            rbAccessControlAuto = new System.Windows.Forms.RadioButton();
            rbPasswordTypeAskPwd = new System.Windows.Forms.RadioButton();
            rbPasswordTypeMRZ = new System.Windows.Forms.RadioButton();
            rbPasswordTypeCAN = new System.Windows.Forms.RadioButton();
            groupBox5 = new System.Windows.Forms.GroupBox();
            label8 = new System.Windows.Forms.Label();
            textBoxDefaultCAN = new System.Windows.Forms.TextBox();
            groupBox6 = new System.Windows.Forms.GroupBox();
            comboBoxMaxLe = new System.Windows.Forms.ComboBox();
            labelMaxLe = new System.Windows.Forms.Label();
            comboBoxApduType = new System.Windows.Forms.ComboBox();
            label9 = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            tabControlDG.SuspendLayout();
            tabPageBAC.SuspendLayout();
            tabPageBAP.SuspendLayout();
            groupBoxCertTA.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBoxCertPA.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox6.SuspendLayout();
            SuspendLayout();
            // 
            // listView1
            // 
            listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            listView1.CheckBoxes = true;
            listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { ColumnSelect, ColumnDatagroup, ColumnContent, ColumnSupported });
            listView1.FullRowSelect = true;
            listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            listView1.Location = new System.Drawing.Point(1, 4);
            listView1.Name = "listView1";
            listView1.Size = new System.Drawing.Size(449, 317);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = System.Windows.Forms.View.Details;
            listView1.ItemCheck += listView1_ItemCheck;
            // 
            // ColumnSelect
            // 
            ColumnSelect.Text = "Select";
            ColumnSelect.Width = 44;
            // 
            // ColumnDatagroup
            // 
            ColumnDatagroup.Text = "Datagroup";
            ColumnDatagroup.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            ColumnDatagroup.Width = 64;
            // 
            // ColumnContent
            // 
            ColumnContent.Text = "                         Content";
            ColumnContent.Width = 257;
            // 
            // ColumnSupported
            // 
            ColumnSupported.Text = "Supported";
            ColumnSupported.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            ColumnSupported.Width = 84;
            // 
            // checkBoxPA
            // 
            checkBoxPA.Location = new System.Drawing.Point(14, 23);
            checkBoxPA.Name = "checkBoxPA";
            checkBoxPA.Size = new System.Drawing.Size(279, 17);
            checkBoxPA.TabIndex = 0;
            checkBoxPA.Text = "PA - Passive Authentication";
            checkBoxPA.UseVisualStyleBackColor = true;
            checkBoxPA.CheckedChanged += checkBoxPA_CheckedChanged;
            // 
            // checkBoxAA
            // 
            checkBoxAA.Location = new System.Drawing.Point(14, 46);
            checkBoxAA.Name = "checkBoxAA";
            checkBoxAA.Size = new System.Drawing.Size(279, 17);
            checkBoxAA.TabIndex = 1;
            checkBoxAA.Text = "AA - Active Authentication";
            checkBoxAA.UseVisualStyleBackColor = true;
            checkBoxAA.CheckedChanged += checkBoxAA_CheckedChanged;
            // 
            // checkBoxCA
            // 
            checkBoxCA.Location = new System.Drawing.Point(14, 69);
            checkBoxCA.Name = "checkBoxCA";
            checkBoxCA.Size = new System.Drawing.Size(279, 17);
            checkBoxCA.TabIndex = 2;
            checkBoxCA.Text = "CA - Chip Authentication";
            checkBoxCA.UseVisualStyleBackColor = true;
            checkBoxCA.CheckedChanged += checkBoxCA_CheckedChanged;
            // 
            // checkBoxTA
            // 
            checkBoxTA.Location = new System.Drawing.Point(14, 92);
            checkBoxTA.Name = "checkBoxTA";
            checkBoxTA.Size = new System.Drawing.Size(279, 17);
            checkBoxTA.TabIndex = 3;
            checkBoxTA.Text = "TA - Terminal Authentication (if CA is enabled)";
            checkBoxTA.UseVisualStyleBackColor = true;
            checkBoxTA.CheckedChanged += checkBoxTA_CheckedChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(checkBoxTA);
            groupBox1.Controls.Add(checkBoxCA);
            groupBox1.Controls.Add(checkBoxAA);
            groupBox1.Controls.Add(checkBoxPA);
            groupBox1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            groupBox1.Location = new System.Drawing.Point(495, 107);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(304, 117);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Reading Options";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(tabControlDG);
            groupBox2.Font = new System.Drawing.Font("Tahoma", 8.25F);
            groupBox2.Location = new System.Drawing.Point(10, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(474, 396);
            groupBox2.TabIndex = 0;
            groupBox2.TabStop = false;
            groupBox2.Text = "DataGroup Reading Options";
            // 
            // tabControlDG
            // 
            tabControlDG.Controls.Add(tabPageBAC);
            tabControlDG.Controls.Add(tabPageBAP);
            tabControlDG.Location = new System.Drawing.Point(11, 20);
            tabControlDG.Name = "tabControlDG";
            tabControlDG.SelectedIndex = 0;
            tabControlDG.Size = new System.Drawing.Size(454, 361);
            tabControlDG.TabIndex = 0;
            // 
            // tabPageBAC
            // 
            tabPageBAC.Controls.Add(listView1);
            tabPageBAC.Location = new System.Drawing.Point(4, 22);
            tabPageBAC.Name = "tabPageBAC";
            tabPageBAC.Padding = new System.Windows.Forms.Padding(3);
            tabPageBAC.Size = new System.Drawing.Size(446, 335);
            tabPageBAC.TabIndex = 0;
            tabPageBAC.Text = "BAC";
            tabPageBAC.UseVisualStyleBackColor = true;
            // 
            // tabPageBAP
            // 
            tabPageBAP.Controls.Add(listView2);
            tabPageBAP.Location = new System.Drawing.Point(4, 22);
            tabPageBAP.Name = "tabPageBAP";
            tabPageBAP.Padding = new System.Windows.Forms.Padding(3);
            tabPageBAP.Size = new System.Drawing.Size(446, 335);
            tabPageBAP.TabIndex = 1;
            tabPageBAP.Text = "BAP";
            tabPageBAP.UseVisualStyleBackColor = true;
            // 
            // listView2
            // 
            listView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            listView2.CheckBoxes = true;
            listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4 });
            listView2.FullRowSelect = true;
            listView2.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            listView2.Location = new System.Drawing.Point(1, 4);
            listView2.Name = "listView2";
            listView2.Size = new System.Drawing.Size(449, 317);
            listView2.TabIndex = 1;
            listView2.UseCompatibleStateImageBehavior = false;
            listView2.View = System.Windows.Forms.View.Details;
            listView2.ItemCheck += listView2_ItemCheck;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Select";
            columnHeader1.Width = 44;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Datagroup";
            columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            columnHeader2.Width = 64;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "                         Content";
            columnHeader3.Width = 257;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Supported";
            columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            columnHeader4.Width = 84;
            // 
            // buttonOK
            // 
            buttonOK.Font = new System.Drawing.Font("Tahoma", 8.25F);
            buttonOK.Location = new System.Drawing.Point(641, 656);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new System.Drawing.Size(80, 27);
            buttonOK.TabIndex = 8;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Font = new System.Drawing.Font("Tahoma", 8.25F);
            buttonCancel.Location = new System.Drawing.Point(726, 656);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(73, 27);
            buttonCancel.TabIndex = 9;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // buttonBrowseDV
            // 
            buttonBrowseDV.Location = new System.Drawing.Point(703, 47);
            buttonBrowseDV.Name = "buttonBrowseDV";
            buttonBrowseDV.Size = new System.Drawing.Size(75, 23);
            buttonBrowseDV.TabIndex = 5;
            buttonBrowseDV.Text = "Browse";
            buttonBrowseDV.UseVisualStyleBackColor = true;
            buttonBrowseDV.Click += buttonBrowseDV_Click;
            // 
            // buttonBrowseIS
            // 
            buttonBrowseIS.Location = new System.Drawing.Point(703, 71);
            buttonBrowseIS.Name = "buttonBrowseIS";
            buttonBrowseIS.Size = new System.Drawing.Size(75, 23);
            buttonBrowseIS.TabIndex = 8;
            buttonBrowseIS.Text = "Browse";
            buttonBrowseIS.UseVisualStyleBackColor = true;
            buttonBrowseIS.Click += buttonBrowseIS_Click;
            // 
            // buttonBrowseISPK
            // 
            buttonBrowseISPK.Location = new System.Drawing.Point(703, 95);
            buttonBrowseISPK.Name = "buttonBrowseISPK";
            buttonBrowseISPK.Size = new System.Drawing.Size(75, 23);
            buttonBrowseISPK.TabIndex = 11;
            buttonBrowseISPK.Text = "Browse";
            buttonBrowseISPK.UseVisualStyleBackColor = true;
            buttonBrowseISPK.Click += buttonBrowseISPK_Click;
            // 
            // groupBoxCertTA
            // 
            groupBoxCertTA.Controls.Add(labelCvcaLinkCert);
            groupBoxCertTA.Controls.Add(textBoxCVCALink);
            groupBoxCertTA.Controls.Add(buttonBrowseCVCALink);
            groupBoxCertTA.Controls.Add(labelIsPrivateKey);
            groupBoxCertTA.Controls.Add(labelIsCert);
            groupBoxCertTA.Controls.Add(textBoxISPK);
            groupBoxCertTA.Controls.Add(textBoxIS);
            groupBoxCertTA.Controls.Add(labelDvCert);
            groupBoxCertTA.Controls.Add(textBoxDV);
            groupBoxCertTA.Controls.Add(buttonBrowseISPK);
            groupBoxCertTA.Controls.Add(buttonBrowseIS);
            groupBoxCertTA.Controls.Add(buttonBrowseDV);
            groupBoxCertTA.Enabled = false;
            groupBoxCertTA.Font = new System.Drawing.Font("Tahoma", 8.25F);
            groupBoxCertTA.Location = new System.Drawing.Point(10, 514);
            groupBoxCertTA.Name = "groupBoxCertTA";
            groupBoxCertTA.Size = new System.Drawing.Size(789, 131);
            groupBoxCertTA.TabIndex = 7;
            groupBoxCertTA.TabStop = false;
            groupBoxCertTA.Text = "Certificates for Terminal Authentication";
            // 
            // labelCvcaLinkCert
            // 
            labelCvcaLinkCert.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            labelCvcaLinkCert.Location = new System.Drawing.Point(6, 26);
            labelCvcaLinkCert.Name = "labelCvcaLinkCert";
            labelCvcaLinkCert.Size = new System.Drawing.Size(135, 13);
            labelCvcaLinkCert.TabIndex = 0;
            labelCvcaLinkCert.Text = "(Optional) CVCA Link:";
            labelCvcaLinkCert.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBoxCVCALink
            // 
            textBoxCVCALink.BackColor = System.Drawing.SystemColors.Window;
            textBoxCVCALink.Location = new System.Drawing.Point(145, 24);
            textBoxCVCALink.Name = "textBoxCVCALink";
            textBoxCVCALink.Size = new System.Drawing.Size(555, 21);
            textBoxCVCALink.TabIndex = 1;
            // 
            // buttonBrowseCVCALink
            // 
            buttonBrowseCVCALink.Location = new System.Drawing.Point(703, 23);
            buttonBrowseCVCALink.Name = "buttonBrowseCVCALink";
            buttonBrowseCVCALink.Size = new System.Drawing.Size(75, 23);
            buttonBrowseCVCALink.TabIndex = 2;
            buttonBrowseCVCALink.Text = "Browse";
            buttonBrowseCVCALink.UseVisualStyleBackColor = true;
            buttonBrowseCVCALink.Click += buttonBrowseCVCALink_Click;
            // 
            // labelIsPrivateKey
            // 
            labelIsPrivateKey.Location = new System.Drawing.Point(6, 99);
            labelIsPrivateKey.Name = "labelIsPrivateKey";
            labelIsPrivateKey.Size = new System.Drawing.Size(135, 13);
            labelIsPrivateKey.TabIndex = 9;
            labelIsPrivateKey.Text = "IS Private Key:";
            labelIsPrivateKey.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelIsCert
            // 
            labelIsCert.Location = new System.Drawing.Point(6, 74);
            labelIsCert.Name = "labelIsCert";
            labelIsCert.Size = new System.Drawing.Size(135, 13);
            labelIsCert.TabIndex = 6;
            labelIsCert.Text = "IS:";
            labelIsCert.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBoxISPK
            // 
            textBoxISPK.BackColor = System.Drawing.SystemColors.Window;
            textBoxISPK.Location = new System.Drawing.Point(145, 96);
            textBoxISPK.Name = "textBoxISPK";
            textBoxISPK.ReadOnly = true;
            textBoxISPK.Size = new System.Drawing.Size(555, 21);
            textBoxISPK.TabIndex = 10;
            // 
            // textBoxIS
            // 
            textBoxIS.BackColor = System.Drawing.SystemColors.Window;
            textBoxIS.Location = new System.Drawing.Point(145, 72);
            textBoxIS.Name = "textBoxIS";
            textBoxIS.ReadOnly = true;
            textBoxIS.Size = new System.Drawing.Size(555, 21);
            textBoxIS.TabIndex = 7;
            // 
            // labelDvCert
            // 
            labelDvCert.Location = new System.Drawing.Point(6, 50);
            labelDvCert.Name = "labelDvCert";
            labelDvCert.Size = new System.Drawing.Size(135, 13);
            labelDvCert.TabIndex = 3;
            labelDvCert.Text = "DV:";
            labelDvCert.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBoxDV
            // 
            textBoxDV.BackColor = System.Drawing.SystemColors.Window;
            textBoxDV.Location = new System.Drawing.Point(145, 48);
            textBoxDV.Name = "textBoxDV";
            textBoxDV.ReadOnly = true;
            textBoxDV.Size = new System.Drawing.Size(555, 21);
            textBoxDV.TabIndex = 4;
            // 
            // openFileDialogCert
            // 
            openFileDialogCert.AddExtension = false;
            openFileDialogCert.Filter = "cvcert files|*.cvcert";
            // 
            // openFileDialogISPK
            // 
            openFileDialogISPK.AddExtension = false;
            openFileDialogISPK.Filter = "private key files|*.pem;*.pkcs8";
            // 
            // checkBoxEnableLog
            // 
            checkBoxEnableLog.Location = new System.Drawing.Point(189, 23);
            checkBoxEnableLog.Name = "checkBoxEnableLog";
            checkBoxEnableLog.Size = new System.Drawing.Size(78, 17);
            checkBoxEnableLog.TabIndex = 4;
            checkBoxEnableLog.Text = "Enable Log";
            checkBoxEnableLog.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(checkboxEmrtdTest);
            groupBox3.Controls.Add(checkBoxFindAntenna);
            groupBox3.Controls.Add(checkBoxAutoDetect);
            groupBox3.Controls.Add(labelLogPath);
            groupBox3.Controls.Add(checkBoxManualMRZ);
            groupBox3.Controls.Add(buttonOpenCSV);
            groupBox3.Controls.Add(checkBoxCSV);
            groupBox3.Controls.Add(checkBoxEnableLog);
            groupBox3.Font = new System.Drawing.Font("Tahoma", 8.25F);
            groupBox3.Location = new System.Drawing.Point(495, 232);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(304, 113);
            groupBox3.TabIndex = 4;
            groupBox3.TabStop = false;
            groupBox3.Text = "Additional options";
            // 
            // checkboxEmrtdTest
            // 
            checkboxEmrtdTest.AutoSize = true;
            checkboxEmrtdTest.Location = new System.Drawing.Point(14, 90);
            checkboxEmrtdTest.Name = "checkboxEmrtdTest";
            checkboxEmrtdTest.Size = new System.Drawing.Size(84, 17);
            checkboxEmrtdTest.TabIndex = 3;
            checkboxEmrtdTest.Text = "eMRTD Test";
            checkboxEmrtdTest.UseVisualStyleBackColor = true;
            checkboxEmrtdTest.CheckedChanged += checkboxEmrtdTest_CheckedChanged;
            // 
            // checkBoxFindAntenna
            // 
            checkBoxFindAntenna.Location = new System.Drawing.Point(14, 69);
            checkBoxFindAntenna.Name = "checkBoxFindAntenna";
            checkBoxFindAntenna.Size = new System.Drawing.Size(147, 17);
            checkBoxFindAntenna.TabIndex = 2;
            checkBoxFindAntenna.Text = "Find antenna during read";
            checkBoxFindAntenna.UseVisualStyleBackColor = true;
            // 
            // checkBoxAutoDetect
            // 
            checkBoxAutoDetect.Location = new System.Drawing.Point(14, 23);
            checkBoxAutoDetect.Name = "checkBoxAutoDetect";
            checkBoxAutoDetect.Size = new System.Drawing.Size(83, 17);
            checkBoxAutoDetect.TabIndex = 0;
            checkBoxAutoDetect.Text = "Auto detect document";
            checkBoxAutoDetect.UseVisualStyleBackColor = true;
            // 
            // labelLogPath
            // 
            labelLogPath.Anchor = System.Windows.Forms.AnchorStyles.Right;
            labelLogPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Italic);
            labelLogPath.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            labelLogPath.Location = new System.Drawing.Point(14, 90);
            labelLogPath.Name = "labelLogPath";
            labelLogPath.Size = new System.Drawing.Size(279, 15);
            labelLogPath.TabIndex = 7;
            labelLogPath.Text = "Path: \\Documents\\ELYCTIS\\";
            labelLogPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBoxManualMRZ
            // 
            checkBoxManualMRZ.Location = new System.Drawing.Point(14, 46);
            checkBoxManualMRZ.Name = "checkBoxManualMRZ";
            checkBoxManualMRZ.Size = new System.Drawing.Size(121, 17);
            checkBoxManualMRZ.TabIndex = 1;
            checkBoxManualMRZ.Text = "Enter MRZ manually";
            checkBoxManualMRZ.UseVisualStyleBackColor = true;
            // 
            // buttonOpenCSV
            // 
            buttonOpenCSV.Location = new System.Drawing.Point(217, 65);
            buttonOpenCSV.Name = "buttonOpenCSV";
            buttonOpenCSV.Size = new System.Drawing.Size(76, 26);
            buttonOpenCSV.TabIndex = 6;
            buttonOpenCSV.Text = "Open file";
            buttonOpenCSV.UseVisualStyleBackColor = true;
            buttonOpenCSV.Click += buttonOpenCSV_Click;
            // 
            // checkBoxCSV
            // 
            checkBoxCSV.Location = new System.Drawing.Point(189, 46);
            checkBoxCSV.Name = "checkBoxCSV";
            checkBoxCSV.Size = new System.Drawing.Size(100, 17);
            checkBoxCSV.TabIndex = 5;
            checkBoxCSV.Text = "Save in CSV file";
            checkBoxCSV.UseVisualStyleBackColor = true;
            // 
            // groupBoxCertPA
            // 
            groupBoxCertPA.Controls.Add(labelExtDSCert);
            groupBoxCertPA.Controls.Add(textBoxExternalDS);
            groupBoxCertPA.Controls.Add(labelCSCACert);
            groupBoxCertPA.Controls.Add(textBoxCSCA);
            groupBoxCertPA.Controls.Add(buttonBrowseExternalDS);
            groupBoxCertPA.Controls.Add(buttonBrowseCSCA);
            groupBoxCertPA.Enabled = false;
            groupBoxCertPA.Font = new System.Drawing.Font("Tahoma", 8.25F);
            groupBoxCertPA.Location = new System.Drawing.Point(10, 420);
            groupBoxCertPA.Name = "groupBoxCertPA";
            groupBoxCertPA.Size = new System.Drawing.Size(789, 84);
            groupBoxCertPA.TabIndex = 6;
            groupBoxCertPA.TabStop = false;
            groupBoxCertPA.Text = "Certificates for Passive Authentication";
            // 
            // labelExtDSCert
            // 
            labelExtDSCert.Location = new System.Drawing.Point(6, 52);
            labelExtDSCert.Name = "labelExtDSCert";
            labelExtDSCert.Size = new System.Drawing.Size(135, 13);
            labelExtDSCert.TabIndex = 3;
            labelExtDSCert.Text = "(Optional) External DS:";
            labelExtDSCert.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBoxExternalDS
            // 
            textBoxExternalDS.BackColor = System.Drawing.SystemColors.Window;
            textBoxExternalDS.Location = new System.Drawing.Point(146, 49);
            textBoxExternalDS.Name = "textBoxExternalDS";
            textBoxExternalDS.Size = new System.Drawing.Size(554, 21);
            textBoxExternalDS.TabIndex = 4;
            // 
            // labelCSCACert
            // 
            labelCSCACert.Location = new System.Drawing.Point(6, 28);
            labelCSCACert.Name = "labelCSCACert";
            labelCSCACert.Size = new System.Drawing.Size(135, 13);
            labelCSCACert.TabIndex = 0;
            labelCSCACert.Text = "CSCA:";
            labelCSCACert.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBoxCSCA
            // 
            textBoxCSCA.BackColor = System.Drawing.SystemColors.Window;
            textBoxCSCA.Location = new System.Drawing.Point(146, 25);
            textBoxCSCA.Name = "textBoxCSCA";
            textBoxCSCA.Size = new System.Drawing.Size(554, 21);
            textBoxCSCA.TabIndex = 1;
            // 
            // buttonBrowseExternalDS
            // 
            buttonBrowseExternalDS.Location = new System.Drawing.Point(703, 48);
            buttonBrowseExternalDS.Name = "buttonBrowseExternalDS";
            buttonBrowseExternalDS.Size = new System.Drawing.Size(75, 23);
            buttonBrowseExternalDS.TabIndex = 5;
            buttonBrowseExternalDS.Text = "Browse";
            buttonBrowseExternalDS.UseVisualStyleBackColor = true;
            buttonBrowseExternalDS.Click += buttonBrowseExternalDS_Click;
            // 
            // buttonBrowseCSCA
            // 
            buttonBrowseCSCA.Location = new System.Drawing.Point(703, 24);
            buttonBrowseCSCA.Name = "buttonBrowseCSCA";
            buttonBrowseCSCA.Size = new System.Drawing.Size(75, 23);
            buttonBrowseCSCA.TabIndex = 2;
            buttonBrowseCSCA.Text = "Browse";
            buttonBrowseCSCA.UseVisualStyleBackColor = true;
            buttonBrowseCSCA.Click += buttonBrowseCSCA_Click;
            // 
            // openFileDialogPA
            // 
            openFileDialogPA.Filter = "\"Certificate Files\"|*.cer;*.der;*.crt;*.cert";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(rbAccessControlPACE);
            groupBox4.Controls.Add(rbAccessControlBAC);
            groupBox4.Controls.Add(rbAccessControlAuto);
            groupBox4.Font = new System.Drawing.Font("Tahoma", 8.25F);
            groupBox4.Location = new System.Drawing.Point(495, 12);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new System.Drawing.Size(151, 89);
            groupBox4.TabIndex = 1;
            groupBox4.TabStop = false;
            groupBox4.Text = "Access control options";
            // 
            // rbAccessControlPACE
            // 
            rbAccessControlPACE.Location = new System.Drawing.Point(14, 60);
            rbAccessControlPACE.Name = "rbAccessControlPACE";
            rbAccessControlPACE.Size = new System.Drawing.Size(51, 17);
            rbAccessControlPACE.TabIndex = 2;
            rbAccessControlPACE.Text = "PACE";
            rbAccessControlPACE.UseVisualStyleBackColor = true;
            rbAccessControlPACE.CheckedChanged += rbAccessControlPACE_CheckedChanged;
            // 
            // rbAccessControlBAC
            // 
            rbAccessControlBAC.Location = new System.Drawing.Point(14, 40);
            rbAccessControlBAC.Name = "rbAccessControlBAC";
            rbAccessControlBAC.Size = new System.Drawing.Size(135, 18);
            rbAccessControlBAC.TabIndex = 1;
            rbAccessControlBAC.Text = "BAC/BAP";
            rbAccessControlBAC.UseVisualStyleBackColor = true;
            rbAccessControlBAC.CheckedChanged += rbAccessControlBAC_CheckedChanged;
            // 
            // rbAccessControlAuto
            // 
            rbAccessControlAuto.Checked = true;
            rbAccessControlAuto.Location = new System.Drawing.Point(14, 20);
            rbAccessControlAuto.Name = "rbAccessControlAuto";
            rbAccessControlAuto.Size = new System.Drawing.Size(135, 17);
            rbAccessControlAuto.TabIndex = 0;
            rbAccessControlAuto.TabStop = true;
            rbAccessControlAuto.Text = "Auto (PACE/BAC/BAP)";
            rbAccessControlAuto.UseVisualStyleBackColor = true;
            rbAccessControlAuto.CheckedChanged += rbAccessControlAuto_CheckedChanged;
            // 
            // rbPasswordTypeAskPwd
            // 
            rbPasswordTypeAskPwd.Location = new System.Drawing.Point(15, 20);
            rbPasswordTypeAskPwd.Name = "rbPasswordTypeAskPwd";
            rbPasswordTypeAskPwd.Size = new System.Drawing.Size(96, 17);
            rbPasswordTypeAskPwd.TabIndex = 0;
            rbPasswordTypeAskPwd.Text = "Ask every time";
            rbPasswordTypeAskPwd.UseVisualStyleBackColor = true;
            rbPasswordTypeAskPwd.CheckedChanged += rbPasswordTypeAskPwd_CheckedChanged;
            // 
            // rbPasswordTypeMRZ
            // 
            rbPasswordTypeMRZ.Checked = true;
            rbPasswordTypeMRZ.Location = new System.Drawing.Point(15, 40);
            rbPasswordTypeMRZ.Name = "rbPasswordTypeMRZ";
            rbPasswordTypeMRZ.Size = new System.Drawing.Size(46, 17);
            rbPasswordTypeMRZ.TabIndex = 1;
            rbPasswordTypeMRZ.TabStop = true;
            rbPasswordTypeMRZ.Text = "MRZ";
            rbPasswordTypeMRZ.UseVisualStyleBackColor = true;
            rbPasswordTypeMRZ.CheckedChanged += rbPasswordTypeMRZ_CheckedChanged;
            // 
            // rbPasswordTypeCAN
            // 
            rbPasswordTypeCAN.Location = new System.Drawing.Point(15, 60);
            rbPasswordTypeCAN.Name = "rbPasswordTypeCAN";
            rbPasswordTypeCAN.Size = new System.Drawing.Size(46, 17);
            rbPasswordTypeCAN.TabIndex = 2;
            rbPasswordTypeCAN.Text = "CAN";
            rbPasswordTypeCAN.UseVisualStyleBackColor = true;
            rbPasswordTypeCAN.CheckedChanged += rbPasswordTypeCAN_CheckedChanged;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(label8);
            groupBox5.Controls.Add(textBoxDefaultCAN);
            groupBox5.Controls.Add(rbPasswordTypeCAN);
            groupBox5.Controls.Add(rbPasswordTypeMRZ);
            groupBox5.Controls.Add(rbPasswordTypeAskPwd);
            groupBox5.Font = new System.Drawing.Font("Tahoma", 8.25F);
            groupBox5.Location = new System.Drawing.Point(657, 12);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new System.Drawing.Size(142, 89);
            groupBox5.TabIndex = 2;
            groupBox5.TabStop = false;
            groupBox5.Text = "Password type";
            // 
            // label8
            // 
            label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Italic);
            label8.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            label8.Location = new System.Drawing.Point(67, 40);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(64, 18);
            label8.TabIndex = 4;
            label8.Text = "default CAN";
            label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxDefaultCAN
            // 
            textBoxDefaultCAN.Font = new System.Drawing.Font("Courier New", 9F);
            textBoxDefaultCAN.Location = new System.Drawing.Point(80, 58);
            textBoxDefaultCAN.MaxLength = 6;
            textBoxDefaultCAN.Name = "textBoxDefaultCAN";
            textBoxDefaultCAN.Size = new System.Drawing.Size(51, 21);
            textBoxDefaultCAN.TabIndex = 3;
            textBoxDefaultCAN.Text = "123456";
            textBoxDefaultCAN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            textBoxDefaultCAN.KeyPress += textBoxDefaultCAN_KeyPress;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(comboBoxMaxLe);
            groupBox6.Controls.Add(labelMaxLe);
            groupBox6.Controls.Add(comboBoxApduType);
            groupBox6.Controls.Add(label9);
            groupBox6.Font = new System.Drawing.Font("Tahoma", 8.25F);
            groupBox6.Location = new System.Drawing.Point(495, 351);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new System.Drawing.Size(304, 57);
            groupBox6.TabIndex = 5;
            groupBox6.TabStop = false;
            groupBox6.Text = "Card configuration";
            // 
            // comboBoxMaxLe
            // 
            comboBoxMaxLe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxMaxLe.FormattingEnabled = true;
            comboBoxMaxLe.Items.AddRange(new object[] { "EF Size", "223", "256", "512", "1024", "2048", "3072", "4096", "Max size" });
            comboBoxMaxLe.Location = new System.Drawing.Point(226, 23);
            comboBoxMaxLe.Name = "comboBoxMaxLe";
            comboBoxMaxLe.Size = new System.Drawing.Size(66, 21);
            comboBoxMaxLe.TabIndex = 3;
            // 
            // labelMaxLe
            // 
            labelMaxLe.AutoSize = true;
            labelMaxLe.Location = new System.Drawing.Point(184, 27);
            labelMaxLe.Name = "labelMaxLe";
            labelMaxLe.Size = new System.Drawing.Size(41, 13);
            labelMaxLe.TabIndex = 2;
            labelMaxLe.Text = "Max Le";
            // 
            // comboBoxApduType
            // 
            comboBoxApduType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxApduType.FormattingEnabled = true;
            comboBoxApduType.Items.AddRange(new object[] { "Automatic", "Short", "Extended" });
            comboBoxApduType.Location = new System.Drawing.Point(91, 23);
            comboBoxApduType.Name = "comboBoxApduType";
            comboBoxApduType.Size = new System.Drawing.Size(78, 21);
            comboBoxApduType.TabIndex = 1;
            // 
            // label9
            // 
            label9.Location = new System.Drawing.Point(7, 27);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(83, 13);
            label9.TabIndex = 0;
            label9.Text = " APDU Selection";
            // 
            // FormOptions
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(809, 694);
            Controls.Add(groupBox5);
            Controls.Add(groupBox4);
            Controls.Add(groupBoxCertPA);
            Controls.Add(groupBox3);
            Controls.Add(groupBoxCertTA);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOK);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(groupBox6);
            DoubleBuffered = true;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "FormOptions";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Options";
            Load += FormOptions_Load;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            tabControlDG.ResumeLayout(false);
            tabPageBAC.ResumeLayout(false);
            tabPageBAP.ResumeLayout(false);
            groupBoxCertTA.ResumeLayout(false);
            groupBoxCertTA.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBoxCertPA.ResumeLayout(false);
            groupBoxCertPA.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ColumnHeader ColumnSelect;
        private System.Windows.Forms.ColumnHeader ColumnDatagroup;
        private System.Windows.Forms.ColumnHeader ColumnContent;
        private System.Windows.Forms.ColumnHeader ColumnSupported;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        public System.Windows.Forms.CheckBox checkBoxPA;
        public System.Windows.Forms.CheckBox checkBoxAA;
        public System.Windows.Forms.CheckBox checkBoxCA;
        public System.Windows.Forms.CheckBox checkBoxTA;
        private System.Windows.Forms.Button buttonBrowseDV;
        private System.Windows.Forms.Button buttonBrowseIS;
        private System.Windows.Forms.Button buttonBrowseISPK;
        private System.Windows.Forms.GroupBox groupBoxCertTA;
        private System.Windows.Forms.Label labelIsPrivateKey;
        private System.Windows.Forms.Label labelIsCert;
        private System.Windows.Forms.Label labelDvCert;
        private System.Windows.Forms.OpenFileDialog openFileDialogCert;
        private System.Windows.Forms.OpenFileDialog openFileDialogISPK;
        public System.Windows.Forms.TextBox textBoxDV;
        public System.Windows.Forms.TextBox textBoxISPK;
        public System.Windows.Forms.TextBox textBoxIS;
        public System.Windows.Forms.CheckBox checkBoxEnableLog;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonOpenCSV;
        public System.Windows.Forms.ListView listView1;
        public System.Windows.Forms.CheckBox checkBoxCSV;
        private System.Windows.Forms.GroupBox groupBoxCertPA;
        private System.Windows.Forms.Label labelExtDSCert;
        public System.Windows.Forms.TextBox textBoxExternalDS;
        private System.Windows.Forms.Label labelCSCACert;
        public System.Windows.Forms.TextBox textBoxCSCA;
        private System.Windows.Forms.Button buttonBrowseExternalDS;
        private System.Windows.Forms.Button buttonBrowseCSCA;
        private System.Windows.Forms.OpenFileDialog openFileDialogPA;
        public System.Windows.Forms.CheckBox checkBoxManualMRZ;
        private System.Windows.Forms.TabControl tabControlDG;
        private System.Windows.Forms.TabPage tabPageBAC;
        private System.Windows.Forms.TabPage tabPageBAP;
        public System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label labelLogPath;
        public System.Windows.Forms.CheckBox checkBoxAutoDetect;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.RadioButton rbAccessControlPACE;
        public System.Windows.Forms.RadioButton rbAccessControlBAC;
        public System.Windows.Forms.RadioButton rbAccessControlAuto;
        public System.Windows.Forms.RadioButton rbPasswordTypeAskPwd;
        public System.Windows.Forms.RadioButton rbPasswordTypeMRZ;
        public System.Windows.Forms.RadioButton rbPasswordTypeCAN;
        public System.Windows.Forms.TextBox textBoxDefaultCAN;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label9;
        public System.Windows.Forms.ComboBox comboBoxApduType;
        public System.Windows.Forms.CheckBox checkBoxFindAntenna;
        public System.Windows.Forms.CheckBox checkboxEmrtdTest;
        private System.Windows.Forms.Label labelCvcaLinkCert;
        public System.Windows.Forms.TextBox textBoxCVCALink;
        private System.Windows.Forms.Button buttonBrowseCVCALink;
        private System.Windows.Forms.ComboBox comboBoxMaxLe;
        private System.Windows.Forms.Label labelMaxLe;
    }
}
