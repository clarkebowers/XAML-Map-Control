// XAML Map Control - https://github.com/ClemensFischer/XAML-Map-Control
// © 2020 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

using System;
#if WINDOWS_UWP
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Media;
#endif

namespace MapControl
{
    /// <summary>
    /// Defines the transformation between cartesian map coordinates in meters
    /// and view coordinates in pixels.
    /// </summary>
    public class ViewTransform
    {
        public static double ZoomLevelToScale(double zoomLevel)
        {
            return 256d * Math.Pow(2d, zoomLevel) / (360d * MapProjection.Wgs84MetersPerDegree);
        }

        public static double ScaleToZoomLevel(double scale)
        {
            return Math.Log(scale * 360d * MapProjection.Wgs84MetersPerDegree / 256d, 2d);
        }

        /// <summary>
        /// Gets the scaling factor from cartesian map coordinates to view coordinates,
        /// i.e. pixels per meter.
        /// </summary>
        public double Scale { get; private set; }

        /// <summary>
        /// Gets the rotation angle of the transform matrix.
        /// </summary>
        public double Rotation { get; private set; }

        /// <summary>
        /// Gets the transform matrix from cartesian map coordinates to view coordinates.
        /// </summary>
        public Matrix MapToViewMatrix { get; private set; }

        /// <summary>
        /// Gets the transform matrix from view coordinates to cartesian map coordinates.
        /// </summary>
        public Matrix ViewToMapMatrix { get; private set; }

        /// <summary>
        /// Transforms a Point from cartesian map coordinates to view coordinates.
        /// </summary>
        public Point MapToView(Point point)
        {
            return MapToViewMatrix.Transform(point);
        }

        /// <summary>
        /// Transforms a Point from view coordinates to cartesian map coordinates.
        /// </summary>
        public Point ViewToMap(Point point)
        {
            return ViewToMapMatrix.Transform(point);
        }

        public void SetTransform(Point mapCenter, Point viewCenter, double scale, double rotation)
        {
            Scale = scale;
            Rotation = rotation;

            var transform = new Matrix(Scale, 0d, 0d, -Scale, -Scale * mapCenter.X, Scale * mapCenter.Y);

            transform.Rotate(Rotation);
            transform.Translate(viewCenter.X, viewCenter.Y);

            MapToViewMatrix = transform;

            transform.Invert();

            ViewToMapMatrix = transform;
        }
    }
}
