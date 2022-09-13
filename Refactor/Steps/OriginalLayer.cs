using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Steps
{
    public class OriginalLayer :Step<List<Node>,Hierarchies>
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
                return $"原始算法 算法方向：{directionDescriptions[direction]}";
            }
        }

        public int direction = 0; // 0: build from bottom; 1: build from top
        private bool iterate = false;
        public OriginalLayer(int direction = 1, bool iterate = false)
        {
            this.direction = direction;
            this.iterate = iterate;
        }
        public override Hierarchies Process(List<Node> topoList)
        {
            Layer remain = topoList.ToList();
            List<Layer> layers = new List<Layer>();
            Layer lastlayer = new Layer();
            int i;
            for (i = 0; i < topoList.Count; i++)
            {
                if (topoList[i].GetOutDegree(direction) == 0)
                {
                    remain.RemoveNode(topoList[i]);
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
