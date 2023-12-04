var lines = File.ReadLines("input.txt");

Card CardFromLine(string line) {
    var id = int.Parse(line.Substring(5, 3));
    var winners = line.Substring(10, 29).Chunk(3).Select(x => int.Parse(x));
    var numbers = line.Substring(42).Chunk(3).Select(x => int.Parse(x));

    return new Card(id, numbers, winners);
}

var cards = lines.Select(CardFromLine);

//part1

int ScoreCard1(Card card) {
    var score = 0;
    foreach(var n in card.numbers) {
        if(card.winners.Contains(n)) {
            if(score > 0) {
                score *= 2;
            }
            else {
                score = 1;
            }
        }
    }
    return score;
}

var scores1 = cards.Select(ScoreCard1);
Console.WriteLine(scores1.Sum());
//part2

int ScoreCard2(Card card) {
    return card.numbers.Count(x => card.winners.Contains(x));
}

var scoreDict = cards.ToDictionary(c => c.id, c => new {Wins = ScoreCard2(c), Copies = 1});

foreach(var (id, card) in scoreDict) {
    for (int i = 1; i <= card.Wins; i++)
    {
        scoreDict[id + i] = new { scoreDict[id + i].Wins, Copies = scoreDict[id + i].Copies + card.Copies };
    }
}

Console.WriteLine(scoreDict.Sum(x => x.Value.Copies));


record Card(int id, IEnumerable<int> numbers, IEnumerable<int> winners);
