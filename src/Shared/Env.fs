module Shared.Env

open dotenv.net

DotEnv.Load(DotEnvOptions(probeForEnv = true, probeLevelsToSearch = 8))

let env key =
    let envVars = DotEnv.Read()
    match envVars.ContainsKey key with
    | true -> Some envVars[key]
    | false -> None

let inline (??-) a b = Option.defaultValue b a