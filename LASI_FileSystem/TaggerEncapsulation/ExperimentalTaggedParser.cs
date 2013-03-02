﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LASI.Algorithm;
using LASI.FileSystem.FileTypes;
namespace LASI.FileSystem.TaggerEncapsulation
{
    public class ExperimentalTaggedParser : TagParser
    {
        public ExperimentalTaggedParser(TaggedFile file) {
            TaggededDocumentFile = file;
            FilePath = TaggededDocumentFile.FullPath;
            TaggedInputData = LoadDocumentFile();
        }
        public ExperimentalTaggedParser(string taggedText) {
            TaggedInputData = taggedText;
        }
        private string LoadDocumentFile() {
            using (var reader = new StreamReader(FilePath, Encoding.UTF8)) {
                return reader.ReadToEnd();
            }
        }
        public override Document LoadDocument() {
            throw new NotImplementedException();
        }

        public override IEnumerable<Paragraph> LoadParagraphs() {
            var fileText = LoadDocumentFile();
            var textParagraphs = ParseParagraphs(fileText);
            foreach (var paraText in textParagraphs) {
                Paragraph para = CreateParagraph(paraText);
            }


            throw new NotImplementedException();
        }

        private Paragraph CreateParagraph(string paraText) {
            var sentences = BreakSentences(paraText);
            foreach (var sentence in sentences) {
                Sentence sent = CreateSentence(sentence);
            }
            throw new NotImplementedException();
        }

        private Sentence CreateSentence(string sentence) {
            var clauses = new List<Clause>();
            var begin = sentence.IndexOfAny(new[] { ' ', '\r', '\t', '\n' });
            for (; ; ) {

                var len = FindEndOfClause(sentence.Substring(begin));
                clauses.Add(CreateClause(sentence.Substring(begin, len)));
                begin = len;

            }

            throw new NotImplementedException();
        }

        private Clause CreateClause(string data) {
            var phrases = new List<Phrase>();

            throw new NotImplementedException();
        }

        private int FindEndOfClause(string data) {
            var clauseTagDelims = new[] { "(S", "(SBAR", "SBARQ", "(SQ", "(SINV" };
            var indeces = from D in clauseTagDelims
                          let I = data.IndexOf(D)
                          where I > -1
                          orderby I ascending
                          select I;
            if (indeces.Count() > 0) {
                return indeces.First();
            } else {
                return data.Length;
            }
        }

        private IEnumerable<string> BreakSentences(string paraText) {
            return paraText.Split(new[] { "(TOP" }, StringSplitOptions.RemoveEmptyEntries).AsEnumerable().Select(s => s.Trim());
        }







    }
}
