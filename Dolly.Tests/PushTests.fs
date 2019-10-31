module Dolly.Tests.Push

open Newtonsoft.Json.Linq
open System.Linq
open Dolly.Push
open NUnit.Framework

[<Test>]
let CreateMessageTest () = 
    let message = createMessage "title" "text" "hash"
    [ "title"; "text" ]
    |> Seq.iter
        (fun i ->
        Assert.AreEqual
            (i, message.Property(i).Value.ToString()))
    let potentialAction = message.Property("potentialAction").Value :?> JArray
    Assert.AreEqual(1, potentialAction.Count)
    Assert.True((potentialAction.First :?> JObject).Properties().Count() > 1)

[<Test>]
let PushMessageTest () =
    let message = createMessage "a" "b" "c"
    let result = (pushMessageAsync_test message).Result
    Assert.AreEqual(200, result.StatusCode |> int)