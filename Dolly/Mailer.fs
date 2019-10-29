
module Dolly.Mailer

open System.Net.Mail
open System.IO


let tail (folder: string) = 
    folder.Split(Path.DirectorySeparatorChar) |> Seq.last

let sendMail sourceFolder targetFolder = 
    let client = new SmtpClient()
    client.Host <- "localhost"
    client.Port <- 25
    let subject = "Report delivery"
    let body = sprintf "Report '%s' was delivered to %s at %s" (tail sourceFolder) (targetFolder) <| System.DateTime.Now.ToShortTimeString()
    client.Send("report@mediagenix.tv", "willem.van.lishout@mediagenix.tv", subject, body)