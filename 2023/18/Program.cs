
var input = File.ReadAllLines("input.txt");

var lines = input.Select(i =>
{
    var split = i.Split(" ");
    return new Line(Enum.Parse<Direction>(split[0]), int.Parse(split[1]));
}).ToArray();

Console.WriteLine(CountSize(lines));

var lines2 = input.Select(i =>
{
    var hexIndex = i.IndexOf("(") +1;
    var distanceStr = i.Substring(hexIndex+1, 5);
    var dirStr = i.Substring(hexIndex+6, 1);
    var dir = dirStr switch {
        "0" => Direction.R,
        "1" => Direction.D,
        "2" => Direction.L,
        "3" => Direction.U,
    };
    return new Line(dir, Convert.ToInt32("0x0" + distanceStr, 16));
}).ToArray();

Console.WriteLine(CountSize(lines2));


long CountSize(Line[] lines)
{
    var segments = WalkLines(lines).ToArray();

    long count = 0;
    for (int i = 0; i < segments.Length; i++)
    {
        // shoelace formula
        count += Determinant(segments[i].start, segments[i].end);
    }

    var sum = segments.Sum(s => s.Length());
    return (count + sum) / 2 + 1;
}

long Determinant(Point a, Point b) {
    return a.x * b.y - b.x * a.y;
}

IEnumerable<Segment> WalkLines(IEnumerable<Line> lines)
{
    var start = new Point(0, 0);

    foreach (var line in lines)
    {
        var next = start.Add(line.Length, line.Direction);
        yield return new Segment(start, next);
        start = next;
    }
}

record Segment(Point start, Point end) {
    public long Length() {
        if(start.x == end.x) {
            return Math.Abs(start.y - end.y);
        }
        else return Math.Abs(start.x - end.x);
    }
};

record Point(long x, long y)
{
    public Point Add(long length, Direction d)
    {
        return d switch
        {
            Direction.U => new Point(x, y - length),
            Direction.D => new Point(x, y + length),
            Direction.L => new Point(x - length, y),
            Direction.R => new Point(x + length, y),
        };
    }
};

record Line(Direction Direction, int Length);

enum Direction
{
    U,
    D,
    L,
    R
}