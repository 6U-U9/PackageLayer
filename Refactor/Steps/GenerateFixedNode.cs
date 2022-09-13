using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Steps
{
    public class GenerateFixedNode : Step<Graph,Graph>
    {
        public override string StepDescription
        {
            get { return "Modify"; }
        }
        public override string DetailDescription
        {
            get { return "Generate Fixed Node"; }
        }
        public override string ChineseDescription
        {
            get
            {
                string s = "";
                foreach (string name in anchorNames)
                    s += name + ";";
                return $"从环节点中分离1层固定：{s} 的包。每个包生成一个独立的节点；环中其他包仍作为一个节点；为了保持图仍为有向无环图，去除指向锚定节点的边";
            }
        }

        public int direction = 1;
        public List<Package> anchors;
        private List<string> anchorNames;

        public GenerateFixedNode(List<string> anchors,int direction =1)
        {
            this.direction = direction;
            this.anchors = new List<Package>();
            this.anchorNames = anchors;
        }
        public override Graph Process(Graph input)
        {
            List<Node> fixedNode = new List<Node>();
            foreach (string name in anchorNames)
            {
                Package p = Package.Get(name);
                this.anchors.Add(p);
                fixedNode.Add(new Node(p));
            }
            
            Graph graph = input;
            foreach (Package package in graph.nodeSet.Keys)
            {
                foreach (Package p in anchors)
                {
                    graph.nodeSet[package].packages.Remove(p);
                }
            }
            foreach (Node node in fixedNode)
            {
                graph.AddNode(node);
            }
            graph.BuildEdges();
            foreach (Node node in fixedNode)
            {
                foreach (Node n in node.GetOutEdges(direction))
                    n.GetInEdges(direction).Remove(node);
                node.GetOutEdges(direction).Clear();
            }
            return graph;
        }
    }
}
