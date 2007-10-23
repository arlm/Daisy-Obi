using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class Strip : UserControl, ISearchable, ISelectableInStripView
    {
        private SectionNode mNode;       // the section node for this strip
        private bool mSelected;          // selected flag
        private StripsView mParentView;  // parent strip view

        /// <summary>
        /// This constructor is used by the designer.
        /// </summary>
        public Strip()
        {
            InitializeComponent();
            mLabel.FontSize = 18.0F;
            mNode = null;
            mLabel.LabelEditedByUser += new EventHandler(delegate(object sender, EventArgs e)
            {
                if (mLabel.Label != "")
                {
                    mParentView.RenameStrip(this);
                }
                else
                {
                    mLabel.Label = mNode.Label;
                }
            });
            Selected = false;
        }

        /// <summary>
        /// Create a new strip with an associated section node.
        /// </summary>
        public Strip(SectionNode node, StripsView parent): this()
        {
            if (node == null) throw new Exception("Cannot set a null section node for a strip!");
            mNode = node;
            Label = mNode.Label;
            mParentView = parent;
        }


        /// <summary>
        /// Add a new block for a phrase node.
        /// </summary>
        public Block AddBlockForPhrase(PhraseNode phrase)
        {
            Block block = new Block(phrase, this);
            mBlocksPanel.Controls.Add(block);
            mBlocksPanel.Controls.SetChildIndex(block, phrase.Index);
            return block;
        }

        /// <summary>
        /// The label of the strip (i.e. the title of the section; editable.)
        /// </summary>
        public string Label
        {
            get { return mLabel.Label; }
            set { mLabel.Label = value; }
        }

        /// <summary>
        /// The section node for this strip.
        /// </summary>
        public SectionNode Node { get { return mNode; } }
        public ObiNode ObiNode { get { return mNode; } }

        /// <summary>
        /// Set the selected flag for the strip. This just tells the strip that it is selected.
        /// </summary>
        public bool Selected
        {
            get { return mSelected; }
            set
            {
                mSelected = value;
                BackColor = mSelected ? Color.Yellow : Color.LightSkyBlue;
            }
        }

        /// <summary>
        /// Select a block in the strip.
        /// </summary>
        public Block SelectedBlock { set { mParentView.Selection = new NodeSelection(value.Node, mParentView, false); } }

        /// <summary>
        /// Start renaming the strip.
        /// </summary>
        public void StartRenaming() { mLabel.Editable = true; }


        // Resize the strip according to the editable label, whose size can change.
        // TODO since there are really two possible heights, we should cache these values.
        private void mLabel_SizeChanged(object sender, EventArgs e)
        {
            mBlocksPanel.Location = new Point(mBlocksPanel.Location.X,
                mLabel.Location.Y + mLabel.Height + mLabel.Margin.Bottom);
            Size = new Size(Width,
                mBlocksPanel.Location.Y + mBlocksPanel.Height + mBlocksPanel.Margin.Bottom);
        }

        // The user clicked on this strip, so select it if it wasn't already selected
        private void Strip_Click(object sender, EventArgs e)
        {
            if (!mSelected) mParentView.SelectedSection = mNode;
        }

        #region ISearchable Members

        public bool Matches(string search)
        {
            return FindInText.Match(Label, search);
        }

        public void Replace(string search, string replace)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        public Block FindBlock(PhraseNode phrase)
        {
            foreach (Control c in mBlocksPanel.Controls)
            {
                if (c is Block && ((Block)c).Node == phrase) return (Block)c;
            }
            return null;
        }

        public void RemoveBlock(PhraseNode phrase)
        {
            mBlocksPanel.Controls.Remove(FindBlock(phrase));
        }
    }
}
