var lines = File.ReadAllLines("input.txt");


Dictionary<string, List<string>> connectedTo = new();
Dictionary<string, List<string>> connectedToSparse = new();

foreach (var line in lines)
{
    var parts = line.Split(": ");
    var from = parts[0];
    var to = parts[1].Split(" ");

    AddConnection(connectedTo, from, to);
    AddConnection(connectedToSparse, from, to);
    foreach (var t in to)
    {
        AddConnection(connectedTo, t, [from]);
    }
}

var allNodes = connectedTo.Keys;
var sparseConnections = connectedToSparse.SelectMany(kv => kv.Value.Select(to => (kv.Key, to))).ToArray();

for (int i = sparseConnections.Length-2; i >= 0; i--)
{
    for (int j = i + 1; j < sparseConnections.Length-1; j++)
    {
        for (int k = j + 1; k < sparseConnections.Length; k++)
        {
            (string from, string to)[] ignored = [sparseConnections[i], sparseConnections[j], sparseConnections[k]];

            var thisGroups = FindGroups(connectedTo, ignored);
            if(thisGroups.Count == 2) {
                
                Console.WriteLine(thisGroups.First().Count * thisGroups.Last().Count);
            }
        }
    }
}

var groups = FindGroups(connectedTo, []);

Console.WriteLine(groups.First().Count());


static List<HashSet<string>> FindGroups(Dictionary<string, List<string>> connectedTo, (string from, string to)[] ignored)
{
    List<HashSet<string>> groups = connectedTo.Keys.Select(x => new HashSet<string>() { x }).ToList();

    foreach (var (from, tos) in connectedTo)
    {
        foreach (var to in tos)
        {
            if (ignored.Any(i => i.from == from && i.to == to || i.from == to && i.to == from)) {
                continue;
            }

            // if to and from are in different groups, merge them

            var toGroup = groups.Single(g => g.Contains(to));
            var fromGroup = groups.Single(g => g.Contains(from));

            if (toGroup != fromGroup) {
                var merged = toGroup.Union(fromGroup).ToHashSet();
                groups.Remove(toGroup);
                groups.Remove(fromGroup);
                groups.Add(merged);
            }
        }
    }

    return groups;
}

void AddConnection(IDictionary<string, List<string>> connections, string from, string[] to)
{
    if (connections.ContainsKey(from))
    {
        connections[from].AddRange(to);
    }
    else
    {
        connections[from] = [.. to];
    }
}