module Dolly.Logging

type LogLevel = Info | Warning | Debug
let logLevel = Warning

let private logger (level: LogLevel) msg  =
    if level <= logLevel then printfn "%s: %s" (level.ToString()) msg  

let logInfo = logger Info
let logWarning = logger Warning
let logDebug = logger Debug