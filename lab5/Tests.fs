namespace GymManagement.Tests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open FsUnit.MsTest
open GymManagement


[<TestClass>]
type VisitorTests() =

    let testVisitorMale = Visitor.create "Vlad" 25 Gender.Male
    let testVisitorFemale = Visitor.create "Anna" 22 Gender.Female

    [<TestMethod>]
    member this.``create should correctly assign properties``() =
        testVisitorMale.Name |> should equal "Vlad"
        testVisitorMale.Age |> should equal 25
        testVisitorMale.Gender |> should equal Gender.Male

    [<TestMethod>]
    member this.``create should handle different inputs``() =
        testVisitorFemale.Name |> should equal "Anna"
        testVisitorFemale.Age |> should equal 22
        testVisitorFemale.Gender |> should equal Gender.Female

    [<DataTestMethod>]
    [<DataRow(0, "Go to the left locker room")>]
    [<DataRow(1, "Go to the right locker room")>]
    member this.``getLockerRoomDirection should return correct direction``(gender: Gender, expectedDirection: string) =
        let visitor = Visitor.create "Test" 30 gender
        let direction = Visitor.getLockerRoomDirection visitor
        direction |> should equal expectedDirection

[<TestClass>]
type TrainerTests() =

    let testTrainer = Trainer.create "Oleg" 35 10 "Bodybuilding"
    let anotherTrainer = Trainer.create "Maria" 28 5 "Yoga"

    [<TestMethod>]
    member this.``create should correctly assign properties``() =
        testTrainer.Name |> should equal "Oleg"
        testTrainer.Age |> should equal 35
        testTrainer.ExperienceYears |> should equal 10
        testTrainer.Specialization |> should equal "Bodybuilding"

    [<TestMethod>]
    member this.``create should handle different trainer data``() =
        anotherTrainer.Name |> should equal "Maria"
        anotherTrainer.Specialization |> should equal "Yoga"
        anotherTrainer.ExperienceYears |> should equal 5

[<TestClass>]
type EquipmentTests() =

    let availableEquipment = Equipment.create "Dumbbell" "Weight" true
    let unavailableEquipment = Equipment.create "Treadmill" "Cardio" false

    [<TestMethod>]
    member this.``create should correctly assign properties``() =
        availableEquipment.Name |> should equal "Dumbbell"
        availableEquipment.Type |> should equal "Weight"
        availableEquipment.IsAvailable |> should be True

        unavailableEquipment.Name |> should equal "Treadmill"
        unavailableEquipment.IsAvailable |> should be False

    [<TestMethod>]
    member this.``markAsUnavailable should set IsAvailable to false``() =
        let updatedEquipment = Equipment.markAsUnavailable availableEquipment
        updatedEquipment.IsAvailable |> should be False
        availableEquipment.IsAvailable |> should be True

    [<TestMethod>]
    member this.``markAsAvailable should set IsAvailable to true``() =
        let updatedEquipment = Equipment.markAsAvailable unavailableEquipment
        updatedEquipment.IsAvailable |> should be True
        unavailableEquipment.IsAvailable |> should be False

[<TestClass>]
type SubscriptionTests() =

    let visitor = Visitor.create "Ivan" 30 Gender.Male
    let trainer = Trainer.create "Petro" 40 15 "CrossFit"
    let subWithoutTrainer = Subscription.create visitor None 6 250.0
    let subWithTrainer = Subscription.create visitor (Some trainer) 12 600.0

    [<TestMethod>]
    member this.``create should correctly assign properties without trainer``() =
        subWithoutTrainer.Visitor |> should equal visitor
        subWithoutTrainer.Trainer |> should equal None
        subWithoutTrainer.Months |> should equal 6
        subWithoutTrainer.Price |> should equal 250.0

    [<TestMethod>]
    member this.``create should correctly assign properties with trainer``() =
        subWithTrainer.Visitor |> should equal visitor
        subWithTrainer.Trainer |> should equal (Some trainer)
        subWithTrainer.Months |> should equal 12
        subWithTrainer.Price |> should equal 600.0

    [<TestMethod>]
    member this.``assignTrainer should add trainer to subscription``() =
        let updatedSub = Subscription.assignTrainer subWithoutTrainer trainer
        updatedSub.Trainer |> should equal (Some trainer)
        subWithoutTrainer.Trainer |> should equal None

    [<TestMethod>]
    member this.``getSubscriptionDetails should return correct string without trainer``() =
        let details = Subscription.getSubscriptionDetails subWithoutTrainer
        Assert.IsTrue(details.Contains("Ivan"))
        Assert.IsTrue(details.Contains("6 months"))
        Assert.IsTrue(details.Contains("without a trainer"))
        Assert.IsTrue(details.Contains("250.00"))

    [<TestMethod>]
    member this.``getSubscriptionDetails should return correct string with trainer``() =
        let details = Subscription.getSubscriptionDetails subWithTrainer
        Assert.IsTrue(details.Contains("Ivan"))
        Assert.IsTrue(details.Contains("12 months"))
        Assert.IsTrue(details.Contains("with trainer Petro"))
        Assert.IsTrue(details.Contains("600.00"))

[<TestClass>]
type GymTests() =

    let initialTrainer = Trainer.create "Olena" 32 8 "Pilates"
    let initialVisitor = Visitor.create "Sofia" 28 Gender.Female
    let initialEquipment = Equipment.create "Yoga Mat" "Accessory" true

    let initialSubscription =
        Subscription.create initialVisitor (Some initialTrainer) 3 120.0

    let emptyGym = Gym.create "Empty Gym" "Nowhere" [] [] [] []

    let populatedGym =
        Gym.create "FitZone" "Lviv" [ initialTrainer ] [ initialVisitor ] [ initialEquipment ] [ initialSubscription ]

    let newVisitor = Visitor.create "Andriy" 31 Gender.Male
    let newTrainer = Trainer.create "Max" 29 6 "Weightlifting"
    let newEquipment = Equipment.create "Barbell" "Weight" true

    [<TestMethod>]
    member this.``addVisitor should increase visitor count and add correct visitor``() =
        let updatedGym = Gym.addVisitor emptyGym newVisitor
        updatedGym.Visitors |> should haveLength 1
        updatedGym.Visitors |> should contain newVisitor
        emptyGym.Visitors |> should be Empty

    [<TestMethod>]
    member this.``addVisitor should add to existing list``() =
        let updatedGym = Gym.addVisitor populatedGym newVisitor
        updatedGym.Visitors |> should haveLength 2
        updatedGym.Visitors |> should contain newVisitor
        updatedGym.Visitors |> should contain initialVisitor

    [<TestMethod>]
    member this.``addTrainer should increase trainer count and add correct trainer``() =
        let updatedGym = Gym.addTrainer emptyGym newTrainer
        updatedGym.Trainers |> should haveLength 1
        updatedGym.Trainers |> should contain newTrainer

    [<TestMethod>]
    member this.``addTrainer should add to existing list``() =
        let updatedGym = Gym.addTrainer populatedGym newTrainer
        updatedGym.Trainers |> should haveLength 2
        updatedGym.Trainers |> should contain newTrainer
        updatedGym.Trainers |> should contain initialTrainer

    [<TestMethod>]
    member this.``addEquipment should increase equipment count and add correct equipment``() =
        let updatedGym = Gym.addEquipment emptyGym newEquipment
        updatedGym.Equipment |> should haveLength 1
        updatedGym.Equipment |> should contain newEquipment

    [<TestMethod>]
    member this.``addEquipment should add to existing list``() =
        let updatedGym = Gym.addEquipment populatedGym newEquipment
        updatedGym.Equipment |> should haveLength 2
        updatedGym.Equipment |> should contain newEquipment
        updatedGym.Equipment |> should contain initialEquipment

[<TestClass>]
type InterfaceFunctionsTests() =

    let visitors =
        [ Visitor.create "Vlad" 25 Gender.Male; Visitor.create "Anna" 22 Gender.Female ]

    let trainers =
        [ Trainer.create "Oleg" 35 10 "Bodybuilding"
          Trainer.create "Maria" 28 5 "Yoga" ]

    let equipment =
        [ Equipment.create "Dumbbell" "Weight" true
          Equipment.create "Treadmill" "Cardio" false ]

    [<DataTestMethod>]
    [<DataRow("Vlad", true)>]
    [<DataRow("NonExistent", false)>]
    member this.``findByName should work for Visitors``(nameToFind: string, shouldFind: bool) =
        let result = InterfaceFunctions.findByName nameToFind visitors

        match shouldFind with
        | true ->
            result.IsSome |> should be True
            result.Value.Name |> should equal nameToFind
        | false -> result |> should equal None

    [<DataTestMethod>]
    [<DataRow("Oleg", true)>]
    [<DataRow("Unknown", false)>]
    member this.``findByName should work for Trainers``(nameToFind: string, shouldFind: bool) =
        let result = InterfaceFunctions.findByName nameToFind trainers

        match shouldFind with
        | true ->
            result.IsSome |> should be True
            result.Value.Name |> should equal nameToFind
        | false -> result |> should equal None

    [<DataTestMethod>]
    [<DataRow("Treadmill", true)>]
    [<DataRow("Bench", false)>]
    member this.``findByName should work for Equipment``(nameToFind: string, shouldFind: bool) =
        let result = InterfaceFunctions.findByName nameToFind equipment

        match shouldFind with
        | true ->
            result.IsSome |> should be True
            result.Value.Name |> should equal nameToFind
        | false -> result |> should equal None

[<TestClass>]
type GymMembershipTests() =

    let startDate = System.DateTime.Now

    [<TestMethod>]
    member this.``Constructor should set properties correctly``() =
        let membership = GymMembership(id = 101, start = startDate, durationMonths = 6)

        membership.MemberID |> should equal 101
        membership.StartDate |> should equal startDate
        membership.EndDate |> should equal (startDate.AddMonths(6))

    [<TestMethod>]
    member this.``Yearly constructor should set 12 months duration``() =
        let membership = GymMembership(id = 102)
        let expectedEndDate = DateTime.Today.AddYears(1)

        membership.MemberID |> should equal 102
        membership.EndDate.Date |> should equal expectedEndDate.Date
        membership.IsActive |> should be True

    [<TestMethod>]
    member this.``ExtendMembership should increase EndDate for positive months``() =
        let membership = GymMembership(id = 103, start = startDate, durationMonths = 3)
        printfn "Status before: %A" membership.IsActive
        let initialEndDate = membership.EndDate

        membership.ExtendMembership(2)
        membership.EndDate |> should equal (initialEndDate.AddMonths(2))
        printfn "Status after: %A" membership.IsActive
        membership.IsActive |> should be True


    [<TestMethod>]
    member this.``ExtendMembership should not change EndDate for zero or negative months``() =
        let membership = GymMembership(id = 104, start = startDate, durationMonths = 3)
        let initialEndDate = membership.EndDate

        membership.ExtendMembership(0)
        membership.EndDate |> should equal initialEndDate
        membership.ExtendMembership(-1)
        membership.EndDate |> should equal initialEndDate

    [<TestMethod>]
    member this.``IsActive should be true if active flag is set and before end date``() =
        let membership =
            GymMembership(id = 105, start = DateTime.Today.AddMonths(-1), durationMonths = 3)

        membership.IsActive <- true
        membership.IsActive |> should be True

    [<TestMethod>]
    member this.``IsActive should be false if inactive flag is set``() =
        let membership =
            GymMembership(id = 106, start = DateTime.Today.AddMonths(-1), durationMonths = 3)

        membership.IsActive <- false
        membership.IsActive |> should be False

    [<TestMethod>]
    member this.``IsActive should be false if end date is in the past regardless of active flag``() =
        let membership = GymMembership(107, DateTime.Today.AddMonths(-6), 3)

        membership.IsActive <- true
        membership.IsActive |> should be False

    [<DataTestMethod>]
    [<DataRow(true, true, 10.0)>]
    [<DataRow(true, false, 0.0)>]
    [<DataRow(false, true, 0.0)>]
    [<DataRow(false, false, 0.0)>]
    [<TestMethod>]
    member this.``GetRemainingDays should return correct value``
        (isActiveFlag: bool, isFutureDate: bool, expectedApproxDays: float)
        =
        let membership = GymMembership()
        membership.MemberID <- 200
        membership.IsActive <- isActiveFlag

        membership.EndDate <-
            if isFutureDate then
                DateTime.Now.AddDays(expectedApproxDays)
            else
                DateTime.Now.AddDays(-10.0)

        let remaining = membership.GetRemainingDays()

        if expectedApproxDays > 0.0 then
            Assert.IsTrue(remaining >= expectedApproxDays - 0.1)
            Assert.IsTrue(remaining < expectedApproxDays + 0.1)
        else
            Assert.AreEqual(0.0, remaining, 0.0001)
