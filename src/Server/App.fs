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

    let post (route: string) (handler: SimpleHandler) (app: express.Express) =
        app.post(U2.Case1 route, (fun req res _ ->
            handler req res
            |> box
        ))
        |> ignore
        app

    type UseBuilder =
        | UseBuilder of express.Express with

            static member (^.) (UseBuilder app, (handler) : express.RequestHandler) =
                app.``use``(handler) |> ignore
                app

            static member (^.) (UseBuilder app, (path, handler) : string * express.RequestHandler)=
                app.``use``(path, handler) |> ignore
                app

    let inline ``use`` args (app: express.Express) = ((UseBuilder app) ^. args)


module Response =

    let send body (res: express.Response) =
        res.send(body) |> ignore

let app = express.Invoke()

// Configure express application
let staticOptions = createEmpty<express.``serve-static``.Options>
staticOptions.index <- Some !^"index.html"


let resolve path = Node.Exports.Path.join(Node.Globals.__dirname, path)
let combine path1 path2 = Node.Exports.Path.join(path1, path2)

let output = resolve ".."
let publicPath = combine output "../public"
let clientPath = combine output "client"

app
// Register the static directories
|> Express.``use`` (express.``static``.Invoke(publicPath, staticOptions))
|> Express.``use`` (express.``static``.Invoke(clientPath, staticOptions))
// Register logger
|> Express.``use`` (morgan.Exports.Morgan.Invoke(morgan.Dev))
|> Express.``use`` (bodyParser.Globals.json())
|> ignore

// Routing

app
|> Express.get
        "/status"
        (fun req res ->
            Response.send "Server is running !" res
        )
|> Express.post
        "/test"
        (fun req res ->
            req.body?test |> printfn "%A"
            res.``end``()
        )
// |> Express.get
//         ""
|> ignore

// Start the server
let port =
    match unbox Node.Globals.``process``.env?PORT with
    | Some x -> x
    | None -> 8080

// Live reload when in dev mode
// #if DEBUG
// let reload = importDefault<obj> "reload"
// let reloadServer = reload$(app)

// let watch = importDefault<obj> "watch"

// let watchOptions = createEmpty<Watch.Options>
// watchOptions.interval <- Some 1.

// Watch.Exports.watchTree(output, watchOptions, fun f cur prev ->
//     reloadServer?reload$() |> ignore
// )
// #endif

[<Pojo>]
type Users =
    { Firstname: string
      Surname: string
      Email: string
      Password: string }

[<Pojo>]
type Database =
    { Users: Users list }

let dbFile = resolve("db.json")
let adapter = Lowdb.FileAsyncAdapter(dbFile)
// let db =
Lowdb.Lowdb(adapter)
    ?``then``(fun (db: Lowdb.Lowdb) ->
        db.defaults(
            { Users =
                [ { Firstname = "Maxime"
                    Surname = "Mangel"
                    Email = "mangel.maxime@fableconf.com"
                    Password = "maxime"
                }]
            }
        ).write()
    )?``then``(fun _ ->
        app.listen(port, !!(fun _ ->
            printfn "Server started: http://localhost:%i" port
        ))
        |> ignore
    )
|> ignore
