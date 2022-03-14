module Users.Database

open Database
open System.Threading.Tasks
open FSharp.Control.Tasks
open Npgsql

type private UserDto =
    { id: System.Guid
      email: string
      password: string
      is_admin: bool
      created_on: System.DateTime
      updated_on: System.DateTime }

let private toUser (u: UserDto) =
    { Id = u.id
      Email = u.email
      Password = u.password
      Type =
          match u.is_admin with
          | true -> Admin
          | false -> User
      CreatedOn = u.created_on
      UpdatedOn = u.updated_on }

let getAll connectionString =
    task {
        use connection = new NpgsqlConnection(connectionString)
        let sql = """
            SELECT
                id, email, password, is_admin,
                created_on, updated_on
            FROM Users;"""

        let! users = query connection sql None
        return users
        |> Result.map (Seq.map toUser)
    }

let getById connectionString id : Task<Result<User option, exn>> =
    task {
        use connection = new NpgsqlConnection(connectionString)
        let query = """
            SELECT
                id, email, password, is_admin,
                created_on, updated_on
            FROM Users
            WHERE id = @id;"""

        let! user = querySingle connection query (dict [ "id" => id ] |> Some)
        return user |> Result.map (Option.map toUser)
    }

let getByEmail connectionString (email: string) : Task<Result<User option, exn>> =
    task {
        use conn = new NpgsqlConnection(connectionString)
        let query = """
            select
                id, email, password,
                is_admin, created_on, updated_on
            FROM Users
            WHERE email = @email; """

        let! user = querySingle conn query (dict [ "email" => email ] |> Some)
        return user |> Result.map (Option.map toUser)
    }

let update connectionString v : Task<Result<int, exn>> =
    task {
        use connection = new NpgsqlConnection(connectionString)
        let query = """
            UPDATE Users
            SET email = @email,
                password = @password,
                is_admin = @is_admin,
                created_on = @created_on,
                updated_on = @updated_on
            WHERE id = @id; """

        return! execute connection query v
    }

let insert connectionString v : Task<Result<int, exn>> =
    task {
        use connection = new NpgsqlConnection(connectionString)
        let query = """
            INSERT INTO Users
                (id, email, password, is_admin, created_on, updated_on)
            VALUES
                (@id, @email, @password, @is_admin, @created_on, @updated_on); """

        return! execute connection query v
    }

let delete connectionString id : Task<Result<int, exn>> =
    task {
        use connection = new NpgsqlConnection(connectionString)

        return! execute connection "DELETE FROM Users WHERE id = @id;" (dict [ "id" => id ])
    }
