using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Util;
using Terraria.GameContent;
using System.Collections.Generic;
using Terraria.Audio;
using LobotomyCorp.Players;
using LobotomyCorp.Buffs;
using LobotomyCorp.Projectiles.QueenLaser;

namespace LobotomyCorp.Projectiles.Realized
{
	public class StarShot2 : ModProjectile
	{
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 660;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            if (Projectile.localAI[1] == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default(Color), 1.2f);
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
                }
                Projectile.rotation += Main.rand.NextFloat(3.14f / Projectile.extraUpdates);
            }
            int dustBool = 16;

            if (Projectile.ai[0] == 1)
            {
                Projectile.scale = 0.5f;
                LobotomyWawPlayer owner = Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>();
                if (Projectile.ai[1] > 120 || owner.LoveAndHateVillain <= -1)
                    Projectile.ai[0] = 0;
                else
                {
                    if (Projectile.ai[1] > 30)
                    {
                        NPC n = Main.npc[owner.LoveAndHateVillain];
                        if (!n.active || n.life <= 0 || !n.chaseable || n.dontTakeDamage || !n.HasBuff<Villain>())
                        {
                            owner.LoveAndHateVillain = -1;
                        }
                        else
                        {
                            float length = Projectile.velocity.Length();
                            float targetAngle = Projectile.AngleTo(n.Center);
                            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(2)).ToRotationVector2() * length;
                        }
                    }
                }
            }
            else if (Projectile.ai[1] > 30)
            {
                if (!Projectile.tileCollide && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    Projectile.tileCollide = true;
            }
            Projectile.ai[1]++;

            if (Projectile.localAI[1] < 20)
                Projectile.localAI[1]++;

            if (Main.rand.NextBool(dustBool))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default(Color), 1.2f);
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
            }
            Projectile.rotation += MathHelper.ToRadians(5 / Projectile.extraUpdates) * Math.Sign(Projectile.velocity.X);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            LobotomyWawPlayer wawPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>();
            if ((wawPlayer.LoveAndHateVillain == target.whoAmI || (target.realLife > -1 && wawPlayer.LoveAndHateVillain == target.realLife)) && Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<Circle1>()] == 0)
            {
                wawPlayer.LoveAndHateArcanaCost -= 5;
                if (wawPlayer.LoveAndHateArcanaCost < 5)
                    wawPlayer.LoveAndHateArcanaCost = 5;
            }
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemTopaz, Projectile.velocity.X, Projectile.velocity.Y);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            LobotomyWawPlayer wawPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>();
            if (wawPlayer.LoveAndHateVillain == target.whoAmI || (target.realLife > -1 && wawPlayer.LoveAndHateVillain == target.realLife))
            {
                modifiers.FinalDamage *= 1.1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = tex.Frame(1, 2);
            Vector2 pos = Projectile.Center - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
            Vector2 origin = new Vector2(97, 30);
            Color color = Color.White * (Projectile.localAI[1] / 25f);
            Main.EntitySpriteDraw(tex, pos, new Rectangle(frame.X, frame.Height, frame.Width, frame.Height), color, Projectile.velocity.ToRotation(), origin, new Vector2(1f, 0.25f), 0);
            Main.EntitySpriteDraw(tex, pos, frame, Color.White, Projectile.rotation, origin, Projectile.scale, 0);

            return false;
        }
    }
}
