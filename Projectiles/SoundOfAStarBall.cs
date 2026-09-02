using LobotomyCorp.Buffs;
using LobotomyCorp.Projectiles.Realized;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles
{
	public class SoundOfAStarBall : ModProjectile
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Anime");
            Main.projPet[Projectile.type] = true;

            //ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

		public override void SetDefaults() {
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.aiStyle = -1;
			Projectile.penetrate = 1;
			Projectile.scale = 1f;
            Projectile.timeLeft = 2;

            Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.friendly = true;

            Projectile.minion = true;
            Projectile.minionSlots = 1;
        }

        public override void AI() {
            
            Player projOwner = Main.player[Projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);

            if (!CheckActive(projOwner))
                return;

            // teleport if too far
            int flyDistance = 1200 + 40 * Projectile.minionPos;
            float chaseDist = Vector2.Distance(Projectile.Center, projOwner.Center);
            if (chaseDist > 2000f)
            {
                Projectile.position.X = projOwner.position.X + (float)(projOwner.width / 2) - (float)(Projectile.width / 2);
                Projectile.position.Y = projOwner.position.Y + (float)(projOwner.height / 2) - (float)(Projectile.height / 2);
            }
            else if (chaseDist > flyDistance)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }

            // Teleport to player
            if (Projectile.ai[0] != 0)
            {
                Projectile.Center = projOwner.Center;
                Projectile.velocity *= 0;
                Projectile.ai[0] = 0;
                Projectile.localAI[0] = 0;
            }
            else
            {
                NPC target = null;
                bool hasTarget = false;

                // If its not on cooldown
                if (Projectile.ai[1] <= 0)
                {
                    // Find target
                    if (Projectile.OwnerMinionAttackTargetNPC != null && Projectile.OwnerMinionAttackTargetNPC.CanBeChasedBy(this) && Collision.CanHit(Projectile, Projectile.OwnerMinionAttackTargetNPC))
                    {
                        hasTarget = true;
                        target = Projectile.OwnerMinionAttackTargetNPC;
                    }
                    // If main target is unreachable, find another target
                    else
                    {
                        float minDist = 10000;
                        foreach (NPC n in Main.ActiveNPCs)
                        {
                            if (n.CanBeChasedBy(n) && Collision.CanHit(Projectile, n))
                            {
                                float curDist = n.Center.Distance(Projectile.Center);
                                if (curDist < minDist)
                                {
                                    target = n;
                                    hasTarget = true;

                                    minDist = curDist;
                                }
                            }
                        }
                    }
                }

                // Regular Behavior
                if (!hasTarget)
                {
                    // Attack cooldown
                    if (Projectile.ai[1] > 0)
                    {
                        Projectile.ai[1]--;
                        Projectile.velocity *= 0.95f;
                    }
                    else
                    {
                        float pos = getOrder();
                        Vector2 ownerCenter = projOwner.Center + new Vector2(2 * pos, 0).RotatedBy(pos * 0.785f);
                        if (doesStarExist)
                        {
                            int bluestartype = ModContent.ProjectileType<SoundOfAStarBlueStar>();
                            foreach (Projectile p in Main.ActiveProjectiles)
                            {
                                if (p.type == bluestartype && p.owner == Projectile.owner)
                                {
                                    ownerCenter = p.Center + new Vector2(8 * pos, 0).RotatedBy(pos * 0.785f);
                                    break;
                                }
                            }
                        }

                        if (!AIHelper.ChaseTargetDirect(Projectile.Center, ownerCenter, ref Projectile.velocity, 6f, 100))
                        {
                            Projectile.velocity *= 0.95f;
                        }
                    }
                    Disperse();
                }
                // Chase Behavior
                else
                {
                    float distance = target.Center.Distance(projOwner.Center) + 100;
                    int pos = 2 * getOrder();
                    distance = 30;
                    Vector2 ownerCenter = projOwner.Center + new Vector2(2 * pos, 0).RotatedBy(pos * 0.785f);

                    if (doesStarExist)
                    {
                        int bluestartype = ModContent.ProjectileType<SoundOfAStarBlueStar>();
                        foreach (Projectile p in Main.ActiveProjectiles)
                        {
                            if (p.type == bluestartype && p.owner == Projectile.owner)
                            {
                                ownerCenter = p.Center + new Vector2(2 * pos, 0).RotatedBy(pos * 0.785f);
                                distance = 30;
                                //distance = target.Center.Distance(p.Center) + 100;
                                break;
                            }
                        }
                    }

                    bool inRange;
                    bool fireFromOwner = true;
                    if (fireFromOwner)
                    {
                        inRange = !AIHelper.ChaseTargetAccel(Projectile.Center, ownerCenter, ref Projectile.velocity, 8f, 1.2f, distance);
                    }
                    else
                    {
                        inRange = !AIHelper.ChaseTargetAccel(Projectile.Center, target.Center, ref Projectile.velocity, 8f, 1.2f, distance);
                    }

                    if (inRange)
                    {
                        if (Collision.CanHit(Projectile, target) && Projectile.ai[2] > Main.rand.Next(15, 30))// && Projectile.Center.Distance(target.Center) < distance)
                        {
                            shoot(target);
                        }
                        Projectile.ai[2]++;
                    }
                    else
                    {
                        if (Collision.CanHit(Projectile, target))// && Projectile.Center.Distance(target.Center) < distance)
                        {
                            if (Projectile.ai[2] > Main.rand.Next(30, 90))
                            {
                                shoot(target);
                            }                            
                            Projectile.ai[2]++;
                        }
                    }
                }
            }

            if (Projectile.localAI[1] > 0f)
            {
                Projectile.localAI[1] -= 0.05f;
                if (Projectile.localAI[1] < 0f)
                    Projectile.localAI[1] = 0f;
            }

            Float();
        }
        private bool CheckActive(Player owner)
        {
            int bufType = ModContent.BuffType<SoundOfAStarBallBuff>();

            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(bufType);

                return false;
            }

            if (owner.HasBuff(bufType) && Projectile.timeLeft < 2)
            {
                Projectile.timeLeft = 3;
            }
            return true;
        }
        private void Disperse()
        {
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.whoAmI != Projectile.whoAmI && proj.type == Projectile.type && proj.owner == Projectile.owner && Math.Abs(Projectile.position.X - proj.position.X) + Math.Abs(Projectile.position.Y - proj.position.Y) < Projectile.width)
                {
                    float num6 = 0.1f;
                    if (Projectile.position.X < proj.position.X)
                        Projectile.velocity.X -= num6;
                    else
                        Projectile.velocity.X += num6;
                    if (Projectile.position.Y < proj.position.Y)
                        Projectile.velocity.Y -= num6;
                    else
                        Projectile.velocity.Y += num6;
                }
            }
        }

        private void Float()
        {
            if (Projectile.velocity.Length() < 1f)
            {
                Projectile.localAI[0]++;
            }
            else
            {
                Projectile.localAI[0] = 0;
            }
        }

        private int getOrder()
        {
            int Pos = 0;
            for (int i = Projectile.whoAmI - 1; i >= 0; i--)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == Projectile.type && Main.projectile[i].owner == Projectile.owner) Pos++;
            }
            return Pos;
        }

        private void shoot(NPC target)
        {
            Vector2 vel = target.Center.DirectionTo(Projectile.Center) * 4;

            // Projectile shoot
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -vel, ModContent.ProjectileType<SoundOfAStarShoot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            Projectile.velocity = vel;
            Projectile.ai[1] = doesStarExist ? 30 : 15;
            Projectile.localAI[1] = 1f;
            Projectile.ai[2] = 0;
        }
        private bool doesStarExist
        {
            get
            {
                Player player = Main.player[Projectile.owner];
                return player.ownedProjectileCounts[ModContent.ProjectileType<SoundOfAStarBlueStar>()] > 0;
            }
        }

        private bool isValidBlueStarTarget(NPC target)
        {
            return doesStarExist && target.life <= target.lifeMax * 0.2f && !target.boss;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 offset = new Vector2(0, 4f * (float)(Math.Sin(Projectile.localAI[0] / 120f * 6.28f)));
            float scale = 0.25f * (float)Math.Sin(Projectile.localAI[1] * 3.14f);

            Main.EntitySpriteDraw(
                TextureAssets.Projectile[Projectile.type].Value,
                Projectile.Center - Main.screenPosition + offset,
                TextureAssets.Projectile[Projectile.type].Frame(),
                lightColor,
                Projectile.rotation,
                TextureAssets.Projectile[Projectile.type].Size()/2,
                Projectile.scale * 0.75f + scale, 
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
    }
}
