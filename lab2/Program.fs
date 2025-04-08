// Only 2 gender
type Gender =
    | Male
    | Female

// Define an interface for items with a Name property
type INamed =
    abstract member Name: string

module Visitor =

    type Visitor =
        { Name: string
          Age: int
          Gender: Gender }

        interface INamed with
            member this.Name = this.Name

    let create name age gender =
        { Name = name
          Age = age
          Gender = gender }

    let getLockerRoomDirection visitor =
        match visitor.Gender with
        | Male -> "Go to the left locker room"
        | Female -> "Go to the right locker room"

module Trainer =

    type Trainer =
        { Name: string
          Age: int
          ExperienceYears: int
          Specialization: string }

        interface INamed with
            member this.Name = this.Name

    let create name age exp spec =
        { Name = name
          Age = age
          ExperienceYears = exp
          Specialization = spec }

    let introduceTrainer trainer =
        printfn
            "%s is a trainer specializing in %s with %d years of experience."
            trainer.Name
            trainer.Specialization
            trainer.ExperienceYears

module Equipment =

    type Equipment =
        { Name: string
          Type: string
          IsAvailable: bool }

        interface INamed with
            member this.Name = this.Name

    let create name etype isAvailable =
        { Name = name
          Type = etype
          IsAvailable = isAvailable }

    let printAvailability equipment =
        if equipment.IsAvailable then
            printfn "%s (%s) is available." equipment.Name equipment.Type
        else
            printfn "%s (%s) is currently unavailable." equipment.Name equipment.Type

    let markAsUnavailable equipment = { equipment with IsAvailable = false }
    let markAsAvailable equipment = { equipment with IsAvailable = true }

type EquipmentInfo<'Details> =
    | EquipmentWithDetails of Equipment.Equipment * 'Details
    | Equipment of Equipment.Equipment

module Subscription =

    // use the types from Visitor and Trainer modules
    type Subscription =
        { Visitor: Visitor.Visitor
          Trainer: Trainer.Trainer option
          Months: int
          Price: float }

    let create visitor trainer months price =
        { Visitor = visitor
          Trainer = trainer
          Months = months
          Price = price }

    let assignTrainer subscription trainer =
        { subscription with
            Trainer = Some trainer }

    let getSubscriptionDetails subscription =
        match subscription.Trainer with
        | Some t ->
            sprintf
                "%s has a subscription for %d months with trainer %s, costing %.2f."
                subscription.Visitor.Name
                subscription.Months
                t.Name
                subscription.Price
        | None ->
            sprintf
                "%s has a subscription for %d months without a trainer, costing %.2f."
                subscription.Visitor.Name
                subscription.Months
                subscription.Price

module Gym =

    // use the types from Visitor, Trainer, Equipment, and Subscription modules
    type Gym =
        { Name: string
          Location: string
          Trainers: Trainer.Trainer list
          Visitors: Visitor.Visitor list
          Equipment: Equipment.Equipment list
          Subscriptions: Subscription.Subscription list }

        interface INamed with
            member this.Name = this.Name

    let create name location trainers visitors equipment subscriptions =
        { Name = name
          Location = location
          Trainers = trainers
          Visitors = visitors
          Equipment = equipment
          Subscriptions = subscriptions }

    let addVisitor gym visitor =
        { gym with
            Visitors = visitor :: gym.Visitors }

    let addTrainer gym trainer =
        { gym with
            Trainers = trainer :: gym.Trainers }

    let addEquipment gym equipment =
        { gym with
            Equipment = equipment :: gym.Equipment }

    let addSubscription gym subscription =
        { gym with
            Subscriptions = subscription :: gym.Subscriptions }

    let listVisitors gym =
        printfn "Visitors at %s:" gym.Name
        gym.Visitors |> List.iter (fun v -> printfn "- %s (%d years old)" v.Name v.Age)

    let listTrainers gym =
        printfn "Trainers at %s:" gym.Name
        gym.Trainers |> List.iter (fun t -> printfn "- %s (%s)" t.Name t.Specialization)

    let listAvailableEquipment gym =
        printfn "Available equipment at %s:" gym.Name

        gym.Equipment
        |> List.filter (fun e -> e.IsAvailable)
        |> List.iter (fun e -> printfn "- %s" e.Name)


// Find smth that have a 'Name' property in a list of items
let findByName<'T when 'T :> INamed> (nameToFind: string) (items: 'T list) : 'T option =
    items |> List.tryFind (fun item -> item.Name = nameToFind)

module Main =

    [<EntryPoint>]
    let main (argv: string array) =

        printfn "Creating entities..."
        let vlad = Visitor.create "Vlad" 25 Gender.Male
        let oleg = Trainer.create "Oleg" 35 10 "Bodybuilding"
        let dumbbell = Equipment.create "Dumbbell (20kg)" "Weight" true
        let monthlySub = Subscription.create vlad None 1 50.0

        printf "\nGet direction to locker room: "
        printfn "%s" (Visitor.getLockerRoomDirection vlad)

        printfn "\nSome work with Trainer:"
        Trainer.introduceTrainer oleg

        printfn "\nWorking with Equipment:"
        Equipment.printAvailability dumbbell
        let unavailableDumbbell = Equipment.markAsUnavailable dumbbell
        Equipment.printAvailability unavailableDumbbell
        let availableDumbbell = Equipment.markAsAvailable unavailableDumbbell
        Equipment.printAvailability availableDumbbell

        printfn "\nGet subscription details:"
        printfn "%s" (Subscription.getSubscriptionDetails monthlySub)
        let assignedSub = Subscription.assignTrainer monthlySub oleg
        printfn "\nGet subscription details again:"
        printfn "%s" (Subscription.getSubscriptionDetails assignedSub)

        printfn "\nCreating Gym and print info about Gym:"

        let sportLife =
            Gym.create "SportLife" "Kyiv Pozniaky" [ oleg ] [ vlad ] [ availableDumbbell ] [ assignedSub ]

        Gym.listVisitors sportLife
        Gym.listTrainers sportLife
        Gym.listAvailableEquipment sportLife

        let newVisitor = Visitor.create "Anna" 22 Gender.Female
        let updatedGym = Gym.addVisitor sportLife newVisitor
        Gym.listVisitors updatedGym

        let treadmill = Equipment.create "Treadmill" "Cardio" true
        let weights = Equipment.create "Dumbbells (5kg)" "Weight" true

        let treadmillInfo = EquipmentWithDetails(treadmill, "Last serviced: 2025-03-15")
        let weightsInfo = Equipment weights

        let showEquipmentExtraInfo equipmentBox =
            match equipmentBox with
            | EquipmentWithDetails(eq, details) -> printfn "%s (%s). Extra info: %s" eq.Name eq.Type details
            | Equipment eq -> printfn "%s (%s)." eq.Name eq.Type

        // check generic function
        let fitFun =
            Gym.create
                "FitFun"
                "Downtown"
                [ Trainer.create "Oleg" 30 7 "Strength" ]
                [ Visitor.create "Lena" 15 Gender.Female ]
                [ Equipment.create "Barbell" "Weight" true ]
                []

        printfn "\nSearching for items by name:"
        let foundVisitor = findByName "Lena" fitFun.Visitors
        let foundTrainer = findByName "Oleg" fitFun.Trainers
        let foundEquipment = findByName "Barbell" fitFun.Equipment

        printfn "Found visitor: %A" foundVisitor // { Name = "Lena"; ... }
        printfn "Found trainer: %A" foundTrainer // { Name = "Oleg"; ... }
        printfn "Found equipment: %A" foundEquipment // { Name = "Barbell"; ... }

        let notFound = findByName "Someone Not Here" fitFun.Visitors
        printfn "Not found: %A" notFound // None

        0
