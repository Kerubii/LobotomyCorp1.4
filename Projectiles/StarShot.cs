using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class StarShot : ModProjectile
	{
		public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
        }

		public override void SetDefaults() {
			Projectile.width = 24;
			Projectile.height = 22;
			Projectile.aiStyle = -1;
            Projectile.alpha = 170;
			Projectile.penetrate = 1;
            Projectile.timeLeft = 660;
			Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
		}

        public override void AI()
        {
            Player projOwner = Main.player[Projectile.owner];
            if (Projectile.ai[0] < 3 && Projectile.ai[2] == 0)
            {
                foreach (Player teammate in Main.ActivePlayers)
                {
                    if (teammate.team == projOwner.team && teammate.whoAmI != projOwner.whoAmI && Projectile.getRect().Intersects(teammate.Hitbox))
                    {
                        switch (Projectile.ai[0])
                        {
                            case 0:
                                teammate.AddBuff(BuffID.Regeneration, 60);
                                break;
                            case 1:
                                teammate.AddBuff(BuffID.ManaRegeneration, 60);
                                break;
                            case 2:
                                teammate.AddBuff(BuffID.Regeneration, 30);
                                teammate.AddBuff(BuffID.ManaRegeneration, 30);
                                break;
                            case 3:
                                Projectile.ai[2] = 1;
                                teammate.Heal(5);
                                break;
                        }
                    }
                }
            }
            Projectile.rotation += 0.12f * Math.Sign(Projectile.velocity.X);
            if (Main.rand.Next(4) == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default(Color), 1.2f);
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int num107 = 0; num107 < 10; num107++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default(Color), 1.2f);
            }
            for (int num108 = 0; num108 < 3; num108++)
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, new Vector2(Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = LobotomyCorp.RedDamage;
            switch (Projectile.ai[0])
            {
                case 1:
                    color = LobotomyCorp.WhiteDamage;
                    break;
                case 2:
                    color = LobotomyCorp.BlackDamage;
                    break;
                case 3:
                    color = LobotomyCorp.PaleDamage;
                    break;
                case 4:
                    color = Color.White;
                    break;
            }
            color.A = (byte)(color.A * (float)(1f - Projectile.alpha / 255f));
            return color;
        }
        /*
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = new Rectangle(0, 0, 144, 374);
            Vector2 origin = frame.Size() / 2;
            frame.Width = frame.Width/2;
            origin.X /= 2f;

            Rectangle frame2 = frame;
            frame2.X += frame.Width;

            /*for (int i = 79; i > 0; i--)
            {
                Color color = Color.Blue;
                float n = 79 - i;
                color *= n / (120f * 1.5f);
                Vector2 oldPos = Projectile.oldPos[i];
                float rot = Projectile.oldRot[i];
                Vector2 pos = oldPos + (Projectile.Size / 2f) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
                Main.EntitySpriteDraw(tex, pos, (Rectangle?)frame2, color, rot, origin, 1f, SpriteEffects.None, 0);
            }

            float scale = 0.9f;
            Color color2 = Color.LightBlue;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + new Vector2(0,Projectile.gfxOffY), (Rectangle?)frame, color2, Projectile.rotation, origin, scale, SpriteEffects.None, 0);
            return false;
        }*/
    }
}
