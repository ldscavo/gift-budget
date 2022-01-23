module Utils

open Expecto

let testParams (name: string) (testCond: 'a -> unit) (input: 'a list) =
    testList name 
        (input |> List.map (fun i -> testCase (sprintf "%s : %A" name i) (fun () -> testCond i)))