open Ionic
open Ionic.Zip

open System
open System.IO

open FSharp.Data
open FSharp.Data.JsonExtensions

//  TODO: Copy to backup directory and add number 

(*
    the code looks for a zipit.json file 
        - if it finds it
            - reads from the file the files to zip
            - reads of the name of the name of the zip vile
            - reads if the zipfile is to be numbered and stored in a backup directory
                - will create the directory if it does not exist

Referenceed:
http://fsharp.github.io/FSharp.Data/library/JsonValue.html
http://stackoverflow.com/questions/20252167/simply-parse-command-line-args
*)

let create_zip_file (dirs :JsonValue[]) (baseDir :string) (files :JsonValue[]) zipfile_name :string = 
      use zipfile = new ZipFile()
      //let writeTo = Path.Combine(baseDir, zipfile_name ) 
      Environment.CurrentDirectory <- baseDir
      for dir in dirs do   
          zipfile.AddDirectory( dir.AsString(), dir.AsString()) |> ignore 
      for file in files do
          zipfile.AddFile(file.AsString(), "")  |> ignore 
      zipfile.Save( zipfile_name :string)
      zipfile_name
 
let getJsonFromFile (fileName :string) =
    let text = File.ReadAllText( fileName)
    JsonValue.Parse(text)

let doTheZip info = 
    try
      let fileList = (info?files.AsArray()) |> Array.ofSeq
      let dirList = (info?directories.AsArray()) |> Array.ofSeq
      let fileName = info?outFileName.AsString()
      let baseDir = info?baseDir.AsString()

      File.Delete(fileName)

      create_zip_file dirList baseDir fileList fileName |> ignore
      0

    with   
    | _ as ex 
           -> (printfn "Generic exception was caught: %s" ex.Message)
              -1     

[<EntryPoint>]
let main argv = 
    if argv.Length = 1 then   
      let argList = argv |> List.ofSeq
      let jsonFile =  argList.Head

      if File.Exists(jsonFile) then
            doTheZip (getJsonFromFile  jsonFile)
          
      else 
        printfn "Failed: %A does not exist" argv 
        -1

    else
      printfn "Wrong number of parameters" 
      -1    