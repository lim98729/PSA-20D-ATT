namespace AccessoryLibrary
{
	partial class loadScope
	{
		/// <summary> 
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region 구성 요소 디자이너에서 생성한 코드

		/// <summary> 
		/// 디자이너 지원에 필요한 메서드입니다. 
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
			this.loadcellChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
			((System.ComponentModel.ISupportInitialize)(this.loadcellChart)).BeginInit();
			this.SuspendLayout();
			// 
			// loadcellChart
			// 
			this.loadcellChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			chartArea1.AxisX.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Seconds;
			chartArea1.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Number;
			chartArea1.AxisY.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
			chartArea1.Name = "ChartArea1";
			this.loadcellChart.ChartAreas.Add(chartArea1);
			this.loadcellChart.Location = new System.Drawing.Point(0, 0);
			this.loadcellChart.Name = "loadcellChart";
			series1.ChartArea = "ChartArea1";
			series1.Name = "Series1";
			series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
			series2.ChartArea = "ChartArea1";
			series2.Name = "Series2";
			series3.ChartArea = "ChartArea1";
			series3.Name = "Series3";
			series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
			series4.ChartArea = "ChartArea1";
			series4.Name = "Series4";
			this.loadcellChart.Series.Add(series1);
			this.loadcellChart.Series.Add(series2);
			this.loadcellChart.Series.Add(series3);
			this.loadcellChart.Series.Add(series4);
			this.loadcellChart.Size = new System.Drawing.Size(343, 325);
			this.loadcellChart.TabIndex = 0;
			// 
			// loadScope
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.loadcellChart);
			this.Name = "loadScope";
			this.Size = new System.Drawing.Size(343, 325);
			this.Load += new System.EventHandler(this.loadScope_Load);
			((System.ComponentModel.ISupportInitialize)(this.loadcellChart)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataVisualization.Charting.Chart loadcellChart;
	}
}
