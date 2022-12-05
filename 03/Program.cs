internal class Program
{
    private static char GetMatchingChar(string a, string b) {
        return a.Where(b.Contains).Distinct().Single();
    }

    private static char GetMatchingChar(string a, string b, string c) {
        return a.Where(b.Contains).Where(c.Contains).Distinct().Single();
    }

    private static int GetCharValue(char c) {
        if(c > 'a') {
            return c - 'a' + 1;
        }
        else {
            return c - 'A' + 27;
        }
    }

    private static int partOne(IEnumerable<Backpack> backpacks) {
        var matchingChars = backpacks.Select(bp => GetMatchingChar(bp.Compartments.Item1, bp.Compartments.Item2));
        var total = matchingChars.Sum(GetCharValue);

        return total;
    }

    private static IEnumerable<IEnumerable<T>> GroupIntoThrees<T>(IEnumerable<T> list) {
        int counter = 0;
        var subList = new List<T>();

        foreach(var item in list) {
            counter++;
            subList.Add(item);

            if (counter == 3) {
                yield return subList;
                counter = 0;
                subList = new List<T>();
            }
        }
    }

    private static int partTwo(IEnumerable<Backpack> backpacks) {
        var threes = GroupIntoThrees(backpacks);

        var badgeItemTypes = threes.Select(three =>
        {
            var arr = three.ToArray();
            return GetMatchingChar(arr[0].Contents, arr[1].Contents, arr[2].Contents);
        });

        var priorities = badgeItemTypes.Select(GetCharValue);
        return priorities.Sum();
    }

    private static void Main(string[] args)
    {
        var lines = File.ReadLines(@"./input.txt");
        var backpacks = lines.Select(x => new Backpack(x));

        Console.WriteLine(partOne(backpacks));     
        Console.WriteLine(partTwo(backpacks));         
    }
}

class Backpack {
    public string Contents { get; private set; }

    public (string, string) Compartments => 
        (Contents.Substring(0, Contents.Length / 2), 
        Contents.Substring(Contents.Length/2));

    public Backpack(string input) {
        Contents = input.Trim();
    }
}



