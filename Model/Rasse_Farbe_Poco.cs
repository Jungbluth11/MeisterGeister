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
    public partial class Rasse_Farbe : INotifyPropertyChanged
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
        public virtual System.Guid RasseGUID
        {
            get { return _rasseGUID; }
            set
            {
                if (_rasseGUID != value)
                {
                    if (Rasse != null && Rasse.RasseGUID != value)
                    {
                        Rasse = null;
                    }
                    _rasseGUID = value;
                }
            }
    
        }
        private System.Guid _rasseGUID;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual long FarbeID
        {
            get { return _farbeID; }
            set
            {
                if (_farbeID != value)
                {
                    if (Farbe != null && Farbe.FarbeID != value)
                    {
                        Farbe = null;
                    }
                    _farbeID = value;
                }
            }
    
        }
        private long _farbeID;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Kategorie
        {
            get { return _kategorie; }
            set
    		{ 
    			_kategorie = value;
    			OnChanged("Kategorie");
    		}
    
        }
        private string _kategorie;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int W20
        {
            get { return _w20; }
            set
    		{ 
    			_w20 = value;
    			OnChanged("W20");
    		}
    
        }
        private int _w20;

        #endregion

        #region Navigation Properties
    
    	[DataMember]
        public virtual Farbe Farbe
        {
            get { return _farbe; }
            set
            {
                if (!ReferenceEquals(_farbe, value))
                {
                    var previousValue = _farbe;
                    _farbe = value;
                    FixupFarbe(previousValue);
                }
            }
        }
        private Farbe _farbe;
    
    	[DataMember]
        public virtual Rasse Rasse
        {
            get { return _rasse; }
            set
            {
                if (!ReferenceEquals(_rasse, value))
                {
                    var previousValue = _rasse;
                    _rasse = value;
                    FixupRasse(previousValue);
                }
            }
        }
        private Rasse _rasse;

        #endregion

        #region Association Fixup
    
        private void FixupFarbe(Farbe previousValue)
        {
    		OnChanged("Farbe");
            if (previousValue != null && previousValue.Rasse_Farbe.Contains(this))
            {
                previousValue.Rasse_Farbe.Remove(this);
            }
    
            if (Farbe != null)
            {
                if (!Farbe.Rasse_Farbe.Contains(this))
                {
                    Farbe.Rasse_Farbe.Add(this);
                }
                if (FarbeID != Farbe.FarbeID)
                {
                    FarbeID = Farbe.FarbeID;
                }
            }
        }
    
        private void FixupRasse(Rasse previousValue)
        {
    		OnChanged("Rasse");
            if (previousValue != null && previousValue.Rasse_Farbe.Contains(this))
            {
                previousValue.Rasse_Farbe.Remove(this);
            }
    
            if (Rasse != null)
            {
                if (!Rasse.Rasse_Farbe.Contains(this))
                {
                    Rasse.Rasse_Farbe.Add(this);
                }
                if (RasseGUID != Rasse.RasseGUID)
                {
                    RasseGUID = Rasse.RasseGUID;
                }
            }
        }

        #endregion

    }
}
