
var lines = File.ReadLines("input.txt");

var puzzles = GetPuzzles(lines);

var horiz = puzzles.Select(GetHorizontalReflections).Select(x => x.FirstOrDefault());
var vert = puzzles.Select(GetVerticalReflections).Select(x => x.FirstOrDefault());

Console.WriteLine(horiz.Sum() * 100 + vert.Sum());

var part2 = puzzles.Select(GetSmudgedReflection);

Console.WriteLine(part2.Sum());

int GetSmudgedReflection(string[] pattern) {
    var initialHoriz = GetHorizontalReflections(pattern).FirstOrDefault();
    var initialVert = GetVerticalReflections(pattern).FirstOrDefault();

    foreach(var candidate in GenerateSmudgeCandidates(pattern)) {
        var newHoriz = GetHorizontalReflections(candidate).Where(x => x != initialHoriz);
        var newVert = GetVerticalReflections(candidate).Where(x => x != initialVert);

        if(newHoriz.Any()) {
            return newHoriz.First() * 100;
        }
        if(newVert.Any()) {
            return newVert.First();
        }
    }
    
    throw new Exception("not found");
}

IEnumerable<string[]> GenerateSmudgeCandidates(string[] pattern) {
    for (int i = 0; i < pattern.Length; i++)
    {
        for (int j = 0; j < pattern[0].Length; j++)
        {
            yield return Smudge(pattern, i, j);
        }
    }
}

string[] Smudge(string[] pattern, int y, int x) {
    var replacement = pattern[y][x] == '#' ? '.' : '#';

    var replacementLine = pattern[y][..x] + replacement + pattern[y][(x + 1)..];

    var result = (string[]) pattern.Clone();
    result[y] = replacementLine;
    return result;
}

IEnumerable<int> GetHorizontalReflections(string[] pattern) {
    for (int i = 0; i < pattern.Length - 1; i++)
    {
        var reflection = true;
        var dMax = Math.Min(i + 1, pattern.Length - i - 1);
        
        for (int d = 1; d <= dMax; d++)
        {
            var line1 = i-d+1;
            var line2 = i+d;
            if (pattern[line1] != pattern[line2]) {
                reflection = false;
                break;
            }
        }
        if (reflection) {
            yield return i + 1;
        }
    }
}


IEnumerable<int> GetVerticalReflections(string[] pattern) {
    var reflected = ReflectAxes(pattern).ToArray();
    return GetHorizontalReflections(reflected);
}

IEnumerable<string> ReflectAxes(string[] pattern) {
    for (int i = 0; i < pattern[0].Length; i++)
    {
        var thisString = pattern.Select(p => p[i]).ToArray();
        yield return new string(thisString);
    }
}


IEnumerable<string[]> GetPuzzles(IEnumerable<string> lines)
{
    var thisOne = new List<string>();
    foreach (var l in lines)
    {
        if(string.IsNullOrWhiteSpace(l)) {
            yield return thisOne.ToArray();
            thisOne = [];
        }
        else {
            thisOne.Add(l);
        }
    }
    yield return thisOne.ToArray();
}
