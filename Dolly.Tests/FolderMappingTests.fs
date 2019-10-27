module Dolly.Tests.FolderMapping

open Dolly.FolderMapping
open Fuchu
open System.IO
open System.Xml.Linq

let withFile fn () = 
    File.WriteAllText("mapping.xml", @"<mappings><customer name='BBC' local='bbcLocal' ftp='bbcFtp'/></mappings>")
    fn ()
    File.Delete("mapping.xml")

[<Tests>]
let tests =
    testList "Folder mapping tests" [
        testCase "A map" <| withFile (fun _ ->
            let mapping = readFolderMappingFromFile "mapping.xml"
            Assert.Equal("There should be 1 mapping", Seq.length mapping, 1)
            let m = Seq.head mapping
            Assert.Equal("The mapping should have the correct customer name", "BBC", m.Customer)
            Assert.Equal("The mapping should have the correct customer name", "bbcLocal", m.LocalFolderName)
            Assert.Equal("The mapping should have the correct customer name", "bbcFtp", m.FtpFolderName)
            )
        testCase "Get customer from local path" <| withFile (fun _ ->
            let path = @"C:\Temp\Reports\bbcLocal\V28\SomeReport"
            let getPossibleCustomer = customerFromPath <| readFolderMappingFromFile "mapping.xml"
            match getPossibleCustomer path with
            | None -> failwith "Customer BBC should have been found"
            | Some {Customer="BBC"} -> () //pass
            | _ -> failwith "A wrong customer was found"
        )

        testCase "Get non-existing customer" <| withFile (fun _ ->
            let path = @"C:\Temp\Reports\None"
            let getPossibleCustomer = customerFromPath <| readFolderMappingFromFile "mapping.xml"
            match getPossibleCustomer path with
            | Some _ -> failwith "A customer should not have been found"
            | None -> () //pass
            )
    ]

