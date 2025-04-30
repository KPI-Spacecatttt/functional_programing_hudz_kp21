open System
open System.IO
open FSharp.Data

[<Literal>]
let CsvFilePath = "Data/Visitors.csv"

type VisitorCsv = CsvProvider<CsvFilePath>

// Asynchronously loads data
let loadDataAsync (filePath: string) : Async<Option<VisitorCsv.Row[]>> =
    async {
        printfn "Try to load data from file: %s ..." filePath

        try
            let! csvData = VisitorCsv.AsyncLoad(filePath)
            let rows = csvData.Rows |> Seq.toArray
            return Some rows
        with ex ->
            printfn "Error loading or processing file '%s': %s" filePath ex.Message
            return None
    }

// Find the oldest visitor
let findOldestVisitorAsync (data: VisitorCsv.Row array) : Async<Option<VisitorCsv.Row>> =
    async {
        if data.Length = 0 then
            return None
        else
            let oldest = data |> Array.maxBy (fun visitor -> visitor.Age)
            return Some oldest
    }

// Average number of visits in the last month by gender
let calculateAverageVisitsByGenderAsync (data: VisitorCsv.Row array) : Async<Map<string, float>> =
    async {
        let avgVisits =
            data
            |> Array.groupBy (fun visitor -> visitor.Gender)
            |> Array.map (fun (gender, group) ->
                let avg = group |> Array.averageBy (fun visitor -> float visitor.VisitsLastMonth)
                (gender, avg))
            |> Map.ofArray

        return avgVisits
    }

// Total monthly fee for visitors older
let calculateTotalFeeForOlderVisitorsAsync (data: VisitorCsv.Row array) (ageThreshold: int) : Async<float> =
    async {
        let totalFee =
            data
            |> Array.filter (fun visitor -> visitor.Age > ageThreshold)
            |> Array.sumBy (fun visitor -> float visitor.MonthlyFee)

        return totalFee
    }

[<EntryPoint>]
let main (argv: string array) : int =
    printfn "Starting the program..."

    let analysisWorkflow =
        async {
            let! dataOption = loadDataAsync CsvFilePath

            match dataOption with
            | None -> printfn "Analysis not possible due to data loading error."
            | Some data when data.Length = 0 -> printfn "File is empty or contains no data."
            | Some data ->
                printfn "\nStarting data analysis..."

                let! oldestVisitorOpt = findOldestVisitorAsync data
                let! avgVisitsMap = calculateAverageVisitsByGenderAsync data
                let! totalFee = calculateTotalFeeForOlderVisitorsAsync data 30

                printfn "\nAnalysis Results..."

                match oldestVisitorOpt with
                | Some visitor ->
                    printfn "1. Oldest Visitor: Name=%s, Age=%d, Gender=%s" visitor.Name visitor.Age visitor.Gender
                | None -> printfn "1. No visitors found."

                printfn "\n2. Average monthly visits (by gender):"

                avgVisitsMap
                |> Map.iter (fun gender avg -> printfn "   - %s: %.1f visits" gender avg)

                printfn "\n3. Total monthly fee for visitors older than 30: %.2f" totalFee
        }

    analysisWorkflow |> Async.RunSynchronously

    0
