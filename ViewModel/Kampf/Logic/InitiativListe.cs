﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using MeisterGeister.Logic.General;

namespace MeisterGeister.ViewModel.Kampf.Logic
{
    public class InitiativListe : ObservableCollection<ManöverInfo>, INotifyPropertyChanged//, INotifyCollectionChanged
    {
        public InitiativListe(Kampf kampf)
        {
            _kampf = kampf;
            CollectionChanged += InitiativListe_CollectionChanged;
        }

        private void InitiativListe_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //TODO: Optimieren. Nur die IndexChanged-Events in Gang setzen, die sich auch wirklich geändert haben
            foreach (ManöverInfo info in this)
                info.OnChanged("Index");
        }

        public ManöverInfo[] this[IKämpfer k]
        {
            get
            {
                return this.Where(mi => mi.KämpferInfo.Kämpfer == k).ToArray();
            }
        }

        private Kampf _kampf;
        public Kampf Kampf
        {
            get { return _kampf; }
            set { _kampf = value; }
        }

        public void LöscheBeendeteManöver()
        {
            RemoveAll(mi => mi.Manöver.VerbleibendeDauer == 0);
            //Längerfristige Aktionen hier 1-2x neu einstellen.
            var l = this.Distinct(new KeyEqualityComparer<ManöverInfo, Manöver.Manöver>(m => m.Manöver)).ToList();
            Clear();
            foreach (var mi in l)
            {
                Add(mi.KämpferInfo, mi.Manöver, 0);
                if (mi.Manöver.VerbleibendeDauer >= 2)
                    Add(mi.KämpferInfo, mi.Manöver, -8);
            }
            l = null;
        }

        #region Add and Remove
        public new void Add(ManöverInfo mi)
        {
            base.Add(mi);
            mi.PropertyChanged += OnManöverInfoChanged;
            Sort();
        }

        public void Add(Manöver.Manöver m, int inimod)
        {
            Add(m.Ausführender, m, inimod);
        }

        public void Add(KämpferInfo ki, Manöver.Manöver m, int inimod)
        {
            Add(new ManöverInfo(ki, m, inimod, Kampf.Kampfrunde));
        }

        public new void Remove(ManöverInfo mi)
        {
            mi.PropertyChanged -= OnManöverInfoChanged;
            base.Remove(mi);
        }

        public new void RemoveAt(int index)
        {
            var mi = this[index];
            this[index].PropertyChanged -= OnManöverInfoChanged;
            base.RemoveAt(index);
        }

        public void Remove(KämpferInfo ki)
        {
            Remove(ki.Kämpfer);
        }

        public void Remove(IKämpfer k)
        {
            foreach (ManöverInfo mi in this[k])
            {
                mi.PropertyChanged -= OnManöverInfoChanged;
                base.Remove(mi);
            }
        }

        public void RemoveAll(Predicate<ManöverInfo> match)
        {
            foreach (ManöverInfo mi in Items.Where(mi => match(mi)).ToList())
            {
                Remove(mi);
            }
        }

        public new void Clear()
        {
            foreach (ManöverInfo mi in Items)
            {
                mi.PropertyChanged -= OnManöverInfoChanged;
            }
            base.Clear();
        }
        #endregion

        private void OnManöverInfoChanged(object o, System.ComponentModel.PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Initiative")
                Sort();
            else if (args.PropertyName == "Manöver")
            {
                var mi = o as ManöverInfo;
                if (mi == null)
                    return;
                //TODO JT: wenn das manöver eine längerfristige Aktion ist oder eine dauer > 1 hat oder mehr als nur eine Angriffsaktion verbraucht,
                if (mi.KämpferInfo != null && ( mi.Manöver is Manöver.LängerfristigeHandlung || mi.Manöver.VerbleibendeDauer > 1 || mi.Manöver.Angriffsaktionen > 1))
                {
                    // dann  werden alle nachfolgenden manöverinfos gelöscht
                    RemoveAll(i => i.KämpferInfo != null && i.KämpferInfo == mi.KämpferInfo && i.InitiativeStart < mi.InitiativeStart);

                    //Danach Aktionen berechnen und StandardaktionenSetzen
                    mi.KämpferInfo.AktionenBerechnen();
                }
            }
        }

        public void Sort()
        {
            var l = Items.ToList();
            l.Sort(CompareInitiative);
            foreach (var item in l)
            {
                int i1 = IndexOf(item), i2 = l.IndexOf(item);
                if(i1!=i2)
                    Move(i1, i2);
            }
            OnChanged("Sort");
        }

        /// <summary>
        /// Höhere Initiative nach oben.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int CompareInitiative(ManöverInfo x, ManöverInfo y)
        {
            // prüfen auf null-Übergabe
            if (x == null && y == null) return 0;
            if (x == null) return 1;
            if (y == null) return -1;
            // Vergleich
            if (x.InitiativeStart > y.InitiativeStart)
                return -1;
            if (x.InitiativeStart < y.InitiativeStart)
                return 1;
            if (x.KämpferInfo.InitiativeBasis > y.KämpferInfo.InitiativeBasis)
                return -1;
            if (x.KämpferInfo.InitiativeBasis < y.KämpferInfo.InitiativeBasis)
                return 1;
            return x.KämpferInfo.Kämpfer.Name.CompareTo(y.KämpferInfo.Kämpfer.Name);
        }

        #region INotifyPropertyChanged
        public new event PropertyChangedEventHandler PropertyChanged;

        public void OnChanged(String info, object source = null)
        {
            if (PropertyChanged != null)
            {
                if (source == null)
                    source = this;
                PropertyChanged(source, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}
