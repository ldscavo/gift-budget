namespace Ideas

open System
open Recipients
open System.Threading.Tasks
open FsToolkit.ErrorHandling

type IIdea =
    abstract Text: string

type IdeaRecipient =
    | IdeaRecipient of Recipient
    | IdeaRecipients of Recipient list
    | NoRecipient

[<CLIMutable>]
type Idea =
    { Id: Guid
      UserId: Guid
      Text: string
      Price: decimal option
      Link: string option
      Recipient: IdeaRecipient
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

type private RecipientsFunc = Guid array -> Task<Result<Recipient list, exn>>

[<CLIMutable>]
type IdeaInput =
    { text: string
      price: decimal option
      link: string option }

    interface IIdea with
        member this.Text = this.text

    member this.toIdea userId =            
        { Id = Guid.NewGuid ()
          UserId = userId
          Text = this.text
          Price = this.price
          Link = this.link
          Recipient = NoRecipient
          CreatedOn = DateTime.Now
          UpdatedOn = DateTime.Now }
             