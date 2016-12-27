using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using HalconDotNet;
using DefineLibrary;
using System.Windows.Forms.DataVisualization.Charting;

namespace AccessoryLibrary
{
	public partial class loadScope : UserControl
	{
		#region Fields

		// form fields
		const int displayGap = 2;
		int pointIndex;

		#endregion	// Fields

		#region Construction and Disposing

		public loadScope()
		{
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_addLoadcellData += new EVENT.InsertHandler_addLoadcellData(addLoadcellData);
			EVENT.onAdd_clearLoadcellData += new EVENT.InsertHandler_clearLoadcellData(clearLoadcellData);
			EVENT.onAdd_controlLoadcellData += new EVENT.InsertHandler_controlLoadcellData(controlLoadcellData);
			#endregion
		}

		#endregion	// Construction and Disposing

        private void loadScope_Load(object sender, EventArgs e)
        {
            makeEmptyScope();
        }

        #region EVENT용 delegate 함수

        delegate void clearLoadcellData_Call();
        void clearLoadcellData()
        {
            if (this.loadcellChart.InvokeRequired)
            {
                clearLoadcellData_Call d = new clearLoadcellData_Call(clearLoadcellData);
                this.loadcellChart.BeginInvoke(d, new object[] { });
            }
            else
            {
                pointIndex = 0;
                loadcellChart.Series[0].Points.Clear();
                loadcellChart.Series[1].Points.Clear();
                loadcellChart.Series[2].Points.Clear();
                loadcellChart.Series[3].Points.Clear();
                loadcellChart.ChartAreas[0].CursorX.Position = -1;
                loadcellChart.ChartAreas[0].CursorY.Position = -1;
            }
        }

        delegate void addLoadcellData_Call(int seriesNum, double xVal, double yVal, double y2Val);
        void addLoadcellData(int seriesNum, double xVal, double yVal, double y2Val)
        {
            if (this.loadcellChart.InvokeRequired)
            {
                addLoadcellData_Call d = new addLoadcellData_Call(addLoadcellData);
                this.loadcellChart.BeginInvoke(d, new object[] { seriesNum, xVal, yVal, y2Val });
            }
            else
            {
                if (y2Val < -1)
                    loadcellChart.Series[seriesNum].Points.AddXY(Math.Round(xVal) / 1000, yVal);
                else
                {
                    if (UtilityControl.graphControlDataDisplay > 0)
                        loadcellChart.Series[seriesNum].Points.AddXY(Math.Round(xVal) / 1000, yVal);
                    if (UtilityControl.graphLoadcellDataDisplay > 0)
                        loadcellChart.Series[seriesNum + 2].Points.AddXY(Math.Round(xVal) / 1000, y2Val);
                }
                pointIndex++;
            }
        }

        delegate void controlLoadcellData_Call(int flag, double ctlVal);
        void controlLoadcellData(int flag, double ctlVal)
        {
            if (this.loadcellChart.InvokeRequired)
            {
                controlLoadcellData_Call d = new controlLoadcellData_Call(controlLoadcellData);
                this.loadcellChart.BeginInvoke(d, new object[] { flag, ctlVal });
            }
            else
            {
                switch (flag)
                {
                    case 0: loadcellChart.ResetAutoValues(); break;
                    case 1: loadcellChart.Invalidate(); break;
                    case 2: loadcellChart.ChartAreas[0].AxisX.Maximum = (int)ctlVal; break;
                    case 3: loadcellChart.ChartAreas[0].AxisY.Maximum = ctlVal; break;
                    case 4: loadcellChart.ChartAreas[0].CursorX.Position = (int)ctlVal; break;
                    case 5: loadcellChart.ChartAreas[0].CursorY.Position = (int)ctlVal; break;
                    default: break;
                }
            }
        }
        #endregion

        public void findMinMax()
		{
			DataPoint maxValuePoint = loadcellChart.Series[0].Points.FindMaxByValue();
			maxValuePoint.Color = Color.FromArgb(255, 128, 128);

			// Find point with minimum Y value and change color
			DataPoint minValuePoint = loadcellChart.Series[0].Points.FindMinByValue();
			minValuePoint.Color = Color.FromArgb(128, 128, 255); 
		}

		private void makeEmptyScope()
		{
			// create new Graph
			loadcellChart.Location = new System.Drawing.Point(displayGap, displayGap);
			loadcellChart.Size = new System.Drawing.Size(this.ClientSize.Width - 2 * displayGap, this.ClientSize.Height - 2 * displayGap);

			//loadcellChart.ChartAreas[0].AxisX.Minimum = 0;
			//loadcellChart.ChartAreas[0].AxisX.Maximum = 5000;		// 5000 mS
			loadcellChart.ChartAreas[0].AxisX.MajorGrid.Interval = 1;
			loadcellChart.ChartAreas[0].AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			loadcellChart.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			loadcellChart.ChartAreas[0].AxisX.MinorGrid.Interval = 0.1;
			loadcellChart.ChartAreas[0].AxisX.MinorGrid.LineColor = Color.LightYellow;
			loadcellChart.ChartAreas[0].AxisX.LineDashStyle = ChartDashStyle.Dash;
			loadcellChart.ChartAreas[0].AxisX.ScrollBar.ButtonColor = System.Drawing.Color.Silver;
			loadcellChart.ChartAreas[0].AxisX.ScrollBar.LineColor = System.Drawing.Color.Black;

			//loadcellChart.ChartAreas[0].AxisY.Minimum = 0;
			//loadcellChart.ChartAreas[0].AxisY.Maximum = 5;		// 5 Kg
			loadcellChart.ChartAreas[0].AxisY.MajorGrid.Interval = 0.1;
			loadcellChart.ChartAreas[0].AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			loadcellChart.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			loadcellChart.ChartAreas[0].AxisY.MinorGrid.Interval = 0.05;
			loadcellChart.ChartAreas[0].AxisY.MinorGrid.LineColor = Color.LightYellow;
			loadcellChart.ChartAreas[0].AxisY.LineDashStyle = ChartDashStyle.Dash;
			loadcellChart.ChartAreas[0].AxisY.ScrollBar.ButtonColor = System.Drawing.Color.Silver;
			loadcellChart.ChartAreas[0].AxisY.ScrollBar.LineColor = System.Drawing.Color.Black;

			//loadChartArea.BackColor = Color.Black;
			loadcellChart.ChartAreas[0].BackColor = System.Drawing.Color.Gainsboro;
			loadcellChart.ChartAreas[0].BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
			loadcellChart.ChartAreas[0].BackSecondaryColor = System.Drawing.Color.White;
			loadcellChart.ChartAreas[0].BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			loadcellChart.ChartAreas[0].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
			loadcellChart.ChartAreas[0].ShadowColor = System.Drawing.Color.Transparent;

			loadcellChart.BackColor = System.Drawing.Color.WhiteSmoke;
			loadcellChart.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
			loadcellChart.BackSecondaryColor = System.Drawing.Color.White;
			loadcellChart.BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
			loadcellChart.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
			loadcellChart.BorderlineWidth = 2;
			loadcellChart.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
			

			// set cursor selection color of X axis cursor & Interval
			loadcellChart.ChartAreas[0].CursorX.Interval = 0.1;
			loadcellChart.ChartAreas[0].CursorY.Interval = 0.1;

			loadcellChart.ChartAreas[0].CursorX.IsUserEnabled = true;
			loadcellChart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
			//loadChartArea.CursorX.Position = 2;
			loadcellChart.ChartAreas[0].CursorX.SelectionColor = Color.Gray;

			loadcellChart.ChartAreas[0].CursorY.IsUserEnabled = true;
			loadcellChart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
			//loadChartArea.CursorY.Position = 2;
			loadcellChart.ChartAreas[0].CursorY.SelectionColor = Color.Gray;

			
			// VPPM to Force Value series
			loadcellChart.Series[0].ChartType = SeriesChartType.FastLine;
			loadcellChart.Series[0].BorderWidth = 1;
			loadcellChart.Series[0].Color = Color.OrangeRed;

			// VPPM to Force Value for Start Point
			loadcellChart.Series[1].ChartType = SeriesChartType.FastPoint;
			loadcellChart.Series[1].BorderWidth = 2;
			loadcellChart.Series[1].Color = Color.Red;

			// VPPM to Force Value series
			loadcellChart.Series[2].ChartType = SeriesChartType.FastLine;
			loadcellChart.Series[2].BorderWidth = 1;
			loadcellChart.Series[2].Color = Color.MidnightBlue;

			// VPPM to Force Value for Start Point
			loadcellChart.Series[3].ChartType = SeriesChartType.FastPoint;
			loadcellChart.Series[3].BorderWidth = 2;
			loadcellChart.Series[3].Color = Color.Blue;
            
			//QueryTimer dwell = new QueryTimer();
			//Random random = new Random();
			//int rndMin, rndMax;
			//dwell.Reset();
			//while (dwell.Elapsed < 5000)
			//{
			//    rndMin = (int)dwell.Elapsed - 200;
			//    rndMax = (int)dwell.Elapsed + 200;
			//    EVENT.addLoadcellData(0, dwell.Elapsed, random.Next(rndMin, rndMax) / 1000.0, random.Next(rndMin, rndMax) / 1000.0);
			//    Thread.Sleep(50);
			//}

			//controlLoadcellData(1);
		}
	}
}
