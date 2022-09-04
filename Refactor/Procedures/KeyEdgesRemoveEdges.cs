using Refactor.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Procedures;

public class KeyEdgesRemoveEdges : Procedure
{
    public string environment;
    public string filepath;
    public string sheetname;

    public int threshold;

    Input input;
    LoadInputAndRemove loadInput;
    FindKeyEdges findKeyEdges;

    public KeyEdgesRemoveEdges(string environment, string outputPath)
    {
        this.environment = environment;
        this.filepath = outputPath + ".xlsx";
        this.sheetname = "边在环中的计数";

        this.threshold = 60000;
        List<string> removePackages = new List<string>()
        {

        };
        List<(string, string)> removeEdges = new List<(string, string)>()
            {
                ("glibc","libselinux"),
            };

        input = new Input(environment);
        loadInput = new LoadInputAndRemove(removePackages, removeEdges);
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
        IEnumerable<Package> packages = loadInput.Process(input);
        List<(Package,Package,int)> edges = findKeyEdges.BuildEdges(packages.ToList());
        Output.EdgesOutput(filepath, sheetname, Description(), edges);
    }
}