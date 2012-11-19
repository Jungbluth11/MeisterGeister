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
using MeisterGeister.View.AudioPlayer;
//using Mp3Lib;

/*
Song title  30 Zeichen
Artist	    30 Zeichen
Album	    30 Zeichen
Year	    4 Zeichen
Comment	    30 Zeichen
Genre	    1 Byte

 * */

public class MyTimer
{
    static int start = 0;
    static int stop = 0;
    public static void start_timer()
    {
        start = Environment.TickCount;
    }

    public static void stop_timer()
    {
        stop_timer("");
    }

    public static void stop_timer(string msg)
    {
        stop = Environment.TickCount;
        print(msg);
    }

    private static void print(string msg)
    {
        string output = "MyTimer(" + msg + "): " + (stop - start) + " Millisekunden";
        System.Diagnostics.Debug.WriteLine(output);
    }
}

public class ThemeGruppe
{
    public string ThemeName;
    public bool IstActiv = false;
    public List<HitchPanel> HintergrundPanel = new List<HitchPanel>();
    public List<HitchPanel> KlangPanel = new List<HitchPanel>();
    public List<GruppenObjekt> Klaenge = new List<GruppenObjekt>();
    public GruppenObjekt Hintergrund = new GruppenObjekt();

    public Audio_Playlist HGThemePlaylist = new Audio_Playlist();
    public AudioTheme pnlAudioTheme = new AudioTheme();
    public Audio_Theme dbAudioTheme = new Audio_Theme();
}

public class KlangZeile 
{
    public UInt16 ID_Zeile;
    public MediaPlayer _mplayer = new MediaPlayer();
    public Audio_Playlist_Titel audiotitel = new Audio_Playlist_Titel();
    public int mediaHashCode = 0;
    public bool istPause = false;
    public bool istLaufend = false;
    public bool istStandby = false;
    public bool playable = true;
    public int pauseMax_wert = 10000;
    public int pauseMin_wert = 0;
    public int volMax_wert = 100;
    public int volMin_wert = 0;

    public StackPanel spnlKlangRow = null;
    public Grid grdKlangRow = null;
    public Image imgTrash = null;
    public ProgressBar pbarTitel = null;    
    public CheckBox chkTitel = null;
    public Label lblDauer = null;
    public Slider sldKlangVol = null;
    public CheckBox chkVolMove = null;
    public TextBox tboxVolMin = null;
    public Button btnVolMinMinus = null;
    public Button btnVolMinPlus = null;
    public TextBox tboxVolMax = null;
    public Button btnVolMaxMinus = null;
    public Button btnVolMaxPlus = null;
    public Slider sldKlangPause = null;
    public CheckBox chkKlangPauseMove = null;
    public TextBox tboxPauseMin = null;
    public Button btnPauseMinMinus = null;
    public Button btnPauseMinPlus = null;
    public TextBox tboxPauseMax = null;
    public Button btnPauseMaxMinus = null;
    public Button btnPauseMaxPlus = null;
    public Slider sldPlaySpeed = null;
       
    public KlangZeile(UInt16 id)
    {
        ID_Zeile = id;
    }
}

public class GruppenObjekt
{
    public int seite;
    public bool WerteGeändert = false;
    public int objGruppe;
    public UInt16 anzTitelAkt = 0;
    public UInt16 anzVolChange = 0;
    public UInt16 anzPauseChange = 0;
    public string playlistName = "";
    public UInt16 maxsongparallel = 0;
    public bool istHintergrundMusik = true;
    public List<KlangZeile> _listZeile = new List<KlangZeile>();
    
    public TabItem tiKlang = null;
    public TabItemControl ticKlang = null;
    public ScrollViewer sviewer;
    public Grid grdKlang = null;
    public Grid grdKlangTop = null;
    public Button btnKlangPause = null;  //Toggle
    public Image btnImgKlangPause = null;

    public CheckBox chkbxTopAktiv = null;
    public Button btnTopVolMin = null;
    public Button btnTopVolDown = null;
    public Button btnTopVolUp = null;
    public Button btnTopVolMax = null;
    public CheckBox chkbxTopVolChange = null;
    public Button btnTopPauseMin = null;
    public Button btnTopPauseDown = null;
    public Button btnTopPauseUp = null;
    public Button btnTopPauseMax = null;
    public CheckBox chkbxTopPauseChange = null;
    public Button btnTopVolMinMinus = null;
    public Button btnTopVolMinPlus = null;
    public Button btnTopVolMaxMinus = null;
    public Button btnTopVolMaxPlus = null;
    public Button btnTopPauseMinMinus = null;
    public Button btnTopPauseMinPlus = null;
    public Button btnTopPauseMaxMinus = null;
    public Button btnTopPauseMaxPlus = null;
}



namespace MeisterGeister.View.AudioPlayer {
    /// <summary>
    /// Interaktionslogik für AudioPlayerView.xaml
    /// </summary>
    /// 

    delegate void updateBGSongCallback(MediaPlayer _player, string tekst);

    public partial class AudioPlayerView : UserControl     {
        private UInt16 tiErstellt = 0;
        private UInt16 rowErstellt = 0;

        private Int16 PauseSprung = 200;
        private Int16 VolSprung = 5;
        private string orgStackString;
        MediaPlayer HintergrundPlayer;
     
        public List<GruppenObjekt> _GrpObjecte = new List<GruppenObjekt>();
        public List<ThemeGruppe> _ThemeGruppe = new List<ThemeGruppe>();
        public int aktiveThemeGruppe = -1;
        
        private Audio_Playlist AktHintergrundPlaylist, AktKlangPlaylist;
        private Audio_Playlist_Titel AktBGPlaylistTitel;
        private ThemeGruppe AktThemeGruppe = new ThemeGruppe();

        System.Timers.Timer BGSongTimer = new System.Timers.Timer();
        DispatcherTimer KlangPlayEndetimer;
        DispatcherTimer KlangProgBarTimer = new DispatcherTimer();
        DispatcherTimer HintergrundProgBarTimer = new DispatcherTimer();

        List<HitchPanel> ThemeLstBGPanel = new List<HitchPanel>();
        List<HitchPanel> ThemeLstKlangPanel = new List<HitchPanel>();
                
        public AudioPlayerView()
        {
            InitializeComponent();
            
            KlangProgBarTimer.Tick += new EventHandler(KlangProgBarTimer_Tick);
            KlangProgBarTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            KlangProgBarTimer.Tag = 0;

            HintergrundProgBarTimer.Tick += new EventHandler(HintergrundProgBarTimer_Tick);
            HintergrundProgBarTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);

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

        public MediaPlayer PlayFile(int seite, int zeile, int posObjGruppe, MediaPlayer _player, String url, double vol)
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
                        _player.MediaEnded += new EventHandler(Player_Ended);
                        _player.SpeedRatio = _GrpObjecte[posObjGruppe]._listZeile[zeile].sldPlaySpeed.Value;
                    }
                    else
                        _player.MediaEnded += new EventHandler(HintergrundPlayer_Ended);
                }
                catch (Exception ex2)
                {
                    ListBoxItem lbItem = (ListBoxItem)lbhintergrundtitellist.SelectedItem;
                    lbItem.Background = Brushes.Yellow;
                    lbItem.ToolTip = "Datei konnte nicht geöffnet werden (Datei vorhanden?)" + ex2;
                    SpieleNeuenHintergrundTitel(-1);
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

        void CheckPlayStandbySongs(int posObjGruppe)
        {            
            if (posObjGruppe == -1)
                return;
            
            int laufende = _GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.istLaufend).Count;

            List <KlangZeile> klZeilenStandby = _GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.istStandby);
            List<KlangZeile> klZeilenStandbyNichtPause = klZeilenStandby.FindAll(t => t.istPause == false);
            int standbyNichtPause = klZeilenStandbyNichtPause.Count;

            if ((laufende == 0 && standbyNichtPause != 0) ||
               (laufende != 0 && standbyNichtPause != 0 && _GrpObjecte[posObjGruppe].maxsongparallel > laufende))
            {                
                int neueSongs;
                if (laufende == 0)
                    neueSongs = _GrpObjecte[posObjGruppe].maxsongparallel; 
                else
                    neueSongs = _GrpObjecte[posObjGruppe].maxsongparallel - laufende;

                if (neueSongs == 0 && _GrpObjecte[posObjGruppe].maxsongparallel == 0)
                    neueSongs = 1;

                for (int i = 0; i < neueSongs; i++)
                {
                    if (standbyNichtPause >= 1)
                    {
                        Würfel w = new Würfel(Convert.ToUInt16(standbyNichtPause));
                        w.Würfeln(1);
                        int zuspielendersong = _GrpObjecte[posObjGruppe]._listZeile.IndexOf(klZeilenStandbyNichtPause[w.Ergebnis-1]);
                        _GrpObjecte[posObjGruppe]._listZeile[zuspielendersong].istStandby = false;

                        // Titel anstarten
                       // StackPanel spnlZeile = _GrpObjecte[posObjGruppe]._listZeile[zuspielendersong].spnlKlangRow;
                        if (_GrpObjecte[posObjGruppe]._listZeile[zuspielendersong].chkTitel != null)
                            chkTitel0_0_Click(_GrpObjecte[posObjGruppe]._listZeile[zuspielendersong].chkTitel, new RoutedEventArgs());

                        standbyNichtPause--;
                        klZeilenStandbyNichtPause.RemoveAt(w.Ergebnis - 1);
                    }
                }
            }
        }

        void KlangPlayEndetimer_Tick(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();

            string s = (sender as DispatcherTimer).Tag.ToString();
            char[] Separator = new char[] { '_' };
            string[] werte = s.Split(Separator, StringSplitOptions.None);

            UInt16 objGruppe = Convert.ToUInt16(werte[0]);
            UInt16 zeile = Convert.ToUInt16(werte[1]);

            int posObjGruppe = GetPosObjGruppe(objGruppe);
            _GrpObjecte[posObjGruppe]._listZeile[zeile].pbarTitel.Value = 0;
            if (_GrpObjecte[posObjGruppe]._listZeile[zeile].chkKlangPauseMove.IsChecked == true)
            {
                Würfel w = new Würfel(Convert.ToUInt16(Convert.ToUInt16(_GrpObjecte[posObjGruppe]._listZeile[zeile].tboxPauseMax.Text) - 
                                            Convert.ToUInt16(_GrpObjecte[posObjGruppe]._listZeile[zeile].tboxPauseMin.Text)));
                w.Würfeln(1);
                _GrpObjecte[posObjGruppe]._listZeile[zeile].sldKlangPause.Value = w.Ergebnis-1;
            }
            // Song aus der Liste der laufenden Songs herausnehmen
            _GrpObjecte[posObjGruppe]._listZeile[zeile].istLaufend = false;

            // Song in die Liste der Standby-Songs aufnehmen wenn nur ein Song in Liste
            if (_GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.istStandby).Count == 0)
            {
                _GrpObjecte[posObjGruppe]._listZeile[zeile].istStandby = true;
                CheckPlayStandbySongs(posObjGruppe);
            }
            else
            {
                // Song in die Liste der Standby-Songs aufnehmen wenn nur mehere Songs verfügbar
                // somit wird nicht 2x der gleiche Song gespielt
                CheckPlayStandbySongs(posObjGruppe);
                _GrpObjecte[posObjGruppe]._listZeile[zeile].istStandby = true;
            }
        }
        
        void HintergrundPlayer_Ended(object sender, EventArgs e)
        {
            (sender as MediaPlayer).Stop();
            lbhintergrundtitellist.Tag = lbhintergrundtitellist.SelectedIndex;
            if (btnBGRepeat.IsChecked.Value)
                (sender as MediaPlayer).Play();
            else
            {
                HintergrundProgBarTimer.Stop();
                SpieleNeuenHintergrundTitel(-1);
            }
        }

        void Player_HintergrundMediaFailed(object sender, ExceptionEventArgs e)
        {
            (sender as MediaPlayer).Stop();
            HintergrundProgBarTimer.Stop();
            ListBoxItem lbItem = (ListBoxItem)lbhintergrundtitellist.SelectedItem;
            lbItem.Background = Brushes.Yellow;
            lbItem.ToolTip = "Datei kann nicht abgespielt werden. Falscher oder nicht kompatibler Typ (" + (sender as MediaPlayer).Source.LocalPath + ")";
            SpieleNeuenHintergrundTitel(-1);
        }

        void Player_Ended(object sender, EventArgs e)
        {
            int posObjGruppe = 0; 

            while (posObjGruppe < _GrpObjecte.Count && 
                !_GrpObjecte[posObjGruppe]._listZeile.Exists(t => t.mediaHashCode.Equals((sender as MediaPlayer).GetHashCode())))
                posObjGruppe++;
            if (posObjGruppe < _GrpObjecte.Count)
            {
                int zeile = _GrpObjecte[posObjGruppe]._listZeile.FindIndex(t => t.mediaHashCode.Equals((sender as MediaPlayer).GetHashCode()));
                                
                int objGruppe = _GrpObjecte[posObjGruppe].objGruppe;
                if (objGruppe == -1)
                    return;

                KlangPlayEndetimer = new DispatcherTimer();
                KlangPlayEndetimer.Interval = TimeSpan.FromMilliseconds(_GrpObjecte[posObjGruppe]._listZeile[zeile].sldKlangPause.Value);
                KlangPlayEndetimer.Tick += new EventHandler(KlangPlayEndetimer_Tick);
                KlangPlayEndetimer.Tag = objGruppe + "_" + zeile;  
                KlangPlayEndetimer.Start();
            }
            App.CloseSplashScreen();
        }


        void Player_KlangMediaFailed(object sender, ExceptionEventArgs e)
        {
            char[] Separator = new char[] { '_' };

            int mediahash = (sender as MediaPlayer).GetHashCode();
            int posObjGruppe = 0;

            while (posObjGruppe < _GrpObjecte.Count &&
                   !_GrpObjecte[posObjGruppe]._listZeile.Exists(t => t.mediaHashCode.Equals(mediahash)))
                posObjGruppe++;
            if (posObjGruppe < _GrpObjecte.Count)
            {
                int zeile = _GrpObjecte[posObjGruppe]._listZeile.FindIndex(t => t.mediaHashCode.Equals(mediahash));

                int objGruppe = _GrpObjecte[posObjGruppe].objGruppe;
                if (objGruppe == -1)
                    return;

                _GrpObjecte[posObjGruppe]._listZeile[zeile].spnlKlangRow.Background = Brushes.Yellow;
                _GrpObjecte[posObjGruppe]._listZeile[zeile].spnlKlangRow.ToolTip = "Datei kann nicht abgespielt werden (Falscher oder nicht kompatibler Typ).";
                _GrpObjecte[posObjGruppe]._listZeile[zeile]._mplayer.Stop();
                _GrpObjecte[posObjGruppe]._listZeile[zeile]._mplayer = null;

                _GrpObjecte[posObjGruppe]._listZeile[zeile].istPause = false;
                _GrpObjecte[posObjGruppe]._listZeile[zeile].istLaufend = false;
                _GrpObjecte[posObjGruppe]._listZeile[zeile].playable = false;
                CheckPlayStandbySongs(posObjGruppe);
            }
        }

        private void KlangProgBarTimer_Tick(object sender, EventArgs e)
        {
            if (KlangProgBarTimer.Tag.ToString() == "0")
                KlangProgBarTimer.Tag = "1";
            else
                KlangProgBarTimer.Tag = "0";
            for (int posObjGruppe = 0; posObjGruppe < _GrpObjecte.Count; posObjGruppe++)
            {
                List<KlangZeile> KlangZeilenLaufend = _GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.istLaufend);

                if (KlangZeilenLaufend.Count != 0)
                {
               //     int zeile;
                    for (int durchlauf = 0; durchlauf < KlangZeilenLaufend.Count; durchlauf++)
                    {
                        if (KlangZeilenLaufend[durchlauf].istPause)
                            continue;
                        int objGruppe = _GrpObjecte[posObjGruppe].objGruppe;
                        if (objGruppe == -1)
                            break;

                        if (KlangZeilenLaufend[durchlauf].pbarTitel != null &&
                            KlangZeilenLaufend[durchlauf].pbarTitel.Tag != null)
                        {
                            DateTime dtJetzt = DateTime.Now;
                            DateTime dtVorher = Convert.ToDateTime(KlangZeilenLaufend[durchlauf].pbarTitel.Tag);

                            TimeSpan diffResult = dtJetzt - dtVorher;
                            if (diffResult.TotalSeconds > 1)
                            {
                                if (!KlangZeilenLaufend[durchlauf]._mplayer.HasAudio)
                                {
                                    KlangZeilenLaufend[durchlauf].spnlKlangRow.Background = Brushes.Yellow;
                                    KlangZeilenLaufend[durchlauf].spnlKlangRow.ToolTip = "Datei kann nicht abgespielt werden";
                                    KlangZeilenLaufend[durchlauf]._mplayer.Stop();
                                    KlangZeilenLaufend[durchlauf]._mplayer = null;
                                    KlangZeilenLaufend[durchlauf].istLaufend = false;
                                    KlangZeilenLaufend[durchlauf].istPause = false;
                                    CheckPlayStandbySongs(posObjGruppe);
                                }
                                else
                                    KlangZeilenLaufend[durchlauf].spnlKlangRow.Background = null;
                            }
                        }
                        if (KlangZeilenLaufend[durchlauf]._mplayer != null &&
                            KlangZeilenLaufend[durchlauf]._mplayer.HasAudio == false &&
                            KlangZeilenLaufend[durchlauf]._mplayer.BufferingProgress == 1)
                            KlangZeilenLaufend[durchlauf].pbarTitel.Tag = DateTime.Now;
                        else
                            KlangZeilenLaufend[durchlauf].pbarTitel.Tag = null;

                        if (KlangProgBarTimer.Tag.ToString() == "0")
                        {
                            if (KlangZeilenLaufend[durchlauf].chkTitel.IsChecked == true &&
                                KlangZeilenLaufend[durchlauf].pbarTitel != null &&
                                KlangZeilenLaufend[durchlauf]._mplayer != null)
                            {
                                if (KlangZeilenLaufend[durchlauf].chkVolMove.IsChecked == true)
                                {
                                    uint v = 5;
                                    if (KlangZeilenLaufend[durchlauf].sldKlangVol.Value >= 
                                        Convert.ToUInt32(KlangZeilenLaufend[durchlauf].tboxVolMax.Text) ||
                                        KlangZeilenLaufend[durchlauf].sldKlangVol.Value <= Convert.ToUInt32(
                                        KlangZeilenLaufend[durchlauf].tboxVolMin.Text))
                                        v = 3;
                                    Würfel w = new Würfel(v);
                                    w.Würfeln(1);
                                    if (KlangZeilenLaufend[durchlauf].sldKlangVol.Value <= Convert.ToUInt32(
                                            KlangZeilenLaufend[durchlauf].tboxVolMin.Text))
                                        KlangZeilenLaufend[durchlauf].sldKlangVol.Value = 
                                            KlangZeilenLaufend[durchlauf].sldKlangVol.Value + (w.Ergebnis - 1) * 2;
                                    else
                                        KlangZeilenLaufend[durchlauf].sldKlangVol.Value += (w.Ergebnis - 3) * 2;
                                }
                                if (KlangZeilenLaufend[durchlauf].pbarTitel.Maximum == 10000 && KlangZeilenLaufend[durchlauf]._mplayer.NaturalDuration.HasTimeSpan)
                                {
                                    KlangZeilenLaufend[durchlauf].pbarTitel.Maximum = KlangZeilenLaufend[durchlauf]._mplayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                                    if (aktiveThemeGruppe >= 0 && 
                                        _GrpObjecte[posObjGruppe].playlistName == _ThemeGruppe[aktiveThemeGruppe].Hintergrund.playlistName)// KlangZeilenLaufend[durchlauf]. .pbarTitel == _ThemeGruppe[aktiveThemeGruppe].Hintergrund._listZeile[durchlauf].pbarTitel)
                                        _ThemeGruppe[aktiveThemeGruppe].pnlAudioTheme.pbarActBGTitel.Maximum = KlangZeilenLaufend[durchlauf].pbarTitel.Maximum;
                                }


                                if (KlangZeilenLaufend[durchlauf].lblDauer.Content.ToString() == 
                                    "-:--" && KlangZeilenLaufend[durchlauf]._mplayer.NaturalDuration.HasTimeSpan)
                                    KlangZeilenLaufend[durchlauf].lblDauer.Content = 
                                        KlangZeilenLaufend[durchlauf]._mplayer.NaturalDuration.TimeSpan.ToString().Substring(3, 5);

                                KlangZeilenLaufend[durchlauf].pbarTitel.Value = KlangZeilenLaufend[durchlauf]._mplayer.Position.TotalMilliseconds;
                                if (aktiveThemeGruppe >= 0 &&
                                        _GrpObjecte[posObjGruppe].playlistName == _ThemeGruppe[aktiveThemeGruppe].Hintergrund.playlistName)
                                    _ThemeGruppe[aktiveThemeGruppe].pnlAudioTheme.pbarActBGTitel.Value = KlangZeilenLaufend[durchlauf].pbarTitel.Value;
                            }
                        }
                    }
                }
            }
        }

        private void slBGVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VolChanged(HintergrundPlayer, (sender as Slider).Value);
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
                        btnBGAbspielen.Tag = 1;
                        btnBGAbspielen_Click(btnBGAbspielen, new RoutedEventArgs());
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
                        btnBGAbspielen.IsEnabled = true;
                        btnBGAbspielen.Tag = 0;
                        btnBGNext.IsEnabled = true;

                        if (rbtnGleichSpielen.IsChecked.Value)
                        {
                            SpieleNeuenHintergrundTitel(-1);
                            if (titel.Count != 0)
                                grdSongInfo.Visibility = Visibility.Visible;
                            else
                            {
                                btnBGAbspielen.Tag = 1;
                                btnBGAbspielen_Click(btnBGAbspielen, new RoutedEventArgs());
                            }
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
        }

        private void HintergrundSongInfo(Visibility sichtbar)
        {
            for (int i = 0; i <= grdSongInfo.Children.Count - 1; i++)
                grdSongInfo.Children[i].Visibility = sichtbar;
        }


        private void SpieleNeuenHintergrundTitel(int Index)
        {
            if (lbhintergrundtitellist.Items.Count == 1)
            {
                lbhintergrundtitellist.SelectedIndex = -1;
                lbhintergrundtitellist.SelectedIndex = 0;
            }
            else
            {
                chkbxPlayRange.IsChecked = false;
                rsldTeilSong.Visibility = Visibility.Hidden;
                rsldTeilSong.LowerValue = 0;
                rsldTeilSong.UpperValue = 10000;
                if (Index != -1)
                {
                    if (lbhintergrundtitellist.SelectedIndex == Index)
                        lbhintergrundtitellist.SelectedIndex = -1;
                    lbhintergrundtitellist.SelectedIndex = Index;
                    lbhintergrundtitellist.ScrollIntoView(lbhintergrundtitellist.SelectedItem);
                }
                else
                {
                    if (btnShuffle.IsChecked.Value)
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
                    else
                    {
                        int i = lbhintergrundtitellist.SelectedIndex;
                        int startIndex = i;
                        if (i == lbhintergrundtitellist.Items.Count - 1)
                            i = 0;
                        else
                            i++;
                        while (((ListBoxItem)lbhintergrundtitellist.Items[i]).Background == Brushes.Yellow ||
                                ((ListBoxItem)lbhintergrundtitellist.Items[i]).Background == Brushes.Red &&
                                i != startIndex)
                        {
                            if (i != lbhintergrundtitellist.Items.Count - 1)
                                i++;
                            else
                                i = 0;
                        }
                        if (i != startIndex)
                        {
                            lbhintergrundtitellist.SelectedIndex = i;
                            lbhintergrundtitellist.ScrollIntoView(lbhintergrundtitellist.SelectedItem);
                        }
                    }
                }
            }
        }

        private void btnBGSpeaker_Click(object sender, RoutedEventArgs e)
        {
            if (!(Convert.ToBoolean(btnBGSpeaker.Tag)))
                btnBGSpeaker.Tag = true;
            else
                btnBGSpeaker.Tag = false;
            if (HintergrundPlayer != null)
                HintergrundPlayer.IsMuted = Convert.ToBoolean(btnBGSpeaker.Tag);
            if (Convert.ToBoolean(btnBGSpeaker.Tag))
                btnImgBGSpeaker.Source = new BitmapImage(new Uri("pack://application:,,,/DSA MeisterGeister;component/Images/Icons/General/speaker-mute.png"));
            else
                btnImgBGSpeaker.Source = new BitmapImage(new Uri("pack://application:,,,/DSA MeisterGeister;component/Images/Icons/General/speaker.png"));
        }

        private void btnBGAbspielen_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(btnBGAbspielen.Tag) == 0)
            {
                if (HintergrundPlayer != null)
                {
                    HintergrundPlayer.IsMuted = btnBGSpeaker.IsPressed;
                    HintergrundPlayer.Play();
                    btnBGAbspielen.Tag = 1;
                    btnImgBGAbspielen.Source = new BitmapImage(new Uri("pack://application:,,,/DSA MeisterGeister;component/Images/Icons/General/pause.png"));
                }
                else
                    SpieleNeuenHintergrundTitel(-1);
            }
            else
            {
                if (HintergrundPlayer != null)
                    HintergrundPlayer.Pause();
                btnBGAbspielen.Tag = 0;
                btnImgBGAbspielen.Source = new BitmapImage(new Uri("pack://application:,,,/DSA MeisterGeister;component/Images/Icons/General/play.png"));
            }
        }

        private void btnBGStoppen_Click(object sender, RoutedEventArgs e)
        {
            if (HintergrundPlayer != null)
            {
                HintergrundPlayer.Stop();
                HintergrundPlayer = null;
            }
            HintergrundProgBarTimer.Stop();
            btnBGAbspielen.Tag = 0;
            grdSongInfo.Visibility = Visibility.Hidden;
            lbhintergrundtitellist.SelectedIndex = -1;
            btnImgBGStoppen.Source = new BitmapImage(new Uri("pack://application:,,,/DSA MeisterGeister;component/Images/Icons/General/play_stop-grau.png")); 
            btnImgBGAbspielen.Source = new BitmapImage(new Uri("pack://application:,,,/DSA MeisterGeister;component/Images/Icons/General/play.png"));
            btnBGStoppen.IsEnabled = false;
        }

        private void lbBackground_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            lbBackground_SelectionChanged(sender, null);
        }

        private void btnBGNext_Click(object sender, RoutedEventArgs e)
        {
            lbhintergrundtitellist.Tag = lbhintergrundtitellist.SelectedIndex;
            if (btnBGRepeat.IsChecked.Value)
                SpieleNeuenHintergrundTitel(lbhintergrundtitellist.SelectedIndex);
            else
                SpieleNeuenHintergrundTitel(-1);
        }

        private void btnBGPrev_Click(object sender, RoutedEventArgs e)
        {
            SpieleNeuenHintergrundTitel(Convert.ToInt16(lbhintergrundtitellist.Tag));
            lbhintergrundtitellist.Tag = -1;
            btnBGPrev.IsEnabled = false;
        }

        private void lbKlang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbKlang.SelectedIndex != -1)
            {
                btnPlaylistLoeschen.IsEnabled = true;
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    Int16 objGruppe;
                                        
                    if (tcKlang.SelectedIndex == 0)
                        objGruppe = Convert.ToInt16(((TabItem)tcKlang.SelectedItem).Name.Substring(7));
                    else
                        objGruppe = Convert.ToInt16(((TabItemControl)tcKlang.SelectedItem).Name.Substring(7));
                    Int16 posObjGruppe = GetPosObjGruppe(objGruppe);
                                        
                    for (int i = 0; i <= grdKlangPlaylistInfo.Children.Count - 1; i++)
                        if ((grdKlangPlaylistInfo.Children[i] as Control).Name != "btnKlangSave") grdKlangPlaylistInfo.Children[i].Visibility = Visibility.Visible;

                    string s = ((ListBoxItem)lbKlang.Items[lbKlang.SelectedIndex]).Content.ToString();

                    _GrpObjecte[posObjGruppe].grdKlang.Visibility = Visibility.Hidden;
                    List<Audio_Playlist> playlistliste = Global.ContextAudio.PlaylistListe.Where(t => t.Name.Equals(s)).ToList();
                    if (playlistliste.Count == 1)
                    {
                        List<Audio_Titel> titelliste = Global.ContextAudio.LoadTitelByPlaylist(playlistliste[0]);
                        PlaylisteLeeren(posObjGruppe);
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

                        tboxklangsongparallel.TextChanged -= new TextChangedEventHandler(tboxklangsongparallel_TextChanged);
                        tboxklangsongparallel.Tag = AktKlangPlaylist.Audio_Playlist_Titel.Count;
                        tboxklangsongparallel.Text = "0";
                        _GrpObjecte[posObjGruppe].istHintergrundMusik = AktKlangPlaylist.Hintergrundmusik;
                        if (AktKlangPlaylist.Hintergrundmusik)
                            _GrpObjecte[posObjGruppe].maxsongparallel = 1;
                        else
                            _GrpObjecte[posObjGruppe].maxsongparallel = 0;
                        tboxklangsongparallel.TextChanged += new TextChangedEventHandler(tboxklangsongparallel_TextChanged);

                        if (titelliste.Count > 0)
                        {
                         //   if (!rbtnGleichSpielen.IsChecked.Value)
                         //       _GrpObjecte[posObjGruppe].btnKlangPause.Tag = true;

                            _GrpObjecte[posObjGruppe].playlistName = AktKlangPlaylist.Name;
                            for (UInt16 x = 0; x <= AktKlangPlaylist.Audio_Playlist_Titel.Count - 1; x++)
                            {
                                Audio_Playlist_Titel playlisttitel = Global.ContextAudio.LoadPlaylist_TitelByPlaylist(AktKlangPlaylist, titelliste[x])[0];
                                KlangNewRow(playlisttitel.Audio_Titel.Pfad, posObjGruppe, x, playlisttitel);
                                
                                if (AktKlangPlaylist.Hintergrundmusik)
                                    ZeigeKlangSettings(posObjGruppe, x, false);
                            }
                            btnKlangSave.Visibility = Visibility.Hidden;
                            _GrpObjecte[posObjGruppe].grdKlang.Visibility = Visibility.Visible;
                            
                            tboxklangsongparallel.Text = AktKlangPlaylist.MaxSongsParallel.ToString();
                            _GrpObjecte[posObjGruppe].maxsongparallel = Convert.ToUInt16(AktKlangPlaylist.MaxSongsParallel);

                            CheckAlleAngehakt(posObjGruppe);
                        } 
                        if (AktKlangPlaylist.Hintergrundmusik)
                        {
                            ZeigeKlangSongsParallel(posObjGruppe, false);
                            ZeigeKlangTop(posObjGruppe, false);
                        }
                        else
                        {
                            ZeigeKlangSongsParallel(posObjGruppe, true);
                            ZeigeKlangTop(posObjGruppe, true);
                        }

                        _GrpObjecte[posObjGruppe].grdKlangTop.Visibility = Visibility.Visible;
                        if (!AktKlangPlaylist.Hintergrundmusik && rbtnGleichSpielen.IsChecked == true && ((TabItem)tcKlang.SelectedItem).Name.StartsWith("tiKlang"))
                            btnKlangPause0.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            //CheckPlayStandbySongs(posObjGruppe);
                    }
                    else
                    {
                        AktKlangPlaylist = null;   
                        var errWin = new MsgWindow("Datenfehler", "Die Playlist-Liste konnte nicht eindeutig in der Datenbank detektiert werden.", null);
                        errWin.ShowDialog();
                        errWin.Close();

                        for (int i = 0; i <= grdKlangPlaylistInfo.Children.Count - 1; i++)
                            if ((grdKlangPlaylistInfo.Children[i] as Control).Name != "btnKlangSave") grdKlangPlaylistInfo.Children[i].Visibility = Visibility.Hidden;
                        _GrpObjecte[posObjGruppe].grdKlangTop.Visibility = Visibility.Hidden;
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

        private void CheckAlleAngehakt(int posObjGruppe)
        {
            if (_GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.chkTitel.IsChecked == true).Count == _GrpObjecte[posObjGruppe].anzTitelAkt)
                _GrpObjecte[posObjGruppe].chkbxTopAktiv.IsChecked = true;
            else
                _GrpObjecte[posObjGruppe].chkbxTopAktiv.IsChecked = false;
            if (_GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.chkTitel.IsChecked == true).Count == _GrpObjecte[posObjGruppe].anzVolChange)
                _GrpObjecte[posObjGruppe].chkbxTopVolChange.IsChecked = true;
            else
                _GrpObjecte[posObjGruppe].chkbxTopVolChange.IsChecked = false;
            if (_GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.chkTitel.IsChecked == true).Count == _GrpObjecte[posObjGruppe].anzPauseChange)
                _GrpObjecte[posObjGruppe].chkbxTopPauseChange.IsChecked = true;
            else
                _GrpObjecte[posObjGruppe].chkbxTopPauseChange.IsChecked = false;
        }

        private void AlleKlangSongsAus(Int16 posObjGruppe, bool checkboxAus, bool ZeileLoeschen)
        {
            if (posObjGruppe == -1)
                return;
            int seite = _GrpObjecte[posObjGruppe].seite;                                    

            for (UInt16 i = 0; i < _GrpObjecte[posObjGruppe]._listZeile.Count; i++) 
            {
                
                if (_GrpObjecte[posObjGruppe]._listZeile[i].chkTitel.IsChecked.Value == true)
                {
                    if (checkboxAus)
                    {
                        _GrpObjecte[posObjGruppe]._listZeile[i].chkTitel.Click -= new RoutedEventHandler(chkTitel0_0_Click);
                        _GrpObjecte[posObjGruppe]._listZeile[i].chkTitel.IsChecked = false;
                    }
                    if (_GrpObjecte[posObjGruppe]._listZeile[i]._mplayer != null)
                    {
                        _GrpObjecte[posObjGruppe]._listZeile[i]._mplayer.MediaEnded -= new EventHandler(Player_Ended); ;
                        _GrpObjecte[posObjGruppe]._listZeile[i]._mplayer.Stop();
                        _GrpObjecte[posObjGruppe]._listZeile[i]._mplayer = null;
                    }
                    _GrpObjecte[posObjGruppe]._listZeile[i].istLaufend = false;
                    _GrpObjecte[posObjGruppe]._listZeile[i].istPause = false;


                    _GrpObjecte[posObjGruppe]._listZeile[i].pbarTitel.Maximum = 100;
                    _GrpObjecte[posObjGruppe]._listZeile[i].pbarTitel.Value = 0;
                }
                if (ZeileLoeschen)
                {             
                    _GrpObjecte[posObjGruppe]._listZeile[i].grdKlangRow.Children
                        .Remove(_GrpObjecte[posObjGruppe]._listZeile[i].imgTrash);

                    _GrpObjecte[posObjGruppe].grdKlang.Children.Remove(_GrpObjecte[posObjGruppe]._listZeile[i].spnlKlangRow);
                    if (seite == 0)
                        _GrpObjecte[posObjGruppe].grdKlang.UnregisterName(_GrpObjecte[posObjGruppe]._listZeile[i].spnlKlangRow.Name);
                    else
                        this.UnregisterName(_GrpObjecte[posObjGruppe]._listZeile[i].spnlKlangRow.Name);                    
                }
            }
        }

        private Int16 GetPosObjGruppe(int objGruppe)
        {
            Int16 posObjGruppe = Convert.ToInt16(_GrpObjecte.FindIndex(t => t.objGruppe == objGruppe));
            return posObjGruppe;              
        }

        private void PlaylisteLeeren(Int16 posObjGruppe)
        {
            if (posObjGruppe == -1)
                return;

            if (_GrpObjecte[posObjGruppe].WerteGeändert)
            {
                if (MessageBox.Show("Das Fenster enthält noch nicht gesicherte Werte." + Environment.NewLine + Environment.NewLine +
                    "Sollen die neuen Werte gesichert werden?", "Änderungen vorhanden", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    btnKlangSave.Tag = posObjGruppe;
                    btnKlangSave.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }

            if (AktKlangPlaylist != null)
            {

                AlleKlangSongsAus(posObjGruppe, true, true);
                ZeigeKlangSongsParallel(posObjGruppe, false);

                if (rbtnGleichSpielen.IsChecked == true)
                    _GrpObjecte[posObjGruppe].btnKlangPause.Tag = false;// .IsPressed = false;
                else
                    _GrpObjecte[posObjGruppe].btnKlangPause.Tag = true;


                if (_GrpObjecte[posObjGruppe].grdKlang != null)
                    _GrpObjecte[posObjGruppe].grdKlang.RowDefinitions.RemoveRange(1, _GrpObjecte[posObjGruppe].grdKlang.RowDefinitions.Count - 2);
            }
            if (_GrpObjecte[posObjGruppe]._listZeile.Count > 0)
                _GrpObjecte[posObjGruppe]._listZeile.RemoveRange(0, _GrpObjecte[posObjGruppe]._listZeile.Count);
            _GrpObjecte[posObjGruppe].anzTitelAkt = 0;
            _GrpObjecte[posObjGruppe].istHintergrundMusik = true;
            _GrpObjecte[posObjGruppe].maxsongparallel = 0;
            _GrpObjecte[posObjGruppe].anzTitelAkt = 0;
            _GrpObjecte[posObjGruppe].anzVolChange = 0;
            _GrpObjecte[posObjGruppe].anzPauseChange = 0;
            _GrpObjecte[posObjGruppe].playlistName = tboxPlaylistName.Text;
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

        private void ZeigeKlangTop(Int16 posObjGruppe, bool sichtbar)
        {
            if (sichtbar)
            {
                for (int i = _GrpObjecte[posObjGruppe].grdKlangTop.ColumnDefinitions.Count - 1; i >= 3; i--)
                    _GrpObjecte[posObjGruppe].grdKlangTop.ColumnDefinitions[i].Width = grdKlangTopX.ColumnDefinitions[i].Width;
            }
            else
            {
                for (int i = _GrpObjecte[posObjGruppe].grdKlangTop.ColumnDefinitions.Count-1; i >= 4; i--)
                    _GrpObjecte[posObjGruppe].grdKlangTop.ColumnDefinitions[i].Width = new GridLength(0);
            }
        }

        private void ZeigeKlangSongsParallel(Int16 posObjGruppe, bool sichtbar)
        {
            if (sichtbar)
            {                
                lblKlangSongsParallel.Visibility = Visibility.Visible;
                tboxklangsongparallel.Text = Convert.ToString(_GrpObjecte[posObjGruppe].maxsongparallel);
                tboxklangsongparallel.Visibility = Visibility.Visible;
                btnSongParMinus.Visibility = Visibility.Visible;
                btnSongParPlus.Visibility = Visibility.Visible;
                _GrpObjecte[posObjGruppe].btnKlangPause.Visibility = Visibility.Visible;
                if (rbtnGleichSpielen.IsChecked == true && ((TabItem)tcKlang.SelectedItem).Name == "tiKlang")
                    btnKlangPause0_Click(_GrpObjecte[posObjGruppe].btnKlangPause, new RoutedEventArgs());
            }
            else
            {
                lblKlangSongsParallel.Visibility = Visibility.Hidden;
                tboxklangsongparallel.Visibility = Visibility.Hidden;
                btnSongParMinus.Visibility = Visibility.Hidden;
                btnSongParPlus.Visibility = Visibility.Hidden;
                _GrpObjecte[posObjGruppe].btnKlangPause.Visibility = Visibility.Hidden;                  
            }
        }

        private void ZeigeKlangSettings(Int16 posObjGruppe, UInt16 row, bool sichtbar)
        {
            if (_GrpObjecte[posObjGruppe]._listZeile[row].spnlKlangRow!= null)
            {
                if (!sichtbar)
                {
                    for (int i = 13; i >= 4; i--)
                        _GrpObjecte[posObjGruppe]._listZeile[row].grdKlangRow.ColumnDefinitions[i].Width = new GridLength(0);
                }
                else
                {
                    for (int i = 13; i >= 4; i--)
                        _GrpObjecte[posObjGruppe]._listZeile[row].grdKlangRow.ColumnDefinitions[i].Width = grdKlangRow0_X.ColumnDefinitions[i].Width;
                }
            }
        }

        private void KlangNewRow(string songdatei, int posObjGruppe, UInt16 row, Audio_Playlist_Titel playlisttitel)
        {   
            bool neuerstellen = true;
            int objGruppe = _GrpObjecte[posObjGruppe].objGruppe;
            if (posObjGruppe == -1)
                return;

            if (((StackPanel)this.FindName("spnlKlangRow" + objGruppe + "_" + row)) != null)
                neuerstellen = false;
            
            StackPanel newStack = (StackPanel)DeepCopy(null, "0_X", objGruppe + "_" + row);

            newStack.Visibility = Visibility.Visible;
            newStack.Tag = rowErstellt;

            _GrpObjecte[posObjGruppe].grdKlang.Children.Add(newStack);
            Grid.SetRow(newStack, _GrpObjecte[posObjGruppe].grdKlang.RowDefinitions.Count - 1);

            Grid grdKlangRow = (Grid)newStack.FindName("grdKlangRow" + objGruppe + "_" + row);
            if (neuerstellen)
                this.RegisterName(newStack.Name, newStack);

            //Papierkorb
            Image imgTrash = (Image)newStack.FindName("imgTrash" + objGruppe + "_" + row);
            imgTrash.Tag = playlisttitel.Audio_Titel.Audio_TitelGUID;
            imgTrash.ToolTip = imgTrash0_X.ToolTip;
            if (neuerstellen)
                imgTrash.MouseUp += new MouseButtonEventHandler(imgTrash0_0_MouseUp);

            //Titel
            CheckBox chkTitel = (CheckBox)newStack.FindName("chkTitel" + objGruppe + "_" + row);
            if (neuerstellen)
                chkTitel.Click += new RoutedEventHandler(chkTitel0_0_Click);
            chkTitel.Content = System.IO.Path.GetFileNameWithoutExtension(songdatei);
            if (playlisttitel.Aktiv)
                chkTitel.IsChecked = playlisttitel.Aktiv;

            chkTitel.ToolTip = songdatei;
            chkTitel.Tag = songdatei;

            // Schieberegler Lautstärke
            Slider sldVol = (Slider)newStack.FindName("sldKlangVol" + objGruppe + "_" + row);
            sldVol.Minimum = 0; 
            sldVol.Maximum = 100;
            sldVol.Value = playlisttitel.Volume;
            sldVol.Tag = rowErstellt;// row;
            if (neuerstellen)
                sldVol.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sldKlangVol0_X_ValueChanged);
            sldVol.ToolTip = Math.Round(sldVol.Value) + " %";

            //Checkbox Automatisch veränderbare Lautstärke
            CheckBox chkVolMove = (CheckBox)newStack.FindName("chkVolMove" + objGruppe + "_" + row);
            chkVolMove.IsChecked = playlisttitel.VolumeChange;
            chkVolMove.Tag = row;
            if (neuerstellen)
                chkVolMove.Click += new RoutedEventHandler(chkVolMove0_0_Click);

            // Volume Minimum Plus/Minus (für ein Song)
            Button btnVolMinMinus = (Button)newStack.FindName("_btnVolMinMinus" + objGruppe + "_" + row);
            if (neuerstellen)
                btnVolMinMinus.Click += new RoutedEventHandler(_btnVolMinMinus0_X_Click);
            Button btnVolMinPlus = (Button)newStack.FindName("_btnVolMinPlus" + objGruppe + "_" + row);
            if (neuerstellen)
                btnVolMinPlus.Click += new RoutedEventHandler(_btnVolMinPlus0_X_Click);

            TextBox tboxVolMin = (TextBox)newStack.FindName("tboxVolMin" + objGruppe + "_" + row);
            tboxVolMin.Text = Convert.ToString(playlisttitel.VolumeMin);
            tboxVolMin.Tag = row;
            if (neuerstellen)
                tboxVolMin.PreviewTextInput += new TextCompositionEventHandler(tboxVolMin0_X_PreviewTextInput);
            if (neuerstellen)
                tboxVolMin.KeyDown += new KeyEventHandler(tboxVolMin0_X_KeyDown);

            // Volume Maximum Plus/Minus (für ein Song)
            Button btnVolMaxMinus = (Button)newStack.FindName("_btnVolMaxMinus" + objGruppe + "_" + row);
            if (neuerstellen)
                btnVolMaxMinus.Click += new RoutedEventHandler(_btnVolMaxMinus0_X_Click);
            Button btnVolMaxPlus = (Button)newStack.FindName("_btnVolMaxPlus" + objGruppe + "_" + row);
            if (neuerstellen)
                btnVolMaxPlus.Click += new RoutedEventHandler(_btnVolMaxPlus0_X_Click);

            TextBox tboxVolMax = (TextBox)newStack.FindName("tboxVolMax" + objGruppe + "_" + row);
            tboxVolMax.Text = Convert.ToString(playlisttitel.VolumeMax);
            tboxVolMax.Tag = row;
            if (neuerstellen)
                tboxVolMax.PreviewTextInput += new TextCompositionEventHandler(tboxVolMin0_X_PreviewTextInput);
            if (neuerstellen)
                tboxVolMax.KeyDown += new KeyEventHandler(tboxVolMax0_X_KeyDown);

            // Schieberegler Zwischenpause
            Slider sldKlangPause = (Slider)newStack.FindName("sldKlangPause" + objGruppe + "_" + row);
            sldKlangPause.Minimum = 0; 
            sldKlangPause.Maximum = 10000; 
            sldKlangPause.Value = playlisttitel.Pause;
            sldKlangPause.TickFrequency = 10;
            sldKlangPause.Tag = rowErstellt; //row;
            if (neuerstellen)
                sldKlangPause.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sldKlangPause0_X_ValueChanged);
            sldKlangPause.ToolTip = Math.Round(sldKlangPause.Value) + " ms";

            // Checkbox veränderbare Zwischenpause
            CheckBox chkKlangPauseMove = (CheckBox)newStack.FindName("chkKlangPauseMove" + objGruppe + "_" + row);
            chkKlangPauseMove.IsChecked = playlisttitel.PauseChange;
            chkKlangPauseMove.Tag = row;
            if (neuerstellen)
                chkKlangPauseMove.Click += new RoutedEventHandler(chkKlangPauseMove0_0_Click);

            // Zwischenpause Minimum Plus/Minus (für ein Song)
            Button btnPauseMinMinus = (Button)newStack.FindName("_btnPauseMinMinus" + objGruppe + "_" + row);
            if (neuerstellen)
                btnPauseMinMinus.Click += new RoutedEventHandler(_btnPauseMinMinus0_X_Click);
            Button btnPauseMinPlus = (Button)newStack.FindName("_btnPauseMinPlus" + objGruppe + "_" + row);
            if (neuerstellen)
                btnPauseMinPlus.Click += new RoutedEventHandler(_btnPauseMinPlus0_X_Click);

            TextBox tboxPauseMin = (TextBox)newStack.FindName("tboxPauseMin" + objGruppe + "_" + row);
            tboxPauseMin.Text = Convert.ToString(playlisttitel.PauseMin);
            tboxPauseMin.Tag = row;
            if (neuerstellen)
                tboxPauseMin.PreviewTextInput += new TextCompositionEventHandler(tboxVolMin0_X_PreviewTextInput);
            if (neuerstellen)
                tboxPauseMin.KeyDown += new KeyEventHandler(tboxPauseMin0_X_KeyDown);

            // Zwischenpause Maximum Plus/Minus (für ein Song)
            Button btnPauseMaxMinus = (Button)newStack.FindName("_btnPauseMaxMinus" + objGruppe + "_" + row);
            if (neuerstellen)
                btnPauseMaxMinus.Click += new RoutedEventHandler(_btnPauseMaxMinus0_X_Click);
            Button btnPauseMaxPlus = (Button)newStack.FindName("_btnPauseMaxPlus" + objGruppe + "_" + row);
            if (neuerstellen)
                btnPauseMaxPlus.Click += new RoutedEventHandler(_btnPauseMaxPlus0_X_Click);

            TextBox tboxPauseMax = (TextBox)newStack.FindName("tboxPauseMax" + objGruppe + "_" + row);
            tboxPauseMax.Text = Convert.ToString(playlisttitel.PauseMax);
            tboxPauseMax.Tag = row;
            if (neuerstellen)
                tboxPauseMax.PreviewTextInput += new TextCompositionEventHandler(tboxVolMin0_X_PreviewTextInput);
            if (neuerstellen)
                tboxPauseMax.KeyDown += new KeyEventHandler(tboxPauseMax0_X_KeyDown);
            
            // Schieberegler Geschwindigeit
            Slider sldSpeed = (Slider)newStack.FindName("sldPlaySpeed" + objGruppe + "_" + row);          
            sldSpeed.Value = playlisttitel.Speed;

            string geschw = "Abspielgeschwindigkeit: ";
            geschw += sldSpeed.Value == .1 ? "sehr langsam" :
                      sldSpeed.Value == .5 ? "langsam" :
                      sldSpeed.Value == .75 ? "gedrosselt" :
                      sldSpeed.Value == 1 ? "normal" :
                      sldSpeed.Value == 2 ? "erhöht" :
                      sldSpeed.Value == 3 ? "schnell" :
                      sldSpeed.Value == 4 ? "sehr schnell" :
                      sldSpeed.Value == 5 ? "utlra schnell" : "nicht definiert";
            sldSpeed.ToolTip = geschw;
            sldSpeed.Tag = row;
            if (neuerstellen)
                sldSpeed.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sldPlaySpeed0_X_ValueChanged);

            RowDefinition rowDef1 = new RowDefinition();
            rowDef1.Height = _GrpObjecte[posObjGruppe].grdKlang.RowDefinitions[1].Height;
            _GrpObjecte[posObjGruppe].grdKlang.RowDefinitions.Insert(_GrpObjecte[posObjGruppe].grdKlang.RowDefinitions.Count, rowDef1);


            //************************************************************************

            KlangZeile klZeile = new KlangZeile(rowErstellt++);

            klZeile.audiotitel = playlisttitel;
            klZeile._mplayer = new MediaPlayer();
            klZeile.mediaHashCode = klZeile._mplayer.GetHashCode();

            klZeile.grdKlangRow = grdKlangRow;
            klZeile.spnlKlangRow = newStack;
            klZeile.imgTrash = imgTrash;
            klZeile.pbarTitel = (ProgressBar)newStack.FindName("pbarTitel" + objGruppe + "_" + row); 
            klZeile.chkTitel = chkTitel;
            klZeile.lblDauer = (Label)newStack.FindName("lblDauer" + objGruppe + "_" + row);
            klZeile.sldKlangVol = sldVol;
            klZeile.chkVolMove = chkVolMove;
            klZeile.tboxVolMin = tboxVolMin;
            klZeile.btnVolMinMinus = btnVolMinMinus;
            klZeile.btnVolMinPlus = btnVolMinPlus;
            klZeile.tboxVolMax = tboxVolMax;
            klZeile.btnVolMaxMinus = btnVolMaxMinus;
            klZeile.btnVolMaxPlus = btnVolMaxPlus;
            klZeile.sldKlangPause = sldKlangPause;
            klZeile.chkKlangPauseMove = chkKlangPauseMove;
            klZeile.tboxPauseMin = tboxPauseMin;
            klZeile.btnPauseMinMinus = btnPauseMinMinus;
            klZeile.btnPauseMinPlus = btnPauseMinPlus;
            klZeile.tboxPauseMax = tboxPauseMax;
            klZeile.btnPauseMaxMinus = btnPauseMaxMinus;
            klZeile.btnPauseMaxPlus = btnPauseMaxPlus;

            klZeile.pauseMin_wert = Convert.ToInt16(tboxPauseMin.Text);
            klZeile.pauseMax_wert = Convert.ToInt16(tboxPauseMax.Text);
            klZeile.volMin_wert = Convert.ToInt16(tboxVolMin.Text);
            klZeile.volMax_wert = Convert.ToInt16(tboxVolMax.Text);

            klZeile.sldPlaySpeed = sldSpeed;

            if (playlisttitel.Aktiv && !_GrpObjecte[posObjGruppe].istHintergrundMusik)
                klZeile.istStandby = true;
            else
                klZeile.istStandby = false;
         //   if (((_GrpObjecte[posObjGruppe].seite == 0) && (_GrpObjecte[posObjGruppe].btnKlangPause).IsChecked == true) ||  // ((ToggleButton)this.FindName("btnKlangPause" + objGruppe)
        //        ((_GrpObjecte[posObjGruppe].seite != 0) && (_GrpObjecte[posObjGruppe].grdKlangTop ((ToggleButton)((Grid)this.FindName("grdKlangTop" + objGruppe)).FindName("btnKlangPause" + objGruppe)).IsChecked == true)))
            if (Convert.ToBoolean((_GrpObjecte[posObjGruppe].btnKlangPause).Tag) == true)
                klZeile.istPause = true;

            klZeile.playable = chkTitel.IsChecked.Value;

            _GrpObjecte[posObjGruppe]._listZeile.Add(klZeile);
            if (chkTitel.IsChecked == true) _GrpObjecte[posObjGruppe].anzTitelAkt++;
            if (chkVolMove.IsChecked == true) _GrpObjecte[posObjGruppe].anzVolChange++;
            if (chkKlangPauseMove.IsChecked == true) _GrpObjecte[posObjGruppe].anzPauseChange++;
        }

        private void chkTitel0_0_Click(object sender, RoutedEventArgs e)
        {
            string s = (sender as CheckBox).Name.Substring(8);

            char[] Separator = new char[] { '_' };
            string[] werte = s.Split(Separator, StringSplitOptions.None);

            Int16 objGruppe = Convert.ToInt16(werte[0]);
            UInt16 zeile = Convert.ToUInt16(werte[1]);
            int posObjGruppe = GetPosObjGruppe(objGruppe);
            if (posObjGruppe == -1)
                return;
            UInt16 seite = Convert.ToUInt16(_GrpObjecte[posObjGruppe].seite);

            string file = (sender as CheckBox).Tag.ToString();
          //  StackPanel spnlZeile = (StackPanel)((Grid)((CheckBox)sender).Parent).Parent;

            if (e.Source != null)
            {
                if ((sender as CheckBox).IsChecked == true)
                    _GrpObjecte[posObjGruppe].anzTitelAkt++;
                else
                    _GrpObjecte[posObjGruppe].anzTitelAkt--;
            }

            if (!_GrpObjecte[posObjGruppe]._listZeile[zeile].istPause)
            {
                if (File.Exists(file) != true)
                {
                    DriveInfo[] Drives = DriveInfo.GetDrives();
                    foreach (DriveInfo Drive in Drives)
                    {
                        if (Drive.DriveType != DriveType.Fixed)
                        {
                            string newfile = Drive.ToString() + file.Substring(3);
                            if (File.Exists(newfile))
                            {
                                file = newfile;
                                break;
                            }
                        }
                    }
                }
                if (File.Exists(file) != true)
                {
                    _GrpObjecte[posObjGruppe]._listZeile[zeile].spnlKlangRow.Background = Brushes.Red;
                    _GrpObjecte[posObjGruppe]._listZeile[zeile].spnlKlangRow.ToolTip = "Datei nicht gefunden";
                    _GrpObjecte[posObjGruppe]._listZeile[zeile].playable = false;
                    _GrpObjecte[posObjGruppe]._listZeile[zeile].istPause = false;
                    _GrpObjecte[posObjGruppe]._listZeile[zeile].istLaufend = false;
                    _GrpObjecte[posObjGruppe]._listZeile[zeile].istStandby = false;
                    CheckPlayStandbySongs(posObjGruppe);
                }
                else
                {
                    _GrpObjecte[posObjGruppe]._listZeile[zeile].playable = true;
                    _GrpObjecte[posObjGruppe]._listZeile[zeile].spnlKlangRow.Background = null;
                    if ((sender as CheckBox).IsChecked.Value == true)
                    {
                        int anzlaufend = _GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.istLaufend == true).Count;
                        if (_GrpObjecte[posObjGruppe].maxsongparallel > anzlaufend)
                        {
                            _GrpObjecte[posObjGruppe]._listZeile[zeile]._mplayer =
                                PlayFile(seite, zeile, posObjGruppe, _GrpObjecte[posObjGruppe]._listZeile[zeile]._mplayer, file, _GrpObjecte[posObjGruppe]._listZeile[zeile].sldKlangVol.Value / 100); ;
                            _GrpObjecte[posObjGruppe]._listZeile[zeile].mediaHashCode = _GrpObjecte[posObjGruppe]._listZeile[zeile]._mplayer.GetHashCode();

                            if (_GrpObjecte[posObjGruppe]._listZeile[zeile]._mplayer.NaturalDuration.HasTimeSpan)
                                _GrpObjecte[posObjGruppe]._listZeile[zeile].pbarTitel.Maximum =
                                    _GrpObjecte[posObjGruppe]._listZeile[zeile]._mplayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                            else
                                _GrpObjecte[posObjGruppe]._listZeile[zeile].pbarTitel.Maximum = 10000;
                            
                            if (aktiveThemeGruppe >= 0 &&
                                _GrpObjecte[posObjGruppe].playlistName == _ThemeGruppe[aktiveThemeGruppe].Hintergrund.playlistName)
                                _ThemeGruppe[aktiveThemeGruppe].pnlAudioTheme.pbarActBGTitel.Maximum = _GrpObjecte[posObjGruppe]._listZeile[zeile].pbarTitel.Maximum;

                            if (e.Source != null) _GrpObjecte[posObjGruppe]._listZeile[zeile].istStandby = false;
                            _GrpObjecte[posObjGruppe]._listZeile[zeile].istLaufend = true;
                            _GrpObjecte[posObjGruppe]._listZeile[zeile].istPause = false;
                        }
                        else
                            _GrpObjecte[posObjGruppe]._listZeile[zeile].istStandby = true;
                    }
                    else
                    {
                        if (_GrpObjecte[posObjGruppe]._listZeile[zeile]._mplayer != null)
                        {
                            _GrpObjecte[posObjGruppe]._listZeile[zeile]._mplayer.Stop();
                            _GrpObjecte[posObjGruppe]._listZeile[zeile]._mplayer = null;
                            _GrpObjecte[posObjGruppe]._listZeile[zeile].istStandby = false;
                            _GrpObjecte[posObjGruppe]._listZeile[zeile].istLaufend = false;
                            _GrpObjecte[posObjGruppe]._listZeile[zeile].istPause = false;

                        }
                        _GrpObjecte[posObjGruppe]._listZeile[zeile].pbarTitel.Maximum = 100;
                        _GrpObjecte[posObjGruppe]._listZeile[zeile].pbarTitel.Value = 0;

                        if (rbtnGleichSpielen.IsChecked == true)
                            CheckPlayStandbySongs(posObjGruppe);
                    }
                    if (_GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.istLaufend).Count > 0)
                    {
                        KlangProgBarTimer.IsEnabled = true;
                        KlangProgBarTimer.Start();
                    }
                    else
                    {
                        for (int i = 0; i < _GrpObjecte.Count; i++)
                        {
                            if (_GrpObjecte[i]._listZeile.FindAll(t => t.istLaufend).Count > 0)
                            {
                                KlangProgBarTimer.IsEnabled = true;
                                KlangProgBarTimer.Start();
                                break;
                            }
                            else
                            {
                                KlangProgBarTimer.IsEnabled = false;
                                KlangProgBarTimer.Stop();
                            }
                        }
                    }
                }
            }
            string cap;
            if (seite == 0)
                cap = _GrpObjecte[posObjGruppe].tiKlang.Header.ToString();
            else
                cap = _GrpObjecte[posObjGruppe].ticKlang._textBlockTitel.Text;
           
            List<Audio_Playlist> playlistliste = Global.ContextAudio.PlaylistListe.Where(t => t.Name.Equals(cap)).ToList();
            if (playlistliste.Count == 1)
            {
           //     List<Audio_Titel> titelliste = Global.ContextAudio.LoadTitelByPlaylist(playlistliste[0]);
                //List<Audio_Titel> titel = ;
                Audio_Playlist_Titel playlisttitel = Global.ContextAudio.LoadPlaylist_TitelByPlaylist(playlistliste[0], Global.ContextAudio.LoadTitelByPlaylist(playlistliste[0])[zeile]).First();

                if (playlisttitel != null)//.Count != 0)
                {
                    playlisttitel.Aktiv = (sender as CheckBox).IsChecked.Value;
                    btnKlangSave.Visibility = Visibility.Visible;
                  /*  try { Global.ContextAudio.Update<Audio_Playlist_Titel>(playlisttitel); }
                    catch (Exception ex)
                    {
                        var errWin = new MsgWindow("Datenfehler", "Die Datenbank konnte nicht aktualisiert werden", ex);
                        errWin.ShowDialog();
                        errWin.Close();
                    }*/
                }
            }            
            CheckAlleAngehakt(posObjGruppe);
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

                Int16 posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));
             //   UInt16 seite = Convert.ToUInt16(tcKlang.SelectedIndex);
             //   Int16 objGruppe = GetObjGruppe(tcKlang.SelectedIndex); 
             //   Int16 posObjGruppe = GetPosObjGruppe(objGruppe);

                tboxklangsongparallel.Tag = _GrpObjecte[posObjGruppe]._listZeile.Count + 1;// zeilenAufSeite[tcKlang.SelectedIndex] + 1;

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
                    playlisttitel[0].Speed = 1;

                    KlangNewRow(datei, posObjGruppe, Convert.ToUInt16(_GrpObjecte[posObjGruppe]._listZeile.Count), playlisttitel[0]);

                    if (AktKlangPlaylist.Hintergrundmusik)
                        ZeigeKlangSettings(posObjGruppe, Convert.ToUInt16(_GrpObjecte[posObjGruppe]._listZeile.Count - 1), false);                    
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
                    Int16 objGruppe = GetObjGruppe(tcKlang.SelectedIndex);
                    Int16 posObjGruppe = GetPosObjGruppe(objGruppe);
                    _GrpObjecte[posObjGruppe].grdKlangTop.Visibility = Visibility.Visible;


                    if (rbIstKlangPlaylist.IsChecked == true)
                        AktKlangPlaylist.Hintergrundmusik = false;
                    else
                        AktKlangPlaylist.Hintergrundmusik = true;

                    if (AktKlangPlaylist.Hintergrundmusik)
                    {
                        ZeigeKlangSongsParallel(posObjGruppe, false);
                        ZeigeKlangTop(posObjGruppe, false); 
                    }
                    else
                    {
                        ZeigeKlangSongsParallel(posObjGruppe, true);
                        ZeigeKlangTop(posObjGruppe, true);                        
                    }
                    CheckAlleAngehakt(posObjGruppe);
                    _GrpObjecte[posObjGruppe].grdKlang.Visibility = Visibility.Visible;
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
                    List<Audio_Titel> s = Global.ContextAudio.LoadTitelByPlaylist(playlistliste[i]).ToList();
                    if (s.Count == 1 && lbBackground.SelectedIndex != -1 &&
                        s != lbBackground.Items[lbBackground.SelectedIndex])
                        lbBackground.SelectedIndex = -1;
                    
                    lbitem.Content = playlistliste[i].Name;
                    
                    if (pos+1 > lbBackground.Items.Count)
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
                    //_GrpObjecte[posObjGruppe].maxsongparallel = 0;
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
                chkbxPlayRange.IsChecked = false;
                rsldTeilSong.Visibility = Visibility.Hidden;
                rsldTeilSong.LowerValue = 0;
                rsldTeilSong.UpperValue = 10000;

                ListBoxItem lbItem = (ListBoxItem)lbhintergrundtitellist.SelectedItem;
                string st = lbItem.Tag.ToString();

                List<Audio_Titel> titel = Global.ContextAudio.LoadTitelByPlaylist(AktHintergrundPlaylist);

                if (titel.Count == 0)
                {
                    if (!File.Exists(titel[lbhintergrundtitellist.SelectedIndex].Pfad))
                        lbItem.Background = Brushes.Red;
                    lbItem.ToolTip = "Datei nicht gefunden";

                    lbBackground_SelectionChanged(lbhintergrundtitellist, e);
                    lbhintergrundtitellist.Tag = -1;
                    btnBGPrev.IsEnabled = false;
                }
                else
                {
                    if (!File.Exists(titel[lbhintergrundtitellist.SelectedIndex].Pfad))
                    {
                        DriveInfo[] Drives = DriveInfo.GetDrives();
                        foreach (DriveInfo Drive in Drives)
                        {
                            if (Drive.DriveType != DriveType.Fixed)
                            {
                                string newfile = Drive.ToString() + titel[lbhintergrundtitellist.SelectedIndex].Pfad.Substring(3);
                                if (File.Exists(newfile))
                                {
                                    titel[lbhintergrundtitellist.SelectedIndex].Pfad = newfile;
                                    break;
                                }
                            }
                        }
                    }
                    if (!File.Exists(titel[lbhintergrundtitellist.SelectedIndex].Pfad))
                    {
                        lbItem.Background = Brushes.Red;
                        lbItem.ToolTip = "Datei nicht gefunden";
                        lbhintergrundtitellist.Tag = -1;
                        btnBGPrev.IsEnabled = false;
                        SpieleNeuenHintergrundTitel(-1);
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

                        AktBGPlaylistTitel = Global.ContextAudio.LoadPlaylist_TitelByPlaylist(AktHintergrundPlaylist, titel[lbhintergrundtitellist.SelectedIndex]).First();
                        chkbxPlayRange.IsChecked = AktBGPlaylistTitel.TeilAbspielen;

                     /*   rsldTeilSong.Maximum = pbarBGSong.Maximum;
                        if (AktBGPlaylistTitel.TeilAbspielen)
                        {
                            //rsldTeilSong.Visibility = Visibility.Visible;
                            rsldTeilSong.LowerValue = AktBGPlaylistTitel.TeilStart.Value;
                            if (AktBGPlaylistTitel.TeilEnde.Value > pbarBGSong.Maximum)
                                pbarBGSong.Maximum = AktBGPlaylistTitel.TeilEnde.Value;
                            rsldTeilSong.UpperValue = AktBGPlaylistTitel.TeilEnde.Value;
                        }
                     */   
                        HintergrundPlayer = PlayFile(-1, 0, -1, HintergrundPlayer, titel[lbhintergrundtitellist.SelectedIndex].Pfad, slBGVolume.Value / 100);
                        btnBGPrev.IsEnabled = true;
                        btnBGStoppen.IsEnabled = true;
                        btnImgBGStoppen.Source  = new BitmapImage(new Uri("pack://application:,,,/DSA MeisterGeister;component/Images/Icons/General/play_stop.png"));
                        if (HintergrundPlayer != null)
                        {
                            btnBGAbspielen.Tag = 1;
                            btnImgBGAbspielen.Source = new BitmapImage(new Uri("pack://application:,,,/DSA MeisterGeister;component/Images/Icons/General/pause.png"));
                            //btnLblBGAbspielen.Content = "Stoppen";
                            
                            pbarBGSong.Value = 0;
                            string s = "0:00";
                            if (HintergrundPlayer.NaturalDuration.HasTimeSpan)
                            {
                                pbarBGSong.Maximum = HintergrundPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                                rsldTeilSong.Minimum = 0;
                                rsldTeilSong.Maximum = HintergrundPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                                if (AktBGPlaylistTitel.TeilAbspielen)
                                {
                                    rsldTeilSong.LowerValue = AktBGPlaylistTitel.TeilStart.Value;
                                    rsldTeilSong.UpperValue = AktBGPlaylistTitel.TeilEnde.Value;
                                    rsldTeilSong.Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    rsldTeilSong.LowerValue = 0;
                                    rsldTeilSong.UpperValue = pbarBGSong.Maximum;
                                }

                                if (HintergrundPlayer.NaturalDuration.TimeSpan.Minutes < 10)
                                    s = "0" + HintergrundPlayer.NaturalDuration.TimeSpan.Minutes + ":";
                                else
                                    s = HintergrundPlayer.NaturalDuration.TimeSpan.Minutes + ":";

                                if (HintergrundPlayer.NaturalDuration.TimeSpan.Seconds < 10)
                                    s += "0" + HintergrundPlayer.NaturalDuration.TimeSpan.Seconds;
                                else
                                    s += HintergrundPlayer.NaturalDuration.TimeSpan.Seconds;
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
                throw new ArgumentException("Aruments out of range");

            //Länge des zu betrachtenden Ausschnittes
            int length = pos2 - pos1 + 1;

            //neues Char-Array anlegen der Länge length
            Char[] chars = new Char[length];

            //packe alle Bytes von pos1 bis pos2 als
            //Char konvertiert in Array chars
            for (int i = 0; i < length; i++)
                chars[i] = Convert.ToChar(bytes[i + pos1]);

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

                    string titel = ConvertByteToString(bytes, 3, 32);
                    lblBgTitel.Content = titel;                    
                    lblBgTitel.Content = titel != "" ? titel : System.IO.Path.GetFileNameWithoutExtension(HintergrundPlayer.Source.LocalPath);
                    lblBgArtist.Content = ConvertByteToString(bytes, 33, 62);
                    lblBgAlbum.Content = ConvertByteToString(bytes, 63, 92);
                    lblBgJahr.Content = ConvertByteToString(bytes, 93, 96);
                    //m_comment = ConvertByteToString(bytes, 97, 126);
                    int z = Convert.ToInt32(bytes[127]);
                    if (z <= _genres.Length - 1)
                        lblBgGenre.Content = _genres[z];
                }
            }

            if (lblBgTimeMax.Content.ToString() == "-:--")
            {
                if (HintergrundPlayer.NaturalDuration.HasTimeSpan)
                {
                    string t = "0:00";
                    pbarBGSong.Maximum = HintergrundPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                    rsldTeilSong.Minimum = 0;
                    rsldTeilSong.Maximum = HintergrundPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;

                    if (AktBGPlaylistTitel.TeilAbspielen)
                    {
                        rsldTeilSong.LowerValue = AktBGPlaylistTitel.TeilStart.Value;
                        rsldTeilSong.UpperValue = AktBGPlaylistTitel.TeilEnde.Value;
                        rsldTeilSong.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        rsldTeilSong.LowerValue = 0;
                        rsldTeilSong.UpperValue = HintergrundPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                    }


                    t = HintergrundPlayer.NaturalDuration.TimeSpan.Minutes < 10 ? "0" + HintergrundPlayer.NaturalDuration.TimeSpan.Minutes + ":" : 
                        HintergrundPlayer.NaturalDuration.TimeSpan.Minutes + ":";

                    if (HintergrundPlayer.NaturalDuration.TimeSpan.Seconds < 10)
                        t += "0" + HintergrundPlayer.NaturalDuration.TimeSpan.Seconds;
                    else
                        t += HintergrundPlayer.NaturalDuration.TimeSpan.Seconds;
                    lblBgTimeMax.Content = t;
                }
            }

            pbarBGSong.Value = HintergrundPlayer.Position.TotalMilliseconds;
            string s = "";
            s = (HintergrundPlayer.Position.Minutes < 10)? "0" + HintergrundPlayer.Position.Minutes + ":" : HintergrundPlayer.Position.Minutes + ":";

            if (HintergrundPlayer.Position.Seconds < 10)
                s += "0" + HintergrundPlayer.Position.Seconds;
            else
                s += HintergrundPlayer.Position.Seconds;
            lblBgTimeActual.Content = s;

            if (chkbxPlayRange.IsChecked.Value && pbarBGSong.Value < rsldTeilSong.LowerValue)
                    HintergrundPlayer.Position = TimeSpan.FromMilliseconds(rsldTeilSong.LowerValue);

            if ((HintergrundPlayer.NaturalDuration.HasTimeSpan &&
                 HintergrundPlayer.Position.TotalMilliseconds == HintergrundPlayer.NaturalDuration.TimeSpan.TotalMilliseconds) ||
                 (HintergrundPlayer.Position.TotalMilliseconds >= rsldTeilSong.UpperValue && chkbxPlayRange.IsChecked.Value))
            {
                if (HintergrundPlayer != null)
                {
                    HintergrundPlayer.Stop();
                    HintergrundPlayer = null;
                }
                HintergrundProgBarTimer.Stop();
                if (btnBGRepeat.IsChecked.Value)
                    SpieleNeuenHintergrundTitel(lbhintergrundtitellist.SelectedIndex);
                else
                    SpieleNeuenHintergrundTitel(-1);
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
                Int16 objGruppe = GetObjGruppe(tcKlang.SelectedIndex);
                int posObjGruppe = GetPosObjGruppe(objGruppe);

                _GrpObjecte[posObjGruppe].anzVolChange = Convert.ToUInt16(
                    _GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.chkVolMove.IsChecked == true).Count);

                if (_GrpObjecte[posObjGruppe].anzPauseChange == _GrpObjecte[posObjGruppe]._listZeile.Count)
                    _GrpObjecte[posObjGruppe].chkbxTopVolChange.IsChecked = true;
                else
                    _GrpObjecte[posObjGruppe].chkbxTopVolChange.IsChecked = false;
                
                playlisttitel[0].VolumeChange = ((CheckBox)sender).IsChecked.Value;
                btnKlangSave.Visibility = Visibility.Visible;
                CheckAlleAngehakt(posObjGruppe);
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
                btnKlangSave.Visibility = Visibility.Visible;  
            }
        }

        private void chkKlangPauseMove0_0_Click(object sender, RoutedEventArgs e)
        {
            int zeile = Convert.ToInt32(((CheckBox)sender).Tag);
            List<Audio_Playlist_Titel> playlisttitel = 
                Global.ContextAudio.LoadPlaylist_TitelByPlaylist(AktKlangPlaylist, Global.ContextAudio.LoadTitelByPlaylist(AktKlangPlaylist)[zeile]);
            
            if (playlisttitel.Count != 0)
            {
                Int16 objGruppe = GetObjGruppe(tcKlang.SelectedIndex);
                int posObjGruppe = GetPosObjGruppe(objGruppe);

                _GrpObjecte[posObjGruppe].anzPauseChange = Convert.ToUInt16(
                    _GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.chkKlangPauseMove.IsChecked == true).Count);

                playlisttitel[0].PauseChange = ((CheckBox)sender).IsChecked.Value;
                btnKlangSave.Visibility = Visibility.Visible;  
                
                CheckAlleAngehakt(posObjGruppe);
            }
        }

        private void sldKlangPause0_0_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).ToolTip = Math.Round(e.NewValue) + " ms";
        }

        private void rsldTeilSong_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AktBGPlaylistTitel.TeilStart = rsldTeilSong.LowerValue;
            AktBGPlaylistTitel.TeilEnde = rsldTeilSong.UpperValue;
            Global.ContextAudio.Update<Audio_Playlist_Titel>(AktBGPlaylistTitel);
        }
        
        private void imgTrash0_0_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Int16 objGruppe = GetObjGruppe(tcKlang.SelectedIndex);
            Int16 posObjGruppe = GetPosObjGruppe(objGruppe);
            
            List<Audio_Titel> titel =  Global.ContextAudio.LoadTitelByPlaylist(AktKlangPlaylist);

            int i = titel.FindIndex(t => t.Audio_TitelGUID.Equals(((Image)sender).Tag));

            if (titel[i].Audio_TitelGUID.Equals(((Image)sender).Tag))
            {
                if (_GrpObjecte[posObjGruppe]._listZeile[i].chkTitel.IsChecked.Value == true)
                {
                    _GrpObjecte[posObjGruppe]._listZeile[i].chkTitel.IsChecked = false;
                    chkTitel0_0_Click(_GrpObjecte[posObjGruppe]._listZeile[i].chkTitel, new RoutedEventArgs());
                }

                List<Audio_Playlist> playlistliste = Global.ContextAudio.PlaylistListe;
                int anz = 0;
                int gefunden = 0;
                while (anz < playlistliste.Count && gefunden <= 1)
                {
                    gefunden += Convert.ToUInt16((Global.ContextAudio.LoadPlaylist_TitelByPlaylist(playlistliste[anz], titel[i])).Count);                            
                    anz++;
                }
                Global.ContextAudio.RemoveTitelFromPlaylist(AktKlangPlaylist, titel[i]);
                if (gefunden <= 1)
                    Global.ContextAudio.RemoveTitel(titel[i]);
                                        
                int vorher = lbKlang.SelectedIndex;
                lbKlang.SelectedIndex = -1;
                lbKlang.SelectedIndex = vorher;
            }

            if (titel.Count == 0)
                ZeigeKlangSongsParallel(posObjGruppe, false);
        }

        private void rbKlangKlang_Click(object sender, RoutedEventArgs e)
        {
            int klangzeile = lbKlang.SelectedIndex;
            string klangname = "";
            if (klangzeile != -1) 
                klangname  = ((ListBoxItem)lbKlang.SelectedItem).Content.ToString();
            if (((TabItem)tcAudioPlayer.SelectedItem).Header.ToString() == "Hintergrund")  // .Content
                AktualisiereHintergrundPlaylist();
            else
                if (((TabItem)tcAudioPlayer.SelectedItem).Header.ToString() == "Playlist-Editor")
                    AktualisiereKlangPlaylist();
            if (klangzeile != -1)
                SelektiereKlangZeile(klangname);
        }

        private void rbKlangAlle_Click(object sender, RoutedEventArgs e)
        {
            int klangzeile = lbKlang.SelectedIndex;
            string klangname = "";
            if (klangzeile != -1)
                klangname = ((ListBoxItem)lbKlang.SelectedItem).Content.ToString();
            if (((TabItem)tcAudioPlayer.SelectedItem).Header.ToString() == "Hintergrund")
                AktualisiereHintergrundPlaylist();
            else
                if (((TabItem)tcAudioPlayer.SelectedItem).Header.ToString() == "Playlist-Editor")
                    AktualisiereKlangPlaylist();
            if (klangzeile != -1)
                SelektiereKlangZeile(klangname);
        }

        private void rbIstKlangPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (AktKlangPlaylist != null)
            {
                AktKlangPlaylist.Hintergrundmusik = false;
                Global.ContextAudio.Update<Audio_Playlist>(AktKlangPlaylist);

                Int16 posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));

                ZeigeKlangSongsParallel(posObjGruppe, true);
                ZeigeKlangTop(posObjGruppe, true);

                for (UInt16 i = 0; i < _GrpObjecte[posObjGruppe]._listZeile.Count; i++)
                {
                    ZeigeKlangSettings(posObjGruppe, i, true);
                    if (_GrpObjecte[posObjGruppe]._listZeile[i].chkTitel.IsChecked == true)
                        _GrpObjecte[posObjGruppe]._listZeile[i].istStandby = true;
                        //AddSongStandby(seite, i);
                }
                btnKlangSave.Visibility = Visibility.Visible;  
                AktualisiereKlangPlaylist();
                if (rbtnGleichSpielen.IsChecked == true && ((TabItem)tcKlang.SelectedItem).Name == "tiKlang")
                    CheckPlayStandbySongs(posObjGruppe);
            }
        }

        private void rbIstMusikPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (AktKlangPlaylist != null)
            {
                AktKlangPlaylist.Hintergrundmusik = true;
                
                Int16 posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));

                AlleKlangSongsAus(posObjGruppe, false, false);

                ZeigeKlangSongsParallel(posObjGruppe, false);
                ZeigeKlangTop(posObjGruppe, false);
                for (int i = 0; i < _GrpObjecte[posObjGruppe]._listZeile.Count; i++)
                    ZeigeKlangSettings(posObjGruppe, Convert.ToUInt16(i), false);
                
                AktKlangPlaylist.MaxSongsParallel = 1;
                _GrpObjecte[posObjGruppe].maxsongparallel = 1;

                btnKlangSave.Visibility = Visibility.Visible;  
                //Global.ContextAudio.Update<Audio_Playlist>(AktKlangPlaylist);
                
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
            tcKlang.Tag = tcKlang.SelectedIndex;
            if (tcKlang.SelectedIndex == tcKlang.Items.Count - 1)
            {
                if (tcKlang.Items.Count == 1)
                    tcKlang.SelectedIndex = -1;
                else
                    tcKlang.SelectedIndex = tcKlang.Items.Count - 2;
            }
            string s = "";
            Int16 posObjGruppe;
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

                GruppenObjekt grpobj = new GruppenObjekt();
                grpobj.seite = tcKlang.SelectedIndex;
                grpobj.tiKlang = ((TabItem)tcKlang.SelectedItem);
                grpobj.objGruppe = 0;
                grpobj.playlistName = s;

                grpobj.sviewer = sviewer0;// scrViewer;
                grpobj.grdKlang = grdKlang0;
                grpobj.grdKlangTop = grdKlangTop0;
                grpobj.btnKlangPause = btnKlangPause0;
                grpobj.btnImgKlangPause = btnImgKlangPause0;

                grpobj.chkbxTopAktiv = chkbxTopAktiv0;
                grpobj.btnTopVolMin = btnTopVolMin0;
                grpobj.btnTopVolDown = btnTopVolDown0;
                grpobj.btnTopVolUp = btnTopVolUp0;
                grpobj.btnTopVolMax = btnTopVolMax0;
                grpobj.chkbxTopVolChange = chkbxTopVolChange0;
                grpobj.btnTopPauseMin = btnTopPauseMin0;
                grpobj.btnTopPauseDown = btnTopPauseDown0;
                grpobj.btnTopPauseUp = btnTopPauseUp0;
                grpobj.btnTopPauseMax = btnTopPauseMax0;
                grpobj.chkbxTopPauseChange = chkbxTopPauseChange0;
                grpobj.btnTopVolMinMinus = btnTopVolMinMinus0;
                grpobj.btnTopVolMinPlus = btnTopVolMinPlus0;
                grpobj.btnTopVolMaxMinus = btnTopVolMaxMinus0;
                grpobj.btnTopVolMaxPlus = btnTopVolMaxPlus0;
                grpobj.btnTopPauseMinMinus = btnTopPauseMinMinus0;
                grpobj.btnTopPauseMinPlus = btnTopPauseMinPlus0;
                grpobj.btnTopPauseMaxMinus = btnTopPauseMaxMinus0;
                grpobj.btnTopPauseMaxPlus = btnTopPauseMaxPlus0;
                _GrpObjecte.Add(grpobj);
                posObjGruppe = GetPosObjGruppe(grpobj.objGruppe);
            }
            else
            {
                Int16 objGruppe = GetObjGruppe(tcKlang.SelectedIndex);
                if (objGruppe == -1)
                    return;
                posObjGruppe = GetPosObjGruppe(objGruppe);
                if (tcKlang.SelectedIndex >= 0)
                {
                    if (tcKlang.SelectedItem.GetType().ToString().EndsWith("TabItemControl"))
                        s = ((TabItemControl)tcKlang.SelectedItem)._textBlockTitel.Text.ToString();
                    else
                        s = ((TabItem)tcKlang.SelectedItem).Header.ToString();
                    
                    List<Audio_Playlist> playlistliste = Global.ContextAudio.PlaylistListe.Where(t => t.Name.Equals(s)).ToList();
                    if (playlistliste.Count == 1 && _GrpObjecte[posObjGruppe]._listZeile.Count > 0)
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
                            //_GrpObjecte[posObjGruppe].maxsongparallel = AktKlangPlaylist.MaxSongsParallel;
                            tboxklangsongparallel.TextChanged += new TextChangedEventHandler(tboxklangsongparallel_TextChanged);

                            //zeilenAufSeite[tcKlang.SelectedIndex] = Convert.ToUInt16(AktKlangPlaylist.Audio_Playlist_Titel.Count);

                            if (playlistliste[0].Hintergrundmusik)
                            {
                                ZeigeKlangSongsParallel(posObjGruppe, false);
                                if (_GrpObjecte[posObjGruppe]._listZeile[0].chkTitel.Visibility == Visibility.Visible)
                                {
                                    ZeigeKlangTop(posObjGruppe, false);
                                    for (int r = 0; r < _GrpObjecte[posObjGruppe]._listZeile.Count; r++)
                                        ZeigeKlangSettings(posObjGruppe, Convert.ToUInt16(r), false);
                                }
                            }
                            else
                            {
                                ZeigeKlangSongsParallel(posObjGruppe, true);
                                if (_GrpObjecte[posObjGruppe]._listZeile[0].chkTitel.Visibility == Visibility.Hidden)
                                {
                                    ZeigeKlangTop(posObjGruppe, true);
                                    for (int r = 0; r < _GrpObjecte[posObjGruppe]._listZeile.Count; r++)
                                        ZeigeKlangSettings(posObjGruppe, Convert.ToUInt16(r), true);
                                }
                            }
                        }
                        _GrpObjecte[posObjGruppe].grdKlangTop.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        lbKlang.SelectionChanged -= new SelectionChangedEventHandler(lbKlang_SelectionChanged);
                        lbKlang.SelectedIndex = -1;
                        lbKlang.SelectionChanged += new SelectionChangedEventHandler(lbKlang_SelectionChanged);

                        rbIstKlangPlaylist.IsChecked = rbKlangKlang.IsChecked;
                        rbIstMusikPlaylist.IsChecked = rbKlangMusik.IsChecked;
                        tboxPlaylistName.Text = s;
                        ZeigeKlangSongsParallel(posObjGruppe, false);

                        tboxklangsongparallel.TextChanged -= new TextChangedEventHandler(tboxklangsongparallel_TextChanged);
                        tboxklangsongparallel.Tag = null;
                        tboxklangsongparallel.Text = "0";

                      /*  int posObjGruppe = GetPosObjGruppe(objGruppe);
                        if (posObjGruppe == -1)
                            return;
                        _GrpObjecte[posObjGruppe].maxsongparallel = 0;*/
                        tboxklangsongparallel.TextChanged += new TextChangedEventHandler(tboxklangsongparallel_TextChanged);
                      //  AktKlangPlaylist = null;
                    }
                }
                SelektiereKlangZeile(s);
                btnKlangSave.Visibility = _GrpObjecte[posObjGruppe].WerteGeändert ? Visibility.Visible : Visibility.Hidden;
            }
            btnKlangSave.Tag = posObjGruppe;
        }

        private void SelektiereKlangZeile(string klangname)
        {
            int i = 0;
            while (i <= lbKlang.Items.Count - 1)
            {
                if (((ListBoxItem)lbKlang.Items[i]).Content.ToString() == klangname)
                {
                    lbKlang.SelectionChanged -= new SelectionChangedEventHandler(lbKlang_SelectionChanged);
                    lbKlang.SelectedIndex = i;
                    lbKlang.SelectionChanged += new SelectionChangedEventHandler(lbKlang_SelectionChanged);
                }
                i++;
            }
        }

        private void tiKlangPlaylistClose_Click(object sender, RoutedEventArgs e)
        {
            //Name des TabItem herausfinden   
            Int16 objGruppe;
            if (e.Source is Button) 
                objGruppe = Convert.ToInt16((((TabItemControl)((StackPanel)(((Button)sender).Parent)).Parent)).Name.Substring(7));
            else
                objGruppe = Convert.ToInt16(((TabItemControl)sender).Name.Substring(7));
            if (objGruppe == -1)
                return;

            Int16 posObjGruppe = GetPosObjGruppe(objGruppe);
            if (posObjGruppe == -1)
                return;

            int seite = _GrpObjecte[posObjGruppe].seite;            

            if (_GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.istLaufend).Count > 0) // laufendeSongs[seite] != null)
            {
                for (int i = 0; i < _GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.istLaufend).Count; i++)
                {
                    _GrpObjecte[posObjGruppe]._listZeile[i]._mplayer.MediaEnded -= new EventHandler(Player_Ended);
                    _GrpObjecte[posObjGruppe]._listZeile[i]._mplayer.Stop();
                }
            }
            PlaylisteLeeren(posObjGruppe);

            this.UnregisterName("tiKlang" + objGruppe);
            this.UnregisterName("Grid" + objGruppe);
            this.UnregisterName("sviewer" + objGruppe);
            this.UnregisterName("grdKlang" + objGruppe);
            this.UnregisterName("grdKlangTop" + objGruppe);
            _GrpObjecte.RemoveAt(seite);

            for (int i = seite; i < _GrpObjecte.Count; i++)
                _GrpObjecte[i].seite--;

            if (tcKlang.SelectedIndex == tcKlang.Items.Count - 2)
                tcKlang.SelectedIndex = tcKlang.SelectedIndex - 1;

            if (tcKlang.Items.Count < 10)
                tiPlus.Visibility = Visibility.Visible;
        }

        private void tiPlus_MouseUp(object sender, MouseButtonEventArgs e)
        {
            tiErstellt++;
            Int16 objGruppe = Convert.ToInt16(tiErstellt);

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
            string [] str_tiHeader = new string[tcKlang.Items.Count-2];
            for (int i = 0; i < tcKlang.Items.Count - 2; i++)
            {
                if (i == 0)
                    str_tiHeader[i] = ((TabItem)tcKlang.Items[i]).Header.ToString();
                else
                    str_tiHeader[i] = ((TabItemControl)tcKlang.Items[i])._textBlockTitel.Text;
            }
            
            while (Global.ContextAudio.PlaylistListe.Where(t => t.Name.Equals(NeuePlaylist)).ToList().Count != 0 ||
                str_tiHeader.Contains(NeuePlaylist))
            {
                NeuePlaylist = "NeuePlayliste" + (tcKlang.Items.Count - 2) + "-" + ver;
                ver++;
            }

            tabItem._textBlockTitel.Text = NeuePlaylist;
            tabItem._buttonClose.Click += new RoutedEventHandler(tiKlangPlaylistClose_Click);
            
            //SeiteXHatObjGruppe[tcKlang.Items.Count - 2] = objGruppe;
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

            Button tbtn = (Button)grdKlangTop.FindName("btnKlangPause" + objGruppe);
            tbtn.Tag = true;
            tbtn.Click += new RoutedEventHandler(btnKlangPause0_Click);

            Image tbtnImg = (Image)tbtn.FindName("btnImgKlangPause" + objGruppe);

            CheckBox chbxTopAkt = (CheckBox)grdKlangTop.FindName("chkbxTopAktiv" + objGruppe);
            chbxTopAkt.Tag = objGruppe;
            chbxTopAkt.Click += new RoutedEventHandler(chkbxTopAktiv0_Click);

            Button btnTopVolMin = (Button)grdKlangTop.FindName("btnTopVolMin" + objGruppe);
            btnTopVolMin.Click += new RoutedEventHandler(btnAllVolUp_Click);
            Button btnTopVolDown = (Button)grdKlangTop.FindName("btnTopVolDown" + objGruppe);
            btnTopVolDown.Click += new RoutedEventHandler(btnAllVolUp_Click);
            Button btnTopVolUp = (Button)grdKlangTop.FindName("btnTopVolUp" + objGruppe);
            btnTopVolUp.Click += new RoutedEventHandler(btnAllVolUp_Click);
            Button btnTopVolMax = (Button)grdKlangTop.FindName("btnTopVolMax" + objGruppe);
            btnTopVolMax.Click += new RoutedEventHandler(btnAllVolUp_Click);

            CheckBox chbxTopVolCh = (CheckBox)grdKlangTop.FindName("chkbxTopVolChange" + objGruppe);
            chbxTopVolCh.Tag = objGruppe;
            chbxTopVolCh.Click += new RoutedEventHandler(chkbxTopVolChange0_Click);

            Button btnTopPauseMin = (Button)grdKlangTop.FindName("btnTopPauseMin" + objGruppe);
            btnTopPauseMin.Click += new RoutedEventHandler(btnAllPauseUp_Click);
            Button btnTopPauseDown = (Button)grdKlangTop.FindName("btnTopPauseDown" + objGruppe);
            btnTopPauseDown.Click += new RoutedEventHandler(btnAllPauseUp_Click);
            Button btnTopPauseUp = (Button)grdKlangTop.FindName("btnTopPauseUp" + objGruppe);
            btnTopPauseUp.Click += new RoutedEventHandler(btnAllPauseUp_Click);
            Button btnTopPauseMax = (Button)grdKlangTop.FindName("btnTopPauseMax" + objGruppe);
            btnTopPauseMax.Click += new RoutedEventHandler(btnAllPauseUp_Click);

            CheckBox chbxTopPauseCh = (CheckBox)grdKlangTop.FindName("chkbxTopPauseChange" + objGruppe);
            chbxTopPauseCh.Tag = objGruppe;
            chbxTopPauseCh.Click += new RoutedEventHandler(chkbxTopVolChange0_Click);

            Button btnTopVolMinMinus = (Button)grdKlangTop.FindName("btnTopVolMinMinus" + objGruppe);
            btnTopVolMinMinus.Click += new RoutedEventHandler(btnTopVolMinMinus0_Click);
            Button btnTopVolMinPlus = (Button)grdKlangTop.FindName("btnTopVolMinPlus" + objGruppe);
            btnTopVolMinPlus.Click += new RoutedEventHandler(btnTopVolMinPlus0_Click);
            Button btnTopVolMaxMinus = (Button)grdKlangTop.FindName("btnTopVolMaxMinus" + objGruppe);
            btnTopVolMaxMinus.Click += new RoutedEventHandler(btnTopVolMaxMinus0_Click);
            Button btnTopVolMaxPlus = (Button)grdKlangTop.FindName("btnTopVolMaxPlus" + objGruppe);
            btnTopVolMaxPlus.Click += new RoutedEventHandler(btnTopVolMaxPlus0_Click);

            Button btnTopPauseMinMinus = (Button)grdKlangTop.FindName("btnTopPauseMinMinus" + objGruppe);
            btnTopPauseMinMinus.Click += new RoutedEventHandler(btnTopPauseMinMinus0_Click);
            Button btnTopPauseMinPlus = (Button)grdKlangTop.FindName("btnTopPauseMinPlus" + objGruppe);
            btnTopPauseMinPlus.Click += new RoutedEventHandler(btnTopPauseMinPlus0_Click);
            Button btnTopPauseMaxMinus = (Button)grdKlangTop.FindName("btnTopPauseMaxMinus" + objGruppe);
            btnTopPauseMaxMinus.Click += new RoutedEventHandler(btnTopPauseMaxMinus0_Click);
            Button btnTopPauseMaxPlus = (Button)grdKlangTop.FindName("btnTopPauseMaxPlus" + objGruppe);
            btnTopPauseMaxPlus.Click += new RoutedEventHandler(btnTopPauseMaxPlus0_Click);

            tcKlang.SelectedIndex = tcKlang.Items.Count - 2 - 1;

            //**********************************************************************************************************
            GruppenObjekt grpobj = new GruppenObjekt();
            grpobj.seite = tcKlang.SelectedIndex;
            grpobj.ticKlang = tabItem;
            grpobj.objGruppe = tiErstellt;
            grpobj.playlistName = NeuePlaylist;

            grpobj.sviewer = scrViewer;
            grpobj.grdKlang = grdKlang;
            grpobj.grdKlangTop = grdKlangTop;
            grpobj.btnKlangPause = tbtn;
            grpobj.btnImgKlangPause = tbtnImg;

            grpobj.chkbxTopAktiv = chbxTopAkt;
            grpobj.btnTopVolMin = btnTopVolMin;
            grpobj.btnTopVolDown = btnTopVolDown;
            grpobj.btnTopVolUp = btnTopVolUp;
            grpobj.btnTopVolMax = btnTopVolMax;
            grpobj.chkbxTopVolChange = chbxTopVolCh;
            grpobj.btnTopPauseMin = btnTopPauseMin;
            grpobj.btnTopPauseDown = btnTopPauseDown;
            grpobj.btnTopPauseUp = btnTopPauseUp;
            grpobj.btnTopPauseMax = btnTopPauseMax;
            grpobj.chkbxTopPauseChange = chbxTopPauseCh;
            grpobj.btnTopVolMinMinus = btnTopVolMinMinus;
            grpobj.btnTopVolMinPlus = btnTopVolMinPlus;
            grpobj.btnTopVolMaxMinus = btnTopVolMaxMinus;
            grpobj.btnTopVolMaxPlus = btnTopVolMaxPlus;
            grpobj.btnTopPauseMinMinus = btnTopPauseMinMinus;
            grpobj.btnTopPauseMinPlus = btnTopPauseMinPlus;
            grpobj.btnTopPauseMaxMinus = btnTopPauseMaxMinus;
            grpobj.btnTopPauseMaxPlus = btnTopPauseMaxPlus;
            
            _GrpObjecte.Add(grpobj);
            //**********************************************************************************************************
            
            lbKlang.SelectionChanged -= new SelectionChangedEventHandler(lbKlang_SelectionChanged);
            lbKlang.SelectedIndex = -1;
            lbKlang.SelectionChanged += new SelectionChangedEventHandler(lbKlang_SelectionChanged);

            tboxPlaylistName.Text = NeuePlaylist;
            ZeigeKlangSongsParallel(GetPosObjGruppe(objGruppe), false);
            
            tboxklangsongparallel.Text = "0";
            rbIstMusikPlaylist.IsChecked = true;

            // Limit der TabItems
   //         if (tcKlang.Items.Count == 10)
   //             tiPlus.Visibility = Visibility.Hidden;
        }

        private void tboxVolMin0_X_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (var item in e.Text)
                e.Handled = !char.IsDigit(item);
        }
        
        private void tboxVolMin0_X_KeyDown(object sender, KeyEventArgs e)
        {
            if (AktKlangPlaylist != null)
            {
                Int16 objGruppe = GetObjGruppe(tcKlang.SelectedIndex);
                int posObjGruppe = GetPosObjGruppe(objGruppe);

                List<Audio_Titel> titel = Global.ContextAudio.LoadTitelByPlaylist(AktKlangPlaylist);
                List<Audio_Playlist_Titel> playlisttitel = Global.ContextAudio.LoadPlaylist_TitelByPlaylist(AktKlangPlaylist, titel[Convert.ToInt32(((TextBox)(sender)).Tag)]);
                if (playlisttitel.Count != 0)
                {
                    int zeile = Convert.ToInt16(((TextBox)(sender)).Tag);
                    playlisttitel[0].VolumeMin = Convert.ToInt16(((TextBox)(sender)).Text);
                    if (Convert.ToInt16(_GrpObjecte[posObjGruppe]._listZeile[zeile].tboxVolMax.Text) < playlisttitel[0].VolumeMin)
                        _GrpObjecte[posObjGruppe]._listZeile[zeile].tboxVolMax.Text = Convert.ToString(playlisttitel[0].VolumeMin);

                    btnKlangSave.Visibility = Visibility.Visible;  
                }
            }
        }

        private void tboxVolMax0_X_KeyDown(object sender, KeyEventArgs e)
        {
            if (AktKlangPlaylist != null)
            {
                Int16 objGruppe = GetObjGruppe(tcKlang.SelectedIndex);
                int posObjGruppe = GetPosObjGruppe(objGruppe);

                List<Audio_Titel> titel = Global.ContextAudio.LoadTitelByPlaylist(AktKlangPlaylist);
                List<Audio_Playlist_Titel> playlisttitel = Global.ContextAudio.LoadPlaylist_TitelByPlaylist(AktKlangPlaylist, titel[Convert.ToInt32(((TextBox)(sender)).Tag)]);
                if (playlisttitel.Count != 0)
                {
                    int zeile = Convert.ToInt16(((TextBox)(sender)).Tag);                    
                    playlisttitel[0].VolumeMax = Convert.ToInt16(((TextBox)(sender)).Text);

                    if (Convert.ToInt16(_GrpObjecte[posObjGruppe]._listZeile[zeile].tboxVolMin.Text) > playlisttitel[0].VolumeMax)
                        _GrpObjecte[posObjGruppe]._listZeile[zeile].tboxVolMin.Text = Convert.ToString(playlisttitel[0].VolumeMax);

                    btnKlangSave.Visibility = Visibility.Visible;  
                }
            }
        }

        private void tboxPauseMin0_X_KeyDown(object sender, KeyEventArgs e)
        {
            if (AktKlangPlaylist != null)
            {
                Int16 objGruppe = GetObjGruppe(tcKlang.SelectedIndex);
                int posObjGruppe = GetPosObjGruppe(objGruppe);

                List<Audio_Titel> titel = Global.ContextAudio.LoadTitelByPlaylist(AktKlangPlaylist);
                List<Audio_Playlist_Titel> playlisttitel = Global.ContextAudio.LoadPlaylist_TitelByPlaylist(AktKlangPlaylist, titel[Convert.ToInt32(((TextBox)(sender)).Tag)]);
                if (playlisttitel.Count != 0)
                {
                    int zeile = Convert.ToInt16(((TextBox)(sender)).Tag);        
                    playlisttitel[0].PauseMin = Convert.ToInt16(((TextBox)(sender)).Text);

                    if (Convert.ToInt16(_GrpObjecte[posObjGruppe]._listZeile[zeile].tboxPauseMin.Text) > playlisttitel[0].PauseMax)
                        _GrpObjecte[posObjGruppe]._listZeile[zeile].tboxPauseMin.Text = Convert.ToString(playlisttitel[0].PauseMax);

                    btnKlangSave.Visibility = Visibility.Visible;  
                }
            }
        }

        private void tboxPauseMax0_X_KeyDown(object sender, KeyEventArgs e)
        {
            if (AktKlangPlaylist != null)
            {
                Int16 objGruppe = GetObjGruppe(tcKlang.SelectedIndex);
                int posObjGruppe = GetPosObjGruppe(objGruppe);

                List<Audio_Titel> titel = Global.ContextAudio.LoadTitelByPlaylist(AktKlangPlaylist);
                List<Audio_Playlist_Titel> playlisttitel = Global.ContextAudio.LoadPlaylist_TitelByPlaylist(AktKlangPlaylist, titel[Convert.ToInt32(((TextBox)(sender)).Tag)]);
                if (playlisttitel.Count != 0)
                {
                    int zeile = Convert.ToInt16(((TextBox)(sender)).Tag);    
                    playlisttitel[0].PauseMax = Convert.ToInt16(((TextBox)(sender)).Text);

                    if (Convert.ToInt16(_GrpObjecte[posObjGruppe]._listZeile[zeile].tboxPauseMax.Text) < playlisttitel[0].PauseMin)
                        _GrpObjecte[posObjGruppe]._listZeile[zeile].tboxPauseMax.Text = Convert.ToString(playlisttitel[0].PauseMin);

                    btnKlangSave.Visibility = Visibility.Visible;  
                }
            }
        }

        private void _btnVolMinMinus0_X_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));
            KlangZeile klZeile = _GrpObjecte[posObjGruppe]._listZeile.Find(t => t.ID_Zeile == Convert.ToUInt16(
                (((StackPanel)((Grid)((Button)sender).Parent).Parent)).Tag));

            int sollWert = klZeile.volMin_wert - VolSprung;

            if (sollWert <= klZeile.sldKlangVol.Maximum)
                klZeile.volMin_wert = sollWert < 0 ? 0 : sollWert;
            else
                klZeile.volMin_wert = Convert.ToInt16(klZeile.sldKlangVol.Minimum);
            klZeile.tboxVolMin.Text = Convert.ToString(klZeile.volMin_wert);
            klZeile.audiotitel.VolumeMin = klZeile.volMin_wert;
            btnKlangSave.Visibility = Visibility.Visible;  
        }


        private void _btnVolMaxMinus0_X_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));
            KlangZeile klZeile = _GrpObjecte[posObjGruppe]._listZeile.Find(t => t.ID_Zeile == Convert.ToUInt16(
                (((StackPanel)((Grid)((Button)sender).Parent).Parent)).Tag));

            int sollWert = klZeile.volMax_wert - VolSprung;
            int max = Convert.ToInt16(klZeile.sldKlangVol.Maximum);

            if (sollWert <= max)
                klZeile.volMax_wert = sollWert < 0 ? 0 : sollWert;
            else
                klZeile.volMax_wert = max;
            klZeile.tboxVolMax.Text = Convert.ToString(klZeile.volMax_wert);
            if (klZeile.volMax_wert < Convert.ToInt16(klZeile.tboxVolMin.Text))
            {
                klZeile.tboxVolMin.Text = klZeile.tboxVolMax.Text;
                klZeile.volMin_wert = klZeile.volMax_wert;
            }
            klZeile.audiotitel.VolumeMax = klZeile.volMax_wert;
            btnKlangSave.Visibility = Visibility.Visible;  
        }

        private void _btnVolMinPlus0_X_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));
            KlangZeile klZeile = _GrpObjecte[posObjGruppe]._listZeile.Find(t => t.ID_Zeile == Convert.ToUInt16(
               (((StackPanel)((Grid)((Button)sender).Parent).Parent)).Tag));

            int sollWert = klZeile.volMin_wert + VolSprung;
            int max = Convert.ToInt16(klZeile.sldKlangVol.Maximum);

            if (sollWert >= klZeile.sldKlangVol.Minimum)
                klZeile.volMin_wert = sollWert > max ? max : sollWert;
            else
                klZeile.volMin_wert = max;
            klZeile.tboxVolMin.Text = Convert.ToString(klZeile.volMin_wert);
            if (klZeile.volMin_wert > Convert.ToInt16(klZeile.tboxVolMax.Text))
            {
                klZeile.tboxVolMax.Text = klZeile.tboxVolMin.Text;
                klZeile.volMax_wert = klZeile.volMin_wert;
            }
            klZeile.audiotitel.VolumeMin = klZeile.volMin_wert;
            btnKlangSave.Visibility = Visibility.Visible;  
        }

        private void _btnVolMaxPlus0_X_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));
            KlangZeile klZeile = _GrpObjecte[posObjGruppe]._listZeile.Find(t => t.ID_Zeile == Convert.ToUInt16(
                (((StackPanel)((Grid)((Button)sender).Parent).Parent)).Tag));

            int sollWert = klZeile.volMax_wert + VolSprung;
            int max = Convert.ToInt16(klZeile.sldKlangVol.Maximum);

            klZeile.volMax_wert = sollWert < max ? sollWert : max;

            klZeile.tboxVolMax.Text = Convert.ToString(klZeile.volMax_wert);
            klZeile.audiotitel.VolumeMax = klZeile.volMax_wert;
            btnKlangSave.Visibility = Visibility.Visible;  
        }



        private void _btnPauseMinMinus0_X_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));
            KlangZeile klZeile = _GrpObjecte[posObjGruppe]._listZeile.Find(t => t.ID_Zeile == Convert.ToUInt16(
                (((StackPanel)((Grid)((Button)sender).Parent).Parent)).Tag));

            int sollWert = klZeile.pauseMin_wert - PauseSprung;

            if (sollWert <= Convert.ToInt16(klZeile.sldKlangPause.Maximum))
                klZeile.pauseMin_wert = sollWert < 0 ? 0 : sollWert;
            else
                klZeile.pauseMin_wert = Convert.ToInt16(klZeile.sldKlangPause.Minimum);

            klZeile.tboxPauseMin.Text = Convert.ToString(klZeile.pauseMin_wert);
            klZeile.audiotitel.PauseMin = klZeile.pauseMin_wert;
            btnKlangSave.Visibility = Visibility.Visible;  
        }

        private void _btnPauseMaxMinus0_X_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));
            KlangZeile klZeile = _GrpObjecte[posObjGruppe]._listZeile.Find(t => t.ID_Zeile == Convert.ToUInt16(
                (((StackPanel)((Grid)((Button)sender).Parent).Parent)).Tag));
            MyTimer.start_timer();

            int sollWert = klZeile.pauseMax_wert - PauseSprung;
            int max = Convert.ToInt16(klZeile.sldKlangPause.Maximum);

            if (sollWert <= max)
                klZeile.pauseMax_wert = sollWert < 0 ? 0 : sollWert;
            else
                klZeile.pauseMax_wert = max;
            
            MyTimer.stop_timer("");
            klZeile.tboxPauseMax.Text = Convert.ToString(klZeile.pauseMax_wert);
            if (klZeile.pauseMax_wert < Convert.ToInt16(klZeile.tboxPauseMin.Text))
            {
                klZeile.tboxPauseMin.Text = klZeile.tboxPauseMax.Text;
                klZeile.pauseMin_wert = klZeile.pauseMax_wert;
            }
            klZeile.audiotitel.PauseMax = klZeile.pauseMax_wert;
            btnKlangSave.Visibility = Visibility.Visible;     
        }

        private void _btnPauseMinPlus0_X_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));
            KlangZeile klZeile = _GrpObjecte[posObjGruppe]._listZeile.Find(t => t.ID_Zeile == Convert.ToUInt16(
               (((StackPanel)((Grid)((Button)sender).Parent).Parent)).Tag));

            int sollWert = klZeile.pauseMin_wert + PauseSprung;
            int max = Convert.ToInt16(klZeile.sldKlangPause.Maximum);

            if (sollWert >= klZeile.sldKlangPause.Minimum)
                klZeile.pauseMin_wert = sollWert > max? max : sollWert;
            else
                klZeile.pauseMin_wert= max;
            klZeile.tboxPauseMin.Text = Convert.ToString(klZeile.pauseMin_wert);
            if (klZeile.pauseMin_wert > Convert.ToInt16(klZeile.tboxPauseMax.Text))
            {
                klZeile.tboxPauseMax.Text = klZeile.tboxPauseMin.Text;
                klZeile.pauseMax_wert = klZeile.pauseMin_wert;
            }
            klZeile.audiotitel.PauseMin = klZeile.pauseMin_wert;
            btnKlangSave.Visibility = Visibility.Visible;          
        }

        private void _btnPauseMaxPlus0_X_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));
            KlangZeile klZeile = _GrpObjecte[posObjGruppe]._listZeile.Find(t => t.ID_Zeile == Convert.ToUInt16(
                (((StackPanel)((Grid)((Button)sender).Parent).Parent)).Tag));
            
            int sollWert = klZeile.pauseMax_wert + PauseSprung;
            int max = Convert.ToInt16(klZeile.sldKlangPause.Maximum);

            klZeile.pauseMax_wert = sollWert < max ? sollWert : max;
            klZeile.tboxPauseMax.Text = Convert.ToString(klZeile.pauseMax_wert);
            klZeile.audiotitel.PauseMax = klZeile.pauseMax_wert;
            btnKlangSave.Visibility = Visibility.Visible;      
        }

        private void UpdateAudio_Playlist_Titel(Audio_Playlist_Titel aPlaylistTitel)
        {
            try { Global.ContextAudio.Update<Audio_Playlist_Titel>(aPlaylistTitel); }
            catch (Exception ex)
            {
                var errWin = new MsgWindow("Datenfehler", "Die Datenbank konnte nicht aktualisiert werden", ex);
                errWin.ShowDialog();
                errWin.Close();
            }
        }

        private void btnKlangPause0_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(Convert.ToInt16(tcKlang.Tag)));  //GetPosObjGruppe(AktKlangPlaylist.Convert.ToInt16(((ToggleButton)sender).Tag));
            ((Button)sender).Tag = !Convert.ToBoolean(((Button)sender).Tag);

            for (int i = 0; i < _GrpObjecte[posObjGruppe]._listZeile.Count; i++)
            {
                _GrpObjecte[posObjGruppe]._listZeile[i].istPause = Convert.ToBoolean(((Button)sender).Tag);// .IsChecked.Value; // true;
                if (_GrpObjecte[posObjGruppe]._listZeile[i].istPause && _GrpObjecte[posObjGruppe]._listZeile[i].istLaufend)
                    _GrpObjecte[posObjGruppe]._listZeile[i]._mplayer.Pause();
                if (!_GrpObjecte[posObjGruppe]._listZeile[i].istPause && _GrpObjecte[posObjGruppe]._listZeile[i].istLaufend)
                    _GrpObjecte[posObjGruppe]._listZeile[i]._mplayer.Play();
                if (!_GrpObjecte[posObjGruppe]._listZeile[i].istPause && !_GrpObjecte[posObjGruppe]._listZeile[i].istLaufend && 
                    _GrpObjecte[posObjGruppe]._listZeile[i].chkTitel.IsChecked.Value)
                    _GrpObjecte[posObjGruppe]._listZeile[i].istStandby = true;
            }
            CheckPlayStandbySongs(posObjGruppe);
            if (Convert.ToBoolean(_GrpObjecte[posObjGruppe].btnKlangPause.Tag))
                _GrpObjecte[posObjGruppe].btnImgKlangPause.Source = new BitmapImage(new Uri("pack://application:,,,/DSA MeisterGeister;component/Images/Icons/General/play.png"));
            else
                _GrpObjecte[posObjGruppe].btnImgKlangPause.Source = new BitmapImage(new Uri("pack://application:,,,/DSA MeisterGeister;component/Images/Icons/General/pause.png"));
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
                        int objGruppe = GetObjGruppe(tcKlang.SelectedIndex);
                        int posObjGruppe = GetPosObjGruppe(objGruppe);
                        _GrpObjecte[posObjGruppe].maxsongparallel = Convert.ToUInt16(AktKlangPlaylist.MaxSongsParallel);

                        if (rbtnGleichSpielen.IsChecked == true || !Convert.ToBoolean(_GrpObjecte[posObjGruppe].btnKlangPause.Tag))
                            CheckPlayStandbySongs(posObjGruppe);

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
                    //maxsongsparallel[tcKlang.SelectedIndex] = AktKlangPlaylist.MaxSongsParallel;
                }
            }
        }

        private Int16 GetObjGruppe(int seite)
        {
            List<GruppenObjekt> listeGrpObjecte = _GrpObjecte.Where(t => t.seite.Equals(seite)).ToList();
            if (listeGrpObjecte.Count == 1)
                return Convert.ToInt16(listeGrpObjecte[0].objGruppe);
            else
                return -1;
        }

        private void btnSongParPlus_Click(object sender, RoutedEventArgs e)
        {
            int dif = Convert.ToInt32(((Button)sender).Tag);
            int momentan = Convert.ToInt32(tboxklangsongparallel.Text);
            int max = Convert.ToInt32(tboxklangsongparallel.Tag);
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));

            if ((dif > 0 && dif + momentan <= max) ||
               ((dif < 0 && dif + momentan >= 0)))
            {
                tboxklangsongparallel.Text = (Convert.ToInt32(tboxklangsongparallel.Text) + dif).ToString();
                _GrpObjecte[posObjGruppe].maxsongparallel = Convert.ToUInt16(tboxklangsongparallel.Text);
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
                            Int16 objGruppe = Convert.ToInt16(GetObjGruppe(tcKlang.SelectedIndex));
                            if (objGruppe == -1)
                                return;

                            Int16 posObjGruppe = GetPosObjGruppe(objGruppe);
                            if (posObjGruppe == -1)
                                return;
                            PlaylisteLeeren(posObjGruppe);

                            tboxPlaylistName.Text = "NeuePlayliste" + tcKlang.SelectedIndex;
                            if (tcKlang.SelectedIndex == 0)
                                ((TabItem)tcKlang.SelectedItem).Header = tboxPlaylistName.Text;
                            else
                                ((TabItemControl)tcKlang.SelectedItem)._textBlockTitel.Text = tboxPlaylistName.Text;

                            tboxklangsongparallel.TextChanged -= new TextChangedEventHandler(tboxklangsongparallel_TextChanged);
                            tboxklangsongparallel.Tag = null;
                            tboxklangsongparallel.Text = "0";
                            _GrpObjecte[posObjGruppe].maxsongparallel = 0;
                            tboxklangsongparallel.TextChanged += new TextChangedEventHandler(tboxklangsongparallel_TextChanged);

                            //zeilenAufSeite[tcKlang.SelectedIndex] = 0;
                            ZeigeKlangSongsParallel(posObjGruppe, false);

                            _GrpObjecte[posObjGruppe].grdKlangTop.Visibility = Visibility.Hidden;
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
                        for (int i = 0; i < titel.Count; i++)
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
            if (_GrpObjecte[0] != null) // _player != null)
            {
                UInt16 seite = 0;
                KlangZeile klZeile = null;
                while (klZeile == null)
                {
                    klZeile = _GrpObjecte[seite]._listZeile.Find(t => t.ID_Zeile == Convert.ToUInt16(((Slider)e.Source).Tag));
                    seite++;
                }
                Int16 neuerWert = Convert.ToInt16(Math.Round(e.NewValue));
                if (klZeile._mplayer != null)
                    klZeile._mplayer.Volume = Convert.ToDouble(neuerWert) / 100;
                klZeile.sldKlangVol.ToolTip = neuerWert + " %";

                klZeile.audiotitel.Volume = neuerWert;
                btnKlangSave.Visibility = Visibility.Visible;  
            }
        }
        private void sldPlaySpeed0_X_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_GrpObjecte[0] != null)
            {
                Slider sld = ((Slider)sender);
                int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));
                int zeile = Convert.ToInt32(sld.Tag);
                double speed = sld.Value;
                if (_GrpObjecte[posObjGruppe]._listZeile[zeile]._mplayer != null)
                    _GrpObjecte[posObjGruppe]._listZeile[zeile]._mplayer.SpeedRatio = speed;

                _GrpObjecte[posObjGruppe]._listZeile[zeile].audiotitel.Speed = _GrpObjecte[posObjGruppe]._listZeile[zeile]._mplayer.SpeedRatio;
                btnKlangSave.Visibility = Visibility.Visible;
                string geschw = "Abspielgeschwindigkeit: ";

                geschw += speed == .1 ? "sehr langsam" :
                          speed == .5 ? "langsam" :
                          speed == .75 ? "gedrosselt" :
                          speed == 1 ? "normal" :
                          speed == 2 ? "erhöht" :
                          speed == 3 ? "schnell" :
                          speed == 4 ? "sehr schnell" : 
                          speed == 5 ? "utlra schnell" : "nicht definiert";
                sld.ToolTip = geschw;
            }
        }
        

        private void sldKlangPause0_X_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_GrpObjecte[0] != null) 
            {
                UInt16 seite = 0;
                KlangZeile klZeile = null;
                while (klZeile == null)
                {
                    klZeile = _GrpObjecte[seite]._listZeile.Find(t => t.ID_Zeile == Convert.ToUInt16(((Slider)e.Source).Tag));
                    seite++;
                }
                long neuerWert = Convert.ToInt64(Math.Round(e.NewValue));
                klZeile.sldKlangPause.ToolTip = neuerWert + " ms";
                klZeile.audiotitel.Pause = neuerWert;
                btnKlangSave.Visibility = Visibility.Visible;         
            }
        }

        private void btnAllVolUp_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));
            double d = Convert.ToDouble(((sender) as Button).Tag);
            for (int zeile = 0; zeile < _GrpObjecte[posObjGruppe]._listZeile.Count; zeile++)
            {
                KlangZeile klZeile = _GrpObjecte[posObjGruppe]._listZeile[zeile];
                if (klZeile.spnlKlangRow != null &&
                    klZeile.chkTitel.IsChecked == true)
                    klZeile.sldKlangVol.Value += d;
            }
        }

        private void btnAllPauseUp_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));
            double d = Convert.ToDouble(((sender) as Button).Tag);

            for (int zeile = 0; zeile < _GrpObjecte[posObjGruppe]._listZeile.Count; zeile++)
            {
                if (_GrpObjecte[posObjGruppe]._listZeile[zeile].spnlKlangRow != null &&
                    _GrpObjecte[posObjGruppe]._listZeile[zeile].chkTitel.IsChecked == true)
                    _GrpObjecte[posObjGruppe]._listZeile[zeile].sldKlangPause.Value += d;
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
                            Int16 objGruppe = Convert.ToInt16(GetObjGruppe(tcKlang.SelectedIndex));
                            if (objGruppe == -1)
                                return;
                            Int16 posObjGruppe = GetPosObjGruppe(objGruppe);
                            _GrpObjecte[posObjGruppe].grdKlangTop.Visibility = Visibility.Visible;


                            if (rbIstKlangPlaylist.IsChecked == true)
                                AktKlangPlaylist.Hintergrundmusik = false;
                            else
                                AktKlangPlaylist.Hintergrundmusik = true;

                            if (AktKlangPlaylist.Hintergrundmusik)
                            {
                                ZeigeKlangSongsParallel(posObjGruppe, false);
                                ZeigeKlangTop(posObjGruppe, false);
                            }
                            else
                            {
                                ZeigeKlangSongsParallel(posObjGruppe, true);
                                ZeigeKlangTop(posObjGruppe, true);
                            }
                            CheckAlleAngehakt(GetPosObjGruppe(objGruppe));
                            _GrpObjecte[posObjGruppe].grdKlang.Visibility = Visibility.Visible;
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
            CustomMessage("Codec Add-On Hinweis", "OGG-Dateien integrieren",
                "OGG-Dateien können nach dem Installieren eines entsprechenden AddOns bzw. " + Environment.NewLine + 
                "Codec-Packs ebenfalls wiedergegeben werden." + Environment.NewLine + Environment.NewLine + 
                "Ein entsprechendes Codec bietet das 'Media Player Codec Pack 4.2.2' und " + Environment.NewLine +
                "kann unter folgender Adresse heruntergeladen werden:" + Environment.NewLine + Environment.NewLine,
                "http://cloudfront.mediaplayercodecpack.com/files/as-tb/media.player.codec.pack.v4.2.2.setup.exe");
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
                hyperl.Click += new RoutedEventHandler(hyperlink_Click);
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
            w.Width = 600;
            w.Height = 300;
            w.Show();
        }

        private void hyperlink_Click(object sender, RoutedEventArgs e)
        {
             System.Diagnostics.Process.Start(((Hyperlink)sender).NavigateUri.AbsoluteUri);
        }

        private void btnTopVolMinMinus0_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));

            List<KlangZeile> klZeilen = _GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.chkTitel.IsChecked.Value);
            for (int i = 0; i < klZeilen.Count; i++)
                _btnVolMinMinus0_X_Click(klZeilen[i].btnVolMinMinus, e);
        }

        private void btnTopVolMinPlus0_Click(object sender, RoutedEventArgs e)
        {            
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));
            
            List<KlangZeile> klZeilen = _GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.chkTitel.IsChecked.Value);

            for (int i = 0; i < klZeilen.Count; i++)
                _btnVolMinPlus0_X_Click(klZeilen[i].btnVolMinMinus, e);
        }

        private void btnTopVolMaxMinus0_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));

            List<KlangZeile> klZeilen = _GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.chkTitel.IsChecked.Value);

            for (int i = 0; i < klZeilen.Count; i++)
                _btnVolMaxMinus0_X_Click(klZeilen[i].btnVolMinMinus, e);
        }

        private void btnTopVolMaxPlus0_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));

            List<KlangZeile> klZeilen = _GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.chkTitel.IsChecked.Value);

            for (int i = 0; i < klZeilen.Count; i++)
                _btnVolMaxPlus0_X_Click(klZeilen[i].btnVolMinMinus, e);
        }        
       
        private void btnTopPauseMinMinus0_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));

            List<KlangZeile> klZeilen = _GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.chkTitel.IsChecked.Value);

            for (int i = 0; i < klZeilen.Count; i++)
                _btnPauseMinMinus0_X_Click(klZeilen[i].btnVolMinMinus, e);    
        }

        private void btnTopPauseMinPlus0_Click(object sender, RoutedEventArgs e)
        {
         /*   int seite = tcKlang.SelectedIndex;
            UInt16 objGruppe = Convert.ToUInt16(seite);
            int posObjGruppe = GetPosObjGruppe(objGruppe);*/
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));

            List<KlangZeile> klZeilen = _GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.chkTitel.IsChecked.Value);

            for (int i = 0; i < klZeilen.Count; i++)
                _btnPauseMinPlus0_X_Click(klZeilen[i].btnVolMinMinus, e);           
        }

        private void btnTopPauseMaxMinus0_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));

            List<KlangZeile> klZeilen = _GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.chkTitel.IsChecked.Value);

            for (int i = 0; i < klZeilen.Count; i++)
                _btnPauseMaxMinus0_X_Click(klZeilen[i].btnVolMinMinus, e);           
        }

        private void btnTopPauseMaxPlus0_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));

            List<KlangZeile> klZeilen = _GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.chkTitel.IsChecked.Value);

            for (int i = 0; i < klZeilen.Count; i++)
                _btnPauseMaxPlus0_X_Click(klZeilen[i].btnVolMinMinus, e);          
        }
        
        private void chkbxTopAktiv0_Click(object sender, RoutedEventArgs e)
        {
            UInt16 objGruppe = Convert.ToUInt16(((CheckBox)sender).Tag);
            int posObjGruppe = GetPosObjGruppe(objGruppe);

            for (int zeile = 0; zeile < _GrpObjecte[posObjGruppe]._listZeile.Count; zeile++)
                _GrpObjecte[posObjGruppe]._listZeile[zeile].chkTitel.IsChecked = ((CheckBox)(e.Source)).IsChecked;

            if (_GrpObjecte[posObjGruppe].anzPauseChange == _GrpObjecte[posObjGruppe]._listZeile.Count && 
                ((CheckBox)(e.Source)).IsChecked == true)
            {
                //Zufallsaktivierung der Zeilen

                List<KlangZeile> klZeileAktiv;
                klZeileAktiv = _GrpObjecte[posObjGruppe]._listZeile.FindAll(t => t.playable == true);

                while (klZeileAktiv.Count > 0)
                {

                    /*       int[] Zeilepos = int[klZeileAktiv.Count];
                           for (int i = 0; i < klZeileAktiv.Count; i++)
                               Zeilepos[i] = _GrpObjecte[posObjGruppe]._listZeile.IndexOf(klZeileAktiv[i]);
                               */
                    Würfel w = new Würfel(Convert.ToUInt16(klZeileAktiv.Count));
                    w.Würfeln(1);
                    int zeileIndex = _GrpObjecte[posObjGruppe]._listZeile.IndexOf(klZeileAktiv[w.Ergebnis - 1]);
                    chkTitel0_0_Click(_GrpObjecte[posObjGruppe]._listZeile[zeileIndex].chkTitel, e);

                    klZeileAktiv = klZeileAktiv.FindAll(t => t.istLaufend != true);
                    klZeileAktiv = klZeileAktiv.FindAll(t => t.istStandby != true);
                }
            }
        }
        
        private void chkbxTopVolChange0_Click(object sender, RoutedEventArgs e)
        {
            Int16 objGruppe = Convert.ToInt16(((CheckBox)sender).Tag);
            int posObjGruppe = GetPosObjGruppe(objGruppe);
            bool changeto = ((CheckBox)sender).IsChecked.Value;
            for (int zeile = 0; zeile < _GrpObjecte[posObjGruppe]._listZeile.Count; zeile++)
            {
                CheckBox chChange;
                if (((CheckBox)e.Source).Name.StartsWith("chkbxTopVol"))
                    chChange = _GrpObjecte[posObjGruppe]._listZeile[zeile].chkVolMove;
                else
                    chChange = _GrpObjecte[posObjGruppe]._listZeile[zeile].chkKlangPauseMove;

                if (_GrpObjecte[posObjGruppe]._listZeile[zeile].chkTitel.IsChecked == true)
                {
                    chChange.IsChecked = changeto;
                    if (((CheckBox)e.Source).Name.StartsWith("chkbxTopVol"))
                        chkVolMove0_0_Click(chChange, new RoutedEventArgs());
                    else
                        chkKlangPauseMove0_0_Click(chChange, new RoutedEventArgs());
                }
            }
            CheckAlleAngehakt(posObjGruppe);
        }


        private void chkbxPlayRange_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked.Value)
                rsldTeilSong.Visibility = Visibility.Visible;
            else
                rsldTeilSong.Visibility = Visibility.Hidden;

            AktBGPlaylistTitel.TeilAbspielen = chkbxPlayRange.IsChecked.Value;
            if (!AktBGPlaylistTitel.TeilAbspielen)
            {
                AktBGPlaylistTitel.TeilStart = null;
                AktBGPlaylistTitel.TeilEnde = null;
            }
            else
            {
                AktBGPlaylistTitel.TeilStart = rsldTeilSong.LowerValue;
                AktBGPlaylistTitel.TeilEnde = rsldTeilSong.UpperValue;
            }
            Global.ContextAudio.Update<Audio_Playlist_Titel>(AktBGPlaylistTitel);                
        }
        
        private void btnShuffle_Click(object sender, RoutedEventArgs e)
        {
            if (((ToggleButton)sender).IsChecked == true)
                btnShuffleImg.Source = new BitmapImage(new Uri("pack://application:,,,/DSA MeisterGeister;component/Images/Icons/General/shuffle.png"));
            else
                btnShuffleImg.Source = new BitmapImage(new Uri("pack://application:,,,/DSA MeisterGeister;component/Images/Icons/General/no_shuffle.png"));            
        }
        
        private void btnAudioThemePlayNext_Click(object sender, RoutedEventArgs e)
        {
            int momLaufend = _ThemeGruppe[Convert.ToInt16(((Button)sender).Tag)].Hintergrund._listZeile.FindIndex(t => t.istLaufend);
            if (momLaufend >= 0)
                _ThemeGruppe[Convert.ToInt16(((Button)sender).Tag)].Hintergrund._listZeile[momLaufend]._mplayer.Stop();
        }

        private void ThemeSchalten(int theme, bool SollZustand)
        {
            if (!SollZustand)
            {
                _ThemeGruppe[theme].pnlAudioTheme.btnAudioTheme.IsChecked = SollZustand;
                _ThemeGruppe[theme].pnlAudioTheme.imgPlay.Tag = 0;
                _ThemeGruppe[theme].pnlAudioTheme.imgPlay.Visibility = Visibility.Hidden;
                _ThemeGruppe[theme].IstActiv = false;
                if (_ThemeGruppe[theme].Hintergrund != null && !Convert.ToBoolean(_ThemeGruppe[theme].Hintergrund.btnKlangPause.Tag))
                {
                    _ThemeGruppe[theme].Hintergrund.btnKlangPause.Tag = true;
                    btnKlangPause0_Click(_ThemeGruppe[theme].Hintergrund.btnKlangPause, new RoutedEventArgs());  

                    for (int i = 0; i < _ThemeGruppe[theme].Klaenge.Count; i++)
                    {
                        _ThemeGruppe[theme].Klaenge[i].btnKlangPause.Tag = true;
                        btnKlangPause0_Click(_ThemeGruppe[theme].Klaenge[i].btnKlangPause, new RoutedEventArgs());
                    }
                } 
                CheckThemePlayListAuswahl(null);
            }
            else
            {
                _ThemeGruppe[theme].IstActiv = SollZustand;

                if (rbtnGleichSpielen.IsChecked.Value)
                {
                    aktiveThemeGruppe = theme;
                    _ThemeGruppe[theme].Hintergrund.btnKlangPause.Tag = SollZustand;
                    btnKlangPause0_Click(_ThemeGruppe[theme].Hintergrund.btnKlangPause, new RoutedEventArgs());
                    
                    for (int i = 0; i < _ThemeGruppe[theme].Klaenge.Count; i++)
                    {
                        _ThemeGruppe[theme].Klaenge[i].btnKlangPause.Tag = !SollZustand;
                        tcKlang.Tag = _ThemeGruppe[theme].Klaenge[i].seite;
                        btnKlangPause0_Click(_ThemeGruppe[theme].Klaenge[i].btnKlangPause, new RoutedEventArgs());
                    }
                }
                CheckThemePlayListAuswahl(_ThemeGruppe[theme]);
            }
        }

        private void btnAudioTheme_Click(object sender, RoutedEventArgs e)
        {
    /*        int momAktTheme = _ThemeGruppe.FindIndex(t => t.IstActiv == true);
            if (momAktTheme >= 0)
                ThemeSchalten(momAktTheme, false);

            if (momAktTheme != Convert.ToInt16(((ToggleButton)sender).Tag))
                ThemeSchalten(Convert.ToInt16(((ToggleButton)sender).Tag), true);*/
        }

        private void CheckThemePlayListAuswahl(ThemeGruppe grpTheme)
        {
            for (int i = 0; i < ThemeLstBGPanel.Count; i++)
            {
                ThemeLstBGPanel[i].btnAngehakt.IsChecked = false;
                ThemeLstBGPanel[i].btnAngehakt.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            for (int i = 0; i < ThemeLstKlangPanel.Count; i++)
            {
                ThemeLstKlangPanel[i].btnAngehakt.IsChecked = false;
                ThemeLstKlangPanel[i].btnAngehakt.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }

            if (grpTheme != null)
            {
                HitchPanel hBGPanel = ThemeLstBGPanel.Find(t => t.lblThemeName.Content.Equals(grpTheme.Hintergrund.playlistName));
                if (!hBGPanel.btnAngehakt.IsChecked.Value)
                {
                    hBGPanel.btnAngehakt.IsChecked = true;
                    hBGPanel.btnAngehakt.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }

                for (int i = 0; i < grpTheme.Klaenge.Count; i++)
                {
                    HitchPanel hKlangPanel = ThemeLstKlangPanel.Find(t => t.lblThemeName.Content.Equals(grpTheme.Klaenge[i].playlistName));
                    if (!hKlangPanel.btnAngehakt.IsChecked.Value)
                    {
                        hKlangPanel.btnAngehakt.IsChecked = true;
                        hKlangPanel.btnAngehakt.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    }
                }
            }
        }

        private void pbarThemeActBGTitel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (aktiveThemeGruppe >= 0)
            {
                _ThemeGruppe[aktiveThemeGruppe].pnlAudioTheme.btnAudioTheme.Click -= new RoutedEventHandler(btnAudioTheme_Click);

                Point pts = e.GetPosition(sender as ProgressBar);
                double total = (sender as ProgressBar).Maximum;
                double res = ((pts.X * 100) / ((double)((sender as ProgressBar).ActualWidth)) / 100);

                int momAktSong = _ThemeGruppe[aktiveThemeGruppe].Hintergrund._listZeile.FindIndex(t => t.istLaufend == true);
                if (momAktSong >= 0)
                    _ThemeGruppe[aktiveThemeGruppe].Hintergrund._listZeile[momAktSong]._mplayer.Position = TimeSpan.FromMilliseconds(total * res);

                _ThemeGruppe[aktiveThemeGruppe].pnlAudioTheme.btnAudioTheme.Click += new RoutedEventHandler(btnAudioTheme_Click);
            }
        }

        private void AktualisiereThemeEditor()
        {
            List<Audio_Playlist> hintergrundplaylist = Global.ContextAudio.PlaylistListe.FindAll(t => t.Hintergrundmusik == true).ToList();            
            List<Audio_Playlist> klangPlaylist = Global.ContextAudio.PlaylistListe.FindAll(t => t.Hintergrundmusik == false).ToList();
            
            //Zeilen des Theme-Editors hinzufügen
            for (int row = 0; row <= hintergrundplaylist.Count - grdHintergrund.RowDefinitions.Count + 1; row++)
            {
                RowDefinition rowDef1 = new RowDefinition();
                rowDef1.Height = new GridLength(grdHintergrund.RowDefinitions[0].Height.Value);
                grdHintergrund.RowDefinitions.Insert(0, rowDef1);
            } 
            for (int row = 0; row <= klangPlaylist.Count / 3 - grdKlaenge.RowDefinitions.Count + 1; row++)
            {
                RowDefinition rowDef2 = new RowDefinition();
                rowDef2.Height = new GridLength(grdKlaenge.RowDefinitions[0].Height.Value);
                grdKlaenge.RowDefinitions.Insert(0, rowDef2);
            }

            //Panels des Theme-Editors hinzufügen
            for (int i = 0; i < hintergrundplaylist.Count; i++)
            {
                HitchPanel _ThemePanel = new HitchPanel();
                _ThemePanel.lblThemeName.Content = hintergrundplaylist[i].Name;
                _ThemePanel.Name = "ThemeBGPanel" + i;
                _ThemePanel.Tag = hintergrundplaylist[i].Audio_PlaylistGUID;
                _ThemePanel.btnAngehakt.Tag = hintergrundplaylist[i].Audio_PlaylistGUID;
                _ThemePanel.btnAngehakt.Checked += new RoutedEventHandler(btnBGthemeAngehakt_Checked);
                _ThemePanel.btnAngehakt.Unchecked += new RoutedEventHandler(btnBGthemeAngehakt_UnChecked);
                grdHintergrund.Children.Add(_ThemePanel);
                Grid.SetRow(_ThemePanel, i);
                Grid.SetColumn(_ThemePanel, 0);
                ThemeLstBGPanel.Add(_ThemePanel);
            }

            for (int i = 0; i < klangPlaylist.Count; i++)
            {
                HitchPanel _ThemeKlangPanel = new HitchPanel();
                _ThemeKlangPanel.lblThemeName.Content = klangPlaylist[i].Name;
                _ThemeKlangPanel.Name = "ThemeKlangPanel" + i;
                _ThemeKlangPanel.Tag = klangPlaylist[i].Audio_PlaylistGUID;
                _ThemeKlangPanel.btnAngehakt.Tag = klangPlaylist[i].Audio_PlaylistGUID;
                _ThemeKlangPanel.btnAngehakt.Checked += new RoutedEventHandler(btnKlangThemeAngehakt_Checked);
                _ThemeKlangPanel.btnAngehakt.Unchecked += new RoutedEventHandler(btnKlangThemeAngehakt_UnChecked);
                grdKlaenge.Children.Add(_ThemeKlangPanel);
                int x = klangPlaylist.Count; //_ThemeGruppe[i].KlangeThemePlaylist.Count;
                Int32 div = Math.Abs((i / 3));
                Grid.SetRow(_ThemeKlangPanel, div);
                Grid.SetColumn(_ThemeKlangPanel, i - div * 3);
                ThemeLstKlangPanel.Add(_ThemeKlangPanel);
            }
        }

        private void AktualisiereThemeGruppe()
        {

            List<Audio_Theme> aThemes = Global.ContextAudio.ThemeListe;              
            int AnzThemes = aThemes.Count;

            AktualisiereThemeEditor();
            
            for (int i = 0; i < AnzThemes; i++)
            {
                lblNoThemes.Content = "Lade " + (i+1) + " von " + AnzThemes + " Audio-Themes ..." + Environment.NewLine + Environment.NewLine + "Bitte warten...";
                lblNoThemes.Refresh();

                ThemeGruppe _ThemeGrp = new ThemeGruppe();
                _ThemeGrp.ThemeName = "Theme" + i;
                
                _ThemeGrp.dbAudioTheme = aThemes[i];
                
                rbKlangAlle.IsChecked = true;
                AktualisiereKlangPlaylist();

                _ThemeGrp.pnlAudioTheme = new AudioTheme();
                _ThemeGrp.pnlAudioTheme.Name = "AudioTheme" + i;
                _ThemeGrp.pnlAudioTheme.btnAudioTheme.Tag = i;
                _ThemeGrp.pnlAudioTheme.btnAudioTheme.Click += new RoutedEventHandler(btnAudioTheme_Click);
                _ThemeGrp.pnlAudioTheme.btnPlayNext.Tag = i;
                _ThemeGrp.pnlAudioTheme.btnPlayNext.Click += new RoutedEventHandler(btnAudioThemePlayNext_Click);

                _ThemeGrp.pnlAudioTheme.pbarActBGTitel.MouseLeftButtonDown += new MouseButtonEventHandler(pbarThemeActBGTitel_MouseLeftButtonDown);
                
                _ThemeGrp.HGThemePlaylist = aThemes[i].Audio_Playlist.First(t => t.Hintergrundmusik == true);

                _ThemeGrp.pnlAudioTheme.lblThemeName.Content = _ThemeGrp.HGThemePlaylist.Name;

                //HintergrundTheme laden
                tcKlang.SelectedIndex = tcKlang.Items.Count - 3;
                tiPlus_MouseUp(null, null);
                int hgItemPos = 0;
                while (((ListBoxItem)lbKlang.Items[hgItemPos]).Content.ToString() != _ThemeGrp.HGThemePlaylist.Name)
                    hgItemPos++;
                lbKlang.SelectedIndex = hgItemPos;

                GruppenObjekt grpObjThemeHG = _GrpObjecte.Find(t => t.seite.Equals(tcKlang.SelectedIndex));
                ((TabItemControl)tcKlang.SelectedItem).Visibility = Visibility.Collapsed;

                _ThemeGrp.Hintergrund = grpObjThemeHG;
                _ThemeGrp.pnlAudioTheme.lblActBGTitel.Content = _ThemeGrp.dbAudioTheme.Name;
                _ThemeGrp.pnlAudioTheme.ToolTip = "Hintergrundmusik:    " + _ThemeGrp.HGThemePlaylist.Name;

                //Klang-Themes laden
                for (int x = 0; x < aThemes[i].Audio_Playlist.Count; x++)
                {
                    if (!aThemes[i].Audio_Playlist.ElementAt(x).Hintergrundmusik)
                    {
                        tcKlang.SelectedIndex = tcKlang.Items.Count - 3;
                        tiPlus_MouseUp(null, null);

                        int plylstItemPos = 0;
                        while (((ListBoxItem)lbKlang.Items[plylstItemPos]).Content.ToString() != aThemes[i].Audio_Playlist.ElementAt(x).Name)
                            plylstItemPos++;
                        lbKlang.SelectedIndex = plylstItemPos;

                        GruppenObjekt grpObjTheme = _GrpObjecte.Find(t => t.seite.Equals(tcKlang.SelectedIndex));
                        ((TabItemControl)tcKlang.SelectedItem).Visibility = Visibility.Collapsed;

                        _ThemeGrp.Klaenge.Add(grpObjTheme);
                        _ThemeGrp.pnlAudioTheme.ToolTip = _ThemeGrp.pnlAudioTheme.ToolTip + Environment.NewLine +
                            "Klang " + (x + 1) + ":  " + aThemes[i].Audio_Playlist.ElementAt(x).Name;
                    }
                }
                //Theme hinzufügen
                _ThemeGruppe.Add(_ThemeGrp);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            lblNoThemes.Content = "Lade Audio-Themes Informationen" + Environment.NewLine + Environment.NewLine + "Bitte warten...";
            lblNoThemes.Visibility = Visibility.Visible;
            lblNoThemes.Refresh();

            for (int i = 1; i < tcKlang.Items.Count - 3; i++)
            {
                if (((TabItemControl)tcKlang.Items[i]).Visibility == Visibility.Collapsed)
                    tiKlangPlaylistClose_Click(((TabItemControl)tcKlang.Items[i]), new RoutedEventArgs());
            }

            for (int i = 0; i < _ThemeGruppe.Count; i++)
                grdUebersicht.Children.Remove(_ThemeGruppe[i].pnlAudioTheme);
            _ThemeGruppe.Clear();

            AktualisiereThemeGruppe();

            lblNoThemes.Content = "Alle Audio-Themes geladen." + Environment.NewLine + Environment.NewLine + "Bitte warten...";
            lblNoThemes.Refresh();
            tcKlang.SelectedIndex = 0;
            lblNoThemes.Content = _ThemeGruppe.Count == 0? "Keine Audio-Themes gefunden": "";

            RowDefinition rowDef1 = new RowDefinition();
            for (int i = 0; i < _ThemeGruppe.Count / 3 - grdUebersicht.RowDefinitions.Count; i++)
            {
                rowDef1.Height = new GridLength(grdUebersicht.RowDefinitions[1].Height.Value);
                grdUebersicht.RowDefinitions.Add(rowDef1);
            }

            for (int i = 0; i < _ThemeGruppe.Count; i++)
            {
                grdUebersicht.Children.Add(_ThemeGruppe[i].pnlAudioTheme);
                int x = _ThemeGruppe.Count;
                Int32 div = Math.Abs((i / 4));
                Grid.SetRow(_ThemeGruppe[i].pnlAudioTheme, 1 + div);
                Grid.SetColumn(_ThemeGruppe[i].pnlAudioTheme, i - div * 4);
            }
            lblNoThemes.Visibility = _ThemeGruppe.Count == 0? Visibility.Visible : Visibility.Hidden;
        }

        private void exThemeEditor_Expanded(object sender, RoutedEventArgs e)
        {
            grdThemes.RowDefinitions[0].Height = new GridLength(grdThemes.RowDefinitions[0].Height.Value, GridUnitType.Auto);
            grdThemes.RowDefinitions[1].Height = new GridLength(22, GridUnitType.Star);
        }

        private void exThemeEditor_Collapsed(object sender, RoutedEventArgs e)
        {
            grdThemes.RowDefinitions[0].Height = new GridLength(grdThemes.RowDefinitions[0].Height.Value, GridUnitType.Star);
            grdThemes.RowDefinitions[1].Height = new GridLength(22, GridUnitType.Pixel);
        }

        private void tiUebersicht_GotFocus(object sender, RoutedEventArgs e)
        {


        }

        private void btnBGthemeAngehakt_Checked(object sender, RoutedEventArgs e)
        {
/*            if (!AktThemeGruppe.IstActiv)
                AktThemeGruppe.ThemeName = (tboxThemeBezeichnung.Text != "") ? tboxThemeBezeichnung.Text : "Neue Themegruppe";

            for (int i = 0; i < ThemeLstBGPanel.Count; i++)
            {
                if (ThemeLstBGPanel[i].btnAngehakt.IsChecked.Value && ThemeLstBGPanel[i].btnAngehakt.Tag != (sender as ToggleButton).Tag)
                {
                    ThemeLstBGPanel[i].btnAngehakt.IsChecked = false;
                    break;
                }
            }
            AktThemeGruppe.IstActiv = true;
            List<Audio_Playlist> aThemePlaylist = AktThemeGruppe.dbAudioTheme.Audio_Playlist.ToList();
            AktThemeGruppe.HGThemePlaylist.Audio_Playlist = Global.ContextAudio.PlaylistListe.Find(t => t.Audio_PlaylistGUID.Equals((sender as ToggleButton).Tag));
*/        }

        private void btnBGthemeAngehakt_UnChecked(object sender, RoutedEventArgs e)
        {

  /*          AktThemeGruppe.IstActiv = false;
            AktThemeGruppe.ThemeName = tboxThemeBezeichnung.Text != "" ? tboxThemeBezeichnung.Text : "Neue Themegruppe";
            AktThemeGruppe.HGThemePlaylist.Audio_Playlist = null;
  */      }

        private void btnKlangThemeAngehakt_Checked(object sender, RoutedEventArgs e)
        {
 /*           if (!AktThemeGruppe.IstActiv)
            {
                AktThemeGruppe.IstActiv = true;
                AktThemeGruppe.ThemeName = tboxThemeBezeichnung.Text != "" ? tboxThemeBezeichnung.Text : "Neue Themegruppe";
            }
            Audio_Playlist aThemePlaylist = new Audio_Playlist();
            aThemePlaylist.Audio_Playlist = Global.ContextAudio.PlaylistListe.Find(t => t.Audio_PlaylistGUID.Equals((sender as ToggleButton).Tag));
            AktThemeGruppe.KlangeThemePlaylist.Add(aThemePlaylist);
 */       }

        private void btnKlangThemeAngehakt_UnChecked(object sender, RoutedEventArgs e)
        {
 /*           AktThemeGruppe.KlangeThemePlaylist.Remove(
                AktThemeGruppe.KlangeThemePlaylist.Where(t => t.Audio_PlaylistGUID.Equals((sender as ToggleButton).Tag)).First());
   */     }


        private void btnThemeUebernehmen_Click(object sender, RoutedEventArgs e)
        {
            AktThemeGruppe.dbAudioTheme.Name = tboxThemeBezeichnung.Text;
            ThemeGruppe thgrp = _ThemeGruppe.Find(t => t.dbAudioTheme.Name == AktThemeGruppe.dbAudioTheme.Name);
            try
            {
                Audio_Theme theme = new Audio_Theme();
                theme.Name = tboxThemeBezeichnung.Text;
                
                for (int i = 0; i < ThemeLstBGPanel.Count; i++)
                {
                    if (ThemeLstBGPanel[i].btnAngehakt.IsChecked.Value)
                    {
                        Guid g = ((Guid)ThemeLstBGPanel[i].Tag);
                        theme.Audio_Playlist.Add(Global.ContextAudio.PlaylistListe.Find(t => t.Audio_PlaylistGUID == g));
                    }
                }
                        
                for (int i = 0; i < ThemeLstKlangPanel.Count; i++)
                {
                    if (ThemeLstKlangPanel[i].btnAngehakt.IsChecked.Value)
                    {
                        Guid g = ((Guid)ThemeLstKlangPanel[i].Tag);
                        theme.Audio_Playlist.Add(Global.ContextAudio.PlaylistListe.Find(t => t.Audio_PlaylistGUID == g));
                    }
                }
                Global.ContextAudio.Save();
        //        theme.Audio_Playlist.Add(playlist1);
          //      theme.Audio_Playlist.Add(playlist2);
            //
      /*          
                Audio_Theme aTheme = Global.ContextAudio.New<Audio_Theme>(); // new Audio_Theme(); //Global.ContextAudio.New<Audio_Theme>();
                
                Audio_Theme_Playlist aThemePlayList = new Audio_Theme_Playlist();  //= Global.ContextAudio.New<Audio_Theme_Playlist>();
                aThemePlayList.Audio_ThemeGUID = aTheme.Audio_ThemeGUID;
                aThemePlayList.Audio_Playlist = AktThemeGruppe.HGThemePlaylist.Audio_Playlist;
                aThemePlayList.Audio_PlaylistGUID = AktThemeGruppe.HGThemePlaylist.Audio_Playlist.Audio_PlaylistGUID;
                
               
                aTheme.Name = tboxThemeBezeichnung.Text;
                AktThemeGruppe.dbAudioTheme.Audio_ThemeGUID = aTheme.Audio_ThemeGUID;
                
                if (thgrp == null)
                    Global.ContextAudio.Insert<Audio_Theme>(aTheme);
                else
                    Global.ContextAudio.Update<Audio_Theme>(aTheme);
                
                if (thgrp == null)
                    Global.ContextAudio.Insert<Audio_Theme_Playlist>(aThemePlayList);
                else
                    Global.ContextAudio.Update<Audio_Theme_Playlist>(aThemePlayList);

                //zur datenbank hinzufügen
                _ThemeGruppe.Add(AktThemeGruppe);
                    
                for (int i = 0; i < AktThemeGruppe.KlangeThemePlaylist.Count; i++)
                {
                    Audio_Theme_Playlist aThemeKlangPlayList = new Audio_Theme_Playlist();  //Global.ContextAudio.New<Audio_Theme_Playlist>();
                    aThemeKlangPlayList.Audio_Playlist = AktThemeGruppe.KlangeThemePlaylist[i].Audio_Playlist;
                    aThemeKlangPlayList.Audio_ThemeGUID = aTheme.Audio_ThemeGUID;
                    aThemeKlangPlayList.Audio_PlaylistGUID = AktThemeGruppe.KlangeThemePlaylist[i].Audio_Playlist.Audio_PlaylistGUID;
                        
                    if (thgrp == null)
                        Global.ContextAudio.Insert<Audio_Theme_Playlist>(aThemeKlangPlayList);   
                    else
                        Global.ContextAudio.Update<Audio_Theme_Playlist>(aThemeKlangPlayList);   
                }  */ 
            }            
            catch
            {
                var errWin = new MsgWindow("Datenbankfehler", "AudioTheme schon vorhanden. Bitte wiederholen Sie den Vorgang und wählen einen anderen Titel");
                errWin.ShowDialog();
                errWin.Close();
            }
        }

        private void btnKlangSave_Click(object sender, RoutedEventArgs e)
        {
            int posObjGruppe = Convert.ToInt16(btnKlangSave.Tag);// GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));
            
            for (int i = 0; i < _GrpObjecte[posObjGruppe]._listZeile.Count; i++)
                UpdateAudio_Playlist_Titel(_GrpObjecte[posObjGruppe]._listZeile[i].audiotitel);
            btnKlangSave.Visibility = Visibility.Hidden;
        }

        private void btnKlangSave_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            int posObjGruppe = GetPosObjGruppe(GetObjGruppe(tcKlang.SelectedIndex));
            _GrpObjecte[posObjGruppe].WerteGeändert = btnKlangSave.Visibility == Visibility.Visible ? true : false;
        }
    }
}