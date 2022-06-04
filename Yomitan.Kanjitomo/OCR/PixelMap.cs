using System;
using System.Collections.Generic;
using System.Drawing;

namespace Yomitan.Kanjitomo.OCR
{
    internal class PixelMap
    {
        private Bitmap _image;
        private Pixel[,] _map;
        public PixelMap(Bitmap image)
        {
            _image = image;
            _map = new Pixel[_image.Width, _image.Height];
            for (int x = 0; x < _image.Width; x++)
            {
                for (int y = 0; y < _image.Height; y++)
                {
                    Pixel pixel = new Pixel(x, y, false);
                    _map[x, y] = pixel;

                    if (x - 1 >= 0)
                    {
                        pixel.Left = _map[x - 1, y];
                        pixel.Left.Right = pixel;
                    }

                    if (y - 1 >= 0)
                    {
                        pixel.Top = _map[x, y - 1];
                        pixel.Top.Bottom = pixel;
                    }
                }
            }
        } 

        public void Initialize(Func<Bitmap, int, int, bool> condition)
        {
            foreach (Pixel pixel in GetPixels())
                _map[pixel.X, pixel.Y].Value = condition(_image, pixel.X, pixel.Y);
        }

        public IEnumerable<Pixel> GetPixels()
        {
            return GetPixels(0, _map.GetLength(0), 0, _map.GetLength(1));
        }

        public IEnumerable<Pixel> GetPixels(int fromX, int toX, int fromY, int toY)
        {
            for (int x = Math.Max(0, fromX); x <= Math.Min(toX, _map.GetLength(0) - 1); x++)
                for (int y = Math.Max(0, fromY); y <= Math.Min(toY, _map.GetLength(1) - 1); y++)
                    yield return _map[x, y];
        }

        public bool GetPixel(int x, int y)
        {
            return _map[x, y].Value;
        }

        public void SetPixel(int x, int y, bool value)
        {
            _map[x, y].Value = value;
        }
    }
}
