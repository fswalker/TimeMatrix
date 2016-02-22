module TimeMatrix.Pages.New

open System.IO
open Suave
open Suave.Filters
open Suave.Operators
open TimeMatrix.Data

let newPage = DotLiquid.page "new.html" None

let addNewTask ctx = async {
    printfn "new task %A" ctx.request.form
    let form = ctx.request.form
    let isNullOrEmpty = Option.fold (fun s x -> x) "" >> (=) ""
    let isOptionChosen = Option.fold (fun s x -> x = "on") false
    let title = form |> List.tryFind (fst >> (=) "title")
    match title with
    | Some t when (t |> snd |> (not << isNullOrEmpty)) ->
      let getTitle = snd >> Option.get
      let isImportant = form |> List.tryFind (fst >> (=) "important") |> Option.bind snd |> isOptionChosen
      let isUrgent = form |> List.tryFind (fst >> (=) "urgent") |> Option.bind snd |> isOptionChosen
      createToDo (getTitle t) isImportant isUrgent false |> addToDo |> ignore
      return! Redirection.FOUND "/" ctx
    | _ ->
      return! Redirection.FOUND "/new" ctx
  }

let webPart =
  path "/new" >=>
    choose [
      GET >=> newPage
      POST >=> addNewTask
    ]
