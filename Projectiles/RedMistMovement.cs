using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
    public class RedMistMovement : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/1stMovementMain";
        public override void SetDefaults()
		{
			Projectile.width = 250;
			Projectile.height = 250;
			Projectile.aiStyle = -1;
			Projectile.penetrate = 2;
			Projectile.scale = 1f;
			Projectile.timeLeft = 120;

			Projectile.tileCollide = false;
			Projectile.hostile = true;
		}

		public override void AI()
        {
			if (Projectile.ai[0] == 0)
			{
                Projectile.rotation = Main.rand.NextFloat(6.28f);
				Projectile.velocity *= 0;
				Projectile.alpha = 255;
				Projectile.scale = 0.7f;
			}


			Projectile.ai[0]++;


			if (Projectile.ai[0] > 60)
			{
				if (Projectile.ai[0] == 90)
				{
					for (int i = 0; i < 6; i++)
					{
						float angle = 6.28f / 6f * i;
						angle += Projectile.rotation;

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(12, 0).RotatedBy(angle), ModContent.ProjectileType<RedMistNote>(), Projectile.damage, Projectile.knockBack);
						}
					}
				}

				if (Projectile.ai[0] < 90)
				{
					Projectile.scale = 0.7f * (float)Terraria.Utils.Lerp(1, 0.25, ((double)Projectile.ai[0] - 60) / 30);

                    Projectile.rotation += MathHelper.ToRadians(8f);
                }
				else
				{
					Projectile.scale = (float)Terraria.Utils.Lerp(Projectile.scale, 1.2f * 0.7f, 0.3f);

                    Projectile.alpha += 10;
                }
            }
			else
			{
                Projectile.rotation += MathHelper.ToRadians(8f);
                if (Projectile.alpha > 50)
                    Projectile.alpha -= 5;
            }
		}

        public override bool CanHitPlayer(Player target)
        {
			return false;
        }
    }
}
