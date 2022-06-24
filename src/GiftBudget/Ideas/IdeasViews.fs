module Ideas.Views

open Giraffe.ViewEngine
open Saturn

let ideaCard ctx idea =
    div [_class "card"] [
        div [_class "card-content"] [
            div [_class "media"] [
                div [_class "media-content"] [
                    p [_class "title is-4"] [
                        a [_href (Links.withId ctx idea.Id)] [
                            str idea.Text
                        ]
                    ]                        
                ]
            ]
        ]
    ]

let ideasList ctx ideas =
    App.layout [
        h1 [_class "title"] [str "Ideas"]
        div [] (ideas |> List.map (ideaCard ctx))
    ]
    
let ideaDetail ctx idea =
    App.layout [
        h1 [_class "title"] [str idea.Text]
        match idea.Recipient with
        | IdeaRecipient recipient ->
            p [] [str recipient.Name]
        | IdeaRecipients recipients ->
            div [] (recipients |> List.map (fun r -> p [] [str r.Name]))
        | NoRecipient ->
            span [] []
    ]