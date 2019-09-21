using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Studio.Controls
{
    public class ToolTabPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            if (InternalChildren.Count <= 1)
                return new Size(availableSize.Width, 0);

            var maxWidth = availableSize.Width / InternalChildren.Count;

            foreach (var child in InternalChildren.OfType<UIElement>())
                child.Measure(new Size(maxWidth, availableSize.Height));

            var actualHeight = InternalChildren.OfType<UIElement>().Max(e => e.DesiredSize.Height);
            var actualWidth = InternalChildren.OfType<UIElement>().Sum(e => e.DesiredSize.Width);

            return new Size(Math.Min(availableSize.Width, actualWidth), Math.Min(availableSize.Height, actualHeight));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (InternalChildren.Count <= 1)
            {
                InternalChildren.OfType<UIElement>().FirstOrDefault()?.Arrange(new Rect());
                return finalSize;
            }

            var totalWidth = InternalChildren.OfType<UIElement>().Sum(e => e.DesiredSize.Width);

            double offset = 0;
            foreach (var child in InternalChildren.OfType<UIElement>())
            {
                var width = totalWidth > finalSize.Width ? finalSize.Width / InternalChildren.Count : child.DesiredSize.Width;
                child.Arrange(new Rect(offset, 0, width, child.DesiredSize.Height));
                offset += width;
            }

            return finalSize;
        }
    }
}
