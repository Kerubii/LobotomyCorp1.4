using LobotomyCorp.Misc;
using LobotomyCorp.NPCs.RedMist;
using LobotomyCorp.Util;
using LobotomyCorp.Visuals.LobEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.RedMist
{
	public class AmritaSarira : ModProjectile
	{
		public override void SetDefaults() {
            Projectile.height = 16;
            Projectile.width = 18;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;

            Projectile.localNPCHitCooldown = 5;
            Projectile.usesLocalNPCImmunity = true;

            Projectile.timeLeft = 60 * 240;
		}

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            int order = Order();
            int max = 1 + (int)Math.Floor(owner.statLife / (owner.statLifeMax / 4f));
            if (max > 4)
                max = 4;
            if (order >= max && Projectile.ai[0] <= 0)
            {
                Projectile.ai[0] = -1;
            }

            if (Projectile.ai[0] == 0)
            {
                Projectile.velocity *= 0.95f;

                Projectile.ai[1]++;

                if (Projectile.ai[1] % 60 == 0)
                {
                    Projectile.ai[2] = 15;
                }

                if (owner.HeldItem.type != ModContent.ItemType<Items.Waw.Amrita>())
                {
                    Projectile.ai[0] = -1;
                }
                else
                {
                    if (Projectile.ai[1] > 60 && owner.heldProj > -1)
                    {
                        Projectile staff = Main.projectile[owner.heldProj];
                        if (Projectile.getRect().Intersects(staff.getRect()))
                        {
                            Projectile.ai[2] = 15;
                            Projectile.velocity = Vector2.Normalize(staff.velocity) * 16;
                            Projectile.ai[0]++;
                            Projectile.netUpdate = true;
                        }
                    }

                    if (owner.altFunctionUse == 2)
                    {
                        Projectile.ai[0] = 100;
                    }
                }
            }
            else if (Projectile.ai[0] > 0)
            {
                if (Projectile.ai[0] == 1)
                {
                    WeaponSmearLine line = new WeaponSmearLine();
                    line.Setup(Projectile, Vector2.Zero, Projectile.velocity.ToRotation(), 15, 1);
                    line.SetupLine(12, 24, -Projectile.velocity.Length() * 8);
                    line.SetShaderImage(image2: MiscAssets.WindTrail, image3: MiscAssets.Gradient);
                    line.Color = Color.White * 0.6f;
                    line.AddEffect();
                }

                if (Main.rand.NextBool(8))
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurificationPowder);
                    Main.dust[d].noGravity = true;
                }

                Projectile.ai[0]++;
                if (Projectile.ai[0] > 15)
                {
                    AIHelper.ChaseTargetAccel(Projectile.Center, owner.MountedCenter, ref Projectile.velocity, 16f, 0.1f);
                    if (Projectile.getRect().Intersects(owner.getRect()))
                    {
                        Projectile.Kill();
                    }
                    if (Projectile.ai[0] > 30)
                        Projectile.tileCollide = false;
                }
            }
            else if (Projectile.ai[0] < 0)
            {
                if (order >= max)
                    Projectile.Kill();

                if (owner.HeldItem.type == ModContent.ItemType<Items.Waw.Amrita>())
                    Projectile.ai[0] = 0;

                Projectile.velocity *= 0.95f;
            }

            if (Projectile.ai[2] > 0)
            {
                Projectile.ai[2]--;
            }
        }

        private int Order()
        {
            int count = 0;
            for (int i = Projectile.whoAmI - 1; i >= 0; i--) 
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == Projectile.type && proj.owner == Projectile.owner)
                {
                    count++;
                }
            }
            return count;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (Projectile.ai[2] > 0)
            {
                int size = (int)(200 * (1f - Projectile.ai[2] / 15f));
                hitbox = new Rectangle((int)hitbox.Center().X - size / 2, (int)hitbox.Center().Y - size / 2, size, size);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[2] > 0 || (Projectile.ai[0] > 0 && Projectile.ai[0] < 20))
                return null;
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] > 0)
            {
                modifiers.SourceDamage += 0.8f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[2] > 0)
            {
                Texture2D tex = TextureAssets.Projectile[ModContent.ProjectileType<SmileShockwave>()].Value;
                float prog = Projectile.ai[2] / 15f;
                float size = 220 * (1f - prog);
                float scale = size / tex.Width;

                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, tex.Frame(), Color.White * 0.4f * prog, 0, tex.Size() / 2, scale, 0);
            }            

            lightColor = Color.White;
            return base.PreDraw(ref lightColor);
        }
    }
}
