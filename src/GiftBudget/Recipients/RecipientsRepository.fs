module Recipients.Repository

open System
open Database
open System.Threading.Tasks
open FSharp.Control.Tasks

[<CLIMutable>]
type RecipientDataEntity =
    { id: Guid
      user_id: Guid
      name: string
      notes: string
      created_on: DateTime
      updated_on: DateTime }

let toRecipient(r: RecipientDataEntity) =
    { Id = r.id
      UserId = r.user_id
      Name = r.name
      Notes = if (String.IsNullOrWhiteSpace r.notes) then None else (Some r.notes)
      CreatedOn = r.created_on
      UpdatedOn = r.updated_on }

let fromRecipient (r: Recipient) =
    { id = r.Id
      user_id = r.UserId
      name = r.Name
      notes =
        match r.Notes with
        | Some note -> note
        | None -> null
      created_on = r.CreatedOn
      updated_on = r.UpdatedOn }

let getAllForUser (env: #IDb) (userId: Guid) =
    task {
        let sql = """
            SELECT id, user_id, name, notes, created_on, updated_on
            FROM Recipients
            WHERE user_id = @userId;
        """
        let! recipients = env.db.query sql (dict ["userId" => userId] |> Some)
        return recipients
        |> Result.map (List.map toRecipient)
    }

let getById (env: #IDb) (id: Guid) =
    task {
        let sql = """
            SELECT id, user_id, name, notes, created_on, updated_on
            FROM Recipients
            WHERE id = @id;
        """

        let! recipients = env.db.querySingle sql (dict ["id" => id] |> Some)        
        return recipients |> Result.map (Option.map toRecipient)
    }

let insert (env: #IDb) (recipient: Recipient) : Task<Result<int, exn>> =
    task {
        let sql = """
            INSERT INTO Recipients
                (id, user_id, name, notes, created_on, updated_on)
            VALUES
                (@id, @user_id, @name, @notes, @created_on, @updated_on)
        """

        return! env.db.execute sql (recipient |> fromRecipient)
    }