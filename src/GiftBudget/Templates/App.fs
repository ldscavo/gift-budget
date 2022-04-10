module App

open Giraffe.ViewEngine

let layout (content: XmlNode list) =
    html [] [
        head [] [
            meta [_charset "utf-8"]
            meta [_name "viewport"; _content "width=device-width, initial-scale=1" ]
            title [] [encodedText "Hello gift_budget"]
            link [_rel "stylesheet"; _href "https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css" ]
            link [_rel "stylesheet"; _href "https://cdn.rawgit.com/Chalarangelo/mini.css/v3.0.1/dist/mini-default.min.css" ]
            link [_rel "stylesheet"; _href "/app.css" ]
        ]
        body [] [
            header [] [
                a [_href "/"; _class "logo"] [str "Gift Budget"]
            ]
            div [_class "container"] [
                div [_class "row"] [
                    div [_class "col-sm-12 col-md-3"] [
                        nav [] [
                            a [_href "#"] [str "Recipients"]
                            a [_href "#"] [str "Budgets"]
                        ]
                    ]
                    div [_class "col-sm-12 col-md-9"]
                        content
                ]
            ]
            script [_src "/app.js"] []
        ]
    ]