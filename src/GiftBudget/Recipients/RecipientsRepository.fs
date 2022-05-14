module Recipients.Repository

open System
open Database
open System.Threading.Tasks
open FSharp.Control.Tasks
open Npgsql

type RecipientDataEntity =
    { id: Guid
      name: string
      notes: string
      created_on: DateTime
      updated_on: DateTime }

let toRecipient(r: RecipientDataEntity) =
    { Id = r.id
      Name = r.name
      Notes = if (String.IsNullOrWhiteSpace r.notes) then None else (Some r.notes)
      CreatedOn = r.created_on
      UpdatedOn = r.updated_on }

let fromRecipient (r: Recipient) =
    { id = r.Id
      name = r.Name
      notes =
        match r.Notes with
        | Some note -> note
        | None -> null
      created_on = r.CreatedOn
      updated_on = r.UpdatedOn }

let getAllForUser connString (userId: Guid) =
    task {
        use connection = new NpgsqlConnection(connString)
        let sql = """
            SELECT id, name, notes, created_on, updated_on
            FROM Recipients
            WHERE user_id = @userId;
        """
        let! recipients = query connection sql (dict ["userId" => userId] |> Some)
        return recipients
        |> Result.map (List.map toRecipient)
    }

let getById connString (id: Guid) =
    task {
        use connection = new NpgsqlConnection(connString)
        let sql = """
            SELECT id, name, notes, created_on, updated_on
            FROM Recipients
            WHERE id = @id;
        """

        let! recipients = querySingle connection sql (dict ["id" => id] |> Some)        
        return recipients |> Result.map (Option.map toRecipient)
    }

let insert connectionString (recipient: Recipient) : Task<Result<int, exn>> =
    task {
        use connection = new NpgsqlConnection(connectionString)
        let sql = """
            INSERT INTO Recipients
                (id, name, notes, created_on, updated_on)
            VALUES
                (@id, @name, @notes, created_on, updated_on)
        """

        return! execute connection sql (recipient |> fromRecipient)
    }