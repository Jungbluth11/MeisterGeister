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
    public partial class Einstellung : INotifyPropertyChanged
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
        public virtual string Kontext
        {
            get { return _kontext; }
            set
    		{ 
    			_kontext = value;
    			OnChanged("Kontext");
    		}
    
        }
        private string _kontext;
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
        public virtual string Wert
        {
            get { return _wert; }
            set
    		{ 
    			_wert = value;
    			OnChanged("Wert");
    		}
    
        }
        private string _wert;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Beschreibung
        {
            get { return _beschreibung; }
            set
    		{ 
    			_beschreibung = value;
    			OnChanged("Beschreibung");
    		}
    
        }
        private string _beschreibung;

        #endregion

    }
}
