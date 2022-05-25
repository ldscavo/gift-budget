module IdeasModel

open Expecto
open System
open Ideas

let idea =
    { Id = Guid.NewGuid ()
      UserId = Guid.NewGuid ()
      Text = "A shiny new pineapple for someone"
      Price = Some 19.99m
      Link = None
      Recipient = NoRecipient
      CreatedOn = DateTime.Now
      UpdatedOn = DateTime.Now }

[<Tests>]
let tests = testList "Idea model tests" [

    testList "Idea model validation tests" [
        
        testCase "A complete idea should be valid" <| fun _ ->
            let result = Validation.validate idea
            Expect.isEmpty result "Validation should pass with no issues raised"

        testCase "An idea with null text is invalid" <| fun _ ->
            let result = Validation.validate { idea with Text = null }
            Expect.isTrue (result.ContainsKey "text") "Idea should have text"

        testCase "An idea with empty text is invalid" <| fun _ ->
            let result = Validation.validate { idea with Text = "" }
            Expect.isTrue (result.ContainsKey "text") "Idea should not be empty"

        testCase "An idea with text of only whitespace is invalid" <| fun _ ->
            let result = Validation.validate { idea with Text = "    " }
            Expect.isTrue (result.ContainsKey "text") "Idea should not be only whitespace"
    ]
]