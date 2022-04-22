module RecipientsModel

open Expecto
open System
open Recipients

let recipient =
    { Id = Guid.NewGuid ()
      Name = "John Doe"
      Notes = Some "He really really like pineapples for some reason"
      CreatedOn = DateTime.Now
      UpdatedOn = DateTime.Now }

[<Tests>]
let test = testList "Recipient model validation tests" [

    testList "Recipient model validation tests" [

        testCase "A complete recipient should be valid" <| fun _ ->
            let result = Validation.validate recipient
            Expect.isEmpty result "Validation should pass with no issues raised"

        testCase "A recipient with a null name is invalid" <| fun _ ->            
            let result = Validation.validate { recipient with Name = null }
            Expect.isTrue (result.ContainsKey "name") "Recipient should have name"

        testCase "A recipient with an empty name is invalid" <| fun _ ->            
            let result = Validation.validate { recipient with Name = "" }
            Expect.isTrue (result.ContainsKey "name") "Recipient cannot have a blank name"

        testCase "A recipient with a name containing only whitespace is invalid" <| fun _ ->            
            let result = Validation.validate { recipient with Name = "     " }
            Expect.isTrue (result.ContainsKey "name") "Recipient's name cannot be only whitespace"
    ]
]