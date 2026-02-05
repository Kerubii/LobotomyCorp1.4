using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.DataStructures;

namespace LobotomyCorp.Projectiles.Realized
{
	public class RegretShock : ModProjectile
	{
		public override string Texture => "LobotomyCorp/Projectiles/SmileShockwave";

        public override void SetDefaults()
        {
            Projectile.width = 240;
            Projectile.height = 240;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.alpha = 0;
            Projectile.timeLeft = 15;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.ai[0];
            Projectile.scale = 1f - (Projectile.timeLeft / 15f);// (float)Math.Sin(1.57f * (1f - (Projectile.timeLeft / 15f)));
            Projectile.alpha = 255 - (int)(255 * (1f - (Projectile.timeLeft / 15f)));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = SmileShockwave.SmileShockwaveTex.Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Vector2 origin = tex.Size() / 2;


            float scaleFactor = (float)Math.Sin(1.57f * Projectile.scale);
            Vector2 scale = new Vector2(scaleFactor, scaleFactor) * 3f;
            Color color = Color.White;
            color *= 1f - scaleFactor;
            Main.EntitySpriteDraw(tex, position, tex.Frame(), color, Projectile.rotation, origin, scale, 0, 0);

            return false;
        }
    }
}
