
var lines = File.ReadLines("./input.txt");
var position = 50;
var count = 0;

foreach (var line in lines)
{
    position = Move(position, line);
    if (position == 0)
    {
        count++;
    }
}

Console.WriteLine(count);

int Move(int current, string input)
{
    var sign = input[0] == 'L' ? -1 : 1;
    var move = sign * int.Parse(input[1..]);
    return (current + move) % 100;
}
