using System;
using System.Collections.Generic;

using urakawa.media.data;

namespace Obi.Commands.Node
{
    public class Copy: Command
    {
        private Clipboard mNewClipboard;         // the new contents of the clipboard
        private Clipboard mOldClipboard;         // the old contents of the clipboard

        public Copy(ProjectView.ProjectView view, bool deep, string label): base(view)
        {
            mOldClipboard = view.Clipboard;
            mNewClipboard = new Clipboard(view.Selection.Node, deep);
            Label = label;
        }

        public Copy(ProjectView.ProjectView view, bool deep) : this(view, deep, "") { }


        public override List<MediaData> getListOfUsedMediaData ()
            {
            if (mNewClipboard != null && mNewClipboard.Node is PhraseNode)
                {
                List<MediaData> mediaList = new List<MediaData> ();
                if (((PhraseNode)mNewClipboard.Node).Audio != null )
                mediaList.Add ( ((PhraseNode)mNewClipboard.Node).Audio.getMediaData () );
                return mediaList;
                }
            else if (mNewClipboard != null && mNewClipboard.Node is SectionNode)
                {
                return GetMediaDataListForSection ( (SectionNode)mNewClipboard.Node );
                                }
            else
                return new List<MediaData> ();
            }


        private List<MediaData> GetMediaDataListForSection ( SectionNode sNode )
            {
            List<MediaData> mediaList = new List<MediaData> ();

            sNode.acceptDepthFirst (
                    delegate ( urakawa.core.TreeNode n )
                        {
                        if (n != null && n is PhraseNode && ((PhraseNode)n).Audio != null )
                            {
                            mediaList.Add ( ((PhraseNode)n).Audio.getMediaData () );
                            }
                        return true;
                        },
                    delegate ( urakawa.core.TreeNode n ) { } );

            return mediaList;
            }


        public override void execute()
        {
            View.Clipboard = mNewClipboard;
            View.Selection = SelectionBefore;
        }

        public override void unExecute()
        {
            View.Clipboard = mOldClipboard;
            base.unExecute();
        }
    }
}
