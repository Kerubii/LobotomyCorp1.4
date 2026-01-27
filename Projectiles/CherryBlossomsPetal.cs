using System;
using LobotomyCorp.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class CherryBlossomsPetal : ModProjectile
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
            Projectile.timeLeft = 120;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
        }    
            
        public override void AI() {
            if (Projectile.ai[0] == 0)
                Projectile.rotation = Main.rand.NextFloat(MathHelper.ToRadians(360));
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 10)
                Projectile.velocity *= 0.98f;
            Projectile.rotation += MathHelper.ToRadians(4) * ((float)Projectile.velocity.Length() / 14f);
            if (Projectile.timeLeft < 50)
                Projectile.alpha += 5;

            if (Main.rand.Next(5) == 0)
            {
                Dust dust;
                dust = Main.dust[Terraria.Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 205, 0f, 0f, 0, new Color(255, 255, 255), 1f)];
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];
            if (!LobItemBase.RedMistMaskUpgrade(Main.player[Projectile.owner], RiskLevel.Teth))
                return;

            // Insert Oricalcum Petal Attack here
            float x = Main.screenPosition.X;
            if (owner.direction < 0)
                x += Main.screenWidth;
            float y = Main.screenPosition.Y;
            y += Main.rand.NextFloat(Main.screenHeight);
            Vector2 speed = target.Center - new Vector2(x, y);
            speed.Normalize();
            speed *= 24f;

            Projectile.NewProjectile(owner.GetSource_FromThis(), x, y, speed.X, speed.Y, ModContent.ProjectileType<CherryBlossomsPetal2>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust dust;
                dust = Main.dust[Terraria.Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 205, 0f, 0f, 0, new Color(255, 255, 255), 1f)];
                dust.noGravity = true;
            }
        }
    }

    public class CherryBlossomsPetal2 : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/CherryBlossomsPetal";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 4;
            Projectile.scale = 1f;
            Projectile.timeLeft = 120;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 120;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X * 0.01f;
            if (Main.rand.NextBool(5))
            {
                Dust dust;
                dust = Main.dust[Terraria.Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 205, 0f, 0f, 0, new Color(255, 255, 255), 1f)];
                dust.noGravity = true;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.penetrate == 1)
                return false;

            return base.CanHitNPC(target);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust dust;
                dust = Main.dust[Terraria.Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 205, 0f, 0f, 0, new Color(255, 255, 255), 1f)];
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.7f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            lightColor = new Color(255, 255, 255, 200);
            for (int i = 0; i < 10; i++)
            {
                float x = Projectile.velocity.X * i * 0.5f;
                float y = Projectile.velocity.Y * i * 0.5f;
                Color color = lightColor;
                float opacity = 1f - (float)i / 10f;
                if (Projectile.penetrate == 1)
                    opacity /= 2;

                color *= opacity;
                Main.EntitySpriteDraw(tex, new Vector2(Projectile.Center.X - Main.screenPosition.X + x, Projectile.Center.Y - Main.screenPosition.Y + y + Projectile.gfxOffY), tex.Frame(), color, Projectile.rotation, tex.Size() / 2, Projectile.scale, 0f);
            }

            return false;
        }
    }
}
