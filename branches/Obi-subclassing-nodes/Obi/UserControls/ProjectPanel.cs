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
            get { return mProject; }
            set
            {
                if (mProject != null) ResetEventHandlers();
                if (value != null) SetEventHandlers(value);
                mProject = value;
                mSplitContainer.Visible = mProject != null;
                ShowTOCPanel();
            }
        }

        #region Setting/resetting handlers

        /// <summary>
        /// Set all the event handlers for the panel
        /// </summary>
        /// <param name="value">The project for which we are setting handlers.</param>
        private void SetEventHandlers(Project project)
        {
            project.AddedPhraseNode += new Events.PhraseNodeHandler(mStripManagerPanel.SyncAddedPhraseNode);
            project.AddedSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncAddedSectionNode);
            project.AddedSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncAddedEmptySectionNode);
            project.CutSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncCutSectionNode);
            project.CutSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncCutSectionNode);
            project.DeletedPhraseNode += new Events.PhraseNodeHandler(mStripManagerPanel.SyncDeleteAudioBlock);
            project.DeletedSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncDeletedNode);
            project.DeletedSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncDeletedSectionNode);
            project.PastedSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncPastedSectionNode);
            project.PastedSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncPastedSectionNode);
            project.RemovedPageNumber += new Events.PhraseNodeHandler(mStripManagerPanel.SyncRemovedPageNumber);
            project.SetPageNumber += new Events.PhraseNodeHandler(mStripManagerPanel.SyncSetPageNumber);
            project.TouchedPhraseNode += new Events.PhraseNodeHandler(mStripManagerPanel.SyncTouchedPhraseNode);
            project.UpdateTime += new Events.PhraseNodeUpdateTimeHandler(mStripManagerPanel.SyncUpdateAudioBlockTime);
            mStripManagerPanel.DeleteBlockRequested += new Events.PhraseNodeHandler(project.DeletePhraseNodeRequested);
            mStripManagerPanel.ImportAudioAssetRequested += new Events.Strip.RequestToImportAssetHandler(project.ImportAssetRequested);
            mStripManagerPanel.MergePhraseNodes += new Events.MergePhraseNodesHandler(project.MergePhraseNodesRequested);
            mStripManagerPanel.MoveAudioBlockBackwardRequested += new Events.PhraseNodeHandler(project.MovePhraseNodeBackwardRequested);
            mStripManagerPanel.MoveAudioBlockForwardRequested += new Events.PhraseNodeHandler(project.MovePhraseNodeForwardRequested);
            mStripManagerPanel.SetMediaRequested += new Events.PhraseNodeSetMediaHandler(project.SetMediaRequested);
            mStripManagerPanel.RequestToCopyPhraseNode += new Events.PhraseNodeHandler(project.CopyPhraseNode);
            mStripManagerPanel.RequestToCutPhraseNode += new Events.PhraseNodeHandler(project.CutPhraseNode);
            mStripManagerPanel.RequestToPastePhraseNode += new Events.ObiNodeHandler(project.PastePhraseNode);
            mStripManagerPanel.RequestToRemovePageNumber += new Events.PhraseNodeHandler(project.RemovePageRequested);
            mStripManagerPanel.RequestToSetPageNumber += new Events.SetPageNumberHandler(project.SetPageRequested);
            mStripManagerPanel.RequestToShallowDeleteSectionNode += new Events.SectionNodeHandler(project.ShallowDeleteSectionNodeRequested);
            mStripManagerPanel.SplitAudioBlockRequested += new Events.SplitPhraseNodeHandler(project.SplitAudioBlockRequested);
            mTOCPanel.RequestToPasteSectionNode += new Events.SectionNodeHandler(project.PasteSectionNodeRequested);

            
            mTOCPanel.RequestToAddChildSectionNode +=
                new Events.Node.Section.RequestToAddChildSectionNodeHandler(project.CreateChildSectionNodeRequested);
            mTOCPanel.RequestToAddSiblingSection +=
                new Events.Node.Section.RequestToAddSiblingSectionNodeHandler(project.CreateSiblingSectionNodeRequested);
            mStripManagerPanel.AddSiblingSection +=
                new Events.Node.Section.RequestToAddSiblingSectionNodeHandler(project.CreateSiblingSectionNodeRequested);
            mTOCPanel.RequestToCutSectionNode +=
                new Events.Node.Section.RequestToCutSectionNodeHandler(project.CutSectionNodeRequested);
            mTOCPanel.RequestToDeleteSectionNode +=
                new Events.Node.Section.RequestToDeleteSectionNodeHandler(project.RemoveNodeRequested);
            project.UndidPasteSectionNode +=
                new Events.Node.Section.UndidPasteSectionNodeHandler(mTOCPanel.SyncUndidPasteSectionNode);
            project.UndidPasteSectionNode +=
                new Events.Node.Section.UndidPasteSectionNodeHandler(mStripManagerPanel.SyncUndidPasteSectionNode);





            //these all relate to moving nodes up and down
            mTOCPanel.RequestToMoveSectionNodeUp += new Events.Node.RequestToMoveSectionNodeUpHandler(project.MoveSectionNodeUpRequested);
            mTOCPanel.RequestToMoveSectionNodeDown += new Events.Node.RequestToMoveSectionNodeDownHandler(project.MoveSectionNodeDownRequested);
            project.MovedSectionNode += new Events.Node.MovedNodeHandler(mTOCPanel.SyncMovedSectionNode);
            project.MovedSectionNode += new Events.Node.MovedNodeHandler(mStripManagerPanel.SyncMovedNode);
            project.UndidMoveNode += new Events.Node.MovedNodeHandler(mTOCPanel.SyncMovedSectionNode);
            project.UndidMoveNode += new Events.Node.MovedNodeHandler(mStripManagerPanel.SyncMovedNode);
            mStripManagerPanel.RequestToMoveSectionNodeDownLinear += new Events.Node.RequestToMoveSectionNodeDownLinearHandler(project.MoveSectionNodeDownLinearRequested);
            mStripManagerPanel.RequestToMoveSectionNodeUpLinear += new Events.Node.RequestToMoveSectionNodeUpLinearHandler(project.MoveSectionNodeUpLinearRequested);
            project.ShallowSwappedSectionNodes += new Events.Node.ShallowSwappedSectionNodesHandler(mTOCPanel.SyncShallowSwapNodes);
            project.ShallowSwappedSectionNodes += new Events.Node.ShallowSwappedSectionNodesHandler(mStripManagerPanel.SyncShallowSwapNodes);

            mTOCPanel.RequestToIncreaseSectionNodeLevel +=
                new Events.Node.RequestToIncreaseSectionNodeLevelHandler(project.IncreaseSectionNodeLevelRequested);
            //marisa: the former "mProject.IncreasedSectionLevel" event is now handled by MovedNode

            mTOCPanel.RequestToDecreaseSectionNodeLevel +=
                new Events.Node.RequestToDecreaseSectionNodeLevelHandler(project.DecreaseSectionNodeLevelRequested);
            project.DecreasedSectionNodeLevel += new Events.Node.DecreasedSectionNodeLevelHandler(mTOCPanel.SyncDecreasedSectionNodeLevel);

            mTOCPanel.RequestToRenameSectionNode +=
                new Events.Node.Section.RequestToRenameSectionNodeHandler(project.RenameSectionNodeRequested);
            mStripManagerPanel.RenameSection +=
                new Events.Node.Section.RequestToRenameSectionNodeHandler(project.RenameSectionNodeRequested);
            project.RenamedSectionNode +=
                new Events.Node.Section.RenamedSectionNodeHandler(mTOCPanel.SyncRenamedSectionNode);
            project.RenamedSectionNode +=
                new Events.Node.Section.RenamedSectionNodeHandler(mStripManagerPanel.SyncRenamedSectionNode);


            // Block events



            // project.MediaSet += new Events.Node.MediaSetHandler(mStripManagerPanel.SyncMediaSet);





            //md: clipboard in the TOC

            mTOCPanel.RequestToCopySectionNode += new Events.Node.RequestToCopySectionNodeHandler(project.CopySectionNodeRequested);
            project.CopiedSectionNode += new Events.Node.CopiedSectionNodeHandler(mTOCPanel.SyncCopiedSectionNode);
            project.CopiedSectionNode += new Events.Node.CopiedSectionNodeHandler(mStripManagerPanel.SyncCopiedSectionNode);
            project.UndidCopySectionNode += new Events.Node.CopiedSectionNodeHandler(mTOCPanel.SyncUndidCopySectionNode);
            project.UndidCopySectionNode += new Events.Node.CopiedSectionNodeHandler(mStripManagerPanel.SyncUndidCopySectionNode);




        }

        private void ResetEventHandlers()
        {
            mProject.AddedPhraseNode -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncAddedPhraseNode);
            mProject.AddedSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncAddedEmptySectionNode);
            mProject.AddedSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncAddedSectionNode);
            mProject.CutSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncCutSectionNode);
            mProject.CutSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncCutSectionNode);
            mProject.DeletedPhraseNode -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncDeleteAudioBlock);
            mProject.DeletedSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncDeletedNode);
            mProject.DeletedSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncDeletedSectionNode);
            mProject.PastedSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncPastedSectionNode);
            mProject.PastedSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncPastedSectionNode);
            mProject.RemovedPageNumber -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncRemovedPageNumber);
            mProject.SetPageNumber -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncSetPageNumber);
            mProject.TouchedPhraseNode -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncTouchedPhraseNode);
            mProject.UpdateTime -= new Events.PhraseNodeUpdateTimeHandler(mStripManagerPanel.SyncUpdateAudioBlockTime);
            mStripManagerPanel.DeleteBlockRequested -= new Events.PhraseNodeHandler(mProject.DeletePhraseNodeRequested);
            mStripManagerPanel.ImportAudioAssetRequested -= new Events.Strip.RequestToImportAssetHandler(mProject.ImportAssetRequested);
            mStripManagerPanel.MergePhraseNodes -= new Events.MergePhraseNodesHandler(mProject.MergePhraseNodesRequested);
            mStripManagerPanel.SetMediaRequested -= new Events.PhraseNodeSetMediaHandler(mProject.SetMediaRequested);
            mStripManagerPanel.RequestToCopyPhraseNode -= new Events.PhraseNodeHandler(mProject.CopyPhraseNode);
            mStripManagerPanel.RequestToCutPhraseNode -= new Events.PhraseNodeHandler(mProject.CutPhraseNode);
            mStripManagerPanel.RequestToPastePhraseNode -= new Events.ObiNodeHandler(mProject.PastePhraseNode);
            mStripManagerPanel.RequestToRemovePageNumber -= new Events.PhraseNodeHandler(mProject.RemovePageRequested);
            mStripManagerPanel.RequestToSetPageNumber -= new Events.SetPageNumberHandler(mProject.SetPageRequested);
            mStripManagerPanel.RequestToShallowDeleteSectionNode -= new Events.SectionNodeHandler(mProject.ShallowDeleteSectionNodeRequested);
            mStripManagerPanel.SplitAudioBlockRequested -= new Events.SplitPhraseNodeHandler(mProject.SplitAudioBlockRequested);
            mTOCPanel.RequestToPasteSectionNode -= new Events.SectionNodeHandler(mProject.PasteSectionNodeRequested);
            


            mTOCPanel.RequestToAddChildSectionNode -=
                new Events.Node.Section.RequestToAddChildSectionNodeHandler(mProject.CreateChildSectionNodeRequested);
            mTOCPanel.RequestToAddSiblingSection -=
                new Events.Node.Section.RequestToAddSiblingSectionNodeHandler(mProject.CreateSiblingSectionNodeRequested);
            mStripManagerPanel.AddSiblingSection -=
                new Events.Node.Section.RequestToAddSiblingSectionNodeHandler(mProject.CreateSiblingSectionNodeRequested);
            mTOCPanel.RequestToCutSectionNode -=
                new Events.Node.Section.RequestToCutSectionNodeHandler(mProject.CutSectionNodeRequested);
            mTOCPanel.RequestToDeleteSectionNode -=
                new Events.Node.Section.RequestToDeleteSectionNodeHandler(mProject.RemoveNodeRequested);
            mProject.UndidPasteSectionNode -=
                new Events.Node.Section.UndidPasteSectionNodeHandler(mStripManagerPanel.SyncUndidPasteSectionNode);
            mProject.UndidPasteSectionNode -=
                new Events.Node.Section.UndidPasteSectionNodeHandler(mTOCPanel.SyncUndidPasteSectionNode);




            //these are all events related to moving nodes up and down
            mTOCPanel.RequestToMoveSectionNodeUp -= new Events.Node.RequestToMoveSectionNodeUpHandler(mProject.MoveSectionNodeUpRequested);
            mTOCPanel.RequestToMoveSectionNodeDown -= new Events.Node.RequestToMoveSectionNodeDownHandler(mProject.MoveSectionNodeDownRequested);
            mProject.MovedSectionNode -= new Events.Node.MovedNodeHandler(mTOCPanel.SyncMovedSectionNode);
            mProject.MovedSectionNode -= new Events.Node.MovedNodeHandler(mStripManagerPanel.SyncMovedNode);
            mProject.UndidMoveNode -= new Events.Node.MovedNodeHandler(mTOCPanel.SyncMovedSectionNode);
            mProject.UndidMoveNode -= new Events.Node.MovedNodeHandler(mStripManagerPanel.SyncMovedNode);
            mStripManagerPanel.RequestToMoveSectionNodeDownLinear -= new Events.Node.RequestToMoveSectionNodeDownLinearHandler(mProject.MoveSectionNodeDownLinearRequested);
            mStripManagerPanel.RequestToMoveSectionNodeUpLinear -= new Events.Node.RequestToMoveSectionNodeUpLinearHandler(mProject.MoveSectionNodeUpLinearRequested);
            mProject.ShallowSwappedSectionNodes -= new Events.Node.ShallowSwappedSectionNodesHandler(mTOCPanel.SyncShallowSwapNodes);
            mProject.ShallowSwappedSectionNodes -= new Events.Node.ShallowSwappedSectionNodesHandler(mStripManagerPanel.SyncShallowSwapNodes);

            mTOCPanel.RequestToIncreaseSectionNodeLevel -= new Events.Node.RequestToIncreaseSectionNodeLevelHandler(mProject.IncreaseSectionNodeLevelRequested);
            //marisa: the former "mProject.IncreasedSectionLevel" event is now handled by MovedNode

            mTOCPanel.RequestToDecreaseSectionNodeLevel -= new Events.Node.RequestToDecreaseSectionNodeLevelHandler(mProject.DecreaseSectionNodeLevelRequested);
            mProject.DecreasedSectionNodeLevel -= new Events.Node.DecreasedSectionNodeLevelHandler(mTOCPanel.SyncDecreasedSectionNodeLevel);

            mTOCPanel.RequestToRenameSectionNode -= new Events.Node.Section.RequestToRenameSectionNodeHandler(mProject.RenameSectionNodeRequested);
            mStripManagerPanel.RenameSection -= new Events.Node.Section.RequestToRenameSectionNodeHandler(mProject.RenameSectionNodeRequested);
            mProject.RenamedSectionNode -= new Events.Node.Section.RenamedSectionNodeHandler(mTOCPanel.SyncRenamedSectionNode);
            mProject.RenamedSectionNode -= new Events.Node.Section.RenamedSectionNodeHandler(mStripManagerPanel.SyncRenamedSectionNode);



            // mProject.MediaSet -= new Events.Node.MediaSetHandler(mStripManagerPanel.SyncMediaSet);

            

            mTOCPanel.RequestToCopySectionNode -= new Events.Node.RequestToCopySectionNodeHandler(mProject.CopySectionNodeRequested);
            mProject.CopiedSectionNode -= new Events.Node.CopiedSectionNodeHandler(mTOCPanel.SyncCopiedSectionNode);
            mProject.CopiedSectionNode -= new Events.Node.CopiedSectionNodeHandler(mStripManagerPanel.SyncCopiedSectionNode);
            mProject.UndidCopySectionNode -= new Events.Node.CopiedSectionNodeHandler(mTOCPanel.SyncUndidCopySectionNode);
            mProject.UndidCopySectionNode -= new Events.Node.CopiedSectionNodeHandler(mStripManagerPanel.SyncUndidCopySectionNode);




        }

        #endregion

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
