//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace MeisterGeister.Model
{
    [DataContract(IsReference=true)]
    public partial class Talent : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
    	public event PropertyChangedEventHandler PropertyChanged;
    	
    	public void OnChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

        #region Primitive Properties
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Talentname
        {
            get { return _talentname; }
            set
    		{ 
    			_talentname = value;
    			OnChanged("Talentname");
    		}
    
        }
        private string _talentname;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual Nullable<int> TalentgruppeID
        {
            get { return _talentgruppeID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_talentgruppeID != value)
                    {
                        if (Talentgruppe != null && Talentgruppe.TalentgruppeID != value)
                        {
                            Talentgruppe = null;
                        }
                        _talentgruppeID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
    
        }
        private Nullable<int> _talentgruppeID;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Eigenschaft1
        {
            get { return _eigenschaft1; }
            set
    		{ 
    			_eigenschaft1 = value;
    			OnChanged("Eigenschaft1");
    		}
    
        }
        private string _eigenschaft1;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Eigenschaft2
        {
            get { return _eigenschaft2; }
            set
    		{ 
    			_eigenschaft2 = value;
    			OnChanged("Eigenschaft2");
    		}
    
        }
        private string _eigenschaft2;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Eigenschaft3
        {
            get { return _eigenschaft3; }
            set
    		{ 
    			_eigenschaft3 = value;
    			OnChanged("Eigenschaft3");
    		}
    
        }
        private string _eigenschaft3;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Talenttyp
        {
            get { return _talenttyp; }
            set
    		{ 
    			_talenttyp = value;
    			OnChanged("Talenttyp");
    		}
    
        }
        private string _talenttyp;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string eBE
        {
            get { return _eBE; }
            set
    		{ 
    			_eBE = value;
    			OnChanged("eBE");
    		}
    
        }
        private string _eBE;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Spezialisierungen
        {
            get { return _spezialisierungen; }
            set
    		{ 
    			_spezialisierungen = value;
    			OnChanged("Spezialisierungen");
    		}
    
        }
        private string _spezialisierungen;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Voraussetzungen
        {
            get { return _voraussetzungen; }
            set
    		{ 
    			_voraussetzungen = value;
    			OnChanged("Voraussetzungen");
    		}
    
        }
        private string _voraussetzungen;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Steigerung
        {
            get { return _steigerung; }
            set
    		{ 
    			_steigerung = value;
    			OnChanged("Steigerung");
    		}
    
        }
        private string _steigerung;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string WikiLink
        {
            get { return _wikiLink; }
            set
    		{ 
    			_wikiLink = value;
    			OnChanged("WikiLink");
    		}
    
        }
        private string _wikiLink;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Untergruppe
        {
            get { return _untergruppe; }
            set
    		{ 
    			_untergruppe = value;
    			OnChanged("Untergruppe");
    		}
    
        }
        private string _untergruppe;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Setting
        {
            get { return _setting; }
            set
    		{ 
    			_setting = value;
    			OnChanged("Setting");
    		}
    
        }
        private string _setting;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Literatur
        {
            get { return _literatur; }
            set
    		{ 
    			_literatur = value;
    			OnChanged("Literatur");
    		}
    
        }
        private string _literatur;

        #endregion

        #region Navigation Properties
    
    	[DataMember]
        public virtual ICollection<Held_Ausrüstung> Held_Ausrüstung
        {
            get
            {
                if (_held_Ausrüstung == null)
                {
                    var newCollection = new FixupCollection<Held_Ausrüstung>();
                    newCollection.CollectionChanged += FixupHeld_Ausrüstung;
                    _held_Ausrüstung = newCollection;
                }
                return _held_Ausrüstung;
            }
            set
            {
                if (!ReferenceEquals(_held_Ausrüstung, value))
                {
                    var previousValue = _held_Ausrüstung as FixupCollection<Held_Ausrüstung>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupHeld_Ausrüstung;
                    }
                    _held_Ausrüstung = value;
                    var newValue = value as FixupCollection<Held_Ausrüstung>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupHeld_Ausrüstung;
                    }
                }
            }
        }
        private ICollection<Held_Ausrüstung> _held_Ausrüstung;
    
    	[DataMember]
        public virtual ICollection<Held_Talent> Held_Talent
        {
            get
            {
                if (_held_Talent == null)
                {
                    var newCollection = new FixupCollection<Held_Talent>();
                    newCollection.CollectionChanged += FixupHeld_Talent;
                    _held_Talent = newCollection;
                }
                return _held_Talent;
            }
            set
            {
                if (!ReferenceEquals(_held_Talent, value))
                {
                    var previousValue = _held_Talent as FixupCollection<Held_Talent>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupHeld_Talent;
                    }
                    _held_Talent = value;
                    var newValue = value as FixupCollection<Held_Talent>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupHeld_Talent;
                    }
                }
            }
        }
        private ICollection<Held_Talent> _held_Talent;
    
    	[DataMember]
        public virtual Talentgruppe Talentgruppe
        {
            get { return _talentgruppe; }
            set
            {
                if (!ReferenceEquals(_talentgruppe, value))
                {
                    var previousValue = _talentgruppe;
                    _talentgruppe = value;
                    FixupTalentgruppe(previousValue);
                }
            }
        }
        private Talentgruppe _talentgruppe;
    
    	[DataMember]
        public virtual ICollection<Fernkampfwaffe> Fernkampfwaffe
        {
            get
            {
                if (_fernkampfwaffe == null)
                {
                    var newCollection = new FixupCollection<Fernkampfwaffe>();
                    newCollection.CollectionChanged += FixupFernkampfwaffe;
                    _fernkampfwaffe = newCollection;
                }
                return _fernkampfwaffe;
            }
            set
            {
                if (!ReferenceEquals(_fernkampfwaffe, value))
                {
                    var previousValue = _fernkampfwaffe as FixupCollection<Fernkampfwaffe>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupFernkampfwaffe;
                    }
                    _fernkampfwaffe = value;
                    var newValue = value as FixupCollection<Fernkampfwaffe>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupFernkampfwaffe;
                    }
                }
            }
        }
        private ICollection<Fernkampfwaffe> _fernkampfwaffe;
    
    	[DataMember]
        public virtual ICollection<Waffe> Waffe
        {
            get
            {
                if (_waffe == null)
                {
                    var newCollection = new FixupCollection<Waffe>();
                    newCollection.CollectionChanged += FixupWaffe;
                    _waffe = newCollection;
                }
                return _waffe;
            }
            set
            {
                if (!ReferenceEquals(_waffe, value))
                {
                    var previousValue = _waffe as FixupCollection<Waffe>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupWaffe;
                    }
                    _waffe = value;
                    var newValue = value as FixupCollection<Waffe>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupWaffe;
                    }
                }
            }
        }
        private ICollection<Waffe> _waffe;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupTalentgruppe(Talentgruppe previousValue)
        {
    		OnChanged("Talentgruppe");
            if (previousValue != null && previousValue.Talent.Contains(this))
            {
                previousValue.Talent.Remove(this);
            }
    
            if (Talentgruppe != null)
            {
                if (!Talentgruppe.Talent.Contains(this))
                {
                    Talentgruppe.Talent.Add(this);
                }
                if (TalentgruppeID != Talentgruppe.TalentgruppeID)
                {
                    TalentgruppeID = Talentgruppe.TalentgruppeID;
                }
            }
            else if (!_settingFK)
            {
                TalentgruppeID = null;
            }
        }
    
        private void FixupHeld_Ausrüstung(object sender, NotifyCollectionChangedEventArgs e)
        {
    		OnChanged("Held_Ausrüstung");
            if (e.NewItems != null)
            {
                foreach (Held_Ausrüstung item in e.NewItems)
                {
                    item.Talent = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Held_Ausrüstung item in e.OldItems)
                {
                    if (ReferenceEquals(item.Talent, this))
                    {
                        item.Talent = null;
                    }
                }
            }
        }
    
        private void FixupHeld_Talent(object sender, NotifyCollectionChangedEventArgs e)
        {
    		OnChanged("Held_Talent");
            if (e.NewItems != null)
            {
                foreach (Held_Talent item in e.NewItems)
                {
                    item.Talent = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Held_Talent item in e.OldItems)
                {
                    if (ReferenceEquals(item.Talent, this))
                    {
                        item.Talent = null;
                    }
                }
            }
        }
    
        private void FixupFernkampfwaffe(object sender, NotifyCollectionChangedEventArgs e)
        {
    		OnChanged("Fernkampfwaffe");
            if (e.NewItems != null)
            {
                foreach (Fernkampfwaffe item in e.NewItems)
                {
                    if (!item.Talent.Contains(this))
                    {
                        item.Talent.Add(this);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Fernkampfwaffe item in e.OldItems)
                {
                    if (item.Talent.Contains(this))
                    {
                        item.Talent.Remove(this);
                    }
                }
            }
        }
    
        private void FixupWaffe(object sender, NotifyCollectionChangedEventArgs e)
        {
    		OnChanged("Waffe");
            if (e.NewItems != null)
            {
                foreach (Waffe item in e.NewItems)
                {
                    if (!item.Talent.Contains(this))
                    {
                        item.Talent.Add(this);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Waffe item in e.OldItems)
                {
                    if (item.Talent.Contains(this))
                    {
                        item.Talent.Remove(this);
                    }
                }
            }
        }

        #endregion

    }
}
