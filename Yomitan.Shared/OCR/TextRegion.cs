using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Yomitan.Shared.OCR
{
    public class TextRegion
    {
        public string Text { get; set; }
        public Rectangle Bounds { get; set; }
        public Rectangle Screen { get; set; }

        public TextRegion(Rectangle bounds, Rectangle screen, string text)
        {
            Bounds = bounds;
            Text = text;
            Screen = screen;
        }

        public TextRegion(Rectangle bounds, string text) : this(bounds, Rectangle.Empty, text) { }

        public TextRegion(Rectangle bounds) : this(bounds, string.Empty) { }

        public TextRegion() : this(Rectangle.Empty) { }

        public bool Contains(Point point)
        {
            return Bounds.Contains(point);
        }

        public TextRegion Append(TextRegion region)
        {
            Bounds = Rectangle.Union(Bounds, region.Bounds);

            StringBuilder builder = new StringBuilder();
            builder.Append(Text);
            builder.Append(region.Text);

            Text = builder.ToString();

            return this;
        }

        public TextRegion Pad(int px)
        {
            Rectangle padded = new Rectangle(
                Bounds.Left - px,
                Bounds.Top - px,
                Bounds.Width + 2 * px,
                Bounds.Height + 2 * px);

            Bounds = padded;
            return this;
        }

        public TextRegion Scale(double scale)
        {
            Rectangle scaled = new Rectangle(
                (int)Math.Round(Bounds.Left * scale),
                (int)Math.Round(Bounds.Top * scale),
                (int)Math.Round(Bounds.Width * scale),
                (int)Math.Round(Bounds.Height * scale)
            );

            Bounds = scaled;
            return this;
        }

        public TextRegion Clone()
        {
            return new TextRegion
            {
                Bounds = Bounds,
                Text = Text,
            };
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
