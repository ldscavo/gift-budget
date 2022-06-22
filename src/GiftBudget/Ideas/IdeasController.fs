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

let resource env =
    controller {
        index (showIdeaList env)
    }