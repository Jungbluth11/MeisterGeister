﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KampfLogic = MeisterGeister.ViewModel.Kampf.Logic;

namespace MeisterGeister.Model
{
    public partial class GegnerBase : KampfLogic.IHasZonenRs, MeisterGeister.Logic.Literatur.ILiteratur
    {
        public GegnerBase()
        {
            GegnerBaseGUID = Guid.NewGuid();
            Name = "Neuer Gegner";
            INIZufall = "1W6";
            GS = 8;
            Aktionen = 2;
        }

        public bool Usergenerated
        {
            get { return !GegnerBaseGUID.ToString().StartsWith("00000000-0000-0000-00"); }
        }

        public List<string> TagListe()
        {
            return (Tags ?? string.Empty).Split(new char[] { ',', ';', '/' }).Select(s => s.Trim()).ToList();
        }

        #region Import Export

        public static GegnerBase Import(string pfad, bool batch = false)
        {
            return Import(pfad, Guid.Empty, batch);
        }

        public static GegnerBase Import(string pfad, Guid newGuid, bool batch = false)
        {
            Service.SerializationService serialization = Service.SerializationService.GetInstance(!batch);
            Guid gegnerGuid = serialization.ImportGegner(pfad, newGuid);
            if (gegnerGuid == Guid.Empty)
                return null;
            Global.ContextKampf.UpdateList<GegnerBase>();
            return Global.ContextKampf.Liste<GegnerBase>().Where(g => g.GegnerBaseGUID == gegnerGuid).First();
        }

        public void Export(string pfad, bool batch = false)
        {
            Service.SerializationService serialization = Service.SerializationService.GetInstance(!batch);
            serialization.ExportGegner(GegnerBaseGUID, pfad);
        }

        public GegnerBase Clone(bool batch = false)
        {
            return Clone(Guid.NewGuid(), batch);
        }

        public GegnerBase Clone(Guid newGuid, bool batch = false)
        {
            Service.SerializationService serialization = Service.SerializationService.GetInstance(!batch);
            Guid gegnerGuid = serialization.CloneGegner(GegnerBaseGUID, newGuid);
            if (gegnerGuid == Guid.Empty)
                return null;
            Global.ContextHeld.UpdateList<GegnerBase>();
            return Global.ContextHeld.Liste<GegnerBase>().Where(h => h.GegnerBaseGUID == gegnerGuid).FirstOrDefault();
        }
        #endregion

        /// <summary>
        /// Eine Zusammenführung aller durchsuchbaren Felder.
        /// </summary>
        private string SuchText
        {
            get { return (Name ?? string.Empty).ToLower() + (Tags ?? string.Empty).ToLower() + (Verbreitung ?? string.Empty).ToLower() + (Literatur ?? string.Empty).ToLower(); }
        }

        /// <summary>
        /// Prüft, ob 'suchWort' im Namen oder in den Tags vorkommt.
        /// </summary>
        /// <param name="suchWort"></param>
        /// <returns></returns>
        public bool Contains(string suchWort)
        {
            return SuchText.Contains(suchWort);
        }

        /// <summary>
        /// Prüft, ob die 'suchWorte' im Namen, der Kategorie oder in den Tags vorkommt.
        /// Es wird dabei eine UND-Prüfung durchgeführt.
        /// </summary>
        /// <param name="suchWorte"></param>
        /// <returns></returns>
        public bool Contains(string[] suchWorte)
        {
            foreach (string wort in suchWorte)
            {
                if (!Contains(wort))
                    return false;
            }
            return true;
        }

        private KampfLogic.Rüstungsschutz _rs = null;
        public KampfLogic.IRüstungsschutz RS
        {
            get
            {
                if (_rs == null)
                    _rs = new KampfLogic.Rüstungsschutz((GegnerBase)this);
                return _rs;
            }
        }

        #region Zauber

        public Zauber AddZauber(Zauber z, int wert, int E1, int E2, int E3)
        {
            if (z == null)
                return null;
            IEnumerable<GegnerBase_Zauber> existierendeZuordnung = GegnerBase_Zauber.Where(hza => hza.ZauberGUID == z.ZauberGUID
                && hza.GegnerBaseGUID == GegnerBaseGUID);
            if (existierendeZuordnung.Count() != 0)
            {
                //Oder eine Exception werfen?
                return existierendeZuordnung.First().Zauber;
            }

            GegnerBase_Zauber hz = Global.ContextHeld.New<GegnerBase_Zauber>();
            hz.GegnerBaseGUID = GegnerBaseGUID;
            hz.GegnerBase = this;

            hz.ZauberGUID = z.ZauberGUID;
            hz.Zauber = z;
            hz.ZfW = wert;
            GegnerBase_Zauber.Add(hz);
            return z;
        }

        #endregion


        public void ParseBemerkung()
        {
            var g = this;
            if (g.Bemerkung != null && g.Bemerkung.Trim() != String.Empty)
                foreach (string zeile in g.Bemerkung.Split(new char[] { '\n', '\r' }))
                {
                    GegnerBase_Angriff ga = Model.GegnerBase_Angriff.Parse(zeile);
                    if (ga != null)
                    {
                        string name = ga.Name; int i = 1;
                        while (g.GegnerBase_Angriff.Where(gba => gba.Name == name).Count() > 0)
                            name = String.Format("{0} ({1})", ga.Name, ++i);
                        ga.Name = name;
                        g.GegnerBase_Angriff.Add(ga);
                    }
                    else
                    {
                        Dictionary<string, int> erschwernisse;
                        IEnumerable<Kampfregel> kampfregeln = Kampfregel.Parse(zeile, out erschwernisse);
                        if (kampfregeln != null && kampfregeln.Count() > 0)
                            foreach (Kampfregel kr in kampfregeln)
                            {
                                if (g.GegnerBase_Kampfregel.Where(gbkr => gbkr.KampfregelGUID == kr.KampfregelGUID).Count() == 0)
                                {
                                    string eName = erschwernisse.Keys.Where(e => kr.Name.ToUpperInvariant().Contains(e.ToUpperInvariant())).FirstOrDefault();
                                    var gkr = new GegnerBase_Kampfregel();
                                    gkr.KampfregelGUID = kr.KampfregelGUID;
                                    gkr.GegnerBaseGUID = g.GegnerBaseGUID;
                                    if (eName != null)
                                        gkr.Erschwernis = erschwernisse[eName];
                                    g.GegnerBase_Kampfregel.Add(gkr);
                                }
                            }
                    }
                }
        }
    }
}
