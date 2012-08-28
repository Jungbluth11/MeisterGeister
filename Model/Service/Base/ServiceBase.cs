﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.Objects;

using MeisterGeister.Model.Extensions;

namespace MeisterGeister.Model.Service {
    public class ServiceBase {
        
        protected static Object syncRoot;

        protected static volatile DatabaseDSAEntities _context = null;
        public static string ConnectionString = "metadata=res://*/Model.MeisterGeisterModel.csdl|res://*/Model.MeisterGeisterModel.ssdl|res://*/Model.MeisterGeisterModel.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=\"Data Source=|DataDirectory|" + "\\" + "Daten\\DatabaseDSA.sdf\"";

        static ServiceBase() {
            syncRoot = new Object();
            Context.SavingChanges += savingChanges;
        }

        protected static event EventHandler savingChanges;

        /// <summary>
        /// Tritt auf, wenn Änderungen in der Datenquelle gespeichert werden.
        /// </summary>
        public static event EventHandler SavingChanges
        {
            add
            {
                ServiceBase.savingChanges += value;
            }
            remove
            {
                ServiceBase.savingChanges -= value;
            }
        }

        protected static DatabaseDSAEntities Context {
            get {
                if (ServiceBase._context == null) {
                    lock (ServiceBase.syncRoot) {
                        if (ServiceBase._context == null)
                            ServiceBase._context = new DatabaseDSAEntities(ConnectionString);
                    }
                }
                return ServiceBase._context;
            }
        }

        public virtual bool Insert<T>(T aModelObject) where T : class {
            try {
                Context.AddObject(typeof(T).Name, aModelObject);
                Liste<T>().Add(aModelObject);
                Save();
                return true;
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.InnerException);
                return false;
            }
        }

        public virtual bool Update<T>(T aModelObject) where T : class {
            try {
                Context.ApplyCurrentValues(typeof(T).Name, aModelObject);
                Save();
                return true;
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.InnerException);
                return false;
            }
        }

        public virtual bool Delete<T>(T aModelObject) where T : class {
            try {
                //Context.AttachTo(typeof(T).Name, aModelObject);
                Context.DeleteObject(aModelObject);
                Save();
                Liste<T>().Remove(aModelObject);
                return true;
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.InnerException);
                return false;
            }
        }

        public virtual System.Data.EntityState GetEntityState(object entity)
        {
            return Context.ObjectStateManager.GetObjectStateEntry(entity).State;
        }

        public virtual IEnumerable<ObjectStateEntry> GetObjectStateEntriesWithEntityState<T>(System.Data.EntityState state)
        {
            return Context.ObjectStateManager.GetObjectStateEntries(state).Where(os => os.Entity is T);
        }

        public virtual IEnumerable<T> GetObjectsWithEntityState<T>(System.Data.EntityState state)
        {
            return Context.ObjectStateManager.GetObjectStateEntries(state).Where(os => os.Entity is T).Select(os => (T)os.Entity);
        }

        protected virtual IDictionary<object, System.Data.EntityState> GetChangedEntities()
        {
            IEnumerable<ObjectStateEntry> objectStates = Context.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added | System.Data.EntityState.Deleted | System.Data.EntityState.Modified);
            IDictionary<object, System.Data.EntityState> ret = new Dictionary<object, System.Data.EntityState>();
            foreach (ObjectStateEntry os in objectStates)
                ret.Add(os.Entity, os.State);
            return ret;
        }

        protected static Dictionary<Type, Object> _lists = new Dictionary<Type, object>();

        public List<T> Liste<T>() where T :  class {
            if (!ServiceBase._lists.ContainsKey(typeof(T)))
                ServiceBase._lists.Add(typeof(T), LoadList<T>());
            return ServiceBase._lists[typeof(T)] as List<T>;
        }

        private List<T> LoadList<T>() where T: class {
            return Context.GetObjectSet<T>().ToList<T>();
        }

        //Für Import
        public void UpdateList<T>() where T : class {
            List<T> tmp = Liste<T>();
            //Erhaltung des Objektes. So bleiben alle gespeicherten Referenzen in anderen Klassen korrekt.
            tmp.Clear();
            tmp.AddRange(LoadList<T>());
        }

        public virtual T New<T>() where T : class {
            return Context.CreateObject<T>();
        }

        /// <summary>
        /// Gibt einen Klon der Model-Klasse zurück.
        /// Der Klon hat nur die einfachen Eigenschaften, die in der Datenbank abgespeichert werden.
        /// Collections werden nicht mit geklont.
        /// </summary>
        public virtual T Clone<T>(T aModelObject) where T : class
        {
            return (T)Context.GetShallowClone(aModelObject);
        }

        public virtual int Save() {
            return Context.SaveChanges();
        }
    }
}