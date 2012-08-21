﻿using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using MeisterGeister.Daten;
using System.Linq;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;
using System.Collections.Generic;
// Eigene Usings
using VM = MeisterGeister.ViewModel;
using MeisterGeister.Logic.General;
using MeisterGeister.View.Windows;
using MeisterGeister.View.Kampf;
using System.IO;

namespace MeisterGeister.View.Helden
{
    /// <summary>
    /// Interaktionslogik für HeldenView.xaml
    /// </summary>
    public partial class HeldenView : System.Windows.Controls.UserControl
    {
        public HeldenView()
        {
            InitializeComponent();


            _comboBoxSonderfertigkeit.SelectedIndex = -1;
            _comboBoxZauber.SelectedIndex = -1;
        }

        public event System.ComponentModel.PropertyChangedEventHandler HeldChanged;

        void SelectedHeld_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "This")
            {
                _energieControlAstralenergie.SetEnergie();
                _energieControlAusdauer.SetEnergie();
                _energieControlKarmaenergie.SetEnergie();
                _energieControlLebensenergie.SetEnergie();

                // Changed Event weitergeben
                if (HeldChanged != null)
                    HeldChanged(sender, e);
            }
        }

        //TODO DW: Trennen in Controls
        private void RefreshHeld(bool refreshRepräsentationen = true)
        {
            _labelAstralenergie.Visibility = System.Windows.Visibility.Visible;
            _labelKarmaenergie.Visibility = System.Windows.Visibility.Visible;
            _imageAeKeHinweis.Visibility = System.Windows.Visibility.Collapsed;

            Held held = SelectedHeld;

            if (!held.Magiebegabt && !held.Geweiht)
                _imageAeKeHinweis.Visibility = System.Windows.Visibility.Visible;

            // Magiebegabung und Astralenergie
            if (!held.Magiebegabt)
            {
                _labelAstralenergie.Visibility = System.Windows.Visibility.Collapsed;
                if (tabControl1.SelectedItem == _tabItemZauber)
                    tabControl1.SelectedItem = _tabItemTalente;
            }
            else
            {
                if (refreshRepräsentationen)
                    _comboBoxRepräsentation.SelectedValue = held.RepräsentationStandard();

                // Zauber-Sortierung aktualisieren
                if (_dataGridHeldZauber.Items is System.Windows.Data.CollectionView)
                {
                    System.Windows.Data.CollectionViewSource csv = (System.Windows.Data.CollectionViewSource)FindResource("heldHeld_ZauberViewSource");
                    if (csv != null && csv.View != null)
                    {
                        csv.View.SortDescriptions.Clear();
                        csv.View.SortDescriptions.Add(new System.ComponentModel.SortDescription("Zaubername", System.ComponentModel.ListSortDirection.Ascending));
                        csv.View.SortDescriptions.Add(new System.ComponentModel.SortDescription("Repräsentation", System.ComponentModel.ListSortDirection.Ascending));
                        _dataGridHeldZauber.ItemsSource = csv.View;
                    }
                }
            }
            // Geweiht und Karmaenergie
            if (!held.Geweiht)
                _labelKarmaenergie.Visibility = System.Windows.Visibility.Collapsed;

            
        }

        public Held SelectedHeld
        {
            get
            {
                Held held = null;
                if(Global.SelectedHeldGUID != Guid.Empty)
                    held = new Held(Global.SelectedHeldGUID);
                return held;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //ListBoxHelden_SelectionChanged(sender, null);
        }

        private void _listBoxHeldSonderfertigkeiten_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (_listBoxHeldSonderfertigkeiten.SelectedItem != null)
            {
                switch (e.Key)
                {
                    // Sonderfertigkeit löschen
                    case Key.Delete:
                        DeleteHeldSonderfertigkeit();
                        break;
                    default:
                        break;
                }
            }
        }

        private DatabaseDSADataSet.Held_SonderfertigkeitRow SelectedSonderfertigkeit
        {
            get
            {
                DatabaseDSADataSet.Held_SonderfertigkeitRow sfRow = null;
                if (_listBoxHeldSonderfertigkeiten.SelectedItem != null)
                    sfRow = (DatabaseDSADataSet.Held_SonderfertigkeitRow)((System.Data.DataRowView)_listBoxHeldSonderfertigkeiten.SelectedItem).Row;
                return sfRow;
            }
        }

        private void ListBoxSonderfertigkeiten_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _listBoxHeldSonderfertigkeiten.SelectedItem = null;
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
                _menuItemSonderfertigkeitLöschen.IsEnabled = true;
                _menuItemSonderfertigkeitlWiki.IsEnabled = true;
            }
        }

        private void MenuItemSonderfertigkeitLöschen_Click(object sender, RoutedEventArgs e)
        {
            DeleteHeldSonderfertigkeit();
        }


        private DatabaseDSADataSet.Held_ZauberRow SelectedZauberRow
        {
            get
            {
                DatabaseDSADataSet.Held_ZauberRow zauberRow = null;
                if (_dataGridHeldZauber.SelectedItem != null)
                    zauberRow = (DatabaseDSADataSet.Held_ZauberRow)((System.Data.DataRowView)_dataGridHeldZauber.SelectedItem).Row;

                return zauberRow;
            }
        }

        private Zauber SelectedZauber
        {
            get { return new Zauber(SelectedZauberRow.ZauberRow); }
        }

        private void DeleteHeldSonderfertigkeit()
        {
            DatabaseDSADataSet.Held_SonderfertigkeitRow sf = SelectedSonderfertigkeit;
            if (MessageBox.Show(string.Format("Soll die Sonderfertigkeit '{0}' entfernt werden?", sf.SonderfertigkeitRow.Name), "Sonderfertigkeit entfernen", MessageBoxButton.YesNo,
                                MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                {
                    SelectedHeld.DeleteSonderfertigkeit(sf);
                    _listBoxHeldSonderfertigkeiten.SelectedIndex = -1;
                }
            }
        }

     

        private void ContextMenuZauber_Opened(object sender, RoutedEventArgs e)
        {
            if (_dataGridHeldZauber.SelectedItem == null)
            {
                _menuItemZauberLöschen.IsEnabled = false;
                _menuItemZauberWiki.IsEnabled = false;
            }
            else
            {
                _menuItemZauberLöschen.IsEnabled = true;
                _menuItemZauberWiki.IsEnabled = true;
            }
        }

       

        

        public event ProbeWürfelnEventHandler ProbeWürfeln;



        private void MenuItemZauberProben_Click(object sender, RoutedEventArgs e)
        {
            if (ProbeWürfeln != null)
            {
                ProbeWürfeln(SelectedZauberRow.ZauberRow.Name);
            }
        }

        private void MenuItemSonderfertigkeitWiki_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.wiki-aventurica.de/wiki/" + SelectedSonderfertigkeit.SonderfertigkeitRow.Name);
        }

        private void ButtonMax_Click(object sender, RoutedEventArgs e)
        {
            switch (((System.Windows.Controls.Button)sender).Tag.ToString())
            {
                case "LE":
                    _intBoxLeAktuell.Value = Convert.ToInt32(_textBlockLebensenergieMax.Text);
                    break;
                case "AU":
                    _intBoxAuAktuell.Value = Convert.ToInt32(_textBlockAusdauerMax.Text);
                    break;
                case "AE":
                    _intBoxAeAktuell.Value = Convert.ToInt32(_textBlockAstralenergieMax.Text);
                    break;
                case "KE":
                    _intBoxKeAktuell.Value = Convert.ToInt32(_textBlockKarmaenergieMax.Text);
                    break;
                default:
                    break;
            }
        }
               
        
        


        private void _comboBoxSonderfertigkeit_DropDownOpened(object sender, EventArgs e)
        {
            SetSonderfertigkeitenAktivierbar();
        }

        private void SetSonderfertigkeitenAktivierbar()
        {
            _comboBoxSonderfertigkeit.ItemsSource = SelectedHeld.SonderfertigkeitenErlernbar;
            _comboBoxSonderfertigkeit.SelectedIndex = -1;
        }


        private void _comboBoxSonderfertigkeit_DropDownClosed(object sender, EventArgs e)
        {
            InsertSonderfertigkeit();
        }

        private void _comboBoxSonderfertigkeit_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                InsertSonderfertigkeit();
        }

        private void InsertSonderfertigkeit()
        {
            if (IsInitialized)
            {
                if (_comboBoxSonderfertigkeit.SelectedItem != null && SelectedHeld.Id != Guid.Empty)
                {
                    var sf = (System.Collections.Generic.KeyValuePair<string, int>)_comboBoxSonderfertigkeit.SelectedItem;
                    if (SelectedHeld.AddSonderfertigkeit(sf.Value) == false)
                        MessageBox.Show(string.Format("Die Sonderfertigkeit '{0}' ist bereits vorhanden.", sf.Key), "Sonderfertigkeit hinzufügen");
                    RefreshHeld();
                    SetSonderfertigkeitenAktivierbar();
                }
            }
        }

        private void _comboBoxZauber_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                InsertZauber();
        }

        private void InsertZauber()
        {
            if (IsInitialized)
            {
                if (_comboBoxZauber.SelectedItem != null && SelectedHeld.Id != Guid.Empty)
                {
                    var zauber = (System.Collections.Generic.KeyValuePair<string, int>)_comboBoxZauber.SelectedItem;
                    string rep = _comboBoxRepräsentation.SelectedValue.ToString();
                    if (SelectedHeld.AddZauber(zauber.Value, rep) == false)
                        MessageBox.Show(string.Format("Der Zauber '{0}' in der Repräsentation '{1}' ist bereits vorhanden.", zauber.Key, rep), "Zauber hinzufügen");
                    RefreshHeld(false);
                    SetZauberAktivierbar();
                }
            }
        }

        private void SetZauberAktivierbar()
        {
            _comboBoxZauber.ItemsSource = Zauber.ZauberList;
            _comboBoxZauber.SelectedIndex = -1;
        }

        private void _comboBoxZauber_DropDownOpened(object sender, EventArgs e)
        {
            SetZauberAktivierbar();
        }

        private void _comboBoxZauber_DropDownClosed(object sender, EventArgs e)
        {
            InsertZauber();
        }

        private void _dataGridHeldZauber_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (sender is System.Windows.Controls.DataGrid && e.Key == Key.Delete)
            {
                var grid = (System.Windows.Controls.DataGrid)sender;

                if (grid.SelectedItems.Count > 0)
                {
                    DatabaseDSADataSet.Held_ZauberRow zauber = SelectedZauberRow;
                    if (MessageBox.Show(string.Format("Soll der Zauber '{0}' entfernt werden?", zauber.ZauberRow.Name), "Zauber entfernen", MessageBoxButton.YesNo,
                                        MessageBoxImage.Question, MessageBoxResult.Yes) != MessageBoxResult.Yes)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void MenuItemZauberLöschen_Click(object sender, RoutedEventArgs e)
        {
            DeleteHeldZauber();
        }

        private void DeleteHeldZauber()
        {
            DatabaseDSADataSet.Held_ZauberRow zauber = SelectedZauberRow;
            if (MessageBox.Show(string.Format("Soll der Zauber '{0}' entfernt werden?", zauber.ZauberRow.Name), "Zauber entfernen", MessageBoxButton.YesNo,
                                        MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                SelectedHeld.DeleteZauber(zauber);
            }
        }

        private void MenuItemZauberWiki_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.wiki-aventurica.de/wiki/" + SelectedZauber.WikiLink);
        }                

        
        
    }
}
