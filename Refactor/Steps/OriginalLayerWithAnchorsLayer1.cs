using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Steps
{
    public class OriginalLayerWithLayer1 : Step<List<Node>,Hierarchies>
    {
        public override string StepDescription
        {
            get { return "Main"; }
        }
        public override string DetailDescription
        {
            get { return "Original Layer Algorithm"; }
        }
        public override string ChineseDescription
        {
            get
            {
                Dictionary<int, string> directionDescriptions;
                directionDescriptions = new Dictionary<int, string>()
                {
                    {0, "Bottom - Up" },
                    {1, "Top - Down" },
                };
                string s = "";
                foreach (string name in anchorNames)
                    s += name + ";";
                return $"原始算法方向：{directionDescriptions[direction]} 1层固定：{s}的包代替入度为0的点作为算法起始时的第一层";
            }
        }

        public int direction = 0; // 0: build from bottom; 1: build from top
        public List<Package> anchors;
        private List<string> anchorNames;
        private bool iterate = false;
        public OriginalLayerWithLayer1(List<string> anchors, int direction = 1, bool iterate = false)
        {
            this.direction = direction;
            this.anchors = new List<Package>();
            this.anchorNames = anchors;
            this.iterate = iterate;
        }
        public override Hierarchies Process(List<Node> topoList)
        {
            foreach (string name in anchorNames)
            {
                this.anchors.Add(Package.Get(name));
            }

            Layer remain = topoList.ToList();
            List<Layer> layers = new List<Layer>();
            Layer lastlayer = new Layer();
            int i;
            for (i = 0; i < topoList.Count; i++)
            {
                if (topoList[i].HasIntersect(anchors))
                {
                    remain.RemoveNode(topoList[i]);
                    lastlayer.AddNode(topoList[i]);
                }
            }
            for(i=0; i<lastlayer.Count; i++)
            {
                topoList.Remove(lastlayer[i]);
            }

            //for (i = 0; i < topoList.Count; i++)
            //{
            //    if (topoList[i].GetOutDegree(direction) == 0)
            //    {
            //        remain.RemoveNode(topoList[i]);
            //        lastlayer.AddNode(topoList[i]);
            //    }
            //    else
            //        break;
            //}
            i = 0;
            layers.Add(lastlayer);

            Layer layer = new Layer();
            while (i < topoList.Count)
            {
                layer.AddNode(topoList[i]);
                remain.RemoveNode(topoList[i]);
                if (!remain.HasOutDegreeTo(lastlayer,direction)) //keynode
                {
                    layers.Add(layer);
                    lastlayer = layer;
                    layer = new List<Node>();
                }
                i++;
            }
            
            if (direction == 0 && !iterate)
                layers.Reverse();
            return layers;
        }
    }
}
