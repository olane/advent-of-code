var lines = File.ReadLines("input.txt");

var readings = lines.Select(l => l.Split(" ").Select(int.Parse));
var nexts = readings.Select(x => PredictNext(x.ToArray())).ToArray();
Console.WriteLine(String.Join(", ", nexts));
Console.WriteLine(nexts.Sum());

var prevs = readings.Select(x => PredictPrev(x.ToArray())).ToArray();
Console.WriteLine(String.Join(", ", prevs));
Console.WriteLine(prevs.Sum());

int PredictNext(int[] sequence) {
    var differences = GetDifferences(sequence).ToArray();
    if(differences.All(x => x == 0)) {
        return sequence.Last();
    }
    else {
        var nextDiff = PredictNext(differences);
        return sequence.Last() + nextDiff;
    }
}

int PredictPrev(int[] sequence) {
    var differences = GetDifferences(sequence).ToArray();
    if(differences.All(x => x == 0)) {
        return sequence.First();
    }
    else {
        var nextDiff = PredictPrev(differences);
        return sequence.First() - nextDiff;
    }
}

IEnumerable<int> GetDifferences(int[] sequence) {
    for (int i = 0; i < sequence.Length - 1; i++)
    {
        yield return sequence[i + 1] - sequence[i]; 
    }
}
