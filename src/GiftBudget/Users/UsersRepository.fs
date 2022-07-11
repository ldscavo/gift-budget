module Users.Database

open Database
open FSharp.Control.Tasks
open FsToolkit.ErrorHandling

[<CLIMutable>]
type UserDto =
    { id: System.Guid
      email: string
      password: string
      is_admin: bool
      created_on: System.DateTime
      updated_on: System.DateTime }

let toUser (u: UserDto) =
    { Id = u.id
      Email = u.email
      Password = u.password
      Type =
          match u.is_admin with
          | true -> Admin
          | false -> User
      CreatedOn = u.created_on
      UpdatedOn = u.updated_on }

let fromUser (u: User) =
    { id = u.Id
      email = u.Email
      password = u.Password
      is_admin =
        match u.Type with
        | User -> false
        | Admin -> true
      created_on = u.CreatedOn
      updated_on = u.UpdatedOn }

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

        let! user = env.db.querySingle query (Some {| id = id |})
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

        let! user = env.db.querySingle query (Some {| email = email |})
        return user |> Option.map toUser
    }

let update (env: #IDb) user =
    task {
        let query = """
            UPDATE Users
            SET email = @email,
                password = @password,
                is_admin = @is_admin,
                created_on = @created_on,
                updated_on = @updated_on
            WHERE id = @id; """

        return! env.db.execute query user
    }

let insert (env: #IDb) user =
    task {
        let query = """
            INSERT INTO Users
                (id, email, password, is_admin, created_on, updated_on)
            VALUES
                (@id, @email, @password, @is_admin, @created_on, @updated_on)
        """

        return! env.db.execute query (user |> fromUser)
    }

let delete (env: #IDb) id =
    task {
        return! env.db.execute "DELETE FROM Users WHERE id = @id;" (dict [ "id" => id ])
    }
