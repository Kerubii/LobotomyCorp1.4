using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using LobotomyCorp.Items.Ruina.Art;
using Steamworks;
using static Humanizer.In;
using LobotomyCorp.Items.He;
using LobotomyCorp.Players;

namespace LobotomyCorp.Projectiles.Realized
{
	public class PleasureSpike : ModProjectile
	{
		private Bezier TailBezier;

        public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			
			Projectile.DamageType = DamageClass.Summon;
			Projectile.friendly = true;
			Projectile.netImportant = true;
		}

		private readonly int stabFrames = 25;
		private readonly int restFrames = 90;

		private int Target { get { return (int)Projectile.ai[0]; } set { Projectile.ai[0] = value; } }
		private float Timer { get { return Projectile.ai[1]; } set { Projectile.ai[1] = value; } }
        private float SpeedStore { get { return Projectile.ai[2]; } set { Projectile.ai[2] = value; } }

        private int TargetAlt { get { return ((int)Projectile.ai[0] + 1) * -1; } set { Projectile.ai[0] = (value + 1) * -1; } }

        public override void AI()
		{
			Player owner = Main.player[Projectile.owner];
			LobotomyWawPlayer lobPlayer = owner.GetModPlayer<LobotomyWawPlayer>();
            if (!owner.active || owner.dead || !lobPlayer.PleasureTail)// || owner.HeldItem.type != ModContent.ItemType<PleasureR>())
                Projectile.Kill();

			// Checks if holding the Realized Item of this wepaon
			bool isHoldingAssociatedItem = owner.HeldItem.type == ModContent.ItemType<PleasureR>();

            Projectile.timeLeft = 10;
			float distanceLimit = 800;

			Vector2 target = owner.Center - new Vector2(owner.direction * 24, 10);
			float speed = 0.16f;
			float maxSpeed = 8f;
			int radius = 10;
			float slowdown = 0.95f;
			float slowLimit = 0.5f;

			if (Target < 0f)
			{
				if (Projectile.velocity.Length() < maxSpeed * 0.7f)
					Projectile.rotation = Terraria.Utils.AngleLerp(Projectile.rotation, -MathHelper.ToRadians(90), 0.12f);
				else
					Projectile.rotation = Terraria.Utils.AngleLerp(Projectile.rotation, Projectile.velocity.ToRotation() + 3.14f, 0.12f);
            }

            // Basic Target finding system
            if (Projectile.Center.Distance(owner.Center) > 800)
			{

			}

			// Alternate Attack
			if (false) //TargetAlt >= 0)
			{
                //NPC npc = Main.npc[Target];
                //if (!owner.CanHit(npc) || !npc.active) ResetTarget();
            }
			else
			{
				if (!isHoldingAssociatedItem)
				{
					float nearest = 1000;
					foreach (NPC n in Main.npc)
					{
						if (n.active && n.CanBeChasedBy(this) && owner.CanHit(n))
						{
							float distance = (n.Center - owner.Center).Length();
							if (distance < nearest)
							{
								nearest = distance;
								Target = n.whoAmI;
							}
						}
					}
				}
				else if (owner.itemAnimation == owner.itemAnimationMax && Projectile.owner == Main.myPlayer)
				{
					Vector2 mouse = Main.MouseWorld;
					float nearest = 1000;
					foreach (NPC n in Main.npc)
					{
						if (n.active && n.CanBeChasedBy(this) && owner.CanHit(n))
						{
							float distance = (n.Center - mouse).Length();
							if (distance < nearest)
							{
								nearest = distance;
								Target = n.whoAmI;
								if (owner.altFunctionUse == 2)
									TargetAlt = n.whoAmI;
							}
						}
					}
                    Projectile.netUpdate = true;
                }

				// If Target is present
				if (Target >= 0)
				{
					NPC npc = Main.npc[Target];

					target = npc.Center;
					speed = 14f;

					if (!owner.CanHit(npc) || !npc.active) ResetTarget();

					//Check if within parameters to stab
					float Distance = (target - Projectile.Center).Length();

					// Stab Parameters
					// Time it takes to stab
					int StabFrames = stabFrames;
					// Time it takes to ready after stabs
					int RestFrames = restFrames;
					// Amount of Stabs
					int amountOfHits = 3;

					if (!isHoldingAssociatedItem)
					{
						amountOfHits = 1;
					}

					if (Timer == 0 && Distance < 200 && Distance > 150)
					{
						Timer = RestFrames + StabFrames * amountOfHits;

						SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Art/Porccu_Strong_Stab1") with { Volume = 0.3f }, Projectile.Center);
					}

					// Get the attack frames
					int moduloFrames = (int)(Timer - RestFrames) % StabFrames;
					// While not during Rest period
					if (Timer > RestFrames)
					{
						// First Frame, pierce through the enemy
						if (moduloFrames == 0)
						{
							TipStretchStart();
							Projectile.velocity = (target - Projectile.Center) / 5;
							SpeedStore = Projectile.velocity.Length();
							Projectile.rotation = Projectile.velocity.ToRotation();
						}
						// Wait for 5 frames as travel time
						else if (moduloFrames > StabFrames - StabFrames / 5)
						{

						}
						// 10 Frames to slowdown
						else if (moduloFrames > StabFrames - StabFrames * 3 / 5)
						{
							Projectile.velocity *= 0.8f;
						}
						// 10 Frames to try return to previous position
						else if (moduloFrames > 0)
						{
							if (Timer < RestFrames + StabFrames)
							{
								bool away = false;
								int dir = Math.Sign(owner.Center.X - target.X);
								if (dir != Math.Sign(Projectile.Center.X - target.X))
									away = true;
								if (away)
									Projectile.velocity = new Vector2(-9.5f, 0).RotatedBy(Projectile.rotation);
							}
							else
								Projectile.velocity = new Vector2(-SpeedStore * 0.75f, 0).RotatedBy(Projectile.rotation);
						}
					}
					else
					{
						Timer = 0;
						Projectile.rotation = Terraria.Utils.AngleLerp(Projectile.rotation, (npc.Center - Projectile.Center).ToRotation(), 0.08f);
						MoveDistanced(target, speed, maxSpeed);
					}

					// Tick Timer down
					if (Timer > 0)
						Timer--;
				}
				else
				{
					if ((owner.Center - Projectile.Center).Length() > 240)
					{
						maxSpeed *= 2;
						speed *= 4;
					}

					Timer = 0;
					MoveTowards(target, speed, maxSpeed, radius, slowdown, slowLimit);
				}
			}

			TailHandler(owner, maxSpeed);
        }

		private void MoveDistanced(Vector2 target, float speed, float maxSpeed, float length1 = 200, float length2 = 150)
		{
			Vector2 diff = target - Projectile.Center;
            float length = diff.Length();
            diff.Normalize();

            if (length > length1)
            {
                Vector2 vel = diff * maxSpeed;
                Projectile.velocity.X = (float)((Projectile.velocity.X * 40.0 + vel.X) / 41.0);
                Projectile.velocity.Y = (float)((Projectile.velocity.Y * 40.0 + vel.Y) / 41.0);
            }
            else if (length < length2)
            {
                maxSpeed *= .75f;
                Vector2 vel = diff * -maxSpeed;
                Projectile.velocity.X = (float)((Projectile.velocity.X * 40.0 + vel.X) / 41.0);
                Projectile.velocity.Y = (float)((Projectile.velocity.Y * 40.0 + vel.Y) / 41.0);
            }
            else
            {
                Projectile.velocity *= 0.97f;
            }
			/*
			if (Projectile.velocity.Length() > maxSpeed)
			{
                Projectile.velocity.Normalize();
                Projectile.velocity *= maxSpeed;
            }*/
        }

		private void MoveTowards(Vector2 position, float speed, float maxSpeed, int radius, float slowdown, float slowLimit)
		{
			Vector2 delt = position - Projectile.Center;
			if (delt.Length() < radius)
			{
				if (Projectile.velocity.Length() > maxSpeed * slowLimit)
					Projectile.velocity *= slowdown;
				return;
			}

			delt.Normalize();
            delt *= speed;

            Projectile.velocity.X += delt.X;
			if ((delt.X > 0 && Projectile.velocity.X < 0) ||
                (delt.X < 0 && Projectile.velocity.X > 0))
			{
				Projectile.velocity.X *= slowdown;
				Projectile.velocity.X += delt.X * 2;
			}

            Projectile.velocity.Y += delt.Y;
            if ((delt.Y > 0 && Projectile.velocity.Y < 0) ||
                (delt.Y < 0 && Projectile.velocity.Y > 0))
            {
                Projectile.velocity.Y *= slowdown;
                Projectile.velocity.Y += delt.Y * 2;
            }

            if (Projectile.velocity.Length() > maxSpeed)
			{
				Projectile.velocity.Normalize();
				Projectile.velocity *= maxSpeed;
			}
			/*
			if (radius == 0)
			{
				delt = position - Projectile.Center;
				if (delt.Length() < maxSpeed && Projectile.velocity.Length() > delt.Length())
					Projectile.velocity = delt;
			}*/
		}

		private void ResetTarget() { Target = -1; }

		private void TailHandler(Player owner, float maxSpeed)
        {
            Vector2 tailPosition = owner.RotatedRelativePoint(owner.MountedCenter) + new Vector2(0, 6);
            Vector2 tailTip = Projectile.Center + Projectile.velocity;

            if (TailBezier == null)
            {
                TailBezier = new Bezier(tailPosition, tailTip);
            }
            TailBezier.SetStartEnd(tailPosition, tailTip);

            float length = (tailTip - tailPosition).Length() / 2;
            if (length < 10)
                length = 10;

            if (Target < 0)
            {
                if (owner.velocity.Length() > 1)
                {
                    TailBezier.CPoint1Move(tailPosition - new Vector2(length, 0).RotatedBy(owner.velocity.ToRotation()), maxSpeed * 1.8f);
                    TailBezier.CPoint2Move(tailTip - new Vector2(length, 0).RotatedBy(Projectile.rotation), maxSpeed * 1.8f);
                }
                else
                {
                    TailBezier.CPoint1Move(tailPosition - new Vector2(8 * owner.direction, -length), maxSpeed / 2);
                    TailBezier.CPoint2Move(tailTip - new Vector2(length, 0).RotatedBy(Projectile.rotation), maxSpeed / 2);
                }
            }
            else
            {
                int dir = tailTip.Y - tailPosition.Y < 0 ? 1 : -1;
                if (length > 60)
                    length = 60;

                //TailBezier.CPoint1Move(tailPosition - new Vector2(8 * owner.direction, dir * length), 2f + owner.velocity.Length() / 2);
                TailBezier.CPoint2Move(tailTip - new Vector2(length, 0).RotatedBy(Projectile.rotation), 12f);
                TailBezier.CPoint1Move(tailPosition - new Vector2(length * 0.75f, 0).RotatedBy(Projectile.rotation + 0.758f * dir), 12f);
            }
        }

		private void TipStretchStart()
		{
			if (Projectile.localAI[0] < 0)
				Projectile.localAI[0] = 14f;
		}

		private float TipScale()
		{
			Projectile.localAI[0]--;
			if (Projectile.localAI[0] <= 0)
				return 1f;
			if (Projectile.localAI[0] > 10f)
				return 1f + 1.2f * (Projectile.localAI[0] - 10f) / 3f;
			return 1f + (Projectile.localAI[0] / 10f);
		}

        public override bool? CanHitNPC(NPC target)
        {
            int StabFrames = stabFrames;
            int RestFrames = restFrames;
			bool Canhit = (Timer - RestFrames) % StabFrames > 15;

            if (Timer > restFrames && Canhit)
				return base.CanHitNPC(target);
			return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			if (Main.rand.NextBool(5))
			{
				if (target.lifeMax * 0.15f < Projectile.damage)
                    LobotomyGlobalNPC.PleasureApplyPleasure(Projectile, target, Main.player[Projectile.owner], damageDone, 5f);
                else
					LobotomyGlobalNPC.PleasureApplyPleasure(Projectile, target, Main.player[Projectile.owner], damageDone);
                for (int i = 0; i < 8; i++)
                {
                    Vector2 velocity = new Vector2(4, 0).RotatedBy(6.28f * i / 8f);
                    Gore.NewGore(Projectile.GetSource_FromThis(), target.Center, velocity, 331, Main.rand.NextFloat(0.9f, 1.1f));
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
			Vector2 origin = tex.Size() / 2;
			origin.Y = tex.Height - 4;
			Vector2 scale = new Vector2(1f, TipScale()) * Projectile.scale;

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, tex.Frame(), lightColor, Projectile.rotation + 1.57f, origin, scale, 0);

            CustomShaderData shader = LobotomyCorp.LobcorpShaders["SwingTrail"].UseImage1(Mod, "Projectiles/Realized/PleasureTail");
			shader.UseImage2(Mod, "Projectiles/Realized/PleasureTailA");
			shader.UseOpacity(1f);
            SlashTrail slashTrail = new SlashTrail(7, 0);
			slashTrail.color = lightColor;
			Vector2[] positions = TailBezier.GetCurveVectors(30);
			float[] rotations = TailBezier.GetAngleVectors(30);
			slashTrail.DrawSpecific(positions, rotations, Vector2.Zero, shader);
            return false;
        }
    }
}
