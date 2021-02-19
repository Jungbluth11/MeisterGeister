﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows.Media.Imaging;
using MeisterGeister.Model;
using MeisterGeister.View.General;
//Eigene usings
using MeisterGeister.Logic.Einstellung;

namespace MeisterGeister.ViewModel.Foundry
{
    public class FoundryViewModel : Base.ToolViewModelBase
    {

        //TODO:  Helden: Bars -Always visible, Lep, AsP
        //TODO:  Foundry Pfad vom User definierbar, read Options, set Web-Connection (lokal oder Inet auswählbar)
        //TODO:  SpielerScreen: Zeige WebBrowser

        public class MyTimer
        {
            static int start = 0;
            static int stop = 0;
            public static void start_timer()
            {
                start = Environment.TickCount;
            }
            public static void stop_timer()
            {
                stop_timer("");
            }
            public static string stop_timer(string msg)
            {
                stop = Environment.TickCount;
                return print(msg);
            }
            private static string print(string msg)
            {
                string output = "MyTimer(" + msg + "): " + (stop - start) + " Millisekunden";
                System.Diagnostics.Debug.WriteLine(output);
                return output;
            }
        }

        public class folder
        {
            public string name { get; set; }
            public string typ { get; set; }
            public string sorting { get; set; }
            public string color { get; set; }
            public string _id { get; set; }
        }

        #region //---- Variablen ----

        List<GegnerArgument> lstGegnerArgument = new List<GegnerArgument>();
        List<HeldenArgument> lstHeldArgument = new List<HeldenArgument>();
        List<PlaylistArgument> lstPListArgument = new List<PlaylistArgument>();

        private void ChangePath(string vonS, string inS)
        {
            if (GegnerPortraitPfad == null) return;
            string neu = null;
            neu = GegnerPortraitPfad.Replace(vonS, inS);
            if (neu != GegnerPortraitPfad)
            {
                GegnerPortraitPfad = neu;
                Einstellungen.SetEinstellung<string>("FoundryGegnerPortraitPfad", GegnerPortraitPfad);
            }
            neu = HeldPortraitPfad.Replace(vonS, inS);
            if (neu != HeldPortraitPfad)
            {
                HeldPortraitPfad = neu;
                Einstellungen.SetEinstellung<string>("FoundryHeldPortraitPfad", HeldPortraitPfad);
            }
            neu = GegnerTokenPfad.Replace(vonS, inS);
            if (neu != GegnerTokenPfad)
            {
                GegnerTokenPfad = neu;
                Einstellungen.SetEinstellung<string>("FoundryGegnerTokenPfad", GegnerTokenPfad);
            }
            neu = HeldTokenPfad.Replace(vonS, inS);
            if (neu != HeldTokenPfad)
            {
                HeldTokenPfad = neu;
                Einstellungen.SetEinstellung<string>("FoundryHeldTokenPfad", HeldTokenPfad);
            }            
        }

        private bool _isLokalInstalliert = false;
        public bool IsLocalInstalliert
        {
            get { return _isLokalInstalliert; }
            set 
            {
                bool didChange = _isLokalInstalliert != value;
                Set(ref _isLokalInstalliert, value);
                if (!value)
                {
                    FTPAdresse = FTPAdresse.TrimEnd(new Char[] { '/' });
                    FoundryPfad = FTPAdresse + "/Data/";
                    ReadFoundryOptions(string.Format("{0}/config/options.json", FTPAdresse));

                    ChangePath(@"\", "/");
                }
                else
                {
                    FoundryPfad = localFoundryPfad;
                    string appFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    string path = appFolderPath + @"\FoundryVTT\Config\";
                    if (Directory.Exists(path))
                        ReadFoundryOptions(path + @"\options.json");

                    ChangePath("/", @"\");
                }
                if (didChange)
                    Init();
            }
        }

        #region //---- FTP ----

        private string _ftpAdresse = "ftp://195.114.11.154:21";
        public string FTPAdresse
        {
            get { return _ftpAdresse; }
            set 
            { 
                Set(ref _ftpAdresse, value);
                Einstellungen.SetEinstellung("FoundryFTPAdresse", value);
            }
        }

        private string _ftpUser = "juergen";
        public string FTPUser
        {
            get { return _ftpUser; }
            set 
            { 
                Set(ref _ftpUser, value);
                Einstellungen.SetEinstellung("FoundryFTPUser", value);
            }
        }

        private string _ftpPasswort= "FoundryVTT2021";
        public string FTPPasswort
        {
            get { return _ftpPasswort; }
            set 
            { 
                Set(ref _ftpPasswort, value);
                Einstellungen.SetEinstellung("FoundryFTPPasswort", value);
            }
        }

        private string _testDatei = @"C:\temp\Test.txt";
        public string TestDatei
        {
            get { return _testDatei; }
            set { Set(ref _testDatei, value); }
        }


        private Base.CommandBase _onBtnFTPConfig = null;
        public Base.CommandBase OnBtnFTPConfig
        {
            get
            {
                if (_onBtnFTPConfig == null)
                    _onBtnFTPConfig = new Base.CommandBase(FTPConfig, null);
                return _onBtnFTPConfig;
            }
        }
        private void FTPConfig(object sender)
        {
            string back = ViewHelper.InputDialog("FTP-Adresse", "Gebe die FTP-Adresse zu dem Server ein\n\nBeispiel Lokal:      "+ @"C:\FoundryVTT            " +
                "\nBeispiel Server:    ftp://195.114.11.154:21", FTPAdresse);
            if (!string.IsNullOrEmpty(back))
                FTPAdresse = back;
            back = ViewHelper.InputDialog("FTP-User", "Gebe den FTP-Usernamen zu dem Server ein", FTPUser);
            if (!string.IsNullOrEmpty(back))
                FTPUser = back;
            back = ViewHelper.InputDialog("FTP-Passwort", "Gebe das FTP-Passwort zu dem Server ein", "");
            if (!string.IsNullOrEmpty(back))
                FTPPasswort = back;
            Init();
        }


        private Base.CommandBase _onBtnConnectFTP = null;
        public Base.CommandBase OnBtnConnectFTP
        {
            get
            {
                if (_onBtnConnectFTP == null)
                    _onBtnConnectFTP = new Base.CommandBase(ConnectFTP, null);
                return _onBtnConnectFTP;
            }
        }
        private void ConnectFTP(object sender)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTPAdresse + "/Data/worlds/test.txt");
            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = new NetworkCredential(FTPUser,FTPPasswort, "");


            // Copy the contents of the file to the request stream.
            byte[] fileContents;
            using (StreamReader sourceStream = new StreamReader(TestDatei))
            {
                fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            }

            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }
        }


        #endregion

        #region //---- Classes ----

        public class PlaylistArgument
        {
            public string GMid { get; set; }
            public string _id { get; set; }
            public string name { get; set; }
            public string permission { get; set; }  // default = 0 user:Gamemaster = 3
            public int sort { get; set; }
            public string flags { get; set; }

            public string preArg
            {
                get 
                {
                    char A = (char)34;
                    string arg = "{" + A + "_id"+A+":" + A + _id + A + "," +
                        A + "name" + A + ":" + A + name + A + "," +
                        A + "permission" + A + ":{" + A + "default" + A + ":0," + A + GMid + A + ":3}," +
                        A + "sort" + A + ":" + sort.ToString() + "," +
                        A + "flags" + A + ":{}";
                    return arg; 
                }
                //{
                  //  "_id":"DUBBfsfZPdvWN8Mn",
        //"name":"neu",
        //"permission":{"default":0,"gNZOk6idrMy6uSkk":3},
        //"sort":100001,
        //"flags":{},
            }
            public class SoundArg
            {
                public List<string> lstArg {get; set;} 
                public string lstTitel { get; set; }
            }

            /*
             * 
             {
            "_id":"hMKrznmD7vhNLDNq",
            "flags":{},
            "path":"Musik/_Stra%C3%9Fenmusik/104-miguel_angel_tallante--cruzada-oma.mp3",
            "repeat":false,
            "volume":0.35355339059327373,
            "name":"104-miguel_angel_tallante--cruzada-oma",
            "playing":false,
            "streaming":false
        }
             * 
             * 
             * 
             */
            public List<SoundArg> lstSounds { get; set; }

            public string postArg
            {
                get
                {
                    char A = (char)34;
                    string arg = A + "mode" + A + ":" + mode.ToString() + "," +
                        A + "playing" + A + ":" + playing + "}";
                    return arg; }
            }
            public int mode { get; set; }
            public string playing { get; set; }

            public string outtext
            {
                get {
                    char A = (char)34;
                    return preArg + "," + A + "sounds" + A + ":[" + string.Join(",", lstSounds.Select(t => t.lstTitel)) + "]," + postArg;
                }
            }
            
        // {"_id":"DUBBfsfZPdvWN8Mn",
        //"name":"neu",
        //"permission":{"default":0,"gNZOk6idrMy6uSkk":3},
        //"sort":100001,
        //"flags":{},
        //"sounds":[
        //        ]
        
        //"mode":1,
        //"playing":false}
    }
        public class dbArgument
        {
            public string Argument
            { get; set; }
            public string Prefix
            { get; set; }
            public string Suffix
            { get; set; }
            public string ArgString
            { get; set; }
        }

        public class GegnerArgument
        {
            public GegnerBase g
            { get; set; }
            public List<dbArgument> lstArguments
            { get; set; }
            public string outcome
            { get; set; }
        }

        public class HeldenArgument
        {
            public Held h
            { get; set; }
            public List<dbArgument> lstArguments
            { get; set; }
            public string outcome
            { get; set; }
        }

        #endregion


        private string _gegnerPortraitPfad = null;
        public string GegnerPortraitPfad
        {
            get { return _gegnerPortraitPfad; }
            set 
            {
                string prevalue = value;
                if (prevalue != null && !prevalue.EndsWith("/") && !prevalue.EndsWith(@"\"))
                    prevalue = prevalue + (IsLocalInstalliert ? @"\" : "/");
                Set(ref _gegnerPortraitPfad, prevalue);
                Einstellungen.SetEinstellung<string>("FoundryGegnerPortraitPfad", GegnerPortraitPfad);
            }
        }


        private string _heldPortraitPfad = null;
        public string HeldPortraitPfad
        {
            get { return _heldPortraitPfad; }
            set
            {
                string prevalue = value;
                if (!prevalue.EndsWith("/") && !prevalue.EndsWith(@"\"))
                    prevalue = prevalue + (IsLocalInstalliert ? @"\" : "/");
                Set(ref _heldPortraitPfad, prevalue);
                Einstellungen.SetEinstellung<string>("FoundryHeldPortraitPfad", HeldPortraitPfad);
            }
        }

        private string _gegnerTokenPfad = null;
        public string GegnerTokenPfad
        {
            get { return _gegnerTokenPfad; }
            set
            {
                string prevalue = value;
                if (!prevalue.EndsWith("/") && !prevalue.EndsWith(@"\"))
                    prevalue = prevalue + (IsLocalInstalliert ? @"\" : "/");
                Set(ref _gegnerTokenPfad, prevalue);
                Einstellungen.SetEinstellung<string>("FoundryGegnerTokenPfad", GegnerTokenPfad);
            }
        }

        private string _heldTokenPfad = null;
        public string HeldTokenPfad
        {
            get { return _heldTokenPfad; }
            set
            {
                string prevalue = value;
                if (!prevalue.EndsWith("/") && !prevalue.EndsWith(@"\"))
                    prevalue = prevalue + (IsLocalInstalliert ? @"\" : "/");
                Set(ref _heldTokenPfad, prevalue);
                Einstellungen.SetEinstellung<string>("FoundryHeldTokenPfad", HeldTokenPfad);
            }
        }

        private string _musikPfad = null;
        public string MusikPfad
        {
            get { return _musikPfad; }
            set
            {
                string prevalue = value;
                if (!prevalue.EndsWith("/") && !prevalue.EndsWith(@"\"))
                    prevalue = prevalue + (IsLocalInstalliert ? @"\" : "/");
                Set(ref _musikPfad, prevalue);
                Einstellungen.SetEinstellung<string>("FoundryMusikPfad", MusikPfad);
            }
        }


        private Held _selectedDBHeld = null;
        public Held SelectedDBHeld
        {
            get { return _selectedDBHeld; }
            set { Set(ref _selectedDBHeld, value); }
        }

        private string _tokenName = null;
        public string TokenName
        {
            get { return _tokenName; }
            set { Set(ref _tokenName, value); }
        }

        private List<string> _lstdisplayName = new List<string>();
        public List<string> lstDisplayName
        {
            get { return _lstdisplayName; }
            set { Set(ref _lstdisplayName, value); }
        }

        private string _displayName = null;
        public string DisplayName
        {
            get { return _displayName; }
            set { Set(ref _displayName, value); }
        }

        private List<string> _lstRepresentedName = new List<string>();
        public List<string> lstRepresentedName
        { 
            get { return _lstRepresentedName; }
            set { Set(ref _lstRepresentedName, value); }
        }

        private string _representedName = null;
        public string RepresentedName
        {
            get { return _representedName; }
            set { Set(ref _representedName, value); }
        }

        private bool _dsa41Version = false;
        public bool DSA41Version
        {
            get { return _dsa41Version; }
            set { Set(ref _dsa41Version, value); }
        }

        private bool _linkActorData = false;
        public bool LinkActorData
        {
            get { return _linkActorData; }
            set { Set(ref _linkActorData, value); }
        }

        private List<string> _lstTokenDisposition = new List<string>();
        public List<string> lstTokenDisposition
        {
            get { return _lstTokenDisposition; }
            set { Set(ref _lstTokenDisposition, value); }
        }

        private string _tokenDisposition = null;
        public string TokenDisposition
        {
            get { return _tokenDisposition; }
            set { Set(ref _tokenDisposition, value); }
        }

        private string _localFoundryPfad = null;
        public string localFoundryPfad
        {
            get { return _localFoundryPfad; }
            set { Set(ref _localFoundryPfad, value); }
        }


        private string _foundryPfad = null;
        public string FoundryPfad
        {
            get { return _foundryPfad; }
            set { Set(ref _foundryPfad, value); }
        }

        private string _playlistStatus = null;
        public string PlaylistStatus
        {
            get { return _playlistStatus; }
            set { Set(ref _playlistStatus, value); }
        }

        private bool _overwritePictureFile = false;
        public bool OverwritePictureFile
        {
            get { return _overwritePictureFile; }
            set { Set(ref _overwritePictureFile, value); }
        }
        private bool _overwritePlaylistFile = false;
        public bool OverwritePlaylistFile
        {
            get { return _overwritePlaylistFile; }
            set { Set(ref _overwritePlaylistFile, value); }
        }

        private bool _copyTitelFile = false;
        public bool CopyTitelFile
        {
            get { return _copyTitelFile; }
            set { Set(ref _copyTitelFile, value); }
        }

        private folder _selectedHeldenFolder= null;
        public folder SelectedHeldenFolder
        {
            get { return _selectedHeldenFolder; }
            set { Set(ref _selectedHeldenFolder, value); }
        }
        private folder _selectedGegnerFolder= null;
        public folder SelectedGegnerFolder
        {
            get { return _selectedGegnerFolder; }
            set { Set(ref _selectedGegnerFolder, value); }
        }

        private string _selectedWorld = null;
        public string SelectedWorld
        {
            get { return _selectedWorld; }
            set 
            { 
                Set(ref _selectedWorld, value);

                if (value != null)
                {
                    if (IsLocalInstalliert)
                        GetActorFolders(string.Format(@"{0}worlds\{1}\data\folders.db", FoundryPfad, value));
                    else
                        GetActorFolders(string.Format("{0}worlds/{1}/data/folders.db", FoundryPfad, value));
                }
            }
        }
        #endregion

        #region //---- FELDER ----


        #endregion

        #region //---- EIGENSCHAFTEN ----

        #endregion

        #region //---- LISTEN ----

        public List<GegnerBase> lstGegnerBase
        {
            get { return Global.ContextHeld.Liste<GegnerBase>().OrderBy(h => h.Name).ToList(); }
        }

        public List<Audio_Playlist> lstPlaylists
        {
            get { return Global.ContextAudio.PlaylistListe.OrderBy(h => h.Name).ToList(); }
        }

        private List<folder> _lstFolders = new List<folder>();
        public List<folder> lstFolders
        {
            get { return _lstFolders; }
            set { Set(ref _lstFolders, value); }
        }

        private List<string> _lstWorlds = new List<string>();
        public List<string> lstWorlds
        {
            get { return _lstWorlds; }
            set { Set(ref _lstWorlds, value); }
        }
        private int _portNo = 0;
        public int PortNo
        {
            get { return _portNo; }
            set { Set(ref _portNo, value); }
        }

        private CefSharp.Wpf.ChromiumWebBrowser _cWebBrowser = new CefSharp.Wpf.ChromiumWebBrowser();
        public CefSharp.Wpf.ChromiumWebBrowser cWebBrowser
        {
            get { return _cWebBrowser; }
            set 
            { 
                Set(ref _cWebBrowser, value);
                cWebBrowser.Address = LocalUri;
            }
        }

        private string _localUri = "http://192.168.178.181:30000/";
        public string LocalUri
        {
            get { return _localUri; }
            set { Set(ref _localUri, value); }
        }
        private string _inetUri = "http://1.2.3.4:30000/";
        public string InetUri
        {
            get { return _inetUri; }
            set { Set(ref _inetUri, value); }
        }
        #endregion

        #region //---- KONSTRUKTOR ----

        public FoundryViewModel()
        {
            FTPAdresse = Einstellungen.GetEinstellung<string>("FoundryFTPAdresse");
            FTPUser = Einstellungen.GetEinstellung<string>("FoundryFTPUser");
            FTPPasswort = Einstellungen.GetEinstellung<string>("FoundryFTPPasswort");

            IsLocalInstalliert = Einstellungen.GetEinstellung<bool>("IsLocalInstalliert");
            
            Init();
            lstRepresentedName.AddRange(lstHeldArgument.Select(t => t.h.Name));
            lstDisplayName.AddRange(new List<string>() {
                "Never Displayed", "When Controlled", "Hovered by Owner", "Hovered by Anyone", "Always for Owner", "Always for Everyone" });
            DisplayName = lstDisplayName.First();
            LinkActorData = false;
            lstTokenDisposition = new List<string>() { "Neutral", "Friendly", "Hostile" };
            TokenDisposition = lstTokenDisposition.Last();

            stdPfad.AddRange(MeisterGeister.Logic.Einstellung.Einstellungen.AudioVerzeichnis.Split(new Char[] { '|' }));
        }

        private List<string> _stdPfad = new List<string>();
        public List<string> stdPfad
        {
            get { return _stdPfad; }
            set { Set(ref _stdPfad, value); }
        }

        private FtpWebRequest _listRequest = null;
        public FtpWebRequest listRequest
        {
            get { return _listRequest; }
            set { Set(ref _listRequest, value); }
        }

        #endregion

        #region //---- Funktionen ----
        void ListFtpDirectory(string url, string rootPath, bool onlyDir, NetworkCredential credentials, List<string> list)
        {
            StreamReader listReader = null;
            var listRequest = (FtpWebRequest)WebRequest.Create(url + rootPath);
            listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            listRequest.Credentials = credentials;
            listRequest.KeepAlive = false;
            listRequest.UsePassive = true;
            listRequest.Timeout= 4000;

            var lines = new List<string>();
            try
            {
                using (var listResponse = (FtpWebResponse)listRequest.GetResponse())
                using (var listStream = listResponse.GetResponseStream())
                using (listReader = new StreamReader(listStream))
                {
                    while (!listReader.EndOfStream)
                    {
                        lines.Add(listReader.ReadLine());
                        if (onlyDir)
                        {
                            string[] tokens =
                                lines.Last().Split(new[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                            string permissions = tokens[0];
                            string name = tokens[8];
                            if (permissions[0] == 'd')
                                list.Add(name);
                        }
                    }
                }

                if (!onlyDir)
                {
                    foreach (string line in lines)
                    {
                        string[] tokens =
                            line.Split(new[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                        string name = tokens[8];
                        string permissions = tokens[0];

                        string filePath = rootPath + name;

                        if (permissions[0] == 'd')
                        {
                            ListFtpDirectory(url, filePath + "/", onlyDir, credentials, list);
                        }
                        else
                        {
                            list.Add(filePath);
                        }
                    }
                }
            }
            catch (Exception ex) {
                ViewHelper.ShowError(string.Format("Beim Lesen der FTP Seite {0} ist ein Fehler aufgetreten", url + rootPath), ex);
            }
            finally
            {
                if (listReader != null)
                    listReader.Close();
            }
        }
        private void LoadWorldsFolder()
        {
            if (IsLocalInstalliert)
            {
                List<string> lst = new List<string>();
                string worldPfad = FoundryPfad + @"worlds";
                List<string> lstFullDir = Directory.GetDirectories(worldPfad).ToList();
                lstFullDir.ForEach(delegate (string s)
                { lst.Add(new Uri(s).Segments.Last().ToUpper()); });

                lstWorlds = lst;
            }
            else
            {
                List<string> list = new List<string>();
                NetworkCredential credentials = new NetworkCredential(FTPUser, FTPPasswort);
                ListFtpDirectory(FTPAdresse + "/data/worlds/", "", true, credentials, list);
                lstWorlds = list;
            }
        }

        private void GetActorFolders(string filepath)
        {
            string FileData = GetFileData(filepath);

            if (FileData != null)
            { 
                List<string> lstFileData = FileData.Split(new Char[] { '\n' }).ToList();
                List<folder> lst = new List<folder>();

                lst.Add(new folder() { name = "" });
                foreach (string data in lstFileData)
                {
                    if (string.IsNullOrEmpty(data))
                        continue;
                    string d = data.Substring(1, data.Length - 3);
                    List<string> line = d.Split(new Char[] { ',' }).ToList();
                    string nameF = line.FirstOrDefault(t => t.StartsWith("\"name\":"));
                    string typF = line.FirstOrDefault(t => t.StartsWith("\"type\":"));
                    string sortingF = line.FirstOrDefault(t => t.StartsWith("\"sorting\":"));
                    string colorF = line.FirstOrDefault(t => t.StartsWith("\"color\":"));
                    string _idF = line.FirstOrDefault(t => t.StartsWith("\"_id\":"));

                    lst.Add(new folder()
                    {
                        name = nameF.Split(new Char[] { ':' }).Last().Replace("\"", ""),
                        typ = typF.Split(new Char[] { ':' }).Last().Replace("\"", ""),
                        sorting = sortingF.Split(new Char[] { ':' }).Last().Replace("\"", ""),
                        color = colorF?.Split(new Char[] { ':' }).Last().Replace("\"", ""),
                        _id = _idF.Split(new Char[] { ':' }).Last().Replace("\"", "")
                    });
                }
                lstFolders = lst;
            }
        }

        private void SetFileData(string file, string daten, bool datenInFile = false)
        {
            if (IsLocalInstalliert)
                File.WriteAllText(file, daten);
            else
            {
                string tempDatei = null;
                if (!datenInFile)
                {
                    tempDatei = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\tempData.txt";
                    File.WriteAllText(tempDatei, daten);
                }
                else
                    tempDatei = daten;

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(file);
                request.Credentials = new NetworkCredential(FTPUser, FTPPasswort);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                using (Stream fileStream = File.OpenRead(tempDatei))
                using (Stream ftpStream = request.GetRequestStream())
                {
                    fileStream.CopyTo(ftpStream);
                }

                if (!datenInFile)
                    File.Delete(tempDatei);
            }
        }
        private string GetFileData(string filepath)
        {
            try
            {
                string FileData = null;
                if (IsLocalInstalliert)
                {
                    try
                    {
                        if (File.Exists(filepath))
                            FileData = File.ReadAllText(filepath).Trim();
                    }
                    catch (Exception ex)
                    {
                        ViewHelper.ShowError("Beim Schreiben der Datei ist ein Fehler aufgetreten", ex);
                    }
                }
                else
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(filepath);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.Credentials = new NetworkCredential(FTPUser, FTPPasswort, "");
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(responseStream);
                    FileData = reader.ReadToEnd().Trim();
                    Console.WriteLine($"Download Complete, status {response.StatusDescription}");
                    reader.Close();
                    response.Close();
                }
                return FileData;
            }
            catch (Exception ex)
            {

                ViewHelper.ShowError(string.Format("Beim Verbinden mit der FTP Seite {0} ist ein Fehler aufgetreten", filepath), ex);
                return null;
            }
        }
        private string GetUserID(string filepath, string username)
        {
            char A = (char)34;
            string id = null;
            string FileData = GetFileData(filepath);

            if (FileData != null)
            { 
                List<string> lstFileData = FileData.Split(new Char[] { '\n' }).ToList();
                string uLine = lstFileData.FirstOrDefault(t => t.Contains(A + "name" + A + ":" + A + username + A));
                string d = uLine.Substring(1, uLine.Length - 3);
                List<string> line = d.Split(new Char[] { ',' }).ToList();
                string _idF = line.FirstOrDefault(t => t.StartsWith(A + "_id" + A + ":"));
                id = _idF.Substring(7, _idF.Length - 8);
            }
            return id;
        }

        #endregion

        #region //---- INSTANZMETHODEN ----

        private void ReadFoundryOptions(string filepath)
        {
            string FileData = GetFileData(filepath);
            if (FileData != null)
            { 
                List<string> lstFileData = FileData.Split(new Char[] { '\n' }).ToList();

                string portLine = lstFileData.FirstOrDefault(t => t.StartsWith("  \"port\":"));
                portLine = portLine.Trim().TrimEnd(new Char[] { ',' });
                int portNo = 0;
                int.TryParse(portLine.Substring(portLine.IndexOf(":") + 1), out portNo);
                PortNo = portNo;

                if (IsLocalInstalliert)
                {
                    string dataPathLine = lstFileData.FirstOrDefault(t => t.StartsWith("  \"dataPath\":"));
                    dataPathLine = dataPathLine.Trim().TrimEnd(new Char[] { ',' }).Trim();
                    string dataPath = dataPathLine.Substring(dataPathLine.IndexOf(":") + 1).Trim().Trim(new Char[] { '"' });
                    dataPath = dataPath.Replace("/", @"\");
                    dataPath += @"\data\";
                    FoundryPfad = dataPath;
                    localFoundryPfad = FoundryPfad;
                    LoadWorldsFolder();
                }
            }
            else
                FoundryPfad = null;
        }
        private void BildSpeichern(string bildDateiname, string WesenPfad, string MGPfad, List<string> lstFTPGegnerPics, List<string> lstDateienKopiert)
        {
            if (!string.IsNullOrEmpty(bildDateiname) && !lstDateienKopiert.Contains(bildDateiname))
            {
                string charBild = bildDateiname;
                string srcCharFilename = System.IO.Path.GetFileName(charBild);
                if (bildDateiname.StartsWith("/"))
                {
                    BitmapImage bmpi1 = new BitmapImage(new Uri("pack://application:,,," + bildDateiname));
                    using (MemoryStream outStream = new MemoryStream())
                    {
                        PngBitmapEncoder enc = new PngBitmapEncoder();
                        enc.Frames.Add(BitmapFrame.Create(bmpi1));

                        FileStream fs = null;
                        if (IsLocalInstalliert)
                        {
                            if (!File.Exists(FoundryPfad + srcCharFilename) || OverwritePictureFile)
                                fs = File.Open(FoundryPfad + srcCharFilename, FileMode.Create);
                        }
                        else
                        {
                            //Speichern temporär ins MG-Verzeichnis 
                            if (!lstFTPGegnerPics.Contains(srcCharFilename) || OverwritePictureFile)
                                fs = File.Open(MGPfad + srcCharFilename, FileMode.Create);
                        }
                        if (fs != null)
                        {
                            enc.Save(fs);
                            fs.Close();
                        }
                        if (!IsLocalInstalliert)
                        {
                            if (!lstFTPGegnerPics.Contains(srcCharFilename) || OverwritePictureFile)
                                SetFileData(FoundryPfad + WesenPfad + System.IO.Path.GetFileName(charBild), MGPfad + srcCharFilename, true);
                            File.Delete(MGPfad + srcCharFilename);
                        }
                    }
                }
                else
                if (File.Exists(charBild))
                {
                    if (IsLocalInstalliert)
                    {
                        if (!File.Exists(FoundryPfad + srcCharFilename) || OverwritePictureFile)
                        {
                            File.Copy(charBild, FoundryPfad + srcCharFilename, OverwritePictureFile);
                        }
                    }
                    else
                    {
                        if (!lstFTPGegnerPics.Contains(srcCharFilename) || OverwritePictureFile)
                        {
                            SetFileData(FoundryPfad + WesenPfad + System.IO.Path.GetFileName(charBild), charBild, true);
                        }
                    }
                }
                lstDateienKopiert.Add(charBild);
            }
        }

        private List<dbArgument> SetDSA41Arguments(GegnerBase g, string GetFilenamePortrait, string GetFilenameToken, string id)
        {
            List<dbArgument> lstArg = new List<dbArgument>();
            char A = (char)34;

            //gArg.lstArguments = new List<dbArgument>();
            lstArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = id, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = g.Name, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "permission" + A + ":{\"default" + A + ":", ArgString = "0,\"3WHJiGe2LNC2VNeR" + A + ":", Suffix = "3}," });
            lstArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "character", Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "data" + A + ":{\"biography" + A + ":\"", ArgString = "", Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "stats" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "health" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = g.LE.ToString(), Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "min" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "max" + A + ":", ArgString = g.LE.ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "endurance" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = g.AU.ToString(), Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "min" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "max" + A + ":", ArgString = g.AU.ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "astral" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = g.AE.ToString(), Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "min" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "max" + A + ":", ArgString = g.AE.ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "karmal" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = g.KE.ToString(), Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "min" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "max" + A + ":", ArgString = g.KE.ToString(), Suffix = "}," });

            lstArg.Add(new dbArgument { Prefix = A + "magic_resistance" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = Math.Max(g.MRKörper.HasValue ? g.MRKörper.Value : 0, g.MRGeist).ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "speed" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = Math.Max(g.MRKörper.HasValue ? g.MRKörper.Value : 0, g.MRGeist).ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "INI" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = g.INIBasis.ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "AT" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = g.AT.ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "PA" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = g.PA.ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "FK" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = g.FK.ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "WS" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = ((int)g.KO / 2).ToString(), Suffix = "}" });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "attributes" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "MU" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (g.MU ?? 0).ToString() });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "KL" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (g.KL ?? 0).ToString() });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "IN" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (g.IN ?? 0).ToString() });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "CH" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (g.CH ?? 0).ToString() });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "FF" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (g.FF ?? 0).ToString() });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "GE" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (g.GE ?? 0).ToString() });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "KO" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = g.KO.ToString() });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "KK" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (g.KK ?? 0).ToString() });
            lstArg.Add(new dbArgument { Prefix = "}}}," });

            //Folder
            if (SelectedGegnerFolder != null && !string.IsNullOrEmpty(SelectedGegnerFolder.name))
            {
                lstArg.Add(new dbArgument { Prefix = A + "folder" + A + ":\"", ArgString = SelectedGegnerFolder._id, Suffix = "\"," });
            }

            lstArg.Add(new dbArgument { Prefix = A + "sort" + A + ":", ArgString = "100001", Suffix = "," });

            lstArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "core" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "sourceId" + A + ":\"", ArgString = "Compendium.world.dsa-gegner.KBxxxx0000000001", Suffix = A + "}" });
            lstArg.Add(new dbArgument { Prefix = "}," });

            //Portrait Image
            lstArg.Add(new dbArgument
            {
                Prefix = A + "img" + A + ":\"",
                ArgString =
                (string.IsNullOrEmpty(GetFilenamePortrait) ? "icons/svg/mystery-man.svg" :
                 GetFilenamePortrait),
                Suffix = A + ","
            });

            lstArg.Add(new dbArgument { Prefix = A + "token" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = g.Name, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "displayName" + A + ":", ArgString = "0", Suffix = "," });

            //Token Image
            lstArg.Add(new dbArgument
            {
                Prefix = A + "img" + A + ":\"",
                ArgString =
                (string.IsNullOrEmpty(GetFilenameToken) ? "icons/svg/mystery-man.svg" :
                GetFilenameToken),
                Suffix = A + ","
            });

            lstArg.Add(new dbArgument { Prefix = A + "tint" + A + ":", ArgString = "null", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "width" + A + ":", ArgString = "1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "height" + A + ":", ArgString = "1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "scale" + A + ":", ArgString = "1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "lockRotation" + A + ":", ArgString = "false", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "vision" + A + ":", ArgString = "false", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "dimSight" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "brightSight" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "dimLight" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "brightLight" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "sightAngle" + A + ":", ArgString = "360", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "lightAngle" + A + ":", ArgString = "360", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "lightAlpha" + A + ":", ArgString = "1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "lightAnimation" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "speed" + A + ":", ArgString = "5", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "intensity" + A + ":", ArgString = "5" });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "actorId" + A + ":\"", ArgString = id, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "actorLink" + A + ":", ArgString = "false", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "disposition" + A + ":", ArgString = "-1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "displayBars" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "bar1" + A + ":{", ArgString = "", Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "bar2" + A + ":{", ArgString = "", Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "randomImg" + A + ":", ArgString = "false", Suffix = "" });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "items" + A + ":[", ArgString = "", Suffix = "]," });
            lstArg.Add(new dbArgument { Prefix = A + "effects" + A + ":[", ArgString = "", Suffix = "]" });
            lstArg.Add(new dbArgument { Prefix = "}" });

            return lstArg;

        }

        private List<dbArgument> SetDSA41Arguments(Held h, string GetFilenamePortrait, string GetFilenameToken, string id)
        {

            List<dbArgument> lstArg = new List<dbArgument>();
            char A = (char)34;

            lstArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = id, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = h.Name, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "permission" + A + ":{\"default" + A + ":", ArgString = "0,\"3WHJiGe2LNC2VNeR" + A + ":", Suffix = "3}," });
            lstArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "character", Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "data" + A + ":{\"biography" + A + ":\"", ArgString = "", Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "stats" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "health" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = h.LebensenergieAktuell.ToString(), Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "min" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "max" + A + ":", ArgString = h.LebensenergieMax.ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "endurance" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = h.AusdauerAktuell.ToString(), Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "min" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "max" + A + ":", ArgString = h.AusdauerMax.ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "astral" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = h.AstralenergieAktuell.ToString(), Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "min" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "max" + A + ":", ArgString = h.AstralenergieMax.ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "karmal" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = h.KarmaenergieAktuell.ToString(), Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "min" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "max" + A + ":", ArgString = h.KarmaenergieMax.ToString(), Suffix = "}," });

            lstArg.Add(new dbArgument { Prefix = A + "magic_resistance" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = h.Magieresistenz.ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "speed" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = h.Geschwindigkeit.ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "INI" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = h.Initiative(false).ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "AT" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = (h.AT ?? 0).ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "PA" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = (h.PA ?? 0).ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "FK" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = h.FernkampfBasis.ToString(), Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "WS" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = h.Wundschwelle.ToString(), Suffix = "}" });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "attributes" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "MU" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (h.MU ?? 0).ToString() });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "KL" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (h.KL ?? 0).ToString() });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "IN" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (h.IN ?? 0).ToString() });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "CH" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (h.CH ?? 0).ToString() });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "FF" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (h.FF ?? 0).ToString() });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "GE" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (h.GE ?? 0).ToString() });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "KO" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (h.KO ?? 0).ToString() });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "KK" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (h.KK ?? 0).ToString() });
            lstArg.Add(new dbArgument { Prefix = "}}}," });

            //Folder
            if (SelectedHeldenFolder != null && !string.IsNullOrEmpty(SelectedHeldenFolder.name))
            {
                lstArg.Add(new dbArgument { Prefix = A + "folder" + A + ":\"", ArgString = SelectedHeldenFolder._id, Suffix = "\"," });
            }
            lstArg.Add(new dbArgument { Prefix = A + "sort" + A + ":", ArgString = "100001", Suffix = "," });

            lstArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "core" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "sourceId" + A + ":\"", ArgString = "Compendium.world.dsa-gegner.KBxxxx0000000001", Suffix = A + "}" });
            lstArg.Add(new dbArgument { Prefix = "}," });

            //Portrait Image
            lstArg.Add(new dbArgument
            {
                Prefix = A + "img" + A + ":\"",
                ArgString =
                (string.IsNullOrEmpty(GetFilenamePortrait) ? "icons/svg/mystery-man.svg" :
                 GetFilenamePortrait),
                Suffix = A + ","
            });

            lstArg.Add(new dbArgument { Prefix = A + "token" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = h.Name, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "displayName" + A + ":", ArgString = "0", Suffix = "," });

            //Token Image
            lstArg.Add(new dbArgument
            {
                Prefix = A + "img" + A + ":\"",
                ArgString =
                (string.IsNullOrEmpty(GetFilenameToken) ? "icons/svg/mystery-man.svg" :
                GetFilenameToken),
                Suffix = A + ","
            });

            lstArg.Add(new dbArgument { Prefix = A + "tint" + A + ":", ArgString = "null", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "width" + A + ":", ArgString = "1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "height" + A + ":", ArgString = "1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "scale" + A + ":", ArgString = "1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "lockRotation" + A + ":", ArgString = "false", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "vision" + A + ":", ArgString = "true", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "dimSight" + A + ":", ArgString = "20", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "brightSight" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "dimLight" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "brightLight" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "sightAngle" + A + ":", ArgString = "180", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "lightAngle" + A + ":", ArgString = "180", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "lightAlpha" + A + ":", ArgString = "1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "lightAnimation" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "speed" + A + ":", ArgString = "5", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "intensity" + A + ":", ArgString = "5" });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "actorId" + A + ":\"", ArgString = id, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "actorLink" + A + ":", ArgString = "false", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "disposition" + A + ":", ArgString = "-1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "displayBars" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "bar1" + A + ":{", ArgString = "", Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "bar2" + A + ":{", ArgString = "", Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "randomImg" + A + ":", ArgString = "false", Suffix = "" });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "items" + A + ":[", ArgString = "", Suffix = "]," });
            lstArg.Add(new dbArgument { Prefix = A + "effects" + A + ":[", ArgString = "", Suffix = "]" });
            lstArg.Add(new dbArgument { Prefix = "}" });

            return lstArg;


            //hArg.outcome = "";
            //foreach (dbArgument arg in lstArg)
            //{ hArg.outcome += arg.Prefix + arg.ArgString + arg.Suffix; }

            //lstHeldArgument.Add(hArg);




            ////////////////////////////////////////////////////////////////////////////////////////////////////////

            ////gArg.lstArguments = new List<dbArgument>();
            //lstArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = id, Suffix = A + "," });
            //lstArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = g.Name, Suffix = A + "," });
            //lstArg.Add(new dbArgument { Prefix = A + "permission" + A + ":{\"default" + A + ":", ArgString = "0,\"3WHJiGe2LNC2VNeR" + A + ":", Suffix = "3}," });
            //lstArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "character", Suffix = A + "," });
            //lstArg.Add(new dbArgument { Prefix = A + "data" + A + ":{\"biography" + A + ":\"", ArgString = "", Suffix = A + "," });
            //lstArg.Add(new dbArgument { Prefix = A + "stats" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "health" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = g.LE.ToString(), Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "min" + A + ":", ArgString = "0", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "max" + A + ":", ArgString = g.LE.ToString(), Suffix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "endurance" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = g.AU.ToString(), Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "min" + A + ":", ArgString = "0", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "max" + A + ":", ArgString = g.AU.ToString(), Suffix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "astral" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = g.AE.ToString(), Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "min" + A + ":", ArgString = "0", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "max" + A + ":", ArgString = g.AE.ToString(), Suffix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "karmal" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = g.KE.ToString(), Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "min" + A + ":", ArgString = "0", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "max" + A + ":", ArgString = g.KE.ToString(), Suffix = "}," });

            //lstArg.Add(new dbArgument { Prefix = A + "magic_resistance" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = Math.Max(g.MRKörper.HasValue ? g.MRKörper.Value : 0, g.MRGeist).ToString(), Suffix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "speed" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = Math.Max(g.MRKörper.HasValue ? g.MRKörper.Value : 0, g.MRGeist).ToString(), Suffix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "INI" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = g.INIBasis.ToString(), Suffix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "AT" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = g.AT.ToString(), Suffix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "PA" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = g.PA.ToString(), Suffix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "FK" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = g.FK.ToString(), Suffix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "WS" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = ((int)g.KO / 2).ToString(), Suffix = "}" });
            //lstArg.Add(new dbArgument { Prefix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "attributes" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "MU" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (g.MU ?? 0).ToString() });
            //lstArg.Add(new dbArgument { Prefix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "KL" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (g.KL ?? 0).ToString() });
            //lstArg.Add(new dbArgument { Prefix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "IN" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (g.IN ?? 0).ToString() });
            //lstArg.Add(new dbArgument { Prefix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "CH" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (g.CH ?? 0).ToString() });
            //lstArg.Add(new dbArgument { Prefix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "FF" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (g.FF ?? 0).ToString() });
            //lstArg.Add(new dbArgument { Prefix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "GE" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (g.GE ?? 0).ToString() });
            //lstArg.Add(new dbArgument { Prefix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "KO" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = g.KO.ToString() });
            //lstArg.Add(new dbArgument { Prefix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "KK" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = "8", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = "0", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (g.KK ?? 0).ToString() });
            //lstArg.Add(new dbArgument { Prefix = "}}}," });

            ////Folder
            //if (SelectedGegnerFolder != null && !string.IsNullOrEmpty(SelectedGegnerFolder.name))
            //{
            //    lstArg.Add(new dbArgument { Prefix = A + "folder" + A + ":\"", ArgString = SelectedGegnerFolder._id, Suffix = "\"," });
            //}

            //lstArg.Add(new dbArgument { Prefix = A + "sort" + A + ":", ArgString = "100001", Suffix = "," });

            //lstArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "core" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "sourceId" + A + ":\"", ArgString = "Compendium.world.dsa-gegner.KBxxxx0000000001", Suffix = A + "}" });
            //lstArg.Add(new dbArgument { Prefix = "}," });

            ////Portrait Image
            //lstArg.Add(new dbArgument
            //{
            //    Prefix = A + "img" + A + ":\"",
            //    ArgString =
            //    (string.IsNullOrEmpty(GetFilenamePortrait) ? "icons/svg/mystery-man.svg" :
            //     GetFilenamePortrait),
            //    Suffix = A + ","
            //});

            //lstArg.Add(new dbArgument { Prefix = A + "token" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = g.Name, Suffix = A + "," });
            //lstArg.Add(new dbArgument { Prefix = A + "displayName" + A + ":", ArgString = "0", Suffix = "," });

            ////Token Image
            //lstArg.Add(new dbArgument
            //{
            //    Prefix = A + "img" + A + ":\"",
            //    ArgString =
            //    (string.IsNullOrEmpty(GetFilenameToken) ? "icons/svg/mystery-man.svg" :
            //    GetFilenameToken),
            //    Suffix = A + ","
            //});

            //lstArg.Add(new dbArgument { Prefix = A + "tint" + A + ":", ArgString = "null", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "width" + A + ":", ArgString = "1", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "height" + A + ":", ArgString = "1", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "scale" + A + ":", ArgString = "1", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "lockRotation" + A + ":", ArgString = "false", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "vision" + A + ":", ArgString = "false", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "dimSight" + A + ":", ArgString = "0", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "brightSight" + A + ":", ArgString = "0", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "dimLight" + A + ":", ArgString = "0", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "brightLight" + A + ":", ArgString = "0", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "sightAngle" + A + ":", ArgString = "360", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "lightAngle" + A + ":", ArgString = "360", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "lightAlpha" + A + ":", ArgString = "1", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "lightAnimation" + A + ":{" });
            //lstArg.Add(new dbArgument { Prefix = A + "speed" + A + ":", ArgString = "5", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "intensity" + A + ":", ArgString = "5" });
            //lstArg.Add(new dbArgument { Prefix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "actorId" + A + ":\"", ArgString = id, Suffix = A + "," });
            //lstArg.Add(new dbArgument { Prefix = A + "actorLink" + A + ":", ArgString = "false", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "disposition" + A + ":", ArgString = "-1", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "displayBars" + A + ":", ArgString = "0", Suffix = "," });
            //lstArg.Add(new dbArgument { Prefix = A + "bar1" + A + ":{", ArgString = "", Suffix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "bar2" + A + ":{", ArgString = "", Suffix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "randomImg" + A + ":", ArgString = "false", Suffix = "" });
            //lstArg.Add(new dbArgument { Prefix = "}," });
            //lstArg.Add(new dbArgument { Prefix = A + "items" + A + ":[", ArgString = "", Suffix = "]," });
            //lstArg.Add(new dbArgument { Prefix = A + "effects" + A + ":[", ArgString = "", Suffix = "]" });
            //lstArg.Add(new dbArgument { Prefix = "}" });

            //return lstArg;

        }


        private List<dbArgument> AddGegnerArgument(string eigenschaft, string color, int? wert, string lastEigenschaft = ",")
        {
            List<dbArgument> addArg = new List<dbArgument>();
            char A = (char)34;

            addArg.Add(new dbArgument { Prefix = A + eigenschaft + A + ":{" });
            addArg.Add(new dbArgument { Prefix = A + "label" + A + ":" + A + "CHAR."+ eigenschaft.ToUpper() + A, Suffix = "," });
            addArg.Add(new dbArgument { Prefix = A + "abrev" + A + ":" + A + "CHARAbbrev."+ eigenschaft.ToUpper() + A, Suffix = "," });
            addArg.Add(new dbArgument { Prefix = A + "color" + A + ":" + A + color + A, Suffix = "," });
            addArg.Add(new dbArgument { Prefix = A + "initial" + A + ":" + (wert??0), Suffix = "," });
            addArg.Add(new dbArgument { Prefix = A + "species" + A + ":0", Suffix = "," });
            addArg.Add(new dbArgument { Prefix = A + "modifier" + A + ":0", Suffix = "," });
            addArg.Add(new dbArgument { Prefix = A + "advances" + A + ":0", Suffix = "," });
            addArg.Add(new dbArgument { Prefix = A + "value" + A + ":"+(wert??0).ToString(), Suffix = "," });
            addArg.Add(new dbArgument { Prefix = A + "bonus" + A + ":0", Suffix = "," });
            addArg.Add(new dbArgument { Prefix = A + "cost" + A + ":" + A + A, Suffix = "," });
            addArg.Add(new dbArgument { Prefix = A + "refund" + A + ":" + A + A, Suffix = "}" + lastEigenschaft });
            return addArg;
        }

        private List<dbArgument> AddGegnerStatus(string status, string Iinitial, string Ivalue, string Iadvances, string Imodifier, string Icurrent, string last = ",")
        {
            List<dbArgument> addArg = new List<dbArgument>();
            char A = (char)34;

            addArg.Add(new dbArgument { Prefix = A + status + A + ":{" });
            if (Iinitial != null)
                addArg.Add(new dbArgument { Prefix = A + "initial" + A + ":", ArgString = Iinitial, Suffix = "," });
            if (Ivalue != null && status != "speed")
                addArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = Ivalue, Suffix = "," });
            if (Iadvances != null)
                addArg.Add(new dbArgument { Prefix = A + "advances" + A + ":", ArgString = Iadvances, Suffix = "," });
            if (Imodifier != null)
                addArg.Add(new dbArgument { Prefix = A + "modifier" + A + ":", ArgString = Imodifier, Suffix = "," });
            if (Ivalue != null && status == "speed")
                addArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = Ivalue, Suffix = "," });
            if (Icurrent != null)
                addArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = Icurrent, Suffix = "," });

                addArg.Add(new dbArgument { Prefix = A + "gearmodifier" + A + ":", ArgString = "0", Suffix = "," });
            addArg.Add(new dbArgument { Prefix = A + "max" + A + ":", ArgString = Icurrent?? Ivalue??"0", Suffix = "}" });

            addArg.Last().Suffix = last;
            return addArg;
        }

        private List<dbArgument> AddStats(string stat, int? Svalue = null, int? Smin = null, int? Smax = null, string last = ",")
        {
            List<dbArgument> addArg = new List<dbArgument>();
            char A = (char)34;

            addArg.Add(new dbArgument { Prefix = A + stat + A + ":{" });
            if (Svalue != null)
                addArg.Add(new dbArgument { Prefix = A + "value" + A + ":", ArgString = Svalue.ToString(), Suffix = "," });
            if (Smin != null)
                addArg.Add(new dbArgument { Prefix = A + "min" + A + ":", ArgString = Smin.ToString(), Suffix = "," });
            if (Smax != null)
                addArg.Add(new dbArgument { Prefix = A + "max" + A + ":", ArgString = Smax.ToString(), Suffix = "}," });
            if (last != ",")
                addArg.Last().Suffix = last;
            return addArg;
        }

        private List<dbArgument> AddAttributes(string attrib, int? Astart = null, int? Amod = null, int? Acurrent = null, string last = ",")
        {
            List<dbArgument> addArg = new List<dbArgument>();
            char A = (char)34;

            addArg.Add(new dbArgument { Prefix = A + attrib + A + ":{" });
            addArg.Add(new dbArgument { Prefix = A + "start" + A + ":", ArgString = (Astart ?? 0).ToString(), Suffix = "," });
            addArg.Add(new dbArgument { Prefix = A + "mod" + A + ":", ArgString = (Amod ?? 0).ToString(), Suffix = "," });
            addArg.Add(new dbArgument { Prefix = A + "current" + A + ":", ArgString = (Acurrent ?? 0).ToString(), Suffix = "}," });

            if (last != ",")
                addArg.Last().Suffix = last;
            return addArg;
        }
        private List<dbArgument> AddZauberArgument(GegnerBase g)
        {
            List<dbArgument> addArg = new List<dbArgument>();
            char A = (char)34;

            if (g.GegnerBase_Zauber.Count == 0) 
                return addArg;

            g.GegnerBase_Zauber.ToList().ForEach(delegate (GegnerBase_Zauber gz)
            {
                string atGuid = Guid.NewGuid().ToString().Substring(19, 17).Replace("-", "");
                addArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = atGuid, Suffix = A + "," });
                addArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = gz.Zauber.Name, Suffix = A + "," });
                addArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "spell", Suffix = A + "," });
                addArg.Add(new dbArgument { Prefix = A + "data" + A + ":{" });
                addArg.Add(new dbArgument { Prefix = A + "description" + A + ":{\"", ArgString = "value" + A + ":" + A + gz.Bemerkung, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "gmdescription" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "characteristic1" + A + ":{\"", ArgString = "value" + A + ":" + A + gz.Zauber.Eigenschaft1.ToLower(), Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "characteristic2" + A + ":{\"", ArgString = "value" + A + ":" + A + gz.Zauber.Eigenschaft2.ToLower(), Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "characteristic3" + A + ":{\"", ArgString = "value" + A + ":" + A + gz.Zauber.Eigenschaft3.ToLower(), Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "castingTime" + A + ":{\"", ArgString = "value" + A + ":" + A + gz.Zauber.Zauberdauer, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "AsPCost" + A + ":{\"", ArgString = "value" + A + ":" + A + gz.Zauber.AsPKosten, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "AsPCostDetail" + A + ":{\"", ArgString = "value" + A + ":" + A + gz.Zauber.Kosten, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "maintainCost" + A + ":{\"", ArgString = "value" + A + ":" + A + "0", Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "distribution" + A + ":{\"", ArgString = "value" + A + ":" + A + gz.Zauber.Repräsentationen, Suffix = A + "}," });
                //Spalte F gibt es in DSA 5 nicht
                addArg.Add(new dbArgument { Prefix = A + "StF" + A + ":{\"", ArgString = "value" + A + ":" + A + 
                    (gz.Zauber.Komplex != "F" ? gz.Zauber.Komplex : "E"), Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "StF" + A + ":{\"", ArgString = "value" + A + ":" + A + gz.Zauber.Komplex, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "resistanceModifier" + A + ":{\"", ArgString = "value" + A + ":" + A + "SK", Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "canChangeCastingTime" + A + ":{\"", ArgString = "value" + A + ":" + A + "true", Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "canChangeCost" + A + ":{\"", ArgString = "value" + A + ":" + A + "true", Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "canChangeRange" + A + ":{\"", ArgString = "value" + A + ":" + A + "true", Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "effectFormula" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "range" + A + ":{\"", ArgString = "value" + A + ":" + A + gz.Zauber.Reichweite, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "duration" + A + ":{\"", ArgString = "value" + A + ":" + A + gz.Zauber.Wirkungsdauer, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "targetCategory" + A + ":{\"", ArgString = "value" + A + ":" + A + gz.Zauber.Zieleinheit, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "talentValue" + A + ":{\"", ArgString = "value" + A + ":" + A + (gz.ZfW ?? 0).ToString(), Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "feature" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}}," });

                addArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{", Suffix = "}," });
                addArg.Add(new dbArgument { Prefix = A + "img" + A + ":", ArgString = A + "systems/dsa5/icons/categories/Spell.webp", Suffix = A + "," });
                addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":", ArgString = "[]" });
                addArg.Add(new dbArgument { Suffix = "}," });
            });

            return addArg;
        }
        private List<dbArgument> AddZauberArgument(Model.Held h)
        {
            List<dbArgument> addArg = new List<dbArgument>();
            char A = (char)34;

            if (h.Held_Zauber.Count == 0)
                return addArg;

            h.Held_Zauber.ToList().ForEach(delegate (Held_Zauber hz)
            {
                string atGuid = Guid.NewGuid().ToString().Substring(19, 17).Replace("-", "");
                addArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = atGuid, Suffix = A + "," });
                addArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = hz.Zauber.Name, Suffix = A + "," });
                addArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "spell", Suffix = A + "," });
                addArg.Add(new dbArgument { Prefix = A + "data" + A + ":{" });
                addArg.Add(new dbArgument { Prefix = A + "description" + A + ":{\"", ArgString = "value" + A + ":" + A + hz.Zauber.Bemerkung, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "gmdescription" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "characteristic1" + A + ":{\"", ArgString = "value" + A + ":" + A + hz.Zauber.Eigenschaft1.ToLower(), Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "characteristic2" + A + ":{\"", ArgString = "value" + A + ":" + A + hz.Zauber.Eigenschaft2.ToLower(), Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "characteristic3" + A + ":{\"", ArgString = "value" + A + ":" + A + hz.Zauber.Eigenschaft3.ToLower(), Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "castingTime" + A + ":{\"", ArgString = "value" + A + ":" + A + hz.Zauber.Zauberdauer, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "AsPCost" + A + ":{\"", ArgString = "value" + A + ":" + A + hz.Zauber.AsPKosten, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "AsPCostDetail" + A + ":{\"", ArgString = "value" + A + ":" + A + hz.Zauber.Kosten, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "maintainCost" + A + ":{\"", ArgString = "value" + A + ":" + A + "0", Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "distribution" + A + ":{\"", ArgString = "value" + A + ":" + A + hz.Zauber.Repräsentationen, Suffix = A + "}," });
                //Spalte F gibt es in DSA 5 nicht
                addArg.Add(new dbArgument { Prefix = A + "StF" + A + ":{\"", ArgString = "value" + A + ":" + A +
                    (hz.Zauber.Komplex != "F"? hz.Zauber.Komplex:"E"), Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "resistanceModifier" + A + ":{\"", ArgString = "value" + A + ":" + A + "SK", Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "canChangeCastingTime" + A + ":{\"", ArgString = "value" + A + ":" + A + "true", Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "canChangeCost" + A + ":{\"", ArgString = "value" + A + ":" + A + "true", Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "canChangeRange" + A + ":{\"", ArgString = "value" + A + ":" + A + "true", Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "effectFormula" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "range" + A + ":{\"", ArgString = "value" + A + ":" + A + hz.Zauber.Reichweite, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "duration" + A + ":{\"", ArgString = "value" + A + ":" + A + hz.Zauber.Wirkungsdauer, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "targetCategory" + A + ":{\"", ArgString = "value" + A + ":" + A + hz.Zauber.Zieleinheit, Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "talentValue" + A + ":{\"", ArgString = "value" + A + ":" + A + (hz.ZfW ?? 0).ToString(), Suffix = A + "}," });
                addArg.Add(new dbArgument { Prefix = A + "feature" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}}," });

                addArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{", Suffix = "}," });
                addArg.Add(new dbArgument { Prefix = A + "img" + A + ":", ArgString = A + "systems/dsa5/icons/categories/Spell.webp", Suffix = A + "," });
                addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":", ArgString = "[]" });
                addArg.Add(new dbArgument { Suffix = "}," });
            });

            return addArg;
        }
        private List<dbArgument> AddAngriffeArgument(GegnerBase g)
        {

            List<dbArgument> addArg = new List<dbArgument>();
            char A = (char)34;

            if (g.GegnerBase_Angriff.Count > 0)
            {
                g.GegnerBase_Angriff.ToList().ForEach(delegate (GegnerBase_Angriff ga)
                {
                    string atGuid = Guid.NewGuid().ToString().Substring(19, 17).Replace("-", "");
                    addArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = atGuid, Suffix = A + "," });
                    addArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = ga.Name, Suffix = A + "," });
                    addArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "trait", Suffix = A + "," });
                    addArg.Add(new dbArgument { Prefix = A + "data" + A + ":{" });
                    addArg.Add(new dbArgument { Prefix = A + "description" + A + ":{\"", ArgString = "value" + A + ":" + A + "Natürliche Waffe", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "gmdescription" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "APValue" + A + ":{\"", ArgString = "value" + A + ":" + A + "0", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "traitType" + A + ":{\"", ArgString = "value" + A + ":" + A + "meleeAttack", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "at" + A + ":{\"", ArgString = "value" + A + ":" + A + ga.AT, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "pa" + A + ":{\"", ArgString = "value" + A + ":" + A + ga.PA, Suffix = A + "}," });
                    string dk = ga.DK == null ? "medium" : ga.DK.Contains("H") ? "short" : ga.DK.Contains("N") ? "medium" : ga.DK.Contains("S") ? "long" : ga.DK.Contains("P") ? "wide" : "medium";
                    addArg.Add(new dbArgument { Prefix = A + "reach" + A + ":{\"", ArgString = "value" + A + ":" + A + dk, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "damage" + A + ":{\"", ArgString = "value" + A + ":" + A + ga.TPWürfelAnzahl + "W" + ga.TPWürfel + "+" + ga.TPBonus, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "reloadTime" + A + ":{\"", ArgString = "value" + A + ":" + A + "1", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "AsPCost" + A + ":{\"", ArgString = "value" + A + ":" + A + "0", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "duration" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "aspect" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}}," });

                    addArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{", Suffix = "}," });
                    addArg.Add(new dbArgument { Prefix = A + "img" + A + ":", ArgString = A + "systems/dsa5/icons/categories/trait.webp", Suffix = A + "," });
                    addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":", ArgString = "[]" });
                    addArg.Add(new dbArgument { Suffix = "}," });
                });
            }
            else
            {
                if (g.AT != 0)
                {
                    string atGuid = Guid.NewGuid().ToString().Substring(19, 17).Replace("-", "");
                    addArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = atGuid, Suffix = A + "," });
                    addArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = "Nahkampf", Suffix = A + "," });
                    addArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "trait", Suffix = A + "," });
                    addArg.Add(new dbArgument { Prefix = A + "data" + A + ":{" });
                    addArg.Add(new dbArgument { Prefix = A + "description" + A + ":{\"", ArgString = "value" + A + ":" + A + "Natürliche Waffe", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "gmdescription" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "APValue" + A + ":{\"", ArgString = "value" + A + ":" + A + "0", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "traitType" + A + ":{\"", ArgString = "value" + A + ":" + A + "meleeAttack", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "at" + A + ":{\"", ArgString = "value" + A + ":" + A + g.AT, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "reach" + A + ":{\"", ArgString = "value" + A + ":" + A + "medium", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "damage" + A + ":{\"", ArgString = "value" + A + ":" + A + "1W6", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "reloadTime" + A + ":{\"", ArgString = "value" + A + ":" + A + "1", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "AsPCost" + A + ":{\"", ArgString = "value" + A + ":" + A + "0", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "duration" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "aspect" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}}," });

                    addArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{", Suffix = "}," });
                    addArg.Add(new dbArgument { Prefix = A + "img" + A + ":", ArgString = A + "systems/dsa5/icons/categories/trait.webp", Suffix = A + "," });
                    addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":", ArgString = "[]" });
                    addArg.Add(new dbArgument { Suffix = "}," });
                }
                if (g.FK != 0)
                {
                    string atGuid = Guid.NewGuid().ToString().Substring(19, 17).Replace("-", "");
                    addArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = atGuid, Suffix = A + "," });
                    addArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = "Fernkampf", Suffix = A + "," });
                    addArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "trait", Suffix = A + "," });
                    addArg.Add(new dbArgument { Prefix = A + "data" + A + ":{" });
                    addArg.Add(new dbArgument { Prefix = A + "description" + A + ":{\"", ArgString = "value" + A + ":" + A + "Natürliche Waffe", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "gmdescription" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "APValue" + A + ":{\"", ArgString = "value" + A + ":" + A + "0", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "traitType" + A + ":{\"", ArgString = "value" + A + ":" + A + "meleeAttack", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "at" + A + ":{\"", ArgString = "value" + A + ":" + A + g.AT, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "reach" + A + ":{\"", ArgString = "value" + A + ":" + A + "medium", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "damage" + A + ":{\"", ArgString = "value" + A + ":" + A + "1W6", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "reloadTime" + A + ":{\"", ArgString = "value" + A + ":" + A + "1", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "AsPCost" + A + ":{\"", ArgString = "value" + A + ":" + A + "0", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "duration" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "aspect" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}}," });

                    addArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{", Suffix = "}," });
                    addArg.Add(new dbArgument { Prefix = A + "img" + A + ":", ArgString = A + "systems/dsa5/icons/categories/trait.webp", Suffix = A + "," });
                    addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":", ArgString = "[]" });
                    addArg.Add(new dbArgument { Suffix = "}," });
                }
            }
            return addArg;
        }
        private List<dbArgument> AddAngriffeArgument(Held h)
        {

            List<dbArgument> addArg = new List<dbArgument>();
            char A = (char)34;

            if (h.Angriffswaffen.Count > 0)
            {
                h.Angriffswaffen.ToList().ForEach(delegate (Kampf.Logic.INahkampfwaffe nw)
                {
                    string atGuid = Guid.NewGuid().ToString().Substring(19, 17).Replace("-", "");
                    addArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = atGuid, Suffix = A + "," });
                    addArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = nw.Name, Suffix = A + "," });
                    addArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "trait", Suffix = A + "," });
                    addArg.Add(new dbArgument { Prefix = A + "data" + A + ":{" });
                    addArg.Add(new dbArgument { Prefix = A + "description" + A + ":{\"", ArgString = "value" + A + ":" + A + nw.Name, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "gmdescription" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "APValue" + A + ":{\"", ArgString = "value" + A + ":" + A + "0", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "traitType" + A + ":{\"", ArgString = "value" + A + ":" + A + "meleeAttack", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "at" + A + ":{\"", ArgString = "value" + A + ":" + A + nw.AT, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "pa" + A + ":{\"", ArgString = "value" + A + ":" + A + nw.PA, Suffix = A + "}," });
                    string dk = nw.Distanzklasse == 0x0? "medium" : 
                        nw.Distanzklasse.ToString().Contains("H") ? "short" : 
                        nw.Distanzklasse.ToString().Contains("N") ? "medium" : 
                        nw.Distanzklasse.ToString().Contains("S") ? "long" : 
                        nw.Distanzklasse.ToString().Contains("P") ? "wide" : "medium";
                    addArg.Add(new dbArgument { Prefix = A + "reach" + A + ":{\"", ArgString = "value" + A + ":" + A + dk, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "damage" + A + ":{\"", ArgString = "value" + A + ":" + A + nw.TPWürfelAnzahl + "W" + nw.TPWürfel + "+" + nw.TPBonus, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "reloadTime" + A + ":{\"", ArgString = "value" + A + ":" + A + "1", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "AsPCost" + A + ":{\"", ArgString = "value" + A + ":" + A + "0", Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "duration" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "aspect" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
                    addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}}," });

                    addArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{", Suffix = "}," });
                    addArg.Add(new dbArgument { Prefix = A + "img" + A + ":", ArgString = A + "systems/dsa5/icons/categories/trait.webp", Suffix = A + "," });
                    addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":", ArgString = "[]" });
                    addArg.Add(new dbArgument { Suffix = "}," });
                });
            }

            return addArg;
        }

        private void AddKampfTalent(List<dbArgument> addArg, GegnerBase g, string dsa5name, string name, string StF, string guidevalue, int pa, int at, int TaW, string wtype)
        {
            char A = (char)34;
            string atGuid = Guid.NewGuid().ToString().Substring(19, 17).Replace("-", "");
            addArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = atGuid, Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = name, Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "combatskill", Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "data" + A + ":{" });
            addArg.Add(new dbArgument { Prefix = A + "StF" + A + ":{\"", ArgString = "value" + A + ":" + A + StF + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Advancementfactor", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "guidevalue" + A + ":{\"", ArgString = "value" + A + ":" + A + guidevalue + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Guide value", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "parry" + A + ":{\"", ArgString = "value" + A + ":" + g.PA + "," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "parryValue", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "attack" + A + ":{\"", ArgString = "value" + A + ":" +g.AT + "," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "attackValue", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "talentValue" + A + ":{\"", ArgString = "value" + A + ":0," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "attackValue", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "weapontype" + A + ":{\"", ArgString = "value" + A + ":" + A + wtype + A + ","  + A + "label" + A + ":" + A + "weapontype"+ A + "," +A+"type" + A + ":" + A + "String" , Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "description" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}}," });

            addArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{", Suffix = "}," });
            addArg.Add(new dbArgument { Prefix = A + "img" + A + ":", ArgString = A + "icons/environment/people/charge.webp", Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":", ArgString = "[]" });
            addArg.Add(new dbArgument { Suffix = "}," });
        }
        private void AddKampfTalent(List<dbArgument> addArg, Held h, string dsa5name, string name, string StF, string guidevalue, int pa, int at, int TaW, string wtype)
        {
            char A = (char)34;
            Held_Talent ht = h.Kampftalente.FirstOrDefault(t => t.Talentname == name);
            if (ht == null)
                return;

            string atGuid = Guid.NewGuid().ToString().Substring(19, 17).Replace("-", "");
            addArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = atGuid, Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = dsa5name, Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "combatskill", Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "data" + A + ":{" });
            addArg.Add(new dbArgument { Prefix = A + "StF" + A + ":{\"", ArgString = "value" + A + ":" + A + StF + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Advancementfactor", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "guidevalue" + A + ":{\"", ArgString = "value" + A + ":" + A + guidevalue + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Guide value", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "parry" + A + ":{\"", ArgString = "value" + A + ":" + ht.Parade + "," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "parryValue", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "attack" + A + ":{\"", ArgString = "value" + A + ":" + ht.Attacke + "," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "attackValue", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "talentValue" + A + ":{\"", ArgString = "value" + A + ":" + ht.TaW +"," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "attackValue", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "weapontype" + A + ":{\"", ArgString = "value" + A + ":" + A + wtype + A + "," + A + "label" + A + ":" + A + "weapontype" + A + "," + A + "type" + A + ":" + A + "String", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "description" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}}," });

            addArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{", Suffix = "}," });
            addArg.Add(new dbArgument { Prefix = A + "img" + A + ":", ArgString = A + "icons/environment/people/charge.webp", Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":", ArgString = "[]" });
            addArg.Add(new dbArgument { Suffix = "}," });
        }

        private List<dbArgument> AddKampftalenteArgument(GegnerBase g)
        {
            List<dbArgument> addArg = new List<dbArgument>();
            char A = (char)34;

            AddKampfTalent(addArg, g, "Anderthalbhänder", "Anderthalbhänder", "E", "kk", g.PA, g.AT, 0, "melee");
            AddKampfTalent(addArg, g, "Armbrüste", "Armbrust", "C", "ff", g.PA, g.AT, 0, "range");
            AddKampfTalent(addArg, g, "Belagerungswaffen", "Belagerungswaffen", "D", "ko", g.PA, g.AT, 0, "melee");
            AddKampfTalent(addArg, g, "Blasrohre", "Blasrohr", "D", "ff", g.PA, g.AT, 0, "range");
            AddKampfTalent(addArg, g, "Bögen", "Bogen", "E", "ff", g.PA, g.AT, 0, "range");
            AddKampfTalent(addArg, g, "Diskusse", "Diskus", "D", "ff", g.PA, g.AT, 0, "range");
            AddKampfTalent(addArg, g, "Dolche", "Dolche", "D", "ge", g.PA, g.AT, 0, "melee");
            AddKampfTalent(addArg, g, "Fechtwaffen", "Fechtwaffen", "E", "ge", g.PA, g.AT, 0, "melee");
            AddKampfTalent(addArg, g, "Hiebwaffen", "Hiebwaffen", "D", "kk", g.PA, g.AT, 0, "melee");
            AddKampfTalent(addArg, g, "Spiesswaffen", "Infanteriewaffen", "D", "kk", g.PA, g.AT, 0, "melee");
            AddKampfTalent(addArg, g, "Kettenstäbe", "Kettenstäbe", "E", "ff", g.PA, g.AT, 0, "melee");
            AddKampfTalent(addArg, g, "Kettenwaffen", "Kettenwaffen", "D", "kk", g.PA, g.AT, 0, "melee");
            AddKampfTalent(addArg, g, "Lanzen", "Lanzenreiten", "E", "kk", g.PA, g.AT, 0, "melee");
            AddKampfTalent(addArg, g, "Peitschen", "Peitsche", "E", "ff", g.PA, g.AT, 0, "melee");
            AddKampfTalent(addArg, g, "Raufen", "Raufen", "C", "ge", g.PA, g.AT, 0, "melee");
            AddKampfTalent(addArg, g, "Ringen", "Ringen", "D", "ge", g.PA, g.AT, 0, "melee");
            AddKampfTalent(addArg, g, "Säbel", "Säbel", "D", "ge", g.PA, g.AT, 0, "melee");
            AddKampfTalent(addArg, g, "Schleudern", "Schleuder", "E", "ff", g.PA, g.AT, 0, "range");
            AddKampfTalent(addArg, g, "Schwerter", "Schwerter", "E", "kk", g.PA, g.AT, 0, "melee");
            AddKampfTalent(addArg, g, "Speere", "Speere", "D", "ff", g.PA, g.AT, 0, "melee");
            AddKampfTalent(addArg, g, "Wurfwaffen", "Wurfbeile", "D", "ff", g.PA, g.AT, 0, "range");
            AddKampfTalent(addArg, g, "Wurfwaffen", "Wurfmesser", "C", "ff", g.PA, g.AT, 0, "range");
            AddKampfTalent(addArg, g, "Wurfwaffen", "Wurfspeere", "C", "ff", g.PA, g.AT, 0, "range");
            AddKampfTalent(addArg, g, "Zweihandflegel", "Zweihandflegel", "D", "kk", g.PA, g.AT, 0, "melee");
            AddKampfTalent(addArg, g, "Zweihandhiebwaffen", "Zweihand-Hiebwaffen", "D", "kk", g.PA, g.AT, 0, "melee");
            AddKampfTalent(addArg, g, "Zweihandschwerter/-säbel", "Zweihandschwerter/-säbel", "E", "kk", g.PA, g.AT, 0, "melee");

            return addArg;
        }

        private List<dbArgument> AddKampftalenteArgument(Held h)
        {
            List<dbArgument> addArg = new List<dbArgument>();
            char A = (char)34;

            AddKampfTalent(addArg, h, "Anderthalbhänder", "Anderthalbhänder", "E", "kk", h.ParadeBasis , h.AttackeBasis, 0, "melee");
            AddKampfTalent(addArg, h, "Armbrüste", "Armbrust", "C", "ff", h.ParadeBasis , h.AttackeBasis, 0, "range");
            AddKampfTalent(addArg, h, "Belagerungswaffen", "Belagerungswaffen", "D", "ko", h.ParadeBasis , h.AttackeBasis, 0, "melee");
            AddKampfTalent(addArg, h, "Blasrohre", "Blasrohr", "D", "ff", h.ParadeBasis , h.AttackeBasis, 0, "range");
            AddKampfTalent(addArg, h, "Bögen", "Bogen", "E", "ff", h.ParadeBasis , h.AttackeBasis, 0, "range");
            AddKampfTalent(addArg, h, "Diskusse", "Diskus", "D", "ff", h.ParadeBasis , h.AttackeBasis, 0, "range");
            AddKampfTalent(addArg, h, "Dolche", "Dolche", "D", "ge", h.ParadeBasis , h.AttackeBasis, 0, "melee");
            AddKampfTalent(addArg, h, "Fechtwaffen", "Fechtwaffen", "E", "ge", h.ParadeBasis , h.AttackeBasis, 0, "melee");
            AddKampfTalent(addArg, h, "Hiebwaffen", "Hiebwaffen", "D", "kk", h.ParadeBasis , h.AttackeBasis, 0, "melee");
            AddKampfTalent(addArg, h, "Spießwaffen", "Infanteriewaffen", "D", "kk", h.ParadeBasis , h.AttackeBasis, 0, "melee");
            AddKampfTalent(addArg, h, "Kettenstäbe", "Kettenstäbe", "E", "ff", h.ParadeBasis , h.AttackeBasis, 0, "melee");
            AddKampfTalent(addArg, h, "Kettenwaffen", "Kettenwaffen", "D", "kk", h.ParadeBasis , h.AttackeBasis, 0, "melee");
            AddKampfTalent(addArg, h, "Lanzen", "Lanzenreiten", "E", "kk", h.ParadeBasis , h.AttackeBasis, 0, "melee");
            AddKampfTalent(addArg, h, "Peitschen", "Peitsche", "E", "ff", h.ParadeBasis , h.AttackeBasis, 0, "melee");
            AddKampfTalent(addArg, h, "Raufen", "Raufen", "C", "ge", h.ParadeBasis , h.AttackeBasis, 0, "melee");
            AddKampfTalent(addArg, h, "Ringen", "Ringen", "D", "ge", h.ParadeBasis , h.AttackeBasis, 0, "melee");
            AddKampfTalent(addArg, h, "Säbel", "Säbel", "D", "ge", h.ParadeBasis , h.AttackeBasis, 0, "melee");
            AddKampfTalent(addArg, h, "Schleudern", "Schleuder", "E", "ff", h.ParadeBasis , h.AttackeBasis, 0, "range");
            AddKampfTalent(addArg, h, "Schwerter", "Schwerter", "E", "kk", h.ParadeBasis , h.AttackeBasis, 0, "melee");
            AddKampfTalent(addArg, h, "Speere", "Speere", "D", "ff", h.ParadeBasis , h.AttackeBasis, 0, "melee");
            AddKampfTalent(addArg, h, "Wurfwaffen", "Wurfbeile", "D", "ff", h.ParadeBasis , h.AttackeBasis, 0, "range");
            AddKampfTalent(addArg, h, "Wurfwaffen", "Wurfmesser", "C", "ff", h.ParadeBasis , h.AttackeBasis, 0, "range");
            AddKampfTalent(addArg, h, "Wurfwaffen", "Wurfspeere", "C", "ff", h.ParadeBasis , h.AttackeBasis, 0, "range");
            AddKampfTalent(addArg, h, "Zweihandflegel", "Zweihandflegel", "D", "kk", h.ParadeBasis , h.AttackeBasis, 0, "melee");
            AddKampfTalent(addArg, h, "Zweihandhiebwaffen", "Zweihand-Hiebwaffen", "D", "kk", h.ParadeBasis , h.AttackeBasis, 0, "melee");
            AddKampfTalent(addArg, h, "Zweihandschwerter/-säbel", "Zweihandschwerter/-säbel", "E", "kk", h.ParadeBasis , h.AttackeBasis, 0, "melee");

            return addArg;
        }
        private List<dbArgument> AddMoneyArgument(int Du, int Si, int He, int Kr)
        {
            List<dbArgument> addArg = new List<dbArgument>();
            char A = (char)34;

            //Dukaten
            string atGuid = Guid.NewGuid().ToString().Substring(19, 17).Replace("-", "");
            addArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = atGuid, Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = "Money-D", Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "money", Suffix = A + "," });

            addArg.Add(new dbArgument { Prefix = A + "data" + A + ":{" });
            addArg.Add(new dbArgument { Prefix = A + "description" + A + ":{\"", ArgString = "value" + A + ":" + A + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Description", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "gmdescription" + A + ":{\"", ArgString = "value" + A + ":" + A + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Description", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "price" + A + ":{\"", ArgString = "value" + A + ":" + Du + "," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "Price", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "quantity" + A + ":{\"", ArgString = "value" + A + ":0," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "Quantity", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "weight" + A + ":{\"", ArgString = "value" + A + ":0," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "weight", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":{\"", ArgString = "value" + A + ":" + A + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Special effect", Suffix = A + "}}," });
            addArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{\"", ArgString = "core" + A + ":{ " + A + "sourceId" + A + ":" + A + "Item.BGfRbsrZ2M2Y2wZ3", Suffix = A + "}}," });
            addArg.Add(new dbArgument { Prefix = A + "img" + A + ":", ArgString = A + "systems/dsa5/icons/money-D.webp", Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":", ArgString = "[]" });
            addArg.Add(new dbArgument { Suffix = "}," });


            //Silber
            atGuid = Guid.NewGuid().ToString().Substring(19, 17).Replace("-", "");
            addArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = atGuid, Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = "Money-S", Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "money", Suffix = A + "," });

            addArg.Add(new dbArgument { Prefix = A + "data" + A + ":{" });
            addArg.Add(new dbArgument { Prefix = A + "description" + A + ":{\"", ArgString = "value" + A + ":" + A + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Description", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "gmdescription" + A + ":{\"", ArgString = "value" + A + ":" + A + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Description", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "price" + A + ":{\"", ArgString = "value" + A + ":" + Si + "," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "Price", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "quantity" + A + ":{\"", ArgString = "value" + A + ":0," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "Quantity", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "weight" + A + ":{\"", ArgString = "value" + A + ":0," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "weight", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":{\"", ArgString = "value" + A + ":" + A + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Special effect", Suffix = A + "}}," });
            addArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{\"", ArgString = "core" + A + ":{ " + A + "sourceId" + A + ":" + A + "Item.ntX4yPa0e9rmmqSZ", Suffix = A + "}}," });
            addArg.Add(new dbArgument { Prefix = A + "img" + A + ":", ArgString = A + "systems/dsa5/icons/money-S.webp", Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":", ArgString = "[]" });
            addArg.Add(new dbArgument { Suffix = "}," });


            //Heller
            atGuid = Guid.NewGuid().ToString().Substring(19, 17).Replace("-", "");
            addArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = atGuid, Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = "Money-H", Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "money", Suffix = A + "," });

            addArg.Add(new dbArgument { Prefix = A + "data" + A + ":{" });
            addArg.Add(new dbArgument { Prefix = A + "description" + A + ":{\"", ArgString = "value" + A + ":" + A + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Description", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "gmdescription" + A + ":{\"", ArgString = "value" + A + ":" + A + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Description", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "price" + A + ":{\"", ArgString = "value" + A + ":" + He + "," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "Price", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "quantity" + A + ":{\"", ArgString = "value" + A + ":0," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "Quantity", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "weight" + A + ":{\"", ArgString = "value" + A + ":0," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "weight", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":{\"", ArgString = "value" + A + ":" + A + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Special effect", Suffix = A + "}}," });
            addArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{\"", ArgString = "core" + A + ":{ " + A + "sourceId" + A + ":" + A + "Item.2wwyflxgKCiz9UIQ", Suffix = A + "}}," });
            addArg.Add(new dbArgument { Prefix = A + "img" + A + ":", ArgString = A + "systems/dsa5/icons/money-H.webp", Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":", ArgString = "[]" });
            addArg.Add(new dbArgument { Suffix = "}," });


            //Kreuzer
            atGuid = Guid.NewGuid().ToString().Substring(19, 17).Replace("-", "");
            addArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = atGuid, Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = "Money-K", Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "money", Suffix = A + "," });

            addArg.Add(new dbArgument { Prefix = A + "data" + A + ":{" });
            addArg.Add(new dbArgument { Prefix = A + "description" + A + ":{\"", ArgString = "value" + A + ":" + A + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Description", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "gmdescription" + A + ":{\"", ArgString = "value" + A + ":" + A + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Description", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "price" + A + ":{\"", ArgString = "value" + A + ":" + Kr + "," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "Price", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "quantity" + A + ":{\"", ArgString = "value" + A + ":0," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "Quantity", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "weight" + A + ":{\"", ArgString = "value" + A + ":0," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "weight", Suffix = A + "}," });
            addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":{\"", ArgString = "value" + A + ":" + A + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Special effect", Suffix = A + "}}," });
            addArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{\"", ArgString = "core" + A + ":{ " + A + "sourceId" + A + ":" + A + "Item.IHSzojKYEn0FloQm", Suffix = A + "}}," });
            addArg.Add(new dbArgument { Prefix = A + "img" + A + ":", ArgString = A + "systems/dsa5/icons/money-K.webp", Suffix = A + "," });
            addArg.Add(new dbArgument { Prefix = A + "effect" + A + ":", ArgString = "[]" });
            addArg.Add(new dbArgument { Suffix = "}," });

            return addArg;
        }

        private List<dbArgument> SetDSA5Arguments(GegnerBase g, string GetFilenamePortrait, string GetFilenameToken, string id)
        {
            List<dbArgument> lstArg = new List<dbArgument>();
            char A = (char)34;

            //gArg.lstArguments = new List<dbArgument>();
            lstArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = id, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = g.Name, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "permission" + A + ":{\"default" + A + ":", ArgString = "0,\"3WHJiGe2LNC2VNeR" + A + ":", Suffix = "3}," });
            lstArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "creature", Suffix = A + "," });
           // lstArg.Add(new dbArgument { Prefix = A + "data" + A + ":{\"biography" + A + ":\"", ArgString = "", Suffix = A + "," });

            lstArg.Add(new dbArgument { Prefix = A + "data" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "characteristics" + A + ":{" });

            lstArg.AddRange(AddGegnerArgument("mu", "#b3241a", g.MU));
            lstArg.AddRange(AddGegnerArgument("kl", "#8259a3", g.KL));
            lstArg.AddRange(AddGegnerArgument("in", "#388834", g.IN));
            lstArg.AddRange(AddGegnerArgument("ch", "#0d0d0d", g.CH));
            lstArg.AddRange(AddGegnerArgument("ff", "#d5b467", g.FF));
            lstArg.AddRange(AddGegnerArgument("ge", "#688ec4", g.GE));
            lstArg.AddRange(AddGegnerArgument("ko", "#fbf5ea", g.KO));
            lstArg.AddRange(AddGegnerArgument("kk", "#d6a878", g.KK, "},"));

            lstArg.Add(new dbArgument { Prefix = A + "status" + A + ":{" });
            lstArg.AddRange(AddGegnerStatus("wounds", (g.KO * 2).ToString(), g.LE.ToString(), "0", (g.LE - g.KO*2).ToString() , g.LE.ToString(), "},"));
            lstArg.AddRange(AddGegnerStatus("astralenergy", g.AE.ToString(), g.AE.ToString(), "0", "0", g.AE.ToString(), "},"));
            lstArg.AddRange(AddGegnerStatus("karmaenergy", g.KE.ToString(), g.KE.ToString(), "0", "0", g.KE.ToString(), "},"));
            lstArg.AddRange(AddGegnerStatus("soulpower", "0", "0", null, "0", null, "},"));
            lstArg.AddRange(AddGegnerStatus("toughness", "0", "0", null, "0", null, "},"));
            lstArg.AddRange(AddGegnerStatus("dodge", null, "0", null, "0", null, "},"));
            lstArg.AddRange(AddGegnerStatus("fatePoints", null, "0", null, "0", "0", "},"));                            //Schicksalspunkte
            lstArg.AddRange(AddGegnerStatus("speed", g.GS.ToString(), "0", null, "0", null, "},"));
            lstArg.AddRange(AddGegnerStatus("initiative", null, g.INIBasis.ToString(), null, "0", g.INIBasis.ToString(), "},"));
            lstArg.Add(new dbArgument { Prefix = A + "size" + A + ":{" + A + "value" + A + ":" , ArgString = A+ "average" + A, Suffix = "}}," });

            lstArg.Add(new dbArgument { Prefix = A + "guidevalue" + A + ":{" +A+"value"+A+":"+A+"-"+A, Suffix="},"});
            lstArg.Add(new dbArgument { Prefix = A + "tradition" + A + ":{" +A+"value"+A+":"+A+A, Suffix="},"});
            lstArg.Add(new dbArgument { Prefix = A + "feature" + A + ":{" +A+"value"+A+":"+A+A, Suffix="},"});
            lstArg.Add(new dbArgument { Prefix = A + "happyTalents" + A + ":{" +A+"value"+A+":"+A+A, Suffix="},"});
            lstArg.Add(new dbArgument { Prefix = A + "details" + A + ":{" });
                lstArg.Add(new dbArgument { Prefix = A + "species" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "gender" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "culture" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "career" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "socialstate" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "home" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "family" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "age" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "haircolor" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "eyecolor" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "height" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "weight" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "distinguishingmark" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "biography" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "notes" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "experience" + A + ":{" + A + "total" + A + ":0," + A + "spent" + A + ":0", Suffix = "}}," });
                lstArg.Add(new dbArgument { Prefix = A + "Home" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });

            lstArg.Add(new dbArgument { Prefix = A + "actionCount" + A + ":{\"", ArgString = "value" + A + ":" + A + g.Aktionen, Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "count" + A + ":{\"", ArgString = "value" + A + ":" + A + g.Auftreten, Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "creatureClass" + A + ":{\"", ArgString = "value" + A + ":" + A + g.Tags, Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "behaviour" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });// + g.Bemerkung
            lstArg.Add(new dbArgument { Prefix = A + "flight" + A + ":{\"", ArgString = "value" + A + ":" + A + g.Jagdreaktion, Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "specialRules" + A + ":{\"", ArgString = "value" + A + ":" + A + g.Verbreitung, Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "conjuringDifficulty" + A + ":{\"", ArgString = "value" + A + ":0" , Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "description" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });;// + g.Bemerkung
            //lstArg.Add(new dbArgument { Prefix = A + "freeLanguagePoints" + A + ":{" +A+"value"+A+":0,"+A+"used"+A+":0", Suffix="},"});
            lstArg.Add(new dbArgument { Prefix = A + "sheetLocked" + A + ":{" +A+"value"+A+":"+"false", Suffix="},"});
            lstArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{", Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "itemModifiers" + A + ":{", Suffix = "}}," });

            //Folder
            if (SelectedGegnerFolder != null && !string.IsNullOrEmpty(SelectedGegnerFolder.name))
            {
                lstArg.Add(new dbArgument { Prefix = A + "folder" + A + ":\"", ArgString = SelectedGegnerFolder._id, Suffix = "\"," });
            }

            lstArg.Add(new dbArgument { Prefix = A + "sort" + A + ":", ArgString = "100001", Suffix = "," });

            lstArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "core" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "sourceId" + A + ":\"", ArgString = "Compendium.world.dsa-gegner.KBxxxx0000000001", Suffix = A + "}" });
            lstArg.Add(new dbArgument { Prefix = "}," });

            //Portrait Image
            lstArg.Add(new dbArgument
            {
                Prefix = A + "img" + A + ":\"",
                ArgString =
                (string.IsNullOrEmpty(GetFilenamePortrait) ? "icons/svg/mystery-man.svg" :
                 GetFilenamePortrait),
                Suffix = A + ","
            });

            lstArg.Add(new dbArgument { Prefix = A + "token" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = g.Name, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "displayName" + A + ":", ArgString = "0", Suffix = "," });

            //Token Image
            lstArg.Add(new dbArgument
            {
                Prefix = A + "img" + A + ":\"",
                ArgString =
                (string.IsNullOrEmpty(GetFilenameToken) ? "icons/svg/mystery-man.svg" :
                GetFilenameToken),
                Suffix = A + ","
            });

            lstArg.Add(new dbArgument { Prefix = A + "tint" + A + ":", ArgString = "null", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "width" + A + ":", ArgString = "1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "height" + A + ":", ArgString = "1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "scale" + A + ":", ArgString = "1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "lockRotation" + A + ":", ArgString = "false", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "vision" + A + ":", ArgString = "false", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "dimSight" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "brightSight" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "dimLight" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "brightLight" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "sightAngle" + A + ":", ArgString = "360", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "lightAngle" + A + ":", ArgString = "360", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "lightAlpha" + A + ":", ArgString = "1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "lightAnimation" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "speed" + A + ":", ArgString = "5", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "intensity" + A + ":", ArgString = "5" });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "actorId" + A + ":\"", ArgString = id, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "actorLink" + A + ":", ArgString = "false", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "disposition" + A + ":", ArgString = "-1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "displayBars" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "bar1" + A + ":{", ArgString = "", Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "bar2" + A + ":{", ArgString = "", Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "randomImg" + A + ":", ArgString = "false", Suffix = "" });
            lstArg.Add(new dbArgument { Prefix = "}," });

            lstArg.Add(new dbArgument { Prefix = A + "items" + A + ":[", ArgString = "", Suffix = "" });
            lstArg.AddRange(AddMoneyArgument(0, 0, 0, 0));

            lstArg.AddRange(AddKampftalenteArgument(g));

            //Add Vor- Nachteile
            /*
             {"_id":"cokjaU2gwCgI6l2r","name":"Angst vor (eigenem Spiegelbild)","type":"disadvantage","data":{"description":{"value":"<p>Der Held fürchtet sich derart vor etwas, dass er hierdurch in seinem Handeln eingeschränkt wird. Der Auslöser der Angst sollte nicht zu selten vorkommen. Beispiele für Ängste sind Angst vor Blut, Magie, Spinnen, Dunkelheit, dem Meer, großer Höhe, freien Flächen, engen Räumen oder vor Toten und Untoten. Grundsätzlich hat der Spielleiter bei der Frage, ob eine bestimmte Angst möglich ist oder nicht, das letzte Wort.</p><p>Beispiele für Ängste</p><p><ul><li>Angst vor bestimmten Tieren, etwa Reptilien, Insekten, Spinnen</li><li>Angst vor Blut</li><li>Angst vor Dunkelheit (Dunkelangst)</li><li>Angst vor Höhe (Höhenangst)</li><li>Angst vor dem Meer (Meeresangst)</li><li>Angst vor engen Räumen (Raumangst)</li><li>Angst vor Toten und Untoten (Totenangst)</li></ul></p>"},"gmdescription":{"value":""},"APValue":{"value":"-8"},"max":{"value":"3"},"requirements":{"value":"keine"},"step":{"value":3},"effect":{"value":""}},"flags":{},"img":"systems/dsa5/icons/categories/Nachteil.webp","effects":[]},{"_id":"LHhkMjt3qqR2Xz47","name":"Schlechte Eigenschaft (Neugier)","type":"disadvantage","data":{"description":{"value":"<p>Jeder Abenteurer ist neugierig, aber einige Abenteurer werden so von ihrer Neugierde getrieben, dass sie unnötige Risiken eingehen, um sie zu befriedigen.</p><p>Viele Helden werden nicht nur von rationalen Motiven angetrieben. Sie sind  beispielsweise gierig, abergläubisch oder jähzornig.</p><p><b>Regel</b>: In jeder Situation, in der der Held mit einem potenziellen Auslöser seiner Schlechten Eigenschaft konfrontiert wird, muss er eine Probe auf Willenskraft bestehen, um sich zu beherrschen. Gelingt ihm diese Probe, ist alles in Ordnung, ansonsten muss er entsprechend der Schlechten Eigenschaft agieren. Seine Schlechte Eigenschaft hat ihn so lange im Griff, wie er dem Auslöser ausgesetzt ist.\n    Der Meister kann für die Probe auf Willenskraft entsprechend der Stärke des Auslösers Erschwernisse oder Erleichterungen aussprechen.\n    Es dürfen bis zu zwei Schlechte Eigenschaften pro Held gewählt werden. Kombinationen, die sich ausschließen (z.B. Geiz und Verschwendungssucht), dürfen nicht gewählt werden. Das letzte Wort darüber hat der Spielleiter.</b></p>"},"gmdescription":{"value":""},"APValue":{"value":"-5"},"max":{"value":"0"},"requirements":{"value":"keine"},"step":{"value":1},"effect":{"value":""}},"flags":{},"img":"systems/dsa5/icons/categories/Nachteil.webp","effects":[]}
             * 
             * 
             * 
             * 
             * 
             * 
             * 
              Waffe:
             {"_id":"6BVLO6Q8311fc2OJ","name":"Heshthot-Peitsche*","type":"meleeweapon","data":{"price":{"value":0},"quantity":{"value":1},"weight":{"value":1},"effect":{"value":"1 Stufe Schmerz"},"description":{"value":"Erzielt der Heshthot mit der Peitsche SP, erhält sein Opfer pro Treffer eine Stufe Schmerz bis zum nächsten Sonnenaufgang."},"gmdescription":{"value":""},"damage":{"value":"1d6+1"},"atmod":{"value":0,"offhandMod":0},"pamod":{"value":0,"offhandMod":0},"reach":{"value":"long"},"damageThreshold":{"value":16},"guidevalue":{"value":"ff"},"combatskill":{"value":"Peitschen"},"worn":{"value":true,"offhand":false,"offHand":false},"structure":{"max":0,"value":0}},"sort":100000,"flags":{},"img":"icons/weapons/swords/greatsword-blue.webp","effects":[]}
              */
            lstArg.AddRange(AddAngriffeArgument(g));
            lstArg.AddRange(AddZauberArgument(g));

            string atGuid = Guid.NewGuid().ToString().Substring(19, 17).Replace("-", "");
            lstArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = atGuid, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = "Natürliche Rüstung", Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "trait", Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "data" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "description" + A + ":{\"", ArgString = "value" + A + ":" + A + "Natürlicher Schutz durch Haut, Schuppen oder dergleichen.", Suffix = A + " }," });
            lstArg.Add(new dbArgument { Prefix = A + "gmdescription" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "APValue" + A + ":{\"", ArgString = "value" + A + ":" + A + "0", Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "traitType" + A + ":{\"", ArgString = "value" + A + ":" + A + "armor", Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "at" + A + ":{\"", ArgString = "value" + A + ":" + A + g.RS[Kampf.Logic.Trefferzone.Gesamt], Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "reach" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "damage" + A + ":{\"", ArgString = "value" + A + ":" + A + "1d6", Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "reloadTime" + A + ":{\"", ArgString = "value" + A + ":" + A + "1", Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "AsPCost" + A + ":{\"", ArgString = "value" + A + ":" + A + "0", Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "duration" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "aspect" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "effect" + A + ":{\"", ArgString = "value" + A + ":" + A, Suffix = A + "}}," });

            lstArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{", Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "img" + A + ":", ArgString = A + "systems/dsa5/icons/categories/Armor.webp", Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "effect" + A + ":", ArgString = "[]" });
            lstArg.Add(new dbArgument { Suffix = "}," });


            if (lstArg.Last().Suffix != "")
                lstArg.Last().Suffix =  "}";
            //add Std.Talente?
            lstArg.Add(new dbArgument { Prefix = "]," });

            lstArg.Add(new dbArgument { Prefix = A + "effects" + A + ":[", ArgString = "", Suffix = "]" });
            lstArg.Add(new dbArgument { Prefix = "}" });

            return lstArg;

        }

        private List<dbArgument> SetDSA5Arguments(Held h, string GetFilenamePortrait, string GetFilenameToken, string id)
        {
            List<dbArgument> lstArg = new List<dbArgument>();
            char A = (char)34;

            //gArg.lstArguments = new List<dbArgument>();
            lstArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = id, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = h.Name, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "permission" + A + ":{\"default" + A + ":", ArgString = "0,\"3WHJiGe2LNC2VNeR" + A + ":", Suffix = "3}," });
            lstArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "character", Suffix = A + "," });
            // lstArg.Add(new dbArgument { Prefix = A + "data" + A + ":{\"biography" + A + ":\"", ArgString = "", Suffix = A + "," });

            lstArg.Add(new dbArgument { Prefix = A + "data" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "characteristics" + A + ":{" });

            lstArg.AddRange(AddGegnerArgument("mu", "#b3241a", h.MU));
            lstArg.AddRange(AddGegnerArgument("kl", "#8259a3", h.KL));
            lstArg.AddRange(AddGegnerArgument("in", "#388834", h.IN));
            lstArg.AddRange(AddGegnerArgument("ch", "#0d0d0d", h.CH));
            lstArg.AddRange(AddGegnerArgument("ff", "#d5b467", h.FF));
            lstArg.AddRange(AddGegnerArgument("ge", "#688ec4", h.GE));
            lstArg.AddRange(AddGegnerArgument("ko", "#fbf5ea", h.KO));
            lstArg.AddRange(AddGegnerArgument("kk", "#d6a878", h.KK, "},"));

            lstArg.Add(new dbArgument { Prefix = A + "status" + A + ":{" });

            lstArg.AddRange(AddGegnerStatus("wounds", "0", h.LebensenergieAktuell.ToString(), "0", (h.LebensenergieMax - h.KO * 2).ToString(), h.LebensenergieMax.ToString(), "},"));
            lstArg.AddRange(AddGegnerStatus("astralenergy", h.AstralenergieMax.ToString(), h.AstralenergieAktuell.ToString(), "0", "0", h.AstralenergieMax.ToString(), "},"));
            lstArg.AddRange(AddGegnerStatus("karmaenergy", h.KarmaenergieMax.ToString(), h.KarmaenergieAktuell.ToString(), "0", "0", h.KarmaenergieMax.ToString(), "},"));
            lstArg.AddRange(AddGegnerStatus("soulpower", "0", "0", null, "0", null, "},"));
            lstArg.AddRange(AddGegnerStatus("toughness", "0", "0", null, "0", null, "},"));
            lstArg.AddRange(AddGegnerStatus("dodge", null, "0", null, "0", null, "},"));
            lstArg.AddRange(AddGegnerStatus("fatePoints", null, "0", null, "0", "0", "},"));                            //Schicksalspunkte
            lstArg.AddRange(AddGegnerStatus("speed", h.Geschwindigkeit.ToString(), "0", null, "0", null, "},"));
            lstArg.AddRange(AddGegnerStatus("initiative", null, h.InitiativeBasis.ToString(), null, "0", h.InitiativeMax().ToString(), "},"));
            lstArg.Add(new dbArgument { Prefix = A + "size" + A + ":{" + A + "value" + A + ":", ArgString = A + "average" + A, Suffix = "}}," });

            lstArg.Add(new dbArgument { Prefix = A + "guidevalue" + A + ":{" + A + "value" + A + ":" + A + "-" + A, Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "tradition" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "feature" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "happyTalents" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "details" + A + ":{" });
                lstArg.Add(new dbArgument { Prefix = A + "species" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "gender" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "culture" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "career" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "socialstate" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "home" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "family" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "age" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "haircolor" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "eyecolor" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "height" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "weight" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "distinguishingmark" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "biography" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "notes" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "experience" + A + ":{" + A + "total" + A + ":0," + A + "spent" + A + ":0", Suffix = "}," });
                lstArg.Add(new dbArgument { Prefix = A + "Home" + A + ":{" + A + "value" + A + ":" + A + A, Suffix = "}}," });

            lstArg.Add(new dbArgument { Prefix = A + "freeLanguagePoints" + A + ":{" + A + "value" + A + ":0," + A + "used" + A + ":0", Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "sheetLocked" + A + ":{" + A + "value" + A + ":" + "false", Suffix = "}," });

            lstArg.Add(new dbArgument { Prefix = A + "stats" + A + ":{" });
            lstArg.AddRange(AddStats("health", h.LebensenergieAktuell, 0, h.LebensenergieMax));
            lstArg.AddRange(AddStats("endurance", h.AusdauerAktuell, 0, h.AusdauerMax));
            lstArg.AddRange(AddStats("astral", h.AstralenergieAktuell, 0, h.AstralenergieMax));
            lstArg.AddRange(AddStats("karmal", h.KarmaenergieAktuell, 0, h.KarmaenergieMax));
            lstArg.AddRange(AddStats("magic_resistance", h.MRGeist, null, null, "},"));
            lstArg.AddRange(AddStats("speed", (int)h.Geschwindigkeit, null, null, "},"));
            lstArg.AddRange(AddStats("INI", h.InitiativeBasis, null, null, "},"));
            lstArg.AddRange(AddStats("AT", h.AT, null, null, "},"));
            lstArg.AddRange(AddStats("PA", h.PA, null, null, "},"));
            lstArg.AddRange(AddStats("FK", h.Fernkampf, null, null, "},"));
            lstArg.AddRange(AddStats("WS", Convert.ToInt32(h.KO / 2), null, null, "}},"));

            lstArg.Add(new dbArgument { Prefix = A + "attributes" + A + ":{" });
            lstArg.AddRange(AddAttributes("MU", h.MU, 0, h.MU));
            lstArg.AddRange(AddAttributes("KL", h.KL, 0, h.KL));
            lstArg.AddRange(AddAttributes("IN", h.IN, 0, h.IN));
            lstArg.AddRange(AddAttributes("CH", h.CH, 0, h.CH));
            lstArg.AddRange(AddAttributes("FF", h.FF, 0, h.FF));
            lstArg.AddRange(AddAttributes("GE", h.GE, 0, h.GE));
            lstArg.AddRange(AddAttributes("KO", h.KO, 0, h.KO));
            lstArg.AddRange(AddAttributes("KK", h.KK, 0, h.KK, "}}},"));

            //Folder
            if (SelectedHeldenFolder != null && !string.IsNullOrEmpty(SelectedHeldenFolder.name))
            {
                lstArg.Add(new dbArgument { Prefix = A + "folder" + A + ":\"", ArgString = SelectedHeldenFolder._id, Suffix = "\"," });
            }

            lstArg.Add(new dbArgument { Prefix = A + "sort" + A + ":", ArgString = "100001", Suffix = "," });

            lstArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "core" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "sourceId" + A + ":\"", ArgString = "Compendium.world.dsa-gegner.KBxxxx0000000001", Suffix = A + "}" });
            lstArg.Add(new dbArgument { Prefix = "}," });

            //Portrait Image
            lstArg.Add(new dbArgument
            {
                Prefix = A + "img" + A + ":\"",
                ArgString =
                (string.IsNullOrEmpty(GetFilenamePortrait) ? "icons/svg/mystery-man.svg" :
                 GetFilenamePortrait),
                Suffix = A + ","
            });

            lstArg.Add(new dbArgument { Prefix = A + "token" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = h.Name, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "displayName" + A + ":", ArgString = "0", Suffix = "," });

            //Token Image
            lstArg.Add(new dbArgument
            {
                Prefix = A + "img" + A + ":\"",
                ArgString =
                (string.IsNullOrEmpty(GetFilenameToken) ? "icons/svg/mystery-man.svg" :
                GetFilenameToken),
                Suffix = A + ","
            });

            lstArg.Add(new dbArgument { Prefix = A + "tint" + A + ":", ArgString = A + "" + A, Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "width" + A + ":", ArgString = "1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "height" + A + ":", ArgString = "1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "scale" + A + ":", ArgString = "1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "lockRotation" + A + ":", ArgString = "false", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "vision" + A + ":", ArgString = "false", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "dimSight" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "brightSight" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "dimLight" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "brightLight" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "sightAngle" + A + ":", ArgString = "360", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "lightAngle" + A + ":", ArgString = "360", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "lightAlpha" + A + ":", ArgString = "1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "lightAnimation" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "speed" + A + ":", ArgString = "5", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "intensity" + A + ":", ArgString = "5" });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "actorId" + A + ":\"", ArgString = id, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "actorLink" + A + ":", ArgString = "false", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "disposition" + A + ":", ArgString = "-1", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "displayBars" + A + ":", ArgString = "0", Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "bar1" + A + ":{", ArgString = "", Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "bar2" + A + ":{", ArgString = "", Suffix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "randomImg" + A + ":", ArgString = "false", Suffix = "" });
            lstArg.Add(new dbArgument { Prefix = "}," });
            lstArg.Add(new dbArgument { Prefix = A + "items" + A + ":[", ArgString = "", Suffix = "" });

            lstArg.AddRange(AddMoneyArgument(0, 0, 0, 0));

            lstArg.AddRange(AddKampftalenteArgument(h));

            //Add Vor- Nachteile
            /*
             {"_id":"cokjaU2gwCgI6l2r","name":"Angst vor (eigenem Spiegelbild)","type":"disadvantage","data":{"description":{"value":"<p>Der Held fürchtet sich derart vor etwas, dass er hierdurch in seinem Handeln eingeschränkt wird. Der Auslöser der Angst sollte nicht zu selten vorkommen. Beispiele für Ängste sind Angst vor Blut, Magie, Spinnen, Dunkelheit, dem Meer, großer Höhe, freien Flächen, engen Räumen oder vor Toten und Untoten. Grundsätzlich hat der Spielleiter bei der Frage, ob eine bestimmte Angst möglich ist oder nicht, das letzte Wort.</p><p>Beispiele für Ängste</p><p><ul><li>Angst vor bestimmten Tieren, etwa Reptilien, Insekten, Spinnen</li><li>Angst vor Blut</li><li>Angst vor Dunkelheit (Dunkelangst)</li><li>Angst vor Höhe (Höhenangst)</li><li>Angst vor dem Meer (Meeresangst)</li><li>Angst vor engen Räumen (Raumangst)</li><li>Angst vor Toten und Untoten (Totenangst)</li></ul></p>"},"gmdescription":{"value":""},"APValue":{"value":"-8"},"max":{"value":"3"},"requirements":{"value":"keine"},"step":{"value":3},"effect":{"value":""}},"flags":{},"img":"systems/dsa5/icons/categories/Nachteil.webp","effects":[]},{"_id":"LHhkMjt3qqR2Xz47","name":"Schlechte Eigenschaft (Neugier)","type":"disadvantage","data":{"description":{"value":"<p>Jeder Abenteurer ist neugierig, aber einige Abenteurer werden so von ihrer Neugierde getrieben, dass sie unnötige Risiken eingehen, um sie zu befriedigen.</p><p>Viele Helden werden nicht nur von rationalen Motiven angetrieben. Sie sind  beispielsweise gierig, abergläubisch oder jähzornig.</p><p><b>Regel</b>: In jeder Situation, in der der Held mit einem potenziellen Auslöser seiner Schlechten Eigenschaft konfrontiert wird, muss er eine Probe auf Willenskraft bestehen, um sich zu beherrschen. Gelingt ihm diese Probe, ist alles in Ordnung, ansonsten muss er entsprechend der Schlechten Eigenschaft agieren. Seine Schlechte Eigenschaft hat ihn so lange im Griff, wie er dem Auslöser ausgesetzt ist.\n    Der Meister kann für die Probe auf Willenskraft entsprechend der Stärke des Auslösers Erschwernisse oder Erleichterungen aussprechen.\n    Es dürfen bis zu zwei Schlechte Eigenschaften pro Held gewählt werden. Kombinationen, die sich ausschließen (z.B. Geiz und Verschwendungssucht), dürfen nicht gewählt werden. Das letzte Wort darüber hat der Spielleiter.</b></p>"},"gmdescription":{"value":""},"APValue":{"value":"-5"},"max":{"value":"0"},"requirements":{"value":"keine"},"step":{"value":1},"effect":{"value":""}},"flags":{},"img":"systems/dsa5/icons/categories/Nachteil.webp","effects":[]}
             * 
             * 
             * 
             * 
              Waffe:
             {"_id":"6BVLO6Q8311fc2OJ","name":"Heshthot-Peitsche*","type":"meleeweapon","data":{"price":{"value":0},"quantity":{"value":1},"weight":{"value":1},"effect":{"value":"1 Stufe Schmerz"},"description":{"value":"Erzielt der Heshthot mit der Peitsche SP, erhält sein Opfer pro Treffer eine Stufe Schmerz bis zum nächsten Sonnenaufgang."},"gmdescription":{"value":""},"damage":{"value":"1d6+1"},"atmod":{"value":0,"offhandMod":0},"pamod":{"value":0,"offhandMod":0},"reach":{"value":"long"},"damageThreshold":{"value":16},"guidevalue":{"value":"ff"},"combatskill":{"value":"Peitschen"},"worn":{"value":true,"offhand":false,"offHand":false},"structure":{"max":0,"value":0}},"sort":100000,"flags":{},"img":"icons/weapons/swords/greatsword-blue.webp","effects":[]}
              */
            lstArg.AddRange(AddAngriffeArgument(h));
            lstArg.AddRange(AddZauberArgument(h));

            //Add all Talente
            int i = 1;
            foreach(Held_Talent ht in h.Held_Talent)
            {
                if (ht.Talent.Talentgruppe.Kurzname == "Kampf" ||
                    ht.Talent.Talentgruppe.Kurzname == "Ritualkenntnis" ||
                    ht.Talent.Talentgruppe.Kurzname == "Meta")
                    continue;
                lstArg.AddRange(GetArgumenteHeldTalent(ht, i));
                i++;
            }

            lstArg.Last().Suffix = "}";

            // end Talente

            lstArg.Add(new dbArgument { Prefix = "]," });

            lstArg.Add(new dbArgument { Prefix = A + "effects" + A + ":[", ArgString = "", Suffix = "]" });
            lstArg.Add(new dbArgument { Prefix = "}" });

            return lstArg;

        }
        private List<dbArgument> GetArgumenteHeldTalent(Held_Talent ht, int sortNo)
        {
            List<dbArgument> lstArg = new List<dbArgument>();
            string GruppenTyp =
                ht.Talent.Talentgruppe.Kurzname == "Kampf" ? "combat" :
                ht.Talent.Talentgruppe.Kurzname == "Körper" ? "body" :
                ht.Talent.Talentgruppe.Kurzname == "Gesellschaft" ? "social" :
                ht.Talent.Talentgruppe.Kurzname == "Natur" ? "nature" :
                ht.Talent.Talentgruppe.Kurzname == "Wissen" ? "knowledge" :
                ht.Talent.Talentgruppe.Kurzname == "Handwerk" ? "trade" :
                ht.Talent.Talentgruppe.Kurzname == "Sprachen/Schriften" ? "knowledge" :
                ht.Talent.Talentgruppe.Kurzname == "Meta" ? "meta" :
                ht.Talent.Talentgruppe.Kurzname == "Gabe" ? "gift" :
                ht.Talent.Talentgruppe.Kurzname == "Ritualkenntnis" ? "ritual" :
                ht.Talent.Talentgruppe.Kurzname == "Liturgiekenntnis" ? "liturg" : "";
            char A = (char)34;
            string id = Guid.NewGuid().ToString().Substring(19, 17).Replace("-", "");
            //gArg.lstArguments = new List<dbArgument>();
            lstArg.Add(new dbArgument { Prefix = "{" + A + "_id" + A + ":\"", ArgString = id, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "name" + A + ":\"", ArgString = ht.Talentname, Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "type" + A + ":\"", ArgString = "skill", Suffix = A + "," });
            lstArg.Add(new dbArgument { Prefix = A + "data" + A + ":", ArgString="{" });
            lstArg.Add(new dbArgument { Prefix = A + "group" + A + ":{", ArgString = A+ "value"+A+":\""+GruppenTyp+A+","+ A+"type"+A+":"+A+"String"+A+","+A+"label"+A+":\"Skillgroup", Suffix = A + "}," });

            lstArg.Add(new dbArgument { Prefix = A + "talentValue" + A + ":{\"", ArgString = "value" + A + ":" + ht.TaW + "," + A + "type" + A + ":" + A + "Number" + A + "," + A + "label" + A + ":" + A + "Fw", Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "characteristic1" + A + ":{\"", ArgString = "value" + A + ":\"" + ht.Talent.Eigenschaft1.ToLower() + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Characteristic1", Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "characteristic2" + A + ":{\"", ArgString = "value" + A + ":\"" + ht.Talent.Eigenschaft2.ToLower() + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Characteristic2", Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "characteristic3" + A + ":{\"", ArgString = "value" + A + ":\"" + ht.Talent.Eigenschaft3.ToLower() + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Characteristic3", Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "RPr" + A + ":{\"", ArgString = "value" + A + ":\"maybe" + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Routinecheck", Suffix = A + "}," });
            string be = string.IsNullOrEmpty(ht.Talent.eBE)? "no" : ht.Talent.eBE == "Belastung"?"BE": ht.Talent.eBE;
            lstArg.Add(new dbArgument { Prefix = A + "burden" + A + ":{\"", ArgString = "value" + A + ":\""+ be + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Burden", Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "description" + A + ":{\"", ArgString = "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Description", Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "gmdescription" + A + ":{\"", ArgString = "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Description", Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "Rpr" + A + ":{\"", ArgString = "value" + A + ":\"maybe" + A + "," + A + "type" + A + ":" + A + "String" + A + "," + A + "label" + A + ":" + A + "Routinecheck", Suffix = A + "}," });
            lstArg.Add(new dbArgument { Prefix = A + "StF" + A + ":{\"", ArgString = "value" + A + ":\"" +ht.Talent.Steigerung, Suffix = A + "}}," });

            lstArg.Add(new dbArgument { Prefix = A + "sort" + A + ":", ArgString = (sortNo*100000).ToString(), Suffix = "," });
            lstArg.Add(new dbArgument { Prefix = A + "flags" + A + ":{" });
            lstArg.Add(new dbArgument { Prefix = A + "core" + A + ":{\""+"sourceId"+A+":"+A+ "Item.W0KTljWMKO7otAZR"+A+"}},"+A+"img"+A+":\""+ "icons/tools/hand/spinning-wheel-brown.webp",Suffix= A+","});
            lstArg.Add(new dbArgument { Prefix = A + "effects" + A + ":[]", Suffix="}," });

            return lstArg;
        }
        /*           
             {"_id":"JDDZiwiT2Myr1BjE",
            "name":"Betören",
            "type":"skill",
            "data":
                {
                "group":{"value":"social","type":"String","label":"Skillgroup"},
                "talentValue":{"value":0,"type":"Number","label":"Fw"},
                "characteristic1":{"value":"mu","type":"String","label":"Characteristic1"},
                "characteristic2":{"value":"ch","type":"String","label":"Characteristic2"},
                "characteristic3":{"value":"ch","type":"String","label":"Characteristic3"},
                "RPr":{"value":"no","type":"String","label":"Routinecheck"},
                "burden":{"value":"maybe","type":"String","label":"Burden"},
                "description":{"type":"String","label":"Description","value":""},
                "gmdescription":{"type":"String","label":"Description","value":""},
                "Rpr":{"value":"no","type":"String","label":"Routinecheck"},
                "StF":{"value":"B"}
                },
            "sort":100000,
            "flags":{"core":{"sourceId":"Item.W0KTljWMKO7otAZR"}},"img":"icons/tools/hand/spinning-wheel-brown.webp",
            "effects":[]},
             */

        public void GetGegnerData()
        {
            lstGegnerArgument.Clear();
            MyTimer.start_timer();
            List<string> lstPicKopiert = new List<string>();
            List<string> lstTokenKopiert = new List<string>();
            string MGPfad = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\";

            List<string> lstFTPGegnerPics = new List<string>();
            if (!IsLocalInstalliert)
            {
                List<string> list = new List<string>();
                NetworkCredential credentials = new NetworkCredential(FTPUser, FTPPasswort);
                ListFtpDirectory(FoundryPfad + GegnerPortraitPfad, "", false, credentials, list);
                lstFTPGegnerPics = list;
            }
            foreach (GegnerBase g in lstGegnerBase)
            {
                BildSpeichern(g.Bild, GegnerPortraitPfad, MGPfad, lstFTPGegnerPics, lstPicKopiert);
                BildSpeichern(g.Bild, GegnerTokenPfad, MGPfad, lstFTPGegnerPics, lstTokenKopiert);

                string GetFilenamePortrait = string.IsNullOrEmpty(g.Bild) ? null: (GegnerPortraitPfad.Replace(@"\", "/") + System.IO.Path.GetFileName(g.Bild));
                //Todo: muss auf g.Token abgeändert werden, sobald Token DB vorhanden & GegnerTokenPfad
                string GetFilenameToken = string.IsNullOrEmpty(g.Bild) ? GetFilenamePortrait : (GegnerPortraitPfad.Replace(@"\", "/") + System.IO.Path.GetFileName(g.Bild));

                //char A = (char)34; // (new Char[] { '"' });
                //                       //4e1d3250-f700-3000-0001-387712958942  => 0001387712958942
                string id = g.GegnerBaseGUID.ToString().Substring(19, 17).Replace("-", "");

                GegnerArgument gArg = new GegnerArgument();
                gArg.g = g;
                gArg.lstArguments = new List<dbArgument>();
                if( DSA41Version)
                    gArg.lstArguments.AddRange(SetDSA41Arguments(g, GetFilenamePortrait, GetFilenameToken, id));
                else
                    gArg.lstArguments.AddRange(SetDSA5Arguments(g, GetFilenamePortrait, GetFilenameToken, id));
                gArg.outcome = "";
                foreach (dbArgument arg in gArg.lstArguments)
                { gArg.outcome += arg.Prefix + arg.ArgString + arg.Suffix; }

                lstGegnerArgument.Add(gArg);
            }
            MyTimer.stop_timer("Gegner-DB-Argument");
        }
        public void GetHeldenData()
        {
            lstHeldArgument.Clear();
            MyTimer.start_timer();

            List<string> lstPicKopiert = new List<string>();
            List<string> lstTokenKopiert = new List<string>();
            string MGPfad = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\";

            List<string> lstFTPHeldenPics = new List<string>();
            if (!IsLocalInstalliert)
            {
                List<string> list = new List<string>();
                NetworkCredential credentials = new NetworkCredential(FTPUser, FTPPasswort);
                ListFtpDirectory(FoundryPfad + HeldPortraitPfad, "", false, credentials, list);
                lstFTPHeldenPics = list;
            }

            foreach (Held h in Global.ContextHeld.HeldenGruppeListe.Where(t => t.AktiveHeldengruppe.Value))
            {
                BildSpeichern(h.Bild, HeldPortraitPfad, MGPfad, lstFTPHeldenPics, lstPicKopiert);
                BildSpeichern(h.Bild, HeldTokenPfad, MGPfad, lstFTPHeldenPics, lstTokenKopiert);

                string GetFilenamePortrait = (string.IsNullOrEmpty(h.Bild) ? null : (HeldPortraitPfad.Replace(@"\","/") + System.IO.Path.GetFileName(h.Bild)));
                //Todo: muss auf g.Token abgeändert werden, sobald Token DB vorhanden & GegnerTokenPfad
                string GetFilenameToken = string.IsNullOrEmpty(h.Bild) ? GetFilenamePortrait : (HeldTokenPfad.Replace(@"\", "/") + System.IO.Path.GetFileName(h.Bild));

                char A = (char)34; // (new Char[] { '"' });
                //4e1d3250-f700-3000-0001-387712958942  => 0001387712958942
                string id = h.HeldGUID.ToString().Substring(19, 17).Replace("-", "");

                HeldenArgument hArg = new HeldenArgument();
                hArg.h = h;
                hArg.lstArguments = new List<dbArgument>();
                if (DSA41Version)
                    hArg.lstArguments.AddRange(SetDSA41Arguments(h, GetFilenamePortrait, GetFilenameToken, id));
                else
                    hArg.lstArguments.AddRange(SetDSA5Arguments(h, GetFilenamePortrait, GetFilenameToken, id));
                hArg.outcome = "";
                foreach (dbArgument arg in hArg.lstArguments)
                { hArg.outcome += arg.Prefix + arg.ArgString + arg.Suffix; }

                lstHeldArgument.Add(hArg);                
            }
            MyTimer.stop_timer("Helden-DB-Argument");
        }
        public void Init()
        {
            if (IsLocalInstalliert)
                FoundryPfad = localFoundryPfad;
            else
                FoundryPfad = FTPAdresse + "/data/";

            if (IsLocalInstalliert)
            {
                string appFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string path = appFolderPath + @"\FoundryVTT\Config\";

                if (Directory.Exists(path))
                    ReadFoundryOptions(path + @"\options.json");
            }
            else
            {
                ReadFoundryOptions(FTPAdresse + "/config/options.json");
            }

            GegnerPortraitPfad = Einstellungen.GetEinstellung<string>("FoundryGegnerPortraitPfad");
            HeldPortraitPfad = Einstellungen.GetEinstellung<string>("FoundryHeldPortraitPfad");
            GegnerTokenPfad = Einstellungen.GetEinstellung<string>("FoundryGegnerTokenPfad");
            HeldTokenPfad = Einstellungen.GetEinstellung<string>("FoundryHeldTokenPfad");
            MusikPfad = Einstellungen.GetEinstellung<string>("FoundryMusikPfad");

            //Read Worlds
            if (string.IsNullOrEmpty(FoundryPfad) ||
                (IsLocalInstalliert && !Directory.Exists(FoundryPfad)))
                return;

            LoadWorldsFolder();
        }
        private void CreateFolder(object sender)
        {
            if (SelectedWorld == null)
            {
                ViewHelper.Popup("Wähle zuerst eine Welt");
                return;
            }
            string newFolder = ViewHelper.InputDialog("Erstelle Actor Verzeichnis", "Gebe den Namen des neuen Verzeichnisses\nfür die Actor ein", "");
            
            if (string.IsNullOrEmpty(newFolder))
                return;
            if (lstFolders.FirstOrDefault(t => t.name == newFolder) != null)
            {
                ViewHelper.Popup(string.Format("Das Verzeichnis {0} existiert bereits.\nFunktion abgebrochen", newFolder));
                return;
            }
            char A = (char)34; // (new Char[] { '"' });
            string _id = Guid.NewGuid().ToString().Substring(19, 17).Replace("-", "");
            
            string outdata = "{"+A+"name:"+A+ newFolder + A + "," + A + "type" + A + ":" + A + "Actor" + A + "," + A + "sort" + A + 
                ":null," + A + "flags" + A + ":{}," + A + "parent" + A + ":null," + A + "sorting" + A + ":" + A + "a" + A + 
                "," + A + "color" + A + ":" + A + A + "," + A + "_id" + A + ":" + A + _id + A + "}\n";

            if (IsLocalInstalliert)
            {
                string foldersFilePath = FoundryPfad + @"worlds\" + SelectedWorld + @"\data\folders.db";
                File.AppendAllText(foldersFilePath, outdata);
            }
            else
            {
                string foldersFilePath = FoundryPfad + @"worlds/" + SelectedWorld + "/data/folders.db";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(foldersFilePath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(FTPUser, FTPPasswort, "");
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                string FileData = reader.ReadToEnd();
                Console.WriteLine($"Download Complete, status {response.StatusDescription}");
                reader.Close();
                response.Close();

                FileData += outdata;
                FtpWebRequest request2 = (FtpWebRequest)WebRequest.Create(foldersFilePath);
                request2.Method = WebRequestMethods.Ftp.UploadFile;
                request2.Credentials = new NetworkCredential(FTPUser, FTPPasswort, "");

                // convert contents to byte.
                byte[] fileContents = Encoding.ASCII.GetBytes(FileData);
                request.ContentLength = fileContents.Length;

                using (Stream requestStream = request2.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }
                using (response = (FtpWebResponse)request2.GetResponse())
                {
                    Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }
            }
            List<folder> lst = new List<folder>();
            lst.AddRange(lstFolders);
            lst.Add(new folder() { name = newFolder, color = "", sorting="a", typ="Actor", _id = _id });
            lstFolders = lst;

            ViewHelper.Popup("Verzeichnis erstellt");
        }
        private bool PathExists(string path)
        {
            if (IsLocalInstalliert)
            {
                return Directory.Exists(path);
            }
            else
            {
                // NACH DEM AUFRUF GIBR ES EINE EXCEPTION BEIM NÄCHSTEN ANFRAGEN DER FTP SEITE!!
                try
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
                    request.Method = WebRequestMethods.Ftp.ListDirectory;
                    request.Credentials = new NetworkCredential(FTPUser, FTPPasswort, "");
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    return true;
                }
                catch (WebException ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region //---- EVENTS ----
        private Base.CommandBase _onBtnCreateFolder = null;
        public Base.CommandBase OnBtnCreateFolder
        {
            get
            {
                if (_onBtnCreateFolder == null)
                    _onBtnCreateFolder = new Base.CommandBase(CreateFolder, null);
                return _onBtnCreateFolder;
            }
        }
        
        private Base.CommandBase _onBtnExportGegner = null;
        public Base.CommandBase OnBtnExportGegner
        {
            get
            {
                if (_onBtnExportGegner == null)
                    _onBtnExportGegner = new Base.CommandBase(ExportGegner, null);
                return _onBtnExportGegner;
            }
        }
        private void ExportGegner(object sender)
        {
            try
            {
                if (SelectedWorld == null)
                {
                    ViewHelper.Popup("Wähle zuerst eine Welt");
                    return;
                }
                if ((SelectedGegnerFolder == null || SelectedGegnerFolder.name == "") &&
                    !ViewHelper.Confirm("Export der Gegner", "Die Gegner-Datenbank wird ins das Hauptverzeichnis exportiert\n" +
                    "Wir empfehlen hier dringend zuvor ein Verzeichnis zu erstellen, um Gegner, NSC und Helden zu separieren\n\nSollen die Gegner in das " +
                    "Hauptverzeichnis exportiert werden?"))
                    return;

                System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Wait); 
                MyTimer.start_timer();
                GetGegnerData();

                List<string> lstErsetzt = new List<string>();
                string A = (new Char[] { '"' }).ToString();

                //Open actors.db
                string actorsPath = IsLocalInstalliert ?
                    FoundryPfad + @"worlds\" + SelectedWorld + @"\data\actors.db" :
                    FoundryPfad + @"worlds/" + SelectedWorld + @"/data/actors.db";
                string FileData = GetFileData(actorsPath);
                if (FileData != null)
                {
                    List<string> lstFileData = FileData.Split(new Char[] { '\n' }).ToList(); //oldFileData
                    string newFileDataAdd = "";
                    //Check Gegner vorhanden -> Ja = Zeile löschen und ersetzen
                    foreach (GegnerArgument garg in lstGegnerArgument)
                    {
                        Nullable<int> pos = lstFileData.IndexOf(lstFileData.Where(t => t.Contains("name\":\"" + garg.g.Name + "\",")).FirstOrDefault());
                        if (pos != null && pos != -1)
                        {
                            lstFileData.RemoveAt(pos.Value);
                            lstErsetzt.Add(garg.g.Name);
                        }
                        newFileDataAdd += "\n" + garg.outcome;
                    }
                    string newFile = string.Join("\n", lstFileData);
                    newFile += newFileDataAdd;

                    //Gegner einfügen
                    SetFileData(actorsPath, newFile);
                    MyTimer.stop_timer("");
                    System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
                    ViewHelper.Popup(string.Format("Alle {0} Einträge wurden überprüft. Es wurden {1} aktualisiert und nach Foundry exportiert",
                        lstGegnerArgument.Count, lstErsetzt.Count));
                }
                else
                {
                    System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
                    ViewHelper.Popup("Die Foundry Datenbank 'actors.db' konnte nicht gefunden werden.\n\n Diese Datei sollte unter " +
                        "folgendem Pfad sein:\n" + actorsPath);
                }
            }
            finally
            {
                System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
            }
        }

        private Base.CommandBase _onBtnExportHelden = null;
        public Base.CommandBase OnBtnExportHelden
        {
            get
            {
                if (_onBtnExportHelden == null)
                    _onBtnExportHelden = new Base.CommandBase(ExportHelden, null);
                return _onBtnExportHelden;
            }
        }

        private void ExportHelden(object sender)
        {
            try
            { 
            if (SelectedWorld == null)
            {
                ViewHelper.Popup("Wähle zuerst eine Welt");
                return;
            }
            if ((SelectedHeldenFolder == null || SelectedHeldenFolder.name == "") && 
                !ViewHelper.Confirm("Export der Helden", "Die Helden werden ins das Hauptverzeichnis exportiert\n" +
                "Wir empfehlen hier zuvor ein Verzeichnis zu erstellen, um Helden, NSC und Gegner zu separieren\n\nSollen die Helden in das" +
                "Hauptverzeichnis exportiert werden?"))
                return;

            //if (!PathExists(FoundryPfad + HeldPortraitPfad) || !PathExists(FoundryPfad + HeldTokenPfad))
            //{ 
            //    ViewHelper.Popup(string.Format("Nicht alle Pfade sind vorhanden.\nBitte überprüfe folgende Pfade:\n\n* {0}\n* {1}",
            //        FoundryPfad + HeldPortraitPfad, FoundryPfad + HeldTokenPfad));
            //    return;
            //}

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Wait);
            MyTimer.start_timer();
            GetHeldenData();
            //Init();

            List<string> lstErsetzt = new List<string>();
            string A = (new Char[] { '"' }).ToString();

            //Open actors.db
            string actorsPath = IsLocalInstalliert ?
                FoundryPfad + @"worlds\" + SelectedWorld + @"\data\actors.db" :
                FoundryPfad + @"worlds/" + SelectedWorld + @"/data/actors.db";
            string FileData = GetFileData(actorsPath);
                if (FileData != null)
                {
                    List<string> lstFileData = FileData.Split(new Char[] { '\n' }).ToList();
                    string newFileDataAdd = "";
                    //Check Held vorhanden -> Ja = Zeile löschen und ersetzen
                    foreach (HeldenArgument harg in lstHeldArgument)
                    {
                        Nullable<int> pos = lstFileData.IndexOf(lstFileData.Where(t => t.Contains("name\":\"" + harg.h.Name + "\",")).FirstOrDefault());
                        if (pos != null && pos != -1)
                        {
                            lstFileData.RemoveAt(pos.Value);
                            lstErsetzt.Add(harg.h.Name);
                        }
                        newFileDataAdd += "\n" + harg.outcome;
                    }
                    string newFile = string.Join("\n", lstFileData);
                    newFile += newFileDataAdd;

                    //Held einfügen
                    SetFileData(actorsPath, newFile);
                    System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
                    ViewHelper.Popup(MyTimer.stop_timer("Refresh Helden-Daten aktualisiert in") + "\n\nAlle Helden wurden eingefügt.\n" +
                        "Folgende Helden wurden ersetzt:\n" +(lstErsetzt.Count > 0 ? string.Join("\n", lstErsetzt.Select(t => "  * "+t).ToList()): ""));
                }
                else
                {
                    System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
                    ViewHelper.Popup("Die Foundry Datenbank 'actors.db' konnte nicht gefunden werden.\n\n Diese Datei sollte unter " +
                    "folgendem Pfad sein:\n" + actorsPath);
                }
            }
            finally
            {
                System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
            }
        }


        private Base.CommandBase _onBtnExportPlaylists = null;
        public Base.CommandBase OnBtnExportPlaylists
        {
            get
            {
                if (_onBtnExportPlaylists == null)
                    _onBtnExportPlaylists = new Base.CommandBase(ExportPlaylists, null);
                return _onBtnExportPlaylists;
            }
        }
        private void ExportPlaylists(object sender)
        {
            MyTimer.start_timer();
            System.Globalization.CultureInfo en = new System.Globalization.CultureInfo("en-US", false);
            int anzTotalTitel = 0;
            if (!Directory.Exists(string.Format(@"{0}{1}", FoundryPfad, MusikPfad)))
                Directory.CreateDirectory(string.Format(@"{0}{1}", FoundryPfad, MusikPfad));

            List<PlaylistArgument> lstPArg = new List<PlaylistArgument>();

            string GMid = GetUserID(string.Format(@"{0}worlds\{1}\data\users.db", FoundryPfad, SelectedWorld), "Gamemaster");

            PlaylistStatus = "Alle Playlisten werden exportiert ...";
            foreach (Audio_Playlist aPlaylist in lstPlaylists)
            {
                PlaylistStatus = string.Format("Export '{0}' ...", aPlaylist.Name);
                int anzTitel = aPlaylist.Audio_Playlist_Titel.Count;
                PlaylistArgument pArg = new PlaylistArgument();
                char A = (char)34; // (new Char[] { '"' });
                                    //4e1d3250-f700-3000-0001-387712958942  => 0001387712958942
                pArg.GMid = GMid;
                pArg._id = aPlaylist.Audio_PlaylistGUID.ToString().Substring(19, 17).Replace("-", "");
                pArg.name = aPlaylist.Name;
                pArg.permission = A + "default" + A + ":0," + A + GMid + A + ":3";
                pArg.sort = 100001;
                pArg.mode = 1;
                pArg.playing = "false";

                int current = 1;
                pArg.lstSounds = new List<PlaylistArgument.SoundArg>();
                //List<Audio_Playlist_Titel> lstTitel = new List<Audio_Playlist_Titel>();
                foreach (Audio_Playlist_Titel aTitel in aPlaylist.Audio_Playlist_Titel.OrderBy(t => t.Audio_Titel.Name))
                {
                    string MGpathFile = aTitel.Audio_Titel.Pfad + @"\" + aTitel.Audio_Titel.Datei;
                    if (!File.Exists(MGpathFile)) continue;

                    PlaylistStatus = string.Format("Export '{0}' {1} of {2} ...", aPlaylist.Name, current, anzTitel);
                    PlaylistArgument.SoundArg sArg = new PlaylistArgument.SoundArg();
                    sArg.lstArg = new List<string>();
                    string aTitelTeilGuid = aTitel.Audio_TitelGUID.ToString().Substring(19, 17).Replace("-", "");
                    sArg.lstArg.Add("{" + A + "_id" +A+":"+A+ aTitelTeilGuid+ A );
                    sArg.lstArg.Add( A + "flags" + A + ":{}");

                    stdPfad.ForEach((Action<string>)delegate (string s) {
                        if (MGpathFile.StartsWith(s))
                            MGpathFile = MGpathFile.Replace(s, (string)this.MusikPfad); });
                    string zielPfad = FoundryPfad + MGpathFile;
                    MGpathFile = MGpathFile.Replace(@"\","/");
                    if (!MGpathFile.StartsWith(MusikPfad))
                        continue;

                    sArg.lstArg.Add(A + "path" + A+":"+A+ MGpathFile+ A );
                    sArg.lstArg.Add(A + "repeat" + A + ":" + "false" );
                    sArg.lstArg.Add(A + "volume"+A+ ":" +((decimal)(aTitel.Volume/100)).ToString(en));
                    sArg.lstArg.Add(A + "name"+A + ":" + A+System.IO.Path.GetFileNameWithoutExtension(aTitel.Audio_Titel.Datei) +A );
                    sArg.lstArg.Add(A + "playing" + A + ":false" );
                    sArg.lstArg.Add(A + "streaming" + A + ":false}" );
                    sArg.lstTitel = string.Join(",", sArg.lstArg);
                    pArg.lstSounds.Add(sArg);

                    if (CopyTitelFile && !File.Exists(zielPfad))
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(zielPfad)))
                            Directory.CreateDirectory(Path.GetDirectoryName(zielPfad));
                        File.Copy(aTitel.Audio_Titel.Pfad + @"\" + aTitel.Audio_Titel.Datei, zielPfad, false);

                    }
                    current++;
                    anzTotalTitel++;
                }
                lstPArg.Add(pArg);

                if (anzTotalTitel ==100)
                { }
            }


            //Open playlists.db
            string worldPath = FoundryPfad + @"worlds\" + SelectedWorld + @"\data\";
            string playlistsFile = worldPath + "playlists.db";
            if (OverwritePlaylistFile)
                File.Delete(playlistsFile);
            List<string> lstFileData = new List<string>();
            string oldFileData = (File.Exists(playlistsFile))? File.ReadAllText(playlistsFile): "";

            string newFileData = oldFileData + string.Join("\n", lstPArg.Select(t => t.outtext));

            //Playlisten einfügen
            File.WriteAllText(playlistsFile, newFileData);

            PlaylistStatus = null;
            string stop = MyTimer.stop_timer("");
            PopUp(string.Format("Playlisten erfolgreich übertragen.\nEs wurden {0} in {1} übertragen.", anzTotalTitel, stop));
        }

        /*
         * 
         * 
         {"_id":"DUBBfsfZPdvWN8Mn",
        "name":"neu",
        "permission":{"default":0,"gNZOk6idrMy6uSkk":3},
        "sort":100001,
        "flags":{},
        "sounds":[
        {
            "_id":"BOTQlxGUhlZFk387",
            "flags":{},
            "path":"Musik/_Stra%C3%9Fenmusik/107-peter_balding--as_summer_closes-oma.mp3",
            "repeat":false,"volume":0.35355339059327373,
            "name":"107-peter_balding--as_summer_closes-oma",
            "playing":false,
            "streaming":false
        },
        {
            "_id":"9w7UzHLKZD0GE3Ao",
            "flags":{},
            "path":"Musik/_Stra%C3%9Fenmusik/107-peter_balding--as_summer_closes-oma.mp3",
            "repeat":false,"volume":0.35355339059327373,
            "name":"107-peter_balding--as_summer_closes-oma",
            "playing":false,
            "streaming":false
        },
        {
            "_id":"hMKrznmD7vhNLDNq",
            "flags":{},
            "path":"Musik/_Stra%C3%9Fenmusik/104-miguel_angel_tallante--cruzada-oma.mp3",
            "repeat":false,
            "volume":0.35355339059327373,
            "name":"104-miguel_angel_tallante--cruzada-oma",
            "playing":false,
            "streaming":false
        }],
        "mode":1,
        "playing":false}

         */
        #endregion
    }
}
