using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using System;
using System.Drawing.Drawing2D;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace LobotomyCorp
{
    public class LobotomyDashPlayer : ModPlayer
    {
        // These indicate what direction is what in the timer arrays used
        public const int DashDown = 0;
        public const int DashUp = 1;
        public const int DashRight = 2;
        public const int DashLeft = 3;

        // The direction the player has double tapped.  Defaults to -1 for no dash double tap
        public int DashDir = -1;

        // The fields related to the dash accessory
        public bool SpecialDash;
        public int DashDelay = 0; // frames remaining till we can dash again
        public int DashTimer = 0; // frames remaining in the dash

        public override void ResetEffects()
        {
            // Reset our equipped flag. If the accessory is equipped somewhere, ExampleShield.UpdateAccessory will be called and set the flag before PreUpdateMovement
            SpecialDash = false;

            // ResetEffects is called not long after player.doubleTapCardinalTimer's values have been set
            // When a directional key is pressed and released, vanilla starts a 15 tick (1/4 second) timer during which a second press activates a dash
            // If the timers are set to 15, then this is the first press just processed by the vanilla logic.  Otherwise, it's a double-tap
            if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[DashDown] < 15)
            {
                DashDir = DashDown;
            }
            else if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[DashUp] < 15)
            {
                DashDir = DashUp;
            }
            else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
            {
                DashDir = DashRight;
            }
            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
            {
                DashDir = DashLeft;
            }
            else
            {
                DashDir = -1;
            }
        }

        // This is the perfect place to apply dash movement, it's after the vanilla movement code, and before the player's position is modified based on velocity.
        // If they double tapped this frame, they'll move fast this frame
        public override void PreUpdateMovement()
        {
            if (Player.GetModPlayer<LobotomyHePlayer>().GrinderMk2Active)
            {
                GrinderMK2Dash(16f, 60, 40);
                return;
            }
        }

        private void GrinderMK2Dash(float DashVelocity, int DashCooldown, int DashDuration)
        {
            //Initial Burst
            if (CanUseDash() && DashDir != -1 && DashDelay == 0)
            {
                Vector2 newVelocity = Player.velocity;
                float dashDirection = -1;

                switch (DashDir)
                {
                    case DashLeft when Player.velocity.X > -DashVelocity:
                    case DashRight when Player.velocity.X < DashVelocity:
                        {
                            dashDirection = DashDir == DashRight ? 1 : -1;
                            newVelocity.X = dashDirection * DashVelocity;
                            break;
                        }
                    default:
                        return; // not moving fast enough, so don't start our dash
                }

                // start our dash
                DashDelay = DashCooldown;
                DashTimer = DashDuration;
                Player.velocity = newVelocity;

                //Some Effects
                Point point = (Player.Center + new Vector2((float)(dashDirection * Player.width / 2 + 2), Player.gravDir * (float)(-Player.height) / 2f + Player.gravDir * 2f)).ToTileCoordinates();
                Point point2 = (Player.Center + new Vector2((float)(dashDirection * Player.width / 2 + 2), 0f)).ToTileCoordinates();
                if (WorldGen.SolidOrSlopedTile(point.X, point.Y) || WorldGen.SolidOrSlopedTile(point2.X, point2.Y))
                {
                    Player.velocity.X /= 2f;
                }
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Helper_Atk") with { Volume = 0.25f }, Player.Center);
            }

            if (DashTimer > 0)
            {
                int dir = Math.Sign(Player.velocity.X);

                Player.velocity.X = DashVelocity * dir;

                int Distance = 54;
                if (Collision.SolidTiles(Player.position + Vector2.UnitY * Player.height, Player.width, Distance + 8, true))
                {
                    Player.gravity = 0;
                    Player.velocity.Y = 0.00001f;
                    if (Collision.SolidTiles(Player.position + Vector2.UnitY * Player.height, Player.width, Distance, true))
                    {
                        Player.velocity.Y = -4f;
                    }
                }

                DashTimer--;
            }

            if (DashDelay > 0)
            {
                Player.velocity.X *= 0.98f;
                DashDelay--;
            }            
        }

        private bool CanUseDash()
        {
            return SpecialDash
                && Player.dashType == -1 // player doesn't have Tabi or EoCShield equipped (give priority to those dashes)
                && !Player.setSolar // player isn't wearing solar armor
                && !Player.mount.Active; // player isn't mounted, since dashes on a mount look weird
        }
    }
}