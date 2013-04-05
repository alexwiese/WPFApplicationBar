// -------------------------------------------------------------------------------------
//
//
// Assembly:    WpfApplicationBar
// Filename:    ApplicationBarWindow.cs
// Created:     02/04/2013 1:43:57 PM
// Version:
//
// 0.0.0.1      AW  02/04/2013  - Initial release.
//
// -------------------------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace WpfApplicationBar
{
    public class ApplicationBarWindow : Window
    {
        // -----------------------------------------------------------------------------
        #region Constants

        private const string ApplicationBarMessageHandle = "AppBarMessage";

        private const string TaskBarClassName = "Shell_TrayWnd";

        #endregion

        // -----------------------------------------------------------------------------
        #region Delegates

        #endregion

        // -----------------------------------------------------------------------------
        #region Events

        #endregion

        // -----------------------------------------------------------------------------
        #region Fields

        private bool isBarRegistered = false;

        private IntPtr hWnd;

        private Screen displayScreen;

        private DisplayEdge currentEdge;

        #endregion

        // -----------------------------------------------------------------------------
        #region Properties

        /// <summary>
        /// Gets or sets the display screen.
        /// </summary>
        /// <value>
        /// The screen the <see cref="ApplicationBarWindow"/> will be displayed on.
        /// </value>
        public Screen DisplayScreen
        {
            get
            {
                return displayScreen;
            }
            set
            {
                displayScreen = value;
                ResetPosition();
            }
        }

        /// <summary>
        /// Gets or sets the current display edge.
        /// </summary>
        /// <value>
        /// The edge of the display the <see cref="ApplicationBarWindow" /> will be displayed on.
        /// </value>
        public DisplayEdge CurrentEdge
        {
            get
            {
                return currentEdge;
            }
            set
            {
                currentEdge = value;
                ResetPosition();
            }
        }

        #endregion

        // -----------------------------------------------------------------------------
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBarWindow"/> class.
        /// </summary>
        public ApplicationBarWindow()
            : base()
        {
            this.DisplayScreen = Screen.PrimaryScreen;

            this.CurrentEdge = DisplayEdge.Top;

            this.Top = DisplayScreen.Bounds.Bottom;

            this.Loaded += (s, e) => this.Visibility = System.Windows.Visibility.Hidden;
        }

        #endregion

        // -----------------------------------------------------------------------------
        #region Methods

        #region Private methods

        private IntPtr ProcessParameter(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (isBarRegistered)
            {
                var iParam = wParam.ToInt32();

                if (iParam == (int)ApplicationBarMessage.FullScreenApplication || iParam == (int)ApplicationBarMessage.PositionChanged)
                {
                    Task.Factory.StartNew(SetPosition);
                }
            }

            return IntPtr.Zero;
        }

        private Rectangle GetTaskbarPosition(IntPtr hWnd)
        {
            var data = new ApplicationBarData();

            data.Size = Marshal.SizeOf(typeof(ApplicationBarData));

            data.hWnd = hWnd;

            IntPtr result = SHAppBarMessage((int)ApplicationBarCommand.GetTaskBarPosition, ref data);

            return data.Rectangle;
        }

        private void Register()
        {
            var appBarData = new ApplicationBarData();

            appBarData.Size = Marshal.SizeOf(appBarData);

            appBarData.hWnd = this.hWnd;

            if (!isBarRegistered)
            {
                var callback = RegisterWindowMessage(ApplicationBarMessageHandle);

                appBarData.CallbackMessage = callback;

                SHAppBarMessage((int)ApplicationBarCommand.New, ref appBarData);

                isBarRegistered = true;
            }
            else
            {
                SHAppBarMessage((int)ApplicationBarCommand.Remove, ref appBarData);

                isBarRegistered = false;
            }
        }

        /// <summary>
        /// Sets the initial position.
        /// </summary>
        private void SetPosition()
        {
            var currentScreen = DisplayScreen;

            var edge = CurrentEdge;

            // Get hWnd for taskbar
            var taskBarHwnd = FindWindow(TaskBarClassName, null);

            // Get the screen the taskbar is displayed on
            var taskBarScreen = Screen.FromHandle(taskBarHwnd);

            // Will be assigned to if the taskbar position conflicts with this app bar
            int? overridePosition = null;

            if (taskBarScreen.Equals(currentScreen))
            {
                // Find task bar bounds
                var taskBarPosition = GetTaskbarPosition(taskBarHwnd);

                // Determine if the taskbar is on the Bottom or Top of the screen
                var taskBarEdge = Math.Abs(currentScreen.Bounds.Bottom - taskBarPosition.Bottom) <
                    Math.Abs(currentScreen.Bounds.Top - taskBarPosition.Top) ? DisplayEdge.Bottom : DisplayEdge.Top;

                // If the taskbar is on the same edge as the app bar then override the position
                if (taskBarEdge == edge)
                {
                    overridePosition = edge == DisplayEdge.Bottom ? taskBarPosition.Top : taskBarPosition.Bottom;
                }
            }

            var appBarData = new ApplicationBarData();

            appBarData.Size = Marshal.SizeOf(appBarData);

            appBarData.hWnd = this.hWnd;

            // Set the edge we wish to dock to
            appBarData.Edge = (int)edge;

            // Create the rectangle to position the window in
            appBarData.Rectangle.Left = (int)currentScreen.Bounds.Left;
            appBarData.Rectangle.Right = (int)currentScreen.Bounds.Right;

            if (appBarData.Edge == (int)DisplayEdge.Top)
            {
                appBarData.Rectangle.Top = overridePosition ?? currentScreen.Bounds.Top;
                appBarData.Rectangle.Bottom = appBarData.Rectangle.Top + (int)this.ActualHeight;
            }
            else
            {
                appBarData.Rectangle.Bottom = overridePosition ?? currentScreen.Bounds.Bottom;
                appBarData.Rectangle.Top = appBarData.Rectangle.Bottom - (int)this.ActualHeight;
            }

            // Set the new AppBar position
            Task.Factory.StartNew(() => SHAppBarMessage((int)ApplicationBarCommand.SetPosition, ref appBarData));

            // Delete this line. I dare you.
            Thread.Sleep(200);

            // Move the window to the AppBar position
            MoveWindow(appBarData.hWnd, appBarData.Rectangle.Left, appBarData.Rectangle.Top,
                       appBarData.Rectangle.Right - appBarData.Rectangle.Left, appBarData.Rectangle.Bottom - appBarData.Rectangle.Top, true);
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Resets the position.
        /// </summary>
        internal void ResetPosition()
        {
            this.Visibility = System.Windows.Visibility.Hidden;

            Task.Factory.StartNew(SetPosition).ContinueWith(t => this.Visibility = System.Windows.Visibility.Visible, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.SourceInitialized" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var source = PresentationSource.FromVisual(this) as HwndSource;

            if (source != null)
                source.AddHook(ProcessParameter);

            // Get this window hWnd
            this.hWnd = new WindowInteropHelper(this).Handle;

            // Register the app bar
            Register();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.ContentRendered" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            Task.Factory.StartNew(SetPosition).ContinueWith(t => this.Visibility = System.Windows.Visibility.Visible, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #region External Methods

        [DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr SHAppBarMessage(int dwMessage, ref ApplicationBarData pData);

        [DllImport("User32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int RegisterWindowMessage(string msg);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        #endregion

        #endregion

        #region Public methods

        #endregion

        #endregion

        // -----------------------------------------------------------------------------
        #region Event Handlers

        #endregion
    }
}
