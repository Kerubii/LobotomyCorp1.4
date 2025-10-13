using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.RedMist
{
	public class OurGalaxySparkleOrbit : ModProjectile
	{
        public override string Texture => "Terraria/Images/Extra_57";

        public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.aiStyle = -1;
            //Projectile.aiStyle = 6;
            //AIType = 10;
            Projectile.penetrate = 1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 30;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] <= 0)
            {
                if (Projectile.ai[1] == 0)
                {
                    Projectile.localAI[0] = Main.rand.NextFloat(0.7f, 0.2f);
                    Projectile.scale = Projectile.localAI[0] * (1f);

                    int num954 = 10 + 10;
                    int num966 = 5;
                    for (int num977 = 0; num977 < num966; num977++)
                    {
                        Dust dust209 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, num954, Projectile.velocity.X, Projectile.velocity.Y, 50)];
                    }
                }
                Projectile.ai[1]++;
                if (Projectile.ai[0] == 0)
                {
                    Projectile.velocity *= 0.95f;

                    if (Projectile.ai[1] == 25)
                    {
                        int orbitCount = 0;
                        foreach (Projectile p in Main.ActiveProjectiles)
                        {
                            if (p.type == Projectile.type && p.ai[0] != 0 && p.whoAmI != Projectile.whoAmI && p.owner == Projectile.owner)
                                orbitCount++;
                            if (orbitCount >= 16)
                                break;
                        }
                        if (orbitCount < 16)
                        {
                            Projectile.ai[0]++;
                            Projectile.timeLeft += 1200;
                            Projectile.ai[1] = Main.rand.Next(360);
                        }
                    }
                }

                if (Projectile.timeLeft < 15)
                {
                    Projectile.scale *= 0.9f;
                }
                else
                    Projectile.scale += 0.05f;
            }
            else if (Projectile.ai[0] == 1)
            {
                if (Main.player[Projectile.owner].altFunctionUse == 2)
                {
                    Projectile.ai[0] = -1;
                    Projectile.ai[1] = 0;
                    Projectile.timeLeft = 30;
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Vector2 newVel = Main.MouseWorld - Projectile.Center;
                        newVel.Normalize();
                        newVel *= 15;
                        newVel = newVel.RotatedByRandom(MathHelper.ToRadians(30));
                        Projectile.velocity = newVel;
                    }
                    return;
                }

                Projectile.ai[1] += 4;
                float speed = 12f;
                Vector2 targetPos = Main.player[Projectile.owner].MountedCenter + new Vector2(90, 0).RotatedBy(MathHelper.ToRadians(Projectile.ai[1]));
                Vector2 vel = targetPos - Projectile.Center;
                if (vel.Length() > speed)
                {
                    vel.Normalize();
                    vel *= speed;
                }
                Projectile.velocity = vel;

                Projectile.scale = 0.6f + 0.2f * (float)Math.Sin(MathHelper.ToRadians(Projectile.ai[1] * 0.3f));
            }
            else if (Projectile.ai[0] == 2)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 60)
                {
                    Projectile.ai[1] = 0;
                    Projectile.ai[0]++;
                }
            }
            else if (Projectile.ai[0] == 3)
            {

            }
        }
    }
}
