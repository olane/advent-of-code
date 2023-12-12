
var lines = File.ReadLines("testinput.txt");

// var arrangements1 = lines.Select(Part1);
// Console.WriteLine(arrangements1.Sum());


var arrangements2 = lines.Select(x => {
    var split = x.Split(" ");
    return String.Join("?", Enumerable.Repeat(split[0], 5)) + " " + String.Join(",", Enumerable.Repeat(split[1], 5));
});

Console.WriteLine(arrangements2.Select(Part1).Sum());

int Part1(string line) {
    var split = line.Split(" ");
    var groups = split[1].Split(",").Select(int.Parse);
    var field = split[0];

    var unknownCount = field.Count(c => c == '?');
    
    var count = 0;

    var unknownDamageCount = groups.Sum() - field.Count(c => c == '#');
    var initialPerm = Enumerable.Repeat('#', unknownDamageCount).Concat(Enumerable.Repeat('.', unknownCount - unknownDamageCount));
    var allPerms = GenerateUniquePermutations(initialPerm.ToArray()).Select(v => new string(v));

    Console.WriteLine(split[1]);
    Console.WriteLine(split[0]);
    foreach (var perm in allPerms)
    {
        var k = 0;
        var permArr = perm.ToArray();

        var thisTestCase = new char[field.Length];
        for (int i = 0; i < field.Length; i++)
        {
            if (field[i] != '?') {
                thisTestCase[i] = field[i];
            }
            else {
                thisTestCase[i] = permArr[k];
                k++;
            }
        }

        var testString = new string(thisTestCase);
        if (SatisfiesGroups(testString, groups)) {
            count++;
           // Console.WriteLine(testString);
        }
    }

    Console.WriteLine(count);
    return count;
}

bool SatisfiesGroups(string field, IEnumerable<int> groups) {
    var fieldGroups = field.Split(".").Where(s => s.Length > 0).Select(s => s.Length);
    return fieldGroups.SequenceEqual(groups);
}

static IEnumerable<char[]> GenerateUniquePermutations(char[] array)
{
    // Sort the array to group identical permutations together
    Array.Sort(array);

    while (true)
    {
        char[] uniquePermutation = (char[])array.Clone();
        yield return uniquePermutation;

        // Find the next lexicographically greater permutation
        int i = array.Length - 2;
        while (i >= 0 && array[i] >= array[i + 1])
        {
            i--;
        }

        if (i < 0)
        {
            // All permutations generated
            break;
        }

        int j = array.Length - 1;
        while (array[j] <= array[i])
        {
            j--;
        }

        // Swap elements at i and j
        Swap(array, i, j);

        // Reverse the elements after i
        Reverse(array, i + 1, array.Length - 1);
    }
}

static void Swap(char[] array, int i, int j)
{
    char temp = array[i];
    array[i] = array[j];
    array[j] = temp;
}

static void Reverse(char[] array, int start, int end)
{
    while (start < end)
    {
        Swap(array, start, end);
        start++;
        end--;
    }
}
