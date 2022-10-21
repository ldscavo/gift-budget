module Ideas.Controller

open Utils
open Saturn
open Ideas
open FsToolkit.ErrorHandling
open Microsoft.AspNetCore.Http
open System

let private showIdeaList env (ctx: HttpContext) =
    task {
        let! maybeIdeas = Repository.getAllForUser env ctx.UserId

        let view = 
            match maybeIdeas with
            | Ok ideas -> Views.ideasList ctx ideas
            | Error ex -> InternalError.layout ex

        return! Controller.renderHtml ctx view
    }

let private showIdeaDetail env ctx id =
    task {
        let! maybeIdea = Repository.getById env id

        let view = 
            match maybeIdea with
            | Ok (Some idea) -> Views.ideaDetail ctx idea
            | Ok None -> NotFound.layout
            | Error ex -> InternalError.layout ex
        
        return! Controller.renderHtml ctx view
    }

let private addIdea env ctx =
    task {
        let query = Controller.getQuery<RecipientQuery> ctx

        let! recipients =
            Recipients.Repository.getAllForUser env ctx.UserId
            |> TaskResult.defaultValue []

        let view = Views.addEditIdea ctx None None query recipients Map.empty
        return! Controller.renderHtml ctx view
    }

let private createIdea env ctx =
    task {
        let! input = Controller.getModel<IdeaInput> ctx
        let validationResult = Validation.validate input
        
        if validationResult.IsEmpty then
            let! result =
                input.toIdea ctx.UserId (Recipients.Repository.getById env)
                |> TaskResult.bind (Repository.insert env)

            match result with
            | Ok _ ->
                return! Controller.redirect ctx "/ideas"
            | Error ex ->
                let view = InternalError.layout ex
                return! Controller.renderHtml ctx view
        else
            let! recipients =
                Recipients.Repository.getAllForUser env ctx.UserId
                |> TaskResult.defaultValue []

            let view = Views.addEditIdea ctx None (Some input) {ForId = None; ForName = None} recipients validationResult
            return! Controller.renderHtml ctx view                
    }

let resource env =
    controller {
        index (showIdeaList env)
        show (showIdeaDetail env)
        add (addIdea env)
        create (createIdea env)
    }