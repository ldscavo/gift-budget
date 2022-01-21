module UsersModel

open Expecto
open System
open Users

let user = {
    id = Guid()
    email = "test@example.com"
    password = "hunter1"
    is_admin = false
    created_on = DateTime.Now
    updated_on = DateTime.Now
}

let testParams (name: string) (testCond: 'a -> unit) (input: 'a list) =
    testList name 
        (input |> List.map (fun i -> testCase (sprintf "%s : %A" name i) (fun () -> testCond i)))

[<Tests>]
let tests = testList "User model validation tests" [
    testList "Password validation tests" [        
        test "A password with alphanumeric contents should be valid" {
            let testUser = { user with password = "hunter1" }
            let result = Validation.validate testUser

            Expect.isEmpty result "Validation should pass with no issues raised"
        }
        test "Password should not be empty" {
            let testUser = { user with password = "" }
            let result = Validation.validate testUser

            Expect.isTrue (result.ContainsKey "password") "Validation issue is returned for empty password"
        }
        test "Password should not be only whitespace" {
            let testUser = { user with password = " " }
            let result = Validation.validate testUser

            Expect.isTrue (result.ContainsKey "password") "Validation issue is returned for empty password"
        }
        test "Password should not null" {
            let testUser = { user with password = null }
            let result = Validation.validate testUser

            Expect.isTrue (result.ContainsKey "password") "Validation issue is returned for null password"
        }
        
    ]

    testList "Email validation tests" [
        test "A valid email passes validation" {
            let testUser = { user with email = "test@example.com" }
            let result = Validation.validate testUser

            Expect.isEmpty result "Validation should pass with no issues raised"
        }

        [
            "testexample.com"
            "testexamplecom"
            "@example.com"
            "test@example"
            "test.com"
        ]
        |> testParams "Invalid emaails throw a validation result" (fun email ->
            let testUser = { user with email = email }
            let result = Validation.validate testUser

            Expect.isTrue (result.ContainsKey "email") "Email address is invalid"
        )
    ]
]