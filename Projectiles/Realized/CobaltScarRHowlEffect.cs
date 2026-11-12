using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.Realized
{
	public class CobaltScarRHowlEffect : ModProjectile
	{

        public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;

			Projectile.timeLeft = 20;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

        public override void AI()
        {
			if (Projectile.ai[0] == 0)
            {
				Projectile.rotation = MathHelper.ToRadians(Main.rand.Next(360));
			}
			Projectile.rotation += MathHelper.ToRadians(4);
			Projectile.ai[0]++;
			Projectile.alpha += 225 / 15;

			if (!Main.player[Projectile.owner].dead)
            {
				Projectile.Center = Main.player[Projectile.owner].MountedCenter;

				if (Main.myPlayer == Projectile.owner)
					LobotomyCorp.ScreenShake(4, 3, 0.45f);
			}
		}

        public override bool PreDraw(ref Color lightColor)
        {
			lightColor = Color.White * (1f - Projectile.alpha / 255f);
			Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle frame = tex.Frame();
			Vector2 position = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
			Vector2 origin = frame.Size() / 2;
			for (int i = 0; i < 3; i++)
            {
				float scale = Projectile.ai[1] * (float)Math.Sin(Projectile.ai[0] / 20f * (0.5f + 0.5f * (1f - i / 3f)) * 1.57f);
				float rot = Projectile.rotation;
				if (i % 2 == 0)
					rot += 3.14f;
				Main.EntitySpriteDraw(tex, position, frame, lightColor, rot, origin, scale, 0, 0);
            }

			return false;
        }

        //public override bool? CanHitNPC(NPC target)
        //{
            //LobotomyGlobalNPC Lnpc = LobotomyGlobalNPC.LNPC(target);
			//if (Lnpc.CobaltScarScarsAmount || Lnpc.CobaltScarScars == 0)
			//	return false;
            //return base.CanHitNPC(target); //Will not hit targets without Scars.
        //}

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			int Scars = LobotomyGlobalNPC.LNPC(target).CobaltScarScarsAmount;

			modifiers.FinalDamage += Scars; 
			//Fuck it this thing does like +65 damage per scar and hits 4 times. It's a lot of damage already.



		}


        public override void ModifyDamageHitbox(ref Rectangle hitbox) //it's not hitting anything at the moment.
		{
			float radius = Projectile.ai[1] * (float)Math.Sin(Projectile.ai[0] / 20f * (0.5f + 0.5f * (1f / 3f)) * 1.57f) * 128f;

			Vector2 origin = Projectile.Center - new Vector2(radius, radius);
			int rr = (int)(radius * 2);
			hitbox = new Rectangle((int)origin.X, (int)origin.Y, rr, rr);
		}
	}
}
