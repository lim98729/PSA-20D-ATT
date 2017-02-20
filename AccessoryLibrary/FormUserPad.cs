using System;
using System.Windows.Forms;
using DefineLibrary;

namespace AccessoryLibrary
{
    public partial class FormUserPad : Form
    {
        public string ID
        {
            get
            {
                return PadStr;
            }
        }

        private string PadStr = "";
        private Timer tm;
        private bool SmallLetter = true;

        public FormUserPad()
        {
            InitializeComponent();
            tm = new Timer();
            tm.Interval = 200;
            tm.Tick += new EventHandler(timer_tick);
            tm.Start();
        }

        public FormUserPad(string value)
        {
            InitializeComponent();
            PadStr = value;
            tm = new Timer();
            tm.Interval = 200;
            tm.Tick += new EventHandler(timer_tick);
            tm.Start();
        }

        public void setTitle(string str)
        {
            Text = str;
        }

        private void timer_tick(object sender, System.EventArgs e)
        {
            tBox_StrPad.Text = PadStr;

            if (SmallLetter)
            {
                btn_A.Text = "a";
                btn_B.Text = "b";
                btn_C.Text = "c";
                btn_D.Text = "d";
                btn_E.Text = "e";
                btn_F.Text = "f";
                btn_G.Text = "g";
                btn_H.Text = "h";
                btn_I.Text = "i";
                btn_J.Text = "j";
                btn_K.Text = "k";
                btn_L.Text = "l";
                btn_M.Text = "m";
                btn_N.Text = "n";
                btn_O.Text = "o";
                btn_P.Text = "p";
                btn_Q.Text = "q";
                btn_R.Text = "r";
                btn_S.Text = "s";
                btn_T.Text = "t";
                btn_U.Text = "u";
                btn_V.Text = "v";
                btn_W.Text = "w";
                btn_X.Text = "x";
                btn_Y.Text = "y";
                btn_Z.Text = "z";
            }
            else
            {
                btn_A.Text = "A";
                btn_B.Text = "B";
                btn_C.Text = "C";
                btn_D.Text = "D";
                btn_E.Text = "E";
                btn_F.Text = "F";
                btn_G.Text = "G";
                btn_H.Text = "H";
                btn_I.Text = "I";
                btn_J.Text = "J";
                btn_K.Text = "K";
                btn_L.Text = "L";
                btn_M.Text = "M";
                btn_N.Text = "N";
                btn_O.Text = "O";
                btn_P.Text = "P";
                btn_Q.Text = "Q";
                btn_R.Text = "R";
                btn_S.Text = "S";
                btn_T.Text = "T";
                btn_U.Text = "U";
                btn_V.Text = "V";
                btn_W.Text = "W";
                btn_X.Text = "X";
                btn_Y.Text = "Y";
                btn_Z.Text = "Z";
            }
        }

        public string GetString()
        {
            return PadStr;
        }

        private void FEngPad_Load(object sender, EventArgs e)
        {
            tBox_StrPad.Focus();
            tBox_StrPad.Select();

            if (Control.IsKeyLocked(Keys.CapsLock))
            {
                SmallLetter = false;
            }
            else SmallLetter = true;
        }

        private void Control_Click(object sender, EventArgs e)
        {
            if (sender.Equals(btn_N0)) PadStr = PadStr + "0";
            else if (sender.Equals(btn_N1)) PadStr = PadStr + "1";
            else if (sender.Equals(btn_N2)) PadStr = PadStr + "2";
            else if (sender.Equals(btn_N3)) PadStr = PadStr + "3";
            else if (sender.Equals(btn_N4)) PadStr = PadStr + "4";
            else if (sender.Equals(btn_N5)) PadStr = PadStr + "5";
            else if (sender.Equals(btn_N6)) PadStr = PadStr + "6";
            else if (sender.Equals(btn_N7)) PadStr = PadStr + "7";
            else if (sender.Equals(btn_N8)) PadStr = PadStr + "8";
            else if (sender.Equals(btn_N9)) PadStr = PadStr + "9";
            else if (sender.Equals(btn_Dash)) PadStr = PadStr + "-";
            else if (sender.Equals(btn_A)) PadStr = PadStr + btn_A.Text;
            else if (sender.Equals(btn_B)) PadStr = PadStr + btn_B.Text;
            else if (sender.Equals(btn_C)) PadStr = PadStr + btn_C.Text;
            else if (sender.Equals(btn_D)) PadStr = PadStr + btn_D.Text;
            else if (sender.Equals(btn_E)) PadStr = PadStr + btn_E.Text;
            else if (sender.Equals(btn_F)) PadStr = PadStr + btn_F.Text;
            else if (sender.Equals(btn_G)) PadStr = PadStr + btn_G.Text;
            else if (sender.Equals(btn_H)) PadStr = PadStr + btn_H.Text;
            else if (sender.Equals(btn_I)) PadStr = PadStr + btn_I.Text;
            else if (sender.Equals(btn_J)) PadStr = PadStr + btn_J.Text;
            else if (sender.Equals(btn_K)) PadStr = PadStr + btn_K.Text;
            else if (sender.Equals(btn_L)) PadStr = PadStr + btn_L.Text;
            else if (sender.Equals(btn_M)) PadStr = PadStr + btn_M.Text;
            else if (sender.Equals(btn_N)) PadStr = PadStr + btn_N.Text;
            else if (sender.Equals(btn_O)) PadStr = PadStr + btn_O.Text;
            else if (sender.Equals(btn_P)) PadStr = PadStr + btn_P.Text;
            else if (sender.Equals(btn_Q)) PadStr = PadStr + btn_Q.Text;
            else if (sender.Equals(btn_R)) PadStr = PadStr + btn_R.Text;
            else if (sender.Equals(btn_S)) PadStr = PadStr + btn_S.Text;
            else if (sender.Equals(btn_T)) PadStr = PadStr + btn_T.Text;
            else if (sender.Equals(btn_U)) PadStr = PadStr + btn_U.Text;
            else if (sender.Equals(btn_V)) PadStr = PadStr + btn_V.Text;
            else if (sender.Equals(btn_W)) PadStr = PadStr + btn_W.Text;
            else if (sender.Equals(btn_X)) PadStr = PadStr + btn_X.Text;
            else if (sender.Equals(btn_Y)) PadStr = PadStr + btn_Y.Text;
            else if (sender.Equals(btn_Z)) PadStr = PadStr + btn_Z.Text;
            else if (sender.Equals(btn_Dot)) PadStr = PadStr + btn_Dot.Text;
            else if (sender.Equals(btn_Close))
            {
                tm.Stop();
                this.Close();
            }
            else if (sender.Equals(btn_Reset)) PadStr = "";
            else if (sender.Equals(btn_Space)) { if (PadStr.Length != 0) PadStr = PadStr + " "; }
            else if (sender.Equals(btn_Enter))
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else if (sender.Equals(btn_Backspace))
            {
                char[] temp = PadStr.ToCharArray();

                if (0 == temp.Length)
                {
                    PadStr = "";
                    return;
                }
                temp[temp.Length - 1] = '/';
                PadStr = new string(temp);
                string[] tmp = PadStr.Split('/');
                PadStr = tmp[0];
            }
            else if (sender.Equals(Btn_Shift))
            {
                if (SmallLetter == true) SmallLetter = false;
                else SmallLetter = true;
            }
        }

        private void FEngPad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= '0' && e.KeyChar <= '9') PadStr += e.KeyChar;

            if (e.KeyChar >= 'a' && e.KeyChar <= 'z') PadStr += e.KeyChar;

            if (e.KeyChar >= 'A' && e.KeyChar <= 'Z') PadStr += e.KeyChar;

            if (e.KeyChar == '-') PadStr += e.KeyChar;

            if (e.KeyChar == '.') PadStr += e.KeyChar;

            if (e.KeyChar == (char)Keys.Enter)
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }

            if (e.KeyChar == (char)Keys.Escape)
            {
                tm.Stop();
                this.Close();
            }

            if (e.KeyChar == (char)Keys.Space) PadStr += " ";

            if (e.KeyChar == (char)Keys.Back)
            {
                char[] temp = PadStr.ToCharArray();

                if (0 == temp.Length)
                {
                    PadStr = "";
                    return;
                }
                temp[temp.Length - 1] = '/';
                PadStr = new string(temp);
                string[] tmp = PadStr.Split('/');
                PadStr = tmp[0];
            }
        }

        private void FormUserPad_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.CapsLock)
            {
                if (SmallLetter == true) SmallLetter = false;
                else SmallLetter = true;
            }
        }
    }
}