module RecipientsRepository

open Expecto
open System
open Recipients
open Recipients.Repository

[<Tests>]
let tests = testList "Recipient database model mapping tests" [
    testList "Recipient domain model to database model tests" [
        let recipient =
            { Id = Guid.NewGuid ()
              UserId = Guid.NewGuid ()
              Name = "John Doe"
              Notes = Some "SUPER into exotic cheeses"
              CreatedOn = DateTime.Today
              UpdatedOn = DateTime.Now }

        testCase "Id is mapped correctly" <| fun _ ->
            let dataRecipient = recipient |> fromRecipient

            Expect.equal recipient.Id dataRecipient.id "Ids are equal"

        testCase "Name is mapped correctly" <| fun _ ->
            let dataRecipient = recipient |> fromRecipient

            Expect.equal recipient.Name dataRecipient.name "Names are equal"

        testCase "Notes is mapped to the string vlaue if there is Some" <| fun _ ->
            let dataRecipient = recipient |> fromRecipient

            Expect.isSome recipient.Notes "Notes has a value"
            Expect.equal recipient.Notes.Value dataRecipient.notes "Note values are equal"

        testCase "Notes are mapped to null if there is None" <| fun _ ->
            let newRecipient = { recipient with Notes = None }
            let dataRecipient = newRecipient |> fromRecipient

            Expect.isNone newRecipient.Notes "Notes does not have a value"
            Expect.isNull dataRecipient.notes "Data model has null for note field"

        testCase "CreatedOn is mapped correctly" <| fun _ ->
            let dataRecipient = recipient |> fromRecipient

            Expect.equal recipient.CreatedOn dataRecipient.created_on "Created on dates are equal"

        testCase "UpdateOn is mapped correctly" <| fun _ ->
            let dataRecipient = recipient |> fromRecipient

            Expect.equal recipient.UpdatedOn dataRecipient.updated_on "Updated on dates are equal"
    ]

    testList "Recipient database model to domain model tests" [
        let dataRecipient =
            { id = Guid.NewGuid ()
              user_id = Guid.NewGuid ()
              name = "John Doe"
              notes = "Collects ceramic figuringes of weasels"
              created_on = DateTime.Today
              updated_on = DateTime.Now }

        testCase "Id is mapped correctly" <| fun _ ->
            let recipient = dataRecipient |> toRecipient

            Expect.equal dataRecipient.id recipient.Id "Ids are equal"

        testCase "Name is mapped correctly" <| fun _ ->
            let recipient = dataRecipient |> toRecipient
            
            Expect.equal dataRecipient.name recipient.Name "Names are equal"

        testCase "Notes is mapped to Some if there is a value" <| fun _ ->
            let recipient = dataRecipient |> toRecipient
            
            Expect.isSome recipient.Notes "Notes is mapped to a Some"
            Expect.equal dataRecipient.notes recipient.Notes.Value "Note values are equal"

        testCase "Notes is mapped to None is the string is null" <| fun _ ->
            let recipient = { dataRecipient with notes = null } |> toRecipient

            Expect.isNone recipient.Notes "Null value is mapped to None"

        testCase "Notes is mapped to None is the string is empty" <| fun _ ->
            let recipient = { dataRecipient with notes = "" } |> toRecipient

            Expect.isNone recipient.Notes "Null value is mapped to None"

        testCase "Notes is mapped to None is the string is only whitespace" <| fun _ ->
            let recipient = { dataRecipient with notes = " " } |> toRecipient

            Expect.isNone recipient.Notes "Null value is mapped to None"

        testCase "CreatedOn is mapped correctly" <| fun _ ->
            let recipient = dataRecipient |> toRecipient

            Expect.equal dataRecipient.created_on recipient.CreatedOn "Created on dates are equal"

        testCase "UpdateOn is mapped correctly" <| fun _ ->
            let recipient = dataRecipient |> toRecipient

            Expect.equal dataRecipient.updated_on recipient.UpdatedOn "Updated on dates are equal"
    ]
]