module Dolly.Tests.Signature

open Dolly.Signature
open System.Xml.Linq
open Fuchu

let assertFalse msg value = Assert.Equal(msg, false, value)
[<Tests>]
let signatureTests =
    testList "Signature tests" [ 

    testCase "Hash length" <| fun _ -> 
        Assert.Equal("Hash length must be 7", @"C:\Users\willem.van.lishout\Documents\Repositories\report-delivery\Dolly.Tests" |> getCurrentCommitHash |> String.length, 7)
    
    testCase "Add comment to document" <| fun _ -> 
        let doc = "<greetings/>" |> XDocument.Parse |> addCommentToDocument "bla"
        let lastComment = doc.LastNode :?> XComment
        Assert.Equal("The comment should be there", lastComment.Value, "bla")

    testCase "Create signature" <| fun _ -> 
        let sign =  @"C:\Users\willem.van.lishout\Documents\Repositories\report-delivery\Dolly.Tests" |> createSignature
        sign.FullString.EndsWith("=") |> assertFalse "The signature must be correct"
]