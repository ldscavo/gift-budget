module Program

open System.Reflection
open SimpleMigrations
open Npgsql
open SimpleMigrations.DatabaseProvider
open SimpleMigrations.Console
open Shared.Env

[<EntryPoint>]
let main argv =
    let assembly = Assembly.GetExecutingAssembly()

    let connectionString =
        Env.get "DATABASE_URL"
        |> Option.bind Shared.Database.connString
        |> function
            | Some conn -> conn
            | None -> failwith "No database connection set!"

    use db = new NpgsqlConnection(connectionString)
    
    let provider = PostgresqlDatabaseProvider(db)
    let migrator = SimpleMigrator(assembly, provider)
    let consoleRunner = ConsoleRunner(migrator)
    
    consoleRunner.Run(argv) |> ignore
    
    0