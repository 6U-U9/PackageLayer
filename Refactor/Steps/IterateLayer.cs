using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor.Steps
{
    public class IterateLayer : Step<List<Node>, Hierarchies>
    {
        public override string StepDescription
        {
            get { return "Main"; }
        }

        public override string DetailDescription
        {
            get { return "Iterated Original Layer Algorithm"; }
        }

        BuildGraph buildGraph;
        MergeCircleNodes mergeCircleNodes;
        BuildIndirectEdges buildIndirectEdges;
        GenerateTopoList generateTopoList;
        OriginalLayer originalLayer;

        public int direction = 0; // 0: build from bottom; 1: build from top
        public IterateLayer(int direction = 1, int length = -1, int methodIndex = 0)
        {
            this.direction = direction;

            buildGraph = new BuildGraph();
            mergeCircleNodes = new MergeCircleNodes();
            buildIndirectEdges = new BuildIndirectEdges(length);
            generateTopoList = new GenerateTopoList(direction, methodIndex);
            originalLayer = new OriginalLayer(direction);
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
            return layerList;
        }
    }
}
