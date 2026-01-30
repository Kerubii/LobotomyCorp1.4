using LobotomyCorp.Items.Ruina.General;
using LobotomyCorp.Players;
using LobotomyCorp.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization.Formatters;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;
using static Terraria.Player;

namespace LobotomyCorp.Projectiles.Realized
{
    public class RemorseNail : ModProjectile
    {
        public static void RemorseOnHitNailActivate(int npc)
        {
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                bool isType = p.type == ModContent.ProjectileType<RemorseNail>() || p.type == ModContent.ProjectileType<RemorseNailEX2>();
                if (isType && (int)p.ai[1] - 1 == npc)
                {
                    p.ai[2] = 1;
                }
            }
        }

        public override void SetStaticDefaults() {
        }

        public override void SetDefaults() {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 6000;

            //Projectile.hide = true;
            Projectile.DamageType = DamageClass.Default;
            Projectile.friendly = true;
            //Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = 5;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 2;
        }

        public override void AI()
        {
            if (Projectile.ai[1] > 0)
            {
                NPC n = Main.npc[(int)(Projectile.ai[1] - 1)];
                if (!n.active || n.life <= 0)
                {
                    Projectile.Kill();
                    return;
                }

                if (Projectile.ai[0] < 0)
                {
                    Projectile.ai[0] = 0;
                    Projectile.penetrate--;
                    Projectile.velocity = new Vector2(n.width / 2 - Main.rand.Next(n.width), n.height / 2 - Main.rand.Next(n.height));
                    Projectile.rotation = (n.Center - (Projectile.Center + Projectile.velocity)).ToRotation();

                    for (int i = 0; i < 14; i++)
                    {
                        Vector2 vel = new Vector2(4, 0).RotatedByRandom(3.14f);
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GemTopaz, vel);
                        dust.noGravity = true;
                    }
                }

                Projectile.Center = n.Center + Projectile.velocity;
                if (Main.myPlayer == Projectile.owner && Main.LocalPlayer.HeldItem.type != ModContent.ItemType<RemorseR>() && Projectile.timeLeft < 30)
                {
                    Projectile.timeLeft = 30;
                    Projectile.netUpdate = true;
                }
                return;
            }

            Projectile.ai[0]++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] > 30)
            {
                Projectile.velocity.Y += 0.12f;
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return Projectile.ai[1] == 0;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if ((Projectile.penetrate < 5 && Projectile.ai[2] == 0) || (Projectile.ai[1] > 0 && target.whoAmI != (int)(Projectile.ai[1] - 1)))
                return false;

            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] == 0 && Projectile.penetrate > 1)
            {
                if (LobotomyGlobalNPC.RemorseGetNailAmount(target.whoAmI) > Main.player[Projectile.owner].GetModPlayer<LobotomyTethPlayer>().RemorseLeerMax)
                    Projectile.Kill();
                Projectile.ai[1] = target.whoAmI + 1;
                Projectile.velocity = Projectile.Center - target.Center;
                Projectile.netUpdate = true;

                if (target.GetGlobalNPC<LobotomyGlobalNPC>().RemorseGuilt > 0)
                {
                    int t1 = -1, t2 = -1;
                    float maxDist = 8000;
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if (n.CanBeChasedBy(Projectile) && n.whoAmI != target.whoAmI)
                        {
                            float currdis = n.Center.Distance(target.Center);
                            if (currdis < maxDist)
                            {
                                maxDist = currdis;
                                t1 = n.whoAmI;
                            }
                        }
                    }

                    maxDist = 8000;
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if (n.CanBeChasedBy(Projectile) && n.whoAmI != target.whoAmI && n.whoAmI != t1)
                        {
                            float currdis = n.Center.Distance(target.Center);
                            if (currdis < maxDist)
                            {
                                maxDist = currdis;
                                t2 = n.whoAmI;
                            }
                        }
                    }
                    if (t1 != -1)
                        Main.player[Projectile.owner].GetModPlayer<LobotomyTethPlayer>().RemorseApplyNail(t1);
                    if (t2 != -1)
                        Main.player[Projectile.owner].GetModPlayer<LobotomyTethPlayer>().RemorseApplyNail(t2);
                }
            }
            if (Projectile.ai[2] != 0)
            {
                Vector2 front = new Vector2(1, 0).RotatedBy(Projectile.rotation);
                Projectile.velocity += front * 4f;

                for (int i = 0; i < 5; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GemTopaz, -front * i * 3);
                    d.noGravity = true;
                }
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/GeneralWorks/Slientgirl_Volt") with { Volume = 0.25f }, target.Center);
            }
            Projectile.ai[2] = 0;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (Projectile.ai[1] != 0)
            {
                hitbox.X -= 46;
                hitbox.Y -= 46;
                hitbox.Width += 100;
                hitbox.Height += 100;
            }            
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[1] > 0)
            {
                Player owner = Main.player[Projectile.owner];
                owner.GetModPlayer<LobotomyTethPlayer>().RemorseGainLeer(1);
                Main.npc[(int)Projectile.ai[1] - 1].GetGlobalNPC<LobotomyGlobalNPC>().RemorseApplyGuilt();
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DamageVariationScale *= 0;
            modifiers.ScalingArmorPenetration += 1f;
            if (Projectile.ai[2] == 1)
            {
                modifiers.FlatBonusDamage += 25;// + Main.player[Projectile.owner].GetModPlayer<LobotomyTethPlayer>().RemorseLeer;
                modifiers.DisableKnockback();
            }
            modifiers.FlatBonusDamage += target.GetGlobalNPC<LobotomyGlobalNPC>().RemorseGuilt;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new Vector2(tex.Width - 8, tex.Height / 2);
            Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Rectangle frame = tex.Frame();
            frame.Width -= (5 - Projectile.penetrate) * 4;
            Main.EntitySpriteDraw(tex, pos, frame, lightColor, Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }
    }
}
