module Recipients.Views

open Giraffe.ViewEngine
open Saturn
open Recipients

let recipientCard ctx recipient =
    div [_class "column is-narrow"] [
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
                a [_class "card-footer-item"; _href (Links.withId ctx recipient.Id)] [
                    span [_class "icon"] [ i [_class "fas fa-user"] [] ]
                    str "Details"
                ]
                a [_class "card-footer-item"; _href "#"] [
                    span [_class "icon"] [ i [_class "fas fa-lightbulb"] [] ]
                    str "Add Idea"
                ]
            ]
        ]
    ]
    

let recipientsList ctx recipients =
    let recipientCardList = 
        match recipients with
        | [] -> [ div [] [str "You've not added any recipients yet!"] ]
        | _ -> recipients |> List.map (recipientCard ctx) 

    App.template ctx [
        h1 [_class "title"] [
            str "Recipients"
            a [_href (Links.add ctx)] [
                span [_class "icon is-small ml-4"] [ i [_class "fas fa-square-plus"] [] ]
            ]
        ]
        div [_class "columns is-multiline"] recipientCardList                  
    ]

let recipientDetail ctx recipient =
    App.template ctx [
        h1 [_class "title"] [str recipient.Name]
        match recipient.Notes with
        | Some notes -> p [] [str notes]
        | None -> span [] []
    ]

let addEditRecipient ctx maybeRecipient maybeInput errors =
    let vals =
        match maybeRecipient, maybeInput with
        | Some _, Some i -> {| name = i.name; notes = (Some i.notes) |}
        | Some r, None -> {| name = r.Name; notes = r.Notes |}
        | None, Some i -> {| name = i.name; notes = (Some i.notes) |}
        | None, None -> {| name = ""; notes = None |}

    App.template ctx  [
        div [_class "modal is-active"] [
            div [_class "modal-background"] []
            div [_class "modal-content"] [
                div [_class "box"] [
                    h1 [_class "title"] [
                        match maybeRecipient with
                        | Some _ -> str "Edit Recipient"
                        | None -> str "Add Recipient"
                    ]
                    form [_action (Links.index ctx); _method "post"] [
                        div [_class "field"] [
                            label [_class "label"] [str "Name"]
                            div [_class "control"] [
                                input
                                    [ _class "input"
                                      _type "text"
                                      _name "name"
                                      _placeholder "John Doe"
                                      _value vals.name ]
                            ]
                            if Map.containsKey "name" errors then
                                p [ _class "help is-danger" ] [ str errors.["name"] ]
                            else span [] []
                        ]
                        div [_class "field"] [
                            label [_class "label"] [str "Notes*"]
                            div [_class "control"] [
                                textarea
                                    [_class "textarea"; _name "notes"]
                                    [str (vals.notes |> Option.defaultValue "")]
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