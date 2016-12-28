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

namespace PSA_Application
{
    public partial class FormIOInfo : Form
    {
        public FormIOInfo()
        {
            InitializeComponent();
        }

        enum CellName { NAME = 0, MODULE = 1, BIT = 2, ONOFF = 3}

        Image imageOn, imageOff, imageDisp;
        RetValue ret;

        private void FormIOInfo_Load(object sender, EventArgs e)
        {
            this.Left = 100;
            this.Top = 50;

            InitializeGridInformation();
        }

        private void InitializeGridInformation()
        {
            string[] inputName0 = { "MC", "Door Open Sensor", "Low Door Open Sensor", "VAC MET", "AIR MET", "SMEMA Next", "SMEMA Pre", "CP6",
                                    "CP7", "CP8", "CP9", "CP10", "Loader Input Sensor", "Unloader Sensor", "Loader Buff Sensor", "Work Area Sensor", "Stoper Sensor", "Side Pusher #1 Sensor", "Side Pusher #2 Sensor",
                                    "PD VAC", "", "Laser Alarm", "Laser Near", "Laser Far", "VPPM Compare", "Over Press Sensor", "", "", "", "Start Button Sensor", "Stop Button Sensor", "Reset Button Sensor" };

            string[] inputName1 = { "Blow MET", "Ionizer MET", "Under Press Sensor", "Head VAC", "Double Detect Sensor", "ATC Open Sensor", "ATC Close Sensor",
                                    "Ionizer CON", "Ionizer Alarm", "Ionizer Level", "Tube Detect Sensor #1", "Tube Detect Sensor #2", "Tube Detect Sensor #3",
                                    "Tube Detect Sensor #4", "Tube Detect Sensor #5", "Tube Detect Sensor #6", "Tube Detect Sensor #7", "Tube Detect Sensor #8",
                                    "Magazine Reset #1", "Magazine Reset #2", "", "Pedestal Up Sensor", "", "", "", "", "", "", "", "", "", ""};
            
            string[] inputNameETC = { "Magazine Detect #1", "Magazine Detect #2", "Tube Guide Sensor #1", "Tube Guide Sensor #2", "Tube Guide Sensor #3", 
                                    "Tube Guide Sensor #4", "Tube Guide Sensor #5", "Tube Guide Sensor #6", "Tube Guide Sensor #7", "Tube Guide Sensor #8",
                                    //"VPPM", "LASER", "Bottom Loadcell", "Head Loadcell"};
                                    "", "", "", ""};

            string[] outputName0 = { "", "Safety", "Door Open", "Door Lock", "형광등", "SMEMA Next", "SMEMA Pre", "Tower Lamp : Red", "Tower Lamp : Yellow", "Tower Lamp : Green",
                                    "Buzzer", "Tool Vacuum On", "Tool Blow", "Auto Tool Changer", "Side Pusher", "STOPER", "Pedestal Vacuum", "Pedestal Blow", "LASER ON", "Set Laser Zero",
                                    "Laser Time", "Set VPPM Speed", "Up Looking Camera Trigger", "Head Camera Trigger", "Set Ionizer Balance", "Ionizer On", "Magazine Reset #1", "Magazine Reset #2",
                                    "Tube Blow #1", "Tube Blow #2", "Tube Blow #3", "Tube Blow #4"};

            //string[] outputNameETC = { "Input Conveyor Belt", "Work Conveyor Belt", "Output Conveyor Belt", "VPPM" };
            string[] outputNameETC = { "Input Conveyor Belt", "Work Conveyor Belt", "Output Conveyor Belt", "" };

            for (int k = 0; k < 32; k++)
            {
                if (inputName1[k] == "Tube Detect Sensor #3" || inputName1[k] == "Tube Detect Sensor #4" || inputName1[k] == "Tube Detect Sensor #7" || inputName1[k] == "Tube Detect Sensor #8")
                    inputName1[k] = "";
                if (inputName1[k] == "Tube Detect Sensor #5") inputName1[k] = "Tube Detect Sensor #3";
                if (inputName1[k] == "Tube Detect Sensor #6") inputName1[k] = "Tube Detect Sensor #4";
            }

            if (mc.swcontrol.mechanicalRevision == 1)
            {
            }

            int nameIndex = 0; int i = 0; int subIndex = 0;
            imageOff = Properties.Resources.Green_LED_OFF;
            imageOn = Properties.Resources.Green_LED;
            while (nameIndex < 32)
            {
                while (nameIndex < 32 && inputName0[nameIndex] == "")
                {
                    nameIndex++;
                }
                if (nameIndex == 32) break;
                GV_InputModule_0.Rows.Add();
                GV_InputModule_0.Rows[i].Cells[(int)CellName.NAME].Value = inputName0[nameIndex];
                GV_InputModule_0.Rows[i].Cells[(int)CellName.MODULE].Value = "Axt Input Module #0";
                GV_InputModule_0.Rows[i].Cells[(int)CellName.BIT].Value = nameIndex.ToString();
                GV_InputModule_0.Rows[i].Cells[(int)CellName.ONOFF].Value = imageOff;
                nameIndex++; i++;
            }

            nameIndex = 0; i = 0;
            while (nameIndex < 32)
            {
                while (nameIndex < 32 && inputName1[nameIndex] == "")
                {
                    nameIndex++;
                }
                if (nameIndex == 32) break;
                GV_InputModule_1.Rows.Add();
                GV_InputModule_1.Rows[i].Cells[(int)CellName.NAME].Value = inputName1[nameIndex];
                GV_InputModule_1.Rows[i].Cells[(int)CellName.MODULE].Value = "Axt Input Module #1";
                GV_InputModule_1.Rows[i].Cells[(int)CellName.BIT].Value = nameIndex.ToString();
                GV_InputModule_1.Rows[i].Cells[(int)CellName.ONOFF].Value = imageOff;
                nameIndex++; i++;
            }

            nameIndex = 0; i = 0;
            while (nameIndex < 10)      // 14
            {
                while (nameIndex < 10 && inputNameETC[nameIndex] == "")     // 14
                {
                    nameIndex++;
                }
                if (nameIndex == 10) break;     // 14
                GV_InputModule_ETC.Rows.Add();
                GV_InputModule_ETC.Rows[i].Cells[(int)CellName.NAME].Value = inputNameETC[nameIndex];
                if(inputNameETC[nameIndex] == "Magazine Detect #1" || inputNameETC[nameIndex] == "Magazine Detect #2" || inputNameETC[nameIndex] == "Tube Guide Sensor #1" || inputNameETC[nameIndex] == "Tube Guide Sensor #2")
                {
                    GV_InputModule_ETC.Rows[i].Cells[(int)CellName.MODULE].Value = "Motor(Axis #9) Input Module";
                    if(inputNameETC[nameIndex] == "Magazine Detect #1") GV_InputModule_ETC.Rows[i].Cells[(int)CellName.BIT].Value = "12";
                    else if(inputNameETC[nameIndex] == "Magazine Detect #2") GV_InputModule_ETC.Rows[i].Cells[(int)CellName.BIT].Value = "13";
                    else if(inputNameETC[nameIndex] == "Tube Guide Sensor #1") GV_InputModule_ETC.Rows[i].Cells[(int)CellName.BIT].Value = "14";
                    else if (inputNameETC[nameIndex] == "Tube Guide Sensor #2") GV_InputModule_ETC.Rows[i].Cells[(int)CellName.BIT].Value = "15";
                }
                else if (inputNameETC[nameIndex] == "Tube Guide Sensor #3" || inputNameETC[nameIndex] == "Tube Guide Sensor #4" || inputNameETC[nameIndex] == "Tube Guide Sensor #5" || inputNameETC[nameIndex] == "Tube Guide Sensor #6")
                {
                    GV_InputModule_ETC.Rows[i].Cells[(int)CellName.MODULE].Value = "Motor(Axis #10) Input Module";
                    if (inputNameETC[nameIndex] == "Tube Guide Sensor #3") GV_InputModule_ETC.Rows[i].Cells[(int)CellName.BIT].Value = "12";
                    else if(inputNameETC[nameIndex] == "Tube Guide Sensor #4") GV_InputModule_ETC.Rows[i].Cells[(int)CellName.BIT].Value = "13";
                    else if (inputNameETC[nameIndex] == "Tube Guide Sensor #5")
                    {
                        GV_InputModule_ETC.Rows[i].Cells[(int)CellName.BIT].Value = "12";
                    }
                    else if (inputNameETC[nameIndex] == "Tube Guide Sensor #6")
                    {
                        GV_InputModule_ETC.Rows[i].Cells[(int)CellName.BIT].Value = "13";
                    }
                }
                else if (inputNameETC[nameIndex] == "Tube Guide Sensor #7" || inputNameETC[nameIndex] == "Tube Guide Sensor #8")
                {
                    GV_InputModule_ETC.Rows[i].Cells[(int)CellName.MODULE].Value = "Motor(Axis #11) Input Module";
                    if (inputNameETC[nameIndex] == "Tube Guide Sensor #7") GV_InputModule_ETC.Rows[i].Cells[(int)CellName.BIT].Value = "12";
                    else if (inputNameETC[nameIndex] == "Tube Guide Sensor #8") GV_InputModule_ETC.Rows[i].Cells[(int)CellName.BIT].Value = "13";
                }
                else 
                {
                    GV_InputModule_ETC.Rows[i].Cells[(int)CellName.MODULE].Value = "Analog Input Module";
                    GV_InputModule_ETC.Rows[i].Cells[(int)CellName.BIT].Value = subIndex.ToString();
                    subIndex++;
                }
                
                GV_InputModule_ETC.Rows[i].Cells[(int)CellName.ONOFF].Value = imageOff;

                nameIndex++; i++;
            }

            nameIndex = 0; i = 0;
            while (nameIndex < 32)
            {
                while (nameIndex < 32 && outputName0[nameIndex] == "")
                {
                    nameIndex++;
                }
                if (nameIndex == 32) break;
                GV_OutputModule.Rows.Add();
                GV_OutputModule.Rows[i].Cells[(int)CellName.NAME].Value = outputName0[nameIndex];
                GV_OutputModule.Rows[i].Cells[(int)CellName.MODULE].Value = "Axt Output Module #0";
                GV_OutputModule.Rows[i].Cells[(int)CellName.BIT].Value = nameIndex.ToString();
                GV_OutputModule.Rows[i].Cells[(int)CellName.ONOFF].Value = imageOff;
                nameIndex++; i++;
            }

            nameIndex = 0; i = 0; subIndex = 0;
            while (nameIndex < 3)       // 4
            {
                while (nameIndex < 3 && outputNameETC[nameIndex] == "")     // 4
                {
                    nameIndex++;
                }
                if (nameIndex == 3) break;      // 4
                GV_OutputModuleETC.Rows.Add();
                GV_OutputModuleETC.Rows[i].Cells[(int)CellName.NAME].Value = outputNameETC[nameIndex];
                if (outputNameETC[nameIndex] == "Input Conveyor Belt" || outputNameETC[nameIndex] == "Work Conveyor Belt" || outputNameETC[nameIndex] == "Output Conveyor Belt")
                {
                    GV_OutputModuleETC.Rows[i].Cells[(int)CellName.MODULE].Value = "Motor(Axis #9) Output Module";
                    GV_OutputModuleETC.Rows[i].Cells[(int)CellName.BIT].Value = subIndex.ToString();
                    subIndex++;
                }
                else
                {
                    GV_OutputModuleETC.Rows[i].Cells[(int)CellName.MODULE].Value = "Analog Output Module";
                    GV_OutputModuleETC.Rows[i].Cells[(int)CellName.BIT].Value = "0";
                }

                GV_OutputModuleETC.Rows[i].Cells[(int)CellName.ONOFF].Value = imageOff;
                nameIndex++; i++;
            }

            GV_InputModule_0.CurrentCell = null;
        }

        private void Cotrol_Click(object sender, EventArgs e)
        {
            UpdateTimer.Enabled = false;
            if (sender.Equals(BT_CLOSE)) this.Close();
        }

        private void RefreshIOInformation()
        {
            if (TC_IOList.SelectedTab == tabInputModule0)
            {
                #region InputModule 0
                mc.IN.MAIN.MC2(out ret.b, out ret.message);                
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[0].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.MAIN.DOOR(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[1].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.MAIN.LOW_DOOR(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[2].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.MAIN.VAC_MET(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[3].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.MAIN.AIR_MET(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[4].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.CV.SMEMA_NEXT(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[5].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.CV.SMEMA_PRE(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[6].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.MAIN.CP6(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[7].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.MAIN.CP7(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[8].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.MAIN.CP8(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[9].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.MAIN.CP9(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[10].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.MAIN.CP10(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[11].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.CV.BD_IN(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[12].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.CV.BD_OUT(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[13].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.CV.BD_BUF(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[14].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.CV.BD_NEAR(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[15].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.CV.BD_STOP_ON(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[16].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.CV.BD_CL1_ON(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[17].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.CV.BD_CL2_ON(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[18].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.PD.VAC_CHK(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[19].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.LS.ALM(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[20].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.LS.NEAR(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[21].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.LS.FAR(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[22].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.ER.COMPARE(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[23].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.HD.LOAD_CHK2(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[24].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.MAIN.START_CHK(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[25].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.MAIN.STOP_CHK(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[26].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.MAIN.RESET_CHK(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_0.Rows[27].Cells[(int)CellName.ONOFF].Value = imageDisp;
                #endregion
            }
            else if (TC_IOList.SelectedTab == tabInputModule1)
            {
                #region InputModule 1
                mc.IN.MAIN.BLOW_MET(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_1.Rows[0].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.MAIN.IONIZER_MET(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_1.Rows[1].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.HD.LOAD_CHK(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_1.Rows[2].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.HD.VAC_CHK(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_1.Rows[3].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.HD.DOUBLE_DET(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_1.Rows[4].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.HD.ATC_OPEN(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_1.Rows[5].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.HD.ATC_CLOSE(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_1.Rows[6].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.MAIN.IONIZER_CON(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_1.Rows[7].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.MAIN.IONIZER_ARM(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_1.Rows[8].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.MAIN.IONIZER_LEV(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_1.Rows[9].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.SF.TUBE_DET(UnitCodeSF.SF1, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_1.Rows[10].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.SF.TUBE_DET(UnitCodeSF.SF2, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_1.Rows[11].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.SF.TUBE_DET(UnitCodeSF.SF3, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_1.Rows[12].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.SF.TUBE_DET(UnitCodeSF.SF4, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_1.Rows[13].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.SF.MG_RESET(UnitCodeSFMG.MG1, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_1.Rows[18].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.SF.MG_RESET(UnitCodeSFMG.MG2, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_1.Rows[19].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.PD.UP_SENSOR_CHK(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_1.Rows[21].Cells[(int)CellName.ONOFF].Value = imageDisp;

                if (mc.swcontrol.mechanicalRevision == 0)
                {
                    
                }

                #endregion
            }
            else if (TC_IOList.SelectedTab == tabInputModuleETC)
            {
                #region InputModule ETC
                mc.IN.SF.MG_DET(UnitCodeSFMG.MG1, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_ETC.Rows[0].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.SF.MG_DET(UnitCodeSFMG.MG2, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_ETC.Rows[1].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.SF.TUBE_GUIDE(UnitCodeSF.SF1, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_ETC.Rows[2].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.SF.TUBE_GUIDE(UnitCodeSF.SF2, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_ETC.Rows[3].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.SF.TUBE_GUIDE(UnitCodeSF.SF3, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_ETC.Rows[4].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.IN.SF.TUBE_GUIDE(UnitCodeSF.SF4, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_InputModule_ETC.Rows[5].Cells[(int)CellName.ONOFF].Value = imageDisp;

                //if (mc.swcontrol.mechanicalRevision == 0)
                //{
                //}
                #endregion
            }

            else if (TC_IOList.SelectedTab == tabOutputModule0)
            {
                #region OutputModule 0
                mc.OUT.MAIN.SAFETY(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[0].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.MAIN.DOOR_OPEN(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[1].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.MAIN.DOOR_LOCK(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[2].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.MAIN.FLUORESCENT(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[3].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.CV.SMEMA_NEXT(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[4].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.CV.SMEMA_PRE(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[5].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.MAIN.T_RED(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[6].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.MAIN.T_YELLOW(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[7].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.MAIN.T_GREEN(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[8].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.MAIN.T_BUZZER(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[9].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.HD.SUC(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[10].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.HD.BLW(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[11].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.HD.ATC(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[12].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.CV.BD_CL(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[13].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.CV.BD_STOP(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[14].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.PD.SUC(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[15].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.PD.BLW(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[16].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.HD.LS.ON(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[17].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.HD.LS.ZERO(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[18].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.HD.LS.TIME(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[19].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.HD.ER.SPEED(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[20].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.HD.HDC_TRIGGER(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[21].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.HD.ULC_TRIGGER(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[22].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.MAIN.IONIZER_Balance(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[23].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.MAIN.IONIZER(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[24].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[25].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[26].Cells[(int)CellName.ONOFF].Value = imageDisp;
                
                mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF1, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[27].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF2, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[28].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF3, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[29].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF4, out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModule.Rows[30].Cells[(int)CellName.ONOFF].Value = imageDisp;
                #endregion
            }
            else if (TC_IOList.SelectedTab == tabOutputModuleETC)
            {
                #region OutputModule ETC
                mc.OUT.CV.FD_MTR1(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModuleETC.Rows[0].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.CV.FD_MTR2(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModuleETC.Rows[1].Cells[(int)CellName.ONOFF].Value = imageDisp;

                mc.OUT.CV.FD_MTR3(out ret.b, out ret.message);
                if (ret.b) imageDisp = imageOn;
                else imageDisp = imageOff;
                GV_OutputModuleETC.Rows[2].Cells[(int)CellName.ONOFF].Value = imageDisp;

                #endregion
            }

        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateTimer.Enabled = false;
            RefreshIOInformation();
            UpdateTimer.Enabled = true;
        }

        private void FormIOInfo_Shown(object sender, EventArgs e)
        {
            UpdateTimer.Enabled = true;
        }
    }
}
