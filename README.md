# Intro

Ever wanted to know if your favorite [boolean proposition](https://en.wikipedia.org/wiki/Predicate_(mathematical_logic)) is satisfiable?

For example: 

A or (B and C and !D)

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

let clause = Or (A, And (And(B, C), Not (D)))

with a simple  

let thatLooksNicer = makeDimacs clause  

you get the equivalent dimacs format.  

Happy SAT solving!

# The basics

## Variables
 
Define vars by making a Var:
let A = Var "A"  
etc.

## Clauses 
Make clauses with And Or and Not:

let clause = A or (B and C) and (not D)  

## Helper functions

### Equal

I did not feel like including a real boolean equality symbol in my language, as it leads to a lot more rewrite rules, but there is a mapping, equal: 

let equal (a:Term, b:Term) =   
    And (Or (Not(a), b), Or (Not(b), a))  

### Superconjuction

When generating SAT input, often you are creating a conjunction of lots of things. this needs to hold, and this and that. Therefore I threw in a nice little helper:

let rec createAndClauseFromList (L: List<Term>) =   
    match L with  
    | a :: [] -> a  
    | a::tail -> And (a, createAndClauseFromList(tail))  

Ypu put in a list of terms and you get one big conjunction.

# How to use this library

## From F Sharp

Using the library from F Sharp is super easy:

 * Create a new F Sharp Project
 * Add a reference to CNFify
 * Add 'open CNFify' to the top of your .fs file
 * Make one big clause (maybe with the helper superconjunction) 
 * makeDimacs gives you a string to put into any SAT solver
 * theWholeShebang runs minisat and gives you a SAT result, UNSAT or the truth assignment per variable:

 satResult = 
     | SAT of List<string * bool>
     | UNSAT
 
## From C Sharp

The basics from C Sharp are easy too:
 * Create a new C Sharp Project
 * Add a reference to CNFify
 * Add 'using CNFify' to the top of your .cs file

But, the rest is a bit more involved unfortunately:

### Defining variables

Defining variables  is a bit more cumbersome from C Sharp. Each type you define in an F Sharp project automatically gets a constructor that starts with new. Thus, you make a new instance of a type with:

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

### Using the equal helper function

Because equal is a function and not a type, there is no 'new' constructor needed:

var E = CNFify.equal(A, B);

### Using the superconjunction helper function

F Sharp lists and C Sharp lists are not the same! So if you create a list of CNFify.Terms in your C Sharp code, you need a bit of magic to use th helper:

FSharpList<CNFify.Term> listFSharp = ListModule.OfSeq(listCSharp);  
CNFify.Term all = CNFify.createAndClauseFromList(listFSharp);  

### Generating the dimacs file

string letsPrintThis = CNFify.makeDimacs(clause)

### Running SAT

var SATresult = CNFify.theWholeShabang(clause)

This gives you a satResult that is or UNSAT or SAT with list of variables and their assignments:
    | SAT of List<string * bool>
    | UNSAT

# Quarto

The reason I made the DSL is because I wanted to generate SAT code for a specific goal: determining whether the board game Quarto can end in a tie. If you want full details, have a look at this slide deck (SlideShare)

[![Board Game Night](http://image.slidesharecdn.com/felienne-online-150630181342-lva1-app6891/95/a-board-game-night-with-geeks-attacking-quarto-ties-with-sat-solvers-1-638.jpg?cb=1435688136)](http://slideshare.net/Felienne/a-board-game-night-with-geeks-attacking-quarto-ties-with-sat-solvers)

This repo also contains the C# code that generates the Quarto propositions, you can use that as an inspiration.

# That sounds awesome, but I like Python better

No problem! There is a similar [library](https://github.com/netom/satispy) for Python.
