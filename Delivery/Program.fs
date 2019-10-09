module Some

open Delivery
open System
open System.Windows.Forms

[<EntryPoint>]
[<STAThread>]
let main argv =
    let from = chooseFolder "Select report folder"
    let _to = chooseFolder "Select destination folder"
    let res = copyFolderTo from _to
    printfn "%s" res.FullName
    0
