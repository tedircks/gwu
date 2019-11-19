namespace BackTester.ZPlotter

open XPlot.Plotly
open BackTester.Shared

module Plotter = 

    // constants for colors and plot configuration
    let BUY_COLOR = "green"
    let SELL_COLOR = "red"
    let SCATTER_TYPE = "markers"
    let SCATTER_MARKER_SIZE = 10
    
    // create plot and legend for trades
    let PlotTrades trades =
        let SCATTER_TRADE_LEGEND = sprintf "PL $%f // BUY (%s) SELL (%s)" trades.PL BUY_COLOR SELL_COLOR
        Scatter(
            name = SCATTER_TRADE_LEGEND
            ,x = (
                trades.TradeList 
                |> List.map (
                    fun tradeData -> tradeData.TradeTime
                )
            )
            ,y = (
                trades.TradeList 
                |> Seq.map (
                    fun tradeData -> tradeData.Price
                )
            )
            ,mode = SCATTER_TYPE
            ,marker = 
                Marker(
                    color = (
                        trades.TradeList 
                        |> Seq.map (
                            fun tradeData -> 
                            if tradeData.NumShares > 0 then BUY_COLOR 
                            else SELL_COLOR)
                    )
                    ,size = SCATTER_MARKER_SIZE
                )
        ) :> Trace

    // driver for plotting stock data and moving averages
    let private plotData hasBonds stockdata = 
        match stockdata 
            with
            | Trend 
                {
                    SimpleMovingAverage = def; 
                    TrendMarcketDataList = trendData
                } -> 
                    Scatter(
                        name = def.Self
                        ,x = (
                            trendData 
                            |> Seq.map (
                                fun (
                                    date, 
                                    sma
                                    ) -> date
                            )
                        )
                        ,y = (
                            trendData 
                            |> Seq.map (
                                fun (
                                    date, 
                                    sma
                                    ) -> sma
                            )
                        )
                    ) :> Trace
            | Stock 
                {
                    Ticker = ticker; 
                    StockMarketData = stockData
                } -> 
                    Scatter(
                        name = ticker
                        ,x = (
                            stockData 
                            |> Seq.map (
                                fun (
                                    date, 
                                    _
                                    ) -> date
                            )
                        )
                        ,y = (
                            stockData 
                            |> Seq.map (
                                fun (
                                    _, 
                                    trade
                                    ) -> trade.Close
                            )
                        )
                    ) :> Trace
    
    // entrance function to pass to the drive stock and trend plotter
    let private PlotData stockdata =
        stockdata 
        |> Seq.map (
            plotData (
                stockdata 
                |> Seq.length = 1
            )
        )

    // pass tradeResults and stockData to our plotting functions
    type PlotType = static member ($) (_:PlotType, (tradeResults:Result, stockData:seq<StockMarketData>)) = 
        seq { 
            yield! PlotData stockData; 
            yield PlotTrades tradeResults
        } 
        |> Chart.Plot 
        |> Chart.Show
       
    // entrance into the module, will be called from the script that runs the application
    let inline Plot results = Unchecked.defaultof<PlotType> $ results