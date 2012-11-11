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
    public partial class VorNachteil : INotifyPropertyChanged
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
        public virtual Nullable<bool> Vorteil
        {
            get { return _vorteil; }
            set
    		{ 
    			_vorteil = value;
    			OnChanged("Vorteil");
    		}
    
        }
        private Nullable<bool> _vorteil;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual Nullable<bool> Nachteil
        {
            get { return _nachteil; }
            set
    		{ 
    			_nachteil = value;
    			OnChanged("Nachteil");
    		}
    
        }
        private Nullable<bool> _nachteil;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual Nullable<bool> HatWert
        {
            get { return _hatWert; }
            set
    		{ 
    			_hatWert = value;
    			OnChanged("HatWert");
    		}
    
        }
        private Nullable<bool> _hatWert;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string WertTyp
        {
            get { return _wertTyp; }
            set
    		{ 
    			_wertTyp = value;
    			OnChanged("WertTyp");
    		}
    
        }
        private string _wertTyp;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Typ
        {
            get { return _typ; }
            set
    		{ 
    			_typ = value;
    			OnChanged("Typ");
    		}
    
        }
        private string _typ;
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
        public virtual System.Guid VorNachteilGUID
        {
            get { return _vorNachteilGUID; }
            set
    		{ 
    			_vorNachteilGUID = value;
    			OnChanged("VorNachteilGUID");
    		}
    
        }
        private System.Guid _vorNachteilGUID;
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
        public virtual ICollection<Held_VorNachteil> Held_VorNachteil
        {
            get
            {
                if (_held_VorNachteil == null)
                {
                    var newCollection = new FixupCollection<Held_VorNachteil>();
                    newCollection.CollectionChanged += FixupHeld_VorNachteil;
                    _held_VorNachteil = newCollection;
                }
                return _held_VorNachteil;
            }
            set
            {
                if (!ReferenceEquals(_held_VorNachteil, value))
                {
                    var previousValue = _held_VorNachteil as FixupCollection<Held_VorNachteil>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupHeld_VorNachteil;
                    }
                    _held_VorNachteil = value;
                    var newValue = value as FixupCollection<Held_VorNachteil>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupHeld_VorNachteil;
                    }
                }
            }
        }
        private ICollection<Held_VorNachteil> _held_VorNachteil;

        #endregion

        #region Association Fixup
    
        private void FixupHeld_VorNachteil(object sender, NotifyCollectionChangedEventArgs e)
        {
    		OnChanged("Held_VorNachteil");
            if (e.NewItems != null)
            {
                foreach (Held_VorNachteil item in e.NewItems)
                {
                    item.VorNachteil = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Held_VorNachteil item in e.OldItems)
                {
                    if (ReferenceEquals(item.VorNachteil, this))
                    {
                        item.VorNachteil = null;
                    }
                }
            }
        }

        #endregion

    }
}
