open Dolly.Actions
open Dolly.Signature
open Dolly.Logging
open Dolly.FolderMapping
open Dolly.Push
open System

let endpointTeams = "https://outlook.office.com/webhook/fad37f7c-a3e6-42d9-b77e-8556115b6dab@00f6d248-8400-431b-b006-63cc85dc46c2/IncomingWebhook/c625d98de23940dd8a604cb13152a167/acc01fe7-e32a-49f5-908d-cbb95bc155e6"
let pushMessageAsync msg = pushMessageAsyncWithEndPoint msg endpointTeams


let getReportName (folder: string) = 
    folder.Split(System.IO.Path.DirectorySeparatorChar) |> Seq.last


let deliver from _to =
    let reportName = getReportName from
    let signature = createSignature from
    sprintf "Cloning report '%s'" reportName |> logInfo
    getCommitLog from |> logInfo
    "Generated signature: " + signature.FullString |> logInfo
    let reportDef = copyFolderTo from _to |> findReportDefinitions
    "Found report definition at: " + reportDef |> logInfo
    let docWithSignature = getDocumentWithSignature reportDef signature
    logInfo "Appending signature..."
    writeDocument reportDef docWithSignature
    let res = createMessage "Report delivery" reportName signature.LastGitHash |> pushMessageAsync 
    match res.Result.StatusCode |> int with
    | 200 -> "Sent acknowledgement to Teams" |> logInfo
    | i -> (sprintf "Could not send message to Teams (status code %d)" i) |> logInfo

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