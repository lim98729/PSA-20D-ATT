using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DefineLibrary;
using System.Threading;
using System.Windows.Input;

namespace AccessoryLibrary
{
    public partial class FormParaSetting : Form
    {
        public FormParaSetting()
        {
            InitializeComponent();
        }

        public ListViewClass paraObject;
        ListViewClass _paraObject;

        private void btnCancel_Click(object sender, EventArgs e)
        {
            paraObject = _paraObject;
            timer1.Enabled = false;
            
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            this.Close();
        }

        private void FormParaSetting_Load(object sender, EventArgs e)
        {
            _paraObject = paraObject;
            refresh(paraObject);
        
            timer1.Enabled = true;
            TB_Set_Data.Select();
            TB_Set_Data.Select(sData_Display.Length, 0);
        }
        void refresh(ListViewClass para)
        {
            this.Text = para.Name;
            TB_Get_Data.Text = para.Value.ToString();
            TB_Get_Pre_Data.Text = para.PreValue.ToString();
            TB_Lower_Limit.Text = para.LowerLimit.ToString();
            TB_Upper_Limit.Text = para.UpperLimit.ToString();
            TB_Description.Text = para.Description;
            TB_Set_Data.Text = sData_Display = "0";
        }


        //double dGet_data;
        //double dSet_data;

        //int Pos_Left, Pos_Top;
        string sData_Display;

        double dData_Display;


        private void Control_Click(object sender, EventArgs e)
        {
            try
            {
                if (TB_Set_Data.ForeColor == Color.Red)
                {
                    sData_Display = "";
                    TB_Set_Data.ForeColor = Color.Black;
                }
                if (TB_Set_Data.Text == "0")
                {
                    if (sender != BT_Dot) sData_Display = "";
                }
                if (sender == BT_0) sData_Display += "0";
                else if (sender == BT_1) sData_Display += "1";
                else if (sender == BT_2) sData_Display += "2";
                else if (sender == BT_3) sData_Display += "3";
                else if (sender == BT_4) sData_Display += "4";
                else if (sender == BT_5) sData_Display += "5";
                else if (sender == BT_6) sData_Display += "6";
                else if (sender == BT_7) sData_Display += "7";
                else if (sender == BT_8) sData_Display += "8";
                else if (sender == BT_9) sData_Display += "9";
                else if (sender == BT_Dot)
                {
                    if (Convert.ToDouble(sData_Display) == 0) sData_Display = "0.";
                    else if (Convert.ToDouble(sData_Display) % Convert.ToInt32(sData_Display) == 0) sData_Display += ".";
                }
                else if (sender == BT_Backspace)
                {
                    if (sData_Display != "")
                    {
                        sData_Display = sData_Display.Remove(sData_Display.Length - 1, 1);//(sData_Display.Length - 2);
                        if (sData_Display == "" || sData_Display == "-") sData_Display = "0";
                    }
                }
                else if (sender == BT_Plus_Minus)
                {
                    dData_Display = Convert.ToDouble(sData_Display);
                    dData_Display *= -1;
					sData_Display = dData_Display.ToString();
                }
                if (sData_Display == "") sData_Display = "0";
        
                
                TB_Set_Data.Text = sData_Display.ToString();
                dData_Display = Convert.ToDouble(sData_Display);

                if (sender != TB_Set_Data)
                {
                    TB_Set_Data.Select();
                    TB_Set_Data.Select(sData_Display.Length, 0);
                }
           }
            catch
            {
                TB_Set_Data.Text = "Error";
                TB_Set_Data.ForeColor = Color.Red;
                TB_Set_Data.Select();
                TB_Set_Data.Select(sData_Display.Length, 0);
            }
        }

        private void BT_Reset_Click(object sender, EventArgs e)
        {
            TB_Set_Data.ForeColor = Color.Black;
            sData_Display = "0";
            TB_Set_Data.Text = sData_Display;

            TB_Set_Data.Select();
        }

        private void BT_Set_Click(object sender, EventArgs e)
        {
            try
            {
                dData_Display = Convert.ToDouble(sData_Display);

                double data = dData_Display;
                if (data < paraObject.LowerLimit || data > paraObject.UpperLimit)
                {
                    TB_Set_Data.Text = "Out Of Range";
                    TB_Set_Data.ForeColor = Color.Red;
                    return;
                }
                if (_paraObject.Value != data) paraObject.Value = data;
                Thread.Sleep(200);
                timer1.Enabled = false;
                this.Close();
            }
            catch
            {
                TB_Set_Data.Text = "Error";
                TB_Set_Data.ForeColor = Color.Red;
            }         
        }

        private void BT_ESC_Click(object sender, EventArgs e)
        {
            paraObject = _paraObject;
            timer1.Enabled = false;
            this.Close();
        }

        private void updateValue(object sender, EventArgs e)
        {
            try 
            {
                sData_Display = TB_Set_Data.Text;
            }
            catch
            {
                TB_Set_Data.Text = "Error";
                TB_Set_Data.ForeColor = Color.Red;
            }
        }

        private void Key_Press(object sender, KeyPressEventArgs e)
        {
            try
            {
                if( TB_Set_Data.Text == "Error" || TB_Set_Data.Text == "Out Of Range")
                {
                    TB_Set_Data.ForeColor = Color.Black;
                    sData_Display = "";
                }

                if (e.KeyChar >= '0' && e.KeyChar <= '9')
                {
                    if (sData_Display == "0") sData_Display = e.KeyChar.ToString();
                    else if(sData_Display == "-0") sData_Display = "-" + e.KeyChar.ToString();
                    else sData_Display += e.KeyChar;
                }

                if (e.KeyChar == (char)Keys.Enter)
                {
                    dData_Display = Convert.ToDouble(sData_Display);

                    double data = dData_Display;
                    if (data < paraObject.LowerLimit || data > paraObject.UpperLimit)
                    {
                        TB_Set_Data.Text = "Out Of Range";
                        TB_Set_Data.ForeColor = Color.Red;
                        TB_Set_Data.Select();
                        TB_Set_Data.Select(sData_Display.Length, 0);
                        return;
                    }
                    if (_paraObject.Value != data) paraObject.Value = data;
                    Thread.Sleep(200);
                    timer1.Enabled = false;
                    this.Close();
                }
                if (e.KeyChar == (char)Keys.Escape)
                {
                    paraObject = _paraObject;
                    timer1.Enabled = false;
                    this.Close();
                }
                if (e.KeyChar == '-')
                {
                    if ('-'.ToString() == sData_Display.Substring(0, 1))
                    {
                        sData_Display = sData_Display.Substring(1);
                    }
                    else
                    {
                        sData_Display = "-" + sData_Display;
                    }
                }
                if (e.KeyChar == (char)Keys.Back)
                {
                    if (sData_Display != "0")
                    {
                        if (sData_Display.Length == 1) sData_Display = "0";
                        else sData_Display = sData_Display.Substring(0, sData_Display.Length - 1);
                    }
                }
                if (e.KeyChar == '.')
                {
                    if (sData_Display == "0")
                    {
                        sData_Display = "0.";
                    }
                    else sData_Display = sData_Display + ".";
                }

                TB_Set_Data.Text = sData_Display.ToString();
                TB_Set_Data.Select();
                TB_Set_Data.Select(sData_Display.Length, 0);
            }
            catch (System.Exception ex)
            {
                TB_Set_Data.Text = "Error";
                TB_Set_Data.ForeColor = Color.Red;
            }
        }
    }
}
