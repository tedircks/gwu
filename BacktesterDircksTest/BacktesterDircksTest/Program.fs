type OrderSide =
    | Buy
    | Sell
type OrderType =
    | Market
    | Limit

type Order(s: OrderSide, t: OrderType, p: float) =
    let mutable S = s
    member this.T = t
    member this.P = p
    member this.Side
        with get() = S
        and set(s) = S <- s

    member this.toggleOrderSide() =
        match S with
        | Buy -> S <- Sell
        | Sell -> S <- Buy


let temp = Order(Buy, Limit, 10.0)
printfn(temp.S)
temp.toggleOrderSide()
printfn(temp.S)

//type CounterMessage = 
//  | Update of float
//  | Reset

//module Helpers =
//    let genRandomNumber (n) =
//        let rnd = new System.Random()
//        float (rnd.Next(n, 100))

//let inbox = MailboxProcessor.Start(fun agent -> 
//    // Function that implements the body of the agent
//    printfn "dircks test"
//    let rec loop sum count = async {
//      // Asynchronously wait for the next message
//      let! msg = agent.Receive()
//      match msg with
//      | Reset -> 
//          // Restart loop with initial values
//          return! loop 0.0 0.0
     
//      | Update value -> 
//          // Update the state and print the statistics
//          let sum, count = sum + value, count + 1.0
//          printfn "Average: %f" (sum / count)
     
//          // Wait before handling the next message
//          do! Async.Sleep(1000)
//          return! loop sum count
//    }     
//    // Start the body with initial values
//    loop 0.0 0.0
//)

//let random = Helpers.genRandomNumber 5
//inbox.Post(Update random)