﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows.Controls;
using System.Windows.Media;
using MeisterGeister.Daten;
using MeisterGeister.View.General;
//Eigene usings
using MeisterGeister.ViewModel.MeisterSpicker.Logic;

namespace MeisterGeister.ViewModel.MeisterSpicker
{
    public class MeisterSpickerViewModel : Base.ToolViewModelBase
    {

        #region //---- Variablen ----

        private OleDbCommand _cmd = new OleDbCommand();
        public OleDbCommand cmd
        {
            get { return _cmd; }
            set { Set(ref _cmd, value); }
        }

        private OleDbConnection _con = new OleDbConnection();
        public OleDbConnection con
        {
            get { return _con; }
            set { Set(ref _con, value); }
        }

        private OleDbDataAdapter _da = new OleDbDataAdapter();
        public OleDbDataAdapter da
        {
            get { return _da; }
            set { Set(ref _da, value); }
        }

        #endregion
        #region //---- FELDER ----

        // Felder
        private string _suchTextGegenstand = string.Empty;
        private string _suchTextBeschreibung = string.Empty;

        // Listen
        private List<MeisterSpickerItem> _MeisterSpickerItemListe;
        private List<MeisterSpickerItem> _filteredMeisterSpickerItemListe;

        //Commands

        #endregion

        #region //---- EIGENSCHAFTEN ----

        private DataTable _spickerdatatable = new DataTable();
        public DataTable SpickerDatatable
        {
            get { return _spickerdatatable; }
            set { Set(ref _spickerdatatable, value); }
        }


        private DataGrid _spickerData = new DataGrid();
        public DataGrid SpickerData
        {
            get { return _spickerData; }
            set { Set(ref _spickerData, value); }
        }

        private DataRowView _spickerDataViewSelected = null;
        public DataRowView SpickerDataViewSelected
        {
            get { return _spickerDataViewSelected; }
            set
            {
                if (DataChanged)
                {
                    if (ViewHelper.Confirm("Daten geändert", "Der Datensatz wurde geändert.\n\nSollen die Änderungen übernommen werden?"))
                    {
                        string delQuery = string.Format("UPDATE Tabelle1 SET Gegenstand='{0}', Preis='{1}', Gewicht='{2}', Qualität='{3}', M1='{4}' WHERE ID={5}",
                            _spickerDataViewSelected.Row.ItemArray[_spickerDataViewSelected.Row.Table.Columns.IndexOf("Gegenstand")],
                            _spickerDataViewSelected.Row.ItemArray[_spickerDataViewSelected.Row.Table.Columns.IndexOf("Preis")],
                            _spickerDataViewSelected.Row.ItemArray[_spickerDataViewSelected.Row.Table.Columns.IndexOf("Gewicht")],
                            _spickerDataViewSelected.Row.ItemArray[_spickerDataViewSelected.Row.Table.Columns.IndexOf("Qualität")],
                            _spickerDataViewSelected.Row.ItemArray[_spickerDataViewSelected.Row.Table.Columns.IndexOf("M1")],
                            _spickerDataViewSelected.Row.ItemArray[_spickerDataViewSelected.Row.Table.Columns.IndexOf("ID")]);
                        string ConString = @"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = " + DatabaseTools.DATABASE_FOLDER + "MeisterSpicker.mdb";
                        con.Close();
                        CreateOleDbCommand(delQuery, ConString);
                        con.Open();
                    }
                    else
                    {
                        DataRowView preDRow = _spickerDataViewSelected;
                        con.Close();
                        DataChanged = false;
                        Init();
                        _spickerDataViewSelected = preDRow;
                        OnChanged(nameof(SpickerDataViewSelected));
                    }
                    DataChanged = false;
                    DataSetCanChange = false;
                }
                Set(ref _spickerDataViewSelected, value); 
            }
        }

        private DataView _spickerDataView = new DataView();
        public DataView SpickerDataView
        {
            get { return _spickerDataView; }
            set { Set(ref _spickerDataView, value); }
        }
        private bool _dataSetCanChange = false;
        public bool DataSetCanChange
        {
            get { return _dataSetCanChange; }
            set 
            { 
                Set(ref _dataSetCanChange, value);
                BackgroundChangeColor = value ? new SolidColorBrush(Colors.LightPink) : new SolidColorBrush(Colors.Transparent);
            }
        }

        private bool _dataChanged = false;
        public bool DataChanged
        {
            get { return _dataChanged; }
            set { Set(ref _dataChanged, value); }
        }
        public string SuchTextGegenstand
        {
            get { return _suchTextGegenstand; }
            set
            {
                Set(ref _suchTextGegenstand, value);
                FilterListe();
            }
        }
        public string SuchTextBeschreibung
        {
            get { return _suchTextBeschreibung; }
            set
            {
                Set(ref _suchTextBeschreibung, value);
                FilterListe();
            }
        }

        private SolidColorBrush _backgroundChangeColor = new SolidColorBrush(Colors.Transparent);
        public SolidColorBrush BackgroundChangeColor
        {
            get { return _backgroundChangeColor; }
            set { Set(ref _backgroundChangeColor, value); }
        }

        #region //---- LISTEN ----

        public List<MeisterSpickerItem> MeisterSpickerItemListe
        {
            get { return _MeisterSpickerItemListe; }
            set
            {
                _MeisterSpickerItemListe = value;
                OnChanged("MeisterSpickerItemListe");
            }
        }
        public List<MeisterSpickerItem> FilteredMeisterSpickerItemListe
        {
            get { return _filteredMeisterSpickerItemListe; }
            set
            {
                _filteredMeisterSpickerItemListe = value;
                OnChanged("FilteredMeisterSpickerItemListe");
            }
        }

        #endregion

        //Commands

        #endregion

        #region //---- KONSTRUKTOR ----

        public MeisterSpickerViewModel()
        {
            Init();
        }

        #endregion

        #region //---- INSTANZMETHODEN ----

        public void Refresh()
        {
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            SpickerDatatable = dt;
            SpickerDataView = SpickerDatatable.DefaultView;
        }

        public void Init()
        {
            loaddata();
        }

        /// <summary>
        /// Filtert die BasarItem-Liste auf Basis des SuchTextes.
        /// </summary>
        private void FilterListe()
        {
            SpickerDatatable.Clear();
            string suchText1 = _suchTextGegenstand.ToLower().Trim();
            string[] suchWorte1 = suchText1.Split(' ');

            string suchText2 = _suchTextBeschreibung.ToLower().Trim();
            string[] suchWorte2 = suchText2.Split(' ');


            if (suchWorte1.Length == 0 || (suchWorte1.Length == 1 && suchWorte1[0] == ""))
                cmd.CommandText = "SELECT * FROM Tabelle1";
            else
                cmd.CommandText = "SELECT * FROM Tabelle1 WHERE Gegenstand LIKE '%" +
                    (suchWorte1.Length == 1 ? suchWorte1[0] + "%'" :
                        (suchWorte1.Length > 1 ? string.Join("%' AND Gegenstand LIKE '%", suchWorte1) + "%'" : "%'"));


            if (!(suchWorte2.Length == 0 || (suchWorte2.Length == 1 && suchWorte2[0] == "")))
            {
                if (cmd.CommandText.Contains("WHERE"))
                {
                    cmd.CommandText += " OR M1 LIKE '%" +
                        (suchWorte2.Length == 1 ? suchWorte2[0] + "%'" :
                            (suchWorte2.Length > 1 ? string.Join("%' AND M1 LIKE '%", suchWorte2) + "%'" : "%'"));
                }
                else
                {
                    cmd.CommandText = "SELECT * FROM Tabelle1 WHERE Gegenstand LIKE '%" +
                      (suchWorte2.Length == 1 ? suchWorte2[0] + "%'" :
                          (suchWorte2.Length > 1 ? string.Join("%' AND Gegenstand LIKE '%", suchWorte2) + "%'" : "%'"));
                    cmd.CommandText += " OR M1 LIKE '%" +
                            (suchWorte2.Length == 1 ? suchWorte2[0] + "%'" :
                                (suchWorte2.Length > 1 ? string.Join("%' AND M1 LIKE '%", suchWorte2) + "%'" : "%'"));
                }
            }
            Refresh();
        }

        #endregion

        private void loaddata()
        {
            try
            {
                if (con.State != System.Data.ConnectionState.Closed)
                    return;
                con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + DatabaseTools.DATABASE_FOLDER + "MeisterSpicker.mdb");
                if (con.State != ConnectionState.Open)
                    con.Open();
                cmd.Connection = con;
                cmd.CommandText = "select * from Tabelle1";
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                SpickerDatatable = dt;
                SpickerDataView = SpickerDatatable.DefaultView;
            }
            catch (Exception ex)
            {
                con.Close();
                ViewHelper.ShowError(ex.Message.ToString(), ex);
            }
        }

        #region //---- EVENTS ----

        private Base.CommandBase _onBtnDatasetNew = null;
        public Base.CommandBase onBtnDatasetNew
        {
            get
            {
                if (_onBtnDatasetNew == null)
                    _onBtnDatasetNew = new Base.CommandBase(DatasetNew, null);
                return _onBtnDatasetNew;
            }
        }
        void DatasetNew(object obj)
        {
            string neu = "Neuer Gegenstand";
            string delQuery = "INSERT INTO Tabelle1 (Gegenstand, M1) VALUES ('"+neu+"','neue Beschreibung') ";
            string ConString = @"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = " + DatabaseTools.DATABASE_FOLDER + "MeisterSpicker.mdb";
            con.Close();
            CreateOleDbCommand(delQuery, ConString);
            con.Open();
            SuchTextGegenstand = neu;
            FilterListe();
        }

        private Base.CommandBase _onBtnDatasetDelete = null;
        public Base.CommandBase onBtnDatasetDelete
        {
            get
            {
                if (_onBtnDatasetDelete == null)
                    _onBtnDatasetDelete = new Base.CommandBase(DatasetDelete, null);
                return _onBtnDatasetDelete;
            }
        }
        void DatasetDelete(object obj)
        {
            if (SpickerDataViewSelected == null)
            {
                ViewHelper.Popup("Bitte erst ein Element auswählen");
                return;
            }
            string delQuery = "DELETE FROM Tabelle1 WHERE ID="+ SpickerDataViewSelected.Row.ItemArray[0] + " ";
            string ConString = @"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = " + DatabaseTools.DATABASE_FOLDER + "MeisterSpicker.mdb";
            con.Close();
            CreateOleDbCommand(delQuery, ConString);
            con.Open();
            ViewHelper.Popup("Eintrag erfolgreich gelöscht!");
            FilterListe();
        }

        static private void CreateOleDbCommand(string queryString, string connectionString)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand(queryString, connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private Base.CommandBase _onBtnDeleteSuchTextGegenstandFilter = null;
        public Base.CommandBase OnBtnDeleteSuchTextGegenstandFilter
        {
            get
            {
                if (_onBtnDeleteSuchTextGegenstandFilter == null)
                    _onBtnDeleteSuchTextGegenstandFilter = new Base.CommandBase(DeleteSuchTextGegenstandFilter, null);
                return _onBtnDeleteSuchTextGegenstandFilter;
            }
        }
        void DeleteSuchTextGegenstandFilter(object obj)
        {
            SuchTextGegenstand = string.Empty;
        }

        private Base.CommandBase _onBtnDeleteSuchTextBeschreibungFilter = null;
        public Base.CommandBase OnBtnDeleteSuchTextBeschreibungFilter
        {
            get
            {
                if (_onBtnDeleteSuchTextBeschreibungFilter == null)
                    _onBtnDeleteSuchTextBeschreibungFilter = new Base.CommandBase(DeleteSuchTextBeschreibungFilter, null);
                return _onBtnDeleteSuchTextBeschreibungFilter;
            }
        }
        void DeleteSuchTextBeschreibungFilter(object obj)
        {
            SuchTextBeschreibung = string.Empty;
        }
        #endregion
    }
}
