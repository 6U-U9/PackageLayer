using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Steps
{

    public class LoadInputAndRemove : Step<Input, IEnumerable<Package>>
    {
        public override string StepDescription
        {
            get { return "Load"; }
        }
        public override string DetailDescription
        {
            get { return "Build Packages"; }
        }
        public override string ChineseDescription
        {
            get
            {
                string ps = "";
                foreach (string name in removePackages)
                    ps += name + "; ";
                string es = "";
                foreach (var e in removeEdges)
                    es += e.Item1+" -> "+e.Item2 + "; ";
                return $"读取输入并移除 点：{ps} 边：{es}";
            }
        }

        public List<string> removePackages;
        public List<(string, string)> removeEdges;
        
        public LoadInputAndRemove(List<string> removePackages, List<(string, string)> removeEdges)
        {
            this.removePackages = removePackages;
            this.removeEdges = removeEdges;
        }
        
        public override IEnumerable<Package> Process(Input input)
        {
            foreach (var edges in input.dependencies)
            {
                if (removePackages.Contains(edges[0]) || removePackages.Contains(edges[1]))
                    continue;
                if (removeEdges.Contains((edges[0],edges[1]))) 
                    continue;
                
                Package dependent = Package.Create(edges[0], input.humanLayers[edges[0]]);
                Package dependency = Package.Create(edges[1], input.humanLayers[edges[1]]);
                dependent.dependency.Add(dependency);
                dependency.dependent.Add(dependent);
            }
            return Package.packages.Values;
        }
    }
}