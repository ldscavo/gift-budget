module Recipients.Controller

open System
open Config
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Saturn

let getLoggedInUserId (ctx: HttpContext) =
    ctx.User.FindFirst "userId"
    |> fun claim -> claim.Value
    |> Guid.Parse

let private showRecipientList (ctx: HttpContext) =
    task {
        let cnf : Config = Controller.getConfig ctx
        let userId = getLoggedInUserId ctx

        let! maybeRecipients = Repository.getAllForUser cnf.connectionString userId        
        
        match maybeRecipients with
        | Ok recipients ->
            let view = Views.recipientsList ctx recipients
            return! Controller.renderHtml ctx view
        | Error ex ->
            return! Controller.renderHtml ctx (InternalError.layout ex)           
    }

let resource =
    controller {
        index showRecipientList
    }