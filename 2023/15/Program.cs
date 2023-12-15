var lines = File.ReadLines("input.txt");

var line = lines.Single();

Console.WriteLine(line.Split(',').Sum(Hash));

var boxes = Enumerable.Range(0, 256).Select(n => new Box([])).ToArray();

foreach (var instruction in line.Split(','))
{
    Execute(instruction, boxes);
}

var boxSums = boxes.Select((b, i) => 
    (i+1) * boxes[i].lenses.Select((l, j) => (j+1)*l.focalLength).Sum()
);

Console.WriteLine(boxSums.Sum());

void Execute(string instruction, Box[] boxes) {
    var split = instruction.Split('=', '-');
    var label = split[0];
    
    var boxNumber = Hash(label);
    var box = boxes[boxNumber];

    if(instruction.Contains('=')){
        boxes[boxNumber] = box.With(label, int.Parse(split[1]));
    }
    else {
        boxes[boxNumber] = box.Without(label);
    }
}

int Hash(string str)
{
    var result = 0;
    foreach(char c in str) {
        result += c;
        result *= 17;
        result %= 256;
    }
    return result;
}

record Box(List<Lens> lenses)
{
    internal Box With(string label, int focalLength)
    {
        if (lenses.Any(l => l.label == label)){
            var newLenses = lenses.Select(l => l.label == label ? new Lens(label, focalLength) : l);
            return new Box(newLenses.ToList());
        }
        else {
            lenses.Add(new Lens(label, focalLength));
            return this;
        }
    }

    internal Box Without(string label)
    {
        return new Box(lenses.Where(l => l.label != label).ToList());
    }
}

record Lens(string label, int focalLength);
