// -------------------------------------------------------------------------------------
//
//
// Assembly:    WpfApplicationBar
// Filename:    ApplicationBarCommand.cs
// Created:     02/04/2013 1:43:57 PM
// Version:
//
// 0.0.0.1      AW  02/04/2013  - Initial release.
//
// -------------------------------------------------------------------------------------

namespace WpfApplicationBar
{
    internal enum ApplicationBarCommand
    {
        New = 0,
        Remove,
        QueryPosition,
        SetPosition,
        GetState,
        GetTaskBarPosition,
        Activate,
        GetAutoHideBar,
        SetAutoHideBar,
        WindowPositionChanged,
        SetState
    }
}
