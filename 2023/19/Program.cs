var lines = File.ReadAllLines("input.txt");

var workflowLines = lines.Where(l => !l.StartsWith("{"));
var itemLines = lines.Where(l => l.StartsWith("{"));

var items = itemLines.Select(PartFromString);

var workflows = WorkflowSet.FromLines(workflowLines);


var acceptedItems = items.Where(workflows.IsAcceptable);
Console.WriteLine(acceptedItems.Sum(i => i.Value()));

var allItems = GenerateItems();
Console.WriteLine(allItems.LongCount(workflows.IsAcceptable));

IEnumerable<Part> GenerateItems()
{
    for (int i = 0; i < 4000; i++)
    {
        for (int j = 0; j < 4000; j++)
        {
            for (int k = 0; k < 4000; k++)
            {
                for (int l = 0; l < 4000; l++)
                {
                    yield return new Part(i, j, k, l);
                }
            }
            Console.Write(".");
        }
        Console.WriteLine("###");
    }
}

Part PartFromString(string str) {
    var parts = str[1..^1].Split(",");
    return new Part(
        int.Parse(parts[0][2..]),
        int.Parse(parts[1][2..]),
        int.Parse(parts[2][2..]),
        int.Parse(parts[3][2..])
    );
}

record Part(int x, int m, int a, int s)
{
    internal int Value()
    {
        return x + m + a + s;
    }
}

internal class WorkflowSet 
{
    public Dictionary<string, Workflow> Workflows {get; private set;}

    public WorkflowSet(IEnumerable<Workflow> workflows) {
        Workflows = workflows.ToDictionary(w =>
        {
            if(w.Label == null) {
                throw new Exception();
            }
            return w.Label;
        });
    }
    
    public static WorkflowSet FromLines(IEnumerable<string> workflowLines) {
        return new WorkflowSet(workflowLines.Select(l => new Workflow(l)).ToArray());
    }

    public bool IsAcceptable(Part part) {
        var currentRule = "in";
        while(true) {
            currentRule = Workflows[currentRule].Apply(part);
            if(currentRule == "R") {
                return false;
            }
            if(currentRule == "A") {
                return true;
            }
        }
    }
}

internal class Workflow
{
    public string Label {get; private set;}
    public Rule[] Rules {get; private set;}
    public string Fallback {get; private set;}

    public Workflow(string str)
    {
        var parts = str.Split("{");
        Label = parts[0] ?? throw new Exception("bad format");
        var rulesStrs = parts[1][..^1].Split(",");

        Fallback = rulesStrs[^1] ?? throw new Exception("bad format");

        Rules = rulesStrs[0..^1].Select(ruleStr => {
            var comparison = ruleStr.Contains('<') ? '<' : '>';
            var split = ruleStr.Split(comparison, ':');
            return new Rule(split[0][0], comparison, int.Parse(split[1]), split[2]);
        }).ToArray();

        if(Rules.All(r => r.positiveResult == Fallback)) {
            Rules = Array.Empty<Rule>();
        }
    }

    internal string Apply(Part part)
    {
        foreach(var rule in Rules) {
            if (rule.Apply(part)) {
                return rule.positiveResult;
            }
        }

        return Fallback;
    }
}

record Rule(char attribute, char comparison, int comparisonValue, string positiveResult)
{
    internal bool Apply(Part part)
    {
        var value = attribute switch
        {
            'x' => part.x,
            'm' => part.m,
            'a' => part.a,
            's' => part.s,
            _ => throw new NotImplementedException(),
        };

        return comparison switch
        {
            '<' => value < comparisonValue,
            '>' => value > comparisonValue,
            _ => throw new NotImplementedException()
        };
    }
}
