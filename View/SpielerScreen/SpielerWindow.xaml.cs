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
using System.Windows.Shapes;

namespace MeisterGeister.View.SpielerScreen
{
    /// <summary>
    /// Interaktionslogik für SpielerWindow.xaml
    /// </summary>
    public partial class SpielerWindow : Window
    {
        private static SpielerWindow _instance;

        private SpielerWindow()
        {
            InitializeComponent();

            int xPoint = 0, yPoint = 0;
            foreach (System.Windows.Forms.Screen objActualScreen in ScreenList)
            {
                if (!objActualScreen.Primary)
                {
                    xPoint = objActualScreen.Bounds.Location.X + 20;
                    yPoint = objActualScreen.Bounds.Location.Y + 20;
                }
            }
            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = Convert.ToDouble(xPoint);
            Top = Convert.ToDouble(yPoint);
        }

        public static SpielerWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SpielerWindow();
                    if (SpielerWindowInstantiated != null)
                        SpielerWindowInstantiated(_instance, new EventArgs());
                }
                return _instance;
            }
        }

        public static event EventHandler SpielerWindowInstantiated;

        new public static void Show()
        {
            ((Window)Instance).Show();
        }

        new public static void Hide()
        {
            if (_instance != null)
            {
                ((Window)Instance).Hide();
                ClearContent();
            }
        }

        new public static void Close()
        {
            if (_instance != null)
            {
                ((Window)Instance).Close();
                _instance = null;
                if (SpielerWindowClosed != null)
                    SpielerWindowClosed(null, new EventArgs());
            }
        }

        public static event EventHandler SpielerWindowClosed;

        internal static void ReOpen()
        {
            Close();
            Show();
        }

        public static void ClearContent()
        {
            Instance.Content = null;
        }

        public static void SetContent(object info)
        {
            Instance.Content = info;
            Instance.IsKampfInfoModus = (info is Kampf.KampfInfoView);

            Show();
        }

        public static void SetTextFromClipboard()
        {
            RichTextBox txtBlock = new RichTextBox();
            FlowDocument flowDoc = new FlowDocument();

            txtBlock.Document = flowDoc;
            txtBlock.Background = (ImageBrush)App.Current.FindResource("BackgroundPergamentQuer");
            txtBlock.BorderBrush = Brushes.Transparent;
            txtBlock.Margin = new Thickness(40);
            txtBlock.Padding = new Thickness(20);

            txtBlock.Paste();

            SetContent(txtBlock);
        }

        public static void SetImage(string pfad, Stretch stretch = Stretch.Uniform)
        {
            try
            {
                Image img = new Image();
                img.Stretch = stretch;

                BitmapImage bmi = new BitmapImage();
                bmi.BeginInit();
                bmi.UriSource = new Uri(pfad, UriKind.Relative);
                bmi.EndInit();

                bmi.Freeze();		// freeze image source, used to move it across the thread
                img.Source = bmi;

                SetContent(img);
            }
            catch { }
        }

        public static void SetKampfInfoView()
        {
            if (Global.CurrentKampf != null)
            {
                Kampf.KampfInfoView infoView = new Kampf.KampfInfoView(Global.CurrentKampf);
                SetContent(infoView);
            }
        }

        public static void SetBodenplanView()
        {
            if (Global.CurrentKampf != null && Global.CurrentKampf.BodenplanWindow != null)
            {
                System.Windows.Media.VisualBrush vb = new VisualBrush(Global.CurrentKampf.BodenplanWindow);
                Rectangle rect = new Rectangle();
                rect.Fill = vb;

                SetContent(rect);
            }
        }

        List<System.Windows.Forms.Screen> ScreenList = System.Windows.Forms.Screen.AllScreens.ToList();

        public bool IsKampfInfoModus { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ScreenList.Count > 1)
            {
                WindowState = System.Windows.WindowState.Maximized;
                WindowStyle = System.Windows.WindowStyle.None;
            }
            else
            { // bei nur einem Screen nicht im Vollbildmodus starten
                WindowState = System.Windows.WindowState.Normal;
                WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            { // Vollbildmodus verlassen
                WindowState = System.Windows.WindowState.Normal;
                WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _instance = null;
            if (SpielerWindowClosed != null)
                SpielerWindowClosed(null, new EventArgs());
        }
    }
}
