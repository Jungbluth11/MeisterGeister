﻿using MeisterGeister.Logic.General;
using MeisterGeister.Model;
using MeisterGeister.View.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeisterGeister.ViewModel.Kampf.Logic.Manöver
{
    public abstract class AbwehrManöver : NahkampfManöver
    {
        public AbwehrManöver(KämpferInfo ausführender)
            : base(ausführender)
        { }

        #region Mods

        public const string GLÜCKLICH_MOD = "Glücklich";
        public const string LINKSHÄNDER_MOD = "Linkshänder";
        public const string FINTE_MOD = "Finte";

        protected NahkampfModifikator<bool> linkshänder;
        protected NahkampfModifikator<bool> glücklich;
        protected NahkampfModifikator<int> finte;

        protected override void InitMods()
        {
            base.InitMods();

            linkshänder = new NahkampfModifikator<bool>(this);
            linkshänder.GetMod = LinkshänderMod;
            mods.Add(LINKSHÄNDER_MOD, linkshänder);

            glücklich = new NahkampfModifikator<bool>(this);
            glücklich.GetMod = GlücklichMod;
            mods.Add(GLÜCKLICH_MOD, glücklich);

            finte = new NahkampfModifikator<int>(this);
            mods.Add(FINTE_MOD, finte);
        }

        private int GlücklichMod(INahkampfwaffe waffe, bool value)
        {
            if (value)
            {
                return (int)Math.Round((Ausführender.Kämpfer.PA ?? 0) / 2.0, MidpointRounding.AwayFromZero);
            }
            return 0;
        }

        private int LinkshänderMod(INahkampfwaffe waffe, bool value)
        {
            return value ? 1 : 0;
        }

        protected override int WasserMod(INahkampfwaffe waffe, Wassertiefe value)
        {
            int mod;
            switch (value)
            {
                case Wassertiefe.Knietief:
                    mod = 2;
                    break;
                case Wassertiefe.Hüfttief:
                    mod = 4;
                    break;
                case Wassertiefe.Schultertief:
                case Wassertiefe.UnterWasser:
                    mod = 6;
                    break;
                default:
                    mod = 0;
                    break;
            }
            return CheckWasserkampf(mod, value, Ausführender.Kämpfer as Held);
        }

        protected override int BeengtMod(INahkampfwaffe waffe, bool value)
        {
            if (value)
            {
                //Anderthalbhänder, Kettenwaffen, Peitschen, Stäbe, Zweihandflegel, Zweihandhiebwaffen und Zweihandschwerter/-säbel, Infanteriewaffen, Speer
                //return 2;
            }
            return 0;
        }

        protected override int PositionGegnerMod(INahkampfwaffe waffe, Position value)
        {
            switch (value)
            {
                case Position.Liegend:
                    return -5;
                case Position.Kniend:
                    return -3;
                case Position.Fliegend:
                    return 4;
                default:
                    return 0;
            }
        }

        protected override int ÜberzahlMod(INahkampfwaffe waffe, int value)
        {
            return Math.Min(2, Math.Max(0, -value));
        }

        protected override int UnbewaffnetMod(INahkampfwaffe waffe, bool value)
        {
            return value ? 2 : 0;
        }

        #endregion

        //TODO JT: In der Probe muss hier ein Paradeerleichterungsmodifikator abgefragt werden. Ebenso Paradeerschwernisse durch Finten oder Linkshändig.
        //Gezielte Schläge sind einfacher zu parieren. (Modifikator mit dem angreifenden Manöver gespeichert zur Verifikation)

        public override void Ausführen()
        {
            base.Ausführen();

            foreach (var wz in WaffeZiel)
            {
                Probe p = new Probe();
                p.Probenname = Name;
                p.Werte = new int[] { Ausführender.Kämpfer.PA ?? 0 };
                p.WerteNamen = "PA";
                ViewHelper.ShowProbeDialog(p, Ausführender.Kämpfer as Held);

                ProbeAuswerten(p, wz.Value);
            }
        }

        protected override void Init()
        {
            base.Init();
            Abwehraktionen = 1;
            Typ = ManöverTyp.Reaktion;
        }

        //TODO JT: Wenn AusdauerImKampf
        //Wenn Waffe schwerer als KK*10 Unzen
        // Ausführender.AusdauerAktuell--;
        //Wenn BE / 3 > 0
        // Ausführender.AusdauerAktuell-= BE/3;
    }
}
