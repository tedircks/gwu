open System
open System.Data
open System.Linq
open Microsoft.FSharp.Data
open Microsoft.FSharp.Linq
open FSharp.Data


[<Literal>]
let connectionString = 
    @"Data Source=laptop-8lhpnalu\sqlexpress;Initial Catalog=BackTesterDB;Integrated Security=True"
do
    use cmd = new SqlCommandProvider<"
        INSERT INTO LogTable (LogDate, LogLevel, LogMessage)
        VALUES (GETDATE(), 'test', 'test')
        " , connectionString>(connectionString)

    cmd.Execute() |> printfn "%A"


