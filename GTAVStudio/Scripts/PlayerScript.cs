using System;
using System.Windows.Forms;
using GTA;
using GTAVStudio.Common;
using GTAVStudio.Extensions;

namespace GTAVStudio.Scripts
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlayerScript : Script
    {
        public static bool Invincible;
        public static bool DisableWantedLevel;
        public static bool FlyMode;

        public PlayerScript()
        {
            Tick += OnTick;
            KeyUp += OnKeyUp;
        }

        private static void OnKeyUp(object sender, KeyEventArgs e)
        {
            var playerInvincibleShortcut = StudioSettings.GetShortcut("PlayerInvincible", Keys.Alt | Keys.I);
            if (playerInvincibleShortcut != Keys.None && e.KeyData == playerInvincibleShortcut)
            {
                OverlayScript.Overlay.PlayerInvincible = Invincible = !Invincible;
                return;
            }

            var disableWantedLevelShortcut = StudioSettings.GetShortcut("DisableWantedLevel", Keys.Alt | Keys.W);
            if (disableWantedLevelShortcut != Keys.None && e.KeyData == disableWantedLevelShortcut)
            {
                OverlayScript.Overlay.DisableWantedLevel = DisableWantedLevel = !DisableWantedLevel;
                return;
            }

            var flyModeShortcut = StudioSettings.GetShortcut("PlayerFlyMode", Keys.Alt | Keys.F);
            if (flyModeShortcut != Keys.None && e.KeyData == flyModeShortcut)
            {
                OverlayScript.Overlay.PlayerFlyMode = FlyMode = !FlyMode;
                return;
            }
        }

        private static void OnTick(object sender, EventArgs e)
        {
            Game.Player.Character.IsInvincible = Invincible;
            if (Invincible)
            {
                Game.Player.Character.Health = Game.Player.Character.MaxHealth;
            }

            Game.Player.IgnoredByPolice = DisableWantedLevel;
            if (DisableWantedLevel)
            {
                Game.Player.WantedLevel = 0;
            }

            Game.Player.Character.HasGravity = true;
            if (FlyMode && !Game.Player.Character.IsSittingInVehicle())
            {
                Game.Player.Character.HasGravity = false;
                
                Game.Player.Character.ApplyFlyModeThisFrame(true);
            }
        }
    }
}