namespace BackTester.Shared
open System

// base level models
type PriceAction = | Open | High | Low | Close
type StockData = { Open:float; High:float; Low:float; Close:float; Volume:int }

// container for stock data 
type StockDataElement = DateTime * StockData
// list of StockDataElements
type StockMarketDataList = StockDataElement seq

// container for simple moving average
type TrendDataElement = DateTime * float
// list of simple moving average trending data
type TrendDataList = TrendDataElement seq

// model for simple moving average maintating a squence of period unioned with price action models
type SimpleMovingAverage = | SimpleMovingAverage of timeSpan:int * price:PriceAction
with member this.Self = match this with | SimpleMovingAverage (period, price)  -> sprintf "SMA%d%A" period price

type Trend = {
    TrendMarcketDataList : TrendDataList 
    Stock : string
    SimpleMovingAverage : SimpleMovingAverage
} with member this.Self = sprintf "%s(%s)" this.SimpleMovingAverage.Self this.Stock

type Stock = {
    Ticker : string
    StockMarketData : StockMarketDataList
}

// ED : most likely the driver of json parsing
type StockMarketData = | Stock of Stock | Trend of Trend