module Server

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Express
open Helpers
open Database

let app = express.Invoke()

// Configure express application
let staticOptions = createEmpty<express.``serve-static``.Options>
staticOptions.index <- Some !^"index.html"



let output = resolve ".."
let publicPath = combine output "../public"
let clientPath = combine output "client"
printfn "%A" publicPath
app
// Register the static directories
|> Express.Sugar.``use`` (express.``static``.Invoke(publicPath, staticOptions))
|> Express.Sugar.``use`` (express.``static``.Invoke(clientPath, staticOptions))
// Register logger
|> Express.Sugar.``use`` (morgan.Exports.Morgan.Invoke(morgan.Dev))
|> Express.Sugar.``use`` (bodyParser.Globals.json())
|> ignore

// Routing

app
|> Express.Sugar.get
        "/status"
        (fun req res ->
            Express.Sugar.Response.send "Server is running !" res
        )
|> Express.Sugar.post
        "/test"
        (fun req res ->
            req.body?test |> printfn "%A"
            res.``end``()
        )
|> Express.Sugar.get
        "/user/list"
        (fun req res ->
            let posts =
                Database.Lowdb
                    .get(!^"Users")
                    .find()
                    .value()

            res.setHeader("Content-Type", !^"application/json")
            Express.Sugar.Response.send (toJson posts) res
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

// let db =
Database.Lowdb
    .defaults(
        { Users =
            [| { Firstname = "Maxime"
                 Surname = "Mangel"
                 Email = "mangel.maxime@fableconf.com"
                 Password = "maxime" }
            |]
        }
    ).write()
|> ignore

app.listen(port, !!(fun _ ->
    printfn "Server started: http://localhost:%i" port
))
|> ignore
