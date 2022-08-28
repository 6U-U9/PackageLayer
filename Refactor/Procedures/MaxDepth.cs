using Refactor.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor.Procedures
{
    public class MaxDepth : Procedure
    {
        public string environment;
        public int direction;
        public string filepath;
        public string sheetname;

        LoadInput loadInput;
        BuildGraph buildGraph;
        MergeCircleNodes mergeCircleNodes;
        MaxDepthLayer maxDepthLayer;

        public MaxDepth(string environment, string outputPath)
        {
            this.environment = environment;
            this.filepath = outputPath + ".xlsx";
            this.sheetname = "最大深度算法";

            int direction = 1;

            loadInput = new LoadInput();
            buildGraph = new BuildGraph();
            mergeCircleNodes = new MergeCircleNodes();
            maxDepthLayer = new MaxDepthLayer(direction);
        }
        public override List<string> Description()
        {
            List<string> description = new List<string>
            {
                loadInput.ToString(),
                buildGraph.ToString(),
                mergeCircleNodes.ToString(),
                maxDepthLayer.ToString(),
            };
            return description;
        }

        public override void Execute()
        {
            Input input = new Input(environment);
            IEnumerable<Package> packages = loadInput.Process(input);
            Graph graph = buildGraph.Process(packages);
            Graph mergedGraph = mergeCircleNodes.Process(graph);
            Hierarchies hierarchies = maxDepthLayer.Process(mergedGraph);
            Output.HierarchiesOutput(filepath, sheetname, Description(), hierarchies);
        }
    }
}
