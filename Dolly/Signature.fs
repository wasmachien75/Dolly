module Dolly.Signature

open System.Diagnostics
open System
open System.Xml.Linq

type Signature = {Timestamp: DateTime; Hash: string; GitTimestamp: DateTime; LastGitHash: string} with
//"release=2018-01-01T12:00:00+02:00 | current git hash=d28b83a3 | last commit=2017-01-01T13:02:02+02:00 | last commit hash=6ba0231d
    member this.FullString = 
        let toStr (timestamp: DateTime) = timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")
        let currentTs, gitTs = this.Timestamp |> toStr, this.GitTimestamp |> toStr
        sprintf "release=%s | current git hash=%s | last commit=%s | last commit hash=%s" currentTs this.Hash gitTs this.LastGitHash

let getShellOutput (input: string) (workingDir: string) = 
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
    getShellOutput "git log --format=format:\"Last commit: %an - %aI - %s\" -n 1" folder

let getLastHashAndTimestamp (folder: string) = 
    let output = (getShellOutput "git log --format=format:\"%h %aI\" -n 1 ." folder).Split(' ')
    (output.[0], output.[1])

let getCurrentCommitHash (folder: string) = 
    getShellOutput "git rev-parse --short HEAD" folder
    
let createSignature folder = 
    let currHash = folder |> getCurrentCommitHash
    let (lastHash, lastTimestamp) = getLastHashAndTimestamp folder
    {Timestamp=DateTime.Now; Hash=currHash; LastGitHash=lastHash; GitTimestamp=DateTime.Parse(lastTimestamp)}

let addCommentToDocument (comment: string) (doc: XDocument) = new XDocument(doc.Root, new XComment(comment))

let getDocumentWithSignature (path: string) (signature : Signature) = path |> XDocument.Load |> addCommentToDocument signature.FullString
