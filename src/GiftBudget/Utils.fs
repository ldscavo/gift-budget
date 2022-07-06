module Utils

open System
open Microsoft.AspNetCore.Http

type HttpContext with
    member this.UserId =
        this.User.FindFirst "userId"
        |> fun claim -> claim.Value
        |> Guid.Parse