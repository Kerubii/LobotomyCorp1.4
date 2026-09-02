using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;

namespace LobotomyCorp.Projectiles
{
    public class CensoredTentacle : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/Censored";

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1.3f;
            Projectile.alpha = 0;
            Projectile.timeLeft = 60;

            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;

            //DrawHeldProjInFrontOfHeldItemAndArms = true;
        }

        Bezier line;

        public override void AI()
        {
            Player projOwner = Main.player[Projectile.owner];
            //Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1 + Main.rand.NextFloat(1.6f);
                line = new Bezier(Projectile.Center, Projectile.Center + Projectile.velocity);
                Projectile.ai[2] = Main.rand.NextFloat(1.6f);
                Projectile.localAI[1] = Main.rand.NextFloat(0.8f, 1.4f);
                if (Main.rand.NextBool(2))SoundEngine.PlaySound(WeaponSound("Censored2_2", true), projOwner.Center);
            }

            Projectile.Center = projOwner.MountedCenter;
            line.SetStartEnd(Projectile.Center, Projectile.Center + Projectile.velocity);
            float offsetRot = Projectile.ai[0] - 1.8f;
            Vector2 distance = Projectile.velocity / 2 * Projectile.localAI[1];
            line.CPoint1Move(Projectile.Center + distance.RotatedBy(offsetRot));
            offsetRot = Projectile.ai[2] - 0.8f;
            line.CPoint2Move(Projectile.Center + Projectile.velocity - distance.RotatedBy(offsetRot));
            //line.DustTest();
            Projectile.ai[1]++;

            if (Projectile.ai[1] > 24)
            {
                Projectile.localAI[0] -= 1f / (60 - 24f);
            }
            else
            {
                Projectile.localAI[0] = (float)Utils.Lerp(Projectile.localAI[0], 1f, 0.2f);
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }        
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            /*if (Collision.CheckAABBvLineCollision2(targetHitbox.TopLeft(), targetHitbox.Size(), ownerMountedCenter, Projectile.Center + Projectile.velocity * Projectile.localAI[0]))
                return true;*/
            float size = 50;
            if (line.Collision(targetHitbox, 0.07f, new Vector2(size, size), Projectile.localAI[0]))
            {
                return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (line == null)
                return false;

            float prog = Projectile.localAI[0];
            if (prog > 1f)
                prog = 1f;
            float currentLength = Math.Clamp(prog, 0.1f, 1f);
            int segments = 12;
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new Vector2(6, tex.Height / 2);
            float segmentLength = currentLength / (float)segments;

            for (int i = segments - 1; i >= 0; i--)
            {
                float scale = Projectile.scale * 0.66f - 0.02f * i;
                float xscale = 70 * scale;
                float point1 = segmentLength * i;
                float point2 = segmentLength * (i + 1);

                if (i != segments - 1)
                {
                    float targetScale = line.BezierPoint(point1).Distance(line.BezierPoint(point2));
                    xscale = targetScale / xscale;
                    if (xscale < scale)
                        xscale = scale;
                }
                else
                {
                    xscale = scale;
                }

                Vector2 vecScale = new Vector2(xscale, scale);
                DrawData draw = line.DrawCurveRotatedtoNext(tex, null, lightColor, vecScale, 0, origin, 0, point1, point2, RandomizeTexture());
                Main.EntitySpriteDraw(draw);
            }
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        private Vector2 RandomizeTexture()
        {
            return new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
        }
    }
}
