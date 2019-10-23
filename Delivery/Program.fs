open Dolly.Actions
open Dolly.Signature
open Dolly.Logging
open Dolly.Config
open System

let deliver from _to = 
    let signature = createSignature from
    logInfo <| "Generated signature: " + signature.FullString
    let reportDef = copyFolderTo from _to |> findReportDefinitions
    logInfo <| "Found report definition at: " + reportDef
    let docWithSignature = getDocumentWithSignature reportDef signature
    logInfo "Appending signature..."
    writeDocument reportDef docWithSignature

let openInExplorerAndQuit (folder : string) = 
    System.Diagnostics.Process.Start (folder) |> ignore
    exit(0)

[<EntryPoint>]
[<STAThread>]
let rec main argv = //source folder can be argument
    logInfo <| "Starting at " + DateTime.Now.ToShortTimeString()
    let from = if Array.isEmpty argv then chooseFolder "Select report folder" else Array.head argv 
    logInfo <| "Source folder: " + from
    let _to = chooseFolder "Select destination folder"
    logInfo <| "Destination folder " + _to
    try
        deliver from _to
        logInfo "Delivery succeeded. Press enter to deliver another report, F12 to open the target folder, or Ctrl + C to quit."
        match Console.ReadKey().Key with
         | ConsoleKey.Enter -> main [||] |> ignore
         | ConsoleKey.F12 -> openInExplorerAndQuit _to
         | _ -> ()

    with
       | _ as e -> 
       printfn "Error: %s" e.Message
       printfn "I will now quit. (Press F12 for stack trace)"
       match Console.ReadKey().Key with
        | ConsoleKey.F12 -> printfn "%s" e.StackTrace
        | _ -> ()
    Console.ReadLine() |> ignore
    0