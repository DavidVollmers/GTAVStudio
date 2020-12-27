using System;
using GTA;

namespace GTAVStudio.Scripts
{
    public class VehicleScript : Script
    {
        public static VehicleHash SpawnVehicleNextFrame;

        public VehicleScript()
        {
            Tick += OnTick;
        }

        private static void OnTick(object sender, EventArgs e)
        {
            if (SpawnVehicleNextFrame != 0)
            {
                var hash = SpawnVehicleNextFrame;
                SpawnVehicleNextFrame = 0;

                SpawnVehicle(hash);
            }
        }

        public static void SpawnVehicle(VehicleHash hash)
        {
            var model = new Model(hash);

            var vehicle = World.CreateVehicle(model, Game.Player.Character.Position);
            if (vehicle == null) return;

            vehicle.Rotation = Game.Player.Character.Rotation;

            if (Game.Player.Character.IsSittingInVehicle())
            {
                vehicle.ForwardSpeed = Game.Player.Character.CurrentVehicle.Speed;
                Game.Player.Character.CurrentVehicle.Delete();
            }

            Game.Player.Character.SetIntoVehicle(vehicle, VehicleSeat.Driver);

            model.MarkAsNoLongerNeeded();
        }
    }
}