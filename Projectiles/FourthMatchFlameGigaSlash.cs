using System;
using LobotomyCorp.ModSystems;
using LobotomyCorp.Utils;
using LobotomyCorp.Visuals.LobEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class FourthMatchFlameGigaSlash : ModProjectile
	{
		public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 126;
            Projectile.height = 126;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 31;
            Projectile.scale = 1f;
            Projectile.timeLeft = 100;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            Projectile.position.X = ownerMountedCenter.X - (float)(Projectile.width / 2);
            Projectile.position.Y = ownerMountedCenter.Y - (float)(Projectile.height / 2);
            Projectile.position += Projectile.velocity * 120;
            if (player.itemAnimation == 1)
                Projectile.Kill();

            if (Projectile.localAI[0] < 12)
            {
                if (Projectile.localAI[0] == 0)
                {
                    FourthMatchSmear ell = new FourthMatchSmear();
                    ell.Setup(Projectile, Vector2.Zero, Projectile.velocity.ToRotation(), player.itemAnimationMax, Math.Sign(player.direction));
                    LobCustomDraw.Instance().AddVEffects(ell);
                }

                Projectile.localAI[0]++;
                for (int i = 0; i < 8; i++)
                {
                    float distance = Main.rand.NextFloat(1f);
                    float rotation = Main.rand.Next(-240, 240);

                    Vector2 dustPos;
                    dustPos.X = (290 + 40 * distance) * (float)Math.Cos(rotation);
                    dustPos.Y = (45 + 10 * distance) * (float)Math.Sin(rotation);

                    Vector2 dustVel = new Vector2(3f, 0).RotatedBy(dustPos.ToRotation() - 1.57f * Math.Sign(Projectile.velocity.X));
                    dustPos = dustPos.RotatedBy(Projectile.velocity.ToRotation());


                    Dust d = Dust.NewDustPerfect(Projectile.Center + dustPos, DustID.Torch, dustVel);

                    if (Main.rand.Next(3) < 2)
                    {
                        d.fadeIn = Main.rand.NextFloat(0.8f, 1.8f);
                        d.noGravity = true;
                    }
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.penetrate <= 1)
                return false;
            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            /*
            if (target.HasBuff(ModContent.BuffType<Buffs.Matchstick>()))
                target.buffTime[target.FindBuffIndex(ModContent.BuffType<Buffs.Matchstick>())] += 300;
            else
                target.AddBuff(ModContent.BuffType<Buffs.Matchstick>(), 300);*/
            target.AddBuff(ModContent.BuffType<Buffs.Matchstick>(), 260);
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Matchstick>(), 260);
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = -1; i < 3; i++)
            {
                if (i == 0)
                    continue;

                float rotation = Projectile.velocity.ToRotation();
                Vector2 center = new Vector2(projHitbox.X, projHitbox.Y) + new Vector2(Projectile.width * i,0).RotatedBy(rotation);

                if (new Rectangle((int)center.X, (int)center.Y, Projectile.width, Projectile.height).Intersects(targetHitbox))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            /*
            Player player = Main.player[Projectile.owner];
            CustomShaderData shader = LobotomyCorp.LobcorpShaders["FourthMatchFlame"].UseOpacity(0.5f * (float)Math.Cos(3.15f * ((float)player.itemAnimation/(float)player.itemAnimationMax)) + 0.5f);

            int dir = Math.Sign(player.direction);
            SlashTrail trail = new SlashTrail(180, 45, 0);
            trail.color = Color.Red;
            float prog = (1 - (player.itemAnimation / (float)player.itemAnimationMax));
            float offset = MathHelper.ToRadians(-85 - 40 * (float)Math.Sin(1.57f * prog)) * dir;
            trail.DrawEllipse(Projectile.Center, Projectile.velocity.ToRotation(), offset, dir * -1, 400, 75, 128, shader);*/            

            return false;
        }
    }

}
