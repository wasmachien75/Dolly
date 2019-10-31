module Dolly.Tests.Config

open Dolly.Config
open System.Xml
open NUnit.Framework
open System.IO

[<SetUp>]
let writeXml () = 
    let writer = XmlWriter.Create("config.xml")
    writer.WriteStartElement("dolly")
    writer.WriteStartElement("config")
    writer.WriteStartElement("dataroot-folder")
    writer.WriteRaw(@"c:\temp\reports")
    writer.WriteEndDocument()
    writer.Close()

[<TearDown>]
let deleteXml () =
    File.Delete("config.xml")

[<Test>]
let ParseConfigTest () = 
    let config = parseConfig "config.xml"
    Assert.AreEqual("c:\\temp\\reports", config.RootFolder)

[<Test>]
let TryParseRealConfigTest () = 
    let maybeConfig = tryGetConfig "config.xml"
    Assert.True(maybeConfig.IsSome)

[<Test>]
let TryParseNonExistingConfigTest () = 
    let maybeConfig = tryGetConfig "abc.xml"
    Assert.False(maybeConfig.IsSome)
