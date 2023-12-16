var lines = File.ReadLines("input.txt");

var field = new Field(lines);

var initLoc = (x:0, y:0);
var initDir = Direction.Right;

Console.WriteLine(GetFieldEnergisation(field, initLoc, initDir));


var topLocs = Enumerable.Range(0, field.Width).Select(i => ((x:i, y:0), Direction.Down));
var bottomLocs = Enumerable.Range(0, field.Width).Select(i => ((x:i, y:field.Height-1), Direction.Up));
var leftLocs = Enumerable.Range(0, field.Height).Select(i => ((x:0, y:i), Direction.Right));
var rightLocs = Enumerable.Range(0, field.Height).Select(i => ((x:field.Width-1, y:i), Direction.Left));

var allLocs = topLocs.Concat(bottomLocs).Concat(leftLocs).Concat(rightLocs);
var max = allLocs.Max(l => GetFieldEnergisation(field, l.Item1, l.Item2));
Console.WriteLine(max);

int GetFieldEnergisation(Field field, (int x, int y) initLoc, Direction initDir) {
    var beams = new BeamField(field.Width, field.Height);
    PropagateBeam(field, beams, initLoc, initDir);
    return beams.CountEnergized();
}

void PropagateBeam(Field field, BeamField beamField, (int x, int y) loc, Direction dir) {
    if(loc.x < 0 || loc.y < 0 || loc.x >= field.Width || loc.y >= field.Height) {
        // we're off the edge, return
        return;
    }

    if (beamField.Get(loc).Has(dir)) {
        // we've done this path, return
        return;
    }
    beamField.Set(loc, dir);

    var fieldLoc = field.Get(loc);
    if(fieldLoc == '/') {
        var newDir = dir switch
        {
            Direction.Up => Direction.Right,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Down,
            Direction.Right => Direction.Up,
        };
        var nextLoc = GetNext(loc, newDir);
        PropagateBeam(field, beamField, nextLoc, newDir);
    }
    else if (fieldLoc == '\\') {
        var newDir = dir switch
        {
            Direction.Up => Direction.Left,
            Direction.Down => Direction.Right,
            Direction.Left => Direction.Up,
            Direction.Right => Direction.Down,
        };
        var nextLoc = GetNext(loc, newDir);
        PropagateBeam(field, beamField, nextLoc, newDir);
    }
    else if (fieldLoc == '-') {
        if (dir == Direction.Left || dir == Direction.Right) {
            var newDir = dir;
            var nextLoc = GetNext(loc, newDir);
            PropagateBeam(field, beamField, nextLoc, newDir);
        }
        else {
            var leftCase = GetNext(loc, Direction.Left);
            var rightCase = GetNext(loc, Direction.Right);
            PropagateBeam(field, beamField, leftCase, Direction.Left);
            PropagateBeam(field, beamField, rightCase, Direction.Right);
        }
    }
    else if (fieldLoc == '|') {
        if (dir == Direction.Up || dir == Direction.Down) {
            var newDir = dir;
            var nextLoc = GetNext(loc, newDir);
            PropagateBeam(field, beamField, nextLoc, newDir);
        }
        else {
            var upCase = GetNext(loc, Direction.Up);
            var downCase = GetNext(loc, Direction.Down);
            PropagateBeam(field, beamField, upCase, Direction.Up);
            PropagateBeam(field, beamField, downCase, Direction.Down);
        }
    }
    else {
        // . case
        var newDir = dir;
        var nextLoc = GetNext(loc, newDir);
        PropagateBeam(field, beamField, nextLoc, newDir);
    }
}

(int x, int y) GetNext((int x, int y) loc, Direction dir)
{
    return dir switch
    {
        Direction.Up => (loc.x, loc.y-1),
        Direction.Down => (loc.x, loc.y+1),
        Direction.Left => (loc.x-1, loc.y),
        Direction.Right => (loc.x+1, loc.y),
    };
}

class BeamField {
    public BeamLocation[,] Beams {get; private set;}

    public BeamField(int Width, int Height) {
        Beams = new BeamLocation[Width, Height];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Beams[x,y] = new BeamLocation(false, false, false, false);
            }
        }
    }

    public BeamLocation Get((int x, int y) loc) => Beams[loc.x, loc.y];

    public void Set((int x, int y) loc, Direction dir)
    {
        Beams[loc.x, loc.y] = Get(loc).With(dir);
    }

    public int CountEnergized() {
        var sum = 0;
        foreach (var loc in Beams)
        {
            if (loc.Up || loc.Down || loc.Left || loc.Right) {
                sum++;
            }
        }
        return sum;
    }
}

record BeamLocation(bool Up, bool Down, bool Left, bool Right) {
    public bool Has(Direction dir) => dir switch
    {
        Direction.Up => Up,
        Direction.Down => Down,
        Direction.Left => Left,
        Direction.Right => Right,
        _ => throw new Exception(),
    };

    internal BeamLocation With(Direction dir)
    {
        return dir switch
        {
            Direction.Up => new BeamLocation(true, Down, Left, Right),
            Direction.Down => new BeamLocation(Up, true, Left, Right),
            Direction.Left => new BeamLocation(Up, Down, true, Right),
            Direction.Right => new BeamLocation(Up, Down, Left, true),
            _ => throw new Exception(),
        };
    }
};

class Field {
    public char[,] Chars {get; private set;}
    public int Width { get; }
    public int Height { get; }

    public Field(IEnumerable<string> lines) {
        Width = lines.First().Length;
        Height = lines.Count();
        Chars = new char[Width, Height];

        var arr = lines.ToArray();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Chars[x,y] = arr[y][x];
            }
        }
    }

    public void Print() {
        for (int y = 0; y < Height; y++)
        {
            var line = "";
            for (int x = 0; x < Width; x++)
            {
                line += Chars[x,y];
            }
            Console.WriteLine(line);
        }
    }

    public char Get((int x, int y) loc) => Chars[loc.x, loc.y];
}

enum Direction
{
    Up,
    Down,
    Left,
    Right
}
