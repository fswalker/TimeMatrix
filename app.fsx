#r "packages/Suave/lib/net40/Suave.dll"
#r "packages/DotLiquid/lib/NET40/DotLiquid.dll"
#r "packages/Suave.DotLiquid/lib/net40/Suave.DotLiquid.dll"

open System.IO
open Suave                 // always open suave
open Suave.Successful      // for OK-result
open Suave.Web             // for config
open Suave.Filters
open Suave.Operators

DotLiquid.setTemplatesDir (__SOURCE_DIRECTORY__ + "/web/views")

/// Browse static files in the 'web' subfolder
let browseStaticFiles ctx = async {
  let root = Path.Combine(ctx.runtime.homeDirectory, "web")
  return! Files.browse root ctx }


let homePage = DotLiquid.page "index.html" None

let webPart = path "/" >=> homePage //request (fun r -> homePage)

let home = OK "Hello F#!"

let app =
  choose [
    webPart
    browseStaticFiles
  ]
