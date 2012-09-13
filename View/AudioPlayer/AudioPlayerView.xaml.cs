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
using System.Timers;
using ComboBox = System.Windows.Controls.ComboBox;
using System.Windows.Threading;
// Eigene Usings
using MeisterGeister.Logic.Settings;
using MeisterGeister.Logic.General;
using MeisterGeister.View.General;
using MeisterGeister.View.Windows;
using Global = MeisterGeister.Global;
using MeisterGeister.Model;
using System.Xml;
using System.IO;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
//using Mp3Lib;

/*
Song title  30 Zeichen
Artist	    30 Zeichen
Album	    30 Zeichen
Year	    4 Zeichen
Comment	    30 Zeichen
Genre	    1 Byte

 * */


namespace MeisterGeister.View.AudioPlayer
{
    /// <summary>
    /// Interaktionslogik für AudioPlayerView.xaml
    /// </summary>
    /// 

    delegate void updateBGSongCallback(MediaPlayer _player, string tekst);

    public partial class AudioPlayerView : UserControl
    {
        DispatcherTimer KlangProgBarTimer = new DispatcherTimer();
        DispatcherTimer HintergrundProgBarTimer = new DispatcherTimer();
        private String[] AnzKlangParallel = new String[0];                   // Seite | Zeile | GetHashCode
        private UInt16 tiErstellt = 0;

        private int[] SeiteXHatObjGruppe = new int[8] { 0, -1, -1, -1, -1, -1, -1, -1 };
        private int[][] laufendeSongs = new int[8][];
        private int[][] standbySongs = new int[8][];
        private int[] maxsongsparallel = new int[8];
        private UInt16[] zeilenAufSeite = new UInt16[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        private Audio_Playlist AktHintergrundPlaylist, AktKlangPlaylist;

        private string orgStackString;

        private MediaPlayer[][] _player = new MediaPlayer[8][];
        MediaPlayer HintergrundPlayer;

        System.Timers.Timer BGSongTimer = new System.Timers.Timer();
        DispatcherTimer KlangPlayEndetimer;


        public AudioPlayerView()
        {
            InitializeComponent();

            KlangProgBarTimer.Tick += new EventHandler(KlangProgBarTimer_Tick);
            KlangProgBarTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            KlangProgBarTimer.Tag = 0;

            HintergrundProgBarTimer.Tick += new EventHandler(HintergrundProgBarTimer_Tick);
            HintergrundProgBarTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            _player[0] = new MediaPlayer[1];

            orgStackString = XamlWriter.Save(spnlKlangRow0_X);
            grdKlang0.Children.Remove(spnlKlangRow0_X);
        }

        private void CloseTab(object source, RoutedEventArgs args)
        {
            TabItem tabItem = (TabItem)args.Source;
            if (tabItem != null)
            {
                TabControl tabControl = (TabControl)tabItem.Parent;
                if (tabControl != null)
                    tabControl.Items.Remove(tabItem);
            }
        }


        public string SongInfo(String musikfile)
        {
            String Filename = "";

            if (musikfile != null)
                Filename = musikfile;
            Filename = Filename + DateTime.Now.ToString();
            return Filename;
        }

        /*      private void UpdateBGSong(MediaPlayer _player, string tekst)
              {
                  if (label1.Dispatcher.CheckAccess() == false)
                  {
                      updateBGSongCallback uCallBack = new updateBGSongCallback(UpdateBGSong);
                      this.Dispatcher.Invoke(uCallBack, (tekst));
                  }
                  else
                  {
       

                      if ((_player.NaturalDuration.HasTimeSpan) && lblBgTimeMax.Content == "")
                      {
                          string maxSec = "00";
                          if (_player.NaturalDuration.TimeSpan.Seconds <= 9)
                              maxSec = "0" + _player.NaturalDuration.TimeSpan.Seconds;
                          else
                              maxSec = Convert.ToString(_player.NaturalDuration.TimeSpan.Seconds);
                          lblBgTimeMax.Content = _player.NaturalDuration.TimeSpan.Minutes + ":" + maxSec;

                          pbarBGSong.Maximum = _player.NaturalDuration.TimeSpan.TotalMilliseconds;
                      }

                      pbarBGSong.Value = _player.Position.TotalMilliseconds;
                      string acSec = "00";
                      if (_player.Position.Seconds <= 9)
                          acSec = "0" + _player.Position.Seconds;
                      else
                          acSec = Convert.ToString(_player.Position.Seconds);

                      lblBgTimeActual.Content = _player.Position.Minutes + ":" + acSec;

                      //(this.FindName(tekst) as Label).Content = "Durchgang "; 
                  }
              }*/

        public MediaPlayer PlayFile(int seite, int zeile, MediaPlayer _player, String url, double vol)
        {
            try
            {
                if (_player == null)
                    _player = new MediaPlayer();
                try
                {
                    _player.Open(new Uri(url));
                    if (seite >= 0)
                    {
                        Array.Resize(ref AnzKlangParallel, AnzKlangParallel.Length + 1);
                        AnzKlangParallel[AnzKlangParallel.Length - 1] = seite + "|" + zeile + "|" + _player.GetHashCode();

                        _player.MediaEnded += new EventHandler(Player_Ended);
                    }
                    else
                        _player.MediaEnded += new EventHandler(HintergrundPlayer_Ended);
                }
                catch (Exception ex2)
                {
                    ListBoxItem lbItem = (ListBoxItem)lbhintergrundtitellist.SelectedItem;
                    lbItem.Background = Brushes.Yellow;
                    lbItem.ToolTip = "Datei konnte nicht geöffnet werden (Datei vorhanden?)" + ex2;
                    SpieleNeuenHintergrundTitel();
                    return null;
                }
                finally
                {
                    _player.Volume = vol;
                    _player.Play();
                }

                if (seite >= 0)
                    _player.MediaFailed += new EventHandler<ExceptionEventArgs>(Player_KlangMediaFailed);
                else
                    _player.MediaFailed += new EventHandler<ExceptionEventArgs>(Player_HintergrundMediaFailed);

                return _player;
            }
            catch (Exception ex)
            {
                var errWin = new MsgWindow("Audio Fehler", "Der Audio Player hat einen Fehler verursacht.", ex);
                errWin.ShowDialog();
                errWin.Close();
                return null;
            }
        }

        public void VolChanged(MediaPlayer _player, double vol)
        {
            if (_player != null)
                _player.Volume = vol / 100;
        }

        void CheckPlayStandbySongs(int objGruppe)
        {
            Grid grdKlang = (Grid)this.FindName("grdKlang" + objGruppe);

            int seite = Array.IndexOf(SeiteXHatObjGruppe, objGruppe);

            if ((laufendeSongs[seite] == null && standbySongs[seite] != null) ||
               (laufendeSongs[seite] != null && standbySongs[seite] != null &&
               (maxsongsparallel[seite] > laufendeSongs[seite].Length)))
            {
                uint anzStandbySongs;
                int neueSongs;
                if (laufendeSongs[seite] == null)
                    neueSongs = Convert.ToInt32(maxsongsparallel[seite]); //klangsongparallel
                else
                    neueSongs = Convert.ToInt32(maxsongsparallel[seite]) - laufendeSongs[seite].Length;

                for (int i = 0; i < neueSongs; i++)
                {
                    if (standbySongs[seite] == null)
                        anzStandbySongs = 0;
                    else
                    {
                        anzStandbySongs = Convert.ToUInt32(standbySongs[seite].Length);
                        if (anzStandbySongs >= 1)
                        {
                            Würfel w = new Würfel(anzStandbySongs);
                            w.Würfeln(1);
                            int zuspielendersong = standbySongs[seite][w.Ergebnis - 1];
                            RemoveSongStandby(seite, w.Ergebnis);

                            // Titel anstarten
                            StackPanel spnlZeile = (StackPanel)this.FindName("spnlKlangRow" + objGruppe + "_" + zuspielendersong);
                            if (((CheckBox)spnlZeile.FindName("chkTitel" + objGruppe + "_" + zuspielendersong)) != null)
                                chkTitel0_0_Click((spnlZeile.FindName("chkTitel" + objGruppe + "_" + zuspielendersong)), new RoutedEventArgs());
                        }
                    }
                }
            }
        }

        void KlangPlayEndetimer_Tick(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();

            string s = ((DispatcherTimer)sender).Tag.ToString();
            char[] Separator = new char[] { '_' };
            string[] werte = s.Split(Separator, StringSplitOptions.None);

            UInt16 seite = Convert.ToUInt16(werte[0]);
            UInt16 zeile = Convert.ToUInt16(werte[1]);
            UInt16 objGruppe = Convert.ToUInt16(SeiteXHatObjGruppe[seite]);

            // Player löschen, auf Ursprung zurück
            _player[seite][zeile] = null;
            ProgressBar prog = (ProgressBar)this.FindName("pbarTitel" + s);
            if (prog == null)
                prog = (ProgressBar)((StackPanel)this.FindName("spnlKlangRow" + objGruppe + "_" + zeile)).FindName("pbarTitel" + s);

            if (prog != null)
                prog.Value = 0;

            StackPanel spnl = ((StackPanel)this.FindName("spnlKlangRow" + objGruppe + "_" + zeile));
            if (((CheckBox)spnl.FindName("chkKlangPauseMove" + objGruppe + "_" + zeile)).IsChecked == true)
            {
                UInt16 v = Convert.ToUInt16(Convert.ToUInt16(((TextBox)spnl.FindName("tboxPauseMax" + objGruppe + "_" + zeile)).Text) -
                                            Convert.ToUInt16((((TextBox)spnl.FindName("tboxPauseMin" + objGruppe + "_" + zeile))).Text));
                Würfel w = new Würfel(v);
                w.Würfeln(1);
                ((Slider)spnl.FindName("sldKlangPause" + objGruppe + "_" + zeile)).Value = w.Ergebnis;
            }
            // Song aus der Liste der laufenden Songs herausnehmen
            RemoveSongLaufend(seite, zeile);

            // Song in die Liste der Standby-Songs aufnehmen
            AddSongStandby(seite, zeile);

            CheckPlayStandbySongs(objGruppe);
        }

        void AddSongStandby(UInt16 seite, UInt16 zeile)
        {
            if (standbySongs[seite] == null)
                Array.Resize(ref standbySongs[seite], 1);
            else
                Array.Resize(ref standbySongs[seite], standbySongs[seite].Length + 1);
            standbySongs[seite][standbySongs[seite].Length - 1] = zeile;
        }

        void RemoveSongStandby(int seite, int zeile)
        {
            for (int x = zeile - 1; x < standbySongs[seite].Length - 1; x++)
                standbySongs[seite][x] = standbySongs[seite][x + 1];
            Array.Resize(ref standbySongs[seite], standbySongs[seite].Length - 1);
            if (standbySongs[seite].Length == 0)
                standbySongs[seite] = null;
        }

        void RemoveSongLaufend(UInt16 seite, UInt16 zeile)
        {
            int r = Array.IndexOf(laufendeSongs[seite], zeile);

            for (int x = r; x < laufendeSongs[seite].Length - 1; x++)
                laufendeSongs[seite][x] = laufendeSongs[seite][x + 1];
            if (laufendeSongs[seite].Length > 0)
                Array.Resize(ref laufendeSongs[seite], laufendeSongs[seite].Length - 1);
        }

        void RemoveSongParallel(int seite, int zeile)
        {
            char[] Separator = new char[] { '|' };
            for (int i = 0; i <= AnzKlangParallel.Length - 1; i++)
            {
                string[] s = AnzKlangParallel[i].Split(Separator, StringSplitOptions.None);

                if (seite.ToString() == s[0] && s[1] == zeile.ToString())
                {
                    for (int x = i; x < AnzKlangParallel.Length - 1; x++)
                        AnzKlangParallel[x] = AnzKlangParallel[x + 1];
                    Array.Resize(ref AnzKlangParallel, AnzKlangParallel.Length - 1);
                    break;
                }
            }
        }

        void HintergrundPlayer_Ended(object sender, EventArgs e)
        {
            ((MediaPlayer)sender).Stop();
            HintergrundProgBarTimer.Stop();
            SpieleNeuenHintergrundTitel();
        }

        void Player_HintergrundMediaFailed(object sender, ExceptionEventArgs e)
        {
            ((MediaPlayer)sender).Stop();
            HintergrundProgBarTimer.Stop();
            ListBoxItem lbItem = (ListBoxItem)lbhintergrundtitellist.SelectedItem;
            lbItem.Background = Brushes.Yellow;
            lbItem.ToolTip = "Datei kann nicht abgespielt werden. Falscher oder nicht kompatibler Typ (" + ((MediaPlayer)sender).Source.LocalPath + ")";
            SpieleNeuenHintergrundTitel();
        }

        void Player_Ended(object sender, EventArgs e)
        {
            Grid grdKlang;
            // MediaPlayerbezug finden
            for (int x = 0; x < AnzKlangParallel.Length; x++)
            {
                char[] Separator = new char[] { '|' };
                string[] s = AnzKlangParallel[x].Split(Separator, StringSplitOptions.None);
                if (s[2] == (((MediaPlayer)sender).GetHashCode()).ToString())
                {
                    int objGruppe = SeiteXHatObjGruppe[Convert.ToUInt16(s[0])];
                    grdKlang = (Grid)this.FindName("grdKlang" + objGruppe);
                    Slider sld = (Slider)this.FindName("sldKlangPause" + objGruppe + "_" + s[1]);
                    if (sld == null)
                        sld = (Slider)((StackPanel)this.FindName("spnlKlangRow" + objGruppe + "_" + s[1])).FindName("sldKlangPause" + objGruppe + "_" + s[1]);


                    KlangPlayEndetimer = new DispatcherTimer();
                    KlangPlayEndetimer.Interval = TimeSpan.FromMilliseconds(sld.Value);
                    KlangPlayEndetimer.Tick += new EventHandler(KlangPlayEndetimer_Tick);
                    KlangPlayEndetimer.Tag = s[0] + "_" + s[1];   // objGruppe + Zeile
                    KlangPlayEndetimer.Start();
                    break;
                }
            }

            App.CloseSplashScreen();
        }


        void Player_KlangMediaFailed(object sender, ExceptionEventArgs e)
        {
            char[] Separator = new char[] { '|' };
            int mediahash = ((MediaPlayer)sender).GetHashCode();

            string[] s;
            for (int i = 0; i <= AnzKlangParallel.Length - 1; i++)
            {
                s = AnzKlangParallel[i].Split(Separator, StringSplitOptions.None);
                if (mediahash == Convert.ToInt32(s[2]))
                {
                    UInt16 zeile = Convert.ToUInt16(s[1]);
                    UInt16 seite = Convert.ToUInt16(s[0]);
                    UInt16 objGruppe = Convert.ToUInt16(SeiteXHatObjGruppe[seite]);

                    Grid grdKlang = (Grid)this.FindName("grdKlang" + objGruppe);
                    StackPanel spnlZeile = (StackPanel)this.FindName("spnlKlangRow" + objGruppe + "_" + zeile);
                    spnlZeile.Background = Brushes.Yellow;
                    spnlZeile.ToolTip = "Datei kann nicht abgespielt werden (Falscher oder nicht kompatibler Typ).";
                    _player[seite][zeile].Stop();
                    _player[seite][zeile] = null;
                    RemoveSongLaufend(seite, zeile);
                    RemoveSongParallel(seite, zeile);
                    CheckPlayStandbySongs(objGruppe);
                }
            }
        }

        private void KlangProgBarTimer_Tick(object sender, EventArgs e)
        {
            if (KlangProgBarTimer.Tag.ToString() == "0")
                KlangProgBarTimer.Tag = "1";
            else
                KlangProgBarTimer.Tag = "0";
            Grid grdKlang;
            for (int i = 0; i + 1 <= AnzKlangParallel.Length; i++)
            {
                char[] Separator = new char[] { '|' };
                string[] s = AnzKlangParallel[i].Split(Separator, StringSplitOptions.None);
                UInt16 zeile = Convert.ToUInt16(s[1]);
                UInt16 seite = Convert.ToUInt16(s[0]);
                UInt16 objGruppe = Convert.ToUInt16(SeiteXHatObjGruppe[seite]);

                grdKlang = (Grid)this.FindName("grdKlang" + objGruppe);

                StackPanel spnlZeile = (StackPanel)this.FindName("spnlKlangRow" + objGruppe + "_" + zeile);
                if (spnlZeile != null && spnlZeile.Tag != null)
                {
                    DateTime dtJetzt = DateTime.Now;
                    DateTime dtVorher = Convert.ToDateTime(spnlZeile.Tag);

                    TimeSpan diffResult = dtJetzt - dtVorher;
                    if (diffResult.TotalSeconds > 1)
                    {
                        if (!_player[seite][zeile].HasAudio)
                        {
                            spnlZeile.Background = Brushes.Yellow;
                            spnlZeile.ToolTip = "Datei kann nicht abgespielt werden";
                            _player[seite][zeile].Stop();
                            _player[seite][zeile] = null;
                            RemoveSongLaufend(seite, zeile);
                            RemoveSongParallel(seite, zeile);
                            CheckPlayStandbySongs(objGruppe);
                        }
                        else
                            spnlZeile.Background = null;
                    }
                }
                if (_player[seite] != null && _player[seite][zeile] != null && _player[seite][zeile].HasAudio == false && 
                    _player[seite][zeile].BufferingProgress == 1)
                    spnlZeile.Tag = DateTime.Now;
                else
                    spnlZeile.Tag = null;

                if (KlangProgBarTimer.Tag.ToString() == "0")
                {
                    CheckBox ch = (CheckBox)spnlZeile.FindName("chkTitel" + objGruppe + "_" + zeile);
                    ProgressBar prog = (ProgressBar)spnlZeile.FindName("pbarTitel" + objGruppe + "_" + zeile);
                    Slider sldVol = (Slider)spnlZeile.FindName("sldKlangVol" + objGruppe + "_" + zeile);

                    if (ch.IsChecked == true && prog != null && _player[seite][zeile] != null)
                    {
                        CheckBox chAkt = (CheckBox)spnlZeile.FindName("chkVolMove" + objGruppe + "_" + zeile);
                        if (chAkt.IsChecked == true)
                        {
                            TextBox tboxMin = (TextBox)spnlZeile.FindName("tboxVolMin" + objGruppe + "_" + zeile);
                            TextBox tboxMax = (TextBox)spnlZeile.FindName("tboxVolMax" + objGruppe + "_" + zeile);
                            uint v = 5;
                            if (sldVol.Value >= Convert.ToUInt32(tboxMax.Text) || sldVol.Value <= Convert.ToUInt32(tboxMin.Text))
                                v = 3;
                            Würfel w = new Würfel(v);
                            w.Würfeln(1);
                            if (sldVol.Value <= Convert.ToUInt32(tboxMin.Text))
                                sldVol.Value = sldVol.Value + (w.Ergebnis - 1) * 2;
                            else
                                sldVol.Value = sldVol.Value + (w.Ergebnis - 3) * 2;
                        }
                        if (prog.Maximum == 10000 && _player[seite][zeile].NaturalDuration.HasTimeSpan)
                            prog.Maximum = _player[seite][zeile].NaturalDuration.TimeSpan.TotalMilliseconds;
                        Label lbl = (Label)spnlZeile.FindName("lblDauer" + objGruppe + "_" + zeile);
                        if (lbl.Content.ToString() == "-:--" && _player[seite][zeile].NaturalDuration.HasTimeSpan)
                            lbl.Content = _player[seite][zeile].NaturalDuration.TimeSpan.ToString().Substring(3, 5);

                        prog.Value = _player[seite][zeile].Position.TotalMilliseconds;
                    }
                }
            }
        }

        private void slBGVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VolChanged(HintergrundPlayer, ((Slider)sender).Value);
        }


        private void lbBackground_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            grdSongInfo.Visibility = Visibility.Hidden;
            if (lbBackground.SelectedItems.Count != 0)
            {
                try
                {
                    if (HintergrundProgBarTimer != null)
                    {
                        HintergrundProgBarTimer.Stop();
                        if (HintergrundPlayer != null)
                        {
                            HintergrundPlayer.Stop();
                            HintergrundPlayer = null;
                        }
                    }

                    List<Audio_Playlist> playlistliste = Global.ContextAudio.PlaylistListe.Where(t => t.Audio_PlaylistGUID.Equals(((ListBoxItem)lbBackground.SelectedItem).Tag)).ToList();
                    if (playlistliste != null)
                    {
                        AktHintergrundPlaylist = playlistliste[0];
                        List<Audio_Titel> titel = Global.ContextAudio.LoadTitelByPlaylist(playlistliste[0]).ToList();
                        lbhintergrundtitellist.Items.Clear();
                        for (int i = 0; i < titel.Count; i++)
                        {
                            ListBoxItem lbitem = new ListBoxItem();
                            lbitem.Name = "titel" + i;
                            lbitem.Tag = titel[i].Audio_TitelGUID;
                            lbitem.Content = titel[i].Name;
                            lbhintergrundtitellist.Items.Add(lbitem);
                        }
                        SpieleNeuenHintergrundTitel();
                        if (titel.Count != 0)
                            grdSongInfo.Visibility = Visibility.Visible;
                        else
                        {
                            btnBGAbspielen.Tag = 1;
                            btnBGAbspielen_Click(btnBGAbspielen, new RoutedEventArgs());
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errWin = new MsgWindow("Playlist Fehler", "Die Playliste konnte nicht geöffnet werden", ex);
                    errWin.ShowDialog();
                    errWin.Close();
                }
            }
            else
                btnBGAbspielen.IsEnabled = false;
        }

        private void HintergrundSongInfo(Visibility sichtbar)
        {
            for (int i = 0; i <= grdSongInfo.Children.Count - 1; i++)
                grdSongInfo.Children[i].Visibility = sichtbar;
        }


        private void SpieleNeuenHintergrundTitel()
        {
            if (lbhintergrundtitellist.Items.Count == 1)
                lbhintergrundtitellist.SelectedIndex = 0;
            else
            {
                int[] titelmoeglich = new int[0];
                for (int i = 0; i <= lbhintergrundtitellist.Items.Count - 1; i++)
                {
                    ListBoxItem lbitem = (ListBoxItem)lbhintergrundtitellist.Items[i];
                    if (lbitem.Background != Brushes.Yellow && lbitem.Background != Brushes.Red) 
                    {
                        Array.Resize(ref titelmoeglich, titelmoeglich.Length + 1);
                        titelmoeglich[titelmoeglich.Length - 1] = i;
                    }
                }
                if (titelmoeglich.Length > 0)
                {
                    Würfel w = new Würfel(Convert.ToUInt32(titelmoeglich.Length));
                    w.Würfeln(1);

                    while (titelmoeglich[w.Ergebnis - 1] == lbhintergrundtitellist.SelectedIndex)
                        w.Würfeln(1);
                    lbhintergrundtitellist.SelectedIndex = titelmoeglich[w.Ergebnis - 1];
                    lbhintergrundtitellist.ScrollIntoView(lbhintergrundtitellist.SelectedItem);
                }
            }
        }

        private void btnBGAbspielen_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(btnBGAbspielen.Tag) == 0)
            {
                SpieleNeuenHintergrundTitel();
            }
            else
            {
                if (HintergrundPlayer != null)
                {
                    HintergrundPlayer.Stop();
                    HintergrundPlayer = null;
                }
                HintergrundProgBarTimer.Stop();
                btnBGAbspielen.Tag = 0;

                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri("pack://application:,,,/DSA MeisterGeister;component/Images/Icons/General/play.png");
                logo.EndInit();
                btnImgBGAbspielen.Source = logo;

                btnLblBGAbspielen.Content = "Abspielen";
                grdSongInfo.Visibility = Visibility.Hidden;
                btnBGNext.IsEnabled = false;
                lbhintergrundtitellist.SelectedIndex = -1;
            }
        }

        private void lbBackground_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            lbBackground_SelectionChanged(sender, null);
        }

        private void btnBGNext_Click(object sender, RoutedEventArgs e)
        {
            SpieleNeuenHintergrundTitel();
        }

        private void lbKlang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbKlang.SelectedIndex != -1)
            {
                btnPlaylistLoeschen.IsEnabled = true;
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    UInt16 objGruppe;
                    if (tcKlang.SelectedIndex == 0)
                        objGruppe = Convert.ToUInt16(((TabItem)tcKlang.SelectedItem).Name.Substring(7));
                    else
                        objGruppe = Convert.ToUInt16(((TabItemControl)tcKlang.SelectedItem).Name.Substring(7));

                    for (int i = 0; i <= grdKlangPlaylistInfo.Children.Count - 1; i++)
                        grdKlangPlaylistInfo.Children[i].Visibility = Visibility.Visible;

                    string s = ((ListBoxItem)lbKlang.Items[lbKlang.SelectedIndex]).Content.ToString();

                    ((Grid)this.FindName("grdKlang" + objGruppe)).Visibility = Visibility.Hidden;
                    List<Audio_Playlist> playlistliste = Global.ContextAudio.PlaylistListe.Where(t => t.Name.Equals(s)).ToList();
                    if (playlistliste.Count == 1)
                    {
                        List<Audio_Titel> titelliste = Global.ContextAudio.LoadTitelByPlaylist(playlistliste[0]);
                        PlaylisteLeeren(objGruppe);
                        AktKlangPlaylist = playlistliste[0];

                        if (AktKlangPlaylist.Hintergrundmusik)
                            rbIstMusikPlaylist.IsChecked = true;
                        else
                            rbIstKlangPlaylist.IsChecked = true;

                        tboxPlaylistName.Text = AktKlangPlaylist.Name;
                        if (tcKlang.SelectedIndex == 0)
                            ((TabItem)tcKlang.SelectedItem).Header = AktKlangPlaylist.Name;
                        else
                            ((TabItemControl)tcKlang.SelectedItem)._textBlockTitel.Text = AktKlangPlaylist.Name;

                        if (titelliste.Count > 0)
                        {
                            int seite = Array.IndexOf(SeiteXHatObjGruppe, objGruppe);

                            tboxklangsongparallel.TextChanged -= new TextChangedEventHandler(tboxklangsongparallel_TextChanged);
                            tboxklangsongparallel.Tag = AktKlangPlaylist.Audio_Playlist_Titel.Count;
                            tboxklangsongparallel.Text = "0";
                            maxsongsparallel[seite] = 0;
                            tboxklangsongparallel.TextChanged += new TextChangedEventHandler(tboxklangsongparallel_TextChanged);

                            for (UInt16 x = 0; x <= AktKlangPlaylist.Audio_Playlist_Titel.Count - 1; x++)
                            {
                                Audio_Playlist_Titel playlisttitel = Global.ContextAudio.LoadPlaylist_TitelByPlaylist(AktKlangPlaylist, titelliste[x])[0];
                                KlangNewRow(playlisttitel.Audio_Titel.Pfad, objGruppe, x, playlisttitel);

                                if (AktKlangPlaylist.Hintergrundmusik)
                                    ZeigeKlangSettings(objGruppe, x, false);
                            }

                            ((Grid)this.FindName("grdKlang" + objGruppe)).Visibility = Visibility.Visible;
                            if (AktKlangPlaylist.Hintergrundmusik)
                            {
                                ZeigeKlangSongsParallel(false);
                                ZeigeKlangTop(objGruppe, false);
                            }
                            else
                                ZeigeKlangSongsParallel(true);

                            tboxklangsongparallel.Text = AktKlangPlaylist.MaxSongsParallel.ToString();
                            maxsongsparallel[seite] = AktKlangPlaylist.MaxSongsParallel;
                            zeilenAufSeite[seite] = Convert.ToUInt16(AktKlangPlaylist.Audio_Playlist_Titel.Count);
                        }

                        ((Grid)this.FindName("grdKlangTop" + objGruppe)).Visibility = Visibility.Visible;
                        CheckPlayStandbySongs(objGruppe);
                    }
                    else
                    {
                        AktKlangPlaylist = null;   
                        var errWin = new MsgWindow("Datenfehler", "Die Playlist-Liste konnte nicht eindeutig in der Datenbank detektiert werden.", null);
                        errWin.ShowDialog();
                        errWin.Close();

                        for (int i = 0; i <= grdKlangPlaylistInfo.Children.Count - 1; i++)
                            grdKlangPlaylistInfo.Children[i].Visibility = Visibility.Hidden;
                        ((Grid)this.FindName("grdKlangTop" + objGruppe)).Visibility = Visibility.Hidden;
                    }
                }
                finally
                {                 
                    Mouse.OverrideCursor = null;
                }
            }
            else
                btnPlaylistLoeschen.IsEnabled = false;
        }

        private void AlleKlangSongsAus(UInt16 objGruppe, bool checkboxAus, bool ZeileLoeschen)
        {
            int seite = Array.IndexOf(SeiteXHatObjGruppe, objGruppe);

            Grid grdKlang = (Grid)this.FindName("grdKlang" + objGruppe);
            
            Array.Resize(ref standbySongs[seite], 0);

            for (UInt16 i = 0; i < zeilenAufSeite[seite]; i++)
            {
                StackPanel StackZeile = (StackPanel)this.FindName("spnlKlangRow" + objGruppe + "_" + i);
                if (((CheckBox)StackZeile.FindName("chkTitel" + objGruppe + "_" + i)).IsChecked.Value == true)
                {
                    if (checkboxAus)
                    {
                        ((CheckBox)StackZeile.FindName("chkTitel" + objGruppe + "_" + i)).Click -= new RoutedEventHandler(chkTitel0_0_Click);
                        ((CheckBox)StackZeile.FindName("chkTitel" + objGruppe + "_" + i)).IsChecked = false;
                    }
                    if (_player[seite] != null && _player[seite][i] != null)
                    {
                        _player[seite][i].MediaEnded -= new EventHandler(Player_Ended); ;
                        _player[seite][i].Stop();
                        _player[seite][i] = null;
                    }
                    if (laufendeSongs[seite] != null && laufendeSongs[seite].Contains(i))
                        RemoveSongLaufend(Convert.ToUInt16(seite), i);

                    ((ProgressBar)StackZeile.FindName("pbarTitel" + objGruppe + "_" + i)).Maximum = 100;
                    ((ProgressBar)StackZeile.FindName("pbarTitel" + objGruppe + "_" + i)).Value = 0;
                    RemoveSongParallel(seite, i);
                }
                if (ZeileLoeschen)
                {
                    ((Grid)StackZeile.FindName("grdKlangRow" + objGruppe + "_" + i)).Children.Remove((Image)StackZeile.FindName("imgTrash" + objGruppe + "_" + i));

                    grdKlang.Children.Remove(StackZeile);
                    if (seite == 0)
                        grdKlang.UnregisterName(StackZeile.Name);
                    else
                        this.UnregisterName(StackZeile.Name);
                }
            }
        }

        private void PlaylisteLeeren(UInt16 objGruppe)
        {
            if (AktKlangPlaylist != null)
            {
                int seite = Array.IndexOf(SeiteXHatObjGruppe, objGruppe);

                Grid grdKlang = (Grid)this.FindName("grdKlang" + objGruppe);

                AlleKlangSongsAus(objGruppe, true, true);

                ZeigeKlangSongsParallel(false);

                if (grdKlang != null)
                    grdKlang.RowDefinitions.RemoveRange(1, grdKlang.RowDefinitions.Count - 2);
                zeilenAufSeite[seite] = 0;
            }
        }

        public UIElement DeepCopy(UIElement element, string oldValue, string newValue)
        {
            string shapestring;
            if (element == null)
                shapestring = orgStackString.Replace(oldValue, newValue);
            else
            {
                shapestring = XamlWriter.Save(element);
                if (oldValue != null)
                    shapestring = shapestring.Replace(oldValue, newValue);
            }

            StringReader stringReader = new StringReader(shapestring);
            XmlReader xmlTextReader = new XmlTextReader(stringReader);
            UIElement DeepCopyobject = (UIElement)XamlReader.Load(xmlTextReader);
            return DeepCopyobject;
        }

        private void ZeigeKlangTop(UInt16 objGruppe, bool sichtbar)
        {
            Grid grdKlang = (Grid)this.FindName("grdKlang" + objGruppe);
            Grid grdKlangTop = (Grid)grdKlang.FindName("grdKlangTop" + objGruppe);
            if (sichtbar)
            {
                for (int i = grdKlangTop.ColumnDefinitions.Count - 1; i >= grdKlangTop.ColumnDefinitions.Count - 9; i--)
                    grdKlangTop.ColumnDefinitions[i].Width = grdKlangTopX.ColumnDefinitions[i].Width;
            }
            else
            {
                for (int i = 12; i >= 4; i--)
                    grdKlangTop.ColumnDefinitions[i].Width = new GridLength(0);
            }
        }

        private void ZeigeKlangSongsParallel(bool sichtbar)
        {
            if (sichtbar)
            {
                lblKlangSongsParallel.Visibility = Visibility.Visible;
                tboxklangsongparallel.Visibility = Visibility.Visible;
                btnSongParMinus.Visibility = Visibility.Visible;
                btnSongParPlus.Visibility = Visibility.Visible;
            }
            else
            {
                lblKlangSongsParallel.Visibility = Visibility.Hidden;
                tboxklangsongparallel.Visibility = Visibility.Hidden;
                btnSongParMinus.Visibility = Visibility.Hidden;
                btnSongParPlus.Visibility = Visibility.Hidden;
            }
        }

        private void ZeigeKlangSettings(UInt16 objGruppe, UInt16 row, bool sichtbar)
        {
            StackPanel spnlZeile = (StackPanel)this.FindName("spnlKlangRow" + objGruppe + "_" + row);
            if (spnlZeile != null)
            {
                Grid grdKlangRow = (Grid)spnlZeile.FindName("grdKlangRow" + objGruppe + "_" + row);

                if (!sichtbar)
                {
                    for (int i = 12; i >= 4; i--)
                        grdKlangRow.ColumnDefinitions[i].Width = new GridLength(0);
                }
                else
                {
                    for (int i = 12; i >= 4; i--)
                        grdKlangRow.ColumnDefinitions[i].Width = grdKlangRow0_X.ColumnDefinitions[i].Width;
                }
            }
        }

        private void KlangNewRow(string songdatei, UInt16 objGruppe, UInt16 row, Audio_Playlist_Titel playlisttitel)
        {
            StackPanel newStack = (StackPanel)DeepCopy(null, "0_X", objGruppe + "_" + row); //spnlKlangRow0_X

            newStack.Visibility = Visibility.Visible;
            //newStack.Name = "spnlKlangRow" + objGruppe + "_" + row;
            Grid grdKlang = (Grid)this.FindName("grdKlang" + objGruppe);

            grdKlang.Children.Add(newStack);
            Grid.SetRow(newStack, grdKlang.RowDefinitions.Count - 1);

            this.RegisterName(newStack.Name, newStack);
            //Papierkorb
            Image imgTrash = (Image)newStack.FindName("imgTrash" + objGruppe + "_" + row);
            imgTrash.Tag = playlisttitel.Audio_Titel.Audio_TitelGUID;
            imgTrash.MouseUp += new MouseButtonEventHandler(imgTrash0_0_MouseUp);

            //Titel
            CheckBox chkTitel = (CheckBox)newStack.FindName("chkTitel" + objGruppe + "_" + row);
            chkTitel.Click += new RoutedEventHandler(chkTitel0_0_Click);
            chkTitel.Content = System.IO.Path.GetFileNameWithoutExtension(songdatei);
            if (playlisttitel.Aktiv)
            {
                chkTitel.IsChecked = playlisttitel.Aktiv;
                AddSongStandby(Convert.ToUInt16(Array.IndexOf(SeiteXHatObjGruppe, objGruppe)), row);
            }
            chkTitel.ToolTip = songdatei;
            chkTitel.Tag = songdatei;

            // Schieberegler Lautstärke
            Slider sldVol = (Slider)newStack.FindName("sldKlangVol" + objGruppe + "_" + row);
            sldVol.Minimum = 0; // playlisttitel.VolumeMin;
            sldVol.Maximum = 100; // playlisttitel.VolumeMax;
            sldVol.Value = playlisttitel.Volume;
            sldVol.Tag = row;
            sldVol.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sldKlangVol0_X_ValueChanged);
            sldVol.ToolTip = Math.Round(sldVol.Value) + " %";

            //Checkbox Automatisch veränderbare Lautstärke
            CheckBox chkVolMove = (CheckBox)newStack.FindName("chkVolMove" + objGruppe + "_" + row);
            chkVolMove.IsChecked = playlisttitel.VolumeChange;
            chkVolMove.Tag = row;
            chkVolMove.Click += new RoutedEventHandler(chkVolMove0_0_Click);

            // Volume Minimum Plus/Minus (für ein Song)
            Button btnVolMinMinus = (Button)newStack.FindName("_btnVolMinMinus" + objGruppe + "_" + row);
            btnVolMinMinus.Click += new RoutedEventHandler(_btnVolMinMinus0_X_Click);
            Button btnVolMinPlus = (Button)newStack.FindName("_btnVolMinPlus" + objGruppe + "_" + row);
            btnVolMinPlus.Click += new RoutedEventHandler(_btnVolMinMinus0_X_Click);

            TextBox tboxVolMin = (TextBox)newStack.FindName("tboxVolMin" + objGruppe + "_" + row);
            tboxVolMin.Text = Convert.ToString(playlisttitel.VolumeMin);
            tboxVolMin.Tag = row;
            tboxVolMin.PreviewTextInput += new TextCompositionEventHandler(tboxVolMin0_X_PreviewTextInput);
            tboxVolMin.TextChanged += new TextChangedEventHandler(tboxVolMin0_X_TextChanged);

            // Volume Maximum Plus/Minus (für ein Song)
            Button btnVolMaxMinus = (Button)newStack.FindName("_btnVolMaxMinus" + objGruppe + "_" + row);
            btnVolMaxMinus.Click += new RoutedEventHandler(_btnVolMinMinus0_X_Click);
            Button btnVolMaxPlus = (Button)newStack.FindName("_btnVolMaxPlus" + objGruppe + "_" + row);
            btnVolMaxPlus.Click += new RoutedEventHandler(_btnVolMinMinus0_X_Click);

            TextBox tboxVolMax = (TextBox)newStack.FindName("tboxVolMax" + objGruppe + "_" + row);
            tboxVolMax.Text = Convert.ToString(playlisttitel.VolumeMax);
            tboxVolMax.Tag = row;
            tboxVolMax.PreviewTextInput += new TextCompositionEventHandler(tboxVolMin0_X_PreviewTextInput);
            tboxVolMax.TextChanged += new TextChangedEventHandler(tboxVolMin0_X_TextChanged);

            // Schieberegler Zwischenpause
            Slider sldKlangPause = (Slider)newStack.FindName("sldKlangPause" + objGruppe + "_" + row);
            sldKlangPause.Minimum = 0; // playlisttitel.PauseMin;
            sldKlangPause.Maximum = 10000; // playlisttitel.PauseMax;
            sldKlangPause.Value = playlisttitel.Pause;
            sldKlangPause.TickFrequency = 10;
            sldKlangPause.Tag = row;
            sldKlangPause.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sldKlangVol0_X_ValueChanged);
            sldKlangPause.ToolTip = Math.Round(sldKlangPause.Value) + " ms";

            // Checkbox veränderbare Zwischenpause
            CheckBox chkKlangPauseMove = (CheckBox)newStack.FindName("chkKlangPauseMove" + objGruppe + "_" + row);
            chkKlangPauseMove.IsChecked = playlisttitel.PauseChange;
            chkKlangPauseMove.Tag = row;
            chkKlangPauseMove.Click += new RoutedEventHandler(chkKlangPauseMove0_0_Click);

            // Zwischenpause Minimum Plus/Minus (für ein Song)
            Button btnPauseMinMinus = (Button)newStack.FindName("_btnPauseMinMinus" + objGruppe + "_" + row);
            btnPauseMinMinus.Click += new RoutedEventHandler(_btnVolMinMinus0_X_Click);
            Button btnPauseMinPlus = (Button)newStack.FindName("_btnPauseMinPlus" + objGruppe + "_" + row);
            btnPauseMinPlus.Click += new RoutedEventHandler(_btnVolMinMinus0_X_Click);

            TextBox tboxPauseMin = (TextBox)newStack.FindName("tboxPauseMin" + objGruppe + "_" + row);
            tboxPauseMin.Text = Convert.ToString(playlisttitel.PauseMin);
            tboxPauseMin.Tag = row;
            tboxPauseMin.PreviewTextInput += new TextCompositionEventHandler(tboxVolMin0_X_PreviewTextInput);
            tboxPauseMin.TextChanged += new TextChangedEventHandler(tboxVolMin0_X_TextChanged);

            // Zwischenpause Maximum Plus/Minus (für ein Song)
            Button btnPauseMaxMinus = (Button)newStack.FindName("_btnPauseMaxMinus" + objGruppe + "_" + row);
            btnPauseMaxMinus.Click += new RoutedEventHandler(_btnVolMinMinus0_X_Click);
            Button btnPauseMaxPlus = (Button)newStack.FindName("_btnPauseMaxPlus" + objGruppe + "_" + row);
            btnPauseMaxPlus.Click += new RoutedEventHandler(_btnVolMinMinus0_X_Click);

            TextBox tboxPauseMax = (TextBox)newStack.FindName("tboxPauseMax" + objGruppe + "_" + row);
            tboxPauseMax.Text = Convert.ToString(playlisttitel.PauseMax);
            tboxPauseMax.Tag = row;
            tboxPauseMax.PreviewTextInput += new TextCompositionEventHandler(tboxVolMin0_X_PreviewTextInput);
            tboxPauseMax.TextChanged += new TextChangedEventHandler(tboxVolMin0_X_TextChanged);


            RowDefinition rowDef1 = new RowDefinition();
            rowDef1.Height = grdKlang.RowDefinitions[1].Height;
            grdKlang.RowDefinitions.Insert(grdKlang.RowDefinitions.Count, rowDef1);

            if (_player[tcKlang.SelectedIndex] == null)
                _player[tcKlang.SelectedIndex] = new MediaPlayer[1];
            else
            {
                if (_player[tcKlang.SelectedIndex].Length < row + 1)
                    Array.Resize(ref _player[tcKlang.SelectedIndex], _player[tcKlang.SelectedIndex].Length + 1);
            }
        }

        private void chkTitel0_0_Click(object sender, RoutedEventArgs e)
        {
            string s = ((CheckBox)sender).Name.Substring(8);

            char[] Separator = new char[] { '_' };
            string[] werte = s.Split(Separator, StringSplitOptions.None);

            UInt16 objGruppe = Convert.ToUInt16(werte[0]);
            UInt16 zeile = Convert.ToUInt16(werte[1]);
            UInt16 seite = Convert.ToUInt16(Array.IndexOf(SeiteXHatObjGruppe, objGruppe));

            string file = ((CheckBox)sender).Tag.ToString();
            StackPanel spnlZeile = (StackPanel)((Grid)((CheckBox)sender).Parent).Parent;

            if (File.Exists(file) != true)
            {
                spnlZeile.Background = Brushes.Red;
                spnlZeile.ToolTip = "Datei nicht gefunden";
                RemoveSongParallel(seite, zeile);
                CheckPlayStandbySongs(objGruppe);
            }
            else
            {
                spnlZeile.Background = null;
                if (((CheckBox)sender).IsChecked.Value == true)
                {
                    if ((laufendeSongs[seite] == null) ||
                       (Convert.ToInt32(maxsongsparallel[seite]) > laufendeSongs[seite].Length))
                    {
                        _player[seite][zeile] = PlayFile(seite, zeile, _player[seite][zeile], file, ((Slider)spnlZeile.FindName("sldKlangVol" + objGruppe + "_" + zeile)).Value / 100);

                        if (_player[seite][zeile].NaturalDuration.HasTimeSpan)
                            ((ProgressBar)spnlZeile.FindName("pbarTitel" + objGruppe + "_" + zeile)).Maximum = _player[seite][zeile].NaturalDuration.TimeSpan.TotalMilliseconds;
                        else
                            ((ProgressBar)spnlZeile.FindName("pbarTitel" + objGruppe + "_" + zeile)).Maximum = 10000;

                        if (laufendeSongs[seite] == null)
                            Array.Resize(ref laufendeSongs[seite], 1);
                        else
                            Array.Resize(ref laufendeSongs[seite], laufendeSongs[seite].Length + 1);
                        laufendeSongs[seite][laufendeSongs[seite].Length - 1] = zeile;

                    }
                    else
                        AddSongStandby(Convert.ToUInt16(seite), zeile);
                }
                else
                {
                    if (_player[seite][zeile] != null)
                    {
                        _player[seite][zeile].Stop();
                        _player[seite][zeile] = null;

                        RemoveSongLaufend(Convert.ToUInt16(seite), zeile);
                    }
                    ((ProgressBar)spnlZeile.FindName("pbarTitel" + objGruppe + "_" + zeile)).Maximum = 100;
                    ((ProgressBar)spnlZeile.FindName("pbarTitel" + objGruppe + "_" + zeile)).Value = 0;

                    RemoveSongParallel(seite, zeile);
                    CheckPlayStandbySongs(objGruppe);
                }
                if (AnzKlangParallel.Length > 0)
                {
                    KlangProgBarTimer.IsEnabled = true;
                    KlangProgBarTimer.Start();
                }
                else
                {
                    KlangProgBarTimer.IsEnabled = false;
                    KlangProgBarTimer.Stop();
                }

                string cap;
                if (seite == 0)
                    cap = ((TabItem)tcKlang.FindName("tiKlang" + objGruppe)).Header.ToString();
                else
                    cap = ((TabItemControl)tcKlang.FindName("tiKlang" + objGruppe))._textBlockTitel.Text;

                List<Audio_Playlist> playlistliste = Global.ContextAudio.PlaylistListe.Where(t => t.Name.Equals(cap)).ToList();
                if (playlistliste.Count == 1)
                {
                    List<Audio_Titel> titelliste = Global.ContextAudio.LoadTitelByPlaylist(playlistliste[0]);
                    List<Audio_Titel> titel = Global.ContextAudio.LoadTitelByPlaylist(playlistliste[0]);
                    List<Audio_Playlist_Titel> playlisttitel = Global.ContextAudio.LoadPlaylist_TitelByPlaylist(playlistliste[0], titel[zeile]);

                    if (playlisttitel.Count != 0)
                    {
                        playlisttitel[0].Aktiv = ((CheckBox)sender).IsChecked.Value;
                        try { Global.ContextAudio.Update<Audio_Playlist_Titel>(playlisttitel[0]); }
                        catch (Exception ex)
                        {
                            var errWin = new MsgWindow("Datenfehler", "Die Datenbank konnte nicht aktualisiert werden", ex);
                            errWin.ShowDialog();
                            errWin.Close();
                        }
                    }
                }
            }
        }

        private void NeueKlangPlaylistInDB()
        {
            string NeuePlaylist;
            if (tcKlang.SelectedIndex == 0)
                NeuePlaylist = ((TabItem)tcKlang.SelectedItem).Header.ToString();
            else
                NeuePlaylist = ((TabItemControl)tcKlang.SelectedItem)._textBlockTitel.Text;

            List<Audio_Playlist> playlistliste;
            int ver = 0;
            while (Global.ContextAudio.PlaylistListe.Where(t => t.Name.Equals(NeuePlaylist)).ToList().Count != 0)
            {
                NeuePlaylist = ((TabItem)tcKlang.SelectedItem).Header.ToString() + "-" + ver;
                ver++;
            }
            if (tcKlang.SelectedIndex == 0)
                ((TabItem)tcKlang.SelectedItem).Header = NeuePlaylist;
            else
                ((TabItemControl)tcKlang.SelectedItem)._textBlockTitel.Text = NeuePlaylist;

            playlistliste = Global.ContextAudio.PlaylistListe.Where(t => t.Name.Equals(NeuePlaylist)).ToList();

            Audio_Playlist playlist = Global.ContextAudio.New<Audio_Playlist>();
            playlist.Name = NeuePlaylist.ToString();
            if (rbIstKlangPlaylist.IsChecked == true) 
                playlist.Hintergrundmusik = false;
            else
                playlist.Hintergrundmusik = true;

            //zur datenbank hinzufügen
            if (Global.ContextAudio.Insert<Audio_Playlist>(playlist))               //erfolgreich hinzugefügt
                AktKlangPlaylist = playlist;
            if (playlist.Hintergrundmusik)
                rbIstMusikPlaylist.IsChecked = true;
            else
                rbIstKlangPlaylist.IsChecked = true;

            AktualisiereKlangPlaylist();
        }

        private void KlangDateiHinzu(string datei)
        {
            //erstelle ein leeres Titel-Objekt
            Audio_Titel titel = Global.ContextAudio.New<Audio_Titel>();
            //eigenschaften setzen
            titel.Name = System.IO.Path.GetFileNameWithoutExtension(datei);
            titel.Pfad = datei;
            //zur datenbank hinzufügen
            if (Global.ContextAudio.Insert<Audio_Titel>(titel))
            {
                Global.ContextAudio.AddTitelToPlaylist(AktKlangPlaylist, titel);

                UInt16 seite = Convert.ToUInt16(tcKlang.SelectedIndex);

                tboxklangsongparallel.Tag = zeilenAufSeite[tcKlang.SelectedIndex] + 1;

                List<Audio_Playlist_Titel> playlisttitel = Global.ContextAudio.LoadPlaylist_TitelByPlaylist(AktKlangPlaylist, titel);
                if (playlisttitel.Count == 1)
                {
                    playlisttitel[0].VolumeChange = false;
                    playlisttitel[0].Volume = 50;
                    playlisttitel[0].VolumeMin = 0;
                    playlisttitel[0].VolumeMax = 100;

                    playlisttitel[0].PauseChange = false;
                    playlisttitel[0].Pause = 1000;
                    playlisttitel[0].PauseMin = 100;
                    playlisttitel[0].PauseMax = 10000;

                    KlangNewRow(datei, Convert.ToUInt16(SeiteXHatObjGruppe[seite]), zeilenAufSeite[seite], playlisttitel[0]);

                    if (AktKlangPlaylist.Hintergrundmusik)
                        ZeigeKlangSettings(Convert.ToUInt16(SeiteXHatObjGruppe[seite]), Convert.ToUInt16(zeilenAufSeite[seite]), false);
                    zeilenAufSeite[seite]++;
                }
            }
        }

        private void grdKlang0_Drop(object sender, DragEventArgs e)
        {
            bool hinzugefuegt = false;
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    if (AktKlangPlaylist == null)
                        NeueKlangPlaylistInDB();

                    string[] gedroppteDateien = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                    string[] extension = new String[4] { ".mp3", ".wav", ".ogg", ".wma" };

                    foreach (string droppedFilePath in gedroppteDateien)
                    {
                        if (Array.IndexOf(extension, droppedFilePath.Substring(droppedFilePath.Length - 4)) != -1)
                        {
                            KlangDateiHinzu(droppedFilePath);
                            hinzugefuegt = true;
                        }
                    }
                }
                if (hinzugefuegt)
                {
                    UInt16 objGruppe = Convert.ToUInt16(SeiteXHatObjGruppe[tcKlang.SelectedIndex]);
                    ((Grid)this.FindName("grdKlangTop" + objGruppe)).Visibility = Visibility.Visible;


                    if (rbIstKlangPlaylist.IsChecked == true)
                        AktKlangPlaylist.Hintergrundmusik = false;
                    else
                        AktKlangPlaylist.Hintergrundmusik = true;

                    if (AktKlangPlaylist.Hintergrundmusik)
                    {
                        ZeigeKlangSongsParallel(false);
                        ZeigeKlangTop(objGruppe, false);
                    }
                    else
                    {
                        ZeigeKlangSongsParallel(true);
                        ZeigeKlangTop(objGruppe, true);
                    }
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private Audio_Playlist UpdatePlaylist(Audio_Playlist AktPlaylist, string NeuerPlaylistName)
        {
            if (AktPlaylist == null)
            {
                Audio_Playlist playlist = Global.ContextAudio.New<Audio_Playlist>();
                playlist.Name = NeuerPlaylistName;
                AktPlaylist = playlist;
            }
            else
            {
                List<Audio_Playlist> playlistliste = Global.ContextAudio.PlaylistListe.Where(t => t.Audio_PlaylistGUID.Equals(AktPlaylist.Audio_PlaylistGUID)).ToList();// .Name.Equals(tboxPlaylistName.Text)).ToList();
                if (playlistliste.Count == 0)
                {
                    Audio_Playlist playlist = Global.ContextAudio.New<Audio_Playlist>();
                    playlist.Name = NeuerPlaylistName;
                    playlist.Hintergrundmusik = false;
                    playlist.MaxSongsParallel = Convert.ToInt32(tboxklangsongparallel.Text);

                    //zur datenbank hinzufügen
                    if (Global.ContextAudio.Insert<Audio_Playlist>(playlist))               //erfolgreich hinzugefügt
                    {
                        List<Audio_Titel> titelMitNeuImNamen = Global.ContextAudio.TitelListe.Where(t => t.Name.StartsWith("Neu")).ToList();
                    }
                }
                else
                {
                    playlistliste[0].Name = NeuerPlaylistName;
                    if (Global.ContextAudio.Update<Audio_Playlist>(playlistliste[0]))
                    {

                    }
                }
            }
            return AktPlaylist;
        }

        private void AktualisiereHintergrundPlaylist()
        {
            UInt16 pos = 0;
            List<Audio_Playlist> playlistliste = Global.ContextAudio.PlaylistListe.ToList();
            for (int i = 0; i < playlistliste.Count; i++)
            {
                if (playlistliste[i].Hintergrundmusik)
                {
                    ListBoxItem lbitem = new ListBoxItem();
                    lbitem.Name = "titel" + i;
                    lbitem.Tag = playlistliste[i].Audio_PlaylistGUID;
                    lbitem.Content = playlistliste[i].Name;
                    if (lbBackground.Items.Count <= i)
                        lbBackground.Items.Add(lbitem);
                    else
                    {
                        ((ListBoxItem)lbBackground.Items[pos]).Name = lbitem.Name;
                        ((ListBoxItem)(lbBackground.Items[pos])).Tag = lbitem.Tag;
                        if (((ListBoxItem)(lbBackground.Items[pos])).Content != lbitem.Content)
                            ((ListBoxItem)(lbBackground.Items[pos])).Content = lbitem.Content;
                    }
                    pos++;
                }
            }
            if (lbBackground.Items.Count > pos && lbBackground.Items.Count != 0)
            {
                for (int i = pos; i < lbBackground.Items.Count; i++)
                    lbBackground.Items.RemoveAt(i);
            }
        }

        private void AktualisiereKlangPlaylist()
        {
            lbKlang.Items.Clear();
            List<Audio_Playlist> playlistliste = Global.ContextAudio.PlaylistListe.ToList();
            for (int i = 0; i < playlistliste.Count; i++)
            {
                if ((playlistliste[i].Hintergrundmusik) && (rbKlangMusik.IsChecked == true || rbKlangAlle.IsChecked == true))
                {
                    ListBoxItem lbitem = new ListBoxItem();
                    lbitem.Name = "titel" + i;
                    lbitem.Tag = playlistliste[i].Audio_PlaylistGUID;
                    lbitem.Content = playlistliste[i].Name;
                    lbKlang.Items.Add(lbitem);
                }
                if ((playlistliste[i].Hintergrundmusik == false) && (rbKlangAlle.IsChecked == true || rbKlangKlang.IsChecked == true))
                {
                    ListBoxItem lbitem = new ListBoxItem();
                    lbitem.Name = "titel" + i;
                    lbitem.Tag = playlistliste[i].Audio_PlaylistGUID;
                    lbitem.Content = playlistliste[i].Name;
                    lbKlang.Items.Add(lbitem);
                }
            }
        }

        private void bntNeuePlaylist_Click(object sender, RoutedEventArgs e)
        {
            string NeuePlaylist = "NeuePlayliste" + tcKlang.SelectedIndex;
            List<Audio_Playlist> playlistliste = Global.ContextAudio.PlaylistListe.Where(t => t.Name.Equals(NeuePlaylist)).ToList();
            if (playlistliste.Count == 0)
            {
                Audio_Playlist playlist = Global.ContextAudio.New<Audio_Playlist>();
                playlist.Name = NeuePlaylist.ToString();
                if (rbKlangKlang.IsChecked.Value)
                    playlist.Hintergrundmusik = false;
                else
                    playlist.Hintergrundmusik = true;

                //zur datenbank hinzufügen
                if (Global.ContextAudio.Insert<Audio_Playlist>(playlist))               //erfolgreich hinzugefügt
                {
                    AktKlangPlaylist = playlist;
                    tboxklangsongparallel.Text = "0";
                    tboxklangsongparallel.Tag = null;
                    playlist.MaxSongsParallel = Convert.ToInt32(tboxklangsongparallel.Text);
                    maxsongsparallel[tcKlang.SelectedIndex] = 0;
                    AktualisiereKlangPlaylist();
                    for (int i = 0; i <= lbKlang.Items.Count - 1; i++)
                        if ((lbKlang.Items[i] as ListBoxItem).Content.ToString() == playlist.Name)
                            lbKlang.SelectedIndex = i;

                    tboxPlaylistName.Background = Brushes.OrangeRed;
                    tboxPlaylistName.Focus();
                    //nun kann man verschiedene titel zB. so holen
                    //          List<Audio_Titel> titelMitNeuImNamen = Global.ContextAudio.TitelListe.Where(t => t.Name.StartsWith("Neu")).ToList();
                }
            }
            else
            {
                var errWin = new MsgWindow("Datenbankfehler", "Playlist schon vorhanden. Bitte wiederholen Sie den Vorgang und wählen einen anderen Titel");
                errWin.ShowDialog();
                errWin.Close();
            }
        }

        private void tiHintergrund_Loaded(object sender, RoutedEventArgs e)
        {
            if (lbBackground.Items.Count == 0)
                AktualisiereHintergrundPlaylist();
        }

        private void tiKlang_Loaded(object sender, RoutedEventArgs e)
        {
            if (lbKlang.Items.Count == 0)
                AktualisiereKlangPlaylist();
        }

        private void lbhintergrundtitellist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((lbhintergrundtitellist.SelectedIndex >= 0) &&
               (((ListBoxItem)lbhintergrundtitellist.SelectedItem).Background != Brushes.Red))
            {
                ListBoxItem lbItem = (ListBoxItem)lbhintergrundtitellist.SelectedItem;
                string st = lbItem.Tag.ToString();

                List<Audio_Titel> titel = Global.ContextAudio.LoadTitelByPlaylist(AktHintergrundPlaylist);

                if (titel.Count == 0)
                {
                    if (!File.Exists(titel[lbhintergrundtitellist.SelectedIndex].Pfad))
                        lbItem.Background = Brushes.Red;
                    lbItem.ToolTip = "Datei nicht gefunden";

                    lbBackground_SelectionChanged(lbhintergrundtitellist, e);
                }
                else
                {
                    if (!File.Exists(titel[lbhintergrundtitellist.SelectedIndex].Pfad))
                    {
                        lbItem.Background = Brushes.Red;
                        lbItem.ToolTip = "Datei nicht gefunden";
                        SpieleNeuenHintergrundTitel();
                    }
                    else
                    {
                        if (HintergrundPlayer != null)
                        {
                            HintergrundPlayer.Stop();
                            HintergrundProgBarTimer.Stop();
                            HintergrundPlayer = null;
                        }
                        lblBgTimeMax.Content = "-:--";
                        lblBgTitel.Content = "";
                        lblBgAlbum.Content = "";
                        lblBgArtist.Content = "";
                        lblBgJahr.Content = "";
                        lblBgGenre.Content = "";
                        HintergrundPlayer = PlayFile(-1, 0, HintergrundPlayer, titel[lbhintergrundtitellist.SelectedIndex].Pfad, slBGVolume.Value / 100);

                        if (HintergrundPlayer != null)
                        {
                            btnBGAbspielen.Tag = 1;

                            BitmapImage logo = new BitmapImage();
                            logo.BeginInit();
                            logo.UriSource = new Uri("pack://application:,,,/DSA MeisterGeister;component/Images/Icons/General/play_stop.png");
                            logo.EndInit();

                            btnImgBGAbspielen.Source = logo;
                            btnLblBGAbspielen.Content = "Stoppen";

                            pbarBGSong.Value = 0;
                            string s = "0:00";
                            if (HintergrundPlayer.NaturalDuration.HasTimeSpan)
                            {
                                pbarBGSong.Maximum = HintergrundPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                                if (HintergrundPlayer.NaturalDuration.TimeSpan.Minutes < 10)
                                    s = "0" + HintergrundPlayer.NaturalDuration.TimeSpan.Minutes + ":";
                                else
                                    s = HintergrundPlayer.NaturalDuration.TimeSpan.Minutes + ":";

                                if (HintergrundPlayer.NaturalDuration.TimeSpan.Seconds < 10)
                                    s = s + "0" + HintergrundPlayer.NaturalDuration.TimeSpan.Seconds;
                                else
                                    s = s + HintergrundPlayer.NaturalDuration.TimeSpan.Seconds;
                                lblBgTimeMax.Content = s;
                            }
                            btnBGNext.IsEnabled = true;
                            btnBGAbspielen.IsEnabled = true;
                            grdSongInfo.Visibility = Visibility.Visible;

                            ListBoxItem lbi = (ListBoxItem)lbBackground.SelectedItem;
                            HintergrundProgBarTimer.Tag = -1;
                            HintergrundProgBarTimer.Start();
                        }
                    }
                }
            }
        }

        private static String ConvertByteToString(byte[] bytes, int pos1, int pos2)
        {
            //pos2 muß größer oder gleich pos1 sein und
            //pos2 darf Länge des Arrays nicht überschreiten
            if ((pos1 > pos2) || (pos2 > bytes.Length - 1))
            {
                throw new ArgumentException("Aruments out of range");
            }

            //Länge des zu betrachtenden Ausschnittes
            int length = pos2 - pos1 + 1;

            //neues Char-Array anlegen der Länge length
            Char[] chars = new Char[length];

            //packe alle Bytes von pos1 bis pos2 als
            //Char konvertiert in Array chars
            for (int i = 0; i < length; i++)
            {
                chars[i] = Convert.ToChar(bytes[i + pos1]);
            }//end for

            //konvertiere Char-Array in String und gebe es zurück
            String s = new String(chars);
            s = s.Replace("\0", "");

            return s; // neu String(s);
        }

        private void HintergrundProgBarTimer_Tick(object sender, EventArgs e)
        {
            if (lblBgTitel.Content.ToString() == "")
            {
                FileInfo file = new FileInfo(HintergrundPlayer.Source.LocalPath);
                Stream str = file.OpenRead();
                byte[] bytes = new byte[128];
                str.Seek(-128, SeekOrigin.End);
                int numBytesToRead = 128;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = str.Read(bytes, numBytesRead, numBytesToRead);

                    if (n == 0)
                        break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                str.Close();

                String tag = ConvertByteToString(bytes, 0, 2);
                if (tag != "TAG")
                {
                    lblBgTitel.Content = System.IO.Path.GetFileNameWithoutExtension(HintergrundPlayer.Source.LocalPath);
                    lblBgArtist.Content = "---";
                    lblBgAlbum.Content = "---";
                    lblBgJahr.Content = "---";
                    lblBgGenre.Content = "---";
                }
                else
                {
                    string[] _genres = {
			            "Blues","Classic Rock","Country","Dance","Disco","Funk","Grunge","Hip-Hop","Jazz","Metal",
			            "New Age","Oldies","Other","Pop","R&B","Rap","Reggae","Rock","Techno","Industrial",
			            "Alternative","Ska","Death Metal","Pranks","Soundtrack","Euro-Techno","Ambient","Trip-Hop",
			            "Vocal","Jazz+Funk","Fusion","Trance","Classical","Instrumental","Acid","House",
			            "Game","Sound Clip","Gospel","Noise","Alternative Rock","Bass","Soul","Punk","Space",
			            "Meditative","Instrumental Pop","Instrumental Rock","Ethnic","Gothic",
			            "Darkwave","Techno-Industrial","Electronic","Pop-Folk","Eurodance","Dream",
			            "Southern Rock","Comedy","Cult","Gangsta","Top 40","Christian Rap","Pop/Funk","Jungle",
			            "Native American","Cabaret","New Wave","Psychadelic","Rave","Showtunes","Trailer","Lo-Fi",
			            "Tribal","Acid Punk","Acid Jazz","Polka","Retro","Musical","Rock & Roll","Hard Rock","Folk",
			            "Folk/Rock","National Folk","Swing","Fast-Fusion","Bebob","Latin","Revival","Celtic","Bluegrass",
			            "Avantgarde","Gothic Rock","Progressive Rock","Psychedelic Rock","Symphonic Rock","Slow Rock",
			            "Big Band","Chorus","Easy Listening","Acoustic","Humour","Speech","Chanson","Opera","Chamber Music",
			            "Sonata","Symphony","Booty Bass","Primus","Porn Groove","Satire","Slow Jam","Club",
			            "Tango","Samba","Folklore","Ballad","Power Ballad","Rhytmic Soul","Freestyle","Duet",
			            "Punk Rock","Drum Solo","Acapella","Euro-House","Dance Hall","Goa","Drum & Bass","Club-House",
			            "Hardcore","Terror","Indie","BritPop","Negerpunk","Polsk Punk","Beat","Christian Gangsta Rap",
			            "Heavy Metal","Black Metal","Crossover","Contemporary Christian",
			            "Christian Rock","Merengue","Salsa","Trash Metal","Anime","JPop","SynthPop"};

                    lblBgTitel.Content = ConvertByteToString(bytes, 3, 32);
                    lblBgArtist.Content = ConvertByteToString(bytes, 33, 62);
                    lblBgAlbum.Content = ConvertByteToString(bytes, 63, 92);
                    lblBgJahr.Content = ConvertByteToString(bytes, 93, 96);
                    //m_comment = ConvertByteToString(bytes, 97, 126);
                    int z = Convert.ToInt32(bytes[127]);
                    if (z <= _genres.Length - 1)
                        lblBgGenre.Content = _genres[z];


               /*     int size;
                    try
                    {
                        size = int.Parse(textBox1.Text);
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Enter requiered size!", "Err");
                        return;
                    }
                        var song = TagLib.File.Create(file);
                        if (song.Tag.Pictures.Length > 0)
                        {
                            // var bin = (byte[])(song.Tag.Pictures[0].Data.Data);                                                
                            song.Tag.Pictures[0].Data.Resize(size);
                        }
                        }
                    }     */
                  //  private BindingSource _tagHandlerBindingSource;
           /*         imgHintergrundTitel.BeginInit();
                    imgHintergrundTitel.Source = (
                        = typeof(Id3Lib.TagHandler);
                    this._artPictureBox.DataBindings.Add(new System.Windows.Forms.Binding("Image", this._tagHandlerBindingSource, "Picture", true));
                    this._tagHandlerBindingSource.DataSource = typeof(Id3Lib.TagHandler);*/
                }
            }

            if (lblBgTimeMax.Content.ToString() == "-:--")
            {
                if (HintergrundPlayer.NaturalDuration.HasTimeSpan)
                {
                    string t = "0:00";
                    pbarBGSong.Maximum = HintergrundPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                    if (HintergrundPlayer.NaturalDuration.TimeSpan.Minutes < 10)
                        t = "0" + HintergrundPlayer.NaturalDuration.TimeSpan.Minutes + ":";
                    else
                        t = HintergrundPlayer.NaturalDuration.TimeSpan.Minutes + ":";

                    if (HintergrundPlayer.NaturalDuration.TimeSpan.Seconds < 10)
                        t = t + "0" + HintergrundPlayer.NaturalDuration.TimeSpan.Seconds;
                    else
                        t = t + HintergrundPlayer.NaturalDuration.TimeSpan.Seconds;
                    lblBgTimeMax.Content = t;
                }
            }

            pbarBGSong.Value = HintergrundPlayer.Position.TotalMilliseconds;
            string s = "";
            if (HintergrundPlayer.Position.Minutes < 10)
                s = "0" + HintergrundPlayer.Position.Minutes + ":";
            else
                s = HintergrundPlayer.Position.Minutes + ":";

            if (HintergrundPlayer.Position.Seconds < 10)
                s = s + "0" + HintergrundPlayer.Position.Seconds;
            else
                s = s + HintergrundPlayer.Position.Seconds;
            lblBgTimeActual.Content = s;

            if ((HintergrundPlayer.NaturalDuration.HasTimeSpan &&
                 HintergrundPlayer.Position.TotalMilliseconds == HintergrundPlayer.NaturalDuration.TimeSpan.TotalMilliseconds))
            {
                if (HintergrundPlayer != null)
                {
                    HintergrundPlayer.Stop();
                    HintergrundPlayer = null;
                }
                HintergrundProgBarTimer.Stop();
                SpieleNeuenHintergrundTitel();
            }
            else
                HintergrundProgBarTimer.Tag = HintergrundPlayer.Position.TotalMilliseconds;
        }

        private void chkVolMove0_0_Click(object sender, RoutedEventArgs e)
        {
            int zeile = Convert.ToInt32(((CheckBox)sender).Tag);

            List<Audio_Titel> titel = Global.ContextAudio.LoadTitelByPlaylist(AktKlangPlaylist);
            List<Audio_Playlist_Titel> playlisttitel = Global.ContextAudio.LoadPlaylist_TitelByPlaylist(AktKlangPlaylist, titel[zeile]);

            if (playlisttitel.Count != 0)
            {
                playlisttitel[0].VolumeChange = ((CheckBox)sender).IsChecked.Value;
                try
                {
                    Global.ContextAudio.Update<Audio_Playlist_Titel>(playlisttitel[0]);
                }
                catch (Exception ex)
                {
                    var errWin = new MsgWindow("Datenfehler", "Die Datenbank konnte nicht aktualisiert werden", ex);
                    errWin.ShowDialog();
                    errWin.Close();
                }
            }
        }

        private void sldKlangPause0_0_LostFocus(object sender, RoutedEventArgs e)
        {
            int zeile = Convert.ToInt32(((Slider)sender).Tag);
            List<Audio_Titel> titel = Global.ContextAudio.LoadTitelByPlaylist(AktKlangPlaylist);
            List<Audio_Playlist_Titel> playlisttitel = Global.ContextAudio.LoadPlaylist_TitelByPlaylist(AktKlangPlaylist, titel[zeile]);

            if (playlisttitel.Count != 0)
            {
                playlisttitel[0].Pause = Convert.ToInt32(Math.Round(((Slider)sender).Value));
                try { Global.ContextAudio.Update<Audio_Playlist_Titel>(playlisttitel[0]); }
                catch (Exception ex)
                {
                    var errWin = new MsgWindow("Datenfehler", "Die Datenbank konnte nicht aktualisiert werden", ex);
                    errWin.ShowDialog();
                    errWin.Close();
                }
            }
        }

        private void chkKlangPauseMove0_0_Click(object sender, RoutedEventArgs e)
        {
            int zeile = Convert.ToInt32(((CheckBox)sender).Tag);
            List<Audio_Titel> titel = Global.ContextAudio.LoadTitelByPlaylist(AktKlangPlaylist);
            List<Audio_Playlist_Titel> playlisttitel = Global.ContextAudio.LoadPlaylist_TitelByPlaylist(AktKlangPlaylist, titel[zeile]);

            if (playlisttitel.Count != 0)
            {
                playlisttitel[0].PauseChange = ((CheckBox)sender).IsChecked.Value;
                try
                {
                    Global.ContextAudio.Update<Audio_Playlist_Titel>(playlisttitel[0]);
                }
                catch (Exception ex)
                {
                    var errWin = new MsgWindow("Datenfehler", "Die Datenbank konnte nicht aktualisiert werden", ex);
                    errWin.ShowDialog();
                    errWin.Close();
                }
            }
        }

        private void sldKlangPause0_0_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).ToolTip = Math.Round(e.NewValue) + " ms";
        }

        private void imgTrash0_0_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Int16 objGruppe = Convert.ToInt16(SeiteXHatObjGruppe[tcKlang.SelectedIndex]);
            List<Audio_Titel> titel = Global.ContextAudio.LoadTitelByPlaylist(AktKlangPlaylist);
            Grid grdKlang = (Grid)this.FindName("grdKlang" + objGruppe);

            for (int i = 0; i <= titel.Count - 1; i++)
                if (titel[i].Audio_TitelGUID.Equals(((Image)sender).Tag))
                {
                    CheckBox chbx = (CheckBox)((StackPanel)this.FindName("spnlKlangRow" + objGruppe + "_" + i)).FindName("chkTitel" + objGruppe + "_" + i);
                    if (chbx.IsChecked.Value == true)
                    {
                        chbx.IsChecked = false;
                        chkTitel0_0_Click(chbx, new RoutedEventArgs());
                    }
                    Global.ContextAudio.RemoveTitelFromPlaylist(AktKlangPlaylist, titel[i]);

                    List<Audio_Playlist> playlistliste = Global.ContextAudio.PlaylistListe;
                    int anz = 0;
                    bool gefunden = false;
                    while (anz < playlistliste.Count && !gefunden)
                    {

                        if ((Global.ContextAudio.LoadPlaylist_TitelByPlaylist(playlistliste[anz], titel[i])).Count > 0)
                            gefunden = true;
                        else
                            anz++;
                    }
                    if (!gefunden)
                        Global.ContextAudio.RemoveTitel(titel[i]);
                                        
                    int vorher = lbKlang.SelectedIndex;
                    lbKlang.SelectedIndex = -1;
                    lbKlang.SelectedIndex = vorher;                     
                }

            if (titel.Count == 0)
                ZeigeKlangSongsParallel(false);
        }

        private void rbKlangKlang_Click(object sender, RoutedEventArgs e)
        {
            if (tcAudioPlayer.SelectedIndex == 0)
                AktualisiereHintergrundPlaylist();
            else
                if (tcAudioPlayer.SelectedIndex == 1)
                    AktualisiereKlangPlaylist();
        }

        private void rbKlangAlle_Click(object sender, RoutedEventArgs e)
        {
            if (tcAudioPlayer.SelectedIndex == 0)
                AktualisiereHintergrundPlaylist();
            else
                if (tcAudioPlayer.SelectedIndex == 1)
                    AktualisiereKlangPlaylist();
        }

        private void rbIstKlangPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (AktKlangPlaylist != null)
            {
                AktKlangPlaylist.Hintergrundmusik = false;
                Global.ContextAudio.Update<Audio_Playlist>(AktKlangPlaylist);

                UInt16 seite = Convert.ToUInt16(tcKlang.SelectedIndex);
                UInt16 objGruppe = Convert.ToUInt16(SeiteXHatObjGruppe[seite]);

                ZeigeKlangSongsParallel(true);
                ZeigeKlangTop(objGruppe, true);

                for (UInt16 i = 0; i < zeilenAufSeite[seite]; i++)
                {
                    ZeigeKlangSettings(objGruppe, i, true);
                    StackPanel spnlZeile = (StackPanel)this.FindName("spnlKlangRow" + objGruppe + "_" + i);
                    if (((CheckBox)spnlZeile.FindName("chkTitel" + objGruppe + "_" + i)).IsChecked == true)
                        AddSongStandby(seite, i);
                }
                AktualisiereKlangPlaylist();
                CheckPlayStandbySongs(objGruppe);
            }
        }

        private void rbIstMusikPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (AktKlangPlaylist != null)
            {
                AktKlangPlaylist.Hintergrundmusik = true;
                Global.ContextAudio.Update<Audio_Playlist>(AktKlangPlaylist);

                UInt16 seite = Convert.ToUInt16(tcKlang.SelectedIndex);
                UInt16 objGruppe = Convert.ToUInt16(SeiteXHatObjGruppe[seite]);

                AlleKlangSongsAus(objGruppe, false, false);

                ZeigeKlangSongsParallel(false);
                ZeigeKlangTop(objGruppe, false);
                for (int i = 0; i < zeilenAufSeite[seite]; i++)
                    ZeigeKlangSettings(objGruppe, Convert.ToUInt16(i), false);
                AktualisiereKlangPlaylist();
            }
        }

        private void tboxPlaylistName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (AktKlangPlaylist == null)
                {
                    List<Audio_Playlist> playlistliste = Global.ContextAudio.PlaylistListe.Where(t => t.Name.Equals(tboxPlaylistName.Text)).ToList();
                    if (playlistliste.Count == 1)
                    {
                        List<Audio_Titel> titelliste = Global.ContextAudio.LoadTitelByPlaylist(playlistliste[0]);
                        AktKlangPlaylist = playlistliste[0];
                        if (rbIstMusikPlaylist.IsChecked == true)
                            AktKlangPlaylist.Hintergrundmusik = true;
                        else
                            AktKlangPlaylist.Hintergrundmusik = false;
                        tboxPlaylistName.Text = AktKlangPlaylist.Name;
                        if (tcKlang.SelectedIndex == 0)
                            ((TabItem)tcKlang.SelectedItem).Header = AktKlangPlaylist.Name;
                        else
                            ((TabItemControl)tcKlang.SelectedItem)._textBlockTitel.Text = AktKlangPlaylist.Name;
                    }
                    else
                    {
                        NeueKlangPlaylistInDB();
                    }
                }
                for (int i = 0; i <= lbKlang.Items.Count - 1; i++)
                {
                    if (((ListBoxItem)lbKlang.Items[i]).Content.ToString() == AktKlangPlaylist.Name)
                        ((ListBoxItem)lbKlang.Items[i]).Content = tboxPlaylistName.Text;
                }
                AktKlangPlaylist.Name = tboxPlaylistName.Text;
                Global.ContextAudio.Update<Audio_Playlist>(AktKlangPlaylist);

                if (tcKlang.SelectedIndex == 0)
                    tiKlang0.Header = AktKlangPlaylist.Name;
                else
                    ((TabItemControl)tcKlang.SelectedItem)._textBlockTitel.Text = AktKlangPlaylist.Name;

                ((TextBox)(sender)).Background = null;
            }
            else
            {
                ((TextBox)(sender)).Background = Brushes.OrangeRed;
            }
        }


        private void pbarBGSong_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (AktHintergrundPlaylist != null)
            {
                Point pts = e.GetPosition(pbarBGSong);
                double total = pbarBGSong.Maximum;
                double res = ((pts.X * 100) / ((double)pbarBGSong.ActualWidth)) / 100;
                HintergrundPlayer.Position = TimeSpan.FromMilliseconds(total * res);
            }
        }

        private void tcKlang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tcKlang.SelectedIndex == tcKlang.Items.Count - 1)
            {
                if (tcKlang.Items.Count == 1)
                    tcKlang.SelectedIndex = -1;
                else
                    tcKlang.SelectedIndex = tcKlang.Items.Count - 2;
            }
            string s = "";
            if (((TabItem)tcKlang.Items[0]).Header.ToString() == "")
            {
                List<Audio_Playlist> plylstliste;
                int ver = 0;
                s = "NeuePlayliste0";
                while (Global.ContextAudio.PlaylistListe.Where(t => t.Name.Equals(s)).ToList().Count != 0)
                {
                    s = "NeuePlayliste0-" + ver;
                    ver++;
                }
                ((TabItem)tcKlang.SelectedItem).Header = s;
                plylstliste = Global.ContextAudio.PlaylistListe.Where(t => t.Name.Equals(s)).ToList();
                tboxPlaylistName.Text = s;
                btnKlangOpen.Focus();
            }
            else
            {
                if (tcKlang.SelectedIndex >= 0)
                {
                    if (tcKlang.SelectedItem.GetType().ToString().EndsWith("TabItemControl"))
                        s = ((TabItemControl)tcKlang.SelectedItem)._textBlockTitel.Text.ToString();
                    else
                        s = ((TabItem)tcKlang.SelectedItem).Header.ToString();

                    UInt16 objGruppe = Convert.ToUInt16(SeiteXHatObjGruppe[tcKlang.SelectedIndex]);

                    List<Audio_Playlist> playlistliste = Global.ContextAudio.PlaylistListe.Where(t => t.Name.Equals(s)).ToList();
                    if (playlistliste.Count == 1)
                    {
                        List<Audio_Titel> titelliste = Global.ContextAudio.LoadTitelByPlaylist(playlistliste[0]);
                        AktKlangPlaylist = playlistliste[0];

                        if (AktKlangPlaylist.Hintergrundmusik)
                            rbIstMusikPlaylist.IsChecked = true;
                        else
                            rbIstKlangPlaylist.IsChecked = true;
                        tboxPlaylistName.Text = AktKlangPlaylist.Name;


                        if (titelliste.Count > 0)
                        {
                            tboxklangsongparallel.TextChanged -= new TextChangedEventHandler(tboxklangsongparallel_TextChanged);
                            tboxklangsongparallel.Tag = AktKlangPlaylist.Audio_Playlist_Titel.Count;

                            tboxklangsongparallel.Text = AktKlangPlaylist.MaxSongsParallel.ToString();
                            maxsongsparallel[tcKlang.SelectedIndex] = AktKlangPlaylist.MaxSongsParallel;
                            tboxklangsongparallel.TextChanged += new TextChangedEventHandler(tboxklangsongparallel_TextChanged);

                            zeilenAufSeite[tcKlang.SelectedIndex] = Convert.ToUInt16(AktKlangPlaylist.Audio_Playlist_Titel.Count);

                            StackPanel sp = (StackPanel)this.FindName("spnlKlangRow" + objGruppe + "_0");
                            CheckBox chbx = (CheckBox)(sp).FindName("chkTitel" + objGruppe + "_0");
                            if (playlistliste[0].Hintergrundmusik)
                            {
                                ZeigeKlangSongsParallel(false);
                                if (chbx.Visibility == Visibility.Visible)
                                {
                                    ZeigeKlangTop(objGruppe, false);
                                    for (int r = 0; r < zeilenAufSeite[tcKlang.SelectedIndex]; r++)
                                        ZeigeKlangSettings(objGruppe, Convert.ToUInt16(r), false);
                                }
                            }
                            else
                            {
                                ZeigeKlangSongsParallel(true);
                                if (chbx.Visibility == Visibility.Hidden)
                                {
                                    ZeigeKlangTop(objGruppe, true);
                                    for (int r = 0; r < zeilenAufSeite[tcKlang.SelectedIndex]; r++)
                                        ZeigeKlangSettings(objGruppe, Convert.ToUInt16(r), true);
                                }
                            }
                        }
                        ((Grid)this.FindName("grdKlangTop" + objGruppe)).Visibility = Visibility.Visible;
                    }
                    else
                    {
                        lbKlang.SelectionChanged -= new SelectionChangedEventHandler(lbKlang_SelectionChanged);
                        lbKlang.SelectedIndex = -1;
                        lbKlang.SelectionChanged += new SelectionChangedEventHandler(lbKlang_SelectionChanged);

                        rbIstKlangPlaylist.IsChecked = rbKlangKlang.IsChecked;
                        rbIstMusikPlaylist.IsChecked = rbKlangMusik.IsChecked;
                        tboxPlaylistName.Text = s;
                        ZeigeKlangSongsParallel(false);

                        tboxklangsongparallel.TextChanged -= new TextChangedEventHandler(tboxklangsongparallel_TextChanged);
                        tboxklangsongparallel.Tag = null;
                        tboxklangsongparallel.Text = "0";
                        maxsongsparallel[tcKlang.SelectedIndex] = 0;
                        tboxklangsongparallel.TextChanged += new TextChangedEventHandler(tboxklangsongparallel_TextChanged);
                    }
                }
                int i = 0;
                while (i <= lbKlang.Items.Count - 1)
                {
                    if (((ListBoxItem)lbKlang.Items[i]).Content.ToString() == s)
                    {
                        lbKlang.SelectionChanged -= new SelectionChangedEventHandler(lbKlang_SelectionChanged);
                        lbKlang.SelectedIndex = i;
                        lbKlang.SelectionChanged += new SelectionChangedEventHandler(lbKlang_SelectionChanged);
                    }
                    i++;
                }
            }
        }


        private void tiKlangPlaylistClose_Click(object sender, RoutedEventArgs e)
        {
            //Name des TabItem herausfinden   
            UInt16 objGruppe;
            if (e.Source is Button) 
                objGruppe = Convert.ToUInt16((((TabItemControl)((StackPanel)(((Button)sender).Parent)).Parent)).Name.Substring(7));
            else
                objGruppe = Convert.ToUInt16(((TabItemControl)sender).Name.Substring(7));
            int seite = Array.IndexOf(SeiteXHatObjGruppe, objGruppe);
            
            if (laufendeSongs[seite] != null)
            {
                for (int i = 0; i < laufendeSongs[seite].Count(); i++)
                {
                    _player[seite][laufendeSongs[seite][i]].MediaEnded -= new EventHandler(Player_Ended);
                    _player[seite][laufendeSongs[seite][i]].Stop();
                }
            }
            PlaylisteLeeren(objGruppe);

            this.UnregisterName("tiKlang" + objGruppe);
            this.UnregisterName("Grid" + objGruppe);
            this.UnregisterName("sviewer" + objGruppe);
            this.UnregisterName("grdKlang" + objGruppe);
            this.UnregisterName("grdKlangTop" + objGruppe);

            for (int i = seite; i < 7; i++)
            {
                SeiteXHatObjGruppe[i] = SeiteXHatObjGruppe[i + 1];
                zeilenAufSeite[i] = zeilenAufSeite[i + 1];
            }
            SeiteXHatObjGruppe[7] = -1;
            zeilenAufSeite[7] = 0;

            if (tcKlang.SelectedIndex == tcKlang.Items.Count - 2)
                tcKlang.SelectedIndex = tcKlang.SelectedIndex - 1;

            if (tcKlang.Items.Count < 10)
                tiPlus.Visibility = Visibility.Visible;
        }

        private void tiPlus_MouseUp(object sender, MouseButtonEventArgs e)
        {
            tiErstellt++;
            Int32 objGruppe = tiErstellt;

            AktKlangPlaylist = null;
            Grid GridTmp = (Grid)DeepCopy(GridX, "X", objGruppe.ToString());

            TabItemControl tabItem = new TabItemControl();
            tabItem.Visibility = Visibility.Visible;
            tabItem._image.Source = null;
            tabItem._image.Width = 0;
            tabItem.Height = 19;
            tabItem.Name = "tiKlang" + objGruppe;

            string NeuePlaylist = "NeuePlayliste" + (tcKlang.Items.Count - 2);

            int ver = 0;
            while (Global.ContextAudio.PlaylistListe.Where(t => t.Name.Equals(NeuePlaylist)).ToList().Count != 0)
            {
                NeuePlaylist = "NeuePlayliste" + (tcKlang.Items.Count - 2) + "-" + ver;
                ver++;
            }

            tabItem._textBlockTitel.Text = NeuePlaylist;
            tabItem._buttonClose.Click += new RoutedEventHandler(tiKlangPlaylistClose_Click);
            
            SeiteXHatObjGruppe[tcKlang.Items.Count - 2] = objGruppe;
            tcKlang.Items.Insert(tcKlang.Items.Count - 2, tabItem);
            this.RegisterName("tiKlang" + objGruppe, tabItem);
            tabItem.Content = GridTmp;
            this.RegisterName(GridTmp.Name, GridTmp);


            ScrollViewer scrViewer = (ScrollViewer)GridTmp.FindName("sviewer" + objGruppe);
            scrViewer.ToolTip = sviewer0.ToolTip;
            scrViewer.Drop += new DragEventHandler(grdKlang0_Drop);
            this.RegisterName(scrViewer.Name, scrViewer);

            Grid grdKlang = (Grid)scrViewer.FindName("grdKlang" + objGruppe);
            this.RegisterName(grdKlang.Name, grdKlang);

            Grid grdKlangTop = (Grid)grdKlang.FindName("grdKlangTop" + objGruppe);
            this.RegisterName(grdKlangTop.Name, grdKlangTop);

            ToggleButton tbtn = (ToggleButton)grdKlangTop.FindName("btnKlangStop" + objGruppe);
            tbtn.Click += new RoutedEventHandler(btnKlangStop0_Click);

            Button btnTopVolMin = (Button)grdKlangTop.FindName("btnTopVolMin" + objGruppe);
            btnTopVolMin.Click += new RoutedEventHandler(btnAllVolUp_Click);
            Button btnTopVolDown = (Button)grdKlangTop.FindName("btnTopVolDown" + objGruppe);
            btnTopVolDown.Click += new RoutedEventHandler(btnAllVolUp_Click);
            Button btnTopVolUp = (Button)grdKlangTop.FindName("btnTopVolUp" + objGruppe);
            btnTopVolUp.Click += new RoutedEventHandler(btnAllVolUp_Click);
            Button btnTopVolMax = (Button)grdKlangTop.FindName("btnTopVolMax" + objGruppe);
            btnTopVolMax.Click += new RoutedEventHandler(btnAllVolUp_Click);

            CheckBox chbxTopVolCh = (CheckBox)grdKlangTop.FindName("chkbxTopVolChange" + objGruppe);
            chbxTopVolCh.Click += new RoutedEventHandler(chkbxTopVolChange0_Click);

            Button btnTopPauseMin = (Button)grdKlangTop.FindName("btnTopPauseMin" + objGruppe);
            btnTopPauseMin.Click += new RoutedEventHandler(btnAllVolUp_Click);
            Button btnTopPauseDown = (Button)grdKlangTop.FindName("btnTopPauseDown" + objGruppe);
            btnTopPauseDown.Click += new RoutedEventHandler(btnAllVolUp_Click);
            Button btnTopPauseUp = (Button)grdKlangTop.FindName("btnTopPauseUp" + objGruppe);
            btnTopPauseUp.Click += new RoutedEventHandler(btnAllVolUp_Click);
            Button btnTopPauseMax = (Button)grdKlangTop.FindName("btnTopPauseMax" + objGruppe);
            btnTopPauseMax.Click += new RoutedEventHandler(btnAllVolUp_Click);

            CheckBox chbxTopPauseCh = (CheckBox)grdKlangTop.FindName("chkBoxTopPause" + objGruppe);
            chbxTopPauseCh.Click += new RoutedEventHandler(chkbxTopVolChange0_Click);

            Button btnTopVolMinMinus = (Button)grdKlangTop.FindName("btnTopVolMinMinus" + objGruppe);
            btnTopVolMinMinus.Click += new RoutedEventHandler(btnTopVolMinMinus0_Click);
            Button btnTopVolMinPlus = (Button)grdKlangTop.FindName("btnTopVolMinPlus" + objGruppe);
            btnTopVolMinPlus.Click += new RoutedEventHandler(btnTopVolMinMinus0_Click);
            Button btnTopVolMaxMinus = (Button)grdKlangTop.FindName("btnTopVolMaxMinus" + objGruppe);
            btnTopVolMaxMinus.Click += new RoutedEventHandler(btnTopVolMinMinus0_Click);
            Button btnTopVolMaxPlus = (Button)grdKlangTop.FindName("btnTopVolMaxPlus" + objGruppe);
            btnTopVolMaxPlus.Click += new RoutedEventHandler(btnTopVolMinMinus0_Click);

            Button btnTopPauseMinMinus = (Button)grdKlangTop.FindName("btnTopPauseMinMinus" + objGruppe);
            btnTopPauseMinMinus.Click += new RoutedEventHandler(btnTopVolMinMinus0_Click);
            Button btnTopPauseMinPlus = (Button)grdKlangTop.FindName("btnTopPauseMinPlus" + objGruppe);
            btnTopPauseMinPlus.Click += new RoutedEventHandler(btnTopVolMinMinus0_Click);
            Button btnTopPauseMaxMinus = (Button)grdKlangTop.FindName("btnTopPauseMaxMinus" + objGruppe);
            btnTopPauseMaxMinus.Click += new RoutedEventHandler(btnTopVolMinMinus0_Click);
            Button btnTopPauseMaxPlus = (Button)grdKlangTop.FindName("btnTopPauseMaxPlus" + objGruppe);
            btnTopPauseMaxPlus.Click += new RoutedEventHandler(btnTopVolMinMinus0_Click);


            tcKlang.SelectedIndex = tcKlang.Items.Count - 2 - 1;

            lbKlang.SelectionChanged -= new SelectionChangedEventHandler(lbKlang_SelectionChanged);
            lbKlang.SelectedIndex = -1;
            lbKlang.SelectionChanged += new SelectionChangedEventHandler(lbKlang_SelectionChanged);

            if (tcKlang.Items.Count == 10)
                tiPlus.Visibility = Visibility.Hidden;
        }

        private void tboxVolMin0_X_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (var item in e.Text)
                e.Handled = !char.IsDigit(item);
        }

        private void tboxVolMin0_X_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AktKlangPlaylist != null)
            {
                UInt16 objGruppe = Convert.ToUInt16(SeiteXHatObjGruppe[tcKlang.SelectedIndex]);

                List<Audio_Titel> titel = Global.ContextAudio.LoadTitelByPlaylist(AktKlangPlaylist);
                List<Audio_Playlist_Titel> playlisttitel = Global.ContextAudio.LoadPlaylist_TitelByPlaylist(AktKlangPlaylist, titel[Convert.ToInt32(((TextBox)(sender)).Tag)]);
                if (playlisttitel.Count != 0)
                {
                    Grid grd = (Grid)((TextBox)(sender)).Parent;
                    if (((TextBox)(sender)).Name.StartsWith("tboxVolMin"))
                    {
                        playlisttitel[0].VolumeMin = Convert.ToInt16(((TextBox)(sender)).Text);
                        
                        if (Convert.ToInt16(((TextBox)grd.FindName("tboxVolMax" + objGruppe + "_" + Convert.ToString(((TextBox)(sender)).Tag))).Text) < playlisttitel[0].VolumeMin)
                            ((TextBox)grd.FindName("tboxVolMax" + objGruppe + "_" + Convert.ToString(((TextBox)(sender)).Tag))).Text = Convert.ToString(playlisttitel[0].VolumeMin);
                    }
                    if (((TextBox)(sender)).Name.StartsWith("tboxVolMax"))
                    {
                        playlisttitel[0].VolumeMax = Convert.ToInt16(((TextBox)(sender)).Text);

                        if (Convert.ToInt16(((TextBox)grd.FindName("tboxVolMin" + objGruppe + "_" + Convert.ToString(((TextBox)(sender)).Tag))).Text) > playlisttitel[0].VolumeMax)
                            ((TextBox)grd.FindName("tboxVolMin" + objGruppe + "_" + Convert.ToString(((TextBox)(sender)).Tag))).Text = Convert.ToString(playlisttitel[0].VolumeMax);
                    }

                    if (((TextBox)(sender)).Name.StartsWith("tboxPauseMin"))
                    {
                        playlisttitel[0].PauseMin = Convert.ToInt16(((TextBox)(sender)).Text);

                        if (Convert.ToInt16(((TextBox)grd.FindName("tboxPauseMin" + objGruppe + "_" + Convert.ToString(((TextBox)(sender)).Tag))).Text) > playlisttitel[0].PauseMax)
                            ((TextBox)grd.FindName("tboxPauseMin" + objGruppe + "_" + Convert.ToString(((TextBox)(sender)).Tag))).Text = Convert.ToString(playlisttitel[0].PauseMax);
                    }
                    if (((TextBox)(sender)).Name.StartsWith("tboxPauseMax"))
                    {
                        playlisttitel[0].PauseMax = Convert.ToInt16(((TextBox)(sender)).Text);

                        if (Convert.ToInt16(((TextBox)grd.FindName("tboxPauseMax" + objGruppe + "_" + Convert.ToString(((TextBox)(sender)).Tag))).Text) < playlisttitel[0].PauseMin)
                            ((TextBox)grd.FindName("tboxPauseMax" + objGruppe + "_" + Convert.ToString(((TextBox)(sender)).Tag))).Text = Convert.ToString(playlisttitel[0].PauseMin);
                    }

                    Global.ContextAudio.Update<Audio_Playlist_Titel>(playlisttitel[0]);
                }
            }
        }

        private void _btnVolMinMinus0_X_Click(object sender, RoutedEventArgs e)
        {
            string s = ((Button)sender).Name;
            string s_tbox = "tbox";
            string s_sld = "sldKlang";
            if (s.StartsWith("_btnVol"))
            {
                s_tbox = s_tbox + "Vol";
                s_sld = s_sld + "Vol";
                s = s.Substring(7);
            }
            else
            {
                s_tbox = s_tbox + "Pause";
                s_sld = s_sld + "Pause";
                s = s.Substring(9);
            }
            if (s.StartsWith("Min"))
                s_tbox = s_tbox + "Min";
            else
                s_tbox = s_tbox + "Max";
            s = s.Substring(3);
            if (s.StartsWith("Minus"))
                s = s.Substring(5);
            else
                s = s.Substring(4);

            int zeile;
            int seite = tcKlang.SelectedIndex;
            if (seite < 10)
                zeile = Convert.ToInt32(s.Substring(2));
            else
                zeile = Convert.ToInt32(s.Substring(3));

            StackPanel spnl = (StackPanel)((Grid)((Button)sender).Parent).Parent;
            UInt16 objGruppe = Convert.ToUInt16(SeiteXHatObjGruppe[seite]);

            TextBox tbox = (TextBox)spnl.FindName(s_tbox + objGruppe + "_" + zeile);
            Slider sld = (Slider)spnl.FindName(s_sld + objGruppe + "_" + zeile);

            int wert = Convert.ToInt32(((Button)sender).Tag);
            if (wert < 0)
            {
                if (Convert.ToInt32(tbox.Text) + wert <= sld.Maximum)
                {
                    if (Convert.ToInt32(tbox.Text) + Convert.ToInt32(((Button)sender).Tag) > sld.Maximum)
                        tbox.Text = Convert.ToString(sld.Maximum);
                    else
                    {
                        if (Convert.ToInt32(tbox.Text) + Convert.ToInt32(((Button)sender).Tag) < 0)
                            tbox.Text = "0";
                        else
                            tbox.Text = (Convert.ToInt32(tbox.Text) + Convert.ToInt32(((Button)sender).Tag)).ToString();
                    }
                }
                else
                    tbox.Text = sld.Minimum.ToString();
            }
            else
            {
                if (Convert.ToInt32(tbox.Text) + wert >= sld.Minimum)
                {
                    if (Convert.ToInt32(tbox.Text) + Convert.ToInt32(((Button)sender).Tag) < 0)
                        tbox.Text = "0";
                    else
                    {
                        if (Convert.ToInt32(tbox.Text) + Convert.ToInt32(((Button)sender).Tag) > sld.Maximum)
                            tbox.Text = Convert.ToString(sld.Maximum);
                        else
                            tbox.Text = (Convert.ToInt32(tbox.Text) + Convert.ToInt32(((Button)sender).Tag)).ToString();
                    }
                }
                else
                    tbox.Text = sld.Maximum.ToString();
            }
        }

        private void btnKlangStop0_Click(object sender, RoutedEventArgs e)
        {
            if (laufendeSongs[tcKlang.SelectedIndex] != null)
            {
                for (int i = 0; i < laufendeSongs[tcKlang.SelectedIndex].Count(); i++)
                    if (((ToggleButton)sender).IsChecked == true)
                        _player[tcKlang.SelectedIndex][laufendeSongs[tcKlang.SelectedIndex][i]].Pause();
                    else
                        _player[tcKlang.SelectedIndex][laufendeSongs[tcKlang.SelectedIndex][i]].Play();
            }
        }

        private void tboxklangsongparallel_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AktKlangPlaylist != null && tcKlang.SelectedIndex >= 0) 
            {
                try
                {
                    if (Convert.ToInt32(tboxklangsongparallel.Text) >= 0 && Convert.ToInt32(tboxklangsongparallel.Text) != AktKlangPlaylist.MaxSongsParallel)
                    {
                        if (Convert.ToInt32(tboxklangsongparallel.Text) > AktKlangPlaylist.Audio_Playlist_Titel.Count)
                            tboxklangsongparallel.Text = AktKlangPlaylist.Audio_Playlist_Titel.Count.ToString();
                        AktKlangPlaylist.MaxSongsParallel = Convert.ToInt32(tboxklangsongparallel.Text);
                        maxsongsparallel[tcKlang.SelectedIndex] = AktKlangPlaylist.MaxSongsParallel;

                        CheckPlayStandbySongs(Convert.ToUInt16(SeiteXHatObjGruppe[tcKlang.SelectedIndex]));

                        try { Global.ContextAudio.Update<Audio_Playlist>(AktKlangPlaylist); }
                        catch (Exception ex)
                        {
                            var errWin = new MsgWindow("Datenfehler", "Die Datenbank konnte nicht aktualisiert werden", ex);
                            errWin.ShowDialog();
                            errWin.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errWin = new MsgWindow("Eingabefehler", "Ungültige Eingabe. Bitte geben Sie nur Ganzzahlwert ein.", ex);
                    errWin.ShowDialog();
                    errWin.Close();
                    tboxklangsongparallel.Text = AktKlangPlaylist.MaxSongsParallel.ToString();
                    maxsongsparallel[tcKlang.SelectedIndex] = AktKlangPlaylist.MaxSongsParallel;
                }
            }
        }

        private void btnSongParPlus_Click(object sender, RoutedEventArgs e)
        {
            int dif = Convert.ToInt32(((Button)sender).Tag);
            int momentan = Convert.ToInt32(tboxklangsongparallel.Text);
            int max = Convert.ToInt32(tboxklangsongparallel.Tag);

            if ((dif > 0 && dif + momentan <= max) ||
               ((dif < 0 && dif + momentan >= 0)))
            {
                tboxklangsongparallel.Text = (Convert.ToInt32(tboxklangsongparallel.Text) + dif).ToString();
                maxsongsparallel[tcKlang.SelectedIndex] = Convert.ToInt32(tboxklangsongparallel.Text);
            }
        }

        private void btnPlaylistLoeschen_Click(object sender, RoutedEventArgs e)
        {
            List<Audio_Playlist> playlistliste = Global.ContextAudio.PlaylistListe.Where(t => t.Name.Equals((lbKlang.SelectedItem as ListBoxItem).Content)).ToList();
            if (playlistliste.Count != 0)
            {
                if (AktKlangPlaylist != null && AktKlangPlaylist.Name == playlistliste[0].Name)
                {
                    for (UInt16 i = 0; i <= lbKlang.Items.Count - 1; i++)
                    {
                        if (((ListBoxItem)lbKlang.Items[i]).Content.ToString() == playlistliste[0].Name)
                        {
                            UInt16 objGruppe = Convert.ToUInt16(SeiteXHatObjGruppe[tcKlang.SelectedIndex]);

                            PlaylisteLeeren(objGruppe);

                            tboxPlaylistName.Text = "NeuePlayliste" + tcKlang.SelectedIndex;
                            if (tcKlang.SelectedIndex == 0)
                                ((TabItem)tcKlang.SelectedItem).Header = tboxPlaylistName.Text;
                            else
                                ((TabItemControl)tcKlang.SelectedItem)._textBlockTitel.Text = tboxPlaylistName.Text;

                            tboxklangsongparallel.TextChanged -= new TextChangedEventHandler(tboxklangsongparallel_TextChanged);
                            tboxklangsongparallel.Tag = null;
                            tboxklangsongparallel.Text = "0";
                            maxsongsparallel[tcKlang.SelectedIndex] = 0;
                            tboxklangsongparallel.TextChanged += new TextChangedEventHandler(tboxklangsongparallel_TextChanged);

                            zeilenAufSeite[tcKlang.SelectedIndex] = 0;
                            ZeigeKlangSongsParallel(false);

                            ((Grid)this.FindName("grdKlangTop" + objGruppe)).Visibility = Visibility.Hidden;
                        }
                    }
                    AktKlangPlaylist = null;
                }

                if (AktHintergrundPlaylist != null && AktHintergrundPlaylist.Name == playlistliste[0].Name)
                {
                    HintergrundProgBarTimer.Stop();
                    if (HintergrundPlayer != null)
                    {
                        HintergrundPlayer.Stop();
                        HintergrundPlayer = null;
                        btnBGAbspielen.IsEnabled = false;
                        AktHintergrundPlaylist = null;
                        lbhintergrundtitellist.Items.Clear();
                    }
                    HintergrundSongInfo(Visibility.Hidden);
                }

                try
                {
                    List<Audio_Titel> titel = Global.ContextAudio.LoadTitelByPlaylist(playlistliste[0]);
                    if (Global.ContextAudio.Delete<Audio_Playlist>(playlistliste[0]))
                    {
                        for (int i = 0; i < titel.Count - 1; i++)
                        {
                            List<Audio_Playlist> plistliste = Global.ContextAudio.PlaylistListe;
                            int anz = 0;
                            bool gefunden = false;
                            while (anz < plistliste.Count && !gefunden)
                            {

                                if ((Global.ContextAudio.LoadPlaylist_TitelByPlaylist(plistliste[anz], titel[i])).Count > 0)
                                    gefunden = true;
                                else
                                    anz++;
                            }
                            if (!gefunden)
                                Global.ContextAudio.RemoveTitel(titel[i]);
                        }
                        AktualisiereKlangPlaylist();
                    }
                }
                catch (Exception ex)
                {
                    var errWin = new MsgWindow("Playlist Fehler", "Die Playliste konnte nicht erfolgreich gelöscht werden", ex);
                    errWin.ShowDialog();
                    errWin.Close();
                }
            }
        }

        private void sldKlangVol0_X_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_player != null)
            {
                string c;
                if (((Slider)sender).Name.StartsWith("sldKlangVol"))
                    c = ((Slider)sender).Name.Substring(11);
                else
                    c = ((Slider)sender).Name.Substring(13);
                char[] Separator = new char[] { '_' };
                string[] werte = c.Split(Separator, StringSplitOptions.None);

                int objGruppe = Convert.ToInt32(werte[0]);
                int reihe = Convert.ToInt32(werte[1]);
                int seite = Array.IndexOf(SeiteXHatObjGruppe, objGruppe);

                if (((Slider)sender).Name.StartsWith("sldKlangVol", StringComparison.CurrentCulture))
                {
                    if (_player[seite].Length - 1 >= reihe && _player[seite][reihe] != null)
                        _player[seite][reihe].Volume = Math.Round(e.NewValue) / 100;
                    ((Slider)sender).ToolTip = Math.Round(e.NewValue) + " %";
                }
                else
                    ((Slider)sender).ToolTip = Math.Round(e.NewValue) + " ms";

                int zeile = Convert.ToInt32(((Slider)sender).Tag);

                string s;
                if (seite > 0)
                    s = ((TabItemControl)tcKlang.FindName("tiKlang" + objGruppe))._textBlockTitel.Text.ToString();
                else
                    s = ((TabItem)tcKlang.FindName("tiKlang" + objGruppe)).Header.ToString();

                List<Audio_Playlist> playlistliste = Global.ContextAudio.PlaylistListe.Where(t => t.Name.Equals(s)).ToList();

                List<Audio_Titel> titel = Global.ContextAudio.LoadTitelByPlaylist(playlistliste[0]);
                List<Audio_Playlist_Titel> playlisttitel = Global.ContextAudio.LoadPlaylist_TitelByPlaylist(playlistliste[0], titel[zeile]);

                if (playlisttitel.Count != 0)
                {
                    if (((Slider)sender).Name.StartsWith("sldKlangVol", StringComparison.CurrentCulture))
                        playlisttitel[0].Volume = Convert.ToInt32(Math.Round(((Slider)sender).Value));
                    else
                        playlisttitel[0].Pause = Convert.ToInt32(Math.Round(((Slider)sender).Value));

                    try
                    {
                        Global.ContextAudio.Update<Audio_Playlist_Titel>(playlisttitel[0]);
                    }
                    catch (Exception ex)
                    {
                        var errWin = new MsgWindow("Datenfehler", "Die Datenbank konnte nicht aktualisiert werden", ex);
                        errWin.ShowDialog();
                        errWin.Close();
                    }
                }
            }
        }

        private void btnAllVolUp_Click(object sender, RoutedEventArgs e)
        {
            int seite = tcKlang.SelectedIndex;
            int zeile = 0;
            UInt16 objGruppe = Convert.ToUInt16(seite);

            StackPanel spnlZeile;
            while (zeile >= 0)
            {
                spnlZeile = (StackPanel)this.FindName("spnlKlangRow" + objGruppe + "_" + zeile);

                if (spnlZeile != null)
                {    
                    Slider sld;
                    if (((Button)e.Source).Name.StartsWith("btnTopVol"))
                        sld = (Slider)spnlZeile.FindName("sldKlangVol" + objGruppe + "_" + zeile);
                    else
                        sld = (Slider)spnlZeile.FindName("sldKlangPause" + objGruppe + "_" + zeile);
            
                    if ((spnlZeile.FindName("chkTitel" + objGruppe + "_" + zeile) as CheckBox).IsChecked == true)
                        sld.Value = sld.Value + Convert.ToInt32(((sender) as Button).Tag);
                    zeile++;
                }
                else
                    zeile = -1;
            }
        }

        private void tboxPlaylistName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (((TextBox)(sender)).Background != null && AktKlangPlaylist != null)
            {
                ((TextBox)(sender)).Text = AktKlangPlaylist.Name;
                ((TextBox)(sender)).Background = null;
            }
        }

        private void tiHintergrund_GotFocus(object sender, RoutedEventArgs e)
        {
            AktualisiereHintergrundPlaylist();
        }

        private void btnKlangOpen_Click(object sender, RoutedEventArgs e)
        {
            // Konfiguren des Öffnen Ddialogs
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.Multiselect = true;
            dlg.DefaultExt = ".mp3;.wav;.wma;.ogg"; // Extensionen
            dlg.Filter = "Alle Musikdateien |*.mp3;*.wav;*.wma;*.ogg|MP3-Dateien|*.mp3|Wave-Dateien|*.wav|Windows Media Player-Dateien|*.wma|OGG-Dateien|*.ogg"; // Filter Dateien pro extension

            // Zeige File-Öffnen Dialog
            Nullable<bool> result = dlg.ShowDialog();

            // Öffnen bestätigt
            if (result == true)
            {
                // Öffne das Dokument
                string filename = dlg.FileName;
                bool hinzugefuegt = false;
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    if (dlg.FileNames.Length != 0)
                    {
                        if (AktKlangPlaylist == null)
                            NeueKlangPlaylistInDB();

                        string[] extension = new String[4] { ".mp3", ".wav", ".wma", ".ogg" };

                        foreach (string dateihinzu in dlg.FileNames)
                        {
                            if (Array.IndexOf(extension, dateihinzu.Substring(dateihinzu.Length - 4)) != -1)
                            {
                                KlangDateiHinzu(dateihinzu);
                                hinzugefuegt = true;
                            }
                        }
                        if (hinzugefuegt)
                        {
                            UInt16 objGruppe = Convert.ToUInt16(SeiteXHatObjGruppe[tcKlang.SelectedIndex]);
                            ((Grid)this.FindName("grdKlangTop" + objGruppe)).Visibility = Visibility.Visible;


                            if (rbIstKlangPlaylist.IsChecked == true)
                                AktKlangPlaylist.Hintergrundmusik = false;
                            else
                                AktKlangPlaylist.Hintergrundmusik = true;

                            if (AktKlangPlaylist.Hintergrundmusik)
                            {
                                ZeigeKlangSongsParallel(false);
                                ZeigeKlangTop(objGruppe, false);
                            }
                            else
                            {
                                ZeigeKlangSongsParallel(true);
                                ZeigeKlangTop(objGruppe, true);
                            }
                        }
                    }
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                    Global.ContextAudio.Save();
                    int i = tcKlang.SelectedIndex;
                    tcKlang.SelectedIndex = -1;
                    tcKlang.SelectedIndex = i;
                }
            }
        }

        private void image1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CustomMessage("Add-On Hinweis", "OGG-Dateien integrieren",
                "OGG-Dateien können nach dem installieren eines entsprechenden AddOns" + Environment.NewLine + "ebenfallse abgespielt werden." +
                Environment.NewLine + Environment.NewLine + "Ein AddOn wäre zum Beispiel das Programm 'WMPTagSupport11' und " + Environment.NewLine +
                "kann unter folgender Adresse heruntergeladen werden:" + Environment.NewLine + Environment.NewLine,
                "http://www.softpointer.com/downloads/WMPTagSupport11.exe");
        }


        private void btnClick(object sender, RoutedEventArgs e)
        {
            ((((Button)(sender)).Parent as Grid).Parent as Window).Close();
        }

        private void CustomMessage(string s_titel, string s_top, string s_mitte, string s_url)
        {
            Window w = new Window();
            Grid grd = new Grid();
            Border brd = new Border();
            TextBlock txTop = new TextBlock();
            TextBlock tx = new TextBlock();
            Paragraph parx = new Paragraph();
            Button btn = new Button();
            ScrollViewer scr = new ScrollViewer();

            RowDefinition rowDef1 = new RowDefinition();
            RowDefinition rowDef2 = new RowDefinition();
            RowDefinition rowDef3 = new RowDefinition();
            rowDef1.Height = new GridLength(55);
            rowDef2.Height = new GridLength(1, GridUnitType.Star);
            rowDef3.Height = new GridLength(65);

            grd.RowDefinitions.Add(rowDef1);
            grd.RowDefinitions.Add(rowDef2);
            grd.RowDefinitions.Add(rowDef3);

            brd.Background = Brushes.Lavender;
            grd.Children.Add(brd);
            Grid.SetRow(brd, 0);
            txTop.Text = s_top;
            txTop.FontSize = 16;
            txTop.Margin = new Thickness(10);
            grd.Children.Add(txTop);
            Grid.SetRow(txTop, 0);

            grd.Children.Add(scr);
            Grid.SetRow(scr, 1);

            Run run1 = new Run(s_mitte);
            run1.FontSize = 12;
            run1.TextDecorations = null;
            run1.FontWeight = FontWeights.Normal;

            tx.TextWrapping = TextWrapping.Wrap;
            tx.Margin = new Thickness(10);
            tx.Inlines.Add(run1);

            if (s_url != null)
            {
                Run run2 = new Run(s_url);
                Hyperlink hyperl = new Hyperlink(run2);
                hyperl.NavigateUri = new Uri(s_url);
                tx.Inlines.Add(hyperl);
            }

            scr.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scr.Content = tx;

            btn.Content = "OK";
            btn.MinWidth = 50;
            btn.Margin = new Thickness(10);
            btn.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            btn.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            btn.Click += new RoutedEventHandler(btnClick);

            grd.Children.Add(btn);
            Grid.SetRow(btn, 2);

            w.Content = grd;
            w.Title = s_titel;
            w.MinHeight = 300;
            w.MinWidth = 250;
            w.Width = 450;
            w.Height = 300;
            w.Show();
        }

        private void chkbxTopVolChange0_Click(object sender, RoutedEventArgs e)
        {
            int seite = tcKlang.SelectedIndex;
            int zeile = 0;
            UInt16 objGruppe = Convert.ToUInt16(seite);

            StackPanel spnlZeile;
            while (zeile >= 0)
            {
                spnlZeile = (StackPanel)this.FindName("spnlKlangRow" + objGruppe + "_" + zeile);

                if (spnlZeile != null)
                {
                    CheckBox chChange;
                    if (((CheckBox)e.Source).Name.StartsWith("chkbxTopVol"))
                        chChange = (CheckBox)spnlZeile.FindName("chkVolMove" + objGruppe + "_" + zeile);
                    else
                        chChange = (CheckBox)spnlZeile.FindName("chkKlangPauseMove" + objGruppe + "_" + zeile);

                    if ((spnlZeile.FindName("chkTitel" + objGruppe + "_" + zeile) as CheckBox).IsChecked == true)
                    {
                        chChange.IsChecked = ((CheckBox)sender).IsChecked;
                        chkVolMove0_0_Click(chChange, new RoutedEventArgs());
                    }
                    zeile++;
                }
                else
                    zeile = -1;
            }
        }

        private void btnTopVolMinMinus0_Click(object sender, RoutedEventArgs e)
        {
            int seite = tcKlang.SelectedIndex;
            int zeile = 0;
            UInt16 objGruppe = Convert.ToUInt16(seite);

            StackPanel spnlZeile;
            while (zeile >= 0)
            {
                spnlZeile = (StackPanel)this.FindName("spnlKlangRow" + objGruppe + "_" + zeile);

                if (spnlZeile != null)
                {
                    Button btnMinus;
                    if (((Button)e.Source).Name.StartsWith("btnTopVol"))
                    {
                        if (((Button)e.Source).Name.StartsWith("btnTopVolMin"))
                            if (((Button)e.Source).Name.StartsWith("btnTopVolMinMinus"))
                                btnMinus = (Button)spnlZeile.FindName("_btnVolMinMinus" + objGruppe + "_" + zeile);
                            else
                                btnMinus = (Button)spnlZeile.FindName("_btnVolMinPlus" + objGruppe + "_" + zeile);
                        else
                            if (((Button)e.Source).Name.StartsWith("btnTopVolMaxMinus"))
                                btnMinus = (Button)spnlZeile.FindName("_btnVolMaxMinus" + objGruppe + "_" + zeile);
                            else
                                btnMinus = (Button)spnlZeile.FindName("_btnVolMaxPlus" + objGruppe + "_" + zeile);
                    }
                    else
                    {
                        if (((Button)e.Source).Name.StartsWith("btnTopPauseMin"))
                            if (((Button)e.Source).Name.StartsWith("btnTopPauseMinMinus"))
                                btnMinus = (Button)spnlZeile.FindName("_btnPauseMinMinus" + objGruppe + "_" + zeile);
                            else
                                btnMinus = (Button)spnlZeile.FindName("_btnPauseMinPlus" + objGruppe + "_" + zeile);
                        else
                            if (((Button)e.Source).Name.StartsWith("btnTopPauseMaxMinus"))
                                btnMinus = (Button)spnlZeile.FindName("_btnPauseMaxMinus" + objGruppe + "_" + zeile);
                            else
                                btnMinus = (Button)spnlZeile.FindName("_btnPauseMaxPlus" + objGruppe + "_" + zeile);
                    }
             
                    if ((spnlZeile.FindName("chkTitel" + objGruppe + "_" + zeile) as CheckBox).IsChecked == true)
                        _btnVolMinMinus0_X_Click(btnMinus, e);
                    
                    zeile++;
                }
                else
                    zeile = -1;
            }
        }
        
        private void chkbxTop0_Click(object sender, RoutedEventArgs e)
        {
            int seite = tcKlang.SelectedIndex;
            int zeile = 0;
            UInt16 objGruppe = Convert.ToUInt16(seite);

            StackPanel spnlZeile;
            while (zeile >= 0)
            {
                spnlZeile = (StackPanel)this.FindName("spnlKlangRow" + objGruppe + "_" + zeile);

                if (spnlZeile != null)
                {
                    (spnlZeile.FindName("chkTitel" + objGruppe + "_" + zeile) as CheckBox).IsChecked = ((CheckBox)(e.Source)).IsChecked;
                    chkTitel0_0_Click((spnlZeile.FindName("chkTitel" + objGruppe + "_" + zeile) as CheckBox), e);
                    zeile++;
                }
                else
                    zeile = -1;
            }
        }
    }
}