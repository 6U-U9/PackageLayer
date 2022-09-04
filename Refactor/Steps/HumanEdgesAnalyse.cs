using Refactor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor.Steps
{
    public class HumanEdgesAnalyse : Step<IEnumerable<Package>, Dictionary<(Package, Package),int>>
    {
        public override string StepDescription
        {
            get { return "Analyse"; }
        }
        public override string DetailDescription
        {
            get { return "Count Edge Catagories"; }
        }
        public override string ChineseDescription
        {
            get { return $"计算不同类别边数量"; }
        }

        public HumanEdgesAnalyse()
        {
        }
        public override Dictionary<(Package, Package), int> Process(IEnumerable<Package> input)
        {
            Dictionary<(Package, Package), int> edges = new Dictionary<(Package, Package), int>();
            foreach (Package package in input)
            {
                foreach (Package dependency in package.dependency)
                {
                    if (package.human == null || dependency.human == null)
                    {
                        Console.WriteLine($"ERROR {package.name} OR {dependency.name} do not have human layer result");
                        continue;
                    }
                    edges[(package,dependency)]= (int)package.human - (int)dependency.human;
                }
            }
            return edges;
        }
    }
}
