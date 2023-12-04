var lines = File.ReadLines("input.txt");

Card CardFromLine(string line) {
    var id = int.Parse(line.Substring(5, 3));
    var winners = line.Substring(10, 29).Chunk(3).Select(x => int.Parse(x));
    var numbers = line.Substring(42).Chunk(3).Select(x => int.Parse(x));

    return new Card(id, numbers, winners);
}

var cards = lines.Select(CardFromLine);

int ScoreCard(Card card) {
    return card.numbers.Count(x => card.winners.Contains(x));
}

//part1
var scores1 = cards.Select(ScoreCard).Where(x => x > 0).Select(x => Math.Pow(2, x-1));
Console.WriteLine(scores1.Sum());

//part2
var scoreDict = cards.ToDictionary(c => c.id, c => new {Wins = ScoreCard(c), Copies = 1});

foreach(var (id, card) in scoreDict) {
    for (int i = 1; i <= card.Wins; i++)
    {
        scoreDict[id + i] = new { scoreDict[id + i].Wins, Copies = scoreDict[id + i].Copies + card.Copies };
    }
}

Console.WriteLine(scoreDict.Sum(x => x.Value.Copies));


record Card(int id, IEnumerable<int> numbers, IEnumerable<int> winners);
