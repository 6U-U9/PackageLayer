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

        //public int color = 0; //0, white; 1, gray; 2, black
        //public Node? forward = null;

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
        public bool Contains(Package package)
        {
            return packages.Contains(package);
        }
        public bool HasIntersect(List<Package> packages)
        {
            return packages.Intersect(this.packages).Count() > 0;
        }
        public int GetInDegree(int direction)
        {
            if (direction == 0) // Bottom -> Up
                return dependencies.Count;
            else if (direction == 1)
                return dependents.Count;
            else
                throw new ArgumentOutOfRangeException(String.Format("direction is {0}, not 0 or 1",direction));
        }
        public int GetOutDegree(int direction)
        {
            if (direction == 0) // Bottom -> Up
                return dependents.Count;
            else if (direction == 1)
                return dependencies.Count;
            else
                throw new ArgumentOutOfRangeException(String.Format("direction is {0}, not 0 or 1", direction));
        }
        public List<Node> GetInEdges(int direction)
        {
            if (direction == 0) // Bottom -> Up
                return dependencies;
            else if (direction == 1)
                return dependents;
            else
                throw new ArgumentOutOfRangeException(String.Format("direction is {0}, not 0 or 1", direction));
        }
        public List<Node> GetOutEdges(int direction)
        {
            if (direction == 0) // Bottom -> Up
                return dependents;
            else if (direction == 1)
                return dependencies;
            else
                throw new ArgumentOutOfRangeException(String.Format("direction is {0}, not 0 or 1", direction));
        }
        public int GetIndirectInDegree(int direction)
        {
            if (direction == 0) // Bottom -> Up
                return indirectDependencies.Count;
            else if (direction == 1)
                return indirectDependents.Count;
            else
                throw new ArgumentOutOfRangeException(String.Format("direction is {0}, not 0 or 1", direction));
        }
        public int GetIndirectOutDegree(int direction)
        {
            if (direction == 0) // Bottom -> Up
                return indirectDependents.Count;
            else if (direction == 1)
                return indirectDependencies.Count;
            else
                throw new ArgumentOutOfRangeException(String.Format("direction is {0}, not 0 or 1", direction));
        }
        public bool RemoveInEdge(int direction, Node node)
        {
            if (direction == 0) // Bottom -> Up
                return dependencies.Remove(node);
            else if (direction == 1)
                return dependents.Remove(node);
            else
                throw new ArgumentOutOfRangeException(String.Format("direction is {0}, not 0 or 1", direction));
        }
        public bool RemoveOutEdge(int direction,Node node)
        {
            if (direction == 0) // Bottom -> Up
                return dependents.Remove(node);
            else if (direction == 1)
                return dependencies.Remove(node);
            else
                throw new ArgumentOutOfRangeException(String.Format("direction is {0}, not 0 or 1", direction));
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
