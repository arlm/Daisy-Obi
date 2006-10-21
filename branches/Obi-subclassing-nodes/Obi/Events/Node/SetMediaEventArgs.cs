using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi.Events.Node
{
    public delegate void SetMediaHandler(object sender, SetMediaEventArgs e);
    public delegate void PhraseAudioSetHandler(object sender, PhraseNode node, SetMediaEventArgs e);

    /// <summary>
    /// This event is fired when a media object has been set on a node.
    /// The node is passed as another argument of the handler.
    /// This event is cancellable (use the Cancel property.)
    /// </summary>
    public class SetMediaEventArgs
    {
        private string mChannel;  // the channel on which to set
        private IMedia mMedia;    // the media object
        private bool mCancel;     // can be cancelled

        public string Channel
        {
            get { return mChannel; }
        }

        public IMedia Media
        {
            get { return mMedia; }
        }

        public bool Cancel
        {
            get { return mCancel; }
            set { mCancel = value; }
        }

        public SetMediaEventArgs(string channel, IMedia media)
        {
            mChannel = channel;
            mMedia = media;
            mCancel = false;
        }
    }
}
