﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Mod = MeisterGeister.ViewModel.Kampf.Logic.Modifikatoren;
using MeisterGeister.Logic.Extensions;
using System.Collections.Specialized;
using MeisterGeister.Model.Extensions;

namespace MeisterGeister.ViewModel.Kampf.Logic
{
    public class KämpferInfo : INotifyPropertyChanged, IDisposable
    {
        private IKämpfer _kämpfer;

        public IKämpfer Kämpfer
        {
            get { return _kämpfer; }
            private set { _kämpfer = value; AktionenBerechnen(); 
                OnChanged("Kämpfer"); 
            }
        }

        private int _initiative;
        public int Initiative
        {
            get
            {
                return _initiative;
            }
            set
            {
                _initiative = value;
                int bonus = Math.Max((int)Math.Floor((_initiative - 11) / 10.0), 0);
                FreieAktionen = 2 + bonus;
                Kämpfer.Modifikatoren.RemoveAll(m => m is Mod.PABonusDurchHoheIni);
                if (bonus > 0)
                    Kämpfer.Modifikatoren.Add(new Mod.PABonusDurchHoheIni(bonus));
                //TODO JT: Wenn INI < 0 -> Kämpfer verliert eine (Angriffs)Aktion
                OnChanged("Initiative");
            }
        }
        public int InitiativeBasis
        {
            get { return Kämpfer.InitiativeBasis; }
        }

        public int InitiativeWurf
        {
            get { return Kämpfer.InitiativeWurf; }
        }

        private int _team;
        public int Team
        {
            get { return _team; }
            set { _team = value; OnChanged("Team"); }
        }

        private Kampf kampf;
        public Kampf Kampf
        {
            get { return kampf; }
            set { kampf = value; OnChanged("Kampf"); }
        }

        public bool IsAktuell
        {
            get 
            {
                if (Kampf == null || Kampf.AktuelleAktion == null)
                    return false;
                return this == Kampf.AktuelleAktion.KämpferInfo; 
            }
        }

        public KämpferInfo(IKämpfer k, Kampf kampf)
        {
            if (k == null)
                throw new ArgumentNullException("IKämpfer k darf nicht null sein.");
            if (kampf == null)
                throw new ArgumentNullException("Kampf kampf darf nicht null sein.");
            Kämpfer = k;
            Kampf = kampf;
            Kämpfer.PropertyChanged += Kämpfer_PropertyChanged;
            Team = 1;
            Initiative = k.Initiative();
            PropertyChanged += DependentProperty.PropagateINotifyProperyChanged;
        }

        private void Kämpfer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "InitiativeBasis" || e.PropertyName == "InitiativeWurf")
            {
                Initiative = InitiativeBasis + InitiativeWurf;
                OnChanged(e.PropertyName);
            }
        }

        #region Aktionen
        private int _aktionen = 2;
        public int Aktionen
        {
            get
            {
                return _aktionen;
            }
            private set
            {
                if (_aktionen == value)
                    return;
                _aktionen = value;
                if (_aktionen < Abwehraktionen + Angriffsaktionen)
                {
                    if (Abwehraktionen + Angriffsaktionen == 0)
                        Angriffsaktionen = 0;
                    else
                        Angriffsaktionen = (int)Math.Round(Math.Min((double)Angriffsaktionen / (double)(Abwehraktionen + Angriffsaktionen), 1) * Aktionen, MidpointRounding.AwayFromZero);
                }
                //OnChanged("Aktionen");
            }
        }

        private int _freieAktionen = 2;
        public int FreieAktionen
        {
            get
            {
                return _freieAktionen;
            }
            private set
            {
                _freieAktionen = value;
                OnChanged("FreieAktionen");
            }
        }

        private int _angriffsaktionen = 1;
        public int Angriffsaktionen
        {
            get { return _angriffsaktionen; }
            set
            {
                if (value > Aktionen)
                    value = Aktionen;
                _angriffsaktionen = value;
                _abwehraktionen = Aktionen - _angriffsaktionen;
                AktionenBerechnen();
                _abwehraktionen = Aktionen - _angriffsaktionen;
                OnChanged("Abwehraktionen"); OnChanged("Angriffsaktionen"); OnChanged("Aktionen");
            }
        }

        private int _abwehraktionen = 1;
        public int Abwehraktionen
        {
            get { return _abwehraktionen; }
            set
            {
                if (value > Aktionen)
                    value = Aktionen;
                _abwehraktionen = value;
                _angriffsaktionen = Aktionen - _abwehraktionen;
                AktionenBerechnen();
                _angriffsaktionen = Aktionen - _abwehraktionen;
                OnChanged("Abwehraktionen"); OnChanged("Angriffsaktionen"); OnChanged("Aktionen");
            }
        }

        private int _verbrauchteAngriffsaktionen = 0;
        public int VerbrauchteAngriffsaktionen
        {
            get { return _verbrauchteAngriffsaktionen; }
            set { _verbrauchteAngriffsaktionen = value; }
        }

        private int _verbrauchteAbwehraktionen = 0;
        public int VerbrauchteAbwehraktionen
        {
            get { return _verbrauchteAbwehraktionen; }
            set { _verbrauchteAbwehraktionen = value; }
        }

        private int _verbrauchteFreieAktionen = 0;
        public int VerbrauchteFreieAktionen
        {
            get { return _verbrauchteFreieAktionen; }
            set { _verbrauchteFreieAktionen = value; }
        }

        private void AktionenBerechnen()
        {
            bool parierwaffenII = false;
            if (Kämpfer is Model.Gegner)
            {
                Aktionen = (Kämpfer as Model.Gegner).Aktionen;
                if (Aktionen != Abwehraktionen + Angriffsaktionen)
                {
                    if (Abwehraktionen + Angriffsaktionen == 0)
                    {
                        _angriffsaktionen = 0; _abwehraktionen = Aktionen;
                    }
                    else
                    {
                        _angriffsaktionen = (int)Math.Round(Math.Min((double)Angriffsaktionen / (double)(Abwehraktionen + Angriffsaktionen), 1) * Aktionen, MidpointRounding.AwayFromZero);
                        _abwehraktionen = Aktionen - _angriffsaktionen;
                    }
                }
            }
            //TODO JT: Sicherstellen, dass auch zwei Waffen geführt werden
            else if (Kampfstil == Kampfstil.BeidhändigerKampf && ((Kämpfer is Model.Held) && (Kämpfer as Model.Held).HatSonderfertigkeitUndVoraussetzungen("Beidhändiger Kampf II")))
            {
                Aktionen = 3;
                if (Abwehraktionen + Angriffsaktionen < 3)
                    _angriffsaktionen = Aktionen - Abwehraktionen;
            }
            else if (Kampfstil == Kampfstil.Schildkampf && ((Kämpfer is Model.Held) && (Kämpfer as Model.Held).HatSonderfertigkeitUndVoraussetzungen("Schildkampf II") && Abwehraktionen >= 1))
            {
                if (Angriffsaktionen >= 2)
                    Aktionen = 2;
                else
                    Aktionen = 3;
                if (Abwehraktionen + Angriffsaktionen < 3)
                    _abwehraktionen = Aktionen - Angriffsaktionen;
            }
            else if (Kampfstil == Kampfstil.Parierwaffenstil && ((Kämpfer is Model.Held) && (parierwaffenII = (Kämpfer as Model.Held).HatSonderfertigkeitUndVoraussetzungen("Parierwaffen II") && Abwehraktionen >= 1 || (Kämpfer as Model.Held).HatSonderfertigkeitUndVoraussetzungen("Tod von Links") && Angriffsaktionen >= 1)))
            {
                Aktionen = 3;
                if (Abwehraktionen + Angriffsaktionen < 3)
                    if (parierwaffenII)
                        _abwehraktionen = Aktionen - Angriffsaktionen;
                    else
                        _angriffsaktionen = Aktionen - Abwehraktionen;
            }
            else
            {
                Aktionen = 2;
                if (Abwehraktionen + Angriffsaktionen < 2)
                    _angriffsaktionen = _abwehraktionen = 1;
            }
            //TODO JT: Myranor: Mehrhändig hinzufügen sicherstellen, dass auch entsprechend viele Waffen geführt werden
        }

        private Kampfstil _kampfstil;
        public Kampfstil Kampfstil
        {
            get { return _kampfstil; }
            set
            {
                if (_kampfstil == value)
                    return;
                _kampfstil = value;
                AktionenBerechnen();
                OnChanged("Kampfstil");
                OnChanged("Abwehraktionen"); OnChanged("Angriffsaktionen"); OnChanged("Aktionen");
            }
        }

        private WaffenloserKampfstil _waffenloserKampfstil;
        public WaffenloserKampfstil WaffenloserKampfstil
        {
            get { return _waffenloserKampfstil; }
            set { _waffenloserKampfstil = value; }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnChanged(String info, object sender = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender ?? this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

        public void Dispose()
        {
            if (Kämpfer != null)
            {
                Kämpfer.PropertyChanged -= Kämpfer_PropertyChanged;
                Kämpfer.Modifikatoren.RemoveAll(m => m is Mod.PABonusDurchHoheIni);
            }
            PropertyChanged -= DependentProperty.PropagateINotifyProperyChanged;
        }
    }

    public class KämpferInfoListe : List<KämpferInfo>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private Dictionary<IKämpfer, KämpferInfo> _kämpfer_kämpferinfo;

        public KämpferInfoListe(Kampf kampf)
        {
            _kampf = kampf;
            _kämpfer_kämpferinfo = new Dictionary<IKämpfer, KämpferInfo>();
        }

        public KämpferInfo this[IKämpfer k]
        {
            get
            {
                return _kämpfer_kämpferinfo[k];
            }
        }

        public IEnumerable<IKämpfer> Kämpfer
        {
            get { return _kämpfer_kämpferinfo.Keys; }
        }

        private Kampf _kampf;
        public Kampf Kampf
        {
            get { return _kampf; }
            set { _kampf = value; }
        }

        #region Add and Remove
        public new void Add(KämpferInfo ki)
        {
            base.Add(ki);
            _kämpfer_kämpferinfo.Add(ki.Kämpfer, ki);
            ki.PropertyChanged += OnKämpferInfoChanged;
            Sort();
            OnCollectionChanged(NotifyCollectionChangedAction.Add, ki);
        }

        public void Add(IKämpfer k)
        {
            Add(k, 1);
        }

        public void Add(IKämpfer k, int team)
        {
            var ki = new KämpferInfo(k, Kampf) { Team = team };
            Add(ki);
            ki.Abwehraktionen = 1;
            ki.Angriffsaktionen = Math.Max(ki.Aktionen - ki.Abwehraktionen, 0);
        }

        public new void Remove(KämpferInfo ki)
        {
            ki.PropertyChanged -= OnKämpferInfoChanged;
            _kämpfer_kämpferinfo.Remove(ki.Kämpfer);
            base.Remove(ki);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, ki);
        }

        public new void RemoveAt(int index)
        {
            var removed = this[index];
            _kämpfer_kämpferinfo.Remove(this[index].Kämpfer);
            this[index].PropertyChanged -= OnKämpferInfoChanged;
            base.RemoveAt(index);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, removed);
        }

        public new void RemoveAll(Predicate<KämpferInfo> match)
        {
            foreach (KämpferInfo k in this.Where(ki => match(ki)).ToList())
                Remove(k);
        }

        public new void RemoveRange(int index, int range)
        {
            throw new NotImplementedException();
        }

        public void Remove(IKämpfer k)
        {
            Remove(this[k]);
        }

        public new void Clear()
        {
            foreach (KämpferInfo k in this.ToList())
                Remove(k);
        }
        #endregion

        private void OnKämpferInfoChanged(object o, System.ComponentModel.PropertyChangedEventArgs args)
        {
            if(args.PropertyName == "Initiative")
                Sort();
            if (args.PropertyName == "Angriffsaktionen")
                OnChanged("Angriffsaktionen", o);
        }

        public new void Sort()
        {
            base.Sort(CompareInitiative);
            OnChanged("Sort");
        }

        /// <summary>
        /// Höhere Initiative nach oben.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int CompareInitiative(KämpferInfo x, KämpferInfo y)
        {
            // prüfen auf null-Übergabe
            if (x == null && y == null) return 0;
            if (x == null) return 1;
            if (y == null) return -1;
            // Vergleich
            if (x.Initiative > y.Initiative)
                return -1;
            if (x.Initiative < y.Initiative)
                return 1;
            if (x.InitiativeBasis > y.InitiativeBasis)
                return -1;
            if (x.InitiativeBasis < y.InitiativeBasis)
                return 1;
            return x.Kämpfer.Name.CompareTo(y.Kämpfer.Name);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnChanged(String info, object sender = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender ?? this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object element)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, element));
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
