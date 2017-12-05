using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Cartographer
{
    //[StructLayout(LayoutKind.Sequential)]
    //public struct RECT
    //{
    //    public int Left;
    //    public int Top;
    //    public int Right;
    //    public int Bottom;

    //    public RECT(int left, int top, int right, int bottom)
    //    {
    //        Left = left;
    //        Top = top;
    //        Right = right;
    //        Bottom = bottom;
    //    }

    //    public RECT(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom)
    //    {
    //    }

    //    public static implicit operator Rectangle(RECT r) => new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);

    //    public static implicit operator RECT(Rectangle r) => new RECT(r);
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public struct POINT
    //{
    //    public int X;
    //    public int Y;

    //    public POINT(int x, int y)
    //    {
    //        X = x;
    //        Y = y;
    //    }

    //    public POINT(Point p) : this(p.X, p.Y)
    //    {
    //    }

    //    public static implicit operator Point(POINT p) => new Point(p.X, p.Y);

    //    public static implicit operator POINT(Point p) => new POINT(p);
    //}

    public static class WinApi
    {
        // Get the focused window.
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        // Send a message to a specific process via the handle.
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        // Places (posts) a message in the message queue associated with the thread that created
        // the specified window and returns without waiting for the thread to process the message.
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr PostMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        // Gets the positions of the corners of a window via the MainWindowHandle.
        //[DllImport("user32.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        // Converts a point in screen space to a point relative to hWnd's window.
        //[DllImport("user32.dll")]
        //public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);
    }
}