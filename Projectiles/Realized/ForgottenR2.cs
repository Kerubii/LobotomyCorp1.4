using System;
using System.Collections.Generic;
using LobotomyCorp.ModSystems;
using LobotomyCorp.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Player;

namespace LobotomyCorp.Projectiles.Realized
{
    public class ForgottenR2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("BiggerHug");
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 1200;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            DrawHeldProjInFrontOfHeldItemAndArms = true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            owner.heldProj = Projectile.whoAmI;

            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0]++;
                Projectile.ai[1] = -1;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            else if (Projectile.ai[0] < 30)
            {
                Projectile.ai[0]++;
            }
            else if (Projectile.ai[0] == 30)
            {
                Projectile.Kill();
            }
            else
            {
                float resistance = owner.GetModPlayer<LobotomyHePlayer>().ForgottenAffectionResistance;
                if (resistance >= 0.4f)
                {
                    owner.immune = true;
                    owner.immuneTime = 5;
                    owner.immuneNoBlink = true;
                }
                owner.AddBuff(BuffID.Slow, 2);

                Projectile.ai[0]++;
                if (Projectile.ai[0] % 30 == 0)
                {
                    if (Projectile.ai[0] % 60 == 0)
                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Teddy_Atk") with { Volume = 0.5f, PitchVariance = 1f }, Projectile.Center);
                    int dustAmount = 24;
                    for (int i = 0; i < dustAmount; i++)
                    {
                        Vector2 circle = new Vector2(8, 0).RotatedBy((float)i / dustAmount * 6.28f);
                        Dust d = Dust.NewDustPerfect(Projectile.Center + circle, 87, circle * Main.rand.NextFloat(2.00f));
                        d.noGravity = true;
                    }
                    int fluffAmount = 5;
                    for (int i = 0; i < fluffAmount; i++)
                    {
                        Vector2 circle = new Vector2(1, 0).RotatedBy(Main.rand.NextFloat(6.28f));
                        Gore g = Main.gore[Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, circle * Main.rand.NextFloat(6f), Main.rand.Next(61, 64))];
                        g.scale = 0.4f;
                    }
                    if (Main.myPlayer == Projectile.owner)
                        ModContent.GetInstance<ScreenSystem>().ScreenShake(10, 8f * resistance, 0.1f);
                }

                if (Projectile.ai[0] >= 60 + 210 * (resistance / 0.4f))
                {
                    if (Projectile.ai[1] >= 0)
                    {
                        NPC hugTarget = Main.npc[(int)Projectile.ai[1]];
                        hugTarget.velocity = Projectile.velocity * 12f;

                        int dustAmount = 24;
                        for (int i = 0; i < dustAmount; i++)
                        {
                            Vector2 circle = new Vector2(8, 0).RotatedBy((float)i / dustAmount * 6.28f);
                            Dust d = Dust.NewDustPerfect(Projectile.Center + circle, 87, circle * Main.rand.NextFloat(2.00f));
                            d.noGravity = true;
                        }
                    }

                    Projectile.Kill();
                }

                //Main.NewText(Projectile.ai[0]);
            }

            if (Projectile.ai[1] >= 0)
            {
                NPC hugTarget = Main.npc[(int)Projectile.ai[1]];
                hugTarget.position = Projectile.Center - hugTarget.Size / 2;

                if (!hugTarget.active || hugTarget.life <= 0)
                    Projectile.Kill();
            }

            owner.itemAnimation = 2;
            owner.itemTime = 2;
            Projectile.scale = 1.5f;
            Projectile.Center = owner.RotatedRelativePoint(owner.MountedCenter) + new Vector2(32 * Projectile.scale, 0).RotatedBy(Projectile.rotation);

            int slashEffect = 5;
            int inbetween = 8;
            if (Projectile.localAI[0] <= slashEffect)
            {
                if (Projectile.localAI[0] == 0)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, Projectile.velocity.X * 6, Projectile.velocity.Y * 6);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].fadeIn = 1.2f;
                    }
                }

                for (int j = -1; j <= 1; j += 2)
                {
                    float angleSlice = 3.14f / slashEffect;
                    float angle = j * (MathHelper.ToRadians(130) - angleSlice * Projectile.localAI[0]);

                    for (int i = 0; i < inbetween; i++)
                    {
                        angle += angleSlice / inbetween * j;

                        float x = (float)Math.Cos(angle) * 80f;
                        float y = (float)Math.Sin(angle) * 20f + 4f * j;

                        float scale = (i + Projectile.localAI[0] * inbetween) / (slashEffect * inbetween);
                        for (int n = -1; n <= 1; n++)
                        {
                            Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(x, y + 8 * n).RotatedBy(Projectile.velocity.ToRotation()), 87, Vector2.Zero);
                            d.noGravity = true;
                            d.scale = 0.2f + 0.6f * scale;
                        }
                    }
                }
            }
            Projectile.localAI[0]++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[1] = target.whoAmI;
            if (target.realLife >= 0)
                Projectile.ai[1] = target.realLife;
            if (Projectile.ai[0] < 30)
                Projectile.ai[0] = 31;
        }

        public override bool? CanHitNPC(NPC target)
        {
            LobotomyHePlayer modPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyHePlayer>();
            if ((target.whoAmI == modPlayer.ForgottenAffection || target.realLife >= 0 && target.realLife == modPlayer.ForgottenAffection) && (Projectile.ai[0] < 30 || Projectile.ai[0] >= 60 && Projectile.ai[0] % 60 == 0))
                return null;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] < 60)
                return;
            LobotomyHePlayer modPlayer = Main.player[Projectile.owner].GetModPlayer<LobotomyHePlayer>();
            modPlayer.ForgottenAffectionResistance = 0f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] < 60)
                return;

            modifiers.FinalDamage += 0.5f;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            Vector2 position = owner.MountedCenter + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            int frame = 1;
            if (Projectile.ai[0] >= 30 && Projectile.ai[0] % 30 < 10)
            {
                float intensity = 2f - Projectile.ai[0] % 30 / 10f;
                position.X += Main.rand.NextFloat(-1f, 1f) * intensity;
                position.Y += Main.rand.NextFloat(-1f, 1f) * intensity;
            }
            position.X -= 4 * owner.direction;
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 Origin = new Vector2(6, 6);
            if (Projectile.localAI[0] <= 5)
            {
                float armScale = 1f - Projectile.localAI[0] / 5;
                Origin -= new Vector2(8, 8) * armScale;
            }
            if (Projectile.localAI[0] <= 10) frame = 0;

            Main.EntitySpriteDraw(tex, position, tex.Frame(1, 2, 0, frame), lightColor, Projectile.rotation - 0.785f, Origin, Projectile.scale, 0, 0);
            return false;
        }
    }

    //Background arm for alternate attack
    public class ForgottenR2Behind : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/Realized/ForgottenR";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("BiggerHug");
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 1200;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.hide = true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (Projectile.ai[0] < 0)
            {
                foreach (Projectile p in Main.projectile)
                {
                    if (p.active && p.owner == Projectile.owner && p.type == ModContent.ProjectileType<ForgottenR2>())
                        Projectile.ai[0] = p.whoAmI;
                }
            }
            else
            {
                Projectile p = Main.projectile[(int)Projectile.ai[0]];
                if (!p.active || p.type != ModContent.ProjectileType<ForgottenR2>())
                    Projectile.Kill();
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.scale = 1.5f;
            Projectile.Center = owner.RotatedRelativePoint(owner.MountedCenter) + new Vector2(32 * Projectile.scale, 0).RotatedBy(Projectile.rotation);

            int slashEffect = 5;
            int inbetween = 8;
            if (Projectile.localAI[0] <= slashEffect)
            {
                if (Projectile.localAI[0] == 0)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, Projectile.velocity.X * 6, Projectile.velocity.Y * 6);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].fadeIn = 1.2f;
                    }
                }

                for (int j = -1; j <= 1; j += 2)
                {
                    float angleSlice = 3.14f / slashEffect;
                    float angle = j * (MathHelper.ToRadians(130) - angleSlice * Projectile.localAI[0]);

                    for (int i = 0; i < inbetween; i++)
                    {
                        angle += angleSlice / inbetween * j;

                        float x = (float)Math.Cos(angle) * 80f;
                        float y = (float)Math.Sin(angle) * 20f + 4f * j;

                        for (int n = -1; n <= 1; n++)
                        {
                            Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(x, y + 8 * n).RotatedBy(Projectile.velocity.ToRotation()), 87, Vector2.Zero);
                            d.noGravity = true;
                            d.scale = 0.8f;
                        }
                    }
                }
            }

            Projectile.localAI[0]++;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool CanHitPvp(Player target)
        {
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            Vector2 position = owner.MountedCenter + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            int frame = 1;

            if (Projectile.ai[0] >= 0)
            {
                float heldAI = Main.projectile[(int)Projectile.ai[0]].ai[0];
                if (heldAI >= 30 && heldAI % 30 < 10)
                {
                    float intensity = 2f - heldAI % 30 / 10f;
                    position.X += Main.rand.NextFloat(-1f, 1f) * intensity;
                    position.Y += Main.rand.NextFloat(-1f, 1f) * intensity;
                }
            }

            position.X += 4 * owner.direction;
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 Origin = new Vector2(6, 6);
            if (Projectile.localAI[0] <= 5)
            {
                float armScale = 1f - Projectile.localAI[0] / 5;
                Origin -= new Vector2(8, 8) * armScale;
            }
            if (Projectile.localAI[0] <= 10) frame = 0;

            Main.EntitySpriteDraw(tex, position, tex.Frame(1, 2, 0, frame), lightColor, Projectile.rotation - 0.785f, Origin, Projectile.scale, 0, 0);
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
    }
}
