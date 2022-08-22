using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer
{
    public class Node
    {
        public List<Package> dependency = new List<Package>();
        public List<Package> packages = new List<Package>();
        public Dictionary<Node,int> circleEdgeCount = new ();
        public int color = 0;//0, white; 1, gray; 2, black
        public bool visited = false;
        public bool started = false;

        public int maxDepth = 0;

        public HashSet<Node> inNodes = new HashSet<Node>();//被哪些包依赖
        public HashSet<Node> indirectInNodes = new HashSet<Node>();
        public HashSet<Node> outNodes = new HashSet<Node>();//依赖哪些包
        public HashSet<Node> indirectOutNodes = new HashSet<Node>();
        public Node? forward = null;
        public Node(Package package)
        {
            this.packages.Add(package);
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
            return inNodes.Count/packages.Count;
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
    public class NodeSet
    {
        public Dictionary<Package, Node> singleNodeSet = new Dictionary<Package, Node>();

        public Dictionary<Package,Node> nodeSet=new Dictionary<Package,Node>();
        public HashSet<Package> packages = new HashSet<Package>();
        public Dictionary<Package, int> humanLayers = new Dictionary<Package, int>();
        public string algorithm="";
        public List<List<Node>> layerList = new List<List<Node>>();
        public Dictionary<Package, int> algorithmLayers = new Dictionary<Package, int>();
        public Dictionary<Package, List<string>> packageCategory = new Dictionary<Package, List<string>>();
        public Dictionary<Package, List<Package>> packageIn = new ();
        public Dictionary<Package, List<Package>> packageOut = new();

        public List<(Package,Package,int)> circleEdgeCount = new();

        public List<List<Node>> findAllCircle()
        {
            List<List<Node>> circles = new();
            Dictionary<Node, List<List<Node>>> circleNodeset = new();
            foreach (Node node in nodeSet.Values)
            {
                circleNodeset[node] = new();
                circleNodeset[node].Add(new List<Node>());
            }
            bool finish = false;
            while (!finish)
            {
                finish = true;
                Dictionary<Node, List<List<Node>>> circleNodesetNext = new();
                foreach (Node node in nodeSet.Values)
                {
                    circleNodesetNext[node] = new();
                    if(circleNodeset[node].Count>0)
                        finish=false;
                }
                foreach (Node node in nodeSet.Values)
                {
                    foreach (List<Node> stack in circleNodeset[node])
                    {
                        if (stack.Contains(node))
                        {
                            //todo : gen circle without duplicate
                            List<Node> nodes = new();
                            nodes.AddRange(stack);
                            nodes.Sort((a, b) =>
                            {
                                return a.packages[0].name.CompareTo(b.packages[0].name);
                            });
                            if (nodes[0] == node)
                                circles.Add(stack);
                        }
                        else
                        {
                            List<Node> nodes = new();
                            nodes.AddRange(stack);
                            nodes.Add(node);
                            foreach (Package p in node.dependency)
                            {
                                circleNodesetNext[getNode(p)].Add(nodes);
                            }
                        }
                    }
                }
                circleNodeset = circleNodesetNext;
            }
            return circles;
        }
        public void dfs_circle_count()
        {
            foreach (Node node in nodeSet.Values)
            {
                Console.WriteLine("----" + node.ToString() + "----");
                dfs_search_circle_count(node,node);
                node.started = true;
            }
        }
        public void dfs_search_circle_count(Node node,Node start)
        {
            node.visited=true;
            for (int i = 0; i < node.dependency.Count; i++)
            {
                Node next = getNode(node.dependency[i]);
                if (next.visited == false && next.started == false)
                {
                    next.forward = node;
                    dfs_search_circle_count(next, start);
                }
                else if (next == start)
                {
                    string cs="";
                    Node? p = node;
                    Node? prev = node.forward;
                    node.circleEdgeCount[next] = node.circleEdgeCount.ContainsKey(next) ? node.circleEdgeCount[next] + 1 : 1;
                    while (p != start)
                    {
                        cs = p.ToString() + cs;
                        prev.circleEdgeCount[p] = prev.circleEdgeCount.ContainsKey(p) ? prev.circleEdgeCount[p] + 1 : 1;
                        p = prev;
                        prev = prev.forward;
                    }
                    cs = start.ToString() + cs + "\n";
                    //Console.WriteLine(cs);
                }
            }
            node.visited = false;
        }
        public void buildEdgeSet()
        {
            foreach (var node in nodeSet.Values)
            {
                foreach (var dependency in node.dependency)
                {
                    int c = 0;
                    if (node.circleEdgeCount.ContainsKey(getNode(dependency)))
                        c = node.circleEdgeCount[getNode(dependency)];
                    circleEdgeCount.Add((node.packages[0],dependency,c));
                }
            }
            circleEdgeCount.Sort((b,a)=>{ return a.Item3.CompareTo(b.Item3); });
        }
        public void removeKeyEdges(int threshold)
        {
            foreach (var edge in circleEdgeCount)
            {
                if (edge.Item3 > threshold)
                {
                    getNode(edge.Item1.name).dependency.Remove(edge.Item2);
                } 
            }
        }

        public Node getNode(string name)
        {
            Package package = new Package(name);
            packages.Add(package);
            if (!nodeSet.ContainsKey(package))
                nodeSet.Add(package, new Node(package));
            if (!singleNodeSet.ContainsKey(package))
                singleNodeSet.Add(package, nodeSet[package]);
            return nodeSet[package];   
        }
        public Node getNode(Package package)
        {
            packages.Add(package);
            if (!nodeSet.ContainsKey(package))
                nodeSet.Add(package, new Node(package));
            if (!singleNodeSet.ContainsKey(package))
                singleNodeSet.Add(package, nodeSet[package]);
            return nodeSet[package];
        }
        public void update(Node node)
        {
            foreach (Package package in node.packages)
                nodeSet[package] = node;
        }
        public void addDependency(string package,string dependency)
        {
            Node packageNode = getNode(package);
            Node dependencyNode = getNode(dependency);
            packageNode.dependency.Add(new Package(dependency));

            Package d = new Package(dependency);
            Package p = new Package(package);

            if (packageIn.ContainsKey(d))
                packageIn[d].Add(p);
            else
                packageIn[d] = new List<Package> { p};
            if (packageOut.ContainsKey(p))
                packageOut[p].Add(d);
            else
                packageOut[p]=new List<Package> { d};
        }
        public void addHumanLayer(string name, int layer)
        {
            Package package = new Package(name);
            humanLayers[package]=layer;
        }

        public void addpackageCategory(string name, string category)
        {
            Package package = new Package(name);
            if(!packageCategory.ContainsKey(package))
                packageCategory[package] = new List<string> { category};
            else
                packageCategory[package].Add(category);
        }
        public double calculatePackageCategoryProbability(Package package, int index, List<List<Node>> layersList)
        {
            List<Node> layer = layersList[index];
            double prob = 0;
            double categoryAllCount=0, categoryLayerCount=0, categoriesLayerSum = 0;
            foreach (Node node in layer)
            {
                foreach (Package p in node.packages)
                {
                    foreach (string category in packageCategory[p])
                    {
                        if (packageCategory[package].Contains(category))
                        {
                            categoryLayerCount++;
                        }
                        categoriesLayerSum++;
                    }
                }
            }
            foreach (List<Node> list in layersList)
            {
                foreach (Node node in list)
                {
                    foreach (Package p in node.packages)
                    {
                        foreach (string category in packageCategory[p])
                        {
                            categoryAllCount++;
                        }
                    }
                }
            }
            prob = (categoryLayerCount*categoryLayerCount)/(categoriesLayerSum*categoryAllCount);
            return prob;
        }
        public double calculatePackageDependProbability(Package package, int index, List<List<Node>> layersList)
        {
            List<Node> inlayer = index - 1 < 0 ? new List<Node>() : layersList[index - 1];
            List<Node> outlayer = index + 1 >= layersList.Count ? new List<Node>() : layersList[index + 1];
            double probin = 1, probout = 1;
            double packageAllInCount = 0, packageLayerInCount = 0, packageAllOutCount = 0, packageLayerOutCount = 0;
            if (packageIn.ContainsKey(package))
            {
                packageAllInCount = packageIn[package].Count;
                foreach (Node node in inlayer)
                {
                    foreach (Package p in node.packages)
                    {
                        if (packageIn[package].Contains(p))
                        {
                            packageLayerInCount++;
                        }
                    }
                }
                probin = (packageLayerInCount+1) / packageAllInCount;
            }
            if (packageOut.ContainsKey(package))
            {
                packageAllOutCount = packageOut[package].Count;
                foreach (Node node in outlayer)
                {
                    foreach (Package p in node.packages)
                    {
                        if (packageOut[package].Contains(p))
                        {
                            packageLayerOutCount++;
                        }
                    }
                }
                probout = (packageLayerOutCount + 1) / packageAllOutCount;
            }
            return probin*probout;
        }
        public double calculatePackageProbability(Package package, int index, List<List<Node>> layersList)
        {
            double c = calculatePackageCategoryProbability(package, index, layersList);
            double d = calculatePackageDependProbability(package, index, layersList);
            return c;
        }
        public void insertPackagesToLayer(List<Package> packages, List<List<Node>> layersList)
        { 
            List<int> packageLayerindexes=new List<int>();
            foreach (Package package in packages)
            { 
                double maxprob=0;
                int maxprobindex = 0;
                for (int i = 0; i < layersList.Count; i++)
                {
                    if (calculatePackageProbability(package, i, layersList) > maxprob)
                    {
                        maxprob=calculatePackageProbability(package, i, layersList);
                        maxprobindex=i;
                    }
                }
                packageLayerindexes.Add(maxprobindex);
            }
            for (int i = 0; i < packages.Count; i++)
            {
                layersList[packageLayerindexes[i]].Add(singleNodeSet[packages[i]]);
            }
        }
        public void relayerCirclePackage(List<List<Node>> layersList)
        {
            List<Package> packages = new List<Package>();
            foreach (List<Node> list in layersList)
            {
                foreach (Node node in list)
                {
                    if (node.packages.Count > 1)
                    {
                        foreach (Package package in node.packages)
                        {
                            packages.Add(package);
                        }
                    }
                }
            }
            insertPackagesToLayer(packages, layersList);
        }

        public void genAlgorithmLayer(List<List<Node>> layerList)
        {
            for (int i = 0; i < layerList.Count; i++)
            {
                for (int j = 0; j < layerList[i].Count; j++)
                {
                    //if (layerList[i][j].packages.Count > 1) continue;
                    foreach (Package package in layerList[i][j].packages)
                    {
                        algorithmLayers[package] = i + 1;
                    }
                }
            }
        }
        public void genAlgorithmLayerV2(List<List<Node>> layerList)
        {
            for (int i = 0; i < layerList.Count; i++)
            {
                for (int j = 0; j < layerList[i].Count; j++)
                {
                    if (layerList[i][j].packages.Count > 1) continue;
                    foreach (Package package in layerList[i][j].packages)
                    {
                        algorithmLayers[package] = i + 1;
                    }
                }
            }
        }
        public void dfs()
        {
            foreach (Node node in nodeSet.Values)
            {
                node.color = 0;
                node.forward = null;
            }
            foreach (Node node in nodeSet.Values)
            {
                if (node.color == 0)
                    dfs_search(node);
            }
        }
        public void dfs_search(Node node)
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
        public void buildMap()
        {
            foreach (Node node in nodeSet.Values)
            {
                node.inNodes.Clear();
                node.outNodes.Clear();
                node.indirectInNodes.Clear();
                node.indirectOutNodes.Clear();
            }
            foreach (Node node in nodeSet.Values)
            {
                foreach (Package package in node.dependency)
                {
                    getNode(package).addInNode(node);
                    node.addOutNode(getNode(package));
                }
            }
        }
        public void buildMap(HashSet<Node> part)
        {
            foreach (Node node in part)
            {
                node.inNodes.Clear();
                node.outNodes.Clear();
                node.indirectInNodes.Clear();
                node.indirectOutNodes.Clear();
            }
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
        }
        public List<Node> generateTopoList()
        {
            buildMap();
            calculateIndirectInNodes();
            List<Node> list = new List<Node>();
            HashSet<Node> nodes = nodeSet.Values.ToHashSet();
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
                temp.Sort((a,b) =>
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
        public List<List<Node>> generateLayer(List<Node> topoList)
        {
            HashSet<Node> remain=topoList.ToHashSet();
            List<List<Node>> layers=new List<List<Node>>();
            List<Node> lastlayer=new List<Node>();
            buildMap(remain);
            int i;
            for (i = 0; i < topoList.Count; i++)
            {
                if (topoList[i].getOutNodeCount() == 0)
                {
                    remain.Remove(topoList[i]);
                    lastlayer.Add(topoList[i]);
                }
                else
                    break;
            }
            layers.Add(lastlayer);

            List<Node> layer = new List<Node>();
            while (i < topoList.Count)
            {
                layer.Add(topoList[i]);
                remain.Remove(topoList[i]);
                if (!isLayerDepend(remain, lastlayer))//keynode
                {
                    layers.Add(layer);
                    lastlayer = layer;
                    layer=new List<Node>();
                }
                i++;
            }
            return layers;
        }
        //remain是否依赖lastlayer
        private bool isLayerDepend(HashSet<Node> remain, List<Node> lastlayer)
        {
            foreach (Node node in remain)
            {
                foreach (Node dependency in node.outNodes)
                {
                    if(lastlayer.Contains(dependency))
                        return true;
                }
            }
            return false;
        }
        private int countLayerNodeDepend(HashSet<Node> remain, List<Node> lastlayer)
        {
            int count = 0;
            foreach (Node node in remain)
            {
                foreach (Node dependency in node.outNodes)
                {
                    if (lastlayer.Contains(dependency))
                        count++;
                }
            }
            return count;
        }
        private int countLayerPackageDepend(HashSet<Node> remain, List<Node> lastlayer)
        {
            int count = 0;
            foreach (Node node in remain)
            {
                foreach (Package dependency in node.dependency)
                {
                    if (lastlayer.Contains(getNode(dependency)))
                        count++;
                }
            }
            return count;
        }
        private void calculateIndirectInNodes()
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
        public List<List<Node>> generateLayerIn(List<Node> topoList)
        {
            HashSet<Node> remain = topoList.ToHashSet();
            List<List<Node>> layers = new List<List<Node>>();
            List<Node> lastlayer = new List<Node>();
            buildMap(remain);
            int i;
            for (i = 0; i < topoList.Count; i++)
            {
                if (topoList[i].getInNodeCount() == 0)
                {
                    remain.Remove(topoList[i]);
                    lastlayer.Add(topoList[i]);
                }
                else
                    break;
            }
            layers.Add(lastlayer);

            List<Node> layer = new List<Node>();
            while (i < topoList.Count)
            {
                layer.Add(topoList[i]);
                remain.Remove(topoList[i]);
                if (!isLayerDepend(lastlayer.ToHashSet(), remain.ToList()))//keynode
                {
                    layers.Add(layer);
                    lastlayer = layer;
                    layer = new List<Node>();
                }
                i++;
            }
            return layers;
        }


        private int calculateNodeMaxDepth()
        {
            //buildMap();
            HashSet<Node> nodes = nodeSet.Values.ToHashSet();
            Queue<Node> queue = new Queue<Node>();
            int maxDepth = 0;
            foreach (Node node in nodes)
            {
                if (node.getOutNodeCount() == 0)
                {
                    node.maxDepth = 0;
                    queue.Enqueue(node);
                }
            }
            while (queue.Count > 0)
            {
                int l = queue.Count;
                for (int i = 0; i < l; i++)
                {
                    Node node = queue.Dequeue();
                    maxDepth=Math.Max(maxDepth,node.maxDepth);
                    foreach (Node dependency in node.inNodes)
                    {
                        dependency.maxDepth=Math.Max(dependency.maxDepth,node.maxDepth+1);
                        queue.Enqueue(dependency);
                    }
                }
            }
            return maxDepth;
        }
        public List<List<Node>> generateLayerBasedMaxDepth()
        {
            buildMap();
            calculateIndirectInNodes();
            int maxDepth=calculateNodeMaxDepth();
            List<List<Node>> layers = new List<List<Node>>();
            List<Node> nodes = nodeSet.Values.ToHashSet().ToList();
            for (int i = 0; i <= maxDepth; i++)
            {
                List<Node> layer = new List<Node>();
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (nodes[j].maxDepth == i)
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
                layers.Add(layer);
            }
            return layers;
        }

        private void calculateIndirectInNodes(HashSet<Node> part)
        {
            HashSet<Node> nodes = part;
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
        public List<Node> generateTopoList(HashSet<Node> part)
        {
            buildMap(part);
            calculateIndirectInNodes(part);
            List<Node> list = new List<Node>();
            HashSet<Node> nodes = part;
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
                    if (a.getInNodeCount() == b.getInNodeCount())
                        return b.getIndirectInNodeCount().CompareTo(a.getIndirectInNodeCount());
                    else
                        return b.getInNodeCount().CompareTo(a.getInNodeCount());
                });

                //temp.Sort((a, b) =>
                //{
                //    return b.getIndirectInNodeCount().CompareTo(a.getIndirectInNodeCount());
                //});

                //temp.Sort((a, b) =>
                //{
                //    if (a.getInNodeCount() == b.getInNodeCount())
                //        return a.getIndirectInNodeCount().CompareTo(b.getIndirectInNodeCount());
                //    else
                //        return a.getInNodeCount().CompareTo(b.getInNodeCount());
                //});

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
        public List<List<Node>> mergeLayer(List<List<Node>> layers, int dependencyThreshold=0)
        {
            buildMap();
            calculateIndirectInNodes();
            if (layers.Count <= 1) return layers;
            List<Node> layer = layers[0];
            List<List<Node>> merged = new List<List<Node>>();
            for (int i = 1; i < layers.Count; i++)
            {
                Console.WriteLine(countLayerNodeDepend(layers[i].ToHashSet(), layer));
                if (countLayerNodeDepend(layers[i].ToHashSet(), layer)< dependencyThreshold)
                {
                    layer = layer.Concat(layers[i]).ToList();
                }
                else
                {
                    merged.Add(layer);
                    layer = layers[i];
                }
            }
            merged.Add(layer);
            return merged;
        }
        public List<List<Node>> mergeLayerWithoutZero(List<List<Node>> layers, int dependencyThreshold = 0)
        {
            buildMap();
            calculateIndirectInNodes();
            if (layers.Count <= 2) return layers;
            List<List<Node>> merged = new List<List<Node>>();
            merged.Add(layers[0]);
            List<Node> layer = layers[1];
            for (int i = 2; i < layers.Count; i++)
            {
                Console.WriteLine(countLayerNodeDepend(layers[i].ToHashSet(), layer));
                if (countLayerNodeDepend(layers[i].ToHashSet(), layer) < dependencyThreshold)
                {
                    layer = layer.Concat(layers[i]).ToList();
                }
                else
                {
                    merged.Add(layer);
                    layer = layers[i];
                }
            }
            merged.Add(layer);
            return merged;
        }
        public List<List<Node>> mergeLayerIn(List<List<Node>> layers, int dependencyThreshold = 0)
        {
            buildMap();
            calculateIndirectInNodes();
            if (layers.Count <= 1) return layers;
            List<Node> layer = layers[0];
            List<List<Node>> merged = new List<List<Node>>();
            for (int i = 1; i < layers.Count; i++)
            {
                Console.WriteLine(countLayerNodeDepend(layers[i].ToHashSet(), layer));
                if (countLayerPackageDepend(layer.ToHashSet(), layers[i]) < dependencyThreshold)
                {
                    layer = layer.Concat(layers[i]).ToList();
                }
                else
                {
                    merged.Add(layer);
                    layer = layers[i];
                }
            }
            merged.Add(layer);
            return merged;
        }
        public List<List<Node>> mergeLayerReverse(List<List<Node>> layers, int dependencyThreshold = 0)
        {
            buildMap();
            calculateIndirectInNodes();
            if (layers.Count <= 1) return layers;
            List<Node> layer = layers[layers.Count - 1];
            List<List<Node>> merged = new List<List<Node>>();
            for (int i = layers.Count-2; i >=0; i--)
            {
                Console.WriteLine(countLayerNodeDepend(layer.ToHashSet(), layers[i]));
                if (countLayerNodeDepend(layer.ToHashSet(), layers[i]) < dependencyThreshold)
                {
                    layer = layer.Concat(layers[i]).ToList();
                }
                else
                {
                    merged.Add(layer);
                    layer = layers[i];
                }
            }
            merged.Add(layer);
            merged.Reverse();
            return merged;
        }
        public List<List<Node>> mergeLayerv2Max(List<List<Node>> layers, int finalLayerCount = 4)
        {
            buildMap();
            calculateIndirectInNodes();
            List<List<Node>> merged = new List<List<Node>>();

            while (finalLayerCount < layers.Count)
            {
                int maxDependency = -1, maxCount = -1;
                for (int i = 1; i < layers.Count; i++)
                {
                    int count = countLayerNodeDepend(layers[i].ToHashSet(), layers[i - 1]);
                    if (count > maxCount)
                    {
                        maxDependency = i - 1;
                        maxCount = count;
                    }
                }
                if (maxDependency >= 0)
                {
                    layers[maxDependency] = layers[maxDependency].Concat(layers[maxDependency + 1]).ToList();
                    layers.RemoveAt(maxDependency + 1);
                }
            }
            return layers;
        }
        public List<List<Node>> mergeLayerv2MaxWithoutZero(List<List<Node>> layers, int finalLayerCount = 4)
        {
            buildMap();
            calculateIndirectInNodes();
            List<List<Node>> merged = new List<List<Node>>();

            while (finalLayerCount < layers.Count)
            {
                int maxDependency = -1, maxCount = -1;
                for (int i = 2; i < layers.Count; i++)
                {
                    int count = countLayerNodeDepend(layers[i].ToHashSet(), layers[i - 1]);
                    if (count > maxCount)
                    {
                        maxDependency = i - 1;
                        maxCount = count;
                    }
                }
                if (maxDependency >= 0)
                {
                    layers[maxDependency] = layers[maxDependency].Concat(layers[maxDependency + 1]).ToList();
                    layers.RemoveAt(maxDependency + 1);
                }
            }
            return layers;
        }
        public List<List<Node>> mergeLayerv2WithoutZero(List<List<Node>> layers, int finalLayerCount = 4)
        {
            buildMap();
            calculateIndirectInNodes();
            List<List<Node>> merged = new List<List<Node>>();

            while (finalLayerCount < layers.Count)
            {
                int minDependency = -1, minCount = int.MaxValue;
                for (int i = 2; i < layers.Count; i++)
                {
                    int count = countLayerNodeDepend(layers[i].ToHashSet(), layers[i - 1]);
                    if (count < minCount)
                    {
                        minDependency = i - 1;
                        minCount = count;
                    }
                }
                if (minDependency >= 0)
                {
                    layers[minDependency] = layers[minDependency].Concat(layers[minDependency + 1]).ToList();
                    layers.RemoveAt(minDependency + 1);
                }
            }
            return layers;
        }
        public List<List<Node>> mergeLayerv2(List<List<Node>> layers, int dependencyThreshold = 25)
        {
            buildMap();
            calculateIndirectInNodes();
            if (layers.Count <= 1) return layers;
            List<List<Node>> merged = new List<List<Node>>();
            int minDependency = -1, minCount = int.MaxValue;
            do
            {
                minDependency = -1;
                minCount = int.MaxValue;
                for (int i = 1; i < layers.Count; i++)
                {
                    int count = countLayerNodeDepend(layers[i].ToHashSet(), layers[i - 1]);
                    if (count < dependencyThreshold && count <minCount)
                    {
                        minDependency = i - 1;
                        minCount = count;  
                    }
                }
                if (minDependency >= 0)
                {
                    layers[minDependency] = layers[minDependency].Concat(layers[minDependency + 1]).ToList();
                    layers.RemoveAt(minDependency + 1);
                }
            }
            while (minDependency >= 0);
            return layers;
        }

        private bool isLayerDependv2(List<Node> layer, List<Node> lastlayer)
        {
            HashSet<Node> lastLayer = lastlayer.ToHashSet();
            HashSet<Node> lastUpper = lastlayer.ToHashSet();
            HashSet<Node> thisLayer = layer.ToHashSet();
            HashSet<Node> thisUpper = lastlayer.ToHashSet();
            foreach (Node node in lastLayer)
            {
                lastUpper.UnionWith(node.inNodes);
            }
            foreach (Node node in thisLayer)
            {
                thisUpper.UnionWith(node.inNodes);
            }
            if (lastUpper.Except(thisUpper).Except(thisLayer).ToList().Count == 0)
                return true;
            return false;
        }
        private bool isLayerDependv2In(List<Node> layer, List<Node> lastlayer)
        {
            HashSet<Node> lastLayer = lastlayer.ToHashSet();
            HashSet<Node> lastDependencies = new HashSet<Node>();
            HashSet<Node> thisLayer = layer.ToHashSet();
            HashSet<Node> thisDependencies = lastlayer.ToHashSet();
            foreach (Node node in lastLayer)
            {
                lastDependencies.UnionWith(node.outNodes);
            }
            foreach (Node node in thisLayer)
            {
                thisDependencies.UnionWith(node.outNodes);
            }
            if(lastDependencies.Except(thisDependencies).Except(thisLayer).ToList().Count==0)
                return true;
            return false;
        }
        public List<Node> generateTopoListIn(HashSet<Node> part)
        {
            buildMap(part);
            calculateIndirectInNodes(part);
            List<Node> list = new List<Node>();
            HashSet<Node> nodes = part;
            while (nodes.Count > 0)
            {
                List<Node> temp = new List<Node>();
                foreach (Node node in nodes)
                {
                    if (node.getInNodeCount() == 0)
                        temp.Add(node);
                }
                temp.Sort((a, b) =>
                {
                    if (a.getInNodeCount() == b.getInNodeCount())
                        return a.getIndirectInNodeCount().CompareTo(b.getIndirectInNodeCount());
                    else
                        return a.getInNodeCount().CompareTo(b.getInNodeCount());
                });
                foreach (Node node in temp)
                {
                    nodes.Remove(node);
                    list.Add(node);
                    foreach (Node dependency in node.outNodes)
                    {
                        dependency.inNodes.Remove(node);
                    }
                }
            }
            return list;
        }
        public List<List<Node>> generateNewLayers(List<Node> topoList)
        {
            buildMap(topoList.ToHashSet());
            calculateIndirectInNodes(topoList.ToHashSet());
            List<List<Node>> newLayers = new List<List<Node>>();
            List<Node> lastlayer = new List<Node>();
            int i;
            for (i = 0; i < topoList.Count; i++)
            {
                if (topoList[i].getOutNodeCount() == 0)
                {
                    lastlayer.Add(topoList[i]);
                }
                else
                    break;
            }
            newLayers.Add(lastlayer);

            List<Node> layer = new List<Node>();
            while (i < topoList.Count)
            {
                layer.Add(topoList[i]);
                if (isLayerDependv2(layer, lastlayer))
                {
                    newLayers.Add(layer);
                    lastlayer = layer;
                    layer = new List<Node>();
                }
                i++;
            }
            return newLayers;
        }
        public List<List<Node>> generateNewLayersIn(List<Node> topoList)
        {
            buildMap(topoList.ToHashSet());
            calculateIndirectInNodes(topoList.ToHashSet());
            List<List<Node>> newLayers = new List<List<Node>>();
            List<Node> lastlayer = new List<Node>();
            int i;
            for (i = 0; i < topoList.Count; i++)
            {
                if (topoList[i].getInNodeCount() == 0)
                {
                    lastlayer.Add(topoList[i]);
                }
                else
                    break;
            }
            newLayers.Add(lastlayer);

            List<Node> layer = new List<Node>();
            while (i < topoList.Count)
            {
                layer.Add(topoList[i]);
                if (isLayerDependv2In(layer, lastlayer))
                {
                    newLayers.Add(layer);
                    lastlayer = layer;
                    layer = new List<Node>();
                }
                i++;
            }
            return newLayers;
        }


        #region 废弃的方法
        //被依赖
        public List<List<Node>> generateInLinearList()
        {
            buildMap();
            HashSet<Node> nodes = nodeSet.Values.ToHashSet();
            Queue<Node> queue= new Queue<Node>();
            foreach (Node node in nodes)
            {
                node.indirectInNodes.UnionWith(node.inNodes);
                if(node.getInNodeCount()==0)
                    queue.Enqueue(node);
            }
            HashSet<Node> finished = new HashSet<Node>();
            List<HashSet<Node>> unsortedLayers= new List<HashSet<Node>>();
            while (queue.Count > 0)
            {
                unsortedLayers.Add(queue.ToHashSet());
                finished.UnionWith(queue.ToHashSet());
                int l = queue.Count;
                for (int i = 0; i < l; i++)
                {
                    Node node = queue.Dequeue();
                    foreach (Package package in node.dependency)
                    {
                        Node dependency = getNode(package);
                        if (finished.Contains(dependency))
                            continue;
                        queue.Enqueue(dependency);
                        dependency.indirectInNodes.UnionWith(node.indirectInNodes);
                    }
                }
            }
            List<List<Node>> sortedlinearList = new List<List<Node>>();
            foreach (HashSet<Node> layer in unsortedLayers)
            {
                sortedlinearList.Add(layer.ToList());
            }
            for (int i = 0; i < sortedlinearList.Count(); i++)
            {
                sortedlinearList[i].Sort((a, b) =>
                {
                    return a.getIndirectInNodeCount().CompareTo(b.getIndirectInNodeCount());
                    /*if (a.getInNodeNums() == b.getInNodeNums())
                        return a.getIndirectDependencyCount().CompareTo(b.getIndirectDependencyCount());
                    else
                        return a.getInNodeNums().CompareTo(b.getInNodeNums());*/
                });
            }

            return sortedlinearList;
        }
        
        public List<int> generateInKeyNode(List<Node> linearList)
        {
            List<int> keyNode = new List<int>();
            int p = 0;
            keyNode.Add(p);
            HashSet<Node> dependencies = new HashSet<Node>();
            while (p < linearList.Count && linearList[p].getInNodeCount() == 0)
            {
                foreach (Package package in linearList[p].dependency)
                    dependencies.Add(getNode(package));
                dependencies.Add(linearList[p]);
                p++;
            }
            while (p < linearList.Count)
            {
                keyNode.Add(p);
                HashSet<Node> nextDependencies = new HashSet<Node>();
                while (p < linearList.Count && dependencies.Contains(linearList[p]))
                {
                    foreach (Package package in linearList[p].dependency)
                        nextDependencies.Add(getNode(package));
                    p++;
                }
                dependencies.UnionWith(nextDependencies);
            }
            return keyNode;
        }
        //依赖
        public List<List<Node>> generateOutLinearList()
        {
            buildMap();
            HashSet<Node> nodes = nodeSet.Values.ToHashSet();
            Queue<Node> queue = new Queue<Node>();
            foreach (Node node in nodes)
            {
                if (node.getOutNodeCount() == 0)
                    queue.Enqueue(node);
            }
            HashSet<Node> finished = new HashSet<Node>();
            List<HashSet<Node>> unsortedLayers = new List<HashSet<Node>>();
            while (queue.Count > 0)
            {
                unsortedLayers.Add(queue.ToHashSet());
                finished.UnionWith(queue.ToHashSet());
                int l = queue.Count;
                for (int i = 0; i < l; i++)
                {
                    Node node = queue.Dequeue();
                    foreach (Node package in node.inNodes)
                    {
                        Node dependency = package;
                        if (finished.Contains(dependency))
                            continue;
                        queue.Enqueue(dependency);
                    }
                }
            }
            List<List<Node>> sortedlinearList = new List<List<Node>>();
            foreach (HashSet<Node> layer in unsortedLayers)
            {
                sortedlinearList.Add(layer.ToList());
            }
            for (int i = 0; i < sortedlinearList.Count(); i++)
            {
                sortedlinearList[i].Sort((a, b) =>
                {
                    return a.getIndirectInNodeCount().CompareTo(b.getIndirectInNodeCount());
                    /*if (a.getInNodeNums() == b.getInNodeNums())
                        return a.getIndirectDependencyCount().CompareTo(b.getIndirectDependencyCount());
                    else
                        return a.getInNodeNums().CompareTo(b.getInNodeNums());*/
                });
            }

            return sortedlinearList;
        }

        public List<int> generateOutKeyNode(List<Node> linearList)
        {
            List<int> keyNode = new List<int>();
            int p = 0;
            keyNode.Add(p);
            HashSet<Node> dependencies = new HashSet<Node>();
            while (p < linearList.Count && linearList[p].getOutNodeCount() == 0)
            {
                dependencies.Add(linearList[p]);
                p++;
            }
            while (p < linearList.Count)
            {
                keyNode.Add(p);
                HashSet<Node> nextDependencies = new HashSet<Node>();
                while (true)
                {
                    foreach (Package package in linearList[p].dependency)
                        nextDependencies.Add(getNode(package));
                    p++;
                }
                dependencies = nextDependencies;
            }
            return keyNode;
        }
        #endregion
    }
}
