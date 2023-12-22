var input = File.ReadAllLines("input.txt");

var bricks = input.Select(Brick.FromInputLine);




var ordered = bricks.OrderBy(b => b.MinZ());


var settled = new List<Brick>();

foreach(var brick in ordered) {
    var bricksBelow = settled.Where(s => s.OverlapsXY(brick));
    var nextZ = 1;

    if(bricksBelow.Any()) {
        var highestBrickBelow = bricksBelow.MaxBy(b => b.MaxZ());
        nextZ = highestBrickBelow.MaxZ() + 1;
    }

    var zOffset = nextZ - brick.MinZ();
    var settledBrick = new Brick(brick.Start.OffsetZ(zOffset), brick.End.OffsetZ(zOffset));
    settled.Add(settledBrick);
}

// part 1
var supportingSomething = new HashSet<Brick>();
foreach (var brick in settled)
{
    var bricksBelow = settled.Where(s => s.OverlapsXY(brick) && s.MaxZ() == brick.MinZ() - 1);
    if (bricksBelow.Count() == 1) {
        supportingSomething.Add(bricksBelow.Single());
    }
}

Console.WriteLine(settled.Count(b => !supportingSomething.Contains(b)));


//part 2

var supportedBy = new Dictionary<Brick, IEnumerable<Brick>>();
foreach (var brick in settled)
{
    var bricksSupportingThisOne = settled.Where(s => s.OverlapsXY(brick) && s.MaxZ() == brick.MinZ() - 1);
    supportedBy[brick] = bricksSupportingThisOne.ToArray();
}

var disintegrateImplications = new Dictionary<Brick, int>();
foreach(var firstBrick in settled) {
    var disintegrated = new List<Brick>();

    IEnumerable<Brick> nextWave = [firstBrick];
    while(nextWave.Any()) {
        disintegrated.AddRange(nextWave);

        var notYetDisintegrated = supportedBy.Where(kv => !disintegrated.Contains(kv.Key));
        var notSupported = notYetDisintegrated
            .Where(kv => kv.Key.MinZ() > 1)
            .Where(kv => kv.Value.All(supportingBrick => disintegrated.Contains(supportingBrick)));
        
        nextWave = notSupported.Select(kv => kv.Key);
    }

    disintegrateImplications.Add(firstBrick, disintegrated.Count - 1);
}


Console.WriteLine(disintegrateImplications.Values.Sum());


record struct Location(int X, int Y, int Z) {
    public static Location FromString(string locString) {
        var parts = locString.Split(",");
        return new Location(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
    }

    internal Location OffsetZ(int zOffset)
    {
        return new Location(X, Y, Z + zOffset);
    }
};

record struct Brick(Location Start, Location End){
    public static Brick FromInputLine(string line) {
        var parts = line.Split("~");
        Location start = Location.FromString(parts[0]);
        Location end = Location.FromString(parts[1]);

        if(end.X < start.X || end.Y < start.Y || end.Z < start.Z) throw new Exception("oh no");

        return new Brick(start, end);
    }

    public int MinZ() => Math.Min(Start.Z, End.Z);
    public int MaxZ() => Math.Max(Start.Z, End.Z);

    internal bool OverlapsXY(Brick brick)
    {
        return OverlapsX(brick) && OverlapsY(brick);
    }
    
    internal bool OverlapsX(Brick brick)
    {
        return Math.Max(brick.Start.X, Start.X) <= Math.Min(brick.End.X, End.X);
    }
    
    internal bool OverlapsY(Brick brick)
    {
        return Math.Max(brick.Start.Y, Start.Y) <= Math.Min(brick.End.Y, End.Y);
    }
};
