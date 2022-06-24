using System.Drawing;
using System.Linq;

namespace Yomitan.Core.Models
{
    public class TextRegion
    {
        public string Text { get; set; }
        public Rectangle Bounds { get; set; }

        public TextRegion(Rectangle bounds, string text)
        {
            Bounds = bounds;
            Text = text;
        }

        public TextRegion(Rectangle bounds) : this(bounds, string.Empty) { }

        public TextRegion() : this(Rectangle.Empty) { }

        public bool Contains(Point point)
        {
            return Bounds.Contains(point);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TextRegion))
                return false;

            TextRegion other = obj as TextRegion;
            return Bounds.Equals(other.Bounds);
        }

        public override int GetHashCode()
        {
            return Bounds.GetHashCode();
        }
    }
}
