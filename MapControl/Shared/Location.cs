// XAML Map Control - https://github.com/ClemensFischer/XAML-Map-Control
// © 2020 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;

namespace MapControl
{
    /// <summary>
    /// A geographic location with latitude and longitude values in degrees.
    /// </summary>
#if !WINDOWS_UWP
    [System.ComponentModel.TypeConverter(typeof(LocationConverter))]
#endif
    public class Location : IEquatable<Location>
    {
        public const double Wgs84EquatorialRadius = 6378137d;
        public const double Wgs84MetersPerDegree = Wgs84EquatorialRadius * Math.PI / 180d;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DegreesToRadians(double degrees)
        {
            return degrees / 180.0 * Math.PI;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }

        private double latitude;
        private double longitude;

        public Location()
        {
        }

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude
        {
            get { return latitude; }
            set { latitude = Math.Min(Math.Max(value, -90d), 90d); }
        }

        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        public bool Equals(Location location)
        {
            return location != null
                && Math.Abs(location.latitude - latitude) < 1e-9
                && Math.Abs(location.longitude - longitude) < 1e-9;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Location);
        }

        public override int GetHashCode()
        {
            return latitude.GetHashCode() ^ longitude.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:F5},{1:F5}", latitude, longitude);
        }

        /// <summary>
        /// Place on the surface of the earth in cartesian coordinates where
        /// the origin is the center of the planet.
        /// 
        //Here are conversion algorythms: 
        //https://stackoverflow.com/questions/1185408/converting-from-longitude-latitude-to-cartesian-coordinates
        //http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        /// <returns></returns>
        public Point3D ToPoint3D()
        {
            var lat = DegreesToRadians(Latitude);
            var lon = DegreesToRadians(Longitude);
            var x = Wgs84EquatorialRadius * Math.Cos(lat) * Math.Cos(lon);
            var y = Wgs84EquatorialRadius * Math.Cos(lat) * Math.Sin(lon);
            var z = Wgs84EquatorialRadius * Math.Sin(lat);
            return new Point3D(x, y, z);
        }

        public static Location operator+(Location x, Location y)
        {
            return new Location(x.latitude + y.latitude, NormalizeLongitude(x.longitude + y.longitude));
        }

        public static Location Parse(string locationString)
        {
            Location location = null;

            if (!string.IsNullOrEmpty(locationString))
            {
                var values = locationString.Split(new char[] { ',' });

                if (values.Length != 2)
                {
                    throw new FormatException("Location string must be a comma-separated pair of double values.");
                }

                location = new Location(
                    double.Parse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture),
                    double.Parse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture));
            }

            return location;
        }

        /// <summary>
        /// Normalizes a longitude to a value in the interval [-180 .. 180].
        /// </summary>
        public static double NormalizeLongitude(double longitude)
        {
            if (longitude < -180d)
            {
                longitude = ((longitude + 180d) % 360d) + 180d;
            }
            else if (longitude > 180d)
            {
                longitude = ((longitude - 180d) % 360d) - 180d;
            }

            return longitude;
        }
    }
}
