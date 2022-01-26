module Server

open Saturn
open Shared.Env
open Config

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
        use_config (fun _ ->
            {
                connectionString = connectionString
            }
    }

[<EntryPoint>]
let main _ =
    printfn "Working directory - %s" (System.IO.Directory.GetCurrentDirectory())    
    run app
    0