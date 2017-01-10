using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Mitten.Mobile.iOS.Views.Renderers
{
    /// <summary>
    /// Handles rendering an activity spinner onto a UIView.
    /// </summary>
    public class ActivitySpinnerRenderer : Renderer
    {
        private static class Constants
        {
            public const double StrokeEndAnimationBeginTime = 0;
            public const double StrokeStartAnimationBeginTime = 0.4;

            public const double StrokeAnimationDuration = 0.75;
            public const double RotationAnimationDuration = 2;

            public static readonly double TotalStrokeAnimationDuration = 
                Math.Max(Constants.StrokeStartAnimationBeginTime, Constants.StrokeEndAnimationBeginTime) +
                Constants.StrokeAnimationDuration;

            public const float DefaultLineWidth = 6.0f;
        }

        private readonly UIColor spinnerColor;
        private readonly float lineWidth;

        /// <summary>
        /// Initializes a new instance of the ActivitySpinnerRenderer class.
        /// </summary>
        /// <param name="view">A view to render on to.</param>
        /// <param name="spinnerColor">The color for the spinner.</param>
        /// <param name="lineWidth">The width of the indicator line.</param>
        public ActivitySpinnerRenderer(UIView view, UIColor spinnerColor, float lineWidth = Constants.DefaultLineWidth)
            : base(view)
        {
            this.View.BackgroundColor = UIColor.Clear;
            this.spinnerColor = spinnerColor;
            this.lineWidth = lineWidth;
        }

        /// <summary>
        /// Renders the activity spinner and starts the animation.
        /// </summary>
        public void RenderToViewWithAnimation()
        {
            CAShapeLayer circleLayer = new CAShapeLayer();

            nfloat circleSize = (float)Math.Min(this.View.Bounds.Width, this.View.Bounds.Height);

            circleLayer.Path = this.CreateCirclePath(circleSize, this.lineWidth);
            circleLayer.Position = new CGPoint(this.View.Bounds.GetMidX(), this.View.Bounds.GetMidY());
            circleLayer.StrokeColor = this.spinnerColor.CGColor;
            circleLayer.FillColor = UIColor.Clear.CGColor;
            circleLayer.LineWidth = this.lineWidth;
            circleLayer.LineCap = CAShapeLayer.CapRound;

            CAAnimation strokeEndAnimation = this.CreateStrokeAnimation("strokeEnd", Constants.StrokeEndAnimationBeginTime);
            CAAnimation strokeStartAnimation = this.CreateStrokeAnimation("strokeStart", Constants.StrokeStartAnimationBeginTime);
            CAAnimation rotationAnimation = this.CreateRotationAnimation();

            circleLayer.AddAnimation(strokeEndAnimation, null);
            circleLayer.AddAnimation(strokeStartAnimation, null);
            circleLayer.AddAnimation(rotationAnimation, null);

            this.View.Layer.AddSublayer(circleLayer);
        }

        private CAAnimation CreateStrokeAnimation(string keyPath, double beginTime)
        {
            CABasicAnimation animation = new CABasicAnimation();

            animation.KeyPath = keyPath;
            animation.BeginTime = beginTime;
            animation.Duration = Constants.StrokeAnimationDuration;
            animation.From = NSObject.FromObject(0);
            animation.To = NSObject.FromObject(1);
            animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut); 

            CAAnimationGroup group = new CAAnimationGroup();
            group.Duration = Constants.TotalStrokeAnimationDuration;
            group.RepeatCount = float.MaxValue;
            group.Animations = new[] { animation };

            return group;
        }

        private CAAnimation CreateRotationAnimation()
        {
            CABasicAnimation animation = new CABasicAnimation();

            animation.KeyPath = "transform.rotation.z";
            animation.From = NSObject.FromObject(0);
            animation.To = NSObject.FromObject(Math.PI * 2);
            animation.Duration = Constants.RotationAnimationDuration;
            animation.RepeatCount = float.MaxValue;
            animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);

            return animation;
        }
    }
}