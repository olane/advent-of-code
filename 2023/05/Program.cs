

var lines = File.ReadLines("input.txt").ToArray();

var seeds = lines[0].Substring(7).Split(" ").Select(long.Parse);

var maps = GetMaps(lines.Skip(2));

IEnumerable<Map> GetMaps(IEnumerable<string> lines)
{
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
            yield return new Map(from, to, maps);
        }
        else {
            var numbers = line.Split(" ").Select(long.Parse).ToArray();
            maps.Add(new RangeMap(numbers[0], numbers[1], numbers[2]));
        }
    }
    
    yield return new Map(from, to, maps);
}

long FindLocation(IEnumerable<Map> maps, string currentProperty, long id) {
    if (currentProperty == "location") {
        return id;
    }
    else {
        var map = maps.First(x => x.from == currentProperty);
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

var seedRanges = seeds.Chunk(2);
var seeds2 = seedRanges.SelectMany(x => rangeEnumerator(x.First(), x.Skip(1).First()));
var seed2Locations = seeds2.Select(x => FindLocation(maps, "seed", x));
Console.WriteLine(seed2Locations.Min());


//records
record Map(string from, string to, List<RangeMap> maps)
{
    internal long DoMap(long id)
    {
        var matchingRange = maps.SingleOrDefault(x => x.targetStart <= id && id < x.targetStart + x.length);
        if (matchingRange != null) {
            var offset = id - matchingRange.targetStart;
            return matchingRange.sourceStart + offset;
        }
        return id;
    }
}


record RangeMap(long sourceStart, long targetStart, long length);
