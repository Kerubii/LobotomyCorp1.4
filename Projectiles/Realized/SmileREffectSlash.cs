using System;
using System.Collections.Generic;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.Player;
using static tModPorter.ProgressUpdate;

namespace LobotomyCorp.Projectiles.Realized
{
    public class SmileREffectSlash : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Items/Ruina/Language/SmileR";

        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 600;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.ai[1] < Projectile.timeLeft)
            {
                Projectile.timeLeft = (int)Projectile.ai[1];
                Projectile.spriteDirection = owner.direction;
            }

            float progress = 1f - Projectile.timeLeft / Projectile.ai[1];

            Projectile.Center = owner.MountedCenter;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(-135 + 300 * (float)Math.Sin(1.57f * progress)) * owner.direction;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > Projectile.ai[1])
                return false;

            float prog = 1f - Projectile.timeLeft / Projectile.ai[1];
            float opacity = 1f;
            if (prog > 0.5f)
            {
                opacity -= ((prog - 0.5f) / 0.5f);
            }
            float length = 300 * prog;
            if (length > 140)
                length = 140;
            length = MathHelper.ToRadians(length);

            int thickness = 50 + (int)(Projectile.ai[2] / 2);
            SlashTrail trail = new SlashTrail(thickness, 1.57f);
            CustomShaderData shader = LobotomyCorp.LobcorpShaders["SwingTrail"].UseOpacity(opacity);
            shader.UseImage1(Mod, "Misc/oil");
            shader.UseImage2(Mod, "Misc/FX_Tex_Trail1");
            shader.UseImage3(Mod, "Misc/Worley");
            trail.color = lightColor;

            int radius = 145 + (int)Projectile.ai[2];
            trail.DrawPartCircle(Projectile.Center, Projectile.rotation, length, Projectile.spriteDirection, radius, 32, shader);

            shader.UseImage1(Mod, "Misc/BloodTexture");
            shader.UseImage2(Mod, "Misc/FX_Tex_Noise_Plasma2");
            shader.UseImage3(Mod, "Misc/FX_Tex_Noise_Plasma1");

            trail.DrawPartCircle(Projectile.Center, Projectile.rotation, length, Projectile.spriteDirection, radius, 32, shader);

            //Texture2D tex = Mod.Assets.Request<Texture2D>("Misc/MimicryEye").Value;


            /*
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            bool Goodbye = false;

            if (Projectile.ai[1] >= 60)
                tex = Mod.Assets.Request<Texture2D>("Items/Ruina/Language/MimicryRAlt").Value;
            float rotation = Projectile.rotation + 0.785f;
            Vector2 origin = new Vector2(8, tex.Height - 8);
            SpriteEffects sp = 0;
            if (Projectile.spriteDirection == -1)
            {
                sp = SpriteEffects.FlipHorizontally;
                origin.X = tex.Width - origin.X;
                rotation += 1.57f;
            }
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, tex.Frame(), lightColor, rotation, origin, Projectile.scale, sp);
            */
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
