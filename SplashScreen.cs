using System;
using System.Windows.Forms;

namespace ELY_TRAVEL_DOC
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SplashScreen
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "SplashScreen";
            this.Load += new System.EventHandler(this.SplashScreen_Load);
            this.ResumeLayout(false);

        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {
            // Add any initialization code here
        }
    }
}
