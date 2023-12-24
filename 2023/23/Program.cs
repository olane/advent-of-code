using System.Dynamic;

var input = File.ReadAllLines("input.txt");

var graph = new Dictionary<Location, List<Link>>();

var start = new Location(1, 0);
var width = input[0].Length;
var height = input.Length;

Walk(start, 0, new Location(1, -1), start);
Console.WriteLine(LongestPath(graph, start).Max());

foreach(var (from, links) in graph) {
    foreach(var link in links) {
        if(link.To.Y != height - 1) {
            graph[link.To].Add(new Link(from, link.Distance));
        }
    }
}

Console.WriteLine(LongestPath(graph, start).Max());


void Walk(Location location, int startingDistance, Location last, Location from) {
    var current = location;
    var distance = startingDistance;

    while (current.Y != height - 1) {
        distance++;
        var neighbors = current.Neighbors().ToArray();

        var next = neighbors.Where(n => n != last && Get(input, n) != '#').ToArray();

        if(next.Count() > 1) {
            foreach(var n in next) {
                if(n.X > current.X && Get(input, n) == '>'
                || n.X < current.X && Get(input, n) == '<'
                || n.Y > current.Y && Get(input, n) == 'v'
                || n.Y < current.Y && Get(input, n) == '^'){
                    Walk(n, 0, current, current);
                }
            }
            break;
        }

        last = current;
        current = next.Single();
    }

    if(graph.ContainsKey(from)) {
        graph[from].Add(new Link(current, distance));
    }
    else {
        graph[from] = [new Link(current, distance)];
    }
}


IEnumerable<int> LongestPath(IDictionary<Location, List<Link>> graph, Location start) {
    var queue = new List<(Location, int, ISet<Location>)>() { (start, 0, new HashSet<Location>()) };

    while(queue.Any()){
        var next = queue.MaxBy(i => i.Item2);
        queue.Remove(next);

        var (loc, distance, visited) = next;

        if (loc.Y == height -1) {
            yield return distance;
        }
        else {
            var links = graph[loc];
            visited.Add(loc);
            queue.AddRange(links
                .Where(l => !visited.Contains(l.To))
                .Select(l => (l.To, distance + l.Distance, visited)));
            
            queue = queue.Distinct().ToList();
        }
    }
}



char Get(string[] input, Location loc) {
    return input[loc.Y][loc.X];
}

record struct Link(Location To, int Distance);

record struct Location(int X, int Y) {
    public IEnumerable<Location> Neighbors() => [
        new Location(X - 1, Y),
        new Location(X + 1, Y),
        new Location(X, Y - 1),
        new Location(X, Y + 1),
    ];
}