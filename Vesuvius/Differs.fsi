(* 
 *  File:   Differs.fsi
 *  Author: Connor Adsit (connor.adsit@gmail.com)
 *  Date:   2015-10-09
 *)

namespace Diffing

(*
* Union Type for an enum:
*      Addition    -- represents a line added to the base file
*      Subtraction -- represents a line removed from the base file
*)
type Operation = Addition | Subtraction

module Differs =
    open System.IO


    (*
     * A change in a file from one version to the next
     *)
    [<Sealed>]
    type Delta =
        new : Operation * int * string -> Delta
        member operation : Operation
        member lineNumber : int
        member text : string

    (*
     * Requirements for diffing two files
     *)
    [<Interface>]
    type IDiffer =
        abstract member diffFiles : string -> string -> Delta[]

    (*
     * A differ that diffs on a line by line basis
     *)
    [<AbstractClass>]
    type ILineDiffer =
        interface IDiffer
        abstract member diffLines : FileStream -> FileStream -> int -> int -> Delta[]

    // Differ used by the main program
    val MainDiffer : IDiffer