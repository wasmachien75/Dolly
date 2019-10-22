module Dolly.Tests.Actions

open Fuchu
open Dolly.Actions
open System.IO
open System.Xml.Linq

let assertTrue msg value = Assert.Equal(msg, true, value)

[<Tests>]

let actionTests = 
    let withDir fn () = 
        let testDir = Directory.CreateDirectory(@"C:\Temp\bla\").FullName
        fn testDir
        Directory.Delete(@"C:\Temp\bla", true)
        
    testList "Action tests" [
        
        testCase "abc" <| withDir (fun testDir ->
            let file, destination = @"C:\Temp\test.txt", testDir
            File.WriteAllText(file, "abc")
            copySingleFile destination file
            assertTrue "The test file was copied successfully" <| File.Exists(testDir + @"test.txt")
            )

        testCase "Copy all files" <| withDir (fun testDir ->
            let writeFiles str = File.WriteAllText(testDir + str, str)
            ["a";"b";"c"] |> Seq.iter writeFiles
            let dir = Directory.CreateDirectory(Path.Combine(testDir, "copyFolder"))
            copyAllFiles testDir dir.FullName
            Assert.Equal("All files are copied", Directory.GetFiles(Path.Combine(testDir, "copyFolder")).Length, 3)
            )

        testCase "Find the report definition" <| withDir (fun testDir ->
            ["report def.xml"; "report.xsl"; "filename.xsl"] |> Seq.iter (fun f -> File.WriteAllText(testDir + f, ""))
            Assert.Equal("The report definition is found", "report def.xml", findReportDefinitions testDir |> Path.GetFileName)
            )

        testCase "Write the document" <| withDir (fun testDir ->
            let fileName = testDir + "test.xml"
            "<hi/>" |> XDocument.Parse |> writeDocument fileName
            assertTrue "The file exists" <| File.Exists(fileName)
            let newDoc = File.ReadAllText(fileName) |> XDocument.Parse
            Assert.Equal("The name of the root of the new file is modified", newDoc.Document.Root.Name.ToString(), "hi")
            )
    ]