module Database

open Fable.Core
open Fable.Import
open Helpers

[<Pojo>]
type Users =
    { Firstname: string
      Surname: string
      Email: string
      Password: string }

[<Pojo>]
type DatabaseData =
    { Users: Users [] }

let dbFile = resolve("../../ressources/db.json")
let adapter = Lowdb.FileSyncAdapter(dbFile)

let mutable do_not_use_directly_db : Lowdb.Lowdb option = Option.None

type Database =
    static member Lowdb
        with get() : Lowdb.Lowdb =
            if do_not_use_directly_db.IsNone then
                do_not_use_directly_db <- Lowdb.Lowdb(adapter) |> Some

            do_not_use_directly_db.Value
