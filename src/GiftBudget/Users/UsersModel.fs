namespace Users

type ILogin =
    abstract email: string
    abstract password: string

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
        member x.email = x.Email
        member x.password = x.Password

[<CLIMutable>]
type Login =
    { email: string
      password: string
      redirectUrl: string }

    interface ILogin with
        member x.email = x.email
        member x.password = x.password

module Validation =    
    open System
    open System.Text.RegularExpressions

    let emailRegex = Regex("\w+@\w+\.\w+")

    let validate (user: ILogin) =
        let validators = [
            fun (u: ILogin) ->
                if String.IsNullOrWhiteSpace u.password then Some ("password", "Password shouldn't be empty")
                else None

            fun (u: ILogin) ->
                if emailRegex.IsMatch u.email then None
                else Some ("email", "Email address is invalid")
        ]

        validators
        |> List.fold
            (fun acc validation ->
                match validation user with
                | Some (key, msg) -> acc |> Map.add key msg
                | None -> acc)
            Map.empty