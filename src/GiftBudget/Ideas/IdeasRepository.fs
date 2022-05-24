module Ideas.Repository

open System
open Database
open System.Threading.Tasks
open FSharp.Control.Tasks
open Npgsql

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