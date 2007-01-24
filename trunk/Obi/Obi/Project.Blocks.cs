using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using urakawa.core;
using urakawa.media;
using Obi.Assets;

namespace Obi
{
    public partial class Project
    {
        public Events.UpdateTimeHandler UpdateTime;
        public Events.PhraseNodeHandler RemovedPageNumber;
        public Events.PhraseNodeHandler SetPageNumber;

        #region clip board (cut/copy/paste/delete)

        /// <summary>
        /// Cut a phrase node: delete it and store it in the clipboard (store the original node, not a copy.)
        /// Issue a command and modify the project.
        /// </summary>
        /// <param name="node">The phrase node to cut.</param>
        public void CutPhraseNode(PhraseNode node)
        {
            if (node != null)
            {
                Commands.Strips.CutPhrase command = new Commands.Strips.CutPhrase(node);
                mClipboard.Phrase = node;
                RemovePhraseNodeAndAsset(node);
                Modified();
                CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// Copy a phrase node by storing a copy in the clipboard.
        /// Issue a command but do not mark the project as modified.
        /// </summary>
        /// <param name="node">The node to copy.</param>
        public void CopyPhraseNode(PhraseNode node)
        {
            if (node != null)
            {
                PhraseNode copy = node.copy(true);
                Commands.Strips.CopyPhrase command = new Commands.Strips.CopyPhrase(copy);
                mClipboard.Phrase = copy;
                CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// Delete a phrase node from the tree and remove its asset from the asset manager.
        /// </summary>
        public void DeletePhraseNode(PhraseNode node)
        {
            if (node != null)
            {
                Commands.Strips.DeletePhrase command = RemovePhraseNodeAndAsset(node);
                Modified();
                CommandCreated(this, new Obi.Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// Paste *before*.
        /// </summary>
        /// <param name="node">The phrase node to paste.</param>
        /// <param name="contextNode">If the context is a section, paste at the end of the section.
        /// If it is a phrase, paste *before* this phrase.</param>
        public void PastePhraseNode(PhraseNode node, ObiNode contextNode)
        {
            if (node != null && contextNode != null)
            {
                SectionNode parent;
                int index;
                if (contextNode is SectionNode)
                {
                    parent = (SectionNode)contextNode;
                    index = parent.PhraseChildCount;
                }
                else if (contextNode is PhraseNode)
                {
                    parent = ((PhraseNode)contextNode).ParentSection;
                    index = ((PhraseNode)contextNode).Index;
                }
                else
                {
                    throw new Exception(String.Format("Cannot paste with context node as {0}.", contextNode.GetType()));
                }
                PhraseNode copy = node.copy(true);
                AddPhraseNode(copy, parent, index);
                Modified();
                Commands.Strips.PastePhrase command = new Commands.Strips.PastePhrase(copy);
                CommandCreated(this, new Obi.Events.Project.CommandCreatedEventArgs(command));
            }
        }

        #endregion

        #region event handlers

        /// <summary>
        /// Import an asset, create a node for it and add it at the given position in its section.
        /// The phrase is named after the imported file name.
        /// </summary>
        public void ImportAssetRequested(object sender, Events.Strip.ImportAssetEventArgs e)
        {
            AudioMediaAsset asset = mAssManager.ImportAudioMediaAsset(e.AssetPath);
            mAssManager.InsureRename(asset, Path.GetFileNameWithoutExtension(e.AssetPath));
            
            //create a phrase node and assign it an asset
            PhraseNode node = getPresentation().getCoreNodeFactory().createNode(PhraseNode.Name, ObiPropertyFactory.ObiNS)
                 as PhraseNode;
            node.Asset = asset;

            AddPhraseNode(node, e.SectionNode, e.Index);
            Commands.Strips.AddPhrase command = new Commands.Strips.AddPhrase(node);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Merge two adjacent phrase nodes: merge their assets into the first node's asset and remove the second node.
        /// </summary>
        public void MergeNodesRequested(object sender, Events.Node.MergeNodesEventArgs e)
        {
            Assets.AudioMediaAsset asset = e.Node.Asset;
            Assets.AudioMediaAsset next = e.Next.Asset;
            // the command is created while the assets are not changed; there is time to copy the original asset before the
            // merge is done.
            Commands.Strips.MergePhrases command = new Commands.Strips.MergePhrases(e.Node, e.Next);
            mAssManager.MergeAudioMediaAssets(asset, next);
            UpdateSeq(e.Node);
            MediaSet(this, new Events.Node.SetMediaEventArgs(e.Origin, e.Node, Project.AudioChannelName,
                GetMediaForChannel(e.Node, Project.AudioChannelName)));
            DeletedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(e.Origin, e.Next));
            e.Next.DetachFromParent();
            TouchedNode(this, new Events.Node.NodeEventArgs(e.Origin, e.Node));
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        public enum Direction { Forward, Backward };

        /// <summary>
        /// Move a phrase node in either direction. May not succeed if there is nowhere to move in that direction.
        /// </summary>
        private void MovePhraseNodeRequested(object sender, Events.Node.PhraseNodeEventArgs e, PhraseNode.Direction dir)
        {
            if (CanMovePhraseNode(e.Node, dir))
            {
                MovePhraseNode(e.Node, dir);
                Commands.Strips.MovePhrase command = new Commands.Strips.MovePhrase(e.Node, dir);
                CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }
        /// <summary>
        /// Move a phrase node forward. May not succeed if the node is last of its kind.
        /// </summary>
        public void MovePhraseNodeForwardRequested(object sender, Events.Node.PhraseNodeEventArgs e)
        {
            MovePhraseNodeRequested(sender, e, PhraseNode.Direction.Forward);
        }

        /// <summary>
        /// Move a phrase node forward.
        /// </summary>
        public void MovePhraseNodeBackwardRequested(object sender, Events.Node.PhraseNodeEventArgs e)
        {
            MovePhraseNodeRequested(sender, e, PhraseNode.Direction.Backward);
        }

        /// <summary>
        /// Try to set the media on a given channel of a node.
        /// Cancel the event the change could not be made (e.g. renaming a block.)
        /// </summary>
        public void SetMediaRequested(object sender, Events.Node.SetMediaEventArgs e)
        {
            if (!DidSetMedia(sender, e.Node, e.Channel, e.Media)) e.Cancel = true;
        }

        /// <summary>
        /// Find the channel and set the media object.
        /// As this may fail, return true if the change was really made or false otherwise.
        /// Throw an exception if the channel could not be found.
        /// </summary>
        internal bool DidSetMedia(object origin, PhraseNode node, string channel, IMedia media)
        {
            ChannelsProperty channelsProp = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            IList channelsList = channelsProp.getListOfUsedChannels();
            for (int i = 0; i < channelsList.Count; i++)
            {
                IChannel ch = (IChannel)channelsList[i];
                if (ch.getName() == channel)
                {
                    Commands.Command command = null;
                   /* if (GetNodeType(node) == NodeType.Phrase && channel == Project.AnnotationChannelName)
                    {
                        // we are renaming a phrase node
                        //md no longer allowed to rename assets
                      /*  Assets.AudioMediaAsset asset = GetAudioMediaAsset(node);
                        string old = mAssManager.RenameAsset(asset, ((TextMedia)media).getText());
                        if (old == asset.Name) return false;
                        command = new Commands.Strips.RenamePhrase(this, node);*/
                  //  }

                    channelsProp.setMedia(ch, media);
                    MediaSet(this, new Events.Node.SetMediaEventArgs(origin, node, channel, media));
                    if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
                    mUnsaved = true;
                    StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
                    // make a command here
                    return true;
                }
            }
            // the channel was not found when it should have been...
            throw new Exception(String.Format(Localizer.Message("channel_not_found"), channel));
        }

        /// <summary>
        /// Handle a request to split an audio block. The event contains the original node that was split and the new asset
        /// created from the split. A new sibling to the original node is to be added.
        /// </summary>
        public void SplitAudioBlockRequested(object sender, Events.Node.SplitPhraseNodeEventArgs e)
        {
            PhraseNode newNode = CreatePhraseNode(e.NewAsset);
            SectionNode parent = (SectionNode)e.Node.getParent();
            int index = parent.indexOf(e.Node) + 1;
            parent.insert(newNode, index);
            UpdateSeq(e.Node);
            MediaSet(this, new Events.Node.SetMediaEventArgs(e.Origin, e.Node, Project.AudioChannelName,
                GetMediaForChannel(e.Node, Project.AudioChannelName)));
            AddedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(e.Origin, newNode));
            Commands.Strips.SplitPhrase command = new Commands.Strips.SplitPhrase(e.Node, newNode);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Handle a request to apply phrase detection to a phrase.
        /// </summary>
        public void ApplyPhraseDetection(object sender, Events.Node.PhraseDetectionEventArgs e)
        {
            AudioMediaAsset originalPhrase = e.Node.Asset;
            List<AudioMediaAsset> phrases = originalPhrase.ApplyPhraseDetection(e.Threshold, e.Gap, e.LeadingSilence);
            if (phrases.Count > 1)
            {
                List<PhraseNode> nodes = new List<PhraseNode>(phrases.Count);
                foreach (AudioMediaAsset phrase in phrases) nodes.Add(CreatePhraseNode(phrase));
                ReplaceNodeWithNodes(e.Node, nodes);
                Commands.Strips.ApplyPhraseDetection command = new Commands.Strips.ApplyPhraseDetection(this, e.Node, nodes);
                CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// Replace a node with a list of nodes.
        /// </summary>
        /// <param name="mNode">The node to remove.</param>
        /// <param name="mPhraseNodes">The nodes to add instead.</param>
        internal void ReplaceNodeWithNodes(PhraseNode node, List<PhraseNode> nodes)
        {
            if (node.getParent().GetType() == Type.GetType("Obi.SectionNode"))
            {
                SectionNode parent = (SectionNode)node.getParent();
                int index = parent.indexOf(node);
                DeletedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, node));
                node.DetachFromParent();

                foreach (PhraseNode n in nodes)
                {
                    parent.AddChildPhrase(n, index);
                    AddedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, n));
                }
                Modified();
            }
            else
            { //TODO: will this ever come up?
            }
        }

        /// <summary>
        /// Replace a list of contiguous nodes with a single one.
        /// </summary>
        /// <param name="mPhraseNodes">The nodes to remove.</param>
        /// <param name="mNode">The node to add instead.</param>
        internal void ReplaceNodesWithNode(List<PhraseNode> nodes, PhraseNode node)
        {
            if (nodes[0].getParent().GetType() == Type.GetType("Obi.SectionNode"))
            {
                SectionNode parent = (SectionNode)nodes[0].getParent();
                int index = nodes[0].Index;

                foreach (PhraseNode n in nodes)
                {
                    DeletedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, n));
                    n.DetachFromParent();
                }
                parent.AddChildPhrase(node, index);
                AddedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, node));
                Modified();
            }
            else
            {//TODO: when will this case arise?
            }
        }

        /// <summary>
        /// A new phrase being recorded.
        /// </summary>
        /// <param name="e">The phrase event originally sent by the recording session.</param>
        /// <param name="parent">Parent core node for the new phrase.</param>
        /// <param name="index">Base index in the parent for new phrases.</param>
        internal void StartRecordingPhrase(Events.Audio.Recorder.PhraseEventArgs e, SectionNode parent, int index)
        {
            PhraseNode phrase = CreatePhraseNode(e.Asset);
            parent.AddChildPhrase(phrase, index);
            UpdateSeq(phrase);
            AddedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, phrase));
            Commands.Strips.AddPhrase command = new Commands.Strips.AddPhrase(phrase);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Update the time information of a phrase being recorded.
        /// </summary>
        /// <param name="e">The phrase event originally sent by the recording session.</param>
        /// <param name="parent">Parent core node for the new phrase.</param>
        /// <param name="index">Base index in the parent for new phrases.</param>
        internal void ContinuingRecordingPhrase(Events.Audio.Recorder.PhraseEventArgs e, SectionNode parent, int index)
        {
            PhraseNode phrase = parent.PhraseChild(index);
            UpdateTime(this, new Events.Strip.UpdateTimeEventArgs(this, phrase, e.Time));            
        }

        /// <summary>
        /// When a phrase has finished recording, update its media object.
        /// </summary>
        /// <param name="e">The phrase event originally sent by the recording session.</param>
        /// <param name="parent">Parent core node for the new phrase.</param>
        /// <param name="index">Base index in the parent for new phrases.</param>
        internal void FinishRecordingPhrase(Events.Audio.Recorder.PhraseEventArgs e, SectionNode parent, int index)
        {
            PhraseNode phrase = parent.PhraseChild(index + e.PhraseIndex);
            UpdateSeq(phrase);
            MediaSet(this, new Events.Node.SetMediaEventArgs(this, phrase, Project.AudioChannelName,
                GetMediaForChannel(phrase, Project.AudioChannelName)));
        }

        #endregion

        #region backend functions





        /// <summary>
        /// This function is called when undeleting a subtree
        /// the phrase nodes already exist under the section node, so they can't be re-added
        /// they just need to be rebuilt in the views
        /// </summary>
        /// <param name="node"></param>
        /// <param name="index"></param>
        //md 20060813
        public void ReconstructPhraseNodeInView(PhraseNode node)
        {
            //we might consider using a different event for this
            //i don't know who else will be listening in the future (more than only viewports?)
            AddedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, node));
        }

        /// <summary>
        /// Determine whether a node can be moved forward or backward in the list of phrase nodes.
        /// </summary>
        private bool CanMovePhraseNode(PhraseNode node, PhraseNode.Direction dir)
        {
            if (dir == PhraseNode.Direction.Forward)
            {
                return node.Index < ((SectionNode)node.getParent()).PhraseChildCount;
            }
            else
            {
                return node.Index > 0;
            }
        }

        /// <summary>
        /// Delete a phrase node from the tree.
        /// </summary>
        /// <param name="node">The phrase node to delete.</param>
        public void RemovePhraseNode(PhraseNode node)
        {
            DeletedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, node));
            node.DetachFromParent();
            Modified();
        }

        /// <summary>
        /// Delete a phrase node from the tree and remove its asset from the asset manager.
        /// </summary>
        /// <param name="node">The phrase node to delete.</param>
        /// <returns>A suitable command for shallow delete.</returns>
        public Commands.Strips.DeletePhrase RemovePhraseNodeAndAsset(PhraseNode node)
        {
            Commands.Strips.DeletePhrase command = new Commands.Strips.DeletePhrase(node);
            mAssManager.RemoveAsset(node.Asset);
            RemovePhraseNode(node);
            return command;
        }

        /// <summary>
        /// Get the next phrase in the section. If this is the last phrase, then return null. If the original node is null,
        /// return null as well.
        /// </summary>
        public static PhraseNode GetNextPhrase(PhraseNode node)
        {
            if (node != null)
            {
                if (node.getParent().GetType() == Type.GetType("Obi.SectionNode"))
                {
                    SectionNode parent = (SectionNode)node.getParent();
                    if (node.Index == parent.PhraseChildCount - 1) return null;
                    else return parent.PhraseChild(node.Index + 1);
                }
                else
                {//TODO: when will this case arise?
                }
            }
            return null;
        }

        /// <summary>
        /// Move a phrase node in the given direction.
        /// </summary>
        public void MovePhraseNode(PhraseNode node, PhraseNode.Direction dir)
        {
            int index = node.Index;
            SectionNode parent = (SectionNode)node.getParent();
            RemovePhraseNode(node);
            AddPhraseNode(node, parent, dir == PhraseNode.Direction.Forward ? index + 1 : index - 1);
        }

        /// <summary>
        /// Edit the annotation (i.e. label) of a phrase node.
        /// </summary>
        internal void EditAnnotationPhraseNode(PhraseNode node, string name)
        {
            TextMedia media = (TextMedia)GetMediaForChannel(node, Project.AnnotationChannelName);
            Assets.AudioMediaAsset asset = node.Asset;
            mAssManager.RenameAsset(asset, name);
            media.setText(asset.Name);
            MediaSet(this, new Events.Node.SetMediaEventArgs(this, node, Project.AnnotationChannelName, media));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Set the audio media asset for a phrase node.
        /// The Sequence media object is updated as well.
        /// </summary>
        internal void SetAudioMediaAsset(PhraseNode node, AudioMediaAsset asset)
        {
            node.Asset = asset;
            UpdateSeq(node);
        }

        /// <summary>
        /// Make a new sequence media object for the asset of this node.
        /// The sequence media object is simply a translation of the list of clips.
        /// </summary>
        //md change from private to internal so it could be used by CopyPhraseAssets
        internal void UpdateSeq(PhraseNode node)
        {
            Assets.AudioMediaAsset asset = node.Asset;
            ChannelsProperty prop = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            SequenceMedia seq =
                (SequenceMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.EMPTY_SEQUENCE);
            foreach (Assets.AudioClip clip in asset.Clips)
            {
                AudioMedia audio = (AudioMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.AUDIO);
                UriBuilder builder = new UriBuilder();
                builder.Scheme = "file";
                builder.Path = clip.Path;
                Uri relUri = mAssManager.BaseURI.MakeRelativeUri(builder.Uri);
                audio.setLocation(new MediaLocation(relUri.ToString()));
                audio.setClipBegin(new Time((long)Math.Round(clip.BeginTime)));
                audio.setClipEnd(new Time((long)Math.Round(clip.EndTime)));
                seq.appendItem(audio);
            }
            prop.setMedia(mAudioChannel, seq);
        }

        /// <summary>
        /// Send a TouchedNode event.
        /// </summary>
        internal void TouchNode(CoreNode node)
        {
            TouchedNode(this, new Events.Node.NodeEventArgs(this, node));
        }

        /// <summary>
        /// Set the page for a phrase node. Create a new node if it did not exist before, otherwise update the label.
        /// </summary>
        internal void SetPageRequested(object sender, Events.Node.SetPageEventArgs e)
        {
            PageProperty pageProp = e.Node.getProperty(typeof(PageProperty)) as PageProperty;
            Commands.Strips.SetNewPageNumber command = null;
            if (pageProp == null)
            {
                pageProp = (PageProperty)getPresentation().getPropertyFactory().createProperty(PageProperty.NodeName,
                    ObiPropertyFactory.ObiNS);
                pageProp.PageNumber = e.PageNumber;
                e.Node.setProperty(pageProp);
                command = new Commands.Strips.SetNewPageNumber(e.Node);
            }
            else
            {
                int prev = pageProp.PageNumber;
                if (e.PageNumber != prev)
                {
                    pageProp.PageNumber = e.PageNumber;
                    command = new Commands.Strips.SetPageNumber(e.Node, prev);
                }
                else
                {
                    return;
                }
            }
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Remove a page for a phrase node.
        /// </summary>
        internal void RemovePageRequested(object sender, Events.Node.PhraseNodeEventArgs e)
        {
            Commands.Strips.RemovePageNumber command = new Commands.Strips.RemovePageNumber(e.Node);
            RemovePage(e.Origin, e.Node);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        /// <summary>
        /// Remove a page label from a phrase node (actually remove the property.)
        /// </summary>
        internal void RemovePage(object origin, PhraseNode node)
        {
            node.removeProperty(typeof(PageProperty));
            RemovedPageNumber(this, new Events.Node.PhraseNodeEventArgs(origin, node));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));            
        }

        #endregion

        /// <summary>
        /// Add an empty phrase node in a section at a given index.
        /// The phrase node gets the empty media asset.
        /// </summary>
        /// <param name="section">The section node to add to.</param>
        /// <param name="index">The index at which the new node is added.</param>
        public void AddEmptyPhraseNode(SectionNode section, int index)
        {
            PhraseNode node = getPresentation().getCoreNodeFactory().createNode(PhraseNode.Name, ObiPropertyFactory.ObiNS)
                as PhraseNode;
            node.Asset = AudioMediaAsset.Empty;
            AddPhraseNode(node, section, index);
            Commands.Strips.AddPhrase command = new Commands.Strips.AddPhrase(node);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            Modified();
        }

        /// <summary>
        /// Add an already existing phrase node.
        /// </summary>
        /// <param name="node">The phrase node to add.</param>
        /// <param name="parent">Its parent section.</param>
        /// <param name="index">Its position in the parent section (with regards to other phrases.)</param>
        public void AddPhraseNode(PhraseNode node, SectionNode parent, int index)
        {
            parent.AddChildPhrase(node, index);
            AddedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, node));
            Modified();
        }

        /// <summary>
        /// Add an already existing phrase node and its asset.
        /// </summary>
        /// <param name="node">The phrase node to add.</param>
        /// <param name="parent">Its parent section.</param>
        /// <param name="index">Its position in the parent section (with regards to other phrases.)</param>
        public void AddPhraseNodeAndAsset(PhraseNode node, SectionNode parent, int index)
        {
            Assets.AudioMediaAsset asset = node.Asset;
            mAssManager.AddAsset(asset);
            AddPhraseNode(node, parent, index);
        }
    }
}