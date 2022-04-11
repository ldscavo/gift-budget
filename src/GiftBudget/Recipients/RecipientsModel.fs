namespace Recipients

open System

type IRecipient =
    abstract Name: string
    abstract Notes: string

[<CLIMutable>]
type Recipient =
    { Id: Guid
      Name: string
      Notes: string
      CreatedOn: DateTime
      UpdatedOn: DateTime }

    interface IRecipient with
        member this.Name = this.Name
        member this.Notes = this.Notes

module Validation =
    let validate (recipient: IRecipient) =
        let validators = [
            fun (g: IRecipient) ->
                if String.IsNullOrEmpty g.Name then
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