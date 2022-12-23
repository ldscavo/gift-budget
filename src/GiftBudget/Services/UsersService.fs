module Users.Service

open System
open BCrypt.Net

type LoginResult =
    | Success of User
    | BadEmail of Login
    | BadPassword of Login
    | LoginError of exn

let verifyLogin maybeUser (login: Login) =
    match maybeUser with
    | Ok (Some user) ->
        if user.Email = login.email then
            let validPassword = BCrypt.Verify(login.password, user.Password)
            if validPassword then Success user else BadPassword login
        else BadEmail login
    | Ok None -> BadEmail login
    | Error exn -> LoginError exn

let createUserFromRegistration (r: Register) =
    { Id = Guid.NewGuid ()
      Email = r.email
      Password = r.password |> BCrypt.HashPassword
      Type = User
      CreatedOn = DateTime.Now
      UpdatedOn = DateTime.Now }