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
using System.Runtime.CompilerServices;

namespace MeisterGeister.Model
{
    [DataContract(IsReference=true)]
    public partial class Gegner : INotifyPropertyChanged
    {
        #region ValidatePropertyChanging
    	protected event Extensions.ValidatePropertyChangingEventHandler ValidatePropertyChanging;
    
    	protected void OnValidatePropertyChanging(object currentValue, object newValue, [CallerMemberName] string propertyName = null)
    	{
    		if(ValidatePropertyChanging != null)
    		{
    			ValidatePropertyChanging(this, propertyName, currentValue, newValue);
    		}
    	}

        #endregion

        #region Primitive Properties
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual System.Guid GegnerGUID
        {
            get { return _gegnerGUID; }
            set
    		{ 
    			Set(ref _gegnerGUID, value);
    		}
    
        }
        private System.Guid _gegnerGUID;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual System.Guid GegnerBaseGUID
        {
            get { return _gegnerBaseGUID; }
            set
            {
                if (_gegnerBaseGUID != value)
                {
                    if (GegnerBase != null && GegnerBase.GegnerBaseGUID != value)
                    {
                        GegnerBase = null;
                    }
                    _gegnerBaseGUID = value;
                }
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
    			Set(ref _name, value);
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
    			Set(ref _bild, value);
    		}
    
        }
        private string _bild;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int ATMod
        {
            get { return _aTMod; }
            set
    		{ 
    			Set(ref _aTMod, value);
    		}
    
        }
        private int _aTMod;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int INIBasis
        {
            get { return _iNIBasis; }
            set
    		{ 
    			Set(ref _iNIBasis, value);
    		}
    
        }
        private int _iNIBasis;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int PA
        {
            get { return _pA; }
            set
    		{ 
    			Set(ref _pA, value);
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
    			Set(ref _lE, value);
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
    			Set(ref _aU, value);
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
    			Set(ref _aE, value);
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
    			Set(ref _kE, value);
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
    			Set(ref _kO, value);
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
    			Set(ref _mRGeist, value);
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
    			Set(ref _mRKörper, value);
    		}
    
        }
        private Nullable<int> _mRKörper;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int RSKopf
        {
            get { return _rSKopf; }
            set
    		{ 
    			Set(ref _rSKopf, value);
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
    			Set(ref _rSBrust, value);
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
    			Set(ref _rSRücken, value);
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
    			Set(ref _rSArmL, value);
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
    			Set(ref _rSArmR, value);
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
    			Set(ref _rSBauch, value);
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
    			Set(ref _rSBeinL, value);
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
    			Set(ref _rSBeinR, value);
    		}
    
        }
        private int _rSBeinR;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int LEAktuell
        {
            get { return _lEAktuell; }
            set
    		{ 
    			Set(ref _lEAktuell, value);
    		}
    
        }
        private int _lEAktuell;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int AUAktuell
        {
            get { return _aUAktuell; }
            set
    		{ 
    			Set(ref _aUAktuell, value);
    		}
    
        }
        private int _aUAktuell;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int AEAktuell
        {
            get { return _aEAktuell; }
            set
    		{ 
    			Set(ref _aEAktuell, value);
    		}
    
        }
        private int _aEAktuell;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int KEAktuell
        {
            get { return _kEAktuell; }
            set
    		{ 
    			Set(ref _kEAktuell, value);
    		}
    
        }
        private int _kEAktuell;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int Wunden
        {
            get { return _wunden; }
            set
    		{ 
    			Set(ref _wunden, value);
    		}
    
        }
        private int _wunden;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int WundenKopf
        {
            get { return _wundenKopf; }
            set
    		{ 
    			Set(ref _wundenKopf, value);
    		}
    
        }
        private int _wundenKopf;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int WundenBrust
        {
            get { return _wundenBrust; }
            set
    		{ 
    			Set(ref _wundenBrust, value);
    		}
    
        }
        private int _wundenBrust;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int WundenArmL
        {
            get { return _wundenArmL; }
            set
    		{ 
    			Set(ref _wundenArmL, value);
    		}
    
        }
        private int _wundenArmL;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int WundenArmR
        {
            get { return _wundenArmR; }
            set
    		{ 
    			Set(ref _wundenArmR, value);
    		}
    
        }
        private int _wundenArmR;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int WundenBauch
        {
            get { return _wundenBauch; }
            set
    		{ 
    			Set(ref _wundenBauch, value);
    		}
    
        }
        private int _wundenBauch;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int WundenBeinL
        {
            get { return _wundenBeinL; }
            set
    		{ 
    			Set(ref _wundenBeinL, value);
    		}
    
        }
        private int _wundenBeinL;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int WundenBeinR
        {
            get { return _wundenBeinR; }
            set
    		{ 
    			Set(ref _wundenBeinR, value);
    		}
    
        }
        private int _wundenBeinR;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Bemerkung
        {
            get { return _bemerkung; }
            set
    		{ 
    			Set(ref _bemerkung, value);
    		}
    
        }
        private string _bemerkung;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int Aktionen
        {
            get { return _aktionen; }
            set
    		{ 
    			Set(ref _aktionen, value);
    		}
    
        }
        private int _aktionen;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual double GS
        {
            get { return _gS; }
            set
    		{ 
    			Set(ref _gS, value);
    		}
    
        }
        private double _gS;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual Nullable<double> GS2
        {
            get { return _gS2; }
            set
    		{ 
    			Set(ref _gS2, value);
    		}
    
        }
        private Nullable<double> _gS2;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual Nullable<double> GS3
        {
            get { return _gS3; }
            set
    		{ 
    			Set(ref _gS3, value);
    		}
    
        }
        private Nullable<double> _gS3;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int AT
        {
            get { return _aT; }
            set
    		{ 
    			Set(ref _aT, value);
    		}
    
        }
        private int _aT;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int FK
        {
            get { return _fK; }
            set
    		{ 
    			Set(ref _fK, value);
    		}
    
        }
        private int _fK;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Token
        {
            get { return _token; }
            set
    		{ 
    			Set(ref _token, value);
    		}
    
        }
        private string _token;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual Nullable<int> TokenSizeX
        {
            get { return _tokenSizeX; }
            set
    		{ 
    			Set(ref _tokenSizeX, value);
    		}
    
        }
        private Nullable<int> _tokenSizeX;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual Nullable<int> TokenSizeY
        {
            get { return _tokenSizeY; }
            set
    		{ 
    			Set(ref _tokenSizeY, value);
    		}
    
        }
        private Nullable<int> _tokenSizeY;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual Nullable<double> TokenOversize
        {
            get { return _tokenOversize; }
            set
    		{ 
    			Set(ref _tokenOversize, value);
    		}
    
        }
        private Nullable<double> _tokenOversize;

        #endregion

        #region Navigation Properties
    
    	[DataMember]
        public virtual ICollection<Gegner_Modifikator> Gegner_Modifikator
        {
            get
            {
                if (_gegner_Modifikator == null)
                {
                    var newCollection = new FixupCollection<Gegner_Modifikator>();
                    newCollection.CollectionChanged += FixupGegner_Modifikator;
                    _gegner_Modifikator = newCollection;
                }
                return _gegner_Modifikator;
            }
            set
            {
                if (!ReferenceEquals(_gegner_Modifikator, value))
                {
                    var previousValue = _gegner_Modifikator as FixupCollection<Gegner_Modifikator>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupGegner_Modifikator;
                    }
                    _gegner_Modifikator = value;
                    var newValue = value as FixupCollection<Gegner_Modifikator>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupGegner_Modifikator;
                    }
                }
            }
        }
        private ICollection<Gegner_Modifikator> _gegner_Modifikator;
    
    	[DataMember]
        public virtual GegnerBase GegnerBase
        {
            get { return _gegnerBase; }
            set
            {
                if (!ReferenceEquals(_gegnerBase, value))
                {
                    var previousValue = _gegnerBase;
                    _gegnerBase = value;
                    FixupGegnerBase(previousValue);
                }
            }
        }
        private GegnerBase _gegnerBase;

        #endregion

        #region Association Fixup
    
        private void FixupGegnerBase(GegnerBase previousValue)
        {
    		OnChanged("GegnerBase");
            if (previousValue != null && previousValue.Gegner.Contains(this))
            {
                previousValue.Gegner.Remove(this);
            }
    
            if (GegnerBase != null)
            {
                if (!GegnerBase.Gegner.Contains(this))
                {
                    GegnerBase.Gegner.Add(this);
                }
                if (GegnerBaseGUID != GegnerBase.GegnerBaseGUID)
                {
                    GegnerBaseGUID = GegnerBase.GegnerBaseGUID;
                }
            }
        }
    
        private void FixupGegner_Modifikator(object sender, NotifyCollectionChangedEventArgs e)
        {
    		OnChanged("Gegner_Modifikator");
            if (e.NewItems != null)
            {
                foreach (Gegner_Modifikator item in e.NewItems)
                {
                    item.Gegner = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Gegner_Modifikator item in e.OldItems)
                {
                    if (ReferenceEquals(item.Gegner, this))
                    {
                        item.Gegner = null;
                    }
                }
            }
        }

        #endregion

    }
}
