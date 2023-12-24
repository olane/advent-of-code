
var lines = File.ReadAllLines("example.txt");

var field = new Field(lines);

field.Print();

var cost = FindMinimalPath(new PathLocation(new (0, 0), Direction.Down), new Location(field.Width-1, field.Height-1), field);
Console.WriteLine(cost);

int FindMinimalPath(PathLocation start, Location end, Field field) {
    var distances = new int[field.Width, field.Height];
    var visited = new bool[field.Width, field.Height];
    for (int i = 0; i < field.Width; i++)
    {
        for (int j = 0; j < field.Height; j++)
        { 
            distances[i, j] = int.MaxValue;
            visited[i, j] = false;
        }
    }

    var queue = new PriorityQueue<PathLocation, int>();
    queue.Enqueue(start, 0);

    distances[start.Location.X, start.Location.Y] = 0;

    while(queue.Count > 0) {
        var current = queue.Dequeue();
        
        if(current.Location == end) {
            Print(distances, field.Width, field.Height);
            return distances[current.Location.X, current.Location.Y];
        }

        var nexts = GetPossibleNextLocations(current, field);
        foreach (var n in nexts)
        {
            // if (visited[n.pl.Location.X, n.pl.Location.Y]) {
            //     continue;
            // }

            // visit
            visited[n.pl.Location.X, n.pl.Location.Y] = true;
            var cost = distances[current.Location.X, current.Location.Y] + n.cost;

            if(cost < distances[n.pl.Location.X, n.pl.Location.Y]) {
                distances[n.pl.Location.X, n.pl.Location.Y] = cost;
                queue.Enqueue(n.pl, cost);
            }
        }
    }

    return -1;
}


IEnumerable<(PathLocation pl, int cost)> GetPossibleNextLocations(PathLocation current, Field field)
{
    var left = TurnLeft(current.Dir);
    var right = TurnRight(current.Dir);

    return Go(current, left, 3, field).Concat(Go(current, right, 3, field));
}

PathLocation Go1(PathLocation lastLoc, Direction dir)
{
    return dir switch
    {
        Direction.Up => new PathLocation(new(lastLoc.Location.X, lastLoc.Location.Y-1), dir),
        Direction.Down => new PathLocation(new(lastLoc.Location.X, lastLoc.Location.Y+1), dir),
        Direction.Left => new PathLocation(new(lastLoc.Location.X-1, lastLoc.Location.Y), dir),
        Direction.Right => new PathLocation(new(lastLoc.Location.X+1, lastLoc.Location.Y), dir),
    };
}


IEnumerable<(PathLocation pl, int cost)> Go(PathLocation lastLoc, Direction dir, int n, Field field)
{
    var cost = 0;
    var pl = lastLoc;
    for (int i = 0; i < n; i++)
    {
        pl = Go1(pl, dir);
        if(pl.Location.X >= 0 && pl.Location.Y >= 0 && pl.Location.X < field.Width && pl.Location.Y < field.Height) {
            cost += field.Get(pl.Location);
            yield return(pl, cost);
        }
        else {
            break;
        }
    }
}

Direction TurnLeft(Direction dir) {
    return dir switch {
        Direction.Up => Direction.Left,
        Direction.Down => Direction.Right,
        Direction.Left => Direction.Down,
        Direction.Right => Direction.Up,
    };
}

Direction TurnRight(Direction dir) {
    return dir switch {
        Direction.Up => Direction.Right,
        Direction.Down => Direction.Left,
        Direction.Left => Direction.Up,
        Direction.Right => Direction.Down,
    };
}


 void Print(int[,] arr, int Width, int Height) {
    for (int y = 0; y < Height; y++)
    {
        var line = "";
        for (int x = 0; x < Width; x++)
        {
            line += arr[x,y].ToString().PadLeft(6);
        }
        Console.WriteLine(line);
    }
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
