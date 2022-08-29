using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Steps
{
    public class MaxDepthLayer : Step<Graph, Hierarchies>
    {
        public override string StepDescription
        {
            get { return "Main"; }
        }
        public override string DetailDescription
        {
            get { return "Max Depth Layer Algorithm"; }
        }

        public int direction = 0; // 0: build from bottom; 1: build from top
        private Dictionary<Node,int> maxDepths = new Dictionary<Node,int>();

        public MaxDepthLayer(int direction = 1)
        {
            this.direction = direction;
        }
        private int calculateNodeMaxDepth(Graph input)
        {
            HashSet<Node> nodes = input.nodeSet.Values.ToHashSet();
            Queue<Node> queue = new Queue<Node>();
            int maxDepth = 0;
            foreach (Node node in nodes)
            {
                if (node.GetOutDegree(direction) == 0)
                {
                    queue.Enqueue(node);
                }
                maxDepths[node] = 0;
            }
            while (queue.Count > 0)
            {
                int l = queue.Count;
                for (int i = 0; i < l; i++)
                {
                    Node node = queue.Dequeue();
                    maxDepth = Math.Max(maxDepth, maxDepths[node]);
                    foreach (Node inNode in node.GetInEdges(direction))
                    {
                        maxDepths[inNode] = Math.Max(maxDepths[inNode], maxDepths[node] + 1);
                        queue.Enqueue(inNode);
                    }
                }
            }
            return maxDepth;
        }
        public override Hierarchies Process(Graph input)
        {
            int maxDepth = calculateNodeMaxDepth(input);
            List<Layer> layers = new List<Layer>();
            List<Node> nodes = input.nodeSet.Values.ToHashSet().ToList();
            for (int i = 0; i <= maxDepth; i++)
            {
                Layer layer = new Layer();
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (maxDepths[nodes[j]] == i)
                    {
                        layer.AddNode(nodes[j]);
                    }
                }
                layers.Add(layer);
            }

            if (direction == 0)
                layers.Reverse();
            return layers;
        }
    }
}
