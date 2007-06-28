using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Zaboom.UserControls
{
    public interface Selectable
    {
        bool Selected { get; set; }
    }

    public partial class AudioBlock : UserControl, Selectable
    {
        private bool selected;
        protected ProjectPanel panel;

        private static readonly int SELECT_TAB_WIDTH = 32;
        private static readonly Color BACK_COLOR_SELECTED = Color.Aquamarine;
        private static readonly Color BACK_COLOR_UNSELECTED = Color.RoyalBlue;

        public AudioBlock()
        {
            InitializeComponent();
            panel = null;
            Selected = false;
        }

        public ProjectPanel Panel
        {
            set
            {
                if (panel != null) throw new Exception("Panel is already set!");
                if (value == null) throw new urakawa.exception.MethodParameterIsNullException("Null panel!"); 
                panel = value;
            }
        }

        public WaveformPanel Waveform { get { return waveformPanel; } }

        private void AudioBlock_Click(object sender, EventArgs e)
        {
            Selected = !selected;
            if (selected)
            {
                panel.ReplaceSelection(this);
            }
            else
            {
                panel.SelectionChanged(this);
            }
        }

        private void waveformPanel_SizeChanged(object sender, EventArgs e)
        {
            Width = waveformPanel.Width + SELECT_TAB_WIDTH;
        }

        #region Selectable Members

        /// <summary>
        /// Used by the project panel to ask or tell the control when it selected or not.
        /// </summary>
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    BackColor = selected ? BACK_COLOR_SELECTED : BACK_COLOR_UNSELECTED;
                }
            }
        }

        #endregion
    }
}
