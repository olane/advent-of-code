var lines = File.ReadLines("input.txt");

List<Part> parts = new List<Part>();
List<Symbol> symbols = new List<Symbol>();

var y = 0;

foreach(var line in lines) {
    var x = 0;

    var currentLength = 0;
    var currentNumber = 0;

    foreach(var c in line) {
        if(int.TryParse(c.ToString(), out var n)) {
            currentNumber *= 10;
            currentNumber += n;
            currentLength++;
        }
        else {
            if (currentLength > 0) {
                parts.Add(new Part(currentNumber, currentLength, x - currentLength, y));
                currentLength = 0;
                currentNumber = 0;
            }

            if (c != '.') {
                symbols.Add(new Symbol(c, x, y));
            }
        }

        x++;
    }

    if(currentLength != 0) {
        parts.Add(new Part(currentNumber, currentLength, x - currentLength, y));
    }

    y++;
}

bool IsAdjacent(Part part, Symbol s) {
    int startYSearch = part.startY - 1;
    int startXSearch = part.startX - 1;
    int endYSearch = part.startY + 1;
    int endXSearch = part.startX + part.length;
    
    return startXSearch <= s.x && s.x <= endXSearch && startYSearch <= s.y && s.y <= endYSearch;
}

// PART 1
bool IsValidPart(Part part, List<Symbol> symbols) {
    return symbols.Any(s => IsAdjacent(part, s));
}

var validPartNumbers = parts.Where(x => IsValidPart(x, symbols)).Select(x => x.number);
Console.WriteLine(validPartNumbers.Sum());

// PART 2
var gears = symbols.Where(s => s.symbol == '*');

int GearRatio(Symbol gear) {
    var adjParts = parts.Where(p => IsAdjacent(p, gear)).ToArray();

    if (adjParts.Length != 2) {
        return 0;
    }

    return adjParts[0].number * adjParts[1].number;
}

Console.WriteLine(gears.Select(GearRatio).Sum());


// x and y start 0,0 top left
record Part(int number, int length, int startX, int startY);
record Symbol(char symbol, int x, int y);