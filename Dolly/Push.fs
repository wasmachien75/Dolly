
module Dolly.Push

open System.Net.Http
open Newtonsoft.Json.Linq

let endpoint = "https://outlook.office.com/webhook/fad37f7c-a3e6-42d9-b77e-8556115b6dab@00f6d248-8400-431b-b006-63cc85dc46c2/IncomingWebhook/c625d98de23940dd8a604cb13152a167/acc01fe7-e32a-49f5-908d-cbb95bc155e6"
let endpointTest = "https://outlook.office.com/webhook/a3c2b56e-ce0d-48da-9a77-4406dddbad07@00f6d248-8400-431b-b006-63cc85dc46c2/IncomingWebhook/c4f4bfc60b164ca490bc01f520da37ff/acc01fe7-e32a-49f5-908d-cbb95bc155e6"
let bitbucketEndpoint = "https://bitbucket.brussels.mediagenix.tv/projects/REPDEV/repos/reports/commits/"
let bla = "bla"

type PropertyValue = 
    | JToken of JToken
    | String of string

let pushMessageAsyncWithEndPoint (msg: JObject) (endPoint: string) = 
    let client = new HttpClient()
    let content = new StringContent(msg.ToString(), System.Text.Encoding.UTF8, "application/json")
    client.PostAsync(endPoint, content)

let pushMessageAsync msg = pushMessageAsyncWithEndPoint msg endpoint

let pushMessageAsync_test msg = pushMessageAsyncWithEndPoint msg endpointTest

let createMessage (title: string) (text: string) (commit: string) =
    let addProp (key: string) (value: PropertyValue) (obj: JObject) =
        let propValue = 
            match value with
            | String value -> new JValue(value) :> JToken
            | JToken value -> value

        obj.Add(key, propValue)
        obj
        
    let intermediateMsg = 
        addProp "themeColor" <| String "FF0000" <| new JObject()
        |> addProp "title" (String title)
        |> addProp "text" (String text)
    
    let action = 
        addProp "@type" <| String "OpenUri" <| new JObject()
        |> addProp "name" (String "View on Bitbucket")

    let potentialAction = new JArray(action)

    intermediateMsg |> addProp "potentialAction" (JToken potentialAction)