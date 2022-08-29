using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Steps
{
    public class BuildGraph : Step<IEnumerable<Package>, Graph>
    {
        public override string StepDescription
        {
            get { return "Load"; }
        }
        public override string DetailDescription
        {
            get { return "Build Graph"; }
        }

        public override Graph Process(IEnumerable<Package> input)
        {
            Graph graph = new Graph();
            foreach (var package in input)
            {
                graph.AddNode(package);
            }
            graph.BuildEdges();
            return graph;
        }
    }
}