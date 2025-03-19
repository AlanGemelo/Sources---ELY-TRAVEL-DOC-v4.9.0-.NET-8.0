using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ELY_TRAVEL_DOC
{
    public partial class FormPasswordPACE : Form
    {
        #region Variables
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
        public Boolean bIsMRZSelected { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
        public Boolean bIsCANSelected { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
        public String szCAN { get; set; }
        #endregion


        #region Constructor
        public FormPasswordPACE(Boolean bIsCANOnlyMode = false)
        {
            InitializeComponent();
            bIsMRZSelected = false;
            bIsCANSelected = false;
            if (bIsCANOnlyMode)
            {
                rbPACEPasswordMRZ.Checked = rbPACEPasswordMRZ.Enabled = false;
                rbPACEPasswordCAN.Checked = rbPACEPasswordCAN.Enabled = true;
                textBoxCAN.Focus();
            }
        }
        #endregion


        #region Event helpers
        private Boolean ValidateForm()
        {
            if (rbPACEPasswordCAN.Checked)
            {
                if (textBoxCAN.Text.Equals(""))
                {
                    MessageBox.Show("ERROR: CAN value is empty!");
                    return false;
                }
            }
            if (rbPACEPasswordMRZ.Checked)
            {
                bIsMRZSelected = true;
            }
            else if (rbPACEPasswordCAN.Checked)
            {
                bIsCANSelected = true;
                szCAN = textBoxCAN.Text;
            }
            return true;
        }
        #endregion


        #region Event triggers
        private void textBoxCAN_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for an invalid character in the KeyDown event
            // Only Numbers & backspace should be handled
            // NOTE: Delete key is handled by default
            e.Handled = !(char.IsDigit(e.KeyChar) || e.KeyChar.Equals('\b'));
            if (!rbPACEPasswordCAN.Checked)
            {
                rbPACEPasswordCAN.Checked = true;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = (!ValidateForm()) ? DialogResult.None : DialogResult.Yes;
        }
        #endregion
    }
}
