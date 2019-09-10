using Studio.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Studio.Controls
{
    public class DocumentTabPanel : TabPanel
    {
        public static readonly DependencyProperty IsPinnedProperty =
            DependencyProperty.RegisterAttached("IsPinned", typeof(bool), typeof(DocumentTabPanel), new PropertyMetadata(false));

        public static bool GetIsPinned(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPinnedProperty);
        }

        public static void SetIsPinned(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPinnedProperty, value);
            (VisualTreeHelper.GetParent(obj) as DocumentTabPanel)?.InvalidateVisual();
        }

        public static readonly DependencyPropertyKey HasOverflowItemsPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("HasOverflowItems", typeof(bool), typeof(DocumentTabPanel), new PropertyMetadata(false));

        public static readonly DependencyProperty HasOverflowItemsProperty = HasOverflowItemsPropertyKey.DependencyProperty;

        public static bool GetHasOverflowItems(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasOverflowItemsProperty);
        }

        private static void SetHasOverflowItems(DependencyObject obj, bool value)
        {
            obj.SetValue(HasOverflowItemsPropertyKey, value);
        }

        private DocumentWell GetAncestorControl()
        {
            return this.FindVisualAncestor<DocumentWell>();
        }

        private int GetRowCount(Size constraint)
        {
            var host = GetAncestorControl();

            int rowCount = 1;
            double currentOffset = 0;

            var pinned = InternalChildren.OfType<UIElement>().Where(t => GetIsPinned(t));
            foreach (var item in pinned)
            {
                if (currentOffset > 0 && currentOffset + item.DesiredSize.Width > constraint.Width)
                {
                    currentOffset = 0;
                    rowCount++;
                }
                currentOffset += item.DesiredSize.Width;
            }

            if (pinned.Any() && host?.PinOnSeparateRow == true)
                rowCount++;

            return rowCount;
        }

        private IEnumerable<UIElement> GetItemsOnRow(Size constraint, int rowIndex)
        {
            var host = GetAncestorControl();

            int rowNum = 0;
            double currentOffset = 0;
            var pinned = InternalChildren.OfType<UIElement>().Where(t => GetIsPinned(t));
            var notPinned = InternalChildren.OfType<UIElement>().Except(pinned);

            foreach (var item in pinned)
            {
                if (currentOffset > 0 && currentOffset + item.DesiredSize.Width > constraint.Width)
                {
                    currentOffset = 0;
                    rowNum++;
                }
                currentOffset += item.DesiredSize.Width;
                if (rowNum == rowIndex) yield return item;
            }

            if (pinned.Any() && host?.PinOnSeparateRow == true)
            {
                rowNum++;
                currentOffset = 0;
            }

            foreach (var item in notPinned)
            {
                if (currentOffset > 0 && currentOffset + item.DesiredSize.Width > constraint.Width)
                {
                    if (host != null) SetHasOverflowItems(host, true);

                    if (rowIndex == -1)
                    {
                        yield return item;
                        continue;
                    }
                    else yield break;
                }

                currentOffset += item.DesiredSize.Width;

                if (rowNum == rowIndex)
                    yield return item;
            }

            if (host != null) SetHasOverflowItems(host, false);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            foreach (var child in InternalChildren.OfType<UIElement>())
                child.Measure(constraint);

            var result = new Size();
            var rowCount = GetRowCount(constraint);
            for (int i = 0; i < rowCount; i++)
            {
                var items = GetItemsOnRow(constraint, i).ToList();
                result.Width = Math.Max(result.Width, items.Sum(e => e.DesiredSize.Width));
                result.Height += items.Max(e => e.DesiredSize.Height);
            }

            result.Height += rowCount - 1; //1 pixel gap between each row

            return result;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var offset = new Point();

            var rowCount = GetRowCount(arrangeSize);
            for (int i = -1; i < rowCount; i++)
            {
                var items = GetItemsOnRow(arrangeSize, i).ToList();
                foreach (var item in items)
                {
                    if (i >= 0)
                    {
                        item.Arrange(new Rect(offset, item.DesiredSize));
                        offset.X += item.DesiredSize.Width;
                    }
                    else item.Arrange(new Rect());
                }

                if (i >= 0 && items.Count > 0)
                {
                    offset.X = 0;
                    offset.Y += items.Max(e => e.DesiredSize.Height) + 1;
                }
            }

            return arrangeSize;
        }
    }
}
