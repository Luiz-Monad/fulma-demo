module Render

open Fulma
open Fable.React
open Fable.React.Props
open Fable.Core
open Fable

let pageNotFound =
    Hero.hero [ Hero.IsFullHeight
                Hero.Color IsDanger ]
        [ Hero.body [ ]
            [ Container.container [ Container.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
                [ Heading.h1 [ ]
                    [ str "404" ] ] ] ]

let converter = Showdown.Globals.Converter.Create()

let contentFromMarkdown options str =
    Content.content
        [ yield! options
          yield Content.Props [ DangerouslySetInnerHTML { __html =  converter.makeHtml str } ] ]
        [ ]
