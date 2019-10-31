module Dolly.Tests.Signature

open Dolly.Signature
open System.Xml.Linq
open System.IO
open NUnit.Framework

let testFolder = "testDir" 

[<SetUp>]
let writeGit () =
    Directory.CreateDirectory(testFolder) |> ignore
    getShellOutput "git init && git config user.name nobody && git config user.email nobody@google.com && echo \"blabla\" > test.test && git add * && git commit -m \"test\" " testFolder |> ignore

[<TearDown>]
let delete () = //git objects are marked read-only and can therefore not be simply deleted
    let rec recursivelyRemoveReadOnlyAndDelete folder = 
        let dir = new DirectoryInfo(folder)
        dir.GetFiles() |> Seq.iter (fun f -> 
            f.Attributes <- FileAttributes.Normal
            f.Delete()
        )
        dir.GetDirectories() |> Seq.iter 
            (fun subDir -> recursivelyRemoveReadOnlyAndDelete subDir.FullName)
        dir.Delete(true)
    recursivelyRemoveReadOnlyAndDelete testFolder

[<Test>]
let HashTest () = 
    Assert.AreEqual(7, testFolder |> getCurrentCommitHash |> String.length)

[<Test>]
let CreateSignatureTest () = 
    let sign =  testFolder |> createSignature
    sign.FullString.EndsWith("=") |> Assert.False

[<Test>]
let SignatureContentTest () = 
    let now = System.DateTime.Now
    let nowPlus1Hour = now.AddHours(1.0)
    let signature = {Hash="abc";LastGitHash="def";Timestamp=now;GitTimestamp=nowPlus1Hour}
    let str = signature.FullString
    (str.Contains("abc") && str.Contains("def") && str.Contains(now.ToString("yyyy-MM-dd")) && str.Contains(nowPlus1Hour.ToString("yyyy-MM-dd")))
    |> Assert.True

[<Test>]
let AddCommentTest () =
    let doc = "<greetings/>" |> XDocument.Parse |> addCommentToDocument "bla"
    let lastComment = doc.LastNode :?> XComment
    Assert.AreEqual(lastComment.Value, "bla")