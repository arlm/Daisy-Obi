using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
  
    class ShallowSwappedSectionNodesEventArgs : NodeEventArgs
    {
        private SectionNode mSwappedNode;

        public SectionNode SwappedNode
        {
            get
            {
                return mSwappedNode;
            }
        }

      
        public ShallowSwappedSectionNodesEventArgs(object origin, SectionNode node, SectionNode swappedNode)
            :
            base(origin, node)
        {
            mSwappedNode = swappedNode;
        }

    }
}
