namespace Budgets

open System

type IBudget =
    abstract Name: string

type ItemStatus =
    | Included
    | Unincluded
    | Purchased

[<CLIMutable>]
type BudgetItem =
    { Id: Guid
      BudgetId: Guid
      RecipientId: Guid
      Name: string
      Price: decimal
      Status: ItemStatus
      CreatedOn: DateTime
      UpdatedOn: DateTime }
    
    interface IBudget with
        member this.Name = this.Name

[<CLIMutable>]
type Budget =
    { Id: Guid
      UserId: Guid
      Name: string
      Items: BudgetItem list
      CreatedOn: DateTime
      UpdatedOn: DateTime }

      interface IBudget with
        member this.Name = this.Name

module Validation =
    let private nameMustNotBeEmpty (budget: IBudget) =
        if String.IsNullOrWhiteSpace budget.Name then
            Some ("text", "Name cannot be empty")
        else
            None

    let validate budget =
        let validators =
            [ nameMustNotBeEmpty ]

        validators
        |> List.fold
            (fun acc validation ->
                match validation budget with
                | Some (key, msg) -> acc |> Map.add key msg
                | None -> acc)
            Map.empty

type BudgetItemInput =
    { name: string
      budget: Guid
      price: decimal }