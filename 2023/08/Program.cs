var lines = File.ReadLines("input.txt");

var nodes = lines.Skip(2).Select(l => new Node(l.Substring(0, 3), l.Substring(7, 3), l.Substring(12, 3)));

var dict = nodes.ToDictionary(n => n.id);
var directions = lines.First();

Console.WriteLine(Part1(dict, directions, dict["AAA"]));

var startNodes = dict.Where(e => e.Key.EndsWith('A')).Select(e => e.Value);
var periods = startNodes.Select(n => FindPeriod(dict, directions, n));
Console.WriteLine(String.Join(", ", periods));
Console.WriteLine(periods.Select(x =>(long)x).Aggregate(lcm));

int FindPeriod(Dictionary<string, Node> dict, string directions, Node startNode) {
    var seenAt = dict.ToDictionary(x => x.Key, x => new List<int>());

    var currentNode = startNode;
    var count = 0;

    while(true) {
        for (int i = 0; i < directions.Length; i++)
        {
            if(seenAt[currentNode.id].Contains(i)) {
                return count - i;
            }
            else {
                seenAt[currentNode.id].Add(i);
            }

            currentNode = directions[i] switch
            {
                'L' => dict[currentNode.left],
                'R' => dict[currentNode.right],
                _ => throw new ArgumentException(),
            };
            count++;
        }
    }
}



int Part1(Dictionary<string, Node> dict, string directions, Node startNode) {
    var currentNode = startNode;
    var count = 0;
    while(true) {
        foreach(char c in directions) {
            switch (c)
            {
                case 'L':
                    currentNode = dict[currentNode.left];
                    break;
                case 'R':
                    currentNode = dict[currentNode.right];
                    break;
                default:
                    throw new ArgumentException();
            }
            count++;
            if (currentNode.id.EndsWith('Z')) {
                return count;
            }
        }
    }
}

static long gcf(long a, long b)
{
    while (b != 0)
    {
        long temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}

static long lcm(long a, long b)
{
    return (a / gcf(a, b)) * b;
}

record Node(string id, string left, string right);
