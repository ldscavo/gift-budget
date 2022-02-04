namespace Users

type ILogin =
    abstract email: string
    abstract password: string

[<CLIMutable>]
type User =
    { id: System.Guid
      email: string
      password: string
      is_admin: bool
      created_on: System.DateTime
      updated_on: System.DateTime }

    interface ILogin with
        member x.email = x.email
        member x.password = x.password

type Login =
    { email: string
      password: string }

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
                | Some (key, validationMessage) -> Map.add key validationMessage acc
                | None -> acc)
            Map.empty