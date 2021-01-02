using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using GTAVStudio.Common;
using GTAVStudio.Extensions;

namespace GTAVStudio.Scripts
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlayerSkillsScript : Script
    {
        public static bool DeadEye;
        private static float _scanRayPosition;
        private static bool _killSwitchThisFrame;
        private static readonly List<Ped> _pedsToKill = new List<Ped>();
        private static Ped _currentTarget;
        private static DateTime _killCooldown = DateTime.UtcNow;
        private static DateTime _lastPedKillTry = DateTime.UtcNow;

        public PlayerSkillsScript()
        {
            KeyUp += OnKeyUp;
            Tick += OnTick;
        }

        private static void OnTick(object sender, EventArgs e)
        {
            if (DeadEye)
            {
                var playerHeadPosition = Game.Player.Character.GetHeadPosition();

                var peds = World.GetNearbyPeds(Game.Player.Character.AbovePosition, 500);
                foreach (var ped in peds)
                {
                    if (ped.IsPlayer || ped.IsDead) continue;

                    var los = Function.Call<int>(Hash.HAS_ENTITY_CLEAR_LOS_TO_ENTITY_IN_FRONT,
                        Game.Player.Character.Handle, ped.Handle);

                    if (los == 0) continue;

                    var isHostile = ped.IsInCombatAgainst(Game.Player.Character);

                    var color = isHostile ? Color.Crimson : Color.LightGray;

                    var headPosition = ped.GetHeadPosition();

                    if (isHostile)
                    {
                        var hit = World.Raycast(Game.Player.Character.AbovePosition, headPosition,
                            IntersectFlags.Everything);

                        var lineColor = hit.HitEntity != null && hit.HitEntity.Handle == ped.Handle
                            ? Color.LightGray
                            : Color.SlateGray;

                        World.DrawLine(playerHeadPosition, headPosition, lineColor);

                        if (_killSwitchThisFrame && !_pedsToKill.Contains(ped))
                        {
                            _pedsToKill.Add(ped);
                        }
                    }

                    var pedDirection = ped.Rotation.RotationToDirection();
                    var markerPosition = headPosition + pedDirection * .25f;

                    World.DrawMarker(MarkerType.VerticleCircle, markerPosition, default, default,
                        new Vector3(.125f, .125f, .125f), color, false, true);
                }

                if (_currentTarget != null)
                {
                    var headPosition = _currentTarget.GetHeadPosition();

                    World.DrawLine(playerHeadPosition, headPosition, Color.Crimson);

                    if (_killCooldown.AddMilliseconds(75) < DateTime.UtcNow)
                    {
                        World.ShootBullet(Game.Player.Character.AbovePosition, headPosition,
                            Game.Player.Character, new WeaponAsset(WeaponHash.HeavySniper), _currentTarget.Health);

                        if (!_currentTarget.IsDead)
                        {
                            _pedsToKill.Add(_currentTarget);
                        }

                        _lastPedKillTry = _killCooldown = DateTime.UtcNow;
                        _currentTarget = null;
                    }
                }

                if (_currentTarget == null)
                {
                    var first = _pedsToKill.FirstOrDefault();
                    if (first != null && _lastPedKillTry.AddMilliseconds(100) < DateTime.UtcNow)
                    {
                        var headPosition = first.GetHeadPosition();

                        World.DrawLine(playerHeadPosition, headPosition, Color.Crimson);

                        _currentTarget = first;
                        _pedsToKill.Remove(first);
                        _killCooldown = DateTime.UtcNow;
                    }
                }
            }
            else
            {
                _scanRayPosition = 0;
            }

            _killSwitchThisFrame = false;
        }

        private static void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.T)
            {
                _killSwitchThisFrame = true;
            }

            var skillDeadEye = StudioSettings.GetShortcut("PlayerSkill_DeadEye", Keys.None);
            if (skillDeadEye != Keys.None && e.KeyData == skillDeadEye)
            {
                OverlayScript.Overlay.PlayerSkillDeadEye = DeadEye = !DeadEye;
                return;
            }
        }
    }
}