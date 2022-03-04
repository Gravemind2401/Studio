using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Studio.Utilities
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out Win32Point pt);

        [DllImport("user32.dll")]
        private static extern IntPtr GetTopWindow(IntPtr hWnd);

        private const uint GW_HWNDNEXT = 2;

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint wCmd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd,
            int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        private const int HWND_TOPMOST = -1;
        private const int HWND_NOTOPMOST = -2;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;

        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point
        {
            public int X;
            public int Y;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct Win32Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static Point GetMousePosition()
        {
            GetCursorPos(out Win32Point pos);
            return new Point(pos.X, pos.Y);
        }

        public static IEnumerable<Window> SortWindowsTopToBottom(IEnumerable<Window> unsorted)
        {
            var byHandle = unsorted.ToDictionary(win =>
              ((HwndSource)PresentationSource.FromVisual(win)).Handle);

            for (IntPtr hWnd = GetTopWindow(IntPtr.Zero); hWnd != IntPtr.Zero; hWnd = GetWindow(hWnd, GW_HWNDNEXT))
            {
                if (byHandle.ContainsKey(hWnd))
                    yield return byHandle[hWnd];
            }
        }

        public static void BringToFront(this Window wnd)
        {
            var hwnd = ((HwndSource)PresentationSource.FromVisual(wnd)).Handle;
            SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }
    }
}
