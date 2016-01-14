﻿using MeisterGeister.Model;
using MeisterGeister.Model.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeisterGeister.Logic.Karte
{
    public class RoutingService
    {
        protected GeoService _geoService;

        public RoutingService()
        {
            this._geoService = new GeoService();
        }

        public double GetZoomAdjustment(Size zoomControlSize, double zoom, Point center, Point routeStartingPoint)
        {
            // Standardmäßig soll keine Änderung stattfinden
            double result = 1.0;

            // Der Zoom muss erst Mal normalisiert werden
            Size normalizedSize = new Size(zoomControlSize.Width / zoom, zoomControlSize.Height / zoom);

            // Anschließend ist die höhere Distance zum zu vergleichenden Punkt zu ermitteln
            double xDistanceToTarget = Math.Abs(center.X - routeStartingPoint.X);
            double yDistanceToTarget = Math.Abs(center.Y - routeStartingPoint.Y);
            bool isXDistanceHigher = xDistanceToTarget > yDistanceToTarget;

            // Als nächstes wird geschaut, ob der Punkt außerhalb des sichtbaren Bereichs liegt
            double distanceToCompare = isXDistanceHigher ? xDistanceToTarget : yDistanceToTarget;
            double distanceToBorder = isXDistanceHigher ? normalizedSize.Width / 2 : normalizedSize.Height / 2;
            bool isOutOfSight = distanceToBorder <= distanceToCompare;

            // Falls dem so ist, muss der Zoom reduziert werden. Der Faktor 1.2 sorgt dafür, 
            // dass die beiden Punkte nicht genau auf dem Rand des sichtbaren Bereichs liegen.
            if (isOutOfSight)
                result = (distanceToCompare / distanceToBorder) * 1.2;

            return result;
        }

        public void GetShortestPath(Point actualStart, Point actualTarget)
        {
            Ort start = GetClosestOrt(actualStart);
            Ort target = GetClosestOrt(actualTarget);

            if(start != null & target != null)
            {
                GetShortestPath(start, target);
            }
        }

        private void GetShortestPath(Ort start, Ort target)
        {
            
        }

        public Ort GetClosestOrt(Point actualStart)
        {
            return _geoService.LoadClosestOrt(actualStart);
        }
    }
}
