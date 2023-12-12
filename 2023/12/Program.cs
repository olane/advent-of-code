
var lines = File.ReadLines("input.txt");

var cache = new Dictionary<string, long>();

var arrangements1 = lines.Select(LineToMatchingArrangements);
Console.WriteLine(arrangements1.Sum());

var arrangements2 = lines.Select(x => {
    var split = x.Split(" ");
    return String.Join("?", Enumerable.Repeat(split[0], 5)) + " " + String.Join(",", Enumerable.Repeat(split[1], 5));
});

Console.WriteLine(arrangements2.Select(LineToMatchingArrangements).Select(x => (long) x).Sum());

long LineToMatchingArrangements(string line) {
    var split = line.Split(" ");
    var groups = split[1].Split(",").Select(int.Parse).ToArray();
    var field = split[0];

    var result =  MatchingArrangements(field, groups);
    return result;
}

long MatchingArrangements(string i, int[] g) {
    var trim = i.Trim('.');
    var key = GetKey(i, g);

    if(!cache.ContainsKey(key)) {
        cache[key] = Calculate(trim, g);
        var revKey = GetKey(new string(trim.Reverse().ToArray()), g.Reverse().ToArray());
        cache[revKey] = cache[key];
    }

    return cache[key];

    string GetKey(string i, int[] g) {
        return i + string.Join(",",g);
    }

    long Calculate(string field, int[] groups) {
        if(groups.Sum() > field.Count(x => x != '.')) {
            return 0;
        }

        if(groups.Sum() < field.Count(x => x == '#')) {
            return 0;
        }

        if(groups.Sum() + groups.Length - 1 > field.Length) {
            // must have enough space for each group with a . between
            return 0;
        }

        if(field.Length == 0) {
            return groups.Any() ? 0 : 1;
        }

        if(!groups.Any()) {
            return field.Contains('#') ? 0 : 1;
        }
        
        if(field.Length == 1) {
            return groups.Count() == 1 && groups[0] == 1 ? 1 : 0;
        }

        if(groups[0] > field.Length) {
            return 0;
        }

        if(groups[0] == field.Length && field.All(x => x != '.')) {
            return groups.Length == 1 ? 1 : 0;
        }

        if(field[0] == '?') {
            // we can treat this as either value, add the results of both options
            var withDot = MatchingArrangements(ReplaceNthChar(field, '.'), groups);
            var withHash = MatchingArrangements(ReplaceNthChar(field, '#'), groups);
            return withDot + withHash;
        }

        if(groups[0] == 1) {
            if(field[0] == '#' && field[1] == '.'){
                // #. matches but consumes two plus a group
                return MatchingArrangements(field[2..], groups[1..]);
            }
            else if(field[0] == '#' && field[1] == '?') {
                // this matches but the ? must be a . so we need to replace it
                return MatchingArrangements(ReplaceNthChar(field[1..], '.'), groups[1..]);
            }
            else {
                // ## can't match a group of 1
                return 0;
            }
        }

        if(field[0] == '#') {
            // we need the next group[0] elements to be # and then the one after to be a .
            if(!field[0..groups[0]].Contains('.') && field[groups[0]] != '#') {
                return MatchingArrangements(field[(groups[0]+1)..], groups[1..]);
            }
            else {
                return 0;
            }
        }

        throw new Exception("unexpected case");
    }
}

string ReplaceNthChar(string field, char replacement) {
    return replacement + field[1..];
}
