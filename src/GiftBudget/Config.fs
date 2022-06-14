module Config

open Database

type Config = {
    connectionString : string
}

type AppEnv(connectionString) =
    interface IDb with member __.db = Database(connectionString)