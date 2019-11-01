
module Dolly.Push

open System.Net.Http
open Newtonsoft.Json.Linq

let bitbucketEndpoint = "https://bitbucket.brussels.mediagenix.tv/projects/REPDEV/repos/reports/commits/"

type PropertyValue = 
    | JToken of JToken
    | String of string

let pushMessageAsyncWithEndPoint (msg: JObject) (endPoint: string) = 
    let client = new HttpClient()
    let content = new StringContent(msg.ToString(), System.Text.Encoding.UTF8, "application/json")
    client.PostAsync(endPoint, content)

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