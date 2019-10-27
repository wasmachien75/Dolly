module Dolly.FolderMapping

open System.Xml.Linq
open System.Linq
open System.IO
open System.Text.RegularExpressions

type FolderMapping = {Customer: string; LocalFolderName: string; FtpFolderName: string}

let readFolderMappingFromFile (file : string) = 
    if not <| File.Exists(file) then Seq.empty
    else
        let doc = file |> XDocument.Load
        let root = doc.Root
        let attrAt (el: XElement) name = el.Attribute(XName.Get(name)).Value
        let customerFromElement (el: XElement) = 
            let attrAtThisEl = attrAt el
            {Customer = "name" |> attrAtThisEl; LocalFolderName = "local" |> attrAtThisEl; FtpFolderName = "ftp" |> attrAtThisEl }
        root.Descendants(XName.Get("customer")) |> Seq.map customerFromElement

let customerFromPath (map: seq<FolderMapping>) (path: string) : FolderMapping option =
    let absPath = Path.GetFullPath(path)
    let regexForIgnore = "C:|v[\dqr]+"
    let possibleLocalNames = absPath.Split(Path.DirectorySeparatorChar) |> Seq.filter (fun t -> not <| Regex.IsMatch(t, regexForIgnore))
    Seq.tryFind (fun fm -> possibleLocalNames |> Seq.contains fm.LocalFolderName) map