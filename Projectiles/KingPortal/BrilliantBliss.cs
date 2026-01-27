using System;
using LobotomyCorp.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.KingPortal
{
	public class BrilliantBliss : ModProjectile
	{
        public static Asset<Texture2D> Broken;

        public override void Load()
        {
            Broken = ModContent.Request<Texture2D>(Texture + "Broken");
        }

        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Brilliant Bliss");
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 60;

            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }    
            
        public override void AI() {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0]++;

            }
        }
    }
}
