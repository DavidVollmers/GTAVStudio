using GTA;
using GTA.Math;
using GTA.Native;

namespace GTAVStudio.Extensions
{
    public static class PedExtensions
    {
        public static Vector3 GetHeadPosition(this Ped ped)
        {
            var headPosition = ped.Position;
            try
            {
                var boneIndex = Function.Call<int>(Hash.GET_PED_BONE_INDEX, ped.Handle, Bone.SkelHead);
                foreach (var bone in ped.Bones)
                {
                    if (bone.Index != boneIndex) continue;
                    headPosition = bone.Position;
                }
            }
            catch
            {
            }

            return headPosition;
        }
    }
}