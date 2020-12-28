using System;
using System.Windows.Forms;
using GTA;
using GTA.Math;
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

                var velocity = new Vector3();
                var rotation = Game.Player.Character.Rotation;
                rotation.X = 0;

                if (OverlayScript.IsGameActive)
                {
                    var playerDirection = Game.Player.Character.Rotation.RotationToDirection().Normalized;
                    var sideRotation = Game.Player.Character.Rotation;
                    sideRotation.Y += 90;
                    var playerSideDirection = sideRotation.RotationToDirection().Normalized;

                    if (User32.GetKeyState(Keys.A).HasFlag(User32.KeyStates.Down)
                        || User32.GetKeyState(Keys.D).HasFlag(User32.KeyStates.Down))
                    {
                        velocity.X += playerSideDirection.X * 2;
                        velocity.Y += playerSideDirection.Y * 2;
                    }

                    var upDownVelocity = 10;
                    if (Game.Player.Character.IsInParachuteFreeFall || Game.Player.Character.IsRagdoll)
                    {
                        upDownVelocity = 20;
                        
                        if (User32.GetKeyState(Keys.W).HasFlag(User32.KeyStates.Down))
                        {
                            velocity.X += playerDirection.X * 80;
                            velocity.Y += playerDirection.Y * 80;
                        }

                        if (User32.GetKeyState(Keys.S).HasFlag(User32.KeyStates.Down))
                        {
                            velocity.X -= playerDirection.X * 80;
                            velocity.Y -= playerDirection.Y * 80;
                        }
                    }
                    else
                    {
                        if (User32.GetKeyState(Keys.W).HasFlag(User32.KeyStates.Down)
                            || User32.GetKeyState(Keys.S).HasFlag(User32.KeyStates.Down))
                        {
                            velocity.X += playerDirection.X * 20;
                            velocity.Y += playerDirection.Y * 20;
                        }
                        
                        rotation.Y = 0;
                        
                        if (User32.GetKeyState(Keys.NumPad4).HasFlag(User32.KeyStates.Down))
                        {
                            rotation.Z += 2;
                        }
                    
                        if (User32.GetKeyState(Keys.NumPad6).HasFlag(User32.KeyStates.Down))
                        {
                            rotation.Z -= 2;
                        }
                    }
                    
                    if (User32.GetKeyState(Keys.NumPad8).HasFlag(User32.KeyStates.Down))
                    {
                        velocity.Z += upDownVelocity;
                    }

                    if (User32.GetKeyState(Keys.NumPad5).HasFlag(User32.KeyStates.Down))
                    {
                        velocity.Z -= upDownVelocity;
                    }
                }

                Game.Player.Character.Velocity = velocity;
                Game.Player.Character.Rotation = rotation;
            }
        }
    }
}