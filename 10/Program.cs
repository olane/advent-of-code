var lines = File.ReadLines("input.txt");

var trace = getXTrace(lines).ToArray();
var interestingCycles = new int[] { 20, 60, 100, 140, 180, 220 };

var sum = interestingCycles.Sum(i => i * trace[i-1]);
Console.WriteLine(sum);

Console.Write(String.Join("\n", renderTrace(trace, 40)));

// trace[i-1] is the value of x _during_ cycle i.
IEnumerable<int> getXTrace(IEnumerable<string> lines) {
    var x = 1;

    foreach (var line in lines)
    {
        if (line == "noop") {
            yield return x;
            continue;
        }

        yield return x;
        yield return x;
        var addition = int.Parse(line.Split(" ")[1]);
        x += addition;
    }
}

bool shouldDraw(int spritePos, int crtPos) => Math.Abs(spritePos - crtPos) <= 1;

IEnumerable<string> renderTrace(IEnumerable<int> trace, int width) {
    int crtX = 0;
    string line = "";
    foreach (var spriteX in trace)
    {
        line += shouldDraw(spriteX, crtX) ? '#' : '.';
        
        if(++crtX >= width) {
            yield return line;
            crtX = 0;
            line = "";
        }
    }

    yield return line;
}
