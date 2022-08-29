namespace Refactor.Core
{
    public class Layer
    {
        public List<Node> nodes = new List<Node>();

        public Layer()
        {
        }
        public Layer(List<Node> nodes)
        {
            this.nodes.AddRange(nodes);
        }
        public Layer(Layer layer)
        {
            this.nodes.AddRange(layer.nodes);
        }
        public Node this[int i]
        {
            get { return nodes[i]; }
            set { nodes[i] = value; }
        }
        public void AddNode(Node node)
        {
            nodes.Add(node);
        }
        public void RemoveNode(Node node)
        {
            nodes.Remove(node);
        }
        public bool Contains(Node node)
        {
            return nodes.Contains(node);
        }
        public bool Contains(Package package)
        {
            foreach (Node node in nodes)
            {
                if(node.Contains(package))
                    return true;
            }
            return false;
        }
        public int Count { get { return nodes.Count; } }
        public Layer Concat(Layer layer)
        {
            return new Layer(nodes.Concat(layer.nodes).ToList());
        }
        public bool IsDependOnThis(IEnumerable<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                foreach (Node dependency in node.dependencies)
                {
                    if (this.nodes.Contains(dependency))
                        return true;
                }
            }
            return false;
        }
        public bool HasOutDegreeTo(Layer nodes, int direction = 1)
        {
            foreach (Node node in this.nodes)
            {
                List<Node> outEdges = node.GetOutEdges(direction);

                foreach (Node outNode in outEdges)
                {
                    if (nodes.Contains(outNode))
                        return true;
                }
            }
            return false;
        }
        public bool HasInDegreeTo(Layer nodes, int direction = 1)
        {
            foreach (Node node in this.nodes)
            {
                List<Node> inEdges = node.GetInEdges(direction);

                foreach (Node inNode in inEdges)
                {
                    if (nodes.Contains(inNode))
                        return true;
                }
            }
            return false;
        }
        public int CountOutDegreeTo(Layer nodes, int direction = 1)
        {
            int count = 0;
            foreach (Node node in this.nodes)
            {
                List<Node> outEdges = node.GetOutEdges(direction);

                foreach (Node outNode in outEdges)
                {
                    if (nodes.Contains(outNode))
                        count++;
                }
            }
            return count;
        }
        public int CountInDegreeTo(Layer nodes, int direction = 1)
        {
            int count = 0;
            foreach (Node node in this.nodes)
            {
                List<Node> inEdges = node.GetInEdges(direction);

                foreach (Node inNode in inEdges)
                {
                    if (nodes.Contains(inNode))
                        count++;
                }
            }
            return count;
        }
        public int CountOutPackagesTo(Layer nodes, int direction = 1)
        {
            int count = 0;
            foreach (Node node in this.nodes)
            {
                foreach (Package package in node.GetOutPackages(direction))
                {
                    if (nodes.Contains(package))
                        count++;
                }
            }
            return count;
        }
        public int CountInPackagesTo(Layer nodes, int direction = 1)
        {
            int count = 0;
            foreach (Node node in this.nodes)
            {
                foreach (Package package in node.GetInPackages(direction))
                {
                    if (nodes.Contains(package))
                        count++;
                }
            }
            return count;
        }
        public bool IsOutDegreeCoveredInNodesAndItsOutDegree(Layer nodes, int direction = 1)
        {
            HashSet<Node> newLayer = nodes;
            HashSet<Node> newOutDegree = nodes;
            HashSet<Node> thisLayer = this.nodes.ToHashSet();
            HashSet<Node> thisOutDegree = this.nodes.ToHashSet();
            foreach (Node node in newLayer)
            {
                newOutDegree.UnionWith(node.GetOutEdges(direction));
            }
            foreach (Node node in thisLayer)
            {
                thisOutDegree.UnionWith(node.GetOutEdges(direction));
            }
            if (thisOutDegree.Except(newOutDegree).Except(thisLayer).ToList().Count == 0)
                return true;
            return false;
        }
        public bool IsInDegreeCoveredInNodesAndItsInDegree(Layer nodes, int direction = 1)
        {
            HashSet<Node> newLayer = nodes;
            HashSet<Node> newInDegree = nodes;
            HashSet<Node> thisLayer = this.nodes.ToHashSet();
            HashSet<Node> thisInDegree = this.nodes.ToHashSet();
            foreach (Node node in newLayer)
            {
                newInDegree.UnionWith(node.GetInEdges(direction));
            }
            foreach (Node node in thisLayer)
            {
                thisInDegree.UnionWith(node.GetInEdges(direction));
            }
            if (thisInDegree.Except(newInDegree).Except(thisLayer).ToList().Count == 0)
                return true;
            return false;
        }

        public List<Package> ToPackages()
        {
            List<Package> packages = new List<Package>();
            foreach (Node node in nodes)
            {
                foreach (Package package in node.packages)
                    packages.Add(package);
            }
            return packages;
        }
        public static implicit operator List<Node>(Layer layer)
        {
            return layer.nodes;
        }
        public static implicit operator HashSet<Node>(Layer layer)
        {
            return layer.nodes.ToHashSet();
        }
        public static implicit operator Layer(List<Node> layer)
        {
            return new Layer(layer);
        }
        public override string ToString()
        {
            string s = "";
            foreach (Node node in nodes)
            {
                s += node.ToString() + "\n";
            }
            return s;
        }
    }
}
