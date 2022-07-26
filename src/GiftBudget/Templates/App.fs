module App

open Giraffe.ViewEngine
open Giraffe.ViewEngine.Htmx
open Giraffe.Htmx
open Microsoft.AspNetCore.Http

let layoutHead =
    head [] [
        meta [_charset "utf-8"]
        meta [_name "viewport"; _content "width=device-width, initial-scale=1"]
        title [] [str "Gift Budget"]
        link [_rel "icon"; _href "/favicon.ico"]
        link [_rel "stylesheet"; _href "https://cdn.jsdelivr.net/npm/bulma@0.9.3/css/bulma.min.css"]
        link [_rel "stylesheet"; _href "/app.css"]
        link [_rel "stylesheet"; _href "/all.min.css"]
        script [_src "https://unpkg.com/htmx.org@1.7.0"] []
    ]

let layout content =
    html [] [
        layoutHead
        body [_hxBoost; _hxTarget "#container"] [
            nav [_id "nav"; _class "navbar"] [
                div [_class "navbar-brand"] [
                    a [_class "navbar-item"; _href "/"] [
                        img [_src "/logo.png"]
                        str "Gift Budget"
                    ]
                ]
                div [_class "navbar-menu"] [
                    div [_class "navbar-start"] [
                        a [_class "navbar-item"; _href "/ideas"] [
                            span [_class "icon"] [i [_class "fas fa-lightbulb"] []]
                            span [] [str "Ideas"]
                        ]
                        a [_class "navbar-item"; _href "/recipients"] [
                            span [_class "icon"] [i [_class "fas fa-user"] []]
                            span [] [str "Recipients"]
                        ]
                        a [_class "navbar-item"; _href "#"] [
                            span [_class "icon"] [i [_class "fas fa-chart-pie"] []]
                            span [] [str "Budgets"]
                        ]
                    ]
                    div [_class "navbar-end"] [
                        a [_class "navbar-item"; _href "/logout"; _hxNoBoost] [str "Logout"]
                    ]
                ]
            ]
            div [_class "section"] [
                div [_id "container"] content
            ]
            div [_class "footer"] [
                div [_class "content has-text-centered"] [
                    p [] [
                        strong [] [str "Gift Budget"]
                        str " by Logan Scavo. Source code available on "
                        a [_href "https://github.com/ldscavo/gift-budget"; _target "_blank"] [str "GitHub"]
                    ]
                ]
            ]
            script [_src "/app.js"] []
        ]
    ]

let template (ctx: HttpContext) content =
    match ctx.Request.IsHtmx && not ctx.Request.IsHtmxRefresh with
    | false -> layout content
    | true -> div [] content

let private modalTemplate (ctx: HttpContext) content =
    [ div [_class "modal is-active"] [
        div [_class "modal-background"] []
        div [_class "modal-content"] [
            div [_class "box"] content
        ]
        a [_class "modal-close is-large"; _href (Saturn.Links.index ctx)] []
    ] ] |> layout

let modal (ctx: HttpContext) content =
    match ctx.Request.IsHtmx && not ctx.Request.IsHtmxRefresh with
    | false -> modalTemplate ctx content
    | true -> div [] content