module Dolly.Tests.Signature

open Dolly.Signature
open System.Xml.Linq
open Fuchu

let assertFalse msg value = Assert.Equal(msg, false, value)
let assertTrue msg value = Assert.Equal(msg, true, value)
[<Tests>]
let signatureTests =
    //To do: fix hardcoded directories
    testList "Signature tests" [ 

    testCase "Hash length" <| fun _ -> 
        Assert.Equal("Hash length must be 7", @"C:\Users\willem.van.lishout\Documents\Repositories\Dolly\Dolly.Tests" |> getCurrentCommitHash |> String.length, 7)
    
    testCase "Add comment to document" <| fun _ -> 
        let doc = "<greetings/>" |> XDocument.Parse |> addCommentToDocument "bla"
        let lastComment = doc.LastNode :?> XComment
        Assert.Equal("The comment should be there", lastComment.Value, "bla")

    testCase "Create signature" <| fun _ -> 
        let sign =  @"C:\Users\willem.van.lishout\Documents\Repositories\Dolly\Dolly.Tests" |> createSignature
        sign.FullString.EndsWith("=") |> assertFalse "The signature must be correct"

    testCase "Test signature content" <| fun _ ->
        let now = System.DateTime.Now
        let nowPlus1Hour = now.AddHours(1.0)
        let signature = {Hash="abc";LastGitHash="def";Timestamp=now;GitTimestamp=nowPlus1Hour}
        let str = signature.FullString
        (str.Contains("abc") && str.Contains("def") && str.Contains(now.ToString("yyyy-MM-dd")) && str.Contains(nowPlus1Hour.ToString("yyyy-MM-dd")))
        |> assertTrue "The signature string should contain all members"
        
        ]