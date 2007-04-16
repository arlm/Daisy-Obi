using System;
using System.Windows.Forms;
using urakawa.core;
using urakawa.media;

namespace Obi.UserControls
{
    public partial class StripManagerPanel
    {
        /// <summary>
        /// Add a new block from a phrase node.
        /// The block is not selected; it should be by whoever wanted to add it (if necessary.)
        /// </summary>
        public void SyncAddedPhraseNode(object sender, Events.Node.PhraseNodeEventArgs e)
        {
            SyncAddedPhraseNode(e.Node);
        }

        private void SyncAddedPhraseNode(PhraseNode node)
        {
            if (node != null && node.ParentSection != null)
            {
                SectionStrip strip = mSectionNodeMap[node.ParentSection];
                AudioBlock block = SetupAudioBlockFromPhraseNode(node);
                strip.InsertAudioBlock(block, node.Index);
                if (node.PageProperty != null) mProjectPanel.Project.RenumberPages();
            }
        }

        /// <summary>
        /// Setup a new audio block from a phrase node.
        /// </summary>
        private AudioBlock SetupAudioBlockFromPhraseNode(PhraseNode node)
        {
            AudioBlock block = new AudioBlock();
            block.Manager = this;
            block.Node = node;
            mPhraseNodeMap[node] = block;
            TextMedia annotation = (TextMedia)Project.GetMediaForChannel(node, Project.AnnotationChannelName);
            if (annotation != null) block.AnnotationBlock.Label = annotation.getText();
            Assets.AudioMediaAsset asset = node.Asset;// Project.GetAudioMediaAsset(node);
            PageProperty pageProp = node.getProperty(typeof(PageProperty)) as PageProperty;
            return block;
        }

        /// <summary>
        /// Delete the block of a phrase node. If it was selected, it gets deselected.
        /// </summary>
        /// <param name="e">The node event with a pointer to the deleted phrase node.</param>
        public void SyncDeleteAudioBlock(object sender, Events.Node.PhraseNodeEventArgs e)
        {
            System.Diagnostics.Debug.Assert(e.Node != null);
            if (e.Node.ParentSection != null)
            {
                SectionStrip strip = mSectionNodeMap[e.Node.ParentSection];
                if (mProjectPanel.CurrentSelectionNode == e.Node) mProjectPanel.CurrentSelection = null;
                strip.RemoveAudioBlock(mPhraseNodeMap[e.Node]);
                mPhraseNodeMap.Remove(e.Node);
                if (e.Node.PageProperty != null) mProjectPanel.Project.RenumberPagesExcluding(e.Node);
            }
        }

        /// <summary>
        /// Changed a media object on a node.
        /// </summary>
        public void SyncMediaSet(object sender, Events.Node.SetMediaEventArgs e)
        {
            if (e.Node.ParentSection != null)
            {
                SectionStrip strip = mSectionNodeMap[(SectionNode)e.Node.getParent()];
                if (e.Channel == Project.AnnotationChannelName)
                {
                    // the label of an audio block has changed
                    strip.SetAnnotationBlock(mPhraseNodeMap[e.Node], ((TextMedia)e.Media).getText());
                }
                else if (e.Channel == Project.AudioChannelName)
                {
                    // the audio asset of an audio block has changed
                    strip.UpdateAssetAudioBlock(mPhraseNodeMap[e.Node]);
                }
            }
        }

        /// <summary>
        /// The node was modified.
        /// </summary>
        public void SyncTouchedNode(object sender, Events.Node.NodeEventArgs e)
        {
            // SelectedNode = e.Node as ObiNode;
        }

        /// <summary>
        /// The time of the asset for a phrase has changed.
        /// </summary>
        internal void SyncUpdateAudioBlockTime(object sender, Events.Strip.UpdateTimeEventArgs e)
        {
            mPhraseNodeMap[e.Node].RefreshDisplay(e.Time);
            System.Diagnostics.Debug.Print("{0} lasts {1}ms", e.Node, e.Time);
        }

        internal void InterceptKeyDownFromChildControl(KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        /// <summary>
        /// The page label has changed.
        /// </summary>
        internal void SyncSetPageNumber(object sender, Events.Node.PhraseNodeEventArgs e)
        {
            mPhraseNodeMap[e.Node].RefreshDisplay();
        }

        /// <summary>
        /// The page label was removed.
        /// </summary>
        internal void SyncRemovedPageNumber(object sender, Events.Node.PhraseNodeEventArgs e)
        {
            mPhraseNodeMap[e.Node].RefreshDisplay();
        }





        /// <summary>
        /// The insert point for a phrase node inside a section.
        /// </summary>
        private struct InsertPoint
        {
            public SectionNode node;  // the section node to add in
            public int index;         // the index at which to add
        }

        /// <summary>
        /// Get the current insertion point.
        /// If a section is selected, this is the end of the section.
        /// If a phrase is selected, this is the index of the phrase
        /// (so that insertion happens before.)
        /// </summary>
        private InsertPoint CurrentInsertPoint
        {
            get
            {
                InsertPoint insert = new InsertPoint();
                if (SelectedPhraseNode != null)
                {
                    insert.node = SelectedPhraseNode.ParentSection;
                    insert.index = SelectedPhraseNode.Index;
                }
                else if (SelectedSectionNode != null)
                {
                    insert.node = SelectedSectionNode;
                    insert.index = SelectedSectionNode.PhraseChildCount;
                }
                else
                {
                    insert.node = null;
                }
                return insert;
            }
        }

        /// <summary>
        /// True if there is a currently available insertion point.
        /// </summary>
        public bool CanInsertPhraseNode
        {
            get { return mProjectPanel.CurrentSelection != null && mProjectPanel.CurrentSelection.Control == this; }
        }

        /// <summary>
        /// True if there is a selected block currently in use that is preceded by another block in use as well.
        /// </summary>
        public bool CanMerge
        {
            get
            {
                return mProjectPanel.CurrentSelectedAudioBlock != null && mProjectPanel.CurrentSelectedAudioBlock.Used &&
                    mProjectPanel.CurrentSelectedAudioBlock.PreviousPhraseInSection != null &&
                    mProjectPanel.CurrentSelectedAudioBlock.PreviousPhraseInSection.Used;
            }
        }

        /// <summary>
        /// Import phrases from sound files at the end of a selected section or before a selected phrase.
        /// The files are chosen through a file browser. This can be cancelled through the file browser's
        /// cancel button, in which case no change is made.
        /// </summary>
        /// <remarks>One "import phrase" command will be created for every phrase that is inserted.</remarks>
        public void ImportPhrases()
        {
            if (CanInsertPhraseNode)
            {
                // we may lose the currently selected phrase when stopping
                InsertPoint insert = CurrentInsertPoint;
                mProjectPanel.TransportBar.Enabled = false;
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                dialog.Filter = Localizer.Message("audio_file_filter");
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    int first_index = insert.index;
                    foreach (string path in dialog.FileNames)
                    {
                        if (mProjectPanel.Project.DidAddPhraseFromFile(path, insert.node, insert.index))
                        {
                            ++insert.index;
                        }
                        else
                        {
                            MessageBox.Show(String.Format(Localizer.Message("import_phrase_error_text"), path),
                                Localizer.Message("import_phrase_error_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    // select the first added phrase
                    mProjectPanel.CurrentSelection = new NodeSelection(insert.node, this);
                }
                mProjectPanel.TransportBar.Enabled = true;
            }
        }

        /// <summary>
        /// Split (with preview) of the selected block.
        /// </summary>
        public void SplitBlock()
        {
            if (mProjectPanel.CurrentSelectedAudioBlock != null)
            {
                PhraseNode phrase = mProjectPanel.CurrentSelectedAudioBlock;
                double time = mProjectPanel.TransportBar._CurrentPlaylist.CurrentTimeInAsset;
                Audio.AudioPlayerState state = mProjectPanel.TransportBar._CurrentPlaylist.State;
                mProjectPanel.TransportBar.Enabled = false;
                Dialogs.Split dialog = new Dialogs.Split(phrase, time, state);
                NodeSelection originalSelection = mProjectPanel.CurrentSelection;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (dialog.ResultAsset != null)
                    {
                        PhraseNode SplitPhraseNode = mProjectPanel.Project.Split(phrase, dialog.ResultAsset ) ;
                        mProjectPanel.SynchronizeWithCoreTree();
                        mProjectPanel.CurrentSelection =
                            new NodeSelection(SplitPhraseNode , this);
                    }
                }
                else
                {
                    // kludge to reselect the original asset which was deselected when playback started
                    mProjectPanel.CurrentSelection = originalSelection;
                }
                mProjectPanel.TransportBar.Enabled = true;
            }
        }

        /// <summary>
        /// Quick split (without preview) of the selected block.
        /// </summary>
        public void QuickSplitBlock()
        {
            if (mProjectPanel.CurrentSelectedAudioBlock != null)
            {
                PhraseNode phrase = mProjectPanel.CurrentSelectedAudioBlock;
                PhraseNode split = null;
                double time = ProjectPanel.TransportBar._CurrentPlaylist.CurrentTimeInAsset;
                mProjectPanel.TransportBar.Enabled = false;
                Assets.AudioMediaAsset asset = phrase.Asset;
                if (time > 0 && time < asset.LengthInMilliseconds)
                {
                    Assets.AudioMediaAsset result = asset.Manager.SplitAudioMediaAsset(asset, time);
                    split = mProjectPanel.Project.Split(phrase, result);
                }
                mProjectPanel.TransportBar.Enabled = true;
                if (split != null)
                {
                    
                    mProjectPanel.SynchronizeWithCoreTree();
                    mProjectPanel.CurrentSelection = new NodeSelection(split, this);
                    mProjectPanel.TransportBar.Play();
                }
            }
        }

        /// <summary>
        /// Merge the selected block with the previous one in the strip.
        /// </summary>
        public void MergeBlocks()
        {
            if (CanMerge)
            {
                mProjectPanel.TransportBar.Enabled = false;
                mProjectPanel.CurrentSelection = new NodeSelection(mProjectPanel.Project.MergeNodes(
                    mProjectPanel.CurrentSelectedAudioBlock.PreviousPhraseInSection,
                    mProjectPanel.CurrentSelectedAudioBlock),
                    this);
                mProjectPanel.TransportBar.Enabled = true;
            }
        }

        /// <summary>
        /// Insert an empty audio block at the current insertion point.
        /// </summary>
        /// <remarks>Currently disabled</remarks>
        public void InsertEmptyAudioBlock()
        {
            InsertPoint insert = this.CurrentInsertPoint;
            if (insert.node != null) mProjectPanel.Project.AddEmptyPhraseNode(insert.node, insert.index);   
        }

        /// <summary>
        /// Move the selected audio block, if any, in the given direction.
        /// </summary>
        /// <param name="direction">Move the block backward or forward.</param>
        public void MoveBlock(PhraseNode.Direction direction)
        {
            if (mProjectPanel.CurrentSelectedAudioBlock != null) mProjectPanel.Project.MovePhraseNode(mProjectPanel.CurrentSelectedAudioBlock, direction);
        }

        /// <summary>
        /// Set a page number on the selected block.
        /// </summary>
        public void SetPageNumber()
        {
            if (mProjectPanel.CurrentSelectedAudioBlock != null) mProjectPanel.Project.SetPageNumberOnPhraseWithUndo(mProjectPanel.CurrentSelectedAudioBlock);
        }

        /// <summary>
        /// Remove a page number on the selected block.
        /// </summary>
        public void RemovePageNumber()
        {
            if (mProjectPanel.CurrentSelectedAudioBlock != null) mProjectPanel.Project.RemovePageNumberFromPhraseWithUndo(mProjectPanel.CurrentSelectedAudioBlock);
        }

        /// <summary>
        /// Select the previous phrase of the selected phrase in the strip manager.
        /// If a section is selected, select the last phrase of the previous section,
        /// or the previous section itself if it doesn't have phrases.
        /// If nothing is selected, select the last phrase of the last section,
        /// or the last section itself if it doesn't have phrases.
        /// </summary>
        public void PreviousPhrase()
        {
            ObiNode prev = null;
            if (mProjectPanel.Project != null)
            {
                if (mProjectPanel.CurrentSelectedAudioBlock != null)
                {
                    prev = mProjectPanel.CurrentSelectedAudioBlock.PreviousPhraseInSection;
                    if (prev == null)
                    {
                        prev = mProjectPanel.CurrentSelectedAudioBlock.ParentSection.PreviousSection;
                        if (prev != null && ((SectionNode)prev).PhraseChildCount > 0)
                            prev = ((SectionNode)prev).PhraseChild(-1);
                    }
                }
                else if (mProjectPanel.CurrentSelectedStrip != null)
                {
                    prev = mProjectPanel.CurrentSelectedStrip.PreviousSection;
                    if (prev != null && ((SectionNode)prev).PhraseChildCount > 0)
                        prev = ((SectionNode)prev).PhraseChild(((SectionNode)prev).PhraseChildCount - 1);
                }
                else
                {
                    SectionNode last = mProjectPanel.Project.LastSection;
                    if (last != null)
                    {
                        prev = last.PhraseChildCount > 0 ? (ObiNode)last.PhraseChild(-1) : (ObiNode)last;
                    }
                }
            }
            if (prev != null) mProjectPanel.CurrentSelection = new NodeSelection(prev, this);
        }

        /// <summary>
        /// Select the next phrase for a selected phrase in the strip manager.
        /// If a section is selected, then select the first phrase of the section
        /// or the next section if it has no phrases.
        /// If nothing is selected, select the first phrase of the first section,
        /// or the first section itself if it has no phrases.
        /// </summary>
        public void NextPhrase()
        {
            ObiNode next = null;
            if (mProjectPanel.Project != null)
            {
                if (mProjectPanel.CurrentSelectedAudioBlock != null)
                {
                    next = mProjectPanel.CurrentSelectedAudioBlock.NextPhraseInSection;
                    if (next == null)
                    {
                        next = mProjectPanel.CurrentSelectedAudioBlock.ParentSection.NextSection;
                        if (next != null && ((SectionNode)next).PhraseChildCount > 0)
                            next = ((SectionNode)next).PhraseChild(0);
                    }
                }
                else if (mProjectPanel.CurrentSelectedStrip != null)
                {
                    next = mProjectPanel.CurrentSelectedStrip.PhraseChildCount > 0 ? (ObiNode)mProjectPanel.CurrentSelectedStrip.PhraseChild(0) :
                        mProjectPanel.CurrentSelectedStrip.NextSection;
                }
                else
                {
                    SectionNode first = mProjectPanel.Project.FirstSection;
                    if (first != null)
                    {
                        next = first.PhraseChildCount > 0 ? (ObiNode)first.PhraseChild(0) : (ObiNode)first;
                    }
                }
            }
            if (next != null) mProjectPanel.CurrentSelection = new NodeSelection(next, this);
        }

        public void GoToPage()
        {
            if (mProjectPanel.Project.Pages > 0)
            {
                Dialogs.GoToPage dialog = new Dialogs.GoToPage(mProjectPanel.Project);
                if (dialog.ShowDialog() == DialogResult.OK && dialog.SelectedPage != null)
                {
                    mProjectPanel.CurrentSelection = new NodeSelection(dialog.SelectedPage, this);
                }
            }
        }

        internal void UpdateAssetForPhrase(PhraseNode node, Obi.Assets.AudioMediaAsset asset)
        {
            node.Asset = asset;
            // SelectedNode = node;

            mPhraseNodeMap[node].RefreshDisplay();
        }
    }
}
