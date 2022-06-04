namespace Yomitan.Kanjitomo.OCR
{
    internal class AssignableRectangle
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;

        public AssignableRectangle(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
