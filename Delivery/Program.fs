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
       printfn "I will now quit."
    Console.ReadLine() |> ignore
    0