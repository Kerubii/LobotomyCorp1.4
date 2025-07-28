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
    public class GoldRushFlurryRAlt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.scale = 1.5f;
            Projectile.friendly = true;
            Projectile.timeLeft = 45;

            Projectile.alpha = 255;
            Projectile.hide = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0]++;
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/Greed_Stab_Change") with { Volume = 0.2f, MaxInstances = 0 }, owner.Center);
            }

            /// ai0 ai1 Used to offset hand to player
            /// After 5 ticks the fist projectile detaches to player and speeds up
            if (Projectile.ai[2] < 8)
            {
                Projectile.Center = owner.Center + Projectile.velocity * (-2 + Projectile.ai[2]);
            }
            else if (Projectile.ai[2] == 8)
            {
                Projectile.velocity *= 5;
                GoldRushHold.DiamondDust(Projectile.Center, DustID.GoldCoin, 4, 1, 5, 1, Projectile.velocity.ToRotation());
            }
            else if (Projectile.timeLeft > 15)
            {
                Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin)];
                d.noGravity = true;
                d.fadeIn = 1.5f;
                d.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) / 3f;
            }
            else
            {
                Projectile.tileCollide = true;
            }

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 65;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            Projectile.spriteDirection = Projectile.direction = owner.direction;
            Projectile.ai[2]++;
        }

        public override void Kill(int timeLeft)
        {
            int amount = 16;
            if (Projectile.timeLeft < 15)
                amount = 4;
            for (int i = 0; i < amount; i++)
            {
                Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin)];
                d.noGravity = true;
                d.fadeIn = 1.5f;
                d.velocity = Projectile.velocity / 3f;
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

            if (Projectile.ai[2] > 8)
            {
                lightColor = Color.Yellow;
                lightColor.A = (byte)(lightColor.A * 0.2f);
                if (Projectile.timeLeft < 15)
                {
                    lightColor *= 0.25f;
                }
                for (int i = 0; i < 5; i++)
                {
                    Color trailColor = lightColor * 0.8f * (1f - (i / 5f));
                    Vector2 trailPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                    Main.EntitySpriteDraw(tex, trailPos, tex.Frame(), trailColor, rotation, origin, scale, sp, 0);
                }
                Color trailColor2 = lightColor * 0.8f;
                Main.EntitySpriteDraw(tex, pos, tex.Frame(), trailColor2 * 0.2f, rotation, origin, scale * 2.5f, sp, 0);
            }

            Main.EntitySpriteDraw(tex, pos, tex.Frame(), lightColor, rotation, origin, scale, sp, 0);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit && target.life > 0 && !LobotomyGlobalNPC.LNPC(target).GoldRushBrokenBlissBuff)
            {
                Main.player[Projectile.owner].GetModPlayer<LobotomyAlephPlayer>().GoldRushApplyBliss(target);
                GoldRushHold.DiamondDust(target.position + new Vector2(target.width / 2, -40), DustID.GoldCoin, 8, 5, 5, 1);
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/Greed_MakeDiamond") with { Volume = 0.2f }, target.Center);
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
}
