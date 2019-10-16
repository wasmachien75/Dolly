open Delivery.Actions
open Delivery.Signature
open System

[<EntryPoint>]
[<STAThread>]
let main argv =
    let from = chooseFolder "Select report folder"
    let _to = chooseFolder "Select destination folder"
    let reportDef = copyFolderTo from _to |> findReportDefinitions
    let doc = getDocumentWithSignature reportDef
    doc.Document.ToString() |> printf "%s"

    0
