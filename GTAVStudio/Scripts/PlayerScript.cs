using System;
using System.Windows.Forms;
using GTA;
using GTAVStudio.Common;

namespace GTAVStudio.Scripts
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlayerScript : Script
    {
        public static bool Invincible;

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
            }
        }

        private static void OnTick(object sender, EventArgs e)
        {
            Game.Player.Character.IsInvincible = Invincible;
            if (Invincible)
            {
                Game.Player.Character.Health = Game.Player.Character.MaxHealth;
            }
        }
    }
}