using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.UserControls
{
    public partial class AnnotationBlock : AbstractBlock
    {
        private AudioBlock mAudioBlock;  // the corresponding audio block

        public AudioBlock AudioBlock
        {
            get { return mAudioBlock; }
            set { mAudioBlock = value; }
        }

       

        public string Label
        {
            get { return mLabel.Text; }
            set { mLabel.Text = value; }
        }

        //md temp
        public void SetLabel(string label)
        {
            mLabel.Text = label;
        }

        public int _Width
        {
            set { Size = new Size(value, Size.Height); }
        }

        public AnnotationBlock()
        {
            InitializeComponent();
            InitializeToolTips();
        }

        internal override void MarkDeselected()
        {
            BackColor = Color.LightYellow;
            mLabel.BackColor = BackColor;
        }

        internal override void MarkSelected()
        {
            BackColor = Color.Yellow;
            mLabel.BackColor = BackColor;
        }

        private void mLabel_Click(object sender, EventArgs e)
        {
            mAudioBlock.Manager.SelectedPhraseNode = mAudioBlock.Node;
        }

        private void mRenameBox_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void mRenameBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    UpdateText();
                    break;
                case Keys.Escape:
                    mRenameBox.Visible = false;
                    break;
                default:
                    break;
            }
        }

        private void AnnotationBlock_Click(object sender, EventArgs e)
        {

        }

        public void StartRenaming()
        {
            mRenameBox.Size = mLabel.Size;
            mRenameBox.BackColor = BackColor;
            mRenameBox.Text = "";
            mRenameBox.SelectedText = mLabel.Text;
            mRenameBox.Visible = true;
            mRenameBox.Focus();
        }

        /// <summary>
        /// Upate the text label from the text box input.
        /// If the input is empty then do not change the text and warn the user.
        /// If the input text is the same as the original text, don't do anything.
        /// The manager is then asked to send a rename event.
        /// The rename event may not happen if there is already an audio block with the same name in the project.
        /// </summary>
        public void UpdateText()
        {
            mRenameBox.Visible = false;
            if (mRenameBox.Text != "" && mRenameBox.Text != mLabel.Text)
            {
                mManager.EditedAudioBlockLabel(this.mAudioBlock, mRenameBox.Text);
            }
            else
            {
                MessageBox.Show(Localizer.Message("empty_label_warning_text"),
                    Localizer.Message("empty_label_warning_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void mRenameBox_Leave(object sender, EventArgs e)
        {
            mRenameBox.Visible = false;
        }

        //md 20061009
        private void InitializeToolTips()
        {
            this.mToolTip.SetToolTip(this, Localizer.Message("annotation_tooltip"));
            this.mToolTip.SetToolTip(this.mRenameBox, Localizer.Message("annotation_tooltip"));
            this.mToolTip.SetToolTip(this.mLabel, Localizer.Message("annotation_tooltip"));

        }

    }
}
