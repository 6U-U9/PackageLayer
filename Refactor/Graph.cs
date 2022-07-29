using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor
{
    public class Graph
    {
        public static BasicGraph initialGraph = new BasicGraph();
        public Dictionary<Package, Node> nodeSet = new Dictionary<Package, Node>();
        public HashSet<Package> packages = new HashSet<Package>();
        public Package getPackage(string name)
        {
            Package temp = new Package(name);
            return getPackage(temp);
        }
        public Package getPackage(Package package)
        {
            Package temp = package;
            if (!packages.TryGetValue(temp, out temp))
                packages.Add(temp);
            return temp;
        }
        public Node getNode(string name)
        {
            Package package = getPackage(name);
            return getNode(package);
        }
        public Node getNode(Package package)
        {
            package = getPackage(package);
            if (!nodeSet.ContainsKey(package))
                nodeSet.Add(package, new Node(package));
            return nodeSet[package];
        }
        public List<Node> getNodes()
        {
            return nodeSet.Values.ToHashSet().ToList();
        }
        public virtual void addDependency(string packageName, string dependencyName)
        {
            //initialGraph.addDependency(package, dependency);
            Package package = getPackage(packageName), dependency = getPackage(dependencyName);
            package.dependency.Add(dependency);
            dependency.dependent.Add(package);

            Node packageNode = getNode(package);
            Node dependencyNode = getNode(dependency);
        }
        public virtual void addHumanLayer(string name, int layer)
        {
            //initialGraph.addHumanLayer(name, layer);
            Package package = getPackage(name);
            package.human = layer;
        }
        public virtual void addPackageCategory(string name, string category)
        {
            //initialGraph.addPackageCategory(name, category);
            Package package = getPackage(name);
            package.category.Add(category);
        }
        public virtual void update(Node node)
        {
            foreach (Package package in node.packages)
                nodeSet[package] = node;
        }
        public virtual void mergeCircleNodes()
        {
            foreach (Node node in nodeSet.Values)
            {
                if (node.color == 0)
                    dfs_search(node);
            }
        }
        private void dfs_search(Node node)
        {
            node.color = 1;
            for (int i = 0; i < node.dependency.Count; i++)
            {
                Node next = getNode(node.dependency[i]);
                if (next.color == 0)
                {
                    next.forward = node;
                    dfs_search(next);
                }
                else if (next.color == 1)
                {
                    List<Node> circle = new List<Node>();
                    while (node != next)
                    {
                        circle.Add(node);
                        node = node.forward;
                    }
                    circle.Add(node);
                    Node newNode = Node.Union(circle);
                    newNode.forward = next.forward;
                    update(newNode);
                    dfs_search(newNode);
                }
                else
                {
                    continue;
                }
            }
            node.color = 2;
        }

        public void clearNodeEdges()
        {
            foreach (Node node in nodeSet.Values)
            {
                node.inNodes.Clear();
                node.outNodes.Clear();
                node.indirectInNodes.Clear();
                node.indirectOutNodes.Clear();
            }
        }
        private void genIndirectNodes()
        {
            //In
            {
                HashSet<Node> nodes = nodeSet.Values.ToHashSet();
                Queue<Node> queue = new Queue<Node>();
                foreach (Node node in nodes)
                {
                    node.indirectInNodes.UnionWith(node.inNodes);
                    if (node.getInNodeCount() == 0)
                        queue.Enqueue(node);
                }
                HashSet<Node> finished = new HashSet<Node>();
                while (queue.Count > 0)
                {
                    finished.UnionWith(queue.ToHashSet());
                    int l = queue.Count;
                    for (int i = 0; i < l; i++)
                    {
                        Node node = queue.Dequeue();
                        foreach (Node dependency in node.outNodes)
                        {
                            dependency.indirectInNodes.UnionWith(node.indirectInNodes);
                            if (finished.Contains(dependency))
                                continue;
                            queue.Enqueue(dependency);
                        }
                    }
                }
            }

            //Out
            {
                HashSet<Node> nodes = nodeSet.Values.ToHashSet();
                Queue<Node> queue = new Queue<Node>();
                foreach (Node node in nodes)
                {
                    node.indirectOutNodes.UnionWith(node.outNodes);
                    if (node.getOutNodeCount() == 0)
                        queue.Enqueue(node);
                }
                HashSet<Node> finished = new HashSet<Node>();
                while (queue.Count > 0)
                {
                    finished.UnionWith(queue.ToHashSet());
                    int l = queue.Count;
                    for (int i = 0; i < l; i++)
                    {
                        Node node = queue.Dequeue();
                        foreach (Node dependent in node.inNodes)
                        {
                            dependent.indirectOutNodes.UnionWith(node.indirectOutNodes);
                            if (finished.Contains(dependent))
                                continue;
                            queue.Enqueue(dependent);
                        }
                    }
                }
            }
        }
        public void buildMap()
        {
            clearNodeEdges();
            foreach (Node node in nodeSet.Values)
            {
                foreach (Package package in node.dependency)
                {
                    getNode(package).addInNode(node);
                    node.addOutNode(getNode(package));
                }
            }
            genIndirectNodes();
        }
        public void buildMap(HashSet<Node> part)
        {
            clearNodeEdges();
            foreach (Node node in part)
            {
                foreach (Package package in node.dependency)
                {
                    if (part.Contains(getNode(package)))
                    {
                        getNode(package).addInNode(node);
                        node.addOutNode(getNode(package));
                    }
                }
            }
            genIndirectNodes();
        }
        
    }


    public class BasicGraph : Graph
    {
        public override void addDependency(string packageName, string dependencyName)
        {
            Package package = getPackage(packageName), dependency = getPackage(dependencyName);
            package.dependency.Add(dependency);
            dependency.dependent.Add(package);

            Node packageNode = getNode(package);
            Node dependencyNode = getNode(dependency);
        }
        public override void addHumanLayer(string name, int layer)
        {
            Package package = getPackage(name);
            package.human = layer;
        }
        public override void addPackageCategory(string name, string category)
        {
            Package package = getPackage(name);
            package.category.Add(category);
        }
        public override void update(Node node)
        {
            throw new NotImplementedException();
        }
        public override void mergeCircleNodes()
        {
            throw new NotImplementedException();
        }
    }
}
