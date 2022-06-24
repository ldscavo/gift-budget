module Database

open Dapper
open System.Data.Common
open System.Collections.Generic
open System.Threading.Tasks
open FSharp.Control.Tasks
open Npgsql

let inline (=>) k v = k, box v

let private execute (connection: #DbConnection) (sql: string) data =
    task {
        try
            let! res = connection.ExecuteAsync(sql, data)
            return Ok res
        with
        | ex -> return Error ex
    }

let private query (connection: #DbConnection) (sql: string) (parameters: 'a option) =
    task {
        try
            let! res =
                match parameters with
                | Some p -> connection.QueryAsync<'T>(sql, p)
                | None -> connection.QueryAsync<'T>(sql)
            return Ok (res |> Seq.toList)
        with
        | ex -> return Error ex
    }

let private querySingle (connection: #DbConnection) (sql: string) (parameters: 'a option) =
    task {
        try
            let! res =
                match parameters with
                | Some p -> connection.QuerySingleOrDefaultAsync<'T>(sql, p)
                | None -> connection.QuerySingleOrDefaultAsync<'T>(sql)
            return
                if isNull (box res) then Ok None
                else Ok (Some res)

        with
        | ex -> return Error ex
    }

type IDatabase =
    abstract execute : string -> 'a -> Task<Result<int,exn>>
    abstract query : string -> 'a option -> Task<Result<'b list, exn>>
    abstract querySingle : string -> 'a option -> Task<Result<'b option, exn>>

type IDb =
    abstract db: IDatabase

type Database(connectionString: string) =
    interface IDatabase with
        member __.execute sql data =
            task {
                use conn = new NpgsqlConnection(connectionString)
                return! execute conn sql data
            }            

        member __.query sql parameters =
            task {
                use conn = new NpgsqlConnection(connectionString)
                return! query conn sql parameters
            }            

        member __.querySingle sql parameters =
            task {
                use conn = new NpgsqlConnection(connectionString)
                return! querySingle conn sql parameters
            }            