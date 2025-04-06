using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class FragmentsFromSomewhere : LobcorpSpear
	{
        protected override float HoldoutRangeMax => base.HoldoutRangeMax - 16;

        public override void ProjectileSpawn(int duration)
        {
            if (duration == Projectile.timeLeft && Main.player[Projectile.owner].GetModPlayer<LobotomyModPlayer>().RedMistMask)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.player[Projectile.owner].Center + Projectile.velocity * 16f, Projectile.velocity, ModContent.ProjectileType<FragmentsFromSomewhereCut>(), Projectile.damage, Projectile.knockBack, Projectile.owner, duration, 6f);
            }
        }
    }
}
