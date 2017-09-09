module App.View

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Types
open State
open Fulma.Layout
open Fulma.Components
open Global
open Fable.Helpers.React
open Fable.Helpers.React.Props

importAll "./sass/main.sass"

let genNavbarItem txt refPage currentPage =
    Navbar.item_a [ if refPage = currentPage then
                        yield Navbar.Item.isActive
                    yield Navbar.Item.props [ Href (toHash refPage) ] ]
        [ str txt ]

let navbarAdminLink currentPage =
    let isActive =
        match currentPage with
        | Admin _ -> true
        | _ -> false
    Navbar.link_a [ if isActive then
                        yield Navbar.Link.isActive
                    yield Navbar.Link.props [ Href (toHash (Admin AdminPage.Index)) ] ]
        [ str "Admin" ]

let navbar currentPage =
    Navbar.navbar [ ]
        [ Navbar.brand_a [ ]
            [ ]
          Navbar.item_a [
              if currentPage = Home then
                yield Navbar.Item.isActive
              yield Navbar.Item.props [ Href (toHash Home) ]
           ]
            [ str "Home" ]
          Navbar.menu [ ]
            [ Navbar.start_div [ ]
                [ Navbar.item_div [ Navbar.Item.hasDropdown
                                    Navbar.Item.isHoverable ]
                    [ navbarAdminLink currentPage
                      Navbar.dropdown_div [ ]
                        [ genNavbarItem "Users" (Admin (AdminPage.User AdminUserPage.Index)) currentPage ] ] ] ] ]


let root (model: Model) dispatch =

    // let pageHtml =
    //     function
    //     | Page.About -> Info.View.root
    //     | Counter -> Counter.View.root model.Counter (CounterMsg >> dispatch)
    //     | CounterList -> CounterList.View.root model.CounterList (CounterListMsg >> dispatch)
    //     | Home -> Home.View.root model.Home (HomeMsg >> dispatch)

    div
        [ ClassName "container" ]
        [ navbar model.CurrentPage

        ]

open Elmish.React
open Elmish.Debug

// App
Program.mkProgram init update root
|> Program.toNavigable (parseHash pageParser) urlUpdate
|> Program.withReact "elmish-app"
//-:cnd:noEmit
#if DEBUG
|> Program.withDebugger
#endif
//+:cnd:noEmit
|> Program.run
