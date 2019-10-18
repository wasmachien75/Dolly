module Delivery.Logger

type LogLevel = Info | Warning | Debug

let private logger (level: LogLevel) msg  =
    printfn "%s: %s" (level.ToString()) msg  

let logInfo msg = logger Info msg
let logWarning msg = logger Warning msg
let logDebug msg = logger Debug msg