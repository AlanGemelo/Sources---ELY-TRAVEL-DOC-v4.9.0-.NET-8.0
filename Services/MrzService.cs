using ElyMRTDDotNet;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ELY_TRAVEL_DOC.Services
{
    public class MrzService
    {
        private readonly ElyMrzParser elyMrzParser;
        private readonly Stopwatch sw;
        private readonly Stopwatch swInquire;
        private string szMrzPwd;
        private const int MRZ_PWD_LEN_ICAO = 24;
        private const int MRZ_PWD_LEN_IDL = 28;

        public MrzService()
        {
            elyMrzParser = new ElyMrzParser();
            sw = new Stopwatch();
            swInquire = new Stopwatch();
        }

        public void ReadMrz(string szMrz, Action<string> printListView, Action displayMrzLinesInTextBox, Action updateReadButtonState)
        {
            swInquire.Stop();
            Console.WriteLine("MRZ read time: " + GetElapsedTimeAsString(swInquire.Elapsed));

            try
            {
                sw.Restart();
                elyMrzParser.Parse(PreProcessMrz(szMrz));
                if (!elyMrzParser.IsParseSuccess())
                    throw new Exception("Invalid MRZ (" + elyMrzParser.ValidationMessage() + ")");
                szMrzPwd = elyMrzParser.GetMrzPwd();
                sw.Stop();
                Console.WriteLine("MRZ Parsing: " + GetElapsedTimeAsString(sw.Elapsed));

                bool bIsReadDocumentValid = ValidateMrz(printListView);
                if (!bIsReadDocumentValid)
                    printListView("Document reading aborted");
                else
                    printListView("MRZ read successfully");
            }
            catch (Exception exc)
            {
                printListView("Error while reading/parsing MRZ: " + exc.Message);
                displayMrzLinesInTextBox();
            }
            finally
            {
                updateReadButtonState();
            }
        }

        private bool ValidateMrz(Action<string> printListView)
        {
            bool bIsReadDocumentValid = false;
            if (elyMrzParser.DocumentType() == null)
            {
                printListView("ERROR: Document type is null");
            }
            else
            {
                if (IsDocTypeD())
                {
                    if (szMrzPwd.Length != MRZ_PWD_LEN_IDL)
                    {
                        printListView("Warning: Invalid MRZ password");
                    }
                    else
                    {
                        if (elyMrzParser.IsCheckDigitOfOverAllMrzValid())
                        {
                            bIsReadDocumentValid = true;
                        }
                        else
                        {
                            printListView("Warning: Invalid MRZ checksum");
                        }
                    }
                }
                else
                {
                    if (szMrzPwd.Length < MRZ_PWD_LEN_ICAO)
                    {
                        printListView("Warning: Invalid MRZ password");
                    }
                    else
                    {
                        if (elyMrzParser.IsCheckDigitOfDocNumValid() &&
                            elyMrzParser.IsCheckDigitOfDobValid() &&
                            elyMrzParser.IsCheckDigitOfDoeValid())
                        {
                            bIsReadDocumentValid = true;
                        }
                        else
                        {
                            printListView("Warning: Invalid MRZ checksum");
                        }
                    }
                }
            }
            return bIsReadDocumentValid;
        }

        private string PreProcessMrz(string szMrz)
        {
            // Preprocess MRZ if necessary
            return szMrz;
        }

        private string GetElapsedTimeAsString(TimeSpan ts)
        {
            return string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        }

        private bool IsDocTypeD()
        {
            // Implement logic to determine if document type is D
            return false;
        }
    }
}
