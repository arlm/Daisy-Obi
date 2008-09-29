using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace Obi.ProjectView
{
    public partial class Strip : UserControl, ISearchable, ISelectableInContentViewWithColors
    {
        private float mAudioScale;           // local audio scale for the strip
        private int mBlockLayoutBaseHeight;  // base height of the block layout (for zooming)
        private ContentView mContentView;    // parent content view
        private bool mHighlighted;           // highlighted flag (when the section node is selected)
        private Mutex mLabelUpdateThread;    // thread to update labels
        private SectionNode mNode;           // the section node for this strip
        private bool mWrap;                  // wrap contents


        /// <summary>
        /// This constructor is used by the designer.
        /// </summary>
        public Strip()
        {
            InitializeComponent();
            mBlockLayoutBaseHeight = mBlockLayout.Height;
            mLabel.Editable = false;
            mNode = null;
            Highlighted = false;
            mWrap = false;
            mLabelUpdateThread = new Mutex();
        }

        /// <summary>
        /// Create a new strip with an associated section node.
        /// </summary>
        public Strip(SectionNode node, ContentView parent)
            : this()
        {
            if (node == null) throw new Exception("Cannot set a null section node for a strip!");
            mNode = node;
            Label = mNode.Label;
            mContentView = parent;
            ZoomFactor = mContentView.ZoomFactor;
            AudioScale = mContentView.AudioScale;
            UpdateColors();
            SetAccessibleName();
        }


        /// <summary>
        /// Get or set the audio scale for *this* strip (may differ from the parent's.)
        /// </summary>
        public float AudioScale
        {
            get { return mAudioScale; }
            set
            {
                if (value > 0.0)
                {
                    mAudioScale = value;
                    foreach (Control c in mBlockLayout.Controls) if (c is AudioBlock) ((AudioBlock)c).AudioScale = value;
                    ResizeToContentsLength(mBlockLayout.Height);
                }
            }
        }

        /// <summary>
        /// Get or set the color settings from the content view.
        /// </summary>
        public ColorSettings ColorSettings
        {
            get { return mContentView == null ? null : mContentView.ColorSettings; }
            set { UpdateColors(value); }
        }

        /// <summary>
        /// Get the content view for the strip.
        /// </summary>
        public ContentView ContentView { get { return mContentView; } }

        /// <summary>
        /// Get the first block in the strip, or null if empty.
        /// </summary>
        public Block FirstBlock
        {
            get { return mBlockLayout.Controls.Count > 1 ? mBlockLayout.Controls[1] as Block : null; }
        }

        /// <summary>
        /// Get or set the highlighted flag for the strip corresponding to the section node being (de)selected.
        /// </summary>
        public bool Highlighted
        {
            get { return mHighlighted; }
            set
            {
                mHighlighted = value && !(mContentView.Selection is TextSelection);
                if (mHighlighted && mLabel.Editable) mLabel.Editable = false;
                UpdateColors();
                if (mHighlighted) UpdateWaveforms(WaveformWithPriority.STRIP_SELECTED_PRIORITY);
            }
        }

        /// <summary>
        /// Get or set the label of the strip where the title of the section can be edited.
        /// </summary>
        public string Label
        {
            get { return mLabel.Label; }
            set
            {
                if (value != null && value != "")
                {
                    mLabel.Label = value;
                    SetAccessibleName();
                }
                int w = mLabel.MinimumSize.Width + mLabel.Margin.Left + mLabel.Margin.Right;
                if (w > MinimumSize.Width) MinimumSize = new Size(w, MinimumSize.Height);
            }
        }

        /// <summary>
        /// Get the last block in the strip, or null if empty.
        /// </summary>
        public Block LastBlock
        {
            get
            {
                return mBlockLayout.Controls.Count > 1 ? mBlockLayout.Controls[mBlockLayout.Controls.Count - 2] as Block :
                    null;
            }
        }

        /// <summary>
        /// Get the tab index of the last block in the strip.
        /// Strip cursors are skipped when tabbing.
        /// </summary>
        public int LastTabIndex
        {
            get
            {
                int last = mBlockLayout.Controls.Count - 2;
                return last >= 0 ? ((Block)mBlockLayout.Controls[last]).LastTabIndex : TabIndex;
            }
        }

        /// <summary>
        /// Get the section node for this strip.
        /// </summary>
        public SectionNode Node { get { return mNode; } }

        /// <summary>
        /// Get the (generic) node for this strip; used for selection.
        /// </summary>
        public ObiNode ObiNode { get { return mNode; } }

        /// <summary>
        /// Get the current selection for this node (i.e. the strip, its label or an index is selected.)
        /// </summary>
        public NodeSelection Selection
        {
            get
            {
                NodeSelection selection = mContentView == null ? null : mContentView.Selection;
                return selection == null || selection.Node != mNode ? null : selection;
            }
        }

        /// <summary>
        /// Set the wrap parameter.
        /// </summary>
        public bool WrapContents
        {
            set
            {
                mWrap = value;
                UpdateSize();
                if (mWrap)
                {
                    mContentView.SizeChanged += new EventHandler(ContentView_SizeChanged);
                }
                else
                {
                    mContentView.SizeChanged -= new EventHandler(ContentView_SizeChanged);
                }
            }
        }

        /// <summary>
        /// Set the zoom factor for the strip and its contents.
        /// </summary>
        public float ZoomFactor
        {
            set
            {
                if (value > 0.0f)
                {
                    mLabel.ZoomFactor = value;
                    int h = (int)Math.Round(value * mBlockLayoutBaseHeight);
                    foreach (Control c in mBlockLayout.Controls)
                    {
                        if (c is Block)
                        {
                            ((Block)c).SetZoomFactorAndHeight(value, h);
                        }
                        else if (c is StripCursor)
                        {
                            ((StripCursor)c).SetHeight(h);
                        }
                    }
                    ResizeToContentsLength(h);
                }
            }
        }


        private delegate Block BlockInvokation(EmptyNode node);

        /// <summary>
        /// Add a new block for an empty node. Return the block once added.
        /// Cursors are added if necessary: one after the block, and one before
        /// if it is the first block of the strip.
        /// </summary>
        public Block AddBlockForNode(EmptyNode node)
        {
            if (InvokeRequired)
            {
                return (Block)Invoke(new BlockInvokation(AddBlockForNode), node);
            }
            else
            {
                if (mBlockLayout.Controls.Count == 0) AddCursorAtBlockLayoutIndex(0);
                Block block = node is PhraseNode ? new AudioBlock((PhraseNode)node, this) : new Block(node, this);
                mBlockLayout.Controls.Add(block);
                mBlockLayout.Controls.SetChildIndex(block, 1 + 2 * node.Index);
                AddCursorAtBlockLayoutIndex(2 + 2 * node.Index);
                block.SetZoomFactorAndHeight(mContentView.ZoomFactor, mBlockLayout.Height);
                block.Cursor = Cursor;
                block.SizeChanged += new EventHandler(Block_SizeChanged);
                UpdateSize();
                return block;
            }
        }

        /// <summary>
        /// Return the block after the selection. In the case of a strip it is the first block.
        /// Return null if this goes past the last block, there are no blocks, or nothing was selected
        /// in the first place.
        /// </summary>
        public Block BlockAfter(ISelectableInContentView item)
        {
            int count = mBlockLayout.Controls.Count;
            int index = item is Strip ?
                ((Strip)item).Selection is StripIndexSelection ? ((StripIndexSelection)((Strip)item).Selection).Index : 0 :
                item is Block ? mBlockLayout.Controls.IndexOf((Control)item) + 2 : count;
            return index < count ? (Block)mBlockLayout.Controls[index] : null;
        }

        /// <summary>
        /// Return the block before the selection. In the case of a strip this is the last block.
        /// Return null if this is before the first block, there are no blocks, or nothing was selected
        /// in the first place.
        /// </summary>
        public Block BlockBefore(ISelectableInContentView item)
        {
            int index = item is Strip ?
                ((Strip)item).Selection is StripIndexSelection ? ((StripIndexSelection)((Strip)item).Selection).Index - 1 :
                    mBlockLayout.Controls.Count - 2 :
                item is Block ? mBlockLayout.Controls.IndexOf((Control)item) - 2 : 0;
            return index >= 0 ? (Block)mBlockLayout.Controls[index] : null;
        }

        /// <summary>
        /// Find the block for the corresponding node inside the strip.
        /// </summary>
        public Block FindBlock(EmptyNode node)
        {
            // Note: we cannot rely on node.Index because we may want to find the block of a node that was deleted
            // from the tree, and it does not have an index anymore. So let's just look for it.
            foreach (Control c in mBlockLayout.Controls) if (c is Block && ((Block)c).Node == node) return (Block)c;
            return null;
        }

        /// <summary>
        /// Find the strip cursor at the given index.
        /// </summary>
        public StripCursor FindStripCursor(int index)
        {
            System.Diagnostics.Debug.Assert(index * 2 < mBlockLayout.Controls.Count, "No strip cursor at index");
            System.Diagnostics.Debug.Assert(mBlockLayout.Controls[index * 2] is StripCursor);
            return (StripCursor)mBlockLayout.Controls[index * 2];
        }

        /// <summary>
        /// Focus on the label.
        /// </summary>
        public void FocusStripLabel() { mLabel.Focus(); }

        /// <summary>
        /// Remove the block for the given node.
        /// Remove the following strip cursor as well
        /// (and the first one if it was the last block.)
        /// </summary>
        public void RemoveBlock(EmptyNode node)
        {
            Block block = FindBlock(node);
            if (block != null)
            {
                int index = mBlockLayout.Controls.IndexOf(block);
                if (index < mBlockLayout.Controls.Count) mBlockLayout.Controls.RemoveAt(index + 1);
                mBlockLayout.Controls.RemoveAt(index);
                if (mBlockLayout.Controls.Count == 1) mBlockLayout.Controls.RemoveAt(0);
                block.SizeChanged -= new EventHandler(Block_SizeChanged);
                UpdateSize();
            }
        }

        /// <summary>
        /// Show the cursor at the current time in the waveform of the current playing block.
        /// </summary>
        public void SetSelectedAudioInBlockFromBlock(Block block, AudioRange audioRange)
        {
            mContentView.SelectionFromStrip = new AudioSelection((PhraseNode)block.Node, mContentView, audioRange);
        }

        /// <summary>
        /// Set the current selected from the block itself. This gets passed to the content view.
        /// </summary>
        public void SetSelectedBlockFromBlock(Block block) { mContentView.SelectedNode = block.Node; }

        /// <summary>
        /// Set the selection from a strip cursor to its index. This gets passed to the content view.
        /// </summary>
        public void SetSelectedIndexFromStripCursor(StripCursor cursor)
        {
            int index = mBlockLayout.Controls.IndexOf(cursor) / 2;
            mContentView.SelectionFromStrip = new StripIndexSelection(Node, mContentView, index);
        }

        /// <summary>
        /// Set the selection from the parent control view. (From ISelectableInContentView)
        /// </summary>
        public void SetSelectionFromContentView(NodeSelection selection)
        {
            SetAccessibleName();
            Highlighted = !(selection is StripIndexSelection) && selection != null;
        }

        /// <summary>
        /// Start renaming the strip.
        /// </summary>
        public void StartRenaming()
        {
            mLabel.Editable = true;
            mContentView.SelectionFromStrip = new TextSelection(mNode, mContentView, Label);
            SetAccessibleName();
        }

        /// <summary>
        /// Get the strip index after the given (selected) item.
        /// </summary>
        public int StripIndexAfter(ISelectableInContentView item)
        {
            int lastIndex = Node.PhraseChildCount + 1;
            int index = item is Strip ?
                ((Strip)item).Selection is StripIndexSelection ? ((StripIndexSelection)((Strip)item).Selection).Index + 1 : 0 :
                item is Block ? (mBlockLayout.Controls.IndexOf((Control)item) + 1) / 2 : lastIndex;
            return index <= lastIndex ? index : lastIndex;
        }

        /// <summary>
        /// Index in the strip before the selected item.
        /// </summary>
        public int StripIndexBefore(ISelectableInContentView item)
        {
            int index = item is Strip ?
                ((Strip)item).Selection is StripIndexSelection ? ((StripIndexSelection)((Strip)item).Selection).Index - 1 :
                    mBlockLayout.Controls.Count :
                item is Block ? mBlockLayout.Controls.IndexOf((Control)item) / 2 - 1 : 0;
            return index >= 0 ? index : 0;
        }

        /// <summary>
        /// Match the label (case independent) when searching.
        /// </summary>
        public string ToMatch() { return Label.ToLowerInvariant(); }

        /// <summary>
        /// ???
        /// </summary>
        public void UpdateBlockLabelsInStrip(object sender, DoWorkEventArgs e)
        {
            mLabelUpdateThread.WaitOne();
            try
            {
                Control BlockControl = null;
                for (int i = 0; i < mBlockLayout.Controls.Count; i++)
                {
                    BlockControl = mBlockLayout.Controls[i];
                    if (BlockControl is Block)
                    {
                        ((Block)BlockControl).UpdateLabelsText();
                    }
                }
            }
            catch (System.Exception)
            {
                mLabelUpdateThread.ReleaseMutex();
                return;
            }
            mLabelUpdateThread.ReleaseMutex();
        }

        /// <summary>
        /// Update the colors of the strip and its contents's.
        /// </summary>
        public void UpdateColors() { UpdateColors(ColorSettings); }

        /// <summary>
        /// Update the tab index for the strip and all of its blocks.
        /// </summary>
        public int UpdateTabIndex(int index)
        {
            TabIndex = index;
            ++index;
            foreach (Control c in mBlockLayout.Controls)
            {
                if (c is Block) index = ((Block)c).UpdateTabIndex(index);
            }
            return index;
        }

        /// <summary>
        /// Update all waveforms in the strip with a given priority. (When rendering for the fist time, or selecting.)
        /// </summary>
        public void UpdateWaveforms(int priority)
        {
            foreach (Control c in mBlockLayout.Controls)
            {
                if (c is AudioBlock)
                {
                    mContentView.RenderWaveform(new WaveformWithPriority(((AudioBlock)c).Waveform, priority));
                }
            }
        }


        // Add content view label to the accessible name of the strip when entering.
        private void AddContentsViewLabel()
        {
            SetAccessibleName();
            if (mContentView.IsEnteringView)
            {
                mLabel.AccessibleName = string.Format("{0} {1}", Localizer.Message("content_view"), mLabel.AccessibleName);
                Thread TrimAccessibleName = new Thread(new ThreadStart(TrimContentsViewAccessibleLabel));
                TrimAccessibleName.Start();
            }
        }

        // Add a cursor at the given index (in the context of the block layout.)
        private void AddCursorAtBlockLayoutIndex(int index)
        {
            StripCursor cursor = new StripCursor(this.Node);
            cursor.SetHeight(mBlockLayout.Height);
            cursor.ColorSettings = ColorSettings;
            cursor.TabStop = false;
            mBlockLayout.Controls.Add(cursor);
            mBlockLayout.Controls.SetChildIndex(cursor, index);
        }

        // Resize the strip so that it fits both its label and its block layout.
        // h is the target height of the block layout.
        private void ResizeToContentsLength(int h)
        {
            if (mWrap)
            {
                throw new Exception("Augh!");
            }
            else
            {
                Control k = mBlockLayout.Controls.Count > 0 ? mBlockLayout.Controls[mBlockLayout.Controls.Count - 1] : null;
                int w = k == null ? Width - mBlockLayout.Margin.Horizontal :
                    k.Location.X + k.Width + k.Margin.Right;
                int h_ = mBlockLayout.Height - h;
                mBlockLayout.Size = new Size(w, h);
                int w_ = mLabel.Width > w ? mLabel.Width : w;
                Size = new Size(mBlockLayout.Location.X + w_ + mBlockLayout.Margin.Right, Height - h_);
            }
        }

        // Set verbose accessible name for the strip 
        private void SetAccessibleName()
        {
            if (Selection is StripIndexSelection)
            {
                mLabel.AccessibleName = Selection.ToString();
            }
            else
            {
                mLabel.AccessibleName = string.Concat(mNode.Used ? "" : Localizer.Message("unused"),
                    mNode.Label,
                    mNode.Duration == 0.0 ? Localizer.Message("empty") : string.Format(Localizer.Message("duration_s_ms"), mNode.Duration / 1000.0),
                    string.Format(Localizer.Message("section_level_to_string"), mNode.IsRooted ? mNode.Level : 0),
                    mNode.PhraseChildCount == 0 ? "" :
                        mNode.PhraseChildCount == 1 ? Localizer.Message("section_one_phrase_to_string") :
                            string.Format(Localizer.Message("section_phrases_to_string"), mNode.PhraseChildCount));
            }
        }

        // Reset the accessible name after a short while.
        private void TrimContentsViewAccessibleLabel()
        {
            Thread.Sleep(750);
            SetAccessibleName();
        }

        // Udpate the color of the strip and its contents.
        private void UpdateColors(ColorSettings settings)
        {
            if (settings != null && mNode != null)
            {
                BackColor =
                mLabel.BackColor =
                    //mBlockLayout.BackColor =
                    mHighlighted ? settings.StripSelectedBackColor :
                    mNode.Used ? settings.StripBackColor : settings.StripUnusedBackColor;
                mLabel.ForeColor =
                    mHighlighted ? settings.StripSelectedForeColor :
                    mNode.Used ? settings.StripForeColor : settings.StripUnusedForeColor;
                mLabel.UpdateColors(settings);
                foreach (Control c in mBlockLayout.Controls)
                {
                    if (c is Block) ((Block)c).UpdateColors();
                }
            }
        }

        // Update the size of the strip to use the available width of the view
        private void UpdateSize()
        {
            if (mWrap)
            {
                UpdateSize_Wrap();
            }
            else
            {
                UpdateSize_NoWrap();
            }
        }

        // Update size when not wrapping contents.
        private void UpdateSize_NoWrap()
        {
            mBlockLayout.AutoSize = false;
            mBlockLayout.WrapContents = false;
            // Compute the minimum width of the block panel (largest of label or block layout width.)
            int wl = mLabel.Width;
            int wb = 0;
            foreach (Control c in mBlockLayout.Controls) wb += c.Width + c.Margin.Horizontal;
            int w = wl > wb ? wl : wb;
            mBlockLayout.Size = new Size(wb, mBlockLayout.Height);
            Size = new Size(w + mBlockLayout.Margin.Horizontal, Height);
        }

        // Update size when wrapping contents.
        private void UpdateSize_Wrap()
        {
            MinimumSize = new Size(mContentView.Width, MinimumSize.Height);
            Width = mContentView.Width;
            mBlockLayout.AutoSize = true;
            mBlockLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mBlockLayout.WrapContents = true;
            Height = mBlockLayout.Location.Y + mBlockLayout.Height + mBlockLayout.Margin.Bottom;
        }


        // Resize when a block size has changed.
        private void Block_SizeChanged(object sender, EventArgs e) { UpdateSize(); }

        // Resize when the parent size changes; only when wrapping strips.
        private void ContentView_SizeChanged(object sender, EventArgs e) { UpdateSize(); }

        // Change the selection depending on the editable state of the label.
        private void Label_EditableChanged(object sender, EventArgs e)
        {
            if (mContentView != null)
            {
                mContentView.SelectionFromStrip = mLabel.Editable ?
                    new TextSelection(mNode, mContentView, mLabel.Label) :
                    new NodeSelection(mNode, mContentView);
            }
        }

        // Update the label of the node after the user edited it.
        private void Label_LabelEditedByUser(object sender, EventArgs e)
        {
            if (mLabel.Label != "")
            {
                // update the label for the node
                mContentView.RenameStrip(this);
                mContentView.SelectionFromStrip = new NodeSelection(mNode, mContentView);
            }
            else
            {
                // restore the previous label from the node
                mLabel.Label = mNode.Label;
            }
        }

        // Resize the strip according to the editable label, which size can change.
        private void Label_SizeChanged(object sender, EventArgs e)
        {
            int y = mBlockLayout.Location.Y;
            mBlockLayout.Location = new Point(mBlockLayout.Location.X,
                mLabel.Location.Y + mLabel.Height + mLabel.Margin.Bottom + mBlockLayout.Margin.Top);
            Size = new Size(Width, Height - y + mBlockLayout.Location.Y);
        }

        // Select when tabbed into
        private void Strip_Enter(object sender, EventArgs e)
        {
            AddContentsViewLabel();
            mLabel.Focus();
            if ((mContentView.SelectedSection != mNode || mContentView.Selection is StripIndexSelection) &&
                !mContentView.Focusing)
            {
                mContentView.SelectedNode = mNode;
            }
        }
    }
}
