
using System.Globalization;

var input = File.ReadAllLines("example.txt");

var lines = input.Select(i =>
{
    var split = i.Split(" ");
    return new Line(Enum.Parse<Direction>(split[0]), int.Parse(split[1]));
}).ToArray();

Console.WriteLine(CountSize(lines));

var lines2 = input.Select(i =>
{
    var distanceStr = i.Substring(6, 5);
    var dirStr = i.Substring(11, 1);
    var dir = dirStr switch {
        "0" => Direction.R,
        "1" => Direction.D,
        "2" => Direction.L,
        "3" => Direction.U,
    };
    return new Line(dir, Convert.ToInt32("0x0" + distanceStr, 16));
}).ToArray();

Console.WriteLine(CountSize(lines2));


int CountSize(Line[] lines)
{
    var lineSegments = WalkLines(lines).ToArray();
    var minX = lineSegments.Select(s => s.start.x).Min();
    var minY = lineSegments.Select(s => s.start.y).Min();
    var maxX = lineSegments.Select(s => s.start.x).Max();
    var maxY = lineSegments.Select(s => s.start.y).Max();

    var (painted, width, height, xOffset, yOffset) = Paint(lineSegments);

    Flood(painted, (xOffset + 1, yOffset + 1), width, height, '#');

    return Count(painted, width, height, '#');
}

int Count(char[,] arr, int width, int height, char c)
{
    var count = 0;
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            if (arr[x, y] == c)
            {
                count++;
            }
        }
    }
    return count;
}

void Flood(char[,] arr, (int x, int y) initPos, int width, int height, char c)
{
    var q = new Queue<(int x, int y)>();
    q.Enqueue(initPos);

    while (q.Count != 0)
    {
        var (x, y) = q.Dequeue();

        if (y >= height) continue;
        if (y < 0) continue;
        if (x >= width) continue;
        if (x < 0) continue;
        if (arr[x, y] == c) continue;

        arr[x, y] = c;
        q.Enqueue((x - 1, y - 1));
        q.Enqueue((x - 1, y));
        q.Enqueue((x - 1, y + 1));
        q.Enqueue((x, y + 1));
        q.Enqueue((x, y - 1));
        q.Enqueue((x + 1, y - 1));
        q.Enqueue((x + 1, y));
        q.Enqueue((x + 1, y + 1));
    }
}


(char[,] arr, int width, int height, int xOffset, int yOffset) Paint(IEnumerable<Segment> lineSegments)
{
    var minX = lineSegments.Select(s => s.start.x).Min();
    var minY = lineSegments.Select(s => s.start.y).Min();
    var maxX = lineSegments.Select(s => s.start.x).Max();
    var maxY = lineSegments.Select(s => s.start.y).Max();

    var xTranslation = 0 - minX;
    var yTranslation = 0 - minY;

    var width = maxX + xTranslation + 1;
    var height = maxY + yTranslation + 1;

    var result = new char[width, height];
    for (int i = 0; i < width; i++)
    {
        for (int j = 0; j < height; j++)
        {
            result[i, j] = '.';
        }
    }

    foreach (var s in lineSegments)
    {
        if (s.start.x == s.end.x)
        {
            //vertical
            for (int y = Math.Min(s.start.y, s.end.y); y <= Math.Max(s.start.y, s.end.y); y++)
            {
                result[s.start.x + xTranslation, y + yTranslation] = '#';
            }
        }
        else
        {
            //horizontal
            for (int x = Math.Min(s.start.x, s.end.x); x <= Math.Max(s.start.x, s.end.x); x++)
            {
                result[x + xTranslation, s.start.y + yTranslation] = '#';
            }
        }
    }

    return (result, width, height, xTranslation, yTranslation);
}

bool PointInSegment(Segment segment, Point point)
{
    if (segment.start.x == point.x && segment.end.x == point.x)
    {
        return point.y >= Math.Min(segment.start.y, segment.end.y) && point.y <= Math.Max(segment.start.y, segment.end.y);
    }
    else if (segment.start.y == point.y && segment.end.y == point.y)
    {
        return point.x >= Math.Min(segment.start.x, segment.end.x) && point.x <= Math.Max(segment.start.x, segment.end.x);
    }
    return false;
}

bool PointInShape(IEnumerable<Segment> segments, Point point)
{
    var y = point.y;
    var verticalSegments = segments.Where(s => s.start.y != s.end.y);
    var relevantSegments = verticalSegments.Where(s => (s.start.y >= y && s.end.y <= y) || s.start.y <= y && s.end.y >= y);

    var maxX = relevantSegments.Select(s => s.start.x).Max();

    //go right, counting winding until we're out
    var x = point.x;
    var winding = 0;
    while (x <= maxX + 1)
    {
        var matchingUpSegment = relevantSegments.FirstOrDefault(s => s.start.x == x && s.start.y >= y && s.end.y <= y);
        var matchingDownSegment = relevantSegments.FirstOrDefault(s => s.start.x == x && s.start.y <= y && s.end.y >= y);

        if (matchingUpSegment != null && x != point.x)
        {
            winding++;
        }
        if (matchingDownSegment != null)
        {
            winding--;
        }

        x++;
    }

    return winding != 0;
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

static void Print(char[,] arr, int width, int height)
{
    for (int y = 0; y < height; y++)
    {
        var line = "";
        for (int x = 0; x < width; x++)
        {
            line += arr[x, y];
        }
        Console.WriteLine(line.Substring(0, 200));
    }
}

record Segment(Point start, Point end);

record Point(int x, int y)
{
    public Point Add(int length, Direction d)
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