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

let publicPath = Node.Exports.Path.join(Node.Globals.__dirname, "../../public")
let clientPath = Node.Exports.Path.join(Node.Globals.__dirname, "../../output/client")

Browser.console.log clientPath
Browser.console.log Node.Globals.__dirname

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

app.listen(port, !!(fun _ ->
    printfn "Server started: http://localhost:%i" port
))
|> ignore
