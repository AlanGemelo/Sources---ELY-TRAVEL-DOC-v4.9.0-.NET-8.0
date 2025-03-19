using ElyMRTDDotNet;
using ElySCardDotNet;
using FreeImageAPI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wsq2Bmp;
using static ElyMRTDDotNet.ElyMRTDDotNet;

namespace ELY_TRAVEL_DOC
{
    public partial class MainForm
    {
        #region Variables
        private const float fDpiStandard = 96.0F;

        private int nImageIndicator = 0;
        private ToolTip toolTip;

        private delegate void _WaitForCardAndReadDocumentDelegate(bool lastCardState);
        private delegate void _ClearDelegate();
        private delegate void _PrintListViewDelegate(String msg, MainForm.Status status);
        private delegate void _UpdatePictureBoxDelegate(PictureBox pBox, bool value);
        private delegate void _UpdatePictureBoxesDg(PictureBox box, int res);
        private delegate void _PrintTextBoxMrzDelegate(String msg);
        #endregion


        #region GUI helpers
        private void InitializeGuiHelpers()
        {
            // Clear labels displaying the device versions
            ClearDeviceVersionLabels();

            // Register device arrival & removal notifications for VCOM and CCID devices
            UsbNotification.RegisterUsbVcomDeviceNotification(this.Handle);
            UsbNotification.RegisterUsbCcidDeviceNotification(this.Handle);

            // Setup font for MRZ display and log view
            SetEmbeddedFontToControl(this.textBoxMrz, Properties.Resources.OCRB, 13.0F);
            AdaptControlsForHighDpi();

            // Initialize tooltip
            toolTip = new ToolTip();
            toolTip.ShowAlways = true;
        }

        void AdaptControlsForHighDpi()
        {
            // After enabling the DpiAwareness in the app.config and app.manifest, the controls have adapted to high DPI automatically.
            // Additionally, it expects the AutoScaleMode of the form set either to Font or DPI based on the contents of the form.
            // And needs the AutoSize as false, and the font as Microsoft Sans Serif with 8points of the respective forms.
            // However, there could be certain controls behaving differently at high DPIs.
            // One such control determined is the column width of ListView.
            // The standard DPI at which the GUIs were designed is 96.0F
            // Hence let us scale it manually to the current device DPI.
            listViewLogs.Columns[0].Width = (int)((listViewLogs.Width / fDpiStandard) * (float)listViewLogs.DeviceDpi);
        }

        private void DeInitialzeGuiHelpers()
        {
            // Unregister device arrival & removal notifications for VCOM and CCID devices
            UsbNotification.UnregisterUsbVcomDeviceNotification();
            UsbNotification.UnregisterUsbCcidDeviceNotification();

            // Hide reading chip form, if opened
            HideFormReadingChip();
        }

        // Native DLLs are stored in "runtimes/{rid}/native/" path structure.
        private string getNativeLibraryPath()
        {
            string rid = Environment.Is64BitProcess ? "win-x64" : "win-x86";

            // Determine the runtimes path
            return Path.Combine("runtimes", rid, "native");
        }

        private void ShowAppVersion()
        {
            System.Reflection.AssemblyName aName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            this.Text = "Grupo Santoro v" + aName.Version.Major.ToString(CultureInfo.CurrentCulture) +
                "." + aName.Version.Minor.ToString(CultureInfo.CurrentCulture) +
                "." + aName.Version.Build.ToString(CultureInfo.CurrentCulture);
            if (aName.Version.Revision != 0)
                this.Text += " RC" + aName.Version.Revision.ToString(CultureInfo.CurrentCulture);

            FileVersionInfo nativeDllVerInfo = FileVersionInfo.GetVersionInfo(Path.Join(getNativeLibraryPath(), "ElyMRTD.dll"));
            FileVersionInfo dotNetDllVerInfo = FileVersionInfo.GetVersionInfo("ElyMRTDdotNet.dll");
            if (!String.IsNullOrEmpty(nativeDllVerInfo.FileVersion) && !String.IsNullOrEmpty(dotNetDllVerInfo.FileVersion))
                labelElyMrtdDllVersion.Text = "DLL ElyMRTD " + nativeDllVerInfo.FileVersion + " (.NET " + dotNetDllVerInfo.FileVersion + ")";
        }

        private void ClearFormGuiControls()
        {
            if (this.InvokeRequired)
            {
                // we were called on a worker thread
                // marshal the call to the user interface thread
                this.Invoke(new _ClearDelegate(ClearFormGuiControls), new object[] { });
                return;
            }
            this.buttonLastLog.Enabled = false;
            if (this.groupBoxReadingState.Controls.Count > 0)
                foreach (Control c in this.groupBoxReadingState.Controls[0].Controls)
                    if (c.GetType().FullName == "System.Windows.Forms.PictureBox")
                        ((PictureBox)c).Image = new Bitmap(c.Width, c.Height);
            if (this.groupBoxAccessControl.Controls.Count > 0)
                foreach (Control c in this.groupBoxAccessControl.Controls[0].Controls)
                    if (c.GetType().FullName == "System.Windows.Forms.PictureBox")
                        ((PictureBox)c).Image = new Bitmap(c.Width, c.Height);
            if (this.groupBoxAccessControlIDL.Controls.Count > 0)
                foreach (Control c in this.groupBoxAccessControlIDL.Controls[0].Controls)
                    if (c.GetType().FullName == "System.Windows.Forms.PictureBox")
                        ((PictureBox)c).Image = new Bitmap(c.Width, c.Height);
            if (this.groupBoxAntenna.Controls.Count >= 1)
                foreach (Control c in this.groupBoxAntenna.Controls[1].Controls)
                    if (c.GetType().FullName == "System.Windows.Forms.PictureBox")
                        ((PictureBox)c).Image = new Bitmap(c.Width, c.Height);
            if (this.groupBoxPersonalData.Controls.Count > 0)
                foreach (Control c in this.groupBoxPersonalData.Controls[0].Controls)
                    if (c.GetType().FullName == "System.Windows.Forms.TextBox")
                        ((TextBox)c).Text = "";
            if (this.groupBoxPersonalDataIDL.Controls.Count > 0)
                foreach (Control c in this.groupBoxPersonalDataIDL.Controls[0].Controls)
                {
                    if (c.GetType().FullName == "System.Windows.Forms.TextBox")
                        ((TextBox)c).Text = "";
                    if (c.GetType().FullName == "System.Windows.Forms.TreeView")
                        ((TreeView)c).Nodes.Clear();
                }
            if (this.tableLayoutPanelPA.Controls.Count > 0)
                foreach (Control c in this.tableLayoutPanelPA.Controls)
                    if (c.GetType().FullName == "System.Windows.Forms.PictureBox")
                        UpdatePictureBoxesDg((PictureBox)c, 10);
            if (this.tableLayoutPanelPA_BAP.Controls.Count > 0)
                foreach (Control c in this.tableLayoutPanelPA_BAP.Controls)
                    if (c.GetType().FullName == "System.Windows.Forms.PictureBox")
                        UpdatePictureBoxesDg((PictureBox)c, 10);
            this.pictureBoxPicture.Image = new Bitmap(this.pictureBoxPicture.Width, this.pictureBoxPicture.Height);
            this.pictureBoxPictureIdl.Image = new Bitmap(this.pictureBoxPicture.Width, this.pictureBoxPicture.Height);
            this.listViewLogs.Items.Clear();
            this.textBoxMrz.Clear();
            this.nImageIndicator = 0;
            if (bmpPortrait != null) bmpPortrait.Dispose();
            if (bmpSignature != null) bmpSignature.Dispose();
            if (this.lbmpBiometricTemplates != null)
                this.lbmpBiometricTemplates.Clear();
            if (this.lbmpIrisBiometricTemplates != null)
                this.lbmpIrisBiometricTemplates.Clear();
        }

        private void ClearDeviceVersionLabels()
        {
            labelScannerFwVersion.Text = "";
            labelScannerNnaVersion.Text = "";
            labelIdBoxSerialNumber.Text = "";
            labelIdBoxProductNumber.Text = "";
            labelContactlessFwVersion.Text = "";
            labelContactlessDriver.Text = "";
        }

        private void ClearListViewLogs()
        {
            this.listViewLogs.Items.Clear();
        }

        private void ClearGroupBoxAntenna()
        {
            if (this.groupBoxAntenna.Controls.Count >= 1)
                foreach (Control c in this.groupBoxAntenna.Controls[1].Controls)
                    if (c.GetType().FullName == "System.Windows.Forms.PictureBox")
                        ((PictureBox)c).Image = new Bitmap(c.Width, c.Height);
        }

        private void DisplayMrzLinesInTextBox()
        {
            String szMrz = "";
            szMrz = elyMrzParser.Line1();
            if (elyMrzParser.Line2() != null)
                szMrz += ("\r\n" + elyMrzParser.Line2());
            if (elyMrzParser.Line3() != null && elyMrzParser.Line3().Length > 5)
                szMrz += ("\r\n" + elyMrzParser.Line3());
            PrintTextBoxMrz(szMrz.ToString());
        }

        private void AddMrzLinesToListView()
        {
            PrintListView("MRZ:");
            PrintListView(elyMrzParser.Line1());
            if (elyMrzParser.Line2().Length > 5)
                PrintListView(elyMrzParser.Line2());
            if (elyMrzParser.Line3().Length > 5)
                PrintListView(elyMrzParser.Line3());
            PrintListView("");
        }

        private void AddTimeInfoToListView(string szPrefix, int nDgLength, long lElapsedMillis)
        {
            float fSpeed = (float)(nDgLength * 8 / (lElapsedMillis / (float)1000) / 1000);
            String szMsg = szPrefix + " Read ("
                                    + "Speed: " + fSpeed.ToString("0.00") + " kbps, "
                                    + "Size: " + nDgLength + " bytes, "
                                    + "Time: " + lElapsedMillis + " ms).";

            PrintListView(szMsg);
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);
        private PrivateFontCollection fonts = new PrivateFontCollection();
        private void SetEmbeddedFontToControl(Control control, byte[] fontData, float fSize)
        {
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;
            fonts.AddMemoryFont(fontPtr, fontData.Length);
            AddFontMemResourceEx(fontPtr, (uint)fontData.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
            Font font = new Font(fonts.Families[0], fSize);
            control.Font = font;
        }
        #endregion


        #region Delegates MainForm
        public enum Status { Normal = 0, Success = 1, Error = 2, Warning = 3};
        public void PrintListView(String msg, MainForm.Status status = Status.Normal)
        {
            if (this.InvokeRequired)
            {
                // we were called on a worker thread
                // marshal the call to the user interface thread
                this.Invoke(new _PrintListViewDelegate(PrintListView), new object[] { msg, status });
                return;
            }
            this.listViewLogs.Items.Add(msg);
            switch (status)
            {
                case Status.Error:
                    this.listViewLogs.Items[this.listViewLogs.Items.Count - 1].ForeColor = System.Drawing.Color.Red;
                    this.listViewLogs.Items[this.listViewLogs.Items.Count - 1].Font = new Font(this.listViewLogs.Items[this.listViewLogs.Items.Count - 1].Font, FontStyle.Bold | FontStyle.Italic);
                    break;
                case Status.Success:
                    this.listViewLogs.Items[this.listViewLogs.Items.Count - 1].ForeColor = System.Drawing.Color.Green;
                    this.listViewLogs.Items[this.listViewLogs.Items.Count - 1].Font = new Font(this.listViewLogs.Items[this.listViewLogs.Items.Count - 1].Font, FontStyle.Bold | FontStyle.Italic);
                    break;
                case Status.Warning:
                    this.listViewLogs.Items[this.listViewLogs.Items.Count - 1].ForeColor = System.Drawing.Color.DarkGray;
                    this.listViewLogs.Items[this.listViewLogs.Items.Count - 1].Font = new Font(this.listViewLogs.Items[this.listViewLogs.Items.Count - 1].Font, FontStyle.Bold | FontStyle.Italic);
                    break;
                default:
                    this.listViewLogs.Items[this.listViewLogs.Items.Count - 1].Font = new Font(this.listViewLogs.Items[this.listViewLogs.Items.Count - 1].Font, FontStyle.Italic);
                    break;
            }
            this.listViewLogs.Items[this.listViewLogs.Items.Count - 1].EnsureVisible();
        }
        public void PrintListViewError(String msg)
        {
            PrintListView(msg, Status.Error);
        }
        public void PrintListViewSuccess(String msg)
        {
            PrintListView(msg, Status.Success);
        }
        public void PrintListViewWarning(String msg)
        {
            PrintListView(msg, Status.Warning);
        }
        public void PrintTextBoxMrz(String msg)
        {
            if (this.InvokeRequired)
            {
                // we were called on a worker thread
                // marshal the call to the user interface thread
                this.Invoke(new _PrintTextBoxMrzDelegate(PrintTextBoxMrz), new object[] { msg });
                return;
            }
            this.textBoxMrz.Clear();
            this.textBoxMrz.Text = msg;
        }

        public void UpdatePictureBox(PictureBox pBox, bool value)
        {
            if (this.InvokeRequired)
            {
                // we were called on a worker thread
                // marshal the call to the user interface thread
                this.Invoke(new _UpdatePictureBoxDelegate(UpdatePictureBox), new object[] { pBox, value });
                return;
            }
            pBox.Image = value ? ELY_TRAVEL_DOC.Properties.Resources.success : ELY_TRAVEL_DOC.Properties.Resources.fail;
        }

        public void UpdatePictureBoxesDg(PictureBox box, int res)
        {
            if (this.InvokeRequired)
            {
                // we were called on a worker thread
                // marshal the call to the user interface thread
                this.Invoke(new _UpdatePictureBoxesDg(UpdatePictureBoxesDg), new object[] { box, res });
                return;
            }
            switch (res)
            {
                case (int)DG_RETURN_CODES.DG_FILE_NOT_FOUND: // -2
                    box.BackColor = System.Drawing.Color.LightGray;
                    break;
                case (int)DG_RETURN_CODES.DG_READ_ERROR: // -1
                    box.BackColor = System.Drawing.Color.Red;
                    break;
                case (int)DG_RETURN_CODES.HASH_VERIFIED: // 0
                    box.BackColor = System.Drawing.Color.Green;
                    break;
                case (int)DG_RETURN_CODES.HASH_INCORRECT: // 1
                    box.BackColor = System.Drawing.Color.Orange;
                    break;
                case (int)DG_RETURN_CODES.HASH_NOT_VERIFIED: // 2
                    box.BackColor = System.Drawing.Color.Yellow;
                    break;
                default:
                    box.BackColor = System.Drawing.Color.White;
                    break;
            }
        }
        #endregion


        #region Messagebox helpers
        private DialogResult ShowMessageBoxToConnectIdBox()
        {
            return MessageBox.Show("Please connect the ID BOX Reader", "ELY TRAVEL DOC",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private DialogResult ShowMessageBoxToPlaceDocument()
        {
            if (IsEppOnIdBox5xxOr6xx(m_szMrz))
            {
                return ShowMessageBoxToPlaceEppOnIdBox5xxOr6xx();
            }
            else
            {
                return MessageBox.Show("Place the document on reader and continue.", "Alert",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private DialogResult ShowMessageBoxToPlaceEppOnIdBox5xxOr6xx()
        {
            return MessageBox.Show("In order to read ePP chip from this device, please close the ePP, place it on reader and continue.", "Alert",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private DialogResult ShowMessageBoxComPortUnavailable()
        {
            return MessageBox.Show("The COM port of the selected device is either incorrect or not available.",
                "COM port not available.", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private DialogResult ShowMessageBoxReaderConnectionError(String szValue)
        {
            return MessageBox.Show("Problem connecting the reader: " + szValue, "ELY TRAVEL DOC",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private DialogResult ShowMessageBoxToUseShortApdu()
        {
            return MessageBox.Show("We could not read the document successfully. This might be " +
                        "related to the \"Automatic\" or \"Extended\" APDU configuration.\n\n" +
                        "Try changing it to \"Short\" under Options and recheck.", "Alert",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private DialogResult ShowMessageBoxHtmlFileUnavailable()
        {
            return MessageBox.Show("The file does not exist yet.\nPlease read a travel document first !", "No html file generated",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion


        #region Tooltip helpers
        private void ShowToolTipClear()
        {
            toolTip.RemoveAll();
            toolTip.Hide(this);
        }

        private void ShowToolTipToClickToRead(Control control)
        {
            toolTip.RemoveAll();
            toolTip.Show("Click to read the document.", control, control.Width, control.Height);
        }

        private void ShowToolTipToDisableAutoDetect(Control control)
        {
            toolTip.RemoveAll();
            toolTip.Show("Auto detect mode is active. Disable it in Options to read manually.", control, control.Width, control.Height);
        }

        private void ShowToolTipToClickToFindAntenna(Control control)
        {
            toolTip.RemoveAll();
            toolTip.SetToolTip(control, "Click to find the antenna to which the document is detected.");
        }
        #endregion


        #region MRZ helpers
        private void ValidateMrzWithDG1()
        {
            if ((formOptions.rbAccessControlBAC.Checked ||
                formOptions.rbPasswordTypeAskPwd.Checked ||
                formOptions.rbPasswordTypeMRZ.Checked) &&
                IsAppletTypeIcao())
            {
                if ((m_szMrz != null) && (!m_szMrz.Equals("")))
                {
                    if (elyMrtd.validateMrzWithDG1(m_szMrz))
                        PrintListView("Scanner MRZ & DG1 MRZ match");
                    else
                        PrintListViewWarning("Scanner MRZ & DG1 MRZ does not match");
                }
                else
                {
                    Console.WriteLine("m_szMrz is null or Empty");
                }
            }
        }

        private Boolean IsEppOnIdBox5xxOr6xx (String szMrz)
        {
            if ((szMrz != null) && (szMrz.Length != 0) && (szMrz[0] == 'P'))
            {
                String[] mrzLines = szMrz.Replace("\r\n", "\r").Split('\r');
                return (
                         (mrzLines[0].Length >= 28) && (mrzLines[1].Length >= 28) && (mrzLines[2].Length == 0)
                         &&
                         (
                           ((mrzLines[0].Length != 44) && (mrzLines[1].Length != 44))
                           || (mrzLines[0].EndsWith("__________") && mrzLines[1].EndsWith("__________"))
                         )
                       );
            }
            return false;
        }

        private String PreProcessMrz(String szMrz)
        {
            // ID BOX 5xx and 6xx are 4 camera based products specifically meant for reading ID1 documents.
            // Because of that, they could read a maximum of 31 characters per line only.
            // However, if we need to read an ePP with it (which contains 44 characters per line), we need the following workaround to allow parsing the MRZ.
            // For 2-Line MRZs starting with 'P', if each line contains atleast 28 characters but not 44 characters, then append remaining characters with '<'.
            if (IsEppOnIdBox5xxOr6xx(szMrz))
            {
                String[] mrzLines = szMrz.Replace("\r\n", "\r").Split('\r');
                String strFillers = "________________";
                szMrz = mrzLines[0] + strFillers.Substring(0, (44 - mrzLines[0].Length)) + "\r\n" +
                        mrzLines[1] + strFillers.Substring(0, (44 - mrzLines[0].Length)) + "\r\n" +
                        "\r\n";
            }
            return szMrz;
        }

        private void DisplayMrzFieldsFromParser()
        {
            if (szMrzPwd != null)
            {
                this.textBoxName.Text = elyMrzParser.FirstName();
                this.textBoxSurname.Text = elyMrzParser.LastName();
                this.textBoxBirthDate.Text = elyMrzParser.DateToString(elyMrzParser.DateOfBirth());
                this.textBoxNationality.Text = elyMrzParser.NationalityName();
                this.textBoxSex.Text = elyMrzParser.Gender();
                this.textBoxExpiryDate.Text = elyMrzParser.isNIDMRZDetected() ?
                    "NA" : elyMrzParser.DateToString(elyMrzParser.ExpiryDate());
                this.textBoxDocumentNumber.Text = elyMrzParser.DocumentNumber();
                this.textBoxDocumentType.Text = elyMrzParser.DocumentTypeDescription();
                this.textBoxIssuer.Text = elyMrzParser.IssuingCountryName();
                this.textBoxOptionalData.Text = ((elyMrzParser.OptionalData() != null) && (elyMrzParser.OptionalData().Count > 0)) ?
                    elyMrzParser.OptionalData()[0] : "NA";
            }
        }

        private void DisplayMrzFieldsFromDg1()
        {
            if (IsAppletTypeIcao())
            {
                if (dg1 != null && dg1.Count == 10)
                {
                    this.textBoxName.Text = dg1[0];
                    this.textBoxSurname.Text = dg1[1];
                    this.textBoxBirthDate.Text = dg1[2];
                    this.textBoxNationality.Text = dg1[3];
                    this.textBoxSex.Text = dg1[4];
                    this.textBoxExpiryDate.Text = dg1[5];
                    this.textBoxDocumentNumber.Text = dg1[6];
                    this.textBoxDocumentType.Text = dg1[7];
                    this.textBoxIssuer.Text = dg1[8];
                    this.textBoxOptionalData.Text = dg1[9];
                }
                else
                    Console.WriteLine("ERROR: Unknown DG1 count for ICAO: %d", dg1.Count);
            }
            else if (IsAppletTypeIdl())
            {
                if (dg1 != null && dg1.Count == 8)
                {
                    this.textBoxFamilyName.Text = dg1[0];
                    this.textBoxGivenNames.Text = dg1[1];
                    this.textBoxBirthDateIDL.Text = dg1[2];
                    this.textBoxExpiryDateIDL.Text = dg1[3].Substring(0, 10);
                    this.textBoxIssuingDate.Text = dg1[4];
                    this.textBoxIssuingAuthority.Text = dg1[5];
                    this.textBoxIssuingCountry.Text = dg1[6];
                    this.textBoxLicenceNumber.Text = dg1[7];
                }
                else
                    Console.WriteLine("ERROR: Unknown DG1 count for IDL: %d", dg1.Count);
            }
            else
                Console.WriteLine("ERROR: Unknown applet: %d", nAppletType);
        }

        private List<string> GatherDg1FieldsFromMrtdObject()
        {
            List<string> dg1 = new List<string>();
            if (IsAppletTypeIcao())
            {
                dg1.Add(elyMrtd.getName());
                dg1.Add(elyMrtd.getSurname());
                dg1.Add(elyMrtd.getBirthDate());
                dg1.Add(elyMrtd.getCountryName(elyMrtd.getNationality()));
                dg1.Add(elyMrtd.getSex());
                dg1.Add(elyMrtd.getValidityDate());
                dg1.Add(elyMrtd.getDocNum());
                dg1.Add(elyMrtd.getDocumentType());
                dg1.Add(elyMrtd.getCountryName(elyMrtd.getIssuingState()));
                dg1.Add(elyMrtd.getOptionalData());
            }
            else if (IsAppletTypeIdl())
            {
                dg1.Add(elyMrtd.getFamilyName());
                dg1.Add(elyMrtd.getGivenNames());
                dg1.Add(elyMrtd.getBirthDate());
                dg1.Add(elyMrtd.getExpiryDate());
                dg1.Add(elyMrtd.getIssuingDate());
                dg1.Add(elyMrtd.getIssuingAuthority());
                dg1.Add(elyMrtd.getIssuingCountry());
                dg1.Add(elyMrtd.getLicenceNumber());
                dg1Categories = new List<ElyMRTDDotNet.VehicleCategory>(elyMrtd.getCategories());
            }
            else
                Console.WriteLine("ERROR: Unknown applet: %d", nAppletType);

            return dg1;
        }

        private string GetCsvEntry(string szVal)
        {
            Regex rgNum = new Regex("[0-9]");
            Regex rgAlpha = new Regex("[A-Z_<]");
            if (string.IsNullOrEmpty(szVal))
                return ("" + ',');
            if (rgNum.IsMatch(szVal) && !(rgAlpha.IsMatch(szVal)))
                return ("=\"" + szVal + "\"" + ",");
            else
                return (szVal + ",");
        }

        private void WriteCsv()
        {
            // Enable the following to test parsed output of MRZ Lines
#if false
            bool isParseSuccess = elyMrzParser.IsParseSuccess();
            Console.WriteLine(elyMrzParser.ValidationMessage());
            Console.WriteLine(elyMrzParser.Line1());
            Console.WriteLine(elyMrzParser.Line2());
            Console.WriteLine(elyMrzParser.Line3());
            bool isChecksumOfMrzValid = elyMrzParser.IsCheckDigitOfOverAllMrzValid();
            bool isChecksumOfDocNum = elyMrzParser.IsCheckDigitOfDocNumValid();
            bool isChecksumOfDob = elyMrzParser.IsCheckDigitOfDobValid();
            bool isChecksumOfDoe = elyMrzParser.IsCheckDigitOfDoeValid();
            bool isChecksumOfOptionalData = elyMrzParser.IsCheckDigitOfOptionalDataValid();
            Console.WriteLine(elyMrzParser.GetMrzPwd());
            Console.WriteLine(elyMrzParser.DocumentType());
            Console.WriteLine(elyMrzParser.DocumentTypeDescription());
            Console.WriteLine(elyMrzParser.AdditionalDocumentType());
            Console.WriteLine(elyMrzParser.IssuingCountryIso());
            Console.WriteLine(elyMrzParser.IssuingCountryName());
            Console.WriteLine(elyMrzParser.FirstName());
            Console.WriteLine(elyMrzParser.LastName());
            Console.WriteLine(elyMrzParser.FullName());
            Console.WriteLine(elyMrzParser.DocumentNumber());
            Console.WriteLine(elyMrzParser.DocumentNumberCheckDigit());
            Console.WriteLine(elyMrzParser.NationalityIso());
            Console.WriteLine(elyMrzParser.NationalityName());
            Console.WriteLine(elyMrzParser.DateToString(elyMrzParser.DateOfBirth()));
            Console.WriteLine(elyMrzParser.DateOfBirthCheckDigit());
            Console.WriteLine(elyMrzParser.Age());
            Console.WriteLine(elyMrzParser.Gender());
            Console.WriteLine(elyMrzParser.DateToString(elyMrzParser.ExpiryDate()));
            Console.WriteLine(elyMrzParser.ExpiryDateCheckDigit());
            Console.WriteLine(elyMrzParser.DateToString(elyMrzParser.IssueDate()));
            Console.WriteLine(elyMrzParser.IssuingAuthority());
            Console.WriteLine(elyMrzParser.PlaceOfBirth());
            for(int i =0; i< elyMrzParser.OptionalData().Count; i++)
            {
                Console.WriteLine((string)elyMrzParser.OptionalData().ElementAt(i));
            }
            Console.WriteLine(elyMrzParser.OptionalDataCheckDigit());
            Console.WriteLine(elyMrzParser.OverAllMrzCheckDigit());

            for (int i = 0; i < elyMrzParser.OptionalData2_TD1().Count; i++)
            {
                Console.WriteLine((string)elyMrzParser.OptionalData2_TD1().ElementAt(i));
            }
#endif
            //string path = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\") + 1);
            System.IO.StreamWriter file = null;
            string path = formOptions.GetLogPath();
            string csvFileName = "\\Extracted data.csv";

            try
            {
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                bool b = System.IO.File.Exists(path + csvFileName);
                file = new System.IO.StreamWriter(path + csvFileName, true);
                if (file == null)
                {
                    PrintListView("CSV file write failed.");
                    return;
                }
                if (!b)
                {
                    file.WriteLine(
                        "(MRZ)L1,(MRZ)L2,(MRZ)L3,"+
                        "(DG1)Name,(DG1)Surname,(DG1)Birth Date,(DG1)Nationality,(DG1)Gender,(DG1)Expiry Date,(DG1)Document Number,(DG1)Document Type,(DG1),(DG1)Issuing State,(DG1)Optional Data," +
                        "(DG11_5F0E)Full name,(DG11_5F10)Personal Number,(DG11_5F2B)Long Birth Date,(DG11_5F11)Birth Place,(DG11_5F42)Residence,(DG11_5F12)Phone Number," +
                        "(DG11_5F13)Profession,(DG11_5F14)Title,(DG11_5F15)Personal summary,(DG11_5F17)Other valid TD numbers,(DG11_5F18)Custody info," +
                        "(DG12_5F19)Issuing Authority,(DG12_5F26)Delivery Date,(DG12_5F1B)Endorsements and Observations," +
                        "(DG12_5F1C)Tax/Exit requirements,(DG12_5F55)Doc. perso. date & time,(DG12_5F56)SN of perso. system," +
                        "(MRZ)Name,(MRZ)Surname,(MRZ)Birth Date,(MRZ)Nationality,(MRZ)Gender,(MRZ)Expiry Date,(MRZ)Document Number,(MRZ)Document Type," +
                        "(MRZ)Issuing State,(MRZ)Optional Data,"+
                        "(DG1_DL)Family name,(DG1_DL)Given names,(DG1_DL)Birth Date,(DG1_DL)Expiry Date,(DG1_DL)Issuing Date,(DG1_DL)Issuing Authority,(DG1_DL)Issuing Country,(DG1_DL)License Number");
                }
                // (MRZ)L1,(MRZ)L2,(MRZ)L3,
                if (elyMrzParser.IsParseSuccess())
                {
                    // Write the MRZ from elyMrzParser
                    file.Write(GetCsvEntry(elyMrzParser.Line1()));
                    file.Write(GetCsvEntry(elyMrzParser.Line2()));
                    file.Write(GetCsvEntry(elyMrzParser.Line3()));
                }
                else
                {
                    // Write the MRZ from DG1
                    if (IsAppletTypeIcao())
                    {
                        file.Write(GetCsvEntry(elyMrtd.getMRZString()));
                    }
                    else
                    {
                        // For DL & DL_EU case , write empty string 
                        file.Write(GetCsvEntry(""));
                    }
                    file.Write(GetCsvEntry(""));
                    file.Write(GetCsvEntry(""));
                }

                if (IsAppletTypeIcao())
                {
                    // (DG1)Name,(DG1)Surname,(DG1)Birth Date,(DG1)Nationality,(DG1)Gender,(DG1)Expiry Date,(DG1)Document Number,(DG1)Document Type (DG1),(DG1)Issuing State,(DG1)Optional Data,
                    if (dg1.Count > 0)
                        foreach (string txt in dg1)
                            file.Write(GetCsvEntry(txt));
                    else
                        for (int i = 0; i < 10; i++)
                            file.Write(GetCsvEntry("DG1 not read"));

                    // (DG11_5F10)Personal Number,(DG11_5F2B)Long Birth Date,(DG11_5F11)Birth Place,(DG11_5F42)Residence,
                    // (DG11_5F12)Phone Number,(DG11_5F13)Profession,(DG11_5F14)Title,(DG11_5F15)Personal summary,
                    // (DG11_5F17)Other valid TD numbers,(DG11_5F18)Custody info,
                    if (dg11.Count > 0)
                        foreach (string txt in dg11)
                            file.Write(GetCsvEntry(txt));
                    else
                        for (int i = 0; i < 11; i++)
                            file.Write(GetCsvEntry("DG11 not read"));

                    // (DG12_5F19)Delivery Date,(DG12_5F26)Issuing Authority,(DG12_5F1B)Endorsements and Observations,
                    // (DG12_5F1C)Tax/Exit requirements,(DG12_5F55)Doc. perso. data & time,(DG12_5F56)SN of perso. system,
                    if (dg12.Count > 0)
                        foreach (string txt in dg12)
                            file.Write(GetCsvEntry(txt));
                    else
                        for (int i = 0; i < 6; i++)
                            file.Write(GetCsvEntry("DG12 not read"));

                    /*if (dg13.Count > 0)
                        foreach (string txt in dg13)
                            file.Write(GetCsvEntry(txt));
                    else
                        file.Write(",");//[TODO]*/
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                        file.Write(GetCsvEntry(""));
                    for (int i = 0; i < 11; i++)
                        file.Write(GetCsvEntry(""));
                    for (int i = 0; i < 6; i++)
                        file.Write(GetCsvEntry(""));
                }

                if (elyMrzParser.IsParseSuccess())
                {
                    if (IsAppletTypeIcao() || IsDocTypeDLWith3LMrz())
                    {
                        // Write the details from elyMrzParser
                        file.Write(GetCsvEntry(elyMrzParser.FirstName()));
                        file.Write(GetCsvEntry(elyMrzParser.LastName()));
                        file.Write(GetCsvEntry(elyMrzParser.DateToString(elyMrzParser.DateOfBirth())));
                        file.Write(GetCsvEntry(elyMrzParser.NationalityName()));
                        file.Write(GetCsvEntry(elyMrzParser.Gender()));
                        file.Write(GetCsvEntry(elyMrzParser.DateToString(elyMrzParser.ExpiryDate())));
                        file.Write(GetCsvEntry(elyMrzParser.DocumentNumber()));
                        file.Write(GetCsvEntry(elyMrzParser.DocumentType()));
                        file.Write(GetCsvEntry(elyMrzParser.IssuingCountryName()));
                        file.Write(((elyMrzParser.OptionalData() != null) && (elyMrzParser.OptionalData().Count > 0)) ?
                            GetCsvEntry(elyMrzParser.OptionalData()[0]) : GetCsvEntry(""));
                    }
                    else
                    {
                        file.Write(GetCsvEntry(""));
                        file.Write(GetCsvEntry(""));
                        file.Write(GetCsvEntry(""));
                        file.Write(GetCsvEntry(""));
                        file.Write(GetCsvEntry(""));
                        file.Write(GetCsvEntry(""));
                        file.Write(GetCsvEntry(""));
                        file.Write(GetCsvEntry(elyMrzParser.DocumentType()));
                        file.Write(GetCsvEntry(""));
                        file.Write(GetCsvEntry(""));
                    }
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                        file.Write(GetCsvEntry(""));
                }

                // Write the details from DG1 for IDL
                if (IsAppletTypeIcao())
                {
                    for (int i = 0; i < 8; i++)
                        file.Write(GetCsvEntry(""));
                }
                else
                {
                    // (DG1_DL)Family name,(DG1_DL)Given names,(DG1_DL)Birth Date,(DG1_DL)Expiry Date,(DG1_DL)Issuing Date,(DG1_DL)Issuing Authority,(DG1_DL)Issuing Country,(DG1_DL)License Number
                    if (dg1.Count > 0)
                        foreach (string txt in dg1)
                            file.Write(GetCsvEntry(txt));
                    else
                        for (int i = 0; i < 8; i++)
                            file.Write(GetCsvEntry("DG1 not read"));
                }
                PrintListView("CSV file written.");
            }
            catch (Exception exc)
            {
                PrintListViewError("Close CSV file before reading.\n" + exc.Message);
            }
            finally
            {
                file?.WriteLine();
                file?.Close();
            }

        }

        private String GetFolderPathToStoreImages()
        {
            if (formOptions.checkBoxCSV.Checked)
            {
                try
                {
                    string path = formOptions.GetLogPath();
                    path = Path.Combine(path, "DataBase");
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);

                    if (!string.IsNullOrEmpty(dg1[0]) && !string.IsNullOrEmpty(dg1[1]))
                    {
                        string folderName = (IsAppletTypeIcao()) ? (dg1[0] + " " + dg1[1]) : (dg1[1] + " " + dg1[0]);
                        folderName = folderName.Replace('/', '-').Replace('\\', '-');
                        path = Path.Combine(path, folderName);

                        if (!System.IO.Directory.Exists(path))
                            System.IO.Directory.CreateDirectory(path);
                    }
                    return path;
                }
                catch (Exception exc)
                {
                    PrintListViewError(exc.Message);
                }
            }
            return "";
        }

        private void SaveJpgImage (Bitmap bmp, string path, string fname)
        {
            if (formOptions.checkBoxCSV.Checked)
            {
                if (bmp != null && !string.IsNullOrEmpty(fname))
                {
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);

                    string fPath = Path.Combine(path, fname);
                    bmp.Save(fPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }

        private void SaveWsqImage(byte[] wsqData, string path, string fname)
        {
            if (formOptions.checkBoxCSV.Checked)
            {
                if (wsqData != null && (wsqData.Length > 0))
                {
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);

                    string fpath = Path.Combine(path, fname);
                    System.IO.File.WriteAllBytes(fpath, wsqData);
                }
            }
        }

        private bool IsDocTypeD()
        {
            // NOTE: If the option is set to ManualMRZ do not depend on elyMrzParser (as it
            // would either haven't parsed an MRZ for this reading or would contain the data
            // from one of the previous parsings)
            // 
            return (!formOptions.checkBoxManualMRZ.Checked
                && elyMrzParser.IsParseSuccess()
                && (elyMrzParser.DocumentType() != null)
                && elyMrzParser.DocumentType().Equals("D")
                && (elyMrzParser.Line1().Length > 0)
                && (elyMrzParser.Line2().Length == 0)
                && (elyMrzParser.Line3().Length == 0));
        }

        private bool IsDocTypeDLWith3LMrz()
        {
            // NOTE: If the option is set to ManualMRZ do not depend on elyMrzParser (as it
            // would either haven't parsed an MRZ for this reading or would contain the data
            // from one of the previous parsings)
            // 
            return (!formOptions.checkBoxManualMRZ.Checked
                && elyMrzParser.IsParseSuccess()
                && (elyMrzParser.DocumentType() != null)
                && elyMrzParser.DocumentType().Equals("D")
                && (elyMrzParser.Line1().Length > 0)
                && (elyMrzParser.Line2().Length > 0)
                && (elyMrzParser.Line3().Length > 0));
        }
        #endregion


        #region Form reading chip
        System.Windows.Forms.Timer autoFormCloseTimer;
        private void StartAutoFormCloseTimer()
        {
            HideFormReadingChip();
            if (autoFormCloseTimer == null)
            {
                autoFormCloseTimer = new System.Windows.Forms.Timer();
                autoFormCloseTimer.Interval = 15 * 1000;
                autoFormCloseTimer.Tick += delegate
                {
                    if (formReadingChip != null)
                    {
                        FormCollection fc = Application.OpenForms;
                        foreach (Form form in fc)
                        {
                            if (form.Text.Equals(formReadingChip.Text))
                            {
                                autoFormCloseTimer.Interval = 15 * 1000;
                                autoFormCloseTimer.Start();
                                //HideFormReadingChip();
                            }
                        }
                    }
                    return;
                };
                autoFormCloseTimer.Start();
            }
        }

        private void StopAutoFormCloseTimer()
        {
            if (autoFormCloseTimer != null)
            {
                if (autoFormCloseTimer.Enabled)
                {
                    autoFormCloseTimer.Stop();
                    autoFormCloseTimer.Dispose();
                }
                autoFormCloseTimer = null;
            }
        }

        private void ShowFormReadingChip()
        {
            formReadingChip.ShowDialog();
            StartAutoFormCloseTimer();  // Auto close "Reading chip form"
        }

        private void HideFormReadingChip()
        {
            StopAutoFormCloseTimer();
            if (formReadingChip != null)
            {
                formReadingChip.Close();
            }
        }
        #endregion


        #region Form reading options helpers
        private bool IsReadingOptionsSetToManualMrzOrPaceCan()
        {
            return (formOptions.checkBoxManualMRZ.Checked ||
                (formOptions.rbAccessControlPACE.Checked && formOptions.rbPasswordTypeCAN.Checked) ||
                (formOptions.rbAccessControlAuto.Checked && formOptions.rbPasswordTypeCAN.Checked));
        }
        private bool IsReadingOptionsDoesNotRequireMrzScanner()
        {
            return IsReadingOptionsSetToManualMrzOrPaceCan();
        }
        #endregion


        #region Time helpers
        private String GetElapsedTimeAsString(TimeSpan ts)
        {
            return String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, (ts.Milliseconds / 10));
        }

        private long GetElapsedMillisAsLong(TimeSpan ts)
        {
            return ((ts.Hours * 3600 * 1000) + (ts.Minutes * 60 * 1000) + (ts.Seconds * 1000) + ts.Milliseconds);
        }
        #endregion


        #region Portrait helpers
        enum Navigate { Left = 0, Right = 1 };
        public int FindSequence(byte[] source, byte[] seq)
        {
            var start = -1;
            for (var i = 0; i < source.Length - seq.Length + 1 && start == -1; i++)
            {
                var j = 0;
                for (; j < seq.Length && source[i + j] == seq[j]; j++) { }
                if (j == seq.Length) start = i;
            }
            return start;
        }

        private int GetPngFormatPos(byte[] photoArray)
        {
            // https://en.wikipedia.org/wiki/Portable_Network_Graphics
            byte[] pngFormat = { 0x89, 0x50, 0x4E, 0x47 };
            return FindSequence(photoArray, pngFormat);
        }
        private int GetJpegFifFormatPos(byte[] photoArray)
        {
            // https://en.wikipedia.org/wiki/JPEG_File_Interchange_Format
            byte[] jpegFifFormat = { 0xFF, 0xD8, 0xFF, 0xE0 };
            return FindSequence(photoArray, jpegFifFormat);
        }
        private int GetJp2FormatPos(byte[] photoArray)
        {
            // https://en.wikipedia.org/wiki/JPEG_2000
            byte[] jp2Format = { 0x00, 0x00, 0x00, 0x0C, 0x6A, 0x50, 0x20, 0x20, 0x0D, 0x0A };
            return FindSequence(photoArray, jp2Format);
        }
        private int GetJp2cFormatPos(byte[] photoArray)
        {
            // https://en.wikipedia.org/wiki/JPEG_2000
            byte[] jp2cFormat = { 0x6A, 0x70, 0x32, 0x63, 0xFF, 0x4F, 0xFF, 0x51 };
            return (FindSequence(photoArray, jp2cFormat) + 4);
        }
        private bool IsPhotoInPngFormat(byte[] photoArray) { return (GetPngFormatPos(photoArray) != -1); }
        private bool IsPhotoInJpegFifFormat(byte[] photoArray) { return (GetJpegFifFormatPos(photoArray) != -1); }
        private bool IsPhotoInJp2Format(byte[] photoArray) { return (GetJp2FormatPos(photoArray) != -1); }
        private bool IsPhotoInJp2cFormat(byte[] photoArray) { return (GetJp2cFormatPos(photoArray) != -1); }

        FreeImageBitmap GetFreeImageBitmapFromByteArray(byte[] photoArray)
        {
            MemoryStream ms = new MemoryStream(photoArray);
            FreeImageBitmap photo = null;

            if (IsPhotoInPngFormat(photoArray))
            {
                photo = FreeImageBitmap.FromStream(ms);
                photo.ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH.FICD_08_BPP);
                photo.Save(ms, FREE_IMAGE_FORMAT.FIF_PNG, FREE_IMAGE_SAVE_FLAGS.PNG_Z_BEST_COMPRESSION);
            }
            else
            {
                // Korean passports failed to get decoded while getting the 'bmpPortrait' from bitmap 'photo'.
                // They used JPEG_FIFF based images starting with FF D8 FF E0
                // For this, we determined that JPEG_FIFF formats shall also be handled like PNG case with FreeImageAPI
                if (IsPhotoInJpegFifFormat(photoArray))
                {
                    photo = FreeImageBitmap.FromStream(ms);
                    photo.ConvertColorDepth(FREE_IMAGE_COLOR_DEPTH.FICD_08_BPP);
                    photo.Save(ms, FREE_IMAGE_FORMAT.FIF_PNG, FREE_IMAGE_SAVE_FLAGS.PNG_Z_BEST_COMPRESSION);
                }
                // Other JPEG formats
                else
                {
                    bool bIsFindJp2cs = false;
                    do
                    {
                        try
                        {
                            if (bIsFindJp2cs && IsPhotoInJp2Format(photoArray))
                            {
                                // Check for JP2 codestream
                                int npos = GetJp2cFormatPos(photoArray);
                                if (npos != -1)
                                {
                                    byte[] photoArrayJp2c = new byte[(photoArray.Length - npos)];
                                    Array.Copy(photoArray, npos, photoArrayJp2c, 0, photoArrayJp2c.Length);
                                    if (ms != null)
                                        ms.Dispose();
                                    ms = new MemoryStream(photoArrayJp2c);
                                }
                            }
                            photo = new FreeImageBitmap(ms);
                        }
                        catch (Exception ex)
                        {
                            if (!bIsFindJp2cs)
                            {
                                bIsFindJp2cs = true;
                                continue;
                            }

                            Console.WriteLine("Error while decoding Picture: " + ex.Message);
                        }

                        break;

                    } while (true);
                }
            }

            if (ms != null)
                ms.Dispose();

            return photo;
        }

        private Bitmap GetBitmapFromByteArray(byte[] photoArray)
        {
            Bitmap bmpPortrait = null;
            FreeImageBitmap photo = GetFreeImageBitmapFromByteArray(photoArray);

            if (photo != null)
            {
                bmpPortrait = new Bitmap(photo.ToBitmap());
                photo.Dispose();
            }

            return bmpPortrait;
        }

        private void EnablePortraitNavigationButtons(bool bIsValue)
        {
            this.buttonImageLeft.Visible = bIsValue;
            this.buttonImageLeft.Enabled = bIsValue;
            this.buttonImageRight.Visible = bIsValue;
            this.buttonImageRight.Enabled = bIsValue;
        }

        private void EnablePortraitNavigationButtonsIdl(bool bIsValue)
        {
            this.buttonImageLeftIdl.Visible = bIsValue;
            this.buttonImageLeftIdl.Enabled = bIsValue;
            this.buttonImageRightIdl.Visible = bIsValue;
            this.buttonImageRightIdl.Enabled = bIsValue;
        }

        private void NavigatePortrait(Navigate navigate, PictureBox picBoxControl)
        {
            int nbOfImages = 0;
            int nbTemplateImages = 0;
            if (this.lbmpBiometricTemplates != null)
            {
                nbOfImages += this.lbmpBiometricTemplates.Count;
                nbTemplateImages += this.lbmpBiometricTemplates.Count;
            }
            if (this.lbmpIrisBiometricTemplates != null)
            {
                nbOfImages += this.lbmpIrisBiometricTemplates.Count;
                nbTemplateImages += this.lbmpIrisBiometricTemplates.Count;
            }
            if (bmpPortrait != null)
                nbOfImages += 1;
            if (bmpSignature != null)
                nbOfImages += 1;
            if (nbOfImages == 0)
                return;

            if (navigate == Navigate.Left)
                nImageIndicator = (nImageIndicator > 0) ? --nImageIndicator : (nbOfImages - 1);
            else
                nImageIndicator = (nImageIndicator < (nbOfImages - 1)) ? ++nImageIndicator : 0;

            if ((nImageIndicator == 0) && (bmpPortrait != null))
                picBoxControl.Image = (Bitmap)bmpPortrait.Clone();
            else if ((nImageIndicator <= 1) && (bmpSignature != null))
                picBoxControl.Image = (Bitmap)bmpSignature.Clone();
            else
            {
                int imageIndex = nImageIndicator - (nbOfImages - nbTemplateImages);
                if (imageIndex < lbmpBiometricTemplates.Count) {
                    if (this.lbmpBiometricTemplates[imageIndex] != null)
                        picBoxControl.Image = (Bitmap)this.lbmpBiometricTemplates[imageIndex].Clone();
                }
                else {
                    imageIndex -= this.lbmpBiometricTemplates.Count;
                    if (imageIndex < lbmpIrisBiometricTemplates.Count)
                    {
                        if (this.lbmpIrisBiometricTemplates[imageIndex] != null)
                            picBoxControl.Image = (Bitmap)this.lbmpIrisBiometricTemplates[imageIndex].Clone();
                    }
                }
            }
        }
        #endregion


        #region Extract biometric template
        public bool IsTemplateInWsqFormat(ElyMRTDDotNet.BiometricTemplate template)
        {
            return (2 == template.Data[0x1D]);
        }

        public void ExtractWsqImagesFromBiometricTemplate(ElyMRTDDotNet.BiometricTemplate template, List<Bitmap> listBmp, string name, int index, string path)
        {
            if (IsTemplateInWsqFormat(template))
            {
                WsqDecoder decoder = new WsqDecoder();
                int count = elyMrtd.getWsqImageCount(template);
                for (int j = 0; j < count; j++)
                {
                    try
                    {
                        byte[] wsqData = elyMrtd.getWsqImageDataAt(template, j);
                        if (wsqData == null || wsqData.Length == 0) { break; }
                        Bitmap bmp = decoder.Decode(wsqData);
                        if (bmp != null)
                        {
                            if (listBmp != null)
                                listBmp.Add(bmp);

                            if (formOptions.checkBoxCSV.Checked)
                            {
                                string imageName = name + "_" + index + "_" + j;

                                string fileName = imageName + ".wsq";
                                SaveWsqImage(wsqData, path, fileName);

                                fileName = imageName + ".jpg";
                                SaveJpgImage(bmp, path, fileName);
                            }
                        }
                    }
                    catch
                    {
                        throw new Exception("Error while decoding Picture");
                    }
                }
            }
        }

        public void ExtractImageFromBiometricTemplate(BiometricTemplate template, List<Bitmap> listBmp, string imageName, int index)
        {
            Bitmap bmp = null;
            byte[] tmp;
            int offset = 0x2E;
            string path = GetFolderPathToStoreImages();

            switch (template.Data[0x1D])
            {
                case 2: // WSQ
                    ExtractWsqImagesFromBiometricTemplate(template, listBmp, imageName, index, path);
                    return;

                case 3: // JPEG
                case 4: // JPEG2000
                case 5: // PNG
                    try
                    {
                        tmp = new byte[template.Data.Length - offset];
                        Array.Copy(template.Data, offset, tmp, 0, template.Data.Length - offset);
                        bmp = GetBitmapFromByteArray(tmp);
                        if (bmp != null)
                        {
                            if (listBmp != null)
                                listBmp.Add(bmp);

                            if (formOptions.checkBoxCSV.Checked)
                            {
                                string fileName = imageName + "_" + index + ".jpg";
                                SaveJpgImage(bmp, path, fileName);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception("Error while decoding Picture");
                    }
                    break;
            }
        }

        public void ExtractTemplateImagesAndDisplay(BiometricTemplate[] templates, List<Bitmap> listBmp, string imageName)
        {
            int index = 0;
            foreach (ElyMRTDDotNet.BiometricTemplate template in templates)
            {
                ExtractImageFromBiometricTemplate(template, listBmp, imageName, index++);
            }
        }
        #endregion


        #region Regex reader helpers
        private Regex REGEX_MULTISLOT = new Regex("CL [Rr]{1}eader [A-F0-9]{16} [0-9]{1}"); // Uses Proprietary CCID driver
        private Regex REGEX_IDREADER = new Regex("ELYCTIS CL [Rr]{1}eader [A-F0-9]{12} [0-9]{1}"); // Uses Microsoft CCID driver
        private Regex REGEX_MICROSOFT_CCID = new Regex("ELYCTIS CL [Rr]{1}eader [0-9]{1}"); // Uses Microsoft CCID driver

        private bool IsElyctisReader(string szReader)
        {
            return (REGEX_MULTISLOT.Match(szReader).Success
                    || REGEX_IDREADER.Match(szReader).Success
                    || REGEX_MICROSOFT_CCID.Match(szReader).Success);
        }

        private bool IsElyctisMultiSlotReader(string szReader)
        {
            return (REGEX_MULTISLOT.Match(szReader).Success);
        }

        private bool IsElyctisIdReader(string szReader)
        {
            return (REGEX_IDREADER.Match(szReader).Success
                    || REGEX_MICROSOFT_CCID.Match(szReader).Success);
        }
        #endregion


        #region Contactless reader version helpers
        ElySCardDotNet.SHARE ConnectToCardInDirectOrShared(string szReader)
        {
            if (m_iCard != null)
            {
                // Connect to card in either direct mode or shared mode
                // - Direct connection if no card in field.
                // - Shared connection if card in field.
                m_readerState[0].szReader = szReader;
                m_iResMan.GetStatusChange(10, m_readerState);
                if ((m_readerState[0].dwEventState & (uint)SCARD_STATE.SCARD_STATE_PRESENT) <= 0)
                {
                    m_iCard.Connect(SHARE.Direct, PROTOCOL.Undefined);
                    return SHARE.Direct;
                }
                else
                {
                    m_iCard.Connect(SHARE.Shared, PROTOCOL.T0orT1);
                    return SHARE.Shared;
                }
            }
            return 0;
        }

        byte[] MultislotTransmitReaderCommand(string szReader, byte[] abyCApdu)
        {
            byte[] abyRApdu = null;

            // Create connection to transact APDU
            m_iCard = m_iResMan.CreateConnection(szReader);
            if (m_iCard != null)
            {
                if (ConnectToCardInDirectOrShared(szReader) != 0)
                    abyRApdu = m_iCard.Transmit(abyCApdu, (uint)abyCApdu.Length);

                m_iCard.Disconnect(DISCONNECT.Leave);
                m_iCard = null;
            }

            return abyRApdu;
        }

        string GetContactlessFwMfrString(string szReader, bool bReaderTypeMultislot)
        {
            byte[] abyCApdu = new byte[] { 0xFF, 0x00, 0x00, 0x00, 0x03, 0x01, 0x00, 0x01, 0x00 };
            byte[] abyRApdu = bReaderTypeMultislot ?
                MultislotTransmitReaderCommand(szReader, abyCApdu) :
                ElychipsetUsb.getElyChipsetManufacturerStringRecord();
            if(abyRApdu == null || abyRApdu.Length < 256)
                return "";

            byte byZero = 0;
            int nStartIndex = 4;
            int nEndIndex = Array.IndexOf(abyRApdu, byZero, nStartIndex);
            int nCount = nEndIndex - nStartIndex;
            if ((nCount <= 0) || (nCount > 252))
                return "";
            return Encoding.ASCII.GetString(abyRApdu, nStartIndex, nCount).Replace('_', ' ');
        }
        #endregion


        #region COM port management
        /// <summary>
        /// Begins recursive registry enumeration
        /// </summary>
        /// <param name="portsToMap">array of port names (i.e. COM1, COM2, etc)</param>
        /// <returns>a hashtable mapping Friendly names to non-friendly port values</returns>
        private Dictionary<string, string> BuildPortNameHash(string[] portsToMap)
        {
            Dictionary<string, string> oReturnTable = new Dictionary<string, string>();
            MineRegistryForPortName("SYSTEM\\CurrentControlSet\\Enum", oReturnTable, portsToMap);
            return oReturnTable;
        }

        /// <summary>
        /// Recursively enumerates registry subkeys starting with startKeyPath looking for 
        /// "Device Parameters" subkey. If key is present, friendly port name is extracted.
        /// </summary>
        /// <param name="startKeyPath">the start key from which to begin the enumeration</param>
        /// <param name="targetMap">dictionary that will get populated with 
        /// nonfriendly-to-friendly port names</param>
        /// <param name="portsToMap">array of port names (i.e. COM1, COM2, etc)</param>
        private void MineRegistryForPortName(string startKeyPath, Dictionary<string, string> targetMap, string[] portsToMap)
        {
            if (targetMap.Count >= portsToMap.Length)
                return;
            using (RegistryKey currentKey = Registry.LocalMachine)
            {
                try
                {
                    using (RegistryKey currentSubKey = currentKey.OpenSubKey(startKeyPath))
                    {
                        string[] currentSubkeys = currentSubKey.GetSubKeyNames();
                        if (currentSubkeys.Contains("Device Parameters") && startKeyPath != "SYSTEM\\CurrentControlSet\\Enum")
                        {
                            object portName = Registry.GetValue("HKEY_LOCAL_MACHINE\\" + startKeyPath + "\\Device Parameters", "PortName", null);
                            if (portName == null || portsToMap.Contains(portName.ToString()) == false)
                                return;
                            object friendlyPortName = Registry.GetValue("HKEY_LOCAL_MACHINE\\" + startKeyPath, "FriendlyName", null);
                            string friendlyName = "N/A";
                            if (friendlyPortName != null)
                                friendlyName = friendlyPortName.ToString();
                            if (friendlyName.Contains(portName.ToString()) == false)
                                friendlyName = string.Format("{0} ({1})", friendlyName, portName);
                            targetMap[portName.ToString()] = friendlyName;
                        }
                        else
                        {
                            foreach (string strSubKey in currentSubkeys)
                                MineRegistryForPortName(startKeyPath + "\\" + strSubKey, targetMap, portsToMap);
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error accessing key '{0}'.. Skipping..", startKeyPath);
                }
            }
        }

        /// <summary>
        /// Check if the registry subkey associated with the given szVid & szPid has the COM port name as szFriendlyComPortName
        /// </summary>
        /// <param name="szFriendlyComPortName">COM port name</param>
        /// <param name="szVid"></param>
        /// <param name="szPid"></param>
        /// <returns>true, if matched</returns>
        /// https://www.codeproject.com/Tips/349002/Select-a-USB-Serial-Device-via-its-VID-PID
        /// 
        private bool IsVidPidMatchFound(String szFriendlyComPortName, String szVid, String szPid)
        {
            String pattern = String.Format("^VID_{0}.PID_{1}", szVid, szPid);
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> comports = new List<string>();
            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
            foreach (String s3 in rk2.GetSubKeyNames())
            {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (String s in rk3.GetSubKeyNames())
                {
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (String s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            if (szFriendlyComPortName.Equals((string)rk6.GetValue("PortName")))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool IsElyctisMrzScanner(KeyValuePair<string, string> kvp)
        {
            return kvp.Value.ToLower().Contains("ely") ||
                    IsVidPidMatchFound(kvp.Key, "2B78", "0005") ||
                    IsVidPidMatchFound(kvp.Key, "2B78", "0010");
        }
        #endregion


        #region Detect device removal and arrival
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == UsbNotification.WmDeviceChange)
            {
                int devType;
                switch ((int)m.WParam)
                {
                    case UsbNotification.DbtDeviceRemoveComplete:
                        devType = System.Runtime.InteropServices.Marshal.ReadInt32(m.LParam, 4);
                        if (devType == UsbNotification.DbtDevTypePort)
                            UsbDeviceRemovedVcom();
                        else if (devType == UsbNotification.DbtDevTypeDeviceInterface)
                            UsbDeviceRemovedCcid();
                        break;
                    case UsbNotification.DbtDeviceArrival:
                        devType = System.Runtime.InteropServices.Marshal.ReadInt32(m.LParam, 4);
                        if (devType == UsbNotification.DbtDevTypePort)
                            UsbDeviceAddedVcom();
                        else if (devType == UsbNotification.DbtDevTypeDeviceInterface)
                            UsbDeviceAddedCcid();
                        break;
                }
            }
        }

        private void Sleep(int millis)
        {
            this.Cursor = Cursors.WaitCursor;
            Thread.Sleep(millis);

            this.Enabled = true;
            this.Cursor = Cursors.Default;
        }

        private void UsbDeviceRemovedVcom()
        {
            Console.WriteLine("An USB (VCOM) device has been removed.");
            try
            {
                if (m_scanner != null && m_scanner.IsConnected())
                {
                    m_scanner.Disconnect();
                    m_scanner = null;
                }
                comboBoxComPort.SelectedIndex = -1;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error: %s" + exc.Message);
            }
        }

        private bool bIsUsbDeviceAddedVcomInProgress = false;

        private void UsbDeviceAddedVcom()
        {
            bIsUsbDeviceAddedVcomInProgress = true;
            Console.WriteLine("An USB (VCOM) device has been connected.");
            try
            {
                if (m_scanner != null && m_scanner.IsConnected())
                {
                    m_scanner.Disconnect();
                    m_scanner = null;
                }

                if (UpdateComboBoxComPort())
                {
                    m_scanner = new Scanner(m_delegateReadMrz);
                    Sleep(2000);
                    comboBoxComPort.SelectedIndex = m_nComPortIndex; // Auto-select to ID-BOX device
                }
                else
                {
                    ClearFormGuiControls();
                    comboBoxComPort.SelectedIndex = -1;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error: %s" + exc.Message);
            }
            bIsUsbDeviceAddedVcomInProgress = false;
        }

        private void UsbDeviceRemovedCcid()
        {
            Console.WriteLine("An USB (CCID) device has been removed.");
            try
            {
                Sleep(1000);
                UpdateComboBoxCcidReaders();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error: %s" + exc.Message);
            }
        }

        private void UsbDeviceAddedCcid()
        {
            Console.WriteLine("An USB (CCID) device has been connected.");
            try
            {
                if (!bIsUsbDeviceAddedVcomInProgress)
                { // Update SCARD details while VCOM is not updating the details of both COM & SCARD devices
                    Sleep(500);
                    UpdateComboBoxCcidReaders();
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error: %s" + exc.Message);
            }
        }
        #endregion


        #region Applet helpers
        private bool IsAppletTypeIcao()
        {
            int nAppletType = elyMrtd.getAppletType();
            return (nAppletType == (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_BAC
                   || nAppletType == (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_PACE);
        }
        private bool IsAppletTypeIdl()
        {
            int nAppletType = elyMrtd.getAppletType();
            return (nAppletType == (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL
                   || nAppletType == (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL_EU);
        }
        #endregion


        #region EF.COM helpers
        private bool IsDocumentAccessible()
        {
            bool bIsDocumentAccessible = false;
            try { bIsDocumentAccessible = GetInfosEfCom(); }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return bIsDocumentAccessible;
        }

        static readonly Dictionary<string, Dictionary<int, byte?>> DgTagLists = new Dictionary<string, Dictionary<int, byte?>>
        {
            { "ICAO", new Dictionary<int, byte?>
                {
                    { 1, /*DG1*/0x61 }, { 2, /*DG2*/0x75 }, { 3, /*DG3*/0x63 }, { 4, /*DG4*/0x76 },
                    { 5, /*DG5*/0x65 }, { 6, /*DG6*/0x66 }, { 7, /*DG7*/0x67 }, { 8, /*DG8*/0x68 },
                    { 9, /*DG9*/0x69 }, { 10, /*DG10*/0x6A }, { 11, /*DG11*/0x6B }, { 12, /*DG12*/0x6C },
                    { 13, /*DG13*/0x6D }, { 14, /*DG14*/0x6E }, { 15, /*DG15*/0x6F }, { 16, /*DG16*/0x70 }
                }
            },
            { "IDL", new Dictionary<int, byte?>
                {
                    { 1/*DG1*/, 0x61 }, { 2, /*DG2*/0x6B }, { 3, /*DG3*/0x6C }, { 4, /*DG4*/0x65 },
                    { 5/*DG5*/, 0x67 }, { 6, /*DG6*/0x75 }, { 7, /*DG7*/0x63 }, { 8, /*DG8*/0x76 },
                    { 9, /*DG9*/0x70 }, { 10, /*DG10 - No value as per specification*/null }, { 11, /*DG11*/0x6D },
                    { 12, /*DG12*/0x71 },{ 13, /*DG13*/0x6F }, { 14, /*DG14*/0x6E }
                }
            },
        };

        private bool IsDgPresentInEfCom(byte dgNumber)
        {
            if (dgList == null || dgList.Count == 0)
                return false;

            if (!IsAppletTypeIcao() && !IsAppletTypeIdl())
                return false;

            // Determine the applet type
            string appletType = IsAppletTypeIcao() ? "ICAO" : "IDL";

            // Check if the requested DG exists in the tag list for the applet type
            return DgTagLists.TryGetValue(appletType, out var dgTagMapping) &&
                   dgTagMapping.TryGetValue(dgNumber, out var expectedTag) &&
                   expectedTag.HasValue && dgList.Contains(expectedTag.Value);
        }
        #endregion


        #region Transmit API test
        // Sample function to send/receive APDUs directly from Application through ElyMrtd library
        private bool TestTransmitApi()
        {
            bool bRet = false;

            do
            {
                // Read EF.COM through transmit API
                PrintListView("Reading EF.COM via transmit API");
                byte[] abyCmdSelectEFCom = new Byte[] { 0x00, 0xA4, 0x02, 0x0C, 0x02, 0x01, 0x1E };
                byte[] abyResSelectEfCom = elyMrtd.transmit(abyCmdSelectEFCom);
                if (elyMrtd.getLastStatusWord() != 0x9000)
                {
                    PrintListView("Error: Select Ef.Com via transmit API failed");
                    break;
                }
                byte[] abyCmdReadBinary = new Byte[] { 0x00, 0xB0, 0x00, 0x00, 0x00 };
                byte[] abyResReadBinary = elyMrtd.transmit(abyCmdReadBinary);
                if (elyMrtd.getLastStatusWord() != 0x9000)
                {
                    PrintListView("Error: Read Ef.Com via transmit API failed");
                    break;
                }

                // Read EF.COM through readEF_COM API
                if (elyMrtd.readEF_COM())
                {
                    byte[] efComData = elyMrtd.getEFCom();
                    PrintListView("EF.COM Read.");
                    if (efComData != null)
                    {
                        byte[] efComResponse = new Byte[efComData.Length];
                        if (abyResReadBinary.Length >= efComData.Length)
                            Array.Copy(abyResReadBinary, efComResponse, efComData.Length);
                        if (!efComData.SequenceEqual(efComResponse))
                        {
                            PrintListView("Transmit API NOT OK");
                            break;
                        }
                        PrintListView("Transmit API OK");
                        bRet = true;
                    }
                    if (GetDgListFromEfCom()) { break; }
                    PrintListViewError("Unable to get DG list from EF.COM");
                }
                else { PrintListViewError("Unable to read EF.COM"); }

            } while (false);

            return bRet;
        }
        #endregion


        #region eMRTD test
        private void RunProcess(string pathToApp, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = pathToApp;
            start.Arguments = args;//args is path to .py file and any cmd line args
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.WriteLine(result);
                }
            }
        }

        private bool RunTestInterfaceScript(string args0, string args1, string args2 = "")
        {
            String pythonPath = formOptions.GetPythonPath();
            if ((pythonPath == null) || (pythonPath == ""))
            {
                PrintListViewError("Python path not available in ELY TRAVEL DOC.ini file");
                return false;
            }
            String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                "\\ELYCTIS\\TestInterface.py";
            String strAgrs = "";
            if (!string.IsNullOrEmpty(args0)) strAgrs = (" " + args0.Trim());
            if (!string.IsNullOrEmpty(args1)) strAgrs += (" " + args1.Trim());
            if (!string.IsNullOrEmpty(args2)) strAgrs += (" " + args2.Trim());
            RunProcess(pythonPath, path + strAgrs);
            return true;
        }

        private void WaitForCardAndReadDocument(bool bPrevCardState)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new _WaitForCardAndReadDocumentDelegate(WaitForCardAndReadDocument), new object[] { bPrevCardState });
                return;
            }
            try
            {
                // Find the PCSC contactless interface
                szScardReader = mszReaderName;
                if ((szScardReader != null) && !szScardReader.Equals("") && !szScardReader.Equals("0"))
                {
                    m_readerState[0].szReader = szScardReader;
                    m_iResMan.GetStatusChange(5, m_readerState);
                    // NOTE: It is possible that there could be no change in state
                    mbCardState = ((m_readerState[0].dwEventState & (uint)SCARD_STATE.SCARD_STATE_PRESENT) <= 0) ? false:true;
                    // Current state changed to CARD_PRESENT from its previous state
                    if (mbCardState && !bPrevCardState)
                    {
                        if (formOptions.checkboxEmrtdTest.Checked)
                        {
                            if (RunTestInterfaceScript("CL", "GetData"))
                            {
                                formOptions = new FormOptions();
                                ClearFormGuiControls();
                                buttonRead.Enabled = false;
                                ReadDocument();
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                PrintListViewError(exc.Message);
            }
        }

        private bool RetrieveTestParams()
        {
            if (!formOptions.checkboxEmrtdTest.Checked)
                return false;

            bool bReturn = false;
            string msg = "eMRTD test: ";
            do
            {
                if (formOptions.rbPasswordTypeMRZ.Checked)
                {
                    if (formOptions.IsUseTestMrzEnabled())
                    {
                        string szMrz = formOptions.GetTestMrz();
                        if (string.IsNullOrEmpty(szMrz))
                        {
                            msg += "Test MRZ is empty";
                            break;
                        }
                        msg += "using MRZ";
                        elyMrzParser.Parse(szMrz);
                        if (!elyMrzParser.IsParseSuccess())
                            throw new Exception(msg + "Invalid MRZ (" + elyMrzParser.ValidationMessage() + ")");
                        szMrzPwd = elyMrzParser.GetMrzPwd();
                        AddMrzLinesToListView();
                        DisplayMrzLinesInTextBox();
                    }
                    else
                    {
                        msg += "UseTestMrz is not enabled";
                        break;
                    }
                }
                else if (formOptions.rbPasswordTypeCAN.Checked)
                {
                    if (string.IsNullOrEmpty(formOptions.textBoxDefaultCAN.Text))
                    {
                        msg += "CAN is empty";
                        break;
                    }
                    msg += "Using CAN: " + (formOptions.textBoxDefaultCAN.Text);
                }
                else
                {
                    msg += "Invalid configuration (MRZ/CAN not configured)";
                    break;
                }

                bReturn = true;

            } while (false);

            if (bReturn)
                PrintListView(msg);
            else
                PrintListViewError(msg);

            return bReturn;
        }

        private bool PushTestResult(string strPutDataValue)
        {
            if (!formOptions.checkboxEmrtdTest.Checked)
                return false;

            bool bReturn = false;
            do
            {
                if (RunTestInterfaceScript("CL", "PutData", strPutDataValue))
                {
                    bReturn = true;
                }

            } while (false);

            return bReturn;
        }
        #endregion


        #region File helpers
        public static void OpenFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                //Process.Start("file://" + fileName); // compatible only to .NET Framework
                var startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C " + string.Format("\"{0}\"", fileName),
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(startInfo);
            }
        }
        #endregion


        #region Console helpers
        private static string ByteArrayToStringFormat(byte[] input)
        {
            if (input != null)
            {
                StringBuilder hex = new StringBuilder(input.Length * 2);
                foreach (byte b in input)
                    hex.AppendFormat("{0:X2} ", b);
                return hex.ToString();
            }
            return "";
        }
        #endregion
    }
}
