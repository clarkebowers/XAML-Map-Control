// XAML Map Control - https://github.com/ClemensFischer/XAML-Map-Control
// © 2020 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
#if WINDOWS_UWP
using Windows.Foundation;
#else
using System.Windows;
#endif

namespace MapControl
{
    /// <summary>
    /// Defines a map projection between geographic coordinates and cartesian map coordinates.
    /// </summary>
    public abstract class MapProjection
    {
        public const double Wgs84EquatorialRadius = 6378137d;
        public const double Wgs84MetersPerDegree = Wgs84EquatorialRadius * Math.PI / 180d;
        public const double Wgs84Flattening = 1d / 298.257223563;
        public static readonly double Wgs84Eccentricity = Math.Sqrt((2d - Wgs84Flattening) * Wgs84Flattening);
        public static readonly Vector One = new Vector(1.0, 1.0);

        /// <summary>
        /// Gets or sets the projection center.
        /// </summary>
        public Location Center { get; set; } = new Location();

        /// <summary>
        /// Indicates if this is a normal cylindrical projection.
        /// </summary>
        public virtual bool IsNormalCylindrical
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the absolute value of the minimum and maximum latitude that can be transformed.
        /// </summary>
        public virtual double MaxLatitude
        {
            get { return 90d; }
        }

        /// <summary>
        /// Gets the relative map scale at the specified Location.
        /// Relative to what? the center of the map?
        /// relative x to y (the two axis)?
        /// </summary>
        public virtual Vector GetRelativeScale(Location location)
        {
            return One;
        }

        /// <summary>
        /// Transforms a Location in geographic coordinates to a Point in cartesian map coordinates.
        /// </summary>
        public abstract Point LocationToMap(Location location);

        /// <summary>
        /// Transforms a Point in cartesian map coordinates to a Location in geographic coordinates.
        /// </summary>
        public abstract Location MapToLocation(Point point);

        /// <summary>
        /// Transforms a BoundingBox in geographic coordinates to a Rect in cartesian map coordinates.
        /// </summary>
        public virtual Rect BoundingBoxToRect(BoundingBox boundingBox)
        {
            return new Rect(
                LocationToMap(new Location(boundingBox.South, boundingBox.West)),
                LocationToMap(new Location(boundingBox.North, boundingBox.East)));
        }

        /// <summary>
        /// Transforms a Rect in cartesian map coordinates to a BoundingBox in geographic coordinates.
        /// </summary>
        public virtual BoundingBox RectToBoundingBox(Rect rect)
        {
            var sw = MapToLocation(new Point(rect.X, rect.Y));
            var ne = MapToLocation(new Point(rect.X + rect.Width, rect.Y + rect.Height));

            return new BoundingBox(sw.Latitude, sw.Longitude, ne.Latitude, ne.Longitude);
        }

        /// <summary>
        /// Logitude should be +/-180 (π)
        /// </summary>
        /// <param name="longitude"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected double NormalizeLongitude(double longitude)
        {
            return longitude % 180.0;
        }

    }
}
