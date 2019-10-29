module Dolly.Tests.Config

open Fuchu
open Dolly.Config
open System.Xml

let writeXml fn () = 
    let writer = XmlWriter.Create("config.xml")
    writer.WriteStartElement("dolly")
    writer.WriteStartElement("config")
    writer.WriteStartElement("dataroot-folder")
    writer.WriteRaw(@"c:\temp\reports")
    writer.WriteEndDocument()
    writer.Close()
    fn ()

[<Tests>]
let configTests =
    testList "Config tests" [
    testCase "Config" <| writeXml (fun _ ->
        let config = parseConfig "config.xml"
        Assert.Equal("Root folder must equal c:\\temp\\reports", "c:\\temp\\reports", config.RootFolder)
    )
    testCase "Try parse real config" <| writeXml (fun _ ->
        let maybeConfig = tryGetConfig "config.xml"
        Assert.Equal("Config should exist", true, maybeConfig.IsSome))

    testCase "Try parse non-existing config" <| fun _ ->
        let maybeConfig = tryGetConfig "abc.xml"
        Assert.Equal("Config should not exist", false, maybeConfig.IsSome)
    ]