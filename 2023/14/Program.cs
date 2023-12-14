

var lines = File.ReadLines("input.txt").ToArray();

var HEIGHT = lines.Length;
var WIDTH = lines[0].Length;

var cubes = GetCharPositions(lines, '#').ToArray();
var rocks = GetCharPositions(lines, 'O');

var rolledRocks = RollAll(cubes, rocks, Direction.North);

var weights = rolledRocks.Select(r => HEIGHT - r.y);
Console.WriteLine(weights.Sum());

var cache = new IEnumerable<(int x, int y)>[1000];
for (long i = 0; i < 1000000000; i++)
{
    rocks = Spin(cubes, rocks);

    for (int j = 1; j < cache.Length; j++)
    {
        var cached = cache[(i-j + cache.Length) % cache.Length];
        if (cached != null && rocks.SequenceEqual(cached))
        {
            // we found a loop. fake iterate as far as possible.
            var period = j;
            var maxSkips = (1000000000 - i) / j;
            i += maxSkips * j;
            // we can stop, it's stable
            continue;
        }
    }
    
    cache[i % cache.Length] = rocks;
}

var weights2 = rocks.Select(r => HEIGHT - r.y);
Console.WriteLine(weights2.Sum());

IEnumerable<(int x, int y)> Spin((int x, int y)[] cubes, IEnumerable<(int x, int y)> rocks)
{
    var r1 = RollAll(cubes, rocks, Direction.North);
    var r2 = RollAll(cubes, r1, Direction.West);
    var r3 = RollAll(cubes, r2, Direction.South);
    var r4 = RollAll(cubes, r3, Direction.East);
    return r4;
}

IEnumerable<(int x, int y)> RollAll((int x, int y)[] cubes, IEnumerable<(int x, int y)> unorderedRocks, Direction d)
{
    var newRocks = new List<(int x, int y)>();

    var rocks = unorderedRocks.OrderBy(r => d switch
    {
        Direction.North => r.y,
        Direction.South => -r.y,
        Direction.East => -r.x,
        Direction.West => r.x,
        _ => throw new NotImplementedException()
    });

    foreach ((int x, int y) rock in rocks)
    {
        var newRock = d switch
        {
            Direction.North => RollNorth(rock, newRocks, cubes),
            Direction.East => RollEast(rock, newRocks, cubes),
            Direction.South => RollSouth(rock, newRocks, cubes),
            Direction.West => RollWest(rock, newRocks, cubes),
            _ => throw new NotImplementedException()
        };

        newRocks.Add(newRock);
    }

    return newRocks;
}

(int x, int y) RollNorth((int x, int y) rock, IEnumerable<(int x, int y)> rocks, IEnumerable<(int x, int y)> cubes) {
    // var relevantRocks = rocks.Where(r => r.x == rock.x && r.y < rock.y).Select(r => r.y);
    // var relevantCubes = cubes.Where(c => c.x == rock.x && c.y < rock.y).Select(c => c.y);
    // var nextObstacleY = Math.Max(relevantRocks.Max(), relevantCubes.Max());
    var columnObstacles = rocks.Where(r => r.x == rock.x).Concat(cubes.Where(c => c.x == rock.x)).Concat([(rock.x, y:-1)]);
    var nextObstacleY = columnObstacles.Where(o => o.y < rock.y).MaxBy(o => o.y).y;
    return (rock.x, nextObstacleY + 1);
}

(int x, int y) RollSouth((int x, int y) rock, IEnumerable<(int x, int y)> rocks, IEnumerable<(int x, int y)> cubes) {
    var columnObstacles = rocks.Where(r => r.x == rock.x).Concat(cubes.Where(c => c.x == rock.x)).Concat([(rock.x, y:HEIGHT)]);
    var nextObstacleY = columnObstacles.Where(o => o.y > rock.y).MinBy(o => o.y).y;
    return (rock.x, nextObstacleY - 1);
}

(int x, int y) RollWest((int x, int y) rock, IEnumerable<(int x, int y)> rocks, IEnumerable<(int x, int y)> cubes) {
    var rowObstacles = rocks.Where(r => r.y == rock.y).Concat(cubes.Where(c => c.y == rock.y)).Concat([(x:-1, rock.y)]);
    var nextObstacleX = rowObstacles.Where(o => o.x < rock.x).MaxBy(o => o.x).x;
    return (nextObstacleX + 1, rock.y);
}

(int x, int y) RollEast((int x, int y) rock, IEnumerable<(int x, int y)> rocks, IEnumerable<(int x, int y)> cubes) {
    var rowObstacles = rocks.Where(r => r.y == rock.y).Concat(cubes.Where(c => c.y == rock.y)).Concat([(x:WIDTH, rock.y)]);
    var nextObstacleX = rowObstacles.Where(o => o.x > rock.x).MinBy(o => o.x).x;
    return (nextObstacleX - 1, rock.y);
}

IEnumerable<(int x, int y)> GetCharPositions(string[] lines, char c)
{
    for (int i = 0; i < lines.Count(); i++)
    {
        var line = lines[i];
        for (int j = 0; j < line.Length; j++)
        {
            if (line[j] == c) {
                yield return (j, i);
            }
        }
    }
}

void PrintField((int x, int y)[] cubes, IEnumerable<(int x, int y)> rocks)
{
    for (int y = 0; y < HEIGHT; y++)
    {
        var line = "";
        for (int x = 0; x < WIDTH; x++)
        {
            line += rocks.Contains((x, y)) ? 'O' : cubes.Contains((x, y)) ? '#' : '.';
        }
        Console.WriteLine(line);
    }

    Console.WriteLine("-----");
}

enum Direction {
    North,
    South,
    East,
    West
}
