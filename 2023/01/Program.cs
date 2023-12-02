
var lines = File.ReadLines("input.txt");


//PART ONE
IEnumerable<IEnumerable<int>> onlyInts = lines.Select(line => toJustInts(line));
IEnumerable<int> calibrationValues = onlyInts.Select(x => toCalibrationValue(x));
Console.WriteLine(calibrationValues.Sum());

IEnumerable<int> toJustInts(string line) {
    foreach(char c in line) {
        if(int.TryParse(c.ToString(), out int n)){
            yield return n;
        }
    }
};

int toCalibrationValue(IEnumerable<int> intsInLine) => intsInLine.First() * 10 + intsInLine.Last();

//PART TWO

string[] numbers = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];


var values = lines.Select(l => calibrationValueFromLine(l));
Console.WriteLine(values.Sum());

int calibrationValueFromLine(string line) {
    return firstIntWithSpellings(line) * 10 + lastIntWithSpellings(line);
}


int firstIntWithSpellings(string line) {
    for(int i = 0; i < line.Length; i++) {
        if(int.TryParse(line[i].ToString(), out int n)){
            return n;
        }

        for (int j = 0; j < numbers.Length; j++)
        {
            int searchEnd = i+numbers[j].Length - 1;
            if(searchEnd >= line.Length) {
                continue;
            }

            if(line.Substring(i, numbers[j].Length) == numbers[j]){
                return j + 1;
            }
        }
    }

    return 0;
};


int lastIntWithSpellings(string line) {
    for(int i = line.Length - 1; i >= 0; i--) {
        if(int.TryParse(line[i].ToString(), out int n)){
            return n;
        }

        for (int j = 0; j < numbers.Length; j++)
        {
            int searchEnd = i+numbers[j].Length - 1;
            if(searchEnd >= line.Length) {
                continue;
            }

            if(line.Substring(i, numbers[j].Length) == numbers[j]){
                return j + 1;
            }
        }
    }

    return 0;
};
