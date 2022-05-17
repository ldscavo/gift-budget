module Recipients.Views

open System
open Giraffe.ViewEngine
open Saturn
open Recipients

let recipientCard ctx (recipient: Recipient) =
    div [_class "tile"] [
        div [_class "card"] [
            div [_class "card-content"] [
                div [_class "media"] [
                    div [_class "media-content"] [
                        p [_class "title is-4"] [
                            str recipient.Name
                        ]                        
                    ]
                ]
                match recipient.Notes with
                | Some notes -> div [_class "content"] [ str notes ]
                | None -> span [] []
            ]        
            div [_class "card-footer"] [
                a [_class "card-footer-item"; _href (Links.withId ctx recipient.Id)] [str "View Details"]
                a [_class "card-footer-item"; _href "#"] [str "Add New Idea"]
            ]
        ]
    ]

let recipientsList ctx (recipients: Recipient list) =
    let recipientCardList = 
        match recipients with
        | [] -> [ div [] [str "You've not added any recipients yet!"] ]
        | _ -> recipients |> List.map (recipientCard ctx) 

    App.layout [
        h1 [_class "title"] [
            str "Recipients"
            a [_href (Links.add ctx)] [
                span [_class "icon is-small ml-4"] [ i [_class "fas fa-square-plus"] [] ]
            ]
        ]
        div [] recipientCardList                  
    ]

let recipientDetail ctx (recipient: Recipient) =
    App.layout [
        h1 [_class "title"] [str recipient.Name]
    ]

let addEditRecipient ctx (maybeRecipient: Recipient option) (maybeInput: RecipientInput option) =
    let (name, notes, status) =
        match maybeRecipient, maybeInput with
        | Some _, Some i -> (i.name, (Some i.notes), "Edit")
        | Some r, None -> (r.Name, r.Notes, "Edit")
        | None, Some i -> (i.name, (Some i.notes), "Add")
        | None, None -> ("", None, "Add")

    App.layout  [
        div [_class "modal is-active"] [
            div [_class "modal-background"] []
            div [_class "modal-content"] [
                div [_class "box"] [
                    h1 [_class "title"] [str $"%s{status} Recipient"]
                    form [_action (Links.index ctx); _method "post"] [
                        div [_class "field"] [
                            label [_class "label"] [str "Name"]
                            div [_class "control"] [
                                input [_class "input"; _type "text"; _name "name"; _placeholder "John Doe"; _value name]
                            ]
                        ]
                        div [_class "field"] [
                            label [_class "label"] [str "Notes*"]
                            div [_class "control"] [
                                textarea [_class "textarea"; _name "notes"] [str (notes |> Option.defaultValue "")]
                            ]
                        ]
                        div [_class "field is-grouped"] [
                            div [_class "control"] [
                                button [_class "button is-link"; _type "submit"] [str "Save"]
                            ]
                            div [_class "control"] [
                                a [_class "button is-link is-light"; _href (Links.index ctx)] [str "Cancel"]
                            ]
                        ]
                    ]
                ]
            ]
            a [_class "modal-close is-large"; _href (Links.index ctx)] []
        ]
    ]