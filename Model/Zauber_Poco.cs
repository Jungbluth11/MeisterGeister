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
    public partial class Zauber : INotifyPropertyChanged
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
        public virtual int ZauberID
        {
            get { return _zauberID; }
            set
    		{ 
    			_zauberID = value;
    			OnChanged("ZauberID");
    		}
    
        }
        private int _zauberID;
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
        public virtual string Komplex
        {
            get { return _komplex; }
            set
    		{ 
    			_komplex = value;
    			OnChanged("Komplex");
    		}
    
        }
        private string _komplex;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Repräsentationen
        {
            get { return _repräsentationen; }
            set
    		{ 
    			_repräsentationen = value;
    			OnChanged("Repräsentationen");
    		}
    
        }
        private string _repräsentationen;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Merkmale
        {
            get { return _merkmale; }
            set
    		{ 
    			_merkmale = value;
    			OnChanged("Merkmale");
    		}
    
        }
        private string _merkmale;
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
        public virtual ICollection<Held_Zauber> Held_Zauber
        {
            get
            {
                if (_held_Zauber == null)
                {
                    var newCollection = new FixupCollection<Held_Zauber>();
                    newCollection.CollectionChanged += FixupHeld_Zauber;
                    _held_Zauber = newCollection;
                }
                return _held_Zauber;
            }
            set
            {
                if (!ReferenceEquals(_held_Zauber, value))
                {
                    var previousValue = _held_Zauber as FixupCollection<Held_Zauber>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupHeld_Zauber;
                    }
                    _held_Zauber = value;
                    var newValue = value as FixupCollection<Held_Zauber>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupHeld_Zauber;
                    }
                }
            }
        }
        private ICollection<Held_Zauber> _held_Zauber;

        #endregion

        #region Association Fixup
    
        private void FixupHeld_Zauber(object sender, NotifyCollectionChangedEventArgs e)
        {
    		OnChanged("Held_Zauber");
            if (e.NewItems != null)
            {
                foreach (Held_Zauber item in e.NewItems)
                {
                    item.Zauber = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Held_Zauber item in e.OldItems)
                {
                    if (ReferenceEquals(item.Zauber, this))
                    {
                        item.Zauber = null;
                    }
                }
            }
        }

        #endregion

    }
}
