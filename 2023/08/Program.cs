var lines = File.ReadLines("input.txt");

var nodes = lines.Skip(2).Select(l => new Node(l.Substring(0, 3), l.Substring(7, 3), l.Substring(12, 3)));

var dict = nodes.ToDictionary(n => n.id);

Console.WriteLine(Part1(dict, lines.First(), dict["AAA"]));

var startNodes = dict.Where(e => e.Key.EndsWith('A')).Select(e => e.Value);

Console.WriteLine(Part2(dict, lines.First()));

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

int Part2(Dictionary<string, Node> dict, string directions) {
    var currentNodes = dict.Where(e => e.Key.EndsWith('A')).Select(e => e.Value);
    var count = 0;

    while(true) {
        foreach(char c in directions) {
            switch (c)
            {
                case 'L':
                    currentNodes = currentNodes.Select(n => dict[n.left]);
                    break;
                case 'R':
                    currentNodes = currentNodes.Select(n => dict[n.right]);
                    break;
                default:
                    throw new ArgumentException();
            }
            count++;
            if (currentNodes.All(n => n.id.EndsWith('Z'))) {
                return count;
            }
        }
    }
}

record Node(string id, string left, string right);
