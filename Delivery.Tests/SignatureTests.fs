module Delivery.Tests.Signature

open NUnit.Framework
open Delivery.Signature
open System.Xml.Linq
open System.Diagnostics
open System.IO

[<Test>]
let TestHash () = 
    Assert.AreEqual( @"C:\Users\willem.van.lishout\Documents\Repositories\report-delivery\Delivery.Tests" |> getCurrentCommitHash |> String.length, 7)

[<Test>]
let AddCommentToDocument() = 
    let doc = "<greetings/>" |> XDocument.Parse |> addCommentToDocument "bla"
    let lastComment = doc.LastNode :?> XComment
    Assert.AreEqual(lastComment.Value, "bla")

[<Test>]
let TestSignature () = 
   let sign =  @"C:\Users\willem.van.lishout\Documents\Repositories\report-delivery\Delivery.Tests" |> createSignature
   sign.FullString.EndsWith("=") |> Assert.False