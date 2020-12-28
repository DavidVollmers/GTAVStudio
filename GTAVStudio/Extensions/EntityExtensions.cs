using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTAVStudio.Common;
using GTAVStudio.Extensions;
using GTAVStudio.Scripts;

namespace GTAVStudio.Extensions
{
    public static class EntityExtensions
    {
        public static void ApplySpeedModeThisFrame(this Entity target)
        {
            var velocity = new Vector3();
            velocity.Z = target.Velocity.Z;

            if (OverlayScript.IsGameActive)
            {
                var targetDirection = target.Rotation.RotationToDirection().Normalized;
                var sideRotation = target.Rotation;
                sideRotation.Y += 90;
                var targetSideDirection = sideRotation.RotationToDirection().Normalized;

                if (User32.GetKeyState(Keys.A).HasFlag(User32.KeyStates.Down)
                    || User32.GetKeyState(Keys.D).HasFlag(User32.KeyStates.Down))
                {
                    velocity.X += targetSideDirection.X * 2;
                    velocity.Y += targetSideDirection.Y * 2;
                }

                if (User32.GetKeyState(Keys.W).HasFlag(User32.KeyStates.Down))
                {
                    velocity.X += targetDirection.X * 80;
                    velocity.Y += targetDirection.Y * 80;
                }

                if (User32.GetKeyState(Keys.S).HasFlag(User32.KeyStates.Down))
                {
                    velocity.X -= targetDirection.X * 80;
                    velocity.Y -= targetDirection.Y * 80;
                }
            }

            target.Velocity = velocity;
        }

        public static void ApplyFlyModeThisFrame(this Entity target, bool isFollowedByCamera = false)
        {
            var velocity = new Vector3();
            var rotation = target.Rotation;
            rotation.X = rotation.Y = 0;

            if (OverlayScript.IsGameActive)
            {
                var targetDirection = target.Rotation.RotationToDirection().Normalized;
                var sideRotation = target.Rotation;
                sideRotation.Y += 90;
                var targetSideDirection = sideRotation.RotationToDirection().Normalized;

                if (User32.GetKeyState(Keys.A).HasFlag(User32.KeyStates.Down)
                    || User32.GetKeyState(Keys.D).HasFlag(User32.KeyStates.Down))
                {
                    velocity.X += targetSideDirection.X * 2;
                    velocity.Y += targetSideDirection.Y * 2;
                }

                var upDownVelocity = 10;
                var forwardBackVelocity = 20;
                var reverseBackwardVelocity = isFollowedByCamera;
                
                if (target is Ped ped && (ped.IsInParachuteFreeFall || ped.IsRagdoll))
                {
                    upDownVelocity = 20;
                    forwardBackVelocity = 80;
                    reverseBackwardVelocity = false;
                }
                
                if (User32.GetKeyState(Keys.W).HasFlag(User32.KeyStates.Down)
                    || User32.GetKeyState(Keys.S).HasFlag(User32.KeyStates.Down) && reverseBackwardVelocity)
                {
                    velocity.X += targetDirection.X * forwardBackVelocity;
                    velocity.Y += targetDirection.Y * forwardBackVelocity;
                }
                    
                if (User32.GetKeyState(Keys.S).HasFlag(User32.KeyStates.Down) && !reverseBackwardVelocity)
                {
                    velocity.X -= targetDirection.X * forwardBackVelocity;
                    velocity.Y -= targetDirection.Y * forwardBackVelocity;
                }

                if (User32.GetKeyState(Keys.NumPad8).HasFlag(User32.KeyStates.Down))
                {
                    velocity.Z += upDownVelocity;
                }

                if (User32.GetKeyState(Keys.NumPad5).HasFlag(User32.KeyStates.Down))
                {
                    velocity.Z -= upDownVelocity;
                }
                
                if (User32.GetKeyState(Keys.NumPad4).HasFlag(User32.KeyStates.Down))
                {
                    rotation.Z += 2;
                }

                if (User32.GetKeyState(Keys.NumPad6).HasFlag(User32.KeyStates.Down))
                {
                    rotation.Z -= 2;
                }
            }

            target.Velocity = velocity;
            target.Rotation = rotation;
        }
    }
}