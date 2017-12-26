(*
Helpers for generating static web pages
npm dependencies: marked, handlebars, highlight.js
*)

module Fable.Helpers.WebGenerator

open System.Collections.Generic
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Node.Exports

let private handleBarsCompile (templateString: string): obj->string = import "compile" "handlebars"
let private marked (markdown: string, opts: obj): string = importDefault "marked"
let private highlight: obj = importAll "highlight.js"

marked?setOptions(
    createObj["highlight" ==> fun code lang ->
        highlight?highlightAuto(code, [|lang|])?value
    ]
) |> ignore

let private renderer = createNew marked?Renderer ()
let private templateCache = Dictionary<string, obj->string>()

let parseMarkdown(content: string) =
    marked(content, createObj["renderer" ==> renderer])

/// Parses a Handlebars template
let parseTemplate (path: string) (context: (string*obj) list) =
    let template =
        match templateCache.TryGetValue(path) with
        | true, template -> template
        | false, _ ->
            let template = Fs.readFileSync(path).toString() |> handleBarsCompile
            templateCache.Add(path, template)
            template
    createObj context |> template

/// Parses a React element invoking ReactDOMServer.renderToString
let parseReact (el: React.ReactElement) =
    ReactDomServer.renderToString el

/// Parses a React element invoking ReactDOMServer.renderToStaticMarkup
let parseReactStatic (el: React.ReactElement) =
    ReactDomServer.renderToStaticMarkup el

type [<Pojo>] InnerHtml =
  { __html: string }

let setInnerHtml (html: string) =
  React.Props.DangerouslySetInnerHTML { __html = html }
