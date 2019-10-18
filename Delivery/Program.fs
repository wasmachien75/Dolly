open Delivery.Actions
open Delivery.Signature
open Delivery.Logger
open System

let deliver from _to = 
    let signature = createSignature from
    logInfo ("Generated signature: " + signature.FullString)
    let reportDef = copyFolderTo from _to |> findReportDefinitions
    logInfo ("Found report definition at: " + reportDef)
    let docWithSignature = getDocumentWithSignature reportDef signature
    logInfo "Appending signature..."
    writeDocument reportDef docWithSignature

[<EntryPoint>]
[<STAThread>]
let main argv =
    logInfo ("Starting at " + DateTime.Now.ToShortTimeString())
    let from = chooseFolder "Select report folder"
    logInfo ("Source folder: " + from)
    let _to = chooseFolder "Select destination folder"
    logInfo ("Destination folder " + _to)
    try
        deliver from _to
        logInfo "Delivery succeeded"
    with
       | _ as e -> 
       printfn "Error: %s" e.Message
       printfn "I will now quit. (Press F12 for stack trace)"
       match Console.ReadKey().Key with
        | ConsoleKey.F12 -> printfn "%s" e.StackTrace
        | _ -> ()
    Console.ReadLine() |> ignore
    0