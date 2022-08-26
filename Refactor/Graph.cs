using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor
{
    public class Graph
    {
        public Dictionary<Package, Node> nodeSet = new Dictionary<Package, Node>();

        public void AddNode(Package package)
        {
            nodeSet[package] = new Node(package);
        }
        public void AddNode(Node node)
        {
            foreach (var package in node.packages)
            {
                nodeSet[package] = node;
            }
        }

        public void BuildEdges()
        {
            foreach (var node in nodeSet.Values)
            {
                node.dependencies.Clear();
                node.dependents.Clear();
            }

            foreach (var node in nodeSet.Values)
            {
                foreach (var package in node.packages)
                {
                    foreach (var dependency in package.dependency)
                    {
                        if(nodeSet.ContainsKey(dependency))
                            node.dependencies.Add(nodeSet[dependency]);
                    }
                    foreach (var dependent in package.dependent)
                    {
                        if(nodeSet.ContainsKey(dependent))
                            node.dependents.Add(nodeSet[dependent]);
                    }
                }
            }
        }

        public Node UnionNodes(List<Node> circle)
        {
            Node newNode = new Node(circle);
            AddNode(newNode);
            BuildEdges();
            return newNode;
        }

        public Graph Copy()
        {
            Graph g = new Graph();
            foreach (var package in nodeSet.Keys)
            {
                g.AddNode(package);
            }
            g.BuildEdges();
            return g;
        }
    }
}