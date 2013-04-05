// -------------------------------------------------------------------------------------
//
//
// Assembly:    WpfApplicationBar
// Filename:    ApplicationBarData.cs
// Created:     02/04/2013 1:43:57 PM
// Version:
//
// 0.0.0.1      AW  02/04/2013  - Initial release.
//
// -------------------------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;

namespace WpfApplicationBar
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ApplicationBarData
    {
        #region Fields

        public int Size;

        public IntPtr hWnd;

        public int CallbackMessage;

        public int Edge;

        public Rectangle Rectangle;

        public IntPtr lParam;

        #endregion
    }
}
