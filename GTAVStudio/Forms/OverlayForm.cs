using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GTA;
using GTAVStudio.Common;
using GTAVStudio.Scripts;
using GTAVStudio.Theme;

namespace GTAVStudio.Forms
{
    class DynamicComponent
    {
        public string TranslationKey { get; set; }

        public IComponent Component { get; set; }

        public string DefaultTranslation { get; set; }
    }

    public class OverlayForm : Form
    {
        private readonly MenuStrip _menuStrip = new MenuStrip();
        private readonly List<DynamicComponent> _dynamicComponents = new List<DynamicComponent>();

        public OverlayForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            MinimumSize = new Size(0, 0);
            foreach (var screen in Screen.AllScreens)
            {
                if (!screen.Bounds.Contains(Location)) continue;
                Location = new Point(screen.Bounds.Left, screen.Bounds.Top);
                Width = screen.Bounds.Width;
                break;
            }

            KeyUp += OnKeyUp;

            SuspendLayout();

            _menuStrip.Renderer = new ThemeToolStripRenderer();
            _menuStrip.Dock = DockStyle.Top;
            MainMenuStrip = _menuStrip;
            Height = _menuStrip.Height;

            #region Vehicles

            var vehicleMenuItem = new ToolStripMenuItem();
            _dynamicComponents.Add(new DynamicComponent
            {
                Component = vehicleMenuItem,
                TranslationKey = "VehiclesMenu",
                DefaultTranslation = "Vehicles"
            });

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

            _menuStrip.Items.Add(vehicleMenuItem);

            #endregion

            #region GTAVStudio

            var gtavStudioMenuItem = new ToolStripMenuItem();
            _dynamicComponents.Add(new DynamicComponent
            {
                Component = gtavStudioMenuItem,
                TranslationKey = "GTAVStudioMenu",
                DefaultTranslation = "Studio"
            });

            var reloadSettingsMenuItem = new ToolStripMenuItem();
            _dynamicComponents.Add(new DynamicComponent
            {
                Component = reloadSettingsMenuItem,
                TranslationKey = "GTAVStudioMenu_ReloadSettings",
                DefaultTranslation = "Reload Settings"
            });
            reloadSettingsMenuItem.Click += (sender, args) => { OverlayScript.ReloadSettingsNextFrame = true; };

            gtavStudioMenuItem.DropDownItems.Add(reloadSettingsMenuItem);

            _menuStrip.Items.Add(gtavStudioMenuItem);

            #endregion

            Controls.Add(_menuStrip);

            SetDynamicComponentValues();

            ResumeLayout();

            User32.SetWindowPos(Handle, User32.HWND_TOPMOST, 0, 0, 0, 0, User32.TOPMOST_FLAGS);
        }

        public void Reload()
        {
            SetDynamicComponentValues();
            Refresh();
        }

        private void SetDynamicComponentValues()
        {
            BackColor = TransparencyKey =
                StudioSettings.GetValue(Constants.Settings.Theme, "TransparencyKey", Color.Red);

            _menuStrip.BackColor = StudioSettings.GetValue(Constants.Settings.Theme, "Menu_BackColor", Color.DimGray);

            foreach (var dynamicComponent in _dynamicComponents)
            {
                if (dynamicComponent.Component is ToolStripMenuItem menuItem)
                {
                    menuItem.Text =
                        StudioTranslations.GetValue(Constants.Translations.Overlay, dynamicComponent.TranslationKey,
                            dynamicComponent.DefaultTranslation);
                }
            }
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