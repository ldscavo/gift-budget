module Recipients.Views

open Giraffe.ViewEngine

let recipientsList ctx (recipients: seq<Recipient>) =
    App.layout [
        div [] [str "Recipients list goes here!"]
    ]