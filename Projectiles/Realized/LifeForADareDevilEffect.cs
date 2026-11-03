using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.Realized
{
    public class LifeForADareDevilEffects : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/Slash2A";

        public override void SetDefaults()
        {
            Projectile.height = 12;
            Projectile.width = 12;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 15;
            Projectile.friendly = true;

            Projectile.hide = true;
        }

        public override void AI()
        {
            if (Projectile.ai[0] > 0)
            {
                if (Projectile.localAI[0] == 0)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        float rotation = 6.28f * (i / 16f);
                        Vector2 velocity = new Vector2(0, -16f * Projectile.ai[1]).RotatedBy(rotation + MathHelper.ToRadians(45));
                        Vector2 position = Projectile.Center + new Vector2(24, 0).RotatedBy(rotation);

                        Dust d = Dust.NewDustPerfect(position, 91, velocity);
                        d.noGravity = true;
                        //d.fadeIn = 1.2f;
                    }
                }
                Projectile.localAI[0]++;
            }
            else
            {
                Projectile.localAI[0]++;
                if (Projectile.localAI[1] == 0)
                {
                    Projectile.rotation = Projectile.ai[0] * -1;
                    Projectile.localAI[1]++;
                }
                if (Projectile.localAI[1] < 3)
                {
                    for (int i = -4; i <= 4; i++)
                    {
                        Vector2 position = Projectile.Center + new Vector2(-16 + 4 * i, 0).RotatedBy(Projectile.rotation);
                        Vector2 velocity = new Vector2(2 * (i / 4f) * Projectile.ai[1], 0).RotatedBy(Projectile.rotation) * Projectile.ai[1] / 384f;
                        Dust d = Dust.NewDustPerfect(position, 91, velocity);
                        d.noGravity = true;
                        d.scale = 0.5f;
                        //d.fadeIn = 1.2f;
                    }
                }
            }
        }

        public override bool? CanDamage()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 position = Projectile.Center - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
            Color color = new Color(240, 240, 255);
            if (Projectile.ai[0] > 0)
            {
                Texture2D tex = LobotomyCorp.Slash2.Value;

                Rectangle frame = tex.Frame();
                Vector2 origin = frame.Size() / 2;

                float rotation = 0;
                rotation += (-270f + 16f * Projectile.localAI[0]) * Projectile.ai[1];
                float scale = 1f + (3f * Projectile.localAI[0] / 30) * Projectile.ai[0];

                if (Projectile.localAI[0] > 7)
                {
                    color *= 1f - (Projectile.localAI[0] - 7) / 7f;
                }

                Main.EntitySpriteDraw(tex, position, frame, color, MathHelper.ToRadians(rotation), origin, scale, Projectile.ai[1] > 0 ? 0 : SpriteEffects.FlipHorizontally, 0);
            }
            else
            {
                Texture2D tex = SpearExtender.SpearTrail;

                Rectangle frame = tex.Frame();
                Vector2 origin = frame.Size() / 2;

                Vector2 scale = new Vector2(Projectile.ai[1] / 384f, 0.2f);
                scale.X *= Projectile.ai[1] / 15f;

                if (Projectile.ai[1] > 10f)
                {
                    scale.Y *= 1f * (1f - (Projectile.localAI[0] - 10) / 5f);
                }

                float rotation = -Projectile.ai[0];
                Main.EntitySpriteDraw(tex, position, frame, color, rotation, origin, scale, 0, 0);
            }

            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
    }

    public class LifeForADareDevilEffectsSlashes : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/Slash2A";

        public override void SetDefaults()
        {
            Projectile.height = 12;
            Projectile.width = 12;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.friendly = true;

            Projectile.hide = true;
        }

        public override void AI()
        {
            NPC target = Main.npc[(int)Projectile.ai[2]];
            if (target.life <= 0 || !target.active)
                Projectile.Kill();

            Projectile.position = target.position;

            if (Projectile.ai[0] <= 0 && Projectile.ai[1] > 0)
            {
                Projectile.ai[0] = 8;
                Projectile.ai[1]--;
                float mult = 1f + Projectile.localAI[0];
                Player player = Main.player[Projectile.owner];
                player.ApplyDamageToNPC(target, Projectile.damage, 0, 0, false, DamageClass.Melee);
                if (Projectile.ai[1] == 0)
                    LifeForADareDevilPierceEffect(player, target.position, (int)(target.width * (mult + 0.2f)), (int)(target.height * (mult + 0.2f)));
                else
                    LifeForADareDevilPierceEffect(player, target.position, (int)(target.width * mult), (int)(target.height * mult));

                if (Main.myPlayer == Projectile.owner)
                {
                    LobotomyCorp.ScreenShake(8, 8 * mult, 1f);
                }

                Projectile.localAI[0] += 0.1f;
            }
            if (Projectile.ai[1] == 0)
                Projectile.Kill();
            Projectile.ai[0]--;
        }

        public void LifeForADareDevilPierceEffect(Player Player, Vector2 pos, int width, int height)
        {
            if (height > width)
                width = height;

            if (width < 16)
                width = 16;

            float scale = 1f + 1.5f * (1f - Player.statLife / (float)Player.statLifeMax2);

            if (Main.myPlayer == Player.whoAmI)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, Vector2.Zero, ModContent.ProjectileType<Projectiles.Realized.LifeForADareDevilEffects>(), 0, 0, Player.whoAmI, -Main.rand.NextFloat(6.28f), width * scale);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
    }
}