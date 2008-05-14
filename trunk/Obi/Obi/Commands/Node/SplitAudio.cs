using urakawa.media.data.audio;
using urakawa.media.timing;

namespace Obi.Commands.Node
{
    class SplitAudio: Command
    {
        private PhraseNode mNode;          // node to split
        private PhraseNode mNewNode;       // node after split
        private Time mSplitPoint;          // split point

        public SplitAudio(ProjectView.ProjectView view) :
            this(view, view.TransportBar.SplitTime)
        {
        }

        public SplitAudio(ProjectView.ProjectView view, double splitTime): base(view)
        {
            mNode = view.GetNodeForSplit();
            mSplitPoint = new Time(splitTime);
            Label = Localizer.Message("split_phrase");
        }

        public override void execute()
        {
            mNewNode = View.Presentation.CreatePhraseNode(mNode.SplitAudio(mSplitPoint));
            mNode.InsertAfterSelf(mNewNode);
            if (UpdateSelection) View.SelectedBlockNode = mNewNode;
        }

        public override void unExecute()
        {
            MergeAudio.Merge(mNode, mNewNode);
            base.unExecute();
        }
    }
}
