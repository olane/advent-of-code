
var input = File.ReadAllLines("input.txt");

long boxSize = 200000000000000;

var hailstones = input.Select(s => Hailstone.FromString(s, boxSize));

Console.WriteLine(CountIntersections(hailstones, new Location(boxSize,boxSize,-1000), new Location(boxSize*2, boxSize*2, 1000)));
 
int CountIntersections(IEnumerable<Hailstone> hailstones, Location startBox, Location endBox) {
    var arr = hailstones.ToArray();

    var count = 0;
    for (int i = 0; i < arr.Length; i++)
    {
        for (int j = i + 1; j < arr.Length; j++)
        {
            count += CountIntersection(arr[i], arr[j], startBox, endBox);
        }
    }

    return count;
}

int CountIntersection2(Hailstone h1, Hailstone h2, Location startBox, Location endBox) {
    if(!isTravellingTowardsBox(h1, startBox, endBox) 
    || !isTravellingTowardsBox(h2, startBox, endBox)) {
        return 0;
    }

    var yEntry1 = (startBox.X - h1.Location.X) / h1.Velocity.X * h1.Velocity.Y + h1.Location.Y;
    var yEntry2 = (startBox.X - h2.Location.X) / h2.Velocity.X * h2.Velocity.Y + h2.Location.Y;

    var yExit1 = (endBox.X - h1.Location.X) / h1.Velocity.X * h1.Velocity.Y + h1.Location.Y;
    var yExit2 = (endBox.X - h2.Location.X) / h2.Velocity.X * h2.Velocity.Y + h2.Location.Y;

    var crosses = yEntry1 > yEntry2 && yExit2 > yExit1 || yEntry2 > yEntry1 && yExit1 > yExit2;

    return crosses ? 1 : 0;
}

bool isTravellingTowardsBox(Hailstone h, Location startBox, Location endBox) {
    var goingXWards = h.Velocity.X > 0;
    var goingYWards = h.Velocity.Y > 0;

    var isXWards = endBox.X > h.Location.X;
    var isYWards = endBox.Y > h.Location.Y;

    return goingXWards == isXWards && goingYWards == isYWards;
}


int CountIntersection(Hailstone h1, Hailstone h2, Location startBox, Location endBox)
{

    double x1 = h1.Location.X;
    double y1 = h1.Location.Y;

    double x2 = h1.Location.X + h1.Velocity.X * 1000000;
    double y2 = h1.Location.Y + h1.Velocity.Y * 1000000;

    double x3 = h2.Location.X;
    double y3 = h2.Location.Y;

    double x4 = h2.Location.X + h2.Velocity.X * 1000000;
    double y4 = h2.Location.Y + h2.Velocity.Y * 1000000;

    double x12 = x1 - x2;
    double x34 = x3 - x4;
    double y12 = y1 - y2;
    double y34 = y3 - y4;

    double c = x12 * y34 - y12 * x34;

    if (Math.Abs(c) < 0.000000000000000000001)
    {
        // No intersection
        return 0;
    }
    else
    {
        // Intersection
        double a = x1 * y2 - y1 * x2;
        double b = x3 * y4 - y3 * x4;

        double x = (a * x34 - b * x12) / c;
        double y = (a * y34 - b * y12) / c;

        return IsInBox(startBox, endBox, x, y) && IsInFuture(h1, x, y) && IsInFuture(h2, x, y) ? 1 : 0;
    }
}

bool IsInFuture(Hailstone h1, double x, double y)
{
    var goingXWards = h1.Velocity.X > 0;
    var goingYWards = h1.Velocity.Y > 0;

    var wasXWards = x > h1.Location.X;
    var wasYWards = y > h1.Location.Y;

    return goingXWards == wasXWards && goingYWards == wasYWards;
}

static bool IsInBox(Location startBox, Location endBox, double x, double y)
{
    return x >= startBox.X && x <= endBox.X && y >= startBox.Y && y <= endBox.Y;
}

record Hailstone(Location Location, Velocity Velocity){
    public static Hailstone FromString(string str, long boxSize) {
        var parts = str.Split(" @ ");
        return new Hailstone(Location.FromString(parts[0], boxSize), Velocity.FromString(parts[1]));
    }
};

record Location(long X, long Y, long Z){
    public static Location FromString(string str, long boxSize) {
        var parts = str.Split(",");
        return new Location(long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2]));
    }
};

record Velocity(long X, long Y, long Z){
    public static Velocity FromString(string str) {
        var parts = str.Split(",");
        return new Velocity(long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2]));
    }
};