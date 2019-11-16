namespace BackTester.ZAPI

module API = 

    // base url to build off of because the query for stock data and moving average data are different
    let BASE_API_URL = Printf.StringFormat<string->string>(@"https://www.alphavantage.co/query?%s&outputsize=full&apikey=ME9K8QA8OQ9FH5HD")

    open System // need for access to datetime
    open FSharp.Data // helpers for json parsing
    open BackTester.Shared // need for models

    // top level jsonParser, used by trend and stock data parsers to get generic structure
    // of json format
    let ParseJson json = 
        match json with
        | JsonValue.Record [|_, JsonValue.Record [|_; _; _, time; _; _|]; _, seq|]
        | JsonValue.Record [|_, JsonValue.Record [|_; _; _, time; _; _; _|]; _, seq|] 
        | JsonValue.Record [|_, JsonValue.Record [|_; _; _, time; _; _; _; _|]; _, seq|]
        | JsonValue.Record [|_, JsonValue.Record [|_; _; _; _, time; _|]; _, seq|] 
            when fst (DateTime.TryParse(time.AsString())) -> 
                time.AsDateTime(),
                match seq with
                    | JsonValue.Record tickers -> tickers
                    | _ -> failwith "ParseJsonError -- Invalid ticker in json pull"
        | _ -> failwith "ParseJsonError -- Unable to fit json into JsonRecord options"

    // parser for trend model, uses top level parser
    let ParseTrendFromJson (date, json) : TrendDataElement = 
        match json with 
            | JsonValue.Record [|_,_volume|] -> (DateTime.Parse(date), _volume.AsFloat())
            | _ -> failwith "GetTrendFromJson Error"

    // parser for stock model, uses top level parser
    let ParseStockFromJson (date, json) : StockDataElement = 
        match json with
            | JsonValue.Record [| 
                _,_open; 
                _,_high; 
                _,_low; 
                _,_close; 
                _, _volume
                |] -> (DateTime.Parse(date), {
                    Open = _open.AsFloat(); 
                    High = _high.AsFloat(); 
                    Low = _low.AsFloat(); 
                    Close = _close.AsFloat(); 
                    Volume = _volume.AsInteger()
                })
            | _ -> failwith "GetStockFromJson Error"

    
    // discriminated union to act almost like an enum for determining what to append to the base url
    type APIQueryParameters = 
        | STOCK_DATA_QUERY of ticker:string
        | MOVING_AVERAGE_QUERY of ticker:string * period:int

    // function to build the url to fetch data from based off the value of the APIQueryParameters discriminated union type
    let GetApiUrl queryType = 
        let urlToAppend = 
           match queryType with
            | MOVING_AVERAGE_QUERY (ticker, period) -> sprintf "function=SMA&symbol=%s&interval=daily&time_period=%d&series_type=close" ticker period 
            | STOCK_DATA_QUERY ticker -> sprintf "function=TIME_SERIES_DAILY&symbol=%s" ticker
            | _ -> failwith "BuilApidUrl failure"
        sprintf BASE_API_URL urlToAppend

    // function that actually makes the api call
    // send it to the appropriate parser
    // the queryType param is the value of the APIQueryParameters type union
    // the parserTypeReference param is a pointer to the specified parser function
    let GetJson parserTypeReference queryType = async {
        let! json = JsonValue.AsyncLoad(GetApiUrl queryType)    
        let date, dataSequence = ParseJson json
        return dataSequence |> Seq.map parserTypeReference
    }

    // function to make asynchronous call for stock data
    let GetStockDataAsync ticker = async {
        let! stockDataList = 
            STOCK_DATA_QUERY ticker
            |> GetJson ParseStockFromJson
        return {
            Ticker = ticker; 
            StockMarketData = stockDataList
        } 
        |> Stock
    }

    // function to make asynchronous call for trend data
    let GetTrendAsync ticker trendType = async {
        let! trendDataList = 
            match trendType with
                | SimpleMovingAverage (period, price) -> MOVING_AVERAGE_QUERY (ticker, period)
                |> GetJson ParseTrendFromJson
        return { 
            Stock = ticker; 
            SimpleMovingAverage = trendType; 
            TrendMarcketDataList = trendDataList 
        } 
        |> Trend
    }

    // entry point into the API module. These functions will pass the call off
    // to their async partner functions

    let GetStockData ticker = 
        GetStockDataAsync ticker  
        |> Async.RunSynchronously

    let GetTrend ticker trendType = 
        GetTrendAsync ticker trendType 
        |> Async.RunSynchronously

