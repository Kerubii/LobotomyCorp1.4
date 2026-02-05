using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Util;
using Terraria.GameContent;

namespace LobotomyCorp.Projectiles.Realized
{
	public class LaetitiaR : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Gift");
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.aiStyle = -1;

			Projectile.DamageType = DamageClass.Summon;
			Projectile.friendly = true;
		}

        public override void AI()
        {
			Projectile.ai[0]++;

			Projectile.rotation = 0.261f * (float)Math.Sin(6.28f * Projectile.ai[0]/60);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			LobotomyGlobalNPC modNPC = target.GetGlobalNPC<LobotomyGlobalNPC>();
			if (modNPC.LaetitiaGiftOwner < 0)
            {
				modNPC.LaetitiaGiftOwner = Projectile.owner;
				modNPC.LaetitiaGiftDamage = 0;
            }
        }
    }
}
