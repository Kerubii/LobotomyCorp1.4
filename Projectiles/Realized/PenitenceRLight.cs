using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Util;
using Terraria.Audio;
using Terraria.GameContent;
using System.IO;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using LobotomyCorp.Players;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;

namespace LobotomyCorp.Projectiles.Realized
{
    public class PenitenceRLight : ModProjectile
    {
        public static Asset<Texture2D> PenitenceRLightTexture;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                PenitenceRLightTexture = Mod.Assets.Request<Texture2D>("Projectiles/Realized/PenitenceRLight", AssetRequestMode.ImmediateLoad);

                Main.QueueMainThreadAction(() =>
                {
                    LobotomyCorp.PremultiplyTexture(PenitenceRLightTexture.Value);
                });
            }
        }

        public override void Unload()
        {
            PenitenceRLightTexture = null;
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 90;
            Projectile.alpha = 255;

            Projectile.DamageType = DamageClass.Magic;

            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0]++;
                Projectile.localAI[0] = MathHelper.ToRadians(Main.rand.Next(360));
                Projectile.localAI[2] = Main.rand.NextFloat(0.8f, 1f);
                Projectile.rotation = MathHelper.ToRadians(90) + MathHelper.ToRadians(Main.rand.Next(-5, 6));
                Projectile.alpha = 255;
            }

            if (Projectile.timeLeft > 15)
            {
                Projectile.alpha -= 17;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            else
                Projectile.alpha += 17;
            Projectile.localAI[0] += MathHelper.ToRadians(1);

            // Scale applied to the light to make it stick at top of screen
            Projectile.localAI[1] = (Projectile.Center.Y - Main.screenPosition.Y) / 400;

            if (Main.rand.NextBool(10))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurificationPowder);
                Main.dust[d].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = PenitenceRLightTexture.Value;
            float prog = (float)Math.Sin(Projectile.localAI[0]);
            Vector2 origin = new Vector2(0, tex.Height / 2);
            Vector2 pos = new Vector2(Projectile.Center.X, Main.screenPosition.Y - 10) - Main.screenPosition;
            if (Projectile.localAI[1] < 1f)
            {
                pos.Y -= 400 - 400 * Projectile.localAI[1];
            }
            float rot = Projectile.rotation + MathHelper.ToRadians(3) * prog;
            Color color = Color.LightYellow * 0.6f * Projectile.Opacity;
            float scaleY = Projectile.localAI[2] * Projectile.Opacity;
            if (Projectile.timeLeft <= 15)
                scaleY = Projectile.localAI[2];
            Vector2 scale = new Vector2(Projectile.localAI[1], scaleY);
            if (scale.X < 1)
                scale.X = 1;

            Main.EntitySpriteDraw(tex, pos, null, color, rot, origin, scale * Projectile.scale, 0, 0);
            return false;
        }

    }
}
