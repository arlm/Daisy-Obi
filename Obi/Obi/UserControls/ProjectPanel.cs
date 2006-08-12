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
    /// <summary>
    /// Top level panel that displays the current project, using a splitter (TOC on the left, strips on the right.)
    /// </summary>
    public partial class ProjectPanel : UserControl
    {
        private Project mProject;  // the project to display

        public Project Project
        {
            get
            {
                return mProject;
            }
            set
            {
                // Reset the handlers from the previous project
                if (mProject != null)
                {
                    mTOCPanel.RequestToAddSiblingSection -= new Events.Node.RequestToAddSiblingNodeHandler(mProject.CreateSiblingSectionNodeRequested);
                    mStripManagerPanel.AddSiblingSection -=
                        new Events.Node.RequestToAddSiblingNodeHandler(mProject.CreateSiblingSectionNodeRequested);
                    
                    mProject.AddedSectionNode -= new Events.Node.AddedSectionNodeHandler(mTOCPanel.SyncAddedSectionNode);
                    mProject.AddedSectionNode -= new Events.Node.AddedSectionNodeHandler(mStripManagerPanel.SyncAddedSectionNode);

                    mTOCPanel.RequestToAddChildSectionNode -= new Events.Node.RequestToAddChildSectionNodeHandler(mProject.CreateChildSectionNodeRequested);
                    
                    //these are all events related to moving nodes up and down
                    mTOCPanel.RequestToMoveSectionNodeUp -= new Events.Node.RequestToMoveSectionNodeUpHandler(mProject.MoveSectionNodeUpRequested);
                    mTOCPanel.RequestToMoveSectionNodeDown -= new Events.Node.RequestToMoveSectionNodeDownHandler(mProject.MoveSectionNodeDownRequested);
                    mProject.MovedNode -= new Events.Node.MovedNodeHandler(mTOCPanel.SyncMovedSectionNode);
                    mProject.MovedNode -= new Events.Node.MovedNodeHandler(mStripManagerPanel.SyncMovedNode);
                    mProject.UndidMoveNode -= new Events.Node.MovedNodeHandler(mTOCPanel.SyncMovedSectionNode);
                    mProject.UndidMoveNode -= new Events.Node.MovedNodeHandler(mStripManagerPanel.SyncMovedNode);

                    mTOCPanel.RequestToIncreaseSectionNodeLevel -= new Events.Node.RequestToIncreaseSectionNodeLevelHandler(mProject.IncreaseSectionNodeLevelRequested);
                    //marisa: the former "mProject.IncreasedSectionLevel" event is now handled by MovedNode

                    mTOCPanel.RequestToDecreaseSectionNodeLevel -= new Events.Node.RequestToDecreaseSectionNodeLevelHandler(mProject.DecreaseSectionNodeLevelRequested);
                    mProject.DecreasedSectionNodeLevel -= new Events.Node.DecreasedSectionNodeLevelHandler(mTOCPanel.SyncDecreasedSectionNodeLevel);

                    mTOCPanel.RequestToRenameSectionNode -= new Events.Node.RequestToRenameNodeHandler(mProject.RenameSectionNodeRequested);
                    mStripManagerPanel.RenameSection -= new Events.Node.RequestToRenameNodeHandler(mProject.RenameSectionNodeRequested);
                    mProject.RenamedNode -= new Events.Node.RenamedNodeHandler(mTOCPanel.SyncRenamedSectionNode);
                    mProject.RenamedNode -= new Events.Node.RenamedNodeHandler(mStripManagerPanel.SyncRenamedNode);

                    mTOCPanel.RequestToDeleteSectionNode -= new Events.Node.RequestToDeleteNodeHandler(mProject.RemoveNodeRequested);
                    mProject.DeletedNode -= new Events.Node.DeletedNodeHandler(mTOCPanel.SyncDeletedSectionNode);
                    mProject.DeletedNode -= new Events.Node.DeletedNodeHandler(mStripManagerPanel.SyncDeletedNode);

                    mStripManagerPanel.ImportAudioAssetRequested -= new Events.Strip.RequestToImportAssetHandler(mProject.ImportAssetRequested);
                    //mProject.ImportedAsset -= new Events.Node.ImportedAssetHandler(mStripManagerPanel.SyncCreateNewAudioBlock);
                    mProject.AddedPhraseNode -= new Events.Node.AddedPhraseNodeHandler(mStripManagerPanel.SyncAddedPhraseNode);

                    mStripManagerPanel.SetMediaRequested -= new Events.Node.SetMediaHandler(mProject.SetMediaRequested);
                    mProject.MediaSet -= new Events.Node.MediaSetHandler(mStripManagerPanel.SyncMediaSet);

                    mStripManagerPanel.SplitAudioBlockRequested -= new Events.Node.SplitNodeHandler(mProject.SplitAudioBlockRequested);
                    mStripManagerPanel.MergeNodes -= new Events.Node.MergeNodesHandler(mProject.MergeNodesRequested);

                    mStripManagerPanel.DeleteBlockRequested -=
                        new Events.Node.RequestToDeleteBlockHandler(mProject.DeletePhraseNodeRequested);
                    mProject.DeletedPhraseNode -= new Events.Node.DeletedNodeHandler(mStripManagerPanel.SyncDeleteAudioBlock);

                    mTOCPanel.RequestToCutSectionNode -= new Events.Node.RequestToCutSectionNodeHandler(mProject.CutSectionNodeRequested);
                    mProject.CutSectionNode -= new Events.Node.CutSectionNodeHandler(mTOCPanel.SyncCutSectionNode);
                    mProject.CutSectionNode -= new Events.Node.CutSectionNodeHandler(mStripManagerPanel.SyncCutSectionNode);
                    mProject.UndidCutSectionNode -= new Events.Node.MovedNodeHandler(mTOCPanel.SyncUndidCutSectionNode);
                    mProject.UndidCutSectionNode -= new Events.Node.MovedNodeHandler(mStripManagerPanel.SyncUndidCopySectionNode);

                    mTOCPanel.RequestToCopySectionNode -= new Events.Node.RequestToCopySectionNodeHandler(mProject.CopySectionNodeRequested);
                    mProject.CopiedSectionNode -= new Events.Node.CopiedSectionNodeHandler(mTOCPanel.SyncCopiedSectionNode);
                    mProject.CopiedSectionNode -= new Events.Node.CopiedSectionNodeHandler(mStripManagerPanel.SyncCopiedSectionNode);
                    mProject.UndidCopySectionNode -= new Events.Node.CopiedSectionNodeHandler(mTOCPanel.SyncUndidCopySectionNode);
                    mProject.UndidCopySectionNode -= new Events.Node.CopiedSectionNodeHandler(mStripManagerPanel.SyncUndidCopySectionNode);

                    mTOCPanel.RequestToPasteSectionNode -= new Events.Node.RequestToPasteSectionNodeHandler(mProject.PasteSectionNodeRequested);
                    mProject.PastedSectionNode -= new Events.Node.PastedSectionNodeHandler(mTOCPanel.SyncPastedSectionNode);
                    mProject.PastedSectionNode -= new Events.Node.PastedSectionNodeHandler(mStripManagerPanel.SyncPastedSectionNode);
                    mProject.UndidPasteSectionNode -= new Events.Node.UndidPasteSectionNodeHandler(mTOCPanel.SyncUndidPasteSectionNode);
                    mProject.UndidPasteSectionNode -= new Events.Node.UndidPasteSectionNodeHandler(mStripManagerPanel.SyncUndidPasteSectionNode);

                    mProject.TouchedNode += new Events.Node.TouchedNodeHandler(mStripManagerPanel.SyncTouchedNode);

                }
                // Set up the handlers for the new project
                if (value != null)
                {
                    mTOCPanel.RequestToAddSiblingSection += new Events.Node.RequestToAddSiblingNodeHandler(value.CreateSiblingSectionNodeRequested);
                    mStripManagerPanel.AddSiblingSection +=
                        new Events.Node.RequestToAddSiblingNodeHandler(value.CreateSiblingSectionNodeRequested);
                    value.AddedSectionNode += new Events.Node.AddedSectionNodeHandler(mTOCPanel.SyncAddedSectionNode);
                    value.AddedSectionNode += new Events.Node.AddedSectionNodeHandler(mStripManagerPanel.SyncAddedSectionNode);

                    mTOCPanel.RequestToAddChildSectionNode += new Events.Node.RequestToAddChildSectionNodeHandler(value.CreateChildSectionNodeRequested);

                    //these all relate to moving nodes up and down
                    mTOCPanel.RequestToMoveSectionNodeUp += new Events.Node.RequestToMoveSectionNodeUpHandler(value.MoveSectionNodeUpRequested);
                    mTOCPanel.RequestToMoveSectionNodeDown += new Events.Node.RequestToMoveSectionNodeDownHandler(value.MoveSectionNodeDownRequested);
                    value.MovedNode += new Events.Node.MovedNodeHandler(mTOCPanel.SyncMovedSectionNode);
                    value.MovedNode += new Events.Node.MovedNodeHandler(mStripManagerPanel.SyncMovedNode);
                    value.UndidMoveNode += new Events.Node.MovedNodeHandler(mTOCPanel.SyncMovedSectionNode);
                    value.UndidMoveNode += new Events.Node.MovedNodeHandler(mStripManagerPanel.SyncMovedNode);

                    mTOCPanel.RequestToIncreaseSectionNodeLevel +=
                        new Events.Node.RequestToIncreaseSectionNodeLevelHandler(value.IncreaseSectionNodeLevelRequested);
                    //marisa: the former "mProject.IncreasedSectionLevel" event is now handled by MovedNode

                    mTOCPanel.RequestToDecreaseSectionNodeLevel +=
                        new Events.Node.RequestToDecreaseSectionNodeLevelHandler(value.DecreaseSectionNodeLevelRequested);
                    value.DecreasedSectionNodeLevel += new Events.Node.DecreasedSectionNodeLevelHandler(mTOCPanel.SyncDecreasedSectionNodeLevel);

                    mTOCPanel.RequestToRenameSectionNode += new Events.Node.RequestToRenameNodeHandler(value.RenameSectionNodeRequested);
                    mStripManagerPanel.RenameSection += new Events.Node.RequestToRenameNodeHandler(value.RenameSectionNodeRequested);
                    value.RenamedNode += new Events.Node.RenamedNodeHandler(mTOCPanel.SyncRenamedSectionNode);
                    value.RenamedNode += new Events.Node.RenamedNodeHandler(mStripManagerPanel.SyncRenamedNode);

                    mTOCPanel.RequestToDeleteSectionNode += new Events.Node.RequestToDeleteNodeHandler(value.RemoveNodeRequested);
                    value.DeletedNode += new Events.Node.DeletedNodeHandler(mTOCPanel.SyncDeletedSectionNode);
                    value.DeletedNode += new Events.Node.DeletedNodeHandler(mStripManagerPanel.SyncDeletedNode);

                    // Block events
                    
                    mStripManagerPanel.ImportAudioAssetRequested +=
                        new Events.Strip.RequestToImportAssetHandler(value.ImportAssetRequested);
                    mStripManagerPanel.DeleteBlockRequested +=
                        new Events.Node.RequestToDeleteBlockHandler(value.DeletePhraseNodeRequested);
                    mStripManagerPanel.MoveAudioBlockForwardRequested +=
                        new Events.Node.RequestToMoveBlockHandler(value.MovePhraseNodeForwardRequested);
                    mStripManagerPanel.MoveAudioBlockBackwardRequested +=
                        new Events.Node.RequestToMoveBlockHandler(value.MovePhraseNodeBackwardRequested);
                    mStripManagerPanel.SetMediaRequested += new Events.Node.SetMediaHandler(value.SetMediaRequested);
                    mStripManagerPanel.SplitAudioBlockRequested +=
                        new Events.Node.SplitNodeHandler(value.SplitAudioBlockRequested);

                    value.AddedPhraseNode +=
                        new Events.Node.AddedPhraseNodeHandler(mStripManagerPanel.SyncAddedPhraseNode);
                    value.DeletedPhraseNode +=
                        new Events.Node.DeletedNodeHandler(mStripManagerPanel.SyncDeleteAudioBlock);
                    value.MediaSet += new Events.Node.MediaSetHandler(mStripManagerPanel.SyncMediaSet);
                    value.TouchedNode += new Events.Node.TouchedNodeHandler(mStripManagerPanel.SyncTouchedNode);




                    mStripManagerPanel.MergeNodes += new Events.Node.MergeNodesHandler(value.MergeNodesRequested);


                    //md: clipboard in the TOC
                    mTOCPanel.RequestToCutSectionNode += new Events.Node.RequestToCutSectionNodeHandler(value.CutSectionNodeRequested);
                    value.CutSectionNode += new Events.Node.CutSectionNodeHandler(mTOCPanel.SyncCutSectionNode);
                    value.CutSectionNode += new Events.Node.CutSectionNodeHandler(mStripManagerPanel.SyncCutSectionNode);
                    value.UndidCutSectionNode += new Events.Node.MovedNodeHandler(mTOCPanel.SyncUndidCutSectionNode);
                    value.UndidCutSectionNode += new Events.Node.MovedNodeHandler(mStripManagerPanel.SyncUndidCopySectionNode);

                    mTOCPanel.RequestToCopySectionNode += new Events.Node.RequestToCopySectionNodeHandler(value.CopySectionNodeRequested);
                    value.CopiedSectionNode += new Events.Node.CopiedSectionNodeHandler(mTOCPanel.SyncCopiedSectionNode);
                    value.CopiedSectionNode += new Events.Node.CopiedSectionNodeHandler(mStripManagerPanel.SyncCopiedSectionNode);
                    value.UndidCopySectionNode += new Events.Node.CopiedSectionNodeHandler(mTOCPanel.SyncUndidCopySectionNode);
                    value.UndidCopySectionNode += new Events.Node.CopiedSectionNodeHandler(mStripManagerPanel.SyncUndidCopySectionNode);

                    mTOCPanel.RequestToPasteSectionNode += new Events.Node.RequestToPasteSectionNodeHandler(value.PasteSectionNodeRequested);
                    value.PastedSectionNode += new Events.Node.PastedSectionNodeHandler(mTOCPanel.SyncPastedSectionNode);
                    value.PastedSectionNode += new Events.Node.PastedSectionNodeHandler(mStripManagerPanel.SyncPastedSectionNode);
                    value.UndidPasteSectionNode += new Events.Node.UndidPasteSectionNodeHandler(mTOCPanel.SyncUndidPasteSectionNode);
                    value.UndidPasteSectionNode += new Events.Node.UndidPasteSectionNodeHandler(mStripManagerPanel.SyncUndidPasteSectionNode);

                } 
                mProject = value;
                mSplitContainer.Visible = mProject != null;
                mSplitContainer.Panel1Collapsed = false;
            }
        }

        public Boolean TOCPanelVisible
        {
            get { return mProject != null && !mSplitContainer.Panel1Collapsed; }
        }

        public StripManagerPanel StripManager
        {
            get { return mStripManagerPanel; }
        }

        public TOCPanel TOCPanel
        {
            get { return mTOCPanel; }
        }

        /// <summary>
        /// Create a new project panel with currently no project.
        /// </summary>
        public ProjectPanel()
        {
            InitializeComponent();
            //mg 20060804: set two convenince attrs on the toc and tree views
            mTOCPanel.ProjectPanel = this;
            mStripManagerPanel.ProjectPanel = this;
            //end mg
            Project = null;
        }

        public void HideTOCPanel()
        {
            mSplitContainer.Panel1Collapsed = true;
        }

        public void ShowTOCPanel()
        {
            mSplitContainer.Panel1Collapsed = false;
        }


        /*public void Clear()
        {
            mTOCPanel.Clear();
        }*/

        /// <summary>
        /// Synchronize the project views with the core tree. Used when opening a XUK file.
        /// </summary>
        public void SynchronizeWithCoreTree(CoreNode root)
        {
            mTOCPanel.SynchronizeWithCoreTree(root);
            mStripManagerPanel.SynchronizeWithCoreTree(root);
        }
    }
}
