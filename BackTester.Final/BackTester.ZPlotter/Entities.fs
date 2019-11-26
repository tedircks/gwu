namespace BackTester.Data

open System

type [<AllowNullLiteral>] Log() = 
    [<DefaultValue>]
    val mutable private id : int
    member this.Id
        with get() = this.Id
        and set(value) = this.Id <- value

    [<DefaultValue>]
    val mutable private logDate : DateTime
    member this.LogDate
        with get() = this.LogDate
        and set(value) = this.LogDate <- value

    [<DefaultValue>]
    val mutable private logLevel : string
    member this.LogLevel
        with get() = this.LogLevel
        and set(value) = this.LogLevel <- value

    [<DefaultValue>]
    val mutable private logMessage : string
    member this.LogMessage
        with get() = this.LogMessage
        and set(value) = this.LogMessage <- value

