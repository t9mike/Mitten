using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace Mitten.Mobile.iOS.Views.Renderers
{
    /// <summary>
    /// Handles rendering the graphics for a check box view.
    /// </summary>
    internal class CheckBoxRenderer : Renderer
    {
        private static class Constants
        {
            public const float LineWidth = 1.0f;
            public const float InnerCircleDiff = 6.0f;
            public const float MinimumSize = 10.0f;
        }

        private readonly CheckBoxView checkBoxView;

        /// <summary>
        /// Initializes a new instance of the CheckBoxRenderer class.
        /// </summary>
        /// <param name="checkBoxView">A check box view.</param>
        public CheckBoxRenderer(CheckBoxView checkBoxView)
            : base(checkBoxView)
        {
            this.checkBoxView = checkBoxView;
        }

        /// <summary>
        /// Renders the check box graphics to the current check box view.
        /// </summary>
        public void RenderToView()
        {
            this.View.Layer.Sublayers = null;

            nfloat outerCircleSize = (float)Math.Min(this.View.Bounds.Width, this.View.Bounds.Height);
            if (outerCircleSize >= Constants.MinimumSize)
            {
                CAShapeLayer outerCircleLayer = new CAShapeLayer();
                outerCircleLayer.Path = this.CreateCirclePath(outerCircleSize, Constants.LineWidth);
                outerCircleLayer.Position = new CGPoint(this.View.Bounds.GetMidX(), this.View.Bounds.GetMidY());
                outerCircleLayer.StrokeColor = this.checkBoxView.Color;
                outerCircleLayer.FillColor = UIColor.Clear.CGColor;
                outerCircleLayer.LineWidth = Constants.LineWidth;
                outerCircleLayer.LineCap = CAShapeLayer.CapRound;

                this.View.Layer.AddSublayer(outerCircleLayer);

                if (this.GetVisualCheckedState())
                {
                    nfloat innerCircleSize = outerCircleSize - Constants.InnerCircleDiff;

                    CAShapeLayer innerCircleLayer = new CAShapeLayer();
                    innerCircleLayer.Path = this.CreateCirclePath(innerCircleSize);
                    innerCircleLayer.Position = new CGPoint(this.View.Bounds.GetMidX(), this.View.Bounds.GetMidY());
                    innerCircleLayer.FillColor = this.checkBoxView.Color;

                    this.View.Layer.AddSublayer(innerCircleLayer);
                }
            }
        }

        private bool GetVisualCheckedState()
        {
            return
                this.checkBoxView.IsPressed
                ? !this.checkBoxView.IsChecked
                : this.checkBoxView.IsChecked;
        }
    }
}