//dotnet new console -lang "F#" -o NameOfProject

// 'name' is inferred to be a string based on usage.
let printMessage name = printfn $"Hello there, {name}!"

// methods from c# can be called in F# code
System.Console.WriteLine("Hello World!")

// Mutable variables
let mutable age: int = 35
age <- 36

//Curried form for functions:
// param1 type -> param2 type -> ... -> paramN type -> return type
let add (x: int) (y: int) : int = x + y
let result = add 3 4  // Fully applied: returns 7

[<EntryPoint>]
let main (argv: string array) = 0 // return an integer exit code


// Conveyor, left to right
let result = 16 |> square |> double |> toString

// right to left
<| square <| double <| toString

// together
square >> double >> toString


// -----------------------------------------------------------------------------
// Type aliases
type ProductCode = string

//To define the unit of measurement, the corresponding attribute is used:
[<Measure>] type m
let x = 1<m> // int
let y = 1.0<m> // float

//Combines using * та /:
[<Measure>] type speed = m/sec
let speed = 10<m/sec>

let wtf = 1000<m> + 1<sec> // error
let wtf2 = 1000<m> + 1 // error
let halfDistance = 1000<m> / 2 // or

open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames - for SI units
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols - for SI units

// -----------------------------------------------------------------------------
// Tuple:
// type1 * type2 * ... * typeN
let t1 = 1, 2
let t2 = (1, 2)
let (x, _, z) = t3

//«tupled form» function
let sum (x, y) = x + y // (int * int) -> int


// -----------------------------------------------------------------------------
// Record:
type ComplexNumber =
    { Real: float
      Imaginary: float }

type Person = { Name: string; Age: int }
// Person.Name Person.Age
let { Name = letName, Age = letAge } = Person
let { Name = letName } = Person

// Anonymous records
let anonKyiv = {| Lat = 50.4542; Long = 30.5267 |}

// -----------------------------------------------------------------------------
//if-then-else
let v = if true then "a" else "b" // value : string

//match ... with
let number = <whatever>
let result =
    match number with
    | 1 -> "one"
    | 2 -> "two"
    | _ -> "unknown" // default

// function match
let check =
    function // function keyword
    | 1 -> "one"
    | 2 -> "two"
    | _ -> "unknown"

// OR
| 3 | 4 -> "three or four"

// null
let x = new Object()

    match x with
    | :? System.Int32 -> printfn "matched an int"
    | _ -> printfn "another type"

    match x with
    | null -> ...
    | _ -> ...

//when + as keywords

// -----------------------------------------------------------------------------
// for...to
for i = 1 to 10 do //increment
    printfn "%d" i
//or
for i = 10 downto 1 do //decrement
    printfn "%d" i

// for...in
for item in enumerable do
    printfn "%A" item

for i in 1 .. 10 do
    printfn "%d" i

for i in 10 .. -2 .. 1 do // step -2
    printfn "%d" i

// while ... do ...
while true do
    ...

// recursive
let rec factorial n = // rec keyword
    if n = 0 then 1
    else n * factorial (n - 1)

let tailSumNumbers limit =
    let rec inner current total =
    match current with
    | 0 -> total
    | x -> inner (current - 1) (current + total)
    inner limit 0

// -----------------------------------------------------------------------------
// Discriminated union
// Each component type (called a "union case") must be marked with a label
// ("case identifier"), and labels must start with a capital letter:
type IntOrBool = I of int | B of bool

type Disk =                             // type Disk =
| MMC of NumberOfPins: int              // | MMC of  int
| HardDisk of RPM: int * Platters: int  // | HardDisk of int * int // tuple

let disk1 = MMC 5
let disk2 = HardDisk (5400, 7)

// empty union case
type Size = Small | Medium | Large

// one case -> type-safe wrappers for basic types
type ValidatedEmail = Email of string

// classic enum
type Printer =
| Injket = 0
| Laserjet = 1
| DotMatrix = 2

// -----------------------------------------------------------------------------
// Namespaces and modules
namespace MyNamespace
module MyModule =
    let add x y = x + y

open MyNamespace.MyModule
let result = add 3 4

// namespace.module / module
module Utilities.MathStuff
let add x y = x + y
let subtract x y = x - y


// -----------------------------------------------------------------------------
// type ... with
type Person = { First: string; Last: string }

type Person with
    static member Create first last = { First = first; Last = last }
    member this.FullName = this.First + " " + this.Last // "this" or any other name

// immediately after the type definition
type Person = { First: string; Last: string } with
    member this.FullName = this.First + " " + this.Last

// for primitive types
type System.Int32 with
    member this.IsEven = this % 2 = 0


// Attributes ------------------------------------------------------------------
Attribute	Description
[<Sealed>]	For a type that has no abstract members, or that should not be extended.
[<Interface>]	For a type that is an interface.
[<AbstractClass>]	For a type that is an abstract class.
[<Struct>]	For a type that is a struct.
[<Measure>]	For a type that is a unit of measure.
[<EntryPoint>]	For the main entry point of the program.
[<Obsolete("message")>]	For a type or member that is obsolete. The message is optional.
[<Literal>]	For a value that is a compile-time constant.