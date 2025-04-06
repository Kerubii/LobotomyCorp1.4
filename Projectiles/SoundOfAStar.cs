using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class SoundOfAStar : ModProjectile
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Anime");
        }

		public override void SetDefaults() {
			Projectile.width = 12;
			Projectile.height = 12;
			Projectile.aiStyle = -1;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
            Projectile.timeLeft = 42;

            Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.friendly = true;
		}

        public override void AI() {
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);

            Projectile.ai[0]++;
            if (Projectile.ai[0] < 32)
            {
                Projectile.position = ownerMountedCenter + Projectile.velocity * Projectile.ai[1];
                if (Projectile.ai[0] < 25)
                    Projectile.ai[1] += 1.2f * (Projectile.ai[0] > 5 ? 1f - ((Projectile.ai[0] - 5f ) / 20f) : 1f);
            }
            else if (Projectile.ai[0] == 32)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 targetPos = Main.MouseWorld;// + new Vector2(Main.rand.Next(32), Main.rand.Next(32));

                    float distance = 250;
                    if (projOwner.HasMinionAttackTargetNPC && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[projOwner.MinionAttackTargetNPC].position, Main.npc[projOwner.MinionAttackTargetNPC].width, Main.npc[projOwner.MinionAttackTargetNPC].height))
                    {
                        targetPos = Main.npc[projOwner.MinionAttackTargetNPC].Center;
                    }
                    else
                        foreach (NPC n in Main.npc)
                        {
                            if (n.active && !n.friendly && !n.dontTakeDamage && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, n.position, n.width, n.height) && n.CanBeChasedBy(this))
                            {
                                float npcDist = n.Center.Distance(Main.MouseWorld);
                                if (npcDist < distance)
                                {
                                    distance = npcDist;
                                    targetPos = n.Center;
                                }
                            }
                        }

                    Vector2 vel = Vector2.Normalize(targetPos - Projectile.Center);
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel * 4, ModContent.ProjectileType<SoundOfAStarShoot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Projectile.velocity = vel * -4f;
                    Projectile.netUpdate = true;

                    SoundEngine.PlaySound(SoundID.Item72, Projectile.Center);
                }
                for (int i = 0; i < 8; i++)
                {
                    Vector2 speed = new Vector2(4, 0).RotatedBy(MathHelper.ToRadians(i * 45));
                    Dust.NewDustPerfect(Projectile.Center, 91, speed, 0, default(Color), 0.5f).noGravity = true;
                }
            }
            else
            {
                Projectile.scale -= 0.1f;
                return;
            }

            if (Projectile.ai[0] > 10)
            {
                //projOwner.itemTime = 2;
                //projOwner.itemAnimation = 2;
                Projectile.direction = projOwner.direction;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(
                TextureAssets.Projectile[Projectile.type].Value,
                Projectile.Center - Main.screenPosition + (Projectile.ai[0] > 45 && Projectile.ai[0] < 60 ? new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)) : Vector2.Zero),
                TextureAssets.Projectile[Projectile.type].Frame(),
                lightColor,
                Projectile.rotation,
                TextureAssets.Projectile[Projectile.type].Size()/2,
                Projectile.scale * 0.5f, 
                0f, 0);
            return false;
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
            return Projectile.ai[0] > 32;
        }
    }
    
    public class SoundOfAStarShoot : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/SoundOfAStar";

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
            Projectile.tileCollide = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            Projectile.tileCollide = false;
            Projectile.extraUpdates = 50;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            if (Projectile.ai[0] > 0 && Projectile.timeLeft > 2)
            {
                Projectile.timeLeft = 2;
            }

            if (Projectile.ai[1]++ > 30)
            {
                Projectile.tileCollide = true;
            }
            Dust.NewDustPerfect(Projectile.Center, 91).noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[0]++;
            Projectile.velocity *= 0;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (Projectile.ai[0] > 0)
            {
                int hitboxSize = 80;
                hitbox.X += hitbox.Width / 2 - hitboxSize / 2;
                hitbox.Y += hitbox.Height / 2 - hitboxSize / 2;
                hitbox.Width = hitboxSize;
                hitbox.Height = hitboxSize;
            }
            base.ModifyDamageHitbox(ref hitbox);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float multiplier = 1f - Projectile.ai[0] / 5f;
            if (multiplier < 0.1f)
                multiplier = 0.1f;
            modifiers.FinalDamage *= multiplier;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 180; i++)
            {
                Vector2 speed = new Vector2(4, 0).RotatedBy(MathHelper.ToRadians(i * 2));
                Dust.NewDustPerfect(Projectile.Center, 91, speed).noGravity = true;
            }
        }
    }
}
