using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor
{
    public class Algorithms
    {
        public virtual string name { get { return ""; } }
        public virtual List<Layer> generateLayersList(Graph graph)
        {
            throw new NotImplementedException();
        }
        public virtual void run(Graph graph, List<object>? parameters = null)
        {
        }
    }
    public class Origin:Algorithms
    {
        public override string name { get { return "原始算法复现"; } }
        public override void run(Graph graph, List<object>? parameters = null)
        {
        }
        public List<Node> generateTopoList(Graph graph)
        {
            graph.buildMap();
            List<Node> list = new List<Node>();
            HashSet<Node> nodes = graph.nodeSet.Values.ToHashSet();
            while (nodes.Count > 0)
            {
                List<Node> temp = new List<Node>();
                foreach (Node node in nodes)
                {
                    //if (node.getInNodeCount() == 0)
                    //    temp.Add(node);
                    if (node.getOutNodeCount() == 0)
                        temp.Add(node);
                }
                temp.Sort((a, b) =>
                {
                    //return b.getIndirectInNodeCount().CompareTo(a.getIndirectInNodeCount());
                    if (a.getInNodeCount() == b.getInNodeCount())
                        return b.getIndirectInNodeCount().CompareTo(a.getIndirectInNodeCount());
                    else
                        return b.getInNodeCount().CompareTo(a.getInNodeCount());
                });
                foreach (Node node in temp)
                {
                    nodes.Remove(node);
                    list.Add(node);
                    //foreach (Node dependency in node.outNodes)
                    //{
                    //    dependency.inNodes.Remove(node);
                    //}
                    foreach (Node dependency in node.inNodes)
                    {
                        dependency.outNodes.Remove(node);
                    }
                }
            }
            return list;
        }
        public override List<Layer> generateLayersList(Graph graph)
        {
            List<Node> topoList = generateTopoList(graph);
            HashSet<Node> remain = topoList.ToHashSet();
            List<Layer> layers = new List<Layer>();
            Layer lastlayer = new Layer();
            graph.buildMap(remain);
            int i;
            for (i = 0; i < topoList.Count; i++)
            {
                if (topoList[i].getOutNodeCount() == 0)
                {
                    remain.Remove(topoList[i]);
                    lastlayer.addNode(topoList[i]);
                }
                else
                    break;
            }
            layers.Add(lastlayer);

            Layer layer = new Layer();
            while (i < topoList.Count)
            {
                layer.addNode(topoList[i]);
                remain.Remove(topoList[i]);
                if (!lastlayer.isDepend(remain))
                {
                    layers.Add(layer);
                    lastlayer = layer;
                    layer = new Layer();
                }
                i++;
            }
            return layers;
        }
    }
    public class MaxDepth : Algorithms
    {
        public override string name { get { return "最大深度"; } }
        private Dictionary<Node, int> nodeMaxDepth = new Dictionary<Node, int>();
        private int calculateNodeMaxDepth(Graph graph)
        {
            HashSet<Node> nodes = graph.nodeSet.Values.ToHashSet();
            Queue<Node> queue = new Queue<Node>();
            int maxDepth = 0;
            foreach (Node node in nodes)
            {
                if (node.getOutNodeCount() == 0)
                {
                    nodeMaxDepth[node] = 0;
                    queue.Enqueue(node);
                }
            }
            while (queue.Count > 0)
            {
                int l = queue.Count;
                for (int i = 0; i < l; i++)
                {
                    Node node = queue.Dequeue();
                    maxDepth = Math.Max(maxDepth, nodeMaxDepth[node]);
                    foreach (Node dependency in node.inNodes)
                    {
                        nodeMaxDepth[dependency] = Math.Max(nodeMaxDepth[dependency], nodeMaxDepth[node] + 1);
                        queue.Enqueue(dependency);
                    }
                }
            }
            return maxDepth;
        }
        public override List<Layer> generateLayersList(Graph graph)
        {
            graph.buildMap();
            int maxDepth = calculateNodeMaxDepth(graph);
            List<Layer> layers = new List<Layer>();
            List<Node> nodes = graph.getNodes();
            for (int i = 0; i <= maxDepth; i++)
            {
                List<Node> layer = new List<Node>();
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (nodeMaxDepth[nodes[j]] == i)
                    {
                        layer.Add(nodes[j]);
                    }
                }
                layer.Sort((a, b) =>
                {
                    if (a.getInNodeCount() == b.getInNodeCount())
                        return b.getIndirectInNodeCount().CompareTo(a.getIndirectInNodeCount());
                    else
                        return b.getInNodeCount().CompareTo(a.getInNodeCount());
                });
                layers.Add(new Layer(layer));
            }
            return layers;
        }
    }
    public class Iterate : Algorithms
    { }
    public class Improved : Algorithms
    { }
}
