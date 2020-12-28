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

        public string ShortcutKey { get; set; }

        public string ShortcutDisplayStringDefault { get; set; }

        public Keys ShortcutKeysDefault { get; set; }
    }

    public class OverlayForm : Form
    {
        private readonly MenuStrip _menuStrip = new MenuStrip();
        private readonly List<DynamicComponent> _dynamicComponents = new List<DynamicComponent>();

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

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

            _menuStrip.RenderMode = ToolStripRenderMode.Professional;
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

            var repairVehicleMenuItem = new ToolStripMenuItem();
            _dynamicComponents.Add(new DynamicComponent
            {
                Component = repairVehicleMenuItem,
                TranslationKey = "VehiclesMenu_Repair",
                DefaultTranslation = "Repair Vehicle",
                ShortcutKey = "RepairVehicle",
                ShortcutKeysDefault = Keys.Alt | Keys.R,
                ShortcutDisplayStringDefault = "Alt+R"
            });
            repairVehicleMenuItem.Click += (sender, args) => { VehicleScript.RepairVehicleNextFrame = true; };
            vehicleMenuItem.DropDownItems.Add(repairVehicleMenuItem);

            var spawnVehicleMenuItem = new ToolStripMenuItem();
            _dynamicComponents.Add(new DynamicComponent
            {
                Component = spawnVehicleMenuItem,
                TranslationKey = "VehiclesMenu_Spawn",
                DefaultTranslation = "Spawn Vehicle"
            });

            var vehicleHashes = Enum.GetValues(typeof(VehicleHash)).OfType<VehicleHash>();
            foreach (var vehicleHash in vehicleHashes.OrderBy(v => Enum.GetName(typeof(VehicleHash), v)))
            {
                var menuItem = new ToolStripMenuItem();
                var text = Enum.GetName(typeof(VehicleHash), vehicleHash);
                _dynamicComponents.Add(new DynamicComponent
                {
                    TranslationKey = "VehiclesMenu_Spawn_" + text,
                    Component = menuItem,
                    DefaultTranslation = text,
                    ShortcutKey = "SpawnVehicle_" + text,
                    ShortcutKeysDefault = Keys.None,
                    ShortcutDisplayStringDefault = "None"
                });
                menuItem.Click += (sender, args) =>
                {
                    VehicleScript.SpawnVehicleNextFrame = vehicleHash;
                    OverlayScript.ToggleOverlayNextFrame = true;
                };
                spawnVehicleMenuItem.DropDownItems.Add(menuItem);
            }

            vehicleMenuItem.DropDownItems.Add(spawnVehicleMenuItem);

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
            ThemeColorTable.Reload();
            Refresh();
        }

        private void SetDynamicComponentValues()
        {
            BackColor = TransparencyKey =
                StudioSettings.GetValue(Constants.Settings.Theme, "TransparencyKey", Color.Red);

            _menuStrip.BackColor = StudioSettings.GetValue(Constants.Settings.Theme, "Menu_BackColor", Color.DimGray);
            _menuStrip.ForeColor = StudioSettings.GetValue(Constants.Settings.Theme, "Menu_ForeColor", Color.Black);

            foreach (var dynamicComponent in _dynamicComponents)
            {
                if (dynamicComponent.Component is ToolStripMenuItem menuItem)
                {
                    menuItem.BackColor =
                        StudioSettings.GetValue(Constants.Settings.Theme, "Menu_Item_BackColor", Color.DimGray);
                    menuItem.ForeColor =
                        StudioSettings.GetValue(Constants.Settings.Theme, "Menu_Item_ForeColor", Color.Black);

                    menuItem.Text =
                        StudioTranslations.GetValue(Constants.Translations.Overlay, dynamicComponent.TranslationKey,
                            dynamicComponent.DefaultTranslation);

                    if (dynamicComponent.ShortcutKey != null)
                    {
                        var shortcutKeys = StudioSettings.GetShortcut(dynamicComponent.ShortcutKey,
                            dynamicComponent.ShortcutKeysDefault);
                        menuItem.ShortcutKeys = shortcutKeys;
                        menuItem.ShowShortcutKeys = shortcutKeys != Keys.None;
                        menuItem.ShortcutKeyDisplayString =
                            string.Join(" + ",
                                StudioSettings.GetValue(Constants.Settings.Shortcuts, dynamicComponent.ShortcutKey,
                                    dynamicComponent.ShortcutDisplayStringDefault).Split('+'));
                    }
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