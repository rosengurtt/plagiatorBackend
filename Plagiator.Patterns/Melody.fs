namespace Plagiator.Patterns

open Plagiator.Models
open System.Collections.Generic
open System
open System.Linq
open Plagiator.Models.Entities

module public Melody =

    let IntersectionComparedToDuration (n1: Note, n2: Note): double =
        let intersection = Math.Min(n1.EndSinceBeginningOfSongInTicks, n2.EndSinceBeginningOfSongInTicks) - Math.Max(n1.StartSinceBeginningOfSongInTicks, n2.StartSinceBeginningOfSongInTicks)
        let  averageDuration: float = float (n1.DurationInTicks + n2.DurationInTicks) / (float 2)
        (float) intersection / averageDuration


    // We consider a melody a monophonic succession of notes. If we have a set of notes
    // that is possibly poliphonic, we consider that the melody is given by the higher notes
    // so we remove any bass notes played at the same time as hight notes
    let ExtractMelodyFromNotes (notes: List<Note>): List<Note> =
        let sortedNotes = notes |> Seq.sortBy(fun x -> x.StartSinceBeginningOfSongInTicks, x.Pitch) |>  Seq.toList
        let mutable i = 0
        let fsharpIsShit = new  List<Note> ()
        while  i < sortedNotes.Count() - 1 do
            let mutable j = 0
            while i+j < sortedNotes.Count() && 
                sortedNotes.[i+j].StartSinceBeginningOfSongInTicks < sortedNotes.[i].EndSinceBeginningOfSongInTicks &&
                IntersectionComparedToDuration(sortedNotes.[i + j], sortedNotes.[j]) > 0.7 do
                j <- j + 1
            if i + j < sortedNotes.Count() then
                fsharpIsShit.Add( sortedNotes.[i + j - 1])
            else
                ()
            if j = 0 then
                i <- i + 1
            else
                i <- i + j 
        fsharpIsShit


    type melody = class
        val Notes: List<Note>

        new (notes:List<Note>) as this =
            { Notes = notes }


    end