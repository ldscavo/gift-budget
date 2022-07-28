module Components

open Giraffe.ViewEngine
open Giraffe.ViewEngine.Htmx
open Saturn

let icon id =
    span [_class "icon"] [
        i [_class $"fas %s{id}"] []
    ]

let modalLink href content =
    a
        [ _class "modal-link"
          _href href
          _hxGet href
          _hxTarget "#modal-content"
          _hxTrigger "click" ]
        content