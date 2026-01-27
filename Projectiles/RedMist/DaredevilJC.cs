using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading.Channels;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace LobotomyCorp.Projectiles.RedMist
{
	public class DaredevilJC : ModProjectile
	{
        public override string Texture => "Terraria/Images/Extra_57";

        public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
        }

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 60;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            if (Projectile.ai[0] % 5 == 0)
            {
                //Spawns Visuals
            }
            Projectile.ai[0]++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

    public class DaredevilJCCut : ModProjectile
    {
        public override string Texture => "Terraria/Images/Extra_57";
    }
}
