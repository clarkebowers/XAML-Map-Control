// XAML Map Control - https://github.com/ClemensFischer/XAML-Map-Control
// © 2020 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;

namespace MapControl
{
    /// <summary>
    /// A collection of Locations with support for parsing.
    /// </summary>
#if !WINDOWS_UWP
    [System.ComponentModel.TypeConverter(typeof(LocationCollectionConverter))]
#endif
    public class LocationCollection : List<Location>
    {
        public LocationCollection()
        {
        }

        public LocationCollection(IEnumerable<Location> locations)
            : base(locations)
        {
        }

        public LocationCollection(params Location[] locations)
            : base(locations)
        {
        }

        public static LocationCollection Parse(string s)
        {
            var strings = s.Split(new char[] { ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);

            return new LocationCollection(strings.Select(l => Location.Parse(l)));
        }

        /// <summary>
        /// Find the top left
        /// </summary>
        /// <param name="locations"></param>
        /// <returns></returns>
        public static Location Min(IEnumerable<Location> locations)
        {
            var minLon = locations.Min(l => l.Longitude);
            var minLat = locations.Min(l => l.Latitude);
            return new Location(minLat, minLon);
        }

        /// <summary>
        /// Find the bottom right
        /// </summary>
        /// <param name="locations"></param>
        /// <returns></returns>
        public static Location Max(IEnumerable<Location> locations)
        {
            var maxLon = locations.Max(l => l.Longitude);
            var maxLat = locations.Max(l => l.Latitude);
            return new Location(maxLat, maxLon);
        }

        public static (Location, Location) Extent(IEnumerable<Location> locations)
        {
            return (Min(locations),Max(locations));
        }

        /// <summary>
        /// Find the center of the bounding box of a collection of locations
        /// </summary>
        /// <param name="locations"></param>
        /// <returns></returns>
        public static Location Center(IEnumerable<Location> locations)
        {
            var minLon = locations.Min(l => l.Longitude);
            var maxLon = locations.Max(l => l.Longitude);
            var minLat = locations.Min(l => l.Latitude);
            var maxLat = locations.Max(l => l.Latitude);
            return new Location(
                (minLat + maxLat) / 2,
                (minLon + maxLon) / 2);
        }

    }
}
