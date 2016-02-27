// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open Ionic
//open Ionic.BZip2
//open Ionic.BZip2
open Ionic.Zip
open System.IO

open FSharp.Data
open FSharp.Data.JsonExtensions

(*
    the code looks for a zipit.json file 
        - if it finds it
            - reads from the file the files to zip
            - reads of the name of the name of the zip vile
            - reads if the zipfile is to be numbered and stored in a backup directory
                - will create the directory if it does not exist
*)

type zipDef = 
    struct
      val directories: List<string>
      val files: List<string>
      val backupPath: string
      val counter: bool
    end

let create_zip_file (dirs :JsonValue[]) (files :JsonValue[]) zipfile_name :string = 
      use zipfile = new ZipFile()
      for dir in dirs do   
          zipfile.AddDirectory(dir.AsString() , dir.AsString()) |> ignore 
      for file in files do
          zipfile.AddFile(file.AsString()) |> ignore 
      zipfile.Save ( zipfile_name:string)
      zipfile_name


 
let getJsonFromFile (fileName :string) =
    let text = File.ReadAllText( fileName)
    JsonValue.Parse(text)

[<EntryPoint>]
let main argv = 
//    let files = ["App.config"; "bot.cs";"Field.cs";"FieldCell.cs";"FieldCellType.cs";
//                 "GameState.cs";"MatchSettings.cs";"MoveType.cs";"Parser.cs";"PieceType.cs";
//                 "PlayerState.cs";"Program.cs"]
//    let info =  JsonValue.Parse(""" { "name": "Tomas", "born": 1985,"siblings": [ "Jan", "Alexander" ] } """)
    if argv.Length = 1 then   
      let argList = argv |> List.ofSeq
      let jsonFile =  argList.Head

      if File.Exists(jsonFile) then
          try
            let info = getJsonFromFile  jsonFile
            let fileList = (info?files.AsArray()) |> Array.ofSeq
            let dirList = (info?directories.AsArray()) |> Array.ofSeq
            let fileName = info?outFileName.AsString()

            File.Delete(fileName)

            create_zip_file dirList fileList fileName |> ignore

            0
            with   
                | _ as ex 
                    -> (printfn "Generic exception was caught: %s" ex.Message)
                       3     
       
      else 
        printfn "Failed: %A does not exist" argv 

        -1
    else
      printfn "Wrong number of parameters" 

      -1
    