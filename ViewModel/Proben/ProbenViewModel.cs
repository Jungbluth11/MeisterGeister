﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using MeisterGeister.Logic.General;
using MeisterGeister.Logic.Einstellung;
using MeisterGeister.Model.Extensions;
using MeisterGeister.ViewModel.Helden.Logic;

namespace MeisterGeister.ViewModel.Proben
{
    public class ProbenViewModel : Base.ToolViewModelBase
    {
        #region //---- COMMANDS ----

        private Base.CommandBase onAddFreieProbe;
        public Base.CommandBase OnAddFreieProbe
        {
            get 
            {
                if (onAddFreieProbe == null)
                    onAddFreieProbe = new Base.CommandBase(AddFreieProbe, null);
                return onAddFreieProbe;
            }
        }
        private void AddFreieProbe(object obj)
        {
            AddProbeItem(null);
            OnChanged("ProbeErgebnisListe");
        }

        private Base.CommandBase onWürfeln;
        public Base.CommandBase OnWürfeln
        {
            get { return onWürfeln; }
        }

        private Base.CommandBase onOpenWiki = null;
        public Base.CommandBase OnOpenWiki
        {
            get
            {
                if (onOpenWiki == null)
                    onOpenWiki = new Base.CommandBase(OpenWiki, null);
                return onOpenWiki;
            }
        }
        private void OpenWiki(object sender)
        {
            if (SelectedProbe != null)
                WikiAventurica.OpenBrowser(SelectedProbe);
        }

        private Base.CommandBase onAddFavorit = null;
        public Base.CommandBase OnAddFavorit
        {
            get
            {
                if (onAddFavorit == null)
                    onAddFavorit = new Base.CommandBase(AddFavorit, null);
                return onAddFavorit;
            }
        }
        private void AddFavorit(object sender)
        {
            if (SelectedProbe != null && !_probeFavoritenListe.Contains(SelectedProbe))
            {
                _probeFavoritenListe.Add(SelectedProbe);
                SaveProbeFavoriten();
                OnChanged("ProbeFavoritenListe");
            }
        }

        private Base.CommandBase onWürfelFavorit = null;
        public Base.CommandBase OnWürfelFavorit
        {
            get
            {
                if (onWürfelFavorit == null)
                    onWürfelFavorit = new Base.CommandBase(WürfelFavorit, null);
                return onWürfelFavorit;
            }
        }
        private void WürfelFavorit(object sender)
        {
            if (sender is Probe)
            {
                SelectedFilterItem = FilterListe.FirstOrDefault();
                SelectedProbe = sender as Probe;
                Würfeln(null);
            }
        }

        private Base.CommandBase onDeleteFavorit = null;
        public Base.CommandBase OnDeleteFavorit
        {
            get
            {
                if (onDeleteFavorit == null)
                    onDeleteFavorit = new Base.CommandBase(DeleteFavorit, null);
                return onDeleteFavorit;
            }
        }
        private Base.CommandBase onDeleteFavoritAll = null;
        public Base.CommandBase OnDeleteFavoritAll
        {
            get
            {
                if (onDeleteFavoritAll == null)
                    onDeleteFavoritAll = new Base.CommandBase(DeleteFavorit, null);
                return onDeleteFavoritAll;
            }
        }
        /// <summary>
        /// Löscht Probe-Favoriten.
        /// </summary>
        /// <param name="sender">Den zu löschenden Probe-Favorit oder 'null' für alle.</param>
        private void DeleteFavorit(object sender)
        {
            if (sender == null)
            {
                if(Confirm("Probe-Favoriten", "Sollen wirklich alle Proben-Favoriten gelöscht werden?"))
                    _probeFavoritenListe.Clear();
            }
            else if (sender is Probe)
                _probeFavoritenListe.Remove(sender as Probe);
            SaveProbeFavoriten();
            OnChanged("ProbeFavoritenListe");
        }

        private Base.CommandBase _onShowSpielerInfo = null;
        public Base.CommandBase OnShowSpielerInfo
        {
            get
            {
                if (_onShowSpielerInfo == null)
                    _onShowSpielerInfo = new Base.CommandBase(ShowSpielerInfo, null);
                return _onShowSpielerInfo;
            }
        }
        private void ShowSpielerInfo(object sender)
        {
            // TODO MT: MVVM konform umbauen
            View.Proben.ProbenSpielerInfoView infoView = new View.Proben.ProbenSpielerInfoView();
            infoView.VM = this;
            View.SpielerScreen.SpielerWindow.SetContent(infoView);
        }

        private Base.CommandBase _onCloseSpielerInfo = null;
        public Base.CommandBase OnCloseSpielerInfo
        {
            get
            {
                if (_onCloseSpielerInfo == null)
                    _onCloseSpielerInfo = new Base.CommandBase(CloseSpielerInfo, null);
                return _onCloseSpielerInfo;
            }
        }
        private void CloseSpielerInfo(object sender)
        {
            // TODO MT: MVVM konform umbauen
            View.SpielerScreen.SpielerWindow.Hide();
        }

        #endregion

        #region //---- EIGENSCHAFTEN & FELDER ----

        [DependentProperty("FilteredProbeListe")]
        public List<Model.Held> HeldListe
        {
            get { return Global.ContextHeld.HeldenGruppeListe; }
        }

        private List<ProbeControlViewModel> _probeErgebnisListe = new List<ProbeControlViewModel>();
        public List<ProbeControlViewModel> ProbeErgebnisListe
        {
            get 
            {
                if (SelectedSortierung != null)
                {
                    switch (SelectedSortierung.Name)
                    {
                        case "Übrig":
                            return _probeErgebnisListe.OrderBy(vm => vm.NichtProben).
                                ThenByDescending(vm => vm.Ergebnis.Übrig).
                                ThenByDescending(vm => vm.Ergebnis.Qualität).
                                ThenByDescending(vm => (vm.Probe != null ? vm.Probe.Fertigkeitswert : 0)).ToList();
                        case "Wert":
                            return _probeErgebnisListe.OrderBy(vm => vm.NichtProben).
                                ThenByDescending(vm => (vm.Probe != null ? vm.Probe.Fertigkeitswert : 0)).
                                ThenByDescending(vm => (vm.Probe != null ? vm.Probe.Werte.Sum() : 0)).
                                ThenBy(vm => (vm.Held != null ? vm.Held.Name : string.Empty)).ToList();
                        case "Name":
                            return _probeErgebnisListe.OrderBy(vm => (vm.Held != null ? vm.Held.Name : string.Empty)).
                                ThenByDescending(vm => (vm.Probe != null ? vm.Probe.Fertigkeitswert : 0)).ToList();
                        default:
                            break;
                    }
                }
                return _probeErgebnisListe;
            }
            set
            {
                _probeErgebnisListe = value;
                OnChanged("ProbeErgebnisListe");
            }
        }

        private Probe _selectedProbe = null;
        public Probe SelectedProbe
        {
            get { return _selectedProbe; }
            set 
            { 
                _selectedProbe = value;
                RefreshProbeErgebnisListe();
                OnChanged("SelectedProbe");
                OnChanged("SelectedProbeIsZauber");
                OnChanged("SelectedProbeHat_eBE");
            }
        }

        [DependentProperty("SelectedProbe")]
        public bool SelectedProbeHat_eBE
        {
            get { return _selectedProbe is Model.Talent; }
        }

        private bool _isBehinderung = true;
        /// <summary>
        /// Behinderung bei Probe berücksichtigen oder nicht.
        /// </summary>
        public bool IsBehinderung
        {
            get { return _isBehinderung; }
            set
            {
                _isBehinderung = value;
                // Wert an ProbeControls weiterreichen
                foreach (ProbeControlViewModel er in ProbeErgebnisListe)
                {
                    if (er.Probe != null)
                        er.Probe.IsBehinderung = _isBehinderung;
                }
                OnChanged("IsBehinderung");
                OnChanged("ProbeErgebnisListe");
            }
        }

        [DependentProperty("SelectedProbe")]
        public bool SelectedProbeIsZauber
        {
            get { return _selectedProbe is Model.Zauber; }
        }

        private bool _isAktivierteProben = true;
        public bool IsAktivierteProben
        {
            get { return _isAktivierteProben; }
            set
            {
                _isAktivierteProben = value;
                FilterProbeListe();
                OnChanged("IsAktivierteProben");
            }
        }

        private FilterItem _selectedFilterItem = null;
        public FilterItem SelectedFilterItem
        {
            get { return _selectedFilterItem; }
            set 
            { 
                _selectedFilterItem = value;
                FilterProbeListe();
                OnChanged("SelectedFilterItem");
            }
        }

        private AnzeigeModusSortierungItem _selectedSortierung;
        public AnzeigeModusSortierungItem SelectedSortierung
        {
            get { return _selectedSortierung; }
            set
            {
                _selectedSortierung = value;
                OnChanged("SelectedSortierung");
                OnChanged("ProbeErgebnisListe");
            }
        }

        private AnzeigeModusSortierungItem _selectedAnzeigeModus;
        public AnzeigeModusSortierungItem SelectedAnzeigeModus
        {
            get { return _selectedAnzeigeModus; }
            set
            {
                _selectedAnzeigeModus = value;
                if (value != null)
                {
                    Einstellungen.ProbenAnzeigeModus = value.Name;
                    SelectedAnzeigeOrientation = (value.Name == "Zeile") ? Orientation.Horizontal : Orientation.Vertical;
                    foreach (var item in ProbeErgebnisListe)
                        item.Orientation = SelectedAnzeigeOrientation;

                    ProbeErgebnisListePanel = CreatePanelTemplate(SelectedAnzeigeOrientation);
                }
                OnChanged("SelectedAnzeigeModus");
            }
        }

        private Orientation SelectedAnzeigeOrientation { get; set; }

        // Mod-Stufen: -7, -3, +/-0, 3, 7, 12, 18, 25
        public int SelectedMod
        {
            get 
            {
                if (Modifikator <= -7)
                    return -7;
                else if (Modifikator <= -3)
                    return -3;
                else if (Modifikator <= 0 && Modifikator < 3)
                    return 0;
                else if (Modifikator >= 25)
                    return 25;
                else if (Modifikator >= 18)
                    return 18;
                else if (Modifikator >= 12)
                    return 12;
                else if (Modifikator >= 7)
                    return 7;
                else if (Modifikator >= 3)
                    return 3;
                return 0;
            }
            set
            {
                Modifikator = value;
                OnChanged("SelectedMod");
            }
        }

        private ItemsPanelTemplate _probeErgebnisListePanel = null;
        public ItemsPanelTemplate ProbeErgebnisListePanel
        {
            get 
            { 
                if (_probeErgebnisListePanel == null)
                    _probeErgebnisListePanel = CreatePanelTemplate(Orientation.Horizontal);
                return _probeErgebnisListePanel; 
            }
            set
            {
                _probeErgebnisListePanel = value;
                OnChanged("ProbeErgebnisListePanel");
            }
        }

        private ItemsPanelTemplate CreatePanelTemplate(Orientation or)
        {
            if (or == Orientation.Horizontal)
            { // ListBox
                System.Windows.FrameworkElementFactory factory =
                    new System.Windows.FrameworkElementFactory(typeof(VirtualizingStackPanel));
                factory.SetValue(VirtualizingStackPanel.OrientationProperty, Orientation.Vertical);
                return new ItemsPanelTemplate(factory);
            }
            else
            { // WrapPanel
                System.Windows.FrameworkElementFactory factory =
                    new System.Windows.FrameworkElementFactory(typeof(WrapPanel));
                return new ItemsPanelTemplate(factory);
            }
        }

        int _modifikator = 0;
        public int Modifikator
        {
            get { return _modifikator; }
            set
            {
                _modifikator = value;

                // Modifikator an ProbeControls weiterreichen
                foreach (ProbeControlViewModel er in ProbeErgebnisListe)
                    er.Modifikator = _modifikator;
                OnChanged("Modifikator");
                OnChanged("SelectedMod");
                OnChanged("ProbeErgebnisListe");
            }
        }

        public string GruppenErgebnis
        {
            get
            {
                int tapSum = 0, vorSum = 0, i = 1;
                string art = SelectedProbe == null ? "Punkte*" : SelectedProbe.PunkteText + "*";
                foreach (ProbeControlViewModel er in ProbeErgebnisListe)
                {
                    if (er.NichtProben)
                        break;
                    if (er.Ergebnis.Übrig >= 0) //nur positive Ergebnisse addieren
                    {
                        tapSum += er.Ergebnis.Übrig;
                        vorSum += Convert.ToInt32(Math.Round((double)er.Ergebnis.Übrig / i, 0, MidpointRounding.AwayFromZero));
                        i++;
                    }
                }

                return string.Format("Unabhängige Zusammenarbeit: {0} {1}\nMit fähigstem Held als Vorarbeiter: {2} {1}", tapSum, art, vorSum);
            }
        }

        public bool SoundAbspielen
        {
            get { return Einstellungen.WuerfelSoundAbspielen; }
            set { Einstellungen.WuerfelSoundAbspielen = value; OnChanged("SoundAbspielen"); }
        }

        // Listen
        List<Probe> _probeListe = new List<Probe>();
        public List<Probe> ProbeListe
        {
            get { return _probeListe; }
            set { _probeListe = value; FilterProbeListe(); OnChanged("ProbeListe"); }
        }

        List<Probe> _filteredProbeListe = new List<Probe>();
        public List<Probe> FilteredProbeListe
        {
            get { return _filteredProbeListe; }
            set { _filteredProbeListe = value; OnChanged("FilteredProbeListe"); }
        }

        List<FilterItem> _filterListe = new List<FilterItem>();
        public List<FilterItem> FilterListe
        {
            get { return _filterListe; }
            set { _filterListe = value; FilterProbeListe(); OnChanged("FilterListe"); }
        }

        List<AnzeigeModusSortierungItem> _anzeigeModusListe = new List<AnzeigeModusSortierungItem>() { new AnzeigeModusSortierungItem("Zeile"), new AnzeigeModusSortierungItem("Kachel") };
        public List<AnzeigeModusSortierungItem> AnzeigeModusListe
        {
            get { return _anzeigeModusListe; }
            set { _anzeigeModusListe = value; OnChanged("AnzeigeModusListe"); }
        }

        List<AnzeigeModusSortierungItem> _sortierungListe = new List<AnzeigeModusSortierungItem>() { new AnzeigeModusSortierungItem("Übrig"), new AnzeigeModusSortierungItem("Wert"),
            new AnzeigeModusSortierungItem("Name")};
        public List<AnzeigeModusSortierungItem> SortierungListe
        {
            get { return _sortierungListe; }
            set { _sortierungListe = value; OnChanged("SortierungListe"); }
        }

        private ObservableCollection<string> _spezielleErfahrungListe = new ObservableCollection<string>();
        public ObservableCollection<string> SpezielleErfahrungListe 
        {
            get { return _spezielleErfahrungListe; }
            set { _spezielleErfahrungListe = value; OnChanged("SpezielleErfahrungListe"); }
        }

        private List<Probe> _probeFavoritenListe = new List<Probe>();
        public List<Probe> ProbeFavoritenListe
        {
            get { return _probeFavoritenListe.OrderBy(item => item.Probenname).ToList(); }
            set { _probeFavoritenListe = value; OnChanged("ProbeFavoritenListe"); }
        }

        #endregion

        #region //---- KONSTRUKTOR ----

        public ProbenViewModel() : this(MeisterGeister.View.General.ViewHelper.Confirm, MeisterGeister.View.General.ViewHelper.ShowError) { }

        public ProbenViewModel(Func<string, string, bool> confirm, Action<string, Exception> showError)
            : base(confirm, showError)
        {
            PropertyChanged += DependentProperty.PropagateINotifyProperyChanged;

            onWürfeln = new Base.CommandBase(Würfeln, null);
            Einstellungen.WuerfelSoundAbspielenChanged += WuerfelSoundAbspielenChanged;
            Global.GruppenProbeWürfeln += Global_GruppenProbeWürfeln;
            Probe.SpezielleErfahrung += Probe_SpezielleErfahrung;

            SelectedAnzeigeModus = AnzeigeModusListe.Where(i => i.Name == Einstellungen.ProbenAnzeigeModus).FirstOrDefault();
            _selectedSortierung = SortierungListe.FirstOrDefault();

            InitFilterListe();
            InitProbeListe();
            Refresh();
            RefreshProbeErgebnisListe();
        }

        #endregion

        #region //---- INSTANZMETHODEN ----

        private void InitFilterListe()
        {
            // Filter-Liste
            _filterListe.Add(new FilterItem("Alle"));
            _filterListe.Add(new FilterItem("Häufig verwendet"));
            _filterListe.Add(new FilterItem("Eigenschaft"));

            _filterListe.Add(new FilterItem("Talent"));
            // Talentgruppen
            _filterListe.Add(new FilterItem("Kampf"));
            _filterListe.Add(new FilterItem("Körper"));
            _filterListe.Add(new FilterItem("Gesellschaft"));
            _filterListe.Add(new FilterItem("Natur"));
            _filterListe.Add(new FilterItem("Wissen"));
            _filterListe.Add(new FilterItem("Handwerk"));
            _filterListe.Add(new FilterItem("Sprachen/Schriften"));
            _filterListe.Add(new FilterItem("Gabe"));
            _filterListe.Add(new FilterItem("Ritualkenntnis"));
            _filterListe.Add(new FilterItem("Liturgiekenntnis"));
            _filterListe.Add(new FilterItem("Meta"));
            _filterListe.Add(new FilterItem("Basis"));
            _filterListe.Add(new FilterItem("Spezial"));

            _filterListe.Add(new FilterItem("Zauber"));

            _selectedFilterItem = FilterListe.FirstOrDefault();
        }

        private void InitProbeListe()
        {
            ProbeListe.Clear();
            // Eigenschaften hinzufügen
            ProbeListe.AddRange(Eigenschaft.EigenschaftenListe);
            // Talente hinzufügen
            ProbeListe.AddRange(Global.ContextTalent.TalentListe);
            // Zauber hinzufügen
            ProbeListe.AddRange(Global.ContextZauber.ZauberListe);

            // Probe-Favoriten
            LoadProbeFavoriten();
        }

        private void LoadProbeFavoriten()
        {
            _probeFavoritenListe.Clear();
            if (!string.IsNullOrEmpty(Einstellungen.ProbenFavoriten))
            {
                string[] favoriten = Einstellungen.ProbenFavoriten.Split('#');
                foreach (string name in favoriten)
                {
                    if (name != string.Empty)
                    {
                        Probe p = ProbeListe.Where(item => item.Probenname == name).FirstOrDefault();
                        if (p != null)
                            _probeFavoritenListe.Add(p);
                    }
                }
                OnChanged("ProbeFavoritenListe");
            }
        }

        private void SaveProbeFavoriten()
        {
            string favoriten = string.Empty;

            foreach (Probe probe in _probeFavoritenListe)
            {
                if (favoriten != string.Empty)
                    favoriten += "#";
                favoriten += probe.Probenname;
            }

            Einstellungen.ProbenFavoriten = favoriten;
        }

        private void FilterProbeListe()
        {
            List<Probe> probeListe = ProbeListe;
            if (IsAktivierteProben)
            { // nur aktivierte Proben
                probeListe = ProbeListe.Where(p => p is Eigenschaft)
                    .Concat(Global.ContextHeld.Liste<Model.Held_Talent>().Where(ht => ht.Held == null ? false : ht.Held.AktiveHeldengruppe == true).Select(ht => (ht.Talent as Probe)).Distinct())
                    .Concat(Global.ContextHeld.Liste<Model.Held_Zauber>().Where(hz => hz.Held == null ? false : hz.Held.AktiveHeldengruppe == true).Select(hz => (hz.Zauber as Probe)).Distinct())
                    .ToList();

                var liMeta = Global.ContextHeld.Liste<Model.Talent>().Where(p => p is Model.Talent && (p as Model.Talent).IsMetaTalent);
                foreach (var talent in liMeta)
                {
                    foreach (var held in HeldListe)
                    {
                        MetaTalent mt = new MetaTalent(talent, held);
                        if (mt.Aktiviert)
                        {
                            probeListe.Add(talent);
                            break;
                        }
                    }
                }
            }

            if (SelectedFilterItem == null)
            {
                FilteredProbeListe = probeListe.OrderBy(p => p.Probenname).ToList();
                return;
            }

            switch (SelectedFilterItem.Name)
            {
                case"Alle":
                    FilteredProbeListe = probeListe.OrderBy(p => p.Probenname).ToList();
                    return;
                case "Häufig verwendet":
                    List<string> häufigList = new List<string> { "Fährtensuchen", "Gassenwissen", "Gefahreninstinkt", "Menschenkenntnis", 
                        "Orientierung", "Schleichen", "Sich Verstecken", "Sinnenschärfe", "Überreden", "Wildnisleben" };
                    FilteredProbeListe = probeListe.Where(p => p is Model.Talent
                        && häufigList.Contains(p.Probenname)).OrderBy(p => p.Probenname).ToList();
                    return;
                case "Eigenschaft":
                    FilteredProbeListe = probeListe.Where(p => p is Eigenschaft).OrderBy(p => p.Probenname).ToList();
                    return;
                case "Talent":
                    FilteredProbeListe = probeListe.Where(p => p is Model.Talent).OrderBy(p => p.Probenname).ToList();
                    return;
                case "Basis":
                case "Spezial":
                    FilteredProbeListe = probeListe.Where(p => p is Model.Talent
                        && (p as Model.Talent).Talenttyp == SelectedFilterItem.Name).OrderBy(p => p.Probenname).ToList();
                    return;
                case "Zauber":
                    FilteredProbeListe = probeListe.Where(p => p is Model.Zauber).OrderBy(p => p.Probenname).ToList();
                    return;
                default:
                    FilteredProbeListe = probeListe.Where(p => p is Model.Talent 
                        && (p as Model.Talent).Talentgruppe.Kurzname == SelectedFilterItem.Name).OrderBy(p => p.Probenname).ToList();
                    return;
            }
        }

        public void Refresh()
        {
            OnChanged("HeldListe");
            FilterProbeListe();
        }

        private void RefreshProbeErgebnisListe()
        {
            _probeErgebnisListe.Clear();
            foreach (var item in HeldListe)
                AddProbeItem(item);
            
            OnChanged("ProbeErgebnisListe");
        }

        private void AddProbeItem(Model.Held held)
        {
            ProbeControlViewModel vm = new ProbeControlViewModel();
            vm.Orientation = SelectedAnzeigeOrientation;
            vm.Held = held;
            vm.Modifikator = Modifikator;
            vm.Gewürfelt += ProbeControlGewürfelt;

            if (held == null)
            {
                vm.Name = "NSC";
            }

            if (SelectedProbe is Model.Talent)
            {
                if ((SelectedProbe as Model.Talent).IsMetaTalent) // Meta-Talent
                {
                    MetaTalent mt = new MetaTalent((SelectedProbe as Model.Talent), held);
                    vm.Probe = mt.Aktiviert ? mt : null;
                }
                else
                {
                    if (held != null)
                        vm.Probe = held.Held_Talent.Where(t => t.Talent == SelectedProbe).FirstOrDefault();
                    else //Wenn kein Held, dann die Eigenschaften mit 10 vorbesetzen
                    {
                        Model.Talent t = new Model.Talent();
                        t.eBE = (SelectedProbe as Model.Talent).eBE;
                        t.Eigenschaft1 = (SelectedProbe as Model.Talent).Eigenschaft1;
                        t.Eigenschaft2 = (SelectedProbe as Model.Talent).Eigenschaft2;
                        t.Eigenschaft3 = (SelectedProbe as Model.Talent).Eigenschaft3;
                        t.TalentGUID = (SelectedProbe as Model.Talent).TalentGUID;
                        t.Talentname = (SelectedProbe as Model.Talent).Talentname;
                        vm.Probe = t;
                        foreach (var item in vm.EigenschaftWurfItemListe)
                            item.Wert = 10;
                        vm.Probe.Fertigkeitswert = 5;
                    }
                }

                if (vm.Probe != null)
                    _probeErgebnisListe.Add(vm);
            }
            else if (SelectedProbe is Eigenschaft)
            {
                if (held != null)
                    vm.Probe = held.Eigenschaft(SelectedProbe.Probenname);
                else //Wenn kein Held, dann die Eigenschaften mit 10 vorbesetzen
                {
                    vm.Probe = new Eigenschaft(SelectedProbe.Probenname, 10);
                }
                if (vm.Probe != null)
                    _probeErgebnisListe.Add(vm);
            }
            else if (SelectedProbe is Model.Zauber)
            {
                if (held != null)
                {
                    var zauberList = held.Held_Zauber.Where(z => z.Zauber == SelectedProbe).OrderByDescending(z => z.ZfW).ToList();
                    for (int i = 0; i < zauberList.Count(); i++)
                    {
                        vm = new ProbeControlViewModel();
                        vm.Orientation = SelectedAnzeigeOrientation;
                        vm.Held = held;
                        vm.Modifikator = Modifikator;
                        vm.Gewürfelt += ProbeControlGewürfelt;
                        vm.Probe = zauberList[i];
                        vm.NichtProben = i > 0;

                        if (vm.Probe != null)
                            _probeErgebnisListe.Add(vm);
                    }
                }
                else //Wenn kein Held, dann die Eigenschaften mit 10 vorbesetzen
                {
                    Model.Zauber z = new Model.Zauber();
                    z.Eigenschaft1 = (SelectedProbe as Model.Zauber).Eigenschaft1;
                    z.Eigenschaft2 = (SelectedProbe as Model.Zauber).Eigenschaft2;
                    z.Eigenschaft3 = (SelectedProbe as Model.Zauber).Eigenschaft3;
                    z.ZauberGUID = (SelectedProbe as Model.Zauber).ZauberGUID;
                    z.Name = (SelectedProbe as Model.Zauber).Name;
                    vm.Probe = z;
                    foreach (var item in vm.EigenschaftWurfItemListe)
                        item.Wert = 10;
                    vm.Probe.Fertigkeitswert = 5;

                    if (vm.Probe != null)
                        _probeErgebnisListe.Add(vm);
                }
                
            }
        }

        private void Würfeln(object obj)
        {
            // Alle Proben neu würfeln
            foreach (var item in ProbeErgebnisListe)
            {
                if (item.NichtProben)
                    break;
                item.LockSoundAbspielen = true;
                item.Würfeln();
                item.LockSoundAbspielen = false;
            }

            // Sound abspielen
            if (Einstellungen.WuerfelSoundAbspielen)
                MeisterGeister.Logic.General.AudioPlayer.PlayWürfel();

            OnChanged("ProbeErgebnisListe");
            OnChanged("GruppenErgebnis");
        }

        #endregion

        #region //---- EVENTS ----

        private void WuerfelSoundAbspielenChanged(object sender, EventArgs e)
        {
            OnChanged("SoundAbspielen");
        }

        private void ProbeControlGewürfelt(object sender, EventArgs e)
        {
            OnChanged("GruppenErgebnis");
        }

        private void Global_GruppenProbeWürfeln(Probe probe, EventArgs e)
        {
            SelectedFilterItem = FilterListe.FirstOrDefault();
            if (probe is Model.Held_Talent)
                SelectedProbe = (probe as Model.Held_Talent).Talent;
            else if (probe is Model.Held_Zauber)
                SelectedProbe = (probe as Model.Held_Zauber).Zauber;
            else
                SelectedProbe = probe;
            Würfeln(null);
        }

        private void Probe_SpezielleErfahrung(object sender, EventArgs e)
        {
            if (sender != null)
                SpezielleErfahrungListe.Insert(0, sender.ToString());
        }

        #endregion
    }

    #region //---- SUBKLASSEN ----

    public class AnzeigeModusSortierungItem
    {
        public AnzeigeModusSortierungItem(string name)
        {
            Name = name;
            SetImagePath();
        }
        public string Name { get; set; }

        private string _imagePath = string.Empty;
        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; }
        }

        private void SetImagePath()
        {
            switch (Name)
            {
                case "Kachel":
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/General/kachel.png";
                    break;
                case "Zeile":
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/General/zeile.png";
                    break;
                default:
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/General/sort.png";
                    break;
            }
        }
    }

    public class FilterItem
    {
        public FilterItem(string name)
        {
            Name = name;
            SetImagePath();
        }
        public string Name { get; set; }

        private string _imagePath = string.Empty;
        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; }
        }

        private void SetImagePath()
        {
            switch (Name)
            {
                case "Alle":
                    // TODO ??: Passenderes Icon
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/General/filter.png";
                    break;
                case "Häufig verwendet":
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/General/neu.png";
                    break;
                case "Eigenschaft":
                    // TODO ??: Passenderes Icon
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/Wuerfel/w20.png";
                    break;
                case "Talent":
                    // TODO ??: Passenderes Icon
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/helden.png";
                    break;
                case "Kampf":
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/nahkampf_01.png";
                    break;
                case "Körper":
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/ueberanstrengung.png";
                    break;
                case "Gesellschaft":
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/helden_kopieren.png";
                    break;
                case "Natur":
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/kraeutersuche.png";
                    break;
                case "Wissen":
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/foliant.png";
                    break;
                case "Handwerk":
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/schmiede.png";
                    break;
                case "Sprachen/Schriften":
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/sprache.png";
                    break;
                case "Gabe":
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/hesinde.png";
                    break;
                case "Ritualkenntnis":
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/zauberzeichen.png";
                    break;
                case "Liturgiekenntnis":
                    // TODO ??: Passenderes Icon
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/audio2.png";
                    break;
                case "Meta":
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/jagd.png";
                    break;
                case "Basis":
                    // TODO ??: Passenderes Icon
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/General/kreise.png";
                    break;
                case "Spezial":
                    // TODO ??: Passenderes Icon
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/General/info.png";
                    break;
                case "Zauber":
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/magie.png";
                    break;
                default:
                    // TODO ??: Passenderes Icon
                    _imagePath = "/DSA MeisterGeister;component/Images/Icons/General/question.png";
                    break;
            }
        }
    }

    public class ProbeErgebnisItem : Base.ViewModelBase
    {
        private Model.Held _held = null;
        public Model.Held Held 
        { 
            get { return _held; } 
            set { _held = value; OnChanged("Held"); } 
        }

        public Probe Probe { get; set; }

        private ProbeControlViewModel _vm = null;
        public ProbeControlViewModel VM
        {
            get { return _vm; }
            set { _vm = value; OnChanged("VM"); }
        }
    }

    #endregion
}
