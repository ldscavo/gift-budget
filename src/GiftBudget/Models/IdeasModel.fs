namespace Ideas

open System
open Recipients
open System.Threading.Tasks
open FsToolkit.ErrorHandling

type IIdea =
    abstract Text: string

[<CLIMutable>]
type Idea =
    { Id: Guid
      UserId: Guid
      Text: string
      Price: decimal option
      Link: string option
      Recipients: Recipient list
      CreatedOn: DateTime
      UpdatedOn: DateTime }

    interface IIdea with
        member this.Text = this.Text

module Validation =
    let private textMustNotBeEmpty (idea: IIdea) =
        if String.IsNullOrWhiteSpace idea.Text then
            Some ("text", "Idea must have some text")
        else
            None

    let validate idea =
        let validators =
            [ textMustNotBeEmpty ]

        validators
        |> List.fold
            (fun acc validation ->
                match validation idea with
                | Some (key, msg) -> acc |> Map.add key msg
                | None -> acc)
            Map.empty

[<CLIMutable>]
type IdeaInput =
    { text: string
      recipient: Guid option
      price: decimal option
      link: string option }

    interface IIdea with
        member this.Text = this.text

    member this.toIdea userId (recipientFunc: Guid -> Task<Result<Recipient option, exn>>) =
        taskResult {
            let! recipient = 
                this.recipient
                |> Option.map recipientFunc
                |> Option.defaultValue (Ok None |> Task.FromResult)

            return
               { Id = Guid.NewGuid ()
                 UserId = userId
                 Text = this.text
                 Price = this.price
                 Link = this.link
                 Recipients =
                     match recipient with
                     | Some r -> [r]
                     | None -> []
                 CreatedOn = DateTime.Now
                 UpdatedOn = DateTime.Now } 
        }
        
[<CLIMutable>]
type RecipientQuery = { ForId: Guid option; ForName: string option }