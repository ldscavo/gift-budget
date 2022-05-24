namespace Ideas

open System
open Recipients

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

module Validation =
    let private textMustNotBeEmpty idea =
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