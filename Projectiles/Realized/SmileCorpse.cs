using LobotomyCorp.Items;
using LobotomyCorp.Players;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;
using static Terraria.Player;

namespace LobotomyCorp.Projectiles.Realized
{
    public class SmileCorpse : ModProjectile
    {
        public static Asset<Texture2D> Glow;

        public override void Load()
        {
            Glow = ModContent.Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Smile");
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 600;
            Projectile.scale = 1f;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // Corpse usess ai[0] to store the amount of corpse stacks it gives when eaten
            if (Projectile.localAI[0] == 0)
            {
                Projectile.frame = Main.rand.Next(3);
                Projectile.rotation = -Main.rand.NextFloat(3.14f);
                Projectile.localAI[0]++;
            }
            if ((int)Projectile.ai[2] == 0)
            {
                Projectile.velocity.Y += defaultGravity / 2;
                Projectile.velocity.X *= 0.98f;
            }
            else
                Projectile.velocity *= 0.95f;
            Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.X);
            Projectile.ai[1]++;
            /*
            Player owner = Main.player[Projectile.owner];
            if (owner.Hitbox.Intersects(Projectile.Hitbox))
            {
                owner.GetModPlayer<LobotomyAlephPlayer>().SmileMountain += (owner.statLifeMax2 / 10);

                Projectile.Kill();
            }*/
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith);
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Glow.Value;
            Vector2 pos = Projectile.Center - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
            Rectangle frame = tex.Frame(1, 3, 0, Projectile.frame);
            Vector2 origin = frame.Size() / 2;
            Color color = Color.Lerp(Color.Red, Color.DarkRed, 0.5f + 0.5f * (float)Math.Sin(6.28f * (Main.timeForVisualEffects % 120 / 120f)));
            float scale = 1f + 0.1f * (float)Math.Sin(6.28f * (Projectile.ai[1] % 90 / 90f));
            if (Projectile.owner == Main.myPlayer && Projectile.ai[1] > 30)
            {        
                Main.EntitySpriteDraw(tex, pos, frame, color, Projectile.velocity.ToRotation() + Projectile.rotation + 1.57f, origin, scale, 0, 0);
            }
            tex = TextureAssets.Projectile[Projectile.type].Value;
            color = lightColor;
            Main.EntitySpriteDraw(tex, pos, frame, color, Projectile.velocity.ToRotation() + Projectile.rotation + 1.57f, origin, scale, 0, 0);
            return false;
        }
    }
}
