# Intro

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

you get the equivalent dimacs format.  

Happy SAT solving!

# How to use this library

## From F Sharp

Using the library from F Sharp is super easy:

 * Create a new F Sharp Project
 * Add a reference to CNFify
 * Add 'open CNFify' to the top of your .fs file
 
### Defining variables
 
Define vars by making a Var: let x = Var "b"

### Creating clauses 
Make clauses with And Or and Not as explained above

## From C Sharp

From C# it is a bit more involved:
 * Create a new C Sharp Project
 * Add a reference to CNFify
 * Add 'using CNFify' to the top of your .cs file

### Defining variables

Defining variables too is a bit more cumbersome from C Sharp. Each type you define in an F Sharp project automatically gets a constructor that starts with new. Thus, you make a new instance of a type with:

var name = CNFify.Term.NewVar("name");  
var name2 = CNFify.Term.NewVar("name2");  

### Creating clauses 

In a similar fasion, you can make a new 'And':

var bothNames = CNFify.Term.NewAnd(name, name2);  

A word of advice: the name of the string, not the variable are mapped!, So similar string will get the same variable is in dimacs, so if you define

var name = CNFify.Term.NewVar("name");  
var name2 = CNFify.Term.NewVar("name");  

name and name2 will be equal! This has bitten me in the ass, beware it does not happen to you too :)

If you need inspiration, this library comes with a demo project, Quarto, see below.

# Quarto

The reason I made the DSL is because I wanted to generate SAT code for a specific goal: determining whether the board game Quarto can end in a tie. If you want full details, have a look at this slide deck (SlideShare)

[![Board Game Night](http://image.slidesharecdn.com/felienne-online-150630181342-lva1-app6891/95/a-board-game-night-with-geeks-attacking-quarto-ties-with-sat-solvers-1-638.jpg?cb=1435688136)](slideshare.net/Felienne/a-board-game-night-with-geeks-attacking-quarto-ties-with-sat-solvers)

This repo also contains the C# code that generates the Quarto propositions, you can use that as an inspiration.
