module Users.Database

open Database
open FSharp.Control.Tasks
open FsToolkit.ErrorHandling

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

let getAll (env: #IDb) =
    taskResult {
        let sql = """
            SELECT
                id, email, password, is_admin,
                created_on, updated_on
            FROM Users; """

        let! users = env.db.query sql None
        return users |> Seq.map toUser
    }

let getById (env: #IDb) id =
    taskResult {
        let query = """
            SELECT
                id, email, password, is_admin,
                created_on, updated_on
            FROM Users
            WHERE id = @id; """

        let! user = env.db.querySingle query (dict [ "id" => id ] |> Some)
        return user |>Option.map toUser
    }

let getByEmail (env: #IDb) (email: string) =
    taskResult {
        let query = """
            select
                id, email, password,
                is_admin, created_on, updated_on
            FROM Users
            WHERE email = @email; """

        let! user = env.db.querySingle query (dict [ "email" => email ] |> Some)
        return user |> Option.map toUser
    }

let update (env: #IDb) v =
    task {
        let query = """
            UPDATE Users
            SET email = @email,
                password = @password,
                is_admin = @is_admin,
                created_on = @created_on,
                updated_on = @updated_on
            WHERE id = @id; """

        return! env.db.execute query v
    }

let insert (env: #IDb) v =
    task {
        let query = """
            INSERT INTO Users
                (id, email, password, is_admin, created_on, updated_on)
            VALUES
                (@id, @email, @password, @is_admin, @created_on, @updated_on); """

        return! env.db.execute query v
    }

let delete (env: #IDb) id =
    task {
        return! env.db.execute "DELETE FROM Users WHERE id = @id;" (dict [ "id" => id ])
    }
