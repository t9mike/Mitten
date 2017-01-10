using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreAnimation;
using CoreGraphics;
using Finch.Mobile.iOS.Extensions;
using Mitten.Mobile.iOS.Extensions;
using Mitten.Mobile.Themes;
using UIKit;

namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// A custom segmented control that supports multiple rows.
    /// </summary>
    public class SegmentedControl : UIControl
    {
        private static class Constants
        {
            public const int DefaultButtonHeight = 36;

            public const int ButtonImagePadding = 0;
            public const int ButtonBorderSize = 2;
            public const int ButtonBorderHalfSize = ButtonBorderSize / 2;
            public const int ButtonBorderRadius = 6;
            public const string ButtonBorderName = "Border";

            public const int MaximumButtonsPerRow = 4;
        }

        private readonly List<NSLayoutConstraint> buttonHeightConstraints;
        private readonly List<UIView> buttonRows;
        private IEnumerable<Tuple<UIButton, SegmentedControlItem>> buttons;
        private ILayoutHandler layoutHandler;
        private ButtonTheme buttonTheme;
        private CGColor buttonBorderColor;
        private int buttonHeight;

        /// <summary>
        /// Initializes a new instance of the SegmentedControl class.
        /// </summary>
        /// <param name="maximumButtonsPerRow">The maximum number of buttons per row.</param>
        public SegmentedControl(int maximumButtonsPerRow = Constants.MaximumButtonsPerRow)
        {
            if (maximumButtonsPerRow < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumButtonsPerRow), "The maximum number of buttons per row must be greater than 0.");
            }

            this.buttonHeightConstraints = new List<NSLayoutConstraint>();
            this.buttonRows = new List<UIView>();
            this.buttonHeight = Constants.DefaultButtonHeight;
            this.layoutHandler = new DefaultLayoutHandler(maximumButtonsPerRow);
        }

        /// <summary>
        /// Occurs after an item has been successfully pressed.
        /// </summary>
        public event Action<SegmentedControlItem> AfterItemPressed = delegate { };

        /// <summary>
        /// Gets or sets the theme used for the buttons in the current control.
        /// </summary>
        public ButtonTheme ButtonTheme 
        { 
            get { return this.buttonTheme; }
            set
            {
                // TODO: consider decoupling the Theme from this control
                Throw.IfArgumentNull(value, nameof(value));

                this.buttonBorderColor = value.BorderColor.ToCGColor();
                this.buttonTheme = value.WithTransparentBorder();

                this.ApplyButtonTheme();
            }
        }

        /// <summary>
        /// Gets or sets the height of the buttons.
        /// </summary>
        public int ButtonHeight
        {
            get { return this.buttonHeight; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be greater than zero.");
                }

                if (this.buttonHeight != value)
                {
                    this.buttonHeight = value;

                    foreach (NSLayoutConstraint constraint in this.buttonHeightConstraints)
                    {
                        constraint.Constant = this.buttonHeight;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a layout handler for the segmented control.
        /// </summary>
        public ILayoutHandler LayoutHandler
        {
            get { return this.layoutHandler; }
            set
            {
                Throw.IfArgumentNull(value, nameof(value));
                this.layoutHandler = value;
                this.LayoutButtons();
            }
        }

        /// <summary>
        /// Gets or sets the list of items for the segmented control.
        /// </summary>
        public IEnumerable<SegmentedControlItem> Items
        {
            get
            {
                if (this.buttons != null)
                {
                    return this.buttons.Select(item => item.Item2);
                }

                return Enumerable.Empty<SegmentedControlItem>();
            }
            set
            {
                Throw.IfArgumentNull(value, nameof(value));

                if (!value.Any())
                {
                    throw new ArgumentException("At least one item must be specified.", nameof(value));
                }

                this.buttons = this.CreateItemButtons(value);
                this.LayoutButtons();

                if (this.buttonTheme != null)
                {
                    this.ApplyButtonTheme();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether or not an item should be selected when pressed.
        /// </summary>
        public bool SelectItemWhenPressed { get; set; }

        /// <summary>
        /// Lays out the subviews.
        /// </summary>
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (this.buttonBorderColor != null)
            {
                this.UpdateButtonBorders();
            }

            this.CenterButtonImages();
        }

        /// <summary>
        /// Selects the item associated with the specified tag.
        /// </summary>
        /// <param name="tag">A tag.</param>
        public void SelectItemWithTag(object tag)
        {
            Throw.IfArgumentNull(tag, nameof(tag));

            IEnumerable<SegmentedControlItem> items = this.Items.Where(item => object.Equals(item.Tag, tag));

            if (!items.Any())
            {
                throw new ArgumentException("Item not found for the provided tag.");
            }

            if (items.Count() > 1)
            {
                throw new ArgumentException("More than one item was found with the provided tag.");
            }

            this.SelectItem(items.Single());
        }

        private void ApplyButtonTheme()
        {
            if (this.buttons != null)
            {
                foreach (Tuple<UIButton, SegmentedControlItem> item in this.buttons)
                {
                    item.Item1.ApplyTheme(
                        this.buttonTheme,
                        item.Item2.Image,
                        default(UIEdgeInsets),
                        true,
                        ViewFontSizes.SmallFontSize);

                    item.Item1.SetTitleColor(this.buttonTheme.BackgroundColor.ToUIColor(), UIControlState.Selected);
                }

                this.UpdateButtonBorders();
            }
        }

        private void LayoutButtons()
        {
            if (this.buttons != null)
            {
                int numberOfRows = this.layoutHandler.CalculateNumberOfRows(this.buttons.Count());

                if (numberOfRows < 1)
                {
                    throw new InvalidOperationException("The layout handler calculated the number of rows as less than 1.");
                }

                foreach (UIView row in this.buttonRows)
                {
                    row.RemoveFromSuperview();
                }

                this.buttonRows.Clear();
                this.buttonHeightConstraints.Clear();

                for (int i = 0; i < numberOfRows; i++)
                {
                    IEnumerable<SegmentedControlItem> itemsForRow = this.layoutHandler.SelectItemsForRow(this.buttons.Select(item => item.Item2), i);
                    IEnumerable<UIButton> buttonsForRow = this.buttons.Where(item => itemsForRow.Contains(item.Item2)).Select(item => item.Item1);

                    this.AddRow(buttonsForRow);
                }

                UIView topRow = this.buttonRows.First();
                topRow.Anchor(AnchorEdges.Horizontal | AnchorEdges.Top);

                for (int i = 1; i < this.buttonRows.Count; i++)
                {
                    UIView currentRow = this.buttonRows[i];
                    currentRow.Anchor(AnchorEdges.Horizontal);
                    currentRow.AnchorBelow(topRow);

                    topRow = currentRow;
                }

                this.buttonRows.Last().Anchor(AnchorEdges.Bottom);
            }
        }

        private void AddRow(IEnumerable<UIButton> items)
        {
            UIView row = new UIView();
            row.TranslatesAutoresizingMaskIntoConstraints = false;
            this.AddSubview(row);

            row.AddSubviews(items.ToArray());

            List<object> namesAndViews = new List<object>();

            StringBuilder sb = new StringBuilder();
            sb.Append("H:|");

            int total = items.Count();
            int count = 0;

            foreach (UIView item in items)
            {
                string name = "item" + count++;

                if (count < total)
                {
                    string nextView = "item" + count;
                    sb.Append("[" + name + "(==" + nextView + ")]");
                }
                else
                {
                    sb.Append("[" + name + "]");
                }

                namesAndViews.Add(name);
                namesAndViews.Add(item);

                item.Anchor(AnchorEdges.Top | AnchorEdges.Bottom);
                this.buttonHeightConstraints.Add(item.AnchorHeight(this.ButtonHeight));
            }

            sb.Append("|");

            row.AddConstraints(NSLayoutConstraint.FromVisualFormat(sb.ToString(), 0, namesAndViews.ToArray()));
            this.buttonRows.Add(row);
        }

        private IEnumerable<Tuple<UIButton, SegmentedControlItem>> CreateItemButtons(IEnumerable<SegmentedControlItem> items)
        {
            List<Tuple<UIButton, SegmentedControlItem>> buttonViews = new List<Tuple<UIButton, SegmentedControlItem>>();

            foreach (SegmentedControlItem item in items)
            {
                UIButton button = new UIButton(UIButtonType.Custom);
                button.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                button.TranslatesAutoresizingMaskIntoConstraints = false;

                button.TouchUpInside += (sender, e) => this.HandleItemButtonPressed(item);

                button.SetTitle(item.Title, UIControlState.Normal);
                button.SetTitle(item.Title, UIControlState.Selected);

                buttonViews.Add(new Tuple<UIButton, SegmentedControlItem>(button, item));
            }

            return buttonViews;
        }

        private void HandleItemButtonPressed(SegmentedControlItem item)
        {
            if (item.ItemPressed())
            {
                if (this.SelectItemWhenPressed)
                {
                    this.SelectItem(item);
                }

                this.AfterItemPressed(item);
            }
        }

        private void SelectItem(SegmentedControlItem item)
        {
            foreach (Tuple<UIButton, SegmentedControlItem> tuple in this.buttons)
            {
                if (tuple.Item2 == item)
                {
                    tuple.Item1.Selected = true;
                    tuple.Item1.BackgroundColor = this.ButtonTheme.FontColor.ToUIColor();
                }
                else
                {
                    tuple.Item1.Selected = false;
                    tuple.Item1.BackgroundColor = this.ButtonTheme.BackgroundColor.ToUIColor();
                }
            }
        }

        private void UpdateButtonBorders()
        {
            for (int i = 0; i < this.buttonRows.Count; i++)
            {
                bool isFirstRow = i == 0;
                bool isLastRow = i == this.buttonRows.Count - 1;

                IEnumerable<UIButton> buttonsForRow = this.buttonRows[i].Subviews.Cast<UIButton>();

                int buttonsInRow = buttonsForRow.Count();
                for (int j = 0; j < buttonsInRow; j++)
                {
                    UIButton button = buttonsForRow.ElementAt(j);
                    button.LayoutIfNeeded();

                    UIRectCorner corners = 0;
                    bool isFirstColumn = j == 0;
                    bool isLastColumn = j == buttonsInRow - 1;

                    if (isFirstRow)
                    {
                        if (isFirstColumn)
                        {
                            corners |= UIRectCorner.TopLeft;
                        }

                        if (isLastColumn)
                        {
                            corners |= UIRectCorner.TopRight;
                        }
                    }

                    if (isLastRow)
                    {
                        if (isFirstColumn)
                        {
                            corners |= UIRectCorner.BottomLeft;
                        }

                        if (isLastColumn)
                        {
                            corners |= UIRectCorner.BottomRight;
                        }
                    }

                    this.UpdateButtonBorder(
                        button, 
                        corners, 
                        isFirstRow, 
                        isLastRow,
                        isFirstColumn,
                        isLastColumn);
                }
            }
        }

        private void UpdateButtonBorder(
            UIButton button, 
            UIRectCorner corners,
            bool isInFirstRow,
            bool isInLastRow,
            bool isInFirstColumn,
            bool isInLastColumn)
        {
            // we need to handle the situation where borders are next to each other, the problem is
            // this creates an appearance where the border is twice as thick so we will use masking
            // to remove borders from some buttons

            CGSize radius = new CGSize(Constants.ButtonBorderRadius, Constants.ButtonBorderRadius);
            CGRect borderBounds = button.Bounds;

            if (!isInLastColumn)
            {
                // extend the border to the right so it overlaps with the border of the next button
                borderBounds =
                    new CGRect(
                        button.Bounds.X,
                        button.Bounds.Y,
                        button.Bounds.Width + Constants.ButtonBorderHalfSize,
                        button.Bounds.Height);
            }

            if (!isInLastRow)
            {
                // extend the border down so it overlaps with the border for the next row
                borderBounds = borderBounds.WithHeight(borderBounds.Height + Constants.ButtonBorderHalfSize);
            }

            UIBezierPath path = UIBezierPath.FromRoundedRect(borderBounds, corners, radius);

            CAShapeLayer maskLayer = new CAShapeLayer();
            maskLayer.Frame = borderBounds;
            maskLayer.Path = path.CGPath;

            button.Layer.Mask = maskLayer;

            CAShapeLayer borderLayer = new CAShapeLayer();
            borderLayer.FillColor = null;
            borderLayer.Frame = borderBounds;
            borderLayer.Name = Constants.ButtonBorderName;
            borderLayer.Path = path.CGPath;
            borderLayer.StrokeColor = this.buttonBorderColor;
            borderLayer.LineWidth = Constants.ButtonBorderSize;

            if (button.Layer.Sublayers != null)
            {
                CAShapeLayer oldLayer = button.Layer.Sublayers.SingleOrDefault(layer => layer.Name == Constants.ButtonBorderName) as CAShapeLayer;
                if (oldLayer != null)
                {
                    oldLayer.RemoveFromSuperLayer();
                }
            }

            button.Layer.AddSublayer(borderLayer);
        }

        private void CenterButtonImages()
        {
            foreach (Tuple<UIButton, SegmentedControlItem> item in this.buttons)
            {
                if (item.Item2.Image != null)
                {
                    CGSize imageSize = item.Item1.ImageView.Frame.Size;
                    CGSize titleSize = item.Item1.TitleLabel.Frame.Size;

                    nfloat totalHeight = imageSize.Height + titleSize.Height + Constants.ButtonImagePadding;

                    item.Item1.ImageEdgeInsets =
                        new UIEdgeInsets(
                            -(totalHeight - imageSize.Height),
                            0.0f,
                            0.0f,
                            -titleSize.Width);

                    item.Item1.TitleEdgeInsets =
                        new UIEdgeInsets(
                            0.0f,
                            -imageSize.Width,
                            -(totalHeight - titleSize.Height),
                            0.0f);
                }
            }
        }

        /// <summary>
        /// Defines a handler responsible for defining the order and layout of buttons in a segmented control.
        /// </summary>
        public interface ILayoutHandler
        {
            /// <summary>
            /// Calculates the number of rows needed to display the specified number of buttons.
            /// </summary>
            /// <param name="numberOfButtons">The number of buttons to display in the segmented control.</param>
            /// <returns>The number of rows.</returns>
            int CalculateNumberOfRows(int numberOfButtons);

            /// <summary>
            /// Selects the items for the row at the specified index.
            /// </summary>
            /// <param name="items">A list of all the items for the current segmented control.</param>
            /// <param name="rowIndex">The row index.</param>
            /// <returns>The items for row.</returns>
            IEnumerable<SegmentedControlItem> SelectItemsForRow(IEnumerable<SegmentedControlItem> items, int rowIndex);
        }

        /// <summary>
        /// The default layout handler for a segmented control.
        /// </summary>
        public class DefaultLayoutHandler : ILayoutHandler
        {
            /// <summary>
            /// Initializes a new instance of the DefaultLayoutHandler class.
            /// </summary>
            /// <param name="maxNumberOfButtonsPerRow">The maximum number of buttons per row.</param>
            public DefaultLayoutHandler(int maxNumberOfButtonsPerRow)
            {
                if (maxNumberOfButtonsPerRow < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(maxNumberOfButtonsPerRow), "The maximum number of buttons per row must be greater than zero.");
                }

                this.MaxNumberOfButtonsPerRow = maxNumberOfButtonsPerRow;
            }

            /// <summary>
            /// Gets the maximum number of buttons per row.
            /// </summary>
            public int MaxNumberOfButtonsPerRow { get; }

            /// <summary>
            /// Calculates the number of rows needed to display the specified number of buttons.
            /// </summary>
            /// <param name="numberOfButtons">The number of buttons to display in the segmented control.</param>
            /// <returns>The number of rows.</returns>
            public int CalculateNumberOfRows(int numberOfButtons)
            {
                if (numberOfButtons <= this.MaxNumberOfButtonsPerRow)
                {
                    return 1;
                }

                return (numberOfButtons + this.MaxNumberOfButtonsPerRow - 1) / this.MaxNumberOfButtonsPerRow;
            }

            /// <summary>
            /// Selects the items for the row at the specified index.
            /// </summary>
            /// <param name="items">A list of all the items for the current segmented control.</param>
            /// <param name="rowIndex">The row index.</param>
            /// <returns>The items for row.</returns>
            public IEnumerable<SegmentedControlItem> SelectItemsForRow(IEnumerable<SegmentedControlItem> items, int rowIndex)
            {
                int numberOfButtons = items.Count();
                if (numberOfButtons <= this.MaxNumberOfButtonsPerRow)
                {
                    return items;
                }

                int startIndex = rowIndex * this.MaxNumberOfButtonsPerRow;
                int count = Math.Min(this.MaxNumberOfButtonsPerRow, numberOfButtons - startIndex + 1);

                return items.Skip(startIndex).Take(count);
            }
        }
    }
}