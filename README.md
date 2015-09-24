# Quarto

Ever wanted to know if your favorite [boolean proposition](https://en.wikipedia.org/wiki/Predicate_(mathematical_logic)) is satisfiable?

For example: 

A or (B and C) and (not D)

Can we assign truth values to A, B, C and D such that this results in true?

The bad news: this problem is NP hard. The good news: There are [lots of](http://www.satlive.org/solvers/) wonderful tools called SAT solvers that can, usually, solves these types of 
propositions with lightning speed! 

But, the are very picky about their input, it needs to be the [dimacs cnf format](http://www.satcompetition.org/2004/format-benchmarks2004.html)

The above proposition in dimacs is:

p cnf 4 3 
1 2 0 
1 3 0 
1 -4 0

So you're saying that these tools can solve NP hard problems but don't do a little bit of transformation on their input? 

Yup! But I fixed it for you. This little DSL in F# lets you type nice propositions and it will output dimacs.

You define variables with string names like this:

let A = Var "A"
etc.

And then you can make propositions with And, Or and Not:

let clause = A or (B and C) and (not D)

with a simple

let thatLooksNicer = makeDimacs clause

you get the equivalent dimacs fomat.

Happy SAT solving!

Felienne
