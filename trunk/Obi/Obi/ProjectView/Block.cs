using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class Block : UserControl, ISelectableInStripView
    {
        private PhraseNode mNode;  // the corresponding node
        private Strip mStrip;      // the parent strip
        private bool mSelected;    // selected flag

        private static readonly float AUDIO_SCALE = 0.01f;

        public Block(PhraseNode node, Strip strip): this()
        {
            mNode = node;
            mStrip = strip;
            mSelected = false;
            if (node.Audio != null)
            {
                long time = node.Audio.getDuration().getTimeDeltaAsMilliseconds();
                mWaveform.Width = (int)Math.Round(time * AUDIO_SCALE);
                mWaveform.Media = node.Audio.getMediaData();
                Size = new Size(mWaveform.Width + mWaveform.Margin.Right + mWaveform.Margin.Left, Height);
            }
            else
            {
                mWaveform.Visible = false;
            }
        }

        public Block() { InitializeComponent(); }


        /// <summary>
        /// The phrase node for this block.
        /// </summary>
        public PhraseNode Node { get { return mNode; } }
        public ObiNode ObiNode { get { return mNode; } }

        /// <summary>
        /// Set the selected flag for the block.
        /// </summary>
        public bool Selected
        {
            get { return mSelected; }
            set
            {
                mSelected = value;
                BackColor = mSelected ? Color.Yellow : Color.HotPink;
            }
        }


        // Select on click
        private void Block_Click(object sender, EventArgs e) { mStrip.SelectedBlock = this; }
        private void mWaveform_Click(object sender, EventArgs e) { mStrip.SelectedBlock = this; }
    }
}
