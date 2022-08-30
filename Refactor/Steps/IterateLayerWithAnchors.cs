using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Steps
{
    public class IterateLayerWithAnchors : Step<List<Node>, Hierarchies>
    {
        public override string StepDescription
        {
            get { return "Main"; }
        }
        public override string DetailDescription
        {
            get { return "Iterated Original Layer Algorithm"; }
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
                return $"迭代算法 算法方向：{directionDescriptions[direction]} 1层锚点：{s}";
            }
        }

        BuildGraph buildGraph;
        MergeCircleNodes mergeCircleNodes;
        BuildIndirectEdges buildIndirectEdges;
        GenerateTopoListWithAnchors generateTopoList;
        OriginalLayerWithAnchors originalLayer;

        public int direction = 0; // 0: build from bottom; 1: build from top
        public List<Package> anchors;
        private List<string> anchorNames;
        public IterateLayerWithAnchors(List<string> anchors, int direction = 1, int length = -1, int methodIndex = 0)
        {
            this.direction = direction;
            this.anchors = new List<Package>();
            this.anchorNames = anchors;

            buildGraph = new BuildGraph();
            mergeCircleNodes = new MergeCircleNodes();
            buildIndirectEdges = new BuildIndirectEdges(length);
            generateTopoList = new GenerateTopoListWithAnchors(anchors, direction, methodIndex);
            originalLayer = new OriginalLayerWithAnchors(anchors, direction);
        }
        public override Hierarchies Process(List<Node> input)
        {
            var layerList = originalLayer.Process(input);
            int layerCount = 0;
            while (layerList.Count != layerCount)
            {
                layerCount = layerList.Count;
                var nextLayerList = new List<Layer>();
                for (int i = 0; i < layerList.Count; i++)
                {
                    var subGraph = buildGraph.Process(layerList[i].ToPackages());
                    var mergedGraph = mergeCircleNodes.Process(subGraph);
                    buildIndirectEdges.Process(mergedGraph);
                    var subTopoList = generateTopoList.Process(mergedGraph);
                    var subLayerList = originalLayer.Process(subTopoList);

                    for (int j = 0; j < subLayerList.Count; j++)
                    {
                        nextLayerList.Add(subLayerList[j]);
                    }
                }
                layerList = nextLayerList;
            }
            Graph full = new Graph();
            for (int i = 0; i < layerList.Count; i++)
                for (int j = 0; j < layerList[i].Count; j++)
                    full.AddNode(layerList[i][j]);
            full.BuildEdges();
            
            if (direction == 0)
                layerList.Reverse();
            return layerList;
        }
    }
}
