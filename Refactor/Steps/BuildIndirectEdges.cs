using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Steps
{
    public class BuildIndirectEdges : Step<Graph, bool>
    {
        public override string StepDescription
        {
            get { return "Build"; }
        }
        public override string DetailDescription
        {
            get { return "Build indirect edges"; }
        }
        public override string ChineseDescription
        {
            get { return $"计算间接依赖数 间接层数：{length}"; }
        }

        public int length = -1; // -1 for iterate until end
        public BuildIndirectEdges(int length = -1)
        {
            this.length = length;
        }
        public override bool Process(Graph input)
        {
            if (length == 0) 
                throw new ArgumentOutOfRangeException("length cannot be 0");

            HashSet<Node> nodes = input.nodeSet.Values.ToHashSet();
            foreach (Node node in nodes)
            {
                {
                    Queue<Node> dependencyQueue = new Queue<Node>();
                    foreach (Node dependency in node.dependencies)
                    {
                        dependencyQueue.Enqueue(dependency);
                    }
                    int step = 0;
                    while (true)
                    {
                        step++;
                        int c = dependencyQueue.Count;
                        for (int i = 0; i < c; i++)
                        {
                            Node dependency = dependencyQueue.Dequeue();
                            if (node.indirectDependencies.Contains(dependency))
                                continue;
                            node.indirectDependencies.Add(dependency);
                            foreach (Node indirect in dependency.dependencies)
                                dependencyQueue.Enqueue(indirect);
                        }
                        if (dependencyQueue.Count == 0)
                            break;
                        if (step == length)
                            break;
                    }
                }
                {
                    Queue<Node> dependentQueue = new Queue<Node>();
                    foreach (Node dependent in node.dependents)
                    {
                        dependentQueue.Enqueue(dependent);
                    }
                    int step = 0;
                    while (true)
                    {
                        step++;
                        int c = dependentQueue.Count;
                        for (int i = 0; i < c; i++)
                        {
                            Node dependent = dependentQueue.Dequeue();
                            if (node.indirectDependents.Contains(dependent))
                                continue;
                            node.indirectDependents.Add(dependent);
                            foreach (Node indirect in dependent.dependents)
                                dependentQueue.Enqueue(indirect);
                        }
                        if (dependentQueue.Count == 0)
                            break;
                        if (step == length)
                            break;
                    }
                }
            }
            return true;
        }
    }
}
