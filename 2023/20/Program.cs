var input = File.ReadAllLines("input.txt");

var switchboard = SwitchBoard.FromInputs(input);


IEnumerable<Pulse> list = new List<Pulse>();
for (int i = 0; i < 1000; i++)
{
    list = list.Concat(switchboard.PressButton());
}

Console.WriteLine(list.LongCount(p => p.High) * list.LongCount(p => !p.High));

// var loop = switchboard.FindLoop();
// var loopList = loop.SelectMany(x => x);
// var loopValue = loopList.LongCount(p => p.High) * loopList.LongCount(p => !p.High);
// Console.WriteLine(loopValue * 1000 / loop.Count);

class SwitchBoard(string[] Broadcast, Dictionary<string, IModule> Modules) {

    public List<List<Pulse>> FindLoop(int maxLength = 1000) {
        List<List<Pulse>> pulseSets = [];

        for (int i = 0; i < maxLength; i++)
        {
            var thisSet = PressButton();
            
            var index = pulseSets.FindIndex(pulseSet => pulseSet.SequenceEqual(thisSet));
            if(index > -1) {
                return pulseSets;
            }

            pulseSets.Add(thisSet);
        }

        return pulseSets;
    }

    public List<Pulse> PressButton() {
        var queue = new Queue<Pulse>(Broadcast.Select(p => new Pulse("broadcast", p, false)));
        return [new Pulse("button", "broadcast", false), .. ProcessPulseQueue(queue)];
    }

    private List<Pulse> ProcessPulseQueue(Queue<Pulse> pulseQueue) {
        List<Pulse> result = [];

        while(pulseQueue.TryDequeue(out var pulse)) {
            result.Add(pulse);
            if(!Modules.TryGetValue(pulse.To, out var target)) {
                continue;
            }

            var newPulses = target.Trigger(pulse);
            foreach (var p in newPulses)
            {
                pulseQueue.Enqueue(p);
            }
        }

        return result;
    }


    public static SwitchBoard FromInputs(string[] inputs) {
        List<IModule> moduleList = [];
        string[] broadcast = [];

        foreach(var line in inputs) {
            var parts = line.Split(" -> ");
            var name = parts[0];
            var targets = parts[1].Split(", ");

            if(name == "broadcaster") {
                broadcast = targets;
            }
            else if(name.StartsWith('%')) {
                moduleList.Add(new FlipFlop(name[1..], targets, false));
            }
            else if(name.StartsWith('&')) {
                var connections = inputs.Count(l => l.Contains(name[1..])) - 1;
                moduleList.Add(new Conjunction(name[1..], targets, connections));
            }
        }
        
        return new SwitchBoard(broadcast, moduleList.ToDictionary(x => x.Label));
    }
}

internal record Pulse(string From, string To, bool High);

interface IModule: IEquatable<IModule> {
    string Label {get;}
    string[] Targets {get;}
    IEnumerable<Pulse> Trigger(Pulse incoming);
};

sealed class FlipFlop(string label, string[] targets, bool on) : IModule
{
    public string Label { get; } = label;

    public string[] Targets { get; } = targets;

    private bool On = on;

    public IEnumerable<Pulse> Trigger(Pulse incoming)
    {
        if(incoming.High) {
            return [];
        }

        On = !On;
        return Targets.Select(t => new Pulse(Label, t, On));
    }

    public bool Equals(IModule? o){
        if (o is not FlipFlop other)
        {
            return false;
        }

        return other.On == On && other.Label == Label && other.Targets.SequenceEqual(Targets);
    }
}

sealed record Conjunction(string label, string[] targets, int connections) : IModule
{
    public string Label { get; } = label;

    public string[] Targets { get; } = targets;

    Dictionary<string, bool> HighInputs = [];

    public IEnumerable<Pulse> Trigger(Pulse incoming)
    {
        HighInputs[incoming.From] = incoming.High;

        var allHigh = HighInputs.Count == connections && HighInputs.Values.All(x => x);

        return Targets.Select(t => new Pulse(Label, t, !allHigh)).ToArray();
    }

    public bool Equals(IModule? o){
        var other = o as Conjunction;
        if(other == null){
            return false;
        }

        foreach (var item in HighInputs)
        {
            if(!other.HighInputs[item.Key] == item.Value){
                return false;
            }
        }
        return other.Label == Label && other.Targets.SequenceEqual(Targets);
    }
}
