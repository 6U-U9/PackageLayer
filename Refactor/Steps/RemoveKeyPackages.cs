using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Steps
{
    [Obsolete]
    public class RemoveKeyPackages : Step<List<Package>, bool>
    {
        public override string StepDescription
        {
            get { return "Pre"; }
        }
        public override string DetailDescription
        {
            get { return "Remove Key Packages"; }
        }
        public override bool Process(List<Package> input)
        {
            foreach (var key in input)
            {
                Package.packages.Remove(key.name); 
                foreach (Package package in Package.packages.Values)
                {
                    package.dependency.Remove(key);
                    package.dependent.Remove(key);
                }
            }
            return true;
        }
    }
}