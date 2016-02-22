module TimeMatrix.Pages.New

open System.IO
open Suave
open Suave.Filters
open Suave.Operators

let newPage = DotLiquid.page "new.html" None

let addNewTask = request (fun request ->
    printfn "new task %A" request
    Successful.OK "OK"
    >=>
    Redirection.FOUND "/" )

let webPart =
  path "/new" >=>
    choose [
      GET >=> newPage
      POST >=> addNewTask
    ]
