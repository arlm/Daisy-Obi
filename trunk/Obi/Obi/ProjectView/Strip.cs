using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic ;

namespace Obi.ProjectView
{
    public partial class Strip : UserControl, ISearchable, ISelectableInContentViewWithColors
    {
        private float mAudioScale;           // local audio scale for the strip
        private int mBlockLayoutBaseHeight;  // base height of the block layout (for zooming)
        private int mBlockHeight;            // height of a single line of the block layout
        private ContentView mContentView;    // parent content view
        private bool mHighlighted;           // highlighted flag (when the section node is selected)
        private Mutex mLabelUpdateThread;    // thread to update labels
        private SectionNode mNode;           // the section node for this strip
        private bool mWrap;                  // wrap contents
        private bool m_IsBlocksVisibilityProcessActive; // @phraseLimit
        private int m_OffsetForFirstPhrase = 0;//@singleSection

        /// <summary>
        /// This constructor is used by the designer.
        /// </summary>
        public Strip()
        {
            InitializeComponent();
            mBlockLayoutBaseHeight = mBlockLayout.Height;
            mBlockHeight = 0;
            mLabel.Editable = false;
            mNode = null;
            Highlighted = false;
            mWrap = false;
            mLabelUpdateThread = new Mutex();
            m_IsBlocksVisibilityProcessActive = false; // @phraseLimit
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
            mContentView.SizeChanged += new EventHandler(delegate(object sender, EventArgs e) { Resize_View(); });
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
                    Resize_Blocks();
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


// @phraseLimit
        /// <summary>
        /// returns true if all the blocks in strip are visible
                /// </summary>
        public bool IsBlocksVisible
            {
            get
                {
                if (mNode.PhraseChildCount == 0)
                    return true;
                else if (mBlockLayout.Controls.Count < (mNode.PhraseChildCount * 2 ) + 1 )
                    return false;
                else
                    return true;
                }
            }
        // @phraseLimit
        public bool IsBlocksVisibilityProcessActive
            {
             get { return m_IsBlocksVisibilityProcessActive ; }
            set { m_IsBlocksVisibilityProcessActive = value ;}
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
                if (mHighlighted) UpdateWaveforms(AudioBlock.STRIP_SELECTED_PRIORITY);
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
                Resize_Label();
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
                Resize_Wrap();
            }
        }

        //@singleSection
        public bool IsContentViewFilledWithBlocks
            {
            get
                {
                int stripsPanelLocation = this.Parent != null ? this.Parent.Location.Y : 0;
                if (mBlockLayout != null && mBlockLayout.Controls.Count > 0
                    && (mBlockLayout.Controls[mBlockLayout.Controls.Count - 1].Location.Y + stripsPanelLocation) > mContentView.ContentViewDepthForCreatingBlocks)
                    {
                    //Console.WriteLine ( mBlockLayout.Controls[mBlockLayout.Controls.Count - 1].Location.Y + .Y );
                    return true;
                    }
                else
                    {
                    return false;
                    }
                }
            }

        //@singleSection
        public int OffsetForFirstPhrase
            {
            get
                {
                return m_OffsetForFirstPhrase;
                }
            }

//@singleSection
        public int BlocksLayoutTopPosition
            {
            get
                {
                Console.WriteLine ( "blocks layout upper y : " + mBlockLayout.Location.Y );
                return mBlockLayout.Location.Y ;
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
                    mBlockHeight = (int)Math.Round(value * mBlockLayoutBaseHeight);
                    foreach (Control c in mBlockLayout.Controls)
                    {
                        if (c is Block)
                        {
                            ((Block)c).SetZoomFactorAndHeight(value, mBlockHeight);
                        }
                        else if (c is StripCursor)
                        {
                            ((StripCursor)c).SetHeight(mBlockHeight);
                        }
                    }
                    Resize_Zoom();
                }
            }
        }

    //@singleSection
    public bool IsContentViewFilledWithBlocksTillPixelDepth ( int pixelDepth )
        {
        //Console.WriteLine ( "pixel check : " + mBlockLayout.Controls[mBlockLayout.Controls.Count - 1].Location.Y + " " + this.Parent.Location.Y );
            if (mBlockLayout != null && mBlockLayout.Controls.Count > 0
                                     && (mBlockLayout.Controls[mBlockLayout.Controls.Count - 1].Location.Y + mBlockLayout.Location.Y + 
                                     this.Parent.Location.Y) > pixelDepth)
                {
                //Console.WriteLine ( "pixel depth in strip " + pixelDepth );
                return true;
                }
            else
                {
                return false;
                }
            
        }

        //@singleSection
        public List<int> GetBoundaryPhrasesIndexForVisiblePhrases ( int startY, int endY )
            {
            List<int> boundaryIndex = new List<int> ();
            
            foreach (Control c in mBlockLayout.Controls)
                {
                if (c is Block)
                    {
                    Block b = (Block)c;
                    if (b.Location.Y >= startY && boundaryIndex.Count == 0) boundaryIndex.Add ( b.Node.Index );

                    if ( boundaryIndex.Count == 1 && 
                        ( b.Location.Y > endY || b == LastBlock ))
                        {
                        boundaryIndex.Add ( b.Node.Index ) ;
                        }
                    }
                }
            return boundaryIndex;
            }

        //@singleSection
        public int GetPhraseCountForContentViewVisibleHeight ( int visibleHeight,int visibleWidth,  EmptyNode node, bool checkWithFollowingPhrases )
            {
            int expectedRows = Convert.ToInt32 ( visibleHeight / (103 * mContentView.ZoomFactor)) + 1 ;
            int expectedMilliseconds = Convert.ToInt32 ( (visibleWidth / 12) * mContentView.ZoomFactor ) * expectedRows * 1000;
            // use time for this
            double time = 0;
            int phraseCount = 0;
            double maxAllowedTimePerPhrase = Convert.ToInt32 ( (visibleWidth / 12) * mContentView.ZoomFactor ) * 1000;

            for (int i = node.Index; i >= 0; --i)
                {
                double phraseDuration = mNode.PhraseChild ( i ).Duration ;
                time += phraseDuration < maxAllowedTimePerPhrase ? phraseDuration : maxAllowedTimePerPhrase;
                phraseCount++;

                if (time > expectedMilliseconds)
                    {
                    Console.WriteLine ( " phrase count for visible page is " + phraseCount );
                    break;
                    }
                }

            return phraseCount;
            }

        //@singleSection
        public bool IsBlockForEmptyNodeExists ( EmptyNode node )
            {
            if ( mBlockLayout.Controls.Count == 0 ) return false;

            if (node.Index < OffsetForFirstPhrase) return false;
            //int startIndexForIteration = node.Index - FirstBlock.Node.Index;
            // in future if first block is not 0 index phrase then it is better to use find block function
            //int startIndexForIteration = node.Index ;
            int startIndexForIteration = node.Index- OffsetForFirstPhrase;
            int blockControlIndex = (startIndexForIteration * 2) + 1;

            if (blockControlIndex >= mBlockLayout.Controls.Count) return false;
            //Console.WriteLine ( "phrase index " + node.Index + "  first node " + FirstBlock.Node.Index );
            if (mBlockLayout.Controls[blockControlIndex] is Block
                && ((Block) mBlockLayout.Controls[blockControlIndex]).Node == node)
                {
                //Console.WriteLine ( "IsBlockForEmptyNodeExists true" );
                return true;
                }
            else
                {
                Console.WriteLine ( "IsBlockForEmptyNodeExists is false for : " + node.ToString () );
                return false ;
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
            if (IsBlockForEmptyNodeExists ( node)) return FindBlock (node );

            // if node index is just one less than offset of strip then allow insert else return null
            if (mBlockLayout.Controls.Count > 1 && OffsetForFirstPhrase - node.Index > 1) return null;

                if (mBlockLayout.Controls.Count == 0)
                {
                    StripCursor cursor = AddCursorAtBlockLayoutIndex(0);

                    m_OffsetForFirstPhrase = node.Index;//@singleSection
                    Console.WriteLine ( "Offset of strip at 0 blocks is " + m_OffsetForFirstPhrase );
                }
                
                Block block = node is PhraseNode ? new AudioBlock((PhraseNode)node, this) : new Block(node, this);
                mBlockLayout.Controls.Add(block);
                //@singleSection: following 2 lines replaced
                //mBlockLayout.Controls.SetChildIndex(block, 1 + 2 * node.Index);
                //AddCursorAtBlockLayoutIndex(2 + 2 * node.Index);
                
                mBlockLayout.Controls.SetChildIndex ( block, 1 + 2 * (node.Index- OffsetForFirstPhrase) );
                AddCursorAtBlockLayoutIndex ( 2 + 2 * (node.Index - OffsetForFirstPhrase) );
                block.SetZoomFactorAndHeight(mContentView.ZoomFactor, mBlockHeight);
                    block.Cursor = Cursor;
                block.SizeChanged += new EventHandler(Block_SizeChanged);

                Resize_Blocks(); 

                UpdateStripCursorsAccessibleName(2 + 2 * node.Index);
                if (mBlockLayout.Controls.IndexOf ( block ) == 1)
                    {
                    m_OffsetForFirstPhrase = node.Index;
                    Console.WriteLine ( "Offset of strip is " + m_OffsetForFirstPhrase );
                    }
                return block;
            }
        }

        //@singleSection
        private delegate Block BlockRangeCreationInvokation(EmptyNode startNode,EmptyNode endNode);

        //@singleSection
        public Block AddsRangeOfBlocks(EmptyNode startNode, EmptyNode endNode)
        {
            if (InvokeRequired)
            {
                return (Block)Invoke(new BlockRangeCreationInvokation(AddsRangeOfBlocks), startNode, endNode);
            }
            else
                {
                for (int i = startNode.Index; i <= endNode.Index; ++i)
                    {
                    CreateBlockForNode ( mNode.PhraseChild ( i),endNode.Index == i ?  true : false);
                    }
                    UpdateColors();
                return null ;
                }
            }

        //@singleSection
        private Block CreateBlockForNode ( EmptyNode node , bool updateSize)
            {
            if (IsBlockForEmptyNodeExists ( node )) return FindBlock ( node );
            // if node index is just one less than offset of strip then allow insert else return null
            if (mBlockLayout.Controls.Count > 1 && OffsetForFirstPhrase - node.Index > 1) return null;
                
                
            if (mBlockLayout.Controls.Count == 0)
                {
                StripCursor cursor = AddCursorAtBlockLayoutIndex ( 0 );

                m_OffsetForFirstPhrase = node.Index;//@singleSection
                Console.WriteLine ( "Offset of strip at 0 blocks is " + m_OffsetForFirstPhrase );
                }
            Block block = node is PhraseNode ? new AudioBlock ( (PhraseNode)node, this ) : new Block ( node, this );
            mBlockLayout.Controls.Add ( block );
            //@singleSection: following 2 lines replaced
            //mBlockLayout.Controls.SetChildIndex(block, 1 + 2 * node.Index);
            //AddCursorAtBlockLayoutIndex(2 + 2 * node.Index);

            mBlockLayout.Controls.SetChildIndex ( block, 1 + 2 * (node.Index - OffsetForFirstPhrase) );
            AddCursorAtBlockLayoutIndex ( 2 + 2 * (node.Index - OffsetForFirstPhrase) );
            block.SetZoomFactorAndHeight ( mContentView.ZoomFactor, mBlockHeight );
            block.Cursor = Cursor;
            block.SizeChanged += new EventHandler ( Block_SizeChanged );

            if ( updateSize )  Resize_Blocks ();

            UpdateStripCursorsAccessibleName ( 2 + 2 * node.Index );
            if (mBlockLayout.Controls.IndexOf ( block ) == 1)
                {
                m_OffsetForFirstPhrase = node.Index;
                Console.WriteLine ( "Offset of strip is " + m_OffsetForFirstPhrase );
                }
            return block;
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
                ((Strip)item).Selection is StripIndexSelection ? ((StripIndexSelection)((Strip)item).Selection).Index : 1 :
                item is Block ? mBlockLayout.Controls.IndexOf((Control)item) + 2 :
                item is StripCursor ? mBlockLayout.Controls.IndexOf((Control)item) + 1 : count;
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
                item is Block ? mBlockLayout.Controls.IndexOf((Control)item) - 2 :
                item is StripCursor ? mBlockLayout.Controls.IndexOf((Control)item) - 1 : -1;
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
            //@singleSection: replaced following three lines
            //System.Diagnostics.Debug.Assert(index * 2 < mBlockLayout.Controls.Count, "No strip cursor at index");
            //System.Diagnostics.Debug.Assert ( mBlockLayout.Controls.Count > index * 2    &&    mBlockLayout.Controls[index * 2] is StripCursor );
            //return mBlockLayout.Controls.Count > index * 2? (StripCursor)mBlockLayout.Controls[index * 2] : null;

        System.Diagnostics.Debug.Assert ( (index - OffsetForFirstPhrase) * 2 < mBlockLayout.Controls.Count, "No strip cursor at index" );
        System.Diagnostics.Debug.Assert ( mBlockLayout.Controls.Count > (index - OffsetForFirstPhrase) * 2 && mBlockLayout.Controls[(index - OffsetForFirstPhrase) * 2] is StripCursor );
        return mBlockLayout.Controls.Count > (index - OffsetForFirstPhrase) * 2 ? (StripCursor)mBlockLayout.Controls[(index - OffsetForFirstPhrase) * 2] : null;
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
        /// 
        public void RemoveBlock ( EmptyNode node )
            {
            RemoveBlock ( node, true );
            }

        private delegate void BlockRemoveInvokation ( EmptyNode node, bool updateSize ); // @phraseLimit

        public void RemoveBlock(EmptyNode node, bool updateSize)
        {
            if (InvokeRequired)
            {
                Invoke ( new BlockRemoveInvokation ( RemoveBlock ), node, updateSize );
            }
        else
            {
            Block block = FindBlock ( node );
            if (block != null)
                {
                int index = mBlockLayout.Controls.IndexOf ( block );
                if (index < mBlockLayout.Controls.Count) mBlockLayout.Controls.RemoveAt ( index + 1 );
                mBlockLayout.Controls.RemoveAt ( index );
                if (mBlockLayout.Controls.Count == 1) mBlockLayout.Controls.RemoveAt ( 0 );
                block.SizeChanged -= new EventHandler ( Block_SizeChanged );
                if (updateSize) Resize_Blocks ();
                UpdateStripCursorsAccessibleName ( index - 1 );

                // dispose block for freeing window handle only if it is not held in clipboard @phraseLimit
                if (mContentView.clipboard == null || (mContentView.clipboard != null && mContentView.clipboard.Node != block.Node))
                    {
                    block.Dispose ();
                    block = null;
                    }

                }
            }
        }


        // @phraseLimit
        /// <summary>
        /// Remove and dispose all blocks in strip without removing phrases
                /// </summary>
        /// <param name="updateSize"></param>
        /// <returns></returns>
        public int RemoveAllBlocks ( bool updateSize )
            {
            int blocksRemovedCount = 0;
            if (mBlockLayout.Controls.Count > 0)
                {
                    for (int i = mBlockLayout.Controls.Count - 1; i > 0; i--)
                    {
                    if (i >= mBlockLayout.Controls.Count) continue;

                    if (mBlockLayout.Controls[i] is Block)
                        {
                        RemoveBlock ( (Block)mBlockLayout.Controls[i], updateSize );
                        blocksRemovedCount++;
                        }
                    }
                }
                return blocksRemovedCount;
            }

        //@singleSection
        public int RemoveAllFollowingBlocks ( bool removeHiddenBlocks, bool updateSize )
            {
            if ( FirstBlock == null ) return 0 ;

            return RemoveAllFollowingBlocks ( FirstBlock.Node, removeHiddenBlocks, updateSize );
            }

//@singleSection
        public int RemoveAllFollowingBlocks (EmptyNode node, bool removeHiddenBlocks ,bool updateSize )
            {
            int blocksRemovedCount = 0;
            int limitIndex = node.Index - OffsetForFirstPhrase;//@singleSection
            if (mBlockLayout.Controls.Count > 0)
                {
                for (int i = mBlockLayout.Controls.Count - 1; i > 0; i--)
                    {
                    if ( i >= mBlockLayout.Controls.Count) continue;

                    if (mBlockLayout.Controls[i] is Block)
                        {
                        if (((Block)mBlockLayout.Controls[i]).Node.Index <=  limitIndex
                            ||    (removeHiddenBlocks && !IsContentViewFilledWithBlocks ) )
                            {
                            Console.WriteLine ("Removal of block till " + i.ToString ()) ;
                            break;
                            }

                        RemoveBlock ( (Block)mBlockLayout.Controls[i], updateSize );
                        blocksRemovedCount++;
                        }
                    }
                }
            return blocksRemovedCount;
            }


        // @phraseLimit
        private delegate void QuickBlockRemoveInvokation ( Block block, bool updateSize ); // @phraseLimit

        // @phraseLimit
        /// <summary>
        /// Remove and dispose a single block passed as parameter
                /// </summary>
        /// <param name="block"></param>
        /// <param name="updateSize"></param>
        private void RemoveBlock (Block block,  bool updateSize )
            {
            if (InvokeRequired)
                {
                Invoke ( new QuickBlockRemoveInvokation ( RemoveBlock ), block, updateSize );
                }
            else
                {
                        //Block block = (Block) mBlockLayout.Controls[i];
                        if (block != null)
                            {
                            int index = mBlockLayout.Controls.IndexOf ( block );
                            if (index < mBlockLayout.Controls.Count)
                                {
                                Control  stripCursorControl = mBlockLayout.Controls[index + 1];
                                mBlockLayout.Controls.RemoveAt ( index + 1 );
                                if (stripCursorControl != null && stripCursorControl is StripCursor) stripCursorControl.Dispose ();
                                                                                                                                            }
                            mBlockLayout.Controls.RemoveAt ( index );
                            if (mBlockLayout.Controls.Count == 1) mBlockLayout.Controls.RemoveAt ( 0 );
                            block.SizeChanged -= new EventHandler ( Block_SizeChanged );
                            if (updateSize) Resize_Blocks ();
                            UpdateStripCursorsAccessibleName ( index - 1 );

                            // dispose block for freeing window handle only if it is not held in clipboard @phraseLimit
                            if (mContentView.clipboard == null || (mContentView.clipboard != null && mContentView.clipboard.Node != block.Node))
                                {
                                block.Dispose ();
                                block = null;
                                }
                            else // in case block is held in clipboard, just destroy handle
                                block.DestroyBlockHandle ();
                            }
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
            //mContentView.SelectionFromStrip = new StripIndexSelection(Node, mContentView, index);//@singleSection: original
            mContentView.SelectionFromStrip = new StripIndexSelection ( Node, mContentView, index + OffsetForFirstPhrase);//@singleSection new
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
            //int lastIndex = Node.PhraseChildCount;//@singleSection: original
        int lastIndex = Node.PhraseChildCount - OffsetForFirstPhrase;//@singleSection: new
            int index = item is Strip ?
                ((Strip)item).Selection is StripIndexSelection ? ((StripIndexSelection)((Strip)item).Selection).Index + 1 : 0 :
                item is Block ? (mBlockLayout.Controls.IndexOf((Control)item) + 1) / 2 :
                item is StripCursor ? mBlockLayout.Controls.IndexOf((Control)item) / 2 + 1 : lastIndex + 1;
            return index <= lastIndex ? index : lastIndex;
        }

        /// <summary>
        /// Index in the strip before the selected item.
        /// </summary>
        public int StripIndexBefore(ISelectableInContentView item)
        {
            int index = item is Strip ?
                ((Strip)item).Selection is StripIndexSelection ? ((StripIndexSelection)((Strip)item).Selection).Index - 1 :
                    (mBlockLayout.Controls.Count - 1) / 2 :
                item is Block ? mBlockLayout.Controls.IndexOf((Control)item) / 2 :
                item is StripCursor ? mBlockLayout.Controls.IndexOf((Control)item) / 2 - 1 : -1;
            return index >= 0 ? index : 0;
        }

        /// <summary>
        /// Match the label (case independent) when searching.
        /// </summary>
        public string ToMatch() { return Label.ToLowerInvariant(); }

        /// <summary>
        ///  Updates labels of all blocks in a strip, to be used with   background worker
        /// </summary>
        private void UpdateBlockLabelsInStrip(object sender, DoWorkEventArgs e)
            {
            UpdateBlockLabelsInStrip () ;
            }

        /// <summary>
        ///  Update labels of all blocks in a strip 
                /// </summary>
            public void UpdateBlockLabelsInStrip () 
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
            }
            finally
            {
                mLabelUpdateThread.ReleaseMutex();
            }
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
                    mContentView.RenderWaveform(((AudioBlock)c).Waveform, priority);
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
        // Return the new cursor.
        private StripCursor AddCursorAtBlockLayoutIndex(int index)
        {
            StripCursor cursor = new StripCursor();
            cursor.SetHeight(mBlockHeight);
            cursor.ColorSettings = ColorSettings;
            cursor.TabStop = false;
            cursor.SetAccessibleNameForIndex(index / 2);
            mBlockLayout.Controls.Add(cursor);
            mBlockLayout.Controls.SetChildIndex(cursor, index);
            return cursor;
        }

        // Compute the full width of the block layout that can accomodate all blocks.
        // This is exclusive of margins.
        private int BlockLayoutFullWidth
        {
            get
            {
                int w = 0;
                foreach (Control c in mBlockLayout.Controls) w += c.Width + c.Margin.Horizontal;
                return w;
            }
        }

        // Get the minimum width necessary for the block layout to contain all blocks.
        private int BlockLayoutMinimumWidth
        {
            get
            {
                int w_min = 0;
                int count = mBlockLayout.Controls.Count;
                if (count > 2)
                {
                    // there is at least one block; the first block must fit the first two cursors
                    w_min = mBlockLayout.Controls[0].Width + mBlockLayout.Controls[0].Margin.Horizontal +
                        mBlockLayout.Controls[1].Width + mBlockLayout.Controls[1].Margin.Horizontal +
                        mBlockLayout.Controls[2].Width + mBlockLayout.Controls[2].Margin.Horizontal;
                    // following blocks are counted with the following cursor
                    for (int i = 3; i < mBlockLayout.Controls.Count - 1; i += 2)
                    {
                        int w = mBlockLayout.Controls[i].Width + mBlockLayout.Controls[i].Margin.Horizontal +
                            mBlockLayout.Controls[i + 1].Width + mBlockLayout.Controls[i + 1].Margin.Horizontal;
                        if (w > w_min) w_min = w;
                    }
                }
                return w_min;
            }
        }

        // Size of the borders
        private int BorderHeight { get { return Bounds.Height - ClientSize.Height; } }
        private int BorderWidth { get { return Bounds.Width - ClientSize.Width; } }

        // Get the height of the strip for the current block layout size/position.
        // Includes the border size.
        private int HeightForContents
        {
            get
            {
            
            return mBlockLayout.Location.Y + Math.Max ( mBlockHeight, mBlockLayout.Height ) +
                mBlockLayout.Margin.Bottom + BorderHeight;
            float sizeMultiplier = 1;//@singleSection
            System.Media.SystemSounds.Asterisk.Play ();
                // determine how many blocks are visible w.r.t. total no. of phrases
            if (LastBlock != null ) //@singleSection
                {
                int lastBlockIndex = mBlockLayout.Controls.IndexOf ( LastBlock ) / 2;
                if (lastBlockIndex > 0  )
                    //&&                   (( mNode.PhraseChildCount - lastBlockIndex ) < 15
                    //||    lastBlockIndex <= 40    ||    lastBlockIndex %10 == 0))
                                    {
                                        sizeMultiplier = mNode.PhraseChildCount / lastBlockIndex;
                                        //Console.WriteLine ( "Phrase index : " + lastBlockIndex + " Strip height scale: " + sizeMultiplier );
                    }
                }
            Console.WriteLine ( "block layout height " + this.Size.ToString()); 
                // If there are no contents, still show space for the block layout
                return mBlockLayout.Location.Y + Math.Max(mBlockHeight,mBlockLayout.Height*2  ) +
                    mBlockLayout.Margin.Bottom + BorderHeight;
            }
        }

        //@singleSection
        public int PredictedStripHeight
            {
            get
                {
                float sizeMultiplier = 1;//@singleSection
                // determine how many blocks are visible w.r.t. total no. of phrases
                if (LastBlock != null) //@singleSection
                    {
                    int lastBlockIndex = mBlockLayout.Controls.IndexOf ( LastBlock ) / 2;
                    if (lastBlockIndex > 0)
                    //&&                   (( mNode.PhraseChildCount - lastBlockIndex ) < 15
                    //||    lastBlockIndex <= 40    ||    lastBlockIndex %10 == 0))
                        {
                        sizeMultiplier = mNode.PhraseChildCount / lastBlockIndex;
                        //Console.WriteLine ( "Phrase index : " + lastBlockIndex + " Strip height scale: " + sizeMultiplier );
                        }
                    }
                // If there are no contents, still show space for the block layout
                return mBlockLayout.Location.Y + Math.Max ( mBlockHeight, Convert.ToInt32 ( mBlockLayout.Height * sizeMultiplier ) ) +
                    mBlockLayout.Margin.Bottom + BorderHeight;
                }
            }

        public void Resize_All() 
        {
            Resize_Label();
            Resize_Blocks(); 
        }

        // Blocks are added, removed, or their width has changed after the audio scale changed.
        // Heights do not change, unless wrapping.
        private void Resize_Blocks()
        {
            if (mWrap)
            {
                Resize_Wrap();
            }
            else
            {
                // the block layout is resized to fit the blocks exactly; we use the last control to get the total width
                // Control k = mBlockLayout.Controls.Count > 0 ? mBlockLayout.Controls[mBlockLayout.Controls.Count - 1] : null;
                // int width_blocks = k == null ? mBlockLayout.Margin.Horizontal : k.Location.X + k.Width + k.Margin.Right;
                int width_blocks = BlockLayoutFullWidth;
                mBlockLayout.Width = width_blocks;
                Width = WidthForContents;
            }
        }

        // Resize after the label has changed (edited or editable status changed)
        // This is not affected by the wrap contents setting.
        private void Resize_Label()
        {
            // move the block layout up or down if the label height has changed
            // and resize the strip accordingly
            mBlockLayout.Location = new Point(mBlockLayout.Location.X,
                mLabel.Location.Y + mLabel.Height + mLabel.Margin.Bottom + mBlockLayout.Margin.Top);
            Size = new Size(WidthForContents,
                mBlockLayout.Location.Y + mBlockLayout.Height + mBlockLayout.Margin.Bottom + BorderHeight);
        }

        // Resize after the view has changed.
        // No effect when not wrapping.
        private void Resize_View()
        {
            if (mWrap) Resize_Wrap();
        }

        // Resize after wrapping has changed.
        private void Resize_Wrap()
        {
            if (mWrap)
            {
                mBlockLayout.AutoSize = true;
                mBlockLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                mBlockLayout.WrapContents = true;
                // The width of the block layout should fit the available space, unless it's narrower.
                // This may be overridden however by the minimum width to fit the widest waveform.
                int width_fit = mContentView.ClientRectangle.Width - Margin.Horizontal - BorderWidth -
                    mBlockLayout.Margin.Horizontal;
                mBlockLayout.MaximumSize =
                    new Size(Math.Max(BlockLayoutMinimumWidth, Math.Min(BlockLayoutFullWidth, width_fit)), 0);
                Size = new Size(WidthForContents, HeightForContents);
            }
            else
            {
                mBlockLayout.AutoSize = false;
                mBlockLayout.WrapContents = false;
                mBlockLayout.MaximumSize = new Size(0, 0);
                mBlockLayout.Height = mBlockHeight;
                Height = HeightForContents;
                Resize_Blocks();
            }
        }

        // Resize after the zoom factor has changed.
        private void Resize_Zoom()
        {
            if (mWrap)
            {
                Resize_Wrap();
            }
            else
            {
                mBlockLayout.Height = mBlockHeight;
                Height = mBlockLayout.Location.Y + mBlockLayout.Height + mBlockLayout.Margin.Bottom + BorderHeight;
                Resize_Blocks();
            }
        }

        // Set verbose accessible name for the strip 
        public void SetAccessibleName()
        {
            if (Selection is StripIndexSelection)
            {
                mLabel.AccessibleName = Selection.ToString();
            }
            else
            {
                mLabel.AccessibleName = string.Concat(mNode.Used ? "" : Localizer.Message("unused"),
                    mNode.Label + " " ,
                    mNode.Duration == 0.0 ? Localizer.Message("empty") : string.Format(Localizer.Message("duration_s_ms"), mNode.Duration / 1000.0),
                    string.Format(Localizer.Message("section_level_to_string"), mNode.IsRooted ? mNode.Level : 0),
                    mNode.PhraseChildCount == 0 ? "" :
                        mNode.PhraseChildCount == 1 ? Localizer.Message("section_one_phrase_to_string") :
                            string.Format(Localizer.Message("section_phrases_to_string"), mNode.PhraseChildCount),
                IsBlocksVisible ? "" : Localizer.Message ("ContentsHidden_StatusMessage") ); // @phraseLimit
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
                    mBlockLayout.BackColor =
                    mHighlighted ? settings.StripSelectedBackColor :
                    mNode.Used ?( mNode.PhraseChildCount > 0 ? settings.StripBackColor : settings.StripWithoutPhrasesBackcolor ): settings.StripUnusedBackColor;
                                mLabel.ForeColor =
                    mHighlighted ? settings.StripSelectedForeColor :
                    mNode.Used ? settings.StripForeColor : settings.StripUnusedForeColor;
                mLabel.UpdateColors(settings);
                foreach (Control c in mBlockLayout.Controls)
                {
                    if (c is Block) ((Block)c).UpdateColors();
                    else if (c is StripCursor) ((StripCursor)c).UpdateColors();
                }
            }
        }

        // Update the accessible label of the strip cursors after the given index.
        private void UpdateStripCursorsAccessibleName(int afterIndex)
        {
            for (int i = afterIndex + 2; i < mBlockLayout.Controls.Count; i += 2)
            {
                System.Diagnostics.Debug.Assert(mBlockLayout.Controls[i] is StripCursor);
                ((StripCursor)mBlockLayout.Controls[i]).SetAccessibleNameForIndex(i / 2);
            }
        }

        // Width of the strip to contain the label and the block layout, including borders.
        private int WidthForContents
        {
            get
            {
                int width_label = mLabel.Width + mLabel.Margin.Horizontal;
                int width_layout = mBlockLayout.Width + mBlockLayout.Margin.Horizontal;
                return Math.Max(width_label, width_layout) + BorderWidth;
            }
        }


        // Resize when a block size has changed.
        private void Block_SizeChanged(object sender, EventArgs e) { Resize_Blocks(); }

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
            if (mLabel.Label.Trim ()  != "")
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

        private void Strip_LocationChanged(object sender, EventArgs e)
        {
            Point offset = ((ScrollableControl)Parent).AutoScrollPosition;
            Point l = new Point(Location.X - offset.X, Location.Y - offset.Y);
            System.Diagnostics.Debug.Print("Location changed to {0}/{1}", Location, l);
        }

        //@singleSection
        /// <summary>
        /// updates strip without updating its blocks
        /// </summary>
        public void RefreshStrip ()
            {
            Label = mNode.Label;
            //UpdateColors ();
            }


        //@singleSection
        private System.Windows.Forms.FlowLayoutPanel m_BackgroundBlockLayout;
        private FlowLayoutPanel CreateBackUpLayout ()
            {
            FlowLayoutPanel backupBlockLayout = new FlowLayoutPanel ();
            backupBlockLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            backupBlockLayout.BackColor = System.Drawing.Color.GreenYellow;
            backupBlockLayout.Location = new System.Drawing.Point ( this.Width, 78 );
            backupBlockLayout.Margin = new System.Windows.Forms.Padding ( 3, 0, 3, 0 );
            backupBlockLayout.Name = "m_BackgroundBlockLayout";
            backupBlockLayout.Size = new System.Drawing.Size ( 303, 109 );
            backupBlockLayout.TabIndex = 1;
            backupBlockLayout.WrapContents = true;
            backupBlockLayout.Click += new System.EventHandler ( this.Strip_Enter );

            this.Controls.Add ( backupBlockLayout );
            return backupBlockLayout;
            }


        EmptyNode m_BackgroundPhrasesLoadStartNode;
        EmptyNode m_BackgroundPhrasesLoadEndNode;
        public void LoadBackUpLayout ( EmptyNode startNode, EmptyNode endNode )
            {
            m_BackgroundBlockLayout = CreateBackUpLayout ();
            m_BackgroundPhrasesLoadStartNode = startNode;
            m_BackgroundPhrasesLoadEndNode = endNode;
            m_BackgroundPhrasesLoadStartIndex = -1;
            m_BackgroundPhrasesloadTimer.Interval = 600;
            m_BackgroundPhrasesloadTimer.Tick += new EventHandler ( BackgroundPhrasesloadTimer_tick );
            m_BackgroundPhrasesloadTimer.Start ();

            }

        System.Windows.Forms.Timer m_BackgroundPhrasesloadTimer = new System.Windows.Forms.Timer ();

        int m_BackgroundPhrasesLoadStartIndex = -1;
        int m_BackgroundPhrasesLoadEndIndex = -1;
        private void BackgroundPhrasesloadTimer_tick ( object sender, EventArgs e )
            {
            m_BackgroundPhrasesloadTimer.Stop ();

            if (m_BackgroundPhrasesLoadStartIndex == -1)
                m_BackgroundPhrasesLoadStartIndex = m_BackgroundPhrasesLoadStartNode.Index;
            else
                m_BackgroundPhrasesLoadStartIndex = m_BackgroundPhrasesLoadEndIndex + 1;

            m_BackgroundPhrasesLoadEndIndex = m_BackgroundPhrasesLoadStartIndex + 10;
            if (m_BackgroundPhrasesLoadEndIndex > m_BackgroundPhrasesLoadEndNode.Index) m_BackgroundPhrasesLoadEndIndex = m_BackgroundPhrasesLoadEndNode.Index;

            BackUpLayoutBlockAddsRangeOfBlocks ( mNode.PhraseChild ( m_BackgroundPhrasesLoadStartIndex ), mNode.PhraseChild ( m_BackgroundPhrasesLoadEndIndex ) );

            if (m_BackgroundPhrasesLoadEndIndex == m_BackgroundPhrasesLoadEndNode.Index)
                {
                m_BackgroundPhrasesLoadStartIndex = -1;
                m_BackgroundPhrasesloadTimer.Stop ();
                UpdateColors ();
                m_BackgroundBlockLayout.Location =new Point ( mBlockLayout.Location.X, m_BackgroundBlockLayout.Height * -1 ) ;
                Console.WriteLine ( "background layout created " + m_BackgroundPhrasesLoadStartIndex + " - " + m_BackgroundPhrasesLoadEndIndex );
                }
            else
                {
                m_BackgroundPhrasesloadTimer.Start ();
                Console.WriteLine ( "console timer starting  " + m_BackgroundPhrasesLoadStartIndex + " - " + m_BackgroundPhrasesLoadEndIndex );
                }
            }

        //@singleSection
        public Block BackUpLayoutBlockAddsRangeOfBlocks ( EmptyNode startNode, EmptyNode endNode )
            {
            if (InvokeRequired)
                {
                return (Block)Invoke ( new BlockRangeCreationInvokation ( BackUpLayoutBlockAddsRangeOfBlocks ), startNode, endNode );
                }
            else
                {
                for (int i = startNode.Index; i <= endNode.Index; ++i)
                    {
                    BackupLayout_CreateBlockForNode ( mNode.PhraseChild ( i ), endNode.Index == i ? true : false );

                    }

                return null;
                }
            }


        //@singleSection
        private Block BackupLayout_CreateBlockForNode ( EmptyNode node, bool updateSize )
            {
            //MessageBox.Show ( node.Index.ToString () );
            if (m_BackgroundBlockLayout.Controls.Count == 0)
                {
                //@ StripCursor cursor = AddCursorAtBlockLayoutIndex ( 0 );
                int index = 0;
                StripCursor cursor = new StripCursor ();
                cursor.SetHeight ( mBlockHeight );
                cursor.ColorSettings = ColorSettings;
                cursor.TabStop = false;
                cursor.SetAccessibleNameForIndex ( index / 2 );
                m_BackgroundBlockLayout.Controls.Add ( cursor );
                m_BackgroundBlockLayout.Controls.SetChildIndex ( cursor, index );
                }
            Block block = node is PhraseNode ? new AudioBlock ( (PhraseNode)node, this ) : new Block ( node, this );
            m_BackgroundBlockLayout.Controls.Add ( block );
            //@singleSection: following 2 lines replaced
            //mBlockLayout.Controls.SetChildIndex(block, 1 + 2 * node.Index);
            //AddCursorAtBlockLayoutIndex(2 + 2 * node.Index);

            m_BackgroundBlockLayout.Controls.SetChildIndex ( block, 1 + 2 * (node.Index) );
            //@ AddCursorAtBlockLayoutIndex ( 2 + 2 * (node.Index - OffsetForFirstPhrase) );
            //--
            StripCursor cursor1 = new StripCursor ();
            int index1 = 2 + 2 * node.Index;
            cursor1.SetHeight ( mBlockHeight );
            cursor1.ColorSettings = ColorSettings;
            cursor1.TabStop = false;
            cursor1.SetAccessibleNameForIndex ( index1 / 2 );
            m_BackgroundBlockLayout.Controls.Add ( cursor1 );
            m_BackgroundBlockLayout.Controls.SetChildIndex ( cursor1, index1 );
            //---

            block.SetZoomFactorAndHeight ( mContentView.ZoomFactor, mBlockHeight );
            block.Cursor = Cursor;
            block.SizeChanged += new EventHandler ( Block_SizeChanged );

            //@ if (updateSize) Resize_Blocks ();

            //@ UpdateStripCursorsAccessibleName ( 2 + 2 * node.Index );
            //---
            m_BackgroundBlockLayout.AutoSize = true;
            m_BackgroundBlockLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            m_BackgroundBlockLayout.WrapContents = true;
            // The width of the block layout should fit the available space, unless it's narrower.
            // This may be overridden however by the minimum width to fit the widest waveform.
            int width_fit = mContentView.ClientRectangle.Width - Margin.Horizontal - BorderWidth -
                m_BackgroundBlockLayout.Margin.Horizontal;
            m_BackgroundBlockLayout.MaximumSize =
                new Size ( Math.Max ( BlockLayoutMinimumWidth, Math.Min ( BlockLayoutFullWidth, width_fit ) ), 0 );
            Size = new Size ( WidthForContents, HeightForContents );
            //---
            //System.Media.SystemSounds.Asterisk.Play ();
            return block;
            }

        public void ReplaceBlockLayout ()
            {
            if (m_BackgroundBlockLayout.Controls.Count == 0) return;
            Point layoutPoint = mBlockLayout.Location;
            mBlockLayout.Dispose ();
            m_BackgroundBlockLayout.Location = layoutPoint;
            mBlockLayout = m_BackgroundBlockLayout;
            }

        //@singleSection
        public void MoveCurrentBlocklayoutToBackground ()
            {
            if (m_BackgroundBlockLayout != null)
                {
                this.Controls.Remove ( m_BackgroundBlockLayout );
                m_BackgroundBlockLayout.Dispose ();
                }
            m_BackgroundBlockLayout = mBlockLayout;
            m_BackgroundBlockLayout.Location = new Point ( this.Location.X, m_BackgroundBlockLayout.Height * -1 );
            mBlockLayout = CreateBackUpLayout ();
            mBlockLayout.Location = new System.Drawing.Point ( 3, 78 );
            Resize_All ();
            Console.WriteLine ( "move to background  " + "size : " + m_BackgroundBlockLayout.Size + " location : " + m_BackgroundBlockLayout.Location);
            }


        //@singleSection
        public bool DisplayPreviousLayout ( EmptyNode expectedLastPhrase)
            {
            if (m_BackgroundBlockLayout == null || m_BackgroundBlockLayout.Controls.Count < 2) return false;

            bool isExpectedPhraseExists = false;
            for (int i = m_BackgroundBlockLayout.Controls.Count - 1; i >= 0; --i)
                {
                if (m_BackgroundBlockLayout.Controls[i] is Block && ((Block)m_BackgroundBlockLayout.Controls[i]).Node == expectedLastPhrase)
                    {
                    isExpectedPhraseExists = true;
                    break;
                    }
                }
            if (!isExpectedPhraseExists) return false;

            Point layoutLocation = new Point ( mBlockLayout.Location.X , mBlockLayout.Location.Y ) ;
            FlowLayoutPanel removePanel = mBlockLayout;
            removePanel.SendToBack ();
            this.Controls.Remove ( removePanel );

            mBlockLayout = m_BackgroundBlockLayout;
            m_OffsetForFirstPhrase = FirstBlock.Node.Index;
            mBlockLayout.Location = layoutLocation;
            mBlockLayout.BringToFront ();
            Resize_All ();
            if (removePanel != null )   removePanel.Dispose ();
            Console.WriteLine ( "Displaying previous layout  : size : location " + mBlockLayout.Size + " : " + mBlockLayout.Location);
            return true;
            }

        //@singleSection
        public void CreateNewLayout ( bool preserveExistingLayout )
            {
            FlowLayoutPanel oldBlocklayout = mBlockLayout;
            oldBlocklayout.SendToBack ();
            mBlockLayout = CreateBackUpLayout ();
            mBlockLayout.Location = new System.Drawing.Point ( 3, 78 );
            mBlockLayout.BringToFront ();
            
            if (!preserveExistingLayout)
                {
                this.Controls.Remove ( oldBlocklayout );
                oldBlocklayout.Dispose ();
                }
            Resize_All ();
            }

        public void DestroyStripHandle ()
            {
            this.DestroyHandle ();
            }
    }
}
