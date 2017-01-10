using System;
using CoreGraphics;
using UIKit;

namespace Mitten.Mobile.iOS.Views.Renderers
{
    /// <summary>
    /// Base class for renderers. A renderer is intended to handle drawing static or animated graphics
    /// on a UIView surface. Using custom controls in the storyboard designer is problematic so this
    /// is intended as a work around, we can add placeholder UIViews to the designer and then apply
    /// renderers at runtime.
    /// </summary>
    public abstract class Renderer
    {
        /// <summary>
        /// Initializes a new instance of the Renderer class.
        /// </summary>
        /// <param name="view">The view this renderer will render on to.</param>
        protected Renderer(UIView view)
        {
            Throw.IfArgumentNull(view, "view");
            this.View = view;
        }

        /// <summary>
        /// Gets the view for the renderer.
        /// </summary>
        public UIView View { get; private set; }

        /// <summary>
        /// Creates a circular path.
        /// </summary>
        /// <param name="diameter">The diameter of the circle.</param>
        /// <returns>A circular path.</returns>
        protected CGPath CreateCirclePath(nfloat diameter)
        {
            return this.CreateCirclePath(diameter, 1.0f);
        }

        /// <summary>
        /// Creates a circular path.
        /// </summary>
        /// <param name="diameter">The diameter of the circle.</param>
        /// <param name="lineWidth">The line width, the default is 1.0.</param>
        /// <returns>A circular path.</returns>
        protected CGPath CreateCirclePath(nfloat diameter, nfloat lineWidth)
        {
            return
                this.CreateCirclePath(
                    diameter,
                    lineWidth,
                    0.0f,
                    this.DegreesToRadians(360.0f));
        }

        /// <summary>
        /// Creates a circular path.
        /// </summary>
        /// <param name="diameter">The diameter of the circle.</param>
        /// <param name="lineWidth">The line width, the default is 1.0.</param>
        /// <param name="startAngleAsRadians">The start angle for the circle as radians.</param>
        /// <param name="endAngleAsRadians">The end angle for the circle as radians.</param>
        /// <returns>A circular path.</returns>
        protected CGPath CreateCirclePath(
            nfloat diameter,
            nfloat lineWidth,
            nfloat startAngleAsRadians,
            nfloat endAngleAsRadians)
        {
            UIBezierPath path = new UIBezierPath();
            path.LineWidth = lineWidth;

            nfloat radius = diameter / 2.0f;

            if (lineWidth > 1.0f)
            {
                radius = radius - lineWidth / 2.0f;
            }

            path.AddArc(
                CGPoint.Empty,
                radius,
                startAngleAsRadians,
                endAngleAsRadians,
                clockWise: true);

            return path.CGPath;
        }

        /// <summary>
        /// Converts the given degrees to radians.
        /// </summary>
        /// <param name="degrees">The degrees.</param>
        /// <returns>The degrees converted to radians.</returns>
        protected nfloat DegreesToRadians(nfloat degrees)
        {
            return (nfloat)((degrees * Math.PI) / 180.0f);
        }
    }
}
