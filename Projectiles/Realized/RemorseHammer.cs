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
    public class RemorseHammer : ModProjectile
    {
        public override void SetStaticDefaults() {
        }

        public override void SetDefaults() {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 6000;

            //Projectile.hide = true;
            Projectile.DamageType = DamageClass.Default;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1]++;
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Projectile.ai[1] > 30)
                {
                    Projectile.ai[1] = 0;
                    Projectile.ai[0] = 2;
                    Projectile.velocity /= 2;
                }
                return;
            }
            else if (Projectile.ai[0] == 1)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 15)
                {
                    Projectile.ai[1] = 0;
                    Projectile.ai[0]++;
                }
            }
            else if (Projectile.ai[0] == 2)
            {
                Player owner = Main.player[Projectile.owner];
                AIHelper.ChaseTargetAccel(Projectile.Center, owner.Center, ref Projectile.velocity, 14, 0.8f);
                if (Main.myPlayer == Projectile.owner)
                {
                    if (Projectile.Hitbox.Intersects(owner.Hitbox))
                    {
                        Projectile.Kill();
                    }
                }
            }
            Projectile.rotation += 0.3f * Projectile.direction;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 2;
                Projectile.velocity.X = -oldVelocity.X;
                Projectile.velocity.Y = -oldVelocity.Y;
                Ricochet(null);
            }
            Projectile.netUpdate = true;
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.velocity.X = -Projectile.velocity.X;
                Projectile.velocity.Y = -Projectile.velocity.Y;
                Projectile.netUpdate = true;
                Projectile.ai[0] = 1;
                Projectile.ai[1] = 0;
            }
            if (Projectile.ai[0] != 2)
            {
                Ricochet(target.whoAmI);
            }
            SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/GeneralWorks/Slientgirl_Hammer") with { Volume = 0.25f }, target.Center);
            RemorseNail.RemorseOnHitNailActivate(target.whoAmI);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DamageVariationScale *= 0;
            modifiers.ScalingArmorPenetration += 1f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new Vector2(tex.Width - 8, tex.Height / 2);
            Vector2 pos = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
            Main.EntitySpriteDraw(tex, pos, tex.Frame(), lightColor, Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }

        private void Ricochet(int? target)
        {
            int bounceTo = -1;
            float distance = 14 * 15;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if ((target == null || n.whoAmI != target) && n.CanBeChasedBy() && hasNail(n.whoAmI))
                {
                    float dist = n.Distance(Projectile.Center);
                    if (dist < distance)
                    {
                        bounceTo = n.whoAmI;
                        distance = dist;
                    }
                }
            }
            if (bounceTo > 0)
            {
                Vector2 dir = Projectile.Center.DirectionTo(Main.npc[bounceTo].Center);
                Projectile.velocity = dir * 14;
                Projectile.ai[0] = 1;
                Projectile.ai[1] = 0;
            }
        }

        private bool hasNail(int target)
        {
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.type == ModContent.ProjectileType<RemorseNail>() && (int)p.ai[1] - 1 == target)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
