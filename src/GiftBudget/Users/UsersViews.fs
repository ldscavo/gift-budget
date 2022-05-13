module Users.Views

open Giraffe.ViewEngine
open Saturn

let getOrDefault f defaultValue opt =
    match opt with
    | Some o -> f o
    | None -> defaultValue

let login ctx login formErrors redirectUrl =
    let maybeError key =
        if Map.containsKey key formErrors then
            p [ _class "help is-danger" ] [ str formErrors.[key] ]
        else span [] []

    html [_style "background: #42babd"] [
        App.layoutHead
        body [_id "login-page"] [
            div [_class "container mt-6"] [
                div [ _class "columns is-centered" ] [
                    div [_class "column is-half"] [
                        div [_class "box"] [
                            h1 [_class "is-size-3 has-text-centered"] [
                                img [_src "/logo.png"]
                                str "Gift Budget"
                            ]
                            form [_class "control"; _action (Links.index ctx); _method "post"] [                    
                                div [_class "field"] [
                                    label [_class "label"] [str "Email"]                        
                                    input [
                                        _class "input"; _name "email"; _placeholder "user@example.com"
                                        _value (login |> getOrDefault (fun l -> l.email) "")
                                    ]                        
                                    maybeError "email"
                                ]
                                div [_class "field"] [
                                    label [_class "label"] [str "Password"]                        
                                    input [_class "input"; _type "password"; _name "password"]                        
                                    maybeError "password"
                                ]
                                div [_class "field"] [
                                    input [
                                        _type "hidden"; _name "redirectUrl"
                                        _value (redirectUrl |> Option.defaultValue "/")
                                    ]
                                    button [ _class "button is-link"; _type "submit" ] [
                                        span [] [str "Logon"]
                                        span [_class "icon is-small"] [ i [_class "fas fa-angles-right"] [] ]
                                    ]
                                ]                        
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]    

let loginSuccess ctx =
    str "HUZZAH"
    |> System.Threading.Tasks.Task.FromResult