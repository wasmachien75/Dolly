module Dolly.Tests.Push

open Newtonsoft.Json.Linq
open System.Linq
open Dolly.Push
open NUnit.Framework

let endpointTest = "https://outlook.office.com/webhook/a3c2b56e-ce0d-48da-9a77-4406dddbad07@00f6d248-8400-431b-b006-63cc85dc46c2/IncomingWebhook/c4f4bfc60b164ca490bc01f520da37ff/acc01fe7-e32a-49f5-908d-cbb95bc155e6"
let pushMessageAsync_test msg = pushMessageAsyncWithEndPoint msg endpointTest

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
    let result = (pushMessageAsync_test message) |> Async.RunSynchronously
    Assert.AreEqual(200, result.StatusCode |> int)