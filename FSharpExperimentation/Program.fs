﻿// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open LASI.Core
open LASI.Core.ComparativeHeuristics
open LASI.ContentSystem
open System.Linq
open System.Threading.Tasks

[<EntryPoint>]
let main argv =
    // load Lookup printing loading feedback messages
    let tasks = Lookup.GetLoadingTasks().AsParallel().ForAll(fun t-> printfn "%A" t.Result)

    // tag, parse, and construct a Document 
    let doc = Tagger.DocumentFromDocX(DocXFile(@"C:\Users\Aluan\Desktop\documents\sec22.docx"))
    // perform default binding on the Document
    Binding.Binder.Bind doc
    // perform default weighting on the Document
    Weighter.Weight doc
    
    // print the document while pattern matching on various Phrase Types and naively binding Pronouns at the phrasal level
    let rec processPhrases (phrs:list<Phrase>)= 
        match phrs with
        |head :: tail -> 
            match head with // process the first phrase in the list
            | :? NounPhrase as np->  
                match np.Paragraph.Phrases.OfPronounPhrase().FirstOrDefault() with 
                    | null -> () // no pronoun Phrase within the paragraph of NounPhrase np
                    | pro -> np.BindPronoun(pro) // bind naively (this is just an example)
                printfn "NP Matched %A" np
            | :? VerbPhrase as vp-> printfn "VP Matched %A" vp        
            | p -> printfn "Not Matched %A" p; 
            processPhrases tail // recursive tail call to continue processing
        | [] -> printfn "" // list has been exhausted
    
    processPhrases (Seq.toList doc.Phrases) //bind and output the document doings.

    // keep reading from the console until the string "exit" is entered.
    let rec input line = 
        match line with
        |"exit" -> ()
        |_ -> input(stdin.ReadLine())
    let line = stdin.ReadLine()
    input line 
    // the last value computed by the function is the exit code
    0 
 

 
