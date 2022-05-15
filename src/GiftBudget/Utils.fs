module Utils

open System
open Microsoft.AspNetCore.Http

let getLoggedInUserId (ctx: HttpContext) =
    ctx.User.FindFirst "userId"
    |> fun claim -> claim.Value
    |> Guid.Parse