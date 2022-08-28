using Refactor.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactor.Procedures
{
    public class Origin : Procedure
    {
        public string environment;
        public int direction;
        public string filepath = "test.xlsx";

        LoadInput loadInput = new LoadInput();
        BuildGraph buildGraph = new BuildGraph();
        MergeCircleNodes mergeCircleNodes = new MergeCircleNodes();
        BuildIndirectEdges buildIndirectEdges = new BuildIndirectEdges(); 
        GenerateTopoList generateTopoList = new GenerateTopoList(); 
        OriginalLayer OriginalLayer = new OriginalLayer();

        public Origin(string environment,string outputPath,int direction = 1)
        {
            this.environment = environment;
            this.direction = direction;
            this.filepath = outputPath + ".xlsx";
        }
        public override List<string> Description()
        {
            List<string> description = new List<string>
            {
                loadInput.ToString(),
                buildGraph.ToString(),
                mergeCircleNodes.ToString(),
                buildIndirectEdges.ToString(),
                generateTopoList.ToString(),
                OriginalLayer.ToString(),
            };
            return description;
        }

        public override void Execute()
        {
            Input input = new Input(environment);
            IEnumerable<Package> packages = loadInput.Process(input);
            Graph graph = buildGraph.Process(packages);
            Graph mergedGraph = mergeCircleNodes.Process(graph);
            buildIndirectEdges.Process(mergedGraph);
            List<Node> topolist = generateTopoList.Process(mergedGraph);
            Hierarchies hierarchies = OriginalLayer.Process(topolist);
            Output.HierarchiesOutput(filepath, "原始算法", Description(), hierarchies);
        }
    }
}
