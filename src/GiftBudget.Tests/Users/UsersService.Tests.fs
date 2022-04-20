module UsersService

open System
open Expecto
open Users
open Users.Service
open BCrypt.Net

[<Tests>]
let tests = testList "UsersService tests" [
    
    testList "Login verification tests" [
        let email = "test@example.com"
        let password = "hunter2"

        let user =
            { Id = Guid.NewGuid()
              Email = email
              Password = BCrypt.HashPassword password
              Type = User
              CreatedOn = DateTime.Now
              UpdatedOn = DateTime.Now }

        let login =
            { email = email
              password = password
              redirectUrl = "" }

        testCase "A login with the correct email and password returns a Success result" <| fun _ ->      
            let result = Service.verifyLogin (user |> Some |> Ok) login
            
            Expect.equal result (Success user) "Correct email and password are successful"
        
        testCase "A login with an incorrect password returns a BadPassword result" <| fun _ ->
            let badLogin = { login with password = "password123" }
            let result = Service.verifyLogin (user |> Some |> Ok) badLogin

            Expect.equal result (BadPassword badLogin) "Password does not match"

        testCase "A login with an incorrect email returns a BadEmail result" <| fun _ ->
            let badLogin = { login with email = "example@test.com" }
            let result = Service.verifyLogin (user |> Some |> Ok) badLogin

            Expect.equal result (BadEmail badLogin) "Email does not match"

        testCase "A login with no user returns a BadEmail result" <| fun _ ->
            let result = Service.verifyLogin (None |> Ok) login

            Expect.equal result (BadEmail login) "User with Email is not there"

        testCase "If the user result is an error, return a LoginError result" <| fun _ ->
            let ex = exn "Database connection error"
            let result = Service.verifyLogin (ex |> Error) login
            
            Expect.equal result (LoginError ex) "The exception is carried into the LoginError"
    ]
]