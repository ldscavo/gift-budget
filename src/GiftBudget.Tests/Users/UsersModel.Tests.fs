﻿module UsersModel

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
        let invalidEmailTest = (fun email -> test (sprintf "email is invalid with value: %s" email) {
            let testUser = { user with email = email }
            let result = Validation.validate testUser

            Expect.isTrue (result.ContainsKey "email") "Email address is invalid"
        }) 

        test "A valid email passes validation" {
            let testUser = { user with email = "test@example.com" }
            let result = Validation.validate testUser

            Expect.isEmpty result "Validation should pass with no issues raised"
        }

        invalidEmailTest "testexample.com"
        invalidEmailTest "testexamplecom"
        invalidEmailTest "@example.com"
        invalidEmailTest "test@example"
        invalidEmailTest "test.com"
    ]
]