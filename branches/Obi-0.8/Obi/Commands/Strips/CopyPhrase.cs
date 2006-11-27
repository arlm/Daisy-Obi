using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    class CopyPhrase: Command
    {
        private PhraseNode mNode;      // current node
        private PhraseNode mPrevNode;  // previous node in the clipboard

        public override string Label
        {
            get { return Localizer.Message("copy_phrase_command_label"); }
        }

        public CopyPhrase(PhraseNode node)
        {
            mNode = node;
            mPrevNode = mNode.Project.PhraseClipBoard;
        }

        public override void Do()
        {
            mNode.Project.PhraseClipBoard = mNode;
        }

        public override void Undo()
        {
            mNode.Project.PhraseClipBoard = mPrevNode;
        }
    }
}
