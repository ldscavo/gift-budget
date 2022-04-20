module Users.Views

open Microsoft.AspNetCore.Http
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

    App.layout [
        div [ _class "container" ] [
            h1 [_class "is-size-3"] [ str "Login" ]
            div [_class "box"] [
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
                        input [ _class "button is-link"; _type "submit"; _value "Login" ]
                    ]                        
                ]
            ]
        ]
        
    ]

let loginSuccess ctx =
    str "HUZZAH"
    |> System.Threading.Tasks.Task.FromResult