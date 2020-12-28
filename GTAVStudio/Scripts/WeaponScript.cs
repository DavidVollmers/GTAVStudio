using System;
using System.Linq;
using System.Windows.Forms;
using GTA;
using GTAVStudio.Common;

namespace GTAVStudio.Scripts
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WeaponScript : Script
    {
        public static WeaponHash SpawnWeaponNextFrame;

        public WeaponScript()
        {
            Tick += OnTick;
            KeyUp += OnKeyUp;
        }

        private static void OnKeyUp(object sender, KeyEventArgs e)
        {
            var weaponHashes = Enum.GetValues(typeof(WeaponHash)).OfType<WeaponHash>();
            foreach (var weaponHash in weaponHashes)
            {
                var text = Enum.GetName(typeof(WeaponHash), weaponHash);
                var shortcut = StudioSettings.GetShortcut("SpawnWeapon_" + text, Keys.None);
                if (shortcut == Keys.None) continue;
                if (e.KeyData == shortcut)
                {
                    SpawnWeapon(weaponHash);
                }
            }
        }

        private static void OnTick(object sender, EventArgs e)
        {
            if (SpawnWeaponNextFrame != 0)
            {
                var hash = SpawnWeaponNextFrame;
                SpawnWeaponNextFrame = 0;

                SpawnWeapon(hash);
            }
        }

        internal static void SpawnWeapon(WeaponHash hash)
        {
            Game.Player.Character.Weapons.Give(hash, 999, true, true);
        }
    }
}