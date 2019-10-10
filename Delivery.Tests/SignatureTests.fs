module Delivery.Tests.Signature

open NUnit.Framework
open Delivery.Signature
open System.Xml.Linq

[<Test>]
let TestHash () = 
    Assert.AreEqual( @"C:\Users\willem.van.lishout\Documents\Repositories\report-delivery\Delivery.Tests" |> getCurrentCommitHash |> String.length, 40)

[<Test>]
let LastNodeIsCommentTest () = 
    Assert.True("<hello/><!--comment-->" |> XDocument.Parse |> xmlFileEndsWithComment)
    Assert.False("<!--pingpong--><life/>" |> XDocument.Parse |> xmlFileEndsWithComment)
    
[<Test>]
let CommentIsSignatureTest () =
    let doc = "<!--nope--><hello/><!--2019-02-01T23:02:12.222+02:00 | SHA-1=AD83D93B-->" |> XDocument.Parse
    Assert.True(doc.LastNode :?> XComment |> commentIsSignature)
    Assert.False(doc.FirstNode :?> XComment |> commentIsSignature)

[<Test>]
let AddCommentToDocument() = 
    let doc = "<greetings/>" |> XDocument.Parse |> documentWithSignature "bla"
    let lastComment = doc.LastNode :?> XComment
    Assert.AreEqual(lastComment.Value, "bla")