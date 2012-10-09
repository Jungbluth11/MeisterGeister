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
    public partial class Ausrüstung : INotifyPropertyChanged
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
        public virtual System.Guid AusrüstungGUID
        {
            get { return _ausrüstungGUID; }
            set
    		{ 
    			_ausrüstungGUID = value;
    			OnChanged("AusrüstungGUID");
    		}
    
        }
        private System.Guid _ausrüstungGUID;
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
        public virtual double Preis
        {
            get { return _preis; }
            set
    		{ 
    			_preis = value;
    			OnChanged("Preis");
    		}
    
        }
        private double _preis;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual int Gewicht
        {
            get { return _gewicht; }
            set
    		{ 
    			_gewicht = value;
    			OnChanged("Gewicht");
    		}
    
        }
        private int _gewicht;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Verbreitung
        {
            get { return _verbreitung; }
            set
    		{ 
    			_verbreitung = value;
    			OnChanged("Verbreitung");
    		}
    
        }
        private string _verbreitung;
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

        #endregion

        #region Navigation Properties
    
    	[DataMember]
        public virtual Fernkampfwaffe Fernkampfwaffe
        {
            get { return _fernkampfwaffe; }
            set
            {
                if (!ReferenceEquals(_fernkampfwaffe, value))
                {
                    var previousValue = _fernkampfwaffe;
                    _fernkampfwaffe = value;
                    FixupFernkampfwaffe(previousValue);
                }
            }
        }
        private Fernkampfwaffe _fernkampfwaffe;
    
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
        public virtual Rüstung Rüstung
        {
            get { return _rüstung; }
            set
            {
                if (!ReferenceEquals(_rüstung, value))
                {
                    var previousValue = _rüstung;
                    _rüstung = value;
                    FixupRüstung(previousValue);
                }
            }
        }
        private Rüstung _rüstung;
    
    	[DataMember]
        public virtual Schild Schild
        {
            get { return _schild; }
            set
            {
                if (!ReferenceEquals(_schild, value))
                {
                    var previousValue = _schild;
                    _schild = value;
                    FixupSchild(previousValue);
                }
            }
        }
        private Schild _schild;
    
    	[DataMember]
        public virtual Waffe Waffe
        {
            get { return _waffe; }
            set
            {
                if (!ReferenceEquals(_waffe, value))
                {
                    var previousValue = _waffe;
                    _waffe = value;
                    FixupWaffe(previousValue);
                }
            }
        }
        private Waffe _waffe;

        #endregion

        #region Association Fixup
    
        private void FixupFernkampfwaffe(Fernkampfwaffe previousValue)
        {
    		OnChanged("Fernkampfwaffe");
            if (previousValue != null && ReferenceEquals(previousValue.Ausrüstung, this))
            {
                previousValue.Ausrüstung = null;
            }
    
            if (Fernkampfwaffe != null)
            {
                Fernkampfwaffe.Ausrüstung = this;
            }
        }
    
        private void FixupRüstung(Rüstung previousValue)
        {
    		OnChanged("Rüstung");
            if (previousValue != null && ReferenceEquals(previousValue.Ausrüstung, this))
            {
                previousValue.Ausrüstung = null;
            }
    
            if (Rüstung != null)
            {
                Rüstung.Ausrüstung = this;
            }
        }
    
        private void FixupSchild(Schild previousValue)
        {
    		OnChanged("Schild");
            if (previousValue != null && ReferenceEquals(previousValue.Ausrüstung, this))
            {
                previousValue.Ausrüstung = null;
            }
    
            if (Schild != null)
            {
                Schild.Ausrüstung = this;
            }
        }
    
        private void FixupWaffe(Waffe previousValue)
        {
    		OnChanged("Waffe");
            if (previousValue != null && ReferenceEquals(previousValue.Ausrüstung, this))
            {
                previousValue.Ausrüstung = null;
            }
    
            if (Waffe != null)
            {
                Waffe.Ausrüstung = this;
            }
        }
    
        private void FixupHeld_Ausrüstung(object sender, NotifyCollectionChangedEventArgs e)
        {
    		OnChanged("Held_Ausrüstung");
            if (e.NewItems != null)
            {
                foreach (Held_Ausrüstung item in e.NewItems)
                {
                    item.Ausrüstung = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Held_Ausrüstung item in e.OldItems)
                {
                    if (ReferenceEquals(item.Ausrüstung, this))
                    {
                        item.Ausrüstung = null;
                    }
                }
            }
        }

        #endregion

    }
}
