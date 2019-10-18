module Delivery.Tests.Actions

open NUnit.Framework
open Delivery.Actions
open System.IO
open System.Xml.Linq

let testDir = @"C:\Temp\bla\"

[<SetUp>]
let Setup () =
    Directory.CreateDirectory(testDir) |> ignore

[<Test>]
let CopyFolder () =
    let from = @"C:\Users\willem.van.lishout\Documents\Repositories\reports\AETN\V29Q3\Continuity day\MX1 Playlist"
    let _to = testDir
    let dirInfo = copyFolderTo from _to
    Assert.AreEqual(dirInfo, testDir + @"MX1 Playlist")

[<Test>]
let CopySingleFile () = 
    let file = @"C:\Temp\test.txt"
    let destination = testDir
    File.WriteAllText(file, "abc")
    copySingleFile destination file
    Assert.True(File.Exists(testDir + @"test.txt"))

[<Test>]
let CopyAllFiles () =
    let writeManyFiles str = 
        File.WriteAllText(testDir + str, str)
    ["a";"b";"c"] |> Seq.iter writeManyFiles
    let dir = Directory.CreateDirectory(Path.Combine(testDir, "copyFolder"))
    copyAllFiles testDir dir.FullName
    Assert.IsNotEmpty(Directory.GetFiles(Path.Combine(testDir, "copyFolder")))

[<Test>]
let FindReportDef () = 
    ["report def.xml"; "report.xsl"; "filename.xsl"] |> Seq.iter (fun f -> File.WriteAllText(testDir + f, ""))
    Assert.AreEqual("report def.xml", findReportDefinitions testDir |> Path.GetFileName)

[<Test>]
let WriteDocumentTest() = 
    let fileName = testDir + "test.xml"
    "<hi/>" |> XDocument.Parse |> writeDocument fileName
    Assert.True(File.Exists(fileName))
    let newDoc = File.ReadAllText(fileName) |> XDocument.Parse
    Assert.AreEqual(newDoc.Document.Root.Name.ToString(), "hi")

    //it should also work with an existing file.

    "<bye/>" |> XDocument.Parse |> writeDocument fileName
    Assert.True(File.Exists(fileName))
    let newDoc = File.ReadAllText(fileName) |> XDocument.Parse
    Assert.AreEqual(newDoc.Document.Root.Name.ToString(), "bye")

[<TearDown>]
let Teardown() = 
    Directory.Delete(@"C:\Temp\bla", true) |> ignore