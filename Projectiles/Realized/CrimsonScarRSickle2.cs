using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Utils;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using static tModPorter.ProgressUpdate;
using LobotomyCorp.Players;

namespace LobotomyCorp.Projectiles.Realized
{
	public class CrimsonScarRSickle2 : ModProjectile
	{
        public override string Texture => "LobotomyCorp/Projectiles/Realized/CrimsonScarRSickle";

        public override void SetStaticDefaults()
        {
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;

			Projectile.scale = 1f;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
			Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (Projectile.ai[2] > 0)
            {
                if (!Main.npc[(int)Projectile.ai[0]].active)
                {
                    Projectile.ai[2] = -1;
                    Projectile.velocity *= 0;
                }

                Projectile.Center = Main.npc[(int)Projectile.ai[0]].Center + Projectile.velocity;
            }
            else if (Projectile.ai[2] < 0)
            {
                Projectile.velocity.Y += 0.1f;
                Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.Y);
            }
            else
            {
                // Dust Spawning
                int size = 70;
                int d = Dust.NewDust(Projectile.Center - new Vector2(size / 2, size / 2), size, size, DustID.Blood, -Projectile.velocity.X, -Projectile.velocity.Y);
                Main.dust[d].noGravity = true;

                Projectile.rotation += MathHelper.ToRadians(7.5f) * Math.Sign(Projectile.velocity.X);
                Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            }

            //  ai1 gives the projectile a timer
            if (Projectile.ai[1] > 0)
            {
                Projectile.ai[1]--;
                if (Projectile.ai[1] < 20)
                {
                    Projectile.alpha += 12;
                }
                if (Projectile.ai[1] == 0)
                    Projectile.Kill();
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.whoAmI != (int)Projectile.ai[0] || Projectile.ai[2] > 0)
                return false;
            return base.CanHitNPC(target);
        }

        public override bool ShouldUpdatePosition()
        {
            return Projectile.ai[2] == 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[2]++;
            for (int i = 0; i < 10; i++)
            {
                int size = 70;
                int d = Dust.NewDust(Projectile.Center - new Vector2(size / 2, size / 2), size, size, DustID.Blood, Projectile.velocity.X/4, Projectile.velocity.Y/4);
                Main.dust[d].noGravity = true;
            }
            Projectile.velocity = Projectile.Center - target.Center;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[2]++;
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.GetGlobalNPC<LobotomyGlobalNPC>().CrimsonScarPrey)
            {
                modifiers.SourceDamage += LobotomyWawPlayer.CrimsonScarPreyBoost;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Vector2 origin = new Vector2(tex.Width / 2 - 10 * Projectile.spriteDirection, 15);
            Rectangle frame = tex.Frame();
            Texture2D tex2 = Mod.Assets.Request<Texture2D>("Projectiles/Realized/CrimsonScarRSickleBlur").Value;
            Rectangle frame2 = tex2.Frame();
            Vector2 origin2 = frame2.Size() / 2;
            lightColor *= Projectile.Opacity;
            SpriteEffects sp = (Projectile.spriteDirection > 0 ? 0 : SpriteEffects.FlipHorizontally);
            bool trail = Projectile.ai[2] > 0;
            if (!trail)
            {
                for (int i = 1; i < 30; i++)
                {
                    if (i > 2f && i % 8 != 0)
                        continue;
                    Vector2 oldPos = Projectile.oldPos[i];
                    Color trailColor = Lighting.GetColor((int)oldPos.X / 16, (int)oldPos.Y / 16) * 0.8f * (1f - i / 30f) * Projectile.Opacity;
                    oldPos += Projectile.Size / 2 - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                    float rot = Projectile.oldRot[i];
                    if (i <= 2f)
                        rot += MathHelper.ToRadians(120) * i;
                    Main.EntitySpriteDraw(tex2, oldPos, frame2, trailColor * 0.8f, rot, origin2, Projectile.scale, sp, 0);
                    Main.EntitySpriteDraw(tex, oldPos, frame, trailColor, rot, origin, Projectile.scale, sp, 0);
                }
                //Main.EntitySpriteDraw(tex2, pos, frame2, lightColor * 0.3f, Projectile.rotation + MathHelper.ToRadians(120), origin2, Projectile.scale, 0, 0);
                //Main.EntitySpriteDraw(tex2, pos, frame2, lightColor * 0.6f, Projectile.rotation + MathHelper.ToRadians(240), origin2, Projectile.scale, 0, 0);
                Main.EntitySpriteDraw(tex2, pos, frame2, lightColor * 0.8f, Projectile.rotation, origin2, Projectile.scale, sp, 0);
                //Main.EntitySpriteDraw(tex, pos, frame, lightColor * 0.3f, Projectile.rotation + MathHelper.ToRadians(120), origin, Projectile.scale, 0, 0);
                //Main.EntitySpriteDraw(tex, pos, frame, lightColor * 0.6f, Projectile.rotation + MathHelper.ToRadians(240), origin, Projectile.scale, 0, 0);
            }
            Main.EntitySpriteDraw(tex, pos, frame, lightColor, Projectile.rotation, origin, Projectile.scale, sp, 0);
            return false;
        }
    }
}
