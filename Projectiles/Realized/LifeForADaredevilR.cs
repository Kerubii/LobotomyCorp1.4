using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System.Collections.Generic;
using LobotomyCorp.Players;
using ReLogic.Content;

namespace LobotomyCorp.Projectiles.Realized
{
	public class LifeForADaredevilR : ModProjectile
	{
		public static Asset<Texture2D> Sparkle;
        public static Asset<Texture2D> Sheath;

        public override void Load()
        {
			Sparkle = ModContent.Request<Texture2D>(Texture+"Sparkle");
			Sheath = ModContent.Request<Texture2D>(Texture + "Sheath");
        }

        public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;

			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = true;
			Projectile.friendly = true;
			Projectile.hide = true;
		}

		public override void AI()
		{
			Player player;
			LobotomyHePlayer modPlayer;
			if (Projectile.owner > -1 && !Main.player[Projectile.owner].dead)
			{
				player = Main.player[Projectile.owner];
				modPlayer = player.GetModPlayer<LobotomyHePlayer>();
			}
			else
			{
				Projectile.Kill();
				return;
			}

			if (Projectile.localAI[0] > 0) //Sparkle FX
            {
				Projectile.localAI[0]--;
            }

			if (player.channel)//Player Stance
			{
				Projectile.timeLeft = player.itemAnimationMax;
				player.itemAnimation = player.itemAnimationMax + 1;
				player.itemTime = player.itemAnimation;

				if (Projectile.ai[0] > 10)//Counter Ready
				{
					modPlayer.LifeForADareDevilCounterStance = true;
				}
				else
				{
					Projectile.ai[0]++;
					if (Projectile.ai[0] == 10)
					{
						Projectile.localAI[0] = sparkleFX; //Sparkle FX Start
						SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Armor_Down"), player.position);
					}
				}
				return;
			}
			else
			{
				Projectile.ai[0]--;
				if (Projectile.ai[0] <= 2)//Counter Time
				{
					modPlayer.LifeForADareDevilCounterStance = false;
				}
            }

			if (!modPlayer.LifeForADareDevilGiftActive)
            {
				if (player.itemAnimation == player.itemAnimationMax / 2 + 1)
                {
					player.itemAnimation = 0;
					player.itemTime = 0;
					Projectile.Kill();
                }
            }

			Projectile.Center = player.Center;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? CanDamage()
        {
			return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
			overPlayers.Add(index);

            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Player player = Main.player[Projectile.owner];

			Texture2D sword = TextureAssets.Projectile[Projectile.type].Value;
			Texture2D sheath = Sheath.Value;

			Vector2 pos = player.Center - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
			Rectangle Frame = sword.Frame();
			Vector2 originPoint = new Vector2( 0, Frame.Height);
			float Rotation = 0;
			SpriteEffects flip = 0;
			int dir = player.direction;

			int animationHalf = player.itemAnimationMax / 2;
			if (dir == -1)
            {
				Rotation = MathHelper.ToRadians(180);
				originPoint.X = Frame.Width;
				flip = SpriteEffects.FlipHorizontally;
            }

			if (player.channel)
            {
				Rotation += MathHelper.ToRadians(90) * dir + rotOffset(player.direction);
				Vector2 sheathPos = pos;
				sheathPos.Y -= 22;
				sheathPos.X += 10 * dir;

				if (Projectile.ai[0] > 4)
                {
					Vector2 swordPos = sheathPos;
					swordPos.Y -= 1.4f * (Projectile.ai[0] - 4);

					Main.EntitySpriteDraw(sword, swordPos, Frame, lightColor, Rotation, originPoint, 1f, flip, 0);
				}
				Main.EntitySpriteDraw(sheath, sheathPos, Frame, lightColor, Rotation, originPoint, 1f, flip, 0);
			}
			else
            {
				Vector2 swordPos = pos;
				swordPos.X += 10f * dir;
				swordPos.Y -= 2;
				float sheathRot = rotOffset(dir) + MathHelper.ToRadians(150) * dir;
				if (dir == -1)
					sheathRot += 3.14f;
				Main.EntitySpriteDraw(sheath, swordPos, Frame, lightColor, sheathRot, originPoint, 1f, flip, 0);

				if (player.itemAnimation < animationHalf)
				{
					float progress = 1f - player.itemAnimation / (player.itemAnimationMax / 2f);
					Rotation += MathHelper.ToRadians(120 - 240 * ((float)Math.Pow(0.9f, 60 * progress))) * dir + rotOffset(dir);

					//float progress = player.itemAnimation / (player.itemAnimationMax / 2f);

					//Rotation += MathHelper.ToRadians(120 - 240 * (float)Math.Sin(progress * 1.57f)) + rotOffset(player.direction);
				}
				else
                {
					dir *= -1;

					if (dir == -1)
					{
						Rotation = MathHelper.ToRadians(180);
						originPoint.X = Frame.Width;
						flip = SpriteEffects.FlipHorizontally;
					}
					else
                    {
						Rotation = 0;
						originPoint.X = 0;
						flip = 0;
					}

					float progress = 1f - ((player.itemAnimation / (player.itemAnimationMax / 2f)) - 1f);
					//int prog = player.itemAnimationMax - player.itemAnimation - 1;
					Rotation += MathHelper.ToRadians(180 - 30 + 150 * (float)Math.Sin(progress * 1.57f)) * dir + rotOffset(dir);
				}

				swordPos = pos;
				Main.EntitySpriteDraw(sword, swordPos, Frame, lightColor, Rotation, originPoint, 1f, flip, 0);

			}

			if (Projectile.localAI[0] > 0)
            {
				Texture2D shine = Sparkle.Value;

				pos.X += 10 * player.direction;
				Frame = shine.Frame();
				originPoint = Frame.Size() / 2;

				float scale = 2f - 1.5f * (Projectile.localAI[0] / sparkleFX);
				Color color = Color.White * (Projectile.localAI[0] / sparkleFX);

				Main.EntitySpriteDraw(shine, pos, Frame, color, 0f, originPoint, scale, 0, 0);
			}
			return false;//base.PreDraw(ref lightColor);
		}

		private const float sparkleFX = 30;

		private float rotOffset(int direction)
		{
			if (direction == 1)
				return 1.0472f;
			return 1.0472f * 2;
		}
	}

	public class LifeForADaredevilRAlt : ModProjectile
	{
        public override string Texture => "LobotomyCorp/Projectiles/Realized/LifeForADaredevilR";

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            //Projectile.hide = true;
        }
        
		protected virtual float HoldoutRangeMin => 24f;
        protected virtual float HoldoutRangeMax => 76f;

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
            int duration = player.itemAnimationMax/2; // Define the duration the projectile will exist in frames

            player.heldProj = Projectile.whoAmI; // Update the player's held projectile id

            Projectile.velocity = Vector2.Normalize(Projectile.velocity); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.

            float halfDuration = duration / 2f;
            float progress;

			if (Projectile.localAI[0] == 0)
			{
				SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Armor_Cut") with { Pitch = Main.rand.NextFloat(-0.5f, 0.5f) }, player.position);
                Projectile.localAI[0]++;
            }

            if (Projectile.ai[0] > 0)
			{
				float rotation = 120f;
				if (Projectile.ai[0] == 1)
					rotation -= 240 * (float)Math.Sin(1.57f * (Projectile.timeLeft / halfDuration));
				rotation *= player.direction;

				Vector2 newVel = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
                Projectile.Center = player.MountedCenter + newVel * HoldoutRangeMax;
                Projectile.rotation = newVel.ToRotation();
				if (Projectile.ai[0] == 1 && Projectile.timeLeft == 1)
				{
					Projectile.timeLeft += 20;
					Projectile.ai[0]++;
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Armor_HeadOff") with { Pitch = Main.rand.NextFloat(-0.05f, 0.05f) }, player.position);
                }
                return false;
			}

            // Reset projectile time left if necessary
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }

            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            if (Projectile.timeLeft < halfDuration)
            {
				progress = 1f;
            }
            else
            {
                progress = (float)Math.Sin(1.57f * (duration - Projectile.timeLeft) / halfDuration);
            }

			if (Projectile.timeLeft == (int)(halfDuration + halfDuration / 2f))
			{
				float rot = Projectile.rotation + 6.28f;
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center + new Vector2(80,0).RotatedBy(Projectile.rotation), Vector2.Zero, ModContent.ProjectileType<LifeForADareDevilEffects>(), 0, 0, Projectile.owner, -rot, 100f);

            }

            // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);

			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.spriteDirection = player.direction;

            return false; // Don't execute vanilla AI.
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {

			if (Projectile.ai[0] < 2)
			{
                Player player = Main.player[Projectile.owner];
                Vector2 endpoint = player.MountedCenter + new Vector2(270, 0).RotatedBy(Projectile.rotation);
				float x = 0;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.MountedCenter, endpoint, 16, ref x);
            }

            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle Frame = tex.Frame();
            Texture2D sheath = ModContent.Request<Texture2D>("LobotomyCorp/Projectiles/Realized/LifeForADaredevilRSheath").Value;

            Vector2 origin = new Vector2(tex.Width, 0);
			float rotation = Projectile.rotation + 1.14416883f;
			if (Projectile.spriteDirection == -1)
			{
				origin.X = 0;
				rotation += 0.785398f;
			}

            int dir = Main.player[Projectile.owner].direction;
            Vector2 pos = Main.player[Projectile.owner].Center - Main.screenPosition + Projectile.gfxOffY * Vector2.UnitY;
            Vector2 originPoint = new Vector2(0, Frame.Height);
            pos.X += 10f * dir;
            pos.Y -= 2;
            float sheathRot = rotOffset(dir) + MathHelper.ToRadians(150) * dir;
			SpriteEffects flip = 0;
			if (dir == -1)
			{
                originPoint.X = Frame.Width;
                flip = SpriteEffects.FlipHorizontally;
                sheathRot += 3.14f;
			}
            Main.EntitySpriteDraw(sheath, pos, Frame, lightColor, sheathRot, originPoint, 1f, flip, 0);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, Frame, lightColor, rotation, origin, Projectile.scale, Projectile.spriteDirection >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            return false;
        }

        private float rotOffset(int direction)
        {
            if (direction == 1)
                return 1.0472f;
            return 1.0472f * 2;
        }

        public override bool? CanHitNPC(NPC target)
        {
			Player player = Main.player[Projectile.owner];
            int duration = player.itemAnimationMax / 4;

			if (Projectile.timeLeft == duration + duration / 2)
                return base.CanHitNPC(target);
			return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			int gift = Main.player[Projectile.owner].GetModPlayer<LobotomyHePlayer>().LifeForADareDevilGift;
			if (gift > 600)
			{
				int slashes = 1;
				if (gift > 1300)
					slashes += 1;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.position, Vector2.Zero, ModContent.ProjectileType<Projectiles.Realized.LifeForADareDevilEffectsSlashes>(), Projectile.damage, 0, Projectile.owner, 20, slashes, target.whoAmI);
            }
			if (Projectile.ai[0] < 1 && gift > 600)
				Projectile.ai[0] = 1;
        }
    }
}
