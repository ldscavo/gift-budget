module Recipients.Views

open Giraffe.ViewEngine

let recipientCard (recipient: Recipient) =
    div [_class "tile"] [
        div [_class "card"] [
            div [_class "card-content"] [
                div [_class "media"] [
                    div [_class "media-content"] [
                        p [_class "title is-4"] [
                            str recipient.Name
                            span [_class "icon"] [ i [_class "fa-solid fa-angle-down"] [] ]
                        ]                        
                    ]
                ]
                match recipient.Notes with
                | Some notes -> div [_class "content"] [ str notes ]
                | None -> span [] []
            ]        
            div [_class "card-footer"] [
                a [_class "card-footer-item"; _href "#"] [str "View Details"]
                a [_class "card-footer-item"; _href "#"] [str "Add New Idea"]
            ]
        ]
    ]

let recipientsList ctx (recipients: Recipient list) =
    let recipientCardList = 
        match recipients with
        | [] -> [ div [] [str "You've not added any recipients yet!"] ]
        | _ -> recipients |> List.map recipientCard 

    App.layout [
        h1 [_class "title"] [str "Recipients"]
        div [] recipientCardList                  
    ]