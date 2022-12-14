


var monkeyProperties = new MonkeyProperties[] {
    new MonkeyProperties(x => x * 7, 5, 1, 6),
    new MonkeyProperties(x => x * x, 17, 2, 5),
    new MonkeyProperties(x => x + 8, 7, 4, 3),
    new MonkeyProperties(x => x + 4, 13, 0, 7),
    new MonkeyProperties(x => x + 3, 19, 7, 3),
    new MonkeyProperties(x => x + 5, 3, 4, 2),
    new MonkeyProperties(x => x + 7, 11, 1, 5),
    new MonkeyProperties(x => x * 3, 2, 0, 6),
};

List<List<ulong>> monkeyItems = new List<List<ulong>>()
{
    new List<ulong>() {74, 64, 74, 63, 53},
    new List<ulong>() {69, 99, 95, 62},
    new List<ulong>() {59, 81},
    new List<ulong>() {50, 67, 63, 57, 63, 83, 97},
    new List<ulong>() {61, 94, 85, 52, 81, 90, 94, 70},
    new List<ulong>() {69},
    new List<ulong>() {54, 55, 58},
    new List<ulong>() {79, 51, 83, 88, 93, 76},
};

ulong[] monkeyInspections = Enumerable.Repeat((ulong)0, 8).ToArray();

ulong monkeysModuloCap = monkeyProperties.Aggregate((ulong)1, (a, x) => a * x.DivisibleTest);

int rounds = 10000;
for (int round = 0; round < rounds; round++)
{
    for (int i = 0; i < monkeyProperties.Length; i++)
    {
        var m = monkeyProperties[i];
        var items = monkeyItems[i];

        foreach (var item in items)
        {
            monkeyInspections[i]++;
            //var newItem = m.Operation(item) / 3;
            var newItem = m.Operation(item) % monkeysModuloCap;

            if (newItem % m.DivisibleTest == 0) {
                monkeyItems[m.IfTrueMonkey].Add(newItem);
            }
            else {
                monkeyItems[m.IfFalseMonkey].Add(newItem);
            }
        }
        monkeyItems[i] = new List<ulong>();
    }
}

var sortedInspections = monkeyInspections.OrderByDescending(x => x).ToArray();

Console.WriteLine(sortedInspections[0] * sortedInspections[1]);

record MonkeyProperties(Func<ulong, ulong> Operation, uint DivisibleTest, int IfTrueMonkey, int IfFalseMonkey);
