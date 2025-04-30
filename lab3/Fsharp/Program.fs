namespace GymManagement

// Only 2 gender
type Gender =
    | Male
    | Female

// Define an interface for items with a Name property
type INamed =
    abstract member Name: string

type IDisplayable =
    // Print information about the object
    abstract member DisplayInfo : unit -> unit
    // Return a description of the object as a string
    abstract member GetDescription : unit -> string

module Visitor =

    type Visitor =
        { Name: string
          Age: int
          Gender: Gender }

        interface INamed with
            member this.Name = this.Name

        interface IDisplayable with
            member this.DisplayInfo() =
                printfn "Visitor Info: Name=%s, Age=%d, Gender=%A" this.Name this.Age this.Gender
            member this.GetDescription() =
                sprintf "Visitor: %s (%d y.o.)" this.Name this.Age

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

        interface IDisplayable with
             member this.DisplayInfo() =
                printfn "Trainer Info: Name=%s, Specialization=%s, Experience=%d years" this.Name this.Specialization this.ExperienceYears
             member this.GetDescription() =
                 sprintf "Trainer: %s (%s)" this.Name this.Specialization

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

        interface IDisplayable with
            member this.DisplayInfo() =
                let availability = if this.IsAvailable then "Available" else "Unavailable"
                printfn "Equipment Info: Name=%s, Type=%s, Status: %s" this.Name this.Type availability
            member this.GetDescription() =
                sprintf "Equipment: %s [%s]" this.Name this.Type

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
    type Gym =
        { Name: string
          Location: string
          Trainers: Trainer.Trainer list
          Visitors: Visitor.Visitor list
          Equipment: Equipment.Equipment list
          Subscriptions: Subscription.Subscription list }

        interface INamed with
            member this.Name = this.Name

        interface IDisplayable with
            member this.DisplayInfo() =
                 printfn "Gym Info: Name=%s, Location=%s. Contains %d trainers, %d visitors, %d equipment items."
                    this.Name this.Location this.Trainers.Length this.Visitors.Length this.Equipment.Length
            member this.GetDescription() =
                 sprintf "Gym: %s (%s)" this.Name this.Location


    let create name location trainers visitors equipment subscriptions =
        { Name = name
          Location = location
          Trainers = trainers
          Visitors = visitors
          Equipment = equipment
          Subscriptions = subscriptions }

    let addVisitor gym visitor = { gym with Visitors = visitor :: gym.Visitors }
    let addTrainer gym trainer = { gym with Trainers = trainer :: gym.Trainers }
    let addEquipment gym equipment = { gym with Equipment = equipment :: gym.Equipment }
    let addSubscription gym subscription = { gym with Subscriptions = subscription :: gym.Subscriptions }

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

module InterfaceFunctions =
    // Some function to demonstrate the use of the IDisplayable interface
    let printItemDescription (item: IDisplayable) =
        let description = item.GetDescription()
        printfn "Item Description: %s" description

    let findByName<'T when 'T :> INamed> (nameToFind: string) (items: 'T list) : 'T option =
        items |> List.tryFind (fun item -> item.Name = nameToFind)

// Own class
type GymMembership =
    val mutable private memberId : int
    val private startDate : System.DateTime
    val mutable private endDate : System.DateTime
    val mutable private isActive : bool


// Constructors
    new(id: int, start: System.DateTime, durationMonths: int) =
        { memberId = id
          startDate = start
          endDate = start.AddMonths(durationMonths)
          isActive = true }

    // Yearly constructor
    new(id: int) =
        let now = System.DateTime.Now
        GymMembership(id, now, 12)

    // Basic constructor
    new() =
        { memberId = 0
          startDate = System.DateTime.MinValue
          endDate = System.DateTime.MinValue
          isActive = false }

    member this.MemberID
        with get() = this.memberId
        and set(value) = this.memberId <- value

    member this.StartDate
        with get() = this.startDate

    member this.EndDate
        with get() = this.endDate
        and set(value) = this.endDate <- value

    member this.IsActive
        with get() = this.isActive && System.DateTime.Now < this.endDate
        and set(value) = this.isActive <- value

    member this.ExtendMembership(months: int) =
        if months > 0 then
            this.endDate <- this.endDate.AddMonths(months)
            this.isActive <- true
            printfn "Membership %d extended by %d months. New end date: %s" this.memberId months (this.endDate.ToString("yyyy-MM-dd"))
        else
            printfn "Cannot extend membership by zero or negative months."

    member this.GetRemainingDays() =
        if this.IsActive then
             let remaining = this.endDate - System.DateTime.Now
             if remaining.TotalDays > 0.0 then remaining.TotalDays else 0.0
        else
             0.0

    member this.DisplayMembershipStatus() =
        let status = if this.IsActive then "Active" else "Inactive"
        printfn "Membership ID: %d | Status: %s | End Date: %s | Remaining Days: %.0f"
                 this.memberId status (this.endDate.ToString("yyyy-MM-dd")) (this.GetRemainingDays())


module Main =

    [<EntryPoint>]
    let main (argv: string array) =

        printfn "Creating entities..."
        let vlad = Visitor.create "Vlad" 25 Gender.Male
        let oleg = Trainer.create "Oleg" 35 10 "Bodybuilding"
        let dumbbell = Equipment.create "Dumbbell (20kg)" "Weight" true
        let anna = Visitor.create "Anna" 22 Gender.Female
        let treadmill = Equipment.create "Treadmill" "Cardio" true

        let monthlySubVlad = Subscription.create vlad (Some oleg) 1 50.0
        let yearlySubAnna = Subscription.create anna None 12 450.0

        let sportLife =
            Gym.create "SportLife" "Kyiv Pozniaky" [ oleg ] [ vlad; anna ] [ dumbbell; treadmill ] [ monthlySubVlad; yearlySubAnna ]

        // interface demo
        printfn "\nUsing IDisplayable Interface"
        printfn "Calling printItemDescription for Visitor:"
        InterfaceFunctions.printItemDescription vlad

        printfn "\nCalling printItemDescription for Trainer:"
        InterfaceFunctions.printItemDescription oleg

        printfn "\nCalling printItemDescription for Gym:"
        InterfaceFunctions.printItemDescription sportLife

        // object expression demo
        printfn "\nCalling printItemDescription with Object Expression:"
        let customDisplayable =
            { new IDisplayable with
                  member _.DisplayInfo() = printfn "This is a custom displayable object."
                  member _.GetDescription() = "Custom Object Description"
            }
        InterfaceFunctions.printItemDescription customDisplayable

        // Class demo
        printfn "\nUsing GymMembership Class:"
        let membership1 = GymMembership(101, System.DateTime.Now.AddMonths(-6), 12)
        let membership2 = GymMembership(102)
        let membership3 = GymMembership()
        membership3.MemberID <- 103
        membership3.EndDate <- System.DateTime.Now.AddMonths(3)
        membership3.IsActive <- true

        membership1.DisplayMembershipStatus()
        membership2.DisplayMembershipStatus()
        membership3.DisplayMembershipStatus()

        printfn "\nExtending membership 1 by 3 months..."
        membership1.ExtendMembership(3)
        membership1.DisplayMembershipStatus()

        printfn "\nDeactivating membership 2..."
        membership2.IsActive <- false
        membership2.DisplayMembershipStatus()
        printfn "Remaining days for membership 2 (should be 0): %.0f" (membership2.GetRemainingDays())

        printfn "\nOriginal Functionality Demo ---"
        printfn "Locker room for Vlad: %s" (Visitor.getLockerRoomDirection vlad)
        Trainer.introduceTrainer oleg
        Equipment.printAvailability dumbbell
        printfn "Subscription details for Vlad: %s" (Subscription.getSubscriptionDetails monthlySubVlad)
        Gym.listVisitors sportLife
        Gym.listAvailableEquipment sportLife

        0