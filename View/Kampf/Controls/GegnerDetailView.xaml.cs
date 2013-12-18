﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Settings = MeisterGeister.Logic.Settings;

namespace MeisterGeister.View.Kampf.Controls
{
    /// <summary>
    /// Interaktionslogik für GegnerDetailView.xaml
    /// </summary>
    public partial class GegnerDetailView : UserControl
    {
        public GegnerDetailView()
        {
            InitializeComponent();
        }

        private void ButtonCloseZonenRsControl_Click(object sender, RoutedEventArgs e)
        {
            _rsZonenRsControl.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ButtonZonenRsControl_Click(object sender, RoutedEventArgs e)
        {
            _rsZonenRsControl.Visibility = System.Windows.Visibility.Visible;
        }

        private void TextBoxAngriffName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender != null && sender is TextBox)
                (sender as TextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Expanded Sections
            string sections = Logic.Settings.Einstellungen.GegnerDetailViewExpandedSections;
            if (sections.Length >= 1)
                expanderBasisinformationen.IsExpanded = (sections[0] == '1');
            if (sections.Length >= 2)
                expanderAngriffe.IsExpanded = (sections[1] == '1');
            if (sections.Length >= 3)
                expanderJagd.IsExpanded = (sections[2] == '1');
        }

        private void Expander_ExpandedCollapsed(object sender, RoutedEventArgs e)
        {
            // Expanded Sections speichern
            if (IsInitialized && IsLoaded)
            {
                string sections = string.Empty;
                sections += (expanderBasisinformationen.IsExpanded ? "1" : "0");
                sections += (expanderAngriffe.IsExpanded ? "1" : "0");
                sections += (expanderJagd.IsExpanded ? "1" : "0");
                Logic.Settings.Einstellungen.GegnerDetailViewExpandedSections = sections;
            }
        }
    }
}
