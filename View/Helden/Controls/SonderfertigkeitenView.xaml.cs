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

using VM = MeisterGeister.ViewModel.Helden;

namespace MeisterGeister.View.Helden.Controls
{
    /// <summary>
    /// Interaktionslogik für SonderfertigkeitenView.xaml
    /// </summary>
    public partial class SonderfertigkeitenView : UserControl
    {
        public SonderfertigkeitenView()
        {
            InitializeComponent();
            VM = new VM.SonderfertigkeitenViewModel(View.General.ViewHelper.Confirm, View.General.ViewHelper.ShowError);
        }

        /// <summary>
        /// Ruft das ViewModel des Views ab oder legt es fest und weist das ViewModel dem DataContext zu.
        /// </summary>
        public VM.SonderfertigkeitenViewModel VM
        {
            get
            {
                if (DataContext == null || !(DataContext is VM.SonderfertigkeitenViewModel))
                    return null;
                return DataContext as VM.SonderfertigkeitenViewModel;
            }
            set { DataContext = value; }
        }
		
        private void ContextMenuSonderfertigkeiten_Opened(object sender, RoutedEventArgs e)
        {
            if (_listBoxHeldSonderfertigkeiten.SelectedItem == null)
            {
                _menuItemSonderfertigkeitLöschen.IsEnabled = false;
                _menuItemSonderfertigkeitlWiki.IsEnabled = false;
            }
            else
            {
                _menuItemSonderfertigkeitLöschen.IsEnabled = !VM.IsReadOnly;
                _menuItemSonderfertigkeitlWiki.IsEnabled = true;
            }
        }
    }
}
