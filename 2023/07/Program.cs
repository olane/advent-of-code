

var lines = File.ReadLines("input.txt");

var hands = lines.Select(ParseHand).ToList();

//part1
hands.Sort(HandComparator);
var scores = hands.Select((h, i) => h.bid * (i + 1));
Console.WriteLine(scores.Sum());

//part2
var handsWithJokers = hands.Select(x => new Hand(ArrReplace(x.cards, 11, 1), x.bid)).ToList();
handsWithJokers.Sort(HandComparatorWithJokers);
var scoresWithJokers = handsWithJokers.Select((h, i) => h.bid * (i + 1));
//handsWithJokers.Reverse();
Console.WriteLine(scoresWithJokers.Sum());

int HandComparator(Hand x, Hand y)
{
    var typeComparison = HandTypeComparator(x, y);
    return typeComparison != 0 
        ? typeComparison
        : HandCardComparator(x, y);
}

int HandComparatorWithJokers(Hand x, Hand y)
{
    var typeComparison = HandTypeComparator(x, y);
    return typeComparison != 0 
        ? typeComparison
        : HandCardComparator(x, y);
}

int HandCardComparator(Hand x, Hand y)
{
    for (int i = 0; i < x.cards.Length; i++)
    {
        if (x.cards[i] > y.cards[i]) {
            return 1;
        }
        else if(y.cards[i] > x.cards[i]) {
            return -1;
        }
    }
    return 0;
}

int HandTypeComparator(Hand x, Hand y)
{
    var xGroups = Jokerise(x.cards).GroupBy(n => n);
    var yGroups = Jokerise(y.cards).GroupBy(n => n);
    
    return CardGroupComparator(xGroups, yGroups);
}

int[] Jokerise(int[] cards) {
    var biggestNonJokerGroup = cards.GroupBy(n => n).Where(g => g.Key != 1).MaxBy(g => g.Count())?.Key;
    if (biggestNonJokerGroup == null) {
        return cards;
    }
    return ArrReplace(cards, 1, (int)biggestNonJokerGroup);
}

int CardGroupComparator(IEnumerable<IGrouping<int, int>> xGroups, IEnumerable<IGrouping<int, int>> yGroups)
{
    //88J99
    var biggestXGroup = xGroups.Max(g => g.Count());
    var biggestYGroup = yGroups.Max(g => g.Count());

    // if(xGroups.Any(g => g.Key == 1)) {
    //     biggestXGroup += xGroups.First(g => g.Key == 1).Count();
    // }
    
    // if(yGroups.Any(g => g.Key == 1)) {
    //     biggestYGroup += yGroups.First(g => g.Key == 1).Count();
    // }

    if (biggestXGroup == 3 && biggestYGroup == 3)
    {
        // special 3 of a kind full house disambiguation
        var xPairCount = xGroups.Where(g => g.Count() == 2).Count();
        var yPairCount = yGroups.Where(g => g.Count() == 2).Count();
        // we know we have a 3 of a kind in both so we just need to
        // know if there are any pairs to see if it's also a full house
        return xPairCount.CompareTo(yPairCount);
    }

    if (biggestXGroup == 2 && biggestYGroup == 2)
    {
        // special pair or 2 pairs disambiguation
        var xPairCount = xGroups.Where(g => g.Count() == 2).Count();
        var yPairCount = yGroups.Where(g => g.Count() == 2).Count();
        return xPairCount.CompareTo(yPairCount);
    }

    return biggestXGroup.CompareTo(biggestYGroup);
}

Hand ParseHand(string line) {
    var split = line.Split(" ");
    var cards = split[0].Select(ToCardInt);
    var bid = int.Parse(split[1]);

    return new Hand(cards.ToArray(), bid);
}

int ToCardInt(char c)
{
    if(c == 'T') {
        return 10;
    }
    
    if(c == 'J') {
        return 11;
    }
    
    if(c == 'Q') {
        return 12;
    }
    
    if(c == 'K') {
        return 13;
    }
    
    if(c == 'A') {
        return 14;
    }

    return int.Parse(c.ToString());
}

int[] ArrReplace(int[] original, int from, int to) {
    return original.Select(x => x == from ? to : x).ToArray();
}

record Hand(int[] cards, int bid);
