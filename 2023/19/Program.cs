var lines = File.ReadAllLines("input.txt");

var workflowLines = lines.Where(l => !l.StartsWith("{"));
var itemLines = lines.Where(l => l.StartsWith("{"));

var items = itemLines.Select(PartFromString);

var workflows = WorkflowSet.FromLines(workflowLines);

var acceptedItems = items.Where(workflows.IsAcceptable);
Console.WriteLine(acceptedItems.Sum(i => i.Value()));

var partRange = new PartRange(
    makeRange(4000, true),
    makeRange(4000, true),
    makeRange(4000, true),
    makeRange(4000, true)
);

var filteredPartRange = workflows.ProcessRange(partRange);
Console.WriteLine(filteredPartRange.Sum(x => x.Value()));

bool[] makeRange(int n, bool def) {
    var range = new bool[n+1];
    Array.Fill(range, def);
    range[0] = false;
    return range;
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

record PartRange(bool[] x, bool[] m, bool[] a, bool[] s){
    internal long Value()
    {
        return (long)x.Count(v => v) * m.Count(v => v) * a.Count(v => v) * s.Count(v => v);
    }

    internal PartRange FilterX(Func<int, bool> comparer)
    {
        return new PartRange(
            x.Select((v, i) => v && comparer(i)).ToArray(),
            m,
            a,
            s
        );
    }
    
    internal PartRange FilterM(Func<int, bool> comparer)
    {
        return new PartRange(
            x,
            m.Select((v, i) => v && comparer(i)).ToArray(),
            a,
            s
        );
    }
    
    internal PartRange FilterA(Func<int, bool> comparer)
    {
        return new PartRange(
            x,
            m,
            a.Select((v, i) => v && comparer(i)).ToArray(),
            s
        );
    }
    
    internal PartRange FilterS(Func<int, bool> comparer)
    {
        return new PartRange(
            x,
            m,
            a,
            s.Select((v, i) => v && comparer(i)).ToArray()
        );
    }

    internal bool Exists()
    {
        return x.Any(v => v) && m.Any(v => v) && a.Any(v => v) && s.Any(v => v);
    }

    internal PartRange Invert()
    {
        return new PartRange(
            x.Select((v, i) => i != 0 && !v).ToArray(),
            m.Select((v, i) => i != 0 && !v).ToArray(),
            a.Select((v, i) => i != 0 && !v).ToArray(),
            s.Select((v, i) => i != 0 && !v).ToArray()
        );
    }

    internal PartRange Intersect(PartRange partRange)
    {
        throw new NotImplementedException();
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

    internal IEnumerable<PartRange> ProcessRange(PartRange partRange)
    {
        var currentRangesAndRules = new (PartRange r, string nextRule)[] {(partRange, "in")};

        while(true) {
            currentRangesAndRules = currentRangesAndRules.SelectMany(rnr => {
                if(rnr.nextRule == "A") {
                    return new []{rnr};
                } else if (rnr.nextRule == "R") {
                    return Array.Empty<(PartRange r, string nextRule)>();
                }

                return Workflows[rnr.nextRule].Apply(rnr.r);
            }).ToArray();

            if(currentRangesAndRules.All(rnr => rnr.nextRule == "A")) {
                return currentRangesAndRules.Where(rnr => rnr.nextRule == "A").Select(rnr => rnr.r);
            }
        }
    }

    private PartRange Combine(IEnumerable<PartRange> ranges)
    {
        return new PartRange(
            Enumerable.Range(0, 4000).Select(i => ranges.Any(r => r.x[i])).ToArray(),
            Enumerable.Range(0, 4000).Select(i => ranges.Any(r => r.m[i])).ToArray(),
            Enumerable.Range(0, 4000).Select(i => ranges.Any(r => r.a[i])).ToArray(),
            Enumerable.Range(0, 4000).Select(i => ranges.Any(r => r.s[i])).ToArray()
        );
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
    
    internal IEnumerable<(PartRange r, string nextRule)> Apply(PartRange partRange)
    {
        var currentRange = partRange;
        foreach(var rule in Rules) {
            var positiveRange = rule.Apply(currentRange);
            if(positiveRange.Exists()) {
                yield return (positiveRange, rule.positiveResult);
            }
            currentRange = rule.ApplyInverse(currentRange);
        }

        if(currentRange.Exists()) {
            yield return (currentRange, Fallback);
        }
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

    internal PartRange Apply(PartRange partRange)
    {
        Func<int, bool> comparer = comparison switch
        {
            '<' => v => v < comparisonValue,
            '>' => v => v > comparisonValue,
            _ => throw new NotImplementedException()
        };

        return attribute switch
        {
            'x' => partRange.FilterX(comparer),
            'm' => partRange.FilterM(comparer),
            'a' => partRange.FilterA(comparer),
            's' => partRange.FilterS(comparer),
            _ => throw new NotImplementedException(),
        };
    }

    internal PartRange ApplyInverse(PartRange partRange)
    {
        Func<int, bool> comparer = comparison switch
        {
            '<' => v => v >= comparisonValue,
            '>' => v => v <= comparisonValue,
            _ => throw new NotImplementedException()
        };

        return attribute switch
        {
            'x' => partRange.FilterX(comparer),
            'm' => partRange.FilterM(comparer),
            'a' => partRange.FilterA(comparer),
            's' => partRange.FilterS(comparer),
            _ => throw new NotImplementedException(),
        };
    }
}
