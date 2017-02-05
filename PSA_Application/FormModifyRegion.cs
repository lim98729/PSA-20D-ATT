using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using PSA_SystemLibrary;
using DefineLibrary;


namespace PSA_Application
{
    public partial class FormModifyRegion : Form
    {
        public FormModifyRegion()
        {
            InitializeComponent();
        }

        bool isRunning;
        object oButton;

        public para_member deltaX;
        public para_member deltaY;
        public para_member deltaT;

        public para_member deltaH;
        public para_member deltaW;

        double[] _dataR1 = new double[(int)MAX_COUNT.BLOB];
        double[] _dataR2 = new double[(int)MAX_COUNT.BLOB];
        double[] _dataC1 = new double[(int)MAX_COUNT.BLOB];
        double[] _dataC2 = new double[(int)MAX_COUNT.BLOB];

        double[] _dataR = new double[(int)MAX_COUNT.BLOB];
        double[] _dataC = new double[(int)MAX_COUNT.BLOB];
        double[] _dataL1 = new double[(int)MAX_COUNT.BLOB];
        double[] _dataL2 = new double[(int)MAX_COUNT.BLOB];
        double[] _dataT = new double[(int)MAX_COUNT.BLOB];
       
        bool All_Item_Selected = false;
        RetValue ret;
        bool bStop;


        double dPos;
        double dSize;
        int number;

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
                try
                {
                    BT_Pos_Speed.Text = "±" + dPos.ToString();
                    BT_Size_Speed.Text = "±" + dSize.ToString();
					mc.hdc.cam.displayBlobTheta(mc.hdc.cam.epoxyBlob);

                    BT_ESC.Focus();
                }
                catch (Exception error)
                {
                    mc.log.debug.write(mc.log.CODE.ERROR, "FormModifyRegion Error : (" + error.Message + "), " + error.StackTrace + "," + error.TargetSite + ", " + error.Source);
                }
            }
        }
        private void FormModifyRegion_Load(object sender, EventArgs e)
        {
			BT_Pos_CCW.Visible = false;
			BT_Pos_CW.Visible = false;
            this.Left = 630;
            this.Top = 200;
            dPos = dSize = 1;
            InitListview();
            mc.para.setting(ref deltaX, "Only Teaching Use", 0, -500, 500);
            mc.para.setting(ref deltaY, "Only Teaching Use", 0, -500, 500);
            mc.para.setting(ref deltaT, "Only Teaching Use", 0, -180, 180);

            mc.para.setting(ref deltaH, "Only Teaching Use", 0, -500, 500);
            mc.para.setting(ref deltaW, "Only Teaching Use", 0, -500, 500);

            for (int i = 0; i < (int)MAX_COUNT.BLOB; i++)
			{
				if (mc.hdc.cam.epoxyBlob[i].isCreate == "true")
				{
					_dataC1[i] = mc.hdc.cam.epoxyBlob[i].findColumn1;
					_dataC2[i] = mc.hdc.cam.epoxyBlob[i].findColumn2;

					_dataR1[i] = mc.hdc.cam.epoxyBlob[i].findRow1;
					_dataR2[i] = mc.hdc.cam.epoxyBlob[i].findRow2;

					//_dataT[i] = mc.hdc.cam.epoxyBlob[i].findTheta;
				}
			}
			mc.hdc.cam.displayBlobTheta(mc.hdc.cam.epoxyBlob);
			InitListview();
            for (int i = 0; i < (int)MAX_COUNT.BLOB; i++)
			{
				if (mc.hdc.cam.epoxyBlob[i].isCreate == "true")
				{
					AddColumnList("Box" + i.ToString());
				}
			}
        }

        #region ListView
        private void InitListview()
        {
            LV_Modify_Area_List.Clear();
            LV_Modify_Area_List.View = View.Details; //자세히 보기 설정
            LV_Modify_Area_List.LabelEdit = false;//라벨변경모드
            LV_Modify_Area_List.Columns.Add("Area Box List", 135, HorizontalAlignment.Center);
        }
        private void MoveItem(int index, ListViewItem oItem)
        {
            // 제거
            LV_Modify_Area_List.SelectedItems[0].Remove();

            // 추가
            LV_Modify_Area_List.Items.Insert(index, oItem);

            oItem.Selected = true;
            oItem.Focused = true;

            LV_Modify_Area_List.Focus();
        }
        public void List_Up()
        {
            if (LV_Modify_Area_List.SelectedItems.Count > 0)
            {
                ListViewItem oItem = (ListViewItem)LV_Modify_Area_List.SelectedItems[0].Clone();
                int index = LV_Modify_Area_List.SelectedItems[0].Index;

                if (index > 0)
                    index--;

                MoveItem(index, oItem);
            }

        }
        public void List_Down()
        {
            if (LV_Modify_Area_List.SelectedItems.Count > 0)
            {
                ListViewItem oItem = (ListViewItem)LV_Modify_Area_List.SelectedItems[0].Clone();
                int index = LV_Modify_Area_List.SelectedItems[0].Index;

                if (index < LV_Modify_Area_List.Items.Count - 1)
                    index++;

                MoveItem(index, oItem);
            }

        }
        private void AddColumnList(string strGrade)
        {
            ListViewItem lvi;
            lvi = new ListViewItem();
            lvi.Text = strGrade;
            this.LV_Modify_Area_List.Items.Add(lvi);
        }
        #endregion

        private void Control_Click(object sender, EventArgs e)
        {
            if (isRunning) return;
            if (sender.Equals(BT_ESC))
            {
                //dataZ = _dataZ;
                //timer.Enabled = false;
                //mc.hdc.LIVE = false;
                //mc.idle(500);
				for (int i = 0; i < (int)MAX_COUNT.BLOB; i++)
				{
					if (mc.hdc.cam.epoxyBlob[i].isCreate == "true")
					{
						mc.hdc.cam.epoxyBlob[i].findColumn1 = _dataC1[i];
						mc.hdc.cam.epoxyBlob[i].findColumn2 = _dataC2[i];

						mc.hdc.cam.epoxyBlob[i].findRow1 = _dataR1[i];
						mc.hdc.cam.epoxyBlob[i].findRow2 = _dataR2[i];

						mc.hdc.cam.epoxyBlob[i].findTheta = _dataT[i];
					}
				}
                this.Close();
            }
            if (sender.Equals(BT_Set))
            {
                //timer.Enabled = false;
                //mc.hdc.LIVE = false;
                //mc.idle(500);
				for (int i = 0; i < (int)MAX_COUNT.BLOB; i++)
				{
					if (mc.hdc.cam.epoxyBlob[i].isCreate == "true")
					{
						mc.hdc.cam.writeBlob(i);
					}
				}
                this.Close();
            }
            if (sender.Equals(BT_Pos_Speed))
            {
                if (dPos == 1) dPos = 10;
                else if (dPos == 10) dPos = 50;
                else if (dPos == 50) dPos = 100;
                else if (dPos == 100) dPos = 1;
                else dPos = 1;
            }
            if (sender.Equals(BT_Size_Speed))
            {
                if (dSize == 1) dSize = 10;
                else if (dSize == 10) dSize = 50;
                else if (dSize == 50) dSize = 100;
                else if (dSize == 100) dSize = 1;
                else dSize = 1;
            }
            if (sender.Equals(RB_All_Item_Selected))
            {
                All_Item_Selected = true;
                RB_All_Item_Selected.Checked = true;
            }
            if (sender.Equals(RB_One_Item_Selected))
            {
                All_Item_Selected = false;
                RB_One_Item_Selected.Checked = true;
            }
            refresh();
        }
        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            if (isRunning) return;

            oButton = sender;
            bStop = false;
            try
            {
                number = LV_Modify_Area_List.SelectedItems[0].Index;
                number = Convert.ToInt32(LV_Modify_Area_List.Items[number].Text.Remove(0, 3));
                Thread th = new Thread(control);
                th.Name = "FormModify_MouseDownThread";
                th.Start();
            }
            catch
            {
                MessageBox.Show("Please select item");
            }
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            oButton = null;
            bStop = true;
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            oButton = null;
            bStop = true;
        }
        void control()
        {
            isRunning = true;
            int interval = 300;
            double temp1,temp2;
            while (true)
            {
                if (oButton == BT_Pos_Left) deltaX.value -= dPos;
                if (oButton == BT_Pos_Right) deltaX.value += dPos;

                if (deltaX.value > deltaX.upperLimit) deltaX.value = deltaX.upperLimit;
                if (deltaX.value < deltaX.lowerLimit) deltaX.value = deltaX.lowerLimit;

                if (oButton == BT_Pos_Up) deltaY.value -= dPos;
                if (oButton == BT_Pos_Down) deltaY.value += dPos;

                if (deltaY.value > deltaY.upperLimit) deltaY.value = deltaY.upperLimit;
                if (deltaY.value < deltaY.lowerLimit) deltaY.value = deltaY.lowerLimit;

                if (oButton == BT_Pos_CCW ) deltaT.value += dPos;
                if (oButton == BT_Pos_CW ) deltaT.value -= dPos;

                if (deltaT.value > deltaT.upperLimit) deltaT.value = deltaT.upperLimit;
                if (deltaT.value < deltaT.lowerLimit) deltaT.value = deltaT.lowerLimit;


                if (oButton == BT_Size_Height_Dec) deltaH.value -= dSize;
                if (oButton == BT_Size_Height_Inc) deltaH.value += dSize;

                if (deltaH.value > deltaH.upperLimit) deltaT.value = deltaH.upperLimit;
                if (deltaH.value < deltaH.lowerLimit) deltaT.value = deltaH.lowerLimit;

                if (oButton == BT_Size_Width_Dec) deltaW.value -= dSize;
                if (oButton == BT_Size_Width_Inc) deltaW.value += dSize;

                if (deltaW.value > deltaW.upperLimit) deltaW.value = deltaW.upperLimit;
                if (deltaW.value < deltaW.lowerLimit) deltaW.value = deltaW.lowerLimit;


                if (!All_Item_Selected)
                {
					mc.hdc.cam.epoxyBlob[number].findColumn1 += deltaX.value;
					mc.hdc.cam.epoxyBlob[number].findColumn2 += deltaX.value;

					mc.hdc.cam.epoxyBlob[number].findRow1 += deltaY.value;
					mc.hdc.cam.epoxyBlob[number].findRow2 += deltaY.value;

					//mc.hdc.cam.epoxyBlob[number].findTheta += deltaT.value;

					temp1 = mc.hdc.cam.epoxyBlob[number].findColumn1 + deltaW.value;
					temp2 = mc.hdc.cam.epoxyBlob[number].findColumn2 - deltaW.value;
					if (temp2 > temp1)
					{
						mc.hdc.cam.epoxyBlob[number].findColumn1 -= deltaW.value;
						mc.hdc.cam.epoxyBlob[number].findColumn2 += deltaW.value;
					}
					temp1 = mc.hdc.cam.epoxyBlob[number].findRow1 - deltaH.value;
					temp2 = mc.hdc.cam.epoxyBlob[number].findRow2 + deltaH.value;
					if (temp2 > temp1)
					{
						mc.hdc.cam.epoxyBlob[number].findRow1 -= deltaH.value;
						mc.hdc.cam.epoxyBlob[number].findRow2 += deltaH.value;
					}
                }
                else
                {
                    for (int i = 0; i < (int)MAX_COUNT.BLOB; i++)
                    {
						if (mc.hdc.cam.epoxyBlob[i].isCreate == "true")
						{
							mc.hdc.cam.epoxyBlob[i].findColumn1 += deltaX.value;
							mc.hdc.cam.epoxyBlob[i].findColumn2 += deltaX.value;

							mc.hdc.cam.epoxyBlob[i].findRow1 += deltaY.value;
							mc.hdc.cam.epoxyBlob[i].findRow2 += deltaY.value;

							//mc.hdc.cam.epoxyBlob[i].findTheta += deltaT.value;

							mc.hdc.cam.epoxyBlob[i].findColumn1 += deltaW.value;
							mc.hdc.cam.epoxyBlob[i].findColumn2 -= deltaW.value;

							mc.hdc.cam.epoxyBlob[i].findRow1 -= deltaH.value;
							mc.hdc.cam.epoxyBlob[i].findRow2 += deltaH.value;
						}
                    }

                }
                deltaX.value = 0;
                deltaY.value = 0;
                deltaT.value = 0;

                deltaW.value = 0;
                deltaH.value = 0;

                refresh();

                interval -= 50; if (interval < 50) interval = 50;
                mc.idle(interval);

                if (bStop) break;
            }
        EXIT:
            isRunning = false;
        }

        
    }
}
