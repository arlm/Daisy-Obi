using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class Preferences : Form
    {
        private string mIdTemplate;  // identifier template
        private string mDefaultDir;  // default project directory

        public string IdTemplate { get { return mIdTemplate; } }
        public string DefaultDir { get { return mDefaultDir; } }

        public Preferences(string template, string dir)
        {
            InitializeComponent();
            mIdTemplate = template;
            mTemplateBox.Text = mIdTemplate;
            mDefaultDir = dir;
            mDirectoryBox.Text = mDefaultDir;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = mDefaultDir;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                mDefaultDir = dialog.SelectedPath;
                mDirectoryBox.Text = mDefaultDir;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mIdTemplate = mTemplateBox.Text;
            mDefaultDir = mDirectoryBox.Text;
        }
    }
}