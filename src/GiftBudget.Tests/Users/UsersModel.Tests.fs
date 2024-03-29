﻿module UsersModel

open Expecto
open System
open Utils
open Users

let user = {
    email = "test@example.com"
    password = "hunter1"
    passwordConfirm = "hunter1"
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
        
        test "Password should match password confirmation" {
            let testUser = { user with password = "hunter1"; passwordConfirm = "hunter2" }
            let result = Validation.validate testUser

            Expect.isTrue (result.ContainsKey "passwordConfirm") "Validation issue is returned for mismatching passwords"
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