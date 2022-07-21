using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Yomitan.Core.Contracts;
using Yomitan.Core.Helpers;
using Yomitan.Core.Models;

namespace Yomitan.OCR
{
    public class KanjitomoTextDetector : ITextDetector
    {
        // TODO: Refactor to Configuration.
        private static readonly int CROP_BOUNDS_OFFSET = 50;
        private static readonly int CROP_BOUNDS_WIDTH = 100;
        private static readonly int CROP_BOUNDS_HEIGHT = 300;
        private static readonly int PROBE_INITIAL_WIDTH = 10;
        private static readonly int PROBE_INITIAL_HEIGHT = 10;
        private static readonly float PROBE_MULTIPLIER = 3.0f;

        public Rectangle GetRegion(ImageSource imageSource, Point originPoint)
        {
            // Crop screenshot into a smaller image.
            Rectangle cropBounds = new Rectangle(
                    Math.Max(0, originPoint.X - CROP_BOUNDS_OFFSET),
                    Math.Max(0, originPoint.Y - CROP_BOUNDS_OFFSET),
                    Math.Min(CROP_BOUNDS_WIDTH, imageSource.Image.Width),
                    Math.Min(CROP_BOUNDS_HEIGHT, imageSource.Image.Height)
            );

            PixelMap bnwMap = null;
            PixelMap visitedMap = null;
            using (Bitmap processedImage = ScreenshotHelper.Bounds(
                    imageSource.Image, cropBounds)
                    .Grayscale()
                    .Sharpen()
                    .Binarize()
            )
            {
                // Boolean map representation of the black and white image.
                // Black pixels = 1, White pixels = 0.
                bnwMap = new PixelMap(processedImage);
                bnwMap.Initialize((bitmap, x, y) =>
                {
                    Color color = bitmap.GetPixel(x, y);
                    return Color.Black.ToArgb().Equals(color.ToArgb());
                });

                // Boolean map representation of all visited pixels.
                // Mark all white pixels as already visited.
                visitedMap = new PixelMap(processedImage);
                visitedMap.Initialize((bitmap, x, y) =>
                {
                    Color color = bitmap.GetPixel(x, y);
                    return Color.White.ToArgb().Equals(color.ToArgb());
                });
            }

            // Set initial probe.
            Rectangle probeBounds = new Rectangle(
                    CROP_BOUNDS_OFFSET,
                    CROP_BOUNDS_OFFSET,
                    PROBE_INITIAL_WIDTH,
                    PROBE_INITIAL_HEIGHT
            );

            // Keep extending text region until bounds are unchanged.
            // Probing is performed downwards the current text region.
            Rectangle prevRegion = Rectangle.Empty;
            Rectangle currRegion = probeBounds;
            while (!currRegion.Equals(prevRegion))
            {
                prevRegion = currRegion;

                foreach (Rectangle probedRegion in ProbeRegion(visitedMap, bnwMap, probeBounds))
                    currRegion = currRegion.Append(probedRegion);

                // Update probe bounds.
                probeBounds = new Rectangle(
                        currRegion.Left,
                        currRegion.Bottom,
                        currRegion.Width,
                        (int)(currRegion.Width * PROBE_MULTIPLIER)
                );
            }

            // Convert back to absolute coordinates.
            currRegion = new Rectangle(
                    originPoint.X - CROP_BOUNDS_OFFSET + currRegion.Left,
                    originPoint.Y - CROP_BOUNDS_OFFSET + currRegion.Top,
                    currRegion.Width,
                    currRegion.Height
            );

            return currRegion;
        }

        private IEnumerable<Rectangle> ProbeRegion(PixelMap visitedMap, PixelMap bnwMap, Rectangle probeBounds)
        {
            Queue<Pixel> visitQueue = new Queue<Pixel>();

            foreach (Pixel probePixel in bnwMap.GetPixels(
                    probeBounds.Left, probeBounds.Right, probeBounds.Top, probeBounds.Bottom))
            {
                // Set initial max bounds.
                Rectangle maxBounds = RectangleHelpers.Create(
                        probePixel.X,
                        probePixel.Y,
                        probePixel.X,
                        probePixel.Y
                );

                visitQueue.Enqueue(probePixel);

                while (visitQueue.Count > 0)
                {
                    Pixel pixel = visitQueue.Dequeue();

                    // Skip visited pixels.
                    if (visitedMap.GetPixel(pixel.X, pixel.Y))
                        continue;

                    // Mark current pixel as visited.
                    visitedMap.SetPixel(pixel.X, pixel.Y, true);

                    // Update max bounds.
                    maxBounds = RectangleHelpers.Create(
                            Math.Min(pixel.X, maxBounds.Left),
                            Math.Min(pixel.Y, maxBounds.Top),
                            Math.Max(pixel.X, maxBounds.Right),
                            Math.Max(pixel.Y, maxBounds.Bottom)
                    );

                    // Check surrounding pixels if unvisited and black.
                    foreach (Pixel surroundingPixel in bnwMap.GetPixels(
                            pixel.X - 1, pixel.X + 1, pixel.Y - 1, pixel.Y + 1))
                    {
                        if (visitedMap.GetPixel(surroundingPixel.X, surroundingPixel.Y))
                            continue;

                        visitQueue.Enqueue(surroundingPixel);
                    }
                }

                // Create text region.
                Rectangle bounds = new Rectangle(
                        maxBounds.Left,
                        maxBounds.Top,
                        maxBounds.Right - maxBounds.Left,
                        maxBounds.Bottom - maxBounds.Top
                );

                if (bounds.Width == 0 || bounds.Height == 0)
                    continue;

                yield return bounds;
            }
        }
    }

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
