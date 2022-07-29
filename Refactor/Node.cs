using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor
{
    public class Node
    {
        public List<Package> dependency = new List<Package>();
        public List<Package> packages = new List<Package>();

        public HashSet<Node> inNodes = new HashSet<Node>();//被哪些包依赖
        public HashSet<Node> indirectInNodes = new HashSet<Node>();
        public HashSet<Node> outNodes = new HashSet<Node>();//依赖哪些包
        public HashSet<Node> indirectOutNodes = new HashSet<Node>();

        public int color = 0;//0, white; 1, gray; 2, black
        public Node? forward = null;

        public Node(Package package)
        {
            this.packages.Add(package);
            this.dependency.AddRange(package.dependency);
        }
        public Node(List<Package> dependency, List<Package> packages)
        {
            this.dependency = dependency;
            this.packages = packages;
        }
        public static Node Union(List<Node> ring)
        {
            List<Package> newDependency = new List<Package>();
            List<Package> newPackages = new List<Package>();
            foreach (Node node in ring)
            {
                newDependency = newDependency.Union(node.dependency).ToList();
                newPackages = newPackages.Union(node.packages).ToList();
            }
            newDependency = newDependency.Except(newPackages).ToList();
            return new Node(newDependency, newPackages);
        }
        public void addInNode(Node node)
        {
            inNodes.Add(node);
        }
        public void addOutNode(Node node)
        {
            outNodes.Add(node);
        }
        public int getInNodeCount()
        {
            return inNodes.Count / packages.Count;
        }
        public int getOutNodeCount()
        {
            return outNodes.Count;
        }
        public int getIndirectInNodeCount()
        {
            return indirectInNodes.Count / packages.Count;
        }
        public int getIndirectOutNodeCount()
        {
            return indirectOutNodes.Count;
        }
        public bool isCircleNode()
        {
            return packages.Count > 1;
        }
        public override string ToString()
        {
            string s = "";
            foreach (Package package in packages)
            {
                s += package.ToString() + ";";
            }
            return s;
        }
    }
}
