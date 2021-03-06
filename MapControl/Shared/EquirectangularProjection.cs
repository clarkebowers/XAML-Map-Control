﻿// XAML Map Control - https://github.com/ClemensFischer/XAML-Map-Control
// © 2020 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

using System;
using System.Globalization;
#if WINDOWS_UWP
using Windows.Foundation;
#else
using System.Windows;
#endif

namespace MapControl
{
    /// <summary>
    /// Equirectangular Projection.
    /// Longitude and Latitude values are transformed linearly to X and Y values in meters.
    /// </summary>
    public class EquirectangularProjection : MapProjection
    {
        public EquirectangularProjection()
        { }

        public override bool IsNormalCylindrical
        {
            get { return true; }
        }

        public override Vector GetRelativeScale(Location location)
        {
            return new Vector(
                1d / Math.Cos(location.Latitude * Math.PI / 180d),
                1d);
        }

        public override Point LocationToMap(Location location)
        {
            return new Point(
                Wgs84MetersPerDegree * location.Longitude,
                Wgs84MetersPerDegree * location.Latitude);
        }

        public override Location MapToLocation(Point point)
        {
            return new Location(
                point.Y / Wgs84MetersPerDegree,
                point.X / Wgs84MetersPerDegree);
        }

    }
}
