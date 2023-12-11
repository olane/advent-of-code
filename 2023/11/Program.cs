var lines = File.ReadLines("input.txt");

var f = new Field(lines);

//part1
var distance = 0;
var galaxies = f.GetGalaxies().ToArray();
for (int i = 0; i < galaxies.Length; i++)
{
    for (int j = i; j < galaxies.Length; j++)
    {
        distance += f.GetManhattanDistance(galaxies[i], galaxies[j], 2);
    }
}

Console.WriteLine(distance);


//part2
long distance2 = 0;
for (int i = 0; i < galaxies.Length; i++)
{
    for (int j = i; j < galaxies.Length; j++)
    {
        distance2 += f.GetManhattanDistance(galaxies[i], galaxies[j], 1000000);
    }
}

Console.WriteLine(distance2);


class Field {
    public char[,] Arr {get; private set;}
    public int Width {get; private set;}
    public int Height {get; private set;}

    public Field(IEnumerable<string> lines) {
        Width = lines.First().Length;
        Height = lines.Count();

        var newArr = new char[Width, Height];

        var y = 0;
        foreach (var l in lines)
        {
            var x = 0;
            foreach (char c in l) {
                newArr[x, y] = c;
                x++;
            }
            y++;
        }

        Arr = newArr;
    }

    private IEnumerable<(int x, int y)> Galaxies {get; set;}
    public IEnumerable<(int x, int y)> GetGalaxies() {
        if(Galaxies != null) return Galaxies;

        var result = new List<(int x, int y)>();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if(Arr[x, y] == '#') {
                    result.Add((x, y));
                }
            }
        }
        Galaxies = result;
        return result;
    }

    private IEnumerable<int> RowCache {get; set;}
    public IEnumerable<int> EmptyRows() {
        if(RowCache != null) {
            return RowCache;
        }

        var galaxies = GetGalaxies();
        RowCache = Enumerable.Range(0, Height).Where(y => !galaxies.Any(g => g.y == y));
        return RowCache;
    }


    private IEnumerable<int> ColCache {get; set;}
    public IEnumerable<int> EmptyColumns() {
        if(ColCache != null) {
            return ColCache;
        }

        var galaxies = GetGalaxies();
        ColCache = Enumerable.Range(0, Width).Where(x => !galaxies.Any(g => g.x == x));
        return ColCache;
    }

    public int GetManhattanDistance((int x, int y) g1, (int x, int y) g2, int expansionFactor) {
        var normalDistance = Math.Abs(g1.x - g2.x) + Math.Abs(g1.y - g2.y);

        var rowSearch = Enumerable.Range(Math.Min(g1.y, g2.y), Math.Abs(g1.y - g2.y));
        var colSearch = Enumerable.Range(Math.Min(g1.x, g2.x), Math.Abs(g1.x - g2.x));

        var doubleRows = rowSearch.Intersect(EmptyRows());
        var doubleCols = colSearch.Intersect(EmptyColumns());

        return normalDistance + (doubleRows.Count() + doubleCols.Count()) * (expansionFactor - 1);
    }
}

