// -------------------------------------------------------------------------------------
//
//
// Assembly:    WpfApplicationBar
// Filename:    Rectangle.cs
// Created:     02/04/2013 1:43:57 PM
// Version:
//
// 0.0.0.1      AW  02/04/2013  - Initial release.
//
// -------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace WpfApplicationBar
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Rectangle
    {
        #region Fields

        public int Left;

        public int Top;

        public int Right;

        public int Bottom;

        #endregion
    }
}
