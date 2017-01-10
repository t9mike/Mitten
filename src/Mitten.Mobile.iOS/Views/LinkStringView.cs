using System;
using System.Collections.Generic;
using System.Text;
using Foundation;
using Mitten.Mobile.ViewModels;
using UIKit;

namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// The view used to display text with embedded links.
    /// </summary>
    public class LinkStringView : UIView
    {
        /// <summary>
        /// Occurs when a link was clicked.
        /// </summary>
        public event Action<LinkString.Part> LinkClicked = part => { };

        /// <summary>
        /// Gets or sets the link string containing the label and link text for the view.
        /// </summary>
        public LinkString LinkString { get; set; }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        public UIColor TextColor { get; set; }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        public UIFont Font { get; set; }

        /// <summary>
        /// Updates the view.
        /// </summary>
        public void UpdateView()
        {
            List<object> objectsAndNames = new List<object>();

            StringBuilder sb = new StringBuilder();
            sb.Append("H:|");

            int count = 1;
            foreach (LinkString.Part part in this.LinkString.Parts)
            {
                if (!string.IsNullOrEmpty(part.Text))
                {
                    UILabel label = this.CreateLabel(part);
                    string name = "label" + count++;

                    objectsAndNames.Add(name);
                    objectsAndNames.Add(label);

                    this.AddSubview(label);

                    sb.Append("[" + name + "]");

                    this.AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|[" + name + "]|", 0, new object[] { name, label }));
                }
            }

            sb.Append("|");

            this.AddConstraints(NSLayoutConstraint.FromVisualFormat(sb.ToString(), 0, objectsAndNames.ToArray()));
        }

        private UILabel CreateLabel(LinkString.Part part)
        {
            UILabel label = new UILabel();

            label.Font = this.Font;
            label.HighlightedTextColor = this.TintColor;
            label.Text = part.Text;
            label.TextColor = this.TextColor;
            label.TranslatesAutoresizingMaskIntoConstraints = false;

            if (part.HasHandler)
            {
                UILongPressGestureRecognizer pressGesture = new UILongPressGestureRecognizer(gesture => this.OnPartPressed(gesture, part, label));
                pressGesture.MinimumPressDuration = 0.001;

                label.AddGestureRecognizer(pressGesture);
                label.UserInteractionEnabled = true;

                this.SetUnderLineStyle(label, NSUnderlineStyle.Single);
            }

            label.SizeToFit();

            return label;
        }

        private void SetUnderLineStyle(UILabel label, NSUnderlineStyle underLineStyle)
        {
            UIStringAttributes attributes = new UIStringAttributes();
            attributes.UnderlineStyle = underLineStyle;
            label.AttributedText = new NSAttributedString(label.Text, attributes);
        }

        private void OnPartPressed(UIGestureRecognizer gesture, LinkString.Part part, UILabel label)
        {
            if (gesture.State == UIGestureRecognizerState.Began)
            {
                label.Highlighted = true;
                this.SetUnderLineStyle(label, NSUnderlineStyle.None);
            }
            else if (gesture.State == UIGestureRecognizerState.Ended)
            {
                label.Highlighted = false;
                this.SetUnderLineStyle(label, NSUnderlineStyle.Single);

                this.LinkClicked(part);
            }
        }
    }
}