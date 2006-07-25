using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using urakawa.core;

namespace Obi.UserControls
{
    public partial class SectionStrip : UserControl
    {
        private StripManagerPanel mManager;  // the manager for this strip
        private CoreNode mNode;              // the core node for this strip

        public string Label
        {
            get
            {
                return mLabel.Text;
            }
            set
            {
                mLabel.Text = value;
            }
        }

        public StripManagerPanel Manager
        {
            set
            {
                mManager = value;
            }
        }

        public CoreNode Node
        {
            get
            {
                return mNode;
            }
            set
            {
                mNode = value;
            }
        }

        public SectionStrip()
        {
            InitializeComponent();
        }

        private void SectionStrip_Click(object sender, EventArgs e)
        {
            mManager.SelectedNode = mNode;
            Console.WriteLine("Selected {0}", mLabel);
        }

        public void MarkSelected()
        {
            BackColor = Color.Gold;
            Console.WriteLine("{0} becomes gold!", mLabel);
        }

        public void MarkDeselected()
        {
            BackColor = Color.PaleGreen;
            Console.WriteLine("{0} becomes green!", mLabel);
        }

        /// <summary>
        /// The strip has a normally invisible text box aligned with the label.
        /// When renaming, the text box is shown and initialized with the original label.
        /// The whole text is selected and the text box is given the focus so that the
        /// user can start editing right away.
        /// </summary>
        public void StartRenaming()
        {
            mTextBox.BackColor = BackColor;
            mTextBox.Text = "";
            mTextBox.SelectedText = mLabel.Text;
            mTextBox.Visible = true;
            mFlowLayoutPanel.Focus();
            mTextBox.Focus();
        }

        /// <summary>
        /// Leaving the text box updates the text property.
        /// </summary>
        private void mTextBox_Leave(object sender, EventArgs e)
        {
            UpdateText();
        }

        /// <summary>
        /// Typing return updates the text property; escape cancels the edit.
        /// </summary>
        private void mTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    UpdateText();
                    break;
                case Keys.Escape:
                    mTextBox.Visible = false;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Upate the text label from the text box input.
        /// If the input is empty, then do not change the text and warn the user.
        /// The manager is then asked to send a rename event.
        /// </summary>
        private void UpdateText()
        {
            mTextBox.Visible = false;
            if (mTextBox.Text != "")
            {
                mLabel.Text = mTextBox.Text;
                mManager.RenamedSectionStrip(this);
            }
            else
            {
                MessageBox.Show(Localizer.Message("empty_label_warning_text"),
                    Localizer.Message("empty_label_warning_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
