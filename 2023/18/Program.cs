
var input = File.ReadAllLines("input.txt");

var lines = input.Select(i => {
    var split = i.Split(" ");
    return new Line(Enum.Parse<Direction>(split[0]), int.Parse(split[1]), split[2]);
}).ToArray();

var lineSegments = WalkLines(lines).ToArray();
var minX = lineSegments.Select(s => s.start.x).Min();
var minY = lineSegments.Select(s => s.start.y).Min();
var maxX = lineSegments.Select(s => s.start.x).Max();
var maxY = lineSegments.Select(s => s.start.y).Max();


//var test = PointInShape(lineSegments, new Point(1, 5));

var count = 0;
for (int y = minY; y <= maxY; y++)
{
    var line = "";
    for (int x = minX; x <= maxX; x++)
    {
        if(PointInShape(lineSegments, new Point(x, y)) || lineSegments.Any(s => PointInSegment(s, new Point(x, y)))) {
            count++;
            line += "#";
        }
        else {
            line += ".";
        }
    }
    Console.WriteLine(line[..100]);
}

Console.WriteLine(count);

bool PointInSegment(Segment segment, Point point) {
    if (segment.start.x == point.x && segment.end.x == point.x) {
        return point.y >= Math.Min(segment.start.y, segment.end.y) && point.y <= Math.Max(segment.start.y, segment.end.y);
    }
    else if (segment.start.y == point.y && segment.end.y == point.y) {
        return point.x >= Math.Min(segment.start.x, segment.end.x) && point.x <= Math.Max(segment.start.x, segment.end.x);
    }
    return false;
}

bool PointInShape(IEnumerable<Segment> segments, Point point) {
    var y = point.y;
    var verticalSegments = segments.Where(s => s.start.y != s.end.y);
    var relevantSegments = verticalSegments.Where(s => (s.start.y >= y && s.end.y <= y) || s.start.y <= y && s.end.y >= y);

    var maxX = relevantSegments.Select(s => s.start.x).Max();

    //go right, counting winding until we're out
    var x = point.x;
    var winding = 0;
    while(x <= maxX+1)
    {
        var matchingUpSegment = relevantSegments.FirstOrDefault(s => s.start.x == x && s.start.y >= y && s.end.y <= y);
        var matchingDownSegment = relevantSegments.FirstOrDefault(s => s.start.x == x && s.start.y <= y && s.end.y >= y);

        if(matchingUpSegment != null && x != point.x) {
            winding++;
        }
        if(matchingDownSegment != null) {
            winding--;
        }

        x++;
    }

    return winding != 0;
}

IEnumerable<Segment> WalkLines(IEnumerable<Line> lines)
{
    var start = new Point(0, 0);

    foreach(var line in lines) {
        var next = start.Add(line.Length, line.Direction);
        yield return new Segment(start, next);
        start = next;
    }
}

record Segment(Point start, Point end);

record Point(int x, int y) {
    public Point Add(int length, Direction d) {
        return d switch {
            Direction.U => new Point(x, y - length),
            Direction.D => new Point(x, y + length),
            Direction.L => new Point(x - length, y),
            Direction.R => new Point(x + length, y),
        };
    }
};

record Line(Direction Direction, int Length, string Colour);

enum Direction {
    U,
    D,
    L,
    R
}