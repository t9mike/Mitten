using System;
using System.Collections.Generic;
using UIKit;

namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// Contains extension methods for a UIView
    /// </summary>
    /// <remarks>
    /// For a good understanding of how a constraint works, see:
    /// https://developer.apple.com/library/content/documentation/UserExperience/Conceptual/AutolayoutPG/AnatomyofaConstraint.html
    /// </remarks>
    public static class UIViewExtensions
    {
        /// <summary>
        /// Anchors the view to its parent, the parent is considered a container meaning the view will be anchored on the inside edges of its parent.
        /// For instance, a constant value of 5 from the right edge will anchor the child to remain 5 spaces to the left of the parent's right edge to remain inside the parent.
        /// </summary>
        /// <param name="view">The view to anchor.</param>
        /// <param name="edges">The edges to anchor.</param>
        /// <param name="constant">A constant identifying the edge margins when anchoring to the parent.</param>
        /// <returns>The NSLayoutConstraints for the anchor.</returns>
        public static NSLayoutConstraint[] Anchor(this UIView view, AnchorEdges edges = AnchorEdges.All, float constant = 0)
        {
            UIViewExtensions.EnsureHasParent(view);

            List<NSLayoutConstraint> constraints = new List<NSLayoutConstraint>();

            if ((edges & AnchorEdges.Top) == AnchorEdges.Top)
            {
                constraints.Add(UIViewExtensions.Activate(view.TopAnchor.ConstraintEqualTo(view.Superview.TopAnchor, constant)));
            }

            if ((edges & AnchorEdges.Bottom) == AnchorEdges.Bottom)
            {
                constraints.Add(UIViewExtensions.Activate(view.BottomAnchor.ConstraintEqualTo(view.Superview.BottomAnchor, -constant)));
            }

            if ((edges & AnchorEdges.Left) == AnchorEdges.Left)
            {
                constraints.Add(UIViewExtensions.Activate(view.LeftAnchor.ConstraintEqualTo(view.Superview.LeftAnchor, constant)));
            }

            if ((edges & AnchorEdges.Right) == AnchorEdges.Right)
            {
                constraints.Add(UIViewExtensions.Activate(view.RightAnchor.ConstraintEqualTo(view.Superview.RightAnchor, -constant)));
            }

            return constraints.ToArray();
        }

        /// <summary>
        /// Centers the view both vertically and horizontally to its parent.
        /// </summary>
        /// <param name="view">A view to center.</param>
        /// <returns>The NSLayoutConstraints for the anchor.</returns>
        public static NSLayoutConstraint[] AnchorCenter(this UIView view)
        {
            UIViewExtensions.EnsureHasParent(view);
            return view.AnchorCenter(view.Superview);
        }

        /// <summary>
        /// Centers the view both vertically and horizontally to the center of another view.
        /// </summary>
        /// <param name="view">A view to center.</param>
        /// <param name="secondView">The second view to center to.</param>
        /// <returns>The NSLayoutConstraints for the anchor.</returns>
        public static NSLayoutConstraint[] AnchorCenter(this UIView view, UIView secondView)
        {
            NSLayoutConstraint[] constraints = new NSLayoutConstraint[2];

            constraints[0] = view.AnchorCenterX(secondView);
            constraints[1] = view.AnchorCenterY(secondView);

            return constraints;
        }

        /// <summary>
        /// Centers the view horizontally to the parent.
        /// </summary>
        /// <param name="view">A view to center.</param>
        /// <param name="constant">An optional offset value when centering.</param>
        /// <returns>The NSLayoutConstraint for the anchor.</returns>
        public static NSLayoutConstraint AnchorCenterX(this UIView view, float constant = 0)
        {
            UIViewExtensions.EnsureHasParent(view);
            return view.AnchorCenterX(view.Superview, constant);
        }

        /// <summary>
        /// Centers the view horizontally to the center of another view.
        /// </summary>
        /// <param name="view">A view to center.</param>
        /// <param name="secondView">The second view to center to.</param>
        /// <param name="constant">An optional offset value when centering.</param>
        /// <returns>The NSLayoutConstraint for the anchor.</returns>
        public static NSLayoutConstraint AnchorCenterX(this UIView view, UIView secondView, float constant = 0)
        {
            return UIViewExtensions.Activate(view.CenterXAnchor.ConstraintEqualTo(secondView.CenterXAnchor, constant));
        }

        /// <summary>
        /// Centers the view vertically to the parent.
        /// </summary>
        /// <param name="view">A view to center.</param>
        /// <param name="constant">An optional offset value when centering.</param>
        /// <returns>The NSLayoutConstraint for the anchor.</returns>
        public static NSLayoutConstraint AnchorCenterY(this UIView view, float constant = 0)
        {
            UIViewExtensions.EnsureHasParent(view);
            return view.AnchorCenterY(view.Superview, constant);
        }

        /// <summary>
        /// Centers the view vertically to the center of another view.
        /// </summary>
        /// <param name="view">A view to center.</param>
        /// <param name="secondView">The second view to center to.</param>
        /// <param name="constant">An optional offset value when centering.</param>
        /// <returns>The NSLayoutConstraint for the anchor.</returns>
        public static NSLayoutConstraint AnchorCenterY(this UIView view, UIView secondView, float constant = 0)
        {
            UIViewExtensions.EnsureHasParent(view);
            return UIViewExtensions.Activate(view.CenterYAnchor.ConstraintEqualTo(secondView.CenterYAnchor, constant));
        }

        /// <summary>
        /// Anchors the view above a layout guide.
        /// </summary>
        /// <param name="view">A view to anchor.</param>
        /// <param name="layoutGuide">A layout guide to anchor to.</param>
        /// <param name="constant">An offset between the two objects.</param>
        /// <returns>The NSLayoutConstraint for the anchor.</returns>
        public static NSLayoutConstraint AnchorAbove(this UIView view, IUILayoutSupport layoutGuide, float constant = 0)
        {
            Throw.IfArgumentNull(layoutGuide, nameof(layoutGuide));
            return UIViewExtensions.Activate(view.BottomAnchor.ConstraintEqualTo(layoutGuide.GetTopAnchor(), -constant));
        }

        /// <summary>
        /// Anchors the view above another.
        /// </summary>
        /// <param name="view">A view to anchor.</param>
        /// <param name="bottomView">A view to place below the current view.</param>
        /// <param name="constant">An offset between the two views.</param>
        /// <returns>The NSLayoutConstraint for the anchor.</returns>
        public static NSLayoutConstraint AnchorAbove(this UIView view, UIView bottomView, float constant = 0)
        {
            Throw.IfArgumentNull(bottomView, nameof(bottomView));
            return UIViewExtensions.Activate(view.BottomAnchor.ConstraintEqualTo(bottomView.TopAnchor, -constant));
        }

        /// <summary>
        /// Anchors the view below a layout guide.
        /// </summary>
        /// <param name="view">A view to anchor.</param>
        /// <param name="layoutGuide">A layout guide to anchor to.</param>
        /// <param name="constant">An offset between the two objects.</param>
        /// <returns>The NSLayoutConstraint for the anchor.</returns>
        public static NSLayoutConstraint AnchorBelow(this UIView view, IUILayoutSupport layoutGuide, float constant = 0)
        {
            Throw.IfArgumentNull(layoutGuide, nameof(layoutGuide));
            return UIViewExtensions.Activate(view.TopAnchor.ConstraintEqualTo(layoutGuide.GetBottomAnchor(), constant));
        }

        /// <summary>
        /// Anchors the view below another.
        /// </summary>
        /// <param name="view">A view to anchor.</param>
        /// <param name="topView">A view to place above the current view.</param>
        /// <param name="constant">An offset between the two views.</param>
        /// <returns>The NSLayoutConstraint for the anchor.</returns>
        public static NSLayoutConstraint AnchorBelow(this UIView view, UIView topView, float constant = 0)
        {
            Throw.IfArgumentNull(topView, nameof(topView));
            return UIViewExtensions.Activate(view.TopAnchor.ConstraintEqualTo(topView.BottomAnchor, constant));
        }

        /// <summary>
        /// Anchors the width and height of the view.
        /// </summary>
        /// <param name="view">A view to anchor.</param>
        /// <param name="constant">The width and height.</param>
        /// <returns>The NSLayoutConstraints for the anchor.</returns>
        public static NSLayoutConstraint[] AnchorSize(this UIView view, float constant)
        {
            NSLayoutConstraint[] constraints = new NSLayoutConstraint[2];

            constraints[0] = view.AnchorHeight(constant);
            constraints[1] = view.AnchorWidth(constant);

            return constraints;
        }

        /// <summary>
        /// Anchors the height of the view.
        /// </summary>
        /// <param name="view">A view to anchor.</param>
        /// <param name="constant">The height.</param>
        /// <returns>The NSLayoutConstraint for the anchor.</returns>
        public static NSLayoutConstraint AnchorHeight(this UIView view, float constant = 0)
        {
            return UIViewExtensions.Activate(view.HeightAnchor.ConstraintEqualTo(constant));
        }

        /// <summary>
        /// Anchors the height of the view to have the same height as another view.
        /// </summary>
        /// <param name="view">A view to anchor.</param>
        /// <param name="secondView">A second view.</param>
        /// <param name="constant">A constant offset.</param>
        /// <returns>The NSLayoutConstraint for the anchor.</returns>
        public static NSLayoutConstraint AnchorHeight(this UIView view, UIView secondView, float constant = 0)
        {
            return UIViewExtensions.Activate(view.HeightAnchor.ConstraintEqualTo(secondView.HeightAnchor, constant));
        }

        /// <summary>
        /// Anchors the width of the view.
        /// </summary>
        /// <param name="view">A view to anchor.</param>
        /// <param name="constant">The width.</param>
        /// <returns>The NSLayoutConstraint for the anchor.</returns>
        public static NSLayoutConstraint AnchorWidth(this UIView view, float constant = 0)
        {
            return UIViewExtensions.Activate(view.WidthAnchor.ConstraintEqualTo(constant));
        }

        /// <summary>
        /// Anchors the width of the view to have the same width as another view.
        /// </summary>
        /// <param name="view">A view to anchor.</param>
        /// <param name="secondView">A second view.</param>
        /// <param name="constant">A constant offset.</param>
        /// <returns>The NSLayoutConstraint for the anchor.</returns>
        public static NSLayoutConstraint AnchorWidth(this UIView view, UIView secondView, float constant = 0)
        {
            return UIViewExtensions.Activate(view.WidthAnchor.ConstraintEqualTo(secondView.WidthAnchor, 1, constant));
        }

        /// <summary>
        /// Anchors the view to the left of another view.
        /// </summary>
        /// <param name="view">A view.</param>
        /// <param name="rightView">A view to align on the right.</param>
        /// <param name="constant">An offset between the two views.</param>
        /// <returns>A new NSLayoutConstraint.</returns>
        public static NSLayoutConstraint AnchorLeftOf(this UIView view, UIView rightView, float constant = 0)
        {
            Throw.IfArgumentNull(rightView, nameof(rightView));
            return UIViewExtensions.Activate(view.RightAnchor.ConstraintEqualTo(rightView.LeftAnchor, -constant));
        }

        /// <summary>
        /// Aligns the view to the right of another view.
        /// </summary>
        /// <param name="view">A view.</param>
        /// <param name="leftView">A view to align on the left.</param>
        /// <param name="constant">An offset between the two view.</param>
        /// <returns>A new NSLayoutConstraint.</returns>
        public static NSLayoutConstraint AnchorRightOf(this UIView view, UIView leftView, float constant = 0)
        {
            Throw.IfArgumentNull(leftView, nameof(leftView));
            return UIViewExtensions.Activate(view.LeftAnchor.ConstraintEqualTo(leftView.RightAnchor, constant));
        }

        private static NSLayoutConstraint Activate(NSLayoutConstraint constraint)
        {
            constraint.Active = true;
            return constraint;
        }

        private static void EnsureHasParent(UIView view)
        {
            if (view.Superview == null)
            {
                throw new InvalidOperationException("A view must be added as a subview to its parent before it can be constrained.");
            }
        }
    }
}