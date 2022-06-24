module Ideas.Controller

open System
open Utils
open Saturn
open Ideas

let private showIdeaList env ctx =
    task {
        let userId = getLoggedInUserId ctx
        let! maybeIdeas = Repository.getAllForUser env userId

        match maybeIdeas with
        | Ok ideas ->
            let view = Views.ideasList ctx ideas
            return! Controller.renderHtml ctx view
        | Error ex ->
            return! Controller.renderHtml ctx (InternalError.layout ex)
    }

let private detail env ctx id =
    task {
        let! maybeIdea = Repository.getById env id

        let view = 
            match maybeIdea with
            | Ok (Some idea) -> Views.ideaDetail ctx idea
            | Ok None -> NotFound.layout
            | Error ex -> InternalError.layout ex
        
        return! Controller.renderHtml ctx view
    }

let resource env =
    controller {
        index (showIdeaList env)
        show (detail env)
    }