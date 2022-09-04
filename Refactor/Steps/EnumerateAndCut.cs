using Refactor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor.Steps
{
    public class EnumerateAndCut : Step<IEnumerable<Package>, Hierarchies>
    {
        public override string StepDescription
        {
            get { return "Main"; }
        }
        public override string DetailDescription
        {
            get { return "Enumerate Algorithm"; }
        }
        public override string ChineseDescription
        {
            get
            {
                return $"枚举算法";
            }
        }

        public int layersCount = 4;
        public int parallelEdgeWeight = 1;
        public int leapEdgeWeight = 3;
        public int reverseLayerWeight = 9;

        public EnumerateAndCut(int parallelEdgeWeight = 1, int leapEdgeWeight = 3, int reverseLayerWeight = 9, int layersCount = 4)
        {
            this.layersCount = layersCount;
            this.parallelEdgeWeight = parallelEdgeWeight;
            this.leapEdgeWeight = leapEdgeWeight;
            this.reverseLayerWeight = reverseLayerWeight;
        }

        private List<Package> packages;
        private Dictionary<Package,int> packagesLayers;
        private Hierarchies hierarchies;
        private int minCost;
        private int calculateCost()
        {
            int cost = 0;
            foreach (Package package in packages)
            {
                if (packagesLayers[package] == 0)
                    continue;
                foreach (Package dependency in package.dependency)
                {
                    if (packagesLayers[dependency] == 0)
                        continue;
                    if (packagesLayers[package] == packagesLayers[dependency])
                        cost += parallelEdgeWeight;
                    else if (packagesLayers[package] < packagesLayers[dependency])
                        cost += reverseLayerWeight;
                    else if (packagesLayers[package] - packagesLayers[dependency] > 1)
                        cost += leapEdgeWeight;
                }
            }
            return cost;
        }
        private void buildHierarchies()
        {
            Hierarchies hierarchies = new Hierarchies();
            for (int i = 1; i <= layersCount; i++)
            {
                Layer layer = new Layer();
                foreach (Package package in packages)
                {
                    if(packagesLayers[package]==i)
                        layer.AddNode(new Node(package));
                }
                hierarchies.AddLayer(layer);
            }
            this.hierarchies = hierarchies;
        }
        private void dfs(int index)
        {
            if (index >= packages.Count)
            {
                if (calculateCost() < minCost)
                {
                    minCost = calculateCost();
                    buildHierarchies();
                }
                return;
            }
            Package package = packages[index];
            for (int i = 1; i <= layersCount; i++)
            {
                packagesLayers[package] = i;
                if (calculateCost() > minCost)
                    break;
                dfs(index + 1);
            }
            packagesLayers[package] = 0;
        }
        public override Hierarchies Process(IEnumerable<Package> input)
        {
            packages = input.ToList();
            packages.Sort((a, b) => { return b.relationCount.CompareTo(a.relationCount); });
            packagesLayers = new();
            foreach (Package p in input)
                packagesLayers[p] = 0;
            minCost = 1200;
            dfs(0);
            return hierarchies;
        }
    }
}
