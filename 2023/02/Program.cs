var lines = File.ReadLines("input.txt");

Game GameFromLine(string line) {
    var split = line.Split(":");
    var gameString = split[0];
    var cubesString = split[1];

    int id = int.Parse(gameString.Substring(5));

    var reveals = cubesString.Split(";").Select(RevealFromString);

    return new Game(id, reveals);
}

Reveal RevealFromString(string str) {
    var split = str.Split(",").Select(x => x.Trim());

    int red = 0;
    int green = 0;
    int blue = 0;

    foreach(string s in split) {
        if(s.Trim().EndsWith("red")) {
            red += int.Parse(s.Split(" ")[0]);
        }
        else if(s.Trim().EndsWith("green")) {
            green += int.Parse(s.Split(" ")[0]);
        }
        else if(s.Trim().EndsWith("blue")) {
            blue += int.Parse(s.Split(" ")[0]);
        }
    }

    return new Reveal(red, blue, green);
}

var games = lines.Select(GameFromLine);

// PART ONE
var maxReds = 12;
var maxGreens = 13;
var maxBlues = 14;

var possibleGames = games.Where(g => !g.Reveals.Any(r => r.Red > maxReds || r.Green > maxGreens || r.Blue > maxBlues));
Console.WriteLine(possibleGames.Select(x => x.Id).Sum());

// PART TWO

int GamePower(Game game) {
    int minReds = 0;
    int minGreens = 0;
    int minBlues = 0;

    foreach(var r in game.Reveals) {
        minReds = Math.Max(r.Red, minReds);
        minGreens = Math.Max(r.Green, minGreens);
        minBlues = Math.Max(r.Blue, minBlues);
    }

    return minReds * minGreens * minBlues;
}

Console.WriteLine(games.Select(GamePower).Sum());

record Game(int Id, IEnumerable<Reveal> Reveals);

record Reveal(int Red, int Blue, int Green);
