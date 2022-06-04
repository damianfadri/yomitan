namespace Yomitan.Kanjitomo.OCR
{
    internal class Pixel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool Value { get; set; }
        public Pixel Top { get; internal set; }
        public Pixel Left { get; internal set; }
        public Pixel Bottom { get; internal set; }
        public Pixel Right { get; internal set; }

        public Pixel(int x, int y, bool value)
        {
            X = x;
            Y = y;
            Value = value;
        }
    }
}
