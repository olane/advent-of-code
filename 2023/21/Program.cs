
var lines = File.ReadAllLines("input.txt");

var steps = 26501365;

var field = new Field(lines);

var startLine = lines.First(x => x.Contains('S'));
var startY = Array.IndexOf(lines, startLine);
var startX = startLine.IndexOf('S');
var start = new Location(startX, startY);

Console.WriteLine(LocationCount(field, 65));
Console.WriteLine(LocationCount(field, 65 + 131));
Console.WriteLine(LocationCount(field, 65 + 131*2));
// this is a pure quadratic, extrapolate it using wolfram alpha (these are x= 0,1,2 and you need x = 201300 because 26501365 = (201300 * 131) + 65, and 131 is the period of the pattern)


int LocationCount(Field field, int steps) {
    List<Location> locations = [start];
    for (int i = 1; i <= steps; i++)
    {
        locations = locations.SelectMany(loc => GetNextLocations(field, loc)).Distinct().ToList();
    }
    return locations.Count;
}


IEnumerable<Location> GetNextLocations(Field field, Location loc)
{
    Location[] locs = [
        new(loc.X+1, loc.Y),
        new(loc.X-1, loc.Y),
        new(loc.X, loc.Y+1),
        new(loc.X, loc.Y-1),
    ];

    return locs.Where(l => field.Get(l) != '#');
}

record struct Location(int X, int Y);

class Field {
    public char[,] Values {get; private set;}
    public int Width { get; }
    public int Height { get; }

    public Field(IEnumerable<string> lines) {
        Width = lines.First().Length;
        Height = lines.Count();
        Values = new char[Width, Height];

        var arr = lines.ToArray();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Values[x,y] = arr[y][x];
            }
        }
    }

    public void Print() {
        for (int y = 0; y < Height; y++)
        {
            var line = "";
            for (int x = 0; x < Width; x++)
            {
                line += Values[x,y];
            }
            Console.WriteLine(line);
        }
    }

    public int Get(Location loc) => Values[(loc.X % Width + Width) % Width, (loc.Y % Height + Height) % Height];
}

enum Direction
{
    Up,
    Down,
    Left,
    Right
}
