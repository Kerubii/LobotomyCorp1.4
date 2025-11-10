using LobotomyCorp.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
        public override void SetStaticDefaults() {
        }

        public override void SetDefaults() {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 300;

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

                Projectile.Center = n.Center + Projectile.velocity;
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
            if (Projectile.penetrate == 1 && Projectile.ai[2] == 0 && target.whoAmI != (int)(Projectile.ai[1] - 1))
                return false;

            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] == 0 && Projectile.penetrate > 1)
            {
                Projectile.ai[1] = target.whoAmI + 1;
                Projectile.velocity = Projectile.Center - target.Center;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DamageVariationScale *= 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = tex.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Main.EntitySpriteDraw(tex, pos, tex.Frame(), lightColor, Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }
    }
}
