﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using MeisterGeister.View.Bodenplan;
using Mod = MeisterGeister.ViewModel.Kampf.Logic.Modifikatoren;
using MeisterGeister.Logic.Extensions;
using System.Collections.Specialized;
using System.ComponentModel;
using MeisterGeister.ViewModel.Base;
using MeisterGeister.Model.Extensions;
using MeisterGeister.ViewModel.Kampf.Logic.Manöver;
using System.Windows.Data;
using System.Globalization;

namespace MeisterGeister.ViewModel.Kampf.Logic
{
    public class Kampf : ViewModelBase, IDisposable
    {
        /*
         * Die Klasse hält und kontrolliert alle globalen Kampfinformationen und -Aktionen
         * neue KR(), nächster Kämpfer(), ...
         * aktueller Kämpfer, aktuelle KR, aktuelle INI ...
         * 
         * Liste der angesagten Aktionen pro KR, Liste der ausgeführten Aktionen pro KR
         * 
         * Eine Liste der Kämpfer
         * Zur Unterstützung einer beliebigen Anzahl an Parteien gehört zu jedem Teilnehmer eine Teamnummer
         */
        public Kampf()
        {
            PropertyChanged += DependentProperty.PropagateINotifyProperyChanged;
            Kämpfer = new KämpferInfoListe(this);
            Kämpfer.CollectionChangedExtended += Kämpfer_CollectionChangedExtended;
            InitiativListe = new InitiativListe(this);
            KampfNeuStarten();
        }

        private ObservableCollection<string> _kampfLog = new ObservableCollection<string>();
        public ObservableCollection<string> KampfLog
        {
            get { return _kampfLog; }
        }

        private InitiativListe _initiativListe;
        public InitiativListe InitiativListe
        {
            get { return _initiativListe; }
            private set { _initiativListe = value; }
        }

        private KämpferInfoListe _kämpfer;
        public KämpferInfoListe Kämpfer
        {
            get { return _kämpfer; }
            private set { _kämpfer = value; }
        }

        private int _kampfrunde;
        public int Kampfrunde
        {
            get { return _kampfrunde; }
            private set
            {
                _kampfrunde = value;
                Kampfzeit = new TimeSpan(0, 0, 3 * Math.Max(value - 1, 0));
                OnChanged("Kampfrunde");
            }
        }

        private TimeSpan _kampfzeit = new TimeSpan();
        public TimeSpan Kampfzeit
        {
            get { return _kampfzeit; }
            private set
            {
                Set(ref _kampfzeit, value);
            }
        }

        private ZeitImKampf aktuelleAktionszeit;
        public ZeitImKampf AktuelleAktionszeit
        {
            get { return aktuelleAktionszeit; }
            private set
            {
                Set(ref aktuelleAktionszeit, value);
            }
        }

        [DependentProperty("AktuelleAktionszeit")]
        private IEnumerable<ManöverInfo> AktuelleAktionen
        {
            get { return InitiativListe.Where(mi => mi.Aktionszeiten.Contains(AktuelleAktionszeit)); }
        }

        private Lichtstufe licht = Lichtstufe.Tageslicht;
        public Lichtstufe Licht
        {
            get { return licht; }
            set
            {
                Set(ref licht, value);
                foreach (ManöverInfo mi in InitiativListe)
                {
                    KampfManöver<IWaffe> manöver = mi.Manöver as KampfManöver<IWaffe>;
                    if (manöver != null)
                        ((ManöverModifikator<Lichtstufe, IWaffe>)manöver.Mods[KampfManöver<IWaffe>.LICHT_MOD]).Value = value;
                }
            }
        }


        /// <summary>
        /// Speichert einen Text im Kampf-Log.
        /// </summary>
        /// <param name="msg">Zu speichernder Log-Text.</param>
        public void Log(string msg)
        {
            KampfLog.Insert(0, string.Format("{0}.{1}: {2}", Kampfrunde, AktuelleAktionszeit.InitiativPhase, msg));
        }

        public void Next()
        {
            foreach (ManöverInfo mi in AktuelleAktionen)
            {
                if (!mi.Ausgeführt)
                    mi.Manöver.Aktion();
            }

            ZeitImKampf next = default(ZeitImKampf);

            if (AktuelleAktionszeit != default(ZeitImKampf))
            {
                var nextActions = InitiativListe.Aktionszeiten.Where(zeit => zeit > AktuelleAktionszeit && zeit.Kampfrunde == Kampfrunde).OrderBy(zeit => zeit);
                next = nextActions.FirstOrDefault();
            }
            else
                next = InitiativListe.Aktionszeiten.Where(zeit => zeit.Kampfrunde == Kampfrunde).FirstOrDefault();

            AktuelleAktionszeit = next;
            if (next == default(ZeitImKampf))
                NeueKampfrunde();
        }

        public void NeueKampfrunde()
        {
            ////Alle Manöver, die noch nicht ausgeführt wurden
            //var nichtAusgeführt = InitiativListe.Where(mi => mi.Ausgeführt == false);
            ////TODO JT: entweder warnen
            //foreach (var mi in nichtAusgeführt)
            //{
            //    //oder einfach als ausgeführt setzen
            //    if (mi.Manöver != null)
            //    {
            //        var probe = mi.Manöver.Ausführen();
            //        //Siehe kommentar in next() ... das design ist mist.
            //        //Wenn die probe ignoriert wird passiert auch nichts weiter, ausser dass die Verbleibenden Aktionen sinken.
            //    }
            //    else
            //        mi.Ausgeführt = true;
            //}
            //Alte Ansagen löschen
            //InitiativListe.LöscheBeendeteManöver();

            Kampfrunde++;

            //Wir speichern unsere neuen Manöver zuerst hier zwischen und fügen dann den kompletten Satz an neuen Aktionen auf einmach zur IniListe hinzu
            //Das verhindert dass bei jedem Manöver ein separates Change-Event ausgelöst wird, welches in der GUI Performance kostet
            List<ManöverInfo> neueManöver = new List<ManöverInfo>();

            foreach (KämpferInfo ki in Kämpfer)
            {
                //Modifikatoren entfernen
                ki.Kämpfer.Modifikatoren.RemoveAll(m => m is Mod.IEndetMitKampfrunde);

                //sollte in KämpferInfo rein
                ki.VerbrauchteAbwehraktionen = 0;
                ki.VerbrauchteAngriffsaktionen = 0;
                ki.VerbrauchteFreieAktionen = 0;

                neueManöver.AddRange(ki.StandardAktionenSetzen(Kampfrunde));

                ki.VerbrauchteAbwehraktionen = 0;
                ki.VerbrauchteAngriffsaktionen = 0;
                ki.VerbrauchteFreieAktionen = 0;
                //Im UI sollten kämpfer ohne Ansage leicht an der Farbe erkennbar sein
                //Kämpfer mit Aufmerksamkeit oder Kampfgespür müssen nicht markiert werden (höchstens mit einer leichten tönung)
            }

            if (neueManöver.Count > 0)
                InitiativListe.AddRange(neueManöver);

            //if (InitiativListe.Count > 0)
            //    INIPhase = InitiativListe[0].InitiativeStart;
            //else
            //    INIPhase = 0;
            //im UI markieren, dass man nun bis zur ersten Aktions-Ansage umwandeln kann
            //UmwandelnMöglich = true;
            //eventuell in die Property?

            Log(string.Format("Kampfrunde {0} gestartet", Kampfrunde));
        }

        public void KampfNeuStarten()
        {
            KampfEnde();
            AktuelleAktionszeit = default(ZeitImKampf);
            InitiativListe.Clear();
            Kampfrunde = 0;

            // INI neu ermitteln
            foreach (KämpferInfo kämpferInfo in Kämpfer)
            {
                if (kämpferInfo != null)
                    kämpferInfo.Initiative = kämpferInfo.Kämpfer.Initiative();
            }

            // KR abschließen
            NeueKampfrunde();
        }

        private void KampfEnde()
        {
            foreach (var ki in Kämpfer)
                ki.Kämpfer.Modifikatoren.RemoveAll(m => m is Mod.IEndetMitKampf);
        }

        public void Orientieren(IKämpfer k)
        {
            Orientieren(Kämpfer[k]);
        }

        public void Orientieren(KämpferInfo ki)
        {
            ki.Initiative = ki.Kämpfer.InitiativeMax();
            ki.Kämpfer.Modifikatoren.RemoveAll(m => m is Mod.IEndetMitAktion);
        }

        public void Kämpfer_CollectionChangedExtended(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                InitiativListe.RemoveAll(mi => args.OldItems.Contains(mi.Manöver.Ausführender));
            }
            else if (args.Action == NotifyCollectionChangedAction.Add)
            {
                InitiativListe.AddRange(args.NewItems.Cast<KämpferInfo>().SelectMany(ki => ki.StandardAktionenSetzen(Kampfrunde)));
            }
            //InitiativListe.Add((KämpferInfo)args.NewItems[0], new Manöver.KeineAktion(((KämpferInfo)args.NewItems[0]).Kämpfer), 0);
        }

        public void Dispose()
        {
            //TODO ??: ich finde diese Lösung noch nicht optimal. Das geht schief, wenn man speichern und laden möchte.
            //Alle Gegenerinstanzen löschen
            foreach (var k in Kämpfer.Where(ki => ki.Kämpfer is Model.Gegner).Select(ki => ki.Kämpfer))
            {
                Kämpfer.Remove(k);
            }

            Kämpfer.CollectionChanged -= Kämpfer_CollectionChangedExtended;

            //Alles aus der Ini-Liste löschen damit die Events abgemeldet werden
            while (InitiativListe.Count > 0)
                InitiativListe.RemoveAt(0);
        }

    }

    public class DoubleLichtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)(Lichtstufe)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Lichtstufe)(int)Math.Round((double)value);
        }
    }

    [Flags]
    public enum TrefferpunkteOptions
    {
        Default = 0x00,
        Ausdauerschaden = 0x01,
        IgnoriertRüstung = 0x02,
        VerringerteWundschwelle = 0x04,
        KeineWunden = 0x08,
        AusdauerschadenMachtKeineEchtenSchadenspunkte = 0x10
    }
}
