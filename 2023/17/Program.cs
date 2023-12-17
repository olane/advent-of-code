
var lines = File.ReadAllLines("input.txt");

var field = new Field(lines);

field.Print();

var paths = GetPaths(new Location(field.Width-1, field.Height-1), field, new Path([new(new Location(0, 0), Direction.Right)]));
var pathCosts = paths.Select(p => Cost(p, field));

Console.WriteLine(pathCosts.Max());

int Cost(Path p, Field field) => p.Locations.Sum(l => field.Get(l.Location));

IEnumerable<Path> GetPaths(Location end, Field field, Path pathSoFar)
{
    if(pathSoFar.Locations.Last().Location == end) {
        return [pathSoFar];
    }

    var nexts = GetPossibleNextLocations(pathSoFar, field);

    var newPaths = nexts.Select(n => {
        var newPath = pathSoFar.Copy();
        newPath.Locations.Add(n);
        return newPath;
    });

    return newPaths.SelectMany(p => GetPaths(end, field, pathSoFar));
}

IEnumerable<PathLocation> GetPossibleNextLocations(Path path, Field field)
{
    var possibleNexts = new List<PathLocation>();

    var lastLoc = path.Locations.Last();
    var last3Dirs = path.Locations.Skip(Math.Max(0, path.Locations.Count - 3)).Select(l => l.Dir);
    if(!last3Dirs.All(d => d == last3Dirs.First())) {
        // we've not gone the same way 3 times in a row, so we can go straight on
        possibleNexts.Add(Go(lastLoc, lastLoc.Dir));
    }
    possibleNexts.Add(TurnLeft(lastLoc));
    possibleNexts.Add(TurnRight(lastLoc));

    return possibleNexts
        .Where(l => l.Location.X >= 0 && l.Location.Y >= 0 && l.Location.X < field.Width && l.Location.Y < field.Height)
        .Where(l => !path.Locations.Any(pl => pl.Location == l.Location));
}

PathLocation Go(PathLocation lastLoc, Direction dir)
{
    return dir switch
    {
        Direction.Up => new PathLocation(new(lastLoc.Location.X, lastLoc.Location.Y-1), dir),
        Direction.Down => new PathLocation(new(lastLoc.Location.X, lastLoc.Location.Y+1), dir),
        Direction.Left => new PathLocation(new(lastLoc.Location.X-1, lastLoc.Location.Y), dir),
        Direction.Right => new PathLocation(new(lastLoc.Location.X+1, lastLoc.Location.Y), dir),
    };
}

PathLocation TurnLeft(PathLocation lastLoc) {
    return lastLoc.Dir switch {
        Direction.Up => Go(lastLoc, Direction.Left),
        Direction.Down => Go(lastLoc, Direction.Right),
        Direction.Left => Go(lastLoc, Direction.Down),
        Direction.Right => Go(lastLoc, Direction.Up),
    };
}

PathLocation TurnRight(PathLocation lastLoc) {
    return lastLoc.Dir switch {
        Direction.Up => Go(lastLoc, Direction.Right),
        Direction.Down => Go(lastLoc, Direction.Left),
        Direction.Left => Go(lastLoc, Direction.Up),
        Direction.Right => Go(lastLoc, Direction.Down),
    };
}

record Path(List<PathLocation> Locations){
    public Path Copy() {
        var locs = new List<PathLocation>(Locations);
        return new Path(locs);
    }
};

record struct PathLocation(Location Location, Direction Dir);

record struct Location(int X, int Y);

class Field {
    public int[,] Values {get; private set;}
    public int Width { get; }
    public int Height { get; }

    public Field(IEnumerable<string> lines) {
        Width = lines.First().Length;
        Height = lines.Count();
        Values = new int[Width, Height];

        var arr = lines.ToArray();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Values[x,y] = int.Parse(arr[y][x].ToString());
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

    public int Get(Location loc) => Values[loc.X, loc.Y];
}

enum Direction
{
    Up,
    Down,
    Left,
    Right
}
