module Delivery.Tests.Actions

open NUnit.Framework
open Delivery.Actions
open System.IO

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

[<TearDown>]
let Teardown() = 
    Directory.Delete(@"C:\Temp\bla", true) |> ignore