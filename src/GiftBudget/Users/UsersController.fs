module Users.Controller

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Config
open Saturn
open BCrypt.Net
open System.Security.Claims
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Authentication

[<CLIMutable>]
type QueryParams = {
    redirectUrl: string option
}

let private showLogin (ctx: HttpContext) =
    task {
        let queryParams = Controller.getQuery<QueryParams> ctx

        return!
            Views.login ctx None Map.empty queryParams.redirectUrl
            |> Controller.renderHtml ctx
    }

let private signInAuthorizedUser user ctx =
    task {
        let claims =
            [ Claim("userId", user.id.ToString())
              Claim("isAdmin", user.is_admin.ToString()) ]

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
                let! _ = signInAuthorizedUser user ctx                
                return! Controller.redirect ctx input.redirectUrl
            else
                let errorMsg = Map.ofList ["password", "Invalid password"]
                return!
                    Views.login ctx (Some input) errorMsg (Some input.redirectUrl)
                    |> Controller.renderHtml ctx

        | Ok None ->
            let errorMsg = Map.ofList ["email", "Invalid email"]
            return!
                Views.login ctx (Some input) errorMsg (Some input.redirectUrl)
                |> Controller.renderHtml ctx

        | Error ex ->
            return! Controller.renderHtml ctx (InternalError.layout ex)
    }

let private logoutUser (ctx: HttpContext) =
    task {
        let! _ = AuthenticationHttpContextExtensions.SignOutAsync ctx
        return! Controller.redirect ctx "/login"
    }

let login =
    controller {
        index showLogin
        create attemptLogin
    }

let logout = controller { index logoutUser }

let loggedInTest = controller {
    index Views.loginSuccess
}