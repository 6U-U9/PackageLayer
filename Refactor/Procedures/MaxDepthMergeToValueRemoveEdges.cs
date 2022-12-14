using Refactor.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Procedures
{
    public class MaxDepthMergeToValueRemoveEdges : Procedure
    {
        public string environment;
        public string filepath;
        public string sheetname;

        Input input;
        LoadInputAndRemove loadInput;
        BuildGraph buildGraph;
        MergeCircleNodes mergeCircleNodes;
        MaxDepthLayer maxDepthLayer;
        MergeLayerToCertainCount mergeLayer;

        public MaxDepthMergeToValueRemoveEdges(string environment, string outputPath)
        {
            this.environment = environment;
            this.filepath = outputPath + ".xlsx";
            this.sheetname = "最大深度算法";

            int direction = 1;
            List<string> removePackages =new List<string>()
            {
                
            };
            List<(string, string)> removeEdges=new List<(string,string)>()
            {
                ("glibc","libselinux"),
            };

            input = new Input(environment);
            loadInput = new LoadInputAndRemove(removePackages, removeEdges);
            buildGraph = new BuildGraph();
            mergeCircleNodes = new MergeCircleNodes();
            maxDepthLayer = new MaxDepthLayer(direction);
            mergeLayer = new MergeLayerToCertainCount(4, direction, 0, 0, 0, 1);
        }
        public override List<string> Description()
        {
            List<string> description = new List<string>
            {
                loadInput.ToString(),
                buildGraph.ToString(),
                mergeCircleNodes.ToString(),
                maxDepthLayer.ToString(),
                mergeLayer.ToString(),
            };
            return description;
        }

        public override void Execute()
        {
            IEnumerable<Package> packages = loadInput.Process(input);
            Graph graph = buildGraph.Process(packages);
            Graph mergedGraph = mergeCircleNodes.Process(graph);
            Hierarchies hierarchies = maxDepthLayer.Process(mergedGraph);
            Hierarchies mergedhierarchies = mergeLayer.Process(hierarchies);
            Output.HierarchiesOutput(filepath, sheetname, Description(), mergedhierarchies);
        }
    }
}
