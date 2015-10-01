﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.EntityClient;

namespace MeisterGeister.Model
{
    public partial class DatabaseDSAEntities : ObjectContext
    {
        public const string ConnectionString = "name=DatabaseDSAEntities";
        public const string ContainerName = "DatabaseDSAEntities";
    
        #region Constructors
    
        public DatabaseDSAEntities()
            : base(ConnectionString, ContainerName)
        {
            this.ContextOptions.LazyLoadingEnabled = true;
    		this.ObjectMaterialized += OnObjectMaterialized;
        }
    
        public DatabaseDSAEntities(string connectionString)
            : base(connectionString, ContainerName)
        {
            this.ContextOptions.LazyLoadingEnabled = true;
    		this.ObjectMaterialized += OnObjectMaterialized;
        }
    
        public DatabaseDSAEntities(string connectionString, bool noproxy, bool nolazyloading)
            : base(connectionString, ContainerName)
        {
    		if(noproxy)
                this.ContextOptions.ProxyCreationEnabled = false;
            if(nolazyloading)
    			this.ContextOptions.LazyLoadingEnabled = false;
    		else
    		{
            this.ContextOptions.LazyLoadingEnabled = true;
    		this.ObjectMaterialized += OnObjectMaterialized;
    		}
        }
    
        public DatabaseDSAEntities(EntityConnection connection)
            : base(connection, ContainerName)
        {
            this.ContextOptions.LazyLoadingEnabled = true;
    		this.ObjectMaterialized += OnObjectMaterialized;
        }
    
        #endregion
    
    	#region Events
    	private void OnObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            if(e.Entity != null && e.Entity is Extensions.IInitializable)
                ((Extensions.IInitializable)e.Entity).Initialize();
        }
    	#endregion
    
        #region ObjectSet Properties
    
        public ObjectSet<Abenteuer> Abenteuer
        {
            get { return _abenteuer  ?? (_abenteuer = CreateObjectSet<Abenteuer>("Abenteuer")); }
        }
        private ObjectSet<Abenteuer> _abenteuer;
    
        public ObjectSet<Abenteuer_Ereignis> Abenteuer_Ereignis
        {
            get { return _abenteuer_Ereignis  ?? (_abenteuer_Ereignis = CreateObjectSet<Abenteuer_Ereignis>("Abenteuer_Ereignis")); }
        }
        private ObjectSet<Abenteuer_Ereignis> _abenteuer_Ereignis;
    
        public ObjectSet<Abenteuer_Szene> Abenteuer_Szene
        {
            get { return _abenteuer_Szene  ?? (_abenteuer_Szene = CreateObjectSet<Abenteuer_Szene>("Abenteuer_Szene")); }
        }
        private ObjectSet<Abenteuer_Szene> _abenteuer_Szene;
    
        public ObjectSet<Abenteuer_Verweis> Abenteuer_Verweis
        {
            get { return _abenteuer_Verweis  ?? (_abenteuer_Verweis = CreateObjectSet<Abenteuer_Verweis>("Abenteuer_Verweis")); }
        }
        private ObjectSet<Abenteuer_Verweis> _abenteuer_Verweis;
    
        public ObjectSet<Alchimierezept> Alchimierezept
        {
            get { return _alchimierezept  ?? (_alchimierezept = CreateObjectSet<Alchimierezept>("Alchimierezept")); }
        }
        private ObjectSet<Alchimierezept> _alchimierezept;
    
        public ObjectSet<Alchimierezept_Setting> Alchimierezept_Setting
        {
            get { return _alchimierezept_Setting  ?? (_alchimierezept_Setting = CreateObjectSet<Alchimierezept_Setting>("Alchimierezept_Setting")); }
        }
        private ObjectSet<Alchimierezept_Setting> _alchimierezept_Setting;
    
        public ObjectSet<Audio_HotkeyWesen> Audio_HotkeyWesen
        {
            get { return _audio_HotkeyWesen  ?? (_audio_HotkeyWesen = CreateObjectSet<Audio_HotkeyWesen>("Audio_HotkeyWesen")); }
        }
        private ObjectSet<Audio_HotkeyWesen> _audio_HotkeyWesen;
    
        public ObjectSet<Audio_Playlist> Audio_Playlist
        {
            get { return _audio_Playlist  ?? (_audio_Playlist = CreateObjectSet<Audio_Playlist>("Audio_Playlist")); }
        }
        private ObjectSet<Audio_Playlist> _audio_Playlist;
    
        public ObjectSet<Audio_Playlist_Titel> Audio_Playlist_Titel
        {
            get { return _audio_Playlist_Titel  ?? (_audio_Playlist_Titel = CreateObjectSet<Audio_Playlist_Titel>("Audio_Playlist_Titel")); }
        }
        private ObjectSet<Audio_Playlist_Titel> _audio_Playlist_Titel;
    
        public ObjectSet<Audio_Theme> Audio_Theme
        {
            get { return _audio_Theme  ?? (_audio_Theme = CreateObjectSet<Audio_Theme>("Audio_Theme")); }
        }
        private ObjectSet<Audio_Theme> _audio_Theme;
    
        public ObjectSet<Audio_Titel> Audio_Titel
        {
            get { return _audio_Titel  ?? (_audio_Titel = CreateObjectSet<Audio_Titel>("Audio_Titel")); }
        }
        private ObjectSet<Audio_Titel> _audio_Titel;
    
        public ObjectSet<Ausrüstung> Ausrüstung
        {
            get { return _ausrüstung  ?? (_ausrüstung = CreateObjectSet<Ausrüstung>("Ausrüstung")); }
        }
        private ObjectSet<Ausrüstung> _ausrüstung;
    
        public ObjectSet<Ausrüstung_Setting> Ausrüstung_Setting
        {
            get { return _ausrüstung_Setting  ?? (_ausrüstung_Setting = CreateObjectSet<Ausrüstung_Setting>("Ausrüstung_Setting")); }
        }
        private ObjectSet<Ausrüstung_Setting> _ausrüstung_Setting;
    
        public ObjectSet<Beschwörbares> Beschwörbares
        {
            get { return _beschwörbares  ?? (_beschwörbares = CreateObjectSet<Beschwörbares>("Beschwörbares")); }
        }
        private ObjectSet<Beschwörbares> _beschwörbares;
    
        public ObjectSet<Dämon> Dämon
        {
            get { return _dämon  ?? (_dämon = CreateObjectSet<Dämon>("Dämon")); }
        }
        private ObjectSet<Dämon> _dämon;
    
        public ObjectSet<Dämon_Domäne> Dämon_Domäne
        {
            get { return _dämon_Domäne  ?? (_dämon_Domäne = CreateObjectSet<Dämon_Domäne>("Dämon_Domäne")); }
        }
        private ObjectSet<Dämon_Domäne> _dämon_Domäne;
    
        public ObjectSet<Einstellung> Einstellung
        {
            get { return _einstellung  ?? (_einstellung = CreateObjectSet<Einstellung>("Einstellung")); }
        }
        private ObjectSet<Einstellung> _einstellung;
    
        public ObjectSet<Elementarwesen> Elementarwesen
        {
            get { return _elementarwesen  ?? (_elementarwesen = CreateObjectSet<Elementarwesen>("Elementarwesen")); }
        }
        private ObjectSet<Elementarwesen> _elementarwesen;
    
        public ObjectSet<Farbe> Farbe
        {
            get { return _farbe  ?? (_farbe = CreateObjectSet<Farbe>("Farbe")); }
        }
        private ObjectSet<Farbe> _farbe;
    
        public ObjectSet<Fernkampfwaffe> Fernkampfwaffe
        {
            get { return _fernkampfwaffe  ?? (_fernkampfwaffe = CreateObjectSet<Fernkampfwaffe>("Fernkampfwaffe")); }
        }
        private ObjectSet<Fernkampfwaffe> _fernkampfwaffe;
    
        public ObjectSet<Gebiet> Gebiet
        {
            get { return _gebiet  ?? (_gebiet = CreateObjectSet<Gebiet>("Gebiet")); }
        }
        private ObjectSet<Gebiet> _gebiet;
    
        public ObjectSet<Gegner> Gegner
        {
            get { return _gegner  ?? (_gegner = CreateObjectSet<Gegner>("Gegner")); }
        }
        private ObjectSet<Gegner> _gegner;
    
        public ObjectSet<Gegner_Modifikator> Gegner_Modifikator
        {
            get { return _gegner_Modifikator  ?? (_gegner_Modifikator = CreateObjectSet<Gegner_Modifikator>("Gegner_Modifikator")); }
        }
        private ObjectSet<Gegner_Modifikator> _gegner_Modifikator;
    
        public ObjectSet<GegnerBase> GegnerBase
        {
            get { return _gegnerBase  ?? (_gegnerBase = CreateObjectSet<GegnerBase>("GegnerBase")); }
        }
        private ObjectSet<GegnerBase> _gegnerBase;
    
        public ObjectSet<GegnerBase_Angriff> GegnerBase_Angriff
        {
            get { return _gegnerBase_Angriff  ?? (_gegnerBase_Angriff = CreateObjectSet<GegnerBase_Angriff>("GegnerBase_Angriff")); }
        }
        private ObjectSet<GegnerBase_Angriff> _gegnerBase_Angriff;
    
        public ObjectSet<GegnerBase_Kampfregel> GegnerBase_Kampfregel
        {
            get { return _gegnerBase_Kampfregel  ?? (_gegnerBase_Kampfregel = CreateObjectSet<GegnerBase_Kampfregel>("GegnerBase_Kampfregel")); }
        }
        private ObjectSet<GegnerBase_Kampfregel> _gegnerBase_Kampfregel;
    
        public ObjectSet<Handelsgut> Handelsgut
        {
            get { return _handelsgut  ?? (_handelsgut = CreateObjectSet<Handelsgut>("Handelsgut")); }
        }
        private ObjectSet<Handelsgut> _handelsgut;
    
        public ObjectSet<Handelsgut_Setting> Handelsgut_Setting
        {
            get { return _handelsgut_Setting  ?? (_handelsgut_Setting = CreateObjectSet<Handelsgut_Setting>("Handelsgut_Setting")); }
        }
        private ObjectSet<Handelsgut_Setting> _handelsgut_Setting;
    
        public ObjectSet<Held> Held
        {
            get { return _held  ?? (_held = CreateObjectSet<Held>("Held")); }
        }
        private ObjectSet<Held> _held;
    
        public ObjectSet<Held_Ausrüstung> Held_Ausrüstung
        {
            get { return _held_Ausrüstung  ?? (_held_Ausrüstung = CreateObjectSet<Held_Ausrüstung>("Held_Ausrüstung")); }
        }
        private ObjectSet<Held_Ausrüstung> _held_Ausrüstung;
    
        public ObjectSet<Held_Inventar> Held_Inventar
        {
            get { return _held_Inventar  ?? (_held_Inventar = CreateObjectSet<Held_Inventar>("Held_Inventar")); }
        }
        private ObjectSet<Held_Inventar> _held_Inventar;
    
        public ObjectSet<Held_Modifikator> Held_Modifikator
        {
            get { return _held_Modifikator  ?? (_held_Modifikator = CreateObjectSet<Held_Modifikator>("Held_Modifikator")); }
        }
        private ObjectSet<Held_Modifikator> _held_Modifikator;
    
        public ObjectSet<Held_Munition> Held_Munition
        {
            get { return _held_Munition  ?? (_held_Munition = CreateObjectSet<Held_Munition>("Held_Munition")); }
        }
        private ObjectSet<Held_Munition> _held_Munition;
    
        public ObjectSet<Held_Pflanze> Held_Pflanze
        {
            get { return _held_Pflanze  ?? (_held_Pflanze = CreateObjectSet<Held_Pflanze>("Held_Pflanze")); }
        }
        private ObjectSet<Held_Pflanze> _held_Pflanze;
    
        public ObjectSet<Held_Sonderfertigkeit> Held_Sonderfertigkeit
        {
            get { return _held_Sonderfertigkeit  ?? (_held_Sonderfertigkeit = CreateObjectSet<Held_Sonderfertigkeit>("Held_Sonderfertigkeit")); }
        }
        private ObjectSet<Held_Sonderfertigkeit> _held_Sonderfertigkeit;
    
        public ObjectSet<Held_Talent> Held_Talent
        {
            get { return _held_Talent  ?? (_held_Talent = CreateObjectSet<Held_Talent>("Held_Talent")); }
        }
        private ObjectSet<Held_Talent> _held_Talent;
    
        public ObjectSet<Held_VorNachteil> Held_VorNachteil
        {
            get { return _held_VorNachteil  ?? (_held_VorNachteil = CreateObjectSet<Held_VorNachteil>("Held_VorNachteil")); }
        }
        private ObjectSet<Held_VorNachteil> _held_VorNachteil;
    
        public ObjectSet<Held_Zauber> Held_Zauber
        {
            get { return _held_Zauber  ?? (_held_Zauber = CreateObjectSet<Held_Zauber>("Held_Zauber")); }
        }
        private ObjectSet<Held_Zauber> _held_Zauber;
    
        public ObjectSet<Inventar> Inventar
        {
            get { return _inventar  ?? (_inventar = CreateObjectSet<Inventar>("Inventar")); }
        }
        private ObjectSet<Inventar> _inventar;
    
        public ObjectSet<Kampfregel> Kampfregel
        {
            get { return _kampfregel  ?? (_kampfregel = CreateObjectSet<Kampfregel>("Kampfregel")); }
        }
        private ObjectSet<Kampfregel> _kampfregel;
    
        public ObjectSet<Kreatur> Kreatur
        {
            get { return _kreatur  ?? (_kreatur = CreateObjectSet<Kreatur>("Kreatur")); }
        }
        private ObjectSet<Kreatur> _kreatur;
    
        public ObjectSet<Kultur> Kultur
        {
            get { return _kultur  ?? (_kultur = CreateObjectSet<Kultur>("Kultur")); }
        }
        private ObjectSet<Kultur> _kultur;
    
        public ObjectSet<Kultur_Name> Kultur_Name
        {
            get { return _kultur_Name  ?? (_kultur_Name = CreateObjectSet<Kultur_Name>("Kultur_Name")); }
        }
        private ObjectSet<Kultur_Name> _kultur_Name;
    
        public ObjectSet<Landschaft> Landschaft
        {
            get { return _landschaft  ?? (_landschaft = CreateObjectSet<Landschaft>("Landschaft")); }
        }
        private ObjectSet<Landschaft> _landschaft;
    
        public ObjectSet<Literatur> Literatur
        {
            get { return _literatur  ?? (_literatur = CreateObjectSet<Literatur>("Literatur")); }
        }
        private ObjectSet<Literatur> _literatur;
    
        public ObjectSet<MenuLink> MenuLink
        {
            get { return _menuLink  ?? (_menuLink = CreateObjectSet<MenuLink>("MenuLink")); }
        }
        private ObjectSet<MenuLink> _menuLink;
    
        public ObjectSet<Modifikator> Modifikator
        {
            get { return _modifikator  ?? (_modifikator = CreateObjectSet<Modifikator>("Modifikator")); }
        }
        private ObjectSet<Modifikator> _modifikator;
    
        public ObjectSet<Munition> Munition
        {
            get { return _munition  ?? (_munition = CreateObjectSet<Munition>("Munition")); }
        }
        private ObjectSet<Munition> _munition;
    
        public ObjectSet<Name> Name
        {
            get { return _name  ?? (_name = CreateObjectSet<Name>("Name")); }
        }
        private ObjectSet<Name> _name;
    
        public ObjectSet<Notizen> Notizen
        {
            get { return _notizen  ?? (_notizen = CreateObjectSet<Notizen>("Notizen")); }
        }
        private ObjectSet<Notizen> _notizen;
    
        public ObjectSet<NscMerkmal> NscMerkmal
        {
            get { return _nscMerkmal  ?? (_nscMerkmal = CreateObjectSet<NscMerkmal>("NscMerkmal")); }
        }
        private ObjectSet<NscMerkmal> _nscMerkmal;
    
        public ObjectSet<Pflanze> Pflanze
        {
            get { return _pflanze  ?? (_pflanze = CreateObjectSet<Pflanze>("Pflanze")); }
        }
        private ObjectSet<Pflanze> _pflanze;
    
        public ObjectSet<Pflanze_Ernte> Pflanze_Ernte
        {
            get { return _pflanze_Ernte  ?? (_pflanze_Ernte = CreateObjectSet<Pflanze_Ernte>("Pflanze_Ernte")); }
        }
        private ObjectSet<Pflanze_Ernte> _pflanze_Ernte;
    
        public ObjectSet<Pflanze_Verbreitung> Pflanze_Verbreitung
        {
            get { return _pflanze_Verbreitung  ?? (_pflanze_Verbreitung = CreateObjectSet<Pflanze_Verbreitung>("Pflanze_Verbreitung")); }
        }
        private ObjectSet<Pflanze_Verbreitung> _pflanze_Verbreitung;
    
        public ObjectSet<Polygon> Polygon
        {
            get { return _polygon  ?? (_polygon = CreateObjectSet<Polygon>("Polygon")); }
        }
        private ObjectSet<Polygon> _polygon;
    
        public ObjectSet<Rasse> Rasse
        {
            get { return _rasse  ?? (_rasse = CreateObjectSet<Rasse>("Rasse")); }
        }
        private ObjectSet<Rasse> _rasse;
    
        public ObjectSet<Rasse_Farbe> Rasse_Farbe
        {
            get { return _rasse_Farbe  ?? (_rasse_Farbe = CreateObjectSet<Rasse_Farbe>("Rasse_Farbe")); }
        }
        private ObjectSet<Rasse_Farbe> _rasse_Farbe;
    
        public ObjectSet<Rasse_Kultur> Rasse_Kultur
        {
            get { return _rasse_Kultur  ?? (_rasse_Kultur = CreateObjectSet<Rasse_Kultur>("Rasse_Kultur")); }
        }
        private ObjectSet<Rasse_Kultur> _rasse_Kultur;
    
        public ObjectSet<Rüstung> Rüstung
        {
            get { return _rüstung  ?? (_rüstung = CreateObjectSet<Rüstung>("Rüstung")); }
        }
        private ObjectSet<Rüstung> _rüstung;
    
        public ObjectSet<Schild> Schild
        {
            get { return _schild  ?? (_schild = CreateObjectSet<Schild>("Schild")); }
        }
        private ObjectSet<Schild> _schild;
    
        public ObjectSet<Setting> Setting
        {
            get { return _setting  ?? (_setting = CreateObjectSet<Setting>("Setting")); }
        }
        private ObjectSet<Setting> _setting;
    
        public ObjectSet<Sonderfertigkeit> Sonderfertigkeit
        {
            get { return _sonderfertigkeit  ?? (_sonderfertigkeit = CreateObjectSet<Sonderfertigkeit>("Sonderfertigkeit")); }
        }
        private ObjectSet<Sonderfertigkeit> _sonderfertigkeit;
    
        public ObjectSet<Sonderfertigkeit_Setting> Sonderfertigkeit_Setting
        {
            get { return _sonderfertigkeit_Setting  ?? (_sonderfertigkeit_Setting = CreateObjectSet<Sonderfertigkeit_Setting>("Sonderfertigkeit_Setting")); }
        }
        private ObjectSet<Sonderfertigkeit_Setting> _sonderfertigkeit_Setting;
    
        public ObjectSet<Talent> Talent
        {
            get { return _talent  ?? (_talent = CreateObjectSet<Talent>("Talent")); }
        }
        private ObjectSet<Talent> _talent;
    
        public ObjectSet<Talentgruppe> Talentgruppe
        {
            get { return _talentgruppe  ?? (_talentgruppe = CreateObjectSet<Talentgruppe>("Talentgruppe")); }
        }
        private ObjectSet<Talentgruppe> _talentgruppe;
    
        public ObjectSet<Trageort> Trageort
        {
            get { return _trageort  ?? (_trageort = CreateObjectSet<Trageort>("Trageort")); }
        }
        private ObjectSet<Trageort> _trageort;
    
        public ObjectSet<Version> Version
        {
            get { return _version  ?? (_version = CreateObjectSet<Version>("Version")); }
        }
        private ObjectSet<Version> _version;
    
        public ObjectSet<VorNachteil> VorNachteil
        {
            get { return _vorNachteil  ?? (_vorNachteil = CreateObjectSet<VorNachteil>("VorNachteil")); }
        }
        private ObjectSet<VorNachteil> _vorNachteil;
    
        public ObjectSet<Waffe> Waffe
        {
            get { return _waffe  ?? (_waffe = CreateObjectSet<Waffe>("Waffe")); }
        }
        private ObjectSet<Waffe> _waffe;
    
        public ObjectSet<Zauber> Zauber
        {
            get { return _zauber  ?? (_zauber = CreateObjectSet<Zauber>("Zauber")); }
        }
        private ObjectSet<Zauber> _zauber;
    
        public ObjectSet<Zauber_Setting> Zauber_Setting
        {
            get { return _zauber_Setting  ?? (_zauber_Setting = CreateObjectSet<Zauber_Setting>("Zauber_Setting")); }
        }
        private ObjectSet<Zauber_Setting> _zauber_Setting;
    
        public ObjectSet<Zauber_Variante> Zauber_Variante
        {
            get { return _zauber_Variante  ?? (_zauber_Variante = CreateObjectSet<Zauber_Variante>("Zauber_Variante")); }
        }
        private ObjectSet<Zauber_Variante> _zauber_Variante;
    
        public ObjectSet<Zauberzeichen> Zauberzeichen
        {
            get { return _zauberzeichen  ?? (_zauberzeichen = CreateObjectSet<Zauberzeichen>("Zauberzeichen")); }
        }
        private ObjectSet<Zauberzeichen> _zauberzeichen;
    
        public ObjectSet<Zauberzeichen_Setting> Zauberzeichen_Setting
        {
            get { return _zauberzeichen_Setting  ?? (_zauberzeichen_Setting = CreateObjectSet<Zauberzeichen_Setting>("Zauberzeichen_Setting")); }
        }
        private ObjectSet<Zauberzeichen_Setting> _zauberzeichen_Setting;

        #endregion

        #region ObjectSet Getter
    	public ObjectSet<T> GetObjectSet<T>() where T : class
    	{
    		if(typeof(T) == typeof(Abenteuer))
    				return (ObjectSet<T>)(Object)Abenteuer;
    		if(typeof(T) == typeof(Abenteuer_Ereignis))
    				return (ObjectSet<T>)(Object)Abenteuer_Ereignis;
    		if(typeof(T) == typeof(Abenteuer_Szene))
    				return (ObjectSet<T>)(Object)Abenteuer_Szene;
    		if(typeof(T) == typeof(Abenteuer_Verweis))
    				return (ObjectSet<T>)(Object)Abenteuer_Verweis;
    		if(typeof(T) == typeof(Alchimierezept))
    				return (ObjectSet<T>)(Object)Alchimierezept;
    		if(typeof(T) == typeof(Alchimierezept_Setting))
    				return (ObjectSet<T>)(Object)Alchimierezept_Setting;
    		if(typeof(T) == typeof(Audio_HotkeyWesen))
    				return (ObjectSet<T>)(Object)Audio_HotkeyWesen;
    		if(typeof(T) == typeof(Audio_Playlist))
    				return (ObjectSet<T>)(Object)Audio_Playlist;
    		if(typeof(T) == typeof(Audio_Playlist_Titel))
    				return (ObjectSet<T>)(Object)Audio_Playlist_Titel;
    		if(typeof(T) == typeof(Audio_Theme))
    				return (ObjectSet<T>)(Object)Audio_Theme;
    		if(typeof(T) == typeof(Audio_Titel))
    				return (ObjectSet<T>)(Object)Audio_Titel;
    		if(typeof(T) == typeof(Ausrüstung))
    				return (ObjectSet<T>)(Object)Ausrüstung;
    		if(typeof(T) == typeof(Ausrüstung_Setting))
    				return (ObjectSet<T>)(Object)Ausrüstung_Setting;
    		if(typeof(T) == typeof(Beschwörbares))
    				return (ObjectSet<T>)(Object)Beschwörbares;
    		if(typeof(T) == typeof(Dämon))
    				return (ObjectSet<T>)(Object)Dämon;
    		if(typeof(T) == typeof(Dämon_Domäne))
    				return (ObjectSet<T>)(Object)Dämon_Domäne;
    		if(typeof(T) == typeof(Einstellung))
    				return (ObjectSet<T>)(Object)Einstellung;
    		if(typeof(T) == typeof(Elementarwesen))
    				return (ObjectSet<T>)(Object)Elementarwesen;
    		if(typeof(T) == typeof(Farbe))
    				return (ObjectSet<T>)(Object)Farbe;
    		if(typeof(T) == typeof(Fernkampfwaffe))
    				return (ObjectSet<T>)(Object)Fernkampfwaffe;
    		if(typeof(T) == typeof(Gebiet))
    				return (ObjectSet<T>)(Object)Gebiet;
    		if(typeof(T) == typeof(Gegner))
    				return (ObjectSet<T>)(Object)Gegner;
    		if(typeof(T) == typeof(Gegner_Modifikator))
    				return (ObjectSet<T>)(Object)Gegner_Modifikator;
    		if(typeof(T) == typeof(GegnerBase))
    				return (ObjectSet<T>)(Object)GegnerBase;
    		if(typeof(T) == typeof(GegnerBase_Angriff))
    				return (ObjectSet<T>)(Object)GegnerBase_Angriff;
    		if(typeof(T) == typeof(GegnerBase_Kampfregel))
    				return (ObjectSet<T>)(Object)GegnerBase_Kampfregel;
    		if(typeof(T) == typeof(Handelsgut))
    				return (ObjectSet<T>)(Object)Handelsgut;
    		if(typeof(T) == typeof(Handelsgut_Setting))
    				return (ObjectSet<T>)(Object)Handelsgut_Setting;
    		if(typeof(T) == typeof(Held))
    				return (ObjectSet<T>)(Object)Held;
    		if(typeof(T) == typeof(Held_Ausrüstung))
    				return (ObjectSet<T>)(Object)Held_Ausrüstung;
    		if(typeof(T) == typeof(Held_Inventar))
    				return (ObjectSet<T>)(Object)Held_Inventar;
    		if(typeof(T) == typeof(Held_Modifikator))
    				return (ObjectSet<T>)(Object)Held_Modifikator;
    		if(typeof(T) == typeof(Held_Munition))
    				return (ObjectSet<T>)(Object)Held_Munition;
    		if(typeof(T) == typeof(Held_Pflanze))
    				return (ObjectSet<T>)(Object)Held_Pflanze;
    		if(typeof(T) == typeof(Held_Sonderfertigkeit))
    				return (ObjectSet<T>)(Object)Held_Sonderfertigkeit;
    		if(typeof(T) == typeof(Held_Talent))
    				return (ObjectSet<T>)(Object)Held_Talent;
    		if(typeof(T) == typeof(Held_VorNachteil))
    				return (ObjectSet<T>)(Object)Held_VorNachteil;
    		if(typeof(T) == typeof(Held_Zauber))
    				return (ObjectSet<T>)(Object)Held_Zauber;
    		if(typeof(T) == typeof(Inventar))
    				return (ObjectSet<T>)(Object)Inventar;
    		if(typeof(T) == typeof(Kampfregel))
    				return (ObjectSet<T>)(Object)Kampfregel;
    		if(typeof(T) == typeof(Kreatur))
    				return (ObjectSet<T>)(Object)Kreatur;
    		if(typeof(T) == typeof(Kultur))
    				return (ObjectSet<T>)(Object)Kultur;
    		if(typeof(T) == typeof(Kultur_Name))
    				return (ObjectSet<T>)(Object)Kultur_Name;
    		if(typeof(T) == typeof(Landschaft))
    				return (ObjectSet<T>)(Object)Landschaft;
    		if(typeof(T) == typeof(Literatur))
    				return (ObjectSet<T>)(Object)Literatur;
    		if(typeof(T) == typeof(MenuLink))
    				return (ObjectSet<T>)(Object)MenuLink;
    		if(typeof(T) == typeof(Modifikator))
    				return (ObjectSet<T>)(Object)Modifikator;
    		if(typeof(T) == typeof(Munition))
    				return (ObjectSet<T>)(Object)Munition;
    		if(typeof(T) == typeof(Name))
    				return (ObjectSet<T>)(Object)Name;
    		if(typeof(T) == typeof(Notizen))
    				return (ObjectSet<T>)(Object)Notizen;
    		if(typeof(T) == typeof(NscMerkmal))
    				return (ObjectSet<T>)(Object)NscMerkmal;
    		if(typeof(T) == typeof(Pflanze))
    				return (ObjectSet<T>)(Object)Pflanze;
    		if(typeof(T) == typeof(Pflanze_Ernte))
    				return (ObjectSet<T>)(Object)Pflanze_Ernte;
    		if(typeof(T) == typeof(Pflanze_Verbreitung))
    				return (ObjectSet<T>)(Object)Pflanze_Verbreitung;
    		if(typeof(T) == typeof(Polygon))
    				return (ObjectSet<T>)(Object)Polygon;
    		if(typeof(T) == typeof(Rasse))
    				return (ObjectSet<T>)(Object)Rasse;
    		if(typeof(T) == typeof(Rasse_Farbe))
    				return (ObjectSet<T>)(Object)Rasse_Farbe;
    		if(typeof(T) == typeof(Rasse_Kultur))
    				return (ObjectSet<T>)(Object)Rasse_Kultur;
    		if(typeof(T) == typeof(Rüstung))
    				return (ObjectSet<T>)(Object)Rüstung;
    		if(typeof(T) == typeof(Schild))
    				return (ObjectSet<T>)(Object)Schild;
    		if(typeof(T) == typeof(Setting))
    				return (ObjectSet<T>)(Object)Setting;
    		if(typeof(T) == typeof(Sonderfertigkeit))
    				return (ObjectSet<T>)(Object)Sonderfertigkeit;
    		if(typeof(T) == typeof(Sonderfertigkeit_Setting))
    				return (ObjectSet<T>)(Object)Sonderfertigkeit_Setting;
    		if(typeof(T) == typeof(Talent))
    				return (ObjectSet<T>)(Object)Talent;
    		if(typeof(T) == typeof(Talentgruppe))
    				return (ObjectSet<T>)(Object)Talentgruppe;
    		if(typeof(T) == typeof(Trageort))
    				return (ObjectSet<T>)(Object)Trageort;
    		if(typeof(T) == typeof(Version))
    				return (ObjectSet<T>)(Object)Version;
    		if(typeof(T) == typeof(VorNachteil))
    				return (ObjectSet<T>)(Object)VorNachteil;
    		if(typeof(T) == typeof(Waffe))
    				return (ObjectSet<T>)(Object)Waffe;
    		if(typeof(T) == typeof(Zauber))
    				return (ObjectSet<T>)(Object)Zauber;
    		if(typeof(T) == typeof(Zauber_Setting))
    				return (ObjectSet<T>)(Object)Zauber_Setting;
    		if(typeof(T) == typeof(Zauber_Variante))
    				return (ObjectSet<T>)(Object)Zauber_Variante;
    		if(typeof(T) == typeof(Zauberzeichen))
    				return (ObjectSet<T>)(Object)Zauberzeichen;
    		if(typeof(T) == typeof(Zauberzeichen_Setting))
    				return (ObjectSet<T>)(Object)Zauberzeichen_Setting;
    		return null;
    	}
    
    	public System.Linq.IQueryable GetObjectSet(Type t)
    	{
    		if(t == typeof(Abenteuer))
    				return Abenteuer;
    		if(t == typeof(Abenteuer_Ereignis))
    				return Abenteuer_Ereignis;
    		if(t == typeof(Abenteuer_Szene))
    				return Abenteuer_Szene;
    		if(t == typeof(Abenteuer_Verweis))
    				return Abenteuer_Verweis;
    		if(t == typeof(Alchimierezept))
    				return Alchimierezept;
    		if(t == typeof(Alchimierezept_Setting))
    				return Alchimierezept_Setting;
    		if(t == typeof(Audio_HotkeyWesen))
    				return Audio_HotkeyWesen;
    		if(t == typeof(Audio_Playlist))
    				return Audio_Playlist;
    		if(t == typeof(Audio_Playlist_Titel))
    				return Audio_Playlist_Titel;
    		if(t == typeof(Audio_Theme))
    				return Audio_Theme;
    		if(t == typeof(Audio_Titel))
    				return Audio_Titel;
    		if(t == typeof(Ausrüstung))
    				return Ausrüstung;
    		if(t == typeof(Ausrüstung_Setting))
    				return Ausrüstung_Setting;
    		if(t == typeof(Beschwörbares))
    				return Beschwörbares;
    		if(t == typeof(Dämon))
    				return Dämon;
    		if(t == typeof(Dämon_Domäne))
    				return Dämon_Domäne;
    		if(t == typeof(Einstellung))
    				return Einstellung;
    		if(t == typeof(Elementarwesen))
    				return Elementarwesen;
    		if(t == typeof(Farbe))
    				return Farbe;
    		if(t == typeof(Fernkampfwaffe))
    				return Fernkampfwaffe;
    		if(t == typeof(Gebiet))
    				return Gebiet;
    		if(t == typeof(Gegner))
    				return Gegner;
    		if(t == typeof(Gegner_Modifikator))
    				return Gegner_Modifikator;
    		if(t == typeof(GegnerBase))
    				return GegnerBase;
    		if(t == typeof(GegnerBase_Angriff))
    				return GegnerBase_Angriff;
    		if(t == typeof(GegnerBase_Kampfregel))
    				return GegnerBase_Kampfregel;
    		if(t == typeof(Handelsgut))
    				return Handelsgut;
    		if(t == typeof(Handelsgut_Setting))
    				return Handelsgut_Setting;
    		if(t == typeof(Held))
    				return Held;
    		if(t == typeof(Held_Ausrüstung))
    				return Held_Ausrüstung;
    		if(t == typeof(Held_Inventar))
    				return Held_Inventar;
    		if(t == typeof(Held_Modifikator))
    				return Held_Modifikator;
    		if(t == typeof(Held_Munition))
    				return Held_Munition;
    		if(t == typeof(Held_Pflanze))
    				return Held_Pflanze;
    		if(t == typeof(Held_Sonderfertigkeit))
    				return Held_Sonderfertigkeit;
    		if(t == typeof(Held_Talent))
    				return Held_Talent;
    		if(t == typeof(Held_VorNachteil))
    				return Held_VorNachteil;
    		if(t == typeof(Held_Zauber))
    				return Held_Zauber;
    		if(t == typeof(Inventar))
    				return Inventar;
    		if(t == typeof(Kampfregel))
    				return Kampfregel;
    		if(t == typeof(Kreatur))
    				return Kreatur;
    		if(t == typeof(Kultur))
    				return Kultur;
    		if(t == typeof(Kultur_Name))
    				return Kultur_Name;
    		if(t == typeof(Landschaft))
    				return Landschaft;
    		if(t == typeof(Literatur))
    				return Literatur;
    		if(t == typeof(MenuLink))
    				return MenuLink;
    		if(t == typeof(Modifikator))
    				return Modifikator;
    		if(t == typeof(Munition))
    				return Munition;
    		if(t == typeof(Name))
    				return Name;
    		if(t == typeof(Notizen))
    				return Notizen;
    		if(t == typeof(NscMerkmal))
    				return NscMerkmal;
    		if(t == typeof(Pflanze))
    				return Pflanze;
    		if(t == typeof(Pflanze_Ernte))
    				return Pflanze_Ernte;
    		if(t == typeof(Pflanze_Verbreitung))
    				return Pflanze_Verbreitung;
    		if(t == typeof(Polygon))
    				return Polygon;
    		if(t == typeof(Rasse))
    				return Rasse;
    		if(t == typeof(Rasse_Farbe))
    				return Rasse_Farbe;
    		if(t == typeof(Rasse_Kultur))
    				return Rasse_Kultur;
    		if(t == typeof(Rüstung))
    				return Rüstung;
    		if(t == typeof(Schild))
    				return Schild;
    		if(t == typeof(Setting))
    				return Setting;
    		if(t == typeof(Sonderfertigkeit))
    				return Sonderfertigkeit;
    		if(t == typeof(Sonderfertigkeit_Setting))
    				return Sonderfertigkeit_Setting;
    		if(t == typeof(Talent))
    				return Talent;
    		if(t == typeof(Talentgruppe))
    				return Talentgruppe;
    		if(t == typeof(Trageort))
    				return Trageort;
    		if(t == typeof(Version))
    				return Version;
    		if(t == typeof(VorNachteil))
    				return VorNachteil;
    		if(t == typeof(Waffe))
    				return Waffe;
    		if(t == typeof(Zauber))
    				return Zauber;
    		if(t == typeof(Zauber_Setting))
    				return Zauber_Setting;
    		if(t == typeof(Zauber_Variante))
    				return Zauber_Variante;
    		if(t == typeof(Zauberzeichen))
    				return Zauberzeichen;
    		if(t == typeof(Zauberzeichen_Setting))
    				return Zauberzeichen_Setting;
    		return null;
    	}
    	

        #endregion

    }
}
