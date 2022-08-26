using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor
{
    public class Node
    {
        public HashSet<Package> packages = new();

        public List<Node> dependents = new(); //被哪些包依赖
        public List<Node> indirectDependents = new();
        public List<Node> dependencies = new(); //依赖哪些包
        public List<Node> indirectDependencies = new();

        public int color = 0; //0, white; 1, gray; 2, black
        public Node? forward = null;

        public Node()
        {
        }

        public Node(Package package)
        {
            this.packages.Add(package);
        }

        public Node(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                foreach (var package in node.packages)
                {
                    this.packages.Add(package);
                }
            }
        }

        public bool IsCircleNode()
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
