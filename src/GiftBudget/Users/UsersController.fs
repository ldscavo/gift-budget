module Users.Controller

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Config
open Saturn
open BCrypt.Net
open System.Security.Claims
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Authentication

let private showLogin (ctx: HttpContext) =
    task {
        return!
            Views.login ctx None Map.empty
            |> Controller.renderHtml ctx
    }

let private saveLogin user ctx =
    task {
        let claims = [
            Claim("userId", user.id.ToString())
            Claim("isAdmin", user.is_admin.ToString())
        ]

        let identity = ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)                
    
        return! AuthenticationHttpContextExtensions.SignInAsync(ctx, ClaimsPrincipal(identity))
    }

let private attemptLogin (ctx: HttpContext) =
    task {
        let cnf = Controller.getConfig ctx
        let! input = Controller.getModel<Login> ctx

        let! maybeUser = Database.getByEmail cnf.connectionString input.email

        match maybeUser with
        | Ok (Some user) ->
            if BCrypt.Verify(input.password, user.password) then      
                let! _ = saveLogin user ctx
                return! Controller.redirect ctx "/test"
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

let loggedInTest = controller {
    index Views.loginSuccess
}