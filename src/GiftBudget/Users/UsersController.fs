module Users.Controller

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.ContextInsensitive
open Config
open Saturn
open BCrypt.Net

let private showLogin (ctx: HttpContext) =
    task {
        return!
            Views.login ctx None Map.empty
            |> Controller.renderHtml ctx
    }

let private attemptLogin (ctx: HttpContext) =
    task {
        let cnf = Controller.getConfig ctx
        let! input = Controller.getModel<Login> ctx

        let! maybeUser = Database.getByEmail cnf.connectionString input.email

        match maybeUser with
        | Ok (Some user) ->
            if BCrypt.Verify(input.password, user.password) then            
                // mark the user as authenticated somehow
                return! Controller.renderHtml ctx (Views.loginSuccess ctx)
            else
                let errorMsg = Map.ofList ["password", "Invalid password"]
                return! Controller.renderHtml ctx (Views.login ctx (Some input) errorMsg)
        | Ok None ->
            let errorMsg = Map.ofList ["email", "Invalid email"]
            return! Controller.renderHtml ctx (Views.login ctx (Some input) errorMsg)
        | Error ex ->
            return! Controller.renderHtml ctx (InternalError.layout ex)
    }

let login =
    controller {
        index showLogin
        create attemptLogin
    }