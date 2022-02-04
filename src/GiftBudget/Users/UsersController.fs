module Users.Controller

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.ContextInsensitive
open Config
open Saturn

let private showLogin (ctx: HttpContext) =
    task {
        return!
            Views.login ctx None Map.empty
            |> Controller.renderHtml ctx
    }

let login =
    controller {
        index showLogin
        create showLogin
    }