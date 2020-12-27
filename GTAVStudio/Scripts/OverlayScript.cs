using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using GTA;
using GTA.UI;
using GTAVStudio.Common;
using GTAVStudio.Forms;

namespace GTAVStudio.Scripts
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class OverlayScript : Script
    {
        private static OverlayForm Overlay = new OverlayForm();
        private static bool _threadStarted;
        private static bool _threadStarting;
        private static bool _overlayToggle;
        public static bool ToggleOverlayNextFrame;

        public OverlayScript()
        {
            KeyUp += OnKeyUp;
            Tick += OnTick;
        }

        private static void OnTick(object sender, EventArgs e)
        {
            if (ToggleOverlayNextFrame)
            {
                ToggleOverlayNextFrame = false;

                ToggleOverlay();
            }

            if (_overlayToggle)
            {
                Hud.ShowCursorThisFrame();
            }
        }

        private static void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == StudioSettings.GetValue(Constants.Settings.Overlay, "ToggleKey", Keys.F12))
            {
                ToggleOverlay();
            }
        }

        internal static void ReloadSettings()
        {
            StudioSettings.Reload();
            StudioTranslations.Reload();
            Overlay.Dispose();
            Overlay = new OverlayForm();
        }
        
        internal static void ToggleOverlay()
        {
            if (_threadStarting) return;

            _overlayToggle = !_overlayToggle;

            if (!_threadStarted)
            {
                _threadStarting = true;

                var thread = new Thread(ShowForm);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            else
            {
                Overlay.Visible = _overlayToggle;
                if (Overlay.Visible)
                {
                    User32.SetForegroundWindow(Overlay.Handle);
                }
                else
                {
                    User32.SetForegroundWindow(Process.GetCurrentProcess().MainWindowHandle);
                }
            }

            Game.Pause(_overlayToggle);
        }

        private static void ShowForm()
        {
            _threadStarted = true;
            _threadStarting = false;
            Application.Run(Overlay);
            Overlay.Focus();
        }
    }
}