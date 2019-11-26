# F# Equity Trading Testing

BackTester is a F# functional language software, which aims to coming up with a trading stock strategy by validating and testing historical stock data. 

1. Types.fs

Types.fs sets various configuration variables such as the stock ticker and the number of days for the moving averages. 
  a.  stock data (Trade time, Tikcer, NumShares, Price) and price action (open, high, low, close) are also defined.
  b.  After Containers for stock data and average moving average are defined, a model for simple moving average maintating a squence of period unioned with price action models is introducted. 
  c. The driver of json parsing is contained. 

2. API.fs

API.fs called an API from alphaavantage: "https://www.alphavantage.co/query?%s&outputsize=full&apikey=" to retrieve the stock data.

API.fs constructs a JsonParser from top level, which parses trend and stock data to generic structure if json format. Then GetApiUrl queryType() function builds the url to fetch data from based off the value of the APIQueryParameters discriminated union type. Next, GetJson parserTypeReference queryType functions calls api to parse trend and stock data. The queryType param is the value of the APIQueryParameters type union, and the parserTypeReference param is a pointer to the specified parser function. GetStockData ticker and GetTrend ticker formulates the entry point to the API module. 

3. Trader.fs

Trader.fs passes options to currentexecution to test strategy.

Type currentexecution sets various functions such as helper function, assert common function, and result object that determines the final results of trading strategy. Meanwhile, it implements the buy and sell options for trader:
  a. curOpen: number of hold stock
  b. curShare is number to Trade
  c. pl is stock to track, and reflects current progress
  d. buy and sell stock options are defined
  
4. Runner.fsx

Runner.fsx is the program for testing the whole project. 

Runner.fsx picks up the GOOGLE stock as testing sample, and defines 50-day low moving average and 200-day high moving average.

Then we defines our algorithms based on financial tool -- MovingAverageStrategy:
  1. for buying stock. When our shorter 50-day SMA crosses above the longer-term (200-day)MA, it's a buy signal. We buy stock as it indicates that the trend is shifting up.
  2. for selling stock. When the shorter 50-day SMA crosses below the 200-day MA, it's a sell signal. We sell stock as it indicates that the trend is shifting down.
  
 The results of the algorithm are returned to the execution script. 

  
 5. Plotter.fs
 
 Plotter.fs implements Xplot graph tool to output our result in chart.
 
 We defines BUY_COLOR = "green", SELL_COLOR = "red", SCATTER_TYPE = "markers", and SCATTER_MARKER_SIZE = 10. Then we create plot and legend for traders. 
 Plotters drives a function for plotting stock data and moving averages, and implements Plotdata type as entrance function to pass to the drive stock and trend plotter. 
 Runners calls tradeResults and stockData to pass the results to display in the GUI.
