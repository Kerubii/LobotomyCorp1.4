using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Utils;
using Terraria.Audio;
using Terraria.GameContent;
using System.IO;
using LobotomyCorp.Players;
using System.Collections.Generic;

namespace LobotomyCorp.Projectiles.Realized
{
    public class GoldRushUpperR : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/Realized/GoldRushFlurryR";

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.timeLeft = 45;
            Projectile.penetrate = -1;  

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
            Projectile.scale = 1.3f;
            Projectile.hide = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0]++;
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/Greed_Vert") with { Volume = 0.2f, MaxInstances = 0 }, owner.Center);
            }

            int dir = owner.direction;

            Projectile.rotation = MathHelper.ToRadians(dir > 0 ? 0 : 180) + MathHelper.ToRadians(125) * dir;
            if (Projectile.timeLeft < 40)
            {
                float time = (Projectile.timeLeft - 32f) / 8f;
                if (Projectile.timeLeft < 32f)
                {
                    Projectile.rotation += MathHelper.ToRadians(-250) * dir;
                }
                else
                {
                    Projectile.rotation -= MathHelper.ToRadians(250 * (float)Math.Sin(1.57f - 1.57f * time) * dir);
                    Vector2 norm = new Vector2(1, 0).RotatedBy(Projectile.rotation);
                    Vector2 dustPos = norm * hitboxRadius(0.5f);
                    Vector2 dustVel = norm * 3;
                    int d = Dust.NewDust(owner.MountedCenter + dustPos - new Vector2(25, 25), 50, 50, DustID.GoldCoin, dustVel.X, dustVel.Y);
                    Main.dust[d].noGravity = true;
                }
            }
            Projectile.Center = owner.RotatedRelativePoint(owner.MountedCenter) + new Vector2(45, 0).RotatedBy(Projectile.rotation);
            if (Projectile.timeLeft > 20)
            {
                owner.itemTime = Projectile.timeLeft - 20;
                owner.itemAnimation = Projectile.timeLeft - 20;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Vector2 origin = new Vector2(tex.Width - 10, 10);
            float rotation = Projectile.rotation + 0.785f;
            lightColor *= (1f - Projectile.alpha / 255f);
            float scale = Projectile.scale;
            
            SpriteEffects sp = SpriteEffects.None;
            if (Projectile.spriteDirection < 0)
            {
                sp = SpriteEffects.FlipHorizontally;
                origin.X = 10;
                rotation = Projectile.rotation + 2.356f;
            }

            if (Projectile.timeLeft < 40)
            {
                // AttackTrail
                float prog = (Projectile.timeLeft - 20f) / 20f;
                if (prog > 0)
                {
                    Player player = Main.player[Projectile.owner];
                    float opacity = (1f - Projectile.alpha / 255f);
                    if (prog < 0.3f)
                        opacity *= prog / 0.3f;
                    CustomShaderData shader = LobotomyCorp.LobcorpShaders["SwingTrail"].UseOpacity(opacity);
                    shader.UseImage1(Mod, "Misc/GreedSnippetTexture");
                    shader.UseImage2(Mod, "Misc/FistTrail");
                    shader.UseImage3(Mod, "Misc/FistTrail");
                    SlashTrail trail = new SlashTrail(30 + 30 * (1f - prog), 1.57f);
                    trail.color = Color.LightYellow * opacity;
                    float startPos = MathHelper.ToRadians(120 - 240 * (float)Math.Sin(1.57f - 1.57f * prog));
                    if (player.direction < 0)
                    {
                        startPos = MathHelper.ToRadians(60 + 240 * (float)Math.Sin(1.57f - 1.57f * prog));
                    }
                    trail.DrawPartCircle(player.MountedCenter, startPos, MathHelper.ToRadians(220 * (float)Math.Sin(1.57f - 1.57f * prog)), -player.direction, 20 + hitboxRadius(1f - prog), 24, shader);
                }
                Main.EntitySpriteDraw(tex, pos, tex.Frame(), lightColor, rotation, origin, scale, sp, 0);
                // Enlarged Fist Visual
                if (Projectile.timeLeft > 10f)
                {
                    float wave = (float)Math.Sin(1.57f * ((Projectile.timeLeft - 10f) / 30f));
                    Color trailColor = Color.Yellow * 0.8f;
                    trailColor.A = (byte)(lightColor.A * 0.2f);
                    trailColor *= wave;
                    Vector2 attackPos = pos + new Vector2(hitboxRadius(0), 0).RotatedBy(Projectile.rotation);
                    Main.EntitySpriteDraw(tex, attackPos, tex.Frame(), trailColor, rotation, tex.Frame().Size()/2, scale + 1.5f, sp, 0);
                }
            }
            else
                Main.EntitySpriteDraw(tex, pos, tex.Frame(), lightColor, rotation, origin, scale, sp, 0);
            return false;
        }

        private float hitboxRadius(float prog)
        {
            return 60 + 30 * (prog);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft > 40 || Projectile.timeLeft < 35)
                return false;
            return base.CanHitNPC(target);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Vector2 center = Main.player[Projectile.owner].MountedCenter;

            center += new Vector2(hitboxRadius(1f) + 10, 0).RotatedBy(Projectile.rotation);
            int size = 100;
            hitbox.X = (int) center.X - size / 2;
            hitbox.Y = (int) center.Y - size / 2;
            hitbox.Height = hitbox.Width = size;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit && target.life > 0 && !LobotomyGlobalNPC.LNPC(target).GoldRushBrokenBlissBuff)
            {
                if (Main.player[Projectile.owner].GetModPlayer<LobotomyAlephPlayer>().GoldRushApplyBliss(target))
                {
                    GoldRushHold.DiamondDust(target.position + new Vector2(target.width / 2, -40), DustID.GoldCoin, 8, 5, 5, 1);
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/Greed_MakeDiamond") with { Volume = 0.2f }, target.Center);
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SetCrit();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
}
