using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Steps
{
    public class ImprovedLayerWithAnchors : Step<List<Node>, Hierarchies>
    {
        public override string StepDescription
        {
            get { return "Main"; }
        }
        public override string DetailDescription
        {
            get { return "Improved Layer Algorithm"; }
        }

        public int direction = 0; // 0: build from bottom; 1: build from top
        public List<Package> anchors;
        private List<string> anchorNames;

        public ImprovedLayerWithAnchors(List<string> anchors, int direction = 1)
        {
            this.direction = direction;
            this.anchors = new List<Package>();
            this.anchorNames = anchors;
        }
        public override Hierarchies Process(List<Node> topoList)
        {
            foreach (string name in anchorNames)
            {
                this.anchors.Add(Package.Get(name));
            }

            List<Layer> layers = new List<Layer>();
            Layer lastlayer = new Layer();
            int i;
            for (i = 0; i < topoList.Count; i++)
            {
                if (topoList[i].HasIntersect(anchors))
                {
                    lastlayer.AddNode(topoList[i]);
                }
            }
            for (i = 0; i < lastlayer.Count; i++)
            {
                topoList.Remove(lastlayer[i]);
            }

            for (i = 0; i < topoList.Count; i++)
            {
                if (topoList[i].GetOutDegree(direction) == 0)
                {
                    lastlayer.AddNode(topoList[i]);
                }
                else
                    break;
            }
            layers.Add(lastlayer);

            Layer layer = new Layer();
            while (i < topoList.Count)
            {
                layer.AddNode(topoList[i]);
                if (lastlayer.IsInDegreeCoveredInNodesAndItsInDegree(layer,direction))
                {
                    layers.Add(layer);
                    lastlayer = layer;
                    layer = new Layer();
                }
                i++;
            }
            
            if (direction == 0)
                layers.Reverse();
            return layers;
        }
    }
}
