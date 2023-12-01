using System.Text.RegularExpressions;

var lines = File.ReadLines(@"./input.txt");

var (stackSet, instructions) = Parser.Parse(lines);
Console.WriteLine(Day5.PartOne(stackSet, instructions));

var (stackSet2, instructions2) = Parser.Parse(lines);
Console.WriteLine(Day5.PartTwo(stackSet2, instructions2));


public static class Day5 {

    public static string PartOne(CrateStackSet stackSet, IEnumerable<MoveInstruction> instructions) {
        foreach (var i in instructions)
        {
            DoMove(i, stackSet);
        }

        return stackSet.ReadTop();
    }

    public static string PartTwo(CrateStackSet stackSet, IEnumerable<MoveInstruction> instructions) {
        foreach (var i in instructions)
        {
            DoFancyMove(i, stackSet);
        }

        return stackSet.ReadTop();
    }

    private static void DoFancyMove(MoveInstruction i, CrateStackSet stackSet)
    {
        stackSet.Move(i.From, i.To, i.Count);
    }

    private static void DoMove(MoveInstruction i, CrateStackSet stackSet)
    {
        for (int crate = 0; crate < i.Count; crate++)
        {
            stackSet.Move(i.From, i.To);
        }
    }
}


public static class Parser {
    
    private static readonly string m_stackEndLine = " 1   2   3   4   5   6   7   8   9 ";
    private static readonly int m_StackCount = 9;

    public static (CrateStackSet, IEnumerable<MoveInstruction>) Parse(IEnumerable<string> lines)
    {
        var linesList = lines.ToList();
        var stackEndLineIndex = linesList.IndexOf(m_stackEndLine);

        var stackLines = linesList.Take(stackEndLineIndex).ToList();
        CrateStackSet stackSet = ParseStackSet(stackLines);

        var instructionLines = linesList.Skip(stackEndLineIndex + 2);

        var instructions = ParseInstructions(instructionLines);

        return (stackSet, instructions);
    }

    private static IEnumerable<MoveInstruction> ParseInstructions(IEnumerable<string> instructionLines)
    {
        return instructionLines.Select(x => ParseInstructionLine(x));
    }

    // format 'move 12 from 9 to 6'
    private static MoveInstruction ParseInstructionLine(string line)
    {
        Regex rx = new Regex(@"move (\d+) from (\d+) to (\d+)",
          RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var groups = rx.Match(line).Groups;
        
        return new MoveInstruction(int.Parse(groups[1].Value), int.Parse(groups[2].Value), int.Parse(groups[3].Value));
    }

    private static CrateStackSet ParseStackSet(List<string> linesList)
    {
        var stackSet = new CrateStackSet(m_StackCount);

        for (int i = linesList.Count - 1; i >= 0; i--)
        {
            AddLineToStacks(stackSet, linesList[i]);
        }

        return stackSet;
    }

    private static void AddLineToStacks(CrateStackSet stackSet, string line)
    {
        var index = 1;
        for (int stack = 1; stack <= m_StackCount; stack++)
        {
            if (line[index] != ' '){
                stackSet.AddToStack(stack, line[index]);
            }
            index += 4;
        }
    }
}

public record MoveInstruction(int Count, int From, int To);

public class CrateStackSet {
    private Dictionary<int, Stack<char>> Stacks;

    public CrateStackSet(int stackCount){
        Stacks = new Dictionary<int, Stack<char>>();

        // 1 indexed
        for (int i = 1; i <= stackCount; i++)
        {
            Stacks[i] = new Stack<char>();
        }
    }

    public void AddToStack(int stackNumber, char item) {
        Stacks[stackNumber].Push(item);
    }

    public void Move(int from, int to, int count = 1)
    {
        var tempStack = new Stack<char>();

        MoveN(Stacks[from], tempStack, count);
        MoveN(tempStack, Stacks[to], count);
    }

    private static void MoveN(Stack<char> from, Stack<char> to, int n){
        for (int i = 0; i < n; i++)
        {
            MoveOne(from, to);
        }
    }

    private static void MoveOne(Stack<char> from, Stack<char> to) {
        var item = from.Pop();
        to.Push(item);
    }

    public string ReadTop() {
        return new string(Stacks.Values.Select(x => x.Peek()).ToArray());
    }
}
