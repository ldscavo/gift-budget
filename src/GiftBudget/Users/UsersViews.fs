namespace Users

open Microsoft.AspNetCore.Http
open Giraffe.GiraffeViewEngine
open Saturn

module Views =
    let login (ctx: HttpContext) =
        App.layout [
            section [ _class "section" ] [
                div [ _class "container" ] [
                    form [ _action (Links.index ctx); _method "post" ] [
                        h1 [] [ str "Login" ]
                        div [] [
                            label [ _for "email" ] [ encodedText "Email:" ]
                            br []
                            input [ _type "email"; _id "email"; _name "email"; _placeholder "user@example.com" ] 
                        ]
                        div [] [
                            label [_for "password"] [ encodedText "Password:" ]
                            br []
                            input [ _type "password"; _id "password"; _name "password" ] 
                        ]
                        div [] [ input [ _type "submit"; _value "Login >>" ] ]
                    ]
                ]
            ]
        ]

    let private form (ctx: HttpContext) (o: User option) (validationResult: Map<string, string>) isUpdate =
        let validationMessage =
            div [ _class "notification is-danger" ] [
                a [ _class "delete"
                    attr "aria-label" "delete" ] []
                encodedText "Oops, something went wrong! Please check the errors below."
            ]

        let field selector lbl key =
            div [ _class "field" ] [
                yield
                    label [ _class "label" ] [
                        encodedText (string lbl)
                    ]
                yield
                    div [ _class "control has-icons-right" ] [
                        yield
                            input [ _class (
                                        if validationResult.ContainsKey key then
                                            "input is-danger"
                                        else
                                            "input"
                                    )
                                    _value (defaultArg (o |> Option.map selector) "")
                                    _name key
                                    _type "text" ]
                        if validationResult.ContainsKey key then
                            yield
                                span [ _class "icon is-small is-right" ] [
                                    i [ _class "fas fa-exclamation-triangle" ] []
                                ]
                    ]
                if validationResult.ContainsKey key then
                    yield
                        p [ _class "help is-danger" ] [
                            encodedText validationResult.[key]
                        ]
            ]

        let buttons =
            div [ _class "field is-grouped" ] [
                div [ _class "control" ] [
                    input [ _type "submit"
                            _class "button is-link"
                            _value "Submit" ]
                ]
                div [ _class "control" ] [
                    a [ _class "button is-text"
                        _href (Links.index ctx) ] [
                        encodedText "Cancel"
                    ]
                ]
            ]

        let cnt =
            [ div [ _class "container " ] [
                  form [ _action (
                             if isUpdate then
                                 Links.withId ctx o.Value.id
                             else
                                 Links.index ctx
                         )
                         _method "post" ] [
                      if not validationResult.IsEmpty then
                          yield validationMessage
                      yield field (fun i -> (string i.id)) "Id" "id"
                      yield field (fun i -> (string i.email)) "Email" "email"
                      yield field (fun i -> (string i.password)) "Password" "password"
                      yield field (fun i -> (string i.is_admin)) "Is_admin" "is_admin"
                      yield field (fun i -> (string i.created_on)) "Created_on" "created_on"
                      yield field (fun i -> (string i.updated_on)) "Updated_on" "updated_on"
                      yield buttons
                  ]
              ] ]

        App.layout ([ section [ _class "section" ] cnt ])

    let add (ctx: HttpContext) (o: User option) (validationResult: Map<string, string>) =
        form ctx o validationResult false

    let edit (ctx: HttpContext) (o: User) (validationResult: Map<string, string>) =
        form ctx (Some o) validationResult true
