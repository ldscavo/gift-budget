module App

open Giraffe.ViewEngine

let layoutHead =
    head [] [
        meta [_charset "utf-8"]
        meta [_name "viewport"; _content "width=device-width, initial-scale=1"]
        title [] [str "Gift Budget"]
        link [_rel "icon"; _href "/favicon.ico"]
        link [_rel "stylesheet"; _href "https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css"]
        link [_rel "stylesheet"; _href "https://cdn.jsdelivr.net/npm/bulma@0.9.3/css/bulma.min.css"]
        link [_rel "stylesheet"; _href "/app.css"]
    ]

let layout content =
    html [] [
        layoutHead
        body [] [
            nav [_id "nav"; _class "navbar"] [
                div [_class "navbar-brand"] [
                    a [_class "navbar-item"; _href "/"] [
                        img [_src "/logo.png"]
                        str "Gift Budget"
                    ]
                ]
                div [_class "navbar-menu"] [
                    div [_class "navbar-end"] [
                        a [_class "navbar-item"] [str "Login"]
                    ]
                ]
            ]
            div [_class "columns"] [
                div [_class "column"] [
                    aside [_class "menu"] [
                        ul [_class "menu-list"] [
                            li [] [a [_href "#"] [str "Recipients"]]
                            li [] [a [_href "#"] [str "Ideas"]]
                            li [] [a [_href "#"] [str "Budgets"]]
                        ]
                    ]
                ]                
                div [_class "column is-four-fifths"] [
                    div [_class "container"]
                        content
                ]
            ]
            script [_src "/app.js"] []
        ]
    ]