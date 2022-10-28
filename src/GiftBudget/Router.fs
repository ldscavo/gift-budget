module Router

open Saturn
open Giraffe
open Microsoft.AspNetCore.Http
open Users
open FSharpPlus

let browser =
    pipeline {
        plug putSecureBrowserHeaders
        plug fetchSession
        set_header "x-pipeline-type" "Browser"
    }

let redirectOnLoggedOut next (ctx: HttpContext) =
    let redirect = ctx.GetRequestUrl ()

    ctx.SetHttpHeader("HX-Redirect", $"/login?redirectUrl={redirect}")
    ctx.SetStatusCode 200    

    Users.Views.login ctx None Map.empty (Some redirect)
    |> Controller.renderHtml ctx

let requires_login =
    pipeline {
        requires_authentication redirectOnLoggedOut  
    }

let defaultView env =
    router {
        get "/" (htmlView Index.layout)
        get "/index.html" (redirectTo false "/")
        get "/default.html" (redirectTo false "/")

        // routes that do not require being logged in
        forward "/login" (Users.Controller.login env)
        forward "/register" (Users.Controller.register env)
    }

let loggedInRouter env =
    router {
        pipe_through requires_login
        forward "/logout" Users.Controller.logout
        forward "/recipients" (Recipients.Controller.resource env)
        forward "/ideas" (Ideas.Controller.resource env)
    }

let appRouter env =
    router {
        not_found_handler (htmlView NotFound.layout) //Use the default 404 webpage
        pipe_through browser //Use the default browser pipeline
    
        forward "" (defaultView env) //Use the default view    
        forward "" (loggedInRouter env)
    }