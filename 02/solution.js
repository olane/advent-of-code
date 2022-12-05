const fs = require('node:fs');
const data = fs.readFileSync('input.txt', 'utf8');

const array = data.toString().replace(/\r\n/g,'\n').split('\n');

const toSymbol = (code) => {
    switch (code) {
        case "A":
            return "rock";

        case "B":
            return "paper";

        case "C":
            return "scissors";
    }
}

const toStrategy = (code) => {
    switch (code) {
        case "X":
            return "lose"

        case "Y":
            return "draw";

        case "Z":
            return "win";
    }
}

const plays = array.map(line => {
    const parts = line.split(' ');
    return {
        elf: toSymbol(parts[0]),
        you: toStrategy(parts[1])
    }
});

const win = 6;
const draw = 3;
const loss = 0;
const handValues = {
    "rock": 1,
    "paper": 2,
    "scissors": 3
};
const valueToHand = {
    1: "rock",
    2: "paper",
    3: "scissors"
}

const winScore = (round) => {
    if(round.elf == round.you) {
        return draw;
    }

    if(((handValues[round.elf] -1 + 3) % 3) == (handValues[round.you] % 3)){
        return loss;
    }

    return win;
}

const getYourStrategy = (round) => {
    if(round.you == "draw") {
        return round.elf;
    }
    if(round.you == "win") {
        return valueToHand[(handValues[round.elf]) % 3 + 1]
    }
    else {
        //lose
        return valueToHand[(handValues[round.elf] + 1) % 3 + 1]
    }
}

const calcScore = (round) => {
    const you = getYourStrategy(round);

    return winScore({elf: round.elf, you: you}) + handValues[you];
}

const scores = plays.map(calcScore);
const totalScore = scores.reduce((a,b)=>a+b, 0);

console.log(totalScore);

