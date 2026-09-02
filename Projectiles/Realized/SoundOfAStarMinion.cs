using LobotomyCorp.Buffs;
using LobotomyCorp.Misc;
using LobotomyCorp.ModSystems;
using LobotomyCorp.Players;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.Realized
{
	public class SoundOfAStarMinion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 2;

            Main.projPet[Projectile.type] = true;

            //ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        private readonly int trailamount = 24;

        public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20; 
			Projectile.aiStyle = -1;

			Projectile.DamageType = DamageClass.Summon;
			Projectile.friendly = true;
			Projectile.tileCollide = false;

			Projectile.netImportant = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 8;

			trailpos = new Vector2[trailamount];
			trailposrot = new float[trailamount];
            trailneg = new Vector2[trailamount];
            trailnegrot = new float[trailamount];

            Projectile.minion = true;
            Projectile.minionSlots = 1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

		private float state
		{
			get { return (int)Projectile.ai[0]; }
			set { Projectile.ai[0] = value; }
		}

		private float twinRotation
		{
			get { return Projectile.ai[1]; }
			set { Projectile.ai[1] = value; }
		}

        private float twinDistance
        {
            get { return Projectile.ai[2]; }
            set { Projectile.ai[2] = value; }
        }

		private Vector2 targetPos = Vector2.Zero;

        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.WriteVector2(targetPos);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			targetPos = reader.ReadVector2();
        }

		private int getExtraUpdates => Projectile.extraUpdates + 1;

		private int getOrder()
		{
            int Pos = 0;
            for (int i = Projectile.whoAmI - 1; i >= 0; i--)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == Projectile.type && Main.projectile[i].owner == Projectile.owner) Pos++;
            }
			return Pos;
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

		private int prefire // Time it takes to reach target position
        {
			get 
			{
                return (doesStarExist ? 5 : 10) * getExtraUpdates; 
			}
		}

        private int firing // Time it takes to execute ding
        {
            get 
			{
                return (doesStarExist ? 20 : 30) * getExtraUpdates; 
			}
        }

        private int postfiring  // Time it takes to recover before returning to the player
        {
            get 
			{
                return (doesStarExist ? 30 : 60) * getExtraUpdates; 
			}
        }


        private Vector2[] trailpos;
		private float[] trailposrot;
		private Vector2[] trailneg;
		private float[] trailnegrot;

        public override void AI()
        {
			int prefireDist = 260; // Distance away from the target before pinging

            Player player = Main.player[Projectile.owner];
            int Pos = getOrder();
			int dir = (Pos % 2 == 0) ? 1 : -1;

			if (Main.rand.NextBool(8))
			{
				int i = Dust.NewDust(Projectile.position + getOrboffset(1), Projectile.width, Projectile.height, DustID.GemSapphire);
				Main.dust[i].noGravity = true;
            }
            if (Main.rand.NextBool(8))
            {
                int i = Dust.NewDust(Projectile.position + getOrboffset(-1), Projectile.width, Projectile.height, DustID.GemSapphire);
                Main.dust[i].noGravity = true;
            }

			if (!CheckActive(player))
				return;

            Projectile.localAI[1] -= 1 / 60f;

			// teleport if too far
            float chaseDist = Vector2.Distance(Projectile.Center, player.Center);
            if (chaseDist > 2000f)
            {
                Projectile.position.X = player.position.X + (float)(player.width / 2) - (float)(Projectile.width / 2);
                Projectile.position.Y = player.position.Y + (float)(player.height / 2) - (float)(Projectile.height / 2);
				state = 0;
            }

            // Default, stays on the player or on the blue star
            // Becomes more spread out the more hearts exists
            if (state <= 1)
			{
				float targetWidth = 60 + (20 * Pos);
				float targetRotation = Projectile.localAI[0];
				targetRotation = MathHelper.WrapAngle(MathHelper.ToRadians(targetRotation));
				Projectile.localAI[0] += dir * 4;

				twinDistance = (float)Terraria.Utils.Lerp(twinDistance, targetWidth, 0.05f);
				twinRotation = Terraria.Utils.AngleTowards(twinRotation, targetRotation, MathHelper.ToRadians(6));

				// Changes whenever Blue Star is alive or just the player
				Vector2 rest = player.MountedCenter;
				int bluestartype = ModContent.ProjectileType<SoundOfAStarBlueStar>();
				if (doesStarExist)
				{
					foreach (Projectile p in Main.ActiveProjectiles)
					{
						if (p.type == bluestartype && p.owner == Projectile.owner)
						{
							rest = p.Center;
							break;
						}
					}
				}

				AIHelper.ChaseTargetLerp(Projectile.Center, rest, ref Projectile.velocity, 16 / getExtraUpdates, 0.8f);

				// Targetting cooldown, set by other factors
				if (state < 0)
				{
					state++;
					return;
				}
				float dist = 1500;
				int most = -1;
				int target = -1;
				bool notarget = false;
				List<Vector2> targets = new List<Vector2>();
				// Initial npc check, find nearest target with the most nearby enemies
				// First checks if theres a valid non killable target if blue star is active
				// Does a second check without that condition if theres no possible enemies to pull them in
				do
				{
					foreach (NPC n in Main.ActiveNPCs)
					{
						bool validTarget = notarget ? true : !isValidBlueStarTarget(n);
						if (n.CanBeChasedBy(this) && Collision.CanHit(rest - new Vector2(8, 8), 16, 16, n.position, n.width, n.height) && validTarget)
						{
							List<Vector2> countNearby = new List<Vector2>();
							findNearby(ref countNearby, n.whoAmI, range: 200);
							int count = countNearby.Count;
							float npcDist = n.Center.Distance(player.MountedCenter);
							if (npcDist < dist && count > most)
							{
								dist = npcDist;
								most = count;
								target = n.whoAmI;
								targets = countNearby;
								notarget = false;
							}
						}
					}
					if (target == -1)
					{
						notarget = !notarget;
					}
				}
				while (notarget && target == -1);
				// Second npc check, find targets within range to main target and average the location
				// If Blue Star is present, averages it towards Blue Star instead
				if (target > -1)
				{
					Vector2 mainTarget = Main.npc[target].Center;
					targets.Add(mainTarget);

					if (doesStarExist)
					{
						Vector2 pullToStar = mainTarget + mainTarget.DirectionTo(rest) * 300;
						targets.Add(pullToStar);
					}

					Vector2 finalTarget = Vector2.Zero;
					foreach (Vector2 p in targets)
					{
						finalTarget += p;
					}
					finalTarget /= targets.Count;
					targetPos = finalTarget;

					// Give other balls a cooldown to delay the attacks
					// Does not work if the ball just finished attacking so it does not delay the remaining ones
					if (state == 0)
					{
                        foreach (Projectile p in Main.ActiveProjectiles)
                        {
                            if (p.active && p.whoAmI != Projectile.whoAmI && p.type == Projectile.type && p.owner == Projectile.owner && p.ai[0] <= 0f)
                            {
                                p.ai[0] = -10;
                                if (doesStarExist)
                                {
                                    p.ai[0] = -8;
                                }
                                p.netUpdate = true;
                            }
                        }
						state++;
                    }

                    // Proceed to next state if a target position is marked down
                    Projectile.velocity *= 0;
                    state++;
                }
				else
				{
					if (state > 0)
						state = 0;
				}
				return;
			}
			// Moving into position
			else if (state < prefire)
			{
				float time = (state + 1f) / prefire;
				twinDistance = (float)Terraria.Utils.Lerp(twinDistance, prefireDist, time);
				Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, time);
				twinRotation += MathHelper.ToRadians(1f / getExtraUpdates) * dir;
			}
			// Closing in
			else if (state < prefire + firing)
			{
				float time = (state - prefire - 1f) / firing;
				twinDistance = prefireDist - prefireDist * (1 - (1 - time) * (1 - time));
				twinRotation += MathHelper.ToRadians((10f + 8 * time) / getExtraUpdates) * dir;
			}
			// Ding
			else if (state == prefire + firing)
			{
				// Sucks enemies in with effects
				for (int i = 0; i < 180; i++)
				{
					Vector2 speed = new Vector2(30, 0).RotatedBy(MathHelper.ToRadians(i * 2));
					Dust.NewDustPerfect(Projectile.Center, 91, speed).noGravity = true;
				}
				// Suck enemies in regardless of stats
				foreach (NPC target in Main.ActiveNPCs)
				{
					if (target.boss || target.dontTakeDamage || target.Distance(Projectile.Center) > 250)
						continue;
                    if (target.knockBackResist > 0f)
                        target.velocity += (target.knockBackResist * Projectile.knockBack) * target.Center.DirectionTo(Projectile.Center);
                }

				twinRotation = Main.rand.NextFloat(3.14f);
				Projectile.scale = 2f;
            }
			else if (state < prefire + firing + postfiring)
			{
                float time = (state - prefire - firing + 1f) / postfiring;
				twinDistance += 8 / getExtraUpdates * (1f - time);
				twinRotation += MathHelper.ToRadians(10f * (1f - time)) * dir;
                Projectile.scale = 2f - 1f * time;
            }
			else
			{
				Projectile.localAI[0] = Main.rand.NextFloat(3.14f);
				state = 0;
			}
			state++;
        }

        public override void PostAI()
        {
            record();
        }

        public override bool MinionContactDamage()
        {
			return true;
        }

		private bool CheckActive(Player owner)
		{
			int bufType = ModContent.BuffType<SoundOfAStarMinionBuff>();

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

		private void findNearby(ref List<Vector2> list, int target, int range)
		{
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.whoAmI != target && n.CanBeChasedBy(this) && n.Distance(Main.npc[target].Center) < range && !isValidBlueStarTarget(n))
                {
                    list.Add(n.Center);
                }
            }
        }

		private void record()
		{			
            for (int i = trailamount - 1; i > 0; i--)
			{
				trailpos[i] = trailpos[i - 1];
                trailneg[i] = trailneg[i - 1];
                trailposrot[i] = trailposrot[i - 1];
                trailnegrot[i] = trailnegrot[i - 1];
            }

            Vector2 orbPos = getOrboffset(1);
            trailpos[0] = Projectile.Center + orbPos;
			float rot = trailpos[1].DirectionTo(trailpos[0]).ToRotation();
            //trailposrot[0] = twinRotation + 1.57f;
            trailposrot[1] = trailposrot[0] = rot;

            orbPos = getOrboffset(-1);
            trailneg[0] = Projectile.Center + orbPos;
            rot = trailneg[1].DirectionTo(trailneg[0]).ToRotation();
            //trailnegrot[0] = twinRotation - 1.57f;
            trailnegrot[1] = trailnegrot[0] = rot;
        }

		private Vector2 getOrboffset(int dir)
		{
            return new Vector2(twinDistance * dir, 0).RotatedBy(twinRotation);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (state == prefire + firing)
			{
				int size = 400;
				hitbox.X += hitbox.Width / 2 - size / 2;
				hitbox.Width = size;
                hitbox.Y += hitbox.Height / 2 - size / 2;
                hitbox.Height = size;
            }
			else
			{
				int dir = Projectile.timeLeft % 2 == 0 ? -1 : 1;
				Vector2 orbPos = Projectile.position + getOrboffset(dir);
				hitbox.X = (int)orbPos.X;
				hitbox.Y = (int)orbPos.Y;
			}
        }

        public override bool? CanHitNPC(NPC target)
        {
			if (isValidBlueStarTarget(target))
				return false;
			if (state > prefire && state < prefire + firing + postfiring)
				return base.CanHitNPC(target);
            return false;            
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
			modifiers.DisableKnockback();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Rectangle frame = tex.Frame(verticalFrames: 2, frameY: 1);

            for (int i = -1; i < 2; i += 2)
            {
                Vector2 orbPos = pos + getOrboffset(i);
                float rot = Projectile.rotation;
                Main.EntitySpriteDraw(tex, orbPos, frame, Color.White, rot, new Vector2(12, 10), Projectile.scale, 0, 0);
            }

            drawTrail();

			/*frame.Y *= 0;
			for (int i = -1; i < 2; i += 2)
			{
				Vector2 orbPos = pos + getOrboffset(i);
				float rot = Projectile.rotation;
				Main.EntitySpriteDraw(tex, orbPos, frame, lightColor, rot, new Vector2(12, 10), Projectile.scale, 0, 0);
			}*/

			return false;
        }

		private void drawTrail()
		{
			float range = (float)Math.Sin(MathHelper.ToRadians((Projectile.localAI[1] * 1.2f)));
            CustomShaderData shader = LobotomyCorp.LobcorpShaders["SwingTrail"].UseOpacity(0.7f + 0.1f * range);
            shader.UseImage1(MiscAssets.StarColor);
            shader.UseImage2(MiscAssets.Gradient);
            shader.UseImage3(MiscAssets.FlameTrail);
			shader.UseCustomShaderDate(Projectile.localAI[1], 0);

            SlashTrail slashTrail = new SlashTrail(14, 0);
            range = (float)Math.Sin(MathHelper.ToRadians((Projectile.localAI[1] * 0.4f)));
            slashTrail.color = Color.White;
			//slashTrail.color = Color.Lerp(new Color(129, 238, 255), new Color(79, 108, 203), 0.5f + 0.5f * range);
			slashTrail.DrawSpecific(trailpos, trailposrot, Vector2.Zero, shader);
            slashTrail.DrawSpecific(trailneg, trailnegrot, Vector2.Zero, shader);
        }
    }
}
