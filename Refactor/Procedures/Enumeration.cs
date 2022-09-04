using Refactor.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refactor.Core;

namespace Refactor.Procedures;

public class Enumeration : Procedure
{
    public string environment;
    public string filepath;
    public string sheetname;

    Input input;
    LoadInput loadInput;
    EnumerateAndCut enumerateAndCut;

    public Enumeration(string environment, string outputPath)
    {
        this.environment = environment;
        this.filepath = outputPath + ".xlsx";
        this.sheetname = "枚举";

        input = new Input(environment);
        loadInput = new LoadInput();
        enumerateAndCut = new EnumerateAndCut();
    }
    public override List<string> Description()
    {
        List<string> description = new List<string>
        {
            loadInput.ToString(),
            enumerateAndCut.ToString(),
        };
        return description;
    }

    public override void Execute()
    {
        IEnumerable<Package> packages = loadInput.Process(input);
        Hierarchies hierarchies = enumerateAndCut.Process(packages);
        Output.HierarchiesOutput(filepath, sheetname, Description(), hierarchies);
    }
}