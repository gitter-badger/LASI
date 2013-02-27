﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LASI.Algorithm;
using LASI.FileSystem;
using LASI.Utilities;
using SharpNLPTaggingModule;
using System.IO;

namespace Scott_Experimentation
{
    class Program
    {
        static void Main(string[] args) {
            var tagger = new SharpNLPTagger(TaggingOption.TagAndAggregate, @"C:\Users\Scott\Desktop\TestSentences.txt");
            var tagged = tagger.ProcessFile();
            var paragraphs = new TaggedFileParser(tagged).LoadParagraphs();
            var document = new Document(paragraphs);

            string sep = "\n***************************************************************\n";

            Word w1 = document.WordAt(2003736);
            Console.WriteLine("Word at 5: {0}", w1.Text);
            var strng1 = document.WordTextAt(500);
            Console.WriteLine("Word Text at 5: {0}{1}", strng1, sep);

            Sentence sent1 = document.SentenceAt(1);
            Console.WriteLine("Sentence at 1: {0}", sent1.Text);
            string strng2 = document.SentenceTextAt(1000);
            Console.WriteLine("Sentence text at 1: {0}{1}", strng2, sep);

            
            for (var x = 0; x <= document.Words.Count(); x++)
            {
                Console.WriteLine("{0}: {1} => {2}",x, document.WordTextAt(x), document.WordAt(x).GetType());
            }
            Console.WriteLine(sep);

            for (var x = 0; x < document.Sentences.Count(); x++)
            {
                Console.WriteLine("{0}: {1}\n", x, document.SentenceTextAt(x));
            }
            Console.WriteLine(sep);

            Console.WriteLine("{0} => {1}", document.WordAt(11).Text, document.WordAt(11).GetType());
            if (document.WordAt(11)is Adverb)
                Console.WriteLine("Match");
            else
                Console.WriteLine("No Match");

            StdIO.WaitForAnyKey();
        }
    }
}