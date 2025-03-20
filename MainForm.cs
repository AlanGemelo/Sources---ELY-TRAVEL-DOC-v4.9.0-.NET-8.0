using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ElySCardDotNet;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Linq.Expressions;
using System.Threading;
using static ElyMRTDDotNet.ElyMRTDDotNet;
using ELY_TRAVEL_DOC.Services;

namespace ELY_TRAVEL_DOC
{
    public partial class MainForm : Form, IDisposable
    {
        #region Variables
        // Variables to read chip
        private volatile int nAppletType;
        private volatile List<string> dg1 = new List<string>();
        private volatile List<ElyMRTDDotNet.VehicleCategory> dg1Categories;
        private volatile Bitmap bmpPortrait;
        private volatile Bitmap bmpSignature;
        private volatile List<string> dg11 = new List<string>();
        private volatile List<string> dg12 = new List<string>();
        //private volatile List<string> dg13;
        private volatile List<Bitmap> lbmpBiometricTemplates;
        private volatile List<Bitmap> lbmpIrisBiometricTemplates;
        bool bDisableImageNavigationButtons = false;
        private string szMrzPwd;
        private const int MRZ_PWD_LEN_ICAO = 24;
        private const int MRZ_PWD_LEN_IDL = 28;
        private ArrayList dgList;

        // Variables Reader
        private ISCardManager m_iResMan = null;
        private SCARD_READERSTATE[] m_readerState;
        private ISCardConnection m_iCard = null;
        string szScardReader = "";
        private bool bIsChipInDocument = false;

        // Variables Scanner
        private int m_nComPortIndex = -1;
        private Scanner m_scanner = null;
        private readonly DelegateReadMrz m_delegateReadMrz = null;
        private String m_szMrz = "";

        // ElyMRTDDotNet objects
        private ElyMRTDDotNet.ElyMRTDDotNet elyMrtd;
        private ElyMRTDDotNet.ElyMrzParser elyMrzParser;

        private FormReadingChip formReadingChip;
        private FormMRZ formMRZ;
        private volatile FormOptions formOptions;

        private Stopwatch sw = new Stopwatch();
        private Stopwatch swInquire = new Stopwatch();

        // Variables eMRTD test
        private String mszReaderName; // Current reader name
        private bool mbCardState = false; // Current card presence state
        Thread ThreadEmrtdTest; // Thread for eMRTD test
        #endregion

        private readonly MrzService mrzService;
        private readonly ChipService chipService;
        private readonly ExcelExportService excelExportService = new ExcelExportService();
        private readonly FilterService filterService = new FilterService();
        private List<string> selectedFields = new List<string>();

        private List<PersonalDataDto> personalDataList = new List<PersonalDataDto>();

        #region Constructor
        public MainForm()
        {
            InitializeComponent();
            this.Icon = new Icon("assets/icon.ico");
            InitializeGuiHelpers();

            elyMrtd = new ElyMRTDDotNet.ElyMRTDDotNet();
            elyMrzParser = new ElyMRTDDotNet.ElyMrzParser();

            try
            {
                m_delegateReadMrz = new DelegateReadMrz(ReadMrz);
                m_scanner = new Scanner(m_delegateReadMrz);
            }
            catch (Exception) { ShowMessageBoxToConnectIdBox(); }

            try
            {
                m_iResMan = new SCardManager();
                m_iResMan.EstablishContext(SCOPE.User);

                m_readerState = new SCARD_READERSTATE[1];
                m_readerState[0] = new SCARD_READERSTATE();
                m_readerState[0].dwCurrentState = 0;
                m_readerState[0].pvUserData = IntPtr.Zero;
            }
            catch (Exception exc) { Console.WriteLine("Error: %s" + exc.Message); }

            try { formOptions = new FormOptions(); }
            catch (Exception exc) { Console.WriteLine("Error: %s" + exc.Message); }

            try
            {
                if (UpdateComboBoxComPort())
                {
                    comboBoxComPort.SelectedIndex = m_nComPortIndex;
                    // NOTE: Changing SelectedIndex triggers comboBoxComPort_SelectedIndexChanged()
                }
                UpdateComboBoxCcidReaders();
                UpdateReadButtonState();
            }

            catch (Exception exc) { Console.WriteLine("Error: %s" + exc.Message); }
            if (formOptions.checkboxEmrtdTest.Checked)
                SetEmrtdTest(true);

            mrzService = new MrzService();
            chipService = new ChipService();

            buttonExportExcel.Click += ButtonExportExcel_Click;
            comboBoxFields.Items.AddRange(new string[] { "Name", "Surname", "BirthDate", "Nationality", "Sex", "ExpiryDate", "DocumentNumber", "DocumentType", "Issuer", "OptionalData" });
            comboBoxFields.SelectedIndexChanged += ComboBoxFields_SelectedIndexChanged;
        }
        #endregion


        #region Destructor
        ~MainForm()
        {
            DeInitialzeGuiHelpers();
            SetEmrtdTest(false);
            if ((m_scanner != null) && (m_scanner.IsConnected()))
            {
                m_scanner.Disconnect();
                m_scanner = null;
            }
            if (m_iCard != null)
            {
                m_iCard.Disconnect(DISCONNECT.Reset);
                m_iCard = null;
            }
            if (m_iResMan != null)
            {
                m_iResMan.ReleaseContext();
                m_iResMan = null;
            }
        }
        public void dispose()
        {
            SetEmrtdTest(false);
        }
        #endregion


        #region Form load
        private void MainForm_Load(object sender, EventArgs e)
        {
            ShowAppVersion();
        }
        #endregion


        #region Read MRZ
        private void ReadMrz(string szMrz)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    // Manually feed MRZ here to override the Scanner value with "\r\n" as line separator
                    //szMrz = "I<UTOD231458907<<<<<<<<<<<<<<<\r\n7408122F1204159UTO<<<<<<<<<<<6\r\nERIKSSON<<ANNA<MARIA<<<<<<<<<<\r\n"; // Default TD1 MRZ
                    //szMrz = "I<UTOERIKSSON<<ANNA<MARIA<<<<<<<<<<<\r\nL898902C<3UTO6908061M9406236<<<<<<<9\r\n\r\n";                   // Default TD2 MRZ
                    //szMrz = "P<UTOERIKSSON<<ANNA<MARIA<<<<<<<<<<<<<<<<<<<\r\nL898902C<3UTO6908061M9406236<<<<<<<<<<<<<<<9\r\n\r\n";   // Default TD3 MRZ
                    //szMrz = "IDFRAPORTE<<<<<<<<<<<<<<<<<<<<131014\r\n0902131014575LUDIVINE<<MARI0210041F6\r\n\r\n";                   // Default NID MRZ
                    this.Invoke(m_delegateReadMrz, new Object[] { szMrz });
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }
                if ((m_scanner != null) && (!m_scanner.EnableContinuousReading) && m_scanner.IsConnected())
                    m_scanner.Disconnect();
                return;
            }
            swInquire.Stop();
            Console.WriteLine("MRZ read time: " + GetElapsedTimeAsString(swInquire.Elapsed));
            ClearFormGuiControls();

            try
            {
                m_szMrz = szMrz;
                sw.Restart();
                elyMrzParser.Parse(PreProcessMrz(szMrz));
                if (!elyMrzParser.IsParseSuccess())
                    throw new Exception("Invalid MRZ (" + elyMrzParser.ValidationMessage() + ")");
                szMrzPwd = elyMrzParser.GetMrzPwd();
                sw.Stop();
                Console.WriteLine("MRZ Parsing: " + GetElapsedTimeAsString(sw.Elapsed));

                bool bIsReadDocumentValid = false;
                if (elyMrzParser.DocumentType() == null)
                {
                    PrintListView("ERROR: Document type is null");
                    this.pictureBoxMRZDetected.Image = ELY_TRAVEL_DOC.Properties.Resources.fail;
                    this.pictureBoxMRZRead.Image = ELY_TRAVEL_DOC.Properties.Resources.fail;
                }
                else
                {
                    if (IsDocTypeD())
                    {
                        // MRZ password length for Driving License is 28 digits
                        if (szMrzPwd.Length != MRZ_PWD_LEN_IDL) {
                            PrintListView("Warning: Invalid MRZ password");
                            this.pictureBoxMRZDetected.Image = ELY_TRAVEL_DOC.Properties.Resources.fail;
                            this.pictureBoxMRZRead.Image = ELY_TRAVEL_DOC.Properties.Resources.warning;
                        }
                        else
                        {
                            this.pictureBoxMRZDetected.Image = ELY_TRAVEL_DOC.Properties.Resources.success;
                            if (elyMrzParser.IsCheckDigitOfOverAllMrzValid()) {
                                this.pictureBoxMRZRead.Image = ELY_TRAVEL_DOC.Properties.Resources.success;
                            } else {
                                PrintListView("Warning: Invalid MRZ checksum");
                                this.pictureBoxMRZRead.Image = ELY_TRAVEL_DOC.Properties.Resources.warning;
                            }
                            // Some document samples may have wrong checkdigits.
                            // ELY TRAVEL DOC allows reading such documents.
                            // Change it to false, according to your application.
                            bIsReadDocumentValid = true;
                        }
                    }
                    else
                    {
                        // --------------------------------------------------------------
                        // NOTE: MRZ password length for ICAO documents is 24 digits
                        // It contains:
                        //  DocNum  : 10+ digits (last digit is checkdigit)
                        //  DOB     : 07 digits (last digit is checkdigit)
                        //  DOE     : 07 digits (last digit is checkdigit)
                        // --------------------------------------------------------------
                        if (szMrzPwd.Length < MRZ_PWD_LEN_ICAO)
                        {
                            PrintListView("Warning: Invalid MRZ password");
                            this.pictureBoxMRZDetected.Image = ELY_TRAVEL_DOC.Properties.Resources.fail;
                            this.pictureBoxMRZRead.Image = ELY_TRAVEL_DOC.Properties.Resources.warning;
                        }
                        else
                        {
                            this.pictureBoxMRZDetected.Image = ELY_TRAVEL_DOC.Properties.Resources.success;
                            if (/*elyMrzParser.IsCheckDigitOfOverAllMrzValid() &&*/
                                // Some document samples may have wrong checkdigit for the overall MRZ. Enable this, if required.
                                elyMrzParser.IsCheckDigitOfDocNumValid() &&
                                elyMrzParser.IsCheckDigitOfDobValid() &&
                                elyMrzParser.IsCheckDigitOfDoeValid()) {
                                this.pictureBoxMRZRead.Image = ELY_TRAVEL_DOC.Properties.Resources.success;
                            }
                            else
                            {
                                // One of the checkdigits is wrong.
                                // For scenarios where MRZ is printed non-standard, the OCR characters could be read
                                // incorrectly by the Scanner. (ELYCTIS MRZ Scanner is ICAO compliant).
                                // In this case, the szMrzPwd shall be modified to the expected way by the developer.
                                // Ex: Replacing 'O's with '0's. One such case is dealt in the following piece of code.
                                if ((elyMrzParser.DocumentType() != null) && (elyMrzParser.DocumentType() == "I") &&
                                    (elyMrzParser.NationalityIso() != null) && (elyMrzParser.NationalityIso() == "UZB") &&
                                    (!elyMrzParser.IsCheckDigitOfDocNumValid()))
                                {
                                    // Try changing 'O' with '0'
                                    string newDocNum = elyMrzParser.DocumentNumber().Replace('O', '0');
                                    szMrzPwd = szMrzPwd.Replace(elyMrzParser.DocumentNumber(), newDocNum);
                                    this.pictureBoxMRZRead.Image = ELY_TRAVEL_DOC.Properties.Resources.success;
                                }
                                else
                                {
                                    PrintListView("Warning: Invalid MRZ checksum");
                                    this.pictureBoxMRZRead.Image = ELY_TRAVEL_DOC.Properties.Resources.warning;
                                }
                            }
                            // Some document samples may have wrong checkdigits.
                            // ELY TRAVEL DOC allows reading such documents.
                            // Change it to false, according to your application.
                            bIsReadDocumentValid = true;
                        }
                    }

                    // Display the MRZ
                    AddMrzLinesToListView();
                    DisplayMrzLinesInTextBox();
                    if (!bIsReadDocumentValid)
                        PrintListViewError("Document reading aborted");
                    else
                        ReadDocument();
                }
            }
            catch (Exception exc)
            {
                PrintListViewError("Error while reading/parsing MRZ: " + exc.Message);
                DisplayMrzLinesInTextBox();
                this.pictureBoxMRZDetected.Image = ELY_TRAVEL_DOC.Properties.Resources.fail;
                this.pictureBoxMRZRead.Image = ELY_TRAVEL_DOC.Properties.Resources.fail;
                this.pictureBoxAntennaSelected.Image = ELY_TRAVEL_DOC.Properties.Resources.fail;
                this.pictureBoxChipRead.Image = ELY_TRAVEL_DOC.Properties.Resources.fail;
            }
            finally
            {
                UpdateReadButtonState();
            }
        }
        #endregion


        #region Detect reader & card
        private bool IsReaderAndCardDetected()
        {
            PrintListView("Detecting chip...");

            try
            {
                // Find the PCSC contactless interface
                szScardReader = GetScardReaders().Contains(comboBoxCcidReaders.Text) ?
                    comboBoxCcidReaders.Text : comboBoxCcidReaders.GetItemText(0);
                if (!szScardReader.Equals("") && !szScardReader.Equals("0"))
                {
                    DialogResult result = DialogResult.None;
                    if (IsEppOnIdBox5xxOr6xx(m_szMrz)) {
                        result = ShowMessageBoxToPlaceEppOnIdBox5xxOr6xx();
                    }
                    m_readerState[0].szReader = szScardReader;
                    do
                    {
                        m_iResMan.GetStatusChange(5, m_readerState);
                        // NOTE: It is possible that there could be no change in state.

                        if ((m_readerState[0].dwEventState & (uint)SCARD_STATE.SCARD_STATE_PRESENT) <= 0)
                        {
                            if (result != DialogResult.OK)
                            {
                                result = ShowMessageBoxToPlaceDocument();
                                continue;
                            }
                            else
                            {
                                PrintListViewError("No card detected !");
                                return false;
                            }
                        }

                        this.pictureBoxAntennaSelected.Image = ELY_TRAVEL_DOC.Properties.Resources.success;

                        this.bIsChipInDocument = true;
                        return this.bIsChipInDocument;

                    } while (true);
                }
            }
            catch (Exception exc)
            {
                PrintListViewError(exc.Message);
                this.pictureBoxAntennaSelected.Image = ELY_TRAVEL_DOC.Properties.Resources.fail;
            }

            return false;
        }
        #endregion


        #region Read Chip
        private void ReadDocument()
        {
            try
            {
                if (formOptions.checkboxEmrtdTest.Checked && !RetrieveTestParams())
                    return;

                // Reset the parse status of the previous MRZ reading
                if (szMrzPwd == null)
                {
                    elyMrzParser = new ElyMRTDDotNet.ElyMrzParser();
                }

                // MRZ password required for other than PACE-CAN based access
                if (formOptions.rbAccessControlBAC.Checked ||
                    formOptions.rbPasswordTypeAskPwd.Checked ||
                    formOptions.rbPasswordTypeMRZ.Checked)
                {
                    if (szMrzPwd == null)
                    {
                        PrintListViewError("MRZ not found");
                        return;
                    }
                    else if ((szMrzPwd.Length < MRZ_PWD_LEN_ICAO)
                        || (IsDocTypeD() && (szMrzPwd.Length != MRZ_PWD_LEN_IDL)))
                    {
                        PrintListViewError("Incorrect MRZ password");
                        return;
                    }
                }

                if (IsReaderAndCardDetected())
                {
                    PrintListView("Reading Document...");
                    formReadingChip = new FormReadingChip();
                    bmpPortrait = null;

                    // Read asynchronously to display the progress bar
                    backgroundWorker.RunWorkerAsync(new object[] {
                        elyMrtd, szScardReader,
                        szMrzPwd/*"1UTO123456789SMITH071078<<<<"*/,
                        IsDocTypeD()});

                    ShowFormReadingChip();
                    if (formOptions.checkBoxEnableLog.Checked)
                        this.buttonLastLog.Enabled = true;
                }
                else
                {
                    this.pictureBoxAntennaSelected.Image = ELY_TRAVEL_DOC.Properties.Resources.fail;
                    this.pictureBoxChipRead.Image = ELY_TRAVEL_DOC.Properties.Resources.fail;

                    if (szMrzPwd != null)
                    {
                        PrintListView("Displaying information from the MRZ. Not from the chip !!");
                        DisplayMrzFieldsFromParser();
                        if (formOptions.checkBoxCSV.Checked)
                            WriteCsv();
                    }
                }
            }
            catch (Exception exc)
            {
                PrintListViewError(exc.Message);
                this.pictureBoxChipRead.Image = ELY_TRAVEL_DOC.Properties.Resources.fail;
                DisplayMrzFieldsFromParser();
                if (formOptions.checkBoxCSV.Checked)
                    WriteCsv();
            }
            finally
            {
                UpdateReadButtonState();
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get the arguments
            ElyMRTDDotNet.ElyMRTDDotNet elyMrtd = (ElyMRTDDotNet.ElyMRTDDotNet)((object[])e.Argument)[0];
            if (formOptions.checkBoxEnableLog.Checked)
            {
                elyMrtd.logInit(formOptions.GetLogPath());
                elyMrtd.logMsg("Scanner FW version", labelScannerFwVersion.Text);
                elyMrtd.logMsg("Scanner NNA version", labelScannerNnaVersion.Text);
                elyMrtd.logMsg("Scanner S/N version", labelIdBoxSerialNumber.Text);
                elyMrtd.logMsg("Scanner P/N version", labelIdBoxProductNumber.Text);
                elyMrtd.logMsg("Contactless FW version", labelContactlessFwVersion.Text);
                elyMrtd.logMsg("Contactless Driver version", labelContactlessDriver.Text);
                if ((m_szMrz != null) && (!m_szMrz.Equals("")))
                {
                    elyMrtd.logMsg("MRZ", m_szMrz);
                }
            }

            string szReader = (string)((object[])e.Argument)[1];
            string szMrzInfo = (string)((object[])e.Argument)[2];
            bool bIsPwdRaw = (bool)((object[])e.Argument)[3];

            // (Unlike ICAO-PACE case) the szMrzPwd must be used in raw for ICAO-IDL
            elyMrtd.useMrzPwdAsRaw(bIsPwdRaw);

            // Read chip
            if (bIsChipInDocument)
            {
                PrintListView("Connecting to reader...");

                if (this.formOptions.checkBoxFindAntenna.Checked)
                    // Update the antenna selection view before reading the document
                    UpdateAntennaSelectionView();
                else
                    // Cleanup the session before reading the document
                    CleanupSession();

                byte byRetry = 2;
                while (true)
                {
                    try { nAppletType = elyMrtd.connect(szReader); }
                    catch (Exception exc) { ShowMessageBoxReaderConnectionError(exc.Message); }
                    if (nAppletType == -1)
                    {
                        if (--byRetry != 0)
                        {
                            PrintListView("Reconnecting with the card...");

                            // Cleanup the session before reading the document
                            CleanupSession();
                            continue;
                        }
                        PrintListView("Please check that an ICAO document is in the ID BOX");
                        return;
                    }
                    break;
                }

                try
                {
                    PrintListView("Connection successful.");

                    try
                    {
                        int nApduFormat = formOptions.comboBoxApduType.SelectedIndex;
                        if (nApduFormat != -1)
                        {
                            elyMrtd.setApduFormat((uint)nApduFormat, formOptions.GetMaxLe());
                        }
                        else
                        {
                            // value '0' is for automatic apdu selection
                            elyMrtd.setApduFormat(0, formOptions.GetMaxLe());
                        }

                        if (IsAppletTypeIcao())
                        {
                            bool bStatus = false;
                            bool bPlainDoc = false;
                            string szConnectionName = null;

                            sw.Restart();

                            // v4.8.0.0: Added support to perform PACE or BAC based on the access control options provided in the Form options
                            // Access control option is set to either "Auto" or "PACE"
                            if (formOptions.rbAccessControlAuto.Checked || formOptions.rbAccessControlPACE.Checked)
                            {
                                if ((formOptions.listView1.Items.Find("listViewItemCA", true))[0].Checked)
                                    GetInfosEfCardAccess();

                                // Document supports PACE
                                if (nAppletType == (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_PACE)
                                {
                                    szConnectionName = "PACE";
                                    PrintListView("Establish PACE...");
                                    if (formOptions.rbPasswordTypeAskPwd.Checked) // PACE Password has to be manually seeked from the user
                                    {
                                        FormPasswordPACE formPasswordPACE = new FormPasswordPACE();
                                        DialogResult dRes = formPasswordPACE.ShowDialog();
                                        if (dRes == System.Windows.Forms.DialogResult.Yes)
                                            bStatus = formPasswordPACE.bIsMRZSelected ?
                                                elyMrtd.establishAccessControl(szMrzInfo, 1) :
                                                elyMrtd.establishAccessControl(formPasswordPACE.szCAN, 2);
                                    }
                                    else if (formOptions.rbPasswordTypeMRZ.Checked) // PACE Password is MRZ
                                        bStatus = elyMrtd.establishAccessControl(szMrzInfo, 1);
                                    else if (formOptions.rbPasswordTypeCAN.Checked) // PACE Password is CAN
                                    {
                                        string szCAN = formOptions.textBoxDefaultCAN.Text;
                                        if (szCAN.Equals(""))
                                        {
                                            FormPasswordPACE formPasswordPACE = new FormPasswordPACE(true /* bIsCANOnlyMode */);
                                            DialogResult dRes = formPasswordPACE.ShowDialog();
                                            if (dRes == System.Windows.Forms.DialogResult.Yes)
                                                szCAN = formPasswordPACE.szCAN;
                                        }
                                        bStatus = elyMrtd.establishAccessControl(szCAN, 2);
                                    }
                                }
                                // Document doesn't support PACE, but the Access control option is set to "Auto"; so try BAC
                                else if (formOptions.rbAccessControlAuto.Checked)
                                {
                                    if (IsDocumentAccessible())
                                    {
                                        bPlainDoc = true;
                                        szConnectionName = "Plain connection";
                                    }
                                    else
                                    {
                                        szConnectionName = "BAC";
                                        PrintListView("Establish BAC...");
                                        bStatus = elyMrtd.establishBAC(szMrzInfo);
                                    }
                                }
                                // Document doesn't support PACE, but the Access control option is set to PACE
                                else
                                {
                                    szConnectionName = "PACE";
                                    PrintListViewError("PACE is not supported");
                                }
                            }
                            // Access control option is set to BAC
                            else
                            {
                                if (IsDocumentAccessible())
                                {
                                    bPlainDoc = true;
                                    szConnectionName = "Plain connection";
                                }
                                else
                                {
                                    szConnectionName = "BAC";
                                    PrintListView("Establish BAC...");
                                    bStatus = elyMrtd.establishBAC(szMrzInfo);
                                }
                            }

                            if (bStatus || bPlainDoc)
                            {
                                // After establishing authentication, reconfirm if the applet type is ICAO or IDL.
                                nAppletType = elyMrtd.getAppletType();

                                sw.Stop();
                                Console.WriteLine(szConnectionName + ": " + GetElapsedTimeAsString(sw.Elapsed));
                                PrintListViewSuccess(szConnectionName + " established.");

                                if (!bPlainDoc)
                                {
                                    if (IsAppletTypeIdl())
                                        UpdatePictureBox(this.pictureBoxBAP, true);
                                    else
                                        UpdatePictureBox(this.pictureBoxBAC, true);

                                    // Read EF.COM after [BAC/BAP] or [PACE]
                                    GetInfosEfCom();
                                }

                                PerformAuthAndGetDgs(ref e);
                            }
                            else
                            {
                                if (IsAppletTypeIdl())
                                    UpdatePictureBox(this.pictureBoxBAP, false);
                                else
                                    UpdatePictureBox(this.pictureBoxBAC, false);
                                PrintListViewError("Can't establish " + szConnectionName + ".");

                                elyMrtd.disconnect();
                                if (formOptions.checkBoxEnableLog.Checked)
                                    elyMrtd.logEnd();

                                if (elyMrzParser.IsParseSuccess())
                                {
                                    PrintListView("Displaying information from the MRZ. Not from the chip !");

                                    try
                                    {
                                        DisplayMrzFieldsFromParser();

                                        if (formOptions.checkBoxCSV.Checked)
                                            WriteCsv();
                                    }
                                    catch (Exception exc)
                                    {
                                        Console.WriteLine("Error: %s" + exc.Message);
                                    }
                                    finally
                                    {
                                        HideFormReadingChip();
                                    }
                                }
                            }

                            e.Result = new object[] { 0 };
                        }
                        else if (IsAppletTypeIdl())
                        {
                            PrintListView("Establish BAP...");
                            if (elyMrtd.establishBAC(szMrzInfo))
                            {
                                PrintListViewSuccess("BAP established.");
                                UpdatePictureBox(this.pictureBoxBAP, true);
                            }
                            else
                            {
                                UpdatePictureBox(this.pictureBoxBAP, false);
                                PrintListViewError("Can't establish BAP.");
                            }
                            // Read EF.COM after [BAC/BAP] or [PACE]
                            GetInfosEfCom();

                            PerformAuthAndGetDgs(ref e);

                            e.Result = new object[] { 0 };
                        }
                    }
                    catch (Exception exc)
                    {
                        e.Result = new object[] { -2, new Exception(exc.Message) };
                    }
                }
                catch (Exception exc)
                {
                    e.Result = new object[] { -1, new Exception(exc.Message) };
                }
                finally
                {
                    elyMrtd.disconnect();
                    if (formOptions.checkBoxEnableLog.Checked)
                        elyMrtd.logEnd();
                    PrintListView("Disconnected.");
                }
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            object[] res = (object[])e.Result;
            try
            {
                if ((res != null) && ((int)res[0] == -1))
                {
                    this.pictureBoxChipRead.Image = ELY_TRAVEL_DOC.Properties.Resources.fail;
                    PrintListViewError(((Exception)res[1]).Message);
                }
                else
                {
                    if ((res != null) && ((int)res[0] == -2))
                    {
                        PrintListView(((Exception)res[1]).Message);
                    }

                    if (IsAppletTypeIcao())
                    {
                        this.tabControl1.SelectedTab = this.tabPageID;

                        if (elyMrzParser.IsParseSuccess() || (dg1 != null && dg1.Count == 10))
                        {
                            if (dg1 != null && dg1.Count == 10)
                                DisplayMrzFieldsFromDg1(); // if DG1 exists, display its fields
                            else
                                DisplayMrzFieldsFromParser(); // else display from elyMrzParser

                            if (formOptions.checkBoxCSV.Checked)
                                WriteCsv();

                            dg1.Clear();
                            dg11.Clear();
                            dg12.Clear();
                            //dg13.Clear();

                            if (bmpPortrait != null)
                            {
                                Console.WriteLine("DG2 not null");
                                try
                                {
                                    this.pictureBoxPicture.Image = (Bitmap)bmpPortrait.Clone();
                                }
                                catch (Exception)
                                {
                                    PrintListViewError("Error while decoding picture !");
                                }
                            }
                            else
                                Console.WriteLine("DG2 null");
                        }
                        else
                        {
                            this.pictureBoxChipRead.Image = ELY_TRAVEL_DOC.Properties.Resources.fail;
                            PrintListViewError(((Exception)res[1]).Message);
                            HideFormReadingChip();
                            return;
                        }
                    }
                    else if (IsAppletTypeIdl())
                    {
                        this.tabControl1.SelectedTab = this.tabPageIDL;
                        if (dg1 != null && dg1.Count == 8)
                        {
                            DisplayMrzFieldsFromDg1(); // if DG1 exists, display its fields
                        }
                        // [TODO]: elyMrzParser for IDL will be added later

                        if (formOptions.checkBoxCSV.Checked)
                            WriteCsv();

                        dg1.Clear();
                        if (dg1Categories != null)
                        {
                            foreach (ElyMRTDDotNet.VehicleCategory category in dg1Categories)
                            {
                                TreeNode node = new TreeNode(category.Category);
                                if (!String.IsNullOrEmpty(category.ValidFrom)) node.Nodes.Add("Valid from: " + category.ValidFrom);
                                if (!String.IsNullOrEmpty(category.ValidTo)) node.Nodes.Add("Valid to: " + category.ValidTo);
                                if (!String.IsNullOrEmpty(category.Code)) node.Nodes.Add("Code: " + category.Code);
                                if (!String.IsNullOrEmpty(category.Sign)) node.Nodes.Add("Sign: " + category.Sign);
                                if (!String.IsNullOrEmpty(category.Value)) node.Nodes.Add("Value: " + category.Value);
                                treeView1.Nodes.Add(node);
                            }
                            treeView1.ExpandAll();
                            dg1Categories.Clear();
                        }

                        if (bmpPortrait != null)
                        {
                            Console.WriteLine("DG4 not null");
                            try
                            {
                                this.pictureBoxPictureIdl.Image = (Bitmap)bmpPortrait.Clone();
                            }
                            catch (Exception)
                            {
                                PrintListViewError("Error while decoding picture !");
                            }
                        }
                        else
                            Console.WriteLine("DG4 null");
                    }
                    this.pictureBoxChipRead.Image = ELY_TRAVEL_DOC.Properties.Resources.success;
                }
                elyMrtd.disconnect();
            }
            catch (Exception exc) { Console.WriteLine("Error: %s" + exc.Message); }
            finally
            {
                szMrzPwd = null;
                HideFormReadingChip();
                if (formOptions.checkboxEmrtdTest.Checked)
                    PushTestResult(ElyMRTDDotNet.ElyMRTDDotNet.getEmrtdTestPdo());
            }
        }

        private void PerformAuthAndGetDgs(ref DoWorkEventArgs e)
        {
            do
            {
                if (formOptions.checkBoxPA.Checked)
                    PassiveAuthentication();
                if (formOptions.checkBoxAA.Checked)
                    ActiveAuthentication();
                if (formOptions.checkBoxCA.Checked)
                    ChipAuthentication();
                    /*if (!ChipAuthentication())
                    {
                        e.Result = new object[] { 0 };
                        break;
                    }*/

                GetInfoMandatoryDgs();
                GetInfoOptionalDgs();

                if (formOptions.checkBoxTA.Checked)
                {
                    try
                    {
                        if (!TerminalAuthentication())
                        {
                            break;
                        }
                    }
                    catch (Exception exc)
                    {
                        e.Result = new object[] { -1, new Exception(exc.Message) };
                    }
                }

                GetInfoProtectedDgs();

            } while (false);
        }

        private void PassiveAuthentication()
        {
            PrintListView("Reading EF.SOD...");

            if (elyMrtd.readEF_SOd())
            {
                // Get EF.SOD file
                byte[] ef_sod = elyMrtd.getEFSOd();
                // Get Document signer certificate from EF.SOD
                byte[] dsc = elyMrtd.getEFSodDocSignerCert();
                // Get Document signer certificate issuer name
                byte[] dscIssuerName = elyMrtd.getEFSodDocSignerCertIssuerName();
                // Get Document signer certificate issuer unique id
                byte[] dscIssuerUniqueId = elyMrtd.getEFSodDocSignerCertIssuerUniqueId();
                // Get Document signer certificate serial number
                byte[] dscSerialNumber = elyMrtd.getEFSodDocSignerCertSerialNumber();
                PrintListView("EF.SOD Read.");

                PrintListView("Perform Passive Authentication.");

                int iRes;

                if (!string.IsNullOrEmpty(this.formOptions.textBoxCSCA.Text) && !string.IsNullOrEmpty(this.formOptions.textBoxExternalDS.Text))
                    // Do passive authentication with verification of Document Signer certificate and CSCA
                    iRes = elyMrtd.passiveAuthentication(this.formOptions.textBoxCSCA.Text, this.formOptions.textBoxExternalDS.Text);
                else if (!string.IsNullOrEmpty(this.formOptions.textBoxCSCA.Text) && dsc != null)
                    // Do passive authentication with verification of CSCA and Document Signer Certificate available in EF.SOD
                    iRes = elyMrtd.passiveAuthentication(this.formOptions.textBoxCSCA.Text);
                else
                    // Do passive authentication(only DG hash verified. No certificate verification)
                    iRes = elyMrtd.passiveAuthentication();

                if (IsAppletTypeIdl())
                    UpdatePictureBox(this.pictureBoxPA_BAP, (iRes == 1));
                else
                    UpdatePictureBox(this.pictureBoxPA, (iRes == 1));

                if (iRes == 1)
                    PrintListViewSuccess("Passive Authentication successful.");
                else
                    PrintListViewError("Passive Authentication failed !");
            }
        }

        private bool ActiveAuthentication()
        {
            bool result = false;
            int iRes = -1;

            if (IsAppletTypeIcao() && IsDgPresentInEfCom(15))
            {
                PrintListView("Reading DG15...");

                iRes = elyMrtd.readDG15();
                UpdatePictureBoxesDg(this.pictureBoxDG15, iRes);

                if (iRes >= 0)
                    PrintListView("DG15 Read.");
                else if (iRes == (int)DG_RETURN_CODES.DG_FILE_NOT_FOUND)
                    PrintListViewWarning("File not found");
                else
                    PrintListViewError("Unable to read DG15 !");
            }
            else if (IsAppletTypeIdl() && IsDgPresentInEfCom(13))
            {
                PrintListView("Reading DG13...");

                iRes = elyMrtd.readDG13();
                UpdatePictureBoxesDg(this.pictureBoxDG13_BAP, iRes);

                if (iRes >= 0)
                    PrintListView("DG13 Read.");
                else if (iRes == (int)DG_RETURN_CODES.DG_FILE_NOT_FOUND)
                    PrintListViewWarning("File not found");
                else
                    PrintListViewError("Unable to read DG13 !");
            }
            else
                PrintListViewWarning("Active authentication not supported");

            // If Read DG success, perform Active Authentication
            if (iRes >= 0)
            {
                PrintListView("Perform Active Authentication.");

                if (elyMrtd.activeAuthentication())
                {
                    PrintListViewSuccess("Active Authentication successful.");
                    Byte[] challenge = elyMrtd.getAaChallenge();
                    Byte[] AAResult = elyMrtd.getAaResult();
                    Console.WriteLine("AA challenge");
                    Console.WriteLine(ByteArrayToStringFormat(challenge));
                    Console.WriteLine("AA result");
                    Console.WriteLine(ByteArrayToStringFormat(AAResult));
                    result = true;
                }
                else
                    PrintListViewError("Active Authentication failed !");

                if (IsAppletTypeIdl())
                    UpdatePictureBox(this.pictureBoxAA_BAP, result);
                else
                    UpdatePictureBox(this.pictureBoxAA, result);
            }

            return result;
        }

        private bool ChipAuthentication()
        {
            bool result = false;

            if (IsDgPresentInEfCom(14))
            {
                PrintListView("Reading DG14...");

                int iRes = elyMrtd.readDG14();
                UpdatePictureBoxesDg(this.pictureBoxDG14, iRes);

                if (iRes >= 0)
                {
                    PrintListView("DG14 Read.");
                    PrintListView("Perform Chip Authentication.");

                    if (elyMrtd.chipAuthentication())
                    {
                        PrintListViewSuccess("Chip Authentication successful.");
                        byte[] eACCAResult = elyMrtd.getEacCaResult();
                        Console.WriteLine("CA result");
                        Console.WriteLine(ByteArrayToStringFormat(eACCAResult));
                        result = true;
                    }
                    else
                        PrintListViewError("Chip Authentication failed !");
                }
                else
                    PrintListViewError("Unable to read DG14 !");

                UpdatePictureBox(this.pictureBoxCA, result);
            }
            else
                PrintListViewWarning("Chip authentication not supported");

            return result;
        }

        private bool TerminalAuthentication()
        {
            bool isTerminalAuthSuccess = false;
            PrintListView("Perform Terminal Authentication.");

            if (!System.IO.File.Exists(this.formOptions.textBoxDV.Text)
                || !System.IO.File.Exists(this.formOptions.textBoxIS.Text)
                || !System.IO.File.Exists(this.formOptions.textBoxISPK.Text))
            {
                PrintListViewError("Terminal Authentication impossible. Please check DV, IS configuration !");
            }
            else
            {
#if true
                // Perform terminal authentication with the provided certificates
                // Note : CVCA Link Certificate is required only if a new CVCA public key is to be added to the list of trust-points
                // into the ICC; otherwise it can be set to NULL (Reference: Section 7.1.4.3.2 Link Certificates of Doc 9303
                // Machine Readable Travel Documents Part 11: Security Mechanisms for MRTDs Eighth Edition, 2021)
                isTerminalAuthSuccess = elyMrtd.terminalAuthentication(
                    this.formOptions.textBoxCVCALink.Text, this.formOptions.textBoxDV.Text,
                    this.formOptions.textBoxIS.Text, this.formOptions.textBoxISPK.Text);
#else
                // Sample code to perform terminal authentication from an external terminal
                byte[] EF_CVCA = null;
                if (elyMrtd.readEF_CVCA())
                    EF_CVCA = elyMrtd.getEFCVCA();

                Byte[] cvcalinkCertificate = File.ReadAllBytes(this.formOptions.textBoxCVCALink.Text);
                Byte[] cvcaChr = elyMrtd.getCertCHR(cvcalinkCertificate);
                Byte[] cvcaCar = elyMrtd.getCertCAR(cvcalinkCertificate);

                Byte[] dvCertificate = File.ReadAllBytes(this.formOptions.textBoxDV.Text);
                Byte[] terminalCertificate = File.ReadAllBytes(this.formOptions.textBoxIS.Text);

                if (dvCertificate != null && terminalCertificate != null)
                {
                    // If cvca link certificate not available then no need to invoke setCvcaLinkCertificate method
                    if (cvcalinkCertificate == null || elyMrtd.setCvcaLinkCertificate(cvcalinkCertificate)) 
                    {
                        if (elyMrtd.setDvCertificate(dvCertificate))
                        {
                            if (elyMrtd.setTerminalCertificate(terminalCertificate))
                            {
                                if (elyMrtd.verifyCertificateChain())
                                {
                                    Byte[] plainData = elyMrtd.getTerminalDataToBeSigned();
                                    if (plainData != null)
                                    {
                                        // Replace this line with your implementation to retrieve the signed data from an external terminal
                                        Byte[] signedData = elyMrtd.test_getsigned_data(plainData, this.formOptions.textBoxISPK.Text);
                                        if (signedData != null)
                                        {
                                            if (elyMrtd.externalTerminalAuthentication(signedData))
                                            {
                                                isTerminalAuthSuccess = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
#endif
            }

            if (isTerminalAuthSuccess)
                PrintListViewSuccess("Terminal Authentication successful.");
            else
                PrintListViewError("Terminal Authentication failed !");

            UpdatePictureBox(this.pictureBoxTA, isTerminalAuthSuccess);
            UpdatePictureBox(this.pictureBoxEAC, isTerminalAuthSuccess);

            return isTerminalAuthSuccess;
        }

        #region Get DG Infos
        private void GetInfoMandatoryDgs()
        {
            GetInfosDg1();
                // As per ICAO spec: Machine Readable Zone Information
                // As per ISO 18013 spec: Demographic data and endorsement/restriction information

            if (!IsAppletTypeIdl())
            {
                GetInfosDg2(); // As per ICAO spec: Encoded identification feature - Face
            }
            else
            {
                GetInfosDg6(); // As per ISO 18013 spec: Facial biometric template
                if (null == bmpPortrait)
                    GetInfosDg4(); // As per ISO 18013 spec: Portrait image of license holder
            }
        }
        private void GetInfoOptionalDgs()
        {
            if (!IsAppletTypeIdl())
            {
                if ((formOptions.listView1.Items.Find("listViewItemDG5", true))[0].Checked)
                    GetInfosDg5(); // As per ICAO spec: Displayed portrait
                if ((formOptions.listView1.Items.Find("listViewItemDG6", true))[0].Checked)
                    GetInfosDg6(); // As per ICAO spec: Reserved for future
                if ((formOptions.listView1.Items.Find("listViewItemDG7", true))[0].Checked)
                    GetInfosDg7(); // As per ICAO spec: Displayed signature
                if ((formOptions.listView1.Items.Find("listViewItemDG8", true))[0].Checked)
                    GetInfosDg8(); // As per ICAO spec: Data feature(s)
                if ((formOptions.listView1.Items.Find("listViewItemDG9", true))[0].Checked)
                    GetInfosDg9(); // As per ICAO spec: Structure feature(s)
                if ((formOptions.listView1.Items.Find("listViewItemDG10", true))[0].Checked)
                    GetInfosDg10(); // As per ICAO spec: Substance feature(s)
                if ((formOptions.listView1.Items.Find("listViewItemDG11", true))[0].Checked)
                    GetInfosDg11(); // As per ICAO spec: Additional personal detail(s)
                if ((formOptions.listView1.Items.Find("listViewItemDG12", true))[0].Checked)
                    GetInfosDg12(); // As per ICAO spec: Additional document detail(s)
                if ((formOptions.listView1.Items.Find("listViewItemDG13", true))[0].Checked)
                    GetInfosDg13(); // As per ICAO spec: Optional detail(s)
                if ((formOptions.listView1.Items.Find("listViewItemDG16", true))[0].Checked)
                    GetInfosDg16(); // As per ICAO spec: Person(s) to notify
            }
            else
            {
                if ((formOptions.listView2.Items.Find("listViewItemDG2", true))[0].Checked)
                    GetInfosDg2(); // As per ISO 18013 spec: License holder details
                if ((formOptions.listView2.Items.Find("listViewItemDG3", true))[0].Checked)
                    GetInfosDg3(); // As per ISO 18013 spec: Issuing authority details
                if ((formOptions.listView2.Items.Find("listViewItemDG5", true))[0].Checked)
                    GetInfosDg5(); // As per ISO 18013 spec: Signature / usual mark image
                if ((formOptions.listView2.Items.Find("listViewItemDG10", true))[0].Checked)
                    GetInfosDg10(); // As per ISO 18013 spec: Reserved for future
                if ((formOptions.listView2.Items.Find("listViewItemDG11", true))[0].Checked)
                    GetInfosDg11(); // As per ISO 18013 spec: Domestic data
                if ((formOptions.listView2.Items.Find("listViewItemDG12", true))[0].Checked)
                    GetInfosDg12(); // As per ISO 18013 spec: Non-match alert

            }
        }
        private void GetInfoProtectedDgs()
        {
            if (!IsAppletTypeIdl())
            {
                if ((formOptions.listView1.Items.Find("listViewItemDG3", true))[0].Checked)
                    GetInfosDg3(); // As per ICAO spec: Additional Identification Feature — Finger(s)
                if ((formOptions.listView1.Items.Find("listViewItemDG4", true))[0].Checked)
                    GetInfosDg4(); // As per ICAO spec: Additional Identification Feature — Iris(es)
            }
            else
            {
                if ((formOptions.listView2.Items.Find("listViewItemDG7", true))[0].Checked)
                    GetInfosDg7(); // As per ISO 18013 spec: Finger biometric template
                if ((formOptions.listView2.Items.Find("listViewItemDG8", true))[0].Checked)
                    GetInfosDg8(); // As per ISO 18013 spec: Iris biometric template
                if ((formOptions.listView2.Items.Find("listViewItemDG9", true))[0].Checked)
                    GetInfosDg9(); // As per ISO 18013 spec: Other biometric template
            }
        }

        private void GetInfosDg1()
        {
            PrintListView("Reading DG1...");

            sw.Restart();
            int iRes = elyMrtd.readDG1();
            sw.Stop();
            Console.WriteLine("DG1 read time: " + GetElapsedTimeAsString(sw.Elapsed));

            if (iRes >= 0)
            {
                // Enable the following code to parse DG1 data
            #if false
                if (!IsDocTypeD())
                {
                    ElyMRTDDotNet.elyMrzParser ElyMrzParserDg1 = new ElyMRTDDotNet.elyMrzParser();
                    ElyMrzParserDg1.Parse(elyMrtd.getMRZString());
                    if (!ElyMrzParserDg1.IsParseSuccess())
                        PrintListViewError("Invalid MRZ from EF.DG1");
                }
            #endif
                dg1 = GatherDg1FieldsFromMrtdObject();
                PrintListView("DG1 Read.");
            }
            else if (iRes == (int)DG_RETURN_CODES.DG_FILE_NOT_FOUND)
                PrintListViewWarning("File not found");
            else
            {
                PrintListViewError("Error while reading DG1.");
                throw new Exception("Unable to read DG1");
            }

            switch (nAppletType)
            {
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_PACE:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_BAC:
                    if (iRes >= 0) {
                        // Enable the following code to validate MRZ (from Scanner) with DG1
                    #if true
                        ValidateMrzWithDG1();
                    #endif
                    }
                    UpdatePictureBoxesDg(this.pictureBoxDG1, iRes);
                    break;
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL_EU:
                    UpdatePictureBoxesDg(this.pictureBoxDG1_BAP, iRes);
                    break;
            }
        }

        private void GetInfosDg2()
        {
            PrintListView("Reading DG2...");

            sw.Restart();
            int iRes = elyMrtd.readDG2();
            sw.Stop();
            Console.WriteLine("DG2 read time: " + GetElapsedTimeAsString(sw.Elapsed));
            AddTimeInfoToListView("DG2", elyMrtd.getDG2().Length, GetElapsedMillisAsLong(sw.Elapsed));

            switch (nAppletType)
            {
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_PACE:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_BAC:
                    if (iRes >= 0)
                    {
                        try
                        {
                            sw.Restart();
                            byte[] photoArray = elyMrtd.getPhoto();
                            sw.Stop();
                            Console.WriteLine("Image decoded: " + GetElapsedTimeAsString(sw.Elapsed));

                            bmpPortrait = null;
                            if (photoArray != null)
                                bmpPortrait = GetBitmapFromByteArray(photoArray);
                            if (bmpPortrait == null)
                                iRes = -1;
                            SaveJpgImage(bmpPortrait, GetFolderPathToStoreImages(), "photo.jpg");
                        }
                        catch (Exception)
                        {
                            PrintListViewError("Error while decoding picture.");
                            throw new Exception("Error while decoding picture");
                        }
                    }
                    UpdatePictureBoxesDg(this.pictureBoxDG2, iRes);
                    break;
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL_EU:
                    UpdatePictureBoxesDg(this.pictureBoxDG2_BAP, iRes);
                    break;
            }

            if (iRes >= 0)
                PrintListView("DG2 Read.");
            else if (iRes == (int)DG_RETURN_CODES.DG_FILE_NOT_FOUND)
                PrintListViewWarning("File not found");
            else
            {
                PrintListViewError("Error while reading DG2.");
                if (!formOptions.comboBoxApduType.Text.Equals("Short"))
                    ShowMessageBoxToUseShortApdu();
            }
        }

        private void GetInfosDg3()
        {
            if (!IsDgPresentInEfCom(3)) { return; }

            PrintListView("Reading DG3...");

            sw.Restart();
            int iRes = elyMrtd.readDG3();
            sw.Stop();
            Console.WriteLine("DG3 read time: " + GetElapsedTimeAsString(sw.Elapsed));
            AddTimeInfoToListView("DG3", elyMrtd.getDG3().Length, GetElapsedMillisAsLong(sw.Elapsed));

            switch (nAppletType)
            {
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_PACE:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_BAC:
                    if (iRes >= 0)
                    {
                        ElyMRTDDotNet.BiometricTemplate[] templates = elyMrtd.getFingersBiometricsTemplates();
                        lbmpBiometricTemplates = new List<Bitmap>();
                        ExtractTemplateImagesAndDisplay(templates, lbmpBiometricTemplates, "finger");
                    }
                    UpdatePictureBoxesDg(this.pictureBoxDG3, iRes);
                    break;
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL_EU:
                    UpdatePictureBoxesDg(this.pictureBoxDG3_BAP, iRes);
                    break;
            }

            if (iRes >= 0)
                PrintListView("DG3 Read.");
            else if (iRes == (int)DG_RETURN_CODES.DG_FILE_NOT_FOUND)
                PrintListViewWarning("File not found");
            else
                PrintListViewError("Error while reading DG3.");
        }

        private void GetInfosDg4()
        {
            if (!IsDgPresentInEfCom(4)) { return; }

            PrintListView("Reading DG4...");

            sw.Restart();
            int iRes = elyMrtd.readDG4();
            sw.Stop();
            Console.WriteLine("DG4 read time: " + GetElapsedTimeAsString(sw.Elapsed));
            AddTimeInfoToListView("DG4", elyMrtd.getDG4().Length, GetElapsedMillisAsLong(sw.Elapsed));

            switch (nAppletType)
            {
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_PACE:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_BAC:
                    if (iRes >= 0)
                    {
                        ElyMRTDDotNet.BiometricTemplate[] templates = elyMrtd.getIrisBiometricsTemplates();
                        lbmpIrisBiometricTemplates = new List<Bitmap>();
                        ExtractTemplateImagesAndDisplay(templates, lbmpIrisBiometricTemplates, "iris");
                    }
                    UpdatePictureBoxesDg(this.pictureBoxDG4, iRes);
                    break;
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL_EU:
                    if (iRes >= 0)
                    {
                        try
                        {
                            sw.Restart();
                            byte[] photoArray = elyMrtd.getPhoto();
                            sw.Stop();
                            Console.WriteLine("Image decoded: " + GetElapsedTimeAsString(sw.Elapsed));

                            bmpPortrait = null;
                            if (photoArray != null)
                                bmpPortrait = GetBitmapFromByteArray(photoArray);
                            if (bmpPortrait == null)
                                iRes = -1;
                            SaveJpgImage(bmpPortrait, GetFolderPathToStoreImages(), "photo.jpg");
                        }
                        catch (Exception)
                        {
                            PrintListViewError("Error while decoding picture.");
                            //throw new Exception("Error while decoding picture");
                        }
                    }
                    UpdatePictureBoxesDg(this.pictureBoxDG4_BAP, iRes);
                    break;
            }

            if (iRes >= 0)
                PrintListView("DG4 Read.");
            else if (iRes == (int)DG_RETURN_CODES.DG_FILE_NOT_FOUND)
                PrintListViewWarning("File not found");
            else
                PrintListViewWarning("DG4 could not be read.");
        }

        private void GetInfosDg5()
        {
            if (!IsDgPresentInEfCom(5)) { return; }

            PrintListView("Reading DG5...");

            int iRes = elyMrtd.readDG5();

            switch (nAppletType)
            {
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_PACE:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_BAC:
                    UpdatePictureBoxesDg(this.pictureBoxDG5, iRes);
                    break;
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL_EU:
                    if (iRes >= 0)
                    {
                        do
                        {
                            sw.Restart();
                            byte[] signatureArray = elyMrtd.getSignature();
                            if (signatureArray == null)
                                break;

                            try
                            {
                                System.IO.MemoryStream ms = new System.IO.MemoryStream(signatureArray);
                                FreeImageAPI.FreeImageBitmap photo = new FreeImageAPI.FreeImageBitmap(ms);

                                bmpSignature = new Bitmap(photo.ToBitmap());

                                if (photo != null) photo.Dispose();
                                if (ms != null) ms.Dispose();

                                sw.Stop();
                                Console.WriteLine("Image decoded: " + GetElapsedTimeAsString(sw.Elapsed));
                                SaveJpgImage(bmpSignature, GetFolderPathToStoreImages(), "signature.jpg");
                            }
                            catch (Exception)
                            {
                                PrintListViewError("Error while decoding picture.");
                                //throw new Exception("Error while decoding picture");
                            }

                        } while (false);
                    }
                    UpdatePictureBoxesDg(this.pictureBoxDG5_BAP, iRes);
                    break;
            }

            if (iRes >= 0)
                PrintListView("DG5 Read.");
            else if (iRes == (int)DG_RETURN_CODES.DG_FILE_NOT_FOUND)
                PrintListViewWarning("File not found");
            else
                PrintListViewError("Error while reading DG5.");
        }

        private void GetInfosDg6()
        {
            if (!IsDgPresentInEfCom(6)) { return; }

            PrintListView("Reading DG6...");

            int iRes = elyMrtd.readDG6();

            switch (nAppletType)
            {
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_PACE:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_BAC:
                    UpdatePictureBoxesDg(this.pictureBoxDG6, iRes);
                    break;
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL_EU:
                    if (iRes >= 0)
                    {
                        try
                        {
                            sw.Restart();
                            byte[] photoArray = elyMrtd.getPhoto();
                            sw.Stop();
                            Console.WriteLine("Image decoded: " + GetElapsedTimeAsString(sw.Elapsed));

                            bmpPortrait = null;
                            if (photoArray != null)
                                bmpPortrait = GetBitmapFromByteArray(photoArray);
                            if (bmpPortrait == null)
                                iRes = -1;
                            SaveJpgImage(bmpPortrait, GetFolderPathToStoreImages(), "photo.jpg");
                        }
                        catch (Exception)
                        {
                            PrintListViewError("Error while decoding picture.");
                            //throw new Exception("Error while decoding picture");
                        }
                    }
                    UpdatePictureBoxesDg(this.pictureBoxDG6_BAP, iRes);
                    break;
            }

            if (iRes >= 0)
                PrintListView("DG6 Read.");
            else if (iRes == (int)DG_RETURN_CODES.DG_FILE_NOT_FOUND)
                PrintListViewWarning("File not found");
            else
                PrintListViewError("Error while reading DG6.");
        }

        private void GetInfosDg7()
        {
            if (!IsDgPresentInEfCom(7)) { return; }

            PrintListView("Reading DG7...");

            int iRes = elyMrtd.readDG7();

            switch (nAppletType)
            {
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_PACE:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_BAC:
                    if (iRes >= 0)
                    {
                        do
                        {
                            sw.Restart();
                            byte[] signatureArray = elyMrtd.getSignature();
                            if (signatureArray == null)
                                break;

                            try
                            {
                                System.IO.MemoryStream ms = new System.IO.MemoryStream(signatureArray);
                                FreeImageAPI.FreeImageBitmap photo = new FreeImageAPI.FreeImageBitmap(ms);

                                bmpSignature = new Bitmap(photo.ToBitmap());

                                if (photo != null) photo.Dispose();
                                if (ms != null) ms.Dispose();

                                sw.Stop();
                                Console.WriteLine("Image decoded: " + GetElapsedTimeAsString(sw.Elapsed));
                                SaveJpgImage(bmpSignature, GetFolderPathToStoreImages(), "signature.jpg");
                            }
                            catch (Exception)
                            {
                                PrintListViewError("Error while decoding picture.");
                                //throw new Exception("Error while decoding picture");
                            }

                        } while (false);
                    }
                    UpdatePictureBoxesDg(this.pictureBoxDG7, iRes);
                    break;
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL_EU:
                    UpdatePictureBoxesDg(this.pictureBoxDG7_BAP, iRes);
                    break;
            }

            if (iRes >= 0)
                PrintListView("DG7 Read.");
            else if (iRes == (int)DG_RETURN_CODES.DG_FILE_NOT_FOUND)
                PrintListViewWarning("File not found");
            else
                PrintListViewError("Error while reading DG7.");
        }

        private void GetInfosDg8()
        {
            if (!IsDgPresentInEfCom(8)) { return; }

            PrintListView("Reading DG8...");

            int iRes = elyMrtd.readDG8();

            switch (nAppletType)
            {
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_PACE:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_BAC:
                    UpdatePictureBoxesDg(this.pictureBoxDG8, iRes);
                    break;
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL_EU:
                    UpdatePictureBoxesDg(this.pictureBoxDG8_BAP, iRes);
                    break;
            }

            if (iRes >= 0)
                PrintListView("DG8 Read.");
            else if (iRes == (int)DG_RETURN_CODES.DG_FILE_NOT_FOUND)
                PrintListViewWarning("File not found");
            else
                PrintListViewError("Error while reading DG8.");
        }

        private void GetInfosDg9()
        {
            if (!IsDgPresentInEfCom(9)) { return; }

            PrintListView("Reading DG9...");

            int iRes = elyMrtd.readDG9();

            switch (nAppletType)
            {
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_PACE:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_BAC:
                    UpdatePictureBoxesDg(this.pictureBoxDG9, iRes);
                    break;
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL_EU:
                    UpdatePictureBoxesDg(this.pictureBoxDG9_BAP, iRes);
                    break;
            }

            if (iRes >= 0)
                PrintListView("DG9 Read.");
            else if (iRes == (int)DG_RETURN_CODES.DG_FILE_NOT_FOUND)
                PrintListViewWarning("File not found");
            else
                PrintListViewError("Error while reading DG9.");
        }

        private void GetInfosDg10()
        {
            if (!IsDgPresentInEfCom(10)) { return; }

            PrintListView("Reading DG10...");

            int iRes = elyMrtd.readDG10();

            switch (nAppletType)
            {
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_PACE:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_BAC:
                    UpdatePictureBoxesDg(this.pictureBoxDG10, iRes);
                    break;
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL_EU:
                    UpdatePictureBoxesDg(this.pictureBoxDG10_BAP, iRes);
                    break;
            }

            if (iRes >= 0)
                PrintListView("DG10 Read.");
            else if (iRes == (int)DG_RETURN_CODES.DG_FILE_NOT_FOUND)
                PrintListViewWarning("File not found");
            else
                PrintListViewError("Error while reading DG10.");
        }

        private void GetInfosDg11()
        {
            if (!IsDgPresentInEfCom(11)) { return; }

            PrintListView("Reading DG11...");

            int iRes = elyMrtd.readDG11();

            switch (nAppletType)
            {
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_PACE:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_BAC:
                    if (iRes >= 0)
                    {
                        dg11 = new List<string>
                        {
                            elyMrtd.getFullName(),
                            elyMrtd.getPersonalNumberDg11(),
                            elyMrtd.getFullBirthDate(),
                            elyMrtd.getBirthPlace(),
                            elyMrtd.getResidence(),
                            elyMrtd.getPhoneNumber(),
                            elyMrtd.getProfession(),
                            elyMrtd.getTitle(),
                            elyMrtd.getPersonalSummary(),
                            elyMrtd.getOtherValidTdNumbers(),
                            elyMrtd.getCustodyInfo()
                        };
                    }
                    UpdatePictureBoxesDg(this.pictureBoxDG11, iRes);
                    break;
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL_EU:
                    UpdatePictureBoxesDg(this.pictureBoxDG11_BAP, iRes);
                    break;
            }

            if (iRes >= 0)
                PrintListView("DG11 Read.");
            else if (iRes == (int)DG_RETURN_CODES.DG_FILE_NOT_FOUND)
                PrintListViewWarning("File not found");
            else
                PrintListViewError("Error while reading DG11.");
        }

        private void GetInfosDg12()
        {
            if (!IsDgPresentInEfCom(12)) { return; }

            PrintListView("Reading DG12...");

            int iRes = elyMrtd.readDG12();

            switch (nAppletType)
            {
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_PACE:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_BAC:
                    if (iRes >= 0)
                    {
                        dg12 = new List<string>
                        {
                            elyMrtd.getIssuingAuthority(),
                            elyMrtd.getDeliveryDate(),
                            elyMrtd.getEndorsementsAndObservations(),
                            elyMrtd.getTaxExitRequirements(),
                            elyMrtd.getDocPersoDateAndTime(),
                            elyMrtd.getSerNumOfPersoSystem()
                        };
                    }
                    UpdatePictureBoxesDg(this.pictureBoxDG12, iRes);
                    break;
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL_EU:
                    UpdatePictureBoxesDg(this.pictureBoxDG12_BAP, iRes);
                    break;
            }

            if (iRes >= 0)
                PrintListView("DG12 Read.");
            else if (iRes == (int)DG_RETURN_CODES.DG_FILE_NOT_FOUND)
                PrintListViewWarning("File not found");
            else
                PrintListViewError("Error while reading DG12.");
        }

        private void GetInfosDg13()
        {
            if (!IsDgPresentInEfCom(13)) { return; }

            PrintListView("Reading DG13...");

            int iRes = elyMrtd.readDG13();

            switch (nAppletType)
            {
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_PACE:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_BAC:
                    if (iRes >= 0)
                    {
                        byte[] dg13 = elyMrtd.getDG13();
                        Console.WriteLine("DG13 data");
                        Console.WriteLine(ByteArrayToStringFormat(dg13));

                        // BER TLV format: 0x6D, <Len>, <Val>
                        // As per ICAO 9303 Part10, Data Elements combining to form DG13 are at the discretion of the issuing State or organization

                        // parse dg13
                    }
                    UpdatePictureBoxesDg(this.pictureBoxDG13, iRes);
                    break;
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.IDL_EU:
                    UpdatePictureBoxesDg(this.pictureBoxDG13_BAP, iRes);
                    break;
            }

            if (iRes >= 0)
                PrintListView("DG13 Read.");
            else if (iRes == (int)DG_RETURN_CODES.DG_FILE_NOT_FOUND)
                PrintListViewWarning("File not found");
            else
                PrintListViewError("Error while reading DG13.");
        }

        private void GetInfosDg16()
        {
            if (!IsDgPresentInEfCom(16)) { return; }

            int iRes = -1;
            switch (nAppletType)
            {
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_PACE:
                case (int)ElyMRTDDotNet.ElyMRTDDotNet.APPLET_TYPES.ICAO_BAC:
                    PrintListView("Reading DG16...");
                    iRes = elyMrtd.readDG16();
                    if (iRes >= 0)
                    {
                        byte[] dg16 = elyMrtd.getDG16();
                        Console.WriteLine("DG16 data");
                        Console.WriteLine(ByteArrayToStringFormat(dg16));

                        // BER TLV format: 0x70, <Len>, <Val>
                        // As per ICAO 9303 Part10, Data Elements in DG16 will contain the person(s) to notify in case of emergency

                        // T      L      V
                        // ---------------
                        // 70     var
                        //               T      L      V
                        //               ---------------
                        //               02     01     Number of templates (occurs only in first template)
                        //               Ax     var    Start of template, where x (x = 1,2,3...) increments for each occurence
                        // 5F50   08                   Date data recorded
                        // 5F51   var                  Name of person
                        // 5F52   var                  Telephone
                        // 5F53   var                  Address

                        // parse dg16
                    }
                    UpdatePictureBoxesDg(this.pictureBoxDG16, iRes);
                    break;
                default:
                    // DG 16 Not Available in DL as per 18013-2E
                    return;
            }

            if (iRes >= 0)
                PrintListView("DG16 Read.");
            else if (iRes == (int)DG_RETURN_CODES.DG_FILE_NOT_FOUND)
                PrintListViewWarning("File not found");
            else
                PrintListViewError("Error while reading DG16.");
        }

        private void GetInfosEfCardAccess()
        {
            PrintListView("Reading EF.CardAccess...");

            byte[] efCardAccess = elyMrtd.getEFCardAccess();
            if (efCardAccess != null)
            //if (elyMrtd.readEF_CardAccess())
            {
                PrintListView("EF.CardAccess Read.");
            }
            else
            {
                //throw new Exception("Unable to read EF.CardAccess");
                PrintListViewError("Unable to read EF.CardAccess");
            }
        }

        private bool GetDgListFromEfCom()
        {
            if (dgList != null)
            {
                dgList.Clear();
            }
            byte[] efCom = elyMrtd.getEFCom();

            if (efCom != null)
            {
                ArrayList list = new ArrayList(efCom);
                // DG list avaliable from tag 0x5C
                if (list != null && list.Contains((byte)0x5C))
                {
                    int index = list.IndexOf((byte)0x5C);
                    // Get length of DG list 
                    int len = (byte)list[index + 1];
                    // Get DG list
                    dgList = list.GetRange(index + 2, len);
                    return true;
                }
            }
            return false;
        }

        private bool GetInfosEfCom()
        {
            PrintListView("Reading EF.COM...");

            if (elyMrtd.readEF_COM())
            {
                PrintListView("EF.COM Read.");
                if (GetDgListFromEfCom()) { return true; }
                PrintListView("Unable to get DG list from EF.COM");
            }
            else
            {
                //throw new Exception("Unable to read EF.COM");
                PrintListView("Unable to read EF.COM");
            }
            return false;
        }

        private void GetInfosEfSod()
        {
            PrintListView("Reading EF.SOD...");

            if (elyMrtd.readEF_SOd())
            {
                PrintListView("EF.SOD Read.");
            }
            else
            {
                //throw new Exception("Unable to read EF.SOD");
                PrintListViewError("Unable to read EF.SOD");
            }
        }
        #endregion

        #endregion


        #region Event helpers
        private string[] ReestablishContextAndGetScardReaderList()
        {
            string[] szReaders = { "" };
            if (m_iResMan != null)
                m_iResMan.ReleaseContext();

            m_iResMan = new SCardManager();

            try {
                if (m_iResMan != null)
                    m_iResMan.EstablishContext(SCOPE.User);
                szReaders = m_iResMan.ListReaders();
            } catch (Exception) {
            }
            return szReaders;
        }

        private string[] GetScardReaders()
        {
            string[] szReaderList = ReestablishContextAndGetScardReaderList();
            string[] szReader = new string[szReaderList.Length];
            int i = 0;
            foreach (string reader in szReaderList)
            {
                if (IsElyctisReader(reader))
                    szReader[i++] = reader;
            }
            return szReader;
        }

        private void DisplayScardReaderDetails(string szReader)
        {
            try
            {
                if (!szReader.Equals(""))
                {
                    if (IsElyctisMultiSlotReader(szReader))
                    {
                        this.labelContactlessDriver.Text = "Driver CTL Proprietary";
                        this.labelContactlessFwVersion.Text = GetContactlessFwMfrString(szReader, true);
                    }
                    else if (IsElyctisIdReader(szReader))
                    {
                        this.labelContactlessDriver.Text = "Driver CTL Microsoft";
                        this.labelContactlessFwVersion.Text = GetContactlessFwMfrString(szReader, false);
                    }
                    else
                        Console.WriteLine("Error detecting the connected reader");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error while creating connection with the reader: " + exc.Message);
                this.labelContactlessFwVersion.Text = String.Format("FW CTL Unknown");
            }
        }

        private void GetAndDisplayScardReaderDetails()
        {
            if (comboBoxCcidReaders.Text.Equals(""))
                UpdateComboBoxCcidReaders();
            else
            {
                szScardReader = GetScardReaders().Contains(comboBoxCcidReaders.Text) ?
                    comboBoxCcidReaders.Text :
                    comboBoxCcidReaders.GetItemText(0);
                DisplayScardReaderDetails(szScardReader);
            }
        }

        private string GetComPortNameFromComboBox()
        {
            return comboBoxComPort.Text.EndsWith(" - ID BOX") ?
                    comboBoxComPort.Text.Replace(" - ID BOX", "") :
                    comboBoxComPort.Text;
        }

        private void GetAndDisplayScannerDetails()
        {
            if (this.comboBoxComPort.Text != string.Empty)
            {
                try
                {
                    this.m_scanner.Disconnect();
                    m_scanner.Connect(GetComPortNameFromComboBox());
                    Console.WriteLine("m_scanner.IsConnected(): " + m_scanner.IsConnected().ToString());
                    if (m_scanner.IsConnected())
                    {
                        try
                        {
                            string version = m_scanner.GetVersion();
                            if (!String.IsNullOrEmpty(version))
                            {
                                labelScannerFwVersion.Text = "FW OCR " + version;
                                if (Convert.ToDouble(version.Substring(0, 4).Replace(".", ",")) > 2.53)
                                {
                                    string serial = m_scanner.GetSerialNumber();
                                    labelIdBoxSerialNumber.Text = (!String.IsNullOrEmpty(serial)) ?
                                        ("S/N " + serial) : ("S/N Unknown");

                                    string prod_info = m_scanner.GetProductInformation();
                                    labelIdBoxProductNumber.Text = (!String.IsNullOrEmpty(prod_info)) ?
                                        ("P/N " + prod_info) : ("P/N Unknown");
                                }
                            }
                            else
                                labelScannerFwVersion.Text = "FW OCR Unknown";

                            string nnaVersion = m_scanner.GetNnaVersion();
                            labelScannerNnaVersion.Text = (!String.IsNullOrEmpty(nnaVersion)) ?
                                ("NNA " + nnaVersion[0] + "." + nnaVersion[1]) : ("NNA Unknown");
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine("Error: %s" + exc.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine("COM Port is in use");
                        ShowMessageBoxComPortUnavailable();
                        return;
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Error: %s" + exc.Message);
                }

                if (m_scanner.EnableContinuousReading != this.formOptions.checkBoxAutoDetect.Checked)
                {
                    m_scanner.EnableContinuousReading = this.formOptions.checkBoxAutoDetect.Checked;
                    if (!m_scanner.EnableContinuousReading && m_scanner.IsConnected())
                        this.m_scanner.Disconnect();
                }
            }
        }

        private void GetDeviceDetails()
        {
            ClearDeviceVersionLabels();
            buttonRead.Enabled = false;
            GetAndDisplayScannerDetails();
            GetAndDisplayScardReaderDetails();
            UpdateReadButtonState();
        }

        private bool UpdateComboBoxComPort()
        {
            // Search for desired COM ports
            comboBoxComPort.Items.Clear();
            Dictionary<string, string> friendlyPorts = BuildPortNameHash(SerialPort.GetPortNames());
            int[] anComboBoxIndexIdBox = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int i = 0, nFound = 0;
            foreach (KeyValuePair<string, string> kvp in friendlyPorts)
            {
                if (IsElyctisMrzScanner(kvp)) {
                    nFound++;
                    comboBoxComPort.Items.Add(kvp.Key + " - ID BOX");
                    anComboBoxIndexIdBox[i++] = comboBoxComPort.Items.Count - 1;
                }
                else
                    comboBoxComPort.Items.Add(kvp.Key);
            }
            // If found, select its index, otherwise select nothing (-1)
            m_nComPortIndex = (nFound > 0) ? anComboBoxIndexIdBox[0] : (nFound - 1);

            return (nFound > 0);
        }

        private bool UpdateComboBoxCcidReaders()
        {
            bool bFound = false;

            // Store current selected reader name
            string szReaderTemp = comboBoxCcidReaders.Text;
            string[] szReaderList = GetScardReaders();

            // Search for available readers
            comboBoxCcidReaders.Items.Clear();
            foreach (string szReader in szReaderList)
            {
                if (szReader != null)
                {
                    bFound = true;
                    comboBoxCcidReaders.Items.Add(szReader);
                }
            }
            // If found, select its index, otherwise select nothing (-1)
            if (bFound) {
                if ((!String.IsNullOrEmpty(szReaderTemp)) && comboBoxCcidReaders.Items.Contains(szReaderTemp))
                    comboBoxCcidReaders.Text = szReaderTemp; // Retain the previous name
                else
                    comboBoxCcidReaders.SelectedIndex = 0; // Triggers SelectedIndexChanged event
            } else {
                labelContactlessDriver.Text = "";
                labelContactlessFwVersion.Text = "";
                comboBoxCcidReaders.SelectedIndex = -1; // Triggers SelectedIndexChanged event
            }
            if (bFound)
                mszReaderName = szReaderTemp;
            return bFound;
        }

        private void UpdateReadButtonState()
        {
            try
            {
                if ((!String.IsNullOrEmpty(comboBoxComPort.Text)) && (m_scanner != null))
                {
                    if (this.formOptions.checkBoxAutoDetect.Checked && !m_scanner.IsConnected())
                        m_scanner.Connect(GetComPortNameFromComboBox());

                    if (m_scanner.IsConnected())
                        m_scanner.EnableContinuousReading = this.formOptions.checkBoxAutoDetect.Checked;

                    buttonRead.Enabled = !labelScannerFwVersion.Text.Equals("FW OCR Unknown") && !m_scanner.EnableContinuousReading;
                    if (m_scanner.EnableContinuousReading && !m_scanner.IsConnected())
                        m_scanner.Connect(GetComPortNameFromComboBox());

                    if (!buttonRead.Enabled && IsReadingOptionsDoesNotRequireMrzScanner() && (comboBoxCcidReaders.Text != "") && (m_iResMan != null))
                        buttonRead.Enabled = true;
                }
                else if (IsReadingOptionsDoesNotRequireMrzScanner() && (comboBoxCcidReaders.Text != "") && (m_iResMan != null))
                    buttonRead.Enabled = true;
                else
                    buttonRead.Enabled = false;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error: %s" + exc.Message);
            }
        }

        private void UpdateAntennaSelectionView()
        {
            try
            {
                if (!szScardReader.Equals(""))
                {
                    m_iCard = m_iResMan.CreateConnection(szScardReader);
                    if (m_iCard != null)
                    {
                        byte[] byCApdu;
                        byte[] byRApdu = new byte[6];
                        byte byGpio = (byte)(IsElyctisIdReader(szScardReader) ? 0x07 : 0x15);
                        m_iCard.Connect(SHARE.Shared, PROTOCOL.T0orT1);
                        byCApdu = new byte[] { 0xFF, 0x00, 0x00, 0x00, 0x02, 0x1B, byGpio };
                        byRApdu = m_iCard.Transmit(byCApdu, (uint)byCApdu.Length);
                        m_iCard.Disconnect(DISCONNECT.Reset);
                        m_iCard = null;

                        if (byRApdu[0] == 0)
                        {
                            PrintListView("Front antenna selected.");
                            UpdatePictureBox(this.pictureBoxFrontAntenna, true);
                        }
                        else if (byRApdu[0] == 1)
                        {
                            PrintListView("Back antenna selected.");
                            UpdatePictureBox(this.pictureBoxBackAntenna, true);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error while getting antenna details: " + exc.Message);
            }
        }

        private void CleanupSession()
        {
            try
            {
                if (!szScardReader.Equals("") && !szScardReader.Equals("0"))
                {
                    m_iCard = m_iResMan.CreateConnection(szScardReader);
                    if (m_iCard != null)
                    {
                        m_iCard.Connect(SHARE.Shared, PROTOCOL.T0orT1);
                        m_iCard.Disconnect(DISCONNECT.Reset);
                        m_iCard = null;
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error while clearing session: " + exc.Message);
            }
        }
        #endregion


        #region Event triggers
        private void buttonRead_MouseEnter(object sender, EventArgs e)
        {
            if (!formOptions.checkBoxAutoDetect.Checked)
                ShowToolTipToClickToRead(buttonRead);
        }

        private void buttonRead_MouseLeave(object sender, EventArgs e)
        {
            ShowToolTipClear();
        }

        private void buttonRead_Click(object sender, EventArgs e)
        {
            ClearFormGuiControls();
            buttonRead.Enabled = false;

            // Set initial access control option
            if (!formOptions.rbAccessControlAuto.Checked
                && !formOptions.rbAccessControlBAC.Checked
                && !formOptions.rbAccessControlPACE.Checked)
            {
                formOptions.rbAccessControlBAC.Checked = true;
            }
            // Set initial password option
            if (!formOptions.rbPasswordTypeAskPwd.Checked
                && !formOptions.rbPasswordTypeCAN.Checked
                && !formOptions.rbPasswordTypeMRZ.Checked)
            {
                formOptions.rbPasswordTypeMRZ.Checked = true;
            }

            // PACE-CAN based access
            if (!formOptions.rbAccessControlBAC.Checked
                && !formOptions.rbPasswordTypeAskPwd.Checked
                && !formOptions.rbPasswordTypeMRZ.Checked)
            {
                ReadDocument();
            }
            // Manual MRZ is selected
            else if (formOptions.checkBoxManualMRZ.Checked)
            {
                this.buttonLastLog.Enabled = false;
                formMRZ = new FormMRZ();
                DialogResult dRes = formMRZ.ShowDialog();
                if (dRes != System.Windows.Forms.DialogResult.Yes)
                {
                    buttonRead.Enabled = true;
                    return;
                }

                szMrzPwd = formMRZ.GetMrzInfo();
                ReadDocument();
            }
            // MRZ based access using Scanner
            else
            {
                if (!m_scanner.IsConnected())
                    m_scanner.Connect(GetComPortNameFromComboBox());

                if (m_scanner.IsConnected())
                {
                    swInquire.Restart();
                    PrintListView("Reading MRZ...");
                    m_scanner.Inquire();
                    return;
                }
            }
        }

        private void buttonOptions_MouseEnter(object sender, EventArgs e)
        {
            if (formOptions.checkBoxAutoDetect.Checked)
                ShowToolTipToDisableAutoDetect(buttonOptions);
        }

        private void buttonOptions_MouseLeave(object sender, EventArgs e)
        {
            ShowToolTipClear();
        }

        private void buttonOptions_Click(object sender, EventArgs e)
        {
            DialogResult res = formOptions.ShowDialog();
            if (res != System.Windows.Forms.DialogResult.OK)
                formOptions = new FormOptions();
            UpdateReadButtonState();
        }

        private void pictureBoxPicture_MouseEnter(object sender, EventArgs e)
        {
            if ((lbmpBiometricTemplates != null && lbmpBiometricTemplates.Count > 0)
                || (lbmpIrisBiometricTemplates != null && lbmpIrisBiometricTemplates.Count > 0)
                || (bmpSignature != null))
                EnablePortraitNavigationButtons(true);
        }

        private void pictureBoxPicture_MouseLeave(object sender, EventArgs e)
        {
            if (bDisableImageNavigationButtons)
            {
                EnablePortraitNavigationButtons(false);
                bDisableImageNavigationButtons = false;
            }
        }

        private void pictureBoxPictureIdl_MouseEnter(object sender, EventArgs e)
        {
            if ((lbmpBiometricTemplates != null && lbmpBiometricTemplates.Count > 0)
                || (lbmpIrisBiometricTemplates != null && lbmpIrisBiometricTemplates.Count > 0)
                || (bmpSignature != null))
                EnablePortraitNavigationButtonsIdl(true);
        }

        private void pictureBoxPictureIdl_MouseLeave(object sender, EventArgs e)
        {
            if (bDisableImageNavigationButtons)
            {
                EnablePortraitNavigationButtonsIdl(false);
                bDisableImageNavigationButtons = false;
            }
        }

        private void buttonImage_MouseEnter(object sender, EventArgs e)
        {
            if ((lbmpBiometricTemplates != null && lbmpBiometricTemplates.Count > 0)
                || (lbmpIrisBiometricTemplates != null && lbmpIrisBiometricTemplates.Count > 0)
                || (bmpSignature != null))
                EnablePortraitNavigationButtons(true);
        }

        private void buttonImage_MouseLeave(object sender, EventArgs e)
        {
            bDisableImageNavigationButtons = true;
        }

        private void buttonImageIdl_MouseEnter(object sender, EventArgs e)
        {
            if ((lbmpBiometricTemplates != null && lbmpBiometricTemplates.Count > 0)
                || (lbmpIrisBiometricTemplates != null && lbmpIrisBiometricTemplates.Count > 0)
                || (bmpSignature != null))
                EnablePortraitNavigationButtonsIdl(true);
        }

        private void buttonImageIdl_MouseLeave(object sender, EventArgs e)
        {
            bDisableImageNavigationButtons = true;
        }

        private void MainForm_MouseEnter(object sender, EventArgs e)
        {
            EnablePortraitNavigationButtons(false);
        }

        private void tabPageID_MouseEnter(object sender, EventArgs e)
        {
            EnablePortraitNavigationButtons(false);
        }

        private void comboBoxComPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetDeviceDetails();
        }

        private void comboBoxComPort_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateComboBoxComPort();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error: %s" + exc.Message);
            }
        }

        private void comboBoxCcidReaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            szScardReader = comboBoxCcidReaders.Text;
            DisplayScardReaderDetails(szScardReader);
        }

        private void comboBoxCcidReaders_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateComboBoxCcidReaders();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error: %s" + exc.Message);
            }
        }

        private void buttonImageLeft_Click(object sender, EventArgs e)
        {
            NavigatePortrait(Navigate.Left, this.pictureBoxPicture);
        }

        private void buttonImageRight_Click(object sender, EventArgs e)
        {
            NavigatePortrait(Navigate.Right, this.pictureBoxPicture);
        }

        private void buttonImageLeftIdl_Click(object sender, EventArgs e)
        {
            NavigatePortrait(Navigate.Left, this.pictureBoxPictureIdl);
        }

        private void buttonImageRightIdl_Click(object sender, EventArgs e)
        {
            NavigatePortrait(Navigate.Right, this.pictureBoxPictureIdl);
        }

        private void buttonLastLog_Click(object sender, EventArgs e)
        {
            var fileName = Path.Combine(formOptions.GetLogPath(), "ELY_TRAVEL_DOC.html");
            if (File.Exists(fileName))
                OpenFile(fileName);
            else
                ShowMessageBoxHtmlFileUnavailable();
        }

        private void buttonUpdateAntenna_MouseEnter(object sender, EventArgs e)
        {
            ShowToolTipToClickToFindAntenna(buttonUpdateAntenna);
        }

        private void buttonUpdateAntenna_MouseLeave(object sender, EventArgs e)
        {
            ShowToolTipClear();
        }

        private void buttonUpdateAntenna_Click(object sender, EventArgs e)
        {
            ClearListViewLogs();
            ClearGroupBoxAntenna();
            if (IsReaderAndCardDetected())
            {
                try
                {
                    if (elyMrtd != null)
                    {
                        elyMrtd.connect(szScardReader);
                        byte gpio = (byte)(IsElyctisIdReader(szScardReader) ? 0x07 : 0x15);
                        byte[] iRes = elyMrtd.getSelectAntenna(gpio);
                        if (iRes[0] == 0)
                        {
                            PrintListView("Front antenna selected.");
                            UpdatePictureBox(this.pictureBoxFrontAntenna, true);
                        }
                        else if (iRes[0] == 1)
                        {
                            PrintListView("Back antenna selected.");
                            UpdatePictureBox(this.pictureBoxBackAntenna, true);
                        }
                    }
                }
                catch (Exception exc)
                {
                    PrintListView(exc.Message);
                }
                finally
                {
                    if (elyMrtd != null)
                        elyMrtd.disconnect();

                    PrintListView("Disconnected.");
                }
            }
        }

        private void buttonAceptar_Click(object sender, EventArgs e)
        {
            SavePersonalData();
            ClearPersonalDataForm();
            ClearPicture();
        }

        private void SavePersonalData()
        {
            var personalData = new PersonalDataDto
            {
                Name = textBoxName.Text,
                Surname = textBoxSurname.Text,
                BirthDate = textBoxBirthDate.Text,
                Nationality = textBoxNationality.Text,
                Sex = textBoxSex.Text,
                ExpiryDate = textBoxExpiryDate.Text,
                DocumentNumber = textBoxDocumentNumber.Text,
                DocumentType = textBoxDocumentType.Text,
                Issuer = textBoxIssuer.Text,
                OptionalData = textBoxOptionalData.Text
            };

            personalDataList.Add(personalData);
        }

        private void ClearPersonalDataForm()
        {
            textBoxName.Text = string.Empty;
            textBoxSurname.Text = string.Empty;
            textBoxBirthDate.Text = string.Empty;
            textBoxNationality.Text = string.Empty;
            textBoxSex.Text = string.Empty;
            textBoxExpiryDate.Text = string.Empty;
            textBoxDocumentNumber.Text = string.Empty;
            textBoxDocumentType.Text = string.Empty;
            textBoxIssuer.Text = string.Empty;
            textBoxOptionalData.Text = string.Empty;
        }

        private void ClearPicture()
        {
            pictureBoxPicture.Image = null;
        }

        private void ComboBoxFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedFields = comboBoxFields.CheckedItems.Cast<string>().ToList();
        }

        private void ButtonExportExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files|*.xlsx";
            saveFileDialog.Title = "Save an Excel File";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var filteredData = filterService.FilterPersonalData(personalDataList, selectedFields);
                excelExportService.ExportPersonalDataToExcel(filteredData, selectedFields, saveFileDialog.FileName);
                MessageBox.Show("Data exported successfully.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion


        #region eMRTD test
        CancellationTokenSource tokenSource = new CancellationTokenSource(); // Create a token source
        public void SetEmrtdTest(bool bStart)
        {
            if (ThreadEmrtdTest == null)
                ThreadEmrtdTest = new Thread(() => EmrtdTest(tokenSource.Token));

            if (ThreadEmrtdTest != null)
            {
                if (bStart)
                {
                    // Start eMRTD test
                    if (!ThreadEmrtdTest.IsAlive)
                        ThreadEmrtdTest.Start();
                }
                else
                {
                    // Stop eMRTD test
                    if (ThreadEmrtdTest.IsAlive)
                    {
                        //ThreadEmrtdTest.Abort();
                        tokenSource.Cancel(); // Request cancellation
                        ThreadEmrtdTest.Join(); // If you want to wait for cancellation, `Join` blocks the calling thread until the thread represented by this instance terminates
                        tokenSource.Dispose(); // Dispose the token source

                        ThreadEmrtdTest = null;
                    }
                }
            }
        }

        private void EmrtdTest(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested) // Check if the caller requested cancellation
                {
                    WaitForCardAndReadDocument(mbCardState);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }
        #endregion
    }

    public class PersonalDataDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string BirthDate { get; set; }
        public string Nationality { get; set; }
        public string Sex { get; set; }
        public string ExpiryDate { get; set; }
        public string DocumentNumber { get; set; }
        public string DocumentType { get; set; }
        public string Issuer { get; set; }
        public string OptionalData { get; set; }
    }
}
