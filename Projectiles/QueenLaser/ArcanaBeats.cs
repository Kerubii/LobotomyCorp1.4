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

namespace LobotomyCorp.Projectiles.QueenLaser
{
	public class ArcanaBeats : ModProjectile
	{
        public static Asset<Texture2D> Inner;
        public static Asset<Texture2D> Blast;

        public override void Load()
        {
            Inner = ModContent.Request<Texture2D>(Texture+"Inner");
            Blast = ModContent.Request<Texture2D>(Texture + "Blast2");
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
            Projectile.timeLeft = 600;

            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }

        private bool channel = false;

        public override void AI() {
            Player owner = Main.player[Projectile.owner];

            // Right click Channeling system
            if (Projectile.ai[0] == 0)
            {
                if (owner.controlUseItem && owner.altFunctionUse == 2)
                    channel = true;
                Projectile.ai[0]++;
            }
            else if (Main.myPlayer == Projectile.owner)
            {
                if (Main.mouseRightRelease || !Main.mouseRight || Projectile.timeLeft < 60)
                {
                    channel = false;
                }
            }

            if (channel)
            {
                Projectile.ai[1]++;
                owner.itemTime = owner.itemAnimation = owner.itemAnimationMax;
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 8;
                    if (Projectile.alpha < 0)
                        Projectile.alpha = 0;
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    float rot = (Main.MouseWorld - owner.Center).ToRotation();
                    Projectile.velocity = new Vector2(12, 0).RotatedBy(rot);
                    if (Projectile.velocity.X > 0)
                        owner.direction = 1;
                    else
                        owner.direction = -1;
                }
            }
            else
            {
                if (Projectile.ai[1] > 30)
                {
                    if (Projectile.ai[2] == 0)
                    {
                        if (Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>().LoveAndHateHatred)
                            SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/MagicalGirl_SnakeAtk_gun") with { Volume = 0.2f }, owner.Center);
                        else
                            SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/MagicalGirl_Gun") with { Volume = 0.2f }, owner.Center);
                    }

                    float rand = Main.rand.NextFloat(0, 1.57f);
                    for (int i = 0; i <= 24; i++)
                    {
                        Vector2 norm = new Vector2(0.5f * (float)Math.Cos(6.28f * i / 25f), 1 * (float)Math.Sin(6.28f * i / 25f)).RotatedBy(Projectile.velocity.ToRotation());
                        Vector2 pos = Projectile.Center + norm * 8;
                        Vector2 vel = norm * 8;
                        Dust d = Dust.NewDustPerfect(pos, DustID.GemAmethyst, vel);
                        d.noGravity = true;
                        vel = norm * 6;
                        d = Dust.NewDustPerfect(pos, DustID.GemAmethyst, vel);
                        d.noGravity = true;
                    }
                    Projectile.ai[1] = 29;
                    Projectile.ai[2] = 1;
                    if (owner.GetModPlayer<LobotomyWawPlayer>().LoveAndHateHatred)
                    {
                        Projectile.ai[2] = 3;
                    }
                }
                else
                {
                    if (Projectile.ai[2] > 0)
                    {
                        Projectile.frameCounter++;
                        if (Projectile.frameCounter > 3)
                        {
                            Projectile.frame++;
                            Projectile.frameCounter = 0;
                        }
                        if (Projectile.ai[1] > 20)
                        {
                            //Create a splash of Dust
                            for (int i = 0; i < 10; i++)
                            {
                                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmethyst, Projectile.velocity.X, Projectile.velocity.Y);
                                Main.dust[d].noGravity = true;
                            }
                        }
                        if (Projectile.ai[2] > 2)
                        {
                            Projectile.ai[0] += 60;

                            Vector2 normVel = Vector2.Normalize(Projectile.velocity);
                            bool collide = false;
                            for (float i = 0; i <= Projectile.ai[0]; i += 5f)
                            {
                                var start = Projectile.Center + normVel * i;
                                if (!Collision.CanHit(Projectile.Center, 1, 1, start, 1, 1))
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        Dust.NewDust(start - new Vector2(15, 15), 30, 30, DustID.GemAmethyst, -normVel.X * 2f, -normVel.Y * 2f);
                                    }
                                    collide = true;
                                    Projectile.ai[0] = i - 5f;
                                    break;
                                }
                            }
                            if (!collide)
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    Dust.NewDust(Projectile.Center + normVel * Projectile.ai[0] - new Vector2(15, 15), 30, 30, DustID.GemAmethyst, 0, 0);
                                }
                            }
                        }
                    }

                    Projectile.ai[1]--;
                    if (Projectile.ai[1] < 0)
                    {
                        Projectile.alpha += 25;
                        if (Projectile.alpha > 255)
                        {
                            Projectile.alpha = 255;
                            Projectile.Kill();
                        }
                    }
                }
            }
            Projectile.Center = owner.MountedCenter + new Vector2(73, 0).RotatedBy(Projectile.velocity.ToRotation());
            Projectile.spriteDirection = Projectile.direction = owner.direction;

            Projectile.rotation += MathHelper.ToRadians(1);
            if (Projectile.rotation > (float)Math.PI * 2)
                Projectile.rotation -= (float)Math.PI * 2;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Vector2 move = Vector2.Normalize(Projectile.velocity) * 50;
            hitbox.X += (int)move.X;
            hitbox.Y += (int)move.Y;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            LobotomyWawPlayer wawPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>();
            if (wawPlayer.LoveAndHateVillain == target.whoAmI || (target.realLife > -1 && wawPlayer.LoveAndHateVillain == target.realLife))
            {
                modifiers.FinalDamage *= 1.1f;
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            if ((Projectile.ai[1] > 20 && Projectile.ai[2] > 0) || Projectile.ai[2] > 2)
                return base.CanHitNPC(target);
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = LobotomyCorp.ArcanaSlaveBackground.Value;
            float rot = Projectile.velocity.ToRotation();
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Vector2 origin = new Vector2(61, 61);
            Rectangle frame = new Rectangle(0, 0, 122, 122);
            Vector2 scale = new Vector2(0.5f, 1f);
            Color color = Color.White * (1 - ((float)Projectile.alpha / 255));

            float mult = 1f;
            MultRange(ref mult, 0, 13);
            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color, rot, origin, (scale + new Vector2(0.03f + 0.02f * (float)Math.Sin(Projectile.rotation))) * mult, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

            texture = Circle1.Circle1Color.Value;
            DrawData circle = new DrawData(texture, position, frame, color, rot, origin, (scale + new Vector2(0.03f + 0.02f * (float)Math.Sin(Projectile.rotation))) * mult, SpriteEffects.None, 0);

            var rotateShader = GameShaders.Misc["LobotomyCorp:Rotate"];
            float rotateprog = Projectile.rotation / (2 * (float)Math.PI);
            rotateShader.UseShaderSpecificData(LobotomyCorp.ShaderRotation(rotateprog));
            rotateShader.UseOpacity(1f);
            //Main.NewText(Projectile.rotation / (2 * (float)Math.PI));
            rotateShader.Apply();

            Main.EntitySpriteDraw(circle);

            texture = TextureAssets.Projectile[Projectile.type].Value;
            circle = new DrawData(texture, position, (Rectangle?)(frame), color, rot, origin, mult * scale, SpriteEffects.None, 0);
            //rotateShader.Apply(null);
            Main.EntitySpriteDraw(circle);

            texture = Inner.Value;
            circle = new DrawData(texture, position, (Rectangle?)(frame), color, rot, origin, mult * scale, Projectile.spriteDirection > 0 ? 0 : SpriteEffects.FlipVertically, 0);
            rotateShader.UseShaderSpecificData(LobotomyCorp.ShaderRotation(0));
            rotateShader.Apply();
            Main.EntitySpriteDraw(circle);

            texture = Circle1.Circle1Outer.Value;
            MultRange(ref mult, 6, 24);
            circle = new DrawData(texture, position, (Rectangle?)(frame), color, rot, origin, mult * scale, SpriteEffects.None, 0);

            rotateShader.UseShaderSpecificData(LobotomyCorp.ShaderRotation(1f - rotateprog));
            rotateShader.Apply(null);
                        
            Main.EntitySpriteDraw(circle);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            if (Projectile.ai[2] > 2 && Projectile.ai[1] >= 5)
            {
                position = Projectile.Center - Main.screenPosition + new Vector2(20, 0).RotatedBy(rot) + new Vector2(0, Projectile.gfxOffY);
                float alpha = 1f;
                MultRange(ref mult, 0, 10);
                Vector2 laserScale = new Vector2(mult, 1f) * Projectile.scale;

                texture = MiniLaser.MiniLaserTexture;
                Rectangle baseFrame = new Rectangle(0, 8, 36, 40);
                Main.EntitySpriteDraw(texture, position, baseFrame, Color.White * alpha * (1 - ((float)Projectile.alpha / 255)), rot + 1.57f, baseFrame.Size() / 2, laserScale, 0, 0);

                float step = 8f * laserScale.X;
                laserScale.X *= 0.8f;
                for (float i = 28; i <= Projectile.ai[0]; i += step)
                {
                    Vector2 normVel = Vector2.Normalize(Projectile.velocity);
                    Color c = Color.White;
                    origin = Projectile.Center + i * normVel;
                    Main.EntitySpriteDraw(texture, origin - Main.screenPosition,
                        new Rectangle(0, 0, 36, 8), Color.White * alpha * (1 - ((float)Projectile.alpha / 255)), rot + 1.57f,
                        new Vector2(18, 4), laserScale, 0, 0);
                }
            }

            if (Projectile.ai[2] > 0 && Projectile.frame < 4)
            {
                texture = Blast.Value;
                frame = texture.Frame(1, 4, 0, Projectile.frame);
                origin = new Vector2(76, 160);
                Main.EntitySpriteDraw(texture, position, frame, color, rot + 1.57f, origin, Projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            LobotomyWawPlayer wawPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>();
            if (Projectile.ai[2] == 1 || Projectile.ai[2] == 3)
            {
                if (target.realLife > -1)
                    target = Main.npc[target.realLife];
                target.AddBuff(ModContent.BuffType<Villain>(), 6000);
                target.GetGlobalNPC<LobotomyGlobalNPC>().InTheNameOfLoveAndHateVillain = 6000;
                wawPlayer.LoveAndHateVillain = target.whoAmI;
                Projectile.ai[2]++;
            }                
            if (Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<Circle1>()] == 0)
            {
                wawPlayer.LoveAndHateArcanaCost -= 5;
                if (wawPlayer.LoveAndHateArcanaCost < 5)
                    wawPlayer.LoveAndHateArcanaCost = 5;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[2] < 3)
                return base.Colliding(projHitbox, targetHitbox);

            bool hit = false;
            if (Projectile.ai[1] > 20)
            {
                ModifyDamageHitbox(ref projHitbox);
                hit = projHitbox.Intersects(targetHitbox);
            }
            if (!hit)
            {
                Vector2 unit = Vector2.Normalize(Projectile.velocity);
                float point = 0f;
                // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
                // It will look for collisions on the given line using AABB
                hit = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                    Projectile.Center + unit * Projectile.ai[0], 22, ref point);
            }
            return hit;
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
