(*
Helpers for IO operations with Node.js
npm dependencies: fs-extra
*)

module Fable.Helpers.Node.IO

open Fable.Core.JsInterop
open Fable.Import.Node.Globals
open Fable.Import.Node.Exports

let private fsExtra: obj = importAll "fs-extra"

/// Resolves a path using the location of the target JS file
/// Note the function is inline so `__dirname` will belong to the calling file
let inline resolve (path: string) =
    Path.resolve(__dirname, path)

let rec ensureDirExists (dir: string) (cont: (unit->unit) option) =
    if Fs.existsSync(!^dir) then
        match cont with Some c -> c() | None -> ()
    else
        ensureDirExists (Path.dirname dir) (Some (fun () ->
            if not(Fs.existsSync !^dir) then
                Fs.mkdirSync dir |> ignore
            match cont with Some c -> c() | None -> ()
        ))

let writeFile (path: string) (content: string) =
    ensureDirExists (Path.dirname path) None
    Fs.writeFileSync(path, content)

let readFile (path: string) =
    Fs.readFileSync(path).toString()

/// Copy a file or directory. The directory can have contents. Like cp -r.
/// Overwrites target files
let copy (source: string) (target: string): unit =
    !!fsExtra?copySync(source, target, createObj["overwrite" ==> true])
