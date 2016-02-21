module TimeMatrix.Pages.Home

open System.IO
open Suave
open Suave.Filters
open Suave.Operators

//TODO model for homepage
let homePage = DotLiquid.page "index.html" None

let webPart = path "/" >=> homePage
