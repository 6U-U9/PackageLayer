using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Steps
{
    public class FindKeyEdges : Step<List<Package>, List<List<Package>>>
    {
        public override string StepDescription
        {
            get { return "Pre"; }
        }
        public override string DetailDescription
        {
            get { return "Find Key Edges of Circles"; }
        }
        public override string ChineseDescription
        {
            get { return $"计算边在多少环中 关键边阈值{threshold}"; }
        }

        public int threshold = 60000;
        public FindKeyEdges(int threshold)
        {
            this.threshold = threshold;
        }
        private Dictionary<Package, bool> started = new Dictionary<Package, bool>();
        private Dictionary<Package, bool> visited = new Dictionary<Package, bool>();
        private Dictionary<Package, Package> forward = new Dictionary<Package, Package>();
        private Dictionary<Package,Dictionary<Package,int>> circleEdgeCount = new();
        private void DfsCircleCount(List<Package> input)
        {
            foreach (Package package in input)
            {
                started[package] = false;
                visited[package] = false;
                forward[package] = Package.NullPackage;
                circleEdgeCount[package] = new Dictionary<Package, int>();
                foreach (Package dependency in package.dependency)
                    circleEdgeCount[package][dependency] = 0;
            }
            foreach (Package package in input)
            {
                //Console.WriteLine("----" + package.ToString() + "----");
                DfsSearchCircleCount(package, package);
                started[package] = true;
            }
        }
        private void DfsSearchCircleCount(Package package, Package start)
        {
            visited[package] = true;
            for (int i = 0; i < package.dependency.Count; i++)
            {
                Package next = package.dependency[i];
                if (!started.ContainsKey(next))
                    continue;
                if (visited[next] == false && started[next] == false)
                {
                    forward[next] = package;
                    DfsSearchCircleCount(next, start);
                }
                else if (next == start)
                {
                    string cs = "";
                    Package p = package;
                    Package prev = forward[package];
                    circleEdgeCount[package][next] ++;
                    while (p != start)
                    {
                        cs = p.ToString() + "; " + cs;
                        circleEdgeCount[prev][p] ++;
                        p = prev;
                        prev = forward[prev];
                    }
                    cs = start.ToString() + "; " + cs + "\n";
                    Console.WriteLine(cs);
                }
            }
            visited[package] = false;
        }
        public List<(Package, Package, int)> BuildEdges(List<Package> input)
        {
            List<(Package, Package, int)> edges = new();
            DfsCircleCount(input);
            foreach (var package in input)
            {
                foreach (var dependency in package.dependency)
                {
                    int c = 0;
                    if (started.ContainsKey(dependency))
                        c = circleEdgeCount[package][dependency];
                    edges.Add((package, dependency, c));
                }
            }
            edges.Sort((b, a) => { return a.Item3.CompareTo(b.Item3); });
            return edges;
        }
        public override List<List<Package>> Process(List<Package> input)
        {
            var edges = BuildEdges(input);
            List<List<Package>> keyEdges = new List<List<Package>>();
            for (int i = 0; i < edges.Count; i++)
            {
                if (edges[i].Item3 > threshold)
                {
                    keyEdges.Add(new List<Package> { edges[i].Item1, edges[i].Item2 });
                }
            }
            return keyEdges;
        }
    }
}
