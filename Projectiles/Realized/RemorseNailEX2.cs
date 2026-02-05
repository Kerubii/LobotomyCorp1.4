using LobotomyCorp.Items.Ruina.General;
using LobotomyCorp.Players;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;
using static Terraria.Player;

namespace LobotomyCorp.Projectiles.Realized
{
    public class RemorseNailEX2 : ModProjectile
    {
        public static void RemorseOnHitNailActivate(int npc)
        {
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.type == ModContent.ProjectileType<RemorseNail>() && (int)p.ai[1] - 1 == npc)
                {
                    p.ai[2] = 1;
                }
            }
        }

        public override void SetStaticDefaults() {
        }

        public override void SetDefaults() {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 6000;

            //Projectile.hide = true;
            Projectile.DamageType = DamageClass.Default;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
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
                    Projectile.velocity = new Vector2(0, -n.height / 4);
                    Projectile.rotation = (n.Center - (Projectile.Center + Projectile.velocity)).ToRotation();
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
                Projectile.ai[1] = target.whoAmI + 1;
                Projectile.velocity = Projectile.Center - target.Center;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[2] != 0)
            {
                Vector2 front = new Vector2(1, 0).RotatedBy(Projectile.rotation);
                Projectile.velocity += front * 16;

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
                hitbox.Width += 250;
                hitbox.Height += 250;
            }            
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[1] > 0)
            {
                Player owner = Main.player[Projectile.owner];
                owner.GetModPlayer<LobotomyTethPlayer>().RemorseGainLeer(1);
                Main.npc[(int)Projectile.ai[1] - 1].GetGlobalNPC<LobotomyGlobalNPC>().RemorseGuilt = 0;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DamageVariationScale *= 0;
            modifiers.ScalingArmorPenetration += 1f;
            if (Projectile.ai[2] == 1)
            {
                modifiers.FlatBonusDamage += 120 + Main.player[Projectile.owner].GetModPlayer<LobotomyTethPlayer>().RemorseLeer;
                modifiers.DisableKnockback();
            }
            modifiers.FlatBonusDamage += target.GetGlobalNPC<LobotomyGlobalNPC>().RemorseGuilt * 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new Vector2(tex.Width - 48, tex.Height / 2);
            Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Rectangle frame = tex.Frame();
            frame.Width -= 24 + (5 - Projectile.penetrate) * 16;
            Main.EntitySpriteDraw(tex, pos, frame, lightColor, Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }
    }
}
