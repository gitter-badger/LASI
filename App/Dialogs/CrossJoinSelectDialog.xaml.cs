﻿using LASI.Core.DocumentStructures;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LASI.App.Dialogs
{
    /// <summary>
    /// Interaction logic for CrossJoinSelectDialog.xaml
    /// </summary>
    public partial class CrossJoinSelectDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the CrossJoinSelectDialog in the context of the given results screen.
        /// </summary>
        /// <param name="owner">The results screen referencing the Documents to Display and owning the new dialog window.</param>
        public CrossJoinSelectDialog(ResultsWindow owner) {
            InitializeComponent();

            foreach (var doc in owner.Documents) {
                var docCheckBox = new CheckBox {
                    Content = doc.Name,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                docCheckBox.Checked += (sender, e) => {
                    selectDocuments.Add(doc);
                    okButton.IsEnabled = selectDocuments.Count > 1 ? true : false;
                };
                docCheckBox.Unchecked += (sender, e) => {
                    selectDocuments.Remove(doc);
                    okButton.IsEnabled = selectDocuments.Count > 1 ? true : false;
                };

                documentSelectStackPanel.Children.Add(docCheckBox);
            }
        }


        private void okButton_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Escape:
                    DialogResult = false;
                    this.Close();
                    break;
            }
        }

        private List<Document> selectDocuments = new List<Document>();

        /// <summary>
        /// Gets the documents selected by the user.
        /// </summary>
        public List<Document> SelectDocuments {
            get {
                return selectDocuments;
            }
        }


    }
}