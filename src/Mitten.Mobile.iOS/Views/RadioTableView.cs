using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using Mitten.Mobile.Themes;
using UIKit;

namespace Mitten.Mobile.iOS.Views
{
    /// <summary>
    /// A table view that acts like a set of radio buttons in which only a single row can be checked at a time.
    /// </summary>
    public class RadioTableView : UITableView
    {
        private static class Constants
        {
            public const int CheckMarkSize = 12;
            public const int RowHeight = 38;
        }

        private readonly IEnumerable<string> items;
        private readonly TableViewSource source;

        private string selectedItem;

        /// <summary>
        /// Initializes a new instance of the RadioTableView class.
        /// </summary>
        /// <param name="items">The items for the table.</param>
        public RadioTableView(IEnumerable<string> items)
            : this(items, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RadioTableView class.
        /// </summary>
        /// <param name="items">The items for the table.</param>
        /// <param name="checkMarkImage">An image to use for the check mark.</param>
        public RadioTableView(IEnumerable<string> items, UIImage checkMarkImage)
        {
            this.AlwaysBounceVertical = false;
            this.RowHeight = Constants.RowHeight;

            this.items = items;

            this.source = new TableViewSource(items, () => this.SelectedItem, this.SelectItemAtIndex);
            this.source.CheckMarkImage = checkMarkImage;

            this.Source = this.source;
        }

        /// <summary>
        /// Occurs when the selected item has changed.
        /// </summary>
        public event Action SelectedItemChanged = delegate { };

        /// <summary>
        /// Gets or sets the currently selected item.
        /// </summary>
        public string SelectedItem 
        { 
            get { return this.selectedItem; }
            set { this.SelectItem(value); }
        }

        /// <summary>
        /// Gets the intrinsic size of the current table's content which is used to communicate to the layout system the size it would like to be based on.
        /// </summary>
        public override CGSize IntrinsicContentSize
        {
            get
            {
                this.LayoutIfNeeded();
                return new CGSize(UIView.NoIntrinsicMetric, this.ContentSize.Height);
            }
        }

        /// <summary>
        /// Applies a theme to the current radio table view.
        /// </summary>
        /// <param name="textTheme">A label theme for the item text.</param>
        /// <param name="checkMarkColor">The color for the check mark.</param>
        public void ApplyTheme(LabelTheme textTheme, UIColor checkMarkColor)
        {
            this.source.TextTheme = textTheme;
            this.source.CheckMarkColor = checkMarkColor;

            for (int rowIndex = 0; rowIndex < this.NumberOfRowsInSection(0); rowIndex++)
            {
                UITableViewCell cell = this.CellAt(NSIndexPath.FromRowSection(rowIndex, 0));

                if (cell != null)
                {
                    this.source.ApplyTheme(cell);
                }
            }
        }

        private void SelectItemAtIndex(int index)
        {
            this.SetSelectedItem(this.items.ElementAt(index));
        }

        private void SelectItem(string item)
        {
            Throw.IfArgumentNullOrWhitespace(item, nameof(item));

            if (this.selectedItem != item)
            {
                int index = this.IndexOf(item);
                if (index < 0)
                {
                    throw new ArgumentException("Item (" + item + ") does not exist in the list of items.", nameof(item));
                }

                this.SetSelectedItem(item);
                this.SelectRow(NSIndexPath.FromRowSection(index, 0), false, UITableViewScrollPosition.None);
            }
        }

        private void SetSelectedItem(string item)
        {
            if (this.selectedItem != item)
            {
                this.selectedItem = item;
                this.SelectedItemChanged();
            }
        }

        private int IndexOf(string item)
        {
            int index = 0;
            foreach (string i in this.items)
            {
                if (item == i)
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        private class TableViewSource : UITableViewSource
        {
            private readonly IEnumerable<string> items;
            private readonly Func<string> getSelectedItem;
            private readonly Action<int> selectAtIndex;

            public TableViewSource(IEnumerable<string> items, Func<string> getSelectedItem, Action<int> selectAtIndex)
            {
                this.items = items;
                this.getSelectedItem = getSelectedItem;
                this.selectAtIndex = selectAtIndex;
            }

            public LabelTheme TextTheme { get; set; }
            public UIImage CheckMarkImage { get; set; }
            public UIColor CheckMarkColor { get; set; }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                tableView.CellAt(indexPath).Accessory = UITableViewCellAccessory.Checkmark;
                this.selectAtIndex(indexPath.Row);
            }

            public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
            {
                tableView.CellAt(indexPath).Accessory = UITableViewCellAccessory.None;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                CheckMarkUITableViewCell cell = new CheckMarkUITableViewCell();
                string item = this.items.ElementAt(indexPath.Row);

                cell.CheckMarkImage = this.CheckMarkImage;
                cell.TextLabel.Text = item;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;

                this.ApplyTheme(cell);

                if (item == this.getSelectedItem())
                {
                    cell.Accessory = UITableViewCellAccessory.Checkmark;
                }

                return cell;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return this.items.Count();
            }

            public void ApplyTheme(UITableViewCell cell)
            {
                if (this.TextTheme != null)
                {
                    cell.TextLabel.ApplyTheme(this.TextTheme);
                }

                UIColor checkMarkColor = this.CheckMarkColor ?? UIColor.Blue;
                ((CheckMarkUITableViewCell)cell).CheckMarkColor = checkMarkColor;
                ((CheckMarkUITableViewCell)cell).CheckMarkImage = this.CheckMarkImage;

                UIImageView checkMark = cell.AccessoryView as UIImageView;
                if (checkMark != null)
                {
                    checkMark.TintColor = checkMarkColor;
                }
            }
        }

        private class CheckMarkUITableViewCell : UITableViewCell
        {
            public UIImage CheckMarkImage { get; set; }
            public UIColor CheckMarkColor { get; set; }

            /// <summary>
            /// Gets or sets the accessory for the cell.
            /// </summary>
            public override UITableViewCellAccessory Accessory
            {
                get { return base.Accessory; }
                set
                {
                    base.Accessory = value;

                    if (this.CheckMarkImage != null)
                    {
                        this.AccessoryView =
                            value == UITableViewCellAccessory.Checkmark
                            ? this.CreateCheckmarkView()
                            : null;
                    }
                }
            }

            private UIView CreateCheckmarkView()
            {
                UIImageView checkMark = new UIImageView(this.CheckMarkImage);

                checkMark.Frame = new CGRect(0, 0, Constants.CheckMarkSize, Constants.CheckMarkSize);
                checkMark.TintColor = this.CheckMarkColor;

                return checkMark;
            }
        }
    }
}