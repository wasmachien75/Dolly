module Delivery.Actions

open System.IO
open System.Windows.Forms

let chooseFolder dialogTitle =
    let dialog = new FolderBrowserDialog()
    dialog.Description <- dialogTitle
    dialog.RootFolder <- System.Environment.SpecialFolder.CommonDocuments
    dialog.ShowNewFolderButton <- false
    if dialog.ShowDialog() = DialogResult.OK then dialog.SelectedPath
    else exit (-1)

let copyFolderTo (folder: string) (destination: string) =
    let dirName = Path.GetFileName(folder)
    let combined = Path.Combine(destination, dirName)
    printfn "Combined: %s" combined
    Directory.CreateDirectory(combined)

    
let copySingleFile (destinationDir: string) (from: string) =
    let destination = Path.Combine(destinationDir, Path.GetFileName(from))
    File.Copy(from, destination)

let copyAllFiles (fromDir: string) (destinationDir: string) = 
    Directory.GetFiles(fromDir) |> Seq.iter (copySingleFile destinationDir)

