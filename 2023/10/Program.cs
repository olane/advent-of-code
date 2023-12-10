var input = File.ReadLines("input.txt").Select(x => x.ToCharArray()).ToArray();

var width = input[0].Length;
var height = input.Length;

var loop = GetLoop(input);
Console.WriteLine(loop.Count());
Console.WriteLine(loop.Count() / 2);

char[,] fillArr = CreateFillArr(width, height, loop);
printArr(fillArr);

Console.WriteLine("---");
var flooded = FloodFillArr(fillArr);
printArr(flooded);

Console.WriteLine("---");

// "Inside" the loop will either be left or right count
// note the flood fill seems to miss some edge cases, 
// need to count ?s that look like they should be in the loop in the output
// and add them on :(
Console.WriteLine(Count(flooded, 'r'));
Console.WriteLine(Count(flooded, 'l'));

int Count(char[,] flooded, char c)
{
    var count = 0;
    foreach (var item in flooded)
    {
        if(item == c) {
            count ++;
        }
    }
    return count;
}

IEnumerable<((int x, int y) pos, char facing)> GetLoop(char[][] map)
{
    var start = FindCoords(map, 'S');
    yield return (start, 'r');

    //can always go right from start in the given inputs
    var currentCoords = (x: start.x + 1, start.y);
    var length = 1;
    var facing = 'r';
    var currentLetter = GetCoord(map, currentCoords);

    while (currentLetter != 'S')
    {
        yield return (currentCoords, facing);

        facing = (currentLetter, facing) switch
        {
            ('|', 'd') => 'd',
            ('|', 'u') => 'u',
            ('-', 'l') => 'l',
            ('-', 'r') => 'r',
            ('L', 'd') => 'r',
            ('L', 'l') => 'u',
            ('J', 'r') => 'u',
            ('J', 'd') => 'l',
            ('7', 'u') => 'l',
            ('7', 'r') => 'd',
            ('F', 'u') => 'r',
            ('F', 'l') => 'd',
        };

        currentCoords = facing switch
        {
            'r' => (currentCoords.x + 1, currentCoords.y),
            'l' => (currentCoords.x - 1, currentCoords.y),
            'u' => (currentCoords.x, currentCoords.y - 1),
            'd' => (currentCoords.x, currentCoords.y + 1),
        };

        length++;
        currentLetter = GetCoord(map, currentCoords);
    }
}



char GetCoord(char[][] map, (int x, int y) coord)
{
    return map[coord.y][coord.x];
}

(int x, int y) FindCoords(char[][] map, char search)
{
    for (int i = 0; i < width; i++)
    {
        for (int j = 0; j < height; j++)
        {
            if (GetCoord(map, (i, j)) == search)
            {
                return (i, j);
            }
        }
    }
    return (-1, -1);
}

char[,] FloodFillArr(char[,] arr) {

    var Ls = GetAll(arr, 'l');

    foreach (var L in Ls)
    {
        Flood(arr, L, 'l');
    }

    var Rs = GetAll(arr, 'r');

    foreach (var R in Rs)
    {
        Flood(arr, R, 'r');
    }
    
    return arr;
}

void Flood(char[,] arr, (int x, int y) initPos, char c)
{
    var q = new Queue<(int x, int y)>();
    q.Enqueue(initPos);

    while(q.Count != 0) {
        var (x, y) = q.Dequeue();

        if(height <= y) return;
        if(y < 0) return;
        if(width <= x) return;
        if(x < 0) return;
        if(arr[y, x] == '+') return;

        arr[y, x] = c;
        q.Enqueue((x + 1, y));
        q.Enqueue((x - 1, y));
        q.Enqueue((x, y + 1));
        q.Enqueue((x, y - 1));
    }
}

IEnumerable<(int x, int y)> GetAll(char[,] arr, char target) {
    for (int j = 0; j < height; j++)
    {
        for (int i = 0; i < width; i++)
        {
            if (arr[j, i] == target) {
                yield return (i, j);
            }
        }
    }
}

char[,] CreateFillArr(int width, int height, IEnumerable<((int x, int y) pos, char facing)> loop)
{
    var fillArr = new char[height, width];
    foreach (var (pos, facing) in loop)
    {
        if (facing == 'r')
        {
            WriteFillArr(fillArr, pos.x, pos.y - 1, 'l');
            WriteFillArr(fillArr, pos.x, pos.y + 1, 'r');
        }
        if (facing == 'l')
        {
            WriteFillArr(fillArr, pos.x, pos.y - 1, 'r');
            WriteFillArr(fillArr, pos.x, pos.y + 1, 'l');
        }
        if (facing == 'u')
        {
            WriteFillArr(fillArr, pos.x + 1, pos.y, 'r');
            WriteFillArr(fillArr, pos.x - 1, pos.y, 'l');
        }
        if (facing == 'd')
        {
            WriteFillArr(fillArr, pos.x - 1, pos.y, 'r');
            WriteFillArr(fillArr, pos.x + 1, pos.y, 'l');
        }
    }

    foreach (var (pos, facing) in loop)
    {
        WriteFillArr(fillArr, pos.x, pos.y, '+');
    }

    return fillArr;
}

void WriteFillArr(char[,] arr, int x, int y, char c)
{
    if (x < 0 || y < 0 || x >= width || y >= height)
    {
        return;
    }
    arr[y, x] = c;
}

void printArr(char[,] arr)
{
    for (int j = 0; j < height; j++)
    {
        for (int i = 0; i < width; i++)
        {
            Console.Write(arr[j, i]);
        }
        Console.Write(Environment.NewLine);
    }
}
