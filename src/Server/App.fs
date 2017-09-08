module Server

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Express

module Express =

    type SimpleHandler = express.Request -> express.Response -> unit

    let get (route: string) (handler: SimpleHandler) (app: express.Express) =
        app.get(U2.Case1 route, (fun req res _ ->
            handler req res
            |> box
        ))
        |> ignore
        app

module Response =

    let send body (res: express.Response) =
        res.send(body) |> ignore

let app = express.Invoke()

app
|> Express.get
        "/status"
        (fun req res ->
            Response.send "Server is running !" res
        )
// |> Express.get
//         ""
|> ignore


let staticOptions = createEmpty<express.``serve-static``.Options>
staticOptions.index <- Some !^"index.html"


let resolve path = Node.Exports.Path.join(Node.Globals.__dirname, path)
let combine path1 path2 = Node.Exports.Path.join(path1, path2)

let output = resolve ".."
let publicPath = combine output "../public"
let clientPath = combine output "client"

// Register path to the public files
app.``use``(express.``static``.Invoke(publicPath, staticOptions))
|> ignore
// Register path to the client files
app.``use``(express.``static``.Invoke(clientPath, staticOptions))
|> ignore

// Get PORT environment variable or use default
let port =
    match unbox Node.Globals.``process``.env?PORT with
    | Some x -> x
    | None -> 8080

#if DEBUG
let reload = importDefault<obj> "reload"
let reloadServer = reload$(app)

let watch = importDefault<obj> "watch"

let watchOptions = createEmpty<Watch.Options>
watchOptions.interval <- Some 1.

Watch.Exports.watchTree(output, watchOptions, fun f cur prev ->
    reloadServer?reload$() |> ignore
)
#endif

app.listen(port, !!(fun _ ->
    printfn "Server started: http://localhost:%i" port
))
|> ignore
