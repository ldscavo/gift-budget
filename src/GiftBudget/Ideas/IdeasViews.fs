module Ideas.Views

open Giraffe.ViewEngine
open Saturn

let ideaLink url =
    span [_class "icon-text"] [
        a [_href url; _target "_blank"] [
            span [_class "icon "] [ i [_class "fas fa-link"] [] ]
        ]
    ]

let ideaRow ctx idea =
    tr [] [
        td [] [
            str idea.Text
            match idea.Link with
            | Some linkUrl -> ideaLink linkUrl
            | None -> span [] []
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
            a [_href (Links.add ctx)] [
                span [_class "icon is-small ml-4"] [ i [_class "fas fa-square-plus"] [] ]
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

let addEditIdea ctx (maybeIdea: Idea option) (input: IdeaInput option) (errors: Map<string, string>) =
    App.template ctx [
        
    ]