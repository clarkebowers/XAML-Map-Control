﻿// XAML Map Control - https://github.com/ClemensFischer/XAML-Map-Control

using System.Collections.Generic;
using System.Linq;
#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#else
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
#endif

namespace MapControl
{
    /// <summary>
    /// A group of ellipses (circle) points defined by a collection of Locations.
    /// </summary>
    public class MapEllipses : MapPath
    {
        public static readonly DependencyProperty LocationsProperty = DependencyProperty.Register(
            nameof(Locations), typeof(IEnumerable<Location>), typeof(MapEllipses),
            new PropertyMetadata(null, (o, e) => ((MapEllipses)o).DataCollectionPropertyChanged(e)));

        /// <summary>
        /// Gets or sets the Locations that define the polyline points.
        /// </summary>
#if !WINDOWS_UWP
        [TypeConverter(typeof(LocationCollectionConverter))]
#endif
        public IEnumerable<Location> Locations
        {
            get { return (IEnumerable<Location>)GetValue(LocationsProperty); }
            set { SetValue(LocationsProperty, value); }
        }

        public MapEllipses()
        {
            Data = new GeometryGroup();
        }

        protected override void UpdateData()
        {
            if (Data != null)
            {
                var group = ((GeometryGroup)Data).Children;

                if (ParentMap != null && Locations != null)
                {
                    var longitudeOffset = GetLongitudeOffset(Location ?? Locations.FirstOrDefault());

                    if (group.Count == Locations.Count())
                    {
                        UpdateEllipsesLocations(group, Locations, longitudeOffset);
                    }
                    else
                    {
                        group.Clear();
                        ((GeometryGroup)Data).Children = AddEllipsesLocations(Locations, longitudeOffset);
                    }
                }
            }
        }
    }
}
