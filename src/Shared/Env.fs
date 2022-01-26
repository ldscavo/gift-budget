module Shared.Env

open dotenv.net

DotEnv.Load(DotEnvOptions(probeForEnv = true, probeLevelsToSearch = 8))

type Env() =
    static member get key =
        let envVars = DotEnv.Read()
        match envVars.ContainsKey key with
        | true -> Some envVars.[key]
        | false -> None
    
    static member unsafeGet key =
        match (Env.get key) with
        | Some value -> value
        | None -> failwith (sprintf "Failed to resolve app parameter: %s" key)    

let inline (??-) a b = Option.defaultValue b a