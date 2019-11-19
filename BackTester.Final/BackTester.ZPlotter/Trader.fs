namespace BackTester.Trader

open Deedle
open BackTester.Shared
open System.Collections.Generic

// pass options to currentExecution to test strategy
type CurrentExecution (options:Options) = 
    // local variables to track progress of trade execution
    let mutable cur = 1
    let mutable pl = 0.
    let mutable trades : TradeData list = []
    let curStocks = Dictionary<string, int * float>()

    // helper functions
    let Convert marketData =  match marketData with | Stock data -> [data.Ticker, data.StockMarketData |> series] |> frame | Trend data -> [data.Self, data.TrendMarcketDataList |> series] |> frame
    let HasValue (option:OptionalValue<_>) = match option with| OptionalValue.Present v -> Some v | OptionalValue.Missing -> None
    let item = options.MarketData |> Seq.map Convert |> Frame.mergeAll |> Frame.sortRowsByKey
    let GetTimeAt diff = item.GetRowKeyAt(int64 (cur + diff))
    
    // interface to assert common functions
    interface IResult with
        // the driver of the execution, this function will execute and track trades
        member this.ExecuteTrade(ticker, numToTrade) = 
            match (this :> IResult).GetOpenPrice(ticker, 0) with
            // if curOpen has value
            | Some curOpen -> 
                // if we current hold the stock
                match curStocks.TryGetValue(ticker) with
                | true, (curShares, chargePrice) ->
                    // if the sign of curShares == sign of numToTrade
                    if sign curShares = sign numToTrade then
                        // track pl
                        pl <- pl - (float numToTrade) * curOpen
                        let newCharge = ((float curShares) * chargePrice + (float numToTrade) * curOpen) / (float (curShares + numToTrade))
                        // modify the number of shares were holding
                        curStocks.[ticker] <- ((curShares + numToTrade), newCharge)
                        // track trade
                        trades <- 
                            {
                                TradeTime = GetTimeAt 0; 
                                Ticker = ticker; 
                                NumShares = numToTrade; 
                                Price = curOpen
                            } 
                            :: trades
                    else
                        if abs numToTrade > abs curShares then
                            // modify pl to track our current progress
                            pl <- pl + (float curShares) * curOpen
                            pl <- pl - (float (curShares + numToTrade)) * curOpen
                            // modify the number of shares were holding
                            curStocks.[ticker] <- ((curShares + numToTrade), curOpen)
                            // sell shares
                            trades <- 
                                {
                                    TradeTime = GetTimeAt 0; 
                                    Ticker = ticker; 
                                    NumShares = -curShares; 
                                    Price = curOpen
                                } 
                                :: trades
                            // buy shares
                            trades <- 
                                {
                                    TradeTime = GetTimeAt 0; 
                                    Ticker = ticker; 
                                    NumShares = (curShares + numToTrade); 
                                    Price = curOpen
                                } 
                                :: trades
                        else 
                            // modify pl to track our current progress
                            pl <- pl + (float numToTrade) * chargePrice
                            if (curShares + numToTrade) <> 0 then curStocks.[ticker] <- ((curShares + numToTrade), chargePrice) else curStocks.Remove(ticker) |> ignore
                            // sell shares
                            trades <- 
                                {
                                    TradeTime = GetTimeAt 0; 
                                    Ticker = ticker; 
                                    NumShares = -numToTrade; 
                                    Price = chargePrice
                                } 
                                :: trades
                | _ -> 
                    // modify pl and the number of shares we hjave
                    pl <- pl - (float numToTrade) * curOpen
                    curStocks.[ticker] <- (numToTrade, curOpen)
                    trades <- 
                        {
                            TradeTime = GetTimeAt 0; 
                            Ticker = ticker; 
                            NumShares = numToTrade; 
                            Price = curOpen
                        } 
                        :: trades
                true
            | _ -> false
        // helper override abstract properties
        member this.GetTrend(id, diff) = 
            let col = item.GetColumn<float>(id)
            col.TryGetAt(cur + diff) |> HasValue
        member this.GetTime(diff) = GetTimeAt diff
        member this.GetCurrentPosition(ticker) = match curStocks.TryGetValue(ticker) with | true, result -> Some result | _ -> None
        member this.GetStock(ticker, diff) = 
            let temp = item.GetColumn<StockData>(ticker) 
            temp.TryGetAt(cur + diff) |> HasValue
        member this.GetOpenPrice(symbol, diff) = (this :> IResult).GetStock(symbol, diff) |> Option.map (fun stockData -> stockData.Open)
        member this.GetClosePrice(symbol, diff) = (this :> IResult).GetStock(symbol, diff) |> Option.map (fun stockData -> stockData.Close)

    // result object that holds the final results of the trading strategy
    member this.Result = 
        { 
            Holdings = curStocks
            TradeList = List.rev trades
            PL = pl + (curStocks.Values |> Seq.fold (fun trades (numToTrade, cost) -> trades + (float numToTrade) * cost) 0.)
        }

    // iterator for the market data
    member this.GetNextItem() = if cur < item.RowCount - 1 then cur <- cur + 1; true else false

module Runner =
    let Execute exe = 
        let result = CurrentExecution(exe)

        let rec Go() = 
            result :> IResult 
            |> exe.Strategy
            if result.GetNextItem() 
            then Go()

        Go()
        result.Result
