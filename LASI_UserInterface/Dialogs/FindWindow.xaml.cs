﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LASI.UserInterface.Dialogs
{
    /// <summary>
    /// Interaction logic for FindWindow.xaml
    /// </summary>
    public partial class FindWindow : Window
    {
        public FindWindow() {
            InitializeComponent();
        }
        /// <summary>
        /// Executes a lexical search over applicable elements when invoked.
        /// </summary>
        /// <param name="sender">The source of the click event.</param>
        /// <param name="e">The arguments passed invocation.</param>
        private void findButton_Click(object sender, RoutedEventArgs e) {
            PerformFind(searchForTextBox.Text);
        }
        /// <summary>
        /// Executes a lexical search over an applicable set of elements in the current UI context using the provided text as the search seed.
        /// </summary>
        /// <param name="searchText">The text which seeds the search.</param>
        private void PerformFind(string searchText) {
            var toHighLight = from uiElement in GetElementsToSearch()
                              where uiElement.Content.ToString().Contains(searchText)
                              select uiElement;
        }

        /// <summary>
        /// Returns the collection of ContentControl UIElements over whose contents to search.
        /// </summary>
        /// <returns>The collection of ContentControl UIElements over whose contents to search.</returns>
        private IEnumerable<ContentControl> GetElementsToSearch() {
            throw new NotImplementedException();
        }

    }
}