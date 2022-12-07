
var input = File.ReadLines("input.txt");

var commands = GetCommands(input);

Dir root = BuildFileTree(commands);
PrintDir(root);

var allDirs = GetAllDirs(root);
var allSmallDirs = allDirs.Where(x => GetDirSize(x) <= 100000);
Console.WriteLine($"Sum of all small directory sizes: {allSmallDirs.Sum(x => GetDirSize(x))}");

var totalSpace = 70000000;
var neededSpace = 30000000;
var currentlyTaken = GetDirSize(root);
var currentlyFree = totalSpace - currentlyTaken;

var sizeToDelete = neededSpace - currentlyFree;

Console.WriteLine($"We have {totalSpace} but {GetDirSize(root)} taken. We need to delete {sizeToDelete}");

var bigEnoughDirs = allDirs.Where(x => GetDirSize(x) >= sizeToDelete).ToList();
var bigEnoughDirsBySize = bigEnoughDirs.OrderBy(x => GetDirSize(x));
var smallest = bigEnoughDirsBySize.First();

Console.WriteLine($"Smallest big enough dir to delete: {smallest.Name} {GetDirSize(smallest)}");

State ChangeDir(State state, string arg)
{
    var (root, currentDir) = state;

    if (arg == "..")
    {
        return new State(root, currentDir.Parent);
    }
    else if (arg == "/")
    {
        return new State(root, root);
    }
    else
    {
        var newDir = new Dir(arg, new List<Item>(), new List<Dir>(), currentDir);
        currentDir.Children.Add(newDir);
        return new State(root, newDir);
    }
}

State ListDir(State state, IEnumerable<string> outputLines)
{
    var (root, currentDir) = state;

    foreach (var line in outputLines)
    {
        var split = line.Split(" ");

        if (split[0] == "dir")
        {
            // ignore, we'll make it when we cd into it
        }
        else
        {
            var size = int.Parse(split[0]);
            var name = split[1];

            currentDir.Files.Add(new Item(name, size));
        }
    }

    return new State(root, currentDir);
}

IEnumerable<Command> GetCommands(IEnumerable<string> inputLines)
{
    var currentCommand = inputLines.First();
    var currentOutputList = new List<string>();

    foreach (var line in inputLines.Skip(1))
    {
        if (line.StartsWith('$'))
        {
            yield return new Command(currentCommand.Trim('$', ' '), currentOutputList);

            currentCommand = line;
            currentOutputList = new List<string>();
        }
        else
        {
            currentOutputList.Add(line);
        }
    }
}

IEnumerable<Dir> GetAllDirs(Dir root)
{
    return root.Children.SelectMany(x => GetAllDirs(x)).Append(root);
}

int GetDirSize(Dir dir)
{
    return dir.Files.Sum(x => x.Size) + dir.Children.Sum(d => GetDirSize(d));
}

void PrintDir(Dir dir, string prefix = "")
{
    Console.WriteLine(prefix + dir.Name + " " + GetDirSize(dir));
    dir.Files.ForEach(x => Console.WriteLine(prefix + "    " + x.Name + " " + x.Size));
    dir.Children.ForEach(x => PrintDir(x, prefix + "    "));
}

Dir BuildFileTree(IEnumerable<Command> commands)
{
    root = new Dir("/", new List<Item>(), new List<Dir>(), null);

    // (root, currentDir)
    var state = new State(root, root);
    foreach (var (command, outputLines) in commands)
    {
        var commandSplit = command.Split(' ');

        state = commandSplit switch
        {
            ["cd", var arg] => ChangeDir(state, arg),
            ["ls"] => ListDir(state, outputLines),
            _ => throw new Exception("command not parsed " + command)
        };
    }

    return root;
}

record State(Dir Root, Dir Current);
record Command(string command, IEnumerable<string> outputLines);
record Item(string Name, int Size);
record Dir(string Name, List<Item> Files, List<Dir> Children, Dir? Parent);




