module Server

open Saturn
open Shared.Env
open Config
open Microsoft.AspNetCore.Authentication
open System

let endpointPipe = pipeline {
    plug head
    plug requestId
}

let app =
    let port = Env.get "PORT" ??- "8085"
    let connectionString =
        Env.get "DATABASE_URL"
        |> Option.bind Shared.Database.connString
        |> Option.get // throws if None

    application {
        pipe_through endpointPipe

        error_handler (fun ex _ -> pipeline { render_html (InternalError.layout ex) })
        use_router Router.appRouter
        url (sprintf "http://0.0.0.0:%s/" port)
        memory_cache
        use_static "static"
        use_gzip
        use_config (fun _ -> { connectionString = connectionString })
        use_cookies_authentication_with_config
            ( fun options ->
                  options.ExpireTimeSpan <- TimeSpan.FromDays 3
                  options.SlidingExpiration <- true
                  options.LoginPath <- "/login" )       
    }

[<EntryPoint>]
let main _ =
    printfn "Working directory - %s" (System.IO.Directory.GetCurrentDirectory())    
    run app
    0