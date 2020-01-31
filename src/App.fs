module App.View

open Elmish
open Fable.React
open Fable.React.Props
open Fable.FontAwesome
open State
open Types
open Fulma

let private navbarEnd =
    Navbar.End.div [ ]
        [ Navbar.Item.div [ ]
            [ Field.div [ Field.IsGrouped ]
                [ Control.p [ ]
                    [ Button.a [ Button.Props [ Href "https://github.com/MangelMaxime/fulma-demo" ] ]
                        [ Icon.icon [] [ Fa.i [ Fa.Brand.Github ] [] ]
                          span [ ] [ str "Source" ] ] ] ] ] ]

let private navbarStart dispatch =
    Navbar.Start.div [ ]
        [ Navbar.Item.a [ Navbar.Item.Props [ OnClick (fun _ ->
                                                        Router.QuestionPage.Index
                                                        |> Router.Question
                                                        |> Router.modifyLocation) ] ]
            [ str "Home" ]
          Navbar.Item.div [ Navbar.Item.HasDropdown
                            Navbar.Item.IsHoverable ]
            [ Navbar.Link.div [ ]
                [ str "Options" ]
              Navbar.Dropdown.div [ ]
                [ Navbar.Item.a [ Navbar.Item.Props [ OnClick (fun _ -> dispatch ResetDatabase)] ]
                    [ str "Reset demo" ] ] ] ]

let private navbarView isBurgerOpen dispatch =
    div [ ClassName "navbar-bg" ]
        [ Container.container [ ]
            [ Navbar.navbar [ Navbar.CustomClass "is-primary" ]
                [ Navbar.Brand.div [ ]
                    [ Navbar.Item.a [ Navbar.Item.Props [ Href "#" ] ]
                        [ Image.image [ Image.Is32x32 ]
                            [ img [ Src "assets/mini_logo.svg" ] ]
                          Heading.p [ Heading.Is4 ]
                            [ str "Fulma-demo" ] ]
                      // Icon display only on mobile
                      Navbar.Item.a [ Navbar.Item.Props [ Href "https://github.com/MangelMaxime/fulma-demo" ]
                                      Navbar.Item.CustomClass "is-hidden-desktop" ]
                                    [ Icon.icon [] [ Fa.i [ Fa.Size Fa.FaLarge; Fa.Brand.Github ] [] ] ]
                      // Make sure to have the navbar burger as the last child of the brand
                      Navbar.burger [ Fulma.Common.CustomClass (if isBurgerOpen then "is-active" else "")
                                      Fulma.Common.Props [
                                        OnClick (fun _ -> dispatch ToggleBurger) ] ]
                        [ span [ ] [ ]
                          span [ ] [ ]
                          span [ ] [ ] ] ]
                  Navbar.menu [ Navbar.Menu.IsActive isBurgerOpen ]
                    [ navbarStart dispatch
                      navbarEnd ] ] ] ]

let private renderPage model dispatch =
    match model with
    | { CurrentPage = Router.Question _
        QuestionDispatcher = Some extractedModel } ->
        Question.Dispatcher.View.root model.Session extractedModel (QuestionDispatcherMsg >> dispatch)
    | _ ->
        Render.pageNotFound

let private root model dispatch =
    div [ ]
        [ navbarView model.IsBurgerOpen dispatch
          renderPage model dispatch ]


open Elmish.React
open Elmish.Debug
open Elmish.Navigation
open Elmish.UrlParser
open Elmish.HMR

// Init the first datas into the database
Database.Init()

Program.mkProgram init update root
|> Program.toNavigable (parseHash Router.pageParser) urlUpdate
|> Program.withReactBatched "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run

// https://webpack.js.org/guides/code-splitting/#dynamic-imports
// https://webpack.js.org/plugins/split-chunks-plugin/#splitchunkscachegroups
// https://webpack.js.org/api/module-methods/#requirecontext
// https://webpack.js.org/configuration/externals/#string

// Tells WebPack to import a file.
[<Fable.Core.Emit("require.context('$0', true, /.+/, 'lazy-once')($1).then(module => { return module.default })")>]
let inline jsImportFile ( context : string ) ( file: string ) : 'T = Fable.Core.Util.jsNative
let libB : Fable.Core.JS.Promise<unit> = jsImportFile "./LibB" "./LibB.fs"

libB
|> Promise.bind (fun _ ->
    promise {
        LibB.showInConsole "Hey I am working from inside a lazy import"
    }
)
|> Promise.start
