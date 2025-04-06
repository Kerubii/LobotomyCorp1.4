using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class MeltyLove : ModProjectile
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Gunk");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 300;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (Projectile.ai[1] == 0)
            {
                if (owner.channel)
                {
                    if (Projectile.ai[0] == 80)
                    {
                        owner.channel = false;
                    }

                    if (Projectile.ai[0] < 80)
                        Projectile.ai[0]++;
                    owner.heldProj = Projectile.whoAmI;
                    Projectile.alpha = 255;
                    Projectile.timeLeft = 300;
                    Projectile.Center = owner.MountedCenter;

                    if (Main.myPlayer == Projectile.owner)
                    {
                        Vector2 delta = Main.MouseWorld - owner.MountedCenter;
                        delta.Normalize();
                        Projectile.velocity = Projectile.velocity.Length() * delta;
                        owner.direction = Math.Sign(delta.X);
                    }

                    owner.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * owner.direction, Projectile.velocity.X * owner.direction);
                    owner.itemTime = owner.itemAnimation = owner.itemAnimationMax;
                    return;
                }
                for (int i = 0; i < 10; i++)
                {
                    Vector2 dustVel = Projectile.velocity / 2 * Main.rand.NextFloat(-0.5f, 1.5f);
                    dustVel = dustVel.RotatedByRandom(MathHelper.ToRadians(20));
                    Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 251, dustVel.X, dustVel.Y)].noGravity = true;
                }

                SoundEngine.PlaySound(LobotomyCorp.WeaponSound("Slime"), Projectile.position);

                float scale = Projectile.ai[0] / 80;
                if (scale > 1f)
                    scale = 1f;

                Projectile.ai[1] = 1;
                Projectile.scale = 1f + scale;
                Projectile.alpha = 0;
            }

            if (Projectile.localAI[1]++ == 2)
            {
                Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 251, -Projectile.velocity.X / 2, -Projectile.velocity.Y / 2)].noGravity = true;
                Projectile.localAI[1] = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool ShouldUpdatePosition()
        {
            Player owner = Main.player[Projectile.owner];
            return !(owner.channel && Projectile.ai[1] == 0);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 251);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Slow>(), 300);

            if (Projectile.ai[0] < 20)
                return;

            int amount = 4;
            float speed = Projectile.velocity.Length();
            if (Projectile.ai[0] > 40)
            {
                amount += (int)(4 * (Projectile.ai[0] - 40) / 40f);
                speed *= 1.8f * ((Projectile.ai[0] - 40) / 40f);
            }
            for (int i = 0; i < amount; i++)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 velRand = new Vector2(speed, 0).RotateRandom(6.28f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velRand, ModContent.ProjectileType<MeltyLoveSmall>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, target.whoAmI);
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float dmgMult = 1f + 1.5f * (Projectile.ai[0] / 80f);
            base.ModifyHitNPC(target, ref modifiers);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                tex.Frame(),
                lightColor * (1f - Projectile.alpha / 255f),
                Projectile.rotation,
                tex.Size() / 2 + new Vector2(4, 0),
                Projectile.scale,
                0,
                0);
            return false;
        }
    }

    public class MeltyLoveSmall : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/MeltyLove";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gunk");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 30;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.alpha += (int)(255 / 30);
            if (Projectile.localAI[1]++ == 2)
            {
                Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 251, -Projectile.velocity.X / 2, -Projectile.velocity.Y / 2)].noGravity = true;
                Projectile.localAI[1] = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool? CanHitNPC(NPC target)
        {
            if ((int)Projectile.ai[1] == target.whoAmI)
                return false;
            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Slow>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                tex.Frame(),
                lightColor * (1f - Projectile.alpha / 255f),
                Projectile.rotation,
                tex.Size() / 2 + new Vector2(4, 0),
                Projectile.scale,
                0,
                0);
            return false;
        }
    }
}
