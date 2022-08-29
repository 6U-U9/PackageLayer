using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Steps
{
    public class MergeCircleNodes : Step<Graph, Graph>
    {
        public override string StepDescription
        {
            get { return "Modify"; }
        }

        public override string DetailDescription
        {
            get { return "Merge nodes in circles to one node"; }
        }

        private Dictionary<Node,int> color = new Dictionary<Node,int>(); //0, white; 1, gray; 2, black
        private Dictionary<Node,Node?> forward = new Dictionary<Node,Node?>();

        private void dfs(Graph graph)
        {
            foreach (Node node in graph.nodeSet.Values)
            {
                color[node] = 0;
                forward[node] = null;
            }

            List<Node> temp = graph.nodeSet.Values.ToList();
            foreach (Node node in temp)
            {
                if (color[node] == 0)
                    dfs_search(graph, node);
            }
        }

        private void dfs_search(Graph graph, Node node)
        {
            color[node] = 1;
            for (int i = 0; i < node.dependencies.Count; i++)
            {
                if (!graph.nodeSet.Values.Contains(node))
                    break;
                Node next = node.dependencies[i];
                if (color[next] == 0)
                {
                    forward[next] = node;
                    dfs_search(graph, next);
                }
                else if (color[next] == 1)
                {
                    List<Node> circle = new List<Node>();
                    while (node != next)
                    {
                        circle.Add(node);
                        node = forward[node];
                    }

                    circle.Add(node);
                    Node newNode = graph.UnionNodes(circle);
                    forward[newNode] = forward[next];
                    color[newNode] = 0;
                    dfs_search(graph, newNode);
                }
                else
                {
                    continue;
                }
            }

            color[node] = 2;
        }

        public override Graph Process(Graph input)
        {
            Graph graph = input.Copy();
            dfs(graph);
            return graph;
        }
    }
}