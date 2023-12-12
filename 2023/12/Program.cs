
using System.Collections;

var lines = File.ReadLines("input.txt");

var cache = new Dictionary<(int length, int bits), IEnumerable<BitArray>>();

var arrangements1 = lines.Select(Part1);
Console.WriteLine(arrangements1.Sum());

var arrangements2 = lines.Select(x => {
    var split = x.Split(" ");
    return String.Join("?", Enumerable.Repeat(split[0], 5)) + " " + String.Join(",", Enumerable.Repeat(split[1], 5));
});

Console.WriteLine(arrangements2.Select(Part1).Sum());

int Part1(string line) {
    var split = line.Split(" ");
    var groups = split[1].Split(",").Select(int.Parse).ToArray();
    var field = split[0];

    var unknownCount = field.Count(c => c == '?');
    
    var count = 0;

    var unknownDamageCount = groups.Sum() - field.Count(c => c == '#');
    var initialPerm = Enumerable.Repeat('#', unknownDamageCount).Concat(Enumerable.Repeat('.', unknownCount - unknownDamageCount));
    // var allPerms = GenerateUniquePermutations(initialPerm.ToArray());

    var allPerms = BinStrings(unknownCount, unknownDamageCount).ToArray();

    Console.WriteLine(split[1]);
    Console.WriteLine(split[0]);

    foreach (var thisPermutation in allPerms)
    {
        var k = 0;

        var groupIndex = 0;
        var inThisGroupSoFar = 0;
        var rejected = false;

        for (int i = 0; i < field.Length; i++)
        {
            var thisChar = field[i] != '?' ? field[i] : thisPermutation[k++] ? '#' : '.';

            if(thisChar == '.' && inThisGroupSoFar > 0) {
                if(groups[groupIndex] == inThisGroupSoFar) {
                    // all good, continue
                    groupIndex++;
                    inThisGroupSoFar = 0;
                }
                else {
                    // violation
                    rejected = true;
                    break;
                }
            }
            else if (thisChar == '#') {
                inThisGroupSoFar++;

                if (inThisGroupSoFar > groups[groupIndex]) {
                    // violation
                    rejected = true;
                    break;
                }
            }
        }

        if(!rejected) {
            count++;
        }
    }
    Console.WriteLine(count);
    return count;
}


IEnumerable<BitArray> BinStrings(int length, int bits) {
    if(!cache.ContainsKey((length, bits))) {
        cache[(length, bits)] = GenBinStrings(length, bits).ToArray();
    }

    return cache[(length, bits)];
}

IEnumerable<BitArray> GenBinStrings(int length, int bitCount) {
    if(bitCount == 0) {
        yield return new BitArray(length, false);
    }
    else if (length == bitCount)
    {
        yield return new BitArray(length, true);
    }
    else
    {
        int first = length / 2;
        int last = length - first;
        int low = Math.Max(0, bitCount - last);
        int high = Math.Min(bitCount, first);
        for (int i = low; i <= high; i++)
        {
            foreach (var f in BinStrings(first, i))
            {
                foreach (var l in BinStrings(last, bitCount - i))
                {
                    yield return Append(f, l);
                }
            }
        }
    }
}

static BitArray Append(BitArray current, BitArray after) {
    var bools = new bool[current.Count + after.Count];
    current.CopyTo(bools, 0);
    after.CopyTo(bools, current.Count);
    return new BitArray(bools);
}
