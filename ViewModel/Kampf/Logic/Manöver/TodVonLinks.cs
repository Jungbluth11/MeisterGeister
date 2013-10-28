﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MeisterGeister.ViewModel.Kampf.Logic.Manöver
{
    public class TodVonLinks : Angriffsaktion
    {
        public TodVonLinks(KämpferInfo ausführender)
            : base(ausführender)
        { }

        public TodVonLinks(KämpferInfo ausführender, IWaffe waffe, KämpferInfo ziel)
            : base(ausführender, waffe, ziel)
        {
        }

        public override String Name
        {
            get { return "Tod von Links"; }
        }

        public static new bool BeherrschtManöver(KämpferInfo ausführender)
        {
            if (ausführender == null)
                return false;
            if (ausführender.Kämpfer is Model.Held)
                return ((Model.Held)ausführender.Kämpfer).HatSonderfertigkeitUndVoraussetzungen("Tod von Links", true);
            else //TODO evtl check auf Kampfregel
                return false;
        }
    }
}
