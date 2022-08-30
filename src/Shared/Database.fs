module Shared.Database

open System
open Npgsql

let connString conn =
    try
        let uri = Uri(conn)
        let userInfo = uri.UserInfo.Split(':')

        let database =
            match uri.LocalPath.TrimStart '/' with
            | "" -> failwith "No database specified"
            | db -> db

        let builder = NpgsqlConnectionStringBuilder (
            Host = uri.Host,
            Port = uri.Port,
            Username = userInfo.[0],
            Password = userInfo.[1],
            Database = database,
            IncludeErrorDetail = true
        )

        Some (builder.ToString())
    with
    | _ -> None