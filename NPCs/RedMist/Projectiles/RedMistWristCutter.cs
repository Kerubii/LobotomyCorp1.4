using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Chat;
using LobotomyCorp;
using Terraria.GameContent;
using LobotomyCorp.Buffs;


namespace LobotomyCorp.NPCs.RedMist
{
	class RedMistWristCutter : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Items/Teth/WristCutter";

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.extraUpdates = 1;

            Projectile.hostile = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
            if (Projectile.getRect().Intersects(Main.player[(int)Projectile.ai[0]].getRect()))
            {
                Main.player[(int)Projectile.ai[0]].AddBuff(ModContent.BuffType<Scars>(), 10);
            }
        }

        public override void OnKill(int timeLeft)
        {
            Main.player[(int)Projectile.ai[0]].AddBuff(ModContent.BuffType<Scars>(), 10);
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new Vector2(tex.Width - Projectile.width / 2, Projectile.height / 2);

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, tex.Frame(), lightColor, Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }
    }
}
