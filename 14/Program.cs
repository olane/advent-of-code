
var lines = File.ReadLines("input.txt");

var walls = lines.Select(line =>
{
    var split = line.Split(" -> ");
    return split.Select(coord =>
    {
        var coordSplit = coord.Split(",");
        return new Point(x: Int16.Parse(coordSplit[0]), y: Int16.Parse(coordSplit[1]));
    });
});

Material[,] grid = GetGrid(walls);

var dropPoint = new Point(500, 0);
var endlessFall = false;
while (!endlessFall)
{
    endlessFall = SimulateSandDrop(grid, dropPoint);
}
DrawGrid(grid, new Point(400, 0), new Point(550, 175));
Console.WriteLine(IterateGrid(grid).Count(x => x == Material.Sand));

var maxY = walls.SelectMany(w => w.Select(p => p.y)).Max();
var walls2 = walls.Append(new [] { new Point(0, maxY + 2), new Point(1050, maxY + 2)});
Material[,] grid2 = GetGrid(walls2);
DrawGrid(grid2, new Point(400, 0), new Point(550, 175));

var full = false;
while (!full) {
    SimulateSandDrop(grid2, dropPoint);
    full = grid2[dropPoint.x, dropPoint.y] == Material.Sand;
}
DrawGrid(grid2, new Point(400, 0), new Point(550, 175));
Console.WriteLine(IterateGrid(grid2).Count(x => x == Material.Sand));

bool SimulateSandDrop(Material[,] grid, Point dropPoint)
{
    var position = dropPoint;
    while (true)
    {
        if (position.y == grid.GetLength(1) - 1)
        {
            return true; // endless fall
        }
        if (grid[position.x, position.y + 1] == Material.Air)
        {
            position = new Point(position.x, position.y + 1);
        }
        else if (grid[position.x - 1, position.y + 1] == Material.Air)
        {
            position = new Point(position.x - 1, position.y + 1);
        }
        else if (grid[position.x + 1, position.y + 1] == Material.Air)
        {
            position = new Point(position.x + 1, position.y + 1);
        }
        else
        {
            // settled
            grid[position.x, position.y] = Material.Sand;
            return false;
        }
    }
}

void AddLineToGrid(Material[,] grid, Point start, Point end)
{
    var x = new[] { start.x, end.x }.OrderBy(c => c).ToArray();
    var y = new[] { start.y, end.y }.OrderBy(c => c).ToArray();

    for (int i = x[0]; i <= x[1]; i++)
    {
        grid[i, start.y] = Material.Stone;
    }
    for (int i = y[0]; i <= y[1]; i++)
    {
        grid[start.x, i] = Material.Stone;
    }
}

void DrawGrid(Material[,] grid, Point? start, Point? end)
{
    start = start ?? new Point(0, 0);
    end = end ?? new Point(grid.GetLength(0), grid.GetLength(1));

    for (int y = start.y; y < end.y; y++)
    {
        var line = "";
        for (int x = start.x; x < end.x; x++)
        {
            line += DrawMaterial(grid[x, y]);
        }
        Console.WriteLine(line);
    }
}

IEnumerable<T> IterateGrid<T>(T[,] grid)
{
    foreach (var item in grid)
    {
        yield return item;
    }
}

char DrawMaterial(Material m) => m switch
{
    Material.Air => '.',
    Material.Stone => '#',
    Material.Sand => 'o',
    _ => '?'
};

Material[,] GetGrid(IEnumerable<IEnumerable<Point>> walls)
{
    var grid = new Material[1100, 1100];
    foreach (var wall in walls)
    {
        Point lastPoint = wall.First();
        foreach (var thisPoint in wall.Skip(1))
        {
            AddLineToGrid(grid, lastPoint, thisPoint);
            lastPoint = thisPoint;
        }
    }

    return grid;
}

enum Material
{
    Air,
    Stone,
    Sand
}

record Point(int x, int y);