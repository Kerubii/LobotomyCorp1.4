using System;
using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Ruina.Natural;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.Players;
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace LobotomyCorp.Projectiles.QueenLaser
{
	public class ItnolahVisual : ModProjectile
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
            float targetRot = Projectile.rotation;
            float snapDistance = 120;
            float snapRotation = 10;

            // Controls the weapon's texture with an associated dust effect
            if (Projectile.localAI[0] == 0)
            {
                if (player.GetModPlayer<LobotomyWawPlayer>().LoveAndHateHatred)
                {
                    for (int i = 0; i < 25; i++)
                    {
                        //62 x 124
                        Dust.NewDust(Projectile.Center - new Vector2(31, 62), 62, 124, DustID.GemAmethyst);
                    }
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/MagicalGirl_ChangeFin") with { Volume = 0.2f }, Projectile.Center);
                    Projectile.localAI[0] = 1;
                }
            }
            else if (Projectile.localAI[0] == 1)
            {
                if (player.GetModPlayer<LobotomyWawPlayer>().LoveAndHateLove)
                {
                    for (int i = 0; i <= 24; i++)
                    {
                        Vector2 norm = new Vector2(1, 0).RotatedBy(Projectile.velocity.ToRotation());
                        Vector2 pos = Projectile.Center + norm * 8;
                        Vector2 vel = norm * 8;
                        Dust d = Dust.NewDustPerfect(pos, DustID.GemAmethyst, vel);
                        d.noGravity = true;
                        vel = norm * 6;
                        d = Dust.NewDustPerfect(pos, DustID.GemAmethyst, vel);
                        d.noGravity = true;
                    }
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/MagicalGirl_ChangeStart") with { Volume = 0.2f }, Projectile.Center);
                    Projectile.localAI[0] = 0;
                }
            }

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
                targetRot = -1.57f - .3f * Projectile.direction;
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
                    if (player.GetModPlayer<LobotomyWawPlayer>().LoveAndHateHatred)
                    {
                        if (Main.myPlayer == Projectile.owner)
                            Projectile.ai[2] = (Main.MouseWorld - player.MountedCenter).ToRotation();
                        Projectile.ai[0] = 4;
                    }
                    if (player.altFunctionUse == 2)
                    {
                        Projectile.ai[0]++;
                    }
                    Projectile.ai[1] = player.itemAnimation;

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

                LobotomyWawPlayer modOwner = player.GetModPlayer<LobotomyWawPlayer>();
                if (modOwner.LoveAndHateVillain > -1 && (Projectile.ai[1] == (int)(player.itemAnimationMax * 0.8f) || Projectile.ai[1] == (int)(player.itemAnimationMax * 0.5f)))
                {
                    NPC n = Main.npc[modOwner.LoveAndHateVillain];
                    if (!n.active || n.life <= 0 || !n.chaseable || n.dontTakeDamage || !n.HasBuff<Villain>())
                    {
                        modOwner.LoveAndHateVillain = -1;
                    }
                    else
                    {
                        int shotType = ModContent.ProjectileType<StarShot2>();
                        Vector2 starVel = new Vector2(8, 0).RotatedBy(Projectile.rotation);
                        int damage = (int)player.GetTotalDamage(DamageClass.Magic).ApplyTo(player.inventory[player.selectedItem].damage);
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, starVel, shotType, damage, Projectile.knockBack, Projectile.owner, 1);
                    }
                }

                targetRot = Projectile.direction > 0 ? 0 : 90;
                if (time > .3f)
                {
                    time = 1f - (time - .3f) / .7f;
                    targetRot += -130 + 270 * (float)Math.Sin(time * 1.57f) * Projectile.direction;
                    targetRot = MathHelper.ToRadians(targetRot);
                }
                else
                {
                    targetRot += -130 + 270 * Projectile.direction;
                    targetRot = MathHelper.ToRadians(targetRot);
                }

                Projectile.ai[1]--; // Attack Speed
                //player.itemTime = player.itemAnimation = player.itemAnimationMax;
                if (!player.ItemAnimationActive)
                {
                    Projectile.ai[1] = 0;
                }
                if (Projectile.ai[1] == 1 && player.channel)
                {
                    Projectile.ai[1] = player.itemAnimationMax;
                }
                else if (Projectile.ai[1] == 0)
                {
                    Projectile.ai[0] = 0;
                    //Projectile.ai[1] = player.itemAnimation = player.itemTime = (int)(player.itemAnimationMax * 1.5f);
                }
                
                snapRotation = 90;
                snapDistance = 16;
                targetPos = mountedCenter + new Vector2(120, 0).RotatedBy(targetRot);
            }
            else if (Projectile.ai[0] == 2) // Arcana Beats
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
                    Vector2 norm = Vector2.Normalize(Main.MouseWorld - mountedCenter);
                    targetPos = mountedCenter + norm * 60;
                    snapRotation = 30;
                    targetRot = norm.ToRotation() - MathHelper.ToRadians(90) * Projectile.direction;
                    if (Projectile.ai[1] > 30)
                    {
                        Projectile.rotation = targetRot;
                    }
                }

                if (player.itemAnimation == 1)
                {
                    Projectile.ai[0] = 0;
                }
            }
            else if (Projectile.ai[0] == 3) //ArcanaSlave
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
                    Vector2 norm = Vector2.Normalize(Main.MouseWorld - mountedCenter);
                    snapRotation = 30;
                    targetRot = norm.ToRotation() - MathHelper.ToRadians(90) * Projectile.direction;
                    targetPos = mountedCenter + norm * 60;
                }
            }
            // Hatred Laser
            else if (Projectile.ai[0] == 4)
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

                LobotomyWawPlayer modOwner = player.GetModPlayer<LobotomyWawPlayer>();
                if (modOwner.LoveAndHateVillain > -1 && (int)Projectile.ai[1] == player.itemAnimationMax * 2 / 3)
                {
                    NPC n = Main.npc[modOwner.LoveAndHateVillain];
                    if (!n.active || n.life <= 0 || !n.chaseable || n.dontTakeDamage || !n.HasBuff<Villain>())
                    {
                        modOwner.LoveAndHateVillain = -1;
                    }
                    else
                    {
                        Vector2 starVel = new Vector2(8, 0).RotatedBy(Projectile.ai[2] - MathHelper.ToRadians(30));
                        Vector2 starVel2 = new Vector2(8, 0).RotatedBy(Projectile.ai[2] + MathHelper.ToRadians(30));
                        int damage = (int)player.GetTotalDamage(DamageClass.Magic).ApplyTo(player.inventory[player.selectedItem].damage);
                        Vector2 pos = player.MountedCenter + new Vector2(140, 0).RotatedBy(Projectile.ai[2]);
                        Projectile.NewProjectile(player.GetSource_FromThis(), pos, starVel, ModContent.ProjectileType<StarShot2>(), damage, Projectile.knockBack, Projectile.owner, 1);
                        Projectile.NewProjectile(player.GetSource_FromThis(), pos, starVel2, ModContent.ProjectileType<StarShot2>(), damage, Projectile.knockBack, Projectile.owner, 1);
                    }
                }

                Projectile.ai[1]--;
                targetRot = Projectile.ai[2];
                targetPos = mountedCenter + new Vector2(60, 0).RotatedBy(targetRot);

                if (Projectile.ai[1] == 0)
                {
                    Projectile.ai[0] = 0;
                    //Projectile.ai[1] = player.itemAnimation = player.itemTime = (int)(player.itemAnimationMax * 1.5f);
                }
            }
            else if (Projectile.ai[0] == 5)
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
                    Vector2 norm = Vector2.Normalize(Main.MouseWorld - mountedCenter);
                    targetPos = mountedCenter + norm * 60;
                    snapRotation = 30;
                    targetRot = norm.ToRotation() - MathHelper.ToRadians(90) * Projectile.direction;
                    if (Projectile.ai[1] > 30)
                    {
                        Projectile.rotation = targetRot;
                    }
                }

                if (player.itemAnimation == 1)
                {
                    Projectile.ai[0] = 0;
                }
            }

            Vector2 delta = targetPos - Projectile.Center;
            Projectile.rotation = Terraria.Utils.AngleTowards(Projectile.rotation, targetRot, MathHelper.ToRadians(snapRotation));

            if (delta.LengthSquared() < snapDistance * snapDistance)
                Projectile.velocity = (delta * 0.5f);
            else
                Projectile.velocity = delta;

            Projectile.timeLeft = 300;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Player owner = Main.player[Projectile.owner];
            Texture2D texture = TextureAssets.Item[ModContent.ItemType<InTheNameOfLoveAndHateR>()].Value;
            if (Projectile.localAI[0] == 0)
                texture = TextureAssets.Item[ModContent.ItemType<InTheNameOfLoveAndHate>()].Value;
            float rot = Projectile.rotation;
            if (Projectile.spriteDirection < 0)
                rot -= MathHelper.ToRadians(180);
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Vector2 origin = texture.Size() / 2;
            Rectangle frame = texture.Frame();
            Vector2 scale = new Vector2(1, 1);
            Color color = lightColor * (1f - Projectile.alpha / 255f);

            if (Main.myPlayer == Projectile.owner && Main.LocalPlayer.GetModPlayer<LobotomyWawPlayer>().LoveAndHateArcanaCost <= Main.LocalPlayer.statManaMax2)
            {
                Color color2 = Color.Pink;
                color2.A = (byte)(color.A * 0.8f);
                color2 *= 0.4f;
                float scale2 = 1.05f + 0.05f * (float)Math.Sin(Main.timeForVisualEffects / 60 * 6.28f);
                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = new Vector2(4 * scale2, 0).RotatedBy(i * MathHelper.ToRadians(90));
                    Main.EntitySpriteDraw(texture, position + offset, (Rectangle?)(frame), color2, rot, origin, scale2, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
                }
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
