using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DefineLibrary;

namespace PSA_Application
{
    public partial class FormErrorReport : Form
    {
        public FormErrorReport()
        {
            InitializeComponent();
            ClassChangeLanguage.ChangeLanguage(this);
        }

        public struct ErrorReport
        {
            public string Time;
            public string Content;
            public string Cause;
            public string Action;
            public string Info;
            public string Code;
            public string Module;
            public int Count;
            public ErrorReport(string Time_, string Content_, string Cause_, string Action_, string Info_, string Code_, string Module_, int Count_)
            {
                Time = Time_;
                Content = Content_;
                Cause = Cause_;
                Action = Action_;
                Info = Info_;
                Code = Code_;
                Module = Module_;
                Count = Count_;
            }
        }
        List<ErrorReport> ErrorReportContent = new List<ErrorReport>();
        List<ErrorReport> ErrorReportContentStat = new List<ErrorReport>();

        public bool ErrorChartMode = false;
        public string StartDay = "";
        public string EndDay = "";
        public string Path = "C:\\PROTEC\\Log\\Error";

        public static string FileRead(string path)
        {
            StreamReader sr = new StreamReader(path);
            string sTemp = sr.ReadToEnd();
            sr.Close();
            return sTemp;
        }



        private void BT_ErrorReport_Click(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                MessageBox.Show(textResource.MB_ERROR_REPORT_DATE_ERROR);
                return;
            }
            foreach (DataGridViewColumn i in ErrorView.Columns)
            {
                i.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            ErrorView.Columns[0].HeaderText = textResource.TB_ERROR_REPORT_OCCURRED_TIME;

            StartDay = dateTimePicker1.Text;
            EndDay = dateTimePicker2.Text;

            textBox1.Text = "";
            ErrorChartMode = false;

            ErrorReportContent.Clear();
            ErrorReportContentStat.Clear();
            ErrorView.Rows.Clear();

            string a = StartDay.Substring(0, 4) + StartDay.Substring(5, 2) + StartDay.Substring(8, 2);
            string b = EndDay.Substring(0, 4) + EndDay.Substring(5, 2) + EndDay.Substring(8, 2);
            DirectoryInfo di = new DirectoryInfo(Path);

            foreach (FileInfo file in di.GetFiles())
            {
                string c = file.Name;
                string d = c.Substring(4, 8);
                ErrorReport tmpErrReport = new ErrorReport("", "", "", "", "", "", "", 1);
                ErrorReport tmpErrReportStat = new ErrorReport("", "", "", "", "", "", "", 1);
                if (Convert.ToInt32(d) >= Convert.ToInt32(a) && Convert.ToInt32(d) <= Convert.ToInt32(b))
                {
                    string filename = Path + "\\" + c;

                    string[] sTemp = FileRead(@filename).Replace("\n", "").Split('\r');

                    for (int i = 0; i < sTemp.Length; i++) // 파일 여러개도 에러별로 싹 배열에 집어넣음.
                    {
                        if (sTemp[i].Contains(textResource.TB_ERROR_REPORT_SW_MODULE))    // 시간, 에러내용, 발생원인, 조치사항, 부가정보, 에러코드, 축정보 
                        {
                            tmpErrReport.Info = "";
                            tmpErrReportStat.Info = "";
                            tmpErrReport.Time = sTemp[i].Substring(1, 14);
                            tmpErrReportStat.Time = sTemp[i].Substring(1, 14);
                            if (sTemp[i].Contains(textResource.TB_ERROR_REPORT_AXIS_NAME))
                            {
								if (sTemp[i].Contains("HOMING")) sTemp[i] = sTemp[i].Substring(36, sTemp[i].Length - 36);
								else if (sTemp[i].Contains("MPI")) sTemp[i] = sTemp[i].Substring(33, sTemp[i].Length - 33);
								else sTemp[i] = sTemp[i].Substring(32, sTemp[i].Length - 32);
                                string[] Temp = new string[2];
                                Temp = sTemp[i].Split(']');
                                tmpErrReport.Module = Temp[0];
                                tmpErrReportStat.Module = Temp[0];
                            }
                        }
                        else if (sTemp[i].Contains(textResource.TB_ERROR_REPORT_CONTENTS_OF_ERROR))
                        {
                            sTemp[i] = sTemp[i].Substring(8, sTemp[i].Length - 8);
                            tmpErrReport.Content = sTemp[i];
                            tmpErrReportStat.Content = sTemp[i];
                        }
                        else if (sTemp[i].Contains(textResource.TB_ERROR_REPORT_SOURCE))
                        {
                            sTemp[i] = sTemp[i].Substring(8, sTemp[i].Length - 8);
                            tmpErrReport.Cause = sTemp[i];
                            tmpErrReportStat.Cause = sTemp[i];
                        }
                        else if (sTemp[i].Contains(textResource.TB_ERROR_REPORT_SOLUTION))
                        {
                            sTemp[i] = sTemp[i].Substring(8, sTemp[i].Length - 8);
                            tmpErrReport.Action = sTemp[i];
                            tmpErrReportStat.Action = sTemp[i];
                        }
                        else if (sTemp[i].Contains(textResource.TB_ERROR_REPORT_ADDITIONAL_INFORMATION))
                        {
                            sTemp[i] = sTemp[i].Substring(8, sTemp[i].Length - 8);
                            tmpErrReport.Info = sTemp[i];
                            tmpErrReportStat.Info = sTemp[i];
                        }
                        else if (sTemp[i].Contains(textResource.TB_ERROR_REPORT_ERRORCODE))
                        {
                            sTemp[i] = sTemp[i].Substring(8, sTemp[i].Length - 8);
                            tmpErrReport.Code = sTemp[i];
                            tmpErrReportStat.Code = sTemp[i];
                            ErrorReportContent.Add(tmpErrReport);
                            ErrorReportContentStat.Add(tmpErrReportStat);
                            tmpErrReport = new ErrorReport("", "", "", "", "", "", "", 1);
                            tmpErrReportStat = new ErrorReport("", "", "", "", "", "", "", 1);

                        }
                    }
                }
            }

            for (int i = 0; i < ErrorReportContent.Count; i++) // 에러 갯수만큼 행 늘리고
            {
                ErrorView.Rows.Add();

                for (int j = 0; j < ErrorView.Columns.Count; j++)
                {
                    ErrorView.Rows[i].Cells[j].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    if (j == 0)
                    {
                        ErrorView.Rows[i].Cells[j].Value = ErrorReportContent[i].Time;
                    }
                    else if (j == 1)
                    {
                        if (ErrorReportContent[i].Content == "") ErrorView.Rows[i].Cells[j].Value = "Not Contents";
                        else ErrorView.Rows[i].Cells[j].Value = ErrorReportContent[i].Content;
                    }
                    else if (j == 2)
                    {
                        if (ErrorReportContent[i].Module == "") ErrorView.Rows[i].Cells[j].Value = "RunTime - Error";
                        else ErrorView.Rows[i].Cells[j].Value = ErrorReportContent[i].Module;
                    }

                }
            }
        }

        private void BT_ErrorChart_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn i in ErrorView.Columns)
            {
                i.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            ErrorView.Columns[0].HeaderText = "Counts";

            if (ErrorReportContent.Count == 0) return;
            textBox1.Text = "";
            ErrorChartMode = true;

            ErrorReport tmp;
            ErrorView.Rows.Clear();



            for (int i = 0; i < ErrorReportContentStat.Count; i++)
            {
                for (int j = 0; j < ErrorReportContentStat.Count; j++)
                {
                    if (i == j || ErrorReportContentStat[j].Count == 0 || ErrorReportContentStat[i].Count == 0) continue;
                    if (ErrorReportContentStat[i].Code == ErrorReportContentStat[j].Code)
                    {
                        tmp = ErrorReportContentStat[i];
                        tmp.Count++;
                        ErrorReportContentStat[i] = tmp;

                        tmp = ErrorReportContentStat[j];
                        tmp.Count--;
                        ErrorReportContentStat[j] = tmp;
                    }
                }
            }

            for (int i = ErrorReportContentStat.Count - 1; i >= 0; i--)
            {
                if (ErrorReportContentStat[i].Count == 0) ErrorReportContentStat.RemoveAt(i);
            }

            for (int i = 0; i < ErrorReportContentStat.Count; i++)
            {
                ErrorView.Rows.Add();

                for (int j = 0; j < ErrorView.Columns.Count; j++)
                {
                    ErrorView.Rows[i].Cells[j].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    if (j == 0)
                    {
                        //ErrorView.Rows[i].Cells[0].EditType = typeof(int);
                        ErrorView.Rows[i].Cells[j].Value = ErrorReportContentStat[i].Count;
                        if (ErrorView.Rows[i].Cells[j].Value.ToString() == "") ErrorView.Rows[i].Cells[j].Value = "No-Value";
                    }
                    else if (j == 1)
                    {
                        ErrorView.Rows[i].Cells[j].Value = ErrorReportContentStat[i].Content;
                        if (ErrorView.Rows[i].Cells[j].Value.ToString() == "") ErrorView.Rows[i].Cells[j].Value = "No-Content";
                    }
                    else if (j == 2)
                    {
                        ErrorView.Rows[i].Cells[j].Value = ErrorReportContentStat[i].Module;
                        if (ErrorView.Rows[i].Cells[j].Value.ToString() == "") ErrorView.Rows[i].Cells[j].Value = "RunTime-Error";
                    }
                }
            }
        }
 
        private void ErrorView_KeyUp(object sender, KeyEventArgs e)
        {
            if (StartDay == "" || EndDay == "") return;

            if (!ErrorChartMode)
            {
                for (int i = 0; i < ErrorView.Rows.Count; i++)
                {
                    if (ErrorView.Rows[i].Selected == true)
                    {
                        textBox1.Text = "▶ " + textResource.TB_ERROR_REPORT_OCCURRED_TIME + " : " + ErrorReportContent[i].Cause + "\r\n" +
                                        "▶ " + textResource.TB_ERROR_REPORT_CONTENTS_OF_ERROR + " " + ErrorReportContent[i].Action + "\r\n" +
                                        "▶ " + textResource.TB_ERROR_REPORT_ADDITIONAL_INFORMATION + " " + ErrorReportContent[i].Info + "\r\n" +
                                        "▶ " + textResource.TB_ERROR_REPORT_ERRORCODE + " " + ErrorReportContent[i].Code;
                    }
                }
            }
            else
            {
                for (int i = 0; i < ErrorView.Rows.Count; i++)
                {
                    if (ErrorView.Rows[i].Selected == true)
                    {
                        textBox1.Text = textResource.TB_ERROR_REPORT_OCCURRED_TIME + "\r\n";
                        for (int z = 0; z < ErrorReportContent.Count; z++)
                        {
                            if (ErrorView.Rows[i].Cells[1].Value.ToString() == ErrorReportContent[z].Content.ToString())
                            {
                                textBox1.Text += (ErrorReportContent[z].Time) + "\r\n";
                            }
                        }
                    }
                }
            }
        }
        private void ErrorView_CellMouseClick(object sender, MouseEventArgs e)
        {
            if (StartDay == "" || EndDay == "") return;

            if (!ErrorChartMode)
            {
                for (int i = 0; i < ErrorView.Rows.Count; i++)
                {
                    if (ErrorView.Rows[i].Selected == true)
                    {
                        textBox1.Text = "▶ " + textResource.TB_ERROR_REPORT_OCCURRED_TIME + " : " + ErrorReportContent[i].Cause + "\r\n" +
                                        "▶ " + textResource.TB_ERROR_REPORT_CONTENTS_OF_ERROR + " " + ErrorReportContent[i].Action + "\r\n" +
                                        "▶ " + textResource.TB_ERROR_REPORT_ADDITIONAL_INFORMATION + " " + ErrorReportContent[i].Info + "\r\n" +
                                        "▶ " + textResource.TB_ERROR_REPORT_ERRORCODE + " " + ErrorReportContent[i].Code;
                    }
                }
            }
            else
            {
                for (int i = 0; i < ErrorView.Rows.Count; i++)
                {
                    if (ErrorView.Rows[i].Selected == true)
                    {
                        textBox1.Text = textResource.TB_ERROR_REPORT_OCCURRED_TIME + "\r\n";
                        for (int z = 0; z < ErrorReportContent.Count; z++)
                        {
                            if (ErrorView.Rows[i].Cells[1].Value.ToString() == ErrorReportContent[z].Content.ToString())
                            {
                                textBox1.Text += (ErrorReportContent[z].Time) + "\r\n";
                            }
                        }
                    }
                }
            }
        }
        private void FormErrorReport_Load(object sender, EventArgs e)
        {
            ErrorView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dateTimePicker1.Text = DateTime.Now.ToString();
            dateTimePicker2.Text = DateTime.Now.ToString();
        }

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}