
var lines = File.ReadAllLines("input.txt");
var field = new Field(lines);

Part1();
Part2();

void Part1() {
    char[] target = ['X', 'M', 'A', 'S'];
    var count = 0;

    foreach (var startingLoc in field.GetAllLocations()) {

        foreach (var direction in GetDirectionVectors()) {
            Vector currentLocation = startingLoc;
            foreach(char c in target) {
                if (!field.IsInField(currentLocation) || field.Get(currentLocation) != c) {
                    count--;
                    break;
                }
                // else we continue with this direction
                currentLocation = AddVectors(currentLocation, direction);
            }
            count++;
        }
    }

    Console.WriteLine(count);
}

void Part2() {
    var count = 0;

    foreach (var startingLoc in field.GetAllLocations()) {
        if (field.Get(startingLoc) != 'A'){
            continue;
        }

        if (startingLoc.X == 0 || startingLoc.Y == 0 || startingLoc.X == field.Width -1 || startingLoc.Y == field.Height - 1) {
            continue;
        }

        var topLeft = AddVectors(startingLoc, new(-1, -1));
        var topRight = AddVectors(startingLoc, new(1, -1));
        var bottomLeft = AddVectors(startingLoc, new(-1, 1));
        var bottomRight = AddVectors(startingLoc, new(1, 1));

        char[] diagonals = [field.Get(topLeft), field.Get(topRight), field.Get(bottomLeft), field.Get(bottomRight)];

        if(diagonals.Count(x => x == 'M') == 2 && diagonals.Count(x => x == 'S') == 2 && diagonals[0] != diagonals[3]) {
            count++;
        }

    }

    Console.WriteLine(count);
}

Vector AddVectors(Vector v1, Vector v2) {
    return new Vector(v1.X + v2.X, v1.Y + v2.Y);
}

IEnumerable<Vector> GetDirectionVectors()
{
    Vector[] dirs = [
        new(-1, -1),
        new(-1, 0),
        new(-1, 1),
        new(0, -1),
        new(0, 1),
        new(1, -1),
        new(1, 0),
        new(1, 1),
    ];

    return dirs;
}

record struct Vector(int X, int Y);

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

    public IEnumerable<Vector> GetAllLocations() {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                yield return new Vector(x, y);
            }
        }
    }

    public bool IsInField(Vector loc) {
        return loc.X >= 0 && loc.Y >= 0 && loc.X < Width && loc.Y < Height;
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

    public char Get(Vector loc) => Values[(loc.X % Width + Width) % Width, (loc.Y % Height + Height) % Height];
}

