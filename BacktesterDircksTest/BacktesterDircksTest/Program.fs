open System
open System.Data
open System.Linq
open Microsoft.FSharp.Data
open Microsoft.FSharp.Linq
open FSharp.Data
//open System.Data.Entity  
//open Microsoft.FSharp.Data.TypeProviders  

//#r "System.Data.Entity.dll"  
//#r "FSharp.Data.TypeProviders.dll"  
//#r "System.Data.Linq.dll"  


[<Literal>]
let connectionString = 
    @"Data Source=laptop-8lhpnalu\sqlexpress;Initial Catalog=BackTesterDB;Integrated Security=True"
/// Copied from properties of database
//Source=(localdb)\Projects;Initial Catalog=TradingSystem;
//Integrated Security=True;
//Connect Timeout=30;Encrypt=False;TrustServerCertificate=False",
//Pluralize = true>

do
    use cmd = new SqlCommandProvider<"
        INSERT INTO LogTable (LogDate, LogLevel, LogMessage)
        VALUES (GETDATE(), 'test', 'test')
        " , connectionString>(connectionString)

    cmd.Execute() |> printfn "%A"


