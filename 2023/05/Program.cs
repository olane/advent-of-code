

using System.Collections.Immutable;

var lines = File.ReadLines("input.txt").ToArray();

var seeds = lines[0].Substring(7).Split(" ").Select(long.Parse).ToArray();

var maps = GetMaps(lines.Skip(2));

Dictionary<string, Map> GetMaps(IEnumerable<string> lines)
{
    var result = new Dictionary<string, Map>();

    string from = "";
    string to = "";
    var maps = new List<RangeMap>();

    foreach(var line in lines) {
        if (line.EndsWith("map:")) {
            from = line.Split(" ")[0].Split("-to-")[0];
            to = line.Split(" ")[0].Split("-to-")[1];
            maps = [];
        }
        else if (string.IsNullOrWhiteSpace(line)){
            result[from] = new Map(from, to, maps.ToImmutableSortedSet(new RangeMapComp()));
        }
        else {
            var numbers = line.Split(" ").Select(long.Parse).ToArray();
            maps.Add(new RangeMap(numbers[0], numbers[1], numbers[2]));
        }
    }
    
    result[from] = new Map(from, to, maps.ToImmutableSortedSet(new RangeMapComp()));

    return result;
}

long FindLocation(Dictionary<string, Map> maps, string currentProperty, long id) {
    if (currentProperty == "location") {
        return id;
    }
    else {
        var map = maps[currentProperty];
        var newId = map.DoMap(id);
        return FindLocation(maps, map.to, newId);
    }
}


//part1
var seedLocations = seeds.Select(x => FindLocation(maps, "seed", x));
Console.WriteLine(seedLocations.Min());


//part2
IEnumerable<long> rangeEnumerator(long start, long extent) {
    for(long i = start; i < start + extent; i ++) {
        yield return i;
    }
}

var seedRanges = seeds.Chunk(2).Take(1);
var seeds2 = seedRanges.SelectMany(x => rangeEnumerator(x.First(), x.Skip(1).First()));
var seed2Locations = seeds2.Select(x => FindLocation(maps, "seed", x));
Console.WriteLine(seed2Locations.Min());


//records
record Map(string from, string to, ImmutableSortedSet<RangeMap> maps)
{
    internal long DoMap(long id)
    {
        RangeMap? matchingRange = GetRange(id);
        if (matchingRange != null)
        {
            var offset = id - matchingRange.targetStart;
            return matchingRange.sourceStart + offset;
        }
        return id;
    }

    private RangeMap? GetRange(long id)
    {
        foreach(var map in maps) {
            if (map.targetStart <= id && id < map.targetStart + map.length) {
                return map;
            }
            if (id >= map.targetStart + map.length) {
                return null;
            }
        }
        return null;
    }
}

internal class RangeMapComp : IComparer<RangeMap>
{
    public int Compare(RangeMap x, RangeMap y)
    {
        return y.targetStart.CompareTo(x.targetStart);
    }
}


record RangeMap(long sourceStart, long targetStart, long length);

record Range(long start, long end);
