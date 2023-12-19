
using System.Text.RegularExpressions;

var lines = File.ReadLines("input.txt");

var sensors = lines.Select(line =>
{
    var rg = new Regex("Sensor at x=(-?\\d+), y=(-?\\d+): closest beacon is at x=(-?\\d+), y=(-?\\d+)");
    var match = rg.Match(line);

    return (s: new Point(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)), b: new Point(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value)));
});

var interestingY = 2000000;

var maxX = sensors.SelectMany(s => new[] {s.b, s.s}).Max(p => p.x);
var minX = sensors.SelectMany(s => new[] {s.b, s.s}).Min(p => p.x);

var allPoints = Enumerable.Range(minX-1, maxX + 1).Select(x => new Point(x, interestingY)).ToArray();

var ruledOutCount = allPoints.Count(p => IsRuledOut(p, sensors));
Console.WriteLine(ruledOutCount);

bool IsRuledOut(Point p, IEnumerable<(Point s, Point b)> sensors)
{
    return sensors.Any(s => ClosestBeaconDistance(s) < ManhattanDistance(p, s.s));
}

int ClosestBeaconDistance((Point s, Point b) sensorInfo) => ManhattanDistance(sensorInfo.s, sensorInfo.b);
int ManhattanDistance(Point a, Point b) => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);

record Point(int x, int y);