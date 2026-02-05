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
    public class RedMistNote : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 11;
            Projectile.height = 11;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 2;
            Projectile.scale = 1f;
            Projectile.timeLeft = 120;

            Projectile.tileCollide = false;
            Projectile.hostile = true;
        }

        public override void AI()
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = tex.Frame();
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() / 2, Projectile.scale * 0.7f, 0, 0);
            return false;
        }
    }
}
