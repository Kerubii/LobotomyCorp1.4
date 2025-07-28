using System;
using System.Collections.Generic;
using LobotomyCorp.Buffs;
using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using XPT.Core.Audio.MP3Sharp.Decoding;

namespace LobotomyCorp.Projectiles.QueenLaser
{
	public class MiniLaser : ModProjectile
	{
        public static Texture2D MiniLaserTexture;

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                MiniLaserTexture = Mod.Assets.Request<Texture2D>("Projectiles/QueenLaser/MiniLaser", AssetRequestMode.ImmediateLoad).Value;

                Main.QueueMainThreadAction(() =>
                {
                    LobotomyCorp.PremultiplyTexture(MiniLaserTexture);
                });
            }
        }
        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Arcana Slave");
        }

        public override void SetDefaults()
        {
            Projectile.width = 125;
            Projectile.height = 125;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 30;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI() {
            Player owner = Main.player[Projectile.owner];

            if (Projectile.timeLeft < 10)
                Projectile.alpha += 25;

            Projectile.ai[1]++;
            if (Projectile.ai[1] > 5)
            {
                for (Projectile.ai[0] = 0; Projectile.ai[0] <= 2200f; Projectile.ai[0] += 5f)
                {
                    var start = Projectile.Center + Projectile.velocity * Projectile.ai[0];
                    if (Main.rand.NextBool(30))
                    {
                        Dust.NewDust(start - new Vector2(15, 15), 30, 30, DustID.GemAmethyst, 0, 0);
                    }

                    if (!Collision.CanHit(Projectile.Center, 1, 1, start, 1, 1))
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Dust.NewDust(start - new Vector2(15, 15), 30, 30, DustID.GemAmethyst, -Projectile.velocity.X * 2f, -Projectile.velocity.Y * 2f);
                        }
                        Projectile.ai[0] -= 5f;
                        break;
                    }
                }
            }

            Projectile.Center = owner.MountedCenter + new Vector2(140, 0).RotatedBy(Projectile.velocity.ToRotation());
            Projectile.spriteDirection = Projectile.direction = owner.direction;

            Projectile.rotation += MathHelper.ToRadians(1);
            if (Projectile.rotation > (float)Math.PI * 2)
                Projectile.rotation -= (float)Math.PI * 2;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[1] > 5)
                return base.CanHitNPC(target);
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            LobotomyWawPlayer wawPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>();
            if (wawPlayer.LoveAndHateVillain == target.whoAmI || (target.realLife > -1 && wawPlayer.LoveAndHateVillain == target.realLife))
            {
                modifiers.FinalDamage *= 1.1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = LobotomyCorp.ArcanaSlaveBackground.Value;
            float rot = Projectile.velocity.ToRotation();
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - new Vector2(20, 0).RotatedBy(rot); ;
            Vector2 origin = new Vector2(61, 61);
            Rectangle frame = new Rectangle(0, 0, 122, 122);
            Vector2 scale = new Vector2(0.5f, 1f);
            Color color = Color.White * (1 - ((float)Projectile.alpha / 255));

            float mult = 1f;
            MultRange(ref mult, 0, 5);
            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color, rot, origin, (scale + new Vector2(0.03f + 0.02f * (float)Math.Sin(Projectile.rotation))) * mult, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

            texture = Mod.Assets.Request<Texture2D>("Projectiles/QueenLaser/Circle1Color").Value;
            DrawData circle = new DrawData(texture, position, frame, color, rot, origin, (scale + new Vector2(0.03f + 0.02f * (float)Math.Sin(Projectile.rotation))) * mult, SpriteEffects.None, 0);

            var rotateShader = GameShaders.Misc["LobotomyCorp:Rotate"];
            float rotateprog = Projectile.rotation / (2 * (float)Math.PI);
            rotateShader.UseShaderSpecificData(LobotomyCorp.ShaderRotation(rotateprog));
            //Main.NewText(Projectile.rotation / (2 * (float)Math.PI));
            rotateShader.Apply();

            Main.EntitySpriteDraw(circle);

            texture = Mod.Assets.Request<Texture2D>("Projectiles/QueenLaser/Circle1").Value;
            circle = new DrawData(texture, position, (Rectangle?)(frame), color, rot, origin, mult * scale, SpriteEffects.None, 0);
            //rotateShader.Apply(null);
            Main.EntitySpriteDraw(circle);

            texture = Mod.Assets.Request<Texture2D>("Projectiles/QueenLaser/ArcanaBeatsInner").Value;
            circle = new DrawData(texture, position, (Rectangle?)(frame), color, rot, origin, mult * scale, Projectile.spriteDirection > 0 ? 0 : SpriteEffects.FlipVertically, 0);
            rotateShader.UseShaderSpecificData(LobotomyCorp.ShaderRotation(0));
            rotateShader.Apply();
            Main.EntitySpriteDraw(circle);

            texture = Mod.Assets.Request<Texture2D>("Projectiles/QueenLaser/Circle1Outer").Value;
            MultRange(ref mult, 3, 10);
            circle = new DrawData(texture, position, (Rectangle?)(frame), color, rot, origin, mult * scale, SpriteEffects.None, 0);

            rotateShader.UseShaderSpecificData(LobotomyCorp.ShaderRotation(1f - rotateprog));
            rotateShader.Apply(null);
                        
            Main.EntitySpriteDraw(circle);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            if (Projectile.ai[1] >= 5)
            {
                position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
                float alpha = 1f;
                MultRange(ref mult, 5, 10);
                Vector2 laserScale = new Vector2(mult, 1f) * Projectile.scale;

                texture = MiniLaserTexture;
                Rectangle baseFrame = new Rectangle(0, 8, 36, 40);
                Main.EntitySpriteDraw(texture, position, baseFrame, Color.White * alpha * (1 - ((float)Projectile.alpha / 255)), rot + 1.57f, baseFrame.Size() / 2, laserScale, 0, 0);

                float step = 8f * laserScale.X;
                laserScale.X *= 0.8f;
                for (float i = 8; i <= Projectile.ai[0]; i += step)
                {
                    Color c = Color.White;
                    origin = Projectile.Center + i * Projectile.velocity;
                    Main.EntitySpriteDraw(texture, origin - Main.screenPosition,
                        new Rectangle(0, 0, 36, 8), Color.White * alpha * (1 - ((float)Projectile.alpha / 255)), rot + 1.57f,
                        new Vector2(18, 4), laserScale, 0, 0);
                }
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            LobotomyWawPlayer wawPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>();
            if ((wawPlayer.LoveAndHateVillain == target.whoAmI || (target.realLife > -1 && wawPlayer.LoveAndHateVillain == target.realLife)) && Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<Circle1>()] == 0)
            {
                wawPlayer.LoveAndHateArcanaCost -= 5;
                if (wawPlayer.LoveAndHateArcanaCost < 5)
                    wawPlayer.LoveAndHateArcanaCost = 5;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = Projectile.velocity;
            float point = 0f;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + unit * Projectile.ai[0], 22, ref point);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        private void MultRange(ref float mult , float min, float max)
        {
            if (Projectile.ai[1] < min)
                mult = 0;
            else if (Projectile.ai[1] > max)
                mult = 1;
            else
                mult = (Projectile.ai[1] - min) / (max - min);
        }
    }
}
