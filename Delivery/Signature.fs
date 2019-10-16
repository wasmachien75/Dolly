module Delivery.Signature

open System.Diagnostics
open System.Xml
open System
open System.Xml.Linq
open System.Text.RegularExpressions

let getCurrentCommitHash (folder: string) = 
    let proc = new Process()
    let info = new ProcessStartInfo()
    info.RedirectStandardOutput <- true
    info.WorkingDirectory <- folder
    info.FileName <- "cmd.exe"
    info.Arguments <- sprintf "/c git -C %s rev-parse HEAD" folder
    proc.StartInfo <- info
    proc.Start() |> ignore
    proc.StandardOutput.ReadToEnd().Trim()

let xmlFileEndsWithComment(file: XDocument) =
    let isComment (node: XNode) = node.GetType() = typeof<XComment>  
    file.LastNode |> isComment

let createSignature folder = 
    let hash = folder |> getCurrentCommitHash
    let dateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")
    [dateTime;" | SHA-1=";hash] |> String.concat ""

let commentIsSignature(comment: XComment) = 
    //a comment should be <!--2019-02-01T23:02:12.222+02:00 | SHA-1=AD83D93B -->
    let regex = "[TZ0-9:+-\.]{29} \| SHA-1=[A-F0-9]{8}"
    (comment.Value, regex) |> Regex.IsMatch

let documentWithSignature (signature: string) (doc: XDocument) = 
    let sigComment = new XComment(signature)
    new XDocument(doc.Root, sigComment)

let getDocumentWithSignature (path: string) = 
    path |> XDocument.Load |> documentWithSignature "bla"
