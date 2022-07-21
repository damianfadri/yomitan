using System.Drawing;

namespace Yomitan.Core.Helpers
{
    public static class RectangleHelpers
    {
        public static Rectangle Clone(this Rectangle rectangle)
        {
            return new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static Rectangle Append(this Rectangle rectangle, Rectangle other)
        {
            return Rectangle.Union(rectangle, other);
        }

        public static Rectangle Create(int left, int top, int right, int bottom)
        {
            return new Rectangle(left, top, right - left, bottom - top);
        }
    }
}
