module Recipients.Repository

open System
open Database
open System.Threading.Tasks
open FSharp.Control.Tasks
open Npgsql

type private RecipientDto =
    { id: Guid
      name: string
      notes: string
      created_on: DateTime
      updated_on: DateTime }

let private toRecipient(r: RecipientDto) =
    { Id = r.id
      Name = r.name
      Notes = if (String.IsNullOrWhiteSpace r.notes) then (Some r.notes) else None
      CreatedOn = r.created_on
      UpdatedOn = r.updated_on }

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
        |> Result.map (Seq.map toRecipient)
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