using Microsoft.Xna.Framework;
using Newtonsoft.Json.Serialization;
using Terraria;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class Horn : LobcorpSpear
	{
        public override void ProjectileSpawn(int duration)
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.GetModPlayer<LobotomyModPlayer>().RedMistMask)
            {
                if (Projectile.timeLeft > duration / 2)
                {
                    owner.velocity = Projectile.velocity * 16;
                }
                else if (Projectile.timeLeft == duration / 2)
                {
                    owner.velocity *= 0.3f;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.GetModPlayer<LobotomyModPlayer>().RedMistMask)
            {
                owner.immune = true;
                owner.immuneTime = Projectile.timeLeft + 5;
            }
        }
    }
}
