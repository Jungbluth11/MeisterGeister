﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//Eigene Usings
using Model = MeisterGeister.Model;
using System.Diagnostics;

namespace MeisterGeister.Model.Service
{
    public class DataService : ServiceBase
    {

        #region //----- EIGENSCHAFTEN ----

        public List<Model.Held> HeldenGruppeListe
        {
            get { return Liste<Held>().Where(h => h.Regelsystem == Global.Regeledition).Where(h => h.AktiveHeldengruppe == true && !(h.HeldGUID.ToString("D").ToUpperInvariant().StartsWith("00000000-0000-0000-045C")) && h.Regelsystem == Global.Regeledition).OrderBy(h => h.Name).ToList(); }
        }

        /// <summary>
        /// Zauberliste mit angewandtem Setting-Filter.
        /// </summary>
        public List<Model.Zauber> ZauberListe
        {
            get
            {
                return Context.Setting.Where(s => s.Aktiv == true).SelectMany(s => s.Zauber_Setting.Select(s_s => s_s.Zauber)).ToList().Distinct().ToList();
            }
        }

        /// <summary>
        /// Talentliste mit angewandtem Setting-Filter
        /// </summary>
        public List<Model.Talent> TalentListe
        {
            get
            {
                return Liste<Talent>().Where(t => t.Regelsystem == Global.Regeledition).Where(t => t.TalentgruppeID != 0)
                    .Where(t => Setting.AktiveSettings.Any(s => (t.Setting ?? "Aventurien").Contains(s.Name))).ToList();
            }
        }

        #endregion

        #region //----- KONSTRUKTOR ----

        public DataService()
        {
        }

        #endregion

        #region //----- DATENBANKABFRAGEN ----

        public List<Model.Sonderfertigkeit> SonderfertigkeitListe
        {
            get
            {
                return Context.Setting.Where(s => s.Aktiv == true).SelectMany(s => s.Sonderfertigkeit_Setting.Select(s_s => s_s.Sonderfertigkeit)).ToList().Distinct().ToList();
            }
        }

        public Held LoadHeldByName(string aName)
        {
            var tmp = Context.Held.Where(held => held.Name == aName).FirstOrDefault();
            return tmp;
        }

        public Held LoadHeldByGUID(Guid id)
        {
            var tmp = Context.Held.Where(held => held.HeldGUID == id).FirstOrDefault();
            return tmp;
        }

        /// <summary>
        /// Case-insensitiv.
        /// </summary>
        public Sonderfertigkeit LoadSonderfertigkeitByName(string aName)
        {
            var tmp = Context.Sonderfertigkeit.Where(s => s.Name.ToLower() == aName.ToLower()).FirstOrDefault();
            return tmp;
        }

        /// <summary>
        /// Case-insensitiv.
        /// </summary>
        public Zauber LoadZauberByName(string aName)
        {
            var tmp = Context.Zauber.Where(s => s.Name.ToLower() == aName.ToLower()).FirstOrDefault();
            if (tmp == null)
                tmp = Context.Zauber.Where(s => s.Name.ToLower().StartsWith(aName.ToLower())).FirstOrDefault();
            return tmp;
        }

        /// <summary>
        /// Case-insensitiv.
        /// </summary>
        public Talent LoadTalentByName(string aName)
        {
            return LoadTalentByName(aName, Global.Regeledition);
        }

        /// <summary>
        /// Case-insensitiv.
        /// </summary>
        public Talent LoadTalentByName(string aName, string regelsystem)
        {
            var tmp = Context.Talent.Where(s => s.Talentname.ToLower() == aName.ToLower() && s.Regelsystem == regelsystem).FirstOrDefault();
            return tmp;
        }

        /// <summary>
        /// Case-insensitiv.
        /// </summary>
        /// <param name="aName"></param>
        /// <returns></returns>
        public VorNachteil LoadVorNachteilByName(string aName)
        {
            return LoadVorNachteilByName(aName, Global.Regeledition);
        }

        /// <summary>
        /// Case-insensitiv.
        /// </summary>
        /// <param name="aName"></param>
        /// <returns></returns>
        public VorNachteil LoadVorNachteilByName(string aName, string regelsystem)
        {
            var tmp = Context.VorNachteil.Where(s => s.Name.ToLower() == aName.ToLower() && s.Regelsystem == regelsystem).FirstOrDefault();
            return tmp;
        }

        #endregion

        #region Alchimie
        public List<Held> LoadHeldenGruppeWithAlchimie()
        {
            List<Held> tmp = Liste<Talent>().Where(s => s.Talentname == "Alchimie" || s.Talentname == "Kochen")
                .Join(Context.Held_Talent, t => t.TalentGUID, ht => ht.TalentGUID, (t, ht) => ht)
                .Join(Context.Held, hs => hs.HeldGUID, h => h.HeldGUID, (hs, h) => h).ToList();
            return tmp.Distinct().ToList();
        }

        public List<Talent> LoadAlchimieHerstellungTalenteByHeld(Held held)
        {
            if (held == null)
                return Liste<Talent>().Where(t => t.Talentname == "Alchimie" || t.Talentname == "Kochen").ToList();
            List<Talent> talente = Liste<Held>().Where(h => h.HeldGUID == held.HeldGUID).Join(Context.Held_Talent, h => h.HeldGUID, ht => ht.HeldGUID, (h, ht) => ht)
                .Join(Context.Talent, ht => ht.TalentGUID, t => t.TalentGUID, (ht, t) => t).Where(t => t.Talentname == "Alchimie" || t.Talentname == "Kochen").ToList();
            return talente;
        }

        public List<string> LoadAlchimieGruppe()
        {
            List<string> tmp = Liste<Alchimierezept>().Select(s => s.Gruppe).Distinct().ToList();
            return tmp;
        }

        public List<Alchimierezept> LoadAlchimieRezepteByGruppe(string gruppe)
        {
            if (!string.IsNullOrEmpty(gruppe))
            {
                List<Alchimierezept> tmp = Liste<Alchimierezept>().Where(s => s.Gruppe == gruppe).ToList();
                return tmp;
            }
            return new List<Alchimierezept>();
        }

        public string LoadAlchimieLaborByRezept(string rezept)
        {
            if (!string.IsNullOrEmpty(rezept))
            {
                List<string> tmp = Liste<Alchimierezept>().Where(s => s.Name == rezept).Select(s => s.Labor).Distinct().ToList();
                return tmp[0];
            }
            return "";
        }

        public List<string> LoadIntensitätsbestimmungFertigkeitenAlchimieByHeld(Held held)
        {
            if (held == null)
                return new List<string>();

            List<string> zauber = Liste<Held>().Where(h => h.HeldGUID == held.HeldGUID).Join(Context.Held_Zauber, h => h.HeldGUID, hz => hz.HeldGUID, (h, hz) => hz)
                .Join(Context.Zauber, hz => hz.ZauberGUID, z => z.ZauberGUID, (hz, z) => z).Where(z => z.Name == "Odem Arcanum" || z.Name == "Oculus Astralis").OrderBy(z => z.Name).Select(z => z.Name).ToList();
            List<string> liturgien = Liste<Held>().Where(h => h.HeldGUID == held.HeldGUID).Join(Context.Held_Sonderfertigkeit, h => h.HeldGUID, hs => hs.HeldGUID, (h, hs) => hs)
                .Join(Context.Sonderfertigkeit, hs => hs.SonderfertigkeitGUID, s => s.SonderfertigkeitGUID, (hs, s) => s)
                .Where(s => s.Name == "Liturgie: Sicht auf Madas Welt" || s.Name == "Keulenritual: Gespür der Keule").OrderBy(s => s.Name).Select(s => s.Name).ToList();
            List<string> talente = Liste<Held>().Where(h => h.HeldGUID == held.HeldGUID).Join(Context.Held_Talent, h => h.HeldGUID, ht => ht.HeldGUID, (h, ht) => ht)
                .Join(Context.Talent, ht => ht.TalentGUID, t => t.TalentGUID, (ht, t) => t).Where(t => t.Talentname == "Pflanzenkunde" || t.Talentname == "Magiegespür").Select(t => t.Talentname).ToList();

            List<string> ret = new List<string>();
            ret.AddRange(zauber);
            ret.AddRange(talente);
            ret.AddRange(liturgien);
            return ret;
        }

        internal List<string> LoadStrukturanalyseFertigkeitenAlchimieByHeld(Held held)
        {
            if (held == null)
                return new List<string>();

            List<string> zauber = Liste<Held>().Where(h => h.HeldGUID == held.HeldGUID).Join(Context.Held_Zauber, h => h.HeldGUID, hz => hz.HeldGUID, (h, hz) => hz)
                .Join(Context.Zauber, hz => hz.ZauberGUID, z => z.ZauberGUID, (hz, z) => z).Where(z => z.Name == "Analys Arcanstruktur" || z.Name == "Oculus Astralis").OrderBy(z => z.Name).Select(z => z.Name).ToList();
            List<string> talente = Liste<Held>().Where(h => h.HeldGUID == held.HeldGUID).Join(Context.Held_Talent, h => h.HeldGUID, ht => ht.HeldGUID, (h, ht) => ht)
                .Join(Context.Talent, ht => ht.TalentGUID, t => t.TalentGUID, (ht, t) => t).Where(t => t.Talentname == "Alchimie" || t.Talentname == "Ritualkenntnis" || t.Talentname == "Liturgiekenntnis").Select(t => t.Talentname).ToList();
            List<string> liturgien = Liste<Held>().Where(h => h.HeldGUID == held.HeldGUID).Join(Context.Held_Sonderfertigkeit, h => h.HeldGUID, hs => hs.HeldGUID, (h, hs) => hs)
                .Join(Context.Sonderfertigkeit, hs => hs.SonderfertigkeitGUID, s => s.SonderfertigkeitGUID, (hs, s) => s)
                .Where(s => s.Name == "Liturgie: Blick der Weberin" || s.Name == "Liturgie: Blick durch Tairachs Augen" || s.Name == "Schalenzauber: Allegorische Analyse").OrderBy(s => s.Name).Select(s => s.Name).ToList();
            List<string> ret = new List<string>();
            ret.AddRange(zauber);
            ret.AddRange(talente);
            ret.AddRange(liturgien);
            return ret;
        }
        #endregion

        #region Einstellungen/Settings
        public Einstellung LoadEinstellungByName(string name)
        {
            //Dies sorgt dafür dass die Datenbankzugriffe für Einstellungen aus dem richtigen Thread geschehen. Es ist eine Lösung, die man generalisieren könnte um den Datenbankzugriff generell threadsicher zu machen.
            if (System.Windows.Threading.Dispatcher.CurrentDispatcher != _dispatcher)
            {
                return (Einstellung)_dispatcher.Invoke((Func<string, Einstellung>)LoadEinstellungByName, name);
            }
            return Context.Einstellung.Where(e => e.Name == name).FirstOrDefault();
        }

        /// <summary>
        /// Alle Einstellungen, deren name so beginnt.
        /// </summary>
        /// <param name="name">Anfang des Namens (case insensitiv)</param>
        /// <returns></returns>
        public IList<Einstellung> LoadEinstellungenByName(string name)
        {
            return Context.Einstellung.Where(e => e.Name.ToLower().StartsWith(name)).ToList();
        }
        #endregion

        #region Kampf und Gegner

        public Gegner CreateGegnerInstance(GegnerBase gegnerBase)
        {
            Gegner g = new Gegner(gegnerBase);
            Insert<Gegner>(g);
            return g;
        }

        #endregion

        #region Literatur/Wege des Wissens
        public Literatur LoadLiteraturByAbkürzung(string abkürzung, bool isErrata = false)
        {
            if (isErrata)
                abkürzung = abkürzung + " Errata";
            return Context.Literatur.Where(l => l.Abkürzung == abkürzung).FirstOrDefault();
        }
        #endregion

        #region Namen und NSC
        //später durch GUID-Variante ersetzen
        //Geschlecht Methode ohne Geschlecht fliegt dann raus
        public List<string> LoadNamenByNamenstypArt(string namenstyp, string art, bool bedeutungStattName = false)
        {
            return Context.Name.Where(n => (n.Herkunft == namenstyp && n.Art == art)).Select(n => bedeutungStattName ? n.Bedeutung : n.Name1).ToList();
        }

        public List<string> LoadNamenByNamenstypArtGeschlecht(string namenstyp, string art, bool weiblich, bool bedeutungStattName = false)
        {
            return Context.Name.Where(n => (n.Herkunft == namenstyp && n.Art == art && (n.Geschlecht == (weiblich ? "w" : "m") || n.Geschlecht == null))).Select(n => bedeutungStattName ? n.Bedeutung : n.Name1).ToList();
        }

        public List<string> getRasseByKulturName(string kultur, bool unüblicheKulturen = false)
        {
            if (string.IsNullOrEmpty(kultur))
            {
                return Liste<Model.Rasse>()
                    .Select(r => r.Name)
                    .Distinct().OrderBy(n => n).ToList();
            }
            else
            {
                if (unüblicheKulturen)
                    return Liste<Model.Kultur>()
                        .Where(k => k.Name == kultur)
                        .Join(Context.Rasse_Kultur, k => k.KulturGUID, rk => rk.KulturGUID, (k, rk) => rk)
                        .Join(Context.Rasse, rk => rk.RasseGUID, r => r.RasseGUID, (rk, r) => r)
                        .Select(r => r.Name).Distinct().OrderBy(n => n).ToList();
                else
                    return Liste<Model.Kultur>()
                        .Where(k => k.Name == kultur)
                        .Join(Context.Rasse_Kultur, k => k.KulturGUID, rk => rk.KulturGUID, (k, rk) => rk)
                        .Where(rk => rk.Unüblich == false)
                        .Join(Context.Rasse, rk => rk.RasseGUID, r => r.RasseGUID, (rk, r) => r)
                        .Select(r => r.Name).Distinct().OrderBy(n => n).ToList();
            }
        }

        public List<string> getKulturByRasseName(string rasse, bool unueblicheKulturen = false)
        {
            if (string.IsNullOrEmpty(rasse))
            {
                return Liste<Model.Kultur>()
                    .Select(k => k.Name)
                    .Distinct().OrderBy(n => n).ToList();
            }
            else
            {
                if (unueblicheKulturen)
                    return Liste<Model.Rasse>()
                        .Where(r => r.Name == rasse)
                        .Join(Context.Rasse_Kultur, r => r.RasseGUID, rk => rk.RasseGUID, (r, rk) => rk)
                        .Join(Context.Kultur, rk => rk.KulturGUID, k => k.KulturGUID, (rk, k) => k)
                        .Select(k => k.Name).Distinct().OrderBy(n => n).ToList();
                else
                    return Liste<Model.Rasse>()
                        .Where(r => r.Name == rasse)
                        .Join(Context.Rasse_Kultur, r => r.RasseGUID, rk => rk.RasseGUID, (k, rk) => rk)
                        .Where(rk => rk.Unüblich == false)
                        .Join(Context.Kultur, rk => rk.KulturGUID, k => k.KulturGUID, (rk, k) => k)
                        .Select(k => k.Name).Distinct().OrderBy(n => n).ToList();
            }
        }
        #endregion

        #region Zauberzeichen
        public List<Held> LoadHeldenGruppeWithZauberzeichen()
        {
            List<Held> tmp = Liste<Sonderfertigkeit>().Where(s => s.Name == "Zauberzeichen" || s.Name == "Runenkunde" || s.Name.StartsWith("Zauberzeichen: Bann") || s.Name.StartsWith("Zauberzeichen: Schutz"))
                .Join(Context.Held_Sonderfertigkeit, s => s.SonderfertigkeitGUID, hs => hs.SonderfertigkeitGUID, (s, hs) => hs)
                .Join(Context.Held, hs => hs.HeldGUID, h => h.HeldGUID, (hs, h) => h).ToList();
            return tmp.Distinct().ToList();
        }
        public List<Zauberzeichen> LoadZauberzeichenByHeld(Model.Held held)
        {
            if (held == null)
                return Liste<Zauberzeichen>().Where(z => z.Typ == "Arkanoglyphe").ToList();
            List<Zauberzeichen> zauberzeichen = Liste<Held>().Where(h => h.HeldGUID == held.HeldGUID).Join(Context.Held_Sonderfertigkeit, h => h.HeldGUID, hs => hs.HeldGUID, (h, hs) => hs)
                .Join(Context.Sonderfertigkeit, hs => hs.SonderfertigkeitGUID, s => s.SonderfertigkeitGUID, (hs, s) => s)
                .Join(Context.Zauberzeichen, s => s.SonderfertigkeitGUID, zz => zz.SonderfertigkeitGUID, (s, zz) => zz).Where(z => z.Typ == "Arkanoglyphe").ToList();
            return zauberzeichen;
        }

        public List<Zauberzeichen> LoadKreiseByHeld(Held held)
        {
            if (held == null)
                return Liste<Zauberzeichen>().Where(z => z.Typ == "Bannkreis" || z.Typ == "Schutzkreis").ToList();
            List<Zauberzeichen> kreise = Liste<Held>().Where(h => h.HeldGUID == held.HeldGUID).Join(Context.Held_Sonderfertigkeit, h => h.HeldGUID, hs => hs.HeldGUID, (h, hs) => hs)
                .Join(Context.Sonderfertigkeit, hs => hs.SonderfertigkeitGUID, s => s.SonderfertigkeitGUID, (hs, s) => s)
                .Join(Context.Zauberzeichen, s => s.SonderfertigkeitGUID, zz => zz.SonderfertigkeitGUID, (s, zz) => zz).Where(z => z.Typ == "Bannkreis" || z.Typ == "Schutzkreis").ToList();
            return kreise;
        }

        public List<Zauberzeichen> LoadRunenByHeld(Held held)
        {
            if (held == null)
                return Liste<Zauberzeichen>().Where(z => z.Typ == "Rune").ToList();
            List<Zauberzeichen> runen = Liste<Held>().Where(h => h.HeldGUID == held.HeldGUID).Join(Context.Held_Sonderfertigkeit, h => h.HeldGUID, hs => hs.HeldGUID, (h, hs) => hs)
                .Join(Context.Sonderfertigkeit, hs => hs.SonderfertigkeitGUID, s => s.SonderfertigkeitGUID, (hs, s) => s)
                .Join(Context.Zauberzeichen, s => s.SonderfertigkeitGUID, zz => zz.SonderfertigkeitGUID, (s, zz) => zz).Where(z => z.Typ == "Rune").ToList();
            return runen;
        }

        private string[] rkNamen = new string[] { "Ritualkenntnis (Gildenmagie)", "Ritualkenntnis (Kristallomantie)", "Ritualkenntnis (Alchimie)", "Ritualkenntnis (Zibiljas)", "Ritualkenntnis (Runenkunde)" };
        public Talent GetMaxRitualkenntnis(Held held)
        {
            if (held == null)
                return null;
            List<Talent> rkList = Liste<Talent>().Where(t => rkNamen.Contains(t.Talentname)).OrderByDescending(t => held.HatTalent(t) ? held.Talentwert(t) : -99).ToList();
            if (rkList.Count == 0 || !held.HatTalent(rkList[0]))
                return null;
            return rkList[0];
        }

        public int LoadMaxRitualkenntnisWertByHeld(Held held)
        {
            Talent rk = GetMaxRitualkenntnis(held);
            if (rk == null)
                return 0;
            return held.Talentwert(rk);
        }

        public List<Talent> LoadZauberzeichenTalenteByHeld(Held held)
        {
            if (held == null)
                return Liste<Talent>().Where(t => t.Talentname == "Feinmechanik" || t.Talentname == "Holzbearbeitung" || t.Talentname == "Malen/Zeichnen" || t.Talentname == "Schneidern" || t.Talentname == "Webkunst").ToList();
            List<Talent> talente = Liste<Held>().Where(h => h.HeldGUID == held.HeldGUID).Join(Context.Held_Talent, h => h.HeldGUID, ht => ht.HeldGUID, (h, ht) => ht)
                .Join(Context.Talent, ht => ht.TalentGUID, t => t.TalentGUID, (ht, t) => t).Where(t => t.Talentname == "Feinmechanik" || t.Talentname == "Holzbearbeitung" || t.Talentname == "Malen/Zeichnen" || t.Talentname == "Schneidern" || t.Talentname == "Webkunst").ToList();
            return talente;
        }

        public List<Talent> LoadRunenTalenteByHeld(Held held)
        {
            if (held == null)
                return Liste<Talent>().Where(t => t.Talentname == "Feinmechanik" || t.Talentname == "Holzbearbeitung" || t.Talentname == "Malen/Zeichnen" || t.Talentname == "Schneidern" || t.Talentname == "Webkunst" || t.Talentname == "Tätowieren").ToList();
            List<Talent> talente = Liste<Held>().Where(h => h.HeldGUID == held.HeldGUID).Join(Context.Held_Talent, h => h.HeldGUID, ht => ht.HeldGUID, (h, ht) => ht)
                .Join(Context.Talent, ht => ht.TalentGUID, t => t.TalentGUID, (ht, t) => t).Where(t => t.Talentname == "Feinmechanik" || t.Talentname == "Holzbearbeitung" || t.Talentname == "Malen/Zeichnen" || t.Talentname == "Schneidern" || t.Talentname == "Webkunst" || t.Talentname == "Tätowieren").ToList();
            return talente;
        }
        #endregion

        #region Beschwörung
        public List<GegnerBase> LoadDämonen()
        {
            return Context.GegnerBase.Where(g => g.Typ == "Dämon" && g.Beschwörbares != null && g.Beschwörbares.Dämon != null).ToList();
        }
        public List<GegnerBase> GetDämonenByKlasse(GegnerBase dämon)
        {
            var alle = from gegner in Context.GegnerBase where gegner.Typ == "Dämon" && gegner.Beschwörbares != null select gegner;
            var domänen = from wesen in alle.ToList()
                          join domäne1 in Context.Dämon_Domäne
                            on wesen.GegnerBaseGUID equals domäne1.GegnerBaseGUID
                          join domäne2 in dämon.Beschwörbares.Dämon.Dämon_Domäne
                            on domäne1.Domäne equals domäne2.Domäne
                          where (wesen.Beschwörbares.Dämon.Hörner == 0) == (dämon.Beschwörbares.Dämon.Hörner == 0)
                          select wesen;
            return domänen.ToList();
        }
        public List<GegnerBase> GetDämonenByDomäne(GegnerBase dämon)
        {
            var alle = from gegner in Context.GegnerBase where gegner.Typ == "Dämon" && gegner.Beschwörbares != null select gegner;
            var domänen = from wesen in alle.ToList()
                          join domäne1 in Context.Dämon_Domäne
                            on wesen.GegnerBaseGUID equals domäne1.GegnerBaseGUID
                          join domäne2 in dämon.Beschwörbares.Dämon.Dämon_Domäne
                            on domäne1.Domäne equals domäne2.Domäne
                          select wesen;
            return domänen.ToList();
        }
        public GegnerBase GetLeichtererDämon(GegnerBase dämon)
        {
            var möglichkeiten = from wesen in GetDämonenByKlasse(dämon)
                                where wesen.Beschwörbares.Beschwörung < dämon.Beschwörbares.Beschwörung
                                orderby wesen.Beschwörbares.Beschwörung descending
                                select wesen;
            if (möglichkeiten.Count() == 0) return null;
            else return möglichkeiten.First();
        }

        public GegnerBase GetSchwerererDämon(GegnerBase dämon)
        {
            var möglichkeiten = from wesen in GetDämonenByKlasse(dämon)
                                where wesen.Beschwörbares.Beschwörung > dämon.Beschwörbares.Beschwörung
                                orderby wesen.Beschwörbares.Beschwörung ascending
                                select wesen;
            if (möglichkeiten.Count() == 0) return null;
            else return möglichkeiten.First();
        }

        public GegnerBase GetGehörntererDämonAusDomäne(GegnerBase dämon)
        {
            var möglichkeiten = from wesen in GetDämonenByDomäne(dämon)
                                where wesen.Beschwörbares.Dämon.Hörner > 0 && wesen.Beschwörbares.Beschwörung > dämon.Beschwörbares.Beschwörung
                                orderby wesen.Beschwörbares.Beschwörung ascending
                                select wesen;
            if (möglichkeiten.Count() == 0) return null;
            else return möglichkeiten.First();
        }

        public GegnerBase GetGehörntererDämon(GegnerBase dämon)
        {
            var möglichkeiten = from wesen in LoadDämonen()
                                where wesen.Beschwörbares.Dämon.Hörner > 0 && wesen.Beschwörbares.Beschwörung > dämon.Beschwörbares.Beschwörung
                                orderby wesen.Beschwörbares.Beschwörung ascending
                                select wesen;
            if (möglichkeiten.Count() == 0) return null;
            else return möglichkeiten.First();
        }

        public List<GegnerBase> LoadElementare()
        {
            return Context.GegnerBase.Where(g => g.Typ == "Elementar" && g.Beschwörbares != null && g.Beschwörbares.Kontrolle != null).ToList();
        }

        public List<GegnerBase> LoadGeister()
        {
            return Context.GegnerBase.Where(g => g.Typ == "Geist").ToList();
        }

        public List<GegnerBase> LoadUntote()
        {
            return Context.GegnerBase.Where(g => g.Typ == "Untot").ToList();
        }
        #endregion
    }
}