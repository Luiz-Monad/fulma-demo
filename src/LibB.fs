module LibB

open Browser.Dom
open Fable.Core.JsInterop

type Exports =
    abstract ShowInConsole: msg : string -> unit

let showInConsole (msg : string) = console.log(msg)

let exports =
    { new Exports with
            member __.ShowInConsole(msg) = showInConsole msg }

exportDefault exports
