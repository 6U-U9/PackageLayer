using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor.Steps
{

    public class LoadInput : Step<Input, IEnumerable<Package>>
    {
        public override string StepDescription
        {
            get { return "Load"; }
        }
        public override string DetailDescription
        {
            get { return "Build Packages"; }
        }

        public override IEnumerable<Package> Process(Input input)
        {
            foreach (var edges in input.dependencies)
            {
                Package dependent = Package.Create(edges[0], input.humanLayers[edges[0]]);
                Package dependency = Package.Create(edges[1], input.humanLayers[edges[1]]);
                dependent.dependency.Add(dependency);
                dependency.dependent.Add(dependent);
            }
            return Package.packages.Values;
        }
    }
}