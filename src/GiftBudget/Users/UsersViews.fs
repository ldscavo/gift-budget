module Users.Views

open Microsoft.AspNetCore.Http
open Giraffe.ViewEngine
open Saturn

let getOrDefault f defaultValue opt =
    match opt with
    | Some o -> f o
    | None -> defaultValue

let login (ctx: HttpContext) (login: Login option) (formErrors: Map<string, string>)  =
    let maybeError key =
        if Map.containsKey key formErrors then
            p [ _class "help is-danger" ] [ str formErrors.[key] ]
        else span [] []

    App.layout [
        section [ _class "section" ] [
            div [ _class "container" ] [
                form [ _action (Links.index ctx); _method "post" ] [
                    h1 [] [ str "Login" ]
                    div [] [
                        label [ _for "email" ] [ encodedText "Email:" ]
                        br []
                        input [
                            _type "email"
                            _id "email"
                            _name "email"
                            _placeholder "user@example.com"
                            _value (login |> getOrDefault (fun l -> l.email) "")
                        ]
                        maybeError "email"
                    ]
                    div [] [
                        label [_for "password"] [ encodedText "Password:" ]
                        br []
                        input [ _type "password"; _id "password"; _name "password" ]
                        maybeError "password"
                    ]
                    div [] [ input [ _type "submit"; _value "Login >>" ] ]
                ]
            ]
        ]
    ]

let loginSuccess ctx = str "HUZZAH"