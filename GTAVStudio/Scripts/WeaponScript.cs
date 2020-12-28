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
        public static bool InfiniteAmmo;
        public static bool ExplosiveAmmo;

        public WeaponScript()
        {
            Tick += OnTick;
            KeyUp += OnKeyUp;
        }

        private static void OnKeyUp(object sender, KeyEventArgs e)
        {
            var infiniteAmmoShortcut = StudioSettings.GetShortcut("InfiniteAmmo", Keys.Alt | Keys.A);
            if (infiniteAmmoShortcut != Keys.None && e.KeyData == infiniteAmmoShortcut)
            {
                OverlayScript.Overlay.InfiniteAmmo = InfiniteAmmo = !InfiniteAmmo;
                return;
            }

            var explosiveAmmoShortcut = StudioSettings.GetShortcut("ExplosiveAmmo", Keys.None);
            if (explosiveAmmoShortcut != Keys.None && e.KeyData == explosiveAmmoShortcut)
            {
                OverlayScript.Overlay.ExplosiveAmmo = ExplosiveAmmo = !ExplosiveAmmo;
                return;
            }

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

            Game.Player.Character.Weapons.Current.InfiniteAmmo = InfiniteAmmo;
            Game.Player.Character.Weapons.Current.InfiniteAmmoClip = InfiniteAmmo;

            if (ExplosiveAmmo && Game.Player.Character.IsShooting)
            {
                World.AddExplosion(Game.Player.Character.LastWeaponImpactPosition, ExplosionType.Grenade, 100, .25f, Game.Player.Character);
            }
        }

        internal static void SpawnWeapon(WeaponHash hash)
        {
            Game.Player.Character.Weapons.Give(hash, 999, true, true);
        }
    }
}