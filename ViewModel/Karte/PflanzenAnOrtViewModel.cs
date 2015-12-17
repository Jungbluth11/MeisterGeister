﻿using MeisterGeister.Logic.Einstellung;
using MeisterGeister.Logic.Kalender;
using MeisterGeister.Logic.Kalender.DsaTool;
using MeisterGeister.Model;
using MeisterGeister.Model.Extensions;
using MeisterGeister.View.Karte;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeisterGeister.ViewModel.Karte
{
    public class PflanzenAnOrtViewModel : Base.ViewModelBase
    {
        private Landschaft überall, fastÜberall;

        public PflanzenAnOrtViewModel()
        {
            PropertyChanged += DependentProperty.PropagateINotifyProperyChanged;
        }

        private double tolerance = 0.2;
        public double Tolerance
        {
            get { return tolerance; }
            set
            {
                Set(ref tolerance, value);
                LadePflanzen();
            }
        }


        private List<string> pflanzenTypen = new List<string>();
        public List<string> PflanzenTypen
        {
            get { return pflanzenTypen; }
            set { Set(ref pflanzenTypen, value); }
        }
        
        private bool? _alleLandschaftsGruppen = true;
        public bool? AlleLandschaftsGruppen
        {
            get { return _alleLandschaftsGruppen; }
            set
            {
                Set(ref _alleLandschaftsGruppen, value);

                if (value != null)
                    LandschaftsGruppen.ForEach(t => t.IsChecked = value);
            }
        }
        
        private bool _nurBekanntePflanzenZeigen = false;
        public bool NurBekanntePflanzenZeigen
        {
            get { return _nurBekanntePflanzenZeigen; }
            set
            {
                Set(ref _nurBekanntePflanzenZeigen, value);
                RegisterEvents();
            }
        }
        private List<LandschaftsGruppeViewModel> landschaftsGruppen = new List<LandschaftsGruppeViewModel>();
        public List<LandschaftsGruppeViewModel> LandschaftsGruppen
        {
            get { return landschaftsGruppen; }
            set { Set(ref landschaftsGruppen, value); }
        }


        private string pflanzenTyp = String.Empty;
        public string PflanzenTyp
        {
            get { return pflanzenTyp; }
            set { Set(ref pflanzenTyp, value); }
        }

        ObservableCollection<Pflanze> pflanzen = new ObservableCollection<Pflanze>();
        public ObservableCollection<Pflanze> Pflanzen
        {
            get { return pflanzen; }
            protected set { Set(ref pflanzen, value); }
        }

        private List<Pflanze> pflanzenInLandschaft = null;

        [DependentProperty("PflanzenTyp")]
        public List<Model.Pflanze> SichtbarePflanzen
        {
            get
            {
                if (pflanzenInLandschaft == null)
                    filterLandschaft();
                return filterTyp(pflanzenInLandschaft).OrderBy(p => p.Name).ToList();
            }
        }

        private Pflanze_Verbreitung suche = null;
        public Pflanze_Verbreitung Suche
        {
            get { return suche; }
            set { Set(ref suche, value); }
        }


        private List<Pflanze> filterTyp(IList<Pflanze> pflanzen)
        {
            if (PflanzenTyp == String.Empty)
                return pflanzen.ToList();
            else return pflanzen.Where(p => p.Pflanze_Typ.Where(pt => pt.Typ == PflanzenTyp).Count() > 0).ToList();
        }

        private void filterLandschaft()
        {
            HashSet<Landschaft> landschaften = new HashSet<Landschaft>();
            landschaften.Add(überall);
            landschaften.Add(fastÜberall);
            foreach (LandschaftsGruppeViewModel gruppe in LandschaftsGruppen)
            {
                foreach (LandschaftViewModel vm in gruppe.Landschaften.Where((l) => l.IsChecked))
                    landschaften.Add(vm.Landschaft);
            }
            pflanzenInLandschaft = Pflanzen.Where((p) => p.Landschaften.Any((l) => landschaften.Contains(l))).ToList();
        }


        private Base.CommandBase _onBtnBekanntePflanzenOpen = null;
        public Base.CommandBase OnBtnBekanntePflanzenOpen
        {
            get
            {
                if (_onBtnBekanntePflanzenOpen == null)
                    _onBtnBekanntePflanzenOpen = new Base.CommandBase(BtnBekanntePflanzenOpen, null);
                return _onBtnBekanntePflanzenOpen;
            }
        }        
        /// <summary>
        /// Button zum Bestimmen der Gebiete in der aktuellen Position
        /// </summary>
        public void BtnBekanntePflanzenOpen(object obj)
        {
            BekanntePflanzenView wndBekanntePflanzen = new BekanntePflanzenView();
            wndBekanntePflanzen.VM.PflanzenAnOrtVM = this;
            wndBekanntePflanzen.ShowDialog();   
        }

        
        public List<Held> HeldenListe
        {
            get { return Global.ContextHeld.HeldenGruppeListe; }
        }

        public Held SelectedHeld
        {
            get { return Global.SelectedHeld; }
            set
            {
                Global.SelectedHeld = value;
                OnChanged();
                RegisterEvents();
            }
        }

        private ObservableCollection<object> _monate = new ObservableCollection<object>();
        public ObservableCollection<object> monate
        {
            get { return _monate; }
            set { Set(ref _monate, value); }
        }

        private object _monatAuswahl = null;
        public object MonatAuswahl
        {
            get { return _monatAuswahl; }
            set
            {
                Set(ref _monatAuswahl, value);
                RegisterEvents();
            }
        }

        public void LadePflanzen()
        {
            if (_monatAuswahl == null)
            {
                ObservableCollection<object> monList = new ObservableCollection<object>();
                monList.Add("Komplettes Jahr");
                foreach (Monat item in Enum.GetValues(typeof(Monat)))
                    monList.Add(item);
                monate = monList;
                MonatAuswahl = monate[0];
            }
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;
            Pflanzen.Clear();
            var gebiete = Global.ContextZooBot.GetGebiete(Global.HeldenPosition, Tolerance);
            HashSet<Guid> pset = new HashSet<Guid>();
            HashSet<string> typen = new HashSet<string>();
            HashSet<Landschaft> landschaften = new HashSet<Landschaft>();
            HashSet<Landschaftsgruppe> gruppen = new HashSet<Landschaftsgruppe>();
            //Leerer Eintrag für alle Pflanzentypen
            typen.Add(String.Empty);
            foreach (var g in gebiete)
            {
                foreach (var p in g.Pflanze)
                {
                    if (pset.Contains(p.PflanzeGUID) || 
                        (MonatAuswahl as string != "Komplettes Jahr" &&
                         !p.GetInErnte(((int)MonatAuswahl) + 1)))
                        continue;

                    //Nicht bekannte Pflanzen überspringen
                    if (NurBekanntePflanzenZeigen &&
                        SelectedHeld.Held_Pflanze.Where(t => t.Bekannt).FirstOrDefault(t => t.Pflanze.PflanzeGUID == p.PflanzeGUID) == null)
                        continue;

                    pset.Add(p.PflanzeGUID);
                    Pflanzen.Add(p);
                    foreach (var pt in p.Pflanze_Typ)
                        typen.Add(pt.Typ);
                    foreach (var l in p.Landschaften)
                    {
                        if (l.Name == "überall")
                        {
                            überall = l;
                            continue;
                        }
                        else if (l.Name.StartsWith("überall "))
                        {
                            fastÜberall = l;
                            continue;
                        }
                        landschaften.Add(l);
                        foreach (var gr in l.Landschaftsgruppe)
                            gruppen.Add(gr);
                    }
                }
            }
            PflanzenTypen = typen.ToList();
            //cache leeren
            LandschaftViewModels.Clear();
            List<LandschaftsGruppeViewModel> gruppenVM = new List<LandschaftsGruppeViewModel>();
            foreach (Landschaftsgruppe gruppe in gruppen)
            {
                var inGruppe = landschaften.Where((l) => l.Landschaftsgruppe.Contains(gruppe));
                gruppenVM.Add(new LandschaftsGruppeViewModel(inGruppe, gruppe, LandschaftViewModels));
            }
            LandschaftsGruppen = gruppenVM;
            pflanzenInLandschaft = null;
            OnChanged("SichtbarePflanzen");
        }

        public override void RegisterEvents()
        {
            base.RegisterEvents();
            LadePflanzen();
            foreach (LandschaftsGruppeViewModel gruppe in LandschaftsGruppen)
            {
                gruppe.PropertyChanged += Gruppe_PropertyChanged;
            }
            Global.StandortChanged += Global_StandortChanged;
        }

        public override void UnregisterEvents()
        {
            Global.StandortChanged -= Global_StandortChanged;
            foreach (LandschaftsGruppeViewModel gruppe in LandschaftsGruppen)
            {
                gruppe.PropertyChanged -= Gruppe_PropertyChanged;
            }
            base.UnregisterEvents();
        }

        private void Gruppe_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                //Vorgefilterte Pflanzen nach Landschaft auf null setzen damit SichtbarePflanzen sie neu lädt
                pflanzenInLandschaft = null;
                OnChanged("SichtbarePflanzen");
                int i = LandschaftsGruppen.Where(t => t.IsChecked != null).Count(t => t.IsChecked.Value);
                if (LandschaftsGruppen.Count == i)
                {
                    if (AlleLandschaftsGruppen != true) AlleLandschaftsGruppen = true;
                }
                else
                {
                    if (AlleLandschaftsGruppen != null) AlleLandschaftsGruppen = null;
                }
            }
        }

        private void Global_StandortChanged(object sender, EventArgs e)
        {
            UnregisterEvents();
            RegisterEvents();
        }

        private Dictionary<Guid, LandschaftViewModel> landschaftViewModels = new Dictionary<Guid, LandschaftViewModel>();
        public Dictionary<Guid, LandschaftViewModel> LandschaftViewModels
        {
            get { return landschaftViewModels; }
            protected set { Set(ref landschaftViewModels, value); }
        }
    }

    public class LandschaftsGruppeViewModel : Base.ViewModelBase
    {
        public LandschaftsGruppeViewModel(IEnumerable<Landschaft> ls, Landschaftsgruppe gruppe, Dictionary<Guid, LandschaftViewModel> lvmCache)
        {
            Gruppe = gruppe;
            landschaften = new List<LandschaftViewModel>();
            foreach (Landschaft l in ls)
            {
                LandschaftViewModel vm = null;
                if (lvmCache.ContainsKey(l.LandschaftGUID))
                    vm = lvmCache[l.LandschaftGUID];
                else
                {
                    vm = new LandschaftViewModel(l);
                    lvmCache.Add(l.LandschaftGUID, vm);
                }
                landschaften.Add(vm);
            }
        }

        public Landschaftsgruppe Gruppe { get; private set; }


        public override void RegisterEvents()
        {
            base.RegisterEvents();
            foreach (LandschaftViewModel vm in landschaften)
            {
                vm.PropertyChanged += landschaft_PropertyChanged;
            }
        }

        public override void UnregisterEvents()
        {
            foreach (LandschaftViewModel vm in landschaften)
            {
                vm.PropertyChanged -= landschaft_PropertyChanged;
            }
            base.UnregisterEvents();
        }

        private void landschaft_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnChanged("IsChecked");
        }
        

        private List<LandschaftViewModel> landschaften;
        public List<LandschaftViewModel> Landschaften
        {
            get { return landschaften; }
            private set { Set(ref landschaften, value); }
        }


        public bool? IsChecked
        {
            get
            {
                bool foundChecked = false, foundUnchecked = false;
                foreach (LandschaftViewModel vm in landschaften)
                {
                    if (vm.IsChecked) foundChecked = true;
                    else foundUnchecked = true;
                }
                if (foundChecked && foundUnchecked) return null;
                else if (foundChecked) return true;
                else return false;
            }
            set
            {
                if (value != null)
                {
                    foreach (LandschaftViewModel vm in landschaften)
                    {
                        vm.IsChecked = value.Value;
                    }
                }
            }
        }

    }

    public class LandschaftViewModel : Base.ViewModelBase
    {
        public LandschaftViewModel(Landschaft landschaft)
        {
            Landschaft = landschaft;
        }
        public Landschaft Landschaft { get; private set; }

        private bool isChecked = true;
        public bool IsChecked
        {
            get { return isChecked; }
            set { Set(ref isChecked, value); }
        }
    }
}
