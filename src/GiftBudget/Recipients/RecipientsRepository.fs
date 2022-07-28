module Recipients.Repository

open System
open Database
open System.Threading.Tasks
open FSharp.Control.Tasks
open FsToolkit.ErrorHandling

[<CLIMutable>]
type RecipientDataEntity =
    { id: Guid
      user_id: Guid
      name: string
      notes: string
      created_on: DateTime
      updated_on: DateTime }

let toRecipient (r: RecipientDataEntity) =
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
    taskResult {
        let sql = """
            SELECT id, user_id, name, notes, created_on, updated_on
            FROM Recipients
            WHERE user_id = @userId;
        """
        let! recipients = env.db.query sql (Some {| userId = userId |})

        return recipients |> List.map toRecipient
    }

let getAllForIdea (env: #IDb) (ideaId: Guid) =
    taskResult {
        let sql = """
            SELECT
                r.id, r.user_id, r.name, r.notes, r.created_on, r.updated_on
            FROM recipients AS r
            JOIN idearecipients AS ir ON
                ir.recipient_id = r.id AND
                ir.idea_id = @ideaId
        """

        let! recipients = env.db.query sql (Some {| ideaId = ideaId |})
        return recipients |> List.map toRecipient
    }

let getById (env: #IDb) (id: Guid) =
    taskResult {
        let sql = """
            SELECT id, user_id, name, notes, created_on, updated_on
            FROM Recipients
            WHERE id = @id;
        """

        let! recipients = env.db.querySingle sql (Some {| id = id |})
        return recipients |> Option.map toRecipient
    }

let getByIds (env: #IDb) (ids: Guid array) =
    taskResult {
        let sql = """
            SELECT id, user_id, name, notes, created_on, updated_on
            FROM Recipients
            WHERE id in @ids;
        """

        let! recipients = env.db.query sql (Some {| ids = ids |})
        return recipients |> List.map toRecipient
    }

let insert (env: #IDb) (recipient: Recipient) =
    task {
        let sql = """
            INSERT INTO Recipients
                (id, user_id, name, notes, created_on, updated_on)
            VALUES
                (@id, @user_id, @name, @notes, @created_on, @updated_on)
        """

        return! env.db.execute sql (recipient |> fromRecipient)
    }