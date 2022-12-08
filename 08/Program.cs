
var lines = File.ReadLines("input.txt");
var grid = lines.Select(line => line.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();

var top = (0, -1);
var bottom = (0, 1);
var left = (-1, 0);
var right = (1, 0);

var directions = new[] { top, bottom, left, right };

int visibleCount = PartOne(grid, directions);
Console.WriteLine(visibleCount);

int highestScenicScore = PartTwo(grid, directions);
Console.WriteLine(highestScenicScore);

bool VisibleFrom(int[][] grid, int height, (int x, int y) position, (int x, int y) direction)
{
    var xWidth = grid[0].Count();
    var yWidth = grid.Count();

    var nextX = direction.x + position.x;
    var nextY = direction.y + position.y;

    if (nextX < 0 || nextX >= xWidth || nextY < 0 || nextY >= yWidth)
    {
        return true;
    }

    if (grid[nextX][nextY] >= height)
    {
        return false;
    }

    return VisibleFrom(grid, height, (nextX, nextY), direction);
}

int PartOne(int[][] grid, (int, int)[] directions)
{
    var visibleCount = 0;
    for (int y = 0; y < grid.Count(); y++)
    {
        for (int x = 0; x < grid[0].Count(); x++)
        {
            foreach (var dir in directions)
            {
                if (VisibleFrom(grid, grid[x][y], (x, y), dir))
                {
                    visibleCount++;
                    break;
                }
            }
        }
    }

    return visibleCount;
}

int VisibilityDistance(int[][] grid, int height, (int x, int y) position, (int x, int y) direction)
{
    var xWidth = grid[0].Count();
    var yWidth = grid.Count();

    var nextX = direction.x + position.x;
    var nextY = direction.y + position.y;

    if (nextX < 0 || nextX >= xWidth || nextY < 0 || nextY >= yWidth)
    {
        return 0;
    }

    if (grid[nextX][nextY] >= height)
    {
        return 1;
    }

    return 1 + VisibilityDistance(grid, height, (nextX, nextY), direction);
}

int ScenicScore(int[][] grid, (int x, int y) position, (int, int)[] directions) {
    var scores = directions.Select(d => VisibilityDistance(grid, grid[position.x][position.y], position, d));
    return scores.Aggregate(1, (a, b) => a * b);
}

int PartTwo(int[][] grid, (int, int)[] directions)
{
    var maxScore = 0;
    for (int y = 0; y < grid.Count(); y++)
    {
        for (int x = 0; x < grid[0].Count(); x++)
        {
            var score = ScenicScore(grid, (x, y), directions);
            if (score > maxScore) {
                maxScore = score;
            }
        }
    }

    return maxScore;
}
