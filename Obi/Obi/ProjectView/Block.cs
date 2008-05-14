using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class Block : UserControl, ISelectableInStripView, ISearchable
    {
        protected EmptyNode mNode;                        // the corresponding node
        private bool mSelected;                           // selected flag
        private ISelectableInStripView mParentContainer;  // not necessarily a strip!

        public Block(EmptyNode node, ISelectableInStripView parent): this()
        {
            mNode = node;
            mParentContainer = parent;
            mSelected = false;
            node.ChangedKind += new EmptyNode.ChangedKindEventHandler(node_ChangedKind);
            node.ChangedPageNumber += new NodeEventHandler<EmptyNode>(node_ChangedPageNumber);
            UpdateColors();
            UpdateLabel();
        }

        public Block() { InitializeComponent(); }


        // Width of the label (including margins)
        protected int LabelFullWidth { get { return mLabel.Margin.Left + mLabel.Width + mLabel.Margin.Right; } }

        // Generate the label string for this block.
        protected virtual void UpdateLabel()
        {
            mLabel.Text = Node.BaseStringShort();
            mLabel.AccessibleName = Node.BaseString();
            Size = new Size(LabelFullWidth, Height);
        }

        private void node_ChangedPageNumber(object sender, NodeEventArgs<EmptyNode> e) { UpdateLabel(); }
        private void node_ChangedKind(object sender, ChangedKindEventArgs e) { UpdateLabel(); }

        /// <summary>
        /// The phrase node for this block.
        /// </summary>
        public EmptyNode Node { get { return mNode; } }
        public ObiNode ObiNode { get { return mNode; } }

        /// <summary>
        /// Set the selected flag for the block.
        /// </summary>
        public virtual bool Selected
        {
            get { return mSelected; }
            set
            {
                mSelected = value;
                UpdateColors();
            }
        }


        /// <summary>
        /// Update the colors of the block when the state of its node has changed.
        /// </summary>
        public void UpdateColors()
        {
            if (mNode != null)
            {
                // TODO Get colors from profile
                BackColor = mSelected ? Color.Yellow : mNode.Used ? Color.HotPink : Color.Gray;
            }
        }

        /// <summary>
        /// Set the selection from the parent view
        /// </summary>
        public virtual NodeSelection SelectionFromView { set { Selected = value != null; } }

        /// <summary>
        /// Get the tab index of the block.
        /// </summary>
        public int LastTabIndex { get { return TabIndex; } }

        /// <summary>
        /// Update the tab index of the block with the new value and return the next index.
        /// </summary>
        public int UpdateTabIndex(int index)
        {
            TabIndex = index;
            ++index;
            return index;
        }

        /// <summary>
        /// The strip that contains this block.
        /// </summary>
        public Strip Strip
        {
            get { return mParentContainer is Strip ? (Strip)mParentContainer : ((Block)mParentContainer).Strip; }

        }

        // Select on click and tabbing
        private void Block_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Print("Click on {0}", this);
            Strip.SelectedBlock = this;
        }

        protected void Block_Enter(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Print("Enter {0}?", this);
            if (!Strip.ParentView.Focusing)
            {
                System.Diagnostics.Debug.Print("Yes.");
                Strip.SelectedBlock = this;
            }
            else
            {
                System.Diagnostics.Debug.Print("No.");
            }
        }
        private void mLabel_Click(object sender, EventArgs e) { Strip.SelectedBlock = this; }

        #region ISearchable Members

        public string ToMatch()
        {
            return mLabel.Text.ToLowerInvariant();
        }

        #endregion
    }
}
