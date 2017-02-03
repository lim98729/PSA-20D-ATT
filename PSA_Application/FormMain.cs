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
using AccessoryLibrary;
using System.IO;
using System.Globalization;
using System.Threading;

namespace PSA_Application
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            // 20140612
            logRemove();
            mc.user.logInUserName = "Operator";

            // 20140618
//  			mc.main.DoStart_Polling();
// 			mc.main.DoReset_Polling();

            #region EVENT 등록
            EVENT.onAdd_powerOff += new EVENT.InsertHandler(powerOff);
            EVENT.onAdd_panelUpResize += new EVENT.InsertHandler(panelUpResize);
            EVENT.onAdd_panelCenterResize += new EVENT.InsertHandler(panelCenterResize);
            EVENT.onAdd_panelBottomResize += new EVENT.InsertHandler(panelBottomResize);

            EVENT.onAdd_mainFormPanelMode += new EVENT.InsertHandler_splitterMode(mainFormPanelMode);
            EVENT.onAdd_refresh += new EVENT.InsertHandler(refresh);
            EVENT.onAdd_userDialogMessage += new EVENT.InsertHandler_enum2str(userDialogMessage);
            //EVENT.onAdd_padStatus += new EVENT.InsertHandler_padStatus(padStatus);
            #endregion

        }
        #region EVENT용 delegate 함수
        delegate void powerOff_Call();
        void powerOff()
        {
            if (this.InvokeRequired)
            {
                powerOff_Call d = new powerOff_Call(powerOff);
                this.BeginInvoke(d, new object[] { });
            }
            else
            {
                //DialogResult result;
                ////mc.message.OkCancel("M/C Type Select : FR / RR", out result);
                //mc.message.OkCancel("Do you really want to quit?", out result);
                //if (result == DialogResult.Cancel)
                //{
                //    return;
                //}
                this.Close();
                //Application.Exit();
            }
        }

        delegate void panelUpResize_Call();
        void panelUpResize()
        {
            if (this.SC_Up.InvokeRequired)
            {
                panelUpResize_Call d = new panelUpResize_Call(panelUpResize);
                this.BeginInvoke(d, new object[] { });
            }
            else
            {
                this.SC_Up.Height = mc.panel.up.height;
                this.SC_Up.SplitterDistance = mc.panel.up.splitterDistance;
            }
        }

        delegate void panelCenterResize_Call();
        void panelCenterResize()
        {
            if (this.SC_Center.InvokeRequired)
            {
                panelCenterResize_Call d = new panelCenterResize_Call(panelCenterResize);
                this.BeginInvoke(d, new object[] { });
            }
            else
            {
                this.SC_Center.Height = mc.panel.center.height;
                this.SC_Center.SplitterDistance = mc.panel.center.splitterDistance;
            }
        }

        delegate void panelBottomResize_Call();
        void panelBottomResize()
        {
            if (this.SC_Bottom.InvokeRequired)
            {
                panelBottomResize_Call d = new panelBottomResize_Call(panelCenterResize);
                this.BeginInvoke(d, new object[] { });
            }
            else
            {
                this.SC_Bottom.Height = mc.panel.bottom.height;
                this.SC_Bottom.SplitterDistance = mc.panel.bottom.splitterDistance;
            }
        }

        delegate void mainFormPanelMode_Call(SPLITTER_MODE up, SPLITTER_MODE center, SPLITTER_MODE bottom);
        void mainFormPanelMode(SPLITTER_MODE up, SPLITTER_MODE center, SPLITTER_MODE bottom)
        {
            if (this.InvokeRequired)
            {
                mainFormPanelMode_Call d = new mainFormPanelMode_Call(mainFormPanelMode);
                this.BeginInvoke(d, new object[] { up, center, bottom });
            }
            else
            {
                panelResize(up, center, bottom);
            }
        }

        delegate void refresh_Call();
        void refresh()
        {
            if (this.InvokeRequired)
            {
                refresh_Call d = new refresh_Call(refresh);
                this.BeginInvoke(d, new object[] { });
            }
            else
            {
                if (mc.para.ETC.preMachine.value == (int)PRE_MC.INSPECTION || mc.para.ETC.preMachine.value == (int)PRE_MC.DISPENSER)
                    modelName = "ATTACH #1";
                else if (mc.para.ETC.preMachine.value == (int)PRE_MC.ATTACH)
                    modelName = "ATTACH #2";

                this.Text = mc.model + " (" + modelName + ") " + " [" + mc.para.mcType.FrRr.ToString() + "] - (Version [" + mc.version + "], SECSGEM [" + ((MPCMODE)mc.para.DIAG.controlState.value).ToString() + "]) " + mc.para.ETC.recipeName.description;
            }
        }

        //delegate void padStatus_Call(BOARD_ZONE boardZone, int x, int y, PAD_STATUS status);
        //void padStatus(BOARD_ZONE boardZone, int x, int y, PAD_STATUS status)
        //{
        //    if (this.InvokeRequired)
        //    {
        //        padStatus_Call d = new padStatus_Call(padStatus);
        //        this.BeginInvoke(d, new object[] { boardZone, x, y, status });
        //    }
        //    else
        //    {
        //        //mc.board.padStatus(boardZone, x, y, status, out ret.b);
        //    }
        //}
        delegate void userDialogMessage_Call(DIAG_SEL_MODE i1, DIAG_ICON_MODE i2, string str);
        void userDialogMessage(DIAG_SEL_MODE i1, DIAG_ICON_MODE i2, string str)
        {
            if (this.InvokeRequired)
            {
                userDialogMessage_Call d = new userDialogMessage_Call(userDialogMessage);
                this.BeginInvoke(d, new object[] { i1, i2, str });
            }
            else
            {
                UserMessageBox(i1, i2, str);
            }
        }
        #endregion

        //RetValue ret;
        string modelName = "";
        // 20140612
        public void logRemove()
        {
            DirectoryInfo di = new DirectoryInfo("C:\\PROTEC\\Log\\");
            DirectoryInfo[] Dir = di.GetDirectories();



            foreach (DirectoryInfo Directory in Dir)
            {
                if (Directory.ToString() == "Image")
                {
                    foreach(DirectoryInfo files in Directory.GetDirectories())
                        foreach (FileInfo file in files.GetFiles())
                        {
                                if (file.CreationTime < DateTime.Now.AddDays(-mc.swcontrol.logSave))
                                    file.Delete();
                        }


                    foreach (DirectoryInfo files in Directory.GetDirectories())
                    {
                        if (files.GetFiles().Length == 0) files.Delete();
                    }

                }
                else
                {
                    foreach (System.IO.FileInfo file in Directory.GetFiles())
                    {
                        if (file.CreationTime < DateTime.Now.AddDays(-mc.swcontrol.logSave))
                            file.Delete();
                    }
                }
            }
        }


        private void FormMain_Load(object sender, EventArgs e)
        {
            this.Top = 0; this.Left = 0;
            this.Width = 1280; this.Height = 1024;

            if (mc.para.ETC.preMachine.value == (int)PRE_MC.INSPECTION || mc.para.ETC.preMachine.value == (int)PRE_MC.DISPENSER)
                modelName = "ATTACH #1";
            else if (mc.para.ETC.preMachine.value == (int)PRE_MC.ATTACH)
                modelName = "ATTACH #2";

            this.Text = mc.model + " (" + modelName + ") " + " [" + mc.para.mcType.FrRr.ToString() + "] - (Version [" + mc.version + "], SECSGEM [" + ((MPCMODE)mc.para.DIAG.controlState.value).ToString() + "])";
            EVENT.mainFormPanelMode(SPLITTER_MODE.NORMAL, SPLITTER_MODE.NORMAL, SPLITTER_MODE.EXPAND);
            EVENT.centerRightPanelMode(CENTERER_RIGHT_PANEL.INITIAL);
            EVENT.bottomRightPanelMode(BOTTOM_RIGHT_PANEL.MAIN);
        }

        private void FormMain_Move(object sender, EventArgs e)
        {
            this.Top = 0; this.Left = 0;
            this.Width = 1280; this.Height = 1024;
        }

        void panelResize(SPLITTER_MODE splitterModeUp, SPLITTER_MODE splitterModeCenter, SPLITTER_MODE splitterModeBottom)
        {
            mc.panel.up.height = 95;
            mc.panel.center.height = 602;
            mc.panel.bottom.height = 287;

            int splitterTargetUp, splitterTargetCenter, splitterTargetBottom;
            int splitterCurrentUp, splitterCurrentCenter, splitterCurrentBottom;

            splitterCurrentUp = SC_Up.SplitterDistance;
            splitterCurrentCenter = SC_Center.SplitterDistance;
            splitterCurrentBottom = SC_Bottom.SplitterDistance;

            if (splitterModeUp == SPLITTER_MODE.NORMAL) splitterTargetUp = (int)SPLITTER_MODE.NORMAL;
            else if (splitterModeUp == SPLITTER_MODE.EXPAND) splitterTargetUp = (int)SPLITTER_MODE.EXPAND;
            else splitterTargetUp = splitterCurrentUp;

            if (splitterModeCenter == SPLITTER_MODE.NORMAL)
            {
                splitterTargetCenter = (int)SPLITTER_MODE.NORMAL;
                EVENT.refresh();
            }
            else if (splitterModeCenter == SPLITTER_MODE.EXPAND)
            {
                splitterTargetCenter = (int)SPLITTER_MODE.EXPAND;
                mc.user.logInDone = false;
                //EVENT.refresh();
            }
            else splitterTargetCenter = splitterCurrentCenter;

            if (splitterModeBottom == SPLITTER_MODE.NORMAL) splitterTargetBottom = (int)SPLITTER_MODE.NORMAL;
            else if (splitterModeBottom == SPLITTER_MODE.EXPAND) splitterTargetBottom = (int)SPLITTER_MODE.EXPAND;
            else splitterTargetBottom = splitterCurrentBottom;

            int gap = 200;// 30;
            while (true)
            {
                mc.idle(0);
                #region current change
                if (splitterCurrentUp > splitterTargetUp)
                {
                    splitterCurrentUp -= gap;
                    if (splitterCurrentUp < splitterTargetUp) splitterCurrentUp = splitterTargetUp;
                }
                if (splitterCurrentUp < splitterTargetUp)
                {
                    splitterCurrentUp += gap;
                    if (splitterCurrentUp > splitterTargetUp) splitterCurrentUp = splitterTargetUp;
                }

                if (splitterCurrentCenter > splitterTargetCenter)
                {
                    splitterCurrentCenter -= gap;
                    if (splitterCurrentCenter < splitterTargetCenter) splitterCurrentCenter = splitterTargetCenter;
                }
                if (splitterCurrentCenter < splitterTargetCenter)
                {
                    splitterCurrentCenter += gap;
                    if (splitterCurrentCenter > splitterTargetCenter) splitterCurrentCenter = splitterTargetCenter;
                }

                if (splitterCurrentBottom > splitterTargetBottom)
                {
                    splitterCurrentBottom -= gap;
                    if (splitterCurrentBottom < splitterTargetBottom) splitterCurrentBottom = splitterTargetBottom;
                }
                if (splitterCurrentBottom < splitterTargetBottom)
                {
                    splitterCurrentBottom += gap;
                    if (splitterCurrentBottom > splitterTargetBottom) splitterCurrentBottom = splitterTargetBottom;
                }

                #endregion

                mc.panel.up.splitterDistance = splitterCurrentUp;
                mc.panel.center.splitterDistance = splitterCurrentCenter;
                mc.panel.bottom.splitterDistance = splitterCurrentBottom;
                if (splitterCurrentUp != splitterTargetUp) panelUpResize();
                if (splitterCurrentCenter != splitterTargetCenter) panelCenterResize();
                if (splitterCurrentBottom != splitterTargetBottom) panelBottomResize();
                if(splitterCurrentUp == splitterTargetUp && splitterCurrentCenter == splitterTargetCenter && splitterCurrentBottom == splitterTargetBottom) break;
            }
            panelUpResize();
            panelCenterResize();
            panelBottomResize();
            SC_Up.IsSplitterFixed = true;
            SC_Center.IsSplitterFixed = true;
            SC_Bottom.IsSplitterFixed = true;
        }

        //private void button1_Click(object sender, EventArgs e)
        //{ 
           
        //    EVENT.hWindowAdvanceMode(true);
        //    EVENT.mainFormPanelMode(SPLITTER_MODE.EXPAND, SPLITTER_MODE.EXPAND, SPLITTER_MODE.EXPAND);
        //    mc.idle(1000);
        //    #region
        //    mc.panel.up.left.width = UpLeft.Width;
        //    mc.panel.up.left.height = UpLeft.Height;
        //    EVENT.statusDisplay("mc.panel.up.left.width =" + mc.panel.up.left.width.ToString());
        //    EVENT.statusDisplay("mc.panel.up.left.height =" + mc.panel.up.left.height.ToString());

        //    mc.panel.up.right.width = UpRight.Width;
        //    mc.panel.up.right.height = UpRight.Height;
        //    EVENT.statusDisplay("mc.panel.up.right.width =" + mc.panel.up.right.width.ToString());
        //    EVENT.statusDisplay("mc.panel.up.right.height =" + mc.panel.up.right.height.ToString());

        //    mc.panel.center.left.width = CenterLeft.Width;
        //    mc.panel.center.left.height = CenterLeft.Height;
        //    EVENT.statusDisplay("mc.panel.center.left.width =" + mc.panel.center.left.width.ToString());
        //    EVENT.statusDisplay("mc.panel.center.left.height =" + mc.panel.center.left.height.ToString());

        //    mc.panel.center.right.width = CenterRight.Width;
        //    mc.panel.center.right.height = CenterRight.Height;
        //    EVENT.statusDisplay("mc.panel.center.right.width =" + mc.panel.center.right.width.ToString());
        //    EVENT.statusDisplay("mc.panel.center.right.height =" + mc.panel.center.right.height.ToString());

        //    mc.panel.bottom.left.width = BottomLeft.Width;
        //    mc.panel.bottom.left.height = BottomLeft.Height;
        //    EVENT.statusDisplay("mc.panel.bottom.left.width =" + mc.panel.bottom.left.width.ToString());
        //    EVENT.statusDisplay("mc.panel.bottom.left.height =" + mc.panel.bottom.left.height.ToString());

        //    mc.panel.bottom.right.width = BottomRight.Width;
        //    mc.panel.bottom.right.height = BottomRight.Height;
        //    EVENT.statusDisplay("mc.panel.bottom.right.width =" + mc.panel.bottom.right.width.ToString());
        //    EVENT.statusDisplay("mc.panel.bottom.right.height =" + mc.panel.bottom.right.height.ToString());
        //    #endregion
        //}

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    EVENT.hWindowAdvanceMode(false);
        //    EVENT.mainFormPanelMode(SPLITTER_MODE.NORMAL, SPLITTER_MODE.NORMAL, SPLITTER_MODE.NORMAL);
        //    mc.idle(1000);
        //    #region
        //    mc.panel.up.left.width = UpLeft.Width;
        //    mc.panel.up.left.height = UpLeft.Height;
        //    EVENT.statusDisplay("mc.panel.up.left.width =" + mc.panel.up.left.width.ToString());
        //    EVENT.statusDisplay("mc.panel.up.left.height =" + mc.panel.up.left.height.ToString());

        //    mc.panel.up.right.width = UpRight.Width;
        //    mc.panel.up.right.height = UpRight.Height;
        //    EVENT.statusDisplay("mc.panel.up.right.width =" + mc.panel.up.right.width.ToString());
        //    EVENT.statusDisplay("mc.panel.up.right.height =" + mc.panel.up.right.height.ToString());

        //    mc.panel.center.left.width = CenterLeft.Width;
        //    mc.panel.center.left.height = CenterLeft.Height;
        //    EVENT.statusDisplay("mc.panel.center.left.width =" + mc.panel.center.left.width.ToString());
        //    EVENT.statusDisplay("mc.panel.center.left.height =" + mc.panel.center.left.height.ToString());

        //    mc.panel.center.right.width = CenterRight.Width;
        //    mc.panel.center.right.height = CenterRight.Height;
        //    EVENT.statusDisplay("mc.panel.center.right.width =" + mc.panel.center.right.width.ToString());
        //    EVENT.statusDisplay("mc.panel.center.right.height =" + mc.panel.center.right.height.ToString());

        //    mc.panel.bottom.left.width = BottomLeft.Width;
        //    mc.panel.bottom.left.height = BottomLeft.Height;
        //    EVENT.statusDisplay("mc.panel.bottom.left.width =" + mc.panel.bottom.left.width.ToString());
        //    EVENT.statusDisplay("mc.panel.bottom.left.height =" + mc.panel.bottom.left.height.ToString());

        //    mc.panel.bottom.right.width = BottomRight.Width;
        //    mc.panel.bottom.right.height = BottomRight.Height;
        //    EVENT.statusDisplay("mc.panel.bottom.right.width =" + mc.panel.bottom.right.width.ToString());
        //    EVENT.statusDisplay("mc.panel.bottom.right.height =" + mc.panel.bottom.right.height.ToString());
        //    #endregion
        //}

        public static DIAG_RESULT UserMessageBox(DIAG_SEL_MODE btnMode, DIAG_ICON_MODE iconMode, string dispMessage, string where="")
        {
            FormSelect ff = new FormSelect();

            if(iconMode== DIAG_ICON_MODE.INFORMATION)
                mc.log.debug.write(mc.log.CODE.INFO, where + "Info Message : " + dispMessage);
            else if(iconMode== DIAG_ICON_MODE.QUESTION)
                mc.log.debug.write(mc.log.CODE.INFO, where + "Sel Message : " + dispMessage);
            else if(iconMode== DIAG_ICON_MODE.WARNING)
                mc.log.debug.write(mc.log.CODE.WARN, where + "Warn Message : " + dispMessage);
            else if(iconMode== DIAG_ICON_MODE.FAILURE)
                mc.log.debug.write(mc.log.CODE.FAIL, where + "Fail Message : " + dispMessage);

            ff.SetDisplayItems(btnMode, iconMode, dispMessage);
            ff.BringToFront();
            ff.ShowDialog();

            DIAG_RESULT rst = FormSelect.diagResult;

            return (rst);
        }

        public static DIAG_RESULT UserMessageBox2(DIAG_SEL_MODE btnMode, DIAG_ICON_MODE iconMode, string dispMessage, string where = "")
        {
            FormUserMessage2 ff = new FormUserMessage2();

            if (iconMode == DIAG_ICON_MODE.INFORMATION)
                mc.log.debug.write(mc.log.CODE.INFO, where + "Info Message : " + dispMessage);
            else if (iconMode == DIAG_ICON_MODE.QUESTION)
                mc.log.debug.write(mc.log.CODE.INFO, where + "Sel Message : " + dispMessage);
            else if (iconMode == DIAG_ICON_MODE.WARNING)
                mc.log.debug.write(mc.log.CODE.WARN, where + "Warn Message : " + dispMessage);
            else if (iconMode == DIAG_ICON_MODE.FAILURE)
                mc.log.debug.write(mc.log.CODE.FAIL, where + "Fail Message : " + dispMessage);

            ff.SetDisplayItems(btnMode, iconMode, dispMessage);
            ff.BringToFront();
            ff.ShowDialog();

            DIAG_RESULT rst = FormUserMessage2.diagResult;

            return (rst);
        }

    }
}
