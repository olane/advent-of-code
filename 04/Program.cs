internal class Program
{
    private static Range ReadRange(string rangeString) {
        var parts = rangeString.Split('-');
        return new Range(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    private static (Range, Range) ReadRangePair(string line) {
        var parts = line.Split(',');
        return (ReadRange(parts[0]), ReadRange(parts[1]));
    }

    private static int PartOne(IEnumerable<(Range, Range)> rangePairs){
        var pairsWithSubset = rangePairs.Where(x => x.Item1.IsSubset(x.Item2) || x.Item2.IsSubset(x.Item1));
        return pairsWithSubset.Count();
    }

    private static int PartTwo(IEnumerable<(Range, Range)> rangePairs){
        var pairsWithOverlap = rangePairs.Where(x => x.Item1.IsOverlapping(x.Item2));
        return pairsWithOverlap.Count();
    }

    private static void Main(string[] args)
    {
        var lines = File.ReadLines(@"./input.txt");

        var rangePairs = lines.Select(x => ReadRangePair(x));

        Console.WriteLine(PartOne(rangePairs));
        Console.WriteLine(PartTwo(rangePairs));
    }
}

internal class Range {
    public Range(int low, int high) {
        Low = low;
        High = high;
    }

    public bool IsSubset(Range other) {
        return other.Low >= this.Low && other.High <= this.High;
    }

    public bool IsOverlapping(Range other) {
        if(other.Low < this.Low && other.High < this.Low){
            return false;
        }
        if(other.Low > this.High && other.High > this.High){
            return false;
        }
        return true;
    }

    public int Low { get; }
    public int High { get; }
}

