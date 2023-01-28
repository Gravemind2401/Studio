using Studio.Utilities;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Studio.Controls
{
    public class DocumentTabPanel : TabPanel
    {
        private const string HasOverflowItemsPropertyName = "HasOverflowItems";

        private static readonly DependencyPropertyKey HasOverflowItemsPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(HasOverflowItemsPropertyName, typeof(bool), typeof(DocumentTabPanel), new PropertyMetadata(false));

        public static readonly DependencyProperty HasOverflowItemsProperty = HasOverflowItemsPropertyKey.DependencyProperty;

        public static bool GetHasOverflowItems(DependencyObject obj) => (bool)obj.GetValue(HasOverflowItemsProperty);
        private static void SetHasOverflowItems(DependencyObject obj, bool value) => obj.SetValue(HasOverflowItemsPropertyKey, value);

        private DocumentWell GetAncestorControl() => this.FindVisualAncestor<DocumentWell>();

        private int GetRowCount(Size constraint)
        {
            var host = GetAncestorControl();

            int rowCount = 1;
            double currentOffset = 0;

            var pinned = InternalChildren.OfType<UIElement>().Where(t => DockManager.GetIsPinned(t));
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
            var pinned = InternalChildren.OfType<UIElement>().Where(t => DockManager.GetIsPinned(t));
            var notPinned = InternalChildren.OfType<UIElement>().Except(pinned);

            foreach (var item in pinned)
            {
                if (currentOffset > 0 && currentOffset + item.DesiredSize.Width > constraint.Width)
                {
                    currentOffset = 0;
                    rowNum++;
                }

                currentOffset += item.DesiredSize.Width;
                if (rowNum == rowIndex)
                    yield return item;
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
                    if (host != null)
                        SetHasOverflowItems(host, true);

                    if (rowIndex == -1)
                    {
                        yield return item;
                        continue;
                    }
                    else
                        yield break;
                }

                currentOffset += item.DesiredSize.Width;

                if (rowNum == rowIndex)
                    yield return item;
            }

            if (host != null)
                SetHasOverflowItems(host, false);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (InternalChildren.Count == 0)
                return Size.Empty;

            foreach (var child in InternalChildren.OfType<UIElement>())
                child.Measure(availableSize);

            var result = new Size();
            var rowCount = GetRowCount(availableSize);
            for (int i = 0; i < rowCount; i++)
            {
                var items = GetItemsOnRow(availableSize, i).ToList();
                result.Width = Math.Max(result.Width, items.Sum(e => e.DesiredSize.Width));
                result.Height += items.Max(e => e.DesiredSize.Height);
            }

            result.Height += rowCount - 1; //1 pixel gap between each row

            return result;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (InternalChildren.Count == 0)
                return finalSize;

            var offset = new Point();
            var rowCount = GetRowCount(finalSize);

            for (int i = -1; i < rowCount; i++)
            {
                var items = GetItemsOnRow(finalSize, i).ToList();
                foreach (var item in items)
                {
                    if (i >= 0)
                    {
                        item.Arrange(new Rect(offset, item.DesiredSize));
                        offset.X += item.DesiredSize.Width;
                    }
                    else
                        item.Arrange(new Rect());
                }

                if (i >= 0 && items.Count > 0)
                {
                    offset.X = 0;
                    offset.Y += items.Max(e => e.DesiredSize.Height) + 1;
                }
            }

            return finalSize;
        }
    }
}
