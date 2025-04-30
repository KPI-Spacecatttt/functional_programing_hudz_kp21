using System;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;
using GymManagement;


public class Program
{
    public static void Main(string[] args)
    {

        // Create F# types
        var genderMale = Gender.Male;
        var genderFemale = Gender.Female;
        Console.WriteLine($"\nCreated Gender: {genderMale}");

        var csVisitor1 = Visitor.create("CSharp Visitor 1", 30, genderMale);
        var csVisitor2 = Visitor.create("CSharp Visitor 2", 28, genderFemale);
        Console.WriteLine($"Created Visitor: {csVisitor1.Name}, Age: {csVisitor1.Age}");

        var csTrainer = Trainer.create("CSharp Trainer", 45, 20, "Functional Training");
        Console.WriteLine($"Created Trainer: {csTrainer.Name}, Spec: {csTrainer.Specialization}");

        var csEquipment1 = Equipment.create("Barbell", "Weights", true);
        var csEquipment2 = Equipment.create("Elliptical", "Cardio", false);
        Console.WriteLine($"Created Equipment: {csEquipment1.Name}, Available: {csEquipment1.IsAvailable}");

        // FSharpOption<T>.None або FSharpOption<T>.Some(value)
        var noTrainerOption = FSharpOption<Trainer.Trainer>.None;
        var trainerOption = new FSharpOption<Trainer.Trainer>(csTrainer);

        var csSub1 = Subscription.create(csVisitor1, trainerOption, 6, 300.50);
        var csSub2 = Subscription.create(csVisitor2, noTrainerOption, 12, 550.0);
        Console.WriteLine($"Created Subscription for: {csSub1.Visitor.Name}");

        // Створення F# списків для використання в Gym
        // Потрібно використовувати Microsoft.FSharp.Collections.ListModule
        var trainersList = ListModule.OfArray(new[] { csTrainer });
        var visitorsList = ListModule.OfArray(new[] { csVisitor1, csVisitor2 });
        var equipmentList = ListModule.OfArray(new[] { csEquipment1, csEquipment2 });
        var subsList = ListModule.OfArray(new[] { csSub1, csSub2 });

        var csGym = Gym.create("Cross-Platform Gym", "Virtual Space", trainersList, visitorsList, equipmentList, subsList);
        Console.WriteLine($"Created Gym: {csGym.Name} at {csGym.Location}");

        // F# functions
        Console.WriteLine("\nCalling F# Functions");

        string direction = Visitor.getLockerRoomDirection(csVisitor1);
        Console.WriteLine($"Locker room direction for {csVisitor1.Name}: {direction}");

        Console.WriteLine("Trainer Introduction (from F#):");
        Trainer.introduceTrainer(csTrainer);

        string subDetails = Subscription.getSubscriptionDetails(csSub1);
        Console.WriteLine($"Subscription Details: {subDetails}");

        var csVisitor3 = Visitor.create("New CSharp User", 22, genderFemale);
        var updatedGym = Gym.addVisitor(csGym, csVisitor3);
        Console.WriteLine($"Added new visitor. Gym now has {updatedGym.Visitors.Length} visitors.");

        Console.WriteLine("Listing visitors from updated Gym (using F# function):");
        Gym.listVisitors(updatedGym);


        Console.WriteLine("\nCalling F# function that accepts IDisplayable:");
        InterfaceFunctions.printItemDescription(csVisitor1);
        InterfaceFunctions.printItemDescription(csTrainer);
        InterfaceFunctions.printItemDescription(csEquipment1);
        InterfaceFunctions.printItemDescription(csGym);


        // Usage GymMembership
        Console.WriteLine("\nUsing F# Class 'GymMembership'");
        var membership1 = new GymMembership(201, new DateTime(2024, 1, 15), 12);
        var membership2 = new GymMembership(202);
        var membership3 = new GymMembership();

        Console.WriteLine($"Created Membership 1 - ID: {membership1.MemberID}, Start: {membership1.StartDate.ToShortDateString()}");
        Console.WriteLine($"Created Membership 2 - ID: {membership2.MemberID}, End: {membership2.EndDate.ToShortDateString()}");
        Console.WriteLine($"Created Membership 3 - ID: {membership3.MemberID}, IsActive: {membership3.IsActive}");

        // get/set
        membership3.MemberID = 203;
        membership3.EndDate = DateTime.Now.AddMonths(1);
        membership3.IsActive = true;
        Console.WriteLine($"Updated Membership 3 - ID: {membership3.MemberID}, End: {membership3.EndDate.ToShortDateString()}, Active: {membership3.IsActive}");

        Console.WriteLine("\nCalling methods on GymMembership objects:");
        membership1.DisplayMembershipStatus();
        membership2.ExtendMembership(6);
        membership2.DisplayMembershipStatus();
        double remainingDays3 = membership3.GetRemainingDays();
        Console.WriteLine($"Membership 3 has {remainingDays3:F0} remaining days.");
    }
}