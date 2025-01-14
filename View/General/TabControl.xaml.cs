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

namespace MeisterGeister.View.General
{
    /// <summary>
    /// Interaktionslogik für TabControl.xaml
    /// </summary>
    public partial class TabItemControl : TabItem
    {
        public string Titel
        {
            get { return _textBlockTitel.Text; }
        }

        public TabItemControl()
        {
            InitializeComponent();

            _contextMenu.Visibility = System.Windows.Visibility.Collapsed;
        }

        public TabItemControl(Control con, string tabName, string bildName)
        {
            InitializeComponent();
            InitializeComponent2(con, tabName, bildName);
        }

        private void InitializeComponent2(Control con, string tabName, string bildName)
        {
            // Bild
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(@"/Images/" + bildName, UriKind.Relative);
            bi.EndInit();
            _image.Source = bi;
            // Titel
            _textBlockTitel.Text = tabName;
            _image.ToolTip = tabName;
            SetToolTitel();

            Logic.Einstellung.Einstellungen.ToolTitelAusblendenChanged += ToolTitelAusblendenChanged;

            Content = con;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            CloseTool();
        }

        private void ButtonCloseAll_Click(object sender, RoutedEventArgs e)
        {
            CloseAllTools();
        }

        private void ButtonCloseOther_Click(object sender, RoutedEventArgs e)
        {
            CloseOtherTools();
        }

        private void CloseTool()
        {
            if (Parent != null && Parent is TabControl)
            {
                ((TabControl)Parent).Items.Remove(this);
                Content = null;
                App.SaveAll();
            }
        }

        private void CloseAllTools()
        {
            if (Parent != null && Parent is TabControl)
            {
                TabControl tabControl = (TabControl)Parent;
                object addTab = tabControl.Items[tabControl.Items.Count - 1];
                tabControl.Items.Clear();
                // Add-Tab ans Ende setzen
                tabControl.Items.Add(addTab);

                App.SaveAll();
            }
        }

        private void CloseOtherTools()
        {
            if (Parent != null && Parent is TabControl)
            {
                TabControl tabControl = (TabControl)Parent;
                object addTab = tabControl.Items[tabControl.Items.Count - 1];
                tabControl.Items.Clear();
                tabControl.Items.Add(this);
                tabControl.SelectedItem = this;
                // Add-Tab ans Ende setzen
                tabControl.Items.Add(addTab);
                
                App.SaveAll();
            }
        }

        private void TabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
            {
                CloseTool();
            }
        }

        private void _checkBoxTitelAusblenden_Checked(object sender, RoutedEventArgs e)
        {
            Logic.Einstellung.Einstellungen.ToolTitelAusblenden = _checkBoxTitelAusblenden.IsChecked;
        }

        private void SetToolTitel()
        {
            if (Logic.Einstellung.Einstellungen.ToolTitelAusblenden)
            {
                _textBlockTitel.Visibility = System.Windows.Visibility.Collapsed;
                _checkBoxTitelAusblenden.IsChecked = true;
            }
            else
            {
                _textBlockTitel.Visibility = System.Windows.Visibility.Visible;
                _checkBoxTitelAusblenden.IsChecked = false;
            }
        }

        private void ToolTitelAusblendenChanged(object sender, EventArgs e)
        {
            SetToolTitel();
        }

    }
}
