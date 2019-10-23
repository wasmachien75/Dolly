module Dolly.Config

open System.Xml.Linq
open System.Linq
open System.IO

type Config = {RootFolder: string; TargetFolder: string}

let parseConfig (file: string) : Config = 
    let getValueForElementInDoc (doc: XDocument) el = 
        let descendants = doc.Root.Descendants(XName.Get(el))
        if Seq.isEmpty descendants then "" else descendants.First().Value
    let doc = file |> XDocument.Load
    let getValue = getValueForElementInDoc doc
    {RootFolder = "rootFolder" |> getValue; TargetFolder = "targetFolder" |> getValue}

let tryGetConfig configFile : Option<Config> = 
    if File.Exists (configFile) then parseConfig configFile |> Some else None
