﻿using LASI.App.Dialogs;
using LASI.ContentSystem;
using LASI.Utilities;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LASI.App
{
    /// <summary>
    /// Interaction logic for ExportResultsDialog.xaml
    /// </summary>
    public partial class ProjectPreviewWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the ProjectPreviewScreen class.
        /// </summary>
        public ProjectPreviewWindow() {
            InitializeComponent();
            var titleText = Resources["CurrentProjectName"] as string ?? Title;
            SetupEvents();
        }

        private void SetupEvents() {
            DocumentManager.NumberOfDocumentsChanged += (sender, e) => numDocsLabel.Content = e.NewValue;
        }

        #region Methods

        #region Document Preview Construction
        /// <summary>
        /// Loads and displays a text preview tab for each document in the project.
        /// </summary>
        public async void LoadDocumentPreviews() {
            foreach (var textfile in FileManager.TxtFiles) {
                await LoadTextandTabAsync(textfile);
            }
            DocumentPreview.SelectedIndex = 0;
        }
        /// <summary>
        /// Asynchronously loads the text of the given file into a new preview tab, displaying it with some rudimentary formatting.
        /// </summary>
        /// <param name="textfile">The Text File from which to load text.</param>
        /// <returns>A System.Threading.Tasks.Task representing the ongoing asynchronous operation.</returns>
        private async Task LoadTextandTabAsync(TxtFile textfile) {
            var processedText = await await textfile.GetTextAsync().ContinueWith(async t => {
                var data = await t;
                return data
                    .SplitRemoveEmpty("\r\n\r\n", "<paragraph>", "</paragraph>")
                    .Select(s => s.Trim())
                    .Aggregate(string.Empty, (folded, e) => folded + "\n\t" + e);
            });
            var item = new TabItem
            {
                Header = textfile.NameSansExt,
                AllowDrop = true,
                Content = new TextBox
                {
                    IsReadOnly = true,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    TextWrapping = TextWrapping.Wrap,
                    Text = processedText,
                    FontSize = 12,

                },
            };
            item.Drop += Grid_Drop;
            DocumentPreview.Items.Add(item);
            DocumentPreview.SelectedItem = item;
        }
        /// <summary>
        /// Asynchronously adds a new document by the specified file path.
        /// </summary>
        /// <param name="docPath">The file path specifying where to find the document.</param>
        /// <returns>A System.Threading.Tasks.Task representing the ongoing asynchronous operation.</returns>
        private async Task DisplayAddNewDocumentDialogImplementation(string docPath) {
            var chosenFile = FileManager.AddFile(docPath, true);
            try {
                await FileManager.ConvertAsNeededAsync();
            }
            catch (FileConversionFailureException e) {
                MessageBox.Show(this, MakeConversionFailureMessage(e.Message));
            }
            var textfile = FileManager.TxtFiles.Where(f => f.NameSansExt == chosenFile.NameSansExt).First();
            await LoadTextandTabAsync(textfile);
            CheckIfAddingAllowed();
            startProcessingButton.IsEnabled = true;
            StartProcessMenuItem.IsEnabled = true;
        }

        private static string MakeConversionFailureMessage(string message) {
            return string.Format(".doc file conversion failed\n{0}", message);
        }

        private void CheckIfAddingAllowed() {
            var addingEnabled = DocumentManager.CanAdd;
            AddNewDocumentButton.IsEnabled = addingEnabled;
            FileMenuAdd.IsEnabled = addingEnabled;
        }

        #endregion

        #region Named Event Handlers

        private async void StartButton_Click(object sender, RoutedEventArgs e) {

            WindowManager.InProgressScreen.PositionAt(this);
            this.Hide();
            WindowManager.InProgressScreen.Show();
            await WindowManager.InProgressScreen.ParseDocuments();
        }

        private void RemoveCurrentDocument_Click(object sender, RoutedEventArgs e) {
            var docSelected = DocumentPreview.SelectedItem;
            if (docSelected != null) {
                DocumentPreview.Items.Remove(docSelected);
                DocumentManager.RemoveDocument((docSelected as TabItem).Header.ToString());
                FileManager.RemoveFile((docSelected as TabItem).Header.ToString());
                CheckIfAddingAllowed();

            }
            if (DocumentManager.IsEmpty) {
                startProcessingButton.IsEnabled = false;
                StartProcessMenuItem.IsEnabled = false;
            }

        }
        private async void Grid_Drop(object sender, DragEventArgs e) {
            await SharedWindowFunctionality.HandleDropAddAttemptAsync(this,
                e,
                async fi => {
                    DocumentManager.AddDocument(fi.Name, fi.FullName);
                    await DisplayAddNewDocumentDialogImplementation(fi.FullName);
                });
        }
        private async void DisplayAddNewDocumentDialog() {
            var openDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "LASI File Types|*.doc; *.docx; *.pdf; *.txt",
                Multiselect = true,

            };
            openDialog.ShowDialog(this);
            if (openDialog.FileNames.Count() <= 0) {
                return;
            }
            for (int i = 0; i < openDialog.SafeFileNames.Length; i++) {
                var file = new FileInfo(openDialog.FileNames[i]);
                if (DocumentManager.FileNamePresent(file.Name)) {
                    MessageBox.Show(this, string.Format("A document named {0} is already part of the project.", file));
                } else if (!file.CanOpen()) {
                    DocumentManager.AddDocument(file.Name, file.FullName);
                    await DisplayAddNewDocumentDialogImplementation(file.FullName);
                } else {
                    MessageBox.Show(this, string.Format("The document {0} is in use by another process, please close any applications which may be using the document and try again.", file));
                }

            }
        }



        private void openLicensesMenuItem_Click_1(object sender, RoutedEventArgs e) {
            var componentsDisplay = new ComponentInfoDialogWindow
            {
                Left = this.Left,
                Top = this.Top,
                Owner = this
            };
            componentsDisplay.ShowDialog();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            SharedWindowFunctionality.ProcessOpenWebsiteRequest(this);
        }

        private void AddDocument_CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e) {
            DisplayAddNewDocumentDialog();
        }

        private void CloseApp_CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e) {
            this.Close();
            Application.Current.Shutdown();
        }

        private void OpenPreferences_CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e) {
            SharedWindowFunctionality.OpenPreferencesWindow(this);
        }

        private void OpenManual_CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e) {
            SharedWindowFunctionality.ProcessOpenManualRequest(this);
        }



        #endregion

        private void AddNewDocumentButton_Click(object sender, RoutedEventArgs e) {
            DisplayAddNewDocumentDialog();
        }

        #endregion


    }
}