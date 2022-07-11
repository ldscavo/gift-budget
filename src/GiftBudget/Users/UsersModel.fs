namespace Users

type ILogin =
    abstract Email: string
    abstract Password: string

type UserType =
    | User
    | Admin

[<CLIMutable>]
type User =
    { Id: System.Guid
      Email: string
      Password: string
      Type: UserType
      CreatedOn: System.DateTime
      UpdatedOn: System.DateTime }

    interface ILogin with
        member x.Email = x.Email
        member x.Password = x.Password

[<CLIMutable>]
type Login =
    { email: string
      password: string
      redirectUrl: string }

    interface ILogin with
        member x.Email = x.email
        member x.Password = x.password

[<CLIMutable>]
type Register =
    { email: string
      password: string
      passwordConfirm: string }

    interface ILogin with
        member x.Email = x.email
        member x.Password = x.password

module Validation =    
    open System
    open System.Text.RegularExpressions

    let emailRegex = Regex("\w+@\w+\.\w+")

    let validate (registration: Register) =
        let validators = [
            fun r ->
                if String.IsNullOrWhiteSpace r.password then Some ("password", "Password shouldn't be empty")
                else None

            fun r ->
                if emailRegex.IsMatch r.email then None
                else Some ("email", "Email address is invalid")

            fun r ->
                if r.password = r.passwordConfirm then None
                else Some ("passwordConfirm", "Passwords do not match")
        ]

        validators
        |> List.fold
            (fun acc validation ->
                match validation registration with
                | Some (key, msg) -> acc |> Map.add key msg
                | None -> acc)
            Map.empty