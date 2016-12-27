using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using HalconLibrary;
using DefineLibrary;

namespace HalconLibrary
{
    public partial class FormHalconTest : Form
    {
        public FormHalconTest()
        {
            InitializeComponent();
        }

        string s1, s2, s3, s4;
        bool b;

        private void Control_Click(object sender, EventArgs e)
        {
            if (sender.Equals(BT_AdvanceMode))
            {
                hWindow.ADVANCE_MODE = !hWindow.ADVANCE_MODE;
            }
            if (sender.Equals(BT_Exit))
            {
                if (hVision.isLive || hVision.isFind) return;
                EVENT.powerOff();

                hVision.cam1.deactivate(out b, out s1);
                hVision.cam2.deactivate(out b, out s2);
                hVision.cam3.deactivate(out b, out s3);
                hVision.cam4.deactivate(out b, out s4);

                hVision.cam1.clearWindow(); hVision.cam1.messageStatus(s1);
                hVision.cam2.clearWindow(); hVision.cam2.messageStatus(s2);
                hVision.cam3.clearWindow(); hVision.cam3.messageStatus(s3);
                hVision.cam4.clearWindow(); hVision.cam4.messageStatus(s4);

                if (hVision.cam1.isActivate || hVision.cam2.isActivate || hVision.cam3.isActivate || hVision.cam4.isActivate) return;
                this.Close();
            }


        }

        private void FormHalconTest_Load(object sender, EventArgs e)
        {
            hWindow.ADVANCE_MODE = true;
        }

    }
}
