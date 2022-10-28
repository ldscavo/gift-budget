module Ideas.Repository

open System
open Database
open System.Threading.Tasks
open FsToolkit.ErrorHandling
open FSharpPlus

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
      Recipients = []
      CreatedOn = i.created_on
      UpdatedOn = i.updated_on }

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

let getRecipientsForIdea (env: #IDb) (idea: Idea) =
    taskResult {
        let! recipients =
            idea.Id |> Recipients.Repository.getAllForIdea env

        return
            { idea with
                Recipients = recipients }
    }

let getRecipientsForIdeaOption (env: #IDb) (idea: Idea) =
    taskResult {
        let! ideaWithRecipients = idea |> getRecipientsForIdea env
        return Some ideaWithRecipients
    }

let getAllForUser (env: #IDb) (userId: Guid) :  Task<Result<Idea list, exn>> =
    taskResult {
        let sql = """
            SELECT
                id, user_id, text, price, link, created_on, updated_on
            FROM ideas
            WHERE user_id = @userId
        """

        let! ideaDataModels = env.db.query sql (Some {| userId = userId |})
        let ideas = ideaDataModels |> List.map toIdea
        return! ideas |> List.traverseTaskResultM (getRecipientsForIdea env)
    }

let getAllForRecipient (env: #IDb) (recipientId: Guid) =
    taskResult {
        let sql = """
            SELECT
                i.id, i.user_id, i.text, i.price, i.link, i.created_on, i.updated_on
            FROM ideas AS i
            JOIN idearecipients AS ir ON
                ir.idea_id = i.id AND
                ir.recipient_id = @recipientId
        """

        let! ideaDataModels = env.db.query sql (Some {| recipientId = recipientId |})
        let ideas = ideaDataModels |> List.map toIdea
        return! ideas |> List.traverseTaskResultM (getRecipientsForIdea env)
    }

let getById (env: #IDb) (id: Guid) =
    taskResultOption {
        let sql = """
            SELECT
                id, user_id, text, price, link, created_on, updated_on
            FROM ideas
            WHERE id = @id
        """

        let! ideaDataModel = env.db.querySingle sql (Some {| id = id |})
        let idea = ideaDataModel |>  toIdea

        return! idea |> (getRecipientsForIdeaOption env)
    }

let addRecipient (env: #IDb) ideaId recipientId =
    taskResult {
        let sql = """
            INSERT INTO idearecipients
                (idea_id, recipient_id)
            VALUES
                (@ideaId, @recipientId)
        """

        let! _ = env.db.execute sql {| ideaId = ideaId; recipientId = recipientId |}
        return ()
    }

let insert (env: #IDb) (idea: Idea) =
    taskResult {
        let sql = """
            INSERT INTO ideas
                (id, user_id, text, price, link, created_on, updated_on)
            VALUES
                (@id, @user_id, @text, @price, @link, @created_on, @updated_on)
        """

        let! result =
            idea
            |> fromIdea
            |> env.db.execute sql

        do!
            idea.Recipients
            |> List.map (fun r -> addRecipient env idea.Id r.Id)
            |> Task.WhenAll
            |> Task.ignore
                                
        return result
    }