module Server

open Saturn
open Shared.Env
open Config
open Microsoft.AspNetCore.Authentication
open System

let endpointPipe =
    pipeline {
        plug head
        plug requestId
    }

let app =
    let connectionString =
        Env.unsafeGet "DATABASE_URL"
        |> Shared.Database.connString
        |> Option.get // throws if None

    let authOptions (options: Cookies.CookieAuthenticationOptions) =
        options.ExpireTimeSpan <- TimeSpan.FromDays 3
        options.SlidingExpiration <- true
        options.LoginPath <- "/login"
        options.ReturnUrlParameter <- "redirectUrl"

    let env = AppEnv(connectionString)

    application {
        pipe_through endpointPipe
        error_handler (fun ex _ -> pipeline { render_html (InternalError.layout ex) })
        use_router (Router.appRouter env)
        memory_cache
        use_static "static"
        use_gzip
        use_config (fun _ -> { connectionString = connectionString })
        use_cookies_authentication_with_config authOptions
        use_iis
    }

[<EntryPoint>]
let main _ =
    printfn "Working directory - %s" (System.IO.Directory.GetCurrentDirectory())    
    run app
    0