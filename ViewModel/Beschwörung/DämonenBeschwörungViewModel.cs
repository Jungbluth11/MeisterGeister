﻿using MeisterGeister.Logic.General;
using MeisterGeister.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeisterGeister.ViewModel.Beschwörung
{
    public class DämonenBeschwörungViewModel : BeschwörungViewModel
    {
        private const string AFFINITÄT = "Affinität zu Dämonen";

        public DämonenBeschwörungViewModel()
        {
            magiekundeProbe = new Base.CommandBase((o) => magiekunde(), null);
            malenProbe = new Base.CommandBase((o) => malen(), null);
            editMagiekunde = new Base.CommandBase((o) => invocatioMagiekunde.Value = 1, null);
            editMalen = new Base.CommandBase((o) => invocatioMalen.Value = 1, null);
        }

        protected override void checkHeld()
        {
            base.checkHeld();
            if (Held != null)
                affinität.Value = Held.HatVorNachteil(AFFINITÄT);
            else
                affinität.Value = false;
        }

        protected override List<GegnerBase> loadWesen()
        {
            return Global.ContextHeld.LoadDämonen();
        }

        protected override void reset()
        {
            base.reset();
            Hörner = 0;
            AndererDämon = false;
            BeschwörungMisslungenErgebnis = BeherrschungMisslungenErgebnis = String.Empty;
            würfleBeschwörungMisslungen = new Base.CommandBase((o) => würfleBeschwörungMisslungenEffekt(), (o) => Status == BeschwörungsStatus.BeschwörungMisslungen);
            WürfleBeherrschungMisslungen = null;
        }

        #region Beschwören

        private Base.CommandBase würfleBeschwörungMisslungen;
        public Base.CommandBase WürfleBeschwörungMisslungen
        {
            get { return würfleBeschwörungMisslungen; }
            private set { Set(ref würfleBeschwörungMisslungen, value); }
        }


        private void würfleBeschwörungMisslungenEffekt()
        {
            int wurf = View.General.ViewHelper.ShowWürfelDialog(blutmagie.Value1 ? "3W6" : "2W6", "Beschwörung Misslungen");
            if (name.Value == 0) wurf += 7;
            if (wurf <= 6)
            {
                BeschwörungMisslungenErgebnis = "Außer einem kalten, übel riechenden Hauch erscheint ... nichts. Die Beschwörungskosten betragen die Hälfte dessen, was für den Spruch üblich ist.";
                //Button deaktivieren
                WürfleBeschwörungMisslungen = new Base.CommandBase((o) => { }, (o) => false);
            }
            else if (wurf <= 11)
            {
                BeschwörungMisslungenErgebnis = "Es erscheint ein Dämon aus derselben Domäne und von derselben Klasse (Niederer oder gehörnter Dämon) wie der angerufene, jedoch von niedrigerer Beschwörungsschwierigkeit. Existiert kein solcher Dämon, gilt die nächst höhere Auswirkung. Die Beschwörungskosten betragen die Hälfte dessen, was für den Spruch üblich ist.";
                Status = BeschwörungsStatus.Beherrschen;
                AndererDämon = true;
            }
            else if (wurf <= 15)
            {
                BeschwörungMisslungenErgebnis = "Es erscheint ein Dämon aus derselben Domäne und von derselben Klasse (Niederer oder gehörnter Dämon) wie der angerufene, jedoch von höherer Beschwörungsschwierigkeit. Existiert kein solcher Dämon, gilt die nächst höhere Auswirkung. Die Beschwörungskosten betragen die Hälfte dessen, was für den Spruch üblich ist.";
                Status = BeschwörungsStatus.Beherrschen;
                AndererDämon = true;
            }
            else if (wurf <= 19)
            {
                BeschwörungMisslungenErgebnis = "Es erscheint ein Dämon aus derselben Domäne, jedoch auf jeden Fall ein Gehörnter Dämon von höherer Beschwörungsschwierigkeit. Existiert kein solcher Dämon, gilt die nächst höhere Auswirkung. Wenn Sie mit den Experten-Regeln zum Schleichenden Verfall spielen, erhält der Beschwörer 1 Punkt Verfall. Die Beschwörungskosten betragen 19 AsP.";
                Status = BeschwörungsStatus.Beherrschen;
                AndererDämon = true;
            }
            else
            {
                BeschwörungMisslungenErgebnis = "Es erscheint ein Gehörnter Dämon von höherer Beschwörungs-Schwierigkeit, unabhängig von der Domäne des Gerufenen. Wenn Sie mit den Experten-Regeln zum Schleichenden Verfall spielen, erhält der Beschwörer 1W6 Punkte Verfall. Die Beschwörungskosten betragen 19 AsP.";
                Status = BeschwörungsStatus.Beherrschen;
                AndererDämon = true;
            }
        }

        protected override void beherrschungMisslungen(ProbenErgebnis erg)
        {
            base.beherrschungMisslungen(erg);
            WürfleBeherrschungMisslungen = new Base.CommandBase((o) => würfleBeherrschungMisslungenEffekt(erg), (o) => Status == BeschwörungsStatus.BeherrschungMisslungen);
        }

        private Base.CommandBase würfleBeherrschungMisslungen = null;
        public Base.CommandBase WürfleBeherrschungMisslungen
        {
            get { return würfleBeherrschungMisslungen; }
            private set { Set(ref würfleBeherrschungMisslungen, value); }
        }

        private void würfleBeherrschungMisslungenEffekt(ProbenErgebnis erg)
        {
            int wurf = View.General.ViewHelper.ShowWürfelDialog("2W6", "Kontrolle Misslungen");
            if (erg.Ergebnis == Logic.General.ErgebnisTyp.PATZER)
            {
                wurf += schwierigkeit.Value2;
                if (erg.Qualität > 0)
                    wurf -= erg.Qualität;
            }
            else wurf += (int)Math.Round(schwierigkeit.Value2 / 2.0, MidpointRounding.AwayFromZero);
            if (Hörner > 0) wurf += 5;
            if (wurf <= 1)
            {
                BeherrschungMisslungenErgebnis = "Der Beschwörer zwingt den Dämon binnen 2 Aktionen doch noch unter seine Kontrolle und presst ihm die Erfüllung eines Dienstes ab, eventuelle weitere Dienste verfallen. Dämon und Beschwörer sind während dieser Zeit in einem Duell der Willenskraft verstrickt und können keine anderen Handlungen unternehmen. Wenn Sie mit den Experten-Regeln zum Schleichenden Verfall spielen, erhält der Beschwörer 2 Punkte Verfall.";
            }
            else if (wurf <= 5)
            {
                BeherrschungMisslungenErgebnis = "Der Dämon zieht sich verärgert in seine Sphäre zurück, alle noch offenen Dienste verfallen.";
            }
            else if (wurf <= 9)
            {
                BeherrschungMisslungenErgebnis = "Der Dämon zieht sich verärgert in seine Sphäre zurück, alle noch offenen Dienste verfallen. " +
                           "Die Beschwörung dieses speziellen Dämonen ist für den Beschwörer in Zukunft um 3 Punkte erschwert. " +
                           "(Dies lässt sich durch 20 AP wieder aufheben; Paktierer können stattdessen 20 Pakt-GP einsetzen; entstammen Dämon und Pakt nicht derselben Domäne, betragen die Kosten 50 Pakt-GP.) 2 Punkte Verfall.";
            }
            else if (wurf <= 13)
            {
                BeherrschungMisslungenErgebnis = "Der Dämon greift den Beschwörer eine Kampfrunde lang mit allen ihm zur Verfügung stehenden Mitteln an – auch mit Angriffen von längerfristiger Auswirkung wie z. B. Besessenheit – und verschwindet dann. Alle weiteren Dienste verfallen. Wenn Sie mit den Experten-Regeln zum Schleichenden Verfall spielen, erhält der Beschwörer 3 Punkte Verfall.";
            }
            else if (wurf <= 17)
            {
                BeherrschungMisslungenErgebnis = "Der Dämon greift den Beschwörer W6+3 Kampfrunden lang mit allen ihm zur Verfügung stehenden Mitteln an; W6+3 Punkte Verfall";
            }
            else if (wurf <= 21)
            {
                BeherrschungMisslungenErgebnis = "Der Dämon greift den Beschwörer W6+3 Kampfrunden lang mit allen ihm zur Verfügung stehenden Mitteln an, jedoch raubt jeder erfolgreiche Angriff (auch z. B. ein Furcht Einflößen zählt in diesem Sinne als Angriff) des Dämons dem Beschwörer zusätzlich einen permanenten AsP (wenn keine mehr vorhanden, dann permanente LeP); W6+3 Punkte Verfall oder eine passende Schlechte Eigenschaft im Wert von 6 GP.";
            }
            else
            {
                BeherrschungMisslungenErgebnis = "Der Dämon greift den Beschwörer W6+3 Kampfrunden lang mit allen ihm zur Verfügung stehenden Mitteln an, jedoch raubt jeder erfolgreiche Angriff (auch z. B. ein Furcht Einflößen zählt in diesem Sinne als Angriff) des Dämons dem Beschwörer zusätzlich einen permanenten AsP (wenn keine mehr vorhanden, dann permanente LeP); W6+3 Punkte Verfall oder eine passende Schlechte Eigenschaft im Wert von 6 GP. Jedoch können nach Meisterentscheid und bösartiger Kreativität weitere Nebeneffekte eintreten. Als Beispiele seien hier genannt: Der Dämon lässt nicht eher vom Beschwörer ab, bis er ausgetrieben wird. Der Dämon wird freigesetzt und macht auf eigene Faust Aventurien unsicher. Der Dämon reißt den Beschwörer in die Niederhöllen. Der Dämon zieht sich in den näheren Limbus zurück und wartet dort auf die nächste Beschwörung, um dann zusätzlich zur gerufenen Entität zu erscheinen. Ein mächtiger Gehörnter zwingt dem Beschwörer einen minderen Pakt auf.";
            }
            //Command wird deaktiviert
            WürfleBeherrschungMisslungen = new Base.CommandBase((o) => { }, (o) => false);
        }

        public override BeschwörungsStatus Status
        {
            get
            {
                return base.Status;
            }
            protected set
            {
                base.Status = value;
                //Hier updaten wir die CanExecute-Eigenschaft der Commands
                if (WürfleBeschwörungMisslungen != null)
                    WürfleBeschwörungMisslungen.Invalidate();
                if (WürfleBeherrschungMisslungen != null)
                    WürfleBeherrschungMisslungen.Invalidate();
            }
        }

        #endregion

        #region Proben

        private Base.CommandBase magiekundeProbe;
        public Base.CommandBase MagiekundeProbe
        {
            get { return magiekundeProbe; }
        }

        private Base.CommandBase malenProbe;
        public Base.CommandBase MalenProbe
        {
            get { return malenProbe; }
        }

        private Base.CommandBase editMagiekunde;
        public Base.CommandBase EditMagiekunde
        {
            get { return editMagiekunde; }
            set { editMagiekunde = value; }
        }

        private Base.CommandBase editMalen;
        public Base.CommandBase EditMalen
        {
            get { return editMalen; }
            set { editMalen = value; }
        }


        private void magiekunde()
        {
            int taW;
            Held_Talent ht = Global.SelectedHeld.GetHeldTalent("Magiekunde", false, out taW);
            ht.Talent.Fertigkeitswert = taW;
            ht.Talent.Modifikator = div(schwierigkeit.Value1, 2);
            ProbenErgebnis erg = ht.Held.TalentProbe(ht.Talent, ht.Talent.Modifikator, "Dämonologie");
            invocatioMagiekunde.Value = erg.Gelungen ? erg.Übrig : 0;
        }
        private void malen()
        {
            int taW;
            Held_Talent ht = Global.SelectedHeld.GetHeldTalent("Malen/Zeichnen", false, out taW);
            ht.Talent.Fertigkeitswert = taW;
            ht.Talent.Modifikator = div(schwierigkeit.Value1, 2);
            ProbenErgebnis erg = ShowProbeDialog(ht.Talent, ht.Held);
            invocatioMalen.Value = erg.Gelungen ? erg.Übrig : 0;
        }

        #endregion

        #region Properties

        private const string MOD_BANNSCHWERT = "Bannschwert";
        private const string MOD_AFFINITÄT = "Affinität";
        private const string MOD_PAKTIERER = "Paktierer";
        private const string MOD_INVOCATIO_INTEGRA = "InvocatioIntegra";
        private const string MOD_INVOCATIO_INTEGRA_MALEN = "InvocatioIntegraMalen";
        private const string MOD_INVOCATIO_INTEGRA_MAGIEKUNDE = "InvocatioIntegraMagiekunde";

        private BeschwörungsModifikator<bool> bannschwert;
        private BeschwörungsModifikator<bool> affinität;
        private BeschwörungsModifikator<int> paktierer;
        private BeschwörungsModifikator<bool, bool> invocatioIntegra;
        private new BeschwörungsModifikator<bool, Opfer> blutmagie;
        private BeschwörungsModifikator<int> invocatioMalen;
        private BeschwörungsModifikator<int> invocatioMagiekunde;

        protected override void addMods()
        {
            //Bei der Dämonenbeschwörung entfällt die materielle Komponente
            Mods.Remove(MOD_MATERIAL);

            //Wenn ein anderer Dämon erscheint als der gerufene wird die Kontrollschwierigkeit verdoppelt
            schwierigkeit.GetKontrollMod = () => schwierigkeit.Value2 * (AndererDämon ? 2 : 1);

            //Ein Bannschwert erleichtert Anrufung und Kontrolle um 1 Punkt
            bannschwert = new BeschwörungsModifikator<bool>();
            bannschwert.GetAnrufungsMod = bannschwert.GetKontrollMod = () => bannschwert.Value ? -1 : 0;
            Mods.Add(MOD_BANNSCHWERT, bannschwert);

            //Affinität zu Dämonen erleichtert die Kontrolle um 3
            affinität = new BeschwörungsModifikator<bool>();
            affinität.GetKontrollMod = () => affinität.Value ? -3 : 0;
            Mods.Add(MOD_AFFINITÄT, affinität);

            //Ein Paktierer der passenden Domäne ist Anrufung und Kontrolle um seinen Kreis der Verdammnis erleichtert
            //Die Kontrolle ist zusätzlich um 3 erleichtert
            paktierer = new BeschwörungsModifikator<int>();
            paktierer.GetAnrufungsMod = () => -paktierer.Value;
            paktierer.GetKontrollMod = () => (paktierer.Value > 0) ? -paktierer.Value - 3 : 0;
            Mods.Add(MOD_PAKTIERER, paktierer);

            //Ohne Wahrer Name ist die Anrufung von Dämonen um 7 Punkte erschwert
            Func<int> defaultMod = name.GetAnrufungsMod;
            name.GetAnrufungsMod = () => (name.Value == 0) ? 7 : defaultMod();

            invocatioIntegra = new BeschwörungsModifikator<bool, bool>();
            //Invocatio Integra erhöht den effektiven ZfW um 7
            invocatioIntegra.GetZauberMod = () => invocatioIntegra.Value1 ? 7 : 0;
            Mods.Add(MOD_INVOCATIO_INTEGRA, invocatioIntegra);

            //Blutmagie erschwert die Kontrolle, und erhöht den ZfW wenn InvocatioIntegra aktiv ist
            blutmagie = new BeschwörungsModifikator<bool, Opfer>();
            blutmagie.GetKontrollMod = () => blutmagie.Value1 ? 2 : 0;
            blutmagie.GetZauberMod = () =>
            {
                if (!invocatioIntegra.Value1 || !blutmagie.Value1) return 0;
                else if (blutmagie.Value2 == Opfer.Tieropfer) return 3;
                else return 7;
            };
            Mods[MOD_BLUTMAGIE] = blutmagie;

            //Malen bringt Bonuspunkte auf die Anrufung wenn InvocatioIntegra mit vorbereitung aktiviert ist
            invocatioMalen = new BeschwörungsModifikator<int>();
            invocatioMalen.GetAnrufungsMod = () => invocatioProbeMod(invocatioMalen.Value);
            Mods.Add(MOD_INVOCATIO_INTEGRA_MALEN, invocatioMalen);

            //Magiekunde bringt Bonuspunkte auf die Anrufung wenn InvocatioIntegra mit vorbereitung aktiviert ist
            invocatioMagiekunde = new BeschwörungsModifikator<int>();
            invocatioMagiekunde.GetAnrufungsMod = () => invocatioProbeMod(invocatioMagiekunde.Value);
            Mods.Add(MOD_INVOCATIO_INTEGRA_MAGIEKUNDE, invocatioMagiekunde);

            //InvocatioIntegra beeinflusst sowohl Blutmagie, als auch die Malen-/Magiekundeprobe
            invocatioIntegra.PropertyChanged += (s, e) => { blutmagie.Invalidate(); invocatioMalen.Invalidate(); invocatioMagiekunde.Invalidate(); };

            //Wenn sich der Wahre Name auf 0 ändert kann Invocatio Integra nicht durchgeführt werden
            name.PropertyChanged += (s, e) => { if (name.Value == 0) invocatioIntegra.Value1 = false; };
        }

        private int invocatioProbeMod(int value)
        {
            if (invocatioIntegra.Value1 && invocatioIntegra.Value2)
            {
                if (value == 0) return 3;
                else return -div(value, 2);
            }
            else return 0;
        }

        private bool andererDämon = false;
        public bool AndererDämon
        {
            get { return andererDämon; }
            set
            {
                Set(ref andererDämon, value);
                schwierigkeit.Invalidate();
            }
        }

        private int hörner = 0;
        public int Hörner
        {
            get { return hörner; }
            set
            {
                Set(ref hörner, value);
                //Invocatio Integra ist nur bei gehörnten Dämonen möglich
                if (hörner == 0)
                    invocatioIntegra.Value1 = false;
                //Der Zauber für die Beschwörung ist bei gehörnten Dämonen ein anderer
                Zauber = Hörner == 0 ? "Invocatio minor" : "Invocatio maior";
            }
        }

        private string beschwörungMisslungenErgebnis;
        public string BeschwörungMisslungenErgebnis
        {
            get { return beschwörungMisslungenErgebnis; }
            set { Set(ref beschwörungMisslungenErgebnis, value); }
        }

        private string beherrschungMisslungenErgebnis;
        public string BeherrschungMisslungenErgebnis
        {
            get { return beherrschungMisslungenErgebnis; }
            set { Set(ref beherrschungMisslungenErgebnis, value); }
        }

        public override string KontrollFormel
        {
            get { return "(MU + MU + KL + CH + ZfW) / 5"; }
        }

        protected override int calcKontrollWert()
        {
            return div(Held.Mut * 2 + Held.Klugheit + Held.Charisma + ZauberWert, 5);
        }

        #endregion
    }
    public enum Opfer
    {
        [Description("Tieropfer")]
        Tieropfer = 0,
        [Description("Opferung eines intelligenten Wesens")]
        IntelligentesWesen
    }
}
