using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Steps
{

    public class GenerateTopoList : Step<Graph, List<Node>>
    {
        public override string StepDescription
        {
            get { return "Main"; }
        }
        public override string DetailDescription
        {
            get
            {
                Dictionary<int, string> directionDescriptions;
                Dictionary<int, string> methodDescriptions;
                directionDescriptions = new Dictionary<int, string>()
                {
                    {0, "Bottom - Up" },
                    {1, "Top - Down" },
                };
                methodDescriptions = new Dictionary<int, string>()
                {
                    {0, "First compare indegree then indirect indegree in descending order" },
                    {1, "First compare indegree then indirect indegree in ascending order" },
                    {2, "Compare indirect indegree in descending order" },
                    {3, "Compare indirect indegree in ascending order" },
                };
                return string.Format("Generate nodes topo list, direction :{0}, sorted by {1}", directionDescriptions[direction], methodDescriptions[methodIndex]);
            }
        }
        public override string ChineseDescription
        {
            get
            {
                Dictionary<int, string> directionDescriptions;
                Dictionary<int, string> methodDescriptions;
                directionDescriptions = new Dictionary<int, string>()
                {
                    {0, "Bottom - Up" },
                    {1, "Top - Down" },
                };
                methodDescriptions = new Dictionary<int, string>()
                {
                    {0, "First compare indegree then indirect indegree in descending order" },
                    {1, "First compare indegree then indirect indegree in ascending order" },
                    {2, "Compare indirect indegree in descending order" },
                    {3, "Compare indirect indegree in ascending order" },
                };
                return $"生成拓扑序列 算法方向：{directionDescriptions[direction]}";
            }
        }

        public int direction = 0; // 0: build from bottom; 1: build from top
        public int methodIndex = 0;
        public Dictionary<int, IComparer<Node>> methods;
        
        public GenerateTopoList(int direction = 1, int methodIndex = 0)
        {
            this.direction = direction;
            this.methodIndex = methodIndex;
            methods = new Dictionary<int, IComparer<Node>>()
            {
                {0,new NodeComparerIndegreeThenIndirectDescend(direction)},
                {1,new NodeComparerIndegreeThenIndirectAscend(direction)},
                {2,new NodeComparerIndirectIndegreeDescend(direction)},
                {3,new NodeComparerIndegreeThenIndirectAscend(direction)},
            };
        }
        public override List<Node> Process(Graph input)
        {
            List<Node> list = new List<Node>();
            HashSet<Node> nodes = input.nodeSet.Values.ToHashSet();
            while (nodes.Count > 0)
            {
                List<Node> temp = new List<Node>();
                foreach (Node node in nodes)
                {
                    if (node.GetOutDegree(direction) == 0)
                        temp.Add(node);
                }
                temp.Sort(methods[methodIndex]);
                foreach (Node node in temp)
                {
                    nodes.Remove(node);
                    list.Add(node);
                    foreach (Node dependent in node.dependents)
                    {
                        dependent.dependencies.Remove(node);
                    }
                }
            }
            input.BuildEdges();
            return list;
        }
        public class NodeComparerIndegreeThenIndirectDescend : IComparer<Node>
        {
            string description = "First compare indegree then indirect indegree in descending order";
            int direction = 0;
            public NodeComparerIndegreeThenIndirectDescend(int direction)
            {
                this.direction = direction;
            }
            public int Compare(Node? a, Node? b)
            {
                if (a == null || b == null)
                    throw new ArgumentNullException("Comparing null node");
                if (a.GetInDegree(direction) == b.GetInDegree(direction))
                    return b.GetIndirectInDegree(direction).CompareTo(a.GetIndirectInDegree(direction));
                else
                    return b.GetInDegree(direction).CompareTo(a.GetInDegree(direction));
            }
        }
        public class NodeComparerIndegreeThenIndirectAscend : IComparer<Node>
        {
            string description = "First compare indegree then indirect indegree in ascending order";
            int direction = 0;
            public NodeComparerIndegreeThenIndirectAscend(int direction)
            {
                this.direction = direction;
            }
            public int Compare(Node? a, Node? b)
            {
                if (a == null || b == null)
                    throw new ArgumentNullException("Comparing null node");
                if (a.GetInDegree(direction) == b.GetInDegree(direction))
                    return a.GetIndirectInDegree(direction).CompareTo(b.GetIndirectInDegree(direction));
                else
                    return a.GetInDegree(direction).CompareTo(b.GetInDegree(direction));
            }
        }
        public class NodeComparerIndirectIndegreeDescend : IComparer<Node>
        {
            string description = "Compare indirect indegree in descending order";
            int direction = 0;
            public NodeComparerIndirectIndegreeDescend(int direction)
            {
                this.direction = direction;
            }
            public int Compare(Node? a, Node? b)
            {
                if (a == null || b == null)
                    throw new ArgumentNullException("Comparing null node");
                return b.GetIndirectInDegree(direction).CompareTo(a.GetIndirectInDegree(direction));
            }
        }
        public class NodeComparerIndirectIndegreeAscend : IComparer<Node>
        {
            string description = "Compare indirect indegree in ascending order";
            int direction = 0;
            public NodeComparerIndirectIndegreeAscend(int direction)
            {
                this.direction = direction;
            }
            public int Compare(Node? a, Node? b)
            {
                if (a == null || b == null)
                    throw new ArgumentNullException("Comparing null node");
                return a.GetIndirectInDegree(direction).CompareTo(b.GetIndirectInDegree(direction));
            }
        }
    }
}