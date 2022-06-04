using System;

namespace Yomitan.Shared.User32
{
    public static class Delegates
    {
        public delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);
    }
}
