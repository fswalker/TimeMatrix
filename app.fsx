#r "packages/Suave/lib/net40/Suave.dll"
#r "packages/DotLiquid/lib/NET40/DotLiquid.dll"
#r "packages/Suave.DotLiquid/lib/net40/Suave.DotLiquid.dll"

open System.IO
open Suave                 // always open suave
open Suave.Successful      // for OK-result
open Suave.Web             // for config
open Suave.Filters
open Suave.Operators

#load "code/data/data.fs"
#load "code/pages/home.fs"
#load "code/pages/new.fs"
open TimeMatrix.Pages

DotLiquid.setTemplatesDir (__SOURCE_DIRECTORY__ + "/web/views")

/// Browse static files in the 'web' subfolder
let browseStaticFiles ctx = async {
  let root = Path.Combine(ctx.runtime.homeDirectory, "web")
  return! Files.browse root ctx }

let app =
  choose [
    Home.webPart
    New.webPart
    browseStaticFiles
  ]
