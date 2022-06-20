module Ideas.Repository

open System
open Database
open FSharp.Control.Tasks

[<CLIMutable>]
type IdeaDataEntity =
    { id: Guid
      user_id: Guid
      text: string
      price: Nullable<decimal>
      link: string
      created_on: DateTime
      updated_on: DateTime }

let toIdea i =
    { Id = i.id
      UserId = i.user_id
      Text = i.text
      Price = Option.ofNullable i.price
      Link =
          match String.IsNullOrWhiteSpace i.link with
          | false -> Some i.link
          | true -> None
      CreatedOn = i.created_on
      UpdatedOn = i.updated_on
      Recipient = NoRecipient }

let fromIdea i =
    { id = i.Id
      user_id = i.UserId
      text = i.Text
      price = Option.toNullable i.Price
      link =
          match i.Link with
          | Some link -> link
          | None -> null
      created_on = i.CreatedOn
      updated_on = i.UpdatedOn }

let getAllForUser (env: #IDb) (userId: Guid) =
    task {
        let sql = """
            SELECT
                id, user_id, text, price, link, created_on, updated_on
            FROM ideas
            WHERE user_id = @userId
        """

        let! ideas = env.db.query sql (dict ["userId" => userId] |> Some)
        return ideas |> Result.map (List.map toIdea)
    }

let getAllForRecipient (env: #IDb) (recipientId: Guid) =
    task {
        let sql = """
            SELECT
                i.id, i.user_id, i.text, i.price, i.link, i.created_on, i.updated_on
            FROM ideas AS i
            JOIN idearecipients AS ir ON
                ir.idea_id = i.id AND
                ir.recipient_id = @recipientId
        """

        let! ideas = env.db.query sql (dict ["recipientId" => recipientId] |> Some)
        return ideas |> Result.map (List.map toIdea)
    }