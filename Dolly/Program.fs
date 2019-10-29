open Dolly.Actions
open Dolly.Signature
open Dolly.Logging
open Dolly.FolderMapping
open Dolly.Mailer
open System

let deliver from _to = 
    let signature = createSignature from
    getCommitLog from |> logInfo
    "Generated signature: " + signature.FullString |> logInfo
    let reportDef = copyFolderTo from _to |> findReportDefinitions
    "Found report definition at: " + reportDef |> logInfo
    let docWithSignature = getDocumentWithSignature reportDef signature
    logInfo "Appending signature..."
    writeDocument reportDef docWithSignature
    logInfo "Sending confirmation mail..."
    sendMail from _to

let openInExplorerAndQuit (folder : string) = 
    System.Diagnostics.Process.Start (folder) |> ignore
    exit(0)

let listenForKeyPress (key: ConsoleKey) fn = 
    if Console.ReadKey().Key = key then fn () else ()

[<EntryPoint>]
[<STAThread>]
let rec main argv = //source folder can be argument
    let customerMapping = readFolderMappingFromFile "mapping.xml"
    logInfo <| "Starting at " + DateTime.Now.ToShortTimeString()

    let from = 
        match Array.tryHead argv with
            | Some x -> x
            | None -> chooseSourceFolder

    logInfo <| "Source folder: " + from
    let _to = chooseTargetFolder <| customerFromPath customerMapping from
    logInfo <| "Destination folder " + _to
    try
        deliver from _to
        logInfo "Delivery succeeded. Press F12 to open the target folder, or Ctrl + C to quit."
        listenForKeyPress ConsoleKey.F12 <| (fun _ -> openInExplorerAndQuit _to)
    with
       | _ as e -> 
       printfn "Error: %s.\nI will now quit. (Press F12 for stack trace)" e.Message
       listenForKeyPress ConsoleKey.F12 (fun _ -> printfn "%s" e.StackTrace) 
    Console.ReadLine() |> ignore
    0