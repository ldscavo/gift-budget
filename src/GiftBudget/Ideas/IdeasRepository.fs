module Ideas.Repository

open System
open Database
open System.Threading.Tasks
open FsToolkit.ErrorHandling

[<CLIMutable>]
type IdeaDataEntity =
    { id: Guid
      user_id: Guid
      text: string
      price: Nullable<decimal>
      link: string
      created_on: DateTime
      updated_on: DateTime }

let toIdeaRecipients recipients =
    match recipients with
    | [] -> NoRecipient
    | [recipient] -> IdeaRecipient recipient
    | _ -> IdeaRecipients recipients

let toIdea i =
    { Id = i.id
      UserId = i.user_id
      Text = i.text
      Price = Option.ofNullable i.price
      Link =
          match String.IsNullOrWhiteSpace i.link with
          | false -> Some i.link
          | true -> None
      Recipient = NoRecipient
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
                Recipient = recipients |> toIdeaRecipients }
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

        let! ideaDataModels = env.db.query sql (dict ["userId" => userId] |> Some)
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

        let! ideaDataModels = env.db.query sql (dict ["recipientId" => recipientId] |> Some)
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

        let! ideaDataModel = env.db.querySingle sql (dict ["id" => id] |> Some)
        let idea = ideaDataModel |>  toIdea

        return! idea |> (getRecipientsForIdeaOption env)
    }