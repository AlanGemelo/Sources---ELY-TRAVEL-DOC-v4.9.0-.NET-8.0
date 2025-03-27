using System;
using System.IO;
using System.Windows.Forms;
namespace ELY_TRAVEL_DOC
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MRZForm form = null;
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                if (File.Exists("ElyMRTDdotNet.dll")
                    && File.Exists("ElySCardDotNet.dll")
                    && File.Exists("FreeImage.Standard.dll")
                    && File.Exists("Wsq2Bmp.dll")
                    && File.Exists("LibUsbDotNet.LibUsbDotNet.dll"))
                {
                
                    Application.Run(form = new MRZForm("es"));
                }
                else
                {
                    MessageBox.Show("ERROR: One or more dll dependency missing");
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            finally
            {
                if (form != null)
                    form.dispose();
            }
        }
    }
}
