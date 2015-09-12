module CNFify
open System

type Term = 
    | Var of string
    | Not of Term
    | Or of Term * Term
    | And of Term * Term
    | True 
    | False

let equal (a:Term, b:Term) = 
    And (Or (Not(a), b), Or (Not(b), a))

let rec applyOnArgs (T, F) = 
    match T with
    | And (a,b) -> And (F a, F b) //apply the function on the arguments
    | Or (a,b) -> Or (F a, F b)
    | Not (a) -> Not (F a)
    | _  -> T

let rec equivalencies(T:Term) = 
    match T with
    | And (a, True) -> a
    | And (a, False) -> False

    | And (True, a) -> a
    | And (False, a) -> False

    | Or (a, True) -> True
    | Or (a, False) -> a

    | Or (True, a) -> True
    | Or (False, a) -> a

    | Or (a, Not b) when a=b -> True
    | And (a, Not b) when a=b -> False

    | Or (Not b, a) when a=b -> True
    | And (Not b, a) when a=b -> False

    | _ -> applyOnArgs(T, equivalencies) //apply equivalencies on the arguments

let rec deMorgan (T:Term) = 
    match T with 
    | Not (Not (a)) -> a //double negation

    | Not (Or (a,b)) -> And (Not a, Not b) //apply the real de Morgan
    | Not (And (a,b)) -> Or (Not a, Not b)

    | _ -> applyOnArgs(T, deMorgan) //apply de Morgan on the arguments

let rec distributive (T:Term) = 
    match T with
    | Or (p, And (a,b)) -> And (Or(p,a), Or (p,b)) //distributive rule
    | Or (And (a,b), p) -> And (Or(p,a), Or (p,b)) //distributive rule

    | _ -> applyOnArgs(T, distributive) //apply de distribution on the arguments

let rec normalize (T:Term) =  //apply normalize until fixpoint is reached
    let y= equivalencies (deMorgan (distributive (T))) in 
    
    if y=T then y else normalize (y)

//make a big /\ or all the list elements, added to make it easy to generate lots of separate things that
//should hold and then concatenate it
let rec createAndFromList (L: List<Term>) = 
    match L with
    | a :: [] -> a
    | a::tail -> And (a, createAndFromList(tail))

let rec createListfromAnd (T:Term) = 
    match T with
    | And ( Var a, Var b) -> [ Var a; Var b]
    | And (a, b) -> List.append (createListfromAnd a) (createListfromAnd b)
    | _ -> [T]

let appendZero (s:string):string = 
    s + " 0" 
 
let rec printMinisat (T:Term):string = 
    match T with 
    | Var s -> "1"+s
    | Not t -> "-" + printMinisat t 
    | Or (a,b) -> printMinisat a + " " + printMinisat b 

//takes in a list and prints it in dimacs format
let makeDimacs(T:Term) = 
    normalize T |>                      //normalize to CNF
    createListfromAnd |>                // make a list of all the disjunctions (clauses)
    List.map printMinisat |>            //print each clause
    List.map appendZero |>              //put a zero at the end (why? who knows?!)
    Seq.distinct                        //filter the duplicates


[<EntryPoint>]
let main argv =

    let s = Var "a"

    printfn "%s" (printMinisat s)
    


    
    let s = Console.ReadLine()
    
    0 // return an integer exit code
