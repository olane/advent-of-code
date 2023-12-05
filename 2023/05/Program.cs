

using System.Collections.Immutable;

var lines = File.ReadLines("testinput.txt").ToArray();

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
long FindLowestLocation(Dictionary<string, Map> maps, string currentProperty, IEnumerable<Range> ranges) {
    if (currentProperty == "location") {
        return ranges.Select(x => x.start).Min();
    }
    else {
        var map = maps[currentProperty];
        var newRanges = map.MapRanges(ranges);
        return FindLowestLocation(maps, map.to, newRanges);
    }
}

var seedRanges = seeds.Chunk(2).Take(1).Select(x => {
        var start = x.First();
        var end = start + x.Skip(1).First();
        return new Range(x.First(), x.Skip(1).First());
    });

var lowestLocation = FindLowestLocation(maps, "seed", seedRanges.ToList());


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

    
    internal IEnumerable<Range> MapRanges(IEnumerable<Range> ranges) {
        return ranges.SelectMany(MapRange);
    }

    internal IEnumerable<Range> MapRange(Range range)
    {
        foreach(var map in maps) {
            var mapRange = new Range(map.targetStart, map.targetStart + map.length - 1);
            var offset = map.targetStart - map.sourceStart;

            if(mapRange.start <= range.start && range.end <= mapRange.end) {
                // we can map in one go, range is fully contained in the map range
                return [range.Offset(offset)];
            }

            else if(mapRange.start <= range.start && range.end > mapRange.end) {
                // our range starts in this map but goes over the end
                var splitRanges = range.Split(mapRange.end + 1);
                
                return new [] {splitRanges[0].Offset(offset)}.Concat(MapRange(splitRanges[1]));
            }

            else if(mapRange.start > range.start && range.end <= mapRange.end) {
                // our range starts before this map but goes into it
                var splitRanges = range.Split(mapRange.start);

                return MapRange(splitRanges[0]).Concat(new [] {splitRanges[1].Offset(offset)});
            }

            else if(range.start <= mapRange.start && mapRange.end <= range.end) {
                // we overlap this range but go over both ends
                var splitRanges = range.Split(mapRange.start, mapRange.end);

                return MapRange(splitRanges[0]).Concat(new [] {splitRanges[1].Offset(offset)}).Concat(MapRange(splitRanges[2]));
            }
        }

        return [range];
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

record Range(long start, long end) {
    public Range[] Split(long startOfNewSecondRange) {
        return [new Range(start, startOfNewSecondRange - 1), new Range(startOfNewSecondRange, end)];
    }

    public Range[] Split(long startOfNewSecondRange, long startOfNewThirdRange) {
        return [new Range(start, startOfNewSecondRange - 1), new Range(startOfNewSecondRange, startOfNewThirdRange - 1), new Range(startOfNewThirdRange, end)];
    }
    
    public Range Offset(long offset) {
        return new Range(start + offset, end + offset);
    }
};
