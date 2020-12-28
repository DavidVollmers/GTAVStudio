using System;
using System.Linq;
using System.Windows.Forms;
using GTA;
using GTA.UI;
using GTAVStudio.Common;

namespace GTAVStudio.Scripts
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VehicleScript : Script
    {
        public static VehicleHash SpawnVehicleNextFrame;
        public static bool RepairVehicleNextFrame;

        public VehicleScript()
        {
            Tick += OnTick;
            KeyUp += OnKeyUp;
        }

        private static void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == StudioSettings.GetShortcut("RepairVehicle", Keys.Alt | Keys.R))
            {
                RepairVehicle();
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