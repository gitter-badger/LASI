﻿using LASI.Algorithm.Binding;
using LASI.Algorithm.DocumentStructures;
using LASI.Algorithm.LexicalLookup;
using LASI.Algorithm.Weighting;
using LASI.ContentSystem;
using LASI.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace LASI.InteropLayer
{

    /// <summary>
    /// Governs the complete analysis and processing of one or more text sources.
    /// Provides synchronous and asynchronoun callback based progress reports.
    /// </summary>
    public sealed class ProcessController : Progress<LASI.InteropLayer.ProcessController.Report>
    {
        /// <summary>
        /// Gets a Task&lt;IEnumerable&lt;LASI.Algorithm.Document&gt;&gt; which, when awaited, loads, analyizes, and aggregates all of the provided TextFile instances as individual documents, collecting them as
        /// a sequence of Bound and Weighted LASI.Algorithm.Document instances. Progress update logic is specified via an asynchronous function parameter.
        /// </summary>
        /// <param name="filesToProcess">The collection of TextFiles to analyize.</param>
        /// <returns>A Task&lt;IEnumerable&lt;LASI.Algorithm.Document&gt;&gt;, when awaited, loads and analyizes, and aggregates all of the provided TextFile instances as individual documents, collecting them as
        /// a sequence of Bound and Weighted LASI.Algorithm.Document instances.</returns>
        /// <example>
        ///Example event registration:
        ///<code>
        /// myProcessController.ProgressChanged += async (sender, e) => MsgBox.Show(e.Message + " " + e.Increment);
        /// </code>
        /// </example>
        public async Task<IEnumerable<Document>> AnalyseAllDocumentsAsync(IEnumerable<LASI.Algorithm.IUntaggedTextSource> filesToProcess) {
            numDocs = filesToProcess.Count();
            stepSize = 2d / numDocs;
            await LoadThesaurus();
            OnReport(new Report { Message = "Tagging Documents", Increment = 0 });
            var taggingTasks = filesToProcess.Select(F => Task.Run(async () => await Tagger.TaggedFromRawAsync(F))).ToList();
            var taggedFiles = new ConcurrentBag<LASI.Algorithm.ITaggedTextSource>();
            while (taggingTasks.Any()) {
                var currentTask = await Task.WhenAny(taggingTasks);
                var taggedFile = await currentTask;
                taggingTasks.Remove(currentTask);
                taggedFiles.Add(taggedFile);
                OnReport(new Report { Message = string.Format("{0}: Tagged", taggedFile.TextSourceName), Increment = stepSize + 1.5 });
            }
            OnReport(new Report { Message = "Tagged Documents", Increment = 3 });
            var tasks = taggedFiles.Select(tagged => ProcessTaggedFileAsync(tagged)).ToList();
            var documents = new ConcurrentBag<Document>();
            while (tasks.Any()) {
                var currentTask = await Task.WhenAny(tasks);
                var processedDocument = await currentTask;
                tasks.Remove(currentTask);
                documents.Add(processedDocument);
            }
            return documents;
        }


        private async Task<Document> ProcessTaggedFileAsync(LASI.Algorithm.ITaggedTextSource tagged) {
            var fileName = tagged.TextSourceName;
            OnReport(new Report { Message = string.Format("{0}: Loading...", fileName), Increment = 0 });
            var doc = await Tagger.DocumentFromTaggedAsync(tagged);
            OnReport(new Report { Message = string.Format("{0}: Loaded", fileName), Increment = 4 });
            OnReport(new Report { Message = string.Format("{0}: Analyzing Syntax...", fileName), Increment = 0 });
            foreach (var task in doc.GetBindingTasks()) {
                OnReport(new Report { Message = task.InitializationMessage, Increment = 0 });
                await task.Task;
                OnReport(new Report { Message = task.CompletionMessage, Increment = task.PercentWorkRepresented * 0.5 / numDocs });
            }
            OnReport(new Report { Message = string.Format("{0}: Correlating Relationships...", fileName), Increment = 0 });
            foreach (var task in doc.GetWeightingTasks()) {
                OnReport(new Report { Message = task.InitializationMessage, Increment = 1 / numDocs });
                await task.Task;
                OnReport(new Report { Message = task.CompletionMessage, Increment = task.PercentWorkRepresented * 0.5 / numDocs });
            }

            OnReport(new Report { Message = string.Format("{0}: Completing Parse...", fileName), Increment = stepSize });
            return doc;
        }
        private async Task LoadThesaurus() {
            OnReport(new Report { Message = "Loading Thesaurus...", Increment = stepSize });
            var thesaurusTasks = Lookup.GetLoadingTasks().ToList();
            while (thesaurusTasks.Any()) {
                var currentTask = await Task.WhenAny(thesaurusTasks);
                OnReport(new Report { Message = await currentTask, Increment = 3 });
                thesaurusTasks.Remove(currentTask);
            }
        }

        protected override void OnReport(Report value) {

            base.OnReport(value);
        }
        private double numDocs;
        private double stepSize;

        #region Helper Types
        public struct Report
        {
            public string Message { get; internal set; }
            public double Increment { get; internal set; }
        }

        #endregion
    }


}


