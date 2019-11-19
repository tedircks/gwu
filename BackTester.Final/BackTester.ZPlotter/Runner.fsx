#I @"\\Mac\Home\Documents\GitHub\gwu\BackTester.Final\BackTester.ZPlotter\bin\Debug"
#r @"C:\Users\dircks\.nuget\packages\xplot.plotly\3.0.1\lib\netstandard2.0\XPlot.Plotly.dll"
#r @"C:\Users\dircks\.nuget\packages\deedle\2.1.0\lib\netstandard2.0\Deedle.dll"
#r @"C:\Users\dircks\.nuget\packages\fsharp.data\3.3.2\lib\netstandard2.0\FSharp.Data.dll"
#r @"\\Mac\Home\Documents\GitHub\gwu\BackTester.Final\BackTester.ZPlotter\bin\Debug\netstandard2.0\BackTester.ZPlotter.dll"

open BackTester.Shared
open BackTester.ZAPI.API
open BackTester.ZPlotter
open BackTester.Trader

let STOCK = "GOOGL"
let MOVING_AVERAGE_LOW = 50
let MOVING_AVERAGE_HIGH = 200
let NUM_STOCKS_TO_TRADE_WHEN_HOLDING = 200
let NUM_STOCKS_TO_TRADE_WHEN_EMPTY = 100

let GET_TREND_LOW = sprintf "SMA%dClose(%s)" MOVING_AVERAGE_LOW STOCK
let GET_TREND_HIGH = sprintf "SMA%dClose(%s)" MOVING_AVERAGE_HIGH STOCK

let stockData = GetStockData STOCK
let movingAverageLow = SimpleMovingAverage (MOVING_AVERAGE_LOW, Close) |> GetTrend STOCK
let movingAverageHgih = SimpleMovingAverage (MOVING_AVERAGE_HIGH, Close) |> GetTrend STOCK

let MovingAverageStrategy (strategy:IResult) =
    let curTrendLow = strategy.GetTrend(GET_TREND_LOW, 0)
    let prevTrendLow = strategy.GetTrend(GET_TREND_LOW, -1)
    let curTrendHigh = strategy.GetTrend(GET_TREND_HIGH, 0)
    let prevTrendHigh = strategy.GetTrend(GET_TREND_HIGH, -1)

    match 
         curTrendLow
        ,curTrendHigh
        ,prevTrendLow
        ,prevTrendHigh
        with
          | Some curLowSMA, 
            Some curHighSMA, 
            Some prevLowSMA, 
            Some prevHighSMA ->
            let NUM_STOCKS_TO_TRADE = 
                match strategy.GetCurrentPosition(STOCK) 
                    with 
                    | Some _ -> NUM_STOCKS_TO_TRADE_WHEN_HOLDING 
                    | _ -> NUM_STOCKS_TO_TRADE_WHEN_EMPTY
            match curLowSMA - curHighSMA, prevLowSMA - prevHighSMA 
                with
                    | curSMADiff, prevSMADiff when curSMADiff > 0. && prevSMADiff <= 0. -> strategy.ExecuteTrade(STOCK, -NUM_STOCKS_TO_TRADE) |> ignore
                    | curSMADiff, prevSMADiff when curSMADiff < 0. && prevSMADiff >= 0. -> strategy.ExecuteTrade(STOCK, NUM_STOCKS_TO_TRADE) |> ignore
                    | _ -> printf "curSMADiff == 0"
            | _ -> printf "NO TRADE"

let MarketData = 
    [
        stockData; 
        movingAverageLow; 
        movingAverageHgih
    ]

let Execute strategy market = 
    {
        MarketData = market; 
        Strategy = strategy
    } 
    |> Runner.Execute

[
    MovingAverageStrategy, 
    MarketData
] 
|> List.map (fun market -> market ||> Execute, snd market |> List.toSeq) 
|> List.map Plotter.Plot 