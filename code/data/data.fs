module TimeMatrix.Data

open System

type ToDo = {
  Title: string
  Important: bool
  Urgent: bool
  Completed: bool
}

//TODO change to MailboxProcessor
let mutable todos : ToDo list = [
    { Title = "Cras justo odio"; Important = true; Urgent = true; Completed = false }
    { Title = "Cras justo odio"; Important = true; Urgent = true; Completed = true }

    { Title = "Cras odiojusto "; Important = true; Urgent = false; Completed = false }
    { Title = "Crasodio justo "; Important = true; Urgent = false; Completed = false }
    { Title = "Cras ojustodio"; Important = true; Urgent = false; Completed = false }

    { Title = "Urgent"; Important = false; Urgent = true; Completed = false }

    { Title = "Maniana"; Important = false; Urgent = false; Completed = false }
    { Title = "Maniana"; Important = false; Urgent = false; Completed = true }
    { Title = "Maniana"; Important = false; Urgent = false; Completed = false }
    { Title = "Maniana"; Important = false; Urgent = false; Completed = true }
    { Title = "Maniana"; Important = false; Urgent = false; Completed = false }
  ]

let getToDoList() =
  todos

let createToDo title important urgent ready = {
  Title = title
  Important = important
  Urgent = urgent
  Completed = ready
}

let addToDo todo =
  todos <- todo :: todos
