using System;
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

namespace LobotomyCorp.Projectiles.QueenLaser
{
	public class Circle1 : ModProjectile
	{
        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Arcana Slave");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 540;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }    
            
        public override void AI() {
            Projectile.ai[1]++;

            Projectile.rotation += MathHelper.ToRadians(3);
            if (Projectile.rotation > (float)Math.PI * 2)
                Projectile.rotation -= (float)Math.PI * 2;

            //86
            //71
            if (Projectile.timeLeft < 30)
                Projectile.alpha += 15;

            if (Projectile.ai[1] > 60)
            {
                if (Projectile.ai[1] == 61)
                {
                    if (Projectile.ai[2] == 1)
                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/MagicalGirl_LaserLoop") with { Volume = 0.2f }, Projectile.Center);
                    else
                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/MagicalGirl_Snake_LaserLoop") with { Volume = 0.2f }, Projectile.Center);
                }

                for (int i = 0; i < 2; i++)
                {
                    int type = DustID.UndergroundHallowedEnemies;
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type, Projectile.velocity.X * 2f, Projectile.velocity.Y * 2f);
                }

                for (Projectile.ai[0] = 0; Projectile.ai[0] <= 4000f; Projectile.ai[0] += 5f)
                {
                    var start = Projectile.Center + Projectile.velocity * Projectile.ai[0];
                    if (Projectile.ai[2] == 0 && Main.rand.NextBool(2000))
                    {
                        Vector2 vel = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30));
                        Gore.NewGore(Projectile.GetSource_FromThis(), start, vel, Main.rand.Next(16, 18), 1f);
                    }

                    if (Projectile.ai[0] > 600 && !Collision.CanHit(Projectile.Center, 1, 1, start, 1, 1))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Dust.NewDust(start - new Vector2 (15, 15), 30, 30, DustID.GemAmethyst, -Projectile.velocity.X * 2f, -Projectile.velocity.Y * 2f);
                        }
                        Projectile.ai[0] -= 5f;
                        break;
                    }
                }
            }
            if (Projectile.ai[1] > 380)
            {
                Projectile.ai[1] += 2;

                Projectile.rotation += MathHelper.ToRadians(8 * StrongVisualScale());
                if (Projectile.rotation > (float)Math.PI * 2)
                    Projectile.rotation -= (float)Math.PI * 2;
            }

            if (Projectile.ai[2] == 1 && Main.myPlayer == Projectile.owner)
            {
                Player owner = Main.player[Projectile.owner];
                Vector2 vec = Main.MouseWorld - owner.MountedCenter;
                vec.Normalize();
                float drag = 0.92f;
                if (Projectile.ai[1] > 380)
                    drag = 0.95f;
                vec = Vector2.Normalize(Vector2.Lerp(vec, Vector2.Normalize(Projectile.velocity), drag));
                if (vec.X != Projectile.velocity.X || vec.Y != Projectile.velocity.Y)
                {
                    Projectile.netUpdate = true;
                }
                Projectile.velocity = vec;
                Projectile.Center = owner.MountedCenter + new Vector2(160, 0).RotatedBy(vec.ToRotation());
                owner.direction = vec.X > 0 ? 1 : -1;
                owner.itemTime = owner.itemAnimation = owner.itemAnimationMax;
                owner.GetModPlayer<LobotomyWawPlayer>().LoveAndHateArcanaCooldown = 3 * 60;
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/MagicalGirl_CastEnd") with { Volume = 0.2f }, Projectile.Center);
            Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>().LoveAndHateCostReset();
            if (Projectile.ai[2] == 1)
            {
                Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>().LoveAndHateHysteriaReset();
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[1] < 60)
                return false;
            
            Vector2 unit = Projectile.velocity;
            float point = 0f;
            int width = 22;
            if (Projectile.ai[1] > 380)
                width = 62;
            // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
            // It will look for collisions on the given line using AABB
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                Projectile.Center + unit * Projectile.ai[0], width, ref point);
        }


        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = LobotomyCorp.ArcanaSlaveBackground.Value;
            float rot = Projectile.velocity.ToRotation();
            int buffer = 64;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - new Vector2(buffer * 2, 0).RotatedBy(rot);
            Vector2 origin = new Vector2(61, 61);
            Rectangle frame = new Rectangle(0, 0, 122, 122);
            Vector2 scale = new Vector2(0.8f, 1.3f) * Projectile.scale;
            Color color = Color.White * (1 - ((float)Projectile.alpha / 255));

            float mult = 1f;
            MultRange(ref mult, 0, 13);
            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color, rot, origin, (scale + new Vector2(0.03f + 0.02f * (float)Math.Sin(Projectile.rotation))) * mult, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

            texture = Mod.Assets.Request<Texture2D>("Projectiles/QueenLaser/Circle1Color").Value;
            DrawData circle = new DrawData(texture, position, frame, color, rot, origin, (scale + new Vector2(0.03f + 0.02f * (float)Math.Sin(Projectile.rotation))) * mult, SpriteEffects.None, 0);

            var rotateShader = GameShaders.Misc["LobotomyCorp:Rotate"];
            float rotateprog = Projectile.rotation / (2 * (float)Math.PI);
            rotateShader.UseOpacity(1f);
            rotateShader.UseShaderSpecificData(LobotomyCorp.ShaderRotation(rotateprog));
            //Main.NewText(Projectile.rotation / (2 * (float)Math.PI));
            rotateShader.Apply();

            Main.EntitySpriteDraw(circle);

            texture = TextureAssets.Projectile[Projectile.type].Value;
            circle = new DrawData(texture, position, (Rectangle?)(frame), color, rot, origin, mult * scale, SpriteEffects.None, 0);
            rotateShader.Apply(null);
            Main.EntitySpriteDraw(circle);

            texture = Mod.Assets.Request<Texture2D>("Projectiles/QueenLaser/Circle1Outer").Value;
            MultRange(ref mult, 6, 24);
            circle = new DrawData(texture, position, (Rectangle?)(frame), color, rot, origin, mult * scale, SpriteEffects.None, 0);

            rotateShader.UseShaderSpecificData(LobotomyCorp.ShaderRotation(1f - rotateprog));
            rotateShader.Apply(null);
                        
            Main.EntitySpriteDraw(circle);


            origin = new Vector2(63, 63);
            frame = new Rectangle(0, 0, texture.Width, texture.Height);

            position += new Vector2(buffer, 0).RotatedBy(rot);
            texture = Mod.Assets.Request<Texture2D>("Projectiles/QueenLaser/Circle2").Value;
            MultRange(ref mult, 20, 42);
            circle = new DrawData(texture, position, (Rectangle?)(frame), color, rot, origin, mult * scale, SpriteEffects.None, 0);

            rotateShader.UseShaderSpecificData(LobotomyCorp.ShaderRotation(rotateprog));
            rotateShader.Apply(null);

            Main.EntitySpriteDraw(circle);

            position += new Vector2(buffer, 0).RotatedBy(rot);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            origin = new Vector2(61, 61);
            frame = new Rectangle(0, 0, texture.Width, texture.Height);

            SpriteEffects spriteeffect = rot > 1.57f || rot < -1.57f ? SpriteEffects.FlipVertically : SpriteEffects.None;

            string Side = "R";

            texture = Mod.Assets.Request<Texture2D>("Projectiles/QueenLaser/HeartWing" + Side).Value;
            MultRange(ref mult, 38, 50);
            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color, rot, origin, (scale + new Vector2(0.05f + 0.025f * (float)Math.Cos(Projectile.rotation)) * 1.2f) * mult, spriteeffect, 0);
            texture = Mod.Assets.Request<Texture2D>("Projectiles/QueenLaser/HeartColor" + Side).Value;
            MultRange(ref mult, 40, 60);
            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color, rot, origin, mult * scale * 1.2f, spriteeffect, 0);
            texture = Mod.Assets.Request<Texture2D>("Projectiles/QueenLaser/HeartOutline" + Side).Value;
            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color, rot, origin, mult * scale * 1.2f, spriteeffect, 0);

            if (Projectile.ai[1] >= 60)
            {
                float alpha = 1f;
                float laserScale = 1f;
                if (Projectile.ai[1] < 90)
                {
                    alpha = (Projectile.ai[1] - 60f) / 30f;
                    laserScale = 0.2f + 0.8f * ((Projectile.ai[1] - 60) / 30);
                }
                else
                    laserScale += 0.05f * (float)Math.Cos(Projectile.rotation);
                laserScale *= Projectile.scale;
                float scale2 = 1f;
                if (Projectile.ai[1] > 380)
                {
                    laserScale *= 1f + 0.4f * StrongVisualScale();
                    scale2 = 1f + 0.4f * StrongVisualScale();
                }

                texture = LobotomyCorp.ArcanaSlaveBase.Value;
                Rectangle baseFrame = texture.Frame(2, 1, 0);
                int frameX = 0;
                if (Projectile.ai[2] == 1)
                {
                    frameX = 60;
                    baseFrame = texture.Frame(2, 1, 1);
                }
                Main.EntitySpriteDraw(texture, position, baseFrame, Color.White * alpha * (1 - ((float)Projectile.alpha / 255)), rot, baseFrame.Size() / 2, laserScale * 1.5f, 0, 0);

                float step = 8f * laserScale;
                texture = LobotomyCorp.ArcanaSlaveLaser.Value;

                
                for (float i = 8; i <= Projectile.ai[0]; i += step)
                {
                    Color c = Color.White;
                    origin = Projectile.Center + i * Projectile.velocity;
                    Main.EntitySpriteDraw(texture, origin - Main.screenPosition,
                        new Rectangle(frameX, 0, 60, 8), Color.White * alpha * (1 - ((float)Projectile.alpha / 255)), rot + 1.57f,
                        new Vector2(30, 4), laserScale, 0, 0);                    
                }
                Texture2D texture2 = LobotomyCorp.ArcanaSlaveLaser2.Value;
                int offset = 0 + (int)Projectile.ai[1] * 12;
                for (int i = 8; i <= Projectile.ai[0]; i += 8)
                {
                    offset -= 8;
                    origin = Projectile.Center + i * Projectile.velocity;
                    Rectangle laserFrame = new Rectangle(frameX, offset % 416, 60, 8);
                    Main.EntitySpriteDraw(texture2, origin - Main.screenPosition,
                        laserFrame, Color.White * alpha * (1 - ((float)Projectile.alpha / 255)) * laserScale * 0.8f, rot + 1.57f,
                        new Vector2(30, 4), scale2, 0, 0);
                }
            }

            Side = "L";
            origin = new Vector2(61, 61);

            texture = Mod.Assets.Request<Texture2D>("Projectiles/QueenLaser/HeartWing" + Side).Value;
            MultRange(ref mult, 38, 50);
            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color, rot, origin, (scale + new Vector2(0.05f + 0.025f * (float)Math.Cos(Projectile.rotation)) * 1.2f) * mult, spriteeffect, 0);
            texture = Mod.Assets.Request<Texture2D>("Projectiles/QueenLaser/HeartColor" + Side).Value;
            MultRange(ref mult, 40, 60);
            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color, rot, origin, mult * scale * 1.2f, spriteeffect, 0);
            texture = Mod.Assets.Request<Texture2D>("Projectiles/QueenLaser/HeartOutline" + Side).Value;
            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color, rot, origin, mult * scale * 1.2f, spriteeffect, 0);

            return false;
        }

        private float StrongVisualScale()
        {
            if (Projectile.ai[1] > 380)
            {
                float scale = (Projectile.ai[1] - 380) / 180;
                if (scale > 1f)
                    return 1f;
                return scale;
            }
            else
                return 0f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[1] > 380)
                modifiers.SourceDamage += 1f;
            LobotomyWawPlayer wawPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>();
            if (wawPlayer.LoveAndHateVillain == target.whoAmI || (target.realLife > -1 && wawPlayer.LoveAndHateVillain == target.realLife))
            {
                modifiers.FinalDamage *= 1.1f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 5;
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
