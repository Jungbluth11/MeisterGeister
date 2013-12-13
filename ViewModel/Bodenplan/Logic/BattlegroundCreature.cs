﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MeisterGeister.ViewModel.Bodenplan.Logic
{
    class BattlegroundCreature:BattlegroundBaseObject
    {
        private string _portraitFilename;
        private double _enemyX=100;
        private double _enemyY=100;

        public static String ICON_DIR = "/Images/Icons/General/";

        public double EnemyX
        {
            get { return _enemyX; }
            set
            {
                _enemyX = value;
                OnPropertyChanged("EnemyX");
            }
        }

        public double EnemyY
        {
            get { return _enemyY; }
            set
            {
                _enemyY = value;
                OnPropertyChanged("EnemyY");
            }
        }

        public String PortraitFileName
        {
            get { return _portraitFilename; }
            set { _portraitFilename = value; }
        }

        public Image LoadImage(Uri uri)
        {
            Image portrait = new Image();
            //portrait.Width = Width - 2 * frameWidth;
            //portrait.Height = Height - 2 * frameWidth;

            BitmapImage myBitmapImage = new BitmapImage();

            try
            {

                myBitmapImage.BeginInit();
                //myBitmapImage.UriSource = new Uri(@ArenaWindow.PORTRAIT_DIR + filename, UriKind.Relative);
                myBitmapImage.UriSource = uri;
                myBitmapImage.DecodePixelWidth = 256;// (int)portrait.Width;
                myBitmapImage.DecodePixelHeight = 256;// (int)portrait.Height;
                myBitmapImage.EndInit();
                portrait.Source = myBitmapImage;

            }
            catch
            {
                portrait = null;
            }

            return portrait;
        }
    }
}
