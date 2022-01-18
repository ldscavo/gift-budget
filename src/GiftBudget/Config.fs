module Config

open System

type Config = {
    connectionString : string
}

let env key =
    match Environment.GetEnvironmentVariable key with
    | null -> None
    | value -> Some value

let inline (??-) a b = Option.defaultValue b a