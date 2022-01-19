module Example

open Expecto

[<Tests>]
let tests = testList "Server" [
    testCase "one is one" <| fun _ ->
        Expect.equal 1 1 "One is one"    
]