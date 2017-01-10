using System;
using CoreGraphics;
using Mitten.Mobile.iOS.Views.Renderers;
using UIKit;

namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// A basic check box view.
    /// </summary>
    public class CheckBoxView : UIControl
    {
        private readonly CheckBoxRenderer renderer;

        private CGColor color;
        private bool isChecked;

        /// <summary>
        /// Initializes a new instance of the CheckBoxView class.
        /// </summary>
        public CheckBoxView()
        {
            this.renderer = new CheckBoxRenderer(this);
            this.color = UIColor.Green.CGColor;

            this.TouchDown += (sender, e) => this.HandleTouchDown();
            this.TouchDragExit += (sender, e) => this.HandleTouchDragExit();
            this.TouchDragEnter += (sender, e) => this.HandleTouchDragEnter();
            this.TouchUpInside += (sender, e) => this.HandleTouchUpInside();
        }

        /// <summary>
        /// Occurs when the IsCheckedChanged property has changed.
        /// </summary>
        public event Action IsCheckedChanged = delegate { };

        /// <summary>
        /// Gets or sets whether or not the current check box is checked.
        /// </summary>
        public bool IsChecked
        {
            get { return this.isChecked; }
            set 
            {
                if (this.isChecked != value)
                {
                    this.isChecked = value;
                    this.renderer.RenderToView();
                    this.IsCheckedChanged();
                }
            }
        }

        /// <summary>
        /// Gets whether or not the current check box is pressed down by the user.
        /// </summary>
        public bool IsPressed { get; private set; }

        /// <summary>
        /// Gets or sets the color for the check box.
        /// </summary>
        public CGColor Color
        {
            get { return this.color; }
            set 
            {
                if (this.color != value)
                {
                    this.color = value;
                    this.renderer.RenderToView();
                }
            }
        }

        /// <summary>
        /// Lays out the subviews.
        /// </summary>
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            this.renderer.RenderToView();
        }

        private void SetPressed(bool isPressed)
        {
            if (this.IsPressed != isPressed)
            {
                this.IsPressed = isPressed;
                this.renderer.RenderToView();
            }
        }

        private void HandleTouchDown()
        {
            this.SetPressed(true);
        }

        private void HandleTouchDragEnter()
        {
            this.SetPressed(true);
        }

        private void HandleTouchDragExit()
        {
            this.SetPressed(false);
        }

        private void HandleTouchUpInside()
        {
            this.SetPressed(false);
            this.IsChecked = !this.IsChecked;
        }
    }
}