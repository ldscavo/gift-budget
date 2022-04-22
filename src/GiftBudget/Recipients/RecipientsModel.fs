namespace Recipients

open System

type IRecipient =
    abstract Name: string

[<CLIMutable>]
type Recipient =
    { Id: Guid
      Name: string
      Notes: string option
      CreatedOn: DateTime
      UpdatedOn: DateTime }

    interface IRecipient with
        member this.Name = this.Name

module Validation =
    let validate (recipient: IRecipient) =
        let validators = [
            fun (g: IRecipient) ->
                if String.IsNullOrWhiteSpace g.Name then
                    Some ("name", "Recipient must have a name")
                else
                    None
        ]

        validators
        |> List.fold
            (fun acc validation ->
                match validation recipient with
                | Some (key, msg) -> acc |> Map.add key msg
                | None -> acc)
            Map.empty