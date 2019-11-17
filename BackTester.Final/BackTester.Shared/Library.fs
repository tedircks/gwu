namespace BackTester.Shared

open System
open System.Collections.Generic

type TradeData = {
    TradeTime : DateTime
    Ticker : string
    NumShares : int
    Price : float
}

type Result = {
    Holdings : Dictionary<string, int * float>
    TradeList : TradeData list
    PL : float
}

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

type IResult = 
    abstract member GetTime : diff:int -> DateTime
    abstract member GetStock : ticker:string * diff:int -> StockData option
    abstract member GetOpenPrice : ticker:string * diff:int -> float option
    abstract member GetCurrentPosition : ticker:string -> (int * float) option
    abstract member ExecuteTrade : ticker:string * shares:int -> bool
    abstract member GetTrend : id:string * diff:int -> float option
    abstract member GetClosePrice : ticker:string * diff:int -> float option

type Options = {
    Strategy : IResult -> unit
    MarketData : seq<StockMarketData>
}