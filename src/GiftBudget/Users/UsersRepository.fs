namespace Users

open Database
open System.Threading.Tasks
open FSharp.Control.Tasks.ContextInsensitive
open Npgsql

module Database =
    let getAll connectionString : Task<Result<User seq, exn>> =
        task {
            use connection = new NpgsqlConnection(connectionString)
            return! query connection "SELECT id, email, password, is_admin, created_on, updated_on FROM Users" None
        }

    let getById connectionString id : Task<Result<User option, exn>> =
        task {
            use connection = new NpgsqlConnection(connectionString)

            return!
                querySingle
                    connection
                    "SELECT id, email, password, is_admin, created_on, updated_on FROM Users WHERE id=@id"
                    (Some <| dict [ "id" => id ])
        }

    let update connectionString v : Task<Result<int, exn>> =
        task {
            use connection = new NpgsqlConnection(connectionString)

            return!
                execute
                    connection
                    "UPDATE Users SET id = @id, email = @email, password = @password, is_admin = @is_admin, created_on = @created_on, updated_on = @updated_on WHERE id=@id"
                    v
        }

    let insert connectionString v : Task<Result<int, exn>> =
        task {
            use connection = new NpgsqlConnection(connectionString)

            return!
                execute
                    connection
                    "INSERT INTO Users(id, email, password, is_admin, created_on, updated_on) VALUES (@id, @email, @password, @is_admin, @created_on, @updated_on)"
                    v
        }

    let delete connectionString id : Task<Result<int, exn>> =
        task {
            use connection = new NpgsqlConnection(connectionString)
            return! execute connection "DELETE FROM Users WHERE id=@id" (dict [ "id" => id ])
        }
