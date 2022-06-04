using System;
using System.Collections.Generic;
using System.Drawing;
using Yomitan.Shared.OCR;
using Yomitan.Shared.Utils;

namespace Yomitan.Kanjitomo.OCR
{
    public class KanjiTomoDetector : IDetector
    {
        // TODO: Refactor to Configuration.
        private static readonly int CROP_BOUNDS_OFFSET = 50;
        private static readonly int CROP_BOUNDS_WIDTH = 100;
        private static readonly int CROP_BOUNDS_HEIGHT = 300;
        private static readonly int PROBE_INITIAL_WIDTH = 10;
        private static readonly int PROBE_INITIAL_HEIGHT = 10;
        private static readonly float PROBE_MULTIPLIER = 3.0f;

        public TextRegion GetRegion(ImageSource imageSource, Point originPoint)
        {
            // Crop screenshot into a smaller image.
            Rectangle cropBounds = new Rectangle(
                Math.Max(0, originPoint.X - CROP_BOUNDS_OFFSET),
                Math.Max(0, originPoint.Y - CROP_BOUNDS_OFFSET),
                Math.Min(CROP_BOUNDS_WIDTH, imageSource.Image.Width),
                Math.Min(CROP_BOUNDS_HEIGHT, imageSource.Image.Height));

            Bitmap processedImage = Screenshot.Bounds(
                    imageSource.Image, cropBounds)
                    .Grayscale()
                    .Sharpen()
                    .Binarize();

            // Boolean map representation of the black and white image.
            // Black pixels = 1, White pixels = 0.
            PixelMap bnwMap = new PixelMap(processedImage);
            bnwMap.Initialize((bitmap, x, y) =>
            {
                Color color = bitmap.GetPixel(x, y);
                return Color.Black.ToArgb().Equals(color.ToArgb());
            });

            // Boolean map representation of all visited pixels.
            // Mark all white pixels as already visited.
            PixelMap visitedMap = new PixelMap(processedImage);
            visitedMap.Initialize((bitmap, x, y) =>
            {
                Color color = bitmap.GetPixel(x, y);
                return Color.White.ToArgb().Equals(color.ToArgb());
            });

            // Set initial probe.
            Rectangle probeBounds = new Rectangle(
                    CROP_BOUNDS_OFFSET, 
                    CROP_BOUNDS_OFFSET, 
                    PROBE_INITIAL_WIDTH, 
                    PROBE_INITIAL_HEIGHT);

            // Keep extending text region until bounds are unchanged.
            // Probing is performed downwards the current text region.
            TextRegion prevRegion = null;
            TextRegion currRegion = new TextRegion(probeBounds);
            while (!currRegion.Equals(prevRegion))
            {
                prevRegion = currRegion.Clone();

                foreach (TextRegion probedRegion in ProbeRegion(visitedMap, bnwMap, probeBounds))
                    currRegion.Append(probedRegion);
                
                // Update probe bounds.
                probeBounds = new Rectangle(
                        currRegion.Bounds.Left,
                        currRegion.Bounds.Bottom,
                        currRegion.Bounds.Width,
                        (int)(currRegion.Bounds.Width * PROBE_MULTIPLIER));
            }

            // Convert back to absolute coordinates.
            currRegion.Bounds = new Rectangle(
                    originPoint.X - CROP_BOUNDS_OFFSET + currRegion.Bounds.Left,
                    originPoint.Y - CROP_BOUNDS_OFFSET + currRegion.Bounds.Top,
                    currRegion.Bounds.Width,
                    currRegion.Bounds.Height);

            return currRegion;
        }

        private IEnumerable<TextRegion> ProbeRegion(PixelMap visitedMap, PixelMap bnwMap, Rectangle probeBounds)
        {
            Queue<Pixel> visitQueue = new Queue<Pixel>();

            foreach (Pixel probePixel in bnwMap.GetPixels(
                    probeBounds.Left, probeBounds.Right, probeBounds.Top, probeBounds.Bottom))
            {
                // Set initial max bounds.
                AssignableRectangle maxBounds = new AssignableRectangle(
                    probePixel.X, 
                    probePixel.Y, 
                    probePixel.X, 
                    probePixel.Y);

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
                    maxBounds.Left = Math.Min(pixel.X, maxBounds.Left);
                    maxBounds.Top = Math.Min(pixel.Y, maxBounds.Top);
                    maxBounds.Right = Math.Max(pixel.X, maxBounds.Right);
                    maxBounds.Bottom = Math.Max(pixel.Y, maxBounds.Bottom);

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
                    maxBounds.Bottom - maxBounds.Top);

                if (bounds.Width == 0 || bounds.Height == 0)
                    continue;

                yield return new TextRegion(bounds);
            }
        }
    }

}
