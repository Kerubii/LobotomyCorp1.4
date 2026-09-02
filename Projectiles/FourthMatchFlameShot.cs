using LobotomyCorp.ModSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class FourthMatchFlameShot : ModProjectile
	{
		public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 300;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
        }    
            
        public override void AI() {
            Projectile.rotation += 0.01f;
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, Projectile.velocity.X, Projectile.velocity.Y);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 180);
            target.immune[Projectile.owner] = 5;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(),Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FourthMatchFlameExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }

    public class FourthMatchFlameExplosion : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/FourthMatchFlameShot";

        public override void SetDefaults()
        {
            Projectile.width = 140;
            Projectile.height = 140;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 2;
            Projectile.alpha = 255;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.rotation += 0.01f;
            Dust dust = new Dust();
            for (int i = 0; i < 20; i++)
            {
                dust = Main.dust[Dust.NewDust(Projectile.Center, 1, 1, 31, 0, 0, 100, new Color(), 1.5f)];
                dust.velocity *= 1.4f;
            }
            for (int i = 0; i < 10; i++)
            {
                dust = Main.dust[Dust.NewDust(Projectile.Center, 1, 1, 6, 0, 0, 100, new Color(), 2.5f)];
                dust.velocity *= 5f;
                dust.noGravity = true;
                dust = Main.dust[Dust.NewDust(Projectile.Center, 1, 1, 6, 0, 0, 100, new Color(), 1.5f)];
                dust.velocity *= 3f;
            }
            Gore gore = Main.gore[Gore.NewGore(Projectile.GetSource_FromAI(),Projectile.Center, new Vector2(), Main.rand.Next(61, 64), 1f)];
            gore.velocity *= 0.4f;
            gore.velocity.X += 1f;
            gore.velocity.Y += 1f;
            gore = Main.gore[Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(), Main.rand.Next(61, 64), 1f)];
            gore.velocity *= 0.4f;
            gore.velocity.X -= 1f;
            gore.velocity.Y += 1f;
            gore = Main.gore[Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(), Main.rand.Next(61, 64), 1f)];
            gore.velocity *= 0.4f;
            gore.velocity.X += 1f;
            gore.velocity.Y -= 1f;
            gore = Main.gore[Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(), Main.rand.Next(61, 64), 1f)];
            gore.velocity *= 0.4f;
            gore.velocity.X -= 1f;
            gore.velocity.Y -= 1f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }
    }

}
