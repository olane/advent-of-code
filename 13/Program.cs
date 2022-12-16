var lines = File.ReadLines("input.txt");

var linePairs = lines
    .Select((line, i) => (line: line, groupIdx: i / 3))
    .GroupBy(x => x.groupIdx)
    .Select(g => g.Take(2).Select(x => x.line));

var packetPairs = linePairs.Select(pair => pair.Select(line => new Packet(line)).ToArray()).ToArray();

var comparisonScores = packetPairs.Select(pair => pair.First().CompareTo(pair[1])).ToArray();

var indexes = comparisonScores.Select((comparisonScore, i) =>
{
    if (comparisonScore <= 0)
    {
        return i + 1;
    }
    else {
        return 0;
    }
}).Where(x => x > 0);

Console.WriteLine(indexes.Sum());

var allPackets = packetPairs.SelectMany(x => x).Concat(new [] { new Packet("[[2]]"), new Packet("[[6]]")});
var sorted = allPackets.OrderBy(x => x).Select(x => x._contentString).ToArray();
Console.WriteLine((Array.IndexOf(sorted, "[[2]]") + 1) * (Array.IndexOf(sorted, "[[6]]") + 1));



interface IPacketItem : IComparable<IPacketItem> { };

class PacketInt : IPacketItem
{
    public int Contents { get; }

    public PacketInt(string item)
    {
        Contents = int.Parse(item);
    }

    public PacketInt(int item)
    {
        Contents = item;
    }

    public int CompareTo(IPacketItem? other)
    {
        return other switch
        {
            PacketInt i => Contents.CompareTo(i.Contents),
            PacketList l => new PacketList(Contents).CompareTo(l),
            _ => throw new Exception()
        };
    }
}

class PacketList : IPacketItem  {
    public IPacketItem[] Contents { get; }

    public PacketList(string input) {
        var listString = input.Substring(1, input.Length - 2);

        var split = splitStringList(listString);

        Contents = split.Select(item =>
        {
            IPacketItem result;
            if(item.StartsWith("[")) {
                result = new PacketList(item);
            }
            else {
                result = new PacketInt(item);
            }
            return result;
        }).ToArray();
    }

    public PacketList(int input) {
        Contents = new[] { new PacketInt(input) };
    }

    private IEnumerable<string> splitStringList(string list) {
        var thisItem = string.Empty;
        var depth = 0;
        foreach (char c in list)
        {
            if(c == ',' && depth == 0) {
                yield return thisItem;
                thisItem = string.Empty;
            }
            else {
                thisItem += c;
                if(c == '[') {
                    depth++;
                }
                else if (c == ']') {
                    depth--;
                }
            }
        }
        if(thisItem.Any()) {
            yield return thisItem;
        }
    }

    public int CompareTo(IPacketItem? other)
    {
        return other switch
        {
            PacketInt i => -i.CompareTo(this),
            PacketList l => CompareLists(this, l),
            _ => throw new Exception()
        };
    }

    private static int CompareLists(PacketList listA, PacketList listB)
    {
        var pairs = listA.Contents.Zip(listB.Contents);
        
        foreach (var (a, b) in pairs)
        {
            if(a == null) {
                return -1;
            }
            else if (b == null) {
                return 1;
            }

            var comparison = a.CompareTo(b);
            if (comparison != 0) {
                return comparison;
            }
        }

        return listA.Contents.Count().CompareTo(listB.Contents.Count());
    }
}

internal class Packet : IComparable<Packet>
{
    public string _contentString { get; }
    public PacketList Contents { get; }

    public Packet(string line)
    {
        _contentString = line;
        Contents = new PacketList(line);
    }

    public int CompareTo(Packet? other)
    {
        if (other == null) return 1;

        return this.Contents.CompareTo(other.Contents);
    }
}