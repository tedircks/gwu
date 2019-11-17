#I @"\\Mac\Home\Documents\GitHub\gwu\BackTester.Final\BackTester.ZPlotter\bin\Debug"
#r @"C:\Users\dircks\.nuget\packages\xplot.plotly\3.0.1\lib\netstandard2.0\XPlot.Plotly.dll"
#r @"C:\Users\dircks\.nuget\packages\deedle\2.1.0\lib\netstandard2.0\Deedle.dll"
#r @"C:\Users\dircks\.nuget\packages\fsharp.data\3.3.2\lib\netstandard2.0\FSharp.Data.dll"
#r @"\\Mac\Home\Documents\GitHub\gwu\BackTester.Final\BackTester.Shared\bin\Debug\netstandard2.0\BackTester.Shared.dll"
#r @"\\Mac\Home\Documents\GitHub\gwu\BackTester.Final\BackTester.ZPlotter\bin\Debug\netstandard2.0\BackTester.Trader.dll"
#r @"\\Mac\Home\Documents\GitHub\gwu\BackTester.Final\BackTester.ZPlotter\bin\Debug\netstandard2.0\BackTester.ZPlotter.dll"
#r @"\\Mac\Home\Documents\GitHub\gwu\BackTester.Final\BackTester.ZPlotter\bin\Debug\netstandard2.0\BackTester.ZAPI.dll"

open BackTester.Shared
open BackTester.ZPlotter
open BackTester.ZAPI
open BackTester.Trader

let STOCK = "GOOGL"
let MOVING_AVERAGE_LOW = 100
let MOVING_AVERAGE_HIGH = 200


let MovingAverageStrategy (strategy:IResult) =
    let curTrendLow = strategy.GetTrend(GET_TREND_LOW, 0)
    let prevTrendLow = strategy.GetTrend(GET_TREND_LOW, -1)
    let curTrendHigh = strategy.GetTrend(GET_TREND_HIGH, 0)
    let prevTrendHigh = strategy.GetTrend(GET_TREND_HIGH, -1)

    //TODO : create trading strategy with moving averages

    
let Execute strategy market = 
    {
        MarketData = market; 
        Strategy = strategy
    } 
    |> Runner.Execute



// TODO : get results from strategy and pass to plotter