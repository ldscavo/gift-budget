module Users.Service

open BCrypt.Net

type LoginResult =
    | Success of User
    | BadEmail of Login
    | BadPassword of Login
    | LoginError of exn

let verifyLogin maybeUser login =
    match maybeUser with
    | Ok (Some user) ->
        if user.Email = login.email then
            let validPassword = BCrypt.Verify(login.password, user.Password)
            if validPassword then Success user else BadPassword login
        else BadEmail login
    | Ok None -> BadEmail login
    | Error exn -> LoginError exn