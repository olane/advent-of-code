var lines = File.ReadLines("input.txt");
var chars = lines.Select(line => line.ToCharArray()).ToArray();

var heights = chars.Select(line => line.Select(x => toHeight(x)).ToArray()).ToArray();

var width = chars.Length;
var height = chars[0].Length;

var start = FindCoordsOfChar(chars, width, height, 'S').Single();
var end = FindCoordsOfChar(chars, width, height, 'E').Single();

Console.WriteLine(ShortestPathLength(start, end, heights));

var starts = FindCoordsOfChar(chars, width, height, 'a');

Console.WriteLine(starts.Select(a => ShortestPathLength(a, end, heights)).Min());

int ShortestPathLength((int x, int y) start, (int x, int y) end, int[][] map) {
   

    var visited = new HashSet<(int x, int y)>();
    var queue = new Queue<((int x, int y), int distance)>();
    
    queue.Enqueue((start, 0));

    while(queue.Any()) {
        var (point, distance) = queue.Dequeue();

        if(visited.Contains(point)){
            continue;
        }

        visited.Add(point);

        if(point == end) {
            return distance;
        }

        var up = (point.x , point.y - 1);
        var down = (point.x, point.y + 1);
        var left = (point.x - 1, point.y);
        var right = (point.x + 1, point.y);

        var dirs = new [] {up, down, left, right};

        var nextToQueue = dirs.Where(x => CanTravel(map, x, GetHeight(map, point)));
        foreach (var item in nextToQueue)
        {
            queue.Enqueue((item, distance + 1));
        }
    }

    return int.MaxValue;
}

bool CanTravel(int[][] map, (int x, int y) point, int currentHeight) {
    if(point.x >= map.Length || point.y >= map[0].Length || point.x < 0 || point.y < 0) {
        return false;
    }

    return GetHeight(map, point) <= currentHeight + 1;
}

int GetHeight(int[][] map, (int x, int y) point) {
    return map[point.x][point.y];
}

int toHeight(char x)
{
    return x switch  {
        'S' => 0,
        'E' => 26,
        var c => c - (int)'a'
    };
}

IEnumerable<(int x, int y)> FindCoordsOfChar(char[][] chars, int width, int height, char character)
{
    var result = new List<(int x, int y)>();
    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            if (chars[x][y] == character)
            {
                result.Add((x, y));
            }
        }
    }

    return result;
}