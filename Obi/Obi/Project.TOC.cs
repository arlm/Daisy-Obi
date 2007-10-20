using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi
{
    public partial class Project
    {
        public event Events.RenameSectionNodeHandler RenamedSectionNode;   // a node was renamed in the presentation
        public event Events.MovedSectionNodeHandler MovedSectionNode;      // a node was moved in the presentation
        public event Events.SectionNodeHandler DecreasedSectionNodeLevel;  // a node's level was decreased in the presentation
        public event Events.MovedSectionNodeHandler UndidMoveSectionNode;  // a node was restored to its previous location
        public event Events.SectionNodeHandler PastedSectionNode;          // a section node was pasted
        public event Events.SectionNodeHandler UndidPasteSectionNode;      // a section node was unpasted
        public event Events.SectionNodeHandler ToggledSectionUsedState;    // a section node used state was changed

        /// <summary>
        /// Cut the whole subtree for a section node.
        /// Actually the section node is just cut off the tree,
        /// but all phrases must be removed from the manager as
        /// well.
        /// </summary>
        /// <param name="node">The node to cut.</param>
        public void CutSectionNode(SectionNode node)
        {
            if (node != null)
            {
                SectionNode copy = node.copy(true);
                //Commands.TOC.CutSectionNode command = new Commands.TOC.CutSectionNode(copy);
                //command.AddCommand(RemoveSectionNode(node));
                mClipboard.Section = copy;
                Modified();
                //CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// Copy a section node as a whole (i.e. the entire subtree.)
        /// The project is unmodified, only the clipboard gets updated.
        /// </summary>
        /// <param name="node">The node to copy.</param>
        public void CopySectionNode(SectionNode node)
        {
            if (node != null)
            {
                SectionNode copy = node.copy(true);
                //CommandCreated(this, new Events.Project.CommandCreatedEventArgs(new Commands.TOC.CopySectionNode(copy)));
                mClipboard.Section = copy;
            }
        }

        /// <summary>
        /// Paste the clipboard node under the parent (or the root if no parent is given.)
        /// </summary>
        /// <param name="parent">Parent to paste under (root if null.)</param>
        public SectionNode PasteSectionNode(TreeNode context)
        {
            if (mClipboard.Section != null)
            {
                if (context == null) context = RootNode;
                SectionNode pasted = PasteCopyOfSectionNode(mClipboard.Section, context);
                //CommandCreated(this,
                //    new Events.Project.CommandCreatedEventArgs(new Commands.TOC.PasteSectionNode(pasted, context)));
                return pasted;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Delete a node and its whole subtree from the core tree.
        /// </summary>
        public void DeleteSectionNode(SectionNode node)
        {
            if (node != null)
            {
                //Commands.TOC.DeleteSectionNode command = RemoveSectionNode(node);
                Modified();
                //CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }

        public void RenameSectionNodeWithCommand(SectionNode node, string label)
        {
            Commands.TOC.Rename command = new Commands.TOC.Rename(node, label);
            RenameSectionNode(node, label);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        public void RenameSectionNode(SectionNode node, string label)
        {
            node.Label = label;
            RenamedSectionNode(this, new Obi.Events.Node.RenameSectionNodeEventArgs(this, node, label));
        }





        /// <summary>
        /// Readd a section node that was previously delete and restore all its contents.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        public void UndeleteSectionNode(SectionNode node, TreeNode parent, int index)
        {
            Visitors.UndeleteSubtree visitor = new Visitors.UndeleteSubtree(this, parent, index);
            node.acceptDepthFirst(visitor);
        }

        /// <summary>
        /// Increase the level of a node.
        /// </summary>
        /// <param name="node">The node to increase the level of.</param>
        /// <returns>Return an increase command (for use in shallow section commands.)</returns>
        /*public Commands.TOC.IncreaseSectionNodeLevel IncreaseSectionNodeLevel(SectionNode node)
        {
            Commands.TOC.IncreaseSectionNodeLevel command = new Commands.TOC.IncreaseSectionNodeLevel(node, (TreeNode)node.getParent());
            SectionNode newParent = node.ParentSection == null ?
                (SectionNode)((TreeNode)node.getParent()).getChild(node.Index - 1) :
                node.ParentSection.SectionChild(node.Index - 1);
            node.DetachFromParent__REMOVE__();
            AppendChildSection(node, newParent);
            MovedSectionNode(this, new Events.Node.MovedSectionNodeEventArgs(this, node, newParent));
            Modified();
            return command;
        }*/

        /// <summary>
        /// Move a section node out (decrease its level.)
        /// </summary>
        /// <param name="node">The node to move out.</param>
        public void MoveSectionNodeOut(SectionNode node)
        {
            if (CanMoveSectionNodeOut(node))
            {
                // Commands.TOC.DecreaseSectionNodeLevel command = DecreaseSectionNodeLevel(node);
                // CommandCreated(this, new Obi.Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// Decrease the level of a section node.
        /// </summary>
        /// <param name="node">The node getting it.</param>
        /*public Commands.TOC.DecreaseSectionNodeLevel DecreaseSectionNodeLevel(SectionNode node)
        {
            SectionNode parent = node.ParentSection;
            Commands.TOC.DecreaseSectionNodeLevel command = new Commands.TOC.DecreaseSectionNodeLevel(node, parent);
            // Make a list of the following siblings which will become children of the node
            List<SectionNode> newChildren = new List<SectionNode>();            
            for (int i = parent.SectionChildCount - 1; i > node.Index; --i)
            {
                newChildren.Insert(0, parent.SectionChild(i).DetachFromParent__REMOVE__());
            }
            node.DetachFromParent__REMOVE__();
            TreeNode newParent = (TreeNode)parent.getParent();
            AddChildSection(node, newParent, parent.Index + 1);
            newChildren.ForEach(delegate(SectionNode n) { AppendChildSection(n, node); });
            DecreasedSectionNodeLevel(this, new Events.Node.SectionNodeEventArgs(this, node));
            Modified();
            return command;
        }*/

        /// <summary>
        /// Increase the level of a section node.
        /// </summary>
        public void MoveSectionNodeIn(SectionNode node)
        {
            if (CanMoveSectionNodeIn(node))
            {
                // Commands.TOC.IncreaseSectionNodeLevel command = IncreaseSectionNodeLevel(node);
                // CommandCreated(this, new Obi.Events.Project.CommandCreatedEventArgs(command));
            }
        }

        //this is almost the same as ShallowDeleteSectionNode
        public void DoShallowCutSectionNode(object origin, SectionNode node)
        {
            //we have to gather this data here, because it might be different at the end
            //however, we can't create the command here, because its data isn't ready yet
            mClipboard.Section = node.copy(false);

            //Avn:  copy phrases falling imidiately under this section
            for (int i = 0; i < node.PhraseChildCount; ++i)
                mClipboard.Section.appendChild(node.PhraseChild(i).copy(true));

            //Commands.TOC.ShallowCutSectionNode command = origin == this ?
            //    null : new Commands.TOC.ShallowCutSectionNode(node);
            int numChildren = node.SectionChildCount;
            for (int i = numChildren - 1; i >= 0; i--)
            {
                //Commands.Command__OLD__ cmdDecrease = DecreaseSectionNodeLevel(node.SectionChild(i));
                //if (command != null) command.AddCommand(cmdDecrease);
            }
            numChildren = node.PhraseChildCount;
            for (int i = numChildren - 1; i >= 0; i--)
            {
                Commands.Command__OLD__ cmdDeletePhrase = DeletePhraseNodeAndMedia(node.PhraseChild(i));
                //if (command != null) command.AddCommand(cmdDeletePhrase);
            }
            //Commands.Command__OLD__ cmdRemove = RemoveSectionNode(node);
            //if (command != null) command.AddCommand(cmdRemove);
            Modified();
            //if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        //md 20061220
        public void UndoShallowCutSectionNode()
        {
            mClipboard.Section = null;
            //the rest of the undo operations for this command are found in 
            //Commands.TOC.ShallowCutSectionNode
        }

        //md 20061220
        internal void UndoShallowCopySectionNode(SectionNode node)
        {
            mClipboard.Section = null;

            //no event is raised (i.e. UndidShallowCopySectionNode) because no one cares

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
     
        }

        //md 20060810
        public void UndoPasteSectionNode(SectionNode node)
        {
            node.DetachFromParent__REMOVE__();
           
            UndidPasteSectionNode(this, new Events.Node.SectionNodeEventArgs(this, node));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));

        }

        //md 20060812
        //shallow delete a section node
        //see Commands.TOC.ShallowDeleteSectionNode if you're wondering how the "undo" works
        public void ShallowDeleteSectionNode(object origin, SectionNode node)
        {
            //we have to gather this data here, because it might be different at the end
            //however, we can't create the command here, because its data isn't ready yet
            //these lines only need to be executed if origin != this
       
            //Commands.TOC.ShallowDeleteSectionNode command = null;
        
            if (origin != this)
            {  
                //command = new Commands.TOC.ShallowDeleteSectionNode(node);
            }
               
            int numChildren = node.SectionChildCount;
            for (int i = numChildren - 1; i>=0; i--)
            {
                //Commands.Command__OLD__ cmdDecrease = DecreaseSectionNodeLevel(node.SectionChild(i));
                //command.AddCommand(cmdDecrease);
            }

            numChildren = node.PhraseChildCount;
            for (int i = numChildren - 1; i>=0; i--)
            {
                Commands.Command__OLD__ cmdDeletePhrase = DeletePhraseNodeAndMedia(node.PhraseChild(i));
                //command.AddCommand(cmdDeletePhrase);
            }

            //Commands.Command__OLD__ cmdRemove = this.RemoveSectionNode(node);
            //command.AddCommand(cmdRemove);

          
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            //if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
             
        }

        /// <summary>
        /// True if the node can be moved in.
        /// </summary>
        public bool CanMoveSectionNodeIn(SectionNode node)
        {
            return node != null && node.Index > 0;
        }

        /// <summary>
        /// True if the node can be moved out.
        /// </summary>
        public bool CanMoveSectionNodeOut(SectionNode node)
        {
            return node != null && node.ParentSection != null;
        }

        /// <summary>
        /// Paste a copy of a section node under a parent. Both must be defined.
        /// Project is modified but no command is issued.
        /// </summary>
        /// <param name="node">The node to paste.</param>
        /// <param name="parent">The parent under which to paste (append.)</param>
        /// <returns>The node actually pasted.</returns>
        public SectionNode PasteCopyOfSectionNode(SectionNode node, TreeNode context)
        {
            SectionNode copy = node.copy(true);
            if (context is SectionNode)
            {
                ((TreeNode)context.getParent()).insertBefore(copy, context);
//                ((SectionNode)context).AppendChildSection(copy);
            }
            else //this command is disabled when a PhraseNode is selected, so if no SectionNode is in focus, we must be pasting under the root
            {
                context.appendChild(copy);
            }
            PastedSectionNode(this, new Events.Node.SectionNodeEventArgs(this, copy));
            Modified();
            return copy;
        }

        /// <summary>
        /// Remove a section node and all of its phrases and subsections from the project.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        /// <returns>The corresponding delete command.</returns>
        /*public Commands.TOC.DeleteSectionNode RemoveSectionNode(SectionNode node)
        {
            Commands.TOC.DeleteSectionNode command = new Commands.TOC.DeleteSectionNode(node);
            for (int i = node.PhraseChildCount - 1; i >= 0; --i)
            {
                command.AddCommand(DeletePhraseNodeAndMedia(node.PhraseChild(i)));
            }
            for (int i = node.SectionChildCount - 1; i >= 0; --i)
            {
                command.AddCommand(RemoveSectionNode(node.SectionChild(i)));
            }
            node.DetachFromParent__REMOVE__();
            return command;
        }*/

        /// <summary>
        /// Shallow cut of a section node, i.e. only the strip.
        /// Sub-sections are moved back one level.
        /// </summary>
        /// <param name="node">The node to cut.</param>
        /// <param name="issueCommand">Issue a command if true.</param>
        public void ShallowCutSectionNode(SectionNode node)
        {
            if (node != null)
            {
                //Commands.TOC.ShallowCutSectionNode command = new Commands.TOC.ShallowCutSectionNode(node);
                mClipboard.Section = node.copy(false);

                //Avn:  copy phrases falling imidiately under this section
                for (int i = 0; i < node.PhraseChildCount; ++i)
                    mClipboard.Section.appendChild(node.PhraseChild(i).copy(true)) ;
                
                for (int i = node.SectionChildCount - 1; i >= 0; --i)
                {
                    //command.AddCommand(DecreaseSectionNodeLevel(node.SectionChild(i)));
                }
                for (int i = node.PhraseChildCount - 1; i >= 0; --i)
                {
                    //command.AddCommand(DeletePhraseNodeAndMedia(node.PhraseChild(i)));
                }
                //command.AddCommand(RemoveSectionNode(node));
                Modified();
                //CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// Copy a section node and its phrases, but not its subsections (i.e. copy one strip.)
        /// The project is unmodified, only the clipboard gets updated.
        /// </summary>
        /// <param name="node">The node to copy.</param>
        /// <param name="issueCommand">Issue a command if true.</param>
        public void ShallowCopySectionNode(SectionNode node, bool issueCommand)
        {
            if (node != null)
            {
                object data = mClipboard.Data;
                SectionNode copy = node.copy(false);
                for (int i = 0; i < node.PhraseChildCount; ++i)
                {
                    copy.appendChild(node.PhraseChild(i).copy(true));
                }
                mClipboard.Section = copy;
                if (issueCommand)
                {
                    // CommandCreated(this,
                    //    new Events.Project.CommandCreatedEventArgs(new Commands.TOC.CopySectionNode(data, copy)));
                }
            }
        }

        /// <summary>
        /// Toggle the use state of a section node.
        /// </summary>
        internal void ToggleSectionUsedState(Events.Node.SectionNodeEventArgs e)
        {
            e.Node.Used = !e.Node.Used;
            ToggledSectionUsedState(this, e);
        }
    }
}
