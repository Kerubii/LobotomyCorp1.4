using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using LobotomyCorp.Projectiles;

namespace LobotomyCorp.NPCs.RedMist
{
    class GoldRushCrystal : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;

            Projectile.hostile = true;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.9f;
            Projectile.rotation = Projectile.ai[0] + 1.57f;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Vector2 origin = tex.Size() / 2;
            Rectangle frame = tex.Frame();
            Main.EntitySpriteDraw(tex, pos, frame, lightColor, Projectile.rotation, origin, Projectile.scale, 0, 0);

            if (Projectile.timeLeft < 15)
            {
                float scale = Projectile.scale + 1f - 0.8f * (float)Math.Sin(1.57f + 1.57f *  Projectile.timeLeft / 15f);

                Color bright = Color.White;
                bright.A = 150;
                bright *= 0.9f * (1f - Projectile.timeLeft / 15f);

                Main.EntitySpriteDraw(tex, pos, frame, bright, Projectile.rotation, origin, scale, 0, 0);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            GoldRushHold.DiamondDust(Projectile.Center, DustID.GoldCoin, 5, 8, 4, 1.2f, Projectile.ai[0]);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            { 
                Vector2 vel = new Vector2(4f, 0).RotatedBy(Projectile.ai[0]);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel * 3, ModContent.ProjectileType<GoldRushShot>(), Projectile.damage, 0);
            }
        }
    }
}
