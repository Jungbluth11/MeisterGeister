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
    public partial class Handelsgut : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
    	public event PropertyChangedEventHandler PropertyChanged;
    	
    	/// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void OnChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

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

        #region Set
    	/// <summary>
        /// Checks if a property already matches a desired value.  Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected bool Set<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;
    
    		OnValidatePropertyChanging(storage, value, propertyName);
    		storage = value;
    		OnChanged(propertyName);
            return true;
        }

        #endregion

        #region Primitive Properties
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual System.Guid HandelsgutGUID
        {
            get { return _handelsgutGUID; }
            set
    		{ 
    			Set(ref _handelsgutGUID, value);
    		}
    
        }
        private System.Guid _handelsgutGUID;
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
        public virtual Nullable<double> Gewicht
        {
            get { return _gewicht; }
            set
    		{ 
    			Set(ref _gewicht, value);
    		}
    
        }
        private Nullable<double> _gewicht;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string ME
        {
            get { return _mE; }
            set
    		{ 
    			Set(ref _mE, value);
    		}
    
        }
        private string _mE;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Kategorie
        {
            get { return _kategorie; }
            set
    		{ 
    			Set(ref _kategorie, value);
    		}
    
        }
        private string _kategorie;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual string Tags
        {
            get { return _tags; }
            set
    		{ 
    			Set(ref _tags, value);
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
    			Set(ref _bemerkung, value);
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
    			Set(ref _literatur, value);
    		}
    
        }
        private string _literatur;
    	///<summary>Database persistent property</summary>
    	[DataMember]
        public virtual Nullable<double> Verpackungseinheit
        {
            get { return _verpackungseinheit; }
            set
    		{ 
    			Set(ref _verpackungseinheit, value);
    		}
    
        }
        private Nullable<double> _verpackungseinheit;

        #endregion

        #region Navigation Properties
    
    	[DataMember]
        public virtual ICollection<Handelsgut_Setting> Handelsgut_Setting
        {
            get
            {
                if (_handelsgut_Setting == null)
                {
                    var newCollection = new FixupCollection<Handelsgut_Setting>();
                    newCollection.CollectionChanged += FixupHandelsgut_Setting;
                    _handelsgut_Setting = newCollection;
                }
                return _handelsgut_Setting;
            }
            set
            {
                if (!ReferenceEquals(_handelsgut_Setting, value))
                {
                    var previousValue = _handelsgut_Setting as FixupCollection<Handelsgut_Setting>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupHandelsgut_Setting;
                    }
                    _handelsgut_Setting = value;
                    var newValue = value as FixupCollection<Handelsgut_Setting>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupHandelsgut_Setting;
                    }
                }
            }
        }
        private ICollection<Handelsgut_Setting> _handelsgut_Setting;

        #endregion

        #region Association Fixup
    
        private void FixupHandelsgut_Setting(object sender, NotifyCollectionChangedEventArgs e)
        {
    		OnChanged("Handelsgut_Setting");
            if (e.NewItems != null)
            {
                foreach (Handelsgut_Setting item in e.NewItems)
                {
                    item.Handelsgut = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Handelsgut_Setting item in e.OldItems)
                {
                    if (ReferenceEquals(item.Handelsgut, this))
                    {
                        item.Handelsgut = null;
                    }
                }
            }
        }

        #endregion

    }
}
