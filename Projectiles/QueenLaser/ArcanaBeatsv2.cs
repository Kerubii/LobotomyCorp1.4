using System;
using LobotomyCorp.Items.Ruina.Natural;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.QueenLaser
{
	public class ArcanaBeatsv2 : ModProjectile
	{
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 300;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }    
            
        public override void AI() {
            Projectile.localAI[1]++;
            Player player = Main.player[Projectile.owner];
            Vector2 mountedCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 targetPos = mountedCenter - new Vector2(40 * player.direction, 10 * (float)Math.Sin(MathHelper.ToRadians(1 * Projectile.localAI[1])));
            Vector2 delta = targetPos - Projectile.Center;

            if (delta.LengthSquared() > 4 * 4)
                Projectile.velocity = (delta * 0.1f);
            else
                Projectile.velocity = delta;

            if (Projectile.ai[0] != 0)
            {
                player.heldProj = Projectile.whoAmI;
            }

            Projectile.spriteDirection = Projectile.direction = player.direction;
            if (Projectile.ai[0] == 0) // Follow player from behind
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 20;
                    if (Projectile.alpha < 0)
                        Projectile.alpha = 0;
                }
                Projectile.rotation = -1.57f - .3f * Projectile.direction;
                if (player.HeldItem.type != ModContent.ItemType<InTheNameOfLoveAndHateR>())
                {
                    Projectile.Kill();
                    Projectile.alpha = 255;
                }
                if (Projectile.ai[1] > 0)
                    Projectile.ai[1] --;
                if (Projectile.ai[1] <= 0 && player.itemAnimation > 0)
                {
                    Projectile.ai[0]++;
                    Projectile.ai[1] = player.itemAnimationMax;

                    Projectile.alpha = 255;

                    for (int i = 0; i < 25; i++)
                    {
                        //62 x 124
                        Dust.NewDust(Projectile.Center - new Vector2(31, 62), 62, 124, DustID.GemAmethyst);
                    }
                }
            }
            else if (Projectile.ai[0] == 1) // Arcana Swing
            {
                if (Projectile.ai[1] < 5)
                { 
                    Projectile.alpha += 51;
                    if (Projectile.alpha > 255)
                        Projectile.alpha = 255;
                }
                else
                {
                    Projectile.alpha -= 51;
                    if (Projectile.alpha < 0)
                        Projectile.alpha = 0;
                }

                float time = Projectile.ai[1] / player.itemAnimationMax;
                Projectile.rotation = Projectile.direction > 0 ? 0 : 90;
                if (time > .3f)
                {
                    time = 1f - (time - .3f) / .7f;
                    Projectile.rotation += -130 + 270 * (float)Math.Sin(time * 1.57f) * Projectile.direction;
                    Projectile.rotation = MathHelper.ToRadians(Projectile.rotation);
                }
                else
                {
                    Projectile.rotation += -130 + 270 * Projectile.direction;
                    Projectile.rotation = MathHelper.ToRadians(Projectile.rotation);
                }

                Projectile.ai[1]--; // Attack Speed
                //player.itemTime = player.itemAnimation = player.itemAnimationMax;
                Projectile.ai[2]++; // Arcana Beats Charge

                if (Projectile.ai[1] == 1 && player.channel)
                {
                    Projectile.ai[1] = player.itemAnimationMax;
                }
                else if (Projectile.ai[1] == 0)
                {
                    Projectile.ai[0] = 0;
                    Projectile.ai[1] = player.itemAnimation = player.itemTime = (int)(player.itemAnimationMax * 1.5f);
                }
            }
            else // Arcana Beats
            {
                if (player.itemAnimation < 5)
                {
                    Projectile.alpha += 51;
                    if (Projectile.alpha > 255)
                        Projectile.alpha = 255;
                }
                else
                {
                    Projectile.alpha -= 51;
                    if (Projectile.alpha < 0)
                        Projectile.alpha = 0;
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.rotation = Vector2.Normalize(Main.MouseWorld - mountedCenter).ToRotation();
                }

                if (player.itemAnimation == 1)
                {
                    Projectile.ai[0] = 0;
                }
            }

            Projectile.timeLeft = 300;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Player owner = Main.player[Projectile.owner];
            Texture2D texture = TextureAssets.Item[ModContent.ItemType<InTheNameOfLoveAndHateR>()].Value;
            float rot = Projectile.rotation;
            if (Projectile.spriteDirection < 0)
                rot -= MathHelper.ToRadians(180);
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Vector2 origin = texture.Size() / 2;
            Rectangle frame = texture.Frame();
            Vector2 scale = new Vector2(1, 1);
            Color color = lightColor * (1f - Projectile.alpha / 255f);
            if (Projectile.ai[0] != 0)
            {
                origin = new Vector2(10, frame.Height / 2);
                if (Projectile.spriteDirection < 0)
                {
                    origin.X = frame.Width - origin.X;
                }
                Player player = Main.player[Projectile.owner];
                Vector2 mountedCenter = player.RotatedRelativePoint(player.MountedCenter, true);
                position = mountedCenter + new Vector2(60, 0).RotatedBy(Projectile.rotation) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            }
            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color, rot, origin, scale, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            

            
            /*
            texture = LobotomyCorp.ArcanaSlaveBackground.Value;
            rot = Projectile.velocity.ToRotation();
            position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            origin = new Vector2(61, 61);
            frame = new Rectangle(0, 0, 122, 122);
            scale = new Vector2(0.5f, 1f);
            color = Color.White * (1 - ((float)Projectile.alpha / 255));

            
            float mult = 1f;
            MultRange(ref mult,  10, 30);
            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color * 0.5f, rot, origin, (scale + new Vector2(0.03f + 0.02f * (float)Math.Sin(Projectile.rotation))) * mult * 0.5f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            var rotateShader = GameShaders.Misc["LobotomyCorp:Rotate"];
            rotateShader.UseShaderSpecificData(LobotomyCorp.ShaderRotation(Projectile.rotation / (2 * (float)Math.PI)));
            rotateShader.Apply(null);

            texture = Mod.Assets.Request<Texture2D>("Projectiles/QueenLaser/Circle1Color").Value;
            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color, rot, origin, (scale + new Vector2(0.03f + 0.02f * (float)Math.Sin(Projectile.rotation))) * mult, SpriteEffects.None, 0);

            texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(texture, position, (Rectangle?)(frame), color, rot, origin, mult * scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            */
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
    }
}
