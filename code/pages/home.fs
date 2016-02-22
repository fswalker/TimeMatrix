module TimeMatrix.Pages.Home

open Suave
open Suave.Filters
open Suave.Operators
open TimeMatrix.Data

type MatrixModel = {
  ImportantUrgent: seq<ToDo>
  ImportantNotUrgent: seq<ToDo>
  NotImportantUrgent: seq<ToDo>
  NotImportantNotUrgent: seq<ToDo>
}

let private isImportant todo =
    todo.Important

let private isUrgent todo =
    todo.Urgent

let private createMatrixModel () =
  let todos = getToDoList()
  { ImportantUrgent = (todos |> List.filter (fun t -> isImportant t && isUrgent t))
    ImportantNotUrgent = todos |> List.filter (fun t -> isImportant t && not << isUrgent <| t)
    NotImportantUrgent = todos |> List.filter (fun t -> not << isImportant <| t && isUrgent t)
    NotImportantNotUrgent = todos |> List.filter (fun t -> not << isImportant <| t && not << isUrgent <| t)
  }
  todos

let homePage = request (fun r ->
  let model = createMatrixModel ()
  printfn "new todos %A" model
  DotLiquid.page "home.html" model
)

let webPart = path "/" >=> homePage
