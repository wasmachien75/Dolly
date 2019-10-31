module Dolly.Tests.FolderMapping

open Dolly.FolderMapping
open System.IO
open NUnit.Framework

[<SetUp>]
let withFile () = File.WriteAllText("mapping.xml", @"<mappings><customer name='BBC' local='bbcLocal' ftp='bbcFtp'/></mappings>")
    
[<TearDown>]

let delete () = File.Delete("mapping.xml")

[<Test>]
let MappingTest () = 
    let mapping = readFolderMappingFromFile "mapping.xml"
    Assert.AreEqual(1, Seq.length mapping)
    let m = Seq.head mapping
    Assert.AreEqual("BBC", m.Customer)
    Assert.AreEqual("bbcLocal", m.LocalFolderName)
    Assert.AreEqual("bbcFtp", m.FtpFolderName)

[<Test>]
let GetCustomerTest() = 
    let path = @"C:\Temp\Reports\bbcLocal\V28\SomeReport"
    let getPossibleCustomer = customerFromPath <| readFolderMappingFromFile "mapping.xml"
    match getPossibleCustomer path with
    | None -> failwith "Customer BBC should have been found"
    | Some {Customer="BBC"} -> () //pass
    | _ -> failwith "A wrong customer was found"

[<Test>]
let GetNonExistingCustomerTest() = 
    let path = @"C:\Temp\Reports\None"
    let getPossibleCustomer = customerFromPath <| readFolderMappingFromFile "mapping.xml"
    match getPossibleCustomer path with
    | Some _ -> failwith "A customer should not have been found"
    | None -> () //pass
