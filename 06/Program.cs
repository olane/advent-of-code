var input = File.ReadAllText("input.txt");

Console.WriteLine(FirstUniqueSequence(input, 4));
Console.WriteLine(FirstUniqueSequence(input, 14));

static int FirstUniqueSequence(string input, int subsequenceLength)
{
    var index = 0;
    var bufferIndex = 0;
    var buffer = new char[subsequenceLength];
    foreach (char c in input)
    {
        buffer[bufferIndex] = c;

        bufferIndex = (bufferIndex + 1) % subsequenceLength;
        index++;

        if (index >= subsequenceLength && buffer.Distinct().Count() == subsequenceLength)
        {
            return index;
        }
    }

    throw new Exception("Not found");
}
