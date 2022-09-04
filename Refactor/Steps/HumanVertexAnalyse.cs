using Refactor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor.Steps
{
    public class HumanVertexAnalyse : Step<IEnumerable<Package>, Dictionary<Package,double>>
    {
        public override string StepDescription
        {
            get { return "Analyse"; }
        }
        public override string DetailDescription
        {
            get { return "Count Vertex Dependency Mean Layer"; }
        }
        public override string ChineseDescription
        {
            get { return $"计算点"; }
        }

        public HumanVertexAnalyse()
        {
        }
        public override Dictionary<Package, double> Process(IEnumerable<Package> input)
        {
            Dictionary<Package, double> vertex = new Dictionary<Package, double>();
            foreach (Package package in input)
            {
                double value = 0;
                int count = 0;
                foreach (Package dependency in package.dependency)
                {
                    value += dependency.human;
                    count++;
                }
                vertex[package] = value / count;
            }
            return vertex;
        }
    }
}
