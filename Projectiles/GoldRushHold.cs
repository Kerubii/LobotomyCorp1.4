using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class GoldRushHold : ModProjectile
	{
        public override string Texture => "LobotomyCorp/Projectiles/GoldRushPunches";

        public override void SetStaticDefaults() {
            // DisplayName.SetDefault("ORA");
        }

		public override void SetDefaults() {
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
            Projectile.timeLeft = 5;
            
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.friendly = true;

			Projectile.localNPCHitCooldown = 8;
			Projectile.usesLocalNPCImmunity = true;

            Projectile.hide = true;
			DrawHeldProjInFrontOfHeldItemAndArms = true;
		}

        public override void AI()
        {
			Player owner = Main.player[Projectile.owner];
            owner.heldProj = Projectile.whoAmI;
            Projectile.spriteDirection = owner.direction;

			if (Projectile.ai[1] == 1)
			{
                Vector2 dashSpeed = Projectile.velocity * 2f;
                owner.velocity = dashSpeed;
                owner.immune = true;
                owner.immuneTime = 10;
                owner.immuneNoBlink = true;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(owner.direction > 0 ? 0f : -180f);

                for (int i = 0; i < 3; i++)
                {
                    Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin)];
                    d.noGravity = true;
                    d.fadeIn = 0.1f;
                    d.velocity = Projectile.velocity * 0.4f;
                }
			}
            else if (Projectile.ai[1] == 2)
            {
                owner.itemTime = 5;
                owner.itemAnimation = 5;
                Projectile.rotation = MathHelper.ToRadians(-45 - (owner.direction > 0 ? 0f : -90f));

                if (Projectile.timeLeft > 20)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin)];
                        d.noGravity = true;
                        d.fadeIn = 1.2f;
                        d.velocity = -new Vector2(Projectile.velocity.Length() / 2, 0).RotatedBy(Projectile.rotation);
                    }
                }
            }
			else
			{
                if (owner.channel)
                {
                    if (Projectile.ai[0] < 300)
                        Projectile.ai[0]++;
                    if ((Projectile.ai[0] + 1) % 60 == 0)
                    {
                        Projectile.localAI[1] = 5;
                    }

                    owner.itemAnimation = 12;
                    owner.itemTime = 12;
                    Projectile.timeLeft = 12;
                    Projectile.ai[2] = -1;

                    if (Main.myPlayer == Projectile.owner)
                    {
                        float vel = Projectile.velocity.Length();
                        Vector2 dist = Vector2.Normalize(Main.MouseWorld - owner.Center);
                        dist *= vel;
                        Projectile.velocity = dist;

                        owner.direction = Math.Sign(dist.X);
                    }
                }
                else
                {
                    // Release
                    Projectile.ai[1]++;
                    Projectile.netUpdate = true;

                    SoundEngine.PlaySound(LobotomyCorp.WeaponSound("greed2"), Projectile.Center);
                    if (Projectile.ai[0] < 60 && Main.myPlayer == Projectile.owner)
                    {
                        Vector2 speed = new Vector2(4f, 0f).RotatedBy(Projectile.velocity.ToRotation());
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.Center, speed, ModContent.ProjectileType<GoldRushPunch>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        Projectile.Kill();
                        return;
                    }
                }
                Projectile.rotation = 0f;
            }

            Projectile.Center = owner.MountedCenter;
            if (Projectile.ai[1] == 1)
            {
                Projectile.Center += Projectile.velocity * 1.5f;
            }
            else if (Projectile.ai[1] == 2)
            {
                float time = 1f - ((Projectile.timeLeft - 22f) / 8f);
                if (time > 1f)
                    time = 1f;

                Projectile.Center += -new Vector2(0, 4f) + new Vector2(10f * owner.direction, -20f) * time;
            }

            if (Projectile.localAI[1] > 0)
            {
                int type = DustID.AmberBolt;
                float prog = 1f - Projectile.localAI[1] / 5f;
                float dist = 8f * prog;

                Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(dist, 0f), type, Vector2.Zero);
                d.noGravity = true;
                d = Dust.NewDustPerfect(Projectile.Center + new Vector2(-dist, 0f), type, Vector2.Zero);
                d.noGravity = true;
                d = Dust.NewDustPerfect(Projectile.Center + new Vector2(0f, dist), type, Vector2.Zero);
                d.noGravity = true;
                d = Dust.NewDustPerfect(Projectile.Center + new Vector2(0f, -dist), type, Vector2.Zero);
                d.noGravity = true;

                Projectile.localAI[1]--;
            }
		}

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (Projectile.ai[1] < 2)
                return;

            int increase = 3;
            Vector2 Center = hitbox.TopLeft() + hitbox.Size() / 2f;
            Center -= new Vector2(hitbox.Width * increase / 2f, hitbox.Height * increase / 2f);
            hitbox.X = (int)Center.X;
            hitbox.Y = (int)Center.Y;
            hitbox.Width *= increase;
            hitbox.Height *= increase;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[1] == 0)
                return false;
            if (Projectile.ai[1] == 2 && (Projectile.timeLeft < 28 && Projectile.timeLeft >= 20))
                return false;
            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] == 2)
            {
                Main.player[Projectile.owner].immune = true;
                Main.player[Projectile.owner].immuneTime = 10;
                target.immune[Projectile.owner] = 10;

                float resist = target.knockBackResist;
                if (resist < 0.5 && !target.boss)
                    resist = 0.5f;
                target.velocity.Y = -20 * resist;

                if (Projectile.localAI[0] == 0)
                {
                    SoundEngine.PlaySound(LobotomyCorp.WeaponSound("greed2").WithPitchOffset(0.2f), Projectile.Center);
                    DiamondDust(Projectile.Center + new Vector2(10f * Main.player[Projectile.owner].direction, -24f), DustID.GoldCoin, 10, 5, 10, 1.5f, MathHelper.ToRadians(Projectile.spriteDirection > 0 ? -45 : 45));
                    Projectile.localAI[0]++;
                }
                for (int i = 0; i < 12; i++)
                {
                    Dust d = Main.dust[Dust.NewDust(target.Center, target.width, target.height, DustID.GoldCoin)];
                    d.noGravity = true;
                    d.fadeIn = 1.2f;
                    d.velocity = new Vector2(8, 0).RotatedBy(MathHelper.ToRadians(Projectile.spriteDirection > 0 ? -45f : -135f));
                }
                Main.player[Projectile.owner].immuneTime = 30;
                Main.player[Projectile.owner].immune = true;
            }

            if (Projectile.ai[2] < 0)
            {
                target.immune[Projectile.owner] = 2;
                Projectile.ai[1] = 2;
                Projectile.ai[2] = target.whoAmI;
                Projectile.timeLeft = 30;
                Main.player[Projectile.owner].velocity *= 0.2f;
                Main.player[Projectile.owner].immuneTime = 30;
                Main.player[Projectile.owner].immune = true;
                Main.player[Projectile.owner].immuneNoBlink = false;

                for (int i = 0; i < 12; i++)
                {
                    Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin)];
                    d.noGravity = true;
                    d.fadeIn = 1.2f;
                    d.velocity = Projectile.velocity;
                }
                DiamondDust(target.Center, DustID.GoldCoin, 5, 8, 8, 1.2f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            Main.player[Projectile.owner].immuneNoBlink = false;
            if (Projectile.ai[0] < 60)
                return;

            if (Projectile.ai[2] < 0)
            {
                Main.player[Projectile.owner].velocity *= 0.2f;

                float strength = (Projectile.ai[0] / 60f);
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 4, ModContent.ProjectileType<GoldRushFlurry>(), (int)(Projectile.damage * strength / 2), Projectile.knockBack, Projectile.owner);
            }

            foreach (NPC n in Main.npc)
            {
                if (Main.player[Projectile.owner].getRect().Contains(n.getRect()))
                {
                    Main.player[Projectile.owner].immuneTime = 30;
                    Main.player[Projectile.owner].immune = true;
                    break;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= (Projectile.ai[0] / 60f);
            if (Projectile.ai[1] == 2)
                modifiers.SourceDamage *= 0.7f;
            modifiers.Knockback *= 2f - target.knockBackResist;
            base.ModifyHitNPC(target, ref modifiers);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Vector2 origin = new Vector2(18, 12);

			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, position, TextureAssets.Projectile[Projectile.type].Frame(), lightColor * (1f - Projectile.alpha / 255f), Projectile.rotation + 0.785f * Projectile.spriteDirection, origin, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			return false;
        }

        public static void DiamondDust(Vector2 pos, int type, int amount, int x, int y, float fade = 1f, float angle = 0)
        {
            for (int i = 0; i < amount; i++)
            {
                float prog = i / (float)amount;
                float x1 = x * prog;
                float x2 = x * (1f - prog);
                float y1 = y * prog;
                float y2 = y * (1f - prog);

                Dust d = Dust.NewDustPerfect(pos, type, new Vector2(x1, -y2).RotatedBy(angle));
                d.noGravity = true;
                d.fadeIn = fade;
                d = Dust.NewDustPerfect(pos, type, new Vector2(-x2, -y1).RotatedBy(angle));
                d.noGravity = true;
                d.fadeIn = fade;
                d = Dust.NewDustPerfect(pos, type, new Vector2(-x1, y2).RotatedBy(angle));
                d.noGravity = true;
                d.fadeIn = fade;
                d = Dust.NewDustPerfect(pos, type, new Vector2(x2, y1).RotatedBy(angle));
                d.noGravity = true;
                d.fadeIn = fade;
            }
        }
    }
}
