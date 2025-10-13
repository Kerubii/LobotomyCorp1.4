using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Intrinsics.X86;
using LobotomyCorp.Buffs;
using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.States;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using XPT.Core.Audio.MP3Sharp.Decoding;

namespace LobotomyCorp.Projectiles.RedMist
{
	public class SodaGeyser : ModProjectile
	{
        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Arcana Slave");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 35;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.localNPCHitCooldown = 35;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI() {
            if (Projectile.ai[1] < 20)
            {
                int target = -1;
                float dist = 50 * 15;
                foreach (NPC n in Main.npc)
                {
                    if (n.active && !n.dontTakeDamage && n.chaseable && Collision.CanHit(Projectile, n))
                    {
                        float length = n.Center.Distance(Projectile.Center);
                        if (length < dist)
                        {
                            target = n.whoAmI;
                            dist = length;
                        }
                    }
                }
                if (target > -1)
                {
                    Projectile.velocity = Main.npc[target].Center - Projectile.Center;
                }

                Projectile.ai[1]++;
                for (int i = 0; i < 4; i++)
                {
                    Vector2 normal = Vector2.Normalize(Projectile.velocity) * 4;
                    int d = Dust.NewDust(Projectile.position, 16, 16, DustID.GemAmethyst, normal.X, normal.Y);
                    Main.dust[d].noGravity = true;
                }
                return;
            }
            Player owner = Main.player[Projectile.owner];
            Projectile.ai[0] += 50;

            Vector2 normVel = Vector2.Normalize(Projectile.velocity);
            bool collide = false;
            for (float i = 0; i <= Projectile.ai[0]; i += 5f)
            {
                var start = Projectile.Center + normVel * i;
                if (!Collision.CanHit(Projectile.Center, 1, 1, start, 1, 1))
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Dust.NewDust(start - new Vector2(15, 15), 30, 30, DustID.GemAmethyst, -normVel.X * 2f, -normVel.Y * 2f);
                    }
                    collide = true;
                    Projectile.ai[0] = i - 5f;
                    break;
                }
            }
            if (!collide)
            {
                for (int i = 0; i < 10; i++)
                {
                    int d = Dust.NewDust(Projectile.Center + normVel * Projectile.ai[0] - new Vector2(15, 15), 30, 30, DustID.GemAmethyst, 0, 0);
                    Main.dust[d].noGravity = true;
                }
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[1] < 20)
                return false;

            Vector2 normal = Vector2.Normalize(Projectile.velocity);
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            float alpha = 1f;
            float mult = (float)Math.Min(1f, Projectile.timeLeft / 5f);
            Vector2 laserScale = new Vector2(mult, 1f) * Projectile.scale;
            float rot = Projectile.velocity.ToRotation();
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle baseFrame = texture.Frame(1, 5, 0, 1);
            Main.EntitySpriteDraw(texture, position + Projectile.ai[0] * normal, baseFrame, Color.White * alpha * (1 - ((float)Projectile.alpha / 255)), rot + 1.57f, baseFrame.Size() / 2, laserScale, 0, 0);

            float step = 16f * laserScale.X;
            laserScale.X *= 0.8f;
            for (float i = Projectile.ai[0]; i > 0; i -= step)
            {
                if (i < 0)
                {
                    i = 0;
                }
                Color c = Color.White;
                Vector2 origin = Projectile.Center + i * normal;
                Main.EntitySpriteDraw(texture, origin - Main.screenPosition,
                    texture.Frame(1, 5), Color.White * alpha * (1 - ((float)Projectile.alpha / 255)), rot + 1.57f,
                    new Vector2(8, 8), laserScale, 0, 0);
            }
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = Projectile.velocity;
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + unit * Projectile.ai[0], 22, ref point);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
}
