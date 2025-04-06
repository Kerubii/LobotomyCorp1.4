using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	class GreenStem : LobcorpSpear
	{
        protected override float HoldoutRangeMin => base.HoldoutRangeMin + 16;
        protected override float HoldoutRangeMax => 128;

        public override void ProjectileSpawn(int duration)
        {
            if (Projectile.timeLeft == duration / 2 && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GreenStemTrap>(), Projectile.damage, 0f, Projectile.owner);
            }
        }
	}
}
