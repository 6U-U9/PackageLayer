using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor.Steps
{
    public class RemoveKeyEdges : Step<List<List<Package>>, bool>
    {
        public override string StepDescription
        {
            get { return "Pre"; }
        }
        public override string DetailDescription
        {
            get { return "Remove Key Edges"; }
        }
        public override bool Process(List<List<Package>> input)
        {
            foreach (var edge in input)
            {
                edge[0].dependency.Remove(edge[1]);
                edge[1].dependent.Remove(edge[0]);
            }
            return true;
        }
    }
}