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
using MeisterGeister.ViewModel.Bodenplan;
using System.Windows.Controls.Primitives;
using MeisterGeister.ViewModel.Bodenplan.Logic;
using MeisterGeister.ViewModel.Kampf.Logic;
using MeisterGeister.View.General;
using System.IO;
using MeisterGeister.View.SpielerScreen;

namespace MeisterGeister.View.Bodenplan
{
    /// <summary>
    /// Interaction logic for BattlegroundTopMenuView.xaml
    /// </summary>
    public partial class BattlegroundTopMenuView : UserControl
    {

        private double _x1, _y1, _x2, _y2;
        private double _xMovingOld, _yMovingOld;
        private bool _zoomChanged = false;

        public BattlegroundTopMenuView()
        {
            InitializeComponent();
            VM = new BattlegroundViewModel();
            
            VM.KampfVM = Global.CurrentKampf;
        }

        public BattlegroundViewModel VM
        {
            get { return DataContext as BattlegroundViewModel; }
            set { DataContext = value; }
        }


        private double _visualisationWidth = 100;
        public double VisualisationWidth
        {
            get { return _visualisationWidth; }
            private set { _visualisationWidth = value; }
        }

        private double _visualisationHeight = 100;
        public double VisualisationHeight
        {
            get { return _visualisationHeight; }
            private set { _visualisationHeight = value; }
        }
        
        private void ButtonEbeneHigher_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as BattlegroundViewModel;
            if (vm != null) vm.ChangeEbeneHeight(true);
        }

        private void ButtonEbeneLower_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as BattlegroundViewModel;
            if (vm != null) vm.ChangeEbeneHeight(false);
        }
        
        private void ButtonEbeneHigherMax_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as BattlegroundViewModel;
            if (vm != null) vm.MoveSelectedObjectToTop(true);
        }

        private void ButtonEbeneLowerMin_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as BattlegroundViewModel;
            if (vm != null) vm.MoveSelectedObjectToTop(false);
        }

        private void Button_SaveXML_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Battleground_"+System.DateTime.Now.ToShortDateString(); // Default file name
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Files (.xml)|*.xml";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                var vm = DataContext as BattlegroundViewModel;
                if (vm != null) vm.SaveBattlegroundToXML(dlg.FileName);
            }            
        }

        private void Button_LoadXML_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Files (.xml)|*.xml"; 
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                var vm = DataContext as BattlegroundViewModel;
                if (vm != null)
                {
                    Global.CurrentKampf.Kampf.Kämpfer.Clear();
                    Global.CurrentKampf.BodenplanViewModel.RemoveCreatureAll();
                    vm.LoadBattlegroundFromXML(dlg.FileName);
                    vm.UpdateCreatureLevelToTop();
                }
            }
        }

        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as BattlegroundViewModel;
            if (vm != null) vm.ClearBattleground();
        }

        private void ButtonStickHeroes_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as BattlegroundViewModel;
            if (vm != null && !vm.BattlegroundObjects.Where(x => x is MeisterGeister.Model.Gegner && x.IsSticked).Any()) vm.StickHeroes();
        }

        private void ButtonSticCreatues_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as BattlegroundViewModel;
            if (vm != null) vm.StickEnemies();
        }
        
        public void sl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 1)
                ((Slider)sender).Value += ((((Slider)sender).Value < ((Slider)sender).Maximum - 4) ? 5 : ((((Slider)sender).Maximum - ((Slider)sender).Value)));
            else
                ((Slider)sender).Value = ((((Slider)sender).Value >= ((Slider)sender).Minimum + 4) ? ((Slider)sender).Value -5 : ((Slider)sender).Minimum);
        }

        public void slSmall_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double d = ((Slider)sender).Value;
            double dr = ((Slider)sender).Minimum + .009;
            double min = ((Slider)sender).Minimum;
            if (e.Delta > 1)
                ((Slider)sender).Value = Math.Round(((((Slider)sender).Value + .01 > ((Slider)sender).Maximum) ? ((Slider)sender).Maximum : ((Slider)sender).Value + .01), 2);
            else
                ((Slider)sender).Value = Math.Round(((((Slider)sender).Value - .01 > ((Slider)sender).Minimum) ? ((Slider)sender).Value - .01 : ((Slider)sender).Minimum), 2);
            

        }

        private void tbtnSpielerIniScreen_Click(object sender, RoutedEventArgs e)
        {
            if (((ToggleButton)e.OriginalSource).IsChecked == true)
                CreateKampfWindow();  
            else
                if (VM.KampfWindow != null) VM.KampfWindow.Close();
        }


        private void CreateKampfWindow()
        {
            var vm = DataContext as BattlegroundViewModel;
            Kampf.KampfInfoView infoView = new Kampf.KampfInfoView(Global.CurrentKampf);

            infoView.grdMain.LayoutTransform = new ScaleTransform(vm.ScaleKampfGrid, vm.ScaleKampfGrid);
            VM.KampfWindow = new Window();

            //SizeToContent auf Width setzt den Screen auf minimale Breite
            if (Global.CurrentKampf.Kampf.Kampfrunde <= 1)
                VM.KampfWindow.SizeToContent = SizeToContent.Width;
            else
                VM.KampfWindow.Width = VM.IniWidthStart != 0 ? VM.IniWidthStart : 426 * vm.ScaleKampfGrid;
            
            VM.KampfWindow.Closing += (object sender, System.ComponentModel.CancelEventArgs e) =>
            {
                var vm2 = DataContext as BattlegroundViewModel;
                if (vm2 != null)
                    vm2.IsShowIniKampf = false;
                VM.KampfWindow = null;
            };
            infoView.scrViewer.MouseEnter += (object sender, MouseEventArgs e) => { MouseIsOverScrViewer = true; };
            infoView.scrViewer.MouseLeave += (object sender, MouseEventArgs e) => { MouseIsOverScrViewer = false; };
            //infoView.scrViewer.ScrollChanged += (object sender, ScrollChangedEventArgs e) =>
            //{
            //};
            VM.KampfWindow.SizeChanged += (object sender, SizeChangedEventArgs e) =>
            {

                //VM.KampfWindow.MinWidth = 430 * vm.ScaleKampfGrid;
                VM.SetIniWindowWidth();
                if ((System.Windows.Forms.Screen.AllScreens.Length > 1 &&
                     VM.KampfWindow.Left > System.Windows.Forms.Screen.AllScreens[0].WorkingArea.Width +
                        System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Width * .5) ||
                    (System.Windows.Forms.Screen.AllScreens.Length == 1 &&
                     VM.KampfWindow.Left > System.Windows.Forms.Screen.AllScreens[0].WorkingArea.Width * .5))
                {
                    VM.KampfWindow.Left = (System.Windows.Forms.Screen.AllScreens.Length > 1) ?
                        System.Windows.Forms.Screen.AllScreens[0].WorkingArea.Width +
                        System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Width - VM.KampfWindow.Width +
                        (e.PreviousSize.Width - e.NewSize.Width) :

                        System.Windows.Forms.Screen.AllScreens[0].WorkingArea.Width - VM.KampfWindow.Width;
                    if (System.Windows.Forms.Screen.AllScreens.Length > 1 &&
                        Global.CurrentKampf.BodenplanViewModel.SpielerScreenActive)
                    {
                        Global.CurrentKampf.BodenplanViewModel.SpielerScreenWindow.Width =
                            System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Width - VM.KampfWindow.Width;
                    }
                }
                //int anzInisInKR = Global.CurrentKampf.Kampf.InitiativListe.Aktionszeiten.Where(kr => kr.Kampfrunde == Global.CurrentKampf.Kampf.Kampfrunde).Count();
                //double width1Ini = (infoView.scrViewer.ExtentWidth /
                //    Global.CurrentKampf.Kampf.InitiativListe.Aktionszeiten.Where(kr => kr.Kampfrunde <= Global.CurrentKampf.Kampf.Kampfrunde).Count())* vm.ScaleKampfGrid;
                //VM.KampfWindow.Width = width1Ini * anzInisInKR + 246 * vm.ScaleKampfGrid;                
                //infoView.scrViewer.ScrollToRightEnd();
                
                if (VM.IniWidthStart != Math.Round(VM.KampfWindow.Width / vm.ScaleKampfGrid))
                    VM.IniWidthStart = Math.Round(VM.KampfWindow.Width / vm.ScaleKampfGrid);               
            };
            //246
            if (VM.SpielerScreenWindow != null) VM.SpielerScreenWindow.Topmost = false;
            VM.KampfWindow.Topmost = true;
            VM.KampfWindow.Content = infoView;
            VM.KampfWindow.Show();
            //SizeToContent muss auf Height gestellt werden, damit die Höhe an die Anz. Kämpfer angepasst wird
            VM.KampfWindow.SizeToContent = SizeToContent.Height;

            //SizeToContent muss wieder auf Manual gesetzt werden da das Window sonst immer größer wird
            VM.KampfWindow.SizeToContent = SizeToContent.Manual;
            VM.KampfWindow.WindowStyle = WindowStyle.None;


            //int anzAktInKR = Global.CurrentKampf.Kampf.InitiativListe.Aktionszeiten.Where(kr => kr.Kampfrunde == Global.CurrentKampf.Kampf.Kampfrunde).Count();
            //double Twidth1Ini = (infoView.scrViewer.ExtentWidth /
            //    Global.CurrentKampf.Kampf.InitiativListe.Aktionszeiten.Where(kr => kr.Kampfrunde <= Global.CurrentKampf.Kampf.Kampfrunde).Count()) * vm.ScaleKampfGrid;
            //VM.KampfWindow.MinWidth = 430 * vm.ScaleKampfGrid;
            //VM.KampfWindow.Width = Twidth1Ini * anzAktInKR + 246 * vm.ScaleKampfGrid;
            //infoView.scrViewer.ScrollToRightEnd();
            VM.SetIniWindowPosition();
            
            VM.IsShowIniKampf = true;
        }

        private void tbtnSpielerScreenActive_Click(object sender, RoutedEventArgs e)
        {
            if (((ToggleButton)e.OriginalSource).IsChecked == true)
            {
                if (Global.CurrentKampf != null && Global.CurrentKampf.BodenplanViewModel != null)
                {
                    SpielerWindow.Close();
                    Global.CurrentKampf.BodenplanViewModel.SpielerScreenActive = true;
                    SpielerWindow.Show();
                }
            }
            else
            {
                if (Global.CurrentKampf != null && Global.CurrentKampf.BodenplanViewModel != null)
                {
                    if (VM.SpielerScreenActive) tbtnSpielerScreenActive.RaiseEvent(new RoutedEventArgs(ToggleButton.ClickEvent)); 
                    SpielerWindow.Close();
                    Global.CurrentKampf.BodenplanViewModel.SpielerScreenActive = false;
                }
            }
        }

        private bool MouseIsOverScrViewer = false;        

    }
}

