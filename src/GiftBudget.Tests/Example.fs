module Example

open Expecto

[<Tests>]
let tests = testList "Example tests" [
    testCase "one is one" <| fun _ ->
        Expect.equal 1 1 "One is one"

    test "Another test example" {
        Expect.equal 1 1 "This is done in a computation expression"
    }
]