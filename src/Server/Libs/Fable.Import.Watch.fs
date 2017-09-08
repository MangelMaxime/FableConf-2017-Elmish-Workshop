namespace Fable.Import

open System
open System.Text.RegularExpressions
open Fable.Core
open Fable.Import.JS
open Fable.Import.Node

module Watch =

    type FileOrFiles = U2<Fs.Stats, obj>

    and [<StringEnum>] EventType =
        | Created
        | Removed

    and [<AllowNullLiteral>] Monitor =
        inherit Events.EventEmitter
        abstract files: obj with get, set
        abstract on: ``event``: EventType * listener: Func<FileOrFiles, Fs.Stats, unit> -> obj
        [<Emit("$0.on('created', $1...")>] abstract on_created: ``event``: Func<FileOrFiles, Fs.Stats, unit> -> obj
        [<Emit("$0.on('removed', $1...")>] abstract on_removed: ``event``: Func<FileOrFiles, Fs.Stats, unit> -> obj
        [<Emit("$0.on('changed',$1...)")>] abstract on_changed: listener: Func<FileOrFiles, Fs.Stats, Fs.Stats, unit> -> obj
        abstract on: ``event``: U2<string, Symbol> * listener: Func<obj, unit> -> obj
        abstract stop: unit -> unit

    and [<AllowNullLiteral>] Options =
        abstract ignoreDotFiles: bool option with get, set
        abstract interval: float option with get, set
        abstract ignoreUnreadableDir: bool option with get, set
        abstract ignoreNotPermitted: bool option with get, set
        abstract ignoreDirectoryPattern: bool option with get, set
        abstract filter: (string -> Fs.Stats -> bool) option with get, set

    [<Import("*", "watch")>]
    type Exports =
        class end
        static member watchTree(root : string, cb : FileOrFiles -> Fs.Stats -> Fs.Stats -> unit) : unit = jsNative
        static member watchTree(root : string, options : Options, cb : FileOrFiles -> Fs.Stats -> Fs.Stats -> unit) : unit = jsNative


// export function watchTree(root: string, callback: (f: FileOrFiles, curr: fs.Stats, prev: fs.Stats) => void): void;
// export function watchTree(root: string, options: Options, callback: (f: FileOrFiles, curr: fs.Stats, prev: fs.Stats) => void): void;
// export function unwatchTree(root: string): void;
// export function createMonitor(root: string, callback: (monitor: Monitor) => void): void;
// export function createMonitor(root: string, options: Options, callback: (monitor: Monitor) => void): void;
