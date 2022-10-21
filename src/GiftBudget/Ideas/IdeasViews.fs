module Ideas.Views

open Giraffe.ViewEngine
open Giraffe.Htmx
open Saturn
open Ideas
open Recipients

let ideaLink text url =
    span [_class "icon-text"] [
        span [] [str text]
        a [_class "icon"; _href url; _target "_blank"] [
            i [_class "fas fa-link"] []
        ]
    ]

let ideaRow ctx idea =
    tr [] [
        td [] [
            match idea.Link with
            | Some linkUrl -> ideaLink idea.Text linkUrl
            | None -> str idea.Text
        ]
        td [] [
            match idea.Price with
            | Some price -> str (price.ToString "C")
            | None -> str ""
        ]
        td [] [
            match idea.Recipient with
            | NoRecipient -> str ""
            | IdeaRecipient recipient -> str recipient.Name
            | IdeaRecipients recipients -> str (recipients |> List.map (fun r -> r.Name) |> String.concat ", ")
        ]
    ]

let ideasList ctx ideas =
    App.template ctx [
        h1 [_class "title"] [
            str "Ideas"
            Components.modalLink (Links.add ctx) [
                span [_class "icon is-small ml-4"] [
                    i [_class "fas fa-square-plus"] []
                ]
            ]
        ]
        table [_class "table"] [
            thead [] [
                th [] [str "Idea"]
                th [] [str "Price"]
                th [] [str "Recipient"]
            ]
            tbody [] (ideas |> List.map (ideaRow ctx))
        ]
    ]
    
let ideaDetail ctx idea =
    App.template ctx [
        h1 [_class "title"] [str idea.Text]
        match idea.Recipient with
        | IdeaRecipient recipient ->
            p [] [str recipient.Name]
        | IdeaRecipients recipients ->
            div [] (recipients |> List.map (fun r -> p [] [str r.Name]))
        | NoRecipient ->
            span [] []
    ]

let addEditIdea ctx maybeIdea (maybeInput: IdeaInput option) maybeRecipient (recipients: Recipient list) (errors: Map<string, string>) =
    App.template ctx [
        div [] [
            h1 [_class "title"] [
                match maybeIdea with
                | Some _ -> str "Edit Idea"
                | None -> str "Add Idea"
            ]
            form [_action "/ideas"; _method "post"] [
                div [_class "field"] [
                    label [_class "label"] [str "Idea"]
                    div [_class "control"] [
                        input
                            [ _class "input"
                              _type "text"
                              _name "text"
                              _placeholder "Socks" ]
                    ]
                ]
                if (recipients |> List.isEmpty |> not) then
                    div [_class "field"] [
                        label [_class "label"] [str "For"]
                        div [_class "select"] [
                            select [_name "recipient"]
                                (recipients |> List.map (fun r ->
                                    option [
                                        _value (r.Id.ToString())
                                        if (maybeRecipient.ForId |> Option.map ((=) r.Id) |> Option.defaultValue false) then _selected
                                    ] [str r.Name])) 
                        ]                                               
                    ]
                div [_class "field"] [
                    label [_class "label"] [str "Price"]
                    div [_class "control"] [
                        input
                            [ _class "input"
                              _type "number"
                              _step "0.01"
                              _name "price" ]
                    ]
                ]
                div [_class "field"] [
                    label [_class "label"] [str "Link"]
                    div [_class "control"] [
                        input
                            [ _class "input"
                              _type "text"
                              _name "link"
                              _placeholder "Socks"]
                    ]
                ]
                div [_class "field is-grouped"] [
                    div [_class "control"] [
                        button [_class "button is-link"; _type "submit"] [str "Save"]
                    ]
                    div [_class "control"] [
                        match ctx.Request.IsHtmx with
                        | false -> a [_class "button is-link is-light"; _href (Links.index ctx)] [str "Cancel"]
                        | true -> button [_class "exit-modal button is-light"; _type "button"] [str "Cancel"]
                    ]
                ]
            ]
        ]
    ]