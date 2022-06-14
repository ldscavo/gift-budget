module Recipients.Controller

open System
open Config
open Utils
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Saturn
open Recipients

let private showRecipientList env (ctx: HttpContext) =
    task {
        let userId = getLoggedInUserId ctx

        let! maybeRecipients = Repository.getAllForUser env userId        
        
        match maybeRecipients with
        | Ok recipients ->
            let view = Views.recipientsList ctx recipients
            return! Controller.renderHtml ctx view
        | Error ex ->
            return! Controller.renderHtml ctx (InternalError.layout ex)           
    }

let private detail env (ctx: HttpContext) (id: Guid) =
    task {
        let cnf = Controller.getConfig ctx
        let! maybeRecipient = Repository.getById env id

        match maybeRecipient with
        | Ok (Some recipient) -> return! Controller.renderHtml ctx (Views.recipientDetail ctx recipient)
        | Ok None -> return! Controller.renderHtml ctx NotFound.layout
        | Error ex -> return! Controller.renderHtml ctx (InternalError.layout ex)
    }     

let private addRecipient (ctx: HttpContext) =
    task {
        let view = Views.addEditRecipient ctx None None Map.empty
        return! Controller.renderHtml ctx view
    }

let private createRecipient env (ctx: HttpContext) =
    task {
        let config : Config = Controller.getConfig ctx

        let! input = Controller.getModel<RecipientInput> ctx
        let validationResult = Validation.validate input

        if validationResult.IsEmpty then
            let userId = getLoggedInUserId ctx
            let recipient = input.toRecipient(userId)            

            let! result = Repository.insert env recipient
            match result with
            | Ok _ ->
                return! Controller.redirect ctx $"/recipients/{recipient.Id.ToString()}"
            | Error _ ->
                return! 
                    Views.addEditRecipient ctx None (Some input) Map.empty
                    |> Controller.renderHtml ctx
        else
            return! 
                Views.addEditRecipient ctx None (Some input) validationResult
                |> Controller.renderHtml ctx
    }

let resource env =
    controller {
        index (showRecipientList env)
        show (detail env)
        add addRecipient
        create (createRecipient env)
    }