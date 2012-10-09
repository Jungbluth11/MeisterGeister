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
    public partial class GegnerBase : INotifyPropertyChanged
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
        public virtual System.Guid GegnerBaseGUID
        {
            get { return _gegnerBaseGUID; }
            set
    		{ 
    			_gegnerBaseGUID = value;
    			OnChanged("GegnerBaseGUID");
    		}
    
        }
        private System.Guid _gegnerBaseGUID;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Name
        {
            get { return _name; }
            set
    		{ 
    			_name = value;
    			OnChanged("Name");
    		}
    
        }
        private string _name;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Bild
        {
            get { return _bild; }
            set
    		{ 
    			_bild = value;
    			OnChanged("Bild");
    		}
    
        }
        private string _bild;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int INIBasis
        {
            get { return _iNIBasis; }
            set
    		{ 
    			_iNIBasis = value;
    			OnChanged("INIBasis");
    		}
    
        }
        private int _iNIBasis;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string INIZufall
        {
            get { return _iNIZufall; }
            set
    		{ 
    			_iNIZufall = value;
    			OnChanged("INIZufall");
    		}
    
        }
        private string _iNIZufall;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int Aktionen
        {
            get { return _aktionen; }
            set
    		{ 
    			_aktionen = value;
    			OnChanged("Aktionen");
    		}
    
        }
        private int _aktionen;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int PA
        {
            get { return _pA; }
            set
    		{ 
    			_pA = value;
    			OnChanged("PA");
    		}
    
        }
        private int _pA;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int LE
        {
            get { return _lE; }
            set
    		{ 
    			_lE = value;
    			OnChanged("LE");
    		}
    
        }
        private int _lE;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int AU
        {
            get { return _aU; }
            set
    		{ 
    			_aU = value;
    			OnChanged("AU");
    		}
    
        }
        private int _aU;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int AE
        {
            get { return _aE; }
            set
    		{ 
    			_aE = value;
    			OnChanged("AE");
    		}
    
        }
        private int _aE;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int KE
        {
            get { return _kE; }
            set
    		{ 
    			_kE = value;
    			OnChanged("KE");
    		}
    
        }
        private int _kE;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int KO
        {
            get { return _kO; }
            set
    		{ 
    			_kO = value;
    			OnChanged("KO");
    		}
    
        }
        private int _kO;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int MRGeist
        {
            get { return _mRGeist; }
            set
    		{ 
    			_mRGeist = value;
    			OnChanged("MRGeist");
    		}
    
        }
        private int _mRGeist;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual Nullable<int> MRKörper
        {
            get { return _mRKörper; }
            set
    		{ 
    			_mRKörper = value;
    			OnChanged("MRKörper");
    		}
    
        }
        private Nullable<int> _mRKörper;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int GS
        {
            get { return _gS; }
            set
    		{ 
    			_gS = value;
    			OnChanged("GS");
    		}
    
        }
        private int _gS;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual Nullable<int> GS2
        {
            get { return _gS2; }
            set
    		{ 
    			_gS2 = value;
    			OnChanged("GS2");
    		}
    
        }
        private Nullable<int> _gS2;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual Nullable<int> GS3
        {
            get { return _gS3; }
            set
    		{ 
    			_gS3 = value;
    			OnChanged("GS3");
    		}
    
        }
        private Nullable<int> _gS3;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int RSKopf
        {
            get { return _rSKopf; }
            set
    		{ 
    			_rSKopf = value;
    			OnChanged("RSKopf");
    		}
    
        }
        private int _rSKopf;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int RSBrust
        {
            get { return _rSBrust; }
            set
    		{ 
    			_rSBrust = value;
    			OnChanged("RSBrust");
    		}
    
        }
        private int _rSBrust;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int RSRücken
        {
            get { return _rSRücken; }
            set
    		{ 
    			_rSRücken = value;
    			OnChanged("RSRücken");
    		}
    
        }
        private int _rSRücken;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int RSArmL
        {
            get { return _rSArmL; }
            set
    		{ 
    			_rSArmL = value;
    			OnChanged("RSArmL");
    		}
    
        }
        private int _rSArmL;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int RSArmR
        {
            get { return _rSArmR; }
            set
    		{ 
    			_rSArmR = value;
    			OnChanged("RSArmR");
    		}
    
        }
        private int _rSArmR;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int RSBauch
        {
            get { return _rSBauch; }
            set
    		{ 
    			_rSBauch = value;
    			OnChanged("RSBauch");
    		}
    
        }
        private int _rSBauch;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int RSBeinL
        {
            get { return _rSBeinL; }
            set
    		{ 
    			_rSBeinL = value;
    			OnChanged("RSBeinL");
    		}
    
        }
        private int _rSBeinL;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int RSBeinR
        {
            get { return _rSBeinR; }
            set
    		{ 
    			_rSBeinR = value;
    			OnChanged("RSBeinR");
    		}
    
        }
        private int _rSBeinR;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual Nullable<int> GW
        {
            get { return _gW; }
            set
    		{ 
    			_gW = value;
    			OnChanged("GW");
    		}
    
        }
        private Nullable<int> _gW;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual Nullable<int> Jagd
        {
            get { return _jagd; }
            set
    		{ 
    			_jagd = value;
    			OnChanged("Jagd");
    		}
    
        }
        private Nullable<int> _jagd;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual Nullable<int> Beschwörung
        {
            get { return _beschwörung; }
            set
    		{ 
    			_beschwörung = value;
    			OnChanged("Beschwörung");
    		}
    
        }
        private Nullable<int> _beschwörung;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual Nullable<int> Kontrolle
        {
            get { return _kontrolle; }
            set
    		{ 
    			_kontrolle = value;
    			OnChanged("Kontrolle");
    		}
    
        }
        private Nullable<int> _kontrolle;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual Nullable<int> Beschwörungskosten
        {
            get { return _beschwörungskosten; }
            set
    		{ 
    			_beschwörungskosten = value;
    			OnChanged("Beschwörungskosten");
    		}
    
        }
        private Nullable<int> _beschwörungskosten;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Tags
        {
            get { return _tags; }
            set
    		{ 
    			_tags = value;
    			OnChanged("Tags");
    		}
    
        }
        private string _tags;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Bemerkung
        {
            get { return _bemerkung; }
            set
    		{ 
    			_bemerkung = value;
    			OnChanged("Bemerkung");
    		}
    
        }
        private string _bemerkung;
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

        #endregion

        #region Navigation Properties
    
    	[DataMember]
        public virtual ICollection<Gegner> Gegner
        {
            get
            {
                if (_gegner == null)
                {
                    var newCollection = new FixupCollection<Gegner>();
                    newCollection.CollectionChanged += FixupGegner;
                    _gegner = newCollection;
                }
                return _gegner;
            }
            set
            {
                if (!ReferenceEquals(_gegner, value))
                {
                    var previousValue = _gegner as FixupCollection<Gegner>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupGegner;
                    }
                    _gegner = value;
                    var newValue = value as FixupCollection<Gegner>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupGegner;
                    }
                }
            }
        }
        private ICollection<Gegner> _gegner;
    
    	[DataMember]
        public virtual ICollection<GegnerBase_Angriff> GegnerBase_Angriff
        {
            get
            {
                if (_gegnerBase_Angriff == null)
                {
                    var newCollection = new FixupCollection<GegnerBase_Angriff>();
                    newCollection.CollectionChanged += FixupGegnerBase_Angriff;
                    _gegnerBase_Angriff = newCollection;
                }
                return _gegnerBase_Angriff;
            }
            set
            {
                if (!ReferenceEquals(_gegnerBase_Angriff, value))
                {
                    var previousValue = _gegnerBase_Angriff as FixupCollection<GegnerBase_Angriff>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupGegnerBase_Angriff;
                    }
                    _gegnerBase_Angriff = value;
                    var newValue = value as FixupCollection<GegnerBase_Angriff>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupGegnerBase_Angriff;
                    }
                }
            }
        }
        private ICollection<GegnerBase_Angriff> _gegnerBase_Angriff;
    
    	[DataMember]
        public virtual ICollection<GegnerBase_Kampfregel> GegnerBase_Kampfregel
        {
            get
            {
                if (_gegnerBase_Kampfregel == null)
                {
                    var newCollection = new FixupCollection<GegnerBase_Kampfregel>();
                    newCollection.CollectionChanged += FixupGegnerBase_Kampfregel;
                    _gegnerBase_Kampfregel = newCollection;
                }
                return _gegnerBase_Kampfregel;
            }
            set
            {
                if (!ReferenceEquals(_gegnerBase_Kampfregel, value))
                {
                    var previousValue = _gegnerBase_Kampfregel as FixupCollection<GegnerBase_Kampfregel>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupGegnerBase_Kampfregel;
                    }
                    _gegnerBase_Kampfregel = value;
                    var newValue = value as FixupCollection<GegnerBase_Kampfregel>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupGegnerBase_Kampfregel;
                    }
                }
            }
        }
        private ICollection<GegnerBase_Kampfregel> _gegnerBase_Kampfregel;

        #endregion

        #region Association Fixup
    
        private void FixupGegner(object sender, NotifyCollectionChangedEventArgs e)
        {
    		OnChanged("Gegner");
            if (e.NewItems != null)
            {
                foreach (Gegner item in e.NewItems)
                {
                    item.GegnerBase = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Gegner item in e.OldItems)
                {
                    if (ReferenceEquals(item.GegnerBase, this))
                    {
                        item.GegnerBase = null;
                    }
                }
            }
        }
    
        private void FixupGegnerBase_Angriff(object sender, NotifyCollectionChangedEventArgs e)
        {
    		OnChanged("GegnerBase_Angriff");
            if (e.NewItems != null)
            {
                foreach (GegnerBase_Angriff item in e.NewItems)
                {
                    item.GegnerBase = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (GegnerBase_Angriff item in e.OldItems)
                {
                    if (ReferenceEquals(item.GegnerBase, this))
                    {
                        item.GegnerBase = null;
                    }
                }
            }
        }
    
        private void FixupGegnerBase_Kampfregel(object sender, NotifyCollectionChangedEventArgs e)
        {
    		OnChanged("GegnerBase_Kampfregel");
            if (e.NewItems != null)
            {
                foreach (GegnerBase_Kampfregel item in e.NewItems)
                {
                    item.GegnerBase = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (GegnerBase_Kampfregel item in e.OldItems)
                {
                    if (ReferenceEquals(item.GegnerBase, this))
                    {
                        item.GegnerBase = null;
                    }
                }
            }
        }

        #endregion

    }
}
