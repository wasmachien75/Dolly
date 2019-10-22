module Dolly.Logging

type LogLevel = Info | Warning | Debug

let private logger (level: LogLevel) msg  =
    printfn "%s: %s" (level.ToString()) msg  

let logInfo = logger Info
let logWarning = logger Warning
let logDebug = logger Debug