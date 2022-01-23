module Database

open Expecto
open Utils
open Shared.Database

[<Tests>]
let tests = testList "Shared Database tests" [
    testList "Connection string parsing tests" [
        test "A correct uri-style postgres connection string is convered to Windows-style" {
            let uri = "postgres://username:password@localhost:1234/database"
            let conn = connString uri

            Expect.isSome conn "The uri parsing was successful"
            
            let expectedValue = "Host=localhost;Port=1234;Username=username;Password=password;Database=database"
            let value = conn |> Option.defaultValue ""
            Expect.equal expectedValue value "The resulting connection string has all the correct parts"
        }

        [
            "postgres://password@localhost:1234/database"
            "postgres://localhost:1234/database"
            "postgres://username:password@:1234/database"
            "postgres://username:password@localhost/database"
            "postgres://username:password@localhost:1234"
            "username:password@localhost:1234/database"
        ]
        |> testParams "A uri without all the values results in a failure" (fun uri ->
            let conn = connString uri

            Expect.isNone conn "The uri parsing was not successful"
        )
    ]
]