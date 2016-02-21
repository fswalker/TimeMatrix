// Part of the build script is based on GitHub repo:
// https://github.com/tpetricek/suave-xplat-gettingstarted

// include Fake libs
#r "packages/FAKE/tools/FakeLib.dll"
#r "packages/FSharp.Compiler.Service/lib/net45/FSharp.Compiler.Service.dll"
#r "packages/Suave/lib/net40/Suave.dll"

open Fake
open System
open System.IO
open Suave
open Suave.Web
open Microsoft.FSharp.Compiler.Interactive.Shell

#load "app.fsx"
open App

// Directories
let buildDir  = "./build/"

// Filesets
let appReferences  =
    !! "/**/*.csproj"
      ++ "/**/*.fsproj"

// version info
let version = "0.1"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir]
)

Target "Build" (fun _ ->
    // compile all projects below src/app/
    MSBuildDebug buildDir "Build" appReferences
        |> Log "AppBuild-Output: "
)

Target "Run" (fun _ ->
  let prodConfig =
      { defaultConfig with
          homeFolder = Some __SOURCE_DIRECTORY__
          logger = Logging.Loggers.saneDefaultsFor Logging.LogLevel.Debug
          bindings = [ HttpBinding.mkSimple HTTP  "0.0.0.0" 80 ] }

  startWebServer prodConfig app
)
Target "Dev" (fun _ ->

  //Dev workflow
  let sbOut = new Text.StringBuilder()
  let sbErr = new Text.StringBuilder()

  let fsiSession =
    let inStream = new StringReader("")
    let outStream = new StringWriter(sbOut)
    let errStream = new StringWriter(sbErr)
    let fsiConfig = FsiEvaluationSession.GetDefaultConfiguration()
    let argv = Array.append [|"/fake/fsi.exe"; "--quiet"; "--noninteractive"; "-d:DO_NOT_START_SERVER"|] [||]
    FsiEvaluationSession.Create(fsiConfig, argv, inStream, outStream, errStream)

  let reportFsiError (e:exn) =
    traceError "Reloading app.fsx script failed."
    traceError (sprintf "Message: %s\nError: %s" e.Message (sbErr.ToString().Trim()))
    sbErr.Clear() |> ignore

  let reloadScript () =
    try
      traceImportant "Reloading app.fsx script..."
      let appFsx = __SOURCE_DIRECTORY__ @@ "app.fsx"
      fsiSession.EvalInteraction(sprintf "#load @\"%s\"" appFsx)
      fsiSession.EvalInteraction("open App")
      match fsiSession.EvalExpression("app") with
      | Some app -> Some(app.ReflectionValue :?> WebPart)
      | None -> failwith "Couldn't get 'app' value"
    with e -> reportFsiError e; None


  let currentApp = ref (fun _ -> async { return None })

  let serverConfig =
    { defaultConfig with
        homeFolder = Some __SOURCE_DIRECTORY__
        logger = Logging.Loggers.saneDefaultsFor Logging.LogLevel.Debug
        bindings = [ HttpBinding.mkSimple HTTP "127.0.0.1" 8083] }

  let reloadAppServer () =
    reloadScript() |> Option.iter (fun app ->
      currentApp.Value <- app
      traceImportant "New version of app.fsx loaded!" )

  let app ctx = currentApp.Value ctx
  let _, server = startWebServerAsync serverConfig app

  // Start Suave to host it on localhost
  reloadAppServer()
  Async.Start(server)
  // Open web browser with the loaded file
  //System.Diagnostics.Process.Start("http://localhost:8083") |> ignore

  // Watch for changes & reload when app.fsx changes
  use watcher = !! (__SOURCE_DIRECTORY__ @@ "*.*") |> WatchChanges (fun _ -> reloadAppServer())
  traceImportant "Waiting for app.fsx edits. Press any key to stop."
  System.Console.ReadLine() |> ignore
)

// Build order
"Clean"
  ==> "Build"
  ==> "Dev"

"Build"
  ==> "Run"

// start build
RunTargetOrDefault "Run"
