module Dolly.Tests.Actions

open NUnit.Framework
open Dolly.Actions
open System.IO
open System.Xml.Linq

let testDir = Path.Combine(Path.GetTempPath(), "testFolder")

[<SetUp>]
let Setup () =
    Directory.CreateDirectory(testDir) |> ignore

[<TearDown>]
let Teardown () = 
    Directory.Delete(testDir, true)

[<Test>]
let CopySingleFile () =
    let dir = Directory.CreateDirectory(Path.Combine(testDir, "sourceDir")).FullName
    let filePath = Path.Combine(dir, "test.txt")
    let destination = testDir
    File.WriteAllText(filePath, "abc")
    copySingleFile destination filePath
    Path.Combine(destination, "test.txt") |> File.Exists |> Assert.True


[<Test>]
let CopyAllFiles () =
    let writeManyFiles str = 
        File.WriteAllText(Path.Combine(testDir, str), str)
    ["a";"b";"c"] |> Seq.iter writeManyFiles
    let dir = Directory.CreateDirectory(Path.Combine(testDir, "copyFolder"))
    copyAllFiles testDir dir.FullName
    Assert.AreEqual(3, Directory.GetFiles(Path.Combine(testDir, "copyFolder")).Length)

[<Test>]
let WriteDocumentTest () =
    let fileName = Path.Combine(testDir, "test.xml")
    "<hi/>" |> XDocument.Parse |> writeDocument fileName
    File.Exists(fileName) |> Assert.True
    let newDoc = File.ReadAllText(fileName) |> XDocument.Parse
    Assert.AreEqual("hi", newDoc.Document.Root.Name.ToString())