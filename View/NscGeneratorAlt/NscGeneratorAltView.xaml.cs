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
//Eigene Usings
using VM = MeisterGeister.ViewModel.NscGeneratorAlt;
//Weitere Usings
using System.Diagnostics;
using System.IO;
using MeisterGeister.View.Windows;
using MeisterGeister.Logic.Kalender;

namespace MeisterGeister.View.NscGeneratorAlt
{
    public partial class NscGeneratorAltView : UserControl
    {
        public NscGeneratorAltView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                (this.DataContext as VM.NscGeneratorAltViewModel).LoadDaten();
            }
            catch (Exception ex)
            {
                View.Windows.MsgWindow errWin = new View.Windows.MsgWindow("Tool", "Beim Laden des Tools ist ein Fehler aufgetreten.", ex);
                errWin.ShowDialog();
            }
        }
        
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (this.DataContext as VM.NscGeneratorAltViewModel).AddNscToNotiz(_listBoxNscs.SelectedItems);
            }
            catch (Exception)
            {
            }
        }
        
    }
}
