module Users.Controller

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Config
open Saturn
open System.Security.Claims
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Authentication
open Users.Service

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
        let claims = [ Claim("userId", user.Id.ToString()) ]
        let identity = ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)                
    
        return! AuthenticationHttpContextExtensions.SignInAsync(ctx, ClaimsPrincipal(identity))
    }

let private attemptLogin env (ctx: HttpContext) =
    task {
        let cnf = Controller.getConfig ctx
        let! input = Controller.getModel<Login> ctx

        let! maybeUser = Database.getByEmail env input.email
        let loginResult = Service.verifyLogin maybeUser input

        match loginResult with
        | LoginResult.Success user ->
            let! _ = signInAuthorizedUser user ctx                
            return! Controller.redirect ctx input.redirectUrl
        | BadPassword login ->
            let errorMsg = Map.ofList ["password", "Invalid password"]
            return!
                Views.login ctx (Some login) errorMsg (Some login.redirectUrl)
                |> Controller.renderHtml ctx
        | BadEmail login ->
            let errorMsg = Map.ofList ["email", "Invalid email"]
            return!
                Views.login ctx (Some login) errorMsg (Some login.redirectUrl)
                |> Controller.renderHtml ctx
        | LoginError ex ->
            return! Controller.renderHtml ctx (InternalError.layout ex)
    }

let private showRegister ctx =
    task {
        let view = Views.register ctx None Map.empty
        return! Controller.renderHtml ctx view
    }

let private logoutUser (ctx: HttpContext) =
    task {
        let! _ = AuthenticationHttpContextExtensions.SignOutAsync ctx
        return! Controller.redirect ctx "/login"
    }

let login env =
    controller {
        index showLogin
        create (attemptLogin env)
    }

let register env =
    controller {
        index showRegister
        //create (attemptLogin env)
    }

let logout =
    controller {
        index logoutUser
    }