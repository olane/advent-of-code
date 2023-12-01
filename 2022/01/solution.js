const fs = require('node:fs');
const data = fs.readFileSync('input.txt', 'utf8');

const array = data.toString().replace(/\r\n/g,'\n').split('\n');

const elves = [0];
let i = 0;
for (const item of array) {
    if(item == '') {
        i++;
        elves[i] = 0;
    }
    else {
        elves[i] += parseInt(item)
    }
}

elves.sort().reverse();

console.log(elves[0]);
console.log(elves[1]);
console.log(elves[2]);

console.log(elves[0] + elves[1] + elves[2]);
