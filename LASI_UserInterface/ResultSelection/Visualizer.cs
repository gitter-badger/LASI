﻿using LASI;
using LASI.Algorithm;
using LASI.Algorithm.DocumentStructures;
using LASI.Algorithm.LexicalLookup;
using LASI.Algorithm.Aliasing;
using LASI.Algorithm.Patternization;
using LASI.InteropLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;


namespace LASI.UserInterface
{
    /// <summary>
    /// Provides static methods for formatting and displaying results to the user.
    /// </summary>
    public static class Visualizer
    {


        #region Chart Transposing Methods

        /// <summary>
        /// Instructs the Charting manager to recreate all Per-Document charts based on the provided ChartKind. 
        /// The given ChartKind will be used for all further chart operations until it is changed via another call to ChangeChartKind.
        /// </summary>
        /// <param name="chartKind">The ChartKind value determining the what data set is to be displayed.</param>
        public static void ChangeChartKind(ChartKind chartKind) {
            ChartKind = chartKind;
            foreach (var pair in documentsByChart) {

                Document doc = pair.Value;
                Chart chart = pair.Key;

                IEnumerable<KeyValuePair<string, float>> data = null;

                switch (chartKind) {
                    case ChartKind.SubjectVerbObject:
                        data = GetVerbWiseData(doc);
                        break;
                    case ChartKind.NounPhrasesOnly:
                        data = GetNounWiseData(doc);
                        break;
                }
                data = data.Take(CHART_ITEM_LIMIT);
                chart.Series.Clear();
                chart.Series.Add(new BarSeries
                {
                    DependentValuePath = "Value",
                    IndependentValuePath = "Key",
                    ItemsSource = data,
                    IsSelectionEnabled = true,

                });
                chart.Title = string.Format("Key Relationships in {0}", doc.Name);
                break;
            }
        }


        /// <summary>
        /// Reconfigures all charts to Subjects Column perspective
        /// </summary>
        /// <returns>A Task which completes on the successful reconstruction of all charts</returns>
        public static async Task ToColumnCharts() {
            foreach (var chart in WindowManager.ResultsScreen.FrequencyCharts.Items.OfType<TabItem>().Select(item => item.Content as Chart).Where(c => c != null)) {
                var items = chart.GetItemSource();
                var series = new ColumnSeries
                {
                    DependentValuePath = "Value",
                    IndependentValuePath = "Key",
                    ItemsSource = items,
                    IsSelectionEnabled = true
                };
                ResetChartContent(chart, series);
            }
            await Task.Delay(1);
        }



        /// <summary>
        /// Reconfigures all charts to Subjects Pie perspective
        /// </summary>
        /// <returns>A Task which completes on the successful reconstruction of all charts</returns>
        public static async Task ToPieCharts() {
            foreach (var chart in WindowManager.ResultsScreen.FrequencyCharts.Items.OfType<TabItem>().Select(item => item.Content as Chart).Where(c => c != null)) {
                var items = chart.GetItemSource();
                var series = new PieSeries
                {
                    DependentValuePath = "Value",
                    IndependentValuePath = "Key",
                    ItemsSource = items,
                    IsSelectionEnabled = true,
                };
                series.IsMouseCaptureWithinChanged += (sender, e) => {
                    series.ToolTip = (series.SelectedItem as DataPoint).DependentValue;
                };

                ResetChartContent(chart, series);
            }
            await Task.Delay(1);
        }

        /// <summary>
        /// Reconfigures all charts to Subjects Bar perspective
        /// </summary>
        /// <returns>A Task which completes on the successful reconstruction of all charts</returns>
        public static async Task ToBarCharts() {
            foreach (var chart in WindowManager.ResultsScreen.FrequencyCharts.Items.OfType<TabItem>().Select(item => item.Content as Chart).Where(c => c != null)) {
                var items = chart.GetItemSource();
                var series = new BarSeries
                {
                    DependentValuePath = "Value",
                    IndependentValuePath = "Key",
                    ItemsSource = items,
                    IsSelectionEnabled = true,
                };
                ResetChartContent(chart, series);
            }
            await Task.Delay(1);
        }
        /// <summary>
        /// Asynchronously initializes, creates and displays the default Chart for the given document. This method should only be called once.
        /// </summary>
        /// <param name="document">The document whose contents are to be charted.</param>
        /// <returns>A Task representing the ongoing asynchronous operation.</returns>
        public static async Task InitChartDisplayAsync(Document document) {
            var chart = await BuildBarChart(document);
            documentsByChart.Add(chart, document);
            var tab = new TabItem
            {
                Header = document.Name,
                Content = chart,
                Tag = chart
            };
            WindowManager.ResultsScreen.FrequencyCharts.Items.Add(tab);
            WindowManager.ResultsScreen.FrequencyCharts.SelectedItem = tab;
            await ToBarCharts();
        }


        private static async Task<Chart> BuildBarChart(Document document) {

            var dataPointSource = ChartKind == ChartKind.NounPhrasesOnly ? await GetNounWiseDataAsync(document) :
                ChartKind == ChartKind.SubjectVerbObject ? GetVerbWiseData(document) :
                GetVerbWiseData(document);
            var topPoints = dataPointSource.OrderByDescending(e => e.Value).Take(CHART_ITEM_LIMIT);
            Series series = new BarSeries
            {
                DependentValuePath = "Value",
                IndependentValuePath = "Key",
                ItemsSource = dataPointSource,
                IsSelectionEnabled = true,
                Tag = document,

            };

            var chart = new Chart
            {
                Title = string.Format("Key Subjects in {0}", document.Name),
                Tag = dataPointSource.ToArray()
            };

            series.MouseMove += (sender, e) => {
                series.ToolTip = (e.Source as DataPoint).IndependentValue;
            };

            chart.Series.Add(series);
            return chart;
        }



        #endregion

        #region General Chart Building Methods
        /// <summary>
        /// Removes all current data points from the chart before adding the provided DataPointSeries to the chart.
        /// </summary>
        /// <param name="chart">The chart whose contents to replace with the given The DataPointSeries.</param>
        /// <param name="series">The DataPointSeries containing the data points which the chart will contain.</param>
        public static void ResetChartContent(Chart chart, DataPointSeries series) {
            chart.Series.Clear();
            chart.Series.Add(series);
        }

        private static IEnumerable<KeyValuePair<string, float>> GetItemSource(this Chart chart) {
            return (chart.Tag as IEnumerable<KeyValuePair<string, float>>).OrderByDescending(e => e.Value).Take(CHART_ITEM_LIMIT).Reverse();

        }

        #endregion
        private static IEnumerable<KeyValuePair<string, float>> GetVerbWiseData(Document doc) {
            var data = GetVerbWiseRelationships(doc);
            return from svs in data

                   let SV = new KeyValuePair<string, float>(
                       string.Format("{0} -> {1}\n", svs.Subject.Text, svs.Verbal.Text) +
                       (svs.Direct != null ? " -> " + svs.Direct.Text : "") +
                       (svs.Indirect != null ? " -> " + svs.Indirect.Text : ""),
                       (float)Math.Round(svs.CombinedWeight, 2))
                   group SV by SV into svg
                   select svg.Key;

        }
        private static IEnumerable<Relationship> GetVerbWiseRelationships(Document doc) {
            var data =
                 from svPair in
                     (from vp in doc.Phrases.OfVerbPhrase()
                          .WithSubject(s => (s as IReferencer) == null || (s as IReferencer).Referent != null).Distinct((L, R) => L.IsSimilarTo(R))
                          .AsParallel().WithDegreeOfParallelism(Concurrency.Max)
                      from s in vp.Subjects.AsParallel().WithDegreeOfParallelism(Concurrency.Max)
                      let sub = s as IReferencer == null ? s : (s as IReferencer).Referent
                      where sub != null
                      from dobj in vp.DirectObjects.DefaultIfEmpty()
                      from iobj in vp.IndirectObjects.DefaultIfEmpty()

                      select new Relationship
                      {
                          Subject = vp.AggregateSubject,
                          Verbal = vp,
                          Direct = vp.AggregateDirectObject,
                          Indirect = vp.AggregateIndirectObject,
                          Prepositional = vp.ObjectOfThePreoposition,
                          CombinedWeight = sub.Weight + vp.Weight + (dobj != null ? dobj.Weight : 0) + (iobj != null ? iobj.Weight : 0)
                      } into tupple
                      where
                      tupple.Subject != null &&
                        tupple.Direct != null ||
                        tupple.Indirect != null &&
                        tupple.Subject.Text != (tupple.Direct ?? tupple.Indirect).Text
                      select tupple).Distinct()
                 select svPair into svps

                 orderby svps.CombinedWeight
                 select svps;
            return data;
        }
        private static async Task<IEnumerable<KeyValuePair<string, float>>> GetNounWiseDataAsync(Document doc) { return await Task.Run(() => GetNounWiseData(doc)); }

        private static IEnumerable<KeyValuePair<string, float>> GetNounWiseData(Document doc) {
            return from NP in doc.Phrases.OfNounPhrase().AsParallel().WithDegreeOfParallelism(Concurrency.Max)
                       .Distinct((x, y) => x.IsSimilarTo(y))

                   group NP by new
                   {
                       NP.Text,
                       NP.Weight
                   } into NP
                   select NP.Key into master
                   orderby master.Weight
                   select new KeyValuePair<string, float>(master.Text, (float)Math.Round(master.Weight, 2));
        }

        /// <summary>
        /// Asynchronously generates, composeses and displays the key relationships view for the given Document.
        /// </summary>
        /// <param name="document">The document for which to build relationships.</param>
        /// <returns>A Task representing the ongoing asynchronous operation.</returns>
        public static async Task DisplayKeyRelationships(Document document) {

            var transformedData = await Task.Factory.StartNew(() => {
                return TransformToGrid(GetVerbWiseRelationships(document));
            });
            var wpfToolKitDataGrid = new Microsoft.Windows.Controls.DataGrid
            {
                ItemsSource = transformedData,
            };
            var tab = new TabItem
            {
                Header = document.Name,
                Content = wpfToolKitDataGrid
            };
            WindowManager.ResultsScreen.SVODResultsTabControl.Items.Add(tab);
            WindowManager.ResultsScreen.SVODResultsTabControl.SelectedItem = tab;

        }
        /// <summary>
        /// Creates and returns a sequence of textual display elements from the given sequence of RelationshipTuple elements.
        /// The resulting sequence is suitable for direct insertion into a DataGrid.
        /// </summary>
        /// <param name="elementsToConvert">The sequence of Relationship Tuple to tranform into textual display elements.</param>
        /// <returns>A sequence of textual display elements from the given sequence of RelationshipTuple elements.</returns>
        internal static IEnumerable<object> TransformToGrid(IEnumerable<Relationship> elementsToConvert) {
            return from e in elementsToConvert.Distinct()
                   orderby e.CombinedWeight
                   select new
                   {
                       Subject = e.Subject != null ? e.Subject.Text : string.Empty,
                       Verbial = e.Verbal != null ?
                              (e.Verbal.PrepositionOnLeft != null ? e.Verbal.PrepositionOnLeft.Text : string.Empty)
                                   + (e.Verbal.Modality != null ? e.Verbal.Modality.Text : string.Empty)
                                   + e.Verbal.Text + (e.Verbal.Modifiers.Any() ? " (adv)> "
                                   + string.Join(" ", e.Verbal.Modifiers.Select(m => m.Text)) : string.Empty)
                               : string.Empty,
                       Direct = e.Direct != null ?
                            (e.Direct.PrepositionOnLeft != null ? e.Direct.PrepositionOnLeft.Text
                                : string.Empty + e.Direct.Text)
                            : string.Empty,
                       Indirect = e.Indirect != null ?
                            (e.Indirect.PrepositionOnLeft != null ? e.Indirect.PrepositionOnLeft.Text : string.Empty)
                                + e.Indirect.Text
                            : string.Empty,
                       Prepositional = e.Prepositional != null ?
                            e.Prepositional.Text : string.Empty
                   };
        }
        /// <summary>
        /// Gets the ChartKind currently used by the ChartManager.
        /// </summary>
        public static ChartKind ChartKind {
            get;
            private set;
        }
        private static Dictionary<Chart, Document> documentsByChart = new Dictionary<Chart, Document>();


        private const int CHART_ITEM_LIMIT = 14;




    }
    #region Result Bulding Helper Types

    /// <summary>
    /// Sometimes an anonymous type simple will not do. So this little class is defined to 
    /// store temporary query data from transposed tables. god it is late. I can't document properly.
    /// </summary>
    internal class Relationship : IEquatable<Relationship>
    {
        private IVerbal verbal;
        private IAggregateEntity subject;
        private IAggregateEntity direct;
        private IAggregateEntity indirect;
        private ILexical prepositional;
        private HashSet<ILexical> elements = new HashSet<ILexical>();

        public IAggregateEntity Subject {
            get {
                return subject;
            }
            set {
                subject = value;
                elements.Add(value);
            }
        }
        public IVerbal Verbal {
            get {
                return verbal;
            }
            set {
                verbal = value;
                elements.Add(value);
            }
        }
        public IAggregateEntity Direct {
            get {
                return direct;
            }
            set {
                direct = value;
                elements.Add(value);
            }
        }
        public IAggregateEntity Indirect {
            get {
                return indirect;
            }
            set {
                indirect = value;
                elements.Add(value);
            }
        }
        public ILexical Prepositional {
            get {
                return prepositional;
            }
            set {
                prepositional = value;
                elements.Add(value);
            }
        }

        public HashSet<ILexical> Elements {
            get {
                return elements;
            }
        }
        public double CombinedWeight {
            get;
            set;
        }
        /// <summary>
        /// Returns a textual representation of the RelationshipTuple.
        /// </summary>
        /// <returns>A textual representation of the RelationshipTuple.</returns>
        public override string ToString() {
            var result = Subject.Text + Verbal.Text;
            if (Direct != null) {
                result += Direct.Text;
            }
            if (Indirect != null) {
                result += Indirect.Text;
            }
            return result;
        }
        public bool Equals(Relationship other) { return this == other; }
        public override bool Equals(object obj) { return this == obj as Relationship; }

        public override int GetHashCode() { return elements.Count; }

        public static bool operator ==(Relationship lhs, Relationship rhs) {

            if ((lhs as object != null || rhs as object == null) || (lhs as object == null || rhs as object != null))
                return false;
            else if (lhs as object == null && rhs as object == null)
                return true;
            else {
                bool result = lhs.Verbal.Text == rhs.Verbal.Text || lhs.Verbal.IsSimilarTo(rhs.Verbal);
                result &= LexicalComparers<IEntity>.AliasOrSimilarity.Equals(lhs.Subject, rhs.Subject);
                if (lhs.Direct != null && rhs.Direct != null) {
                    result &= LexicalComparers<IEntity>.AliasOrSimilarity.Equals(lhs.Direct, rhs.Direct);
                } else if (lhs.Direct == null || rhs.Direct == null)
                    return false;
                if (lhs.Indirect != null && rhs.Indirect != null) {
                    result &= LexicalComparers<IEntity>.AliasOrSimilarity.Equals(lhs.Indirect, rhs.Indirect);
                } else if (lhs.Indirect == null || rhs.Indirect == null)
                    return false;
                return result;
            }
        }

        public static bool operator !=(Relationship lhs, Relationship rhs) {
            return !(lhs == rhs);
        }
    }

    #endregion
}