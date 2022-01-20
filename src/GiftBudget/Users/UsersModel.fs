namespace Users

[<CLIMutable>]
type User = {
    id: System.Guid
    email: string
    password: string
    is_admin: bool
    created_on: System.DateTime
    updated_on: System.DateTime
}

module Validation =    
    open System
    open System.Text.RegularExpressions

    let emailRegex = Regex("\w+@\w+\.\w+")

    let validate user =
        let validators = [
            fun u ->
                if String.IsNullOrWhiteSpace u.password then Some ("password", "Password shouldn't be empty")
                else None

            fun u ->
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
