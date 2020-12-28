using System;
using GTA.Math;

namespace GTAVStudio.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 RotationToDirection(this Vector3 rotation)
        {
            var num = rotation.Z * 0.0174532924f;
            var num2 = rotation.X * 0.0174532924f;
            var num3 = Math.Abs((float) Math.Cos(num2));
            return new Vector3
            {
                X = (float) (-(float) Math.Sin(num) * (double) num3),
                Y = (float) ((float) Math.Cos(num) * (double) num3),
                Z = (float) Math.Sin(num2)
            };
        }
    }
}