
module Dolly.Push

open Dolly.Logging
open System.Net.Http
open Newtonsoft.Json.Linq

let bitbucketEndpoint = "https://bitbucket.brussels.mediagenix.tv/projects/REPDEV/repos/reports/commits/"

type PropertyValue = 
    | JToken of JToken
    | String of string

let handleResult (res: HttpResponseMessage) = 
    match res.StatusCode |> int with
    | 200 -> "Sent acknowledgement to Teams" |> logInfo
    | i -> (sprintf "Could not send message to Teams (status code %d)" i) |> logInfo

let pushMessageAsyncWithEndPoint (msg: JObject) (endPoint: string) = 
    let client = new HttpClient()
    msg.ToString() |> logInfo
    let content = new StringContent(msg.ToString(), System.Text.Encoding.UTF8, "application/json")
    client.PostAsync(endPoint, content) |> Async.AwaitTask

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
    
    let target = 
        addProp "os" <| String "default" <| new JObject() 
        |> addProp "uri" (String (bitbucketEndpoint + commit))
    
    let action = 
        addProp "@type" <| String "OpenUri" <| new JObject()
        |> addProp "name" (String "View on Bitbucket")
        |> addProp "targets" (JToken (new JArray(target)))

    let potentialAction = new JArray(action)
    
    intermediateMsg |> addProp "potentialAction" (JToken potentialAction)