using log4net;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using Yomitan.Core.Helpers;
using Yomitan.Core.Models.OCR;
using Yomitan.Core.Services;
using Yomitan.Shared.Utils;

namespace Yomitan.Service
{
    public class HoverMode : IDisposable
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // TODO: Retrieve from configuration file.
        private static readonly int SCAN_TEXT_INTERVAL_MS = 200;

        private readonly ITextDetector _textDetector;
        private readonly ITextRecognizer _textRecognizer;

        private readonly DispatcherTimer _mouseTimer;
        private volatile bool _isProcessing;

        public event EventHandler<TextRegion> Hovered;

        public HoverMode(ITextDetector textDetector, ITextRecognizer textRecognizer)
        {
            _textDetector = textDetector;
            _textRecognizer = textRecognizer;

            _mouseTimer = new DispatcherTimer();
            _mouseTimer.Interval = TimeSpan.FromMilliseconds(SCAN_TEXT_INTERVAL_MS);
            _mouseTimer.Tick += ScanTextOnPointer;
        }

        public void Start()
        {
            _mouseTimer.Start();
        }

        public void Stop()
        {
            _mouseTimer.Stop();
        }

        private void ScanTextOnPointer(object sender, EventArgs e)
        {
            if (_isProcessing || !ShouldScanText())
                return;

            try
            {
                _isProcessing = true;
                ScanTextOnPointerImpl();
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private void ScanTextOnPointerImpl()
        {
            Logger.Debug("Taking screenshot of current screen...");

            Point currentPos = Cursor.Position;
            Screen screen = Screen.FromPoint(currentPos);
            using (Bitmap screenshotImage = ScreenshotHelper.Bounds(screen.Bounds))
            {
                Logger.Debug("Detecting text regions...");
                ImageSource screenshotSource = new ImageSource(screenshotImage);
                Rectangle detectedRegion = _textDetector.GetRegion(screenshotSource, Cursor.Position);

                if (Rectangle.Empty.Equals(detectedRegion) 
                        || detectedRegion.Width == 0 
                        || detectedRegion.Height == 0)
                    return;

                Logger.Debug("Cropping image to hovered region...");
                using (Bitmap croppedImage = ScreenshotHelper.Bounds(screenshotImage, detectedRegion))
                using (Bitmap paddedImage = croppedImage.Pad(Color.White))
                {
                    Logger.Debug("Running OCR on hovered region...");
                    ImageSource croppedSource = new ImageSource(paddedImage);
                    TextRegion updatedRegion = _textRecognizer.Read(croppedSource);

                    updatedRegion.Bounds = detectedRegion;

                    Logger.Debug("Passing recognized text to listeners...");
                    Hovered?.Invoke(this, updatedRegion);
                }
            }
        }

        private bool ShouldScanText()
        {
            return Control.ModifierKeys == Keys.Shift;
        }

        public void Dispose()
        {
            if (_mouseTimer != null)
                _mouseTimer.Stop();
        }
    }
}
