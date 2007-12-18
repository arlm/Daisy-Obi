using System;
using System.Drawing;
using System.Windows.Forms;
using urakawa.core.events;
using System.Collections;

namespace Obi.ProjectView
{
  
    /// <summary>
    /// The view of the table of contents, mostly a wrapper over a tree view.
    /// </summary>
    public partial class TOCView : UserControl, IControlWithRenamableSelection
    {
        private ProjectView mView;         // the parent project view
        private NodeSelection mSelection;  // actual selection context
        //dummy nodes and their owner (the node that spawned them)
        private DummyNode mDummyBefore;
        private DummyNode mDummyChild;
        private DummyNode mDefaultDummy;
        private TreeNode mOwnerOfDummies;

        public EventHandler DummyNodeSelected;  //triggered when a dummy node has been selected

        /// <summary>
        /// Create a new TOC view as part of a project view.
        /// </summary>
        public TOCView() { InitializeComponent(); }


        /// <summary>
        /// This needs to be refined when the dummy section is implemented.
        /// </summary>
        public bool CanAddSection { get { return !(mSelection is TextSelection); } }

        /// <summary>
        /// An actual section must be selected to be copied (i.e. not the dummy section or the text of the section.)
        /// </summary>
        public bool CanCopySection { get { return IsSectionSelected; } }

        /// <summary>
        /// True if there is a selected section and it can be moved out (i.e. decrease its level)
        /// </summary>
        public bool CanMoveSectionIn
        {
            get { return IsSectionSelected && Commands.TOC.MoveSectionIn.CanMoveNode((SectionNode)mSelection.Node); }
        }

        /// <summary>
        /// True if there is a selected section and it can be moved out (i.e. decrease its level)
        /// </summary>
        public bool CanMoveSectionOut
        {
            get { return IsSectionSelected && Commands.TOC.MoveSectionOut.CanMoveNode((SectionNode)mSelection.Node); }
        }

        /// <summary>
        /// True if the selected node can be removed (deleted or cut)
        /// </summary>
        public bool CanRemoveSection { get { return IsSectionSelected; } }

        /// <summary>
        /// True if the selected node can be renamed.
        /// </summary>
        public bool CanRenameSection { get { return IsSectionSelected; } }

        /// <summary>
        /// True if the used state of the selected section can be changed
        /// </summary>
        public bool CanSetSectionUsedStatus
        {
            get { return IsSectionSelected && mSelection.Node.ParentAs<ObiNode>().Used; }
        }

        /// <summary>
        /// Make the tree node for this section visible.
        /// </summary>
        public void MakeTreeNodeVisibleForSection(SectionNode section) { FindTreeNode(section).EnsureVisible(); }

        /// <summary>
        /// Resynchronize strips and TOC views depending on which node is visible.
        /// </summary>
        public void ResyncViews()
        {
            foreach (TreeNode n in mTOCTree.Nodes) SetStripsVisibilityForNode(n, true);
        }

        /// <summary>
        /// Get or set the current selection.
        /// Only the project view can set the selection.
        /// </summary>
        public NodeSelection Selection
        {
            get { return mSelection; }
            set
            {
                if (mSelection != value)
                {
                    mSelection = value;
                    if (!(value is DummySelection))
                    {
                        TreeNode n = value == null ? null : FindTreeNode((SectionNode)value.Node);
                        // ignore the select event, since we were asked to change the selection;
                        // but allow the selection not coming from the user
                        mTOCTree.AfterSelect -= new TreeViewEventHandler(TOCTree_AfterSelect);
                        mTOCTree.BeforeSelect -= new TreeViewCancelEventHandler(TOCTree_BeforeSelect);
                        mTOCTree.SelectedNode = n;
                        if (n != null) mView.MakeStripVisibleForSection(n.Tag as SectionNode);
                        mTOCTree.AfterSelect += new TreeViewEventHandler(TOCTree_AfterSelect);
                        mTOCTree.BeforeSelect += new TreeViewCancelEventHandler(TOCTree_BeforeSelect);
                    }
                }
            }
        }

        /// <summary>
        /// Select and start renaming a section node.
        /// </summary>
        public void SelectAndRename(ObiNode node)
        {
            DoToNewNode((SectionNode)node, delegate()
            {
                mView.Selection = new TextSelection((SectionNode)node, this, ((SectionNode)node).Label);
                FindTreeNode((SectionNode)node).BeginEdit();
            });
        }

        /// <summary>
        /// Set a new presentation to be displayed.
        /// </summary>
        public void SetNewPresentation()
        {
            mTOCTree.Nodes.Clear();
            CreateTreeNodeForSectionNode(mView.Presentation.RootNode);
            mView.Presentation.treeNodeAdded += new TreeNodeAddedEventHandler(Presentation_treeNodeAdded);
            mView.Presentation.treeNodeRemoved += new TreeNodeRemovedEventHandler(Presentation_treeNodeRemoved);
            mView.Presentation.RenamedSectionNode += new NodeEventHandler<SectionNode>(Presentation_RenamedSectionNode);
            mView.Presentation.UsedStatusChanged += new NodeEventHandler<ObiNode>(Presentation_UsedStatusChanged);
        }

        /// <summary>
        /// Set the parent project view.
        /// </summary>
        public ProjectView ProjectView { set { mView = value; } }


        // Convenience method to add a new tree node for a section. Return the added tree node.
        private TreeNode AddSingleSectionNode(ObiNode node)
        {
            TreeNode n = null;
            if (node is SectionNode && node.IsRooted)
            {
                if (node.ParentAs<SectionNode>() != null)
                {
                    TreeNode p = FindTreeNode(node.ParentAs<SectionNode>());
                    n = p.Nodes.Insert(node.Index, node.GetHashCode().ToString(), ((SectionNode)node).Label);
                }
                else
                {
                    n = mTOCTree.Nodes.Insert(node.Index, node.GetHashCode().ToString(), ((SectionNode)node).Label);
                }
                n.Tag = node;
                ChangeColorUsed(n, node.Used);
            }
            return n;
        }

        // Change the color of a node to reflect its used status
        private void ChangeColorUsed(TreeNode n, bool used)
        {
            n.ForeColor = used ? Color.Black : Color.LightGray;
        }

        // Create a new tree node for a section node and all of its descendants
        private void CreateTreeNodeForSectionNode(ObiNode node)
        {
            TreeNode n = AddSingleSectionNode(node);
            if (n != null)
            {
                n.EnsureVisible();
                n.ExpandAll();
                ChangeColorUsed(n, node.Used);
            }
            if (n != null || node is RootNode)
            {
                for (int i = 0; i < node.SectionChildCount; ++i) CreateTreeNodeForSectionNode(node.SectionChild(i));
            }
        }

        private delegate void DoToNewNodeDelegate();

        // Do f() to a section node that may not yet be in the tree; if it's not, set an event to look out for
        // its addition. This is used for naming new sections for instance.
        private void DoToNewNode(SectionNode section, DoToNewNodeDelegate f)
        {
            if (IsInTree(section))
            {
                f();
            }
            else
            {
                TreeNodeAddedEventHandler h = delegate(object o, TreeNodeAddedEventArgs e) { };
                h = delegate(object o, TreeNodeAddedEventArgs e)
                {
                    if (e.getTreeNode() == section)
                    {
                        f();
                        mView.Presentation.treeNodeAdded -= h;
                    }
                };
                mView.Presentation.treeNodeAdded += h;
            }
        }

        /// <summary>
        /// Find the tree node for a section node. The labels must also match.
        /// </summary>
        private TreeNode FindTreeNode(SectionNode section)
        {
            TreeNode n = FindTreeNodeWithoutLabel(section);
            if (n == null) throw new TreeNodeNotFoundException(String.Format(
                "Could not find tree node for section with label \"{0}\"", section.Label));
            if (n.Text != section.Label)
            {
                throw new TreeNodeFoundWithWrongLabelException(
                    String.Format("Found tree node matching section node #{0} but labels mismatch (wanted \"{1}\" but got \"{2}\").",
                    section.GetHashCode(), section.Label, n.Text));
            }
            return n;
        }

        /// <summary>
        /// Find a tree node for a section node, regardless of its label.
        /// The node that we are looking for must be present, but its label
        /// may not be the same.
        /// </summary>
        private TreeNode FindTreeNodeWithoutLabel(SectionNode section)
        {
            TreeNode[] nodes = mTOCTree.Nodes.Find(section.GetHashCode().ToString(), true);
            foreach (TreeNode n in nodes) if (n.Tag == section) return n;
            return null;
        }

        // Check whether a node is in the tree view
        private bool IsInTree(SectionNode section)
        {
            if (section != null)
            {
                TreeNode[] nodes = mTOCTree.Nodes.Find(section.GetHashCode().ToString(), true);
                foreach (TreeNode n in nodes) if (n.Tag == section && n.Text == section.Label) return true;
            }
            return false;
        }

        // True if a section (not dummy, or not its text) is selected.
        private bool IsSectionSelected
        {
            get { return mSelection != null && mSelection.GetType() == typeof(NodeSelection); }
        }

        // True if there is a selection and it is not the dummy node.
        private bool IsSelectionNotDummy
        {
            get { return mSelection != null && !(mSelection is DummySelection); }
        }

        // When a node was renamed, show the new name in the tree.
        private void Presentation_RenamedSectionNode(object sender, NodeEventArgs<SectionNode> e)
        {
            TreeNode n = FindTreeNodeWithoutLabel(e.Node);
            n.Text = e.Node.Label;
        }

        // Add new section nodes to the tree
        private void Presentation_treeNodeAdded(object o, TreeNodeAddedEventArgs e)
        {
            if (e.getTreeNode() is SectionNode)
            {
                // ignore the selection of the new tree node
                mTOCTree.AfterSelect -= new TreeViewEventHandler(TOCTree_AfterSelect);
                CreateTreeNodeForSectionNode(e.getTreeNode() as SectionNode);
                mTOCTree.AfterSelect += new TreeViewEventHandler(TOCTree_AfterSelect);
            }
        }

        // Node used status changed
        private void Presentation_UsedStatusChanged(object sender, NodeEventArgs<ObiNode> e)
        {
            if (e.Node is SectionNode && IsInTree((SectionNode)e.Node)) ChangeColorUsed(FindTreeNode((SectionNode)e.Node), e.Node.Used);
        }

        // Remove deleted section nodes from the tree
        void Presentation_treeNodeRemoved(object o, TreeNodeRemovedEventArgs e)
        {
            SectionNode section = e.getTreeNode() as SectionNode;
            if (section != null && IsInTree(section)) mTOCTree.Nodes.Remove(FindTreeNode(section));
        }

        // Set the strips visibility for the given tree node according to expandednessity
        private void SetStripsVisibilityForNode(TreeNode node, bool visible)
        {
            mView.SetStripVisibilityForSection((SectionNode)node.Tag, visible);
            foreach (TreeNode n in node.Nodes) SetStripsVisibilityForNode(n, visible && node.IsExpanded);
        }

        // When a node is collapsed, hide strips corresponding to the collapsed nodes.
        private void TOCTree_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            SetStripsVisibilityForNode(e.Node, true);
        }

        // When a node is expanded, make the strips reappear
        private void TOCTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            SetStripsVisibilityForNode(e.Node, true);
        }

        // Rename the section after the text of the tree node has changed.
        // Cancel if the text is empty.
        private void TOCTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Tag != null && e.Label != null && e.Label != "")
            {
                mView.RenameSectionNode((SectionNode)e.Node.Tag, e.Label);
            }
            else
            {
                e.CancelEdit = true;
                mView.Selection = new NodeSelection((SectionNode)e.Node.Tag, this);
            }
        }

        // Pass a new selection to the main view.
        // Do not act on reselection of the same item to avoid infinite loops.
        private void TOCTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // check for dummy here
            if (e.Node is DummyNode)
            {
                DummySelection ds = new DummySelection((SectionNode)mOwnerOfDummies.Tag, this);
                ds.Dummy = (DummyNode)e.Node;
                if (ds != mView.Selection) mView.Selection = ds;
            }
            else
            {
                NodeSelection s = new NodeSelection((SectionNode)e.Node.Tag, this);
                if (s != mView.Selection) mView.Selection = s;
            }
        }

        // Make a text selection in the view.
        private void TOCTree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!(e.Node.Tag is SectionNode))
            {
                e.CancelEdit = true;
            }
            else if (mSelection != null)
            {
                mView.Selection = new TextSelection((SectionNode)e.Node.Tag, this, e.Node.Text);
            }
        }

        // Filter out unwanted tree selections (not caused by the user clicking, expanding, collapsing or keyboarding.)
        private void TOCTree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown) e.Cancel = true;
            else if (!(e.Node is DummyNode)) RemoveDummyNodes();
        }

        /// <summary>
        /// Remove our two dummy nodes
        /// </summary>
        public void RemoveDummyNodes()
        {
            if (mDummyBefore != null && mDummyBefore.TreeView != null) mDummyBefore.Remove();
            if (mDummyChild != null && mDummyChild.TreeView != null) mDummyChild.Remove();
            if (mDefaultDummy != null && mDefaultDummy.TreeView != null) mDefaultDummy.Remove();
        }
      
        /// <summary>
        /// See if there is ambiguity around the given node, and insert dummies.  The rules for inserting dummy nodes are:
        /// 1. If the node is the first of a group of siblings, it gets a dummy node as an older sibling (DUMMY_BEFORE)
        /// 2. If the node has no children, it gets a dummy child (DUMMY_CHILD)
        /// </summary>
        /// <returns>true if dummy nodes had to be added; false otherwise</returns>
        public bool AddDummyNodes()
        {
            TreeNode treeNode = this.mTOCTree.SelectedNode;
            if (treeNode == null || treeNode is DummyNode) return false;

            bool ret = false;
            //see if the node is the first of a group of siblings
            if ((treeNode.Parent != null && treeNode.Parent.FirstNode == treeNode) ||
                (treeNode.Parent == null && treeNode.PrevNode == null))
            {
                mDummyBefore = new DummyNode(DummyNode.DummyType.BEFORE);
                mDummyBefore.Text = "Dummy before";
                if (treeNode.Parent != null) treeNode.Parent.Nodes.Insert(0, mDummyBefore);
                else mTOCTree.Nodes.Insert(0, mDummyBefore);
                ret = true;
            }
            if (treeNode.Nodes.Count == 0)
            {
                mDummyChild = new DummyNode(DummyNode.DummyType.CHILD);
                mDummyChild.Text = "Dummy child";
                treeNode.Nodes.Insert(0, mDummyChild);
                ret = true;
            }
            //if we added other dummies, we should also add the default dummy so that the user
            //understands all their options
            if (ret)
            {
                mDefaultDummy = new DummyNode(DummyNode.DummyType.DEFAULT);
                mDefaultDummy.Text = "Default dummy";
                int defaultIndex;
                defaultIndex = treeNode.Index + 1;
                if (treeNode.Parent != null) treeNode.Parent.Nodes.Insert(defaultIndex, mDefaultDummy);
                else mTOCTree.Nodes.Insert(defaultIndex, mDefaultDummy);
            }
            if (ret) mOwnerOfDummies = treeNode;
            else mOwnerOfDummies = null;
            treeNode.Expand();
            return ret;
        }

        private void mTOCTree_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (mView.Selection is DummySelection && mView.CanPaste) PasteAndRemoveDummyNodes();
            }
        }
        private void mTOCTree_DoubleClick(object sender, EventArgs e)
        {
            if (mView.Selection is DummySelection && mView.CanPaste) PasteAndRemoveDummyNodes();
        }
        private void PasteAndRemoveDummyNodes()
        {
            mView.Paste();
            RemoveDummyNodes();
        }
    }

    /// <summary>
    /// Raised when a tree node could not be found.
    /// </summary>
    public class TreeNodeNotFoundException : Exception
    {
        public TreeNodeNotFoundException(string msg) : base(msg) { }
    }

    /// <summary>
    /// Raised when a tree node is found but with the wrong label.
    /// </summary>
    public class TreeNodeFoundWithWrongLabelException: Exception
    {
        public TreeNodeFoundWithWrongLabelException(string msg) : base(msg) { }
    }

    /// <summary>
    /// This class represents a dummy TOC node.  It might turn out to be overkill, I'm not sure yet.
    /// </summary>
    public class DummyNode : TreeNode
    {
        private DummyType mDummyType;
        public enum DummyType { BEFORE, CHILD, DEFAULT };

        public DummyNode(DummyType type)
            : base()
        {
            mDummyType = type;
        }

        public DummyType Type
        {
            get { return mDummyType; }
            set { mDummyType = value; }
        }

    }
}