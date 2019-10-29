module Dolly.Signature

open System.Diagnostics
open System
open System.Xml.Linq

type Signature = {Timestamp: DateTime; Hash: string} with
    member this.FullString = 
        let dateTime = this.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")
        [dateTime;" | SHA-1=";this.Hash] |> String.concat ""

let getShellOutput (input: string, workingDir: string) = 
    let proc, info = new Process(), new ProcessStartInfo()
    info.UseShellExecute <- false
    info.RedirectStandardOutput <- true
    info.RedirectStandardError <- true
    info.WorkingDirectory <- workingDir
    info.FileName <- "cmd.exe"
    info.Arguments <- sprintf "/c %s" input
    proc.StartInfo <- info
    proc.Start() |> ignore
    proc.WaitForExit()
    let stdout, stderr, exitCode = proc.StandardOutput.ReadToEnd(), proc.StandardError.ReadToEnd(), proc.ExitCode
    proc.Close()
    match exitCode with
        | 0 -> stdout.Trim()
        | _ -> failwithf "Failure when retrieving shell output for command '%s' -> %s" (stderr.Trim()) <| input

let getCommitLog (folder: string) = 
    getShellOutput("git log --format=format:\"Last commit: %an - %aI - %s\" -n 1", folder)

let getCurrentCommitHash (folder: string) = 
    getShellOutput("git rev-parse --short HEAD", folder)
    
let createSignature folder = {Timestamp=DateTime.Now; Hash=folder |> getCurrentCommitHash}

let addCommentToDocument (comment: string) (doc: XDocument) = new XDocument(doc.Root, new XComment(comment))

let getDocumentWithSignature (path: string) (signature : Signature) = path |> XDocument.Load |> addCommentToDocument signature.FullString
