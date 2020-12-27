using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GTA;
using GTAVStudio.Common;
using GTAVStudio.Scripts;

namespace GTAVStudio.Forms
{
    public class OverlayForm : Form
    {
        public OverlayForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Red;
            TransparencyKey = Color.Red;
            StartPosition = FormStartPosition.Manual;
            MinimumSize = new Size(0, 0);
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                if (!screen.Bounds.Contains(Location)) continue;
                Location = new Point(screen.Bounds.Left, screen.Bounds.Top);
                Width = screen.Bounds.Width;
                break;
            }

            KeyUp += OnKeyUp;

            SuspendLayout();

            var menuStrip = new MenuStrip();
            menuStrip.Dock = DockStyle.Top;
            menuStrip.BackColor = Color.DimGray;
            MainMenuStrip = menuStrip;
            Height = menuStrip.Height;

            #region Vehicles

            var vehicleMenuItem = new ToolStripMenuItem();
            vehicleMenuItem.Text =
                StudioTranslations.GetValue(Constants.Translations.Overlay, "VehiclesMenu", "Vehicles");

            var vehicleHashes = Enum.GetValues(typeof(VehicleHash)).OfType<VehicleHash>();
            foreach (var vehicleHash in vehicleHashes.OrderBy(v => Enum.GetName(typeof(VehicleHash), v)))
            {
                var menuItem = new ToolStripMenuItem();
                menuItem.Text = Enum.GetName(typeof(VehicleHash), vehicleHash);
                menuItem.Click += (sender, args) =>
                {
                    VehicleScript.SpawnVehicleNextFrame = vehicleHash;
                    OverlayScript.ToggleOverlayNextFrame = true;
                };
                vehicleMenuItem.DropDownItems.Add(menuItem);
            }

            menuStrip.Items.Add(vehicleMenuItem);

            #endregion

            #region GTAVStudio

            var gtavStudioMenuItem = new ToolStripMenuItem();
            gtavStudioMenuItem.Text =
                StudioTranslations.GetValue(Constants.Translations.Overlay, "GTAVStudioMenu", "Studio");

            var reloadSettingsMenuItem = new ToolStripMenuItem();
            reloadSettingsMenuItem.Text =
                StudioTranslations.GetValue(Constants.Translations.Overlay, "GTAVStudioMenu_ReloadSettings",
                    "Reload Settings");
            reloadSettingsMenuItem.Click += (sender, args) =>
            {
                OverlayScript.ReloadSettings();
                OverlayScript.ToggleOverlayNextFrame = true;
            };

            gtavStudioMenuItem.DropDownItems.Add(reloadSettingsMenuItem);

            menuStrip.Items.Add(gtavStudioMenuItem);

            #endregion

            Controls.Add(menuStrip);

            ResumeLayout();

            User32.SetWindowPos(Handle, User32.HWND_TOPMOST, 0, 0, 0, 0, User32.TOPMOST_FLAGS);
        }

        private static void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == StudioSettings.GetValue(Constants.Settings.Overlay, "ToggleKey", Keys.F12)
                || e.KeyCode == Keys.Escape)
            {
                OverlayScript.ToggleOverlayNextFrame = true;
            }
        }
    }
}