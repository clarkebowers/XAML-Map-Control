// XAML Map Control - https://github.com/ClemensFischer/XAML-Map-Control
// © 2020 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MapControl
{
    public partial class MapPath : Shape, IWeakEventListener
    {
        public static readonly DependencyProperty DataProperty = Path.DataProperty.AddOwner(typeof(MapPath));

        public Geometry Data
        {
            get { return (Geometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        protected override Geometry DefiningGeometry
        {
            get { return Data; }
        }

        #region Method used only by derived classes MapPolyline, MapPolygon and MapMultiPolygon

        protected void DataCollectionPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                CollectionChangedEventManager.RemoveListener(oldCollection, this);
            }

            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                CollectionChangedEventManager.AddListener(newCollection, this);
            }

            UpdateData();
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            UpdateData();
            return true;
        }

        protected void AddPolylineLocations(PathFigureCollection pathFigures, IEnumerable<Location> locations, double longitudeOffset, bool closed)
        {
            if (locations.Count() >= 2)
            {
                var points = locations.Select(location => LocationToView(location, longitudeOffset));
                var figure = new PathFigure
                {
                    StartPoint = points.First(),
                    IsClosed = closed,
                    IsFilled = closed
                };

                figure.Segments.Add(new PolyLineSegment(points.Skip(1), true));
                pathFigures.Add(figure);
            }
        }

        protected void UpdateEllipsesLocations(
            GeometryCollection group,
            IEnumerable<Location> locations,
            double longitudeOffset)
        {
            Debug.Assert(group.Count == locations.Count());
            var i = 0;
            foreach (var location in locations)
            {
                ((EllipseGeometry)group[i]).Center = LocationToView(location, longitudeOffset);
                i++;
            }
        }

        protected GeometryCollection AddEllipsesLocations(
            IEnumerable<Location> locations, 
            double longitudeOffset)
        {
            var One = new Vector(1, 1);
            var points = locations.Select(location => LocationToView(location, longitudeOffset));
            var elipses = points.Select(p => createEllipseGeometry(p));
            var group = new GeometryCollection(elipses);
            return group;
        }

        private EllipseGeometry createEllipseGeometry(Point center)
        {
            return new EllipseGeometry
            {
                RadiusX = 1,
                RadiusY = 1,
                Center = center,
            };
        }

        #endregion
    }
}
