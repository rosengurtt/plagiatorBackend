namespace Plagiator.Patterns

open Plagiator.Models
open System.Collections.Generic
open System
open System.Linq

module public Patterns =

    
    let StringifyList (strings: List<string>): string =
        String.Join(",", strings)

    // If we find the pattern 1,2,3,4 we will also find the patterns 1,2,3 and 2,3,4
    // But if 1,2,3 and 2,3,4 never happer on their own, but always as part of 1,2,3,4
    // then we remove them
    let RemoveSubsequences (sequences: Dictionary<string, List<int>>): Dictionary<string, List<int>> =
        let auxObj = new Dictionary<string, List<int>>()
        let retObj = new Dictionary<string, List<int>>()
  
        // With this loop we remove 123 because it is part of 1234 and doesn't happen on its own
        let sortedKeysAsc = sequences.Keys |> Seq.sortBy(fun x -> x) |> Seq.toList

        for i in 0..sequences.Keys.Count - 1 do
            if not (sortedKeysAsc.[i + 1].Contains(sortedKeysAsc.[i])) ||
                sequences.[sortedKeysAsc.[i]].Count > sequences.[sortedKeysAsc.[i + 1]].Count then
                auxObj.Add(sortedKeysAsc.[i], sequences.[sortedKeysAsc.[i]])
            else
                ()

        // With this loop we remove 234 because it is part of 1234 and doesn't happen on its own
        let sortedKeysDesc = auxObj.Keys |> Seq.sortByDescending(fun x -> x) |> Seq.toList
        for i in 0..auxObj.Keys.Count - 1 do
            if not (sortedKeysDesc.[i + 1].Contains(sortedKeysDesc.[i])) ||
                auxObj.[sortedKeysDesc.[i]].Count > auxObj.[sortedKeysDesc.[i + 1]].Count then
                retObj.Add(sortedKeysDesc.[i], auxObj.[sortedKeysDesc.[i]])
            else
                ()
        retObj

    // When we look for the patterns, we find first all the existing sequences, regardless of
    // how many times they are found. RemoveSingletons is used to remove the ones that 
    // happen only once
    let RemoveSingletons (sequences: Dictionary<string, List<int>>): Dictionary<string, List<int>> =
        let keysToRemove = sequences |> Seq.filter (fun x -> x.Value.Count = 1) |> Seq.map(fun ele -> ele.Key)
        keysToRemove |> Seq.iter(fun k -> sequences.Remove(k) |> ignore<bool>)
        sequences


    // It finds patterns in a string that consists of comma separated elements
    // The elements can be numbers that represent pitches or durations, or they can be
    // strings that represent other things like notes (composed by a pitch and a duration
    let FindPatternsInListOfStrings (someString: string,  minLength,  maxLength): Dictionary<string, List<int>> =
        let strings = someString.Split(",").ToList()
        let  sequences = new Dictionary<string, List<int>>()
        for i in 0 .. strings.Count - minLength do
            for j in minLength .. maxLength do
                if i + j < strings.Count then
                    let seq = StringifyList(strings.GetRange(i,j))
                    if (sequences.Keys.Contains(seq)) then
                       ()
                    else
                        sequences.Add(seq, new List<int>())
                    sequences.[seq].Add(i)
                else
                    ()
        let sequences = RemoveSubsequences sequences
        RemoveSingletons sequences



