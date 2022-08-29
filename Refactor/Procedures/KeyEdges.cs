using Refactor.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Procedures;

public class KeyEdges : Procedure
{
    public string environment;
    public string filepath;
    public string sheetname;

    public int threshold;

    LoadInput loadInput;
    private FindKeyEdges findKeyEdges;

    public KeyEdges(string environment, string outputPath)
    {
        this.environment = environment;
        this.filepath = outputPath + ".xlsx";
        this.sheetname = "边在环中的计数";

        this.threshold = 60000;

        loadInput = new LoadInput();
        findKeyEdges = new FindKeyEdges(threshold);
    }
    public override List<string> Description()
    {
        List<string> description = new List<string>
        {
            loadInput.ToString(),
            findKeyEdges.ToString(),
        };
        return description;
    }

    public override void Execute()
    {
        Input input = new Input(environment);
        IEnumerable<Package> packages = loadInput.Process(input);
        List<(Package,Package,int)> edges = findKeyEdges.BuildEdges(packages.ToList());
        Output.EdgesOutput(filepath, sheetname, Description(), edges);
    }
}