module Dolly.Tests.Push

open Fuchu
open Newtonsoft.Json.Linq
open System.Linq
open Dolly.Push
open System.Threading.Tasks

[<Tests>]
let test =
    testList "Push tests"
        [ testCase "It should" <| (fun _ ->
          let message = createMessage "title" "text" "hash"
          [ "title"; "text" ]
          |> Seq.iter
              (fun i ->
              Assert.Equal
                  ("The property should exist and have the right value", i, message.Property(i).Value.ToString()))
          let potentialAction = message.Property("potentialAction").Value :?> JArray
          Assert.Equal("Potential action should have one element", 1, potentialAction.Count)
          Assert.Equal("It should have properties", true, (potentialAction.First :?> JObject).Properties().Count() > 1))
          testCase "Push to Teams" <| (fun _ ->
          let message = createMessage "a" "b" "c"
          let result = (pushMessageAsync_test message).Result
          Assert.Equal("", result.StatusCode |> int, 200)) ]
