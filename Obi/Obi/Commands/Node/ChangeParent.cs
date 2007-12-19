using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    /// <summary>
    /// Internal command to change a parent's node.
    /// </summary>
    public class ChangeParent: Command
    {
        private ObiNode mNode;            // the moving node
        private ObiNode mPreviousParent;  // its previous parent
        private ObiNode mNewParent;       // the new parent
        private int mPreviousOffset;      // offset for indexes in the original node

        public ChangeParent(ProjectView.ProjectView view, ObiNode node, ObiNode parent, int offset): base(view)
        {
            mNode = node;
            mPreviousParent = node.ParentAs<ObiNode>();
            mNewParent = parent;
            mPreviousOffset = offset;
        }

        public ChangeParent(ProjectView.ProjectView view, ObiNode node, ObiNode parent)
            : this(view, node, parent, 0) {}

        public override void execute()
        {
            System.Diagnostics.Debug.Print("Detaching node <{0}> from parent <{1}>", mNode, mPreviousParent);
            mNode.Detach();
            mNewParent.AppendChild(mNode);
            System.Diagnostics.Debug.Print("Added <{0}> to parent <{1}>", mNode, mNewParent);
        }

        public override void unExecute()
        {
            mNode.Detach();
            mPreviousParent.Insert(mNode, mPreviousOffset);
            base.unExecute();
        }
    }
}
