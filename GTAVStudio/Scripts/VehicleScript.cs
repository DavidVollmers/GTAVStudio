using System;
using System.Linq;
using System.Windows.Forms;
using GTA;
using GTAVStudio.Common;
using GTAVStudio.Extensions;

namespace GTAVStudio.Scripts
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VehicleScript : Script
    {
        public static VehicleHash SpawnVehicleNextFrame;
        public static bool RepairVehicleNextFrame;
        public static bool VehicleInvincible;
        public static bool SpeedMode;
        public static bool StickySeatMode;

        public VehicleScript()
        {
            Tick += OnTick;
            KeyUp += OnKeyUp;
        }

        private static void OnKeyUp(object sender, KeyEventArgs e)
        {
            var speedModeShortcut = StudioSettings.GetShortcut("VehicleSpeedMode", Keys.None);
            if (speedModeShortcut != Keys.None && e.KeyData == speedModeShortcut)
            {
                OverlayScript.Overlay.VehicleSpeedMode = SpeedMode = !SpeedMode;
                return;
            }

            var stickySeatModeShortcut = StudioSettings.GetShortcut("VehicleStickySeatMode", Keys.None);
            if (stickySeatModeShortcut != Keys.None && e.KeyData == stickySeatModeShortcut)
            {
                OverlayScript.Overlay.StickySeatMode = StickySeatMode = !StickySeatMode;
                return;
            }

            var repairVehicleShortcut = StudioSettings.GetShortcut("RepairVehicle", Keys.Alt | Keys.R);
            if (repairVehicleShortcut != Keys.None && e.KeyData == repairVehicleShortcut)
            {
                RepairVehicle();
                return;
            }

            var vehicleInvincibleShortcut = StudioSettings.GetShortcut("VehicleInvincible", Keys.Alt | Keys.V);
            if (vehicleInvincibleShortcut != Keys.None && e.KeyData == vehicleInvincibleShortcut)
            {
                OverlayScript.Overlay.VehicleInvincible = VehicleInvincible = !VehicleInvincible;
                return;
            }

            var vehicleHashes = Enum.GetValues(typeof(VehicleHash)).OfType<VehicleHash>();
            foreach (var vehicleHash in vehicleHashes)
            {
                var text = Enum.GetName(typeof(VehicleHash), vehicleHash);
                var shortcut = StudioSettings.GetShortcut("SpawnVehicle_" + text, Keys.None);
                if (shortcut == Keys.None) continue;
                if (e.KeyData == shortcut)
                {
                    SpawnVehicle(vehicleHash);
                }
            }
        }

        private static void OnTick(object sender, EventArgs e)
        {
            if (SpawnVehicleNextFrame != 0)
            {
                var hash = SpawnVehicleNextFrame;
                SpawnVehicleNextFrame = 0;

                SpawnVehicle(hash);
            }

            if (RepairVehicleNextFrame)
            {
                RepairVehicleNextFrame = false;

                RepairVehicle();
            }

            Game.Player.Character.CanFlyThroughWindscreen = !StickySeatMode;
            Game.Player.Character.CanBeDraggedOutOfVehicle = !StickySeatMode;
            
            if (Game.Player.Character.IsSittingInVehicle())
            {
                Game.Player.Character.CurrentVehicle.IsInvincible = VehicleInvincible;
                if (VehicleInvincible)
                {
                    Game.Player.Character.CurrentVehicle.Repair();
                }

                if (SpeedMode)
                {
                    Game.Player.Character.CurrentVehicle.ApplySpeedModeThisFrame();
                }
            }
            else if (Game.Player.Character.LastVehicle != null)
            {
                if (Game.Player.Character.LastVehicle.IsInvincible)
                {
                    Game.Player.Character.LastVehicle.IsInvincible = false;
                }
            }
        }

        internal static void RepairVehicle()
        {
            if (Game.Player.Character.IsSittingInVehicle())
            {
                Game.Player.Character.CurrentVehicle.Repair();
            }
        }

        internal static void SpawnVehicle(VehicleHash hash)
        {
            var model = new Model(hash);

            var vehicle = World.CreateVehicle(model, Game.Player.Character.Position);
            if (vehicle == null) return;

            vehicle.Rotation = Game.Player.Character.Rotation;

            if (Game.Player.Character.IsSittingInVehicle())
            {
                vehicle.Velocity = Game.Player.Character.CurrentVehicle.Velocity;
                vehicle.Speed = Game.Player.Character.CurrentVehicle.Speed;
                Game.Player.Character.CurrentVehicle.Delete();
            }

            Game.Player.Character.SetIntoVehicle(vehicle, VehicleSeat.Driver);

            model.MarkAsNoLongerNeeded();
        }
    }
}