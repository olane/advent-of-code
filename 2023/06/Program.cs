var lines = File.ReadLines("input.txt").ToArray();

var times = lines[0].Substring(11).Chunk(7).Select(x => int.Parse(x)).ToArray();
var distances = lines[1].Substring(11).Chunk(7).Select(x => int.Parse(x)).ToArray();

var races = new List<Race>();
for (int i = 0; i < times.Count(); i++)
{
    races.Add(new Race(times[i], distances[i]));
}

int CountWaysToWin(Race race) {
    var count = 0;
    for (int holdDownTime = 0; holdDownTime <= race.time; holdDownTime++)
    {
        var speed = holdDownTime;
        var distance = speed * (race.time - holdDownTime);

        if (distance > race.distanceRecord) {
            count++;
        }
    }
    return count;
}

//part1
Console.WriteLine(races.Select(CountWaysToWin).Aggregate((a, b) => a*b));


//part2
var longRace = new Race(
    long.Parse(lines[0].Substring(11).Replace(" ", "")),
    long.Parse(lines[1].Substring(11).Replace(" ", ""))
);

Console.WriteLine(CountWaysToWin(longRace));

record Race(long time, long distanceRecord);