module Dolly.Config

open System.Xml.Linq
open System.Linq
open System
open System.IO
open Dolly.Logging

type Config = {RootFolder: string; TargetFolder: string; LogLevel: LogLevel}

let rootFolderTagName = "dataroot-folder"
let targetFolderTagName = "target-folder"
let logLevelTagName = "log-level"

let getValueForElementInDoc (doc: XDocument) el = 
    let descendants = doc.Root.Descendants(XName.Get(el))
    if Seq.isEmpty descendants then "" else descendants.First().Value

let getLogLevel (doc: XDocument) : LogLevel = 
    let level = logLevelTagName |> getValueForElementInDoc doc
    if level = "" 
        then
            "No logLevel fouund in config; defaulting to Warning" |> logDebug
            LogLevel.Warning
    else 
        try
            Enum.Parse(typeof<LogLevel>, level) :?> LogLevel
        with 
            :? ArgumentException -> failwithf "Wrong logLevel specified: %s" level
        
let parseConfig (file: string) : Config = 
    let doc = file |> XDocument.Load
    let getValue = getValueForElementInDoc doc
    let rootFolder = rootFolderTagName |> getValue
    let targetFolder = targetFolderTagName |> getValue
    let logLevel = getLogLevel doc
    {RootFolder = rootFolderTagName |> getValue; TargetFolder = targetFolderTagName |> getValue; LogLevel = Enum.Parse(typeof<LogLevel>, logLevel |> getValue) :?> LogLevel}

let tryGetConfig configFile : Option<Config> = 
    if File.Exists (configFile) then parseConfig configFile |> Some else None

let Configuration = tryGetConfig "config.xml"
