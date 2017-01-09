using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using PSA_SystemLibrary;

namespace PSA_Application
{
    public partial class FormProtecBackup : Form
    {
        public FormProtecBackup(string src_, string dest_)
        {
            src = src_; dest = dest_;
            tmpsrc = src_; tmpdest = dest_;
            InitializeComponent();
        }

        public delegate void SetProgCallBack(int vv);
        public delegate void SetLabelCallBack();
        public delegate void ExitCallback();

        string src, dest, tmpsrc, tmpdest;
        int srcSize;
        int vv = 1;

        Thread th;

        private void FormProtecBackup_Load(object sender, EventArgs e)
        {
            countFolder(src);
            progressBar1.Maximum = srcSize;

            th = new Thread(CopyFolder);
            th.Start();
        }

        public void countFolder(string sourceFolder)
        {
            string[] files = Directory.GetFiles(sourceFolder);
            string[] folders = Directory.GetDirectories(sourceFolder);

            foreach (string file in files) srcSize++;

            foreach (string folder in folders)
            {
                if (folder == "C:\\PROTEC\\Log") continue;
                string name = Path.GetFileName(folder);
                countFolder(folder);
            }
        }

        public void CopyFolder()
        {
            try
            {
                string destFolder = tmpdest;
                string sourceFolder = tmpsrc;

                if (!Directory.Exists(destFolder))
                    Directory.CreateDirectory(destFolder);

                string[] files = Directory.GetFiles(sourceFolder);
                string[] folders = Directory.GetDirectories(sourceFolder);

                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    string dest = Path.Combine(destFolder, name);
                    File.Copy(file, dest);
                    Thread.Sleep(10);
                    SetProgBar(vv);
                    SetLabel();
                    vv++;

                    if (vv == srcSize)
                    {
                        Thread.Sleep(1000);
                        this.Exit();
                    }
                }

                foreach (string folder in folders)
                {
                    if (folder == "C:\\PROTEC\\Log") continue;
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name);
                    tmpsrc = folder; tmpdest = dest;
                    CopyFolder();
                }
            }
            catch (Exception ex)
            {
                mc.message.alarm(ex.Message, "Backup Error");
            }
        }

        private void SetProgBar(int Test)
        {
            if (this.progressBar1.InvokeRequired)
            {
                SetProgCallBack dele = new SetProgCallBack(SetProgBar);
                this.Invoke(dele, new object[] { Test });
            }
            else
                this.progressBar1.Value = Test;
        }

        private void SetLabel()
        {
            if (this.label1.InvokeRequired)
            {
                SetLabelCallBack dele = new SetLabelCallBack(SetLabel);
                this.Invoke(dele);
            }
            else
            {
                this.label1.Text = "Backup Files..";
            }
        }

        private void Exit()
        {
            ExitCallback dele = new ExitCallback(Close);
            this.Invoke(dele);
        }
    }
}
