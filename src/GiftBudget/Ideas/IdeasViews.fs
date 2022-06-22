module Ideas.Views

open Giraffe.ViewEngine
open Saturn
open Ideas

let ideaCard ctx (idea: Idea) =
    div [_class "card"] [
        div [_class "card-content"] [
            div [_class "media"] [
                div [_class "media-content"] [
                    p [_class "title is-4"] [
                        str idea.Text
                    ]                        
                ]
            ]
        ]
    ]

let ideasList ctx ideas =
    App.layout [
        div [] (ideas |> List.map (ideaCard ctx))
    ]    