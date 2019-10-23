module Dolly.Signature

open System.Diagnostics
open System
open System.Xml.Linq

type Signature = {Timestamp: DateTime; Hash: string} with
    member this.FullString = 
        let dateTime = this.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")
        [dateTime;" | SHA-1=";this.Hash] |> String.concat ""

///Gets the hash of the commit at HEAD in a folder.
let getCurrentCommitHash (folder: string) = 
    let proc, info = new Process(), new ProcessStartInfo()
    info.UseShellExecute <- false
    info.RedirectStandardOutput <- true
    info.RedirectStandardError <- true
    info.WorkingDirectory <- folder
    info.FileName <- "cmd.exe"
    info.Arguments <- "/c git rev-parse --short HEAD"
    proc.StartInfo <- info
    proc.Start() |> ignore
    proc.WaitForExit()
    match proc.ExitCode with
        | 0 -> proc.StandardOutput.ReadToEnd().Trim()
        | _ -> failwithf "Failure when retrieving git hash. [%s]" (proc.StandardError.ReadToEnd().Trim())
    

let createSignature folder = {Timestamp=DateTime.Now; Hash=folder |> getCurrentCommitHash}

///Appends a comment to the end of an XDocument.
let addCommentToDocument (comment: string) (doc: XDocument) = new XDocument(doc.Root, new XComment(comment))

///Appends a signature to the end of an XML file.
let getDocumentWithSignature (path: string) (signature : Signature) = path |> XDocument.Load |> addCommentToDocument signature.FullString
