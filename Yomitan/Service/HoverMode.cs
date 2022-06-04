using log4net;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using Yomitan.Shared.OCR;
using Yomitan.Shared.Utils;

namespace Yomitan.Service
{
    public class HoverMode : IDisposable
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // TODO: Retrieve from configuration file.
        private static readonly int SCAN_TEXT_INTERVAL_MS = 200;

        private readonly IDetector _detector;
        private readonly IRecognizer _recognizer;

        private readonly DispatcherTimer _mouseTimer;
        private volatile bool _isProcessing;

        public event EventHandler<TextRegion> Hovered;

        public HoverMode(IDetector detector, IRecognizer recognizer)
        {
            _detector = detector;
            _recognizer = recognizer;

            _mouseTimer = new DispatcherTimer();
            _mouseTimer.Interval = TimeSpan.FromMilliseconds(SCAN_TEXT_INTERVAL_MS);
            _mouseTimer.Tick += ScanTextOnPointer;
            _mouseTimer.Start();
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
            using (Bitmap screenshotImage = Screenshot.Bounds(screen.Bounds))
            {
                Logger.Debug("Detecting text regions...");
                ImageSource screenshotSource = new ImageSource(screenshotImage);
                TextRegion region = _detector.GetRegion(screenshotSource, Cursor.Position);

                if (region == null || region.Bounds.Width == 0 || region.Bounds.Height == 0)
                    return;

                Logger.Debug("Cropping image to hovered region...");
                using (Bitmap croppedImage = Screenshot.Bounds(screenshotImage, region.Bounds))
                using (Bitmap paddedImage = croppedImage.Pad(Color.White))
                {
                    Logger.Debug("Running OCR on hovered region...");
                    ImageSource croppedSource = new ImageSource(paddedImage);
                    string text = _recognizer.Read(croppedSource);

                    Logger.Debug("Passing recognized text to listeners...");
                    Hovered?.Invoke(this, new TextRegion(region.Bounds, screen.Bounds, text));
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
