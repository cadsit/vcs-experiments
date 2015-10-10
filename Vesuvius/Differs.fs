(*
 * File:    Differs.fs
 * Author:  Connor Adsit (connor.adsit@gmail.com)
 * Date:    2015-10-09
 *)

namespace Diffing

(*
* Union Type for an enum:
*      Addition    -- represents a line added to the base file
*      Subtraction -- represents a line removed from the base file
*)
type Operation = Addition | Subtraction


(*
 * Contains all sorts of diffing utilities to be used by Vesuvius
 *)
module Differs =
    open System.IO
    
    (*
     * A change in a file from one version to the next
     *)
    [<Sealed>]
    type Delta(operation : Operation, lineNumber : int, text : string) =
        // The type of change
        member self.operation = operation

        // The line the change occurred on
        member self.lineNumber = lineNumber

        // The new text
        member self.text = text


    (*
     * Requirements for diffing two files
     *)
    type IDiffer =
        interface
            abstract member diffFiles : string -> string -> Delta[]
        end


    (*
     * A differ that diffs on a line by line basis
     *)
     [<AbstractClass>]
    type ILineDiffer() =
        abstract member diffLines : FileStream -> FileStream -> int -> int -> Delta[]

        interface IDiffer with
            (*
             * Diffs files on a line by line basis
             * Parameters:
             *      f1 -- string location to the base file
             *      f2 -- string location to the new file
             *
             * Returns a list of all changes needed to turn the base file into the new file
             *)
            member self.diffFiles (f1 : string) (f2 : string) =
                let file1 = new FileStream(f1, FileMode.Open)
                let file2 = new FileStream(f2, FileMode.Open)
                self.diffLines file1 file2 0 0


    (*
     * Differ that [will] describe the diff as all lines removed from the original file and
     * all lines from the new file added in
     *)
    type BadLineDiffer() =
        inherit ILineDiffer()
            (*
             * Replaces all lines in the first file with all lines in the second
             * Parameters:
             *      f1 -- FileStream for the base file
             *      f2 -- FileStream for the new file
             *      line1 -- current line number of the base file
             *      line2 -- current line number of the new file
             *
             * Returns a list of all changes needed to turn the base file into the new file
             *)
            override self.diffLines (f1 : FileStream) (f2 : FileStream) (line1 : int) (line2 : int) = [||]

    // Differ used by the main program
    let MainDiffer : IDiffer = (BadLineDiffer() :> IDiffer)