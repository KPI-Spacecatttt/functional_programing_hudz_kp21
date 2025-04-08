// Only 2 gender
type Gender =
    | Male
    | Female

// Entity
type Equipment =
    { Name: string
      Type: string
      IsAvailable: bool }

type Visitor =
    { Name: string
      Age: int
      Gender: Gender }

type Trainer =
    { Name: string
      Age: int
      ExperienceYears: int
      Specialization: string }

type Subscription =
    { Visitor: Visitor
      Trainer: Trainer option
      Months: int
      Price: float }

type Gym =
    { Name: string
      Location: string
      Trainers: Trainer list
      Visitors: Visitor list }

//Fabric functions
let createVisitor name age gender =
    { Name = name
      Age = age
      Gender = gender }

let createTrainer name age exp spec =
    { Name = name
      Age = age
      ExperienceYears = exp
      Specialization = spec }

let createSubscription visitor trainer months price =
    { Visitor = visitor
      Trainer = trainer
      Months = months
      Price = price }

let createGym name location trainers visitors =
    { Name = name
      Location = location
      Trainers = trainers
      Visitors = visitors }

let createEquipment name etype isAvailable =
    { Name = name
      Type = etype
      IsAvailable = isAvailable }

// Functions for work with entities
let addVisitor gym visitor =
    { gym with
        Visitors = visitor :: gym.Visitors }

let addTrainer gym trainer =
    { gym with
        Trainers = trainer :: gym.Trainers }

let assignTrainer subscription trainer =
    { subscription with
        Trainer = Some trainer }

// Partial function
let assignTrainerJames subscription =
    { subscription with
        Trainer = Some(createTrainer "James" 25 6 "Group classes") }

let checkEquipmentAvailability equipment = equipment.IsAvailable

// Match
let wayToLockerRoom visitor =
    match visitor.Gender with
    | Male -> "Go left side"
    | Female -> "Go right side"

// Каррована функція
let calculatePrice months =
    fun pricePerMonth -> months * pricePerMonth

let calculatePriceByYear  = calculatePrice 12

// Tuple
let getDataBySubscription subscription =
    (subscription.Visitor.Name, subscription.Months, (float subscription.Months * subscription.Price))


// Main code
[<EntryPoint>]
let main (argv: string array) =

    printfn "Create gym:"
    let visitor = createVisitor "Vlad" 25 Male
    let trainer = createTrainer "Oleg" 35 10 "Bodybuilding"
    let gym = createGym "SportLife" "Kyiv Pozniaky" [ trainer ] [ visitor ]
    printfn "%A\n" gym

    printfn "Create subscription:"
    let subscription = createSubscription visitor None 12 3000.0

    // Konveer
    subscription
    |> assignTrainerJames
    |> fun sub ->
        printfn
            "Visitor: %s, Trainer: %s, Months: %d, Price: %.2f\n"
            sub.Visitor.Name
            sub.Trainer.Value.Name
            sub.Months
            sub.Price

    // I need a way to the locker room
    visitor |> wayToLockerRoom |> printfn "%s\n"

    printfn "Check subscription:"

    match getDataBySubscription subscription with
    | (name, months, totalPrice) when months < 3 ->
        printfn
            "Dear %s, Your subscription ends in less than 3 months. You have %d month(s) left. Total Price: %.2f"
            name
            months
            totalPrice
    | (name, months, totalPrice) ->
        printfn "Dear %s, Your subscription is valid for %d month(s). Total Price: %.2f" name months totalPrice



    0
