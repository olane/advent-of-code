var lines = File.ReadLines("input.txt");

var moves = lines.SelectMany(line =>
{
    var parts = line.Split(" ");
    var distance = int.Parse(parts[1]);
    return Enumerable.Repeat(parts[0], distance);
});

var moveVectors = moves.Select(x => x switch
{
    "U" => new Vector(0, 1),
    "D" => new Vector(0, -1),
    "L" => new Vector(-1, 0),
    "R" => new Vector(1, 0),
    _ => throw new Exception()
});

Console.WriteLine(RunSimulation(moveVectors, 2));
Console.WriteLine(RunSimulation(moveVectors, 10));

int RunSimulation(IEnumerable<Vector> moveVectors, int ropeLenth)
{
    var startState = new List<State>() { new State(ropeLenth) };

    var states = moveVectors.Aggregate(startState, (states, v) =>
    {
        var newState = incrementState(states.Last(), v);
        return states.Append(newState).ToList();
    });

    var tailPositionCount = states.Select(x => x.Knots.Last()).Distinct().Count();
    return tailPositionCount;
}

State incrementState(State currentState, Vector headMove)
{
    var newKnots = new Vector[currentState.Knots.Length];

    newKnots[0] = add(currentState.Knots[0], headMove);
    for (int i = 1; i < newKnots.Length; i++)
    {
        var move = getTailMoveVector(newKnots[i - 1], currentState.Knots[i]);
        newKnots[i] = add(currentState.Knots[i], move);
    }

    return new State(newKnots);
}

Vector getTailMoveVector(Vector head, Vector tail)
{
    var distance = subtract(head, tail);

    if (isAdjacent(distance))
    {
        return new Vector(0, 0);
    }

    return new Vector(clamp(distance.x), clamp(distance.y));
}

Vector subtract(Vector a, Vector b) => new Vector(a.x - b.x, a.y - b.y);
Vector add(Vector a, Vector b) => new Vector(a.x + b.x, a.y + b.y);
bool isAdjacent(Vector distance) => Math.Abs(distance.x) <= 1 && Math.Abs(distance.y) <= 1;
int clamp(int x, int min = -1, int max = 1) => Math.Max(Math.Min(x, max), min);

record Vector(int x, int y);

class State
{
    public Vector[] Knots { get; }

    public State(Vector[] knots)
    {
        Knots = knots;
    }

    public State(int numberOfKnots)
    {
        Knots = Enumerable.Repeat(new Vector(0, 0), numberOfKnots).ToArray();
    }
}
