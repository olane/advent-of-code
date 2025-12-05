namespace Day1;

public static class Mover
{
    public static (int newPosition, int zeroes) Move2(int current, string input)
    {
        var sign = input[0] == 'L' ? -1 : 1;
        var move = sign * int.Parse(input[1..]);

        // Calculate new position (with proper modulo handling for negatives)
        var newPosition = (current + move) % 100;
        if (newPosition < 0) newPosition += 100;

        // Count zeros during rotation
        int zeros;
        if (move >= 0)
        {
            // Moving right (clockwise)
            zeros = FloorDiv(current + move, 100) - FloorDiv(current, 100);
        }
        else
        {
            // Moving left (counter-clockwise)
            zeros = FloorDiv(current - 1, 100) - FloorDiv(current + move - 1, 100);
        }

        return (newPosition, zeros);
    }

    private static int FloorDiv(int a, int b)
    {
        return (int)Math.Floor((double)a / b);
    }
}
