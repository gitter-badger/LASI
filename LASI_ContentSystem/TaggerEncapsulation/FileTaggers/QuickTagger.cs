﻿using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using LASI.ContentSystem.TaggerEncapsulation;

namespace LASI.ContentSystem.TaggerEncapsulation
{

    sealed class QuickTagger : SharpNLPTagger
    {
        public QuickTagger(TaggerMode option)
            : base(option) {

        }
        public LASI.Algorithm.ITaggedTextSource TagTextSource(LASI.Algorithm.IRawTextSource source) {
            SourceText = base.PreProcessText(source.GetText());
            return new LASI.Algorithm.TaggedTextFragment(base.ParseViaTaggingMode(), source.Name);

        }
        public async Task<LASI.Algorithm.ITaggedTextSource> TagTextSourceAsync(LASI.Algorithm.IRawTextSource source) {
            SourceText = base.PreProcessText(source.GetText());
            return new LASI.Algorithm.TaggedTextFragment(await base.ParseViaTaggingModeAsync(), source.Name);

        }
        public string TagTextSource(string source) {
            SourceText = base.PreProcessText(source);
            return base.ParseViaTaggingMode();

        }
        public async Task<string> TagTextSourceAsync(string source) {
            SourceText = base.PreProcessText(source);
            return await base.ParseViaTaggingModeAsync();

        }



    }
}