﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MeisterGeister.Model;
using MeisterGeister.Model.Extensions;
using MeisterGeister.ViewModel.Kampf.Logic;
using K = MeisterGeister.ViewModel.Kampf.Logic.Kampf;

namespace MeisterGeister.ViewModel.Kampf
{
    public class KampfViewModel : Base.ViewModelBase
    {
        private K _kampf = null;
        public KampfViewModel(Action<K> showGegnerView, Func<string, string, bool> confirm) : base(confirm, View.General.ViewHelper.ShowError)
        {
            this.showGegnerView = showGegnerView;

            _kampf = new K();
            _kampf.OnNeueKampfrunde += _kampf_OnNeueKampfRunde;
            Kampf.PropertyChanged += Kampf_PropertyChanged;
            InitiativListe.PropertyChanged += InitiativListe_PropertyChanged;
        }

        void Kampf_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "INIPhase") OnChanged("INIPhase");
        }

        void _kampf_OnNeueKampfRunde(object sender, int kampfrunde)
        {
            OnChanged("Kampfrunde");
        }

        private void InitiativListe_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnChanged("InitiativListe");
        }

        public K Kampf
        {
            get { return _kampf; }
            set { _kampf = value; OnChanged("Kampf"); }
        }

        [DependentProperty("Kampf")]
        public InitiativListe InitiativListe
        {
            get { return Kampf != null ? Kampf.InitiativListe : null; }
        }

        public int Kampfrunde
        {
            get { return Kampf != null ? Kampf.Kampfrunde : 0; }
        }
        
        public float INIPhase
        {
            get { return Kampf != null ? Kampf.Kampfrunde : 0; }
        }

        public KämpferInfoListe KämpferListe
        {
            get { return Kampf != null ? Kampf.Kämpfer : null; }
        }

        public KämpferInfo SelectedKämpferInfo
        {
            get { return (SelectedManöverInfo != null) ? SelectedManöverInfo.KämpferInfo : null; }
            //set { SelectedTreeItem = value; }
        }

        private ManöverInfo _selectedManöverInfo = null;
        public ManöverInfo SelectedManöverInfo
        {
            get { return _selectedManöverInfo; }
            set { _selectedManöverInfo = value; OnChanged("SelectedManöverInfo"); OnChanged("SelectedKämpferInfo"); }
        }
        
        private bool kämpferSelected = false;
        /// <summary>
        /// Um ManöverInfo und auch KämpferInfo unterscheiden zu können.
        /// </summary>
        public bool KämpferSelected
        {
            get { return kämpferSelected; }
            set { kämpferSelected = value; OnChanged("KämpferSelected"); }
        }

        private int schaden = 5;
        public int Schaden
        {
            get { return schaden; }
            set { schaden = value; OnChanged("Schaden"); }
        }

        private TrefferpunkteOptions wundschwellenOption = TrefferpunkteOptions.Default;
        public TrefferpunkteOptions WundschwellenOption
        {
            get { return wundschwellenOption; }
            set { wundschwellenOption = value; OnChanged("WundschwellenOption"); }
        }

        private TrefferpunkteOptions ausdauerSchadenMachtKeineSchadenspunkte = TrefferpunkteOptions.Default;
        public TrefferpunkteOptions? AusdauerSchadenMachtKeineSchadenspunkte
        {
            get { return ausdauerSchadenMachtKeineSchadenspunkte; }
            set { ausdauerSchadenMachtKeineSchadenspunkte = value ?? TrefferpunkteOptions.Default; OnChanged("AusdauerSchadenMachtKeineSchadenspunkte"); }
        }

        private Trefferzone selectedTrefferzone = Trefferzone.Unlokalisiert;
        public Trefferzone SelectedTrefferzone
        {
            get { return selectedTrefferzone; }
            set { selectedTrefferzone = value; OnChanged("SelectedTrefferzone"); }
        }


        #region // ---- COMMANDS ----

        private Base.CommandBase onAddHelden = null;
        public Base.CommandBase OnAddHelden
        {
            get
            {
                if (onAddHelden == null)
                    onAddHelden = new Base.CommandBase(AddHelden, null);
                return onAddHelden;
            }
        }

        private void AddHelden(object obj)
        {
            KämpferInfo ki = null;
            foreach (Model.Held held in Global.ContextHeld.HeldenGruppeListe)
            {
                if (!KämpferListe.Kämpfer.Contains(held))
                {
                    ki = new KämpferInfo(held);
                    KämpferListe.Add(held);
                }
            }
            var k = KämpferListe[0];
        }

        private Base.CommandBase onDeleteKämpfer = null;
        public Base.CommandBase OnDeleteKämpfer
        {
            get
            {
                if (onDeleteKämpfer == null)
                    onDeleteKämpfer = new Base.CommandBase(DeleteKämpfer, null);
                return onDeleteKämpfer;
            }
        }

        private void DeleteKämpfer(object obj)
        {
            if (SelectedKämpferInfo != null && Confirm("Kämpfer entfernen", String.Format("Soll der Kämpfer {0} entfernt werden?", SelectedKämpferInfo.Kämpfer.Name)))
                KämpferListe.Remove(SelectedKämpferInfo);
        }

        private Base.CommandBase onDeleteAllKämpfer = null;
        public Base.CommandBase OnDeleteAllKämpfer
        {
            get
            {
                if (onDeleteAllKämpfer == null)
                    onDeleteAllKämpfer = new Base.CommandBase(DeleteAllKämpfer, null);
                return onDeleteAllKämpfer;
            }
        }

        private void DeleteAllKämpfer(object obj)
        {
            if(Confirm("Liste leeren", "Sollen alle Kämpfer entfernt werden?"))
                KämpferListe.Clear();
        }

        private Base.CommandBase onShowGegnerView = null;
        public Base.CommandBase OnShowGegnerView
        {
            get
            {
                if (onShowGegnerView == null)
                    onShowGegnerView = new Base.CommandBase(ShowGegnerView, null);
                return onShowGegnerView;
            }
        }

        private void ShowGegnerView(object obj)
        {
            if (showGegnerView != null)
                showGegnerView(Kampf);
        }

        private Action<K> showGegnerView;


        private Base.CommandBase onNext = null;
        public Base.CommandBase OnNext
        {
            get
            {
                if (onNext == null)
                    onNext = new Base.CommandBase(Next, null);
                return onNext;
            }
        }

        private void Next(object obj)
        {
            var mi = Kampf.Next();
            if (mi != null)
            {
                //if(SelectedManöverInfo != null)
                    //SelectedManöverInfo.IsSelected = false;
                //mi.IsSelected = true;
                KämpferSelected = false;
                SelectedManöverInfo = mi;
            }
        }

        private Base.CommandBase onTrefferpunkte = null;
        public Base.CommandBase OnTrefferpunkte
        {
            get
            {
                if (onTrefferpunkte == null)
                    onTrefferpunkte = new Base.CommandBase(Trefferpunkte, null);
                return onTrefferpunkte;
            }
        }

        private void Trefferpunkte(object obj)
        {
            if (SelectedKämpferInfo == null)
                return;
            TrefferpunkteOptions opt = TrefferpunkteOptions.Default;
            if (obj is TrefferpunkteOptions)
                opt = (TrefferpunkteOptions)obj;
            Kampf.Trefferpunkte(SelectedKämpferInfo.Kämpfer, Schaden, SelectedTrefferzone, opt);
        }

        private Base.CommandBase onKarmaenergieAbziehen = null;
        public Base.CommandBase OnKarmaenergieAbziehen
        {
            get
            {
                if (onKarmaenergieAbziehen == null)
                    onKarmaenergieAbziehen = new Base.CommandBase(KarmaenergieAbziehen, null);
                return onKarmaenergieAbziehen;
            }
        }

        private void KarmaenergieAbziehen(object obj)
        {
            if (SelectedKämpferInfo == null)
                return;
            SelectedKämpferInfo.Kämpfer.KarmaenergieAktuell -= Math.Max(Schaden, 0);
        }

        private Base.CommandBase onAstralenergieAbziehen = null;
        public Base.CommandBase OnAstralenergieAbziehen
        {
            get
            {
                if (onAstralenergieAbziehen == null)
                    onAstralenergieAbziehen = new Base.CommandBase(AstralenergieAbziehen, null);
                return onAstralenergieAbziehen;
            }
        }

        private void AstralenergieAbziehen(object obj)
        {
            if (SelectedKämpferInfo == null)
                return;
            SelectedKämpferInfo.Kämpfer.AstralenergieAktuell -= Math.Max(Schaden, 0);
        }

        #endregion // ---- COMMANDS ----

        #region Subklassen
        public class KämpferNahkampfwaffe : Logic.INahkampfwaffe
        {
            private Held _held;
            private Waffe _waffe;
            private GegnerBase_Angriff _gegner_angriff;

            public KämpferNahkampfwaffe(Held held, Waffe waffe)
            {
                _held = held; _waffe = waffe;
            }

            public KämpferNahkampfwaffe(GegnerBase_Angriff ga)
            {
                _gegner_angriff = ga;
            }

            //public Logic.IKämpfer Kämpfer
            //{
            //    get
            //    {
            //        if (_gegner_angriff != null)
            //            return _gegner_angriff.Gegner;
            //        return _held;
            //    }
            //}

            public Logic.Distanzklasse Distanzklasse
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.Distanzklasse;
                    return _waffe.Distanzklasse;
                }
            }

            public string Name
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.Name;
                    return _waffe.Name;
                }
            }

            public int TPWürfel
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.TPWürfel;
                    return _waffe.TPWürfel;
                }
            }

            public int TPWürfelAnzahl
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.TPWürfelAnzahl;
                    return _waffe.TPWürfelAnzahl;
                }
            }

            public int TPBonus
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.TPBonus;
                    return _waffe.TPBonus;
                }
            }

            public int TPKKBonus
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.TPKKBonus;
                    return _waffe.TPKKBonus(_held);
                }
            }

            public int AT
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.AT;
                    return 0;
                }
            }
        }

        public class KämpferFernkampfwaffe : Logic.IFernkampfwaffe
        {
            private Held _held;
            private Model.Fernkampfwaffe _waffe;
            private GegnerBase_Angriff _gegner_angriff;

            public KämpferFernkampfwaffe(Held held, Model.Fernkampfwaffe waffe)
            {
                _held = held; _waffe = waffe;
            }

            public KämpferFernkampfwaffe(GegnerBase_Angriff ga)
            {
                _gegner_angriff = ga;
            }

            //public Logic.IKämpfer Kämpfer
            //{
            //    get
            //    {
            //        if (_gegner_angriff != null)
            //            return _gegner_angriff.Gegner;
            //        return _held;
            //    }
            //}

            public int? RWSehrNah
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.RWSehrNah;
                    return _waffe.RWSehrNah;
                }
            }

            public int? RWNah
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.RWNah;
                    return _waffe.RWNah;
                }
            }

            public int? RWMittel
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.RWMittel;
                    return _waffe.RWMittel;
                }
            }

            public int? RWWeit
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.RWWeit;
                    return _waffe.RWWeit;
                }
            }

            public int? RWSehrWeit
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.RWSehrWeit;
                    return _waffe.RWSehrWeit;
                }
            }

            public int? TPSehrNah
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.TPSehrNah;
                    return _waffe.TPSehrNah;
                }
            }

            public int? TPNah
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.TPNah;
                    return _waffe.TPNah;
                }
            }

            public int? TPMittel
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.TPMittel;
                    return _waffe.TPMittel;
                }
            }

            public int? TPWeit
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.TPWeit;
                    return _waffe.TPWeit;
                }
            }

            public int? TPSehrWeit
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.TPSehrWeit;
                    return _waffe.TPSehrWeit;
                }
            }

            public string Name
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.Name;
                    return _waffe.Name;
                }
            }

            public int TPWürfel
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.TPWürfel;
                    return _waffe.TPWürfel ?? 0;
                }
            }

            public int TPWürfelAnzahl
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.TPWürfelAnzahl;
                    return _waffe.TPWürfelAnzahl ?? 0;
                }
            }

            public int TPBonus
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.TPBonus;
                    return _waffe.TPBonus ?? 0;
                }
            }

            public int TPKKBonus
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.TPKKBonus;
                    return _waffe.TPKKBonus(_held);
                }
            }

            public int AT
            {
                get
                {
                    if (_gegner_angriff != null)
                        return _gegner_angriff.AT;
                    return 0;
                }
            }
        }
        #endregion

        //Command NeueKampfrunde
    }

    public class TrefferpunkteOptionsConverter : System.Windows.Data.IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TrefferpunkteOptions opt = TrefferpunkteOptions.Default;
            foreach (object o in values)
            {
                opt |= (TrefferpunkteOptions)o;
            }
            return opt;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            List<TrefferpunkteOptions> opt = new List<TrefferpunkteOptions>();
            foreach (var o in Enum.GetValues(typeof(TrefferpunkteOptions)))
            {
                if (((TrefferpunkteOptions)value & (TrefferpunkteOptions)o) == (TrefferpunkteOptions)o)
                    opt.Add((TrefferpunkteOptions)o);
            }
            if(opt.Count == 0)
                return new object[] {TrefferpunkteOptions.Default};
            return opt.Select(a => (object)a).ToArray();

        }
    }
}
