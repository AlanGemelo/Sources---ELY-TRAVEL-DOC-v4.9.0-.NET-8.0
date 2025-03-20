using ElyMRTDDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ELY_TRAVEL_DOC.Services
{
    public class ChipService
    {
        private readonly ElyMRTDDotNet.ElyMRTDDotNet elyMrtd;
        private readonly Stopwatch sw;
        private bool bIsChipInDocument;
        private int nAppletType;

        public ChipService()
        {
            elyMrtd = new ElyMRTDDotNet.ElyMRTDDotNet();
            sw = new Stopwatch();
            bIsChipInDocument = false; // Asignar valor inicial
        }

        public async Task ReadDocumentAsync(string szScardReader, string szMrzPwd, bool isDocTypeD, Action<string> printListView, Action updateReadButtonState)
        {
            try
            {
                if (bIsChipInDocument)
                {
                    printListView("Reading Document...");
                    sw.Restart();

                    nAppletType = await Task.Run(() => elyMrtd.connect(szScardReader));
                    if (nAppletType == -1)
                    {
                        printListView("Please check that an ICAO document is in the ID BOX");
                        return;
                    }

                    bool bStatus = await Task.Run(() => elyMrtd.establishBAC(szMrzPwd));
                    if (bStatus)
                    {
                        sw.Stop();
                        printListView("BAC established.");
                        printListView("Document read successfully.");
                    }
                    else
                    {
                        printListView("Can't establish BAC.");
                    }
                }
                else
                {
                    printListView("No chip detected.");
                }
            }
            catch (Exception exc)
            {
                printListView("Error while reading document: " + exc.Message);
            }
            finally
            {
                updateReadButtonState();
            }
        }
    }
}
