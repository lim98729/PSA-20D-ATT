using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using DefineLibrary;
using System.Threading;

namespace PSA_Application
{
    public partial class FormOK : Form
    {
        public FormOK()
        {
            InitializeComponent();
        }
        public int number;
        public int left, top;
        public bool teachend = false;

        private void button1_Click(object sender, EventArgs e)
        {
            if (teachend == false) return;
            timer1.Enabled = false;
            this.Close();
        }

      

        private void FormOK_Load(object sender, EventArgs e)
        {
            this.Left = 650;
            this.Top = 500; 
            this.TopMost = true;
            timer1.Enabled = true;
            teachend = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            this.Enabled = false;
			halcon_region tmpRigion = new halcon_region();

            mc.hdc.cam.createBlob(number, mc.hdc.cam.epoxyBlob);
	
			
			teachend = true;
            this.Enabled = true;
        }

    }
}
